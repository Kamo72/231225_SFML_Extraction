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

    }
}
