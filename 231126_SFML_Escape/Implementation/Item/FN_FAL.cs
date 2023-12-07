using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class FN_FAL : Weapon
    {
        static FN_FAL() 
        {
            WeaponStatus status = new WeaponStatus()
            {
                typeDt = new WeaponStatus.TypeData()
                {
                    caliberType = CaliberType.mm7p62x51,
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
                    roundPerMinute = 700f,
                },
            };

            WeaponLibrary.Set("FN_FAL", status);
        }
        public FN_FAL() : base("FN_FAL")
        {
            SetupBasicData("시험형 무기", "시험형 무기입니다.", 3.5f, new Vector2i(3, 2), Rarerity.COMMON, 20000f);
            InitTopParts(new Vector2i(200, 75), new Texture[]
            {
                ResourceManager.textures["FN_FAL_body_handle"],
                ResourceManager.textures["FN_FAL_body_upper"],
                ResourceManager.textures["FN_FAL_body_middle"],
                ResourceManager.textures["FN_FAL_body_lower"],
                ResourceManager.textures["FN_FAL_body_grip"],
            });
        }



        public override RenderTexture GetSideSprite()
        {
            RenderTexture wTexture = new RenderTexture(200, 200);
            /* TO DRAW */

            /* TO DRAW */
            return wTexture;
        }
        
        public override RenderTexture GetTopSprite()
        {
            Vector2i size = new Vector2i(200,75);
            RenderTexture wTexture = new RenderTexture((uint)size.X, (uint)size.Y);
            /* TO DRAW */

            float time = VideoManager.GetTimeTotal();
            Vector2f rot = new Vector2f((float)Math.Cos(time) * 10f, (float)Math.Sin(time) * 10f);
            float depth = 0f;

            //wTexture.Draw(ResourceManager.textures["FN_FAL_body_grip"])
            

            /* TO DRAW */
            return wTexture;
        }
    }
}
