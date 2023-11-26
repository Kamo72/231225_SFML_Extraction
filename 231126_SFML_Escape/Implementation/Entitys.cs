using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class Humanoid : Entity
    {
        public Humanoid(Gamemode gamemode, Vector2f position) : base(gamemode, position, new Circle(position, 0.3f))
        {

        }

        const float accel = 0.2f, accelPer = 1.00f;
        Vector2f speed = new Vector2f(0, 0);

        protected override void DrawProcess()
        {
            DrawManager.worldTex[2].Draw(mask, CameraManager.worldRenderState);
        }

        protected override void LogicProcess()
        {

        }

        protected override void PhysicsProcess()
        {

        }
    }
    internal class Player : Humanoid
    {
        public Player(Gamemode gamemode, Vector2f position) : base(gamemode, position)
        {

        }

        protected override void DrawProcess()
        {
            DrawManager.worldTex[2].Draw(mask, CameraManager.worldRenderState);
        }

        protected override void LogicProcess()
        {

        }

        protected override void PhysicsProcess()
        {

        }


    }
}
