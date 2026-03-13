using Microsoft.EntityFrameworkCore;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Interfaces;
using ONFarm.Infrastructure.Data;

namespace ONFarm.Infrastructure.Repositories;

public class RappelRepository : IRappelRepository
{
    private readonly IDbContextFactory<ONFarmDbContext> _factory;

    public RappelRepository(IDbContextFactory<ONFarmDbContext> factory)
        => _factory = factory;

    public async Task<IEnumerable<Rappel>> GetDuJourAsync()
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var today = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        var tomorrow = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(1), DateTimeKind.Utc);

        return await ctx.Rappels
            .Include(r => r.Patient)
            .Where(r => r.DateRappel >= today && r.DateRappel < tomorrow)
            .OrderBy(r => r.DateRappel)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rappel>> GetByPatientAsync(Guid patientId)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Rappels
            .Where(r => r.PatientId == patientId)
            .OrderBy(r => r.DateRappel)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rappel>> GetNonVusAsync()
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Rappels
            .Include(r => r.Patient)
            .Where(r => !r.EstVu)
            .OrderBy(r => r.DateRappel)
            .ToListAsync();
    }

    public async Task<Rappel> AddAsync(Rappel rappel)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Rappels.Add(rappel);
        await ctx.SaveChangesAsync();
        return rappel;
    }

    public async Task<Rappel> UpdateAsync(Rappel rappel)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Rappels.Update(rappel);
        await ctx.SaveChangesAsync();
        return rappel;
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var r = await ctx.Rappels.FindAsync(id);
        if (r is null) return;
        ctx.Rappels.Remove(r);
        await ctx.SaveChangesAsync();
    }
}