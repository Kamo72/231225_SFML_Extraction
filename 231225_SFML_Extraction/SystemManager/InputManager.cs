using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;

namespace _231109_SFML_Test
{
    internal static class InputManager
    {
        static InputManager()
        {
            ResetCommandTable();
        }

        static Entity master;

        public static Vector2f mousePosition = new Vector2f(0, 0);
        public static Vector2f mouseDelta = new Vector2f(0, 0);
        static Vector2f mousePositionPre = new Vector2f(0, 0);

        static Vector2f mouseSpeed = new Vector2f(0.5f, 0.5f);


        public static bool mouseAllow = true;
        static Vector2f screenSize { get { return (Vector2f)VideoManager.resolutionNow; } }
        public static bool CommandCheck(CommandType cmdType)
        {
            if (Program.window.HasFocus() == false)
                return false;
            return commandDic[cmdType].Check();
        }

        #region [입력 장치 제어]
        //키값
        public class KeyData
        {
            public bool isKey;      //키인지 마우스인지
            public Keyboard.Key? keyCode; //키 코드
            public Mouse.Button? mouseCode;   //마우스 코드

            public bool isActivatedBefore;    //전 프레임에서 활성화 됐었는지?

            public KeyData(Keyboard.Key keyCode)
            {
                isKey = true;
                this.keyCode = keyCode;
                this.mouseCode = null;
                isActivatedBefore = false;
            }
            public KeyData(Mouse.Button mouseCode)
            {
                isKey = false;
                this.keyCode = null;
                this.mouseCode = mouseCode;
                isActivatedBefore = false;
            }

            public bool IsActivated()
            {
                return isKey ? Keyboard.IsKeyPressed((Keyboard.Key)keyCode) : Mouse.IsButtonPressed((Mouse.Button)mouseCode);
            }
        }
        //키 읽기 방식
        public enum KeyReadType
        {
            PRESS,      //전 프레임 비활성화 / 이번 프레임 활성화
            PRESSING,   //그냥 활성화
            RELEASE,    //전 프레임  활성화 / 이번 프레임 비활성화
            DOUBLE,     //최근 Press클릭 시점을 저장. Press 시, 가깝다면 활성화
            TOGGLE,     //Press와 같긴 한데, 토글이라는 개념이 중간에 추가됨.

            /* PRESS는 PRESSing으로 작동
             * RELEASE는 작동 X
             * TOGGLE는 PRESSING 작동
             * DOUBLE는 작동 X
             */
        }
        //조작 정보
        public class CommandData
        {
            public string commandName;

            public KeyData firstKey;
            public KeyData secondKey;

            public KeyReadType keyReadType;
            public List<KeyReadType> keyReadWhiteList;

            //DOUBLE
            public const float doubleClickDelay = 0.3f;
            private float doubleClickPast;

            //TOGGLE
            private bool toggleActivated;


            public CommandData(string commandName, KeyReadType keyReadType, KeyData firstKey = null, KeyData secondKey = null, List<KeyReadType> whiteList = null)
            {
                this.keyReadType = keyReadType;
                this.commandName = commandName;
                this.firstKey = firstKey;
                this.secondKey = secondKey;

                keyReadWhiteList = whiteList;

                doubleClickPast = 9999f;
                toggleActivated = false;
            }

            public void ToggleRealease()
            {
                toggleActivated = false;
            }

            public bool Check()
            {
                bool result;
                KeyData keyToTry = secondKey != null ? (KeyData)secondKey : (KeyData)firstKey;

                if (keyReadType == KeyReadType.RELEASE) Console.WriteLine("Release Test : tryResult " + keyToTry.IsActivated() + " && keyToTry.isActivatedBefore " + keyToTry.isActivatedBefore);
                //키 없음
                if (firstKey == null && secondKey == null)
                {
                    return false;
                }

                //키 두개
                if (firstKey != null && secondKey != null)
                {
                    //첫 키가 눌리지 않았으면 return false
                    if (((KeyData)firstKey).IsActivated() == false)
                    {
                        return false;
                    }
                }

                //keyToTry 키 검사
                //키 입력 결과
                bool tryResult = keyToTry.IsActivated();

                //키 입력 검사(keyToTry)
                switch (keyReadType)
                {
                    case KeyReadType.PRESS:
                        result = tryResult && keyToTry.isActivatedBefore == false;
                        break;

                    case KeyReadType.PRESSING:
                        result = tryResult;
                        break;

                    case KeyReadType.DOUBLE:
                        result = tryResult && keyToTry.isActivatedBefore == false && (doubleClickPast < doubleClickDelay);
                        break;

                    case KeyReadType.RELEASE:
                        result = tryResult == false && keyToTry.isActivatedBefore;
                        break;

                    case KeyReadType.TOGGLE:
                        if (tryResult && keyToTry.isActivatedBefore == false)
                            toggleActivated = !toggleActivated;
                        result = toggleActivated;
                        break;
                    
                    default:
                        result = false;
                        break;
                }

                return result;
            }

            //Check실행 후에 이전 키의 입력을 최신화
            public void AfterCheckProcess()
            {
                if (firstKey != null)
                    firstKey.isActivatedBefore = firstKey.IsActivated();
                if (secondKey != null)
                    secondKey.isActivatedBefore = secondKey.IsActivated();

                doubleClickPast += 1f / 60f;

                KeyData keyToCheck = secondKey ?? firstKey;
                if (keyToCheck.IsActivated()) doubleClickPast = doubleClickPast < doubleClickDelay? 0f : 0f;
            }

        }
        //조작값
        public enum CommandType
        {
            MOVE_FORWARD,
            MOVE_BACKWARD,
            MOVE_RIGHT,
            MOVE_LEFT,

            SPRINT,
            CROUNCH,
            PRONE,

            INTERACT,

            FIRE,
            AIM,

            MAGAZINE_CHANGE,
            MAGAZINE_INSPECT,
            MAGAZINE_REMOVE,

            SELECTOR_CHANGE,
            SELECTOR_INSPECT,

            MELEE,
            GRANADE,
            TACTIC,
        }

        //조작 리스트
        public static Dictionary<CommandType, CommandData> commandDic;

        public static void ResetCommandTable()
        {
            commandDic = new Dictionary<CommandType, CommandData>();

            commandDic[CommandType.MOVE_FORWARD] = new CommandData("앞으로 이동", KeyReadType.PRESSING, new KeyData(Keyboard.Key.W));
            commandDic[CommandType.MOVE_BACKWARD] = new CommandData("뒤로 이동", KeyReadType.PRESSING, new KeyData(Keyboard.Key.S));
            commandDic[CommandType.MOVE_LEFT] = new CommandData("왼쪽으로 이동", KeyReadType.PRESSING, new KeyData(Keyboard.Key.A));
            commandDic[CommandType.MOVE_RIGHT] = new CommandData("오른쪽으로 이동", KeyReadType.PRESSING, new KeyData(Keyboard.Key.D));

            //commandDic[CommandType.SPRINT] = new CommandData("달리기", KeyReadType.TOGGLE, new KeyData(KeyCode.LeftShift));
            //commandDic[CommandType.CROUNCH] = new CommandData("숙이기", KeyReadType.TOGGLE, new KeyData(KeyCode.C));
            //commandDic[CommandType.PRONE] = new CommandData("포복", KeyReadType.TOGGLE, new KeyData(KeyCode.X));

            //commandDic[CommandType.INTERACT] = new CommandData("상호작용", KeyReadType.PRESS, new KeyData(KeyCode.F));

            commandDic[CommandType.FIRE] = new CommandData("격발", KeyReadType.DOUBLE, new KeyData(Mouse.Button.Left));
            commandDic[CommandType.AIM] = new CommandData("조준", KeyReadType.PRESSING, new KeyData(Mouse.Button.Right));
            //commandDic[CommandType.MAGAZINE_CHANGE] = new CommandData("재장전", KeyReadType.PRESS, new KeyData(KeyCode.R));
            //commandDic[CommandType.MAGAZINE_INSPECT] = new CommandData("잔탄 확인", KeyReadType.PRESS, new KeyData(KeyCode.LeftControl), new KeyData(KeyCode.R));
            //commandDic[CommandType.MAGAZINE_REMOVE] = new CommandData("탄창 제거", KeyReadType.PRESS, new KeyData(KeyCode.LeftAlt), new KeyData(KeyCode.R));

            //commandDic[CommandType.MELEE] = new CommandData("근접 공격", KeyReadType.PRESS, new KeyData(KeyCode.V));
            //commandDic[CommandType.GRANADE] = new CommandData("살상 장비", KeyReadType.RELEASE, new KeyData(KeyCode.E));
            //commandDic[CommandType.TACTIC] = new CommandData("전술 장비", KeyReadType.RELEASE, new KeyData(KeyCode.Q));
        }

        #endregion

        public static void RefreshProcess()
        {
            try
            {
                foreach (CommandType cmdType in commandDic.Keys)
                    commandDic[cmdType].AfterCheckProcess();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + ex.StackTrace);
            }
        }
        public static void DebugProcess()
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                lock (Program.window)
                {
                    Program.window.Close();
                }
            }

            //if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            //{
            //    CameraManager.position.Y -= 10;
            //}
            //if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            //{
            //    CameraManager.position.Y+=10;
            //}


            //if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            //{
            //    CameraManager.position.X -= 10;
            //}
            //if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            //{
            //    CameraManager.position.X+=10;
            //}

            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                CameraManager.rotation--;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.E))
            {
                CameraManager.rotation++;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.R))
            {
                CameraManager.zoomValue *= 1.001f;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.F))
            {
                CameraManager.zoomValue /= 1.001f;
            }


        }
        public static void MouseProcess()
        {
            Vector2f mouseSpeed = new Vector2f(
                1.0f * InputManager.mouseSpeed.X,
                1.0f * InputManager.mouseSpeed.Y
                ); //감도 보정

            mousePosition = (Vector2f)Mouse.GetPosition();

            //마우스 변위 대입
            mouseDelta = new Vector2f(
                (mousePosition.X - mousePositionPre.X) * mouseSpeed.X,
                (mousePosition.Y - mousePositionPre.Y) * mouseSpeed.Y
                );

            //마우스 고정 중이라면 중앙에 고정 
            if (mouseAllow == false)
            {
                Mouse.SetPosition(VideoManager.resolutionNow / 2);
            }


            //마우스 위치 기억
            mousePositionPre = (Vector2f)Mouse.GetPosition();
        }

        public static Action inputManagerProcess = ()=>
        {
            if (Program.window.HasFocus())
            {
                InputManager.RefreshProcess();
                InputManager.DebugProcess();
            }
        };
    }
}
