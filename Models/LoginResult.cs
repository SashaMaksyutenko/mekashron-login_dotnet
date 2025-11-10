using System.Text.Json.Serialization;

namespace LoginApp.Models
{
    public class LoginResult
    {
        [JsonPropertyName("ResultCode")] public int ResultCode { get; set; }
        [JsonPropertyName("ResultMessage")] public string ResultMessage { get; set; } = "";
        [JsonPropertyName("EntityId")] public int EntityId { get; set; }
        [JsonPropertyName("Email")] public string Email { get; set; } = "";
        [JsonPropertyName("FTPHost")] public string FTPHost { get; set; } = "";
        [JsonPropertyName("lid")] public string lid { get; set; } = "";
    }
}