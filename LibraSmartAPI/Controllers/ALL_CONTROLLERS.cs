// This file contains ALL controllers migrated from Python backend
// Copy-paste individual controllers to separate files later if needed

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraSmartAPI.Data;
using LibraSmartAPI.Models;

namespace LibraSmartAPI.Controllers;

// ===================== GENRES CONTROLLER =====================
[ApiController]
public class GenresController : ControllerBase
{
    private readonly LibraryContext _context;

    public GenresController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet("api/admin/genres")]
    public async Task<IActionResult> GetAllGenres()
    {
        var genres = await _context.Genres.ToListAsync();
        return Ok(genres.Select(g => new { id = g.Id, genre_name = g.GenreName }));
    }

    [HttpPost("api/admin/genres")]
    public async Task<IActionResult> CreateGenre([FromBody] GenreRequest request)
    {
        var genre = new Genre { GenreName = request.genre_name };
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();
        return Ok(new { id = genre.Id, genre_name = genre.GenreName });
    }

    [HttpPatch("api/admin/genres/{genre_id}")]
    public async Task<IActionResult> UpdateGenre(int genre_id, [FromBody] GenreRequest request)
    {
        var genre = await _context.Genres.FindAsync(genre_id);
        if (genre == null) return NotFound(new { detail = "Genre not found" });
        genre.GenreName = request.genre_name;
        await _context.SaveChangesAsync();
        return Ok(new { id = genre.Id, genre_name = genre.GenreName });
    }

    [HttpDelete("api/admin/genres/{genre_id}")]
    public async Task<IActionResult> DeleteGenre(int genre_id)
    {
        var genre = await _context.Genres.FindAsync(genre_id);
        if (genre == null) return NotFound(new { detail = "Genre not found" });
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
        return Ok(new { detail = "Genre deleted" });
    }
}

// ===================== LIBRARIES CONTROLLER =====================
[ApiController]
public class LibrariesController : ControllerBase
{
    private readonly LibraryContext _context;

    public LibrariesController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet("api/admin/libraries")]
    public async Task<IActionResult> GetAllLibraries()
    {
        var libraries = await _context.Libraries.ToListAsync();
        return Ok(libraries.Select(l => new
        {
            id = l.Id,
            library_name = l.LibraryName,
            address = l.Address,
            phone = l.Phone
        }));
    }

    [HttpPost("api/admin/libraries")]
    public async Task<IActionResult> CreateLibrary([FromBody] LibraryRequest request)
    {
        var library = new Library
        {
            LibraryName = request.library_name,
            Address = request.address,
            Phone = request.phone
        };
        _context.Libraries.Add(library);
        await _context.SaveChangesAsync();
        return Ok(new
        {
            id = library.Id,
            library_name = library.LibraryName,
            address = library.Address,
            phone = library.Phone
        });
    }

    [HttpPatch("api/admin/libraries/{library_id}")]
    public async Task<IActionResult> UpdateLibrary(int library_id, [FromBody] LibraryUpdateRequest request)
    {
        var library = await _context.Libraries.FindAsync(library_id);
        if (library == null) return NotFound(new { detail = "Library not found" });

        if (request.library_name != null) library.LibraryName = request.library_name;
        if (request.address != null) library.Address = request.address;
        if (request.phone != null) library.Phone = request.phone;

        await _context.SaveChangesAsync();
        return Ok(new
        {
            id = library.Id,
            library_name = library.LibraryName,
            address = library.Address,
            phone = library.Phone
        });
    }

    [HttpDelete("api/admin/libraries/{library_id}")]
    public async Task<IActionResult> DeleteLibrary(int library_id)
    {
        var library = await _context.Libraries.FindAsync(library_id);
        if (library == null) return NotFound(new { detail = "Library not found" });
        _context.Libraries.Remove(library);
        await _context.SaveChangesAsync();
        return Ok(new { detail = "Library deleted" });
    }
}

// ===================== READERS CONTROLLER =====================
[ApiController]
public class AdminReadersController : ControllerBase
{
    private readonly LibraryContext _context;

    public AdminReadersController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet("api/admin/readers")]
    public async Task<IActionResult> GetAllReaders()
    {
        var readers = await _context.Readers.ToListAsync();
        return Ok(readers.Select(r => new
        {
            id = r.Id,
            full_name = r.FullName,
            email = r.Email,
            phone = r.Phone,
            library_card_number = r.LibraryCardNumber,
            created_at = r.CreatedAt
        }));
    }

    [HttpGet("api/admin/readers/{reader_id}")]
    public async Task<IActionResult> GetReader(int reader_id)
    {
        var reader = await _context.Readers.FindAsync(reader_id);
        if (reader == null) return NotFound(new { detail = "Reader not found" });
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

    [HttpPost("api/admin/readers")]
    public async Task<IActionResult> CreateReader([FromBody] ReaderCreateRequest request)
    {
        if (await _context.Readers.AnyAsync(r => r.Email == request.email))
            return BadRequest(new { detail = "Email already exists" });

        if (await _context.Readers.AnyAsync(r => r.LibraryCardNumber == request.library_card_number))
            return BadRequest(new { detail = "Library card number already exists" });

        var reader = new Reader
        {
            FullName = request.full_name,
            Email = request.email,
            Password = request.password,
            Phone = request.phone,
            LibraryCardNumber = request.library_card_number,
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

    [HttpPatch("api/admin/readers/{reader_id}")]
    public async Task<IActionResult> UpdateReader(int reader_id, [FromBody] ReaderUpdateRequest request)
    {
        var reader = await _context.Readers.FindAsync(reader_id);
        if (reader == null) return NotFound(new { detail = "Reader not found" });

        if (request.full_name != null) reader.FullName = request.full_name;
        if (request.email != null) reader.Email = request.email;
        if (request.password != null) reader.Password = request.password;
        if (request.phone != null) reader.Phone = request.phone;
        if (request.library_card_number != null) reader.LibraryCardNumber = request.library_card_number;

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

    [HttpDelete("api/admin/readers/{reader_id}")]
    public async Task<IActionResult> DeleteReader(int reader_id)
    {
        var reader = await _context.Readers.FindAsync(reader_id);
        if (reader == null) return NotFound(new { detail = "Reader not found" });
        _context.Readers.Remove(reader);
        await _context.SaveChangesAsync();
        return Ok(new { detail = "Reader deleted" });
    }

    [HttpGet("api/admin/readers/{reader_id}/loans")]
    public async Task<IActionResult> GetReaderLoans(int reader_id)
    {
        var loans = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book)
            .Include(l => l.Staff)
            .Where(l => l.ReaderId == reader_id)
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
}

// ===================== REQUEST MODELS =====================
public class GenreRequest { public string genre_name { get; set; } = ""; }
public class LibraryRequest { public string library_name { get; set; } = ""; public string? address { get; set; } public string? phone { get; set; } }
public class LibraryUpdateRequest { public string? library_name { get; set; } public string? address { get; set; } public string? phone { get; set; } }
public class ReaderCreateRequest { public string full_name { get; set; } = ""; public string email { get; set; } = ""; public string password { get; set; } = ""; public string? phone { get; set; } public string library_card_number { get; set; } = ""; }
public class ReaderUpdateRequest { public string? full_name { get; set; } public string? email { get; set; } public string? password { get; set; } public string? phone { get; set; } public string? library_card_number { get; set; } }
