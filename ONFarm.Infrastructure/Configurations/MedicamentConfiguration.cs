using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONFarm.Domain.Entities;

namespace ONFarm.Infrastructure.Configurations;
public class MedicamentConfiguration : IEntityTypeConfiguration<Medicament>
{
    public void Configure(EntityTypeBuilder<Medicament> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Nom).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Dosage).HasMaxLength(100);
        builder.Property(m => m.Frequence).HasMaxLength(100);
    }
}
