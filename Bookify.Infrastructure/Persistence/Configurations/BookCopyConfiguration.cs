using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Persistence.Configurations;
internal class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        // apply sequence to BookCopy Model
        builder.Property(e => e.SerialNumber)
            .HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");

        builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
    }
}
