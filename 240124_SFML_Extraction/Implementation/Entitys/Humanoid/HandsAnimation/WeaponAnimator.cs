using _231109_SFML_Test;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _231109_SFML_Test.Humanoid.Hands;

namespace _231109_SFML_Test
{
    internal partial class Humanoid
    {
        public partial class Hands
        {
            public class WeaponAnimator : Animator
            {
                public Weapon weapon;

                Vector2f boltVec
                {
                    get => new Vector2f(
                        weapon.boltValue.backwardValue,
                        weapon.boltValue.lockValue
                        );
                    set
                    {
                        weapon.boltValue.backwardValue = value.X;
                        weapon.boltValue.lockValue = value.Y;
                    }
                }
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

                Phase centralPhase;
                public Phase weaponPhase;
                Phase rightPhase, leftPhase, magazinePhase;

                RectangleShape rightShape, leftShape;
                float sprintTime = 0f;
                static Random random = new Random();
                Magazine magazineNew = null, magazineOld = null;

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
                                Phase tPhase = new Phase(new Vector2f(-10f, isReversed ? -6f : 6f) * (1 - hands.master.aim.adsValue), 0f);

                                weaponPhase.position = (weaponPhase.position + tPhase.position * 0.07f) / 1.07f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + 0f.ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + (hands.handPosTremble + hands.handPosMovement) * (1f - hands.master.aim.adsValue * 0.8f);
                                centralPhase.rotation = weaponPhase.rotation + (hands.handRotTremble) * (1f - hands.master.aim.adsValue * 0.8f);

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);
                            }
                            break;

                        case AnimationState.FIRE:
                            {
                                //이동할 무기위치를 찾기
                                float timeRatio = hands.time / hands.timeMax;
                                Phase tPhase = new Phase(new Vector2f(0f -
                                    timeRatio * ((float)random.NextDouble() + 2.0f) * 10f,
                                    timeRatio * (float)(random.NextDouble() - 0.5f) * 10f), -timeRatio);

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + tPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (tPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                if (weapon.magazineAttached != null && weapon.magazineAttached.AmmoPeek() != null)
                                {
                                    weapon.boltValue.backwardValue = 1f - (float)Math.Pow(timeRatio, 2f);
                                }
                                else
                                {
                                    weapon.boltValue.backwardValue = 1f;
                                    weapon.boltValue.lockValue = 1f;
                                }


                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);
                            }
                            break;
                        case AnimationState.SPRINT:
                            {
                                sprintTime += hands.master.gamemode.deltaTime * 10f * hands.master.aim.moveRatio;
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

                        case AnimationState.INVENTORY: break;
                        case AnimationState.INTERACTION: break;

                        case AnimationState.EQUIP:
                            {
                                //이동할 무기위치를 찾기
                                Phase tPhase = weapon.AbleSub() ? new Phase(new Vector2f(50f, -30f), -90f) : new Phase(new Vector2f(50f, 30f), 90f);
                                float timeRatio = hands.time / hands.timeMax;

                                tPhase.position.X = tPhase.position.X.Lerp(0f, timeRatio);
                                tPhase.position.Y = tPhase.position.Y.Lerp(0f, timeRatio);
                                tPhase.rotation = tPhase.rotation.Lerp(0f, timeRatio);

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + tPhase.position * 0.07f) / 1.07f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (tPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

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

                        case AnimationState.BOLT_ROUND:
                            {
                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio;

                                Phase rhPhase = new Phase();
                                Phase wPhase = new Phase();

                                //볼트 잡기
                                if (Mathf.InRange(0f, timeRatio, 0.07f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.07f - 0f);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(-5f, inTimeRatio),
                                        0f.Lerp(-2f, inTimeRatio)),
                                        0f.Lerp(-5f, inTimeRatio)
                                        );

                                    //boltVec = new Vector2f(0f, weapon.status.typeDt.boltLockerType == BoltLockerType.LOCK_TO_FIRE ? 1f : 0f);
                                    boltVec = new Vector2f(0f, 0f);
                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            weapon.specialPos.pistolPos.X.Lerp(weapon.specialPos.boltPos.X, inTimeRatio),
                                            weapon.specialPos.pistolPos.Y.Lerp(weapon.specialPos.boltPos.Y, inTimeRatio)),
                                            70f.Lerp(40f, inTimeRatio), isReversed);

                                }
                                //볼트 잡고 대기 - 볼트액션 잠금 해제
                                else if (Mathf.InRange(0.07f, timeRatio, 0.16f))
                                {
                                    inTimeRatio = (timeRatio - 0.07f) / (0.16f - 0.07f);

                                    wPhase = new Phase(new Vector2f(-5f, -2f), -5f);

                                    boltVec = new Vector2f(0f, weapon.status.typeDt.boltLockerType == BoltLockerType.LOCK_TO_FIRE ? 1f * inTimeRatio : 0f);
                                    Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                        + boltVec.X * weapon.boltVec.backwardVec
                                        + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                    rhPhase = boltPhase;
                                }
                                //볼트 당기기
                                else if (Mathf.InRange(0.16f, timeRatio, 0.47f))
                                {
                                    inTimeRatio = (timeRatio - 0.16f) / (0.47f - 0.16f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        -5f.Lerp(-6f, lerpValue),
                                        -2f.Lerp(+2f, lerpValue))
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 20f * lerpValue,
                                        -5f.Lerp(-10f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 20f * lerpValue
                                        );

                                    boltVec = new Vector2f(lerpValue, 0f);
                                    Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                        + boltVec.X * weapon.boltVec.backwardVec
                                        + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                    rhPhase = boltPhase;
                                }
                                //볼트 당기고 대기
                                else if (Mathf.InRange(0.47f, timeRatio, 0.62f))
                                {
                                    inTimeRatio = (timeRatio - 0.47f) / (0.62f - 0.47f);

                                    wPhase = new Phase(new Vector2f(-6f, +7f)
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 2f,
                                        -10f
                                        + ((float)random.NextDouble() - 0.5f) * 2f
                                        );
                                    boltVec = new Vector2f(1f, 0f);

                                    Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                        + boltVec.X * weapon.boltVec.backwardVec
                                        + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                    rhPhase = boltPhase;
                                }
                                //볼트 놓기 또는 전진
                                else if (Mathf.InRange(0.62f, timeRatio, 0.74f))
                                {
                                    inTimeRatio = (timeRatio - 0.62f) / (0.74f - 0.62f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    float trembleRatio = (inTimeRatio > 0.5f) ? (inTimeRatio - 0.5f) * 2f : 0f;

                                    wPhase = new Phase(
                                        new Vector2f(
                                            -6f.Lerp(2f, lerpValue),
                                            +2f.Lerp(0f, lerpValue)
                                            )
                                        + new Vector2f(
                                            (float)random.NextDouble() - 0.5f,
                                            (float)random.NextDouble() - 0.5f
                                        ) * 20f * trembleRatio,
                                        -10f.Lerp(5f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 20f * trembleRatio
                                        );

                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        boltVec = new Vector2f(Mathf.Clamp(0f, 1f - inTimeRatio, 1f), 0f);

                                        Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                            + boltVec.X * weapon.boltVec.backwardVec
                                            + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                        rhPhase = boltPhase;

                                    }
                                    else
                                    {
                                        boltVec = new Vector2f(Mathf.Clamp(0f, 1f - (float)Math.Pow(inTimeRatio, 0.1f), 1f), 0f);
                                    }
                                }
                                //볼트 놓고 대기 또는 볼트 락
                                else if (Mathf.InRange(0.74f, timeRatio, 0.80f))
                                {
                                    inTimeRatio = (timeRatio - 0.74f) / (0.80f - 0.74f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(2f, 0f), 5f);

                                    boltVec = new Vector2f(0f, weapon.status.typeDt.boltLockerType == BoltLockerType.LOCK_TO_FIRE ? lerpValue : 0f);
                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                            + boltVec.X * weapon.boltVec.backwardVec
                                            + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);
                                        rhPhase = boltPhase;
                                    }
                                }
                                //다시 전방 손잡이 잡기
                                else if (Mathf.InRange(0.80f, timeRatio, 0.92f))
                                {
                                    inTimeRatio = (timeRatio - 0.80f) / (0.92f - 0.80f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        +2f.Lerp(-1f, lerpValue),
                                        +0f.Lerp(-1f, lerpValue))
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 2f * lerpValue,
                                        +5f.Lerp(-2f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 2f * lerpValue
                                        );


                                    boltVec = new Vector2f(0f, 0f);

                                    Phase pistolPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 70f, isReversed);
                                    rhPhase = PosRotToAbsPhase(new Vector2f(
                                        rhPhase.position.X.Lerp(pistolPhase.position.X, 0.01f),
                                        rhPhase.position.Y.Lerp(pistolPhase.position.Y, 0.01f)),
                                        rhPhase.rotation.Lerp(pistolPhase.rotation, 0.01f),
                                        isReversed);

                                }
                                //후 딜레이
                                else if (Mathf.InRange(0.96f, timeRatio, 1.01f))
                                {
                                    inTimeRatio = (timeRatio - 0.92f) / (1.00f - 0.92f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    boltVec = new Vector2f(0f, 0f);

                                    wPhase = new Phase(new Vector2f(
                                        -1f.Lerp(0f, lerpValue),
                                        -1f.Lerp(0f, lerpValue)),
                                        -2f.Lerp(0f, lerpValue)
                                        );

                                    Phase secGripPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 70f, isReversed);
                                    rhPhase = secGripPhase;
                                }

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = rhPhase;
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);

                            }
                            break;
                        case AnimationState.BOLT_BACK:
                            {
                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio;

                                Phase rhPhase = new Phase();
                                Phase wPhase = new Phase();

                                //마무리 부분 && 볼트까지 이동 부분

                                //볼트 잡기
                                if (Mathf.InRange(0f, timeRatio, 0.07f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.07f - 0f);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(-5f, inTimeRatio),
                                        0f.Lerp(-2f, inTimeRatio)),
                                        0f.Lerp(-5f, inTimeRatio)
                                        );

                                    //boltVec = new Vector2f(0f, weapon.status.typeDt.boltLockerType == BoltLockerType.LOCK_TO_FIRE ? 1f : 0f);
                                    boltVec = new Vector2f(0f, 0f);
                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            weapon.specialPos.pistolPos.X.Lerp(weapon.specialPos.boltPos.X, inTimeRatio),
                                            weapon.specialPos.pistolPos.Y.Lerp(weapon.specialPos.boltPos.Y, inTimeRatio)),
                                            70f.Lerp(40f, inTimeRatio), isReversed);

                                }
                                //볼트 잡고 대기 - 볼트액션 잠금 해제
                                else if (Mathf.InRange(0.07f, timeRatio, 0.16f))
                                {
                                    inTimeRatio = (timeRatio - 0.07f) / (0.16f - 0.07f);

                                    wPhase = new Phase(new Vector2f(-5f, -2f), -5f);

                                    boltVec = new Vector2f(0f, weapon.status.typeDt.boltLockerType == BoltLockerType.LOCK_TO_FIRE ? 1f * inTimeRatio : 0f);
                                    Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                        + boltVec.X * weapon.boltVec.backwardVec
                                        + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                    rhPhase = boltPhase;
                                }
                                //볼트 당기기
                                else if (Mathf.InRange(0.16f, timeRatio, 0.47f))
                                {
                                    inTimeRatio = (timeRatio - 0.16f) / (0.47f - 0.16f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        -5f.Lerp(-6f, lerpValue),
                                        -2f.Lerp(+2f, lerpValue))
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 20f * lerpValue,
                                        -5f.Lerp(-10f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 20f * lerpValue
                                        );

                                    boltVec = new Vector2f(lerpValue, 0f);
                                    Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                        + boltVec.X * weapon.boltVec.backwardVec
                                        + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                    rhPhase = boltPhase;
                                }
                                //볼트 당기고 대기
                                else if (Mathf.InRange(0.47f, timeRatio, 0.62f))
                                {
                                    inTimeRatio = (timeRatio - 0.47f) / (0.62f - 0.47f);

                                    wPhase = new Phase(new Vector2f(-6f, +7f)
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 2f,
                                        -10f
                                        + ((float)random.NextDouble() - 0.5f) * 2f
                                        );
                                    boltVec = new Vector2f(1f, 0f);

                                    Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                        + boltVec.X * weapon.boltVec.backwardVec
                                        + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                    rhPhase = boltPhase;
                                }
                                //볼트 잠금
                                else if (Mathf.InRange(0.62f, timeRatio, 0.74f))
                                {
                                    inTimeRatio = (timeRatio - 0.62f) / (0.74f - 0.62f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(
                                        new Vector2f(
                                            (-6f).Lerp(-6f, lerpValue),
                                            +2f.Lerp(2f, lerpValue)
                                            ),
                                        -10f.Lerp(5f, lerpValue)
                                        );

                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        boltVec = new Vector2f(1f, 0f);
                                    }
                                    else
                                    {
                                        boltVec = new Vector2f(0f, 0f);
                                    }

                                    Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                        + boltVec.X * weapon.boltVec.backwardVec
                                        + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                    rhPhase = boltPhase;
                                }
                                //볼트 놓고 대기 또는 볼트 락
                                else if (Mathf.InRange(0.74f, timeRatio, 0.80f))
                                {
                                    inTimeRatio = (timeRatio - 0.74f) / (0.80f - 0.74f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(2f, 0f), 5f);

                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        boltVec = new Vector2f(1f, 0f);
                                    }
                                    else
                                    {
                                        boltVec = new Vector2f(0f, 0f);
                                    }

                                    Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                        + boltVec.X * weapon.boltVec.backwardVec
                                        + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                    rhPhase = boltPhase;
                                }
                                //다시 전방 손잡이 잡기
                                else if (Mathf.InRange(0.80f, timeRatio, 0.92f))
                                {
                                    inTimeRatio = (timeRatio - 0.80f) / (0.92f - 0.80f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        +2f.Lerp(-1f, lerpValue),
                                        +0f.Lerp(-1f, lerpValue))
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 2f * lerpValue,
                                        +5f.Lerp(-2f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 2f * lerpValue
                                        );


                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        boltVec = new Vector2f(1f, 0f);
                                    }
                                    else
                                    {
                                        boltVec = new Vector2f(0f, 0f);
                                    }

                                    Phase pistolPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 70f, isReversed);
                                    rhPhase = PosRotToAbsPhase(new Vector2f(
                                        rhPhase.position.X.Lerp(pistolPhase.position.X, 0.01f),
                                        rhPhase.position.Y.Lerp(pistolPhase.position.Y, 0.01f)),
                                        rhPhase.rotation.Lerp(pistolPhase.rotation, 0.01f),
                                        isReversed);

                                }
                                //후 딜레이
                                else if (Mathf.InRange(0.96f, timeRatio, 1.01f))
                                {
                                    inTimeRatio = (timeRatio - 0.92f) / (1.00f - 0.92f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        boltVec = new Vector2f(1f, 0f);
                                    }
                                    else
                                    {
                                        boltVec = new Vector2f(0f, 0f);
                                    }

                                    wPhase = new Phase(new Vector2f(
                                        -1f.Lerp(0f, lerpValue),
                                        -1f.Lerp(0f, lerpValue)),
                                        -2f.Lerp(0f, lerpValue)
                                        );

                                    Phase secGripPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 70f, isReversed);
                                    rhPhase = secGripPhase;
                                }

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = rhPhase;
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);

                            }
                            break;
                        case AnimationState.BOLT_FOWARD:
                            {
                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio;

                                Phase lhPhase = new Phase();
                                Phase wPhase = new Phase();

                                //노리쇠 전진기로 이동
                                if (Mathf.InRange(0f, timeRatio, 0.42f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.42f - 0f);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(-5f, inTimeRatio),
                                        0f.Lerp(-2f, inTimeRatio)),
                                        0f.Lerp(-5f, inTimeRatio)
                                        );

                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        boltVec = new Vector2f(1f, 0f);

                                        Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                            + boltVec.X * weapon.boltVec.backwardVec
                                            + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                        lhPhase = boltPhase;

                                    }
                                    else
                                    {
                                        boltVec = new Vector2f(1f,  1f);

                                        lhPhase = PosRotToRelPhase(new Vector2f(
                                                weapon.specialPos.secGripPos.X.Lerp(weapon.specialPos.ejectPos.X, inTimeRatio),
                                                weapon.specialPos.secGripPos.Y.Lerp(weapon.specialPos.ejectPos.Y, inTimeRatio)),
                                                30f.Lerp(40f, inTimeRatio), isReversed);
                                    }

                                }
                                //볼트 놓기 또는 전진
                                else if (Mathf.InRange(0.42f, timeRatio, 0.67f))
                                {
                                    inTimeRatio = (timeRatio - 0.42f) / (0.67f - 0.42f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);
                                    float trembleRatio = (inTimeRatio > 0.5f) ? (inTimeRatio - 0.5f) * 2f : 0f;

                                    wPhase = new Phase(
                                        new Vector2f(
                                            -6f.Lerp(2f, lerpValue),
                                            +2f.Lerp(0f, lerpValue)
                                            )
                                        + new Vector2f(
                                            (float)random.NextDouble() - 0.5f,
                                            (float)random.NextDouble() - 0.5f
                                        ) * 20f * trembleRatio,
                                        -10f.Lerp(5f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 20f * trembleRatio
                                        );

                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        boltVec = new Vector2f(Mathf.Clamp(0f, 1f -inTimeRatio, 1f), 0f);

                                        Phase boltPhase = PosRotToRelPhase(weapon.specialPos.boltPos
                                            + boltVec.X * weapon.boltVec.backwardVec
                                            + boltVec.Y * weapon.boltVec.lockVec, 70f, isReversed);

                                        lhPhase = boltPhase;
                                    }
                                    else
                                    {
                                        boltVec = new Vector2f(Mathf.Clamp(0f, 1f - (float)Math.Pow(inTimeRatio, 2f), 1f), 0f);

                                        Phase ejectPhase = PosRotToRelPhase(weapon.specialPos.ejectPos, 30f, isReversed);
                                        lhPhase = ejectPhase;
                                    }
                                }
                                //볼트 놓고 대기 또는 볼트 락
                                else if (Mathf.InRange(0.67f, timeRatio, 0.75f))
                                {
                                    inTimeRatio = (timeRatio - 0.67f) / (0.75f - 0.67f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(2f, 0f), 5f);

                                    boltVec = new Vector2f(0f, weapon.status.typeDt.boltLockerType == BoltLockerType.LOCK_TO_FIRE ? lerpValue : 0f);
                                    if (weapon.status.typeDt.mechanismType == MechanismType.MANUAL_RELOAD)
                                    {
                                        Phase ejectPhase = PosRotToRelPhase(weapon.specialPos.ejectPos, 30f, isReversed);
                                        lhPhase = ejectPhase;
                                    }
                                }
                                //다시 전방 손잡이 잡기
                                else if (Mathf.InRange(0.75f, timeRatio, 0.90f))
                                {
                                    inTimeRatio = (timeRatio - 0.75f) / (0.90f - 0.75f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        +2f.Lerp(-1f, lerpValue),
                                        +0f.Lerp(-1f, lerpValue))
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 2f * lerpValue,
                                        +5f.Lerp(-2f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 2f * lerpValue
                                        );


                                    boltVec = new Vector2f(0f, 0f);

                                    Phase secGripPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                    lhPhase = PosRotToAbsPhase(new Vector2f(
                                        lhPhase.position.X.Lerp(secGripPhase.position.X, 0.03f),
                                        lhPhase.position.Y.Lerp(secGripPhase.position.Y, 0.03f)),
                                        lhPhase.rotation.Lerp(secGripPhase.rotation, 0.03f),
                                        isReversed);

                                }
                                //후 딜레이
                                else if (Mathf.InRange(0.96f, timeRatio, 1.01f))
                                {
                                    inTimeRatio = (timeRatio - 0.92f) / (1.00f - 0.92f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    boltVec = new Vector2f(0f, 0f);

                                    wPhase = new Phase(new Vector2f(
                                        -1f.Lerp(0f, lerpValue),
                                        -1f.Lerp(0f, lerpValue)),
                                        -2f.Lerp(0f, lerpValue)
                                        );

                                    Phase secGripPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                    lhPhase = secGripPhase;
                                }

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = lhPhase ;
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);

                            }
                            break;

                        case AnimationState.MAGAZINE_REMOVE:
                            {
                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio = 0f;

                                Phase rhPhase = new Phase();
                                Phase wPhase = new Phase();
                                Phase mPhase = new Phase();

                                //탄창 붙잡기, 무기 들기
                                if (Mathf.InRange(0f, timeRatio, 0.26f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.26f - 0f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(+5f, lerpValue),
                                        0f.Lerp(+3f, lerpValue)),
                                        0f.Lerp(isReversed? 45f : -45f, lerpValue)
                                        );

                                    mPhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);

                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            weapon.specialPos.secGripPos.X.Lerp(weapon.specialPos.magPos.X, inTimeRatio),
                                            weapon.specialPos.secGripPos.Y.Lerp(weapon.specialPos.magPos.Y, inTimeRatio)),
                                            70f.Lerp(mPhase.rotation - 20f, inTimeRatio), isReversed);
                                }
                                //탄창 탈착
                                else if (Mathf.InRange(0.26f, timeRatio, 0.40f))
                                {
                                    inTimeRatio = (timeRatio - 0.26f) / (0.40f - 0.26f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        +05f + (float)Math.Sin(lerpValue * Math.PI) * -2f,
                                        +03f + (float)Math.Sin(lerpValue * Math.PI) * -7f),
                                        (isReversed ? 45f : -45f) + (float)Math.Sin(lerpValue * Math.PI) * 10f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X,
                                            weapon.specialPos.magPos.Y + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -3f) + (lerpValue * -2f)
                                            ),
                                        0f.Lerp(-10f, lerpValue)
                                        + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -10f),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                //탄창 이동
                                else if (Mathf.InRange(0.40f, timeRatio, 0.51f))
                                {
                                    inTimeRatio = (timeRatio - 0.40f) / (0.51f - 0.40f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X + 0f).Lerp(weapon.specialPos.magPos.X + 5f, powValue),
                                            (weapon.specialPos.magPos.Y + -2f).Lerp(weapon.specialPos.magPos.Y + 15f, powValue)
                                            ),
                                        (-10f).Lerp(50f, powValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                //탄창 넣기
                                else if (Mathf.InRange(0.51f, timeRatio, 0.59f))
                                {
                                    inTimeRatio = (timeRatio - 0.51f) / (0.59f - 0.51f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X + 5f).Lerp(weapon.specialPos.magPos.X - 20f, lerpValue),
                                            (weapon.specialPos.magPos.Y + 15f).Lerp(weapon.specialPos.magPos.Y - 20f, lerpValue)
                                            ),
                                        50f.Lerp(25f, lerpValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 뒤적거리기
                                else if (Mathf.InRange(0.59f, timeRatio, 0.86f))
                                {

                                    inTimeRatio = (timeRatio - 0.59f) / (0.86f - 0.59f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    float trembleRatio = 1f - Math.Abs(0.5f - inTimeRatio) * 2f;
                                    Vector2f trembleVec = new Vector2f()
                                    {
                                        X = noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal() + 1231) * 5f,
                                        Y = noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal() + 9687) * 5f,
                                    } * trembleRatio;

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X - 20f + trembleVec.X,
                                    weapon.specialPos.magPos.Y - 20f + trembleVec.Y
                                            ),
                                        25f + noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal()) * 10f,
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                // 총기 원 위치, 다시 권총 손잡이 잡기
                                else if (Mathf.InRange(0.86f, timeRatio, 1.01f))
                                {
                                    if (magazineOld != null) magazineOld = null;

                                    inTimeRatio = (timeRatio - 0.80f) / (1.01f - 0.80f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +5f.Lerp(0f, lerpValue),
                                        +3f.Lerp(0f, lerpValue)),
                                        (isReversed ? 45f : -45f).Lerp(0f, lerpValue)
                                        );

                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            mPhase.position.X.Lerp(weapon.specialPos.secGripPos.X, lerpValue),
                                            mPhase.position.Y.Lerp(weapon.specialPos.secGripPos.Y, lerpValue)
                                        ),
                                        mPhase.rotation + (-20f).Lerp(70f, lerpValue),
                                        isReversed
                                        );
                                }


                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rhPhase.position = isReversed ? new Vector2f(rhPhase.position.X, rhPhase.position.Y) : rhPhase.position;
                                rightPhase = rhPhase;
                                leftPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                magazinePhase = mPhase;

                            }
                            break;
                        case AnimationState.MAGAZINE_ATTACH:
                            {
                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio = 0f;

                                Phase rhPhase = new Phase();
                                Phase wPhase = new Phase();
                                Phase mPhase = new Phase();


                                //탄창 가지러 이동, 무기 들기 
                                if (Mathf.InRange(0f, timeRatio, 0.26f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.26f - 0f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(+5f, lerpValue),
                                        0f.Lerp(+3f, lerpValue)),
                                        0f.Lerp(isReversed ? 45f : -45f, lerpValue)
                                        );

                                    rhPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.secGripPos.X).Lerp(weapon.specialPos.magPos.X - 20f, lerpValue),
                                            (weapon.specialPos.secGripPos.Y).Lerp(weapon.specialPos.magPos.Y - 20f, lerpValue)
                                            ),
                                        50f.Lerp(25f, lerpValue),
                                        isReversed
                                        );

                                }
                                //탄창 뒤적거리기
                                else if (Mathf.InRange(0.26f, timeRatio, 0.53f))
                                {
                                    inTimeRatio = (timeRatio - 0.26f) / (0.53f - 0.26f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    float trembleRatio = 1f - Math.Abs(0.5f - inTimeRatio) * 2f;
                                    Vector2f trembleVec = new Vector2f()
                                    {
                                        X = noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal() + 1231) * 5f,
                                        Y = noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal() + 9687) * 5f,
                                    } * trembleRatio;

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X - 20f + trembleVec.X,
                                    weapon.specialPos.magPos.Y - 20f + trembleVec.Y
                                            ),
                                        25f + noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal()) * 10f,
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 꺼내기
                                else if (Mathf.InRange(0.53f, timeRatio, 0.61f))
                                {

                                    if (magazineNew != null)
                                    {
                                        weapon.magazineAttached = magazineNew;
                                        magazineNew = null;
                                    }

                                    inTimeRatio = (timeRatio - 0.53f) / (0.61f - 0.53f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X - 20f).Lerp(weapon.specialPos.magPos.X + 5f, powValue),
                                            (weapon.specialPos.magPos.Y - 20f).Lerp(weapon.specialPos.magPos.Y + 15f, powValue)
                                            ),
                                        (50f).Lerp(-10f, powValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                //탄창 이동
                                else if (Mathf.InRange(0.61f, timeRatio, 0.72f))
                                {
                                    inTimeRatio = (timeRatio - 0.61f) / (0.72f - 0.61f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X + 5f).Lerp(weapon.specialPos.magPos.X + 0f, lerpValue),
                                            (weapon.specialPos.magPos.Y + 15f).Lerp(weapon.specialPos.magPos.Y - 2f, lerpValue)
                                            ),
                                        25f.Lerp(0f, lerpValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 장착
                                else if (Mathf.InRange(0.72f, timeRatio, 0.86f))
                                {

                                    inTimeRatio = (timeRatio - 0.72f) / (0.86f - 0.72f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f + (float)Math.Sin(lerpValue * Math.PI) * -2f,
                                        +03f + (float)Math.Sin(lerpValue * Math.PI) * -7f),
                                        (isReversed ? 45f : -45f) + (float)Math.Sin(lerpValue * Math.PI) * 10f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X,
                                            weapon.specialPos.magPos.Y + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -3f) + ((1f - lerpValue) * -2f)
                                            ),
                                        (-10f).Lerp(0f, lerpValue)
                                        + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -10f),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                // 총기 원 위치, 다시 전방 손잡이 잡기
                                else if (Mathf.InRange(0.86f, timeRatio, 1.01f))
                                {
                                    if (magazineOld != null) magazineOld = null;

                                    inTimeRatio = (timeRatio - 0.80f) / (1.01f - 0.80f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +5f.Lerp(0f, lerpValue),
                                        +3f.Lerp(0f, lerpValue)),
                                        (isReversed ? 45f : -45f).Lerp(0f, lerpValue)
                                        );

                                    mPhase = PosRotToRelPhase(
                                       new Vector2f(
                                           weapon.specialPos.magPos.X,
                                           weapon.specialPos.magPos.Y
                                           ),
                                       0f,
                                       isReversed
                                       );

                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            mPhase.position.X.Lerp(weapon.specialPos.secGripPos.X, lerpValue),
                                            mPhase.position.Y.Lerp(weapon.specialPos.secGripPos.Y, lerpValue)
                                        ),
                                        mPhase.rotation + (-20f).Lerp(70f, lerpValue),
                                        isReversed
                                        );
                                }

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = rhPhase;
                                leftPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                magazinePhase = mPhase;
                            }
                            break;
                        case AnimationState.MAGAZINE_CHANGE: 
                            {
                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio = 0f;

                                Phase rhPhase = new Phase();
                                Phase wPhase = new Phase();
                                Phase mPhase = new Phase();

                                //탄창 붙잡기, 무기 들기
                                if (Mathf.InRange(0f, timeRatio, 0.16f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.16f - 0f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(+5f, lerpValue),
                                        0f.Lerp(+3f, lerpValue)),
                                        0f.Lerp(isReversed ? 45f : -45f, lerpValue)
                                        );

                                    mPhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);

                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            weapon.specialPos.secGripPos.X.Lerp(weapon.specialPos.magPos.X, inTimeRatio),
                                            weapon.specialPos.secGripPos.Y.Lerp(weapon.specialPos.magPos.Y, inTimeRatio)),
                                            70f.Lerp(mPhase.rotation - 20f, inTimeRatio), isReversed);
                                }
                                //탄창 탈착
                                else if (Mathf.InRange(0.16f, timeRatio, 0.25f))
                                {
                                    inTimeRatio = (timeRatio - 0.16f) / (0.25f - 0.16f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        +05f + (float)Math.Sin(lerpValue * Math.PI) * -2f,
                                        +03f + (float)Math.Sin(lerpValue * Math.PI) * -7f),
                                        (isReversed ? 45f : -45f) + (float)Math.Sin(lerpValue * Math.PI) * 10f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X,
                                            weapon.specialPos.magPos.Y + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -3f) + (lerpValue * -2f)
                                            ),
                                        0f.Lerp(-10f, lerpValue)
                                        + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -10f),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                //탄창 이동
                                else if (Mathf.InRange(0.25f, timeRatio, 0.32f))
                                {
                                    inTimeRatio = (timeRatio - 0.25f) / (0.32f - 0.25f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X + 0f).Lerp(weapon.specialPos.magPos.X + 5f, powValue),
                                            (weapon.specialPos.magPos.Y + -2f).Lerp(weapon.specialPos.magPos.Y + 15f, powValue)
                                            ),
                                        (-10f).Lerp(50f, powValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                //탄창 넣기
                                else if (Mathf.InRange(0.32f, timeRatio, 0.37f))
                                {
                                    inTimeRatio = (timeRatio - 0.32f) / (0.37f - 0.32f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X + 5f).Lerp(weapon.specialPos.magPos.X - 20f, lerpValue),
                                            (weapon.specialPos.magPos.Y + 15f).Lerp(weapon.specialPos.magPos.Y - 20f, lerpValue)
                                            ),
                                        50f.Lerp(25f, lerpValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 뒤적거리기
                                else if (Mathf.InRange(0.37f, timeRatio, 0.54f))
                                {

                                    inTimeRatio = (timeRatio - 0.37f) / (0.54f - 0.37f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    float trembleRatio = 1f - Math.Abs(0.5f - inTimeRatio) * 2f;
                                    Vector2f trembleVec = new Vector2f()
                                    {
                                        X = noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal() + 1231) * 5f,
                                        Y = noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal() + 9687) * 5f,
                                    } * trembleRatio;

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X - 20f + trembleVec.X,
                                    weapon.specialPos.magPos.Y - 20f + trembleVec.Y
                                            ),
                                        25f + noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal()) * 10f,
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 뒤적거리기
                                else if (Mathf.InRange(0.54f, timeRatio, 0.66f))
                                {
                                    if (magazineOld != null) magazineOld = null;

                                    inTimeRatio = (timeRatio - 0.54f) / (0.66f - 0.54f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    float trembleRatio = 1f - Math.Abs(0.5f - inTimeRatio) * 2f;
                                    Vector2f trembleVec = new Vector2f()
                                    {
                                        X = noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal() + 1231) * 5f,
                                        Y = noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal() + 9687) * 5f,
                                    } * trembleRatio;

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X - 20f + trembleVec.X,
                                    weapon.specialPos.magPos.Y - 20f + trembleVec.Y
                                            ),
                                        25f + noise.GetPerlin(hands.time * 1000f, VideoManager.GetTimeTotal()) * 10f,
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 꺼내기
                                else if (Mathf.InRange(0.66f, timeRatio, 0.73f))
                                {
                                    if (magazineNew != null)
                                    {
                                        weapon.magazineAttached = magazineNew;
                                        magazineNew = null;
                                    }

                                    inTimeRatio = (timeRatio - 0.66f) / (0.73f - 0.66f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X - 20f).Lerp(weapon.specialPos.magPos.X + 5f, powValue),
                                            (weapon.specialPos.magPos.Y - 20f).Lerp(weapon.specialPos.magPos.Y + 15f, powValue)
                                            ),
                                        (50f).Lerp(-10f, powValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                //탄창 이동
                                else if (Mathf.InRange(0.73f, timeRatio, 0.80f))
                                {
                                    inTimeRatio = (timeRatio - 0.73f) / (0.80f - 0.73f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X + 5f).Lerp(weapon.specialPos.magPos.X + 0f, lerpValue),
                                            (weapon.specialPos.magPos.Y + 15f).Lerp(weapon.specialPos.magPos.Y - 2f, lerpValue)
                                            ),
                                        25f.Lerp(0f, lerpValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 장착
                                else if (Mathf.InRange(0.80f, timeRatio, 0.90f))
                                {

                                    inTimeRatio = (timeRatio - 0.80f) / (0.90f - 0.80f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f + (float)Math.Sin(lerpValue * Math.PI) * -2f,
                                        +03f + (float)Math.Sin(lerpValue * Math.PI) * -7f),
                                        (isReversed ? 45f : -45f) + (float)Math.Sin(lerpValue * Math.PI) * 10f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X,
                                            weapon.specialPos.magPos.Y + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -3f) + ((1f - lerpValue) * -2f)
                                            ),
                                        (-10f).Lerp(0f, lerpValue)
                                        + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -10f),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                // 총기 원 위치, 다시 전방 손잡이 잡기
                                else if (Mathf.InRange(0.90f, timeRatio, 1.01f))
                                {
                                    if (magazineOld != null) magazineOld = null;

                                    inTimeRatio = (timeRatio - 0.90f) / (1.01f - 0.90f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +5f.Lerp(0f, lerpValue),
                                        +3f.Lerp(0f, lerpValue)),
                                        (isReversed ? 45f : -45f).Lerp(0f, lerpValue)
                                        );

                                    mPhase = PosRotToRelPhase(
                                       new Vector2f(
                                           weapon.specialPos.magPos.X,
                                           weapon.specialPos.magPos.Y
                                           ),
                                       0f,
                                       isReversed
                                       );

                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            mPhase.position.X.Lerp(weapon.specialPos.secGripPos.X, lerpValue),
                                            mPhase.position.Y.Lerp(weapon.specialPos.secGripPos.Y, lerpValue)
                                        ),
                                        mPhase.rotation + (-20f).Lerp(70f, lerpValue),
                                        isReversed
                                        );
                                }

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rhPhase.position = isReversed ? new Vector2f(rhPhase.position.X, rhPhase.position.Y) : rhPhase.position;
                                rightPhase = rhPhase;
                                leftPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                magazinePhase = mPhase;
                            } break;
                        case AnimationState.MAGAZINE_INSPECT:
                            {
                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio = 0f;

                                Phase rhPhase = new Phase();
                                Phase wPhase = new Phase();
                                Phase mPhase = new Phase();

                                //탄창 붙잡기, 무기 들기
                                if (Mathf.InRange(0f, timeRatio, 0.16f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.16f - 0f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(+5f, lerpValue),
                                        0f.Lerp(+3f, lerpValue)),
                                        0f.Lerp(isReversed ? 45f : -45f, lerpValue)
                                        );

                                    mPhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);

                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            weapon.specialPos.secGripPos.X.Lerp(weapon.specialPos.magPos.X, inTimeRatio),
                                            weapon.specialPos.secGripPos.Y.Lerp(weapon.specialPos.magPos.Y, inTimeRatio)),
                                            70f.Lerp(mPhase.rotation - 20f, inTimeRatio), isReversed);
                                }
                                //탄창 탈착
                                else if (Mathf.InRange(0.16f, timeRatio, 0.25f))
                                {
                                    inTimeRatio = (timeRatio - 0.16f) / (0.25f - 0.16f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        +05f + (float)Math.Sin(lerpValue * Math.PI) * -2f,
                                        +03f + (float)Math.Sin(lerpValue * Math.PI) * -7f),
                                        (isReversed ? 45f : -45f) + (float)Math.Sin(lerpValue * Math.PI) * 10f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X,
                                            weapon.specialPos.magPos.Y + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -3f) + (lerpValue * -2f)
                                            ),
                                        0f.Lerp(-10f, lerpValue)
                                        + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -10f),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                //탄창 탈착 후 이동
                                else if (Mathf.InRange(0.25f, timeRatio, 0.32f))
                                {
                                    inTimeRatio = (timeRatio - 0.25f) / (0.32f - 0.25f);
                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X + 0f).Lerp(weapon.specialPos.magPos.X + 5f, powValue),
                                            (weapon.specialPos.magPos.Y + -2f).Lerp(weapon.specialPos.magPos.Y + 15f, powValue)
                                            ),
                                        (-10f).Lerp(50f, powValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                //탄창 확인
                                else if (Mathf.InRange(0.32f, timeRatio, 0.73f))
                                {
                                    inTimeRatio = (timeRatio - 0.32f) / (0.73f - 0.32f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );


                                    Vector2f trembleVec = new Vector2f(
                                        noise.GetPerlin(VideoManager.GetTimeTotal() * 300f, 1531) * 4f,
                                        noise.GetPerlin(VideoManager.GetTimeTotal() * 300f, 17) * 4f
                                        );
                                    float trembleRot = noise.GetPerlin(VideoManager.GetTimeTotal() * 300f, 9071) * 5f;

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X + 05f + trembleVec.X,
                                            weapon.specialPos.magPos.Y + 15f + trembleVec.Y
                                            ),
                                        50f + trembleRot,
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 장착 전 이동
                                else if (Mathf.InRange(0.73f, timeRatio, 0.80f))
                                {
                                    inTimeRatio = (timeRatio - 0.73f) / (0.80f - 0.73f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f,
                                        +03f),
                                        isReversed ? 45f : -45f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            (weapon.specialPos.magPos.X + 5f).Lerp(weapon.specialPos.magPos.X + 0f, lerpValue),
                                            (weapon.specialPos.magPos.Y + 15f).Lerp(weapon.specialPos.magPos.Y - 2f, lerpValue)
                                            ),
                                        50f.Lerp(0f, lerpValue),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );

                                }
                                //탄창 장착
                                else if (Mathf.InRange(0.80f, timeRatio, 0.90f))
                                {

                                    inTimeRatio = (timeRatio - 0.80f) / (0.90f - 0.80f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +05f + (float)Math.Sin(lerpValue * Math.PI) * -2f,
                                        +03f + (float)Math.Sin(lerpValue * Math.PI) * -7f),
                                        (isReversed ? 45f : -45f) + (float)Math.Sin(lerpValue * Math.PI) * 10f
                                        );

                                    mPhase = PosRotToRelPhase(
                                        new Vector2f(
                                            weapon.specialPos.magPos.X,
                                            weapon.specialPos.magPos.Y + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -3f) + ((1f - lerpValue) * -2f)
                                            ),
                                        (-10f).Lerp(0f, lerpValue)
                                        + ((float)Math.Sin(Mathf.Clamp(0f, lerpValue * 10f, 1f) * Math.PI) * -10f),
                                        isReversed
                                        );

                                    rhPhase = PosRotToAbsPhase(
                                        mPhase.position,
                                        mPhase.rotation - 20f,
                                        false
                                        );
                                }
                                // 총기 원 위치, 다시 전방 손잡이 잡기
                                else if (Mathf.InRange(0.90f, timeRatio, 1.01f))
                                {
                                    if (magazineOld != null) magazineOld = null;

                                    inTimeRatio = (timeRatio - 0.90f) / (1.01f - 0.90f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);
                                    float powValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        +5f.Lerp(0f, lerpValue),
                                        +3f.Lerp(0f, lerpValue)),
                                        (isReversed ? 45f : -45f).Lerp(0f, lerpValue)
                                        );

                                    mPhase = PosRotToRelPhase(
                                       new Vector2f(
                                           weapon.specialPos.magPos.X,
                                           weapon.specialPos.magPos.Y
                                           ),
                                       0f,
                                       isReversed
                                       );

                                    rhPhase = PosRotToRelPhase(new Vector2f(
                                            mPhase.position.X.Lerp(weapon.specialPos.secGripPos.X, lerpValue),
                                            mPhase.position.Y.Lerp(weapon.specialPos.secGripPos.Y, lerpValue)
                                        ),
                                        mPhase.rotation + (-20f).Lerp(70f, lerpValue),
                                        isReversed
                                        );
                                }

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rhPhase.position = isReversed ? new Vector2f(rhPhase.position.X, rhPhase.position.Y) : rhPhase.position;
                                rightPhase = rhPhase;
                                leftPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                magazinePhase = mPhase;
                            } break;

                        case AnimationState.SELECTOR_CHANGE:
                            {
                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio;

                                Phase wPhase = new Phase();

                                //볼트 잡기
                                if (Mathf.InRange(0f, timeRatio, 0.10f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.10f - 0f);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(-5f, inTimeRatio),
                                        0f.Lerp(-2f, inTimeRatio)),
                                        0f.Lerp(-5f, inTimeRatio)
                                        );
                                }
                                //볼트 잡고 대기 - 볼트액션 잠금 해제
                                else if (Mathf.InRange(0.10f, timeRatio, 0.22f))
                                {
                                    inTimeRatio = (timeRatio - 0.10f) / (0.22f - 0.10f);

                                    wPhase = new Phase(new Vector2f(-5f, -2f), -5f);
                                }
                                //볼트 당기기
                                else if (Mathf.InRange(0.22f, timeRatio, 0.39f))
                                {
                                    inTimeRatio = (timeRatio - 0.22f) / (0.39f - 0.22f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(new Vector2f(
                                        -5f.Lerp(-6f, lerpValue),
                                        -2f.Lerp(+2f, lerpValue))
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 20f * lerpValue,
                                        -5f.Lerp(-10f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 20f * lerpValue
                                        );
                                }

                                else if (Mathf.InRange(0.39f, timeRatio, 0.63f))
                                {
                                    inTimeRatio = (timeRatio - 0.39f) / (0.63f - 0.39f);

                                    float lerpValue = MathF.Pow(inTimeRatio, 2f);

                                    wPhase = new Phase(
                                        new Vector2f(
                                            -6f.Lerp(2f, lerpValue),
                                            +2f.Lerp(0f, lerpValue)
                                            ),
                                        -10f.Lerp(5f, lerpValue)
                                        );
                                }
                                //다시 전방 손잡이 잡기
                                else if (Mathf.InRange(0.63f, timeRatio, 0.82f))
                                {
                                    inTimeRatio = (timeRatio - 0.63f) / (0.82f - 0.63f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        + 2f.Lerp(-1f, lerpValue),
                                        + 0f.Lerp(-1f, lerpValue))
                                        + new Vector2f((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f) * 2f * lerpValue,
                                        + 5f.Lerp(-2f, lerpValue)
                                        + ((float)random.NextDouble() - 0.5f) * 2f * lerpValue
                                        );
                                }
                                //후 딜레이
                                else if (Mathf.InRange(0.82f, timeRatio, 1.01f))
                                {
                                    inTimeRatio = (timeRatio - 0.82f) / (1.00f - 0.82f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        -1f.Lerp(0f, lerpValue),
                                        -1f.Lerp(0f, lerpValue)),
                                        -2f.Lerp(0f, lerpValue)
                                        );
                                }

                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);

                            }
                            break;
                        case AnimationState.WEAPON_INSPECT_SHORT:
                            {

                                float timeRatio = hands.time / hands.timeMax;
                                float inTimeRatio;

                                Phase wPhase = new Phase();

                                //볼트 잡기
                                if (Mathf.InRange(0f, timeRatio, 0.10f))
                                {
                                    inTimeRatio = (timeRatio - 0f) / (0.10f - 0f);

                                    wPhase = new Phase(new Vector2f(
                                        0f.Lerp(-5f, inTimeRatio),
                                        0f.Lerp(-2f, inTimeRatio)),
                                        0f.Lerp(-5f, inTimeRatio)
                                        );
                                }
                                //볼트 잡고 대기 - 볼트액션 잠금 해제
                                else if (Mathf.InRange(0.10f, timeRatio, 0.90f))
                                {
                                    inTimeRatio = (timeRatio - 0.10f) / (0.90f - 0.10f);

                                    wPhase = new Phase(new Vector2f(-5f, -2f), -5f);
                                }
                                //후 딜레이
                                else if (Mathf.InRange(0.90f, timeRatio, 1.01f))
                                {
                                    inTimeRatio = (timeRatio - 0.90f) / (1.00f - 0.90f);

                                    float lerpValue = MathF.Sqrt(inTimeRatio);

                                    wPhase = new Phase(new Vector2f(
                                        -5f.Lerp(0f, lerpValue),
                                        -2f.Lerp(0f, lerpValue)),
                                        -5f.Lerp(0f, lerpValue)
                                        );
                                }


                                //무기 위치 이동
                                weaponPhase.position = (weaponPhase.position + wPhase.position * 0.12f) / 1.12f;
                                weaponPhase.rotation = (weaponPhase.rotation.ToRadian().ToVector() + (wPhase.rotation).ToRadian().ToVector()).ToDirection().ToDirection();

                                centralPhase.position = weaponPhase.position + hands.handPosTremble + hands.handPosMovement;
                                centralPhase.rotation = weaponPhase.rotation + hands.handRotTremble;

                                //무기 위상에 맞게 세부 위상 조정
                                rightPhase = PosRotToRelPhase(weapon.specialPos.pistolPos, 30f, isReversed);
                                leftPhase = PosRotToRelPhase(weapon.specialPos.secGripPos, 70f, isReversed);
                                magazinePhase = PosRotToRelPhase(weapon.specialPos.magPos, 0f, isReversed);
                            }
                            break;
                        case AnimationState.TDEVICE_INTERACTION:
                            {
                                
                            } break;
                        default: Console.WriteLine("정의되지 않은 애니메이션 타입"); break;
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
                    weapon.magazineAttached?.DrawHandable(DrawManager.texWrHigher, magPos, magRot, CameraManager.worldRenderState, sizeVec);
                    magazineOld?.DrawHandable(DrawManager.texWrHigher, magPos, magRot, CameraManager.worldRenderState, sizeVec);
                    //magazineNew?.DrawHandable(DrawManager.texWrHigher, magPos, magRot, CameraManager.worldRenderState, sizeVec);

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
                                if ((int)hands.master.movement.targetIndex >= 1.99f
                                    && hands.master.movement.nowValue > 0.99f)
                                    ChangeState(AnimationState.SPRINT);
                            }
                            break;
                        case AnimationState.FIRE: { } break;
                        case AnimationState.SPRINT:
                            {
                                //시간값 제어 = 질주태세
                                if ((int)hands.master.movement.targetIndex <= 1.01f)
                                    hands.time -= hands.master.gamemode.deltaTime * 2f;

                                else if (hands.time < 0.01f)
                                    hands.time += 0.02f;

                                hands.time = Mathf.Clamp(0f, hands.time, hands.timeMax);

                                //질주 딜레이 해제 시 대기 상태로
                                if (hands.time <= 0.01f) ChangeState(AnimationState.IDLE);
                            }
                            break;

                        case AnimationState.INVENTORY: { } break;
                        case AnimationState.INTERACTION: { } break;

                        case AnimationState.EQUIP: { } break;
                        case AnimationState.UNEQUIP: { } break;

                        case AnimationState.BOLT_BACK:
                            {
                                if ((int)hands.master.movement.targetIndex >= 1.99f
                                    && hands.master.movement.nowValue > 0.99f)
                                    hands.master.movement.targetIndex = Movement.MovementState.IDLE;


                                if (hands.time < weapon.status.timeDt.reloadTime.Item3 * 0.2f
                                && hands.time + hands.master.gamemode.deltaTime > weapon.status.timeDt.reloadTime.Item3 * 0.2f)
                                {
                                    DoEject();
                                }
                            }
                            break;
                        case AnimationState.BOLT_FOWARD:
                            {
                                if ((int)hands.master.movement.targetIndex >= 1.99f
                                    && hands.master.movement.nowValue > 0.99f)
                                    hands.master.movement.targetIndex = Movement.MovementState.IDLE;
                            } break;
                        case AnimationState.BOLT_ROUND:
                            {
                                if ((int)hands.master.movement.targetIndex >= 1.99f
                                    && hands.master.movement.nowValue > 0.99f)
                                    hands.master.movement.targetIndex = Movement.MovementState.IDLE;

                                if (hands.time < weapon.status.timeDt.reloadTime.Item3 * 0.5f
                                && hands.time + hands.master.gamemode.deltaTime > weapon.status.timeDt.reloadTime.Item3 * 0.5f) 
                                {
                                    DoEject();
                                }

                            } break;
                        
                        case AnimationState.MAGAZINE_ATTACH:
                            {
                                if ((int)hands.master.movement.targetIndex >= 1.99f
                                    && hands.master.movement.nowValue > 0.99f)
                                    hands.master.movement.targetIndex = Movement.MovementState.IDLE;
                            } break;
                        case AnimationState.MAGAZINE_REMOVE:
                            {
                                if ((int)hands.master.movement.targetIndex >= 1.99f
                                    && hands.master.movement.nowValue > 0.99f)
                                    hands.master.movement.targetIndex = Movement.MovementState.IDLE;
                            } break;
                        case AnimationState.MAGAZINE_CHANGE:
                            {
                                if ((int)hands.master.movement.targetIndex >= 1.99f
                                    && hands.master.movement.nowValue > 0.99f)
                                    hands.master.movement.targetIndex = Movement.MovementState.IDLE;
                            } break;
                        case AnimationState.MAGAZINE_INSPECT:
                            {
                                if ((int)hands.master.movement.targetIndex >= 1.99f
                                    && hands.master.movement.nowValue > 0.99f)
                                    hands.master.movement.targetIndex = Movement.MovementState.IDLE;
                            } break;
                    }

                    hands.time += hands.master.gamemode.deltaTime;
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
                        case AnimationState.FIRE:
                            {
                                hands.state = AnimationState.FIRE;
                                hands.time = 0f;
                                hands.timeMax = 0.02f;
                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);

                                    //볼트가 존재하고
                                    if (weapon.status.typeDt.mechanismType == MechanismType.NONE) return;

                                    //탄창이 실린더가 아니라면?
                                    if (weapon.status.typeDt.magazineType == MagazineType.SYLINDER) return;

                                    DoEject();

                                    //탄창에서 새 탄을 받음.
                                    Ammo? newAmmo = weapon.magazineAttached != null ? weapon.magazineAttached.AmmoPop() : null;

                                    if (newAmmo != null)
                                    {
                                        //받아왔다면?
                                        weapon.chambers.Add(newAmmo);

                                        if (weapon.status.typeDt.mechanismType != MechanismType.OPEN_BOLT)
                                        {
                                            weapon.boltValue = (0f, 0f);
                                        }
                                    }
                                    else
                                    {
                                        //못받아왔다면? = 탄창 empty
                                        if (weapon.status.typeDt.boltLockerType == BoltLockerType.ACTIVATE)
                                        {
                                            weapon.boltValue = (1f, 1f);
                                        }
                                    }
                                };
                            }
                            break;
                        case AnimationState.SPRINT:
                            {
                                hands.state = AnimationState.SPRINT;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.sprintTime;
                                changeStateCallback = null;
                            }
                            break;

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

                        case AnimationState.BOLT_ROUND:
                            {
                                if (weapon.isFireableState(hands.state) == false) break;
                                hands.state = AnimationState.BOLT_ROUND;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.reloadTime.Item3;
                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);

                                    //볼트가 존재하고
                                    if (weapon.status.typeDt.mechanismType == MechanismType.NONE) return;

                                    //탄창이 실린더가 아니라면?
                                    if (weapon.status.typeDt.magazineType == MagazineType.SYLINDER) return;

                                    //탄창에서 새 탄을 받음.
                                    Ammo newAmmo = weapon.magazineAttached.AmmoPop();

                                    if (newAmmo != null)
                                        //받아왔다면?
                                        weapon.chambers.Add(newAmmo);

                                    weapon.boltValue = (0f, 0f);
                                };
                            }
                            break;
                        case AnimationState.BOLT_BACK:
                            {
                                if (weapon.isFireableState(hands.state) == false) break;
                                hands.state = AnimationState.BOLT_BACK;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.reloadTime.Item3 * 0.90f;
                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);

                                    //볼트가 존재하고
                                    if (weapon.status.typeDt.mechanismType == MechanismType.NONE) return;

                                    //탄창이 실린더가 아니라면?
                                    if (weapon.status.typeDt.magazineType == MagazineType.SYLINDER) return;

                                    //볼트 고정
                                    if (weapon.status.typeDt.boltLockerType == BoltLockerType.LOCK_TO_FIRE)
                                    {
                                        //볼트 & 펌프 액션
                                        weapon.boltValue = (1f, 0f);
                                    }
                                    else
                                    {
                                        //오픈 & 클로즈드 볼트
                                        weapon.boltValue = (1f, 1f);
                                    }
                                };
                            }
                            break;
                        case AnimationState.BOLT_FOWARD:
                            {
                                if (weapon.isFireableState(hands.state) == false) break;
                                hands.state = AnimationState.BOLT_FOWARD;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.reloadTime.Item3 * 0.30f;
                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);

                                    //볼트가 존재하고
                                    if (weapon.status.typeDt.mechanismType == MechanismType.NONE) return;

                                    //탄창이 실린더가 아니라면?
                                    if (weapon.status.typeDt.magazineType == MagazineType.SYLINDER) return;

                                    //탄창에서 새 탄을 받음.
                                    Ammo newAmmo = weapon.magazineAttached.AmmoPop();

                                    if (newAmmo != null)
                                        //받아왔다면?
                                        weapon.chambers.Add(newAmmo);

                                    //볼트 고정
                                    if (weapon.status.typeDt.boltLockerType == BoltLockerType.LOCK_TO_FIRE)
                                    {
                                        //볼트 & 펌프 액션
                                        weapon.boltValue = (0f, 1f);
                                    }
                                    else
                                    {
                                        //오픈 & 클로즈드 볼트
                                        weapon.boltValue = (0f, 0f);
                                    }


                                };
                            }
                            break;

                        case AnimationState.MAGAZINE_ATTACH:
                            {
                                if (weapon.isFireableState(hands.state) == false) break;
                                if (weapon.magazineAttached != null) break;

                                //새 탄창을 가져옴.
                                magazineNew = new FN_FAL_MAG20(typeof(mm7p62x51_AP));

                                hands.state = AnimationState.MAGAZINE_ATTACH;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.reloadTime.Item2;
                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);
                                };

                            }
                            break;
                        case AnimationState.MAGAZINE_REMOVE:
                            {
                                if (weapon.isFireableState(hands.state) == false) break;
                                if (weapon.magazineAttached == null) break;

                                magazineOld = weapon.magazineAttached;
                                weapon.magazineAttached = null;

                                hands.state = AnimationState.MAGAZINE_REMOVE;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.reloadTime.Item1;

                                changeStateCallback = () =>
                                {
                                    //이전 탄창에 대한 처리
                                    magazineOld = null;

                                    ChangeState(AnimationState.IDLE);
                                };

                            }break;
                        case AnimationState.MAGAZINE_CHANGE: 
                            {
                                if (weapon.isFireableState(hands.state) == false) break;
                                if (weapon.magazineAttached == null) break;

                                magazineOld = weapon.magazineAttached;
                                weapon.magazineAttached = null;
                                magazineNew = new FN_FAL_MAG20(typeof(mm7p62x51_AP));

                                hands.state = AnimationState.MAGAZINE_CHANGE;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.reloadTime.Item1 * 0.86f + weapon.status.timeDt.reloadTime.Item2 * 0.75f;

                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);
                                };

                            }
                            break;
                        case AnimationState.MAGAZINE_INSPECT:
                            {
                                if (weapon.isFireableState(hands.state) == false) break;
                                if (weapon.magazineAttached == null) break;

                                hands.state = AnimationState.MAGAZINE_INSPECT;
                                hands.time = 0f;
                                hands.timeMax = weapon.status.timeDt.reloadTime.Item1 * 0.50f + weapon.status.timeDt.reloadTime.Item2 * 0.50f;

                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);
                                };
                            }
                            break;

                        case AnimationState.SELECTOR_CHANGE:
                            {
                                hands.state = AnimationState.SELECTOR_CHANGE;
                                hands.time = 0f;
                                hands.timeMax = 0.30f;

                                changeStateCallback = () =>
                                {
                                    int nowIndex = weapon.status.typeDt.selectorList.IndexOf(weapon.selectorNow);

                                    SelectorType newSel = weapon.status.typeDt.selectorList.Count == (nowIndex + 1) ?
                                        weapon.status.typeDt.selectorList[0] :
                                        weapon.status.typeDt.selectorList[nowIndex + 1];

                                    weapon.selectorNow = newSel;

                                    ChangeState(AnimationState.IDLE);
                                };
                            } break;
                        case AnimationState.WEAPON_INSPECT_SHORT:
                            {
                                hands.state = AnimationState.WEAPON_INSPECT_SHORT;
                                hands.time = 0f;
                                hands.timeMax = 0.60f;

                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);
                                };
                            } break;
                        case AnimationState.TDEVICE_INTERACTION:
                            {
                                hands.state = AnimationState.TDEVICE_INTERACTION;
                                hands.time = 0f;
                                hands.timeMax = 0.30f;

                                changeStateCallback = () =>
                                {
                                    ChangeState(AnimationState.IDLE);
                                };
                            } break;

                        default: break;
                    }
                }

                void DoEject() 
                {
                    //약실의 탄피 제거
                    bool isEjected = false;
                    if (weapon.chambers.Remove(weapon.chambers.Find(a => a.isUsed)))
                        isEjected = true;

                    else if (weapon.chambers.Count != 0 && weapon.chambers.Remove(weapon.chambers[0]))
                        isEjected = true;

                    if (isEjected)
                    {
                        //Y값이 뒤집힌경우 배출구 편차 보정
                        Vector2f chamberSep = (-90f <= hands.handRot && hands.handRot <= 90f) ? weapon.specialPos.ejectPos : new Vector2f(weapon.specialPos.ejectPos.X, -weapon.specialPos.ejectPos.Y);

                        //배출구 위치를 세계 좌표로 변환
                        Vector2f chamberPos = hands.master.Position + hands.handPos + chamberSep.RotateFromZero(hands.handRot);

                        new CartridgeBig(hands.master.gamemode, chamberPos, 50f);
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
        }
    }
}
