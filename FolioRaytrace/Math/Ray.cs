using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolioRaytrace.RayMath
{
    public struct Ray
    {
        public Ray() { }
        public Ray(Vector3 orig, Vector3 dir)
        {
            _orig = orig;
            _dir = dir.Unit();
        }

        public Vector3 Orig
        {
            get => _orig;
            set => _orig = value;
        }
        public Vector3 Direction => _dir;

        public Vector3 Proceed(double t)
        {
            return Orig + (t * Direction);
        }

        public Ray ProceedRay(double t) => new Ray(Proceed(t), Direction);

        private Vector3 _orig;
        private Vector3 _dir;

        public override string ToString()
        {
            return $"Orig: {Orig}, Direction: {Direction}";
        }

    }

}
