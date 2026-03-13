using Microsoft.EntityFrameworkCore;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Interfaces;
using ONFarm.Infrastructure.Data;

namespace ONFarm.Infrastructure.Repositories;

public class OrdonnanceRepository : IOrdonnanceRepository
{
    private readonly IDbContextFactory<ONFarmDbContext> _factory;

    public OrdonnanceRepository(IDbContextFactory<ONFarmDbContext> factory)
        => _factory = factory;

    public async Task<IEnumerable<Ordonnance>> GetByPatientAsync(Guid patientId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Ordonnances
            .Where(o => o.PatientId == patientId)
            .OrderByDescending(o => o.DateFacture)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ordonnance>> GetImmimentesAsync()
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        var limite = today.AddDays(7);

        return await ctx.Ordonnances
            .Include(o => o.Patient)
            .Where(o => o.ProchainRenouvellement >= today &&
                        o.ProchainRenouvellement <= limite)
            .OrderBy(o => o.ProchainRenouvellement)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ordonnance>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        
        var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
        var toUtc = DateTime.SpecifyKind(to.Date.AddDays(1), DateTimeKind.Utc);

        return await ctx.Ordonnances
            .Include(o => o.Patient)
            .Where(o => o.DateFacture >= fromUtc && o.DateFacture < toUtc)
            .OrderByDescending(o => o.DateFacture)
            .ToListAsync();
    }

    public async Task<Ordonnance> AddAsync(Ordonnance ordonnance)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Ordonnances.Add(ordonnance);
        await ctx.SaveChangesAsync();
        return ordonnance;
    }

    public async Task<Ordonnance> UpdateAsync(Ordonnance ordonnance)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Ordonnances.Update(ordonnance);
        await ctx.SaveChangesAsync();
        return ordonnance;
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var o = await ctx.Ordonnances.FindAsync(id);
        if (o is null) return;
        ctx.Ordonnances.Remove(o);
        await ctx.SaveChangesAsync();
    }
}