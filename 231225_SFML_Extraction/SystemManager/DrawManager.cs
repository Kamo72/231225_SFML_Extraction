using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;

namespace _231109_SFML_Test
{
    internal static class DrawManager
    {
        static DrawManager()
        {
            //해상도 변경될 때마다 자동으로 텍스쳐 조정
            VideoManager.ChangedResolution += ResolutionChanged;

            ResolutionChanged(VideoManager.resolutionNow);
        }

        //해상도에 맞게 텍스쳐 초기화
        public static void ResolutionChanged(Vector2i resolution)
        {
            //이전의 텍스쳐 Dispose
            for (int idx = 0; idx < uiTex.Length; idx++)
                uiTex[idx]?.Dispose();
            for (int idx = 0; idx < worldTex.Length; idx++)
                worldTex[idx]?.Dispose();

            resultTex?.Dispose();


            //새로운 텍스쳐 생성
            for (int idx = 0; idx < uiTex.Length; idx++)
                uiTex[idx] = new RenderTexture((uint)resolution.X, (uint)resolution.Y);

            for (int idx = 0; idx < worldTex.Length; idx++)
                worldTex[idx] = new RenderTexture((uint)resolution.X, (uint)resolution.Y);

            resultTex = new RenderTexture((uint)resolution.X, (uint)resolution.Y);
        }

        //입력받을 텍스쳐

        #region [텍스쳐 파라미터]
        public static RenderTexture texUiBackground { get { return uiTex[0]; } }
        public static RenderTexture texUiInterface { get { return uiTex[1]; } }
        public static RenderTexture texUiPopup { get { return uiTex[2]; } }
        public static RenderTexture texUiWhole { get { return uiTex[3]; } }
        public static RenderTexture texWrBackground { get { return worldTex[0]; } }
        public static RenderTexture texWrLower { get { return worldTex[1]; } }
        public static RenderTexture texWrHigher { get { return worldTex[2]; } }
        public static RenderTexture texWrEffect { get { return worldTex[3]; } }
        public static RenderTexture texWrAugment { get { return worldTex[4]; } }
        #endregion

        public static RenderTexture[] uiTex = new RenderTexture[4];     //0 = 화면 효과 1 = UI, 2 = 팝업 3 = 전체 효과
        public static RenderTexture[] worldTex = new RenderTexture[5];  //0 = 배경, 1 = 바닥 1 = 동적 2 = 효과 3 = 증강 효과
        public static RenderTexture resultTex;


        //입력받은 텍스쳐 초기화 및 결과 텍스쳐 반환
        public static void ResultTexture()
        {
            //결과를 담을 텍스쳐 생성
            Vector2f resolution = (Vector2f)VideoManager.resolutionNow;
            resultTex.Clear();

            //레이어들을 결과 텍스쳐에 도합
            for (int idx = 0; idx < worldTex.Length; idx++)
                resultTex.Draw(new Sprite(worldTex[idx].Texture, new IntRect(0, (int)resolution.Y, (int)resolution.X, -(int)resolution.Y)));
            for (int idx = 0; idx < uiTex.Length; idx++)
                resultTex.Draw(new Sprite(uiTex[idx].Texture, new IntRect(0, (int)resolution.Y, (int)resolution.X, -(int)resolution.Y)));


            //레이어들 초기화
            for (int idx = 1; idx < worldTex.Length; idx++)
                worldTex[idx].Clear(new Color(0, 0, 0, 0));

            for (int idx = 0; idx < uiTex.Length; idx++)
                uiTex[idx].Clear(new Color(0, 0, 0, 0));


            //결과 텍스쳐 반환
            Sprite resultSprite = new Sprite(resultTex.Texture, new IntRect(0, (int)resolution.Y, (int)resolution.X, -(int)resolution.Y));
            Program.window.Draw(resultSprite);

            //정리
            resultSprite.Dispose();

        }


    }
}
