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
        CreateSampleOffsets(Vector3 deltaU, Vector3 deltaV, uint lv)
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
    }

    internal class WorkItem
    {
        public WorkItem(
            World.World world,
            int bufferI, 
            Vector3 pixelCenter, 
            PixelAddOffsetList addOffsets, 
            Vector3 cameraPos) {

            World = world;
            BufferI = bufferI;
            PixelAddOffsets = addOffsets;
            PixelCenter = pixelCenter;
            CameraPos = cameraPos;
        }

        public World.World World;
        public readonly int BufferI = 0;
        public readonly Vector3 PixelCenter;
        public readonly Vector3 CameraPos;
        public readonly PixelAddOffsetList PixelAddOffsets;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // カメラの設定
            var camera = new Camera.Camera();
            camera.ImageWidth = 720;
            camera.ImageHeight = 480;
            //camera.Transform.Position = Vector3.s_Zero;
            //camera.Transform.Rotation = new Rotation(0, 0, 0, EAngleUnit.Degrees);
            camera.Transform = Transform.FromLookAt(Vector3.s_Zero, Vector3.s_UnitZ * 2);
            camera.ViewportHeight = 2.0;

            // Uは右、Vは下に進む。
            var viewportU = Vector3.s_UnitX * camera.ViewportWidth;
            var viewportV = Vector3.s_UnitY * camera.ViewportHeight * -1;
            var pixelDeltaU = viewportU / (double)camera.ImageWidth;
            var pixelDeltaV = viewportV / (double)camera.ImageHeight;

            // UL := UpperLeft
            // カメラのviewportの左上(Local座標)を記録する。
            // ただしGrid上ではPixelが定数のままだと1個分が足りないので、0.5個分ずらす。
            var localViewportUL = (Vector3.s_UnitZ * camera.FocalLength) - (0.5 * (viewportU + viewportV));
            var viewportUpperLeft = camera.Transform.Position;
            viewportUpperLeft += camera.Transform.RotationQuat.Rotate(localViewportUL);

            var camPixelDeltaU = camera.Transform.RotationQuat.Rotate(pixelDeltaU);
            var camPixelDeltaV = camera.Transform.RotationQuat.Rotate(pixelDeltaV);
            var pixelDeltaUV = 0.5 * (camPixelDeltaU + camPixelDeltaV);
            var pixelUpperLeft = viewportUpperLeft + pixelDeltaUV;

            // AA（4個サンプリング）のためのオフセットも用意しておく
            // １つ目はUで、2つ目はVで展開する。
            var pixelAddOffsets = Utility.CreateSampleOffsets(camPixelDeltaU, camPixelDeltaV, 10);
            // 0から255までの値をだけを持つ。CastingするとFloorされるため。
            Console.WriteLine($"P3\n{camera.ImageWidth} {camera.ImageHeight}\n255");

            var world = new World.World();
            {
                var mat = new Material.BasicDiffuse();
                mat.Albedo = Utility.IntColor3ToVector3(235, 64, 52);
                mat.AttenuationColor = Vector3.s_One * 0.75;
                mat.Roughness = 1.0;
                world.AddObject(new ShapeSphere(new Vector3(0, 0, 2), 1), mat);
            }
            {
                var mat = new Material.BasicDielectric();
                mat.Albedo = new Vector3(1.0, 1.0, 1.0);
                mat.AttenuationColor = Vector3.s_One * 0.9;
                mat.RefractiveIndex = 1.5;
                world.AddObject(new ShapeSphere(new Vector3(-2, 0, 2), 1), mat);
            }
            {
                var mat = new Material.BasicDiffuse();
                mat.Albedo = Utility.IntColor3ToVector3(235, 195, 52);
                mat.AttenuationColor = Vector3.s_One * 0.9;
                mat.Roughness = 0.0;
                world.AddObject(new ShapeSphere(new Vector3(2, 0, 2), 1), mat);
            }
            {
                var mat = new Material.BasicDiffuse();
                mat.Albedo = Utility.IntColor3ToVector3(148, 191, 48);
                mat.AttenuationColor = Vector3.s_One * 0.9;
                mat.Roughness = 1.0;
                world.AddObject(new ShapeSphere(new Vector3(0, -51, 2), 50), mat);
            }

            var renderBuffer = new Vector3[camera.ImagePixels];
            var workItems = new List<WorkItem>();
            for (int y = 0; y < camera.ImageHeight; ++y)
            {
                for (int x = 0; x < camera.ImageWidth; ++x)
                {
                    var pixelCenter = pixelUpperLeft + (x * camPixelDeltaU) + (y * camPixelDeltaV); ;
                    var bufferI = (camera.ImageWidth * y) + x;
                    var cameraPos = camera.Transform.Position;

                    var workItem = new WorkItem(
                        world, 
                        bufferI, 
                        pixelCenter, 
                        pixelAddOffsets, 
                        cameraPos);
                    workItems.Add(workItem);
                }
            }

            // CLRに並列処理を全部お任せしよ。
            // https://stackoverflow.com/questions/14039051/parallel-foreach-keeps-spawning-new-threads
            Parallel.ForEach(workItems, delegate (WorkItem newItem)
            {
                // [0, 1]になる
                Vector3 color = Vector3.s_Zero;
                foreach (var (addU, addV) in pixelAddOffsets)
                {
                    // Rayを作って、飛ばす。
                    var targetPixel = newItem.PixelCenter + addU + addV;
                    var targetRay = new Ray(newItem.CameraPos, targetPixel - newItem.CameraPos);

                    var setting = new World.World.RenderSetting();
                    setting.Ray = targetRay;
                    setting.CycleLimitCount = 50;

                    Vector3 targetColor;
                    newItem.World.Render(out targetColor, setting);
                    color += targetColor;
                }
                color /= pixelAddOffsets.Count;

                // Gamma補正も行う。現在のcolorはLienarなので…
                color.X = Math.Sqrt(color.X);
                color.Y = Math.Sqrt(color.Y);
                color.Z = Math.Sqrt(color.Z);
                renderBuffer[newItem.BufferI] = color;
            });

            // バッファー出力 (処理ネック)
            foreach (var color in renderBuffer)
            {
                Console.WriteLine($"{Utility.To255Color(color)}");
            }

        }
    }
}
