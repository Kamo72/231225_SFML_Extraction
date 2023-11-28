using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Rm = _231109_SFML_Test.ResourceManager;
using Cm = _231109_SFML_Test.CameraManager;
using Dm = _231109_SFML_Test.DrawManager;
using Im = _231109_SFML_Test.InputManager;
using Sm = _231109_SFML_Test.SoundManager;
using Vm = _231109_SFML_Test.VideoManager;

namespace _231109_SFML_Test
{
    class UiTest : Ui 
    {
        public UiTest(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, size)
        {
            Clicked += () =>
            { 
                Random random = new Random();
                lock(((GamemodeIngame)gamemode).boxs)
                    for (int i = 0; i <= 100; i++)
                    {
                        Box box = new Box(new Vector2f(random.Next(5000) - 2500, random.Next(5000) - 2500), new Vector2f(random.Next(200) + 20, random.Next(200) + 20));
                        box.Texture = ResourceManager.textures["smgIcon"];
                        box.Rotation = random.Next(360);
                        ((GamemodeIngame)gamemode).boxs.Add(box);
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

    class UiMenuButtonTitle : Ui
    {
        public UiMenuButtonTitle(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, size)
        {
            text = new Text();
            TextSet(Rm.fonts["Jalnan"], 10, " ");

            gamemode.drawEvent += TextProcess;
        }

        //텍스트 제공
        public Text text;
        public bool isMultiline = false;
        public float margin = 5f;
        string originalString;
        public void TextSet(Font font, float size, string str)
        {
            text.Font = font;
            text.CharacterSize = (uint)size;
            TextSet(str);
        }
        public void TextSet(string str)
        {
            originalString = str;
            text.DisplayedString = originalString;
            text.Origin = new Vector2f(text.GetGlobalBounds().Width / 2, text.GetGlobalBounds().Height / 2);
        }


        //Process
        protected void TextProcess()
        {
            Dm.texUiInterface.Draw(text);
            try
            {
                if (isMultiline)
                {
                    text.DisplayedString = originalString;

                    float uiWidth = GetGlobalBounds().Width;
                    float textWidth = text.GetGlobalBounds().Width;

                    //한줄의 길이를 구해봄
                    float lineSize = uiWidth - margin * 2f;

                    if (lineSize > textWidth) return;  //한줄이라면 변화 없음

                    //일단 몇줄로 나눠지는지 마지막엔 몇글자가 남는지 구해봄
                    int lineCount = (int)Math.Ceiling(textWidth / lineSize);
                    float lineRemain = (int)(textWidth % lineSize);

                    //일단 문자열을 구한다.
                    string str = originalString;
                    if (originalString == null) return;

                    List<int> testPos = new List<int>();

                    //줄바꿈 위치를 추측해 때려 넣는다
                    for (int i = 0; i < lineCount - 1; i++)
                    {
                        float idxRatio = (lineSize) * (i + 1) / (lineSize * lineCount + lineRemain);
                        int idxTarget = (int)Math.Floor(str.Length * idxRatio);

                        if (str[idxTarget] == '\n') return; //해도 의미 없어보인다는...

                        str = str.Insert(idxTarget, "\n");
                        testPos.Add(idxTarget);
                    }
                    string debugMsg = str + lineCount + testPos.Count;
                    foreach (int i in testPos)
                        debugMsg += $"[{i}]";
                    Console.WriteLine(debugMsg);

                    //결과를 대입
                    text.DisplayedString = str;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        protected override void DrawProcess()
        {
            this.OutlineColor = Color.White;
            this.OutlineThickness = 10;
            this.FillColor = new Color(0, 0, 0, 0);

            Dm.texUiInterface.Draw(this);
        }

        protected override void LogicProcess()
        {
            text.Position = Position;
        }
    }
}
