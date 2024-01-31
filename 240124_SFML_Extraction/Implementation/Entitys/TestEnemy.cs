using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class TestEnemy : Humanoid
    {
        int sidValue;
        public TestEnemy(Gamemode gamemode, Vector2f position) : base(gamemode, position)
        {
            sidValue = (int)(position.X* 125 + position.Y);

            ((CircleShape)mask).FillColor = Color.Red;
            noise = new FastNoise((int)(sidValue));
            movement.accelPer *= 0.9f;


            // hands.handling = new FN_FAL();
            inventory.weaponPrimary.DoEquipItem(new FN_FAL());
            hands.SetHandling(inventory.weaponPrimary.item as Weapon);
        }

        Entity targetEntity = null;
        float aggre = 0f, aggreMax = 3f;
        FastNoise noise;

        Dictionary<InputManager.CommandType, bool> aiCommandDic = new Dictionary<InputManager.CommandType, bool>()
        {
            { InputManager.CommandType.FIRE, false },
            { InputManager.CommandType.AIM, false },
            { InputManager.CommandType.MAGAZINE_CHANGE, false },
            //TODO
        };

        public void AiCommandProcess()
        {
            if (isDisposed) return;

            aggre -= gamemode.deltaTime;

            GamemodeIngame gm = gamemode as GamemodeIngame;
            Entity targetAble = gm.entitys[0];
            Line sightChecker = new Line(Position, targetAble.Position);

            float targetDir = (targetAble.Position - Position).ToDirection().ToDirection();
            float aimDir = aimPosition.ToDirection().ToDirection();

            if (Math.Abs(targetDir - aimDir) < 90f || Math.Abs(Math.Abs(targetDir - aimDir) - 360f) < 90f)
            {
                foreach (Structure str in gm.structures)
                    if (str.mask.IsCollision(sightChecker)) return;

                aggre = aggreMax;
                targetEntity = targetAble;
            }


            //상대 있음
            if (targetEntity != null)
            {
                if (aggre < 0f)
                {
                    targetEntity = null;
                    return;
                }

                movement.moveDir = (targetEntity.Position - Position).Normalize();

                Vector2f tPos = targetEntity.Position +
                    new Vector2f(
                        noise.GetPerlin(VideoManager.GetTimeTotal() * 10f, 13),
                        noise.GetPerlin(VideoManager.GetTimeTotal() * 10f, 1231)
                        ) * 50f;

                AimPosition = (AimPosition + tPos * 0.03f) / (1f + 0.03f);

                float aimTargetDis = (AimPosition - targetEntity.Position).Magnitude();
                float meTargetDis = (Position - targetEntity.Position).Magnitude();

                aiCommandDic[InputManager.CommandType.AIM] = (aimTargetDis < 120f) && meTargetDis > 400f ? true : false;
                aiCommandDic[InputManager.CommandType.FIRE] = (aimTargetDis < 50f) ? true : false;


                aiCommandDic[InputManager.CommandType.MAGAZINE_CHANGE] = false;
                if (hands.handling is Weapon w) 
                {
                    if (w.magazineAttached != null) 
                    {
                        if (w.magazineAttached.ammoCount == 0)
                        {
                            aiCommandDic[InputManager.CommandType.MAGAZINE_CHANGE] = true;
                        }
                    }
                }
            }
            //상대 없음
            else
            {
                movement.moveDir = new Vector2f(
                    noise.GetPerlin(VideoManager.GetTimeTotal() * 100f, 12531f),
                    noise.GetPerlin(VideoManager.GetTimeTotal() * 100f, 1613f)).Normalize();

                Vector2f tPos = Position +
                    new Vector2f(
                        noise.GetPerlin(VideoManager.GetTimeTotal() * 10f, 13),
                        noise.GetPerlin(VideoManager.GetTimeTotal() * 10f, 1231)
                        ) * 400f;

                AimPosition = (AimPosition + tPos * 0.03f) / (1f + 0.03f);


                aiCommandDic[InputManager.CommandType.AIM] = false;
                aiCommandDic[InputManager.CommandType.FIRE] = false;

            }
        }

        protected override void DrawProcess()
        {
            base.DrawProcess();
        }

        protected override void LogicProcess()
        {
            base.LogicProcess();
            AiCommandProcess();

            //손의 무기 제어
            hands.LogicHandlingProcess(cmd => aiCommandDic.ContainsKey(cmd)? aiCommandDic[cmd] : false);



            GamemodeIngame gm = gamemode as GamemodeIngame;
            if (gm.entitys[0] == null) return;

        }

        protected override void PhysicsProcess()
        {
            base.PhysicsProcess();
        }
    }
}
