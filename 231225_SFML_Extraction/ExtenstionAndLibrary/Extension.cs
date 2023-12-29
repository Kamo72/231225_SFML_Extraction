using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    public static class Vector2fEx
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

        //Vector2f를 원점을 기준으로 회전하는 Extension 메서드
        public static Vector2f RotateFromZero(this Vector2f vector, float rotation)
        {
            // 라디안으로 변환
            float radians = rotation * (float)(Math.PI / 180.0);

            // 회전 행렬을 사용하여 벡터를 회전
            float x = vector.X * (float)Math.Cos(radians) - vector.Y * (float)Math.Sin(radians);
            float y = vector.X * (float)Math.Sin(radians) + vector.Y * (float)Math.Cos(radians);

            return new Vector2f(x, y);
        }

        // float를 벡터로 변환하는 Extension 메서드
        public static Vector2f ToVector(this float direction)
        {
            return new Vector2f((float)Math.Cos(direction), (float)Math.Sin(direction));
        }

        public static Vector2f Zero { get { return new Vector2f(0f, 0f); } }

    }

    public static class Vector3fEx
    {
        // Magnitude (length) of a Vector3f
        public static float Magnitude(this Vector3f vector)
        {
            return (float)System.Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }
    }
    public class TypeEx 
    {
        public static bool IsChildByParent(Type childClass, Type parentClass)
        {
            while (childClass != null && childClass != typeof(object))
            {
                var currentType = childClass.IsGenericType ? childClass.GetGenericTypeDefinition() : childClass;
                if (parentClass == currentType)
                    return true;

                childClass = childClass.BaseType;
            }
            return false;
        }
    }


}
