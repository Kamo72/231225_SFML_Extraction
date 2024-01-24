using SFML.Graphics;
using SFML.System;
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
 
using SFML.Window;

namespace _231109_SFML_Test
{
    internal class GamemodeLogo : Gamemode
    {
        public GamemodeLogo(TotalManager tm) : base(tm, 60) 
        {
            //TextLabel logo = new TextLabel(this, (Vector2f)Vm.resolutionNow / 2f, (Vector2f)Vm.resolutionNow);
            //logo.TextSet(Rm.fonts["Jalnan"], 100f, Color.Red, "뿌직");

            logo = new UiTest(this, (Vector2f)Vm.resolutionNow / 2f, new Vector2f(600f, 600f));
            logo.mask.Texture = Rm.textures["valve"];
            logicEvent += () =>
            {
                Vector2f perlinNoise = new Vector2f(noise.GetPerlin(Vm.GetTimeTotal() * 8000f, 10f), noise.GetPerlin(Vm.GetTimeTotal() * 8000f, 1235f));
                logo.Position = (Vector2f)Vm.resolutionNow / 2f + perlinNoise * 4f;
            };
        }
        
        Ui logo;
        FastNoise noise = new FastNoise();


        const int logoTimePre = 2000;
        const int logoTimeMax = 10000;
        const int logoTimeEdge = 3000;

        protected override void LogicProcess()
        {
            Time time = clock.ElapsedTime;
            int miliSec = time.AsMilliseconds();

            if (miliSec > logoTimeMax) 
            {
                totalManager.SetGamemodeType(GamemodeType.MAIN_MENU);
            }


            List<Keyboard.Key> keysWhiteList = new List<Keyboard.Key>
                {
                    Keyboard.Key.Escape,
                    Keyboard.Key.F1,
                    Keyboard.Key.F2,
                    Keyboard.Key.F3,
                    Keyboard.Key.F4,
                    Keyboard.Key.F5,
                    Keyboard.Key.F6,
                    Keyboard.Key.F7,
                    Keyboard.Key.F8,
                    Keyboard.Key.F9,
                    Keyboard.Key.F10,
                    Keyboard.Key.F11,
                    Keyboard.Key.F12,
                };
            foreach (Keyboard.Key key in System.Enum.GetValues(typeof(Keyboard.Key)))
            {
                if (keysWhiteList.Contains(key)) continue;
                if (Keyboard.IsKeyPressed(key))
                    totalManager.SetGamemodeType(GamemodeType.MAIN_MENU);
            }

        }


        protected override void DrawProcess()
        {
            //배경이 하얀 효과
            Dm.texUiBackground.Clear(Color.Black);


            //페이드 인 아웃 투명도 조절
            Time time = clock.ElapsedTime;
            float logoTimeNow = time.AsMilliseconds();

            float gammaRatio;
            if (logoTimeNow < logoTimePre)
                //초기 부분
                gammaRatio = 0f;
            if (logoTimeNow < logoTimePre+logoTimeEdge)
                //시작 부분
                gammaRatio = logoTimeNow / (logoTimePre +logoTimeEdge);
            else if (logoTimeMax - logoTimeEdge < logoTimeNow)
                //끝나는 부분
                gammaRatio = 1f - (logoTimeNow - (logoTimeMax - logoTimeEdge)) / logoTimeEdge;
            else
                //중간 부분
                gammaRatio = 1f;

            byte alphaValue = (byte)(255 * Math.Max(Math.Min( 1f - gammaRatio, 1f), 0f));


            //페이드 인 아웃 표과
            DrawManager.texUiWhole.Clear(new Color(0, 0, 0, alphaValue));
        }

    }
}
