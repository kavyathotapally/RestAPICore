using System.Text.Json.Serialization;

namespace RestAPICore.Models.Response

{
    public class AuthAgentResponse
    {
        public string accessToken { get; set; }


        [JsonPropertyName("OldAuth")]
        public OldAuthDetails OldAuth { get; set; }

        public class OldAuthDetails
        {
            [JsonPropertyName("Key")]
            public string Key { get; set; }

            [JsonPropertyName("UserId")]
            public int UserId { get; set; }
        }
    }
}
