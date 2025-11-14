using Microsoft.AspNetCore.Mvc;
using LibraSmartAPI.Services;

namespace LibraSmartAPI.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    // Reader login (Vue frontend expects /api/auth/login)
    [HttpPost("api/auth/login")]
    public IActionResult LoginReader([FromBody] LoginRequest request)
    {
        var (success, token, reader) = _authService.AuthenticateReader(request.Email, request.Password);

        if (!success || reader == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        return Ok(new
        {
            id = reader.Id,
            full_name = reader.FullName,
            email = reader.Email,
            phone = reader.Phone,
            library_card_number = reader.LibraryCardNumber,
            token
        });
    }

    // Staff login (Vue frontend expects /api/admin/auth/login)
    [HttpPost("api/admin/auth/login")]
    public IActionResult LoginStaff([FromBody] LoginRequest request)
    {
        var (success, token, staff) = _authService.AuthenticateStaff(request.Email, request.Password);

        if (!success || staff == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        return Ok(new
        {
            id = staff.Id,
            full_name = staff.FullName,
            email = staff.Email,
            position = staff.Position,
            library_id = staff.LibraryId,
            role_id = staff.RoleId,
            token
        });
    }

    // Reader profile
    [HttpGet("api/auth/me/{id}")]
    public IActionResult GetReaderProfile(int id)
    {
        // TODO: implement
        return Ok(new { id, message = "Not implemented yet" });
    }

    // Staff profile
    [HttpGet("api/admin/auth/me/{id}")]
    public IActionResult GetStaffProfile(int id)
    {
        // TODO: implement
        return Ok(new { id, message = "Not implemented yet" });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
