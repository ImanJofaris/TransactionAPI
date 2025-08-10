using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TransactionAPI.Models;

namespace TransactionAPI.Validators
{
    public class ValidationResult
    {
        public bool IsValid => !Errors.Any();
        public List<string> Errors { get; } = new();

        public void AddError(string error)
        {
            if (!string.IsNullOrWhiteSpace(error))
                Errors.Add(error);
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
                AddError(error);
        }
    }
}
