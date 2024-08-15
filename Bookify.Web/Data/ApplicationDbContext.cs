using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<BookCategory>()
                   .HasKey(b => new { b.CategoryId, b.BookId });

            // add sequence to shared schema in db
            builder.HasSequence<int>("SerialNumber", "Shared")
                   .StartsAt(1000001);

            // apply sequence to BookCopy Model
            builder.Entity<BookCopy>()
                .Property(e => e.SerialNumber)
                .HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");

            // change name of identity tables
            //builder.Entity<IdentityUser>().ToTable("Users");
            //builder.Entity<IdentityRole>().ToTable("Roles");
            //builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");



        }
    }
}
