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
            waves = new List<SoundUnion>()
            {
            new SoundUnion(0.5f),
            new SoundUnion(0.5f),
            new SoundUnion(0.5f),
            new SoundUnion(0.5f),
            new SoundUnion(0.5f),
            };

        }

        public static float totalVol = 0.5f;

        public static Entity listener;

        public static SoundUnion waveBgm        { get { return waves[0]; } }  //배경음악
        public static SoundUnion waveAmb        { get { return waves[1]; } } //앰비언트
        public static SoundUnion waveEffect     { get { return waves[2]; } }  //효과
        public static SoundUnion waveFootsteb   { get { return waves[3]; } } //발소리
        public static SoundUnion waveVoice      { get { return waves[4]; } }    //말소리

        static List<SoundUnion> waves;

        public static void SoundProcess()
        {
            foreach (var wave in waves) {
                wave.SoundClearProcess();
                wave.SoundPositionProcess();
            }
        }

        public class SoundUnion
        {
            public SoundUnion(float vol)
            {
                this.vol = vol;
                soundList = new Dictionary<Sound, Vector2f>();
            }
            public float vol;
            protected Dictionary<Sound, Vector2f> soundList;

            //소리 추가
            public Sound AddSound(SoundBuffer soundBuf) 
            {
                Sound sound = new Sound(soundBuf);
                lock (soundList)
                    soundList[sound] = Vector2fEx.Zero;

                sound.Volume *= vol * Sm.totalVol * 10f;
                sound.RelativeToListener = false;
                sound.Play();

                return sound;
            }
            public Sound AddSound(SoundBuffer soundBuf, Vector2f position, float distance)
            {
                Sound sound = new Sound(soundBuf);
                lock (soundList)
                    soundList[sound] = position;

                sound.Volume *= vol * Sm.totalVol * 10f;
                sound.RelativeToListener = true;
                sound.MinDistance = distance;
                sound.Play();

                //회전 후 적용
                {
                    Vector2f listenerPos = Sm.listener.Position;
                    float listenerRot = Sm.listener.Direction;
                    Vector2f relativePos = position - listenerPos;

                    Vector2f rotatedPos = relativePos.RotateFromZero(listenerRot);

                    sound.Position = new Vector3f(rotatedPos.X, 0f, rotatedPos.Y);
                }
                return sound;
            }

            public void SoundClearProcess()
            {
                //Console.WriteLine("SoundClearProcess" + soundList.Count);

                List<Sound> toRemove = new List<Sound>();

                for (int i = 0; i < soundList.Count; i++)
                {
                    soundList.Keys.ToList().ForEach(key =>
                    {
                        if (key.Status != SoundStatus.Playing)
                            toRemove.Add(key);
                    });
                }

                lock (soundList)
                    foreach (Sound sound in toRemove)
                    {
                        soundList.Remove(sound);
                        sound.Dispose();
                    }
            }

            public void SoundPositionProcess()
            {
                try
                {
                    if (Sm.listener == null) throw new Exception("리스너 등록 안됨");

                    lock(soundList)
                    foreach (Sound sound in soundList.Keys)
                    {
                        if (sound.RelativeToListener == false) continue;

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
                    //Console.WriteLine(e.ToString());
                }
            }

        }




    }
}
