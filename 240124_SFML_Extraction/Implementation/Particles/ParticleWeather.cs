using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace _231109_SFML_Test
{
    internal class SnowFall : Particle
    {
        public SnowFall(Gamemode gamemode, Vector2f position, float speed)
            : base(gamemode, 240, position, new Vector2f(1f, 1f), 0f)
        {
            rotationFixed = (180f.ToRadian() + new Vector2f(speed, -1f).ToDirection()).ToDirection();

            drawable = new CircleShape(0f);
            drawable.Radius = 10f;
            drawable.Position = position;
            drawable.Scale = new Vector2f((float)Math.Sqrt(Math.Max( Math.Abs(speed),1f)), 1f) * (float)random.NextDouble() * 1f;
            drawable.Origin = new Vector2f(1f, 1f) * drawable.Radius / 2f;
            drawable.Texture = ResourceManager.textures["LIGHT_radial"];
            drawable.FillColor = new Color(Color.White){ A = 192 };

            //Func<float> getRand = () => (float)random.NextDouble() * 2f - 1f;
            this.speedFixed = Math.Abs(speed);
        }


        float rotationFixed;
        CircleShape drawable;
        float speed, speedFixed;
        static FastNoise noise = new FastNoise(1536);


        public override void DrawProcess()
        {
            if (drawable == null) return;

            speed = speedFixed + 3f + noise.GetPerlin(drawable.Position.Y / 10f, drawable.Position.X / 10f, (VideoManager.GetTimeTotal() + lifeNow) * 1000f) / 0.5f;

            drawable.Origin = new Vector2f(1f, 1f) * drawable.Radius / 2f;
            //drawable.FillColor = new Color(Color.White) { A = 255 };
            drawable.Rotation = rotationFixed + noise.GetPerlin(drawable.Position.X / 10f, drawable.Position.Y / 10f, (VideoManager.GetTimeTotal() * 2f + lifeNow) * 2f) * 90f;
            drawable.Position += drawable.Rotation.ToRadian().ToVector() * speed;

            DrawManager.texWrEffect.Draw(drawable, CameraManager.worldRenderState);
        }

    }


}
