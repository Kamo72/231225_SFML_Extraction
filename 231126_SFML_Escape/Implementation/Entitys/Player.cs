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
using SFML.Window;

namespace _231109_SFML_Test
{
    internal class Player : Humanoid
    {
        public Player(Gamemode gamemode, Vector2f position) : base(gamemode, position)
        {

        }

        protected override void DrawProcess()
        {
            //마스크 그리기 (임시)
            DrawManager.texUiInterface.Draw(mask, CameraManager.worldRenderState);


            //마우스 변위만큼 조준점 이동
            AimVector = new Vector2f(AimVector.X, Mathf.Clamp(40f, AimVector.Y, 5000f));
            AimVector += new Vector2f(InputManager.mouseDelta.X , -InputManager.mouseDelta.Y);


            CameraManager.zoomValue = Mathf.Clamp(0.5f, aimDistance /  900f, 3.0f);

            Console.WriteLine(aimDistance);
            CircleShape cir = new CircleShape(5f);
            cir.FillColor = Color.Red;
            cir.Origin = new Vector2f(cir.Radius, cir.Radius);
            cir.Position = AimPosition;
            DrawManager.texWrAugment.Draw(cir, CameraManager.worldRenderState);
            cir.Dispose();

        }

        protected override void LogicProcess()
        {
            CameraManager.targetPos = Position + (-Direction - 90f).ToRadian().ToVector() *  VideoManager.resolutionNow.Y * 0.4f * CameraManager.zoomValue;
            //카메라 회전 = 캐릭터 회전
            CameraManager.targetRot = Direction;
        }

        protected override void PhysicsProcess()
        {
            //Console.WriteLine(Position);

            moveDir = Vector2fEx.Zero;
            if (Im.CommandCheck(Im.CommandType.MOVE_LEFT)) moveDir +=     (-Direction + 180f).ToRadian().ToVector();
            if (Im.CommandCheck(Im.CommandType.MOVE_RIGHT)) moveDir +=    (-Direction + 000f).ToRadian().ToVector();
            if (Im.CommandCheck(Im.CommandType.MOVE_FORWARD)) moveDir +=  (-Direction + 270f).ToRadian().ToVector();
            if (Im.CommandCheck(Im.CommandType.MOVE_BACKWARD)) moveDir += (-Direction + 090f).ToRadian().ToVector();

            base.PhysicsProcess();
        }


    }
}
