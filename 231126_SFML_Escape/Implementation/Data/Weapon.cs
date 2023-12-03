using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class Weapon : Equipable
    {


        //주무기 장착 조건, 보조무기 장착 조건
        public bool AbleSub()
        {
            if (size.X <= 3 && size.Y <= 1)
                return true;

            return false;
        }
        public bool AbleMain()
        {
            if (size.X >= 3 || size.Y >= 2)
                return true;

            return false;
        }

    }
}
