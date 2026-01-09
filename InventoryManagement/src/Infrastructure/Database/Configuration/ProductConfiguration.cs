using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.WmsProductId)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.AcquireDate)
            .IsRequired();

        builder.Property(p => p.SoldDate)
            .IsRequired(false);

        builder.Property(p => p.CancelDate)
            .IsRequired(false);

        builder.Property(p => p.ReturnDate)
            .IsRequired(false);

        builder.OwnsOne(p => p.AcquisitionCost, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("AcquisitionCost_Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            price.Property(m => m.Currency)
                .HasColumnName("AcquisitionCost_Currency")
                .HasConversion(
                    c => c.Code,
                    code => Domain.ValueObjects.Currency.FromCode(code))
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(p => p.AcquisitionCostUSD, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("AcquisitionCostUSD_Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            price.Property(m => m.Currency)
                .HasColumnName("AcquisitionCostUSD_Currency")
                .HasConversion(
                    c => c.Code,
                    code => Domain.ValueObjects.Currency.FromCode(code))
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey("SupplierId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey("CategoryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(p => p.Events);
    }
}
