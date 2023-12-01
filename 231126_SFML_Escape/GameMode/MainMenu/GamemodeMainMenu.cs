using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;
using SFML.Audio;
using _231109_SFML_Test;
using SFML.Window;
using System.Timers;

namespace _231109_SFML_Test
{
    internal class GamemodeMainMenu : Gamemode
    {
        public GamemodeMainMenu(TotalManager tm) : base(tm, 60)
        {
            #region [게임 제목]
            TextLabel textLabel = new TextLabel(this, new Vector2f(Vm.resolutionNow.X - 500f, 100f), new Vector2f(400f, 100f) );
            textLabel.TextSet(Rm.fonts["Jalnan"], 120f, Color.White, "게임 제목");
            #endregion


            #region [타이틀 항목]
            TextMainMenu titleBtn;

            titleBtn = new TextMainMenu(this, new Vector2f(100f, Vm.resolutionNow.Y - 270f), new Vector2f(100f, 30f));
            titleBtn.TextSet(Rm.fonts["Jalnan"], 25f, Color.White, "이어하기");
            titleBtn.isActivated = false;
            titleBtn.Clicked += () => { };
            textMenus.Add(titleBtn);

            titleBtn = new TextMainMenu(this, new Vector2f(100f, Vm.resolutionNow.Y - 220f), new Vector2f(100f, 30f));
            titleBtn.TextSet(Rm.fonts["Jalnan"], 25f, Color.White, "새로시작");
            titleBtn.Clicked += () =>
            {
                if (titleBtn.isActivated)
                    EffectChangeGamemode(() => totalManager.SetGamemodeType(GamemodeType.INGAME));
            };
            textMenus.Add(titleBtn);

            titleBtn = new TextMainMenu(this, new Vector2f(100f, Vm.resolutionNow.Y - 170f), new Vector2f(100f, 30f));
            titleBtn.TextSet(Rm.fonts["Jalnan"], 25f, Color.White, "정보조회");
            titleBtn.Clicked += () => { };
            textMenus.Add(titleBtn);

            titleBtn = new TextMainMenu(this, new Vector2f(100f, Vm.resolutionNow.Y - 120f), new Vector2f(100f, 30f));
            titleBtn.TextSet(Rm.fonts["Jalnan"], 25f, Color.White, "환경설정");
            titleBtn.Clicked += () => { };
            textMenus.Add(titleBtn);

            titleBtn = new TextMainMenu(this, new Vector2f(100f, Vm.resolutionNow.Y - 070f), new Vector2f(100f, 30f));
            titleBtn.TextSet(Rm.fonts["Jalnan"], 25f, Color.White, "게임종료");
            titleBtn.Clicked += () => { if (titleBtn.isActivated) Program.window.Close(); };
            textMenus.Add(titleBtn);
            #endregion
        }

        List<TextMainMenu> textMenus = new List<TextMainMenu>();


        protected override void DrawProcess()
        {
            //효과가 적용 중
            if (eftClock != null) 
            {
                byte rgbValue = (byte)( Math.Max(Math.Min(eftClock.ElapsedTime.AsSeconds()/ eftSec, 1f),0f)  * 255);

                Color color = new Color(0, 0, 0, rgbValue);
                Dm.texUiWhole.Clear(color);

                color = new Color(255, 255, 255, (byte)(255 - rgbValue));
                Dm.texUiPopup.Clear(color);
            }

        }

        protected override void LogicProcess()
        {
        }

        Clock eftClock;
        Action eftCallback;
        const float eftSec = 3f;
        //화면 전환 효과
        void EffectChangeGamemode(Action callback) 
        {
            foreach (var menus in textMenus) 
                menus.isActivated = false;

            eftClock = new Clock();
            eftCallback = () => {
                if (eftClock.ElapsedTime.AsSeconds() > eftSec)
                {
                    callback();
                    eftClock.Dispose();
                    logicEvent -= eftCallback;
                }
            };
            logicEvent += eftCallback;

        }





    }
}
