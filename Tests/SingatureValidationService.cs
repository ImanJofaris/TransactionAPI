// Tests/SignatureValidationServiceTests.cs
using TransactionAPI.Models;
using TransactionAPI.Services;
using Xunit;

namespace TransactionAPI.Tests;

public class SignatureValidationServiceTests
{
    private readonly SignatureValidationService _service = new();

    [Fact]
    public void ValidateSignature_ValidSignature_ReturnsTrue()
    {
        // Arrange - Create request with known signature
        var request = new TransactionRequest
        {
            PartnerKey = "FAKEGOOGLE",
            PartnerRefNo = "FG-00001",
            TotalAmount = 1000,
            PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
            Timestamp = "2024-08-15T02:11:22.0000000Z"
        };

        // Generate the correct signature using our service
        var expectedSignature = _service.GenerateSignature(request);
        request.Sig = expectedSignature;

        // Act
        var result = _service.ValidateSignature(request);

        // Assert
        Assert.True(result, $"Expected signature: {expectedSignature}, but validation failed");
    }

    [Fact]
    public void ValidateSignature_InvalidSignature_ReturnsFalse()
    {
        // Arrange
        var request = new TransactionRequest
        {
            PartnerKey = "FAKEGOOGLE",
            PartnerRefNo = "FG-00001",
            TotalAmount = 1000,
            PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
            Timestamp = "2024-08-15T02:11:22.0000000Z",
            Sig = "InvalidSignature"
        };

        // Act
        var result = _service.ValidateSignature(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GenerateSignature_KnownValues_ReturnsExpectedSignature()
    {
        // Arrange - Using the exact sample from the document
        var request = new TransactionRequest
        {
            PartnerKey = "FAKEGOOGLE",
            PartnerRefNo = "FG-00001",
            TotalAmount = 1000,
            PartnerPassword = "RkFLRVBBU1NXT1JEMTIzNA==",
            Timestamp = "2024-08-15T02:11:22.0000000Z"
        };

        // Act
        var signature = _service.GenerateSignature(request);

        // Assert - This should match the sample signature from the document
        var expectedSignature = "MDE3ZTBkODg4ZDNhYzU0ZDBlZWRmNmU2NmUyOWRhZWU4Y2M1NzQ1OTIzZGRjYTc1ZGNjOTkwYzg2MWJlMDExMw==";
        Assert.Equal(expectedSignature, signature);
    }
}