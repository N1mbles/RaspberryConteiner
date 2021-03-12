using Newtonsoft.Json;

namespace Rasberry.Api
{
    public class DataTemperature
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }
    }
}
