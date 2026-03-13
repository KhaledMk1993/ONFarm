using Microsoft.EntityFrameworkCore;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Interfaces;
using ONFarm.Infrastructure.Data;

namespace ONFarm.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly IDbContextFactory<ONFarmDbContext> _factory;

    public PatientRepository(IDbContextFactory<ONFarmDbContext> factory)
        => _factory = factory;

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Patients
            .Include(p => p.Ordonnances)
            .Include(p => p.Medicaments)
            .Include(p => p.Rappels)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Nom)
            .ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(Guid id)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        return await ctx.Patients
            .Include(p => p.Ordonnances)
            .Include(p => p.Medicaments)
            .Include(p => p.Rappels)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string query)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var q = query.ToLower();
        return await ctx.Patients
            .Include(p => p.Ordonnances)
            .Where(p => p.IsActive &&
                (p.Nom.ToLower().Contains(q) ||
                 p.Prenom.ToLower().Contains(q) ||
                 (p.Adresse != null && p.Adresse.ToLower().Contains(q))))
            .OrderBy(p => p.Nom)
            .ToListAsync();
    }

    public async Task<Patient> AddAsync(Patient patient)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Patients.Add(patient);
        await ctx.SaveChangesAsync();
        return patient;
    }

    public async Task<Patient> UpdateAsync(Patient patient)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        ctx.Patients.Update(patient);
        await ctx.SaveChangesAsync();
        return patient;
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        var patient = await ctx.Patients.FindAsync(id);
        if (patient is null) return;
        patient.IsActive = false; // Soft delete
        await ctx.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Patient> patients)
    {
        await using var ctx = await _factory.CreateDbContextAsync();
        await ctx.Patients.AddRangeAsync(patients);
        await ctx.SaveChangesAsync();
    }
}