using FolioRaytrace.RayMath;
using FolioRaytrace.RayMath.SDF;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace FolioRaytrace
{
    internal sealed class Utility
    {
        public static string To255Color(Vector3 v)
        {
            const double k_MUL = 255.999;
            var mv = k_MUL * v;
            return $"{(int)mv.X} {(int)mv.Y} {(int)mv.Z}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // カメラの設定
            var camera = new Camera.Camera();
            camera.ImageWidth = 1280;
            camera.ImageHeight = 720;
            camera.Transform.Position = Vector3.s_Zero;
            camera.Transform.Rotation = new Rotation(0, 0, 0, EAngleUnit.Degrees);
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
            var viewportUpperLeft = camera.Transform.Position + camera.Transform.RotationQuat.Rotate(localViewportUL);

            var camPixelDeltaU = camera.Transform.RotationQuat.Rotate(pixelDeltaU);
            var camPixelDeltaV = camera.Transform.RotationQuat.Rotate(pixelDeltaV);
            var pixelDeltaUV = 0.5 * (camPixelDeltaU + camPixelDeltaV);
            var pixelUpperLeft = viewportUpperLeft + pixelDeltaUV;

            // AA（4個サンプリング）のためのオフセットも用意しておく
            // １つ目はUで、2つ目はVで展開する。
            var pixelAddOffsets = new List<(Vector3, Vector3)>();
            pixelAddOffsets.Add((camPixelDeltaU * 0.25, camPixelDeltaV * 0.25));
            pixelAddOffsets.Add((camPixelDeltaU * -0.25, camPixelDeltaV * 0.25));
            pixelAddOffsets.Add((camPixelDeltaU * -0.25, camPixelDeltaV * -0.25));
            pixelAddOffsets.Add((camPixelDeltaU * 0.25, camPixelDeltaV * -0.25));

            // Print image width and height
            // 0から255までの値をだけを持つ。CastingするとFloorされるため。
            Console.WriteLine($"P3\n{camera.ImageWidth} {camera.ImageHeight}\n255");

            var world = new World.World();
            world.AddObject(new FolioRaytrace.RayMath.SDF.ShapeSphere(new Vector3(0, 0, 2), 1));
            world.AddObject(new FolioRaytrace.RayMath.SDF.ShapeSphere(new Vector3(0, -51, 2), 50));

            for (int y = 0; y < camera.ImageHeight; ++y)
            {
                System.Diagnostics.Debug.WriteLine($"Scanlines remaining: {camera.ImageHeight - y}");

                for (int x = 0; x < camera.ImageWidth; ++x)
                {
                    // Rayを作って、飛ばす。
                    var pixelOffsetCenterUV = (x * camPixelDeltaU) + (y * camPixelDeltaV);

                    // [0, 1]になる
                    Vector3 color = Vector3.s_Zero;
                    foreach (var (addU, addV) in pixelAddOffsets)
                    {
                        var targetPixel = pixelUpperLeft + pixelOffsetCenterUV + addU + addV;
                        var targetRay = new Ray(camera.Transform.Position, targetPixel - camera.Transform.Position);

                        Vector3 targetColor;

                        var setting = new World.World.RenderSetting();
                        setting.Ray = targetRay;
                        setting.CycleLimitCount = 25;

                        world.Render(out targetColor, setting);
                        color += targetColor;
                    }

                    color /= pixelAddOffsets.Count;

                    // Gamma補正も行う。現在のcolorはLienarなので…
                    color.X = Math.Sqrt(color.X);
                    color.Y = Math.Sqrt(color.Y);
                    color.Z = Math.Sqrt(color.Z);

                    Console.WriteLine($"{Utility.To255Color(color)}");
                }
            }

            System.Diagnostics.Debug.WriteLine("Done.");
        }
    }
}
