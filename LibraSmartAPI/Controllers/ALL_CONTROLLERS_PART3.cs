// Part 3: Copies, Staff, Profile, History, Stats

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraSmartAPI.Data;
using LibraSmartAPI.Models;

namespace LibraSmartAPI.Controllers;

// ===================== COPIES CONTROLLER =====================
[ApiController]
public class CopiesController : ControllerBase
{
    private readonly LibraryContext _context;

    public CopiesController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet("api/admin/copies")]
    public async Task<IActionResult> GetAllCopies()
    {
        var copies = await _context.BookCopies
            .Include(c => c.Book)
            .Include(c => c.Library)
            .ToListAsync();

        return Ok(copies.Select(c => new
        {
            id = c.Id,
            book_id = c.BookId,
            library_id = c.LibraryId,
            inventory_number = c.InventoryNumber,
            status = c.Status,
            book_title = c.Book.Title,
            library_name = c.Library.LibraryName
        }));
    }

    [HttpGet("api/admin/copies/library/{library_id}")]
    public async Task<IActionResult> GetCopiesByLibrary(int library_id)
    {
        var copies = await _context.BookCopies
            .Include(c => c.Book)
            .Include(c => c.Library)
            .Where(c => c.LibraryId == library_id)
            .ToListAsync();

        return Ok(copies.Select(c => new
        {
            id = c.Id,
            book_id = c.BookId,
            library_id = c.LibraryId,
            inventory_number = c.InventoryNumber,
            status = c.Status,
            book_title = c.Book.Title,
            library_name = c.Library.LibraryName
        }));
    }

    [HttpPost("api/admin/copies")]
    public async Task<IActionResult> CreateCopy([FromBody] CopyCreateRequest request)
    {
        var copy = new BookCopy
        {
            BookId = request.book_id,
            LibraryId = request.library_id,
            InventoryNumber = request.inventory_number,
            Status = request.status ?? "available"
        };

        _context.BookCopies.Add(copy);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = copy.Id,
            book_id = copy.BookId,
            library_id = copy.LibraryId,
            inventory_number = copy.InventoryNumber,
            status = copy.Status
        });
    }

    [HttpPatch("api/admin/copies/{copy_id}")]
    public async Task<IActionResult> UpdateCopy(int copy_id, [FromBody] CopyUpdateRequest request)
    {
        var copy = await _context.BookCopies.FindAsync(copy_id);
        if (copy == null) return NotFound(new { detail = "Copy not found" });

        if (request.status != null) copy.Status = request.status;
        if (request.inventory_number != null) copy.InventoryNumber = request.inventory_number;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = copy.Id,
            book_id = copy.BookId,
            library_id = copy.LibraryId,
            inventory_number = copy.InventoryNumber,
            status = copy.Status
        });
    }

    [HttpDelete("api/admin/copies/{copy_id}")]
    public async Task<IActionResult> DeleteCopy(int copy_id)
    {
        var copy = await _context.BookCopies.FindAsync(copy_id);
        if (copy == null) return NotFound(new { detail = "Copy not found" });

        _context.BookCopies.Remove(copy);
        await _context.SaveChangesAsync();

        return Ok(new { detail = "Copy deleted" });
    }
}

// ===================== STAFF CONTROLLER =====================
[ApiController]
public class StaffController : ControllerBase
{
    private readonly LibraryContext _context;

    public StaffController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet("api/admin/staff")]
    public async Task<IActionResult> GetAllStaff([FromQuery] int current_staff_id)
    {
        var staff = await _context.Staff
            .Include(s => s.Role)
            .Include(s => s.Library)
            .ToListAsync();

        return Ok(staff.Select(s => new
        {
            id = s.Id,
            full_name = s.FullName,
            position = s.Position,
            library_id = s.LibraryId,
            email = s.Email,
            role_id = s.RoleId,
            library_name = s.Library.LibraryName,
            role_name = s.Role.Name
        }));
    }

    [HttpPost("api/admin/staff")]
    public async Task<IActionResult> CreateStaff([FromQuery] int current_staff_id, [FromBody] StaffCreateRequest request)
    {
        if (await _context.Staff.AnyAsync(s => s.Email == request.email))
            return BadRequest(new { detail = "Email already exists" });

        var newStaff = new Staff
        {
            FullName = request.full_name,
            Position = request.position,
            LibraryId = request.library_id,
            Email = request.email,
            Password = request.password,
            RoleId = request.role_id
        };

        _context.Staff.Add(newStaff);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = newStaff.Id,
            full_name = newStaff.FullName,
            position = newStaff.Position,
            library_id = newStaff.LibraryId,
            email = newStaff.Email,
            role_id = newStaff.RoleId
        });
    }

    [HttpPatch("api/admin/staff/{staff_id}")]
    public async Task<IActionResult> UpdateStaff(int staff_id, [FromQuery] int current_staff_id, [FromBody] StaffUpdateRequest request)
    {
        var staff = await _context.Staff.FindAsync(staff_id);
        if (staff == null) return NotFound(new { detail = "Staff not found" });

        if (request.full_name != null) staff.FullName = request.full_name;
        if (request.position != null) staff.Position = request.position;
        if (request.library_id.HasValue) staff.LibraryId = request.library_id.Value;
        if (request.email != null) staff.Email = request.email;
        if (request.password != null) staff.Password = request.password;
        if (request.role_id.HasValue) staff.RoleId = request.role_id.Value;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = staff.Id,
            full_name = staff.FullName,
            position = staff.Position,
            library_id = staff.LibraryId,
            email = staff.Email,
            role_id = staff.RoleId
        });
    }

    [HttpDelete("api/admin/staff/{staff_id}")]
    public async Task<IActionResult> DeleteStaff(int staff_id, [FromQuery] int current_staff_id)
    {
        var staff = await _context.Staff.FindAsync(staff_id);
        if (staff == null) return NotFound(new { detail = "Staff not found" });

        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();

        return Ok(new { detail = "Staff deleted" });
    }
}

// ===================== PROFILE CONTROLLER =====================
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly LibraryContext _context;

    public ProfileController(LibraryContext context)
    {
        _context = context;
    }

    [HttpPatch("api/profile/{reader_id}")]
    public async Task<IActionResult> UpdateProfile(int reader_id, [FromBody] ProfileUpdateRequest request)
    {
        var reader = await _context.Readers.FindAsync(reader_id);
        if (reader == null) return NotFound(new { detail = "Reader not found" });

        if (request.full_name != null) reader.FullName = request.full_name;
        if (request.email != null) reader.Email = request.email;
        if (request.password != null) reader.Password = request.password;
        if (request.phone != null) reader.Phone = request.phone;

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
}

// ===================== HISTORY CONTROLLER =====================
[ApiController]
public class HistoryController : ControllerBase
{
    private readonly LibraryContext _context;

    public HistoryController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet("api/history/{reader_id}")]
    public async Task<IActionResult> GetReadingHistory(int reader_id)
    {
        var loans = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book)
            .Include(l => l.Staff)
            .Where(l => l.ReaderId == reader_id && l.ReturnDate != null)
            .OrderByDescending(l => l.ReturnDate)
            .ToListAsync();

        return Ok(loans.Select(l => new
        {
            id = l.Id,
            reader_id = l.ReaderId,
            copy_id = l.CopyId,
            staff_id = l.StaffId,
            loan_date = l.LoanDate,
            due_date = l.DueDate,
            return_date = l.ReturnDate,
            status = l.Status,
            book_title = l.Copy.Book.Title,
            staff_name = l.Staff.FullName
        }));
    }

    [HttpGet("api/history/{reader_id}/stats")]
    public async Task<IActionResult> GetReadingStats(int reader_id)
    {
        var loans = await _context.Loans
            .Where(l => l.ReaderId == reader_id)
            .ToListAsync();

        return Ok(new
        {
            total_loans = loans.Count,
            active_loans = loans.Count(l => l.Status == "active"),
            returned_loans = loans.Count(l => l.Status == "returned"),
            overdue_loans = loans.Count(l => l.Status == "overdue")
        });
    }
}

// ===================== STATS CONTROLLER =====================
[ApiController]
public class StatsController : ControllerBase
{
    private readonly LibraryContext _context;

    public StatsController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet("api/admin/stats/dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        return Ok(new
        {
            total_books = await _context.Books.CountAsync(),
            total_readers = await _context.Readers.CountAsync(),
            total_loans = await _context.Loans.CountAsync(),
            active_loans = await _context.Loans.CountAsync(l => l.Status == "active"),
            overdue_loans = await _context.Loans.CountAsync(l => l.Status == "overdue")
        });
    }

    [HttpGet("api/admin/stats/popular-books")]
    public async Task<IActionResult> GetPopularBooks([FromQuery] int limit = 10)
    {
        var popularBooks = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book)
            .GroupBy(l => l.Copy.BookId)
            .Select(g => new
            {
                book_id = g.Key,
                book_title = g.First().Copy.Book.Title,
                loan_count = g.Count()
            })
            .OrderByDescending(x => x.loan_count)
            .Take(limit)
            .ToListAsync();

        return Ok(popularBooks);
    }

    [HttpGet("api/admin/stats/popular-genres")]
    public async Task<IActionResult> GetPopularGenres([FromQuery] int limit = 10)
    {
        var popularGenres = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book).ThenInclude(b => b.Genre)
            .GroupBy(l => l.Copy.Book.GenreId)
            .Select(g => new
            {
                genre_id = g.Key,
                genre_name = g.First().Copy.Book.Genre.GenreName,
                loan_count = g.Count()
            })
            .OrderByDescending(x => x.loan_count)
            .Take(limit)
            .ToListAsync();

        return Ok(popularGenres);
    }

    [HttpGet("api/admin/stats/active-readers")]
    public async Task<IActionResult> GetActiveReaders([FromQuery] int limit = 10)
    {
        var activeReaders = await _context.Loans
            .Include(l => l.Reader)
            .GroupBy(l => l.ReaderId)
            .Select(g => new
            {
                reader_id = g.Key,
                reader_name = g.First().Reader.FullName,
                loan_count = g.Count()
            })
            .OrderByDescending(x => x.loan_count)
            .Take(limit)
            .ToListAsync();

        return Ok(activeReaders);
    }

    [HttpGet("api/admin/stats/library/{library_id}")]
    public async Task<IActionResult> GetLibraryStats(int library_id)
    {
        var totalCopies = await _context.BookCopies.CountAsync(c => c.LibraryId == library_id);
        var availableCopies = await _context.BookCopies.CountAsync(c => c.LibraryId == library_id && c.Status == "available");

        return Ok(new
        {
            library_id,
            total_copies = totalCopies,
            available_copies = availableCopies,
            on_loan_copies = await _context.BookCopies.CountAsync(c => c.LibraryId == library_id && c.Status == "on_loan")
        });
    }
}

// ===================== REQUEST MODELS =====================
public class CopyCreateRequest { public int book_id { get; set; } public int library_id { get; set; } public string inventory_number { get; set; } = ""; public string? status { get; set; } }
public class CopyUpdateRequest { public string? status { get; set; } public string? inventory_number { get; set; } }
public class StaffCreateRequest { public string full_name { get; set; } = ""; public string position { get; set; } = ""; public int library_id { get; set; } public string email { get; set; } = ""; public string password { get; set; } = ""; public int role_id { get; set; } }
public class StaffUpdateRequest { public string? full_name { get; set; } public string? position { get; set; } public int? library_id { get; set; } public string? email { get; set; } public string? password { get; set; } public int? role_id { get; set; } }
public class ProfileUpdateRequest { public string? full_name { get; set; } public string? email { get; set; } public string? password { get; set; } public string? phone { get; set; } }
