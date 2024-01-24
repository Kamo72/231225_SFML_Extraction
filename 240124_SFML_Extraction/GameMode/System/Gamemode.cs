using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;
using Timer = System.Timers.Timer;

namespace _231109_SFML_Test
{
    internal abstract class Gamemode : IDisposable
    {
        protected TotalManager totalManager;

        public Gamemode(TotalManager tm, double logicFps)
        {
            totalManager = tm;
            tm.gmNow = this;
            Console.WriteLine("gamemode Now is " + tm.gmNow.GetType().ToString());
            this.logicFps = logicFps;

            deltaClock = new Clock();

            //로직 타이머 시작
            timer = new Timer(1000d / logicFps);
            timer.Elapsed += (s, e) => {
                try
                {
                    logicEvent?.Invoke();   //로직 처리 호출
                    InputManager.inputManagerProcess();


                    deltaTime = deltaClock.ElapsedTime.AsSeconds();
                    deltaClock.Restart();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message + ex.StackTrace); }
            };
            timer.Start();

            clock = new Clock();
            logicEvent += LogicProcess;
            drawEvent += DrawProcess;
            drawEvent += InputManager.MouseProcess;


            InputManager.mouseAllow = true;
        }

        //로직 타이머 관련 변수
        public readonly double logicFps;
        Timer timer;

        protected Clock clock, deltaClock;

        public float deltaTime = 0f;

        //로직, 드로우, 드로우 UI 등의 이벤트
        public event Action logicEvent;
        public event Action drawEvent;

        //TM에서 접근하는 드로우 이벤트
        public void DoDraw()
        {
            drawEvent?.Invoke();   //월드 드로우 처리 호출
        }

        protected abstract void LogicProcess();
        protected abstract void DrawProcess();


        //소멸자
        public void DisposablesAdd(IDisposable disposable) { disposables.Add(disposable); }
        public void DisposablesRemove(IDisposable disposable) { disposables.Remove(disposable); }

        List<IDisposable> disposables = new List<IDisposable>();
        ~Gamemode() 
        {
            Dispose();
        }
        public virtual void Dispose()
        {
            Console.WriteLine("Debug - Gamemode Dipose Init");
            if (totalManager.gmNow == this) { totalManager.gmNow = null; }

            Console.WriteLine("Debug - To Diposable heap - " + disposables.Count);
            while (disposables.Count > 0)
            {
                if (disposables[0] == null) 
                {
                    disposables.RemoveAt(0);
                    continue;
                }
                Console.WriteLine("Debug - Dipose  heap - " + disposables[0].ToString());
                IDisposable disposable = disposables[0];

                if (disposable is TextLabel ui) ui.Dispose();
                else disposables[0].Dispose();

                //disposables.RemoveAt(0);

            }

            Console.WriteLine("Debug - To Diposable heap - " + disposables.Count);


            timer.Dispose();
            GC.SuppressFinalize(this);
            Console.WriteLine("Debug - Gamemode Dipose End");

            GC.Collect();
        }
    }
}
