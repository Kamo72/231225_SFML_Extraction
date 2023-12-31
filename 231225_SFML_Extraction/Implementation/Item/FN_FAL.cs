﻿using SFML.Graphics;
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
                    moa = 1.80f,
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
                        typeof(FN_FAL_MAG10),
                        typeof(FN_FAL_MAG20),
                    },
                    muzzleVelocity = 2080f,
                    roundPerMinute = 700f,
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
                3.5f, new Vector2i(3, 2), Rarerity.COMMON, 20000f);
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

            specialPos = new Dictionary<string, Vector2f>
            {
                ["magPos"] = new Vector2f(13.5f, -0.5f),
                ["muzzlePos"] = new Vector2f(105.0f, -7.5f), 
                ["ejectPos"] = new Vector2f(13.5f, +0.5f), 
                ["pistolPos"] = new Vector2f(-2.5f, -6.5f), 
                ["secGripPos"] = new Vector2f(23.5f, -3.5f),
                ["boltPos"] = new Vector2f(14.5f, +0.5f),
            };

            //테스트
            magazineAttached = new FN_FAL_MAG10(typeof(mm7p39x51_AP));
        }

        public override void DrawHandable(RenderTexture texture, Vector2f position, float direction, Vector2f scaleRatio, RenderStates renderStates)
        {
            if (magazineAttached != null)
            {
                Vector2f magazinePos =
                    specialPos["magPos"].X * (direction).ToRadian().ToVector() +
                    specialPos["magPos"].Y * (direction-90f).ToRadian().ToVector() * (scaleRatio.Y < 0 ? 1f : -1f);

                //magazineAttached.DrawHandable(texture, position + (direction / 5.8f).ToVector() * 100f, rotation, direction, renderStates, 3.5f);
                magazineAttached.DrawHandable(texture,
                    position + new Vector2f(magazinePos.X * Math.Abs(scaleRatio.X), magazinePos.Y * Math.Abs(scaleRatio.Y)),
                    direction, renderStates, scaleRatio);
            }


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
                ResourceManager.textures["FN_FAL_MAG10"],
            });
        }
        public FN_FAL_MAG10(Type ammoType) : base("FN_FAL_MAG10", ammoType)
        {
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
        }
        public FN_FAL_MAG20() : base("FN_FAL_MAG20")
        {
            InitTopParts(new Vector2i(45, 45), new Texture[]
            {
                ResourceManager.textures["FN_FAL_MAG20"],
            });
        }
        public FN_FAL_MAG20(Type ammoType) : base("FN_FAL_MAG20", ammoType)
        {
            InitTopParts(new Vector2i(45, 45), new Texture[]
            {
                ResourceManager.textures["FN_FAL_MAG20"],
            });
        }
    }
}
