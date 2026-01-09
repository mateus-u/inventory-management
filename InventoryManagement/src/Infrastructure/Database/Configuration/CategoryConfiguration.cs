using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Shortcode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey("ParentId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.Shortcode).IsUnique();

        builder.Ignore(x => x.Events);
    }
}
