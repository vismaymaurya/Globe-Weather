using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

namespace GW.Weather
{
    using GW.UI;
    public class GetCurrentWeatherInfo : MonoBehaviour
    {
        public enum EPhase
        {
            NotStarted,
            GetWeatherData,

            Failed,
            Succeeded
        }

        [System.Serializable]
        public class WeatherInfo
        {
            public TextMeshProUGUI cityName;
            public TextMeshProUGUI temparature;
            public TextMeshProUGUI humidity;
            public TextMeshProUGUI pressure;
            public TextMeshProUGUI wind;
            public TextMeshProUGUI condition;
        }

        #region OpenWeatherInformation

        public class OpenWeather_Coordinates
        {
            [JsonProperty("lon")] public double Longitude { get; set; }
            [JsonProperty("lat")] public double Latitude { get; set; }
        }

        // Condition Info: https://openweathermap.org/weather-conditions
        public class OpenWeather_Condition
        {
            [JsonProperty("id")] public int ConditionID { get; set; }
            [JsonProperty("main")] public string Group { get; set; }
            [JsonProperty("description")] public string Description { get; set; }
            [JsonProperty("icon")] public string Icon { get; set; }
        }

        public class OpenWeather_KeyInfo
        {
            [JsonProperty("temp")] public double Temperature { get; set; }
            [JsonProperty("feels_like")] public double Temperature_FeelsLike { get; set; }
            [JsonProperty("temp_min")] public double Temperature_Minimum { get; set; }
            [JsonProperty("temp_max")] public double Temperature_Maximum { get; set; }
            [JsonProperty("pressure")] public int Pressure { get; set; }
            [JsonProperty("sea_level")] public int PressureAtSeaLevel { get; set; }
            [JsonProperty("grnd_level")] public int PressureAtGroundLevel { get; set; }
            [JsonProperty("humidity")] public int Humidity { get; set; }
        }

        public class OpenWeather_Wind
        {
            [JsonProperty("speed")] public double Speed { get; set; }
            [JsonProperty("deg")] public int Direction { get; set; }
            [JsonProperty("gust")] public double Gust { get; set; }
        }

        public class OpenWeather_Clouds
        {
            [JsonProperty("all")] public int Cloudiness { get; set; }
        }

        public class OpenWeather_Rain
        {
            [JsonProperty("1h")] public int VolumeInLastHour { get; set; }
            [JsonProperty("3h")] public int VolumeInLast3Hours { get; set; }
        }

        public class OpenWeather_Snow
        {
            [JsonProperty("1h")] public int VolumeInLastHour { get; set; }
            [JsonProperty("3h")] public int VolumeInLast3Hours { get; set; }
        }

        public class OpenWeather_Internal
        {
            [JsonProperty("type")] public int Internal_Type { get; set; }
            [JsonProperty("id")] public int Internal_ID { get; set; }
            [JsonProperty("message")] public double Internal_Message { get; set; }
            [JsonProperty("country")] public string CountryCode { get; set; }
            [JsonProperty("sunrise")] public int SunriseTime { get; set; }
            [JsonProperty("sunset")] public int SunsetTime { get; set; }
        }

        class OpenWeatherResponse
        {
            [JsonProperty("coord")] public OpenWeather_Coordinates Location { get; set; }
            [JsonProperty("weather")] public List<OpenWeather_Condition> WeatherConditions { get; set; }
            [JsonProperty("base")] public string Internal_Base { get; set; }
            [JsonProperty("main")] public OpenWeather_KeyInfo KeyInfo { get; set; }
            [JsonProperty("visibility")] public int Visibility { get; set; }
            [JsonProperty("wind")] public OpenWeather_Wind Wind { get; set; }
            [JsonProperty("clouds")] public OpenWeather_Clouds Clouds { get; set; }
            [JsonProperty("rain")] public OpenWeather_Rain Rain { get; set; }
            [JsonProperty("snow")] public OpenWeather_Snow Snow { get; set; }
            [JsonProperty("dt")] public int TimeOfCalculation { get; set; }
            [JsonProperty("sys")] public OpenWeather_Internal Internal_Sys { get; set; }
            [JsonProperty("timezone")] public int Timezone { get; set; }
            [JsonProperty("id")] public int CityID { get; set; }
            [JsonProperty("name")] public string CityName { get; set; }
            [JsonProperty("cod")] public int Internal_COD { get; set; }
        }

        #endregion

        #region Public Variables

        public WeatherInfo weatherInfo;

        public EPhase Phase { get; private set; } = EPhase.NotStarted;

        #endregion

        #region Private Variables

        const string OpenWeatherAPIKey = "08b6ef7ea01d0bf94ed3874cf3acdc57";

        const string URL_GetWeatherData = "https://api.openweathermap.org/data/2.5/weather";

        private float longitudeValue;

        OpenWeatherResponse weatherData;

        #endregion

        #region Private Function

        private void Update()
        {
            longitudeValue = GetComponent<UIController>().longitude;
        }

        #endregion

        #region Public Function

        public void UpdateWeather()
        {
            StartCoroutine(GetWeather_WeatherInfo());

            foreach (var condition in weatherData.WeatherConditions)
            {
                weatherInfo.condition.text = "Condition : " + condition.Description;
            }

            var temp = weatherData.KeyInfo.Temperature;
            var newWind = weatherData.Wind.Speed * 10;
            var newTemp = temp - 273;

            weatherInfo.cityName.text = weatherData.CityName;
            weatherInfo.temparature.text = "Temperature : " + newTemp.ToString();
            weatherInfo.humidity.text = "Humidity : " + weatherData.KeyInfo.Humidity.ToString();
            weatherInfo.pressure.text = "Pressure : " + weatherData.KeyInfo.Pressure.ToString();
            weatherInfo.wind.text = "Wind : " + newWind.ToString();

            
        }

        #endregion

        #region Coroutines

        IEnumerator GetWeather_WeatherInfo()
        {
            Phase = EPhase.GetWeatherData;
            float v_lon = longitudeValue;

            string weatherURL = URL_GetWeatherData;
            weatherURL += $"?lat={1}";
            weatherURL += $"&lon={v_lon}";
            weatherURL += $"&APPID={OpenWeatherAPIKey}";


            // attempt to retrieve WeatherInfo
            using (UnityWebRequest request = UnityWebRequest.Get(weatherURL))
            {
               // request.timeout = 2;
                yield return request.SendWebRequest();

                // did the request fail
                if (request.result == UnityWebRequest.Result.Success)
                {
                    weatherData = JsonConvert.DeserializeObject<OpenWeatherResponse>(request.downloadHandler.text);
                    Phase = EPhase.Succeeded;
                }
                else
                {
                    Debug.LogError($"Failed to WeatherData: {request.downloadHandler.text}");
                    Phase = EPhase.Failed;
                }
            }
        }

        #endregion
    }
}