using SFML.Graphics;
using SFML.System;
using SFML.Window;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SFML.Graphics.Glsl;

namespace _231109_SFML_Test
{
    internal static class DrawManager
    {
        static DrawManager()
        {
            //해상도 변경될 때마다 자동으로 텍스쳐 조정
            VideoManager.ChangedResolution += ResolutionChanged;

            ResolutionChanged(VideoManager.resolutionNow);
            LoadShaders();
        }

        public static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        public static void LoadShaders() 
        {
            try
            {
                string path = "S:\\[GitHub]\\231225_SFML_Extraction\\231225_SFML_Extraction\\Shaders\\";

                string vertex = "void main()"
                                +"{"
                                +    "gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;"
                                +    "gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;"
                                +    "gl_FrontColor = gl_Color;"
                                +"}";

                string frag = "uniform vec4 color;"
                                +"uniform float expand;"
                                +"uniform vec2 center;"
                                +"uniform float radius;"
                                +"uniform float windowHeight;"
                                +"void main(void)"
                                +"{"
                                +"vec2 centerFromSfml = vec2(center.x, windowHeight - center.y);"
                                +"vec2 p = (gl_FragCoord.xy - centerFromSfml) / radius;"
                                +    "float r = sqrt(dot(p, p));"
                                +    "if (r < 1.0)"
                                +    "{"
                                +        "gl_FragColor = mix(color, gl_Color, (r - expand) / (1 - expand));"
                                +    "}"
                                +    "else"
                                +    "{"
                                +        "gl_FragColor = gl_Color;"
                                +    "}"
                                +"}";

                //string  geom =   "#version 330 core\r\nlayout(points) in;\r\nlayout(triangle_strip, max_vertices = 3) out;\r\n\r\nvoid main()\r\n{\r\n    gl_Position = gl_in[0].gl_Position;\r\n    EmitVertex();\r\n\r\n    gl_Position = gl_in[0].gl_Position + vec4(1.0, 0.0, 0.0, 0.0);\r\n    EmitVertex();\r\n\r\n    gl_Position = gl_in[0].gl_Position + vec4(0.0, 1.0, 0.0, 0.0);\r\n    EmitVertex();\r\n\r\n    EndPrimitive();\r\n}";
                //        geom =   "#version 330 core\r\nlayout (points) in;\r\nlayout (line_strip, max_vertices = 2) out;\r\n\r\nvoid main() {    \r\n    gl_Position = gl_in[0].gl_Position + vec4(-0.1, 0.0, 0.0, 0.0); \r\n    EmitVertex();\r\n\r\n    gl_Position = gl_in[0].gl_Position + vec4( 0.1, 0.0, 0.0, 0.0);\r\n    EmitVertex();\r\n    \r\n    EndPrimitive();\r\n} ";
                
                Shader shader = Shader.FromString(vertex, null, frag);
                shaders["gradCircle"] = shader;
                

            }
            catch (Exception ex){ Console.WriteLine(ex.Message + ex.StackTrace); }
        }

        //해상도에 맞게 텍스쳐 초기화
        public static void ResolutionChanged(Vector2i resolution)
        {
            //이전의 텍스쳐 Dispose
            for (int idx = 0; idx < uiTex.Length; idx++)
                uiTex[idx]?.Dispose();
            for (int idx = 0; idx < worldTex.Length; idx++)
                worldTex[idx]?.Dispose();

            resultTex?.Dispose();


            //새로운 텍스쳐 생성
            for (int idx = 0; idx < uiTex.Length; idx++)
                uiTex[idx] = new RenderTexture((uint)resolution.X, (uint)resolution.Y);

            for (int idx = 0; idx < worldTex.Length; idx++)
                worldTex[idx] = new RenderTexture((uint)resolution.X, (uint)resolution.Y);

            resultTex = new RenderTexture((uint)resolution.X, (uint)resolution.Y);
        }

        //입력받을 텍스쳐

        #region [텍스쳐 파라미터]
        public static RenderTexture texUiBackground { get { return uiTex[0]; } }
        public static RenderTexture texUiInterface { get { return uiTex[1]; } }
        public static RenderTexture texUiPopup { get { return uiTex[2]; } }
        public static RenderTexture texUiWhole { get { return uiTex[3]; } }
        public static RenderTexture texWrBackground { get { return worldTex[0]; } }
        public static RenderTexture texWrLower { get { return worldTex[1]; } }
        public static RenderTexture texWrHigher { get { return worldTex[2]; } }
        public static RenderTexture texWrEffect { get { return worldTex[3]; } }
        public static RenderTexture texWrLight { get { return worldTex[4]; } }
        public static RenderTexture texWrAugment { get { return worldTex[5]; } }
        #endregion

        public static RenderTexture[] uiTex = new RenderTexture[4];     //0 = 화면 효과 1 = UI, 2 = 팝업 3 = 전체 효과
        public static RenderTexture[] worldTex = new RenderTexture[6];  //0 = 배경, 1 = 바닥 1 = 동적 2 = 효과 3 = 증강 효과
        public static RenderTexture resultTex;


        //입력받은 텍스쳐 초기화 및 결과 텍스쳐 반환
        public static void ResultTexture()
        {
            //결과를 담을 텍스쳐 생성
            Vector2f resolution = (Vector2f)VideoManager.resolutionNow;
            resultTex.Clear();

            //레이어들을 결과 텍스쳐에 도합
            for (int idx = 0; idx < worldTex.Length; idx++)
                resultTex.Draw(new Sprite(worldTex[idx].Texture, new IntRect(0, (int)resolution.Y, (int)resolution.X, -(int)resolution.Y)), new RenderStates(BlendMode.Alpha));
            for (int idx = 0; idx < uiTex.Length; idx++)
                resultTex.Draw(new Sprite(uiTex[idx].Texture, new IntRect(0, (int)resolution.Y, (int)resolution.X, -(int)resolution.Y)), new RenderStates(BlendMode.Alpha));


            //레이어들 초기화
            for (int idx = 1; idx < worldTex.Length; idx++)
                worldTex[idx].Clear(new Color(0, 0, 0, 0));

            for (int idx = 0; idx < uiTex.Length; idx++)
            {
                //if (uiTex[idx] == texWrLight)
                //{
                //    texWrLight.Clear(new Color(0, 0, 0, 125));
                //    continue;
                //}

                uiTex[idx].Clear(new Color(0, 0, 0, 0));
            }

            //결과 텍스쳐 반환
            Sprite resultSprite = new Sprite(resultTex.Texture, new IntRect(0, (int)resolution.Y, (int)resolution.X, -(int)resolution.Y));
            Program.window.Draw(resultSprite);

            //정리
            resultSprite.Dispose();
        }


    }

}
