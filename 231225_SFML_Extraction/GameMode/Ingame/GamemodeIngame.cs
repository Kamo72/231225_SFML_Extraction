using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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


            new TestEnemy(this, new Vector2f(1000, 1000));
            new TestEnemy(this, new Vector2f(-1000, 1000));
            new TestEnemy(this, new Vector2f(1000, -1000));
            new TestEnemy(this, new Vector2f(-1000, -1000));

            //배경 그리기 객체
            ibd = new IngameBackgroundDrawer();
            ild = new IngameLightDrawer(lights, structures);

            //마우스 허용 안됨.
            InputManager.mouseAllow = false;

            //벽 테스트
            structures.Add(new ConcreteBox(this, new Vector2f(+300f, +300f), new Vector2f(300f, 300f)));
            structures.Add(new ConcreteBox(this, new Vector2f(+300f, -300f), new Vector2f(300f, 300f)));
            structures.Add(new ConcreteBox(this, new Vector2f(-300f, +300f), new Vector2f(300f, 300f)));
            structures.Add(new ConcreteBox(this, new Vector2f(-300f, -300f), new Vector2f(300f, 300f)));

            spawner = new Timer(2000f);
            spawner.Elapsed += (s,e)=>
            {
                if (entitys.Count < 5) new TestEnemy(this, new Vector2f(0, 0));
            };
            spawner.Start();
        }

        Timer spawner;

        IngameBackgroundDrawer ibd;
        IngameLightDrawer ild;
        public List<ILightSource> lights = new List<ILightSource>();

        public List<Structure> structures = new List<Structure>();
        public List<Entity> entitys = new List<Entity>();
        public List<Projectile> projs = new List<Projectile>();


        protected override void DrawProcess()
        {
            Font font = ResourceManager.fonts["Jalnan"];
            string msg =
                $"cameraPos : {CameraManager.position.X} : {CameraManager.position.Y}\n" +
                $"cameraScl : {CameraManager.size.X} : {CameraManager.size.Y}\n" +
                $"cameraRot : {CameraManager.rotation}\n";
            Text text = new Text(msg, font);
            text.Position = new Vector2f(VideoManager.resolutionNow.X - 600f, 0f);
            //text.Origin = new Vector2f(-110f, 0f);
            DrawManager.texUiInterface.Draw(text);

            string msgFps =
                $"fps : {VideoManager.fpsNow}";
            Text ntext = new Text(msgFps, font);
            ntext.Position = new Vector2f(VideoManager.resolutionNow.X - 600f, 300f);
            //text.Origin = new Vector2f(-110f, 0f);
            DrawManager.texUiInterface.Draw(ntext);


            //FN_FAL fN_FAL = new FN_FAL();
            //fN_FAL.DrawHandable(DrawManager.texUiInterface, new Vector2f(500f, 500), VideoManager.GetTimeTotal() * 100f, new Vector2f(1f, -1f) * 10f);


           

            ibd.DrawBackgroundProcess();
            ild.Draw();
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

        public override void Dispose()
        {
            if (ibd != null) ibd.Dispose();
            base.Dispose();
        }
    }
}
