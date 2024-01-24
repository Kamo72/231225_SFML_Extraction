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
            public float sprintTime => weapon == null ? 0.070f : weapon.status.timeDt.sprintTime;


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

    }
}
