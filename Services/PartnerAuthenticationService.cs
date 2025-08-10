using System.Security.Cryptography;
using System.Text;
using TransactionAPI.Models;
using TransactionAPI.Services.Interfaces;

namespace TransactionAPI.Services;

public class SignatureValidationService : ISignatureValidationService
{
    public bool ValidateSignature(TransactionRequest request)
    {
        try
        {
            var expectedSignature = GenerateSignature(request);
            //maybe need to trim the signature since the sample has space
            if (expectedSignature == request.Sig)
                return true;
            else
                return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string GenerateSignature(TransactionRequest request)
    {
        var signatureString = BuildSignatureString(request);
        var hash = ComputeSha256Hash(signatureString);
        var base64Hash = Convert.ToBase64String(Encoding.UTF8.GetBytes(hash));

        return base64Hash;
    }

    private string BuildSignatureString(TransactionRequest request)
    {
        DateTime dt = DateTime.Parse(request.Timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind);
        string timestamp = dt.ToString("yyyyMMddHHmmss");
        //var timestamp = DateTime.Parse(request.Timestamp).ToString("yyyyMMddHHmmss");
        return $"{timestamp}{request.PartnerKey}{request.PartnerRefNo}{request.TotalAmount}{request.PartnerPassword}";
        // Parse the ISO 8601 timestamp and convert to yyyyMMddHHmmss format
        //if (DateTime.TryParse(request.Timestamp, out var parsedTimestamp))
        //{
        //    var formattedTimestamp = parsedTimestamp.ToString("yyyyMMddHHmmss");
        //    return $"{formattedTimestamp}{request.PartnerKey}{request.PartnerRefNo}{request.TotalAmount}{request.PartnerPassword}";
        //}

        //throw new ArgumentException("Invalid timestamp format");
    }

    private string ComputeSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLower();
    }
}