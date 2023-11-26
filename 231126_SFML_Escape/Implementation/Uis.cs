using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    class UiTest : Ui 
    {
        public UiTest(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, size)
        {
            Clicked += () =>
            { 
                Random random = new Random();
                lock(((GamemodeMainMenu)gamemode).boxs)
                    for (int i = 0; i <= 100; i++)
                    {
                        Box box = new Box(new Vector2f(random.Next(5000) - 2500, random.Next(5000) - 2500), new Vector2f(random.Next(200) + 20, random.Next(200) + 20));
                        box.Texture = ResourceManager.textures["smgIcon"];
                        box.Rotation = random.Next(360);
                        ((GamemodeMainMenu)gamemode).boxs.Add(box);
                    }
                CameraManager.GetShake(10f);
            };

        }

        protected override void DrawProcess()
        {
            DrawManager.uiTex[1].Draw(this);
        }

        protected override void LogicProcess()
        {
        }
    }
}
