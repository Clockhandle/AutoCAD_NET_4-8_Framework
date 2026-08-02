using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyMiningPlugin.Services
{
    public class MapInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }
    }

    internal class MapApiResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("data")]
        public List<MapInfo> Data { get; set; }
    }

    public static class MapApiService
    {
        private static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Fetches the list of available maps from the server.
        /// Calls GET {serverUrl}/api/maps and returns the map list.
        /// </summary>
        public static async Task<List<MapInfo>> FetchMapsAsync(string serverUrl)
        {
            string url = serverUrl.TrimEnd('/') + "/api/maps";
            string json = await _client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<MapApiResponse>(json);
            if (result != null && result.Success && result.Data != null)
                return result.Data;
            return new List<MapInfo>();
        }
    }
}
