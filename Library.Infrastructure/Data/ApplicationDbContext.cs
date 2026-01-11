using Library.Domain.Entities;
using Library.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Loan> Loans => Set<Loan>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Book>(e =>
        {
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Author).HasMaxLength(200);
            e.Property(x => x.Isbn).HasMaxLength(50);
        });
        builder.Entity<Loan>(e =>
        {
            e.HasOne(x => x.Book)
             .WithMany()
             .HasForeignKey(x => x.BookId);

            e.Property(x => x.Quantity).IsRequired();
        });
    }
}