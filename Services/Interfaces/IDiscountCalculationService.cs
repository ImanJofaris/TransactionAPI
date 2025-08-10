using Microsoft.AspNetCore.Mvc;

namespace TransactionAPI.Services.Interfaces
{
    public interface IDiscountCalculationService
    {
        (long totalDiscount, long finalAmount) CalculateDiscount(long totalAmount);
    }
}
