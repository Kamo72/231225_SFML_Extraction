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
using System.Security.Claims;

namespace _231109_SFML_Test
{
    internal class Player : Humanoid
    {
        public Player(Gamemode gamemode, Vector2f position) : base(gamemode, position)
        {
            CameraManager.traggingProcess = () =>
            {
                CameraManager.position = Position + (-Direction - 90f).ToRadian().ToVector() * VideoManager.resolutionNow.Y * 0.4f * CameraManager.zoomValue;
                //카메라 회전 = 캐릭터 회전
                CameraManager.rotation = Direction;
            };

            hands.handling = new FN_FAL();
        }

        protected override void DrawProcess()
        {
            hands.DrawHandlingProcess();

            //마스크 그리기 (임시)
            DrawManager.texUiInterface.Draw(mask, CameraManager.worldRenderState);

            //마우스 변위만큼 조준점 이동
            aimPosition = new Vector2f(
                Mathf.Clamp(0f, aimPosition.X, Vm.resolutionNow.X),
                Mathf.Clamp(0f, aimPosition.Y, Vm.resolutionNow.Y)
                );
            AimPosition += new Vector2f(InputManager.mouseDelta.X , InputManager.mouseDelta.Y);

            //CameraManager.zoomValue = Mathf.Clamp(0.5f, (aimPosition - Position).Magnitude() /  650f, 3.0f);

            CircleShape cir = new CircleShape(5f);
            cir.FillColor = Color.White;
            cir.Origin = new Vector2f(cir.Radius, cir.Radius);
            cir.Position = aimPosition;
            DrawManager.texUiInterface.Draw(cir);
            cir.Dispose();


        }

        protected override void LogicProcess()
        {
            CameraManager.targetPos = Position + (aimPosition - (Vector2f)Vm.resolutionNow / 2f) * 0.5f;
            //카메라 회전 = 캐릭터 회전

        }

        protected override void PhysicsProcess()
        {
            try
            {
                //이동 방향 지정
                moveDir = Vector2fEx.Zero;
                if (Im.CommandCheck(Im.CommandType.MOVE_LEFT)) moveDir += (-Direction + 180f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_RIGHT)) moveDir += (-Direction + 000f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_FORWARD)) moveDir += (-Direction + 270f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_BACKWARD)) moveDir += (-Direction + 090f).ToRadian().ToVector();

                base.PhysicsProcess();

                //갖고 있는 아이템의 조작
                hands.LogicHandlingProcess();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


    }
}
