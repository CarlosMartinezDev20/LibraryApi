using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasCharSet("utf8mb4");
        modelBuilder.UseCollation("utf8mb4_unicode_ci");

        // Relaciones
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Loans)
            .WithOne(l => l.Book)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        modelBuilder.Entity<Book>()
            .HasIndex(b => new { b.Title, b.Author });

        // ⚠️ Unicidad de Email solo para registros no eliminados
        // MySQL no soporta índices filtrados tipo "WHERE IsDeleted = 0", así que usamos compuesto.
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Email, u.IsDeleted })
            .IsUnique();

        // 🔎 Filtros globales de soft delete
        modelBuilder.Entity<Book>().HasQueryFilter(b => !b.IsDeleted);
        modelBuilder.Entity<Loan>().HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
    }
}
