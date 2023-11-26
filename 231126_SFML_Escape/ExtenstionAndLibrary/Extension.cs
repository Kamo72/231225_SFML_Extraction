using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    public static class SFMLExtensions
    {
        // Vector2f를 방향으로 변환하는 Extension 메서드
        public static float ToDirection(this Vector2f vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        // Vector2f의 크기를 계산하는 Extension 메서드
        public static float Magnitude(this Vector2f vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }

        // Vector2f를 정규화하는 Extension 메서드
        public static Vector2f Normalize(this Vector2f vector)
        {
            float magnitude = vector.Magnitude();
            if (magnitude > 0)
                return new Vector2f(vector.X / magnitude, vector.Y / magnitude);
            else
                return vector;
        }

        // float를 벡터로 변환하는 Extension 메서드
        public static Vector2f ToVector(this float direction)
        {
            return new Vector2f((float)Math.Cos(direction), (float)Math.Sin(direction));
        }
    }
}
