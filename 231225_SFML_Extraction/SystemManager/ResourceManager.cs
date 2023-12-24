using SFML.Audio;
using SFML.Graphics;
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
using System.Windows.Forms;
using System.IO;

namespace _231109_SFML_Test
{
    internal static class ResourceManager
    {
        static ResourceManager()
        {
            fonts = new Dictionary<string, Font>();
            textures = new Dictionary<string, Texture>();
            bgms = new Dictionary<string, SoundBuffer>();
            sfxs = new Dictionary<string, SoundBuffer>();

            BUILD_SUPPORT_LOAD();
            LoadResources();
        }
        public static Dictionary<string, Font> fonts;
        public static Dictionary<string, Texture> textures;
        public static Dictionary<string, SoundBuffer> bgms;
        public static Dictionary<string, SoundBuffer> sfxs;

        //상대를 불러옵니다.
        static void LoadResources()
        {
            //fonts
            {
                string header = @"Assets\Fonts\";
                fonts.Add("Jalnan", new Font(header + "Jalnan.ttf"));
            }

            //sprites
            {
                string header = @"Assets\Textures\";
                textures.Add("smgIcon", new Texture(header + "smgIcon.png"));
                textures.Add("valve", new Texture(header + "valveOnSanghan.png"));
                textures.Add("texGrass", new Texture(header + "texGrassCompressed.jpg"));
                textures.Add("texConcrete", new Texture(header + "texConcrete.jpg"));

                {
                    textures.Add("FN_FAL_body_grip", new Texture(header + "FN_FAL_body_grip.png"));
                    textures.Add("FN_FAL_body_lower", new Texture(header + "FN_FAL_body_lower.png"));
                    textures.Add("FN_FAL_body_middle", new Texture(header + "FN_FAL_body_middle.png"));
                    textures.Add("FN_FAL_body_upper", new Texture(header + "FN_FAL_body_upper.png"));
                    textures.Add("FN_FAL_body_handle", new Texture(header + "FN_FAL_body_handle.png"));

                    textures.Add("FN_FAL_body_bolt", new Texture(header + "FN_FAL_body_bolt.png"));
                }
                {
                    textures.Add("FN_FAL_MAG10_0", new Texture(header + "FN_FAL_MAG10_0.png"));
                    textures.Add("FN_FAL_MAG10_1", new Texture(header + "FN_FAL_MAG10_1.png"));
                    textures.Add("FN_FAL_MAG10_2", new Texture(header + "FN_FAL_MAG10_2.png"));
                    textures.Add("FN_FAL_MAG10_3", new Texture(header + "FN_FAL_MAG10_3.png"));
                }
            }

            //bgm
            {
                string header = @"Assets\Musics\";
                //...
            }

            //sfx
            {
                string header = @"Assets\Sounds\";
                sfxs.Add("SpiralMissileFly", new SoundBuffer(header + "SpiralMissileFly.ogg"));
                sfxs.Add("EnemyHit", new SoundBuffer(header + "EnemyHit.ogg"));
            }
        }


        static void BUILD_SUPPORT_LOAD()
        {
            string slnPath, dirPath;

            slnPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            slnPath = System.IO.Path.GetDirectoryName(slnPath);
            slnPath = System.IO.Path.GetDirectoryName(slnPath);
            slnPath = System.IO.Path.GetDirectoryName(slnPath);
            //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape
            {
                dirPath = slnPath + @"\Sprites\FN_FAL\";
                //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape\SpritesIde\FN_FAL\

                CopyFile(dirPath + @"FN_FAL_body_grip.png",     @"Assets\Textures\");
                CopyFile(dirPath + @"FN_FAL_body_lower.png",    @"Assets\Textures\");
                CopyFile(dirPath + @"FN_FAL_body_middle.png",   @"Assets\Textures\");
                CopyFile(dirPath + @"FN_FAL_body_upper.png",    @"Assets\Textures\");
                CopyFile(dirPath + @"FN_FAL_body_handle.png", @"Assets\Textures\");

                CopyFile(dirPath + @"FN_FAL_body_bolt.png", @"Assets\Textures\");
            }
            {
                dirPath = slnPath + @"\Sprites\FN_FAL_MAG10\";
                //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape\SpritesIde\FN_FAL_10MAG\

                CopyFile(dirPath + @"FN_FAL_MAG10_0.png", @"Assets\Textures\");
                CopyFile(dirPath + @"FN_FAL_MAG10_1.png", @"Assets\Textures\");
                CopyFile(dirPath + @"FN_FAL_MAG10_2.png", @"Assets\Textures\");
                CopyFile(dirPath + @"FN_FAL_MAG10_3.png", @"Assets\Textures\");

            }

        }

        static void CopyFile(string filePath, string toPath)
        {
            // 복사될 경로가 없다면 생성
            if (!Directory.Exists(toPath))
            {
                Directory.CreateDirectory(toPath);
            }

            // 파일명 추출
            string fileName = Path.GetFileName(filePath);

            // 파일 복사
            string destinationFilePath = Path.Combine(toPath, fileName);
            File.Copy(filePath, destinationFilePath, true); // 두 번째 인자는 이미 파일이 존재하는 경우 덮어쓸지 여부입니다.
        }
    }
}
