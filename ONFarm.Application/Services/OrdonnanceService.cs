using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Enums;
using ONFarm.Domain.Interfaces;

namespace ONFarm.Application.Services;

public class OrdonnanceService : IOrdonnanceService
{
    private readonly IOrdonnanceRepository _repo;

    public OrdonnanceService(IOrdonnanceRepository repo)
        => _repo = repo;

    public Task<IEnumerable<Ordonnance>> GetByPatientAsync(Guid patientId)
        => _repo.GetByPatientAsync(patientId);

    public Task<IEnumerable<Ordonnance>> GetOrdonnancesImmimentesAsync()
        => _repo.GetImmimentesAsync();

    public Task<IEnumerable<Ordonnance>> GetByDateRangeAsync(DateTime from, DateTime to)
        => _repo.GetByDateRangeAsync(from, to);

    public Task<Ordonnance> AddOrdonnanceAsync(Ordonnance ordonnance)
        => _repo.AddAsync(ordonnance);

    // FIX : récupère l'ordonnance, met à jour le statut, sauvegarde via UpdateAsync
    public async Task<Ordonnance> UpdateStatutAsync(Guid id, StatutOrdonnance statut)
    {
        var all = await _repo.GetByDateRangeAsync(
            DateTime.UtcNow.AddYears(-10),
            DateTime.UtcNow.AddYears(10));

        var ordonnance = all.FirstOrDefault(o => o.Id == id)
                         ?? throw new InvalidOperationException($"Ordonnance {id} introuvable.");

        ordonnance.Statut = statut;
        return await _repo.UpdateAsync(ordonnance);
    }

    public Task DeleteOrdonnanceAsync(Guid id)
        => _repo.DeleteAsync(id);
}