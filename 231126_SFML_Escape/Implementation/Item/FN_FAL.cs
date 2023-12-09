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
                    magazineWhiteList = new List<Type> 
                    {
                        typeof(FN_FAL),
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

        public override void DrawSideSprite(RenderTexture texture, Vector2f position, float rotation, RenderStates renderStates)
        { 
            base.DrawSideSprite(texture, position, rotation, renderStates);
        }
        
        public override void DrawTopSprite(RenderTexture texture, Vector2f position, Vector2f rotation, RenderStates renderStates)
        {
            //Vector2i size = new Vector2i(200, 75);
            float rotRatio = 0.04f;
            float depth;
            float depthAdjusted = -1f;
            for (int i = 0; i < topParts.Length; i++)
            {
                RectangleShape shape = topParts[i];
                depth = i - (topParts.Length / 2f) + depthAdjusted;
                shape.Scale = new Vector2f(
                    (float)Math.Cos(rotation.X.ToRadian()),
                    (float)Math.Cos(rotation.Y.ToRadian())
                    ) * 10f;
                shape.Position = position + depth * rotation * rotRatio;

                texture.Draw(shape, renderStates);
            }
        }
    }

    internal class FN_FAL_MAG10 : Magazine
    {
        static FN_FAL_MAG10() 
        {
            MagazineStatus status = new MagazineStatus()
            {
                ammoSize = 10,
                whiteList = new List<CaliberType>()
                {
                    CaliberType.mm7p62x51,
                },
                adjusts = new List<WeaponAdjust>()
                {
                    new WeaponAdjust("조준 시간 감소", WeaponAdjustType.PROS, (ws)=> ws.timeDt.adsTime *= 0.81f),
                    new WeaponAdjust("재장전 시간 감소", WeaponAdjustType.PROS,
                    (ws)=> {
                        ws.timeDt.reloadTime[0]  *= 0.85f;
                        ws.timeDt.reloadTime[1]  *= 0.74f;
                        ws.timeDt.reloadTime[2]  *= 0.92f;
                    }),
                    new WeaponAdjust("이동 속도 증가", WeaponAdjustType.PROS, (ws)=> ws.moveDt.basicRatio *= 1.09f),
                    new WeaponAdjust("탄약 감소", WeaponAdjustType.CONS, (ws)=>{ }),
                },
            };

            MagazineLibrary.Set("FN_FAL_MAG10", status); 
        }
        public FN_FAL_MAG10() : base("FN_FAL_MAG10")
        {

        }
    }

}
