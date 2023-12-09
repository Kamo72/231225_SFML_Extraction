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
    internal abstract class Weapon : Equipable, IEquipable
    {
        static Weapon() 
        {
        }
        public Weapon(string weaponCode)
        {
            //기존 스텟 가져오기
            this.weaponCode = weaponCode;
            status = statusOrigin;


        }

        public WeaponStatus statusOrigin { get { return WeaponLibrary.Get(weaponCode); } }
        public WeaponStatus status;
        string weaponCode;

        #region [탑뷰 스프라이트]
        //인게임 총기 스프라이트를 생성형으로 반환
        public virtual void DrawTopSprite(RenderTexture texture, Vector2f position, Vector2f rotation, RenderStates renderStates) 
        {
            float rotRatio = 0.04f;
            float depth;

            for (int i = 0; i < topParts.Length; i++)
            {
                RectangleShape shape = topParts[i];
                depth = i - topParts.Length / 2f;
                shape.Scale = new Vector2f(
                    (float)Math.Cos(rotation.X.ToRadian()),
                    (float)Math.Cos(rotation.Y.ToRadian())
                    );
                shape.Position = position + depth * rotation * rotRatio;

                texture.Draw(shape, renderStates);
            }
        }
        public void DrawTopSprite(RenderTexture texture, Vector2f position, Vector2f rotation) { DrawTopSprite(texture, position, rotation, RenderStates.Default); }
        
        //탑뷰 드로우를 위한 Rects
        protected RectangleShape[] topParts;
        public Vector2i topSpriteSize;

        protected void InitTopParts(Vector2i topSpriteSize, params Texture[] textures)
        {
            this.topSpriteSize = topSpriteSize;

            RectangleShape[] rects = new RectangleShape[textures.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                rects[i] = new RectangleShape((Vector2f)topSpriteSize);
                rects[i].Texture = textures[i];
            }

            topParts = rects;
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

    internal abstract class Magazine 
    {
        public Magazine(string magazineCode) 
        {
            this.magazineCode = magazineCode;
            status = statusOrigin;
            ammoStack = new Stack<Ammo> { };
        }
        public Magazine(string magazineCode, Type ammoType) : this(magazineCode)
        {
            //입력한 탄 유형으로 탄약 채우기
            bool isAmmo = TypeEx.IsSubclassOfRawGeneric(ammoType, typeof(Ammo));

            if (isAmmo == false) return;

            while (true)
            {
                try
                {
                    AmmoPush(Activator.CreateInstance(ammoType) as Ammo);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + e.StackTrace);
                    break;
                }
            }
        }

        string magazineCode;
        public MagazineStatus statusOrigin { get { return MagazineLibrary.Get(magazineCode); } }
        public MagazineStatus status;

        Stack<Ammo> ammoStack; //탄창 내 탄약 <스택>

        public bool AmmoPush(Ammo ammo)
        {
            try
            {
                //예외
                if (status.whiteList.Contains(ammo.caliber) == false) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "호환되지 않는 탄종");
                if (ammoStack.Count >= status.ammoSize) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "탄창이 가득 찼음");
                if (ammo.stackNow <= 0) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "탄약의 Stack값이 정상적이지 않음");

                //처리
                ammo.stackNow--;
                Ammo splitted = Activator.CreateInstance(ammo.GetType()) as Ammo;
                ammoStack.Push(splitted);

                return true;
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.ToString() + "\n\n" +e.StackTrace);
                return false;
            }
        }
        public Ammo AmmoPop()
        {
            if(ammoStack.Count <= 0) return null;

            return ammoStack.Pop();
        }

    }

    internal abstract class Ammo : Item, IStackable
    {
        public Ammo(string ammoCode)
        {
            this.ammoCode = ammoCode;
        }

        public string ammoCode;


        public CaliberType caliber; //구경


        //IStackable
        public int stackNow { get; set; }
        public int stackMax { get; set; }
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
    internal static class MagazineLibrary
    {
        static MagazineLibrary() 
        {
            magazineLib = new Dictionary<string, MagazineStatus>();
        }

        static Dictionary<string, MagazineStatus> magazineLib;

        public static MagazineStatus Get(string magazineName)
        {
            return magazineLib[magazineName];
        }
        public static void Set(string magazineName, MagazineStatus magazineStatus)
        {
            if (magazineLib.ContainsKey(magazineName)) throw new Exception("magazineLib - 중복된 키 삽입!");
            magazineLib.Add(magazineName, magazineStatus);
        }
    }
    internal static class AmmoLibrary
    {
        static AmmoLibrary()
        {
            ammoLib = new Dictionary<string, AmmoStatus>();
        }

        static Dictionary<string, AmmoStatus> ammoLib;

        public static AmmoStatus Get(string magazineName)
        {
            return ammoLib[magazineName];
        }
        public static void Set(string magazineName, AmmoStatus magazineStatus)
        {
            if (ammoLib.ContainsKey(magazineName)) throw new Exception("ammoLib - 중복된 키 삽입!");
            ammoLib.Add(magazineName, magazineStatus);
        }
    }
    #endregion


}
