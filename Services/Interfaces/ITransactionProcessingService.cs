using Microsoft.AspNetCore.Mvc;
using TransactionAPI.Models;

namespace TransactionAPI.Services.Interfaces
{
    public interface ITransactionProcessingService
    {
        Task<TransactionResponse> ProcessTransactionAsync(TransactionRequest request);
    }
}
