
using SFML.System;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;

namespace _231109_SFML_Test
{
    internal class IngameWeatherTimeManager : IDisposable
    {
        public IngameWeatherTimeManager() 
        {
            //시간 초기화
            TimeInit();

            //시간 타이머 생성
            timer = new Timer(1000d / 60d); //50d / 3d = 16.6666d
            timer.Elapsed += (s, e) => ingameTime.Add(new TimeSpan(0, 0, 1));
            timer.Elapsed += (s, e) => SetDayLightColor();
            timer.Start();

            //파티클 타이머 생성
            particleTimer = new Timer(1000d / 10d);
            particleTimer.Elapsed += (s, e) => ParticleThreadSnow();
            particleTimer.Elapsed += (s, e) => ParticleThreadWind();
            particleTimer.Elapsed += (s, e) => ParticleThreadRain();
            particleTimer.Stop();
        }

        //[시간]
        public Timer timer;
        public DateTime ingameTime;
        public Color dayLightColor = Color.Transparent;

        //시간 초기화
        void TimeInit() 
        {
            ingameTime = new DateTime(2026, 4, 12, 8, 0, 0);

            //원래라면 게임 로드할 때 원래 시간 가져와야 함.
        }

        void SetDayLightColor()
        {
            //일조시간은 12월 중순 부터 다음해 12월 중순까지 -Sin함수를 그림
            //일단 기준 좌표는 북위 40도 기준이다. 대한민국 서울의 경우 37.5도
            //가장 해가 긴 6월 중순에는 14.5시간     [05:10] - [19:54] 대략 14시간 반 > 05:00 - 19:30
            //가장 해가 짧은 12월 중순에는 9.5시간   [07:54] - [17:22] 대략 09시간 반 > 08:00 - 17:00

            //흠... 해 뜨고 지는 시간은 알았으니 이제 이걸 어케 잘 해봐야겠네
            dayLightColor = Color.Black;
        }



        //[기상]
        Weather weatherNow;

        void weatherLoad() { }

        //기상 값
        public struct Weather 
        {
            WeatherType weatherType;    //계절 유형

            float wind;     //기본 바람 벡터 0f ~ 10f ~ (10f는 폭풍)
            float rainfall;     //습도 = 강우 확률 0~1f = 확률성 강우, 1f~ 폭우
                                //전에 비가 왔다면 강우 확률 보정 0.5f 적용.
                                //그럼에도 비가 그쳤다면 이틀 정도 강우 확률 보정 -0.5f 적용
            float temperature;      //기온
        }

        //기상 유형 분류
        public enum WeatherType 
        {
            SUNNY,          //맑음    -낮은 습도. 낮은 바람.
            RAINY,          //강우    -강우 성공. 기온 높음
            STORM,          //폭풍    -강우 실패 중 높은 바람
            SNOWY,          //강설    -강우 성공. 기온 낮음

            CLOUDY,         //구름    -높은 습도. 강우 실패. 낮은 바람.
            RAINY_STORM,    //폭우    -강우 성공 중, 기온 높음. 매우 높은 습도 또는 높은 바람
            SNOWY_STORM,    //폭설    -강우 성공 중, 기온 낮음. 매우 높은 습도 또는 높은 바람
            FOGGY,          //안개    -구름 발생 중, 매우 높은 습도
            DUST_STORM,     //황사    -강우 실패 중, 기온 높음. 매우 낮은 습도.
        }

        //파티클
        Timer particleTimer;
        void ParticleThreadSnow() { }
        void ParticleThreadWind() { }
        void ParticleThreadRain() { }




        //소멸자
        ~IngameWeatherTimeManager() 
        {
            Dispose();
        }
        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();

            particleTimer.Stop();
            particleTimer.Dispose();
        }
    }
}
