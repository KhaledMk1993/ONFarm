using ClosedXML.Excel;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Interfaces;
using System.Globalization;

namespace ONFarm.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repo;

    public PatientService(IPatientRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Patient>> GetAllPatientsAsync() => _repo.GetAllAsync();

    public Task<Patient?> GetPatientByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public Task<IEnumerable<Patient>> SearchPatientsAsync(string query) => _repo.SearchAsync(query);

    public Task<Patient> AddPatientAsync(Patient patient) => _repo.AddAsync(patient);

    public Task<Patient> UpdatePatientAsync(Patient patient) => _repo.UpdateAsync(patient);

    public Task DeletePatientAsync(Guid id) => _repo.DeleteAsync(id);

}