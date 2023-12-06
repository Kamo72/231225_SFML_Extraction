using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class TestWeapon : Weapon
    {
        static TestWeapon() 
        {
            WeaponStatus status = new WeaponStatus()
            {
                typeDt = new WeaponStatus.TypeData()
                {
                    caliberType = CaliberType.mm9x18,
                    mechanismType = MechanismType.CLOSED_BOLT,
                    magazineType = MagazineType.MAGAZINE,
                    boltLockerType = BoltLockerType.ACTIVATE,
                    selectorList = new List<SelectorType>()
                    {
                        SelectorType.SEMI,
                        SelectorType.AUTO,
                    },
                },
                timeDt = new WeaponStatus.TimeData()
                {
                    adsTime = 0.400f,
                    reloadTime = new float[3] { 0.710f, 0.950f, 0.380f },
                    sprintTime = 0.350f,
                    swapTime = 0.250f,
                },
                aimDt = new WeaponStatus.AimData()
                {
                    moa = 17.0f,
                    aimStable = 18f,
                    
                    hipAccuracy = 20f,
                    hipRecovery = 0.2f,

                    recoilFixed = new Vector2f(7.0f, 35.0f),
                    recoilRandom = new Vector2f(3.5f, 5.0f),
                },
                detailDt = new WeaponStatus.DetailData()
                {
                    chamberSize = 1,
                    magazineWhiteList = new List<object> 
                    {
                    
                    },
                    muzzleVelocity = 2080f,
                    roundPerMinute = 950f,
                },
            };

            WeaponLibrary.Set("TestWeapon", status);
        }
        public TestWeapon() : base("TestWeapon", "TestWeaponTop", new Vector2f(30f, 30f))
        {
            SetupBasicData("시험형 무기", "시험형 무기입니다.", 3.5f, new Vector2i(3, 2), Rarerity.COMMON, 20000f);
        }

        public override RenderTexture GetIngameSprite()
        {
            RenderTexture wTexture = new RenderTexture(100, 100);
            /* TO DRAW */
            return wTexture;
        }
    }
}
