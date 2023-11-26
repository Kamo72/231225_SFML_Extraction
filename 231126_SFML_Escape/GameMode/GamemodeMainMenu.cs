using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class GamemodeMainMenu : Gamemode
    {
        public GamemodeMainMenu(TotalManager tm) : base(tm, 60) 
        {
            Random random = new Random();

            lock (boxs)
                for (int i = 0; i <= 100; i++) 
                {
                    Box box = new Box(new Vector2f(random.Next(5000) - 2500, random.Next(5000) - 2500), new Vector2f(random.Next(200) +20, random.Next(200) + 20));
                    box.Texture = ResourceManager.textures["smgIcon"];
                    box.Rotation = random.Next(360);
                    boxs.Add(box);
                }

            uis.Add(new UiTest(this, new Vector2f(100f, 100f), new Vector2f(100f, 100f)));

            entity = new Player(this, new Vector2f(0, 0));
        }
        Entity entity;
        List<Ui> uis = new List<Ui>();
        public List<Box> boxs = new List<Box>(); 

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


            //float timeValue = clock.ElapsedTime.AsSeconds();
            //Vector2f newPos = new Vector2f((float)Math.Cos(timeValue), (float)Math.Sin(timeValue)) * 300f;
            //box.Position = newPos;

            lock (boxs)
                foreach (Box box in boxs)
                    if(CameraManager.IsSkippable(box.Position) == false)
                        DrawManager.uiTex[1].Draw(box, CameraManager.worldRenderState);

        }

        protected override void LogicProcess()
        {


        }
    }
}
