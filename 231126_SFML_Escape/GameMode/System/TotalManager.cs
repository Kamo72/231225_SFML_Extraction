using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;

namespace _231109_SFML_Test
{
    internal class TotalManager
    {
        public TotalManager()
        {
            SetGamemodeType(GamemodeType.LOGO);
        }

        public void DrawAll()
        {
            //카메라 위상에 맞게 Transform 최신화
            CameraManager.RefreshTransform();
            //카메라 흔들림 적용
            CameraManager.ShakeProcess();

            //그릴거 다 그리기
            gmNow?.DoDraw();
            
            //결과 텍스쳐 최신화
            DrawManager.ResultTexture();
        }

        public Gamemode gmNow;
        public GamemodeType gamemode = GamemodeType.NONE;
        public void SetGamemodeType(GamemodeType gamemode)
        {
            if (this.gamemode == gamemode) return;
            
            gmNow?.Dispose();
            //gmNow = null;
            this.gamemode = gamemode;

            switch (this.gamemode)
            {
                case GamemodeType.LOGO:
                    gmNow = new GamemodeLogo(this);
                    break;
                case GamemodeType.MAIN_MENU:
                    gmNow = new GamemodeMainMenu(this);
                    break;
                case GamemodeType.INGAME:
                    gmNow = new GamemodeIngame(this);
                    break;
                case GamemodeType.RESULT:
                    break;
            }
        }


    }

    public enum GamemodeType
    {
        NONE,
        LOGO,
        MAIN_MENU,
        INGAME,
        RESULT,
    }

}
