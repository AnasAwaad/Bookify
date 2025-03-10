﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Persistence.Configurations;
internal class SubscriperUserConfiguration : IEntityTypeConfiguration<Subscriper>
{
    public void Configure(EntityTypeBuilder<Subscriper> builder)
    {
        builder.HasIndex(e=>e.NationalId).IsUnique();
        builder.HasIndex(e=>e.Email).IsUnique();
        builder.HasIndex(e=>e.MobileNumber).IsUnique();

        builder.Property(e=>e.FirstName).HasMaxLength(100);
        builder.Property(e=>e.LastName).HasMaxLength(100);
        builder.Property(e=>e.NationalId).HasMaxLength(20);
        builder.Property(e=>e.MobileNumber).HasMaxLength(15);
        builder.Property(e=>e.Email).HasMaxLength(150);
        builder.Property(e=>e.ImageUrl).HasMaxLength(500);
        builder.Property(e=>e.ImageThumbnailUrl).HasMaxLength(500);
        builder.Property(e=>e.Address).HasMaxLength(500);

        builder.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
    }
}
