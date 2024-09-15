using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Persistence.Configurations;
internal class RentalCopyConfiguration : IEntityTypeConfiguration<RentalCopy>
{
    public void Configure(EntityTypeBuilder<RentalCopy> builder)
    {
        builder.HasKey(b => new { b.RentalId, b.BookCopyId });

        builder.HasQueryFilter(r => r.Rental!.IsActive);

        builder.Property(e => e.RentalDate).HasDefaultValueSql("CAST(GETDATE() AS Date)");
    }
}
