using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraSmartAPI.Models;

/// <summary>
/// Роли библиотекарей
/// </summary>
[Table("roles")]
public class Role
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    // Связи
    public virtual ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
}
