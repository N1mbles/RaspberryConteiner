using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Rasberry.Api
{
    /// <summary>
    /// Get temperature from russbery flask server, and parse from json
    /// </summary>
    public class GetTemp
    {
        HttpClient client = new HttpClient();

        public async Task<double> GetTemperature(string Url)
        {
            var responseAnswer = await client.GetAsync(Url);
            if (responseAnswer.IsSuccessStatusCode)
            {
                //
                string response = await client.GetStringAsync(Url);
                DataTemperature data = JsonConvert.DeserializeObject<DataTemperature>(response);
                return data.Temperature;
            }
            return 404;
        }
    }
}
