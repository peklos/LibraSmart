using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LibraSmartAPI.Data;
using LibraSmartAPI.Models;

namespace LibraSmartAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly LibraryContext _context;
    private readonly IConfiguration _configuration;

    public AuthenticationService(LibraryContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public string GenerateJwtToken(string userId, string userType, string email)
    {
        var secret = _configuration["JWT:Secret"] ?? "LibraSmart_Super_Secret_Key_2024_Min32Characters!";
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, userType),
            new Claim("user_type", userType)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (bool Success, string? Token, Reader? Reader) AuthenticateReader(string email, string password)
    {
        var reader = _context.Readers.FirstOrDefault(r => r.Email == email);

        if (reader == null || reader.Password != password)
        {
            return (false, null, null);
        }

        var token = GenerateJwtToken(reader.Id.ToString(), "reader", reader.Email);
        return (true, token, reader);
    }

    public (bool Success, string? Token, Staff? Staff) AuthenticateStaff(string email, string password)
    {
        var staff = _context.Staff.FirstOrDefault(s => s.Email == email);

        if (staff == null || staff.Password != password)
        {
            return (false, null, null);
        }

        var token = GenerateJwtToken(staff.Id.ToString(), "staff", staff.Email);
        return (true, token, staff);
    }
}
