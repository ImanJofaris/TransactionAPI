// Tests/DiscountCalculationServiceTests.cs
using TransactionAPI.Services;
using Xunit;
using Xunit.Abstractions;

namespace TransactionAPI.Tests;

public class DiscountCalculationServiceTests
{
    private readonly DiscountCalculationService _service = new();
    private readonly ITestOutputHelper _output;

    public DiscountCalculationServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CalculateDiscount_Sample1_ReturnsCorrectDiscount()
    {
        // Arrange: MYR 1000 (100000 cents)
        long totalAmount = 100000;

        // Act
        var (totalDiscount, finalAmount) = _service.CalculateDiscount(totalAmount);

        // Debug output
        _output.WriteLine($"Total Amount: {totalAmount} cents (MYR {totalAmount / 100.0})");
        _output.WriteLine($"Total Discount: {totalDiscount} cents (MYR {totalDiscount / 100.0})");
        _output.WriteLine($"Final Amount: {finalAmount} cents (MYR {finalAmount / 100.0})");

        // Assert: 10% discount for amount between 801-1200 MYR
        Assert.Equal(10000, totalDiscount);
        Assert.Equal(90000, finalAmount);
    }

    [Fact]
    public void CalculateDiscount_Sample2_ReturnsCorrectDiscount()
    {
        // Arrange: MYR 1205 (120500 cents) - ends in 5, above 900
        long totalAmount = 120500;

        // Act
        var baseDiscount = _service.CalculateBaseDiscountPercentage(totalAmount);
        var conditionalDiscount = _service.CalculateConditionalDiscountPercentage(totalAmount);
        var (totalDiscount, finalAmount) = _service.CalculateDiscount(totalAmount);

        // Debug output
        _output.WriteLine($"Total Amount: {totalAmount} cents (MYR {totalAmount / 100.0})");
        _output.WriteLine($"Base Discount: {baseDiscount}%");
        _output.WriteLine($"Conditional Discount: {conditionalDiscount}%");
        _output.WriteLine($"Total Discount Percentage: {Math.Min(baseDiscount + conditionalDiscount, 20)}%");
        _output.WriteLine($"Total Discount: {totalDiscount} cents (MYR {totalDiscount / 100.0})");
        _output.WriteLine($"Final Amount: {finalAmount} cents (MYR {finalAmount / 100.0})");

        // Assert: 20% cap applied (15% base + 10% conditional = 25%, capped at 20%)
        Assert.Equal(24100, totalDiscount);
        Assert.Equal(96400, finalAmount);
    }

    [Theory]
    [InlineData(10000, 0, 10000)]     // MYR 100 - No discount
    [InlineData(30000, 1500, 28500)]  // MYR 300 - 5% discount
    [InlineData(70000, 4900, 65100)]  // MYR 700 - 7% discount
    [InlineData(150000, 22500, 127500)] // MYR 1500 - 15% discount
    public void CalculateDiscount_VariousAmounts_ReturnsExpectedResults(
        long amount, long expectedDiscount, long expectedFinal)
    {
        // Act
        var (totalDiscount, finalAmount) = _service.CalculateDiscount(amount);

        // Debug output
        _output.WriteLine($"Amount: {amount} (MYR {amount / 100.0}), Expected: {expectedDiscount}, Actual: {totalDiscount}");

        // Assert
        Assert.Equal(expectedDiscount, totalDiscount);
        Assert.Equal(expectedFinal, finalAmount);
    }

    [Fact]
    public void CalculateConditionalDiscount_PrimeNumber_ReturnsAdditionalDiscount()
    {
        // Arrange: Test with a known prime number above MYR 500
        long primeAmount = 50003; // This should be prime and > 50000 cents (MYR 500)

        // Act
        var conditionalDiscount = _service.CalculateConditionalDiscountPercentage(primeAmount);

        // Debug
        _output.WriteLine($"Testing prime number: {primeAmount} (MYR {primeAmount / 100.0})");
        _output.WriteLine($"Conditional discount: {conditionalDiscount}%");

        // Assert
        Assert.Equal(8, conditionalDiscount);
    }
}