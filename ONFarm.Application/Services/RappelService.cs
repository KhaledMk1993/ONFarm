using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Interfaces;

namespace ONFarm.Application.Services;

public class RappelService : IRappelService
{
    private readonly IRappelRepository _repo;

    public RappelService(IRappelRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Rappel>> GetRappelsDuJourAsync() => _repo.GetDuJourAsync();
    public Task<IEnumerable<Rappel>> GetRappelsNonVusAsync() => _repo.GetNonVusAsync();
    public Task<IEnumerable<Rappel>> GetByPatientAsync(Guid patientId) => _repo.GetByPatientAsync(patientId);
    public Task<Rappel> CreateRappelAsync(Rappel rappel) => _repo.AddAsync(rappel);

    public async Task MarquerCommeVuAsync(Guid id)
    {
        var rappels = await _repo.GetNonVusAsync();
        var rappel = rappels.FirstOrDefault(r => r.Id == id);
        if (rappel != null)
        {
            rappel.EstVu = true;
            await _repo.UpdateAsync(rappel);
        }
    }

    public Task DeleteRappelAsync(Guid id) => _repo.DeleteAsync(id);

    public async Task<int> GetCountNonVusAsync()
    {
        var all = await _repo.GetNonVusAsync();
        return all.Count();
    }
}