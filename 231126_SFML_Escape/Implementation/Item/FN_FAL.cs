using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            InitTopParts(new Vector2i(200, 100), new Texture[]
            {
                ResourceManager.textures["FN_FAL_body_handle"],
                ResourceManager.textures["FN_FAL_body_upper"],
                ResourceManager.textures["FN_FAL_body_middle"],
                ResourceManager.textures["FN_FAL_body_lower"],
                ResourceManager.textures["FN_FAL_body_grip"],
            });

            topPartsBolt = new RectangleShape((Vector2f)topSpriteSize);
            topPartsBolt.Texture = ResourceManager.textures["FN_FAL_body_bolt"];
            topPartsBolt.Origin = new Vector2f(50, 50);


            //테스트
            magazineAttached = new FN_FAL_MAG10();
        }

        RectangleShape topPartsBolt;
        Magazine magazineAttached;


        public override void DrawSideSprite(RenderTexture texture, Vector2f position, float rotation, RenderStates renderStates)
        { 
            base.DrawSideSprite(texture, position, rotation, renderStates);
        }

        public override void DrawTopSprite(RenderTexture texture, Vector2f position, Vector2f rotation, float direction, float scaleRatio, RenderStates renderStates)
        {
            float depthAdjusted = -1f;

            float time = VideoManager.GetTimeTotal();
            rotation = new Vector2f((float)Math.Cos(time), (float)Math.Sin(time)) * 20f;
            //rotation = new Vector2f(20f, 20f);

            if (magazineAttached != null)
            {
                //magazineAttached.DrawTopSprite(texture, position + (direction / 5.8f).ToVector() * 100f, rotation, direction, renderStates, 3.5f);
                magazineAttached.DrawTopSprite(texture, position + (direction).ToRadian().ToVector() * 10f * scaleRatio, rotation, direction, renderStates, 2.5f, scaleRatio);
            }


            for (int i = topParts.Length-1; i >= 0; i--)
            {
                float depth = i - (topParts.Length / 2f) + depthAdjusted;

                RectangleShape shape = topParts[i];
                DrawTopSpritePart(texture, shape, position, rotation, direction, renderStates, depth, scaleRatio);

                if (i == 2)
                    DrawTopSpritePart(texture, topPartsBolt, position, rotation, direction, renderStates, depth, scaleRatio);
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
            InitTopParts(new Vector2i(45,45), new Texture[]
            {
                ResourceManager.textures["FN_FAL_MAG10_0"],
                ResourceManager.textures["FN_FAL_MAG10_1"],
                ResourceManager.textures["FN_FAL_MAG10_2"],
                ResourceManager.textures["FN_FAL_MAG10_3"],
            });
        }


        //public override void DrawTopSprite(RenderTexture texture, Vector2f position, Vector2f rotation, float direction, RenderStates renderStates, float depthAdjusted)
        //{
        //    base.DrawTopSprite(texture, position, rotation, direction, renderStates, depthAdjusted);
        //}

    }

    internal class FN_FAL_MAG20 : Magazine
    {
        static FN_FAL_MAG20()
        {
            MagazineStatus status = new MagazineStatus()
            {
                ammoSize = 20,
                whiteList = new List<CaliberType>()
                {
                    CaliberType.mm7p62x51,
                },
                adjusts = new List<WeaponAdjust>()
                {
                },
            };

            MagazineLibrary.Set("FN_FAL_MAG20", status);
        }
        public FN_FAL_MAG20() : base("FN_FAL_MAG20")
        {

        }
    }
}
