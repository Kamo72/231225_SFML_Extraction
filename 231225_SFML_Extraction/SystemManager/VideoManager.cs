using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Threading;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;

namespace _231109_SFML_Test
{
    internal static class VideoManager
    {
        static VideoManager() 
        {
            resolutionNow = resolutionPreset[7];
            drawfpsNow = drawfpsPreset[3];          //144

            clockTotal = new Clock();
            clockDelta = new Clock();
        }

        static Clock clockTotal, clockDelta;
        static int frameTotal, frameDelta;

        //근 1초 사이의 프레임(1초마다 최신화)
        public static int fpsNow;

        //Program.window에 접근
        public static RenderWindow window { set { Program.window = value;  } get { return Program.window; } }

        //시간 관련된 값을 제공하는 함수
        public static float GetTimeTotal()
        {
            return clockTotal.ElapsedTime.AsMicroseconds() / 1000000f;
        }
        public static float GetTimeDelta()
        {
            return clockDelta.ElapsedTime.AsMicroseconds() / 1000000f;
        }
        public static bool FrameResetable()
        {
            float deltaTime = clockDelta.ElapsedTime.AsMicroseconds() / 1000f;

            return deltaTime > 1000f / drawfpsNow;
        }
        
        //드로우 프레임이 너무 일찍 끝났다면, 남는 시간 동안 지연시킨다. + 프레임 계산 처리
        public static void FrameReset()
        {
            while (FrameResetable() == false) 
            {
                float remainTime = 1f / drawfpsNow - GetTimeDelta();
                Thread.Sleep((int)(remainTime * 0.9f));
            }

            //전 프레임 지연 초 수
            int pastTime = (int)Math.Floor(GetTimeTotal() - GetTimeDelta());
            int nowTime = (int)Math.Floor(GetTimeTotal());
            
            if (pastTime != nowTime) 
            {
                fpsNow = frameDelta;
                frameDelta = 0;
            }

            frameTotal++;
            frameDelta++;

            clockDelta.Restart();
        }

        //목표 프레임과 해상도에 맞게 윈도우를 새로 만든다. 프로그램 실행 중 언제든 변경 가능.
        public static void SetWindow() 
        {
            SetWindow(resolutionNow, drawfpsNow);
        }
        public static void SetWindow(Vector2i resolution, uint drawfps)
        {
            window?.Dispose();

            lock (window ?? new object())
            {
                resolutionNow = resolution;
                drawfpsNow = drawfps;

                ContextSettings settings = new ContextSettings();
                settings.AntialiasingLevel = 8; // 원하는 안티얼라이어싱 레벨을 설정합니다.

                VideoMode videoMode = new VideoMode((uint)resolutionNow.X, (uint)resolutionNow.Y);
                window = new RenderWindow(videoMode, "TitleName", Styles.Fullscreen, settings);
                window.SetFramerateLimit(drawfpsNow);
                window.SetVerticalSyncEnabled(true); // 수직 동기화를 활성화하여 안티얼라이어싱을 적용할 수 있습니다.
            }

            ChangedResolution?.Invoke(resolutionNow);
        }

        //해상도 변경 이벤트 << SetWindow
        public static event Action<Vector2i> ChangedResolution;


        //현재 해상도 및 해상도 프리셋
        public static Vector2i resolutionNow;
        public static Vector2i[] resolutionPreset = new Vector2i[]
        {
            new Vector2i (1024, 768),   //00 XGA 
            new Vector2i (1280, 720),   //01 HD ☆
            new Vector2i (1280, 768),   //02 WXGA 
            new Vector2i (1280, 800),   //03 WXGA 
            new Vector2i (1920, 1080),  //04 FHD ☆ 
            new Vector2i (2048, 1080),  //05 2K 
            new Vector2i (2560, 1080),  //06 UWHD
            new Vector2i (2560, 1440),  //07 WQHD ☆
            new Vector2i (3440, 1440),  //08 UWQHD
            new Vector2i (3840, 2160),  //09 UHD
            new Vector2i (4096, 2160),  //10 4K ☆
        };

        //현재 목표프레임 및 목표프레임 프리셋
        public static uint drawfpsNow;
        public static uint[] drawfpsPreset = new uint[]
        {
            (uint)60,
            (uint)90,
            (uint)120,
            (uint)144,
            (uint)165,
            (uint)240,
        };




    }
}
