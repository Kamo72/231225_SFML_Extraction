using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;


using static _231109_SFML_Test.Storage;
using System.Windows.Forms;
using SFML.Graphics;

namespace _231109_SFML_Test
{
    internal class Humanoid : Entity
    {
        public Humanoid(Gamemode gamemode, Vector2f position) : base(gamemode, position, new Circle(position, 10f))
        { }

        public const float accel = 3000f;    //가속
        public float accelPer = 1.00f;      //가속 배율
        public const float friction = 8.0f;   //마찰

        //속도 벡터
        public Vector2f speed = Vector2fEx.Zero;
        //가속 벡터 (최대 1)
        public Vector2f moveDir = Vector2fEx.Zero;

        public float aimDistance = 100f;

        public Vector2f AimVector 
        {
            get { return new Vector2f(Direction, aimDistance); }
            set { 
                Direction = value.X;
                aimDistance = value.Y;
            }
        }
        public Vector2f AimPosition
        {
            get { return Position + (-Direction-90).ToRadian().ToVector() * aimDistance; }
            set {
                Vector2f aimPosRel = value - Position;
                Direction = aimPosRel.ToDirection();
                aimDistance = aimPosRel.Magnitude();
            }
        }




        public Inventory inventory;
        public class Inventory
        {
            public Humanoid master;

            #region [장착 슬롯]
            public class EquipSlot<T> where T : Equipable
            {
                public T item;
                public virtual bool DoEquipItem(T item)
                {
                    if (this.item == null)
                    {
                        this.item = item;
                        return true;
                    }
                    return false;
                }
                public bool UnEquipItem()
                {
                    if (this.item != null)
                    {
                        this.item = null;
                        return true;
                    }
                    return false;
                }
            }
            public class EquipWeaponSub : EquipSlot<Weapon>
            {
                public override bool DoEquipItem(Weapon item)
                {
                    if (item.size.X <= 2) { return true; }

                    return base.DoEquipItem(item);
                }
            }

            public EquipSlot<Weapon> weaponPrimary = new EquipSlot<Weapon>();
            public EquipSlot<Weapon> weaponSub = new EquipWeaponSub();

            public EquipSlot<Helmet> helmet = new EquipSlot<Helmet>();
            public EquipSlot<Weapon> weaponSecondary = new EquipSlot<Weapon>();
            public EquipSlot<Headgear> headgear = new EquipSlot<Headgear>();
            public EquipSlot<PlateCarrier> plateCarrier = new EquipSlot<PlateCarrier>();
            public EquipSlot<Backpack> backpack = new EquipSlot<Backpack>();

            public Storage pocket = new Storage(new Vector2i(5, 2));
            #endregion


            public Inventory(Humanoid master)
            {
                this.master = master;
            }

            //드랍된 아이템 줍기
            public bool TakeItem(Item item)
            {
                StorageNode? newPlace;

                //주머니 먼저 삽입
                newPlace = pocket.GetPosInsert(item);
                if (newPlace != null)
                {
                    if (item.onStorage != null)
                    {
                        item.onStorage.RemoveItem(item);
                    }
                    item.onStorage = pocket;
                    pocket.Insert((StorageNode)newPlace);
                    return true;
                }

                //가방에 삽입
                newPlace = backpack.item.storage.GetPosInsert(item);
                if (newPlace != null)
                {
                    if (item.onStorage != null)
                    {
                        item.onStorage.RemoveItem(item);
                    }
                    item.onStorage = backpack.item.storage;
                    backpack.item.storage.Insert((StorageNode)newPlace);
                    return true;
                }

                return false;
            }

            //빠른 장착
            public bool EquipItemQuick(Item item)
            {
                if (item is Weapon newWeapon)
                {
                    if (newWeapon.AbleSub())
                    {
                        if (weaponSub.DoEquipItem(newWeapon))
                        {
                            newWeapon.BeEquip(master);
                            return true;
                        }
                    }

                    if (newWeapon.AbleMain())
                    {
                        if (weaponPrimary.DoEquipItem(newWeapon))
                        {
                            newWeapon.BeEquip(master);
                            return true;
                        }
                        if (weaponSecondary.DoEquipItem(newWeapon))
                        {
                            newWeapon.BeEquip(master);
                            return true;
                        }
                    }
                }
                else if (item is Headgear newHeadgear)
                {
                    if (headgear.DoEquipItem(newHeadgear))
                    {
                        newHeadgear.BeEquip(master);
                        return true;
                    }
                }
                else if (item is Backpack newBackpack)
                {
                    if (backpack.DoEquipItem(newBackpack))
                    {
                        newBackpack.BeEquip(master);
                        return true;
                    }
                }
                else if (item is PlateCarrier newPlateCarrier)
                {
                    if (plateCarrier.DoEquipItem(newPlateCarrier))
                    {
                        newPlateCarrier.BeEquip(master);
                        return true;
                    }
                }
                else if (item is Helmet newHelmet)
                {
                    if (helmet.DoEquipItem(newHelmet))
                    {
                        newHelmet.BeEquip(master);
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("EquipItem - 해당 아이템은 장착할 수 없습니다.");
                    return false;
                }

                Console.WriteLine("EquipItem - 적절한 장착 위치를 찾지 못했습니다.");
                return false;
            }

            //슬롯 지정 장착
            public bool EquipItemTarget<T>(EquipSlot<T> slot, T item) where T : Equipable
            {
                if (slot.DoEquipItem(item))
                {
                    item.BeEquip(master);
                    return true;
                }
                return false;
            }

            //아이템 장착 해제
            public bool UnEquipItem<T>(EquipSlot<T> slot, bool doThrow) where T : Equipable
            {
                //해당 슬롯에 아이템이 없음.
                if (slot.item == null) { return false; }

                //인벤토리로
                if (!doThrow)
                {
                    bool isSuceed = TakeItem(slot.item);
                    if (!isSuceed) { return false; }
                }
                else //필드로
                {
                    ThrowItem(slot.item);
                }

                slot.UnEquipItem();
                slot.item.UnEquip();

                return true;
            }

            //해당 아이템 드랍하기
            public void ThrowItem(Item item)
            {
                if (item.onStorage != null)
                {
                    item.onStorage.RemoveItem(item);
                }

                //item.ControllerInitiate(master.position, master.Direction);
            }

        }




        protected override void DrawProcess()
        {
            DrawManager.texWrHigher.Draw(mask, CameraManager.worldRenderState);
        }

        protected override void LogicProcess()
        {

        }

        protected override void PhysicsProcess()
        {
            //마찰에 의한 감속
            double deltaTime = 1d / gamemode.logicFps;
            speed *= (float)(1d - friction * deltaTime);

            //이동에 의한 가속
            Vector2f accelVec = moveDir.Magnitude() > 1f? moveDir.Normalize() : moveDir;
            speed += accelVec * (float)(accel * accelPer * deltaTime);

            #region [벽에 의한 충돌]

            Circle maskC = (Circle)mask;
            Vector2f originPos = maskC.Position;
            Vector2f toMove = speed * (float)deltaTime;
            
            Action<Vector2f> collisionAct = (springVec) =>
            {
                maskC.Position = originPos;
                speed = springVec; //양축 속도를 조진다. 탄성 때문에 살짝 밀림
            };

            foreach (Structure structure in ((GamemodeIngame)gamemode).structures)
            {
                maskC.Position = originPos + toMove;

                //(x,y)충돌 없음.
                if (structure.mask.IsCollision(mask) == false) { /*Console.WriteLine("0,0");*/ continue; }

                Vector2f springVec = (Position - structure.Position).Normalize() * 20f;
                //Console.WriteLine(pullBack);

                //충돌이 있다는건 알았음. 이제 y축 충돌인지 x축 충돌인지 둘 다 인지 검사
                //(0, y)
                maskC.Position = originPos + new Vector2f(0f, toMove.Y);
                if (structure.mask.IsCollision(mask) == false)
                {
                    //x축 문제라는걸 유추 가능
                    //Console.WriteLine("x, 0");
                    collisionAct(new Vector2f(0f, springVec.Y));
                    continue;
                }

                //y축에 문제 있는 상태. x축에도 문제 있는지 확인하고 처리
                //(x, 0)
                maskC.Position = originPos + new Vector2f(toMove.X, 0f);
                if (structure.mask.IsCollision(mask) == false)
                {
                    //y축의 문제라는걸 유추 가능.
                    //Console.WriteLine("0, y");
                    collisionAct(new Vector2f(springVec.X, 0f));
                    continue;
                }

                //x, y축 모두의 문제
                //(x, y)
                //Console.WriteLine("x, y");
                collisionAct(springVec);
                continue;

            }
            #endregion

            //속도에 의한 변위
            Position += speed * (float)deltaTime;

        }
    }
    
}
