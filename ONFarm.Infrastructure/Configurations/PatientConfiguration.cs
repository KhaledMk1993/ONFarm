using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ONFarm.Domain.Entities;

namespace ONFarm.Infrastructure.Configurations;
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Nom).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Prenom).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Telephone).HasMaxLength(20);
        builder.Property(p => p.Adresse).HasMaxLength(250);

        builder.Ignore(p => p.NomComplet);
        builder.Ignore(p => p.DerniereFacture);
        builder.Ignore(p => p.ProchaineFacture);

        builder.HasMany(p => p.Medicaments)
            .WithOne(m => m.Patient)
            .HasForeignKey(m => m.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Ordonnances)
            .WithOne(o => o.Patient)
            .HasForeignKey(o => o.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Rappels)
            .WithOne(r => r.Patient)
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
