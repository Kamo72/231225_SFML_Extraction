using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal static class Mathf
    {
        public static float ToRadian(this float dir) => (float)(dir / 180f * Math.PI);
        
        public static float ToDirection(this float rad) => (float)(rad * 180f / Math.PI);
        
        internal static float Clamp(float min, float value, float max) => Math.Min(Math.Max(value, min), max);
        
        internal static bool InRange(float min, float value, float max) => min <= value && value <= max;

        internal static float PercentMultiflex(float percent, float multiflex) => 1f + (percent - 1f) * multiflex;
        
    }
}
