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
    }
}
