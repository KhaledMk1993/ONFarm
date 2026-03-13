using ONFarm.Domain.Entities;

namespace ONFarm.Domain.Interfaces;
public interface IOrdonnanceRepository
{
    Task<IEnumerable<Ordonnance>> GetByPatientAsync(Guid patientId);
    Task<IEnumerable<Ordonnance>> GetImmimentesAsync();
    Task<IEnumerable<Ordonnance>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<Ordonnance> AddAsync(Ordonnance ordonnance);
    Task<Ordonnance> UpdateAsync(Ordonnance ordonnance);
    Task DeleteAsync(Guid id);
}
