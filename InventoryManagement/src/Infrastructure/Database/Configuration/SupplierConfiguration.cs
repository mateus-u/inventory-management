using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Address)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.Property(s => s.Currency)
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromCode(code))
            .HasColumnName("Currency")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(s => s.Country)
            .HasConversion(
                country => country.Code,
                code => Country.FromCode(code))
            .HasColumnName("Country")
            .HasMaxLength(2)
            .IsRequired();

        builder.Ignore(x => x.Events);
    }
}
