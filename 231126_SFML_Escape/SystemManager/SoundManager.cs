using SFML.Audio;
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
    internal static class SoundManager
    {

        static SoundManager() 
        {
            sound = new Sound(Rm.bgms["EnemyHit"]);
            sound.Play();
        }

        static Sound sound;
        static float totalVol = 0.5f;

        public class SoundUnion 
        {

        }





    }
}
