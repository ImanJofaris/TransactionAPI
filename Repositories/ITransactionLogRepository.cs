using TransactionAPI.Models;

namespace TransactionAPI.Repositories;

public interface ITransactionLogRepository
{
    Task LogTransactionAsync(TransactionRequest request, TransactionResponse response, string additionalInfo = null);
    Task LogErrorAsync(string error, Exception? exception = null);
}