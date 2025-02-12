namespace BlazorWasmApp.Models
{
    public class WeatherResponse
    {
        public Coord coord { get; set; } = new Coord();
        public List<Weather> weather { get; set; } = new List<Weather>();
        public string weatherBase { get; set; } = string.Empty; // Changed from 'base' to 'weatherBase'
        public Main main { get; set; } = new Main();
        public int visibility { get; set; }
        public Wind wind { get; set; } = new Wind();
        public Clouds clouds { get; set; } = new Clouds();
        public int dt { get; set; }
        public Sys sys { get; set; } = new Sys();
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public int cod { get; set; }
    }

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string icon { get; set; } = string.Empty;
    }

    public class Main
    {
        public float temp { get; set; }
        public float feels_like { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public int sea_level { get; set; }
        public int grnd_level { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public int deg { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; } = string.Empty;
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    // 5-Day Forecast Models
    public class ForecastResponse
    {
        public string cod { get; set; } = string.Empty;
        public int message { get; set; }
        public int cnt { get; set; } // Number of entries in the list (typically 40 for 5 days)
        public List<ForecastList> list { get; set; } = new List<ForecastList>(); // Contains forecast data every 3 hours
        public City city { get; set; } = new City();
    }

    public class ForecastList
    {
        public int dt { get; set; } // Time of the forecasted data (Unix timestamp)
        public Main main { get; set; } = new Main(); // Weather details (temperature, pressure, etc.)
        public List<Weather> weather { get; set; } = new List<Weather>(); // List of weather conditions
        public Clouds clouds { get; set; } = new Clouds(); // Cloud coverage percentage
        public Wind wind { get; set; } = new Wind(); // Wind information (speed, direction)
        public int visibility { get; set; } // Visibility in meters
        public float pop { get; set; } // Probability of precipitation
        public SysForecast sys { get; set; } = new SysForecast(); // Internal system data
        public string dt_txt { get; set; } = string.Empty; // Date/time of the forecasted data as string
    }

    public class SysForecast
    {
        public string pod { get; set; } = string.Empty; // "pod" indicates "part of the day" (n: night, d: day)
    }

    public class City
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public Coord coord { get; set; } = new Coord(); // City coordinates
        public string country { get; set; } = string.Empty;
        public int population { get; set; } // City population
        public int timezone { get; set; } // City timezone (offset from UTC in seconds)
        public int sunrise { get; set; } // Sunrise time (Unix timestamp)
        public int sunset { get; set; } // Sunset time (Unix timestamp)
    }
}
