using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraSmartWPF.Models;

/// <summary>
/// Библиотеки (филиалы)
/// </summary>
[Table("libraries")]
public class Library
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("library_name")]
    public string LibraryName { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    [Column("address")]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("phone")]
    public string Phone { get; set; } = string.Empty;

    // Связи
    public virtual ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
    public virtual ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
