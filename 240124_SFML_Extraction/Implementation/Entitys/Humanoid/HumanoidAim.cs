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


            //이동
            Movement.MovementData moveData => master.movement.nowMovement;
            float moveValue => master.movement.nowValue;
            public float sprintTime => status.timeDt.sprintTime;
            public float moveRatio => master.movement.speed.Magnitude() / 100f / 3.7f;


            //ADS 전환 및 값 -1 : 질주, 0 : 대기, 1 : 조준
            //딜레이 -1 ~ 0 : 질주 전환 / 0 ~ 1 : 조준 전환
            public float adsValue = 0f;
            public float adsTime => status.timeDt.adsTime;
            public bool isAds = false;

            public void AdsProcess()
            {
                float deltaTime = VideoManager.GetTimeDelta();

                //조준 중(1으로 수렴)
                if (isAds)
                {
                    adsValue =
                        adsValue < 0.01f ?
                            Math.Min(adsValue + 1f / sprintTime * deltaTime, 1f) :
                            Math.Min(adsValue + 1f / adsTime * deltaTime, 1f);
                    master.movement.targetIndex = Movement.MovementState.IDLE;
                }
                //질주 중(-1으로 수렴)
                else if (master.movement.targetIndex == Movement.MovementState.SPRINT)
                    adsValue =
                        adsValue < -0.01f ?
                            Math.Max(adsValue - 1f / sprintTime * deltaTime, -1f) :
                            Math.Max(adsValue - 1f / adsTime * deltaTime, -1f);

                //둘 모두 아닌 상태(0으로 수렴)
                else
                    adsValue =
                        adsValue < -0.01f ?
                            Math.Min(adsValue + 1f / sprintTime * deltaTime, 0f) :
                        adsValue > 0.01f ?
                            Math.Max(adsValue - 1f / adsTime * deltaTime, 0f) :
                            adsValue = 0.00f;
                
            }



            //실제 마우스 점, 동적 마우스 점;
            public Vector2f staticDot => master.aimPosition;
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

                if (master is Player player)
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
                        float adjustRatio = Mathf.PercentMultiflex(4f, ratio);

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
                    CameraManager.GetShake(hipRecoilStrength / 4f);
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

                    if (moveRatio > 0.01f) //이동 중
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
                time += VideoManager.GetTimeDelta() * adsStanceAccuracy + (1f + recoilVec.Magnitude() / 10f);

                float x = adsStanceAccuracy * noise.GetPerlin(0f, time);
                float y = adsStanceAccuracy * noise.GetPerlin(1335f, time);

                adsStanceVec = new Vector2f(x, y) * master.aimPosition.Magnitude() / 1000f;
                //Console.WriteLine($"x : {x}, y : {y}");
            }


            #endregion


        }
    }
}
