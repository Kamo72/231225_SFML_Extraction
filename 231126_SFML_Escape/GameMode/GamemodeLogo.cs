using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class GamemodeLogo : Gamemode
    {
        public GamemodeLogo(TotalManager tm) : base(tm, 60) { }

        const int logoTimeMax = 4;
        const int logoTimeEdge = 1;

        protected override void LogicProcess()
        {
            Time time = clock.ElapsedTime;
            int miliSec = time.AsMilliseconds();

            if (miliSec > logoTimeMax) 
            {
                totalManager.SetGamemodeType(GamemodeType.MAIN_MENU);
            }
            Console.WriteLine(miliSec.ToString()  + "/"+ logoTimeMax.ToString());
        }

        protected override void DrawProcess() 
        {
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

            byte rgbValue = (byte)(255 * Math.Max(Math.Min( gammaRatio, 1f), 0f));

            Vector2f res = (Vector2f)VideoManager.resolutionNow;
            RectangleShape shape = new RectangleShape(res);
            shape.FillColor = new Color(rgbValue, rgbValue, rgbValue);
            DrawManager.uiTex[0].Draw(shape);

        }

    }
}
