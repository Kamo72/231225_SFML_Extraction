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
using SFML.Graphics;
using SFML.System;

namespace _231109_SFML_Test
{
    internal class TextMainMenu : TextLabel
    {
        public TextMainMenu(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, size)
        {
            noise = new FastNoise(seed);
            seed += 127;
            isActivated = true;
        }

        float onMouseGauge = 0f;
        const float mGaugeDelta = 0.260f;

        public bool isActivated;
        
        FastNoise noise;
        static int seed = 1337; 
        
        protected override void LogicProcess()
        {
            onMouseGauge += IsMouseOn() && isActivated ? mGaugeDelta : -mGaugeDelta;
            onMouseGauge = Math.Max(onMouseGauge, 0f);
            onMouseGauge = Math.Min(onMouseGauge, 1f);

        }
        protected override void DrawProcess()
        {

            byte rgbValue = (byte)(240 - (1f - onMouseGauge) * 150);

            Color color = new Color(rgbValue, rgbValue, rgbValue);

            text.FillColor = color;

            Vector2f perlinNoise = new Vector2f(noise.GetPerlin(Vm.GetTimeTotal() * 8000f, 10f), noise.GetPerlin(Vm.GetTimeTotal() * 8000f, 1235f));

            text.Position = Position + perlinNoise * 8f * onMouseGauge;
        }

    }
}
