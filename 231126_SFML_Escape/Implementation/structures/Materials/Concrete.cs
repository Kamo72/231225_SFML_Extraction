
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
using SFML.Graphics;
using System.Diagnostics;

namespace _231109_SFML_Test
{
    internal abstract class Concrete : Structure
    {
        public Concrete(Gamemode gamemode, Vector2f position, ICollision mask) : base(gamemode, position, mask, DestructLevel.NONE, 0.0f, 4.8f)
        {
            if (mask is Shape shape)
            {
                shape.Texture = Rm.textures["texConcrete"];
                shape.OutlineThickness = 10.0f;
                shape.OutlineColor = Color.Black;
            }
        }
        protected override void DrawProcess()
        {
            DrawManager.texWrHigher.Draw(mask, Cm.worldRenderState);
        }
    }

}