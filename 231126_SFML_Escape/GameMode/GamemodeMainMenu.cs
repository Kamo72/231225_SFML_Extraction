using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;
using SFML.Audio;

namespace _231109_SFML_Test
{
    internal class GamemodeMainMenu : Gamemode
    {
        public GamemodeMainMenu(TotalManager tm) : base(tm, 60) 
        {
            Random random = new Random();

            Console.WriteLine($"GamemodeMainMenu Init >>> ");
            lock (boxs)
                for (int i = 0; i <= 100; i++)
                {
                    Console.WriteLine($"GamemodeMainMenu Init 1 >>> ");
                    Box box = new Box(new Vector2f(random.Next(5000) - 2500, random.Next(5000) - 2500), new Vector2f(random.Next(200) +20, random.Next(200) + 20));

                    Console.WriteLine($"GamemodeMainMenu Init 2 >>> ");
                    box.Texture = ResourceManager.textures["smgIcon"];

                    Console.WriteLine($"GamemodeMainMenu Init 3 >>> ");
                    box.Rotation = random.Next(360);

                    Console.WriteLine($"GamemodeMainMenu Init 4 >>> ");
                    boxs.Add(box);
                }

            Console.WriteLine($"GamemodeMainMenu Init 5 >>> ");
            Ui ui = new UiTest(this, new Vector2f(100f, 100f), new Vector2f(100f, 100f));
            ui.Clicked += () =>
            {
                Sound sound = new Sound(new SoundBuffer(@"Assets\Sounds\SpiralMissileFly.ogg"));
                sound.RelativeToListener = true;
                sound.Play();
                sound.Position = new Vector3f((float)random.NextDouble() * 2f, (float)random.NextDouble() * 2f, (float)random.NextDouble() * 2f)    ;
                Console.WriteLine(sound.ToString());    
                sounds.Add(sound);
            };
            uis.Add(ui);



            //entity = new Player(this, new Vector2f(0, 0));

            Console.WriteLine($"GamemodeMainMenu Init End >>> ");
        }
        Entity entity;
        List<Ui> uis = new List<Ui>();
        public List<Box> boxs = new List<Box>(); 
        public List<Sound> sounds = new List<Sound>();
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
                $"fps : {VideoManager.fpsNow} - Boxs.Count : {boxs.Count}";
            Text ntext = new Text(msgFps, font);
            ntext.Position = new Vector2f(VideoManager.resolutionNow.X - 600f, 300f);
            //text.Origin = new Vector2f(-110f, 0f);
            DrawManager.uiTex[1].Draw(ntext);


            lock (boxs)
                foreach (Box box in boxs)
                    if(CameraManager.IsSkippable(box.Position) == false)
                        DrawManager.uiTex[1].Draw(box, CameraManager.worldRenderState);

        }

        protected override void LogicProcess()
        {
            Console.WriteLine($"GamemodeMainMenu LogicProcess >>> ");


        }
    }
}
