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
using SFML.Graphics;
using SFML.System;

namespace _231109_SFML_Test
{
    internal class TextMainMenu : TextLabel
    {
        public TextMainMenu(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, size)
        {

        }

        protected override void LogicProcess()
        {
            base.LogicProcess();
        }
        protected override void DrawProcess()
        {
            base.DrawProcess();
        }


    }
}
