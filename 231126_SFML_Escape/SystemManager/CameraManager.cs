using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

namespace _231109_SFML_Test
{
    internal static class CameraManager
    {
        static CameraManager()
        {
            VideoManager.ChangedResolution += ChangedResolution;
            ChangedResolution(VideoManager.resolutionNow);
            shakeRand = new Random();
        }

        public static Vector2f position = new Vector2f(0f, 0f);
        public static float rotation = 0f;
        public static Vector2f size;

        public static float zoomValue = 1f;

        static Vector2f positionShake = new Vector2f(0f, 0f);
        static float rotationShake = 0f;
        static float sizeShake = 1f;

        static Random shakeRand;
        static double shakeValue = 0d;
        const double shakeReduction = 5d;

        static void ChangedResolution(Vector2i resolution)
        {
            size = (Vector2f)resolution;
        }

        public static RenderStates worldRenderState = RenderStates.Default;

        public static void RefreshTransform()
        {
            //그리려는 대상의 위상을 가져옴
            worldRenderState.Transform = Transform.Identity;

            //적용할 기초 값들 구함.
            float resolutionRatio = size.X / VideoManager.resolutionNow.X;

            Vector2f pos = position + positionShake;
            float rot = rotation + rotationShake;
            float siz = resolutionRatio * sizeShake * zoomValue;

            Vector2f tPos = (Vector2f)Mouse.GetPosition(Program.window);

            //기초값을 토대로 적용
            worldRenderState.Transform.Translate(-pos + (size / 2f));
            worldRenderState.Transform.Scale(new Vector2f(1/siz, 1/siz), pos);
            worldRenderState.Transform.Rotate(rot, pos);
        }
        
        public static bool IsSkippable(Vector2f position, float scale = 0f) 
        {
            Vector2f distVec = position - CameraManager.position;
            double distScala = Math.Pow(distVec.X * distVec.X + distVec.Y * distVec.Y, 0.5d);
            double sizeScala = Math.Pow(size.X * size.X + size.Y * size.Y, 0.5d) / 2;

            if (distScala - scale < sizeScala)
                return false;
            else
                return true;
        }

        public static void ShakeProcess() 
        {
            double reduction = shakeReduction * 1f / (VideoManager.fpsNow + 1);
            shakeValue *= (1f - reduction);
            Console.WriteLine($"{shakeValue} >>> {reduction}");

            if (shakeValue < 0) shakeValue = 0;

            float posDir = (float)shakeRand.NextDouble() * 360f;
            float posLen = (float)shakeRand.NextDouble() * (float)shakeValue / 2f;
            float rot = (float)(shakeRand.NextDouble() -0.5d) * (float)shakeValue / 15f;
            float size = (float)(shakeRand.NextDouble() - 0.5d) * (float)shakeValue / 700f + 1f;

            positionShake = new Vector2f(
                posLen * (float)Math.Cos(posDir / 180f * Math.PI),
                posLen * (float)Math.Sin(posDir / 180f * Math.PI)
                );
            rotationShake = rot;
            sizeShake = size;
        }

        public static void GetShake(float shakeValue)
        {
            Console.WriteLine($"GetShake >>> {CameraManager.shakeValue}+{shakeValue}");
            CameraManager.shakeValue += shakeValue;
        }
    }
}
