using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TransactionAPI.Models;

namespace TransactionAPI.Validators
{
    public class TransactionValidator : ITransactionValidator
    {
        public ValidationResult ValidateRequest(TransactionRequest request)
        {
            var result = new ValidationResult();

            // Validate mandatory fields
            var mandatoryValidation = ValidateMandatoryFields(request);
            result.AddErrors(mandatoryValidation.Errors);

            // Validate business logic
            var businessValidation = ValidateBusinessLogic(request);
            result.AddErrors(businessValidation.Errors);

            return result;
        }

        public ValidationResult ValidateMandatoryFields(TransactionRequest request)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(request.PartnerKey))
                result.AddError("partnerkey is Required.");

            if (string.IsNullOrWhiteSpace(request.PartnerRefNo))
                result.AddError("partnerrefno is Required.");

            if (string.IsNullOrWhiteSpace(request.PartnerPassword))
                result.AddError("partnerpassword is Required.");

            if (string.IsNullOrWhiteSpace(request.Timestamp))
                result.AddError("timestamp is Required.");

            if (string.IsNullOrWhiteSpace(request.Sig))
                result.AddError("sig is Required.");

            return result;
        }

        public ValidationResult ValidateBusinessLogic(TransactionRequest request)
        {
            var result = new ValidationResult();

            // Total amount validation
            if (request.TotalAmount <= 0)
                result.AddError("totalamount must be positive.");

            // Timestamp validation (±5 minutes)
            ValidateTimestamp(request.Timestamp, result);

            // Item validations
            ValidateItems(request, result);

            return result;
        }

        private void ValidateTimestamp(string timestamp, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(timestamp)) return;

            if (!DateTime.TryParse(timestamp, out var parsedTimestamp))
            {
                result.AddError("Invalid timestamp format.");
                return;
            }

            var serverTime = DateTime.UtcNow;

            var timeDifference = Math.Abs((serverTime - parsedTimestamp).TotalMinutes);

            //iman temp
            //if (timeDifference > 5)
            //    result.AddError("Expired.");
        }

        private void ValidateItems(TransactionRequest request, ValidationResult result)
        {
            if (request.Items?.Any() != true) return;

            long calculatedTotal = 0;

            foreach (var item in request.Items)
            {
                if (string.IsNullOrWhiteSpace(item.PartnerItemRef))
                    result.AddError("partneritemref is Required.");

                if (string.IsNullOrWhiteSpace(item.Name))
                    result.AddError("name is Required.");

                if (item.Qty <= 0 || item.Qty > 5)
                    result.AddError("Quantity must be between 1 and 5.");

                if (item.UnitPrice <= 0)
                    result.AddError("unitprice must be positive.");

                calculatedTotal += item.Qty * item.UnitPrice;
            }

            if (calculatedTotal != request.TotalAmount)
                result.AddError("Invalid Total Amount.");
        }
    }
}
