using SFML.Graphics;
using SFML.System;
using System.Runtime.CompilerServices;
using static _231109_SFML_Test.Humanoid.Hands;


namespace _231109_SFML_Test
{
    internal partial class Humanoid
    {
        public Hands hands;
        public partial class Hands
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
                trembleTime += master.gamemode.deltaTime * 100f;
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
                if (Program.tm == null) return;
                if (master.gamemode == null) return;
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

                INVENTORY,  //인벤토리
                INTERACTION,//상호작용

                UNEQUIP,    //장착 해제
                EQUIP,      //장착

                BOLT_ROUND,     //노리쇠 후퇴전진
                BOLT_BACK,      //노리쇠 후퇴고정
                BOLT_FOWARD,    //노리쇠 전진

                MAGAZINE_CHANGE,    //탄창 교체
                MAGAZINE_INSPECT,   //탄창 확인
                MAGAZINE_REMOVE,    //탄창 제거
                MAGAZINE_ATTACH,    //탄창 부착
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


            public AnimationState state = AnimationState.EQUIP;
            public float time = 0f, timeMax = 1f;
            public Animator nowAnimator;
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
