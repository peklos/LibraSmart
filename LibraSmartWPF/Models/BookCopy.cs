using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraSmartWPF.Models;

/// <summary>
/// Экземпляры книг (физические копии в библиотеках)
/// </summary>
[Table("book_copies")]
public class BookCopy
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("book_id")]
    public int BookId { get; set; }

    [Required]
    [Column("library_id")]
    public int LibraryId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("inventory_number")]
    public string InventoryNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "available"; // available, on_loan, maintenance, lost

    // Связи
    [ForeignKey(nameof(BookId))]
    public virtual Book Book { get; set; } = null!;

    [ForeignKey(nameof(LibraryId))]
    public virtual Library Library { get; set; } = null!;

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
