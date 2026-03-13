using ONFarm.Domain.Entities;

namespace ONFarm.Application.Interfaces;

public interface IRappelService
{
    Task<IEnumerable<Rappel>> GetRappelsDuJourAsync();
    Task<IEnumerable<Rappel>> GetRappelsNonVusAsync();
    Task<IEnumerable<Rappel>> GetByPatientAsync(Guid patientId);
    Task<Rappel> CreateRappelAsync(Rappel rappel);
    Task MarquerCommeVuAsync(Guid id);
    Task DeleteRappelAsync(Guid id);
    Task<int> GetCountNonVusAsync();
}