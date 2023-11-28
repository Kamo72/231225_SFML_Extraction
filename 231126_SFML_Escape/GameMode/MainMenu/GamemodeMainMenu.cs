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

namespace _231109_SFML_Test
{
    internal class GamemodeMainMenu : Gamemode
    {
        public GamemodeMainMenu(TotalManager tm) : base(tm, 60)
        {
            UiMenuButtonTitle ui = new UiMenuButtonTitle(this, (Vector2f)Vm.resolutionNow / 2f, new Vector2f(300f, 100f));
            ui.TextSet(Rm.fonts["Jalnan"], 100f, "testMessage");
            ui.margin = 10f;
            ui.isMultiline = true;
            uis.Add(ui);
        }

        List<Ui> uis = new List<Ui>();

        protected override void DrawProcess()
        {

        }

        protected override void LogicProcess()
        {

        }
    }
}
