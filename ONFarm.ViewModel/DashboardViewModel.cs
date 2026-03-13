using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using System.Collections.ObjectModel;

namespace ONFarm.ViewModel;

public class DashboardViewModel : ObservableObject
{
    private readonly IPatientService _patientService;
    private readonly IRappelService _rappelService;
    private readonly IOrdonnanceService _ordonnanceService;

    private int _totalPatients;
    private int _rappelsDuJour;
    private int _ordonnancesImminentes;
    private bool _isLoading;
    private string _activeTab = "Vue d'ensemble";

    // Propriétés bindables
    public int TotalPatients
    {
        get => _totalPatients;
        set => SetProperty(ref _totalPatients, value);
    }

    public int RappelsDuJour
    {
        get => _rappelsDuJour;
        set => SetProperty(ref _rappelsDuJour, value);
    }

    public int OrdonnancesImminentes
    {
        get => _ordonnancesImminentes;
        set => SetProperty(ref _ordonnancesImminentes, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string ActiveTab
    {
        get => _activeTab;
        set => SetProperty(ref _activeTab, value);
    }

    public string DateAujourdhui => DateTime.Now.ToString("dddd d MMMM yyyy");

    // Collections pour binding DataGrid / List
    public ObservableCollection<Patient> Patients { get; } = new();
    public ObservableCollection<Rappel> RappelsUrgents { get; } = new();
    public ObservableCollection<Ordonnance> OrdonnancesAFaire { get; } = new();

    // Commandes
    public IAsyncRelayCommand LoadDataCommand { get; }
    public IRelayCommand<string> SwitchTabCommand { get; }
    public IRelayCommand<Patient> OpenPatientCommand { get; }
    public IRelayCommand AddPatientCommand { get; }

    // Events pour communication avec MainViewModel
    public event Action<Patient>? OpenPatientRequested;
    public event Action? AddPatientRequested;

    // Constructeur principal
    public DashboardViewModel(
        IPatientService patientService,
        IRappelService rappelService,
        IOrdonnanceService ordonnanceService)
    {
        _patientService = patientService;
        _rappelService = rappelService;
        _ordonnanceService = ordonnanceService;

        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
        SwitchTabCommand = new RelayCommand<string>(tab => ActiveTab = tab ?? "Vue d'ensemble");
        OpenPatientCommand = new RelayCommand<Patient>(p => OpenPatientRequested?.Invoke(p!));
        AddPatientCommand = new RelayCommand(() => AddPatientRequested?.Invoke());
    }

    // Constructeur vide pour DesignInstance XAML
    public DashboardViewModel() { }

    // Chargement des données
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var patients = await _patientService.GetAllPatientsAsync();
            var rappels = await _rappelService.GetRappelsDuJourAsync();
            var ordonnances = await _ordonnanceService.GetOrdonnancesImmimentesAsync();

            Patients.Clear();
            foreach (var p in patients) Patients.Add(p);

            RappelsUrgents.Clear();
            foreach (var r in rappels) RappelsUrgents.Add(r);

            OrdonnancesAFaire.Clear();
            foreach (var o in ordonnances) OrdonnancesAFaire.Add(o);

            TotalPatients = Patients.Count;
            RappelsDuJour = RappelsUrgents.Count;
            OrdonnancesImminentes = OrdonnancesAFaire.Count;
        }
        finally
        {
            IsLoading = false;
        }
    }
}