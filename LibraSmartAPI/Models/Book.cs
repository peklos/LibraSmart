using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraSmartAPI.Models;

/// <summary>
/// Книги
/// </summary>
[Table("books")]
public class Book
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(300)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Column("author")]
    public string Author { get; set; } = string.Empty;

    [Column("publication_year")]
    public int? PublicationYear { get; set; }

    [Required]
    [Column("genre_id")]
    public int GenreId { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [MaxLength(20)]
    [Column("isbn")]
    public string? ISBN { get; set; }

    // Связи
    [ForeignKey(nameof(GenreId))]
    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<BookCopy> Copies { get; set; } = new List<BookCopy>();
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
