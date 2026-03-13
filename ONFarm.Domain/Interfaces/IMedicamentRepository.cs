using ONFarm.Domain.Entities;

namespace ONFarm.Domain.Interfaces;
public interface IMedicamentRepository
{
    Task<IEnumerable<Medicament>> GetByPatientAsync(Guid patientId);
    Task<Medicament> AddAsync(Medicament medicament);
    Task<Medicament> UpdateAsync(Medicament medicament);
    Task DeleteAsync(Guid id);
}
