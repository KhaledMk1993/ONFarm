using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Enums;
using ONFarm.Domain.Interfaces;

namespace ONFarm.Application.Services;

public class OrdonnanceService : IOrdonnanceService
{
    private readonly IOrdonnanceRepository _repo;

    public OrdonnanceService(IOrdonnanceRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Ordonnance>> GetByPatientAsync(Guid patientId) => _repo.GetByPatientAsync(patientId);
    public Task<IEnumerable<Ordonnance>> GetOrdonnancesImmimentesAsync() => _repo.GetImmimentesAsync();
    public Task<IEnumerable<Ordonnance>> GetByDateRangeAsync(DateTime from, DateTime to) => _repo.GetByDateRangeAsync(from, to);
    public Task<Ordonnance> AddOrdonnanceAsync(Ordonnance ordonnance) => _repo.AddAsync(ordonnance);

    public async Task<Ordonnance> UpdateStatutAsync(Guid id, StatutOrdonnance statut)
    {
        var ordonnances = await _repo.GetByPatientAsync(Guid.Empty); // à remplacer selon logique
        throw new NotImplementedException("Use UpdateAsync directly on repository");
    }

    public Task DeleteOrdonnanceAsync(Guid id) => _repo.DeleteAsync(id);
}