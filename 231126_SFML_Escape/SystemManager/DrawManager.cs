using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            for(int idx = 0; idx < uiTex.Length; idx++)
                uiTex[idx] = new RenderTexture((uint)resolution.X, (uint)resolution.Y);

            for (int idx = 0; idx < worldTex.Length; idx++)
                worldTex[idx] = new RenderTexture((uint)resolution.X, (uint)resolution.Y);

            resultTex = new RenderTexture((uint)resolution.X, (uint)resolution.Y);
        }

        //입력받을 텍스쳐
        public static RenderTexture[] uiTex = new RenderTexture[3];     //0 = 화면 효과 1 = UI 2 = 효과
        public static RenderTexture[] worldTex = new RenderTexture[4];  //0 = 배경 1 = 동적 2 = 효과 3 = 증강 효과
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
            for (int idx = 0; idx < worldTex.Length; idx++)
                worldTex[idx].Clear(new Color(0,0,0,0));

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
