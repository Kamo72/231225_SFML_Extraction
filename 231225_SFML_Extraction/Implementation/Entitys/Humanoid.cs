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
using static _231109_SFML_Test.Humanoid;

namespace _231109_SFML_Test
{
    internal partial class Humanoid : Entity//, IInteractable 대화 시스템~
    {
        public Humanoid(Gamemode gamemode, Vector2f position, float healthMax = 400) : base(gamemode, position, new Circle(position, 30f))
        {
            inventory = new Inventory(this);
            hands = new Hands(this);
            health = new Health(this, healthMax);
            movement = new Movement(this);


            GamemodeIngame ingm = gamemode as GamemodeIngame;
            ingm.entitys.Add(this);
        }

        public Vector2f aimPosition = Vector2fEx.Zero;
        public Vector2f AimPosition {
            get { return aimPosition + Position; }
            set { aimPosition = value - Position; }
        }
        


        protected override void DrawProcess()
        {
            DrawManager.texWrHigher.Draw(mask, CameraManager.worldRenderState);
            hands.DrawHandlingProcess();
            hands.DrawAnimatorsProcess();
        }

        protected override void LogicProcess()
        {
            hands.InteractableListRefresh();
        }


        protected override void PhysicsProcess()
        {
            movement.MovementProcess();
        }

        public override void Dispose()
        {
            base.Dispose();

            GamemodeIngame ingm = gamemode as GamemodeIngame;
            ingm.entitys.Remove(this);
        }

    }
    
}
