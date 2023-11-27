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
using System.ComponentModel;
using SFML.System;

namespace _231109_SFML_Test
{
    internal static class SoundManager
    {

        static SoundManager()
        {
            waveBgm = new SoundUnion(0.5f);
            waveAmb = new SoundUnion3D(0.5f);
            waveEffect = new SoundUnion3D(0.5f);
            waveFootsteb = new SoundUnion3D(0.5f);
            waveVoice = new SoundUnion3D(0.5f);

        }

        public static float totalVol = 0.5f;

        public static Entity listener;

        static SoundUnion waveBgm;  //배경음악
        static SoundUnion3D waveAmb;  //앰비언트
        static SoundUnion3D waveEffect ;   //효과
        static SoundUnion3D waveFootsteb ; //발소리
        static SoundUnion3D waveVoice;    //말소리

        public class SoundUnion
        {
            public SoundUnion(float vol)
            {
                this.vol = vol;
                soundList = new List<Sound>();
            }
            public float vol;
            protected List<Sound> soundList;
            
            
            public void AddSound(Sound sound)
            {
                sound.Volume *= vol * Sm.totalVol;
                soundList.Add(sound);
            }

            void SoundClearProcess()
            {
                List<Sound> toRemove = new List<Sound>();

                for (int i = 0; i < soundList.Count; i++)
                {
                    soundList.ForEach(key =>
                    {
                        if (key.Status != SoundStatus.Playing)
                            toRemove.Add(key);
                    });
                }

                foreach (Sound sound in toRemove)
                {
                    soundList.Remove(sound);
                    sound.Dispose();
                }
            }


        }

        public class SoundUnion3D
        {
            public SoundUnion3D(float vol)
            {
                this.vol = vol;
                soundList = new Dictionary<Sound, Vector2f> ();
            }
            public float vol;
            protected Dictionary<Sound, Vector2f> soundList;
            
            
            public void AddSound3D(Sound sound, Vector2f position) 
            {
                soundList[sound] = position;
                sound.RelativeToListener = true;
            }

            void SoundClearProcess()
            {
                List<Sound> toRemove = new List<Sound>();

                for (int i = 0; i < soundList.Count; i++)
                {
                    soundList.Keys.ToList().ForEach(key =>
                    {
                        if (key.Status != SoundStatus.Playing)
                            toRemove.Add(key);
                    });
                }

                foreach (Sound sound in toRemove)
                {
                    soundList.Remove(sound);
                    sound.Dispose();
                }
            }

            void SoundPositionProcess()
            {
                try
                {
                    if (Sm.listener == null) throw new Exception("리스너 등록 안됨");

                    foreach (Sound sound in soundList.Keys)
                    {
                        Vector2f worldPos = soundList[sound];
                        Vector2f listenerPos = Sm.listener.Position;
                        float listenerRot = Sm.listener.Direction;
                        Vector2f relativePos = worldPos - listenerPos;

                        Vector2f rotatedPos = relativePos.RotateFromZero(listenerRot);

                        sound.Position = new Vector3f(rotatedPos.X, 0f, rotatedPos.Y);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }




    }
}
