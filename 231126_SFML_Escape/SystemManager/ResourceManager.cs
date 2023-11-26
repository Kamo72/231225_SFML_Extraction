using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal static class ResourceManager
    {
        static ResourceManager()
        {
            fonts = new Dictionary<string, Font>();
            textures = new Dictionary<string, Texture>();
            
            LoadResources();
        }
        public static Dictionary<string, Font> fonts;
        public static Dictionary<string, Texture> textures;

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
                //...
            }
        }
    }
}
