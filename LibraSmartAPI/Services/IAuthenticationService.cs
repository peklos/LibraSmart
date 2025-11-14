using LibraSmartAPI.Models;

namespace LibraSmartAPI.Services;

public interface IAuthenticationService
{
    string GenerateJwtToken(string userId, string userType, string email);
    (bool Success, string? Token, Reader? Reader) AuthenticateReader(string email, string password);
    (bool Success, string? Token, Staff? Staff) AuthenticateStaff(string email, string password);
}
