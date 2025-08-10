using TransactionAPI.Services.Interfaces;

namespace TransactionAPI.Services;

public class DiscountCalculationService : IDiscountCalculationService
{
    public (long totalDiscount, long finalAmount) CalculateDiscount(long totalAmount)
    {
        var baseDiscountPercentage = CalculateBaseDiscountPercentage(totalAmount);
        var conditionalDiscountPercentage = CalculateConditionalDiscountPercentage(totalAmount);

        // Apply cap of 20% maximum discount
        var totalDiscountPercentage = Math.Min(baseDiscountPercentage + conditionalDiscountPercentage, 20.0);
        var totalDiscount = (long)(totalAmount * totalDiscountPercentage / 100);
        var finalAmount = totalAmount - totalDiscount;

        return (totalDiscount, finalAmount);
    }

    public double CalculateBaseDiscountPercentage(long totalAmount)
    {
        var amountInMyr = totalAmount / 100.0;

        return amountInMyr switch
        {
            < 200 => 0,           // < MYR 200
            <= 500 => 5,         // MYR 200-500
            <= 800 => 7,         // MYR 501-800
            <= 1200 => 10,       // MYR 801-1200
            _ => 15               // > MYR 1200
        };
        //return totalAmount switch
        //{
        //    < 20000 => 0,           // < MYR 200
        //    <= 50000 => 5,         // MYR 200-500
        //    <= 80000 => 7,         // MYR 501-800
        //    <= 120000 => 10,       // MYR 801-1200
        //    _ => 15                 // > MYR 1200
        //};
    }

    public double CalculateConditionalDiscountPercentage(long totalAmount)
    {
        double conditionalDiscount = 0;
        var amountInMyr = totalAmount / 100.0;

        // Prime number check above MYR 500
        if (amountInMyr > 500 && IsPrime(totalAmount))
            conditionalDiscount += 8;

        // Ends in 5 and above MYR 900
        if (amountInMyr > 900 && totalAmount % 10 == 5)
            conditionalDiscount += 10;

        return conditionalDiscount;
        //double conditionalDiscount = 0;

        //// Prime number check above MYR 500 (50000 cents)
        //if (totalAmount > 50000 && IsPrime(totalAmount))
        //    conditionalDiscount += 8;

        //// Ends in 5 and above MYR 900 (90000 cents)
        //if (totalAmount > 90000 && totalAmount % 10 == 5)
        //    conditionalDiscount += 10;

        //return conditionalDiscount;
    }

    private bool IsPrime(long number)
    {
        if (number < 2) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        for (long i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0) return false;
        }
        return true;
    }
}