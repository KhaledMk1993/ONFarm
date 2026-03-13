using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONFarm.Domain.Entities;

namespace ONFarm.Infrastructure.Configurations;
public class RappelConfiguration : IEntityTypeConfiguration<Rappel>
{
    public void Configure(EntityTypeBuilder<Rappel> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Message).HasMaxLength(500);
    }
}
