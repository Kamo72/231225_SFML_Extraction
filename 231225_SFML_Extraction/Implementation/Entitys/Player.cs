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

namespace _231109_SFML_Test
{
    internal class Player : Humanoid
    {
        public Player(Gamemode gamemode, Vector2f position) : base(gamemode, position, 1000)
        {
            CameraManager.traggingProcess = () =>
            {
                CameraManager.position = Position + (-Direction - 90f).ToRadian().ToVector() * VideoManager.resolutionNow.Y * 0.4f * CameraManager.zoomValue;
                //카메라 회전 = 캐릭터 회전
                CameraManager.rotation = Direction;
            };

            hands.handling = new FN_FAL();

            //테스트
            hlTexture = ResourceManager.textures["LIGHT_radial"];
            highlightShape = new RectangleShape(new Vector2f(100f, 100f));
            highlightShape.Origin = highlightShape.Size / 2f;
            highlightShape.Texture = hlTexture;

            DrawUiInit();
        }

        //테스트
        Texture hlTexture;
        RectangleShape highlightShape;



        protected override void DrawProcess()
        {
            base.DrawProcess();

            //마스크 그리기 (임시)
            DrawManager.texWrHigher.Draw(mask, CameraManager.worldRenderState);

            //마우스 변위만큼 조준점 이동
            aimPosition = new Vector2f(
                Mathf.Clamp(0f, aimPosition.X, Vm.resolutionNow.X),
                Mathf.Clamp(0f, aimPosition.Y, Vm.resolutionNow.Y)
                );
            AimPosition += new Vector2f(InputManager.mouseDelta.X , InputManager.mouseDelta.Y);

            //방향 최신화
            Direction = (aimPosition - (Vector2f)Vm.resolutionNow /2f).ToDirection().ToDirection();

            //UI
            DrawInventoryProcess();
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
            CameraManager.targetPos = Position + (aimPosition - (Vector2f)Vm.resolutionNow / 2f) * 0.5f;
        }

        protected override void PhysicsProcess()
        {
            try
            {
                //이동 방향 지정
                moveDir = Vector2fEx.Zero;
                if (Im.CommandCheck(Im.CommandType.MOVE_LEFT))      moveDir += (+180f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_RIGHT))     moveDir += (+000f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_BACKWARD))  moveDir += (+090f).ToRadian().ToVector();
                if (Im.CommandCheck(Im.CommandType.MOVE_FORWARD))   moveDir += (+270f).ToRadian().ToVector();

                base.PhysicsProcess();

                //갖고 있는 아이템의 조작
                hands.LogicHandlingProcess();
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
                    if (closestDis > newDis)
                    {
                        interactableChoosen = interactable;
                        closestDis = newDis;
                    }
                }
            }

            if (interactableChoosen is Entity intEntt)
                if (Im.CommandCheck(Im.CommandType.INTERACT))
                    hands.Interact(intEntt);
        }


        //UI
        Vector2f hpUiSize = new Vector2f(600f, 30f);
        RectangleShape hpMaxBox, hpNowBox, hpBleedingBox;
        CircleShape aimPoint;
        void DrawUiInit() 
        {
            aimPoint = new CircleShape(5f);
            aimPoint.FillColor = Color.White;
            aimPoint.Origin = new Vector2f(aimPoint.Radius, aimPoint.Radius);

            hpMaxBox = new RectangleShape(hpUiSize);
            hpMaxBox.OutlineColor = new Color(20, 20, 20);
            hpMaxBox.OutlineThickness = 6f;
            hpMaxBox.FillColor = new Color(40, 40, 40);
            hpMaxBox.Position = new Vector2f(0f, Vm.resolutionNow.Y) + new Vector2f(hpUiSize.X, -hpUiSize.Y) / 2f + new Vector2f(40f, -40f);
            hpMaxBox.Origin = hpUiSize / 2f;

            hpNowBox = new RectangleShape(hpUiSize);
            hpNowBox.FillColor = new Color(40, 210, 40);
            hpNowBox.Position = new Vector2f(0f, Vm.resolutionNow.Y) + new Vector2f(hpUiSize.X, -hpUiSize.Y) / 2f + new Vector2f(40f, -40f);
            hpNowBox.Origin = hpUiSize / 2f;

            hpBleedingBox = new RectangleShape(new Vector2f(0f, hpUiSize.Y));
            hpBleedingBox.Position = new Vector2f(0f, Vm.resolutionNow.Y) + new Vector2f(hpUiSize.X, -hpUiSize.Y) / 2f + new Vector2f(40f, -40f);
            hpBleedingBox.Origin = hpUiSize / 2f;



        }
        void DrawUiDispose() 
        {
            aimPoint.Dispose();
        }
        void DrawInventoryProcess()
        {
        
        }
        void DrawHudProcess()
        {
            //조준점 그리기
            aimPoint.Position = aimPosition;
            DrawManager.texUiInterface.Draw(aimPoint);

            //체력 게이지
            float hpRatio = Mathf.Clamp(0f, health.healthNow / health.healthMax, 1f);
            hpNowBox.Size = new Vector2f(hpUiSize.X * hpRatio, hpUiSize.Y);
            DrawManager.texUiInterface.Draw(hpMaxBox);
            DrawManager.texUiInterface.Draw(hpNowBox);

            //출혈
            float tTime = Math.Abs(Vm.GetTimeTotal()*4f % 2f - 1f); //깜빡이는 이펙트
            hpBleedingBox.FillColor = new Color((byte)(40 * 170 * tTime), 40, 40);

            float bleedingRatio = Mathf.Clamp(0f, health.bleeding / health.healthNow, 1f);
            hpBleedingBox.Size = new Vector2f(hpNowBox.Size.X * bleedingRatio, hpUiSize.Y);
            hpBleedingBox.Position = new Vector2f(0f, Vm.resolutionNow.Y) + new Vector2f(hpUiSize.X, -hpUiSize.Y) / 2f + new Vector2f(40f, -40f) + new Vector2f(hpNowBox.Size.X * (1f-bleedingRatio), 0f);

            DrawManager.texUiInterface.Draw(hpBleedingBox);

        }

    }
}
