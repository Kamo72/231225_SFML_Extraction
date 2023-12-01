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
    internal class Player : Humanoid
    {
        public Player(Gamemode gamemode, Vector2f position) : base(gamemode, position)
        {

        }

        protected override void DrawProcess()
        {
            ((Circle)mask).Scale = new Vector2f(100f, 100f);
            DrawManager.texUiInterface.Draw(mask, CameraManager.worldRenderState);
        }

        protected override void LogicProcess()
        {
            CameraManager.targetPos = Position;
        }

        protected override void PhysicsProcess()
        {
            Console.WriteLine(Position);

            moveDir = Vector2fEx.Zero;
            if (Im.CommandCheck(Im.CommandType.MOVE_LEFT)) moveDir +=     (Direction + 180f).ToRadian().ToVector();
            if (Im.CommandCheck(Im.CommandType.MOVE_RIGHT)) moveDir +=    (Direction + 000f).ToRadian().ToVector();
            if (Im.CommandCheck(Im.CommandType.MOVE_FORWARD)) moveDir +=  (Direction + 270f).ToRadian().ToVector();
            if (Im.CommandCheck(Im.CommandType.MOVE_BACKWARD)) moveDir += (Direction + 090f).ToRadian().ToVector();

            base.PhysicsProcess();
        }


    }
}
