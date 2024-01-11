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
                    {
                        textures.Add("FN_FAL_bolt", new Texture(header + "FN_FAL_bolt.png"));
                        textures.Add("FN_FAL_body", new Texture(header + "FN_FAL_body.png"));
                        textures.Add("FN_FAL_pistolGrip_basic", new Texture(header + "FN_FAL_pistolGrip_basic.png"));
                        textures.Add("FN_FAL_stock_basic", new Texture(header + "FN_FAL_stock_basic.png"));
                        textures.Add("FN_FAL_barrel_533mm", new Texture(header + "FN_FAL_barrel_533mm.png"));
                        textures.Add("FN_FAL_muzzle_Israeli", new Texture(header + "FN_FAL_muzzle_Israeli.png"));
                        textures.Add("FN_FAL_handGuard_dsArms", new Texture(header + "FN_FAL_handGuard_dsArms.png"));
                        textures.Add("FN_FAL_chargingHandle", new Texture(header + "FN_FAL_chargingHandle.png"));
                    }
                    {
                        textures.Add("FN_FAL_MAG10", new Texture(header + "FN_FAL_MAG10.png"));
                        textures.Add("FN_FAL_MAG20", new Texture(header + "FN_FAL_MAG20.png"));
                        textures.Add("FN_FAL_MAG50", new Texture(header + "FN_FAL_MAG50.png"));
                    }
                }
                {
                    textures.Add("LIGHT_radial", new Texture(header + "RadialAlphaGradient.png"));
                }
                {
                    textures.Add("ITEM_Oddment", new Texture(header + "Oddment.png"));
                }
                {
                    textures.Add("K_WoodenAmmoBox", new Texture(header + "K_WoodenAmmoBox.png"));
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
                var tdirPath = slnPath + @"\Sprites\WEAPONS_DMR";
                //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape\Sprites\WEAPONS_DMR

                {
                    dirPath = tdirPath + @"\FN_FAL\";
                    //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape\Sprites\WEAPONS_DMR\FN_FAL\

                    CopyFile(dirPath + @"FN_FAL_bolt.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_body.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_pistolGrip_basic.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_stock_basic.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_barrel_533mm.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_muzzle_Israeli.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_handGuard_dsArms.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_chargingHandle.png", @"Assets\Textures\");
                }
                {
                    dirPath = tdirPath + @"\FN_FAL_MAG\";
                    //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape\Sprites\WEAPONS_DMR\FN_FAL_MAG\

                    CopyFile(dirPath + @"FN_FAL_MAG10.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_MAG20.png", @"Assets\Textures\");
                    CopyFile(dirPath + @"FN_FAL_MAG50.png", @"Assets\Textures\");

                }
            }

            {
                dirPath = slnPath + @"\Sprites\LIGHT\";
                //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape\Sprites\LIGHT\

                CopyFile(dirPath + @"RadialAlphaGradient.png", @"Assets\Textures\");

            }

            {
                dirPath = slnPath + @"\Sprites\ITEMS\";
                //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape\Sprites\ITEMS\

                CopyFile(dirPath + @"Oddment.png", @"Assets\Textures\");

            }

            {
                dirPath = slnPath + @"\Sprites\CONTAINERS\";
                //S:\[GitHub]\231126_SFML_Escape\231126_SFML_Escape\Sprites\CONTAINERS\

                CopyFile(dirPath + @"K_WoodenAmmoBox.png", @"Assets\Textures\");

            }
        }

        static void CopyFile(string filePath, string toPath)
        {
            // 복사될 경로가 없다면 생성
            if (Directory.Exists(toPath) == false)
                Directory.CreateDirectory(toPath);
            

            // 파일명 추출
            string fileName = Path.GetFileName(filePath);

            // 파일 복사
            string destinationFilePath = Path.Combine(toPath, fileName);
            File.Copy(filePath, destinationFilePath, true); // 두 번째 인자는 이미 파일이 존재하는 경우 덮어쓸지 여부입니다.
        }
    }
}
