using ONFarm.Domain.Entities;

namespace ONFarm.Application.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<Patient>> GetAllPatientsAsync();
    Task<Patient?> GetPatientByIdAsync(Guid id);
    Task<IEnumerable<Patient>> SearchPatientsAsync(string query);
    Task<Patient> AddPatientAsync(Patient patient);
    Task<Patient> UpdatePatientAsync(Patient patient);
    Task DeletePatientAsync(Guid id);
}