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
using SFML.System;

namespace _231109_SFML_Test
{
    internal class Bullet : Projectile
    {
        public Bullet(Gamemode gamemode, int lifeTime, ICollision mask, Vector2f position, float rotation = 0, float speed = 0) : base(gamemode, lifeTime, mask, position, rotation, speed)
        {
        }

        public override void DrawProcess()
        {
        }

        public override void LogicProcess()
        {

        }
    }
}
