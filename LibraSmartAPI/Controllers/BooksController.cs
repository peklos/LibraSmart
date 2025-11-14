using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraSmartAPI.Data;
using LibraSmartAPI.Models;

namespace LibraSmartAPI.Controllers;

[ApiController]
public class BooksController : ControllerBase
{
    private readonly LibraryContext _context;

    public BooksController(LibraryContext context)
    {
        _context = context;
    }

    // READER: Get books catalog with filters
    [HttpGet("api/books")]
    public async Task<IActionResult> GetBooksCatalog(
        [FromQuery] int? genre_id,
        [FromQuery] string? author,
        [FromQuery] string? search)
    {
        var query = _context.Books.Include(b => b.Genre).AsQueryable();

        if (genre_id.HasValue)
        {
            query = query.Where(b => b.GenreId == genre_id.Value);
        }

        if (!string.IsNullOrEmpty(author))
        {
            query = query.Where(b => b.Author.Contains(author));
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
        }

        var books = await query.ToListAsync();

        var result = books.Select(b => new
        {
            id = b.Id,
            title = b.Title,
            author = b.Author,
            publication_year = b.PublicationYear,
            genre_id = b.GenreId,
            description = b.Description,
            isbn = b.ISBN,
            genre_name = b.Genre.GenreName
        });

        return Ok(result);
    }

    // READER: Get book details
    [HttpGet("api/books/{book_id}")]
    public async Task<IActionResult> GetBookDetails(int book_id)
    {
        var book = await _context.Books
            .Include(b => b.Genre)
            .FirstOrDefaultAsync(b => b.Id == book_id);

        if (book == null)
        {
            return NotFound(new { detail = "Book not found" });
        }

        return Ok(new
        {
            id = book.Id,
            title = book.Title,
            author = book.Author,
            publication_year = book.PublicationYear,
            genre_id = book.GenreId,
            description = book.Description,
            isbn = book.ISBN,
            genre_name = book.Genre.GenreName
        });
    }

    // READER: Get book availability in libraries
    [HttpGet("api/books/{book_id}/availability")]
    public async Task<IActionResult> GetBookAvailability(int book_id)
    {
        var book = await _context.Books.FindAsync(book_id);

        if (book == null)
        {
            return NotFound(new { detail = "Book not found" });
        }

        var copies = await _context.BookCopies
            .Include(c => c.Library)
            .Where(c => c.BookId == book_id)
            .ToListAsync();

        var availability = copies
            .GroupBy(c => c.LibraryId)
            .Select(g => new
            {
                library_id = g.Key,
                library_name = g.First().Library.LibraryName,
                total = g.Count(),
                available = g.Count(c => c.Status == "available"),
                on_loan = g.Count(c => c.Status == "on_loan"),
                maintenance = g.Count(c => c.Status == "maintenance")
            })
            .ToList();

        return Ok(new
        {
            book_id = book_id,
            book_title = book.Title,
            libraries = availability
        });
    }

    // ADMIN: Get all books
    [HttpGet("api/admin/books")]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _context.Books.Include(b => b.Genre).ToListAsync();

        var result = books.Select(b => new
        {
            id = b.Id,
            title = b.Title,
            author = b.Author,
            publication_year = b.PublicationYear,
            genre_id = b.GenreId,
            description = b.Description,
            isbn = b.ISBN,
            genre_name = b.Genre.GenreName
        });

        return Ok(result);
    }

    // ADMIN: Create book
    [HttpPost("api/admin/books")]
    public async Task<IActionResult> CreateBook([FromBody] BookCreateRequest request)
    {
        var genre = await _context.Genres.FindAsync(request.genre_id);
        if (genre == null)
        {
            return NotFound(new { detail = "Genre not found" });
        }

        var book = new Book
        {
            Title = request.title,
            Author = request.author,
            PublicationYear = request.publication_year,
            GenreId = request.genre_id,
            Description = request.description,
            ISBN = request.isbn
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = book.Id,
            title = book.Title,
            author = book.Author,
            publication_year = book.PublicationYear,
            genre_id = book.GenreId,
            description = book.Description,
            isbn = book.ISBN
        });
    }

    // ADMIN: Update book
    [HttpPatch("api/admin/books/{book_id}")]
    public async Task<IActionResult> UpdateBook(int book_id, [FromBody] BookUpdateRequest request)
    {
        var book = await _context.Books.FindAsync(book_id);

        if (book == null)
        {
            return NotFound(new { detail = "Book not found" });
        }

        if (request.title != null) book.Title = request.title;
        if (request.author != null) book.Author = request.author;
        if (request.publication_year.HasValue) book.PublicationYear = request.publication_year;
        if (request.genre_id.HasValue)
        {
            var genre = await _context.Genres.FindAsync(request.genre_id.Value);
            if (genre == null)
            {
                return NotFound(new { detail = "Genre not found" });
            }
            book.GenreId = request.genre_id.Value;
        }
        if (request.description != null) book.Description = request.description;
        if (request.isbn != null) book.ISBN = request.isbn;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = book.Id,
            title = book.Title,
            author = book.Author,
            publication_year = book.PublicationYear,
            genre_id = book.GenreId,
            description = book.Description,
            isbn = book.ISBN
        });
    }

    // ADMIN: Delete book
    [HttpDelete("api/admin/books/{book_id}")]
    public async Task<IActionResult> DeleteBook(int book_id)
    {
        var book = await _context.Books.FindAsync(book_id);

        if (book == null)
        {
            return NotFound(new { detail = "Book not found" });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return Ok(new { detail = "Book deleted" });
    }
}

public class BookCreateRequest
{
    public string title { get; set; } = string.Empty;
    public string author { get; set; } = string.Empty;
    public int? publication_year { get; set; }
    public int genre_id { get; set; }
    public string? description { get; set; }
    public string? isbn { get; set; }
}

public class BookUpdateRequest
{
    public string? title { get; set; }
    public string? author { get; set; }
    public int? publication_year { get; set; }
    public int? genre_id { get; set; }
    public string? description { get; set; }
    public string? isbn { get; set; }
}
