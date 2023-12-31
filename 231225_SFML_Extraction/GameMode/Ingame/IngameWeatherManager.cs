
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    internal class IngameWeatherManager
    {
        public IngameWeatherManager() 
        {
            
        }

        public struct Weather 
        {
            WeatherType weatherType;    //계절 유형

            Vector2f windFixed;     //기본 바람 방향
            float windRandom;       //바람의 편차
            Vector2f windVector;    //결과적 바람 방향
        }


        public enum WeatherType 
        {
            SUNNY,
            CLOUDY,
            RAINY,
            RAINY_STORM,
            SNOWY,
            SNOWY_STORM,
            FOGGY,
        }


        //눈 파티클, 바람 파티클, 비 파티클

        DateTime ingameTime = new DateTime(2026, 4, 12);
        



    }
}
