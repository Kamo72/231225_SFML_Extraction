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
            }

            //bgm
            {
                string header = @"Assets\Musics\";
                //...
            }

            //sfx
            {
                string header = @"Assets\Sounds\";
                try
                {

                    SoundBuffer soundBuffer = new SoundBuffer(header + "SpiralMissileFly.ogg");
                    sfxs.Add("EnemyHit", soundBuffer);
                    //...
                    new Sound(soundBuffer).Play();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
