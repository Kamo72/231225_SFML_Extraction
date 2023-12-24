using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    class UiTest : Ui 
    {
        public UiTest(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, size)
        {
            Clicked += () =>
            { 
                Random random = new Random();
                CameraManager.GetShake(10f);
            };

        }

        protected override void DrawProcess()
        {
            DrawManager.texUiInterface.Draw(this.mask);
        }

        protected override void LogicProcess()
        {
        }
    }

}
