using Microsoft.EntityFrameworkCore;

namespace Library_Management.Models
{
    public class AppDbContext:DbContext
    {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
                base.OnModelCreating(modelBuilder);

                // Book: ISBN unique
                modelBuilder.Entity<Book>()
                    .HasIndex(b => b.ISBN)
                    .IsUnique();

                // Member: Email unique
                modelBuilder.Entity<Member>()
                    .HasIndex(m => m.Email)
                    .IsUnique();

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Seller)
                .WithMany() // أو WithMany(m => m.Books) لو ضفت Navigation property في Member
                .HasForeignKey(b => b.SellerId)
                .OnDelete(DeleteBehavior.Restrict); // يمنع cascade delete

        }
        // DbSets = الجداول
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Borrowing> Borrowings => Set<Borrowing>();

    }
}
