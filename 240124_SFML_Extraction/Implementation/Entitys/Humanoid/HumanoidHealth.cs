using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;


namespace _231109_SFML_Test
{
    internal partial class Humanoid
    {

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

                            if (master is Player) 
                            {
                                SoundManager.waveEffect.AddSound(ResourceManager.sfxs["Hitted"]);
                            }
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
