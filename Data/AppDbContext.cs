using Microsoft.EntityFrameworkCore;
using BookstoreCatalog.Mvc.Models;
using System.Net;

namespace BookstoreCatalog.Mvc.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("Genres");
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(150);
            entity.Property(b => b.Price).HasColumnType("decimal(18,2)");
            entity.HasOne(b => b.Genre)
                  .WithMany(g => g.Books)
                  .HasForeignKey(b => b.GenreId);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sales");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasMany(o => o.SaleItems)
                  .WithOne(oi => oi.Sale)
                  .HasForeignKey(oi => oi.SaleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.ToTable("SaleItems");
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(oi => oi.Sale)
                  .WithMany(o => o.SaleItems)
                  .HasForeignKey(oi => oi.SaleId);
            entity.HasOne(oi => oi.Book)
                  .WithMany(p => p.SaleItems)
                  .HasForeignKey(oi => oi.BookId);
        });

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Romance" },
            new Genre { Id = 2, Name = "Chill" },
            new Genre { Id = 3, Name = "Self-Help" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Đắc Nhân Tâm", Author = "Dale Carnegie", Price = 130000, Stock = 10, GenreId = 3, ISBN = "8935280919266" },
            new Book { Id = 2, Title = "Thực sắc", Author = "Ninh Viễn", Price = 320000, Stock = 15, GenreId = 1, ISBN = "9786045598351" },
            new Book { Id = 3, Title = "Rooms Tuyển tập tranh minh họa", Author = "Senbon Umishima", Price = 200000, Stock = 21, GenreId = 2, ISBN = "9786044809953" },
            new Book { Id = 4, Title = "Rồi hoa sẽ nở - Bloom into you", Author = "Nakatani Nio", Price = 1500000, Stock = 7, GenreId = 1, ISBN = "9786043828627" },
            new Book { Id = 5, Title = "Tuổi trẻ đáng giá bao nhiêu", Author = "Tuệ Nghi", Price = 90000, Stock = 3, GenreId = 3, ISBN = "9786043199703" }
        );
    }
}