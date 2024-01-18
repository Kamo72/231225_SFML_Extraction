using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Policy;
using System.Timers;
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
                AnimationInit();
            }

            //수정 필요.
            public void SetHandling(IHandable handable)
            {
                this.handling = handable;

                Animator hUnit;
                if (handable is Weapon w) hUnit = new WeaponUnit(w);
                else if (handable is Item i) hUnit = new ItemUnit(i, Vector2fEx.Zero, 90f);
                else throw new Exception("SetHandling - IHandable handable을 지원하는 타입이 존재하지 않습니다.");

                if (handable is Weapon wea)
                    master.aim.SetWeapon(wea);

                animators.Add(hUnit);
            }
            public void LooseHandling()
            {
                this.handling = null;

                Animator animatorToRemove = animators.Find((a) => a is HandableUnit);
                animatorToRemove.Dispose();
                animators.Remove(animatorToRemove);

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

                if (interactingTarget != null)
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

                public static Phase Lerp(float value, Phase start, Phase end)
                {
                    Phase ret = new Phase();
                    ret.position = start.position * (1f - value) + end.position * value;
                    Vector2f rotVec = start.rotation.ToRadian().ToVector() * (1f - value) + end.rotation.ToRadian().ToVector() * value;
                    ret.rotation = rotVec.ToDirection();

                    return ret;
                }
            }
            protected abstract class Animator : IDisposable
            {
                Phase phasePro;
                public Phase phase
                {
                    get { return phasePro; }
                    set
                    {
                        Vector2f sepPos = value.position - phasePro.position;
                        float sepRot = value.rotation - phasePro.rotation;

                        phasePro = value;
                        foreach (Animator item in attacheds.Keys)
                        {
                            item.position += sepPos;
                            item.rotation += sepRot;

                            Vector2f difPos = item.position - phasePro.position;
                            difPos.RotateFromZero(sepRot);
                            item.position = phasePro.position + difPos;
                        }
                    }
                }
                public float size;

                public Animator(Vector2f position, float rotation, float size) : this(new Phase(position, rotation), size) { }
                public Animator(Phase phase, float size)
                {
                    this.phase = phase;
                    this.size = size;
                }

                public abstract void DrawManual(Vector2f position, float rotation);
                protected abstract void LogicProcess();


                #region [기타 편의를 위한 메서드들]

                public Vector2f position
                {
                    get { return phasePro.position; }
                    set => phase = new Phase(value, phase.rotation);
                }
                public float rotation
                {
                    get { return phasePro.rotation; }
                    set => phase = new Phase(phase.position, value);
                }


                public void Draw(Hands hands)
                {
                    Vector2f tPos = hands.handRot.ToRadian().ToVector() * phase.position.X +
                         (hands.handRot + 90f).ToRadian().ToVector() * phase.position.Y +
                         hands.handPos +
                         hands.master.Position;
                    float tRot = hands.handRot + phase.rotation;

                    Phase newPhase = new Phase(tPos, tRot);

                    DrawManual(newPhase);
                }
                public void DrawManual(Phase phase) => DrawManual(phase.position, phase.rotation);
                #endregion

                #region [애니메이터 탈부착]
                protected Dictionary<Animator, Phase> attacheds = new Dictionary<Animator, Phase>();
                protected Animator attachTo;

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
                    attachTo?.BeDettach(this);
                    attachTo = null;
                }

                //프로세스
                public void AttachesProcess()
                {
                    LogicProcess();

                    foreach (Animator animator in attacheds.Keys) animator.AttachesProcess();
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

            protected abstract class HandableUnit : Animator
            {
                public HandableUnit(IHandable handable) : base(new Vector2f(), 90f, 1f)
                {
                    this.handable = handable;
                }

                protected IHandable handable;

                public override void DrawManual(Vector2f position, float rotation)
                {
                    handable.DrawHandable(DrawManager.texWrHigher, position, rotation, new Vector2f(1f, 1f) * size, CameraManager.worldRenderState);
                }

                protected override abstract void LogicProcess();
            }


            protected class HandUnit : Animator
            {
                public enum AnimationType
                {
                    LOOSE,
                    IDLE,
                }
                public HandUnit(bool isRightArm) : base(Vector2fEx.Zero, 0f, 1f)
                {
                    this.isRightArm = isRightArm;

                    phaseTarget = isRightArm ? new Phase(new Vector2f(50f, 100f), 0f) : new Phase(new Vector2f(-50f, 100f), 0f);
                    animaType = AnimationType.IDLE;

                    drawShape = new RectangleShape(new Vector2f(32, 32));
                    drawShape.Origin = drawShape.Size / 2f;
                    drawShape.Position = phase.position;
                    drawShape.Rotation = phase.rotation;
                    //drawShape.Texture = ResourceManager.textures["hand_idle"];

                }

                public RectangleShape drawShape;

                public AnimationType animaType;
                public Phase phaseTarget;   //사용자의 위상이 수렴하는 곳.
                public bool isRightArm;

                //피 부착을 위한 이동
                public Animator grabTarget = null;
                public Phase grabPhase;

                public override void DrawManual(Vector2f position, float rotation)
                {
                    drawShape.Position = position;
                    drawShape.Rotation = rotation;

                    DrawManager.texWrHigher.Draw(drawShape, CameraManager.worldRenderState);
                }

                //
                void SetGrab(Animator animator, Phase phase)
                {
                    grabTarget = animator;
                    grabPhase = phase;
                }
                void CancelGrab()
                {
                    grabTarget = null;
                }

                protected void ChangeAnimation(AnimationType animationType)
                {
                    switch (animationType)
                    {
                        case AnimationType.IDLE:
                            //SetGrab()
                            break;
                        case AnimationType.LOOSE:
                            CancelGrab();
                            phaseTarget = isRightArm ? new Phase(new Vector2f(50f, 100f), 0f) : new Phase(new Vector2f(-50f, 100f), 0f);
                            break;
                    }
                }

                protected override void LogicProcess()
                {
                    switch (animaType)
                    {
                        case AnimationType.IDLE:
                            //if (attachTo != null)
                            //    Phase.Lerp(0.1f, phase, phaseTarget);
                            break;

                        case AnimationType.LOOSE:
                            if (grabTarget != null) CancelGrab();
                            break;
                    }

                }
            }
            protected class WeaponUnit : HandableUnit
            {
                public WeaponUnit(Weapon weapon) : base(weapon) { }

                protected override void LogicProcess() { }
            }

            protected class ItemUnit : Animator
            {
                public ItemUnit(Item item, Vector2f position, float rotation) : base(position, rotation, 1f)
                {
                    this.item = item;

                    shape = new RectangleShape(new Vector2f(64, 64));
                    shape.Texture = ResourceManager.textures[item.spriteName];
                    shape.Position = position;
                    shape.Rotation = rotation;
                }

                Item item;
                RectangleShape shape;

                public override void DrawManual(Vector2f position, float rotation)
                {
                    shape.Position = position;
                    shape.Rotation = rotation;
                    DrawManager.texWrHigher.Draw(shape, CameraManager.worldRenderState);
                }

                protected override void LogicProcess() { }
            }

            List<Animator> animators;
            public void AnimationInit()
            {
                animators = new List<Animator>
                {
                    new HandUnit(true),
                    new HandUnit(false),
                };
            }
            public void AnimationProcess()
            {

            }
            public void DrawAnimatorsProcess()
            {
                foreach (Animator ani in animators) ani.Draw(this);
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
                bleedingTimer = new Timer(100d);
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
            public const float bleedingReduce = 0.005f;
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

        public Movement movement;
        public class Movement
        {
            public Humanoid master;
            public Movement(Humanoid master)
            {
                this.master = master;
                weapon = null;
            }

            public void MovementProcess() 
            {
                StateProcess();
                RefreshProcess();
                PhysicProcess();
            }


            //무기 확인
            public Weapon weapon;
            public void SetWeapon(Weapon weapon) => this.weapon = weapon;

            //무브먼트 데이터 구조
            public struct MovementData
            {
                public MovementData(float friction, float accel, bool handUsable, bool directive)
                {
                    this.friction = friction;
                    this.accel = accel;
                    this.handUsable = handUsable;
                    this.directive = directive;
                }

                public float friction;  //마찰
                public float accel;     //가속

                public bool handUsable; //장비 사용 가능

                public bool directive;  //방향 전환...
                public static MovementData Lerp(float value, MovementData s, MovementData e)
                {
                    return new MovementData()
                    {
                        accel = s.accel * (1f - value) + e.accel * value,
                        friction = s.friction * (1f - value) + e.friction * value,
                        handUsable = s.handUsable && e.handUsable,
                        directive = s.directive || e.directive,
                    };
                }
            }
            public enum MovementState
            {
                CROUNCH,
                IDLE,
                SPRINT,
            }

            //고정 프리셋, 실제 프리셋
            static (MovementData crounch, MovementData walk, MovementData sprint) movementOrigin =
            (
                crounch: new MovementData(11.0f, 2200f, true, false), //숙이기
                walk: new MovementData(8.0f, 2800f, true, false),    //기본
                sprint: new MovementData(5.0f, 3500f, false, true)   //스프린트
            );
            (MovementData crounch, MovementData walk, MovementData sprint) movementPreset = movementOrigin;

            //무브먼트 데이터
            public MovementData nowMovement;
            public MovementState nowState = MovementState.IDLE;
            public float nowValue = 1f;

            const MovementState basicIndex = MovementState.IDLE;   //걷기 상태 인덱스
            public MovementState targetIndex = MovementState.IDLE; //목표 상태 인덱스
            public const float crounchTime = 0.300f;
            public float sprintTime => weapon == null? 0.200f : weapon.status.timeDt.sprintTime;


            void StateProcess()
            {
                float deltaTime = VideoManager.GetTimeDelta();

                switch (targetIndex)
                {
                    case MovementState.CROUNCH:
                        {
                            //달리는 상태라면? 
                            if (nowValue > 1.01f)
                            {
                                nowValue = Mathf.Clamp(0f, nowValue - 1f / sprintTime * deltaTime, 2f);
                                break;
                            }

                            //서 있는 상태라면?
                            if (nowValue > 0.01f)
                            {
                                nowValue = Mathf.Clamp(0f, nowValue - 1f / crounchTime * deltaTime, 2f);
                                break;
                            }

                            nowValue = 0.00f;
                            break;
                        }

                    case MovementState.IDLE:
                        {
                            //웅크리기 상태라면? 
                            if (nowValue < 0.99f)
                            {
                                nowValue = Mathf.Clamp(0f, nowValue + 1f / crounchTime * deltaTime, 2f);
                                break;
                            }

                            //달리는 상태라면? 
                            if (nowValue > 1.01f)
                            {
                                nowValue = Mathf.Clamp(0f, nowValue - 1f / sprintTime * deltaTime, 2f);
                                break;
                            }

                            nowValue = 1.00f;
                            break;
                        }
                    
                    case MovementState.SPRINT:
                        { 
                            //이동 중이 아니라면?
                            if (moveDir.Magnitude() < 0.01f)
                            {
                                targetIndex = MovementState.IDLE;
                                break;
                            }

                            //웅크리기 상태라면? 
                            if (nowValue < 0.99f)
                            {
                                nowValue = Mathf.Clamp(0f, nowValue + 1f / crounchTime * deltaTime, 2f);
                                break;
                            }

                            //서 있는 상태라면?
                            if (nowValue < 1.99f)
                            {
                                nowValue = Mathf.Clamp(0f, nowValue + 1f / sprintTime * deltaTime, 2f);
                                break;
                            }

                            nowValue = 2.00f;
                            break;
                        }

                }
            }

            //손을 사용할 수 있는가?
            public bool handUsable => nowMovement.handUsable; 
            //방향전환이 전방인가?
            public bool directive => nowMovement.directive;

            //프로세스
            public float accelPer = 1.00f;      //가속 배율
            void PhysicProcess()
            {

                //마찰에 의한 감속
                double deltaTime = 1d / master.gamemode.logicFps;
                speed *= (float)(1d - nowMovement.friction * deltaTime);

                //이동에 의한 가속
                Vector2f accelVec = moveDir.Magnitude() > 1f ? moveDir.Normalize() : moveDir;
                speed += accelVec * (float)(nowMovement.accel * accelPer * deltaTime);

                #region [충돌]

                Circle maskHum = master.mask as Circle;
                Vector2f posOrigin = master.Position;
                Vector2f vecOrigin = speed;// * (float)deltaTime;

                GamemodeIngame gm = master.gamemode as GamemodeIngame;

                //벽과의 충돌
                foreach (Structure stru in gm.structures)
                {
                    maskHum.Position = posOrigin + new Vector2f(vecOrigin.X * (float)deltaTime, 0f);
                    if (maskHum.IsCollision(stru.mask))
                        vecOrigin.X = Math.Abs(vecOrigin.X) * Math.Sign(vecOrigin.X) * -0.5f;

                    maskHum.Position = posOrigin + new Vector2f(0f, vecOrigin.Y * (float)deltaTime);
                    if (maskHum.IsCollision(stru.mask))
                        vecOrigin.Y = Math.Abs(vecOrigin.Y) * Math.Sign(vecOrigin.Y) * -0.5f;

                    maskHum.Position = posOrigin;
                    speed = vecOrigin;
                }

                //엔티티와의 충돌
                foreach (Entity ent in gm.entitys)
                {
                    if (ent == null) continue;
                    if (ent.isDisposed == true) continue;
                    if (ent.Position == master.Position) continue;

                    if (ent is Container || ent is Humanoid)
                        if (ent.mask.IsCollision(master.mask))
                        {
                            float dis = (master.Position - ent.Position).Magnitude();
                            float pushMultipier = 1f / (dis + 1f) * 10000f;
                            Vector2f push = (master.Position - ent.Position).Normalize() * pushMultipier;
                            speed += push;
                            if (ent is Humanoid human)
                                human.movement.speed -= push;
                        }
                }
                #endregion

                //속도에 의한 변위
                master.Position += speed * (float)deltaTime;
            }

            //무브먼트 데이터 초기화
            void RefreshProcess()
            {
                //기본(걷기) 상태라면?
                if (Math.Abs(nowValue - (float)basicIndex) < 0.001f)
                {
                    nowMovement = movementPreset.walk;
                    nowState = MovementState.IDLE;
                }
                //로우레디 상태라면?
                else if (nowValue < (float)basicIndex)
                {
                    float ratio = nowValue;
                    nowMovement = MovementData.Lerp(ratio, movementPreset.crounch, movementPreset.walk);
                    nowState = MovementState.CROUNCH;
                }
                //스프린트 상태라면?
                else if (nowValue > (float)basicIndex)
                {
                    float ratio = nowValue - 1f;
                    nowMovement = MovementData.Lerp(ratio, movementPreset.walk, movementPreset.sprint);
                    nowState = MovementState.SPRINT;
                }
            }

            //이동 정보
            public Vector2f speed = Vector2fEx.Zero; // 속도 벡터
            public Vector2f moveDir = Vector2fEx.Zero; // 가속 벡터 (최대 1)

        }

        public Aim aim;
        public class Aim
        {
            public Humanoid master;
            public Aim(Humanoid master)
            {
                this.master = master;
                weapon = null;

                recoilVec = Vector2fEx.Zero;
                traggingDot = master.Position;
                damageVec = Vector2fEx.Zero;
            }

            //이동
            Movement.MovementData moveData => master.movement.nowMovement;
            float moveValue => master.movement.nowValue;

            //무기
            Weapon weapon;
            WeaponStatus status;
            WeaponStatus.AimData aimData => status.aimDt;

            //무기 제어 함수
            public void SetWeapon(Weapon weapon)
            {
                this.weapon = weapon;
                status = GetAdjustAppliedStatus(weapon);
            }
            public WeaponStatus GetAdjustAppliedStatus(Weapon weapon)
            {
                //TODO
                return weapon.status;
            }

            //통합 프로세스
            public void AimProcess()
            {
                AdsProcess();

                DamageVectorProcess();
                RecoilVectorProcess();
                AdsStanceVectorProcess();

                HipStanceProcess();
                HipRecoilProcess();

                TraggingDotProcess();

            }


            //ADS 전환 및 값 -1 : 질주, 0 : 대기, 1 : 조준
            //딜레이 -1 ~ 0 : 질주 전환 / 0 ~ 1 : 조준 전환
            public float adsValue = 0f;
            public float adsTime => status.timeDt.adsTime;
            public bool isAds = false;

            public void AdsProcess() 
            {
                float deltaTime = VideoManager.GetTimeDelta();

                if (isAds)
                {
                    adsValue = Math.Min(adsValue + 1f / adsTime * deltaTime, 1f);
                    if (master.movement.targetIndex == Movement.MovementState.SPRINT) master.movement.targetIndex = Movement.MovementState.IDLE;
                }
                else if (moveData.handUsable == false)
                {
                    adsValue = Math.Max(adsValue - 1f / sprintTime * deltaTime, -1f);
                }
                else
                {
                    adsValue = 
                        adsValue < -0.01f ?
                            Math.Min(adsValue + 1f / sprintTime * deltaTime, 0f) :
                        adsValue > 0.01f ?
                            Math.Max(adsValue - 1f / adsTime * deltaTime, 0f) :
                            adsValue = 0.00f;
                }
            }
            
            public float sprintTime => status.timeDt.sprintTime;
            public float moveRatio => master.movement.speed.Magnitude() / 100f / 3.7f;


            //실제 마우스 점, 동적 마우스 점;
            public Vector2f staticDot => master.aimPosition ;
            public Vector2f dynamicDot => traggingDot + damageVec + recoilVec + adsStanceVec + (Vector2f)VideoManager.resolutionNow / 2f;



            #region [트래킹 도트]

            public Vector2f traggingDot;
            /// <summary>
            /// 트래킹 도트 수렴 속도
            /// </summary>
            float traggingDotSpeed => aimData.hip.traggingSpeed;

            void TraggingDotProcess() =>
                traggingDot = (traggingDot * 100f + staticDot * traggingDotSpeed) / (100f + traggingDotSpeed);

            #endregion


            #region [피격 반응 벡터]

            Vector2f damageVec;
            /// <summary>
            /// 피격 반응 회복 속도
            /// </summary>
            float damageVecRecovery;
            
            static Random random = new Random();
            public void GetDamageVector(float value) =>
                damageVec += new Vector2f((float)(random.NextDouble() - 0.5f) * value, (float)(random.NextDouble() - 0.5f) * value);

            void DamageVectorProcess()
            {
                damageVec = (damageVec * 100f) / (100f + damageVecRecovery);

                if(master is Player player)
                    CameraManager.GetShake(damageVec.Magnitude());
            }
            #endregion


            /// <summary>
            /// 지향 사격 탄퍼짐
            /// </summary>
            public float hipSpray => (hiptStanceSpray + hipRecoilSpray)
                * Math.Min(1f - adsValue, 1f) 
                * master.aimPosition.Magnitude() / 1000f;


            #region [지향 사격 - 자세(지향 사격 정확도)]

            /// <summary>
            /// 지향 사격 자세 탄퍼짐
            /// </summary>
            float hiptStanceSpray = 0f;
            float hipStanceRecovery => aimData.hip.stance.recovery;
            float hipStanceAccuracy
            {
                get
                {
                    if (moveValue > 1.01f) //달리기 중
                    {
                        float ratio = Math.Max(-adsValue, 0f);
                        float adjustRatio = Mathf.PercentMultiflex(10f, ratio);

                        return aimData.hip.stance.accuracy * adjustRatio;
                    }
                    else if (moveRatio > 0.01f) //이동 중
                    {
                        float ratio = Mathf.Clamp(0f, moveRatio, 1f);
                        float adjustRatio = Mathf.PercentMultiflex(aimData.hip.stance.accuracyAdjust.walk, ratio);

                        return aimData.hip.stance.accuracy * adjustRatio;
                    }
                    
                    if (moveValue < 0.99f) //웅크리기
                    {
                        float ratio = 1f - moveValue;
                        float adjustRatio = Mathf.PercentMultiflex(aimData.hip.stance.accuracyAdjust.crounch, ratio);

                        return aimData.hip.stance.accuracy * adjustRatio;
                    }

                    return aimData.hip.stance.accuracy;
                }
            }

            void HipStanceProcess()
            {
                if (hiptStanceSpray < hipStanceAccuracy)
                    hiptStanceSpray = Math.Min(hipStanceAccuracy, hiptStanceSpray + 10f);
                else
                    hiptStanceSpray = Math.Max(hipStanceAccuracy, hiptStanceSpray - hipStanceRecovery);
            }

            #endregion


            #region [지향 사격 - 반동(지향 사격 반동)]

            /// <summary>
            /// 지향 사격 반동 탄퍼짐
            /// </summary>
            float hipRecoilSpray = 0f;
            float hipRecoilStrength => aimData.hip.recoil.strength;
            float hipRecoilRecovery
            {
                get
                {
                    if (moveValue > 1.01f) //달리기 중
                    {
                        float ratio = moveValue - 1f;
                        float adjustRatio = Mathf.PercentMultiflex(ratio, 0.5f);

                        return aimData.hip.recoil.recovery * adjustRatio;
                    }
                    else if (moveRatio > 0.01f) //이동 중
                    {
                        float ratio = Mathf.Clamp(0f, moveRatio, 1f);
                        float adjustRatio = Mathf.PercentMultiflex(aimData.hip.recoil.recoveryAdjust.walk, ratio);

                        return aimData.hip.recoil.recovery * adjustRatio;
                    }

                    if (moveValue < 0.99f) //웅크리기
                    {
                        float ratio = 1f - moveValue;
                        float adjustRatio = Mathf.PercentMultiflex(aimData.hip.recoil.recoveryAdjust.crounch, ratio);

                        return aimData.hip.recoil.recovery * adjustRatio;
                    }

                    return aimData.hip.recoil.recovery;
                }
            }

            public void GetHipRecoil()
            {
                hipRecoilSpray += hipRecoilStrength;

                if (master is Player player)
                    CameraManager.GetShake(hipRecoilStrength);
            }

            void HipRecoilProcess() =>
                hipRecoilSpray = (hipRecoilSpray * 100f) / (100f + hipRecoilRecovery);

            #endregion


            #region [반동 벡터(조준점 반동 제어)]

            /// <summary>
            /// 반동 벡터
            /// </summary>
            public Vector2f recoilVec;

            /// <summary>
            /// 반동 벡터 회복 속도
            /// </summary>
            float recoilVecRecovery => aimData.ads.recoil.recovery;
            Vector2f recoilVecFix => aimData.ads.recoil.fix;
            Vector2f recoilVecRandom => aimData.ads.recoil.random;
            float recoilStrengthAdjust
            {
                get
                {
                    if (moveValue < 0.99f) //웅크리기
                    {
                        float ratio = 1f - moveValue;
                        float adjustRatio = Mathf.PercentMultiflex(aimData.ads.recoil.strengthAdjust.crounch, ratio);

                        return 1f * adjustRatio;
                    }

                    if (moveRatio  > 0.01f) //이동 중
                    {
                        float ratio = Mathf.Clamp(0f, moveRatio, 1f);
                        float adjustRatio = Mathf.PercentMultiflex(aimData.ads.recoil.strengthAdjust.walk, ratio);

                        return 1f * adjustRatio;
                    }

                    return 1f;
                }
            }

            public void GetRecoilVector()
            {
                Vector2f ret = recoilVecFix 
                    + new Vector2f(
                        (float)(random.NextDouble() - 0.5f) * recoilVecRandom.X, 
                        (float)(random.NextDouble() - 0.5f) * recoilVecRandom.Y)
                    * recoilStrengthAdjust;
                recoilVec += ret;
            }

            void RecoilVectorProcess() =>
                recoilVec = (recoilVec * 100f) / (100f + recoilVecRecovery);

            #endregion


            #region [조준 사격 - 자세(조준점 안정)]

            /// <summary>
            /// 조준 사격 정확도
            /// </summary>
            public Vector2f adsStanceVec;

            float adsStanceAccuracy
            {
                get
                {
                    if (moveValue < 0.99f) //웅크리기
                    {
                        float ratio = 1f - moveValue;
                        float adjustRatio = Mathf.PercentMultiflex(aimData.ads.stance.accuracyAdjust.crounch, ratio);

                        return aimData.ads.stance.accuracy * adjustRatio;
                    }

                    if (moveRatio > 0.01f) //이동 중
                    {
                        float ratio = Mathf.Clamp(0f, moveRatio, 1f);
                        float adjustRatio = Mathf.PercentMultiflex(aimData.ads.stance.accuracyAdjust.walk, ratio);

                        return aimData.ads.stance.accuracy * adjustRatio;
                    }

                    return aimData.ads.stance.accuracy;
                }
            }

            float time;
            static FastNoise noise = new FastNoise();
            void AdsStanceVectorProcess()
            {
                time += VideoManager.GetTimeDelta() * adsStanceAccuracy  + (1f + recoilVec.Magnitude() / 10f); 

                float x = adsStanceAccuracy * noise.GetPerlin(0f, time);
                float y = adsStanceAccuracy * noise.GetPerlin(1335f, time);

                adsStanceVec = new Vector2f(x, y)  * master.aimPosition.Magnitude() / 1000f;
                //Console.WriteLine($"x : {x}, y : {y}");
            }


            #endregion


        }
    }
}
