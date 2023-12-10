using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{

    #region [7.39x51mm NATO]
    internal class mm7p39x51_AP : Ammo
    {
        static mm7p39x51_AP()
        {
            AmmoStatus status = new AmmoStatus()
            {
                caliber = CaliberType.mm7p62x51,
                lethality = new AmmoStatus.Lethality()
                {
                    damage = 35,
                    pierceLevel = 5.3f,
                    pellitCount = 1,
                },
                adjustment = new AmmoStatus.Adjustment()
                {
                    accuracyRatio = 1.25f,
                    recoilRatio = 1.15f,
                    speedRatio = 1,
                },
            };

            AmmoLibrary.Set("7.39x51mm_AP", status);
        }
        public mm7p39x51_AP() : base("7.39x51mm_AP")
        {
        }
    }
    #endregion


}
