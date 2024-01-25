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
using System.Collections;
using static FastNoise;
using static _231109_SFML_Test.Humanoid;
using System.Drawing.Printing;

namespace _231109_SFML_Test
{
    internal partial class Player : Humanoid
    {
        public Player(Gamemode gamemode, Vector2f position) : base(gamemode, position, 1000)
        {
            CameraManager.traggingProcess = () =>
            {
                CameraManager.position = Position + (-Direction - 90f).ToRadian().ToVector() * VideoManager.resolutionNow.Y * 0.4f * CameraManager.zoomValue;
                //카메라 회전 = 캐릭터 회전
                CameraManager.rotation = Direction;
            };

            // hands.handling = new FN_FAL();
            inventory.weaponPrimary.DoEquipItem(new FN_FAL());

            if(inventory.weaponPrimary.item != null)
                hands.SetHandling(inventory.weaponPrimary.item as Weapon);

            //UI 객체들 초기화
            DrawUiInit([
                new PlayerCrosshairDrawer(this),
                new PlayerHpDrawer(this),
                new PlayerTabDrawer(this),
                new PlayerAdsDrawer(this),
            ]);
        }


        protected override void DrawProcess()
        {
            if (isDisposed) return;

            base.DrawProcess();

            //마스크 그리기 (임시)
            DrawManager.texWrHigher.Draw(mask, CameraManager.worldRenderState);

            //마우스 변위만큼 조준점 이동
            aimPosition = new Vector2f(
                Mathf.Clamp(-Vm.resolutionNow.X / 2f, aimPosition.X, Vm.resolutionNow.X / 2f),
                Mathf.Clamp(-Vm.resolutionNow.Y / 2f, aimPosition.Y, Vm.resolutionNow.Y / 2f)
                );
            AimPosition += new Vector2f(InputManager.mouseDelta.X , InputManager.mouseDelta.Y);


            //UI
            DrawHudProcess();
        }

        protected override void LogicProcess()
        {
            try
            {
                base.LogicProcess();
                this.InteractionProcess();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message + ex.StackTrace); }


            //상호작용
            if (Im.CommandCheck(Im.CommandType.INTERACT)) 
                if (interactableChoosen is Entity intEntt)
                    hands.Interact(intEntt);

            //인벤토리
            if (Im.CommandCheck(Im.CommandType.INVENTORY))
                if (hands.onInventory) InventoryClose();
                else InventoryOpen();

            CameraManager.targetPos = Position + aimPosition * 0.5f;
        }

        protected override void PhysicsProcess()
        {
            try
            {
                //이동 자세 전환
                if (Im.CommandCheck(Im.CommandType.SPRINT)) movement.targetIndex = movement.targetIndex == Movement.MovementState.SPRINT ? Movement.MovementState.IDLE : Movement.MovementState.SPRINT;
                if (Im.CommandCheck(Im.CommandType.CROUNCH)) movement.targetIndex = movement.targetIndex == Movement.MovementState.CROUNCH ? Movement.MovementState.IDLE : Movement.MovementState.CROUNCH;

                //이동 방향 지정
                movement.moveDir = Vector2fEx.Zero;
                if (Im.CommandCheck(Im.CommandType.MOVE_LEFT))      movement.moveDir += (+180f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_RIGHT))     movement.moveDir += (+000f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_BACKWARD))  movement.moveDir += (+090f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_FORWARD))   movement.moveDir += (+270f).ToRadian().ToVector();

                base.PhysicsProcess();

                //플레이어 탭 드로워.로직 프로세스
                PlayerTabDrawer ptd = uiList?.Find(ui => ui is PlayerTabDrawer) as PlayerTabDrawer;
                ptd?.LogicProcess();

                //갖고 있는 아이템의 조작
                hands.LogicHandlingProcess(cmd => InputManager.CommandCheck(cmd));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


        //상호작용
        List<IInteractable> interactables = new List<IInteractable>();
        IInteractable interactableChoosen;
        protected void InteractionProcess() 
        {
            float closestDis = 9999f;

            //그전의 상호작용 가능한 목록에서 변함이 없다면? 바꿀 필요가 없겠쥬?
            bool hasChanged = interactables.SequenceEqual(hands.interactables) == false;
            if (hasChanged)
            {
                foreach(var inter in interactables)
                    inter.isHighlighed = false;

                interactables = hands.interactables;                    //새로 초기화하고

                foreach (var inter in interactables)
                    inter.isHighlighed = true;

                interactableChoosen = interactables.FirstOrDefault();   //기본으로 첫번째 요소를 상호작용 대상으로
            }

            //상호작용 가능한 목록 중 마우스로 하이라이트 됐다면?
            foreach (IInteractable interactable in interactables)
            {
                if (interactable is Entity intEnt)
                {
                    float newDis = (AimPosition - intEnt.Position).Magnitude();

                    //Console.WriteLine(closestDis+ $"({AimPosition})" +  ">" + newDis +$"({intEnt.Position})");
                    if (closestDis > newDis)
                    {
                        //Console.WriteLine(closestDis + "==>" + newDis);
                        interactableChoosen = interactable;
                        closestDis = newDis;
                    }
                }
            }
            
        }


        public override void Dispose()
        {
            base.Dispose();
            DrawUiDispose();
        }

    }
}
