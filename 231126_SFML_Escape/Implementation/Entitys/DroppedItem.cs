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
        public DroppedItem(Gamemode gamemode, Vector2f position, Item item) : base(gamemode, position, new Circle(position, 10f))
        {
            this.item = item;

            drawShape = new RectangleShape(new Vector2f(100f, 100f));

            Texture texture = ResourceManager.textures[item.spriteName];
            if (texture != null) throw new Exception("DroppedItem - DrawProcess - item의 텍스쳐를 불러오지 못했습니다...");
            drawShape.Texture = texture;
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
            if (caster.inventory.TakeItem(item))
                Dispose();

            Console.WriteLine("DroppedItem - BeInteract - 실패");
        }


        //Entity 구현
        protected override void DrawProcess()
        {
            DrawManager.texWrLower.Draw(drawShape, CameraManager.worldRenderState);
        }

        protected override void LogicProcess()
        {
            drawShape.Position = Position;
            drawShape.Rotation = Direction;
        }

        protected override void PhysicsProcess()
        {

        }



        public override void Dispose()
        {
            base.Dispose();
            drawShape?.Dispose();
        }

    }
}
