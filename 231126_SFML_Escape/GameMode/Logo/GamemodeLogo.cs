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

namespace _231109_SFML_Test
{
    internal class GamemodeLogo : Gamemode
    {
        public GamemodeLogo(TotalManager tm) : base(tm, 60) 
        {
            TextMainMenu logo = new TextMainMenu(this, (Vector2f)Vm.resolutionNow / 2f, (Vector2f)Vm.resolutionNow);
            logo.TextSet(Rm.fonts["Jalnan"], 100f, Color.Black, "히히상한이 바보");
            ui = logo;
        }

        Ui ui;

        const int logoTimeMax = 2000;
        const int logoTimeEdge = 500;

        protected override void LogicProcess()
        {
            Time time = clock.ElapsedTime;
            int miliSec = time.AsMilliseconds();

            if (miliSec > logoTimeMax) 
            {
                totalManager.SetGamemodeType(GamemodeType.MAIN_MENU);
            }

            
        }


        protected override void DrawProcess()
        {
            //페이드 인 아웃 표과
            RectangleShape shape = new RectangleShape((Vector2f)Vm.resolutionNow);
            shape.FillColor = Color.White;
            Dm.texUiBackground.Draw(shape);



            Time time = clock.ElapsedTime;
            float logoTimeNow = time.AsMilliseconds();

            float gammaRatio;
            if (logoTimeNow < logoTimeEdge)
                //시작 부분
                gammaRatio = logoTimeNow / logoTimeEdge;
            else if (logoTimeMax - logoTimeEdge < logoTimeNow)
                //끝나는 부분
                gammaRatio = 1f - (logoTimeNow - (logoTimeMax - logoTimeEdge)) / logoTimeEdge;
            else
                //중간 부분
                gammaRatio = 1f;

            byte alphaValue = (byte)(255 * Math.Max(Math.Min( gammaRatio, 1f), 0f));


            //페이드 인 아웃 표과
            RectangleShape shapeFade = new RectangleShape((Vector2f)Vm.resolutionNow);
            shapeFade.FillColor = new Color(255, 255, 255, alphaValue);
            DrawManager.texUiWhole.Draw(shapeFade);

        }

    }
}
