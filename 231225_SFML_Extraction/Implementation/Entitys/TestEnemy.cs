using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class TestEnemy : Humanoid
    {
        public TestEnemy(Gamemode gamemode, Vector2f position) : base(gamemode, position)
        {
            ((CircleShape)mask).FillColor = Color.Red;
            noise = new FastNoise((int)(VideoManager.GetTimeTotal() * 37));
            movement.accelPer*= 0.9f;
        }

        Entity targetEntity = null;
        float aggre = 0f, aggreMax = 3f;
        FastNoise noise;

        protected override void DrawProcess()
        {
            base.DrawProcess();

            GamemodeIngame gm = gamemode as GamemodeIngame;
            Entity targetAble = gm.entitys[0];
            Line sightChecker = new Line(Position, targetAble.Position);


            foreach (Structure str in gm.structures)
                if (str.mask.IsCollision(sightChecker)) return;

            aggre = aggreMax;
            targetEntity = targetAble;
        }

        protected override void LogicProcess()
        {
            base.LogicProcess();

            if (targetEntity != null)
                movement.moveDir = (targetEntity.Position - Position).Normalize();
            else
                movement.moveDir = new Vector2f(
                    noise.GetPerlin(VideoManager.GetTimeTotal() * 100f, 12531f),
                    noise.GetPerlin(VideoManager.GetTimeTotal() * 100f, 1613f)).Normalize();

            aggre -= VideoManager.GetTimeDelta();
            if (aggre < 0f) targetEntity = null;

            GamemodeIngame gm = gamemode as GamemodeIngame;
            if (gm.entitys[0] == null) return;

        }

        protected override void PhysicsProcess()
        {
            base.PhysicsProcess();
        }
    }
}
