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
        public Humanoid(Gamemode gamemode, Vector2f position) : base(gamemode, position, new Circle(position, 30f))
        {
            inventory = new Inventory(this);
            hands = new Hands(this);
        }

        public const float accel = 3000f;    //가속
        public float accelPer = 1.00f;      //가속 배율
        public const float friction = 8.0f;   //마찰

        //속도 벡터
        public Vector2f speed = Vector2fEx.Zero;
        //가속 벡터 (최대 1)
        public Vector2f moveDir = Vector2fEx.Zero;

        public float aimDistance = 100f;

        public Vector2f AimVector 
        {
            get { return new Vector2f(Direction, aimDistance); }
            set { 
                Direction = value.X;
                aimDistance = value.Y;
            }
        }
        public Vector2f AimPosition
        {
            get { return Position + (-Direction-90).ToRadian().ToVector() * aimDistance; }
            set {
                Vector2f aimPosRel = value - Position;
                Direction = aimPosRel.ToDirection();
                aimDistance = aimPosRel.Magnitude();
            }
        }




        protected override void DrawProcess()
        {
            DrawManager.texWrHigher.Draw(mask, CameraManager.worldRenderState);
        }

        protected override void LogicProcess()
        {

        }

        protected override void PhysicsProcess()
        {
            //마찰에 의한 감속
            double deltaTime = 1d / gamemode.logicFps;
            speed *= (float)(1d - friction * deltaTime);

            //이동에 의한 가속
            Vector2f accelVec = moveDir.Magnitude() > 1f? moveDir.Normalize() : moveDir;
            speed += accelVec * (float)(accel * accelPer * deltaTime);

            #region [벽에 의한 충돌]

            Circle maskC = (Circle)mask;
            Vector2f originPos = maskC.Position;
            Vector2f toMove = speed * (float)deltaTime;
            
            Action<Vector2f> collisionAct = (springVec) =>
            {
                maskC.Position = originPos;
                speed = springVec; //양축 속도를 조진다. 탄성 때문에 살짝 밀림
            };

            foreach (Structure structure in ((GamemodeIngame)gamemode).structures)
            {
                maskC.Position = originPos + toMove;

                //(x,y)충돌 없음.
                if (structure.mask.IsCollision(mask) == false) { /*Console.WriteLine("0,0");*/ continue; }

                Vector2f springVec = (Position - structure.Position).Normalize() * 20f;
                //Console.WriteLine(pullBack);

                //충돌이 있다는건 알았음. 이제 y축 충돌인지 x축 충돌인지 둘 다 인지 검사
                //(0, y)
                maskC.Position = originPos + new Vector2f(0f, toMove.Y);
                if (structure.mask.IsCollision(mask) == false)
                {
                    //x축 문제라는걸 유추 가능
                    //Console.WriteLine("x, 0");
                    collisionAct(new Vector2f(0f, springVec.Y));
                    continue;
                }

                //y축에 문제 있는 상태. x축에도 문제 있는지 확인하고 처리
                //(x, 0)
                maskC.Position = originPos + new Vector2f(toMove.X, 0f);
                if (structure.mask.IsCollision(mask) == false)
                {
                    //y축의 문제라는걸 유추 가능.
                    //Console.WriteLine("0, y");
                    collisionAct(new Vector2f(springVec.X, 0f));
                    continue;
                }

                //x, y축 모두의 문제
                //(x, y)
                //Console.WriteLine("x, y");
                collisionAct(springVec);
                continue;

            }
            #endregion

            //속도에 의한 변위
            Position += speed * (float)deltaTime;

        }


    }
    
}
