using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _231109_SFML_Test
{
    internal class DroppedItem : Entity, IInteractable
    {
        public DroppedItem(Gamemode gamemode, Vector2f position, Item item) : base(gamemode, position, new Circle(position, 30f))
        {
            this.item = item;

            GamemodeIngame ingm = gamemode as GamemodeIngame;
            lock (ingm.entitys)
                ingm.entitys.Add(this);

            Texture drawTexture = ResourceManager.textures[item.spriteName];
            drawShape = new RectangleShape(new Vector2f(100f, 100f));
            drawShape.Origin = drawShape.Size / 2f;
            drawShape.Texture = drawTexture;



            InitHighlight();
        }

        RectangleShape drawShape;
        Item item;


        //IInteractable 구현
        public bool IsInteractable(Humanoid caster)
        {
            return true;
        }

        public void BeInteract(Humanoid caster)
        {
            Console.WriteLine("DroppedItem - 상호작용 실행");
            if (caster.inventory.TakeItem(item))
                Dispose();
            else
            {
                Position = caster.Position;
                speed = caster.Direction.ToRadian().ToVector() * 1000f;
            }
        }


        #region [Highlight]

        public void InitHighlight()
        {
            Texture hlTexture = ResourceManager.textures["LIGHT_radial"];
            highlightShape = new RectangleShape(new Vector2f(100f, 100f));
            highlightShape.Origin = highlightShape.Size / 2f;
            highlightShape.Texture = hlTexture;

            highlightText = new Text("F : 획득", ResourceManager.fonts["Jalnan"]);
            highlightText.CharacterSize = 20;
            highlightText.Origin = new Vector2f(highlightText.GetLocalBounds().Width / 2, highlightText.GetLocalBounds().Height / 2);
            highlightText.FillColor = new Color(220, 220, 220);
            highlightText.OutlineThickness = 2;
            highlightText.OutlineColor = new Color(20, 20, 20);
        }
        public Text highlightText { get; set; }
        public RectangleShape highlightShape { get; set; }
        public bool isHighlighed { get; set; }  = false;
        public float highlighValue { get; set; } = 0f;

        public void DrawHighlight()
        {
            try
            {
                if (isDisposed) return;

                //하이라이트 여부 > 진행도
                highlighValue = Mathf.Clamp(0f, highlighValue + (isHighlighed ? +0.03f : -0.03f), 1f);
                
                //진행도 0이라면 안 그림
                if (highlighValue < 0.0001f) return;
                
                byte highlighAlpha = (byte)(255 * highlighValue);

                //하이라이트 진행도 > 크기
                //highlightShape.Size = new Vector2f(100f, 100f) * highlighValue;
                //highlightShape.Origin = highlightShape.Size / 2f;

                //highlightShape.Position = Position;
                highlightText.Position = Position + new Vector2f(0f, 50f);
                highlightText.FillColor = new Color(highlightText.FillColor) { A = highlighAlpha };
                highlightText.OutlineColor = new Color(highlightText.OutlineColor) { A = highlighAlpha };

                //DrawManager.texWrLower.Draw(highlightShape, CameraManager.worldRenderState);

                DrawManager.texWrAugment.Draw(highlightText, CameraManager.worldRenderState);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message + ex.StackTrace); }

        }

        #endregion


        protected override void DrawProcess()
        {
            if(isDisposed) return;
            try
            {
                //드로우 위치
                drawShape.Position = Position;

                //하이라이트 그리기
                DrawHighlight();

                //드로우
                DrawManager.texWrLower.Draw(drawShape, CameraManager.worldRenderState);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message + ex.StackTrace); }
        }

        //Entity 구현
        protected override void LogicProcess()
        {
            if (isDisposed) return;
            drawShape.Position = Position;
            drawShape.Rotation = Direction;

            GamemodeIngame gm = gamemode as GamemodeIngame;
            foreach (Entity ent in gm.entitys)
            {
                if (ent.Position == this.Position) continue;
                if ((ent is DroppedItem) == false) continue;
                if (ent.mask.IsCollision(mask))
                {
                    float dis = (Position - ent.Position).Magnitude();
                    float pushMultipier = 1f / (dis + 1f) * 100f;
                    Vector2f push = (Position - ent.Position).Normalize() * pushMultipier;
                    speed += push;
                    if (ent is DroppedItem human)
                        human.speed -= push;
                }
            }

        }


        
        //[물리]
        public const float friction = 12.0f;   //마찰

        public Vector2f speed = Vector2fEx.Zero; // 속도 벡터
        protected override void PhysicsProcess()
        {
            //마찰에 의한 감속
            double deltaTime = 1d / gamemode.logicFps;
            speed *= (float)(1d - friction * deltaTime);

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
                    if (Math.Sign(posOrigin.X - stru.Position.X) != Math.Sign(vecOrigin.X))
                        vecOrigin.X = Math.Abs(vecOrigin.X) * Math.Sign(vecOrigin.X) * -0.5f;

                maskHum.Position = posOrigin + new Vector2f(0f, vecOrigin.Y * (float)deltaTime);
                if (maskHum.IsCollision(stru.mask))
                    if ( Math.Sign(posOrigin.Y - stru.Position.Y) != Math.Sign(vecOrigin.Y))
                        vecOrigin.Y = Math.Abs(vecOrigin.Y) * Math.Sign(vecOrigin.Y) * -0.5f;
                

                maskHum.Position = posOrigin;
                if (maskHum.IsCollision(stru.mask))
                    vecOrigin += (posOrigin - stru.Position).ToDirection().ToRadian().ToVector() * 10f;

                maskHum.Position = posOrigin;
                speed = vecOrigin;
            }

            //엔티티와의 충돌
            foreach (Entity ent in gm.entitys)
            {
                if (ent == null) continue;
                if (ent.isDisposed == true) continue;
                if (ent.Position == this.Position) continue;
                if (ent is DroppedItem == false) continue;
                if (ent.mask.IsCollision(mask))
                {
                    float dis = (Position - ent.Position).Magnitude();
                    float pushMultipier = 1f / (dis + 1f) * 10000f;
                    Vector2f push = (Position - ent.Position).Normalize() * pushMultipier;
                    speed += push;
                    if (ent is DroppedItem item)
                        item.speed -= push;
                }
            }
            #endregion

            //속도에 의한 변위
            Position += speed * (float)deltaTime;

        }

        public override void Dispose()
        {
            isDisposed = true;
            base.Dispose();

            highlightShape?.Dispose();
            drawShape?.Dispose();

            GamemodeIngame ingm = gamemode as GamemodeIngame;
            ingm.entitys.Remove(this);

        }

    }
}
