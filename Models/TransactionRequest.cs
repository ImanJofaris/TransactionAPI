using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TransactionAPI.Models;

public class TransactionRequest
{
    [JsonPropertyName("partnerkey")]
    public string PartnerKey { get; set; } = string.Empty;

    [JsonPropertyName("partnerrefno")]
    public string PartnerRefNo { get; set; } = string.Empty;

    [JsonPropertyName("partnerpassword")]
    public string PartnerPassword { get; set; } = string.Empty;

    [JsonPropertyName("totalamount")]
    public long TotalAmount { get; set; }

    [JsonPropertyName("items")]
    public List<ItemDetail> Items { get; set; } = new();

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    [JsonPropertyName("sig")]
    public string Sig { get; set; } = string.Empty;
}