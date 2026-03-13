using ONFarm.Domain.Entities;

namespace ONFarm.Domain.Interfaces;
public interface IRappelRepository
{
    Task<IEnumerable<Rappel>> GetDuJourAsync();
    Task<IEnumerable<Rappel>> GetByPatientAsync(Guid patientId);
    Task<IEnumerable<Rappel>> GetNonVusAsync();
    Task<Rappel> AddAsync(Rappel rappel);
    Task<Rappel> UpdateAsync(Rappel rappel);
    Task DeleteAsync(Guid id);
}
