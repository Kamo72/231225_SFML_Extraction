using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{

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
            bool isAmmo = TypeEx.IsChildByParent(ammoType, typeof(Ammo));

            if (isAmmo == false) return;

            try
            {
                while (true)
                {
                    Ammo ammo = Activator.CreateInstance(ammoType) as Ammo;
                    ammo.stackNow = 9999;
                    AmmoPush(ammo);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }

        string magazineCode;
        public MagazineStatus statusOrigin { get { return MagazineLibrary.Get(magazineCode); } }
        public MagazineStatus status;

        #region [탄 관리]
        Stack<Ammo> ammoStack; //탄창 내 탄약 <스택>

        public bool AmmoPush(Ammo ammo)
        {
            //예외
            if (status.whiteList.Contains(ammo.status.caliber) == false) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "호환되지 않는 탄종" + $"{ammo.status.caliber.ToString()} != {status.whiteList.ToArray()}");
            if (ammoStack.Count >= status.ammoSize) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "탄창이 가득 찼음");
            if (ammo.stackNow <= 0) throw new Exception("Magazine - AmmoPush - 불가능한 작업 : " + "탄약의 Stack값이 정상적이지 않음");

            //처리
            ammo.stackNow--;
            Ammo splitted = Activator.CreateInstance(ammo.GetType()) as Ammo;
            ammoStack.Push(splitted);

            return true;
        }
        public Ammo AmmoPop()
        {
            if (ammoStack.Count <= 0) return null;

            return ammoStack.Pop();
        }
        public Ammo AmmoPeek() 
        {
            if (ammoStack.Count <= 0) return null;

            return ammoStack.Peek();
        }
        #endregion

        #region [탑뷰 드로우 관리]

        protected void InitTopParts(Vector2i topSpriteSize, params Texture[] textures)
        {
            this.topSpriteSize = topSpriteSize;

            RectangleShape[] rects = new RectangleShape[textures.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                rects[i] = new RectangleShape((Vector2f)topSpriteSize);
                rects[i].Texture = textures[i];
                rects[i].Origin = (Vector2f)topSpriteSize/2;
            }

            topParts = rects;
        }

        //탑뷰 드로우를 위한 Rects
        protected RectangleShape[] topParts;
        public Vector2i topSpriteSize;

        //인게임 총기 스프라이트를 생성형으로 반환
        public virtual void DrawHandable(RenderTexture texture, Vector2f position, float direction, RenderStates renderStates, Vector2f scaleRatio)
        {
            for (int i = topParts.Length - 1; i >= 0; i--)
            {

                RectangleShape shape = topParts[i];
                DrawHandablePart(texture, shape, position, direction, renderStates, scaleRatio);
            }
        }
        public void DrawHandable(RenderTexture texture, Vector2f position, float direction, Vector2f scaleRatio) { DrawHandable(texture, position, direction, RenderStates.Default, scaleRatio); }

        protected void DrawHandablePart(RenderTexture texture, RectangleShape shape, Vector2f position, float direction, RenderStates renderStates, Vector2f scaleRatio)
        {
            shape.Scale = scaleRatio;
            shape.Position = position;
            shape.Rotation = direction;

            texture.Draw(shape, renderStates);
        }

        #endregion
    }

    internal abstract class Ammo : Item, IStackable
    {
        public Ammo(string ammoCode)
        {
            this.ammoCode = ammoCode;
            status = AmmoLibrary.Get(ammoCode);
        }

        public string ammoCode;

        public AmmoStatus status;

        //IStackable
        public int stackNow { get; set; }
        public int stackMax { get; set; }
    }


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
            public float damage;          //피해량
            public float pierceLevel;   //관통계수
            public float bleeding;   //출혈 발생
            public int pellitCount;   //펠릿 갯수
        }
        public Lethality lethality;

        public struct Adjustment
        {
            public float accuracyRatio; //정확도 배율
            public float recoilRatio;   //반동 배율
            public float speedRatio;    //탄속 배율
        }
        public Adjustment adjustment;

        public struct Tracer 
        {
            public bool isTraced;  //예광탄 여부
            public float radius;   //예광탄 크기
            public Color color;   //예광탄 색
        }
        public Tracer tracer;
    }
    #endregion

    #region [탄창&탄약 데이터셋 제공자]
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
