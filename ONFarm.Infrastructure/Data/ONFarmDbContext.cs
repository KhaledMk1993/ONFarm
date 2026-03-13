using Microsoft.EntityFrameworkCore;
using ONFarm.Domain.Entities;

namespace ONFarm.Infrastructure.Data;

public class ONFarmDbContext(DbContextOptions<ONFarmDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Medicament> Medicaments => Set<Medicament>();
    public DbSet<Ordonnance> Ordonnances => Set<Ordonnance>();
    public DbSet<Rappel> Rappels => Set<Rappel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ONFarmDbContext).Assembly);
    }
}