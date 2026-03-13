using Microsoft.EntityFrameworkCore;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Interfaces;
using ONFarm.Infrastructure.Data;

namespace ONFarm.Infrastructure.Repositories;

public class MedicamentRepository : IMedicamentRepository
{
    private readonly IDbContextFactory<ONFarmDbContext> _factory;

    public MedicamentRepository(IDbContextFactory<ONFarmDbContext> factory)
        => _factory = factory;

    public async Task<IEnumerable<Medicament>> GetByPatientAsync(Guid patientId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Medicaments
            .Where(m => m.PatientId == patientId)
            .ToListAsync();
    }

    public async Task<Medicament> AddAsync(Medicament medicament)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Medicaments.Add(medicament);
        await ctx.SaveChangesAsync();
        return medicament;
    }

    public async Task<Medicament> UpdateAsync(Medicament medicament)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Medicaments.Update(medicament);
        await ctx.SaveChangesAsync();
        return medicament;
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var m = await ctx.Medicaments.FindAsync(id);
        if (m is null) return;
        ctx.Medicaments.Remove(m);
        await ctx.SaveChangesAsync();
    }
}