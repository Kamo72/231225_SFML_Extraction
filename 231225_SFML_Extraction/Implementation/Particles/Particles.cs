using SFML.Graphics;
using SFML.System;
using System;
using Vm = _231109_SFML_Test.VideoManager;

namespace _231109_SFML_Test
{
    internal class BloodSplit : Particle
    {
        public BloodSplit(Gamemode gamemode, Vector2f position, float height, float rotation = 0) : base(gamemode, random.Next(50, 100), position, new Vector2f(1f, 1f), rotation)
        {
            drawable = new Line(position, position);
            drawable.fillColor = Color.White;
            drawable.thickness = 1f;
            this.height = height;

            Func<float> getRand = () => (float)random.NextDouble() * 2f - 1f;

            Vector2f speed2D = (rotation + getRand() * 70f).ToRadian().ToVector() * (getRand() * 300f + 100f);
            Console.WriteLine(speed2D);
            float hSpeed = +7f + getRand() * 3f;
            speed = new Vector3f(speed2D.X, speed2D.Y, hSpeed);

        }

        Line drawable;
        float height, gravity = -100f;
        Vector3f speed;

        public override void DrawProcess()
        {
            drawable.positionFrom = drawable.positionTo;
            Vector2f positionNew;

            if (height <= 0) speed = new Vector3f(0f, 0f, 0f);

            positionNew = drawable.positionFrom;
            positionNew += new Vector2f(speed.X / Vm.fpsNow, speed.Y / Vm.fpsNow);
            positionNew += new Vector2f(0f, height * 0.002f);

            speed *= 0.99f;
            height += speed.Z / Vm.fpsNow;
            speed.Z += gravity / Vm.fpsNow;

            drawable.positionTo = positionNew;

            DrawManager.texWrEffect.Draw(drawable, CameraManager.worldRenderState);

        }

    }


    internal class BloodSpray : Particle
    {
        public BloodSpray(Gamemode gamemode, Vector2f position, float rotation = 0) : base(gamemode, random.Next(14, 28), position, new Vector2f(1f, 1f), rotation)
        {
            radMax = (float)random.NextDouble() * 10f;
            drawable = new CircleShape(0f);
            drawable.Position = position;

            drawable.Origin = new Vector2f(1f, 1f) * drawable.Radius / 2f;
            drawable.Texture = ResourceManager.textures["LIGHT_radial"];

            Func<float> getRand = () => (float)random.NextDouble() * 2f - 1f;

            speed = (rotation + getRand() * 70f).ToRadian().ToVector() * (getRand() * 300f + 100f);

        }
        float radMax;
        CircleShape drawable;
        Vector2f speed;

        public override void DrawProcess()
        {
            if (drawable == null) return;

            drawable.Radius = radMax * ((float)lifeNow / lifeMax);
            byte alpha = (byte)(255 * (float)lifeNow / lifeMax);
            drawable.FillColor = new Color(255, 0, 0, alpha);
            //drawable.FillColor = Color.White;

            Vector2f positionNew = drawable.Position;


            positionNew += new Vector2f(speed.X / Vm.fpsNow, speed.Y / Vm.fpsNow);
            speed *= 0.99f;

            drawable.Position = positionNew;

            DrawManager.texWrEffect.Draw(drawable, CameraManager.worldRenderState);

        }

    }


    internal class MuzzleSmoke : Particle
    {
        public MuzzleSmoke(Gamemode gamemode, Vector2f position, float rotation = 0) : base(gamemode, random.Next(10, 50), position, new Vector2f(1f, 1f), rotation)
        {
            radMax = (float)random.NextDouble() * 30f;
            drawable = new CircleShape(0f);
            drawable.Position = position;

            drawable.Origin = new Vector2f(1f, 1f) * drawable.Radius / 2f;
            drawable.Texture = ResourceManager.textures["LIGHT_radial"];

            Func<float> getRand = () => (float)random.NextDouble() * 2f - 1f;

            speed = (rotation + getRand() * 90).ToRadian().ToVector() * (getRand() * 200f + 50f);

        }
        float radMax;
        CircleShape drawable;
        Vector2f speed;

        public override void DrawProcess()
        {
            if (drawable == null) return;

            drawable.Radius = radMax * (1.5f - (float)lifeNow / lifeMax) / 1.5f ;
            drawable.Origin = new Vector2f(drawable.Radius / 2f, drawable.Radius / 2f);
            byte alpha = (byte)(255 * (float)lifeNow / lifeMax);
            drawable.FillColor = new Color(170, 170, 170, (byte)(alpha / 2));
            //drawable.FillColor = Color.White;

            Vector2f positionNew = drawable.Position;

            positionNew += new Vector2f(speed.X / Vm.fpsNow, speed.Y / Vm.fpsNow);
            speed += ((float)random.NextDouble() * 360f).ToRadian().ToVector();
            speed *= 0.94f;

            drawable.Position = positionNew;

            DrawManager.texWrEffect.Draw(drawable, CameraManager.worldRenderState);
        }

    }

    internal class MuzzleFlash : Particle, ILightSource
    {
        public MuzzleFlash(Gamemode gamemode, Vector2f position, float scale, float rotation = 0)
            : base(gamemode, 3, position, Vector2fEx.Zero, rotation)
        {
            lPosition = position;
            GamemodeIngame gm = gamemode as GamemodeIngame;
            gm.lights.Add(this);
            lScale = scale *((float)random.NextDouble() * 0.4f + 0.8f);
        }


        public LightType lType { get; set; } = LightType.RADIAL;
        public Vector2f lPosition { get; set; }
        public float lRotation { get; set; }
        public float lScale { get; set; }
        public Color lColor { get; set; } = new Color(255, 123, 0, 255);

        public override void DrawProcess()
        {
        }


        public override void Dispose()
        {
            GamemodeIngame gm = gamemode as GamemodeIngame;
            gm.lights.Remove(this);
            base.Dispose();
        }
    }
}
