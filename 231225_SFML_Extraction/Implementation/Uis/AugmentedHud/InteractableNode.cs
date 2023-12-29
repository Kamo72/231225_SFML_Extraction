using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class InteractableNode : TextLabel
    {
        public InteractableNode(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, size)
        {
        }
        protected override void DrawProcess()
        {
            base.DrawProcess();
        }
        protected override void LogicProcess()
        {
            base.LogicProcess();
        }
    }
}
