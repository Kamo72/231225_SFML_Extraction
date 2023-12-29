using SFML.Graphics;
using SFML.System;
using System;
using System.Drawing.Printing;
using Vm = _231109_SFML_Test.VideoManager;

namespace _231109_SFML_Test
{
    internal class CartridgeBig : Particle
    {
        public CartridgeBig(Gamemode gamemode, Vector2f position, float height, float rotation = 0) : base(gamemode, random.Next(500, 1000), position, new Vector2f(1f, 1f), rotation)
        {
            drawable = new RectangleShape(new Vector2f(8f, 3f));
            drawable.Origin = drawable.Size / 2f;
            drawable.Rotation = rotation;
            drawable.FillColor = Color.Yellow;


            this.height = height;
            pos2D = position + new Vector2f(0f, height * 0.4f);

            Func<float> getRand = () => (float)random.NextDouble() * 2f - 1f;

            Vector2f speed2D = (rotation + getRand() * 70f).ToRadian().ToVector() * (getRand() * 300f + 100f);
            Console.WriteLine(speed2D);
            float hSpeed = +70f + getRand() * 30f;
            speed = new Vector3f(speed2D.X, speed2D.Y, hSpeed);

            rot = rotation + getRand() * 3600f;
            rotSpeed = getRand() * 30f;
        }

        RectangleShape drawable;
        float height, gravity = -1000f;
        Vector3f speed;
        Vector2f pos2D;
        float rot, rotSpeed;

        public override void DrawProcess()
        {
            if (isDisposed) return;

            DrawManager.texWrBackground.Draw(drawable, CameraManager.worldRenderState);
            
            if (speed.Magnitude() < 30f) return;

            if (height <= 0)
            {
                Func<float> getRand = () => (float)random.NextDouble() * 2f - 1f;

                height = 0.1f;
                speed = new Vector3f(speed.X * getRand(), speed.Y * getRand(), -speed.Y) * 0.6f;
                rotSpeed *= getRand() * Math.Abs(rotSpeed) * 1.5f;
                rotSpeed = Mathf.Clamp(-3600f, rotSpeed, 3600f);
            }

            pos2D += new Vector2f(speed.X / Vm.fpsNow, speed.Y / Vm.fpsNow);
            Vector2f positionNew = pos2D;
            positionNew += new Vector2f(0f, -height * 0.4f);

            speed *= 0.99f;
            height += speed.Z / Vm.fpsNow;
            speed.Z += gravity / Vm.fpsNow;

            drawable.Position = positionNew;

            rot += rotSpeed / Vm.fpsNow;
            drawable.Rotation = rot;
        }

        public override void LogicProcess()
        {
        }

        public override void Dispose()
        {

            drawable.Dispose();
            base.Dispose();
        }
    }


}
