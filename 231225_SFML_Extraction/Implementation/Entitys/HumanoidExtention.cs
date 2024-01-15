using SFML.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static _231109_SFML_Test.Humanoid;
using static _231109_SFML_Test.Humanoid.Hands;
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

                public virtual string GetCartegory()
                {
                    switch (equipmentType)
                    {
                        case EquipmentType.HEADGEAR:
                            return "머리장비";
                        case EquipmentType.PLATE_CARRIER:
                            return "방탄복 플레이트";
                        case EquipmentType.HELMET:
                            return "헬멧";
                        case EquipmentType.BACKPACK:
                            return "가방";
                        default:
                            return "???";
                    }
                }
            }
            public class EquipSlotWeapon : EquipSlot
            {
                public EquipSlotWeapon(bool isMain, bool isFirst) : base(EquipmentType.WEAPON) { this.isMain = isMain; }

                bool isMain, isFirst;

                public override bool DoEquipItem(Item item)
                {
                    // 무장인지?
                    if (item is Weapon weapon)
                    {

                        // 주무장 칸인데 주무장이 아니야?
                        if (weapon.AbleMain() != isMain)
                            return false;

                        // 보조무장 칸인데 보조무장이 아니야?
                        else if (weapon.AbleSub() != !isMain)
                            return false;
                    }

                    // 무장이 아니야? 나가
                    else return false;


                    if (this.item == null)
                    {
                        this.item = (IEquipable)item;
                        return true;
                    }

                    return false;
                }

                public override string GetCartegory()
                {
                    return isMain ? isFirst ? "주무장" : "부무장" : "보조무장";
                }
            }

            public EquipSlot weaponPrimary = new EquipSlotWeapon(true, true);
            public EquipSlot weaponSecondary = new EquipSlotWeapon(true, false);
            public EquipSlot weaponSub = new EquipSlotWeapon(false, false);

            public EquipSlot helmet = new EquipSlot(EquipmentType.HELMET);
            public EquipSlot headgear = new EquipSlot(EquipmentType.HEADGEAR);
            public EquipSlot plateCarrier = new EquipSlot(EquipmentType.PLATE_CARRIER);
            public EquipSlot backpack = new EquipSlot(EquipmentType.BACKPACK);

            public Storage pocket = new Storage(new Vector2i(8, 10));
            #endregion

            #region [제공 함수]
            //드랍된 아이템 줍기
            public bool TakeItem(Item item)
            {
                StorageNode? newPlace;

                //주머니 먼저 삽입
                newPlace = pocket.GetPosInsert(item);
                if (newPlace.HasValue)
                {
                    item.onStorage?.RemoveItem(item);
                    pocket.Insert(newPlace.Value);

                    return true;
                }

                //가방에 삽입
                if (backpack.item is Backpack bp)
                {
                    newPlace = bp.storage.GetPosInsert(item);
                    if (newPlace.HasValue)
                    {
                        item.onStorage?.RemoveItem(item);
                        bp.storage.Insert(newPlace.Value);

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
                item.onStorage?.RemoveItem(item);
                item.DroppedItem(master.Position);

                item.droppedItem.speed = master.hands.handRot.ToRadian().ToVector() * 1000f;
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

            //인벤토리 상호작용
            public IInteractable interactingTarget = null;   //지속적인 상호작용 대상
            public bool onInventory = false;        //인벤토리를 여는 중

            public const float interactableRange = 100f;
            public List<IInteractable> interactables;
            public void InteractableListRefresh()
            {
                GamemodeIngame igm = (GamemodeIngame)master.gamemode;

                List<IInteractable> interactables = new List<IInteractable>();

                lock (igm.entitys)
                    foreach (Entity ent in igm.entitys)
                    {

                        float dis = (ent.Position - master.Position).Magnitude();
                        if (dis > interactableRange) continue;

                        if (ent is IInteractable interactable)
                        {
                            interactables.Add(interactable);
                        }
                    }

                lock (this.interactables)
                    this.interactables = interactables;

                if(interactingTarget != null)
                if (interactables.Contains(interactingTarget) == false) 
                {
                    interactingTarget = null;
                    onInventory = false;
                }
            }
            public void Interact(Entity entity)
            {
                if (onInventory == false)
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
                    new Vector2f(1f, 1.5f * (-90f <= master.Direction && master.Direction <= 90f ? 1f : -1f)),
                    CameraManager.worldRenderState
                    );
            }

            public void LogicHandlingProcess()
            {
                //조작 가능 체크
                if (handling == null) return;
                if (handling.commandsReact == null) return;
                if (master.hands.onInventory == true) return;

                //플레이어 기준 조작 handling.commandsReact.Keys는 조작유형이기 때문에 공유 가능
                //handling.commandsReact.Values는 사용자 입력이기 때문에 AI가 조작 불가능하다...
                foreach (InputManager.CommandType cmd in handling.commandsReact.Keys)
                {
                    bool isTrue = InputManager.CommandCheck(cmd);
                    handling.commandsReact[cmd](this, isTrue);
                }
            }




            #endregion
            #region [애니메이션]
            public struct Phase 
            {
                public Phase(Vector2f position, float rotation)
                {
                    this.position = position;
                    this.rotation = rotation;
                }

                public Vector2f position;
                public float rotation;

                public static Phase Lerp(float value, Phase start, Phase end) { return new Phase(); }
            }

            public abstract class Animator : IDisposable
            {
                public Phase phase;
                public float size;

                #region [기타 편의를 위한 메서드들]
                public Vector2f position
                {
                    get { return phase.position; }
                    set => phase.position = value;
                }
                public float rotation
                {
                    get { return phase.rotation; }
                    set => phase.rotation = value;
                }


                public void Draw() => DrawManual(phase);
                public void DrawManual(Phase phase) => DrawManual(phase.position, phase.rotation);
                #endregion

                public abstract void DrawManual(Vector2f position, float rotation);



                #region [애니메이터 탈부착]
                Dictionary<Animator, Phase> attacheds = new Dictionary<Animator, Phase>();
                Animator attachTo;

                //피동 탈부착
                public void BeAttach(Animator animator, Vector2f position, float rotation) => BeAttach(animator, new Phase(position, rotation));
                public void BeAttach(Animator animator, Phase phase) => attacheds[animator] = phase;
                public void BeDettach(Animator animator) => attacheds.Remove(animator);

                //능동 탈부착
                public void DoAttach(Animator animator, Vector2f position, float rotation) => DoAttach(animator, new Phase(position, rotation));
                public void DoAttach(Animator animator, Phase phase) 
                {
                    attachTo.BeAttach(animator, phase);
                    attachTo = animator;
                }
                public void DoDettach() 
                {
                    if (attachTo == null) throw new Exception("DoDettach - 아니 attachTo가 null인데 어디서 뗀다는거?");
                    attachTo.BeDettach(this);
                    attachTo = null;
                }

                //프로세스
                public void AttachesProcess()
                {
                    //TODO

                    foreach(Animator animator in attacheds.Keys) animator.AttachesProcess();
                }
                #endregion

                #region [소멸자 처리]
                public bool isDiposed = false;
                ~Animator() => Dispose();
                public void Dispose()
                {
                    isDiposed = true;

                    foreach (Animator animator in attacheds.Keys) animator.DoDettach(); // 나에게 붙어있는 애니메이터들 제거.
                    DoDettach();  //내가 붙어 있는 애니메이터에서 제거

                    GC.SuppressFinalize(this);
                }
                #endregion
            }

            public enum HandAnimationType
            {
                IDLE,
            }
            public class HandUnit
            {
                public HandUnit(Hands hands, bool isRightArm)
                {
                    this.hands = hands;
                    this.isRightArm = isRightArm;

                    steadyOffset = isRightArm ? new Vector2f(50f, 100f) : new Vector2f(-50f, 100f);
                    animaType = HandAnimationType.IDLE;


                }
                public Hands hands;
                public HandAnimationType animaType;
                public Vector2f steadyOffset;
                public bool isRightArm;
            }
            public void AnimationProcess()
            {

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
                bleedingTimer = new Timer(10d);
                bleedingTimer.Elapsed += (s, e) =>
                {
                    if (bleeding <= 0.001f) return;

                    Damage damage = new Damage() { damage = bleeding * bleedingReduce, damageType = DamageType.BLEEDING };
                    GetDamage(damage);
                    bleeding = Mathf.Clamp(0f, bleeding * (1f - bleedingReduce), 9999f);
                };
                bleedingTimer.Start();
            }

            //체력
            public float healthMax, healthNow;
            //출혈
            public float bleeding = 0f;
            public const float bleedingReduce = 0.0005f;
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

                if (healthNow <= 0f) master.Dispose();
                //관통값 반환 > 탄이 계속 나아갈 수 있을지?
                return damageStatus.pierce;
            }

            public Helmet helmet { get { return master.inventory.helmet.item as Helmet; } }
            public ArmourPlate plate
            {
                get
                {
                    PlateCarrier plateCarrier = master.inventory.plateCarrier.item as PlateCarrier;
                    if (plateCarrier == null) return null;
                    if (plateCarrier.armourPlate == null) return null;

                    return plateCarrier.armourPlate;
                }
            }


        }
    }
}
