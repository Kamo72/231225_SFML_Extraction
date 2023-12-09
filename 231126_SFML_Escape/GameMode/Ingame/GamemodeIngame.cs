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

        public GamemodeIngame(TotalManager tm) : base(tm, 90)
        {
            //랜덤
            Random random = new Random();

            //테스트 UI
            Ui ui = new UiTest(this, new Vector2f(100f, 100f), new Vector2f(100f, 100f));
            ui.Clicked += () =>
            {
                SoundManager.waveEffect.AddSound( ResourceManager.sfxs["SpiralMissileFly"], Vector2fEx.Zero, 1000f);
            };

            //플레이어 객체 생성
            Entity entity = new Player(this, new Vector2f(0, 0));
            SoundManager.listener = entity;
            
            //배경 그리기 객체
            ibd = new IngameBackgroundDrawer();

            //마우스 허용 안됨.
            InputManager.mouseAllow = false;


            //벽 테스트
            structures.Add(new ConcreteBox(this, new Vector2f(+300f, +300f), new Vector2f(300f, 300f)));
            structures.Add(new ConcreteBox(this, new Vector2f(+300f, -300f), new Vector2f(300f, 300f)));
            structures.Add(new ConcreteBox(this, new Vector2f(-300f, +300f), new Vector2f(300f, 300f)));
            structures.Add(new ConcreteBox(this, new Vector2f(-300f, -300f), new Vector2f(300f, 300f)));



            //상속 
            Console.WriteLine("P : B  C : A" + TypeEx.IsSubclassOfRawGeneric(typeof(Bbb), typeof(Aaa)));
            Console.WriteLine("P : A  C : B" + TypeEx.IsSubclassOfRawGeneric(typeof(Aaa), typeof(Bbb)));
            Console.WriteLine("P : A  C : A" + TypeEx.IsSubclassOfRawGeneric(typeof(Aaa), typeof(Aaa)));






            //무기 테스트
            fnfal = new FN_FAL();
        }

        class Aaa { }
        class Bbb : Aaa { }



        IngameBackgroundDrawer ibd;

        public List<Structure> structures = new List<Structure>();
        public List<Entity> entitys = new List<Entity>();
        public Weapon fnfal;


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
            DrawManager.texUiInterface.Draw(text);

            string msgFps =
                $"fps : {VideoManager.fpsNow} ";
            Text ntext = new Text(msgFps, font);
            ntext.Position = new Vector2f(VideoManager.resolutionNow.X - 600f, 300f);
            //text.Origin = new Vector2f(-110f, 0f);
            DrawManager.texUiInterface.Draw(ntext);

            fnfal.DrawTopSprite(DrawManager.texUiInterface, new Vector2f(500f, 500f), Vector2fEx.Zero);
            
            







            ibd.DrawBackgroundProcess();
        }

        int i = 0;
        protected override void LogicProcess()
        {

            if (i++ > 40)
            {
                i -= 40;
                ibd.RefreshBackgroundProcess();
            }
        }
    }
}
