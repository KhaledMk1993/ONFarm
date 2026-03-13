using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using System.Collections.ObjectModel;

namespace ONFarm.ViewModel;

public class PatientDetailViewModel : ObservableObject
{
    private readonly IPatientService _patientService;
    private readonly IOrdonnanceService _ordonnanceService;
    private readonly IMedicamentService _medicamentService;
    private readonly IRappelService _rappelService;

    private Patient? _patient;
    public Patient? Patient { get => _patient; set { if (SetProperty(ref _patient, value)) EditCommand.NotifyCanExecuteChanged(); } }

    public ObservableCollection<Ordonnance> Ordonnances { get; } = new();
    public ObservableCollection<Medicament> Medicaments { get; } = new();
    public ObservableCollection<Rappel> Rappels { get; } = new();

    public IAsyncRelayCommand<Guid> LoadCommand { get; }
    public IRelayCommand EditCommand { get; }
    public IAsyncRelayCommand AddOrdonnanceCommand { get; }
    public IAsyncRelayCommand AddMedicamentCommand { get; }
    public IAsyncRelayCommand AddRappelCommand { get; }

    public event Action<Patient>? EditRequested;

    public PatientDetailViewModel(
        IPatientService ps,
        IOrdonnanceService os,
        IMedicamentService ms,
        IRappelService rs)
    {
        _patientService = ps;
        _ordonnanceService = os;
        _medicamentService = ms;
        _rappelService = rs;

        LoadCommand = new AsyncRelayCommand<Guid>(LoadAsync);
        EditCommand = new RelayCommand(() => EditRequested?.Invoke(Patient!), () => Patient is not null);
        AddOrdonnanceCommand = new AsyncRelayCommand(AddOrdonnanceAsync);
        AddMedicamentCommand = new AsyncRelayCommand(AddMedicamentAsync);
        AddRappelCommand = new AsyncRelayCommand(AddRappelAsync);
    }

    public async Task LoadAsync(Guid patientId)
    {
        Patient = await _patientService.GetPatientByIdAsync(patientId);
        if (Patient is null) return;

        Ordonnances.Clear();
        foreach (var o in await _ordonnanceService.GetByPatientAsync(patientId)) Ordonnances.Add(o);

        Medicaments.Clear();
        foreach (var m in await _medicamentService.GetByPatientAsync(patientId)) Medicaments.Add(m);

        Rappels.Clear();
        foreach (var r in await _rappelService.GetByPatientAsync(patientId)) Rappels.Add(r);
    }

    private async Task AddOrdonnanceAsync()
    {
        if (Patient is null) return;
        var ordonnance = new Ordonnance { PatientId = Patient.Id, DateFacture = DateTime.Today, ProchainRenouvellement = DateTime.Today.AddMonths(1) };
        await _ordonnanceService.AddOrdonnanceAsync(ordonnance);
        await LoadAsync(Patient.Id);
    }

    private async Task AddMedicamentAsync() => await Task.CompletedTask; // Dialog handled in View
    private async Task AddRappelAsync()
    {
        if (Patient is null) return;
        var rappel = new Rappel { PatientId = Patient.Id, DateRappel = DateTime.Today.AddDays(1), Message = $"Rappel pour {Patient.NomComplet}" };
        await _rappelService.CreateRappelAsync(rappel);
        await LoadAsync(Patient.Id);
    }
}