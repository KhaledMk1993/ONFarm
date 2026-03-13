using ONFarm.Domain.Entities;

namespace ONFarm.Application.Interfaces;

public interface IMedicamentService
{
    Task<IEnumerable<Medicament>> GetByPatientAsync(Guid patientId);
    Task<Medicament> AddMedicamentAsync(Medicament medicament);
    Task<Medicament> UpdateMedicamentAsync(Medicament medicament);
    Task DeleteMedicamentAsync(Guid id);
}