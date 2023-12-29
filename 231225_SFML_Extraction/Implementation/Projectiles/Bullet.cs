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
using SFML.System;
using SFML.Graphics;

namespace _231109_SFML_Test
{
    internal class Bullet : Projectile, ILightSource
    {
        public Bullet(Gamemode gamemode, AmmoStatus ammoStatus, Vector2f position, float rotation = 0, float speed = 0)
            : base(gamemode, 120, new Line(position, position + rotation.ToRadian().ToVector() * speed),
                  position, rotation, speed)
        {
            this.ammoStatus = ammoStatus;
            
            GamemodeIngame ingm = gamemode as GamemodeIngame;
            ingm.projs.Add(this);
            if (ammoStatus.tracer.isTraced == true)
                lock (ingm.lights)
                {
                    ingm.lights.Add(this);
                    lScale = ammoStatus.tracer.radius;
                    lColor = ammoStatus.tracer.color;
                }

            collisionCheck = CollisionCheck;
        }

        AmmoStatus ammoStatus;
        List<Entity> hittedBefore = new List<Entity>();

        public LightType lType { get; set; } = LightType.RADIAL;
        public Vector2f lPosition { get; set; }
        public float lRotation { get; set; } = 0f;
        public float lScale { get; set; } = 0f;
        public Color lColor { get; set; } = Color.Yellow;

        void CollisionCheck()
        {
            GamemodeIngame ingm = gamemode as GamemodeIngame;
            
            new MuzzleSmoke(ingm, position, rotation - 180f);

            foreach (Entity ent in ingm.entitys)
            {
                if (ent.mask.IsCollision(this.mask) == false) continue;

                if (hittedBefore.Contains(ent)) continue;

                if (ent is Humanoid human)
                {
                    Humanoid.Health.Damage damage = new Humanoid.Health.Damage()
                    {
                        damage = ammoStatus.lethality.damage,
                        pierce = ammoStatus.lethality.pierceLevel,
                        bleeding = ammoStatus.lethality.bleeding,

                        hittedPart = Humanoid.Health.HittedPart.HEAD, //조정 필요
                        damageType = Humanoid.Health.DamageType.BULLET,
                    };
                    hittedBefore.Add(ent);
                    //Console.WriteLine("데미지 체크! " + Vm.GetTimeTotal());

                    float ret = human.health.GetDamage(damage);
                    human.speed += rotation.ToRadian().ToVector() * speed.Magnitude() / 10f;
                    for (int i = 0; i < 30; i++)
                        new BloodSpray(gamemode, position, rotation);

                    Dispose();
                    break;
                }
            }

            foreach (Structure str in ingm.structures)
            {
                if (str.mask.IsCollision(this.mask))
                {
                    Dispose();
                }
            }
        }

        public override void DrawProcess()
        {
            lPosition = position;
            lRotation = rotation;
            //DrawManager.texWrEffect.Draw(mask, CameraManager.worldRenderState);
        }

        public override void LogicProcess()
        {
            if (lifeNow <= 0) Dispose();
        }
    

        public override void Dispose()
        {
            //Console.WriteLine("Projectile Destroyed! pos : " + position + "rot : " + rotation);
            base.Dispose();

            GamemodeIngame ingm = gamemode as GamemodeIngame;
            ingm.projs.Remove(this);
            if (ammoStatus.tracer.isTraced == true)
                lock (ingm.lights)
                    ingm.lights.Remove(this);
        }
    }
}
