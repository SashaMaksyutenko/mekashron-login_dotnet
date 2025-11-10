using System.Text.Json.Serialization;

namespace LoginApp.Models
{
    public class RegisterResult
    {
        [JsonPropertyName("ResultCode")] public int ResultCode { get; set; }
        [JsonPropertyName("ResultMessage")] public string ResultMessage { get; set; } = "";
        [JsonPropertyName("EntityId")] public int EntityId { get; set; }
        [JsonPropertyName("AffiliateResultCode")] public int AffiliateResultCode { get; set; }
        [JsonPropertyName("AffiliateResultMessage")] public string AffiliateResultMessage { get; set; } = "";
    }
}