using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONFarm.Domain.Entities;

namespace ONFarm.Infrastructure.Configurations;
public class OrdonnanceConfiguration : IEntityTypeConfiguration<Ordonnance>
{
    public void Configure(EntityTypeBuilder<Ordonnance> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Statut).HasConversion<string>();
        builder.Ignore(o => o.EstImminente);
        builder.Ignore(o => o.EstEnRetard);
    }
}
