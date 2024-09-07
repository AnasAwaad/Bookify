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
        public DbSet<Area> Areas { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalCopy> RentalCopies { get; set; }
        public DbSet<Subscriper> Subscripers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<BookCategory>()
                   .HasKey(b => new { b.CategoryId, b.BookId });
            builder.Entity<RentalCopy>()
                   .HasKey(b => new { b.RentalId, b.BookCopyId });

            // add sequence to shared schema in db
            builder.HasSequence<int>("SerialNumber", "Shared")
                   .StartsAt(1000001);

            // apply sequence to BookCopy Model
            builder.Entity<BookCopy>()
                .Property(e => e.SerialNumber)
                .HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");

            builder.Entity<Rental>().HasQueryFilter(r => r.IsActive);
            builder.Entity<RentalCopy>().HasQueryFilter(r => r.Rental!.IsActive);

            // change behavior of foreign key to restrict behavior
            var cascadeFKs = builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

            foreach (var item in cascadeFKs)
            {
                item.DeleteBehavior = DeleteBehavior.Restrict;
            }



        }
    }
}
