
using SFML.System;
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
using System.Diagnostics;

namespace _231109_SFML_Test
{

    class TextLabel : Ui
    {
        public TextLabel(Gamemode gamemode, Vector2f position, Vector2f size) : base(gamemode, position, size)
        {
            text = new Text("",Rm.fonts["Jalnan"]);
            TextSet(Rm.fonts["Jalnan"], 10, Color.White, "");

            gamemode.drawEvent += TextProcess;
        }

        //텍스트 제공
        public Text text;
        public bool isMultiline = false;
        public float margin = 5f;
        string originalString;

        public void TextSet(Font font, float size, Color color, string str)
        {
            text.Font = font;
            text.FillColor = color;
            text.CharacterSize = (uint)size;
            TextSet(str);
        }
        public void TextSet(string str)
        {
            try
            {
                originalString = str;

                text.DisplayedString = originalString;
                FloatRect floatRect = text.GetGlobalBounds();

                text.Origin = new Vector2f(floatRect.Width / 2f, floatRect.Height / 2f);
            }
            catch(AccessViolationException ex)
            {
                Console.WriteLine(ex.ToString() + ex.StackTrace);
            }
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
                    
                    float uiWidth = mask.GetGlobalBounds().Width;
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

                    //줄바꿈 위치를 추측해 때려 넣는다
                    for (int i = 0; i < lineCount - 1; i++)
                    {
                        float idxRatio = (lineSize) * (i + 1) / (lineSize * lineCount + lineRemain);
                        int idxTarget = (int)Math.Ceiling(str.Length * idxRatio);

                        if (str[idxTarget] == '\n') return; //해도 의미 없어보인다는...

                        str = str.Insert(idxTarget, "\n");
                    }

                    //결과를 대입
                    text.DisplayedString = str;
                    //수정된 문자열에 맞춰서 중심을 다시 잡음
                    text.Origin = new Vector2f(text.GetGlobalBounds().Width, text.GetGlobalBounds().Height) / 2f;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        protected override void DrawProcess()
        {
            if (isDisposed) return;

            this.mask.OutlineColor = Color.White;
            this.mask.OutlineThickness = 10;
            this.mask.FillColor = new Color(0, 0, 0, 0);
        }

        protected override void LogicProcess()
        {
           text.Position = Position;
        }


        public override void Dispose()
        {
            base.Dispose();
            gamemode.drawEvent -= TextProcess;
            text?.Dispose();
        }
    }
}
