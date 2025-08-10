using TransactionAPI.Services.Interfaces;
using TransactionAPI.Models; 
using System.Text;


public class PartnerAuthenticationService : IPartnerAuthenticationService
{
    private readonly List<Partner> _allowedPartners = new()
    {
        new Partner { PartnerNo = "FG-00001", PartnerKey = "FAKEGOOGLE", Password = "FAKEPASSWORD1234" },
        new Partner { PartnerNo = "FG-00002", PartnerKey = "FAKEPEOPLE", Password = "FAKEPASSWORD4578" }
    };

    public Task<bool> AuthenticatePartnerAsync(string partnerKey, string encodedPassword)
    {
        try
        {
            var decodedPassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPassword));
            var partner = _allowedPartners.FirstOrDefault(p => p.PartnerKey == partnerKey);

            return Task.FromResult(partner?.Password == decodedPassword);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}