using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class GamemodeIngame : Gamemode
    {

        public GamemodeIngame(TotalManager tm) : base(tm, 60)
        {
            Random random = new Random();

            Ui ui = new UiTest(this, new Vector2f(100f, 100f), new Vector2f(100f, 100f));
            ui.Clicked += () =>
            {
                Sound sound = new Sound(new SoundBuffer(@"Assets\Sounds\SpiralMissileFly.ogg"));
                sound.RelativeToListener = true;
                sound.Play();
                sound.Position = new Vector3f((float)random.NextDouble() * 2f, (float)random.NextDouble() * 2f, (float)random.NextDouble() * 2f);
                Console.WriteLine(sound.ToString());
            };

            entity = new Player(this, new Vector2f(0, 0));

            ibd = new IngameBackgroundDrawer();
        }

        IngameBackgroundDrawer ibd;
        Entity entity;
        
        
        protected override void DrawProcess()
        {
            Font font = ResourceManager.fonts["Jalnan"];
            string msg =
                $"cameraPos : {CameraManager.position.X} : {CameraManager.position.Y}\n" +
                $"cameraScl : {CameraManager.size.X} : {CameraManager.size.Y}\n" +
                $"cameraRot : {CameraManager.rotation}";
            Text text = new Text(msg, font);
            text.Position = new Vector2f(VideoManager.resolutionNow.X - 600f, 0f);
            //text.Origin = new Vector2f(-110f, 0f);
            DrawManager.uiTex[1].Draw(text);

            string msgFps =
                $"fps : {VideoManager.fpsNow} ";
            Text ntext = new Text(msgFps, font);
            ntext.Position = new Vector2f(VideoManager.resolutionNow.X - 600f, 300f);
            //text.Origin = new Vector2f(-110f, 0f);
            DrawManager.uiTex[1].Draw(ntext);


            ibd.DrawBackgroundProcess();
        }

        int i = 0;
        protected override void LogicProcess()
        {
            if (i++ > 10)
            {
                i -= 10;
                ibd.RefreshBackgroundProcess();
            }
        }
    }
}
