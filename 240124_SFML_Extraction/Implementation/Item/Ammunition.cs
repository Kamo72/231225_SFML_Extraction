using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{

    #region [7.62x51mm NATO]
    internal class mm7p62x51_AP : Ammo
    {
        static mm7p62x51_AP()
        {
            AmmoStatus status = new AmmoStatus()
            {
                caliber = CaliberType.mm7p62x51,
                lethality = new AmmoStatus.Lethality()
                {
                    damage = 59f,
                    bleeding = 56f,
                    pierceLevel = 5.3f,
                    pellitCount = 1,
                },
                adjustment = new AmmoStatus.Adjustment()
                {
                    accuracyRatio = 1.25f,
                    recoilRatio = 1.15f,
                    speedRatio = 1,
                },
                tracer = new AmmoStatus.Tracer() 
                {
                    isTraced = false,
                    color = new Color(255, 123, 0, 255),
                    radius = 120f,
                },
            };

            AmmoLibrary.Set("7.62x51mm_AP", status);
        }
        public mm7p62x51_AP() : base("7.62x51mm_AP") { }
    }
    #endregion


}
