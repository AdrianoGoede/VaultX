#nullable disable
using Newtonsoft.Json;

namespace VaultX.Repositories.Models
{
    public record CoinListRecord
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}