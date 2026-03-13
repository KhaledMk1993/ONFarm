using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Interfaces;

namespace ONFarm.Application.Services;

public class MedicamentService : IMedicamentService
{
    private readonly IMedicamentRepository _repo;

    public MedicamentService(IMedicamentRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Medicament>> GetByPatientAsync(Guid patientId) => _repo.GetByPatientAsync(patientId);
    public Task<Medicament> AddMedicamentAsync(Medicament medicament) => _repo.AddAsync(medicament);
    public Task<Medicament> UpdateMedicamentAsync(Medicament medicament) => _repo.UpdateAsync(medicament);
    public Task DeleteMedicamentAsync(Guid id) => _repo.DeleteAsync(id);
}