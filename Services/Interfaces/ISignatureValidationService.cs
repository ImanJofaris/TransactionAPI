using Microsoft.AspNetCore.Mvc;
using TransactionAPI.Models;

namespace TransactionAPI.Services.Interfaces
{
    public interface ISignatureValidationService
    {
        bool ValidateSignature(TransactionRequest request);
    }
}
