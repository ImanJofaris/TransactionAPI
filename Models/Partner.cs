using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TransactionAPI.Models;

public class Partner
{
    public string PartnerNo { get; set; } = string.Empty;
    public string PartnerKey { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}