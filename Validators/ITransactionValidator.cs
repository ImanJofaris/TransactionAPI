using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TransactionAPI.Models;

namespace TransactionAPI.Validators
{
    public interface ITransactionValidator
    {
        ValidationResult ValidateRequest(TransactionRequest request);
        ValidationResult ValidateMandatoryFields(TransactionRequest request);
        ValidationResult ValidateBusinessLogic(TransactionRequest request);
    }
}
