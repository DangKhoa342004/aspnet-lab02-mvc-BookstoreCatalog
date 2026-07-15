using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BookstoreCatalog.Mvc.Models;
using System.Net;
using System.Data;
using System.Dynamic;

namespace BookstoreCatalog.Mvc.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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
            entity.Property(b => b.ISBN).IsRequired().HasMaxLength(100);
            entity.Property(b => b.Price).HasColumnType("decimal(18,2)");
            entity.Property(b => b.RowVersion).IsRowVersion();

            entity.HasOne(b => b.Genre).WithMany(g => g.Books).HasForeignKey(b => b.GenreId);
            entity.HasQueryFilter(b => !b.IsDeleted);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Sales");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

            entity.HasMany(o => o.SaleItems).WithOne(oi => oi.Sale).HasForeignKey(oi => oi.SaleId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.ToTable("SaleItems");
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");

            entity.HasOne(oi => oi.Sale).WithMany(o => o.SaleItems).HasForeignKey(oi => oi.SaleId);
            entity.HasOne(oi => oi.Book).WithMany(p => p.SaleItems).HasForeignKey(oi => oi.BookId).IsRequired(false);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Action).IsRequired().HasMaxLength(50);    
            entity.Property(l => l.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(l => l.EntityId).HasMaxLength(50);
            entity.Property(l => l.UserName).HasMaxLength(100);
            entity.Property(l => l.IpAddress).HasMaxLength(45);
            entity.Property(l => l.Result).IsRequired().HasMaxLength(20);
            entity.Property(l => l.Note).HasMaxLength(500);
        });

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Romance" },
            new Genre { Id = 2, Name = "Chill" },
            new Genre { Id = 3, Name = "Self-Help" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, ISBN = "893-528-09-1926-6", Title = "Đắc Nhân Tâm", Author = "Dale Carnegie", 
                Price = 130000, Quantity = 10, MinStock = 3, GenreId = 3, 
                CreatedAt = new DateTime(2026,1,2), IsDeleted = false, RowVersion = Array.Empty<byte>() },
            new Book { Id = 2, ISBN = "978-604-55-9835-1", Title = "Thực sắc", Author = "Ninh Viễn", 
                Price = 320000, Quantity = 15, MinStock = 4, GenreId = 1, 
                CreatedAt = new DateTime(2023,5,12), IsDeleted = false, RowVersion = Array.Empty<byte>() },
            new Book { Id = 3, ISBN = "978-604-48-0995-3", Title = "Rooms Tuyển tập tranh minh họa", Author = "Senbon Umishima", 
                Price = 200000, Quantity = 21, MinStock = 4, GenreId = 2, 
                CreatedAt = new DateTime(2024,12,3), IsDeleted = false, RowVersion = Array.Empty<byte>() },
            new Book { Id = 4, ISBN = "978-604-38-2862-7", Title = "Rồi hoa sẽ nở - Bloom into you", Author = "Nakatani Nio", 
                Price = 1500000, Quantity = 7, MinStock = 6, GenreId = 1, 
                CreatedAt = new DateTime(2026,4,7), IsDeleted = false, RowVersion = Array.Empty<byte>() },
            new Book { Id = 5, ISBN = "978-604-31-9970-3", Title = "Tuổi trẻ đáng giá bao nhiêu", Author = "Tuệ Nghi", 
                Price = 90000, Quantity = 3, GenreId = 3, MinStock = 10, 
                CreatedAt = new DateTime(2024,2,5), IsDeleted = false, RowVersion = Array.Empty<byte>() }
        );
    }
}