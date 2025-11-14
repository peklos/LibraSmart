using Microsoft.AspNetCore.Mvc;
using LibraSmartAPI.Services;

namespace LibraSmartAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("login/reader")]
    public IActionResult LoginReader([FromBody] LoginRequest request)
    {
        var (success, token, reader) = _authService.AuthenticateReader(request.Email, request.Password);

        if (!success || reader == null)
        {
            return Unauthorized(new { message = "Неверный email или пароль" });
        }

        return Ok(new
        {
            token,
            user = new
            {
                id = reader.Id,
                fullName = reader.FullName,
                email = reader.Email,
                phone = reader.Phone,
                libraryCardNumber = reader.LibraryCardNumber,
                type = "reader"
            }
        });
    }

    [HttpPost("login/staff")]
    public IActionResult LoginStaff([FromBody] LoginRequest request)
    {
        var (success, token, staff) = _authService.AuthenticateStaff(request.Email, request.Password);

        if (!success || staff == null)
        {
            return Unauthorized(new { message = "Неверный email или пароль" });
        }

        return Ok(new
        {
            token,
            user = new
            {
                id = staff.Id,
                fullName = staff.FullName,
                email = staff.Email,
                position = staff.Position,
                libraryId = staff.LibraryId,
                roleId = staff.RoleId,
                type = "staff"
            }
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
