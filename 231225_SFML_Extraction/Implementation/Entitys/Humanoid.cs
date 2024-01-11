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


using static _231109_SFML_Test.Storage;
using System.Windows.Forms;
using SFML.Graphics;
using System.IO.Ports;
using System.Drawing;

namespace _231109_SFML_Test
{
    internal partial class Humanoid : Entity//, IInteractable 대화 시스템~
    {
        public Humanoid(Gamemode gamemode, Vector2f position, float healthMax = 400) : base(gamemode, position, new Circle(position, 30f))
        {
            inventory = new Inventory(this);
            hands = new Hands(this);
            health = new Health(this, healthMax);


            GamemodeIngame ingm = gamemode as GamemodeIngame;
            ingm.entitys.Add(this);
        }

        public Vector2f aimPosition = Vector2fEx.Zero;
        public Vector2f AimPosition {
            get { return aimPosition + Position; }
            set { aimPosition = value - Position; }
        }
        public float aimRange = 2000f;


        protected override void DrawProcess()
        {
            DrawManager.texWrHigher.Draw(mask, CameraManager.worldRenderState);
            hands.DrawHandlingProcess();
        }

        protected override void LogicProcess()
        {
            hands.InteractableListRefresh();
        }


        //[물리]
        public float accel = 3000f;    //가속
        public float accelPer = 1.00f;      //가속 배율
        public const float friction = 8.0f;   //마찰

        public Vector2f speed = Vector2fEx.Zero; // 속도 벡터
        public Vector2f moveDir = Vector2fEx.Zero; // 가속 벡터 (최대 1)
        protected override void PhysicsProcess()
        {
            //마찰에 의한 감속
            double deltaTime = 1d / gamemode.logicFps;
            speed *= (float)(1d - friction * deltaTime);

            //이동에 의한 가속
            Vector2f accelVec = moveDir.Magnitude() > 1f? moveDir.Normalize() : moveDir;
            speed += accelVec * (float)(accel * accelPer * deltaTime);

            #region [충돌]

            Circle maskHum = mask as Circle;
            Vector2f posOrigin = Position;
            Vector2f vecOrigin = speed;// * (float)deltaTime;

            GamemodeIngame gm = gamemode as GamemodeIngame;
            //벽과의 충돌
            foreach (Structure stru in gm.structures)
            {
                maskHum.Position = posOrigin + new Vector2f(vecOrigin.X * (float)deltaTime, 0f);
                if (maskHum.IsCollision(stru.mask))
                    vecOrigin.X = Math.Abs(vecOrigin.X) * Math.Sign(vecOrigin.X) * -0.5f;

                maskHum.Position = posOrigin + new Vector2f(0f, vecOrigin.Y * (float)deltaTime);
                if (maskHum.IsCollision(stru.mask))
                    vecOrigin.Y = Math.Abs(vecOrigin.Y) * Math.Sign(vecOrigin.Y) * -0.5f;

                maskHum.Position = posOrigin;
                speed = vecOrigin;
            }

            //엔티티와의 충돌
            foreach (Entity ent in gm.entitys)
            {
                if (ent == null) continue;
                if (ent.isDisposed == true) continue;
                if (ent.Position == this.Position) continue;
                if (ent is Humanoid == false) continue;
                if (ent.mask.IsCollision(mask))
                {
                    float dis = (Position - ent.Position).Magnitude();
                    float pushMultipier = 1f / (dis + 1f) * 10000f;
                    Vector2f push = (Position - ent.Position).Normalize() * pushMultipier;
                    speed += push;
                    if (ent is Humanoid human)
                        human.speed -= push;
                }
            }
            #endregion

            //속도에 의한 변위
            Position += speed * (float)deltaTime ;

        }



        public override void Dispose()
        {
            base.Dispose();

            GamemodeIngame ingm = gamemode as GamemodeIngame;
            ingm.entitys.Remove(this);
        }

    }
    
}
