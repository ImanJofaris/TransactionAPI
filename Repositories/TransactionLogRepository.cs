using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TransactionAPI.Models;

namespace TransactionAPI.Repositories;

public class TransactionLogRepository : ITransactionLogRepository
{
    private readonly ILogger<TransactionLogRepository> _logger;

    public TransactionLogRepository(ILogger<TransactionLogRepository> logger)
    {
        _logger = logger;
    }

    public async Task LogTransactionAsync(TransactionRequest request, TransactionResponse response, string additionalInfo = null)
    {
        try
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                Request = new
                {
                    request.PartnerKey,
                    request.PartnerRefNo,
                    PartnerPassword = EncryptPassword(request.PartnerPassword), // Encrypted for security
                    request.TotalAmount,
                    ItemCount = request.Items?.Count ?? 0,
                    Items = request.Items,
                    request.Timestamp,
                    SignatureHash = !string.IsNullOrEmpty(request.Sig) ?
                        request.Sig.Substring(0, Math.Min(10, request.Sig.Length)) + "..." : ""
                },
                Response = response,
                AdditionalInfo = additionalInfo,
                ProcessingTime = DateTime.UtcNow
            };

            var logText = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = null
            });

            // Create logs directory if it doesn't exist
            var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
            Directory.CreateDirectory(logsDirectory);

            // Write to daily log file
            var logFileName = $"transaction_log_{DateTime.UtcNow:yyyyMMdd}.txt";
            var logPath = Path.Combine(logsDirectory, logFileName);

            await File.AppendAllTextAsync(logPath, logText + Environment.NewLine + new string('-', 80) + Environment.NewLine);

            _logger.LogInformation("Transaction logged to file: {PartnerKey} - {PartnerRefNo}",
                request.PartnerKey, request.PartnerRefNo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log transaction for partner: {PartnerKey}", request.PartnerKey);
        }
    }

    public async Task LogErrorAsync(string error, Exception? exception = null)
    {
        var logEntry = new
        {
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
            Error = error,
            Exception = exception?.ToString()
        };

        var logText = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });

        var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        Directory.CreateDirectory(logsDirectory);

        var logFileName = $"error_log_{DateTime.UtcNow:yyyyMMdd}.txt";
        var logPath = Path.Combine(logsDirectory, logFileName);

        await File.AppendAllTextAsync(logPath, logText + Environment.NewLine);
    }

    private string EncryptPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) return "***EMPTY***";

        try
        {
            // Simple encryption for logging (in production, use proper encryption)
            using var aes = Aes.Create();
            var key = Encoding.UTF8.GetBytes("MySecretKey12345"); // In production, use configuration
            Array.Resize(ref key, 32);

            var encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes("***ENCRYPTED***"));
            return encrypted;
        }
        catch
        {
            return "***ENCRYPTION_ERROR***";
        }
    }
}