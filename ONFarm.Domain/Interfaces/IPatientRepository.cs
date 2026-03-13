using ONFarm.Domain.Entities;

namespace ONFarm.Domain.Interfaces;
public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(Guid id);
    Task<IEnumerable<Patient>> SearchAsync(string query);
    Task<Patient> AddAsync(Patient patient);
    Task<Patient> UpdateAsync(Patient patient);
    Task DeleteAsync(Guid id);
    Task AddRangeAsync(IEnumerable<Patient> patients);
}
