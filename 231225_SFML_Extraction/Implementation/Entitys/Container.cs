using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _231109_SFML_Test
{
    internal class Container : Entity, IInteractable
    {
        //창고
        public Storage storage;

        //상자 열기 제어
        public bool isOpen = false;
        public Humanoid openBy = null;
        public RectangleShape drawShape;

        public string name = "";

        public Container(Gamemode gamemode, Vector2f position, Vector2i storageSize, Vector2f size, string TextureName, string name)
            : base(gamemode, position, new Box(position, size * 0.7f))
        {
            //하이라이트 그리기
            InitHighlight();

            //창고 생성
            storage = new Storage(storageSize);

            //그리기 도형 생성
            drawShape = new RectangleShape(size);
            drawShape.Origin = size / 2f;
            drawShape.Position = position;
            drawShape.Texture = ResourceManager.textures[TextureName];

            this.name = name;
        }

        //상자 열기
        public bool Open(Humanoid entity)
        {
            if (isOpen == false)
            {
                isOpen = true;
                openBy = entity;

                if (entity is Player player) player.InventoryOpen();
                else entity.hands.onInventory = true;

                entity.hands.interactingTarget = this;

                return true;
            }
            return false;
        }
        //상자 닫기
        public void Close()
        {
            if (isOpen == false)
            {
                Console.WriteLine("Container - Close 열려있지 않은 컨터이너를 닫으려고 시도");
                return;
            }

            isOpen = false;
            openBy = null;
        }


        #region [Highlight]

        public void InitHighlight()
        {
            Texture hlTexture = ResourceManager.textures["LIGHT_radial"];
            highlightShape = new RectangleShape(new Vector2f(100f, 100f));
            highlightShape.Origin = highlightShape.Size / 2f;
            highlightShape.Texture = hlTexture;

            highlightText = new Text("F : 열기", ResourceManager.fonts["Jalnan"]);
            highlightText.CharacterSize = 20;
            highlightText.Origin = new Vector2f(highlightText.GetLocalBounds().Width / 2, highlightText.GetLocalBounds().Height / 2);
            highlightText.FillColor = new Color(220, 220, 220);
            highlightText.OutlineThickness = 2;
            highlightText.OutlineColor = new Color(20, 20, 20);
        }
        public Text highlightText { get; set; }
        public RectangleShape highlightShape { get; set; }
        public bool isHighlighed { get; set; } = false;
        public float highlighValue { get; set; } = 0f;

        public void DrawHighlight()
        {
            //하이라이트 여부 > 진행도
            highlighValue = Mathf.Clamp(0f, highlighValue + (isHighlighed ? +0.03f : -0.03f), 1f);
            byte highlighAlpha = (byte)(255 * highlighValue);

            //하이라이트 진행도 > 크기
            highlightShape.Size = new Vector2f(100f, 100f) * highlighValue;
            highlightShape.Origin = highlightShape.Size / 2f;

            highlightShape.Position = Position;
            highlightText.Position = Position + new Vector2f(0f, 50f);
            highlightText.FillColor = new Color(highlightText.FillColor) { A = highlighAlpha };
            highlightText.OutlineColor = new Color(highlightText.OutlineColor) { A = highlighAlpha };

            DrawManager.texWrLower.Draw(highlightShape, CameraManager.worldRenderState);
            DrawManager.texWrAugment.Draw(highlightText, CameraManager.worldRenderState);
        }

        #endregion


        public bool IsInteractable(Humanoid caster)
        {
            return true;
        }

        public void BeInteract(Humanoid caster)
        {
            Open(caster);
            //Close는 Humanoid측에서 진행
        }


        protected override void LogicProcess()
        {
            if (isOpen)
            if (openBy != null)
            if (openBy.hands.onInventory == false)
                Close();
        }

        protected override void PhysicsProcess()
        {
        }

        protected override void DrawProcess()
        {
            DrawHighlight();
            
            drawShape.Position = Position;
            DrawManager.texWrLower.Draw(drawShape, CameraManager.worldRenderState);
        }

    }

    internal class K_WoodenAmmoBox : Container
    {
        public K_WoodenAmmoBox(Gamemode gamemode, Vector2f position)
            : base(gamemode, position, new Vector2i(6, 4), new Vector2f(50f, 50f), "K_WoodenAmmoBox", "군용 목제 탄약 상자") { }
    }
}
