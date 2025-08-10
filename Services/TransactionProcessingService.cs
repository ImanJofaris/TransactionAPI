using TransactionAPI.Models;
using TransactionAPI.Services.Interfaces;
using TransactionAPI.Validators;
using TransactionAPI.Repositories;

namespace TransactionAPI.Services;

public class TransactionProcessingService : ITransactionProcessingService
{
    private readonly ITransactionValidator _validator;
    private readonly IPartnerAuthenticationService _authService;
    private readonly ISignatureValidationService _signatureService;
    private readonly IDiscountCalculationService _discountService;
    private readonly ITransactionLogRepository _logRepository;
    private readonly ILogger<TransactionProcessingService> _logger;

    public TransactionProcessingService(
        ITransactionValidator validator,
        IPartnerAuthenticationService authService,
        ISignatureValidationService signatureService,
        IDiscountCalculationService discountService,
        ITransactionLogRepository logRepository,
        ILogger<TransactionProcessingService> logger)
    {
        _validator = validator;
        _authService = authService;
        _signatureService = signatureService;
        _discountService = discountService;
        _logRepository = logRepository;
        _logger = logger;
    }

    public async Task<TransactionResponse> ProcessTransactionAsync(TransactionRequest request)
    {
        var response = new TransactionResponse();

        try
        {
            _logger.LogInformation("Processing transaction for partner: {PartnerKey}, RefNo: {RefNo}",
                request.PartnerKey, request.PartnerRefNo);

            // Step 1: Validate request
            var validationResult = _validator.ValidateRequest(request);
            if (!validationResult.IsValid)
            {
                response.Result = 0;
                response.ResultMessage = string.Join(", ", validationResult.Errors);
                await _logRepository.LogTransactionAsync(request, response, "Validation failed");
                return response;
            }

            // Step 2: Authenticate partner
            var isAuthenticated = await _authService.AuthenticatePartnerAsync(request.PartnerKey, request.PartnerPassword);
            if (!isAuthenticated)
            {
                response.Result = 0;
                response.ResultMessage = "Access Denied!";
                await _logRepository.LogTransactionAsync(request, response, "Authentication failed");
                return response;
            }

            // Step 3: Validate signature
            var isSignatureValid = _signatureService.ValidateSignature(request);
            if (!isSignatureValid)
            {
                response.Result = 0;
                response.ResultMessage = "Access Denied!";
                await _logRepository.LogTransactionAsync(request, response, "Signature validation failed");
                return response;
            }

            // Step 4: Calculate discount and final amount
            var (totalDiscount, finalAmount) = _discountService.CalculateDiscount(request.TotalAmount);

            // Step 5: Build success response
            response.Result = 1;
            response.TotalAmount = request.TotalAmount;
            response.TotalDiscount = totalDiscount;
            response.FinalAmount = finalAmount;

            await _logRepository.LogTransactionAsync(request, response, "Transaction processed successfully");
            _logger.LogInformation("Transaction processed successfully for partner: {PartnerKey}", request.PartnerKey);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing transaction for partner: {PartnerKey}", request.PartnerKey);

            response.Result = 0;
            response.ResultMessage = "Internal server error occurred.";
            await _logRepository.LogTransactionAsync(request, response, $"Exception: {ex.Message}");

            return response;
        }
    }
}