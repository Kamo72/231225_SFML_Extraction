using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace _231109_SFML_Test
{
    internal abstract class Weapon : Equipable, IHandable, IDurable
    {
        static Weapon() { }
        public Weapon(string weaponCode)
        {
            //기존 스텟 가져오기
            this.weaponCode = weaponCode;
            status = statusOrigin;


            commandsReact = new Dictionary<InputManager.CommandType, Action<Humanoid.Hands, bool>>()
            {
                { InputManager.CommandType.FIRE, (hands, isTrue) => {
                    if(isTrue) CameraManager.GetShake(10f);
                } },
                { InputManager.CommandType.AIM, (hands, isTrue) => { } },
                //{ InputManager.CommandType.MAGAZINE_CHANGE, (hands, isTrue) => { } },
                //{ InputManager.CommandType.SPRINT, (hands, isTrue) => { } },
                //{ InputManager.CommandType.MELEE, (hand, isTrue) => { } }
            };
        }

        #region [변수 목록]
        //인터페이스 구현
        public float equipTime => status.timeDt.swapTime;
        public float equipValue { get; set; } = 0f;
        public float durableNow { get; set; } = 100f;
        public float durableMax { get; set; } = 0f;
        public bool zeroToDestruct { get; set; } = true;
        
        //허용된 조작 목록
        public Dictionary<InputManager.CommandType, Action<Humanoid.Hands, bool>> commandsReact { get; set; }
            

        //스테이터스
        public WeaponStatus statusOrigin { get { return WeaponLibrary.Get(weaponCode); } }
        public WeaponStatus status;
        string weaponCode;
        #endregion

        #region [탑뷰 스프라이트]
        protected void InitTopParts(Vector2i topSpriteSize, params Texture[] textures)
        {
            this.topSpriteSize = topSpriteSize;

            RectangleShape[] rects = new RectangleShape[textures.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                rects[i] = new RectangleShape((Vector2f)topSpriteSize);
                rects[i].Texture = textures[i];
                rects[i].Origin = new Vector2f(50, 50);
            }

            topParts = rects;
        }

        //탑뷰 드로우를 위한 Rects
        protected RectangleShape[] topParts;
        public Vector2i topSpriteSize;

        //인게임 총기 스프라이트를 생성형으로 반환
        public virtual void DrawTopSprite(RenderTexture texture, Vector2f position, Vector2f rotation, float direction, float scaleRatio, RenderStates renderStates)
        {
            float depth;
            float depthAdjusted = -1f;

            for (int i = topParts.Length - 1; i >= 0; i--)
            {
                depth = i - (topParts.Length / 2f) + depthAdjusted;

                RectangleShape shape = topParts[i];
                DrawTopSpritePart(texture, shape, position, rotation, direction, renderStates, depth, scaleRatio);
            }
        }
        public void DrawTopSprite(RenderTexture texture, Vector2f position, Vector2f rotation, float direction, float scaleRatio) { DrawTopSprite(texture, position, rotation, direction, scaleRatio, RenderStates.Default); }

        protected void DrawTopSpritePart(RenderTexture texture, RectangleShape shape, Vector2f position, Vector2f rotation, float direction, RenderStates renderStates, float depth, float scaleRatio)
        {
            float rotRatio = 0.07f * scaleRatio;

            shape.Scale = new Vector2f(
                (float)Math.Cos(rotation.X.ToRadian()),
                (float)Math.Cos(rotation.Y.ToRadian())
                ) * scaleRatio;
            shape.Position = position + depth * (rotation.ToDirection() + direction.ToRadian()).ToVector() * rotation.Magnitude() * rotRatio;
            shape.Rotation = direction;

            texture.Draw(shape, renderStates);
        }

        #endregion

        #region [사이드뷰 스프라이트]
        //인게임 총기 스프라이트를 생성형으로 반환
        public virtual void DrawSideSprite(RenderTexture texture, Vector2f position, float rotation, RenderStates renderStates)
        {
            sidePart.Position = position;
            sidePart.Rotation = rotation;
            texture.Draw(sidePart, renderStates);
        }

        protected RectangleShape sidePart;
        #endregion

        #region [주무기, 보조무기 장착 조건]
        //주무기 장착 조건, 보조무기 장착 조건
        public bool AbleSub()
        {
            if (size.X <= 3 && size.Y <= 1)
                return true;

            return false;
        }
        public bool AbleMain()
        {
            if (size.X >= 3 || size.Y >= 2)
                return true;

            return false;
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            foreach(RectangleShape r in topParts)
                r?.Dispose();
            sidePart?.Dispose();

        }
    }

    #region [총기 기본 정보]
    public enum CaliberType
    {
        p22,
        p45,
        mm9x18,
        mm9x19,
        mm5p56x45,
        mm7p62x51,
    }
    public enum SelectorType
    {
        SEMI,
        AUTO,
        BURST2,
        BURST3
    }
    public enum MechanismType
    {
        CLOSED_BOLT,    //폐쇄 노리쇠
        OPEN_BOLT,      //개방 노리쇠
        MANUAL_RELOAD,  //수동 약실 장전
        NONE,           //볼트 없음
    }
    public enum MagazineType
    {
        MAGAZINE,   //박스형 탄창 - 빠른 교체
        SYLINDER,   //탄창X 약실만 - 중절식, 리볼버 등 탄창 개념이 없음.
        TUBE,       //튜브형 탄창 - 전탄 소진 시, 약실 장전
        INTERNAL,   //내장형 탄창 - 장전 시 볼트 재낌    
    }
    public enum BoltLockerType
    {
        ACTIVATE,       //전탄 소진 시 노리쇠 후퇴 고정 ex M4A1
        ONLY_MANUAL,    //수동으로만 노리쇠 후퇴 고정 ex MP5
        NONE,           //노리쇠 후퇴 고정 불가 ex AK47
    }

    public struct WeaponStatus
    {
        public WeaponStatus(TypeData typeData, AimData aimData, TimeData timeData, MovementData movementData, DetailData detailData)
        { 
            this.typeDt = typeData;
            this.aimDt = aimData;
            this.timeDt = timeData;
            this.moveDt = movementData;
            this.detailDt = detailData;
        }


        //무장 유형 정보
        public TypeData typeDt;
        public struct TypeData 
        {
            public TypeData(MechanismType mechanismType, MagazineType magazineType, BoltLockerType boltLockerType, List<SelectorType> selectorList, CaliberType caliberType) 
            {
                this.mechanismType = mechanismType;
                this.magazineType = magazineType;
                this.boltLockerType = boltLockerType;
                this.selectorList = selectorList;
                this.caliberType = caliberType;
            }

            public MechanismType mechanismType;     //작동 방시 
            public MagazineType magazineType;       //탄창 방식
            public BoltLockerType boltLockerType;   //노리쇠멈치 방식
            public List<SelectorType> selectorList; //조정간
            public CaliberType caliberType;         //구경
        }

        //조준 정보
        public AimData aimDt;
        public struct AimData 
        {
            public AimData(float moa, float aimStable, float hipAccuracy, float hipRecovery, Vector2f recoilFixed, Vector2f recoilRandom) 
            {
                this.moa = moa;
                this.aimStable = aimStable;
                this.hipAccuracy = hipAccuracy;
                this.hipRecovery = hipRecovery;

                this.recoilFixed = recoilFixed;
                this.recoilRandom = recoilRandom;
            }

            public float moa;       //최소 조준점 탄퍼짐 (거리 1000기준).
            public float aimStable; //조준 안정도 조준점 흐트러짐 크기 및 흐트러짐 속도
            public float hipAccuracy;   //지향 사격 최소 조준점 흐트러짐 크기
            public float hipRecovery;   //지향 사격 회복 속도

            public Vector2f recoilFixed;    //고정 반동
            public Vector2f recoilRandom;   //랜덤 반동
        }

        //행동 소요 시간 정보 
        public TimeData timeDt;
        public struct TimeData {
            public TimeData(float adsTime, float sprintTime, float[] reloadTime, float swapTime)
            {
                this.adsTime = adsTime;
                this.sprintTime = sprintTime;
                this.reloadTime = reloadTime;
                this.swapTime = swapTime;
            }

            public float adsTime;       //조준 속도
            public float sprintTime;    //질주 후 사격 전환 속도
            public float[] reloadTime;  //재장전 속도 - 박스(분리 / 결합 (장전)) - 실린더(사출 / 장전 / 결합) - 내부(볼트 재낌 /장전) - (장전 준비 / (약실 장전) / 튜브 장전 )
            public float swapTime;      //무기 교체 속도
        }

        //이동 속도 정보
        public MovementData moveDt;
        public struct MovementData 
        {
            public MovementData(float basicRatio, float sprintRatio, float adsRatio) 
            {
                this.basicRatio = basicRatio;
                this.sprintRatio = sprintRatio;
                this.adsRatio = adsRatio;
            }

            public float basicRatio;
            public float sprintRatio;
            public float adsRatio;
        }

        //상세 정보
        public DetailData detailDt;
        public struct DetailData 
        {
            public DetailData(float roundPerMinute, int chamberSize, List<Type> magazineWhiteList, float muzzleVelocity) 
            {
                this.roundPerMinute = roundPerMinute;
                this.chamberSize = chamberSize;
                this.magazineWhiteList = magazineWhiteList;
                this.muzzleVelocity = muzzleVelocity;
            }

            public float RoundDelay { get { return 60f / roundPerMinute; } }
            public float roundPerMinute;
            public int chamberSize; //약실 크기
            public List<Type> magazineWhiteList;  //장착 가능한 탄창리스트
            public float muzzleVelocity;    //총구 속도
        }


    }
    #endregion

    #region [총기 옵션 정보]

    public enum WeaponAdjustType
    {
        PROS,   //장
        CONS,   //단
        NONE,   //?
    }
    public struct WeaponAdjust
    {
        public WeaponAdjust(string description, WeaponAdjustType adjustType, Action<WeaponStatus> adjustFun)
        {
            this.description = description;
            this.adjustType = adjustType;
            this.adjustFun = adjustFun;
        }

        public string description;
        public WeaponAdjustType adjustType;
        public Action<WeaponStatus> adjustFun;
    }

    #endregion

    #region [탄창$탄약 기본 정보]
    internal struct MagazineStatus 
    {
        public MagazineStatus(int ammoSize, List<CaliberType> whiteList, List<WeaponAdjust> adjusts) 
        {
            this.ammoSize = ammoSize;
            this.whiteList = whiteList;
            this.adjusts = adjusts;
        }

        public int ammoSize; //탄창 크기
        public List<CaliberType> whiteList; //허용 탄종
        public List<WeaponAdjust> adjusts;  //스텟 보정

    }

    internal struct AmmoStatus
    {
        public CaliberType caliber; //구경

        public struct Lethality
        {
            public int damage;          //피해량
            public float pierceLevel;   //관통계수
            public int pellitCount;   //펠릿 갯수
        }
        public Lethality lethality;

        public struct Adjustment {
            public float accuracyRatio; //정확도 배율
            public float recoilRatio;   //반동 배율
            public float speedRatio;    //탄속 배율
        }
        public Adjustment adjustment;
    }
    #endregion

    #region [총기 데이터셋 제공자]
    internal static class WeaponLibrary
    {
        static WeaponLibrary() 
        {
            weaponLib = new Dictionary<string, WeaponStatus>();
            WeaponDataLoad();
        }

        static Dictionary<string, WeaponStatus> weaponLib;
        static void WeaponDataLoad() 
        {
            //정적 생성자를 불러오는 역할?
            FN_FAL fn_fal;
        }
        public static WeaponStatus Get(string weaponName)
        {
            return weaponLib[weaponName];
        }
        public static void Set(string weaponName, WeaponStatus weaponStatus)
        {
            if (weaponLib.ContainsKey(weaponName)) throw new Exception("weaponLib - 중복된 키 삽입!");
            weaponLib.Add(weaponName, weaponStatus);
        }

    }

    #endregion




}
