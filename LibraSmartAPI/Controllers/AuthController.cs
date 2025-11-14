using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraSmartAPI.Data;
using LibraSmartAPI.Models;

namespace LibraSmartAPI.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly LibraryContext _context;

    public AuthController(LibraryContext context)
    {
        _context = context;
    }

    // Reader registration
    [HttpPost("api/auth/register")]
    public async Task<IActionResult> RegisterReader([FromBody] ReaderRegisterRequest request)
    {
        // Check if email exists
        if (await _context.Readers.AnyAsync(r => r.Email == request.email))
        {
            return BadRequest(new { detail = "Email already registered" });
        }

        // Generate library card number
        var lastReader = await _context.Readers.OrderByDescending(r => r.Id).FirstOrDefaultAsync();
        string cardNumber;
        if (lastReader != null)
        {
            var lastNumber = int.Parse(lastReader.LibraryCardNumber.Split('-').Last());
            cardNumber = $"LIB-2024-{(lastNumber + 1):D3}";
        }
        else
        {
            cardNumber = "LIB-2024-001";
        }

        var reader = new Reader
        {
            FullName = request.full_name,
            Email = request.email,
            Password = request.password,
            Phone = request.phone,
            LibraryCardNumber = cardNumber,
            CreatedAt = DateTime.UtcNow
        };

        _context.Readers.Add(reader);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = reader.Id,
            full_name = reader.FullName,
            email = reader.Email,
            phone = reader.Phone,
            library_card_number = reader.LibraryCardNumber,
            created_at = reader.CreatedAt
        });
    }

    // Reader login
    [HttpPost("api/auth/login")]
    public async Task<IActionResult> LoginReader([FromBody] LoginRequest request)
    {
        var reader = await _context.Readers.FirstOrDefaultAsync(r => r.Email == request.email);

        if (reader == null || reader.Password != request.password)
        {
            return Unauthorized(new { detail = "Invalid email or password" });
        }

        return Ok(new
        {
            id = reader.Id,
            full_name = reader.FullName,
            email = reader.Email,
            phone = reader.Phone,
            library_card_number = reader.LibraryCardNumber,
            created_at = reader.CreatedAt
        });
    }

    // Reader profile
    [HttpGet("api/auth/me/{reader_id}")]
    public async Task<IActionResult> GetReaderProfile(int reader_id)
    {
        var reader = await _context.Readers.FindAsync(reader_id);

        if (reader == null)
        {
            return NotFound(new { detail = "Reader not found" });
        }

        return Ok(new
        {
            id = reader.Id,
            full_name = reader.FullName,
            email = reader.Email,
            phone = reader.Phone,
            library_card_number = reader.LibraryCardNumber,
            created_at = reader.CreatedAt
        });
    }

    // Staff login
    [HttpPost("api/admin/auth/login")]
    public async Task<IActionResult> LoginStaff([FromBody] LoginRequest request)
    {
        var staff = await _context.Staff
            .Include(s => s.Role)
            .Include(s => s.Library)
            .FirstOrDefaultAsync(s => s.Email == request.email);

        if (staff == null || staff.Password != request.password)
        {
            return Unauthorized(new { detail = "Invalid email or password" });
        }

        return Ok(new
        {
            id = staff.Id,
            full_name = staff.FullName,
            email = staff.Email,
            position = staff.Position,
            library_id = staff.LibraryId,
            role_id = staff.RoleId
        });
    }

    // Staff profile
    [HttpGet("api/admin/auth/me/{staff_id}")]
    public async Task<IActionResult> GetStaffProfile(int staff_id)
    {
        var staff = await _context.Staff
            .Include(s => s.Role)
            .Include(s => s.Library)
            .FirstOrDefaultAsync(s => s.Id == staff_id);

        if (staff == null)
        {
            return NotFound(new { detail = "Staff not found" });
        }

        return Ok(new
        {
            id = staff.Id,
            full_name = staff.FullName,
            email = staff.Email,
            position = staff.Position,
            library_id = staff.LibraryId,
            role_id = staff.RoleId
        });
    }
}

public class LoginRequest
{
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}

public class ReaderRegisterRequest
{
    public string full_name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string? phone { get; set; }
}
