using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraSmartAPI.Models;

/// <summary>
/// Жанры книг
/// </summary>
[Table("genres")]
public class Genre
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("genre_name")]
    public string GenreName { get; set; } = string.Empty;

    // Связи
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
