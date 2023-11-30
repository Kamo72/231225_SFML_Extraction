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

namespace _231109_SFML_Test
{
    internal class Humanoid : Entity
    {
        public Humanoid(Gamemode gamemode, Vector2f position) : base(gamemode, position, new Circle(position, 0.3f))
        {

        }

        public const float accel = 0.2f;    //가속
        public float accelPer = 1.00f;      //가속 배율
        public const float friction = 2.4f;   //마찰

        //속도 벡터
        public Vector2f speed = Vector2fEx.Zero;
        //가속 벡터 (최대 1)
        public Vector2f moveDir = Vector2fEx.Zero;

        

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
            double deltaTime = 1000d / gamemode.logicFps;
            speed *= (float)(1d - friction * deltaTime);

            //이동에 의한 가속
            Vector2f accelVec = moveDir.Magnitude() > 1f? moveDir.Normalize() : moveDir;
            speed += accelVec * (float)(accel * accelPer * deltaTime);

            //속도에 의한 변위
            Position += speed * (float)deltaTime;
        }
    }
    
}
