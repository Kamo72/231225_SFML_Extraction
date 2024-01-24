using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class ConcreteBox : Concrete
    {
        public ConcreteBox(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, new Box(position, size))
        {
        }

        public override List<Vector2f> GetPoligon()
        {
            Vector2f size = ((Box)mask).Size;
            return new List<Vector2f>()
            {
                Position + new Vector2f(+size.X / 2f, +size.Y / 2f),
                Position + new Vector2f(+size.X / 2f, -size.Y / 2f),
                Position + new Vector2f(-size.Y / 2f, -size.Y / 2f),
                Position + new Vector2f(-size.X / 2f, +size.Y / 2f),
            };
        }
    }
}
