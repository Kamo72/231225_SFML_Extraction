using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class FN_FAL : Weapon
    {
        static FN_FAL()
        {
            string adsName = "IronSight_FN_FAL";

            Player.AdsData.adsLibrary[adsName] = new Player.AdsData
            {
                adsType = Player.AdsData.AdsType.IRON_SIGHT,
                sightMask = "",
                sightSprite = "",
                depthSpritePair = new Dictionary<int, string>
                {
                    { 70, "IronSight_FN_FAL_far" },
                    { 100, "IronSight_FN_FAL_close" },
                },
            };

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
                    adsTime = 0.200f,
                    reloadTime = (0.710f, 0.950f, 0.380f),
                    sprintTime = 0.110f,
                    swapTime = 0.250f,
                },
                aimDt = new WeaponStatus.AimData()
                {
                    hip = new WeaponStatus.AimData.HipData()
                    {
                        stance = new WeaponStatus.AimData.HipData.HipStancelData()
                        {
                            recovery = 4.50f,
                            accuracy = 23.00f,
                            accuracyAdjust = (crounch: 0.62f, walk: 1.95f),
                        },
                        recoil = new WeaponStatus.AimData.HipData.HipRecoilData()
                        {
                            recovery = 12.00f,
                            recoveryAdjust = (crounch: 1.74f, walk: 0.70f),
                            strength = 39.00f,
                        },
                        traggingSpeed = 18.00f,
                    },
                    ads = new WeaponStatus.AimData.AdsData()
                    {
                        stance = new WeaponStatus.AimData.AdsData.AdsStancelData()
                        {
                            accuracy = 40.00f,
                            accuracyAdjust = (crounch: 0.45f, walk: 1.67f),
                        },
                        recoil = new WeaponStatus.AimData.AdsData.AdsRecoilData()
                        {
                            fix = new Vector2f(21.0f, -75.0f),
                            random = new Vector2f(65.5f, 55.0f),
                            recovery = 28.50f,
                            strengthAdjust = (crounch: 0.48f, walk: 1.22f),
                        },
                        moa = 1f,
                        adsName = adsName,
                    },
                },
                moveDt = new WeaponStatus.MovementData()
                {
                    speed = 1.00f,
                    speedAdjust = (crounch: 1.00f, ads: 1.00f, sprint: 1.00f),
                },
                detailDt = new WeaponStatus.DetailData()
                {
                    chamberSize = 1,
                    magazineWhiteList = new List<Type>
                    {
                        typeof(FN_FAL_MAG10),
                        typeof(FN_FAL_MAG20),
                    },
                    muzzleVelocity = 2080f,
                    roundPerMinute = 700f,
                    effectiveRange = 750f,
                    barrelLength = 50f,
                }, 
            };

            WeaponLibrary.Set("FN_FAL", status);
        }   
        public FN_FAL() : base("FN_FAL")
        {
            SetupBasicData(
                "FN FAL",
                null,
                "7.62x51mm NATO탄을 사용하는 벨기에제 지정사수 소총입니다. 튼튼하고 다루기 쉬운데도 불구하고 강력한 전자동 사격 기능을 가지고 있습니다.",
                3.5f, new Vector2i(4, 2), Rarerity.COMMON, 20000f);
            InitHandableParts(new Vector2i(200, 100), new Texture[]
            {
                ResourceManager.textures["FN_FAL_chargingHandle"],
                ResourceManager.textures["FN_FAL_handGuard_dsArms"],
                ResourceManager.textures["FN_FAL_muzzle_Israeli"],
                ResourceManager.textures["FN_FAL_barrel_533mm"],
                ResourceManager.textures["FN_FAL_stock_basic"],
                ResourceManager.textures["FN_FAL_pistolGrip_basic"],
                ResourceManager.textures["FN_FAL_body"],
                ResourceManager.textures["FN_FAL_bolt"],

            });

            specialPos =
            (
                magPos: new Vector2f(13.5f, -0.5f),
                muzzlePos: new Vector2f(105.0f, -7.5f),
                ejectPos: new Vector2f(13.5f, +0.5f),
                pistolPos: new Vector2f(-5.5f, +5.0f),
                secGripPos: new Vector2f(30.5f, -3.0f),
                boltPos: new Vector2f(14.5f, +0.5f)
            );

            //테스트
            magazineAttached = new FN_FAL_MAG10(typeof(mm7p39x51_AP));
        }

        public override void DrawHandable(RenderTexture texture, Vector2f position, float direction, Vector2f scaleRatio, RenderStates renderStates)
        {
            //if (magazineAttached != null)
            //{
            //    Vector2f magazinePos =
            //        specialPos.magPos.X * (direction).ToRadian().ToVector() +
            //        specialPos.magPos.Y * (direction-90f).ToRadian().ToVector() * (scaleRatio.Y < 0 ? 1f : -1f);

            //    //magazineAttached.DrawHandable(texture, position + (direction / 5.8f).ToVector() * 100f, rotation, direction, renderStates, 3.5f);
            //    magazineAttached.DrawHandable(texture,
            //        position + new Vector2f(magazinePos.X * Math.Abs(scaleRatio.X), magazinePos.Y * Math.Abs(scaleRatio.Y)),
            //        direction, renderStates, scaleRatio);
            //}

            for (int i = topParts.Length-1; i >= 0; i--)
            {
                RectangleShape shape = topParts[i];
                DrawHandablePart(texture, shape, position,  direction, scaleRatio, renderStates);
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
                        ws.timeDt.reloadTime.Item1  *= 0.85f;
                        ws.timeDt.reloadTime.Item2  *= 0.74f;
                        ws.timeDt.reloadTime.Item3  *= 0.92f;
                    }),
                    new WeaponAdjust("이동 속도 증가", WeaponAdjustType.PROS, (ws)=> ws.moveDt.speed *= 1.09f),
                    new WeaponAdjust("탄약 감소", WeaponAdjustType.CONS, (ws)=>{ }),
                },
            };
            MagazineLibrary.Set("FN_FAL_MAG10", status); 
        }
        
        public FN_FAL_MAG10() : this(null) { }
        public FN_FAL_MAG10(Type ammoType) : base("FN_FAL_MAG10", ammoType)
        {
            SetupBasicData("FN FAL 10발 들이 탄창", "FN_FAL_MAG10", "FN FAL의 10발 들이 탄창입니다.", 0.25f, new Vector2i(1, 1), Rarerity.COMMON, 2000);
            InitTopParts(new Vector2i(45, 45), new Texture[]
            {
                ResourceManager.textures["FN_FAL_MAG10"],
            });
        }
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
            MagazineLibrary.Set("FN_FAL_MAG20", status);
        }
        
        public FN_FAL_MAG20() : this(null) { }
        public FN_FAL_MAG20(Type ammoType) : base("FN_FAL_MAG20", ammoType)
        {
            SetupBasicData("FN FAL 20발 들이 탄창", "FN_FAL_MAG20", "FN FAL의 20발 들이 탄창입니다.", 0.42f, new Vector2i(1, 2), Rarerity.UNCOMMON, 6400);
            InitTopParts(new Vector2i(45, 45), new Texture[]
            {
                ResourceManager.textures["FN_FAL_MAG20"],
            });
        }
    }
}
