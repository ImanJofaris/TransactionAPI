using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TransactionAPI.Models;

public class ItemDetail
{
    [JsonPropertyName("partneritemref")]
    public string PartnerItemRef { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("qty")]
    public int Qty { get; set; }
    
    [JsonPropertyName("unitprice")]
    public long UnitPrice { get; set; }
}