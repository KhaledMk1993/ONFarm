using ONFarm.Domain.Entities;
using ONFarm.Domain.Enums;

namespace ONFarm.Application.Interfaces;

public interface IOrdonnanceService
{
    Task<IEnumerable<Ordonnance>> GetByPatientAsync(Guid patientId);
    Task<IEnumerable<Ordonnance>> GetOrdonnancesImmimentesAsync();
    Task<IEnumerable<Ordonnance>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Ordonnance> AddOrdonnanceAsync(Ordonnance ordonnance);
    Task<Ordonnance> UpdateStatutAsync(Guid id, StatutOrdonnance statut);
    Task DeleteOrdonnanceAsync(Guid id);
}