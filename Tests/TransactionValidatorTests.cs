// Tests/TransactionValidatorTests.cs
using TransactionAPI.Models;
using TransactionAPI.Validators;
using Xunit;

namespace TransactionAPI.Tests;

public class TransactionValidatorTests
{
    private readonly TransactionValidator _validator = new();

    [Fact]
    public void ValidateRequest_ValidRequest_ReturnsNoErrors()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var result = _validator.ValidateRequest(request);

        // Assert
        Assert.True(result.IsValid, $"Validation failed with errors: {string.Join(", ", result.Errors)}");
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateRequest_MissingPartnerKey_ReturnsError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.PartnerKey = "";

        // Act
        var result = _validator.ValidateRequest(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("partnerkey is Required.", result.Errors);
    }

    [Fact]
    public void ValidateRequest_ExpiredTimestamp_ReturnsError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Timestamp = DateTime.UtcNow.AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");

        // Act
        var result = _validator.ValidateRequest(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Expired.", result.Errors);
    }

    [Fact]
    public void ValidateRequest_InvalidTotalAmount_ReturnsError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.TotalAmount = 500; // Items total to 1000, but totalamount is 500

        // Act
        var result = _validator.ValidateRequest(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Invalid Total Amount.", result.Errors);
    }

    [Fact]
    public void ValidateRequest_QuantityExceedsLimit_ReturnsError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Items[0].Qty = 6; // Exceeds limit of 5
        request.TotalAmount = 3000; // Update total to match new quantity

        // Act
        var result = _validator.ValidateRequest(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Quantity must be between 1 and 5.", result.Errors);
    }

    private TransactionRequest CreateValidRequest()
    {
        return new TransactionRequest
        {
            PartnerKey = "FAKEGOOGLE",
            PartnerRefNo = "FG-00001",
            PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
            TotalAmount = 1000, // This should match items total
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"),
            Sig = "validSignature",
            Items = new List<ItemDetail>
            {
                // Total: 2 * 500 = 1000 (matches TotalAmount)
                new() { PartnerItemRef = "i-001", Name = "Test Item", Qty = 2, UnitPrice = 500 }
            }
        };
    }
}