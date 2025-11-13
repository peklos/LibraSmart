using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraSmartWPF.Models;

/// <summary>
/// Бронирования книг
/// </summary>
[Table("reservations")]
public class Reservation
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("reader_id")]
    public int ReaderId { get; set; }

    [Required]
    [Column("book_id")]
    public int BookId { get; set; }

    [Required]
    [Column("library_id")]
    public int LibraryId { get; set; }

    [Column("reservation_date")]
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "active"; // active, completed, cancelled

    // Связи
    [ForeignKey(nameof(ReaderId))]
    public virtual Reader Reader { get; set; } = null!;

    [ForeignKey(nameof(BookId))]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey(nameof(LibraryId))]
    public virtual Library Library { get; set; } = null!;
}
