using SFML.Graphics;
using SFML.System;
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
        public mm7p62x51_AP() : base("7.62x51mm_AP")
        {
            SetupBasicData("7.62x51mm NATO M61",
                "Undefined",
                "150.5 그레인(9.8 g) 7.62×51mm NATO 철갑탄. 장갑을 관통하기 위해 만들어졌으며 우수한 관통력을 보여줍니다.", 0.02f, new Vector2i(1, 1), Rarerity.RARE, 1800f);
        }
    }


    internal class mm7p62x51_FRANGIBLE : Ammo
    {
        static mm7p62x51_FRANGIBLE()
        {
            AmmoStatus status = new AmmoStatus()
            {
                caliber = CaliberType.mm7p62x51,
                lethality = new AmmoStatus.Lethality()
                {
                    damage = 108f,
                    bleeding = 120f,
                    pierceLevel = 1.0f,
                    pellitCount = 1,
                },
                adjustment = new AmmoStatus.Adjustment()
                {
                    accuracyRatio = 1.00f,
                    recoilRatio = 0.95f,
                    speedRatio = 0.80f,
                },
                tracer = new AmmoStatus.Tracer()
                {
                    isTraced = false,
                    color = new Color(255, 123, 0, 255),
                    radius = 120f,
                },
            };

            AmmoLibrary.Set("7.62x51mm_FRANGIBLE", status);
        }
        public mm7p62x51_FRANGIBLE() : base("7.62x51mm_FRANGIBLE")
        {
            SetupBasicData("7.62x51mm NATO M160",
                "Undefined",
                " 108.5그레인(7.0g) 7.62×51mm NATO 파쇄성탄자. 탄두를 깨지기 쉽게 만들어져 관통 효과가 없지만, 목표물에 도달할 때 조각나 충격 지점에 큰 흔적을 남깁니다. 비 장갑 표적에 적합하며, 큰 데미지와 출혈을 일으킬 수 있습니다.",
                0.02f, new Vector2i(1, 1), Rarerity.UNCOMMON, 450f);
        }
    }
    #endregion


}
