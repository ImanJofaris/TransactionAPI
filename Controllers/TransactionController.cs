using Microsoft.AspNetCore.Mvc;
using TransactionAPI.Models;
using TransactionAPI.Services.Interfaces;

namespace TransactionAPI.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionProcessingService _transactionService;
    private readonly ILogger<TransactionController> _logger;

    public TransactionController(
        ITransactionProcessingService transactionService,
        ILogger<TransactionController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpPost("submittrxmessage")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionResponse>> SubmitTransaction([FromBody] TransactionRequest request)
    {
        _logger.LogInformation("Received transaction request from partner: {PartnerKey}", request?.PartnerKey);

        if (request == null)
        {
            var errorResponse = new TransactionResponse
            {
                Result = 0,
                ResultMessage = "Request body is required."
            };
            return Ok(errorResponse); // Return 200 with error in body as per API spec
        }

        var response = await _transactionService.ProcessTransactionAsync(request);
        return Ok(response);
    }
}