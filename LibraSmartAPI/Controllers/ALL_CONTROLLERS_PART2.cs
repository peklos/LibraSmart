// Part 2: Reservations, Loans, Copies, Staff, Profile, History, Stats

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraSmartAPI.Data;
using LibraSmartAPI.Models;

namespace LibraSmartAPI.Controllers;

// ===================== RESERVATIONS CONTROLLER =====================
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly LibraryContext _context;

    public ReservationsController(LibraryContext context)
    {
        _context = context;
    }

    // READER: Create reservation
    [HttpPost("api/reservations")]
    public async Task<IActionResult> CreateReservation([FromQuery] int reader_id, [FromBody] ReservationCreateRequest request)
    {
        var reader = await _context.Readers.FindAsync(reader_id);
        if (reader == null) return NotFound(new { detail = "Reader not found" });

        var book = await _context.Books.FindAsync(request.book_id);
        if (book == null) return NotFound(new { detail = "Book not found" });

        var library = await _context.Libraries.FindAsync(request.library_id);
        if (library == null) return NotFound(new { detail = "Library not found" });

        var existing = await _context.Reservations.FirstOrDefaultAsync(r =>
            r.ReaderId == reader_id && r.BookId == request.book_id && r.Status == "active");

        if (existing != null)
            return BadRequest(new { detail = "You already have an active reservation for this book" });

        var reservation = new Reservation
        {
            ReaderId = reader_id,
            BookId = request.book_id,
            LibraryId = request.library_id,
            ReservationDate = DateTime.UtcNow,
            Status = "active"
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = reservation.Id,
            reader_id = reservation.ReaderId,
            book_id = reservation.BookId,
            library_id = reservation.LibraryId,
            reservation_date = reservation.ReservationDate,
            status = reservation.Status
        });
    }

    // READER: Get my reservations
    [HttpGet("api/reservations/my/{reader_id}")]
    public async Task<IActionResult> GetMyReservations(int reader_id)
    {
        var reservations = await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.Reader)
            .Include(r => r.Library)
            .Where(r => r.ReaderId == reader_id)
            .ToListAsync();

        return Ok(reservations.Select(r => new
        {
            id = r.Id,
            reader_id = r.ReaderId,
            book_id = r.BookId,
            library_id = r.LibraryId,
            reservation_date = r.ReservationDate,
            status = r.Status,
            book_title = r.Book.Title,
            reader_name = r.Reader.FullName,
            library_name = r.Library.LibraryName
        }));
    }

    // READER: Cancel reservation
    [HttpDelete("api/reservations/{reservation_id}")]
    public async Task<IActionResult> CancelReservation(int reservation_id)
    {
        var reservation = await _context.Reservations.FindAsync(reservation_id);
        if (reservation == null) return NotFound(new { detail = "Reservation not found" });

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return Ok(new { detail = "Reservation cancelled" });
    }

    // ADMIN: Get all reservations
    [HttpGet("api/admin/reservations")]
    public async Task<IActionResult> GetAllReservations()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.Reader)
            .Include(r => r.Library)
            .ToListAsync();

        return Ok(reservations.Select(r => new
        {
            id = r.Id,
            reader_id = r.ReaderId,
            book_id = r.BookId,
            library_id = r.LibraryId,
            reservation_date = r.ReservationDate,
            status = r.Status,
            book_title = r.Book.Title,
            reader_name = r.Reader.FullName,
            library_name = r.Library.LibraryName
        }));
    }

    // ADMIN: Get active reservations
    [HttpGet("api/admin/reservations/active")]
    public async Task<IActionResult> GetActiveReservations()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Book)
            .Include(r => r.Reader)
            .Include(r => r.Library)
            .Where(r => r.Status == "active")
            .ToListAsync();

        return Ok(reservations.Select(r => new
        {
            id = r.Id,
            reader_id = r.ReaderId,
            book_id = r.BookId,
            library_id = r.LibraryId,
            reservation_date = r.ReservationDate,
            status = r.Status,
            book_title = r.Book.Title,
            reader_name = r.Reader.FullName,
            library_name = r.Library.LibraryName
        }));
    }

    // ADMIN: Update reservation
    [HttpPatch("api/admin/reservations/{reservation_id}")]
    public async Task<IActionResult> UpdateReservation(int reservation_id, [FromBody] ReservationUpdateRequest request)
    {
        var reservation = await _context.Reservations.FindAsync(reservation_id);
        if (reservation == null) return NotFound(new { detail = "Reservation not found" });

        reservation.Status = request.status;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = reservation.Id,
            reader_id = reservation.ReaderId,
            book_id = reservation.BookId,
            library_id = reservation.LibraryId,
            reservation_date = reservation.ReservationDate,
            status = reservation.Status
        });
    }

    // ADMIN: Delete reservation
    [HttpDelete("api/admin/reservations/{reservation_id}")]
    public async Task<IActionResult> DeleteReservation(int reservation_id)
    {
        var reservation = await _context.Reservations.FindAsync(reservation_id);
        if (reservation == null) return NotFound(new { detail = "Reservation not found" });

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return Ok(new { detail = "Reservation deleted" });
    }
}

// ===================== LOANS CONTROLLER =====================
[ApiController]
public class LoansController : ControllerBase
{
    private readonly LibraryContext _context;

    public LoansController(LibraryContext context)
    {
        _context = context;
    }

    // READER: Get my loans
    [HttpGet("api/loans/my/{reader_id}")]
    public async Task<IActionResult> GetMyLoans(int reader_id)
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

    // READER: Get active loans
    [HttpGet("api/loans/my/{reader_id}/active")]
    public async Task<IActionResult> GetActiveLoans(int reader_id)
    {
        var loans = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book)
            .Include(l => l.Staff)
            .Where(l => l.ReaderId == reader_id && l.Status == "active")
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

    // READER: Get overdue loans
    [HttpGet("api/loans/my/{reader_id}/overdue")]
    public async Task<IActionResult> GetOverdueLoans(int reader_id)
    {
        var loans = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book)
            .Include(l => l.Staff)
            .Where(l => l.ReaderId == reader_id && l.Status == "overdue")
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

    // ADMIN: Get all loans
    [HttpGet("api/admin/loans")]
    public async Task<IActionResult> GetAllLoans()
    {
        var loans = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book)
            .Include(l => l.Reader)
            .Include(l => l.Staff)
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
            reader_name = l.Reader.FullName,
            staff_name = l.Staff.FullName
        }));
    }

    // ADMIN: Create loan
    [HttpPost("api/admin/loans")]
    public async Task<IActionResult> CreateLoan([FromBody] LoanCreateRequest request)
    {
        var loan = new Loan
        {
            ReaderId = request.reader_id,
            CopyId = request.copy_id,
            StaffId = request.staff_id,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            Status = "active"
        };

        _context.Loans.Add(loan);

        var copy = await _context.BookCopies.FindAsync(request.copy_id);
        if (copy != null) copy.Status = "on_loan";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = loan.Id,
            reader_id = loan.ReaderId,
            copy_id = loan.CopyId,
            staff_id = loan.StaffId,
            loan_date = loan.LoanDate,
            due_date = loan.DueDate,
            status = loan.Status
        });
    }

    // ADMIN: Return loan
    [HttpPatch("api/admin/loans/{loan_id}/return")]
    public async Task<IActionResult> ReturnLoan(int loan_id)
    {
        var loan = await _context.Loans.Include(l => l.Copy).FirstOrDefaultAsync(l => l.Id == loan_id);
        if (loan == null) return NotFound(new { detail = "Loan not found" });

        loan.ReturnDate = DateTime.UtcNow;
        loan.Status = "returned";

        if (loan.Copy != null) loan.Copy.Status = "available";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            id = loan.Id,
            reader_id = loan.ReaderId,
            copy_id = loan.CopyId,
            staff_id = loan.StaffId,
            loan_date = loan.LoanDate,
            due_date = loan.DueDate,
            return_date = loan.ReturnDate,
            status = loan.Status
        });
    }

    // ADMIN: Get overdue loans
    [HttpGet("api/admin/loans/overdue")]
    public async Task<IActionResult> GetAllOverdueLoans()
    {
        var loans = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book)
            .Include(l => l.Reader)
            .Include(l => l.Staff)
            .Where(l => l.Status == "overdue")
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
            reader_name = l.Reader.FullName,
            staff_name = l.Staff.FullName
        }));
    }

    // ADMIN: Get active loans
    [HttpGet("api/admin/loans/active")]
    public async Task<IActionResult> GetAllActiveLoans()
    {
        var loans = await _context.Loans
            .Include(l => l.Copy).ThenInclude(c => c.Book)
            .Include(l => l.Reader)
            .Include(l => l.Staff)
            .Where(l => l.Status == "active")
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
            reader_name = l.Reader.FullName,
            staff_name = l.Staff.FullName
        }));
    }
}

// ===================== REQUEST MODELS =====================
public class ReservationCreateRequest { public int book_id { get; set; } public int library_id { get; set; } }
public class ReservationUpdateRequest { public string status { get; set; } = ""; }
public class LoanCreateRequest { public int reader_id { get; set; } public int copy_id { get; set; } public int staff_id { get; set; } }
