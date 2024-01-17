using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal static class Mathf
    {
        public static float ToRadian(this float dir)
        {
            return (float)(dir / 180f * Math.PI);
        }
        public static float ToDirection(this float rad)
        {
            return (float)(rad * 180f / Math.PI);
        }

        internal static float Clamp(float min, float value, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        internal static float PercentMultiflex(float percent, float multiflex) 
        {
            float sep = percent - 1f;
            return 1f + sep * multiflex;
        }
    }
}
