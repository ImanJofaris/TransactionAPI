﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TransactionAPI.Models;

public class TransactionResponse
{
    [JsonPropertyName("result")]
    public int Result { get; set; }

    [JsonPropertyName("totalamount")]
    public long? TotalAmount { get; set; }

    [JsonPropertyName("totaldiscount")]
    public long? TotalDiscount { get; set; }

    [JsonPropertyName("finalamount")]
    public long? FinalAmount { get; set; }

    [JsonPropertyName("resultmessage")]
    public string? ResultMessage { get; set; }
}