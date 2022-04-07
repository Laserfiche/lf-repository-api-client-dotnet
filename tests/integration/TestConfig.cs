using Newtonsoft.Json;

namespace Laserfiche.Repository.Api.Client.IntegrationTest
{
    public class TestConfig
    {
        [JsonProperty]
        public string Username { get; set; }

        [JsonProperty]

        public string Password { get; set; }

        [JsonProperty]
        public string AccountId { get; set; }

        [JsonProperty]
        public string RepositoryId { get; set; }

        [JsonProperty]
        public string Domain { get; set; }
    }
}
