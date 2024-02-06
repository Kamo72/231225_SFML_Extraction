using _231109_SFML_Test;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
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
                    adsTime = 0.340f,
                    reloadTime = (2.250f, 2.600f, 2.400f),
                    sprintTime = 0.220f,
                    swapTime = 0.860f,
                },
                aimDt = new WeaponStatus.AimData()
                {
                    hip = new WeaponStatus.AimData.HipData()
                    {
                        stance = new WeaponStatus.AimData.HipData.HipStancelData()
                        {
                            recovery = 4.50f,
                            accuracy = 220.00f,
                            accuracyAdjust = (crounch: 0.62f, walk: 1.95f),
                        },
                        recoil = new WeaponStatus.AimData.HipData.HipRecoilData()
                        {
                            recovery = 4.00f,
                            recoveryAdjust = (crounch: 1.74f, walk: 0.70f),
                            strength = 194.00f,
                        },
                        traggingSpeed = 12.00f,
                    },
                    ads = new WeaponStatus.AimData.AdsData()
                    {
                        stance = new WeaponStatus.AimData.AdsData.AdsStancelData()
                        {
                            accuracy = 95.00f,
                            accuracyAdjust = (crounch: 0.45f, walk: 1.67f),
                        },
                        recoil = new WeaponStatus.AimData.AdsData.AdsRecoilData()
                        {
                            fix = new Vector2f(27.0f, -112.0f),
                            random = new Vector2f(91.5f, 102.0f),
                            recovery = 8.50f,
                            strengthAdjust = (crounch: 0.48f, walk: 1.22f),
                        },
                        moa = 4.5f,
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
                    loudness = 10000f,
                }, 
                attachDt = new WeaponStatus.AttachData() 
                {
                    socketList = new List<AttachSocket> {
                        new AttachSocket( AttachmentType.BARREL,
                            new Vector2f(41f, -0.5f), true,
                            [
                                typeof(FN_FAL_Barrel_533mm),
                            ],
                            new FN_FAL_Barrel_533mm()
                        ),
                        new AttachSocket( AttachmentType.STOCK,
                            new Vector2f(-23f, 4f), true,
                            [
                                typeof(FN_FAL_Polymer_Stock),
                            ],
                            new FN_FAL_Polymer_Stock()
                        ),
                        new AttachSocket( AttachmentType.TOP_RECIEVER,
                            new Vector2f(6f, -1f), false,
                            [
                                typeof(FN_FAL_DuskCover_Basic),
                            ],
                            new FN_FAL_DuskCover_Basic()
                        ),
                        new AttachSocket( AttachmentType.PISTOL_GRIP,
                            new Vector2f(-4.5f, +7.5f), true,
                            [
                                typeof(FN_FAL_PistolGrip_ArBased),
                            ],
                            new FN_FAL_PistolGrip_ArBased()
                        ),
                    },
                },
            };

            WeaponLibrary.Set("FN_FAL", status);
        }   
        public FN_FAL() : base("FN_FAL")
        {
            SetupBasicData(
                "FN FAL",
                "Undefined",
                "7.62x51mm NATO탄을 사용하는 벨기에제 지정사수 소총입니다. 튼튼하고 다루기 쉬운데도 불구하고 강력한 전자동 사격 기능을 가지고 있습니다.",
                3.5f, new Vector2i(4, 2), Rarerity.COMMON, 20000f);
            InitHandableParts(new Vector2i(200, 100),
            [
                ResourceManager.textures["FN_FAL_chargingHandle"],
                ResourceManager.textures["FN_FAL_body"],
                ResourceManager.textures["FN_FAL_bolt"],

            ],
            [
                ResourceManager.textures["FN_FAL_chargingHandle"],
                ResourceManager.textures["FN_FAL_bolt"],
            ]);

            specialPos =
            (
                magPos: new Vector2f(13.5f, 1.5f),
                muzzlePos: new Vector2f(105.0f, -0f),
                ejectPos: new Vector2f(13.5f, -2.5f),
                pistolPos: new Vector2f(-5.5f, +5.0f),
                secGripPos: new Vector2f(30.5f, -3.0f),
                boltPos: new Vector2f(14.5f, -2.5f)
            );
            boltVec =
            (
                backwardVec: new Vector2f(-20.0f, 0.0f),
                lockVec: new Vector2f(0.0f, 0.0f)
            );

            //테스트
            magazineAttached = new FN_FAL_MAG10(typeof(mm7p62x51_AP));
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
                    new WeaponAdjust("조준 시간 감소", WeaponAdjustType.PROS,
                    ws => { ws.timeDt.adsTime *= 0.81f;  return ws; }),
                    new WeaponAdjust("재장전 시간 감소", WeaponAdjustType.PROS,
                    ws => {
                        ws.timeDt.reloadTime.Item2  *= 0.74f;
                        ws.timeDt.reloadTime.Item3  *= 0.92f;
                        ws.timeDt.reloadTime.Item1  *= 0.85f;
                        return ws;
                    }),
                    new WeaponAdjust("이동 속도 증가", WeaponAdjustType.PROS, ws => { ws.moveDt.speed *= 1.09f; return ws; }),
                    new WeaponAdjust("탄약 감소", WeaponAdjustType.CONS, ws => { return ws; }),
                },
            };
            MagazineLibrary.Set("FN_FAL_MAG10", status); 
        }
        
        public FN_FAL_MAG10() : this(null) { }
        public FN_FAL_MAG10(Type ammoType) : base("FN_FAL_MAG10", ammoType)
        {
            SetupBasicData("FN FAL 10발 들이 탄창", "FN_FAL_MAG10", "FN FAL의 10발 들이 탄창입니다.", 0.25f, new Vector2i(1, 1), Rarerity.COMMON, 2000);
            InitTopParts(new Vector2i(45, 45),
            [
                ResourceManager.textures["FN_FAL_MAG10"],
            ]);
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


    internal class FN_FAL_Barrel_533mm : Attachment, IAttachable
    {
        public FN_FAL_Barrel_533mm() : base(AttachmentType.BARREL, new List<WeaponAdjust>
        {
            //부착물 옵션
            new WeaponAdjust("총기 명중률 감소", WeaponAdjustType.CONS, ws => {ws.aimDt.ads.moa *= 1.10f; return ws; }),
            new WeaponAdjust("조준점 이동 속도 증가", WeaponAdjustType.PROS, ws => {ws.aimDt.hip.traggingSpeed *= 1.05f; return ws; }),
        })
        {
            //아이템 정보
            base.SetupBasicData(
                //아이템 이름
                "FN FAL 533mm 총열",
                //스프라이트 이름
                "FN_FAL_barrel_533mm",
                //설명
                "FN FAL이 기본적으로 사용하는 533mm 길이의 총열입니다. 7.51x51mm NATO 총알을 충분한 속도로 발사하는데 적합하며, 다루기 좋은 길이 덕분에 전반적으로 사용하기 좋습니다.",
                //무게
                0.670f,
                //아이템 크기
                new Vector2i(3, 1),
                //희귀도
                Rarerity.COMMON,
                //가격
                3700f
                );
            sizeAdjust = (0, 0, 0, 3);

            //부착물 슬롯들
            attachments = new List<AttachSocket>
            {
                new AttachSocket(AttachmentType.MUZZLE, new Vector2f(26f, 00f),
                    false,
                    new List<Type>()
                    {
                        typeof(FN_FAL_Flashhider_FnAmerica),
                    },
                    new FN_FAL_Flashhider_FnAmerica()
                ),
                new AttachSocket(AttachmentType.HANDGUARD, new Vector2f(0f, 0f),
                    true,
                    new List<Type>()
                    {
                        typeof(FN_FAL_HandGuard_DsArms),
                    },
                    new FN_FAL_HandGuard_DsArms()
                ),
            };

        }
        public List<AttachSocket> attachments { get; set; }

    }

    internal class FN_FAL_HandGuard_DsArms : Attachment, IAttachable
    {
        public FN_FAL_HandGuard_DsArms() : base(AttachmentType.BARREL, new List<WeaponAdjust>
        {
            //부착물 옵션
            new WeaponAdjust("총기 명중률 감소", WeaponAdjustType.CONS, ws => {ws.aimDt.ads.moa *= 1.10f; return ws; }),
            new WeaponAdjust("조준 속도 증가", WeaponAdjustType.PROS, ws => {ws.timeDt.adsTime *= 0.97f; return ws; }),
        })
        {
            //아이템 정보
            base.SetupBasicData(
                //아이템 이름
                "FN FAL DS Arms 총열덮개",
                //스프라이트 이름
                "FN_FAL_handGuard_dsArms",
                //설명
                "FN FAL의 533mm 총열에 맞는 크기의 총열 덮개입니다. 낮은 확장성이 발목을 잡지만, 튼튼하고 다루기 쉬워 아직까지도 널리 쓰이고 있습니다. 양각대를 부착해 고정 사수 역할을 수행 할 수도 있습니다.",
                //무게
                0.350f,
                //아이템 크기
                new Vector2i(2, 1),
                //희귀도
                Rarerity.COMMON,
                //가격
                2800f
                );

            //부착물 슬롯들
            attachments = new List<AttachSocket>
            {
                new AttachSocket(AttachmentType.FOREGRIP, new Vector2f(14f, -3f),
                    false,
                    null,
                    null
                ),
            };

        }
        public List<AttachSocket> attachments { get; set; }

    }

    internal class FN_FAL_Flashhider_FnAmerica : Attachment
    {
        public FN_FAL_Flashhider_FnAmerica() : base(AttachmentType.MUZZLE, new List<WeaponAdjust>
        {
            //부착물 옵션
            new WeaponAdjust("수직 반동 감소", WeaponAdjustType.PROS, ws => {ws.aimDt.ads.recoil.fix *= 0.850f; return ws; }),
            new WeaponAdjust("조준 속도 감소", WeaponAdjustType.CONS, ws => {ws.timeDt.adsTime *= 1.05f; return ws; }),
            new WeaponAdjust("조준점 이동 속도 감소", WeaponAdjustType.CONS, ws => {ws.timeDt.adsTime *= 0.94f; return ws; }),
            new WeaponAdjust("소음 감소", WeaponAdjustType.PROS, ws => {ws.detailDt.loudness *= 0.81f; return ws; }),
            new WeaponAdjust("지향 반동 감소", WeaponAdjustType.PROS, ws => {ws.aimDt.hip.recoil.strength *= 0.92f; return ws; }),
        })
        {
            //아이템 정보
            base.SetupBasicData(
                //아이템 이름
                "FN America FAL Flash Hider",
                //스프라이트 이름
                "FN_FAL_flashhider_fnAmerica",
                //설명
                "FN America사의 FN FAL 소염기입니다. 길쭉한 외향 때문에 총기가 길어진다는 단점이 있지만 총기 반동와 총구 화염을 효과적으로 줄여줍니다.",
                //무게
                0.080f,
                //아이템 크기
                new Vector2i(2, 1),
                //희귀도
                Rarerity.COMMON,
                //가격
                2500f
                );
            sizeAdjust = (0, 0, 0, 1);
        }

    }

    internal class FN_FAL_Polymer_Stock : Attachment
    {
        public FN_FAL_Polymer_Stock() : base(AttachmentType.STOCK, new List<WeaponAdjust>
        {
            //부착물 옵션
            new WeaponAdjust("반동 감소", WeaponAdjustType.PROS, ws =>
            {
                ws.aimDt.ads.recoil.fix *= 0.320f;
                ws.aimDt.ads.recoil.random *= 0.210f;
                ws.aimDt.hip.recoil.strength *= 0.385f;
                return ws;
            }),
            new WeaponAdjust("반동 회복 가속", WeaponAdjustType.PROS, ws =>
            {
                ws.aimDt.ads.recoil.recovery *= 2.400f;
                ws.aimDt.hip.recoil.recovery *= 1.800f;
                return ws;
            }),
            new WeaponAdjust("조준 안정 증가", WeaponAdjustType.PROS, ws => {ws.aimDt.ads.stance.accuracy *= 0.16f; return ws; }),
            new WeaponAdjust("이동 속도 증가", WeaponAdjustType.PROS, ws => {ws.moveDt.speed *= 1.07f; return ws; }),

            new WeaponAdjust("조준 속도 감소", WeaponAdjustType.CONS, ws => {ws.timeDt.adsTime *= 2.67f; return ws; }),
            new WeaponAdjust("조준점 이동 속도 감소", WeaponAdjustType.CONS, ws => {ws.timeDt.adsTime *= 0.59f; return ws; }),
        })
        {
            //아이템 정보
            base.SetupBasicData(
                //아이템 이름
                "FN FAL 폴리머 개머리판",
                //스프라이트 이름
                "FN_FAL_stock_basic",
                //설명
                "FN FAL의 고분자 폴리머 소재 개머리판입니다. 뭉특한 생김새에 걸맞게 튼튼하지만 가벼워 다루기 쉽습니다. 제거해도 사용은 가능하지만 권장되지 않습니다.",
                //무게
                1.200f,
                //아이템 크기
                new Vector2i(2, 1),
                //희귀도
                Rarerity.COMMON,
                //가격
                2200f
                );
            sizeAdjust = (0, 0, 1, 0);
        }
    }

    internal class FN_FAL_PistolGrip_ArBased : Attachment
    {
        public FN_FAL_PistolGrip_ArBased() : base(AttachmentType.PISTOL_GRIP, new List<WeaponAdjust>
        {
            //부착물 옵션
        })
        {
            //아이템 정보
            base.SetupBasicData(
                //아이템 이름
                "FN FAL AR기반 권총 손잡이",
                //스프라이트 이름
                "FN_FAL_pistolGrip_arBased",
                //설명
                "FN FAL 전용 폴리머 권총 손잡이입니다. 표면 처리 없이 단순한 권총 손잡이의 형태를 띄고 있습니다.",
                //무게
                0.100f,
                //아이템 크기
                new Vector2i(1, 1),
                //희귀도
                Rarerity.COMMON,
                //가격
                1100f
                );
            sizeAdjust = (0, 1, 0, 0);
        }
    }

    internal class FN_FAL_DuskCover_Basic : Attachment
    {
        public FN_FAL_DuskCover_Basic() : base(AttachmentType.TOP_RECIEVER, new List<WeaponAdjust>
        {
            //부착물 옵션
            new WeaponAdjust("조준 속도 증가", WeaponAdjustType.PROS, ws => {ws.timeDt.adsTime *= 0.98f; return ws; }),
        })
        {
            //아이템 정보
            base.SetupBasicData(
                //아이템 이름
                "FN FAL 먼지덮게",
                //스프라이트 이름
                "FN_FAL_dustCover_basic",
                //설명
                "FN FAL의 기본 먼지 덮개입니다. 이물질이 유입되는 것을 방지하고 작동중인 노리쇠가 사용자를 치는 것을 방지해줍니다.",
                //무게
                0.060f,
                //아이템 크기
                new Vector2i(2, 1),
                //희귀도
                Rarerity.COMMON,
                //가격
                700f
                );
        }
    }

}
