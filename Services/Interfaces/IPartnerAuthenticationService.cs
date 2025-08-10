using Microsoft.AspNetCore.Mvc;
using TransactionAPI.Models;

namespace TransactionAPI.Services.Interfaces
{
    public interface IPartnerAuthenticationService
    {
        Task<bool> AuthenticatePartnerAsync(string partnerKey, string password);
    }
}
