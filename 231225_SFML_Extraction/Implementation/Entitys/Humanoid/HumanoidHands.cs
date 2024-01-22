using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace _231109_SFML_Test
{
    internal partial class Humanoid
    {
        public Hands hands;
        public class Hands
        {
            public Humanoid master;
            public Hands(Humanoid master)
            {
                this.master = master;
                interactables = new List<IInteractable>();
            }

            //수정 필요.
            public void SetHandling(IHandable handable)
            {
                if (this.handling == handable) return;

                this.handling = handable;
                if (handable is Weapon w)
                {
                    nowAnimator = new WeaponAnimator(this, w);
                    master.aim.SetWeapon(w);
                }
                else throw new Exception("SetHandling - IHandable handable을 지원하는 타입이 존재하지 않습니다.");

            }
            public void LooseHandling()
            {
                this.handling = null;

                nowAnimator.Dispose();
                nowAnimator = null;
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
            public IHandable handlingSwapTo = null;

            public Vector2f handPosTremble;
            public Vector2f handPos = Vector2fEx.Zero;
            public Vector2f handPosTarget = Vector2fEx.Zero;
            public const float handPosSpeed = 0.2f;

            public float handRotTremble;
            public float handRot = 0f;
            public float handRotTarget = 0f;
            public const float handRotSpeed = 0.2f;

            public Vector2f handPosMovement;

            static FastNoise noise = new FastNoise();
            float trembleTime = 0f;

            public void SetSwapHandling(IHandable newHandable)
            {
                nowAnimator.ChangeState(AnimationState.UNEQUIP);
                handlingSwapTo = newHandable;
            }

            public void PhaseHandlingProcess()
            {
                //handPosTremble / handRotTremble
                trembleTime += VideoManager.GetTimeDelta() * 100f;
                trembleTime += master.aim.moveRatio * 2f;

                handPosMovement = master.movement.speed.Normalize() * master.aim.moveRatio * 5f;

                handPosTremble = new Vector2f(noise.GetPerlin(11f, trembleTime), noise.GetPerlin(1231f, trembleTime)) * 10f;
                handPosTremble += new Vector2f(noise.GetPerlin(11f, VideoManager.GetTimeTotal() * 5f), noise.GetPerlin(1231f, VideoManager.GetTimeTotal() * 5f));

                handRotTremble = noise.GetPerlin(41f, VideoManager.GetTimeTotal() * 5f) * 10f + noise.GetPerlin(1331f, trembleTime) * 5f;

                //handPos
                handPosTarget = master.Direction.ToRadian().ToVector() * 50f;
                handPos = (handPos + handPosTarget * handPosSpeed) / (1f + handPosSpeed);

                //handRot
                handRotTarget = master.Direction;

                Vector2f handRotVec = handRot.ToRadian().ToVector();
                Vector2f handRotTargetVec = handRotTarget.ToRadian().ToVector();

                handRotVec = (handRotVec + handRotTargetVec * handRotSpeed) / (1f + handRotSpeed);
                handRot = handRotVec.ToDirection().ToDirection();
            }

            public void LogicHandlingProcess(Func<InputManager.CommandType, bool> commandFunc)
            {
                //조작 가능 체크
                if (handling == null) return;
                if (handling.commandsReact == null) return;
                if (master.hands.onInventory == true) return;

                //플레이어 기준 조작 handling.commandsReact.Keys는 조작유형이기 때문에 공유 가능
                //handling.commandsReact.Values는 사용자 입력이기 때문에 AI가 조작 불가능하다...
                foreach (InputManager.CommandType cmd in handling.commandsReact.Keys)
                {
                    handling.commandsReact[cmd](this, commandFunc(cmd));
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

                public Phase GetRelativePhase(Phase bePhase)
                {
                    Phase ret = this;
                    Vector2f sepPos = bePhase.position.RotateFromZero(rotation);
                    float sepRot = bePhase.rotation;

                    ret.position += sepPos;
                    ret.rotation += sepRot;

                    return ret;
                }
            }
            public enum AnimationState
            {
                IDLE,       //대기
                FIRE,       //격발
                SPRINT,     //질주
                ADS,        //조준

                INVENTORY,  //인벤토리
                INTERACTION,//상호작용

                UNEQUIP,    //장착 해제
                EQUIP,      //장착
            }
            public abstract class Animator : IDisposable
            {
                protected Hands hands;

                public Animator(Hands hands)
                {
                    this.hands = hands;
                }

                public abstract void DrawProcess();
                public abstract void StateProcess();
                public abstract void ChangeState(AnimationState newState);
                public Action changeStateCallback;

                ~Animator() => Dispose();
                public virtual void Dispose() { GC.SuppressFinalize(this); }
            }

            public class WeaponAnimator : Animator
            {
                public Weapon weapon;
                public WeaponAnimator(Hands hands, Weapon weapon) : base(hands)
                {
                    this.weapon = weapon;

                    rightShape = new RectangleShape(new Vector2f(8, 8));
                    leftShape = new RectangleShape(new Vector2f(8, 8));

                    weaponPhase = weapon.AbleSub() ? new Phase(new Vector2f(10f, -50f), 90f) : new Phase(new Vector2f(10f, 50f), -90f);
                    rightPhase = weaponPhase.GetRelativePhase(new Phase(weapon.specialPos.pistolPos, 30f));
                    leftPhase = weaponPhase.GetRelativePhase(new Phase(weapon.specialPos.secGripPos, 70f));
                    magazinePhase = weaponPhase.GetRelativePhase(new Phase(weapon.specialPos.magPos, 0f));

                    ChangeState(AnimationState.EQUIP);
                }

                public Phase centralPhase;
                public Phase weaponPhase, rightPhase, leftPhase, magazinePhase;
                RectangleShape rightShape, leftShape;
                float sprintTime = 0f;

                public override void DrawProcess()
                {
                    //크기 제어
                    float sizeRatio = 1.5f;
                    bool isReversed = !Mathf.InRange(-90f, hands.handRot, 90f);
                    Vector2f sizeVec = new Vector2f(1f, isReversed ? -1f : 1f) * sizeRatio;

                    switch (hands.state)
                    {
                        case AnimationState.IDLE:
                            {
                                //무기 위치 이동
                                weaponPhase.position = weaponPhase.position / 1.07f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + 0f.ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);
                            }
                            break;

                        case AnimationState.SPRINT:
                            {
                                sprintTime += VideoManager.GetTimeDelta() * 10f * hands.master.aim.moveRatio;
                                float timeRatio = (float)Math.Pow(hands.time / hands.timeMax, 2f);

                                //무기 위치 이동
                                Vector2f tVec = new Vector2f
                                    (
                                        (float)Math.Abs(Math.Cos(sprintTime)) - 1f,
                                        (float)Math.Sin(sprintTime)
                                    );
                                float tRot = 60f;

                                weaponPhase.position = (weaponPhase.position + tVec * timeRatio) / 1.07f;
                                weaponPhase.rotation =
                                    (
                                        weaponPhase.rotation.ToRadian().ToVector() +
                                        ((isReversed ? tRot : -tRot) * timeRatio).ToRadian().ToVector()
                                    ).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);
                            }
                            break;
                        case AnimationState.ADS: break;

                        case AnimationState.INVENTORY: break;
                        case AnimationState.INTERACTION: break;

                        case AnimationState.EQUIP:
                            {
                                //이동할 무기위치를 찾기
                                Phase tPhase = weapon.AbleSub() ? new Phase(new Vector2f(10f, -50f), 90f) : new Phase(new Vector2f(10f, 50f), -90f);
                                float timeRatio = hands.time / hands.timeMax;

                                tPhase.position.X = tPhase.position.X.Lerp(0f, timeRatio);
                                tPhase.position.Y = tPhase.position.Y.Lerp(0f, timeRatio);
                                tPhase.rotation = tPhase.rotation.Lerp(0f, timeRatio);

                                //무기 위치 이동
                                weaponPhase.position = weaponPhase.position / 1.07f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + 0f.ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);
                            }
                            break;
                        case AnimationState.UNEQUIP:
                            {
                                //이동할 무기위치를 찾기
                                Phase tPhase = weapon.AbleSub() ? new Phase(new Vector2f(10f, -50f), 90f) : new Phase(new Vector2f(10f, 50f), -90f);
                                float timeRatio = 1f - hands.time / hands.timeMax;

                                tPhase.position.X = tPhase.position.X.Lerp(0f, timeRatio);
                                tPhase.position.Y = tPhase.position.Y.Lerp(0f, timeRatio);
                                tPhase.rotation = tPhase.rotation.Lerp(0f, timeRatio);

                                //무기 위치 이동
                                weaponPhase.position = weaponPhase.position / 1.07f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + 0f.ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);
                            }
                            break;
                        default: break;
                    }

                    #region [드로우 수행 위치]

                    //드로우의 기준으로 잡을 위상 로드
                    Vector2f centralPos = hands.master.Position + hands.handPos;
                    float centralRot = hands.handRot;

                    //손 도형의 위치와 회전 조정
                    rightShape.Position = rightPhase.position.RotateFromZero(centralRot) * sizeRatio + centralPos;
                    rightShape.Rotation = rightPhase.rotation + centralRot;
                    leftShape.Position = leftPhase.position.RotateFromZero(centralRot) * sizeRatio + centralPos;
                    leftShape.Rotation = leftPhase.rotation + centralRot;

                    //무기와 탄창의 위치와 회전 조정
                    Vector2f magPos = magazinePhase.position.RotateFromZero(centralRot) * sizeRatio + centralPos;
                    float magRot = magazinePhase.rotation + centralRot;
                    Vector2f weaPos = centralPhase.position.RotateFromZero(centralRot) * sizeRatio + centralPos;
                    float weaRot = centralPhase.rotation + centralRot;

                    //실제 드로우
                    weapon.magazineAttached.DrawHandable(DrawManager.texWrHigher, magPos, magRot, CameraManager.worldRenderState, sizeVec);
                    weapon.DrawHandable(DrawManager.texWrHigher, weaPos, weaRot, sizeVec, CameraManager.worldRenderState);
                    rightShape.Draw(DrawManager.texWrHigher, CameraManager.worldRenderState);
                    leftShape.Draw(DrawManager.texWrHigher, CameraManager.worldRenderState);

                    #endregion

                    //콜백처리
                    if (hands.time > hands.timeMax && changeStateCallback != null) changeStateCallback();

                }

                public override void StateProcess()
                {
                    switch (hands.state)
                    {
                        case AnimationState.IDLE:
                            {
                                if ((int)hands.master.movement.targetIndex >= 1.99f && hands.master.movement.nowValue > 0.99f) ChangeState(AnimationState.SPRINT);
                            }
                            break;
                        case AnimationState.FIRE: { } break;
                        case AnimationState.SPRINT:
                            {
                                //시간값 제어 = 질주태세
                                if ((int)hands.master.movement.targetIndex <= 1.01f)
                                    hands.time -= VideoManager.GetTimeDelta() * 2f;

                                else if (hands.time < 0.01f)
                                    hands.time += 0.02f;

                                hands.time = Mathf.Clamp(0f, hands.time, hands.timeMax);

                                //질주 딜레이 해제 시 대기 상태로
                                if (hands.time <= 0.01f) ChangeState(AnimationState.IDLE);
                            }
                            break;
                        case AnimationState.ADS: { } break;

                        case AnimationState.INVENTORY: { } break;
                        case AnimationState.INTERACTION: { } break;

                        case AnimationState.EQUIP: { } break;
                        case AnimationState.UNEQUIP: { } break;
                    }

                    hands.time += VideoManager.GetTimeDelta();
                }

                public override void ChangeState(AnimationState newState)
                {
                    switch (newState)
                    {
                        case AnimationState.IDLE:
                            {
                                hands.state = AnimationState.IDLE;
                                hands.time = 0f;
                                hands.timeMax = 0f;
                                changeStateCallback = null;
                            }
                            break;
                        case AnimationState.FIRE: break;
                        case AnimationState.SPRINT:
                            {
                                hands.state = AnimationState.SPRINT;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.sprintTime;
                                changeStateCallback = null;
                            }
                            break;
                        case AnimationState.ADS: break;

                        case AnimationState.INVENTORY: break;
                        case AnimationState.INTERACTION: break;

                        case AnimationState.EQUIP:
                            {
                                hands.state = AnimationState.EQUIP;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.swapTime;
                                changeStateCallback = () => ChangeState(AnimationState.IDLE);
                            }
                            break;
                        case AnimationState.UNEQUIP: break;
                        default: break;
                    }

                }

                public Phase PosRotToRelPhase(Vector2f pos, float rot, bool isReversed)
                {
                    Vector2f phasePos = new Vector2f(pos.X, pos.Y * (isReversed ? -1f : 1f));
                    return centralPhase.GetRelativePhase(new Phase(phasePos, rot * (isReversed ? -1f : 1f)));
                }
                public Phase PosRotToAbsPhase(Vector2f pos, float rot, bool isReversed)
                {
                    Vector2f phasePos = new Vector2f(pos.X, pos.Y * (isReversed ? -1f : 1f));
                    return new Phase(phasePos, rot * (isReversed ? -1f : 1f));

                }
            }

            public AnimationState state = AnimationState.EQUIP;
            public float time = 0f, timeMax = 1f;
            Animator nowAnimator;
            public void AnimationProcess()
            {
                nowAnimator?.DrawProcess();
                nowAnimator?.StateProcess();
            }

            public Vector2f AnimationGetPos(Vector2f specialPos)
            {
                if (nowAnimator is WeaponAnimator wAnim) 
                    return wAnim.weaponPhase.GetRelativePhase(new Phase(specialPos, 0f)).position;
                else return new Vector2f();
            }
            #endregion

        }
    }
}
