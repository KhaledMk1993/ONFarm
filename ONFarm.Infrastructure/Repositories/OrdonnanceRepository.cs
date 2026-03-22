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

    // ── Helper : force Kind=Utc sur un DateTime ──
    private static DateTime ToUtc(DateTime dt)
        => dt.Kind == DateTimeKind.Utc
            ? dt
            : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    // ── Normalise les 3 champs DateTime de l'entité ──
    private static void NormalizeUtc(Ordonnance o)
    {
        o.DateFacture = ToUtc(o.DateFacture);
        o.ProchainRenouvellement = ToUtc(o.ProchainRenouvellement);
        o.DateCreation = ToUtc(o.DateCreation);
    }

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

        // FIX 1 : UTC
        NormalizeUtc(ordonnance);

        // FIX 2 : duplicate PK_Patients
        // Le Patient existe déjà en BD. On dit à EF qu'il est Unchanged
        // pour qu'il ne tente pas de le re-insérer.
        // La FK PatientId sur l'ordonnance suffit à établir la relation.
        if (ordonnance.Patient is not null)
            ctx.Entry(ordonnance.Patient).State = EntityState.Unchanged;

        ctx.Ordonnances.Add(ordonnance);
        await ctx.SaveChangesAsync();
        return ordonnance;
    }

    public async Task<Ordonnance> UpdateAsync(Ordonnance ordonnance)
    {
        await using var ctx = await _factory.CreateDbContextAsync();

        // FIX 1 : UTC
        NormalizeUtc(ordonnance);

        // FIX 2 : même protection pour l'update
        if (ordonnance.Patient is not null)
            ctx.Entry(ordonnance.Patient).State = EntityState.Unchanged;

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