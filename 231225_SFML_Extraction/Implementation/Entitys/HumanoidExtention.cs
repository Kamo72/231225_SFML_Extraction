using SFML.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static _231109_SFML_Test.Humanoid;
using static _231109_SFML_Test.Storage;

namespace _231109_SFML_Test
{
    internal partial class Humanoid
    {

        public Inventory inventory;
        public class Inventory
        {
            public Humanoid master;

            public Inventory(Humanoid master)
            {
                this.master = master;
            }

            #region [장착 슬롯]
            public class EquipSlot
            {
                public EquipSlot(EquipmentType equipmentType)
                {
                    this.equipmentType = equipmentType;
                }

                public IEquipable item;
                public EquipmentType equipmentType;

                public virtual bool DoEquipItem(Item item)
                {
                    switch (equipmentType)
                    {
                        case EquipmentType.HEADGEAR: if (item is Headgear == false) return false; break;
                        case EquipmentType.WEAPON: if (item is Weapon == false) return false; break;
                        case EquipmentType.BACKPACK: if (item is Headgear == false) return false; break;
                        case EquipmentType.PLATE_CARRIER: if (item is Headgear == false) return false; break;
                        case EquipmentType.HELMET: if (item is Headgear == false) return false; break;
                    }

                    if (this.item == null)
                    {
                        this.item = (IEquipable)item;
                        return true;
                    }
                    return false;
                }
                public Item UnEquipItem()
                {
                    if (this.item != null)
                    {
                        Item item = (Item)this.item;
                        this.item = null;
                        return item;
                    }
                    return null;
                }
            }
            public class EquipSlotWeapon : EquipSlot
            {
                public EquipSlotWeapon(bool isMain) : base(EquipmentType.WEAPON) { this.isMain = isMain; }

                bool isMain;

                public override bool DoEquipItem(Item item)
                {
                    if (base.DoEquipItem(item) == false)
                        return false;

                    if (item is Weapon weapon)
                    {
                        if (weapon.AbleMain() != isMain)
                        {
                            return false;
                        }
                    }
                    else { return false; }

                    return true;
                }
            }

            public EquipSlot weaponPrimary = new EquipSlotWeapon(true);
            public EquipSlot weaponSecondary = new EquipSlotWeapon(true);
            public EquipSlot weaponSub = new EquipSlotWeapon(false);

            public EquipSlot helmet = new EquipSlot(EquipmentType.HELMET);
            public EquipSlot headgear = new EquipSlot(EquipmentType.HEADGEAR);
            public EquipSlot plateCarrier = new EquipSlot(EquipmentType.PLATE_CARRIER);
            public EquipSlot backpack = new EquipSlot(EquipmentType.BACKPACK);

            public Storage pocket = new Storage(new Vector2i(5, 2));
            #endregion

            #region [제공 함수]
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
                if (backpack.item is Backpack bp)
                {
                    newPlace = bp.storage.GetPosInsert(item);
                    if (newPlace != null)
                    {
                        if (item.onStorage != null)
                            item.onStorage.RemoveItem(item);

                        item.onStorage = bp.storage;
                        bp.storage.Insert((StorageNode)newPlace);
                        return true;
                    }
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
            public bool EquipItemTarget(EquipSlot slot, IEquipable item)
            {
                if (slot.DoEquipItem((Item)item))
                {
                    item.UnEquip(master);
                    return true;
                }
                return false;
            }

            //아이템 장착 해제
            public bool UnEquipItem(EquipSlot slot, bool doThrow)
            {
                //해당 슬롯에 아이템이 없음.
                if (slot.item == null) { return false; }

                //인벤토리로
                if (!doThrow)
                {
                    bool isSuceed = TakeItem((Item)slot.item);
                    if (!isSuceed) { return false; }
                }
                else //필드로
                {
                    ThrowItem((Item)slot.item);
                }

                slot.UnEquipItem();
                slot.item.UnEquip(master);

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
            #endregion
        }

        public Hands hands;
        public class Hands
        {
            public Humanoid master;
            public Hands(Humanoid master)
            {
                this.master = master;
                interactables = new List<IInteractable>();
            }

            #region [상호작용]
            public const float interactableRange = 1000f;
            public List<IInteractable> interactables;
            public void InteractableListRefresh()
            {
                GamemodeIngame igm = (GamemodeIngame)master.gamemode;

                List<IInteractable> interactables = new List<IInteractable>();

                foreach (Entity ent in igm.entitys)
                {
                    float dis = (ent.Position - master.Position).Magnitude();
                    if (dis > interactableRange) continue;

                    if (ent is IInteractable interactable)
                        interactables.Add(interactable);
                }

                lock (this.interactables)
                    this.interactables = interactables;
            }
            public void Interact(Entity entity)
            {
                if (entity is IInteractable interactable)
                    interactable.BeInteract(master);

            }
            #endregion

            #region [장비 착용]
            public IHandable handling = null;


            public Vector2f handPos = Vector2fEx.Zero;
            public Vector2f handPosTarget = Vector2fEx.Zero;
            public const float handPosSpeed = 0.2f;
            public float handRot = 0f;
            public float handRotTarget = 0f;
            public const float handRotSpeed = 0.2f;

            public void DrawHandlingProcess()
            {
                if (handling == null) return;

                handPosTarget = master.Direction.ToRadian().ToVector() * 50f;
                handPos = (handPos + handPosTarget * handPosSpeed) / (1f + handPosSpeed);

                handRotTarget = master.Direction;

                Vector2f handRotVec = handRot.ToRadian().ToVector();
                Vector2f handRotTargetVec = handRotTarget.ToRadian().ToVector();

                handRotVec = (handRotVec + handRotTargetVec * handRotSpeed) / (1f + handRotSpeed);
                handRot = handRotVec.ToDirection().ToDirection();


                handling.DrawHandable(
                    DrawManager.texWrHigher,
                    master.Position + handPos,
                    handRot,
                    new Vector2f(1f, (-90f <= master.Direction && master.Direction <= 90f) ? 1f : -1f) * 1.5f,
                    CameraManager.worldRenderState
                    );
            }
            public void LogicHandlingProcess()
            {
                if (handling == null) return;
                if (handling.commandsReact == null) return;

                foreach (InputManager.CommandType cmd in handling.commandsReact.Keys)
                {
                    bool isTrue = InputManager.CommandCheck(cmd);
                    handling.commandsReact[cmd](this, isTrue);
                }
            }
            #endregion


        }

        public Health health;
        public class Health
        {
            public Humanoid master;
            public Health(Humanoid master, float healthMax)
            {
                this.master = master; 
                this.healthMax = healthMax;
                this.healthNow = healthMax;
                bleedingTimer = new Timer(1000d);
                bleedingTimer.Elapsed += (s, e) =>
                {
                    if (bleeding <= 0.001f) return;
                    
                    Damage damage = new Damage() { damage = bleeding / 100f, damageType = DamageType.BLEEDING };
                    GetDamage(damage);
                    bleeding = Mathf.Clamp(0f, bleeding - bleedingReduce, 9999f);
                };
                bleedingTimer.Start();
            }

            //체력
            public float healthMax, healthNow;
            //출혈
            public float bleeding = 0f;
            public const float bleedingReduce = 1f;
            Timer bleedingTimer;


            #region [피해를 다루는 정보들]
            public struct Damage
            {
                public float damage;
                public float pierce;
                public float bleeding;

                public HittedPart hittedPart;
                public DamageType damageType;
            }

            public enum DamageType
            {
                BULLET,
                BLEEDING,
                //STARVATION,
            }
            public enum HittedPart 
            {
                HEAD,
                BODY,
                LIMBS
            }
            #endregion
            public float GetDamage(Damage damageStatus) 
            {
                switch (damageStatus.damageType) 
                {
                    case DamageType.BULLET:
                        {
                            switch (damageStatus.hittedPart)
                            {
                                case HittedPart.HEAD:
                                    {
                                        damageStatus.damage *= 2f; //헤드샷 배율 2배

                                        //헬멧이 있다면 방어도 처리
                                        if (helmet == null) break;
                                        float blockValue = helmet.armourLevel - damageStatus.pierce;
                                        //완전 관통
                                        if (blockValue < -2f)
                                        {
                                            //100%
                                            //헬멧 내구도 감소
                                        }
                                        //부분 관통
                                        else if (blockValue < 2f)
                                        {
                                            float fierRatio = (-blockValue + 4f) / 8f; //75%~25%
                                            damageStatus.damage *= fierRatio;
                                            damageStatus.bleeding *= fierRatio;
                                            damageStatus.pierce = -blockValue;
                                            //헬멧 내구도 감소
                                        }
                                        //완전 방어
                                        else
                                        {
                                            damageStatus.damage *= 0.05f; //5%
                                            damageStatus.bleeding *= 0f;    //0%
                                            damageStatus.pierce = -1f;
                                            //헬멧 내구도 감소
                                        }
                                    }
                                    break;
                                case HittedPart.BODY:
                                    {
                                        damageStatus.damage *= 1f; //몸샷 배율 1배

                                        //방탄판이 있다면 방어도 처리
                                        if (plate == null) break;
                                        float blockValue = plate.armourLevel - damageStatus.pierce;
                                        //완전 관통
                                        if (blockValue < -2f)
                                        {
                                            //100%
                                            //방탄판 내구도 감소
                                        }
                                        //부분 관통
                                        else if (blockValue < 2f)
                                        {
                                            float fierRatio = (-blockValue + 4f) / 8f; 
                                            damageStatus.damage *= fierRatio;           //75.0%~25.0%
                                            damageStatus.bleeding *= fierRatio * 0.5f;  //37.5%~12.5%
                                            damageStatus.pierce = -blockValue;
                                            //방탄판 내구도 감소
                                        }
                                        //완전 방어
                                        else
                                        {
                                            damageStatus.damage *= 0.05f; //5%
                                            damageStatus.bleeding *= 0f;    //0%
                                            damageStatus.pierce = -1f;
                                            //방탄판 내구도 감소
                                        }
                                    }
                                    break;
                                case HittedPart.LIMBS:
                                    {
                                        damageStatus.damage *= 0.5f; //사지 배율 0.5배

                                        //방탄판이 있다면 방어도 처리
                                        if (plate == null) break;
                                        float blockValue = plate.armourLevel - damageStatus.pierce;
                                        //완전 관통
                                        if (blockValue < -2f)
                                        {
                                            //100%
                                            //방탄판 내구도 감소
                                        }
                                        //부분 관통
                                        else if (blockValue < 2f)
                                        {
                                            float fierRatio = (-blockValue + 4f) / 8f; //75%~25%
                                            damageStatus.damage *= fierRatio;
                                            damageStatus.bleeding *= fierRatio;
                                            damageStatus.pierce = -blockValue;
                                            //방탄판 내구도 감소
                                        }
                                        //완전 방어
                                        else
                                        {
                                            damageStatus.damage *= 0.05f; //5%
                                            damageStatus.bleeding *= 0f;    //0%
                                            damageStatus.pierce = -1f;
                                            //방탄판 내구도 감소
                                        }
                                    }
                                    break;
                            }
                            damageStatus.pierce /= 2f;
                            damageStatus.pierce = -0.7f;

                            //추가적으로 할만한거...?
                            if(healthNow <= 0f) master.Dispose();
                        }
                        break;
                    case DamageType.BLEEDING:
                        {

                        }
                        break;
                }

                //Console.WriteLine($"{(damageStatus.damageType == DamageType.BULLET? "총탄" : "출혈")}데미지 : {damageStatus.damage} {damageStatus.bleeding}");
                //데미지 처리
                healthNow -= damageStatus.damage;
                bleeding += damageStatus.bleeding;

                //관통값 반환 > 탄이 계속 나아갈 수 있을지?
                return damageStatus.pierce;
            }

            public Helmet helmet { get { return master.inventory.helmet.item as Helmet; } }
            public ArmourPlate plate {
                get
                {
                    PlateCarrier plateCarrier = master.inventory.plateCarrier.item as PlateCarrier;
                    if(plateCarrier == null) return null;
                    if(plateCarrier.armourPlate == null) return null;

                    return plateCarrier.armourPlate;
                }
            }


        }
    }
}
