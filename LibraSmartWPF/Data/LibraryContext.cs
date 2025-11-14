using Microsoft.EntityFrameworkCore;
using LibraSmartWPF.Models;

namespace LibraSmartWPF.Data;

public class LibraryContext : DbContext
{
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Library> Libraries { get; set; } = null!;
    public DbSet<Staff> Staff { get; set; } = null!;
    public DbSet<Reader> Readers { get; set; } = null!;
    public DbSet<Genre> Genres { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<BookCopy> BookCopies { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Loan> Loans { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Путь к базе данных в папке пользователя
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LibraSmart",
            "librasmart.db"
        );

        // Создаем папку если её нет
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка индексов
        modelBuilder.Entity<Reader>()
            .HasIndex(r => r.Email)
            .IsUnique();

        modelBuilder.Entity<Reader>()
            .HasIndex(r => r.LibraryCardNumber)
            .IsUnique();

        modelBuilder.Entity<Staff>()
            .HasIndex(s => s.Email)
            .IsUnique();

        modelBuilder.Entity<BookCopy>()
            .HasIndex(bc => bc.InventoryNumber)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();

        modelBuilder.Entity<Genre>()
            .HasIndex(g => g.GenreName)
            .IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

        // Настройка каскадного удаления
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Reader)
            .WithMany(r => r.Loans)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Copy)
            .WithMany(c => c.Loans)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Staff)
            .WithMany(s => s.Loans)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
