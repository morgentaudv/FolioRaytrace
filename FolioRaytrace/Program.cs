using FolioRaytrace.Camera;
using FolioRaytrace.RayMath;
using FolioRaytrace.SDF;
using FolioRaytrace.World;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace FolioRaytrace
{
    using PixelAddOffsetList = List<(Vector3, Vector3)>;

    internal sealed class Utility
    {
        public static string To255Color(Vector3 v)
        {
            const double k_MUL = 255.999;
            var mv = k_MUL * v;
            return $"{(int)mv.X} {(int)mv.Y} {(int)mv.Z}";
        }

        public static Vector3 IntColor3ToVector3(int x, int y, int z)
        {
            var fx = Math.Clamp(x, 0, 255) / 255.0;
            var fy = Math.Clamp(y, 0, 255) / 255.0;
            var fz = Math.Clamp(z, 0, 255) / 255.0;
            return new Vector3(fx, fy, fz);
        }

        public static PixelAddOffsetList
        CreateSampleOffsets(Vector3 deltaU, Vector3 deltaV, int lv)
        {
            var results = new PixelAddOffsetList();
            var clv = Math.Clamp(lv, 1, 20);
            var offset = 1.0 / (clv + 1);

            var offsetU = deltaU * offset;
            var offsetV = deltaV * offset;

            var cursorV = -offsetV;
            for (var y = 1; y <= clv; ++y)
            {
                var cursorU = -deltaU;
                for (var x = 1; x <= clv; ++x)
                {
                    cursorU += offsetU;
                    results.Add((cursorU, cursorV));
                }

                cursorV += offsetV;
            }

            return results;
        }

        /// <summary>
        /// 文字がASCII文字かを確認する。
        /// </summary>
        public static bool IsCharAscii(char c)
        {
            // https://stackoverflow.com/questions/18596245/in-c-how-can-i-detect-if-a-character-is-a-non-ascii-character
            // そもそも基本ライブラリに存在しないのがありえない気がした。
            return c <= 127;
        }
    }

    internal class WorkItem
    {
        public WorkItem(
            World.World world,
            int bufferI, 
            Vector3 pixelCenter, 
            PixelAddOffsetList addOffsets, 
            Vector3 cameraPos,
            World.RenderBuffer renderBuffer) {

            World = world;
            BufferI = bufferI;
            PixelAddOffsets = addOffsets;
            PixelCenter = pixelCenter;
            CameraPos = cameraPos;
            RenderBuffer = renderBuffer;
        }

        public void Execute()
        {
            // [0, 1]になる
            Vector3 color = Vector3.s_Zero;
            foreach (var (addU, addV) in PixelAddOffsets)
            {
                // Rayを作って、飛ばす。
                var targetPixel = PixelCenter + addU + addV;
                var targetRay = new Ray(CameraPos, targetPixel - CameraPos);

                var setting = new World.World.RenderSetting();
                setting.Ray = targetRay;
                setting.CycleLimitCount = 50;

                Vector3 targetColor;
                World.Render(out targetColor, setting);
                color += targetColor;
            }
            color /= PixelAddOffsets.Count;

            // Gamma補正も行う。現在のcolorはLienarなので…
            color.X = Math.Sqrt(color.X);
            color.Y = Math.Sqrt(color.Y);
            color.Z = Math.Sqrt(color.Z);
            RenderBuffer[BufferI] = color;
        }

        private World.World World;
        private readonly int BufferI = 0;
        private readonly Vector3 PixelCenter;
        private readonly Vector3 CameraPos;
        private readonly PixelAddOffsetList PixelAddOffsets;
        private World.RenderBuffer RenderBuffer;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Command Argumentsの解析
            CLI.ParseResult parseResult;
            if (!CLI.CommandParser.TryParse(out parseResult, args))
            {
                Console.WriteLine("Usage : ./FolioRaytrace.exe -o, --output OutputPath");
                return;
            }

            var lookAt = Vector3.s_UnitZ * 2;
            var lookFrom = Vector3.s_UnitY;

            // カメラの設定
            var camera = new Camera.Camera();
            camera.ImageWidth = parseResult.ImageWidth;
            camera.ImageHeight = parseResult.ImageHeight;
            camera.Transform = Transform.FromLookAt(lookFrom, lookAt);
            camera.FieldOfViewAngleDeg = 100.0;
            camera.FocusDistance = (lookAt - lookFrom).Length * 0.9;
            camera.DefocusAngleDeg = 0.0;

            // Uは右、Vは下に進む。
            var viewportU = Vector3.s_UnitX * camera.ViewportWidth;
            var viewportV = Vector3.s_UnitY * camera.ViewportHeight * -1;
            var pixelDeltaU = viewportU / (double)camera.ImageWidth;
            var pixelDeltaV = viewportV / (double)camera.ImageHeight;

            // UL := UpperLeft
            // カメラのviewportの左上(Local座標)を記録する。
            // ただしGrid上ではPixelが定数のままだと1個分が足りないので、0.5個分ずらす。
            var localViewportUL = (Vector3.s_UnitZ * camera.FocusDistance) - (0.5 * (viewportU + viewportV));
            var viewportUpperLeft = camera.Transform.Position;
            viewportUpperLeft += camera.Transform.RotationQuat.Rotate(localViewportUL);

            var camPixelDeltaU = camera.Transform.RotationQuat.Rotate(pixelDeltaU);
            var camPixelDeltaV = camera.Transform.RotationQuat.Rotate(pixelDeltaV);
            var pixelUpperLeft = viewportUpperLeft + (0.5 * (camPixelDeltaU + camPixelDeltaV));

            // AA（4個サンプリング）のためのオフセットも用意しておく
            // １つ目はUで、2つ目はVで展開する。
            var pixelAddOffsets = Utility.CreateSampleOffsets(
                camPixelDeltaU, 
                camPixelDeltaV, 
                parseResult.SampleLevel);

            World.World? world = null;
            if (parseResult.UseDefaultWorld)
            {
                world = World.World.GetDefaultWorld();
            }
            //var world = new World.World();
            //{
            //    var mat = new Material.BasicDiffuse();
            //    mat.Albedo = Utility.IntColor3ToVector3(235, 64, 52);
            //    mat.AttenuationColor = Vector3.s_One * 0.75;
            //    mat.Roughness = 1.0;
            //    world.AddObject(new ShapeSphere(new Vector3(0, 0, 2), 1), mat);
            //}
            //{
            //    var mat = new Material.BasicDielectric();
            //    mat.Albedo = new Vector3(1.0, 1.0, 1.0);
            //    mat.AttenuationColor = Vector3.s_One * 0.9;
            //    mat.RefractiveIndex = 1.5;
            //    world.AddObject(new ShapeSphere(new Vector3(-2, 0, 2), 1), mat);
            //}
            //{
            //    var mat = new Material.BasicDiffuse();
            //    mat.Albedo = Utility.IntColor3ToVector3(235, 195, 52);
            //    mat.AttenuationColor = Vector3.s_One * 0.9;
            //    mat.Roughness = 0.0;
            //    world.AddObject(new ShapeSphere(new Vector3(2, 0, 2), 1), mat);
            //}
            //{
            //    var mat = new Material.BasicDiffuse();
            //    mat.Albedo = Utility.IntColor3ToVector3(148, 191, 48);
            //    mat.AttenuationColor = Vector3.s_One * 0.9;
            //    mat.Roughness = 1.0;
            //    world.AddObject(new ShapeSphere(new Vector3(0, -51, 2), 50), mat);
            //}

            var renderBuffer = new World.RenderBuffer(camera.ImageWidth, camera.ImageHeight);
            var workItems = new List<WorkItem>();
            var rng = new Random(Environment.TickCount);
            for (int y = 0; y < camera.ImageHeight; ++y)
            {
                for (int x = 0; x < camera.ImageWidth; ++x)
                {
                    var pixelCenter = pixelUpperLeft + (x * camPixelDeltaU) + (y * camPixelDeltaV); ;
                    var bufferI = (camera.ImageWidth * y) + x;

                    // 24-03-10 DoFを実装。
                    var cameraPos = camera.Transform.Position;
                    var defocusVec = Vector3.s_Zero;
                    {
                        var defocusRadius = camera.FocusDistance * Math.Tan(camera.DefocusAngleRad * 0.5);
                        var unitRotRad = rng.NextDouble() * Math.PI;
                        var localX = Math.Cos(unitRotRad) * defocusRadius * rng.NextDouble();
                        var localY = Math.Sin(unitRotRad) * defocusRadius * rng.NextDouble();
                        var localVec = new Vector3(localX, localY, 0);

                        defocusVec = camera.Transform.RotationQuat.Rotate(localVec);
                    }

                    var workItem = new WorkItem(
                        world!, 
                        bufferI, 
                        pixelCenter, 
                        pixelAddOffsets, 
                        cameraPos + defocusVec,
                        renderBuffer);
                    workItems.Add(workItem);
                }
            }

            if (parseResult.UseParallel)
            {
                // CLRに並列処理を全部お任せしよ。
                // https://stackoverflow.com/questions/14039051/parallel-foreach-keeps-spawning-new-threads
                Parallel.ForEach(workItems, delegate (WorkItem newItem)
                { newItem.Execute(); });
            }
            else
            {
                foreach (var workItem in workItems)
                {
                    workItem.Execute();
                }
            }

            if (parseResult.IsDebugMode)
            {
                renderBuffer.WriteDebugText(System.DateTime.Now.ToString(),
                    new Vector3(1, 0, 0),
                    8, 8);

                var origCoord = Coordinates.FromAxisZ(lookAt - lookFrom);
                var checkCoord = Coordinates.FromRotation(camera.Transform.Rotation);
                var log = string.Format("{0}\n{1}\n\n{2}\n{3}\n{4}\n\n{5}\n{6}\n{7}",
                    string.Format($"POS: {camera.Transform.Position}"),
                    string.Format($"ROT: {camera.Transform.Rotation}"),

                    string.Format($"ORX: {origCoord.XAxis}"),
                    string.Format($"ORY: {origCoord.YAxis}"),
                    string.Format($"ORZ: {origCoord.ZAxis}"),

                    string.Format($"CKX: {checkCoord.XAxis}"),
                    string.Format($"CKY: {checkCoord.YAxis}"),
                    string.Format($"CKZ: {checkCoord.ZAxis}")
                    );
                renderBuffer.WriteDebugText(log, new Vector3(0, 0, 0), 8, 24);
            }

            // バッファー出力 (処理ネック)
            // 0から255までの値をだけを持つ。CastingするとFloorされるため。
            var outputPath = parseResult.ExplicitOutputPath;
            if (parseResult.UseDefaultOutputPath)
            {
                // https://hironimo.com/prog/c-sharp/c-date-format/
                outputPath = string.Format($"Default_{System.DateTime.Now.ToString("yyyyMMdd_HHmmss")}.ppm");
            }

            using (var outputFile = new StreamWriter(outputPath))
            {
                outputFile.WriteLine($"P3\n{camera.ImageWidth} {camera.ImageHeight}\n255");
                renderBuffer.ExportPPM(outputFile);
            }
        }
    }
}
