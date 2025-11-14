using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraSmartWPF.Models;

/// <summary>
/// Выдачи книг (активные займы)
/// </summary>
[Table("loans")]
public class Loan
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("reader_id")]
    public int ReaderId { get; set; }

    [Required]
    [Column("copy_id")]
    public int CopyId { get; set; }

    [Required]
    [Column("staff_id")]
    public int StaffId { get; set; }

    [Required]
    [Column("loan_date")]
    public DateTime LoanDate { get; set; }

    [Required]
    [Column("due_date")]
    public DateTime DueDate { get; set; }

    [Column("return_date")]
    public DateTime? ReturnDate { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("status")]
    public string Status { get; set; } = "active"; // active, returned, overdue

    // Связи
    [ForeignKey(nameof(ReaderId))]
    public virtual Reader Reader { get; set; } = null!;

    [ForeignKey(nameof(CopyId))]
    public virtual BookCopy Copy { get; set; } = null!;

    [ForeignKey(nameof(StaffId))]
    public virtual Staff Staff { get; set; } = null!;
}
