using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraSmartAPI.Models;

/// <summary>
/// Библиотекари/сотрудники
/// </summary>
[Table("staff")]
public class Staff
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("position")]
    public string Position { get; set; } = string.Empty;

    [Required]
    [Column("library_id")]
    public int LibraryId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Column("role_id")]
    public int RoleId { get; set; }

    // Связи
    [ForeignKey(nameof(RoleId))]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey(nameof(LibraryId))]
    public virtual Library Library { get; set; } = null!;

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
