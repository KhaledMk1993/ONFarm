using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using ONFarm.Domain.Enums;
using System.Collections.ObjectModel;

namespace ONFarm.ViewModel;

public class OrdonnanceViewModel : ObservableObject
{
    // ──────────────────────────────────────────────
    // Services
    // ──────────────────────────────────────────────
    private readonly IPatientService _patientService;
    private readonly IOrdonnanceService _ordonnanceService;   // ← AJOUTÉ

    // ──────────────────────────────────────────────
    // Collections
    // ──────────────────────────────────────────────
    public ObservableCollection<Ordonnance> Ordonnances { get; } = new();
    public ObservableCollection<Ordonnance> FilteredOrdonnances { get; } = new();
    public ObservableCollection<Patient> Patients { get; } = new();
    public ObservableCollection<Patient> FilteredPatients { get; } = new();

    // ──────────────────────────────────────────────
    // État du formulaire
    // ──────────────────────────────────────────────
    private bool _isEditing = false;
    private Guid? _editingId = null;

    private bool _isPanelVisible = false;
    public bool IsPanelVisible
    {
        get => _isPanelVisible;
        set => SetProperty(ref _isPanelVisible, value);
    }

    // ──────────────────────────────────────────────
    // Ordonnance sélectionnée
    // ──────────────────────────────────────────────
    private Ordonnance? _selectedOrdonnance;
    public Ordonnance? SelectedOrdonnance
    {
        get => _selectedOrdonnance;
        set
        {
            if (SetProperty(ref _selectedOrdonnance, value) && value is not null)
                LoadOrdonnanceIntoForm(value);
        }
    }

    // ──────────────────────────────────────────────
    // Champs du formulaire
    // ──────────────────────────────────────────────
    private Patient? _selectedPatient;
    public Patient? SelectedPatient
    {
        get => _selectedPatient;
        set => SetProperty(ref _selectedPatient, value);
    }

    private string _searchPatient = string.Empty;
    public string SearchPatient
    {
        get => _searchPatient;
        set
        {
            if (SetProperty(ref _searchPatient, value))
            {
                FilterPatients();
                OnPropertyChanged(nameof(HasFilteredPatients));
            }
        }
    }

    public bool HasFilteredPatients =>
        !string.IsNullOrWhiteSpace(SearchPatient) && FilteredPatients.Count > 0 && SelectedPatient is null;

    private DateTime _dateFacture = DateTime.Today;
    public DateTime DateFacture
    {
        get => _dateFacture;
        set => SetProperty(ref _dateFacture, value);
    }

    private DateTime _prochainRenouvellement = DateTime.Today.AddDays(30);
    public DateTime ProchainRenouvellement
    {
        get => _prochainRenouvellement;
        set => SetProperty(ref _prochainRenouvellement, value);
    }

    // ── SelectedStatut : string lisible ──
    private string _selectedStatut = "En attente";
    public string SelectedStatut
    {
        get => _selectedStatut;
        set => SetProperty(ref _selectedStatut, value);
    }

    private string _notes = string.Empty;
    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    // ──────────────────────────────────────────────
    // Options statut formulaire
    // ──────────────────────────────────────────────
    public IReadOnlyList<string> StatutOptions { get; } = new[]
    {
        "En attente",
        "Préparé",
        "Bientôt",
        "Livré"
    };

    // ──────────────────────────────────────────────
    // Options filtre (avec "Tous les statuts" en tête)
    // ──────────────────────────────────────────────
    public IReadOnlyList<string> FilterOptions { get; } = new[]
    {
        "Tous les statuts",
        "En attente",
        "Préparé",
        "Bientôt",
        "Livré"
    };

    // ──────────────────────────────────────────────
    // Filtre sélectionné — initialisé sur "Tous les statuts"
    // ──────────────────────────────────────────────
    private string _filterType = "Tous les statuts";
    public string FilterType
    {
        get => _filterType;
        set
        {
            if (SetProperty(ref _filterType, value))
                ApplyFilter();
        }
    }

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
                ApplyFilter();
        }
    }

    // ──────────────────────────────────────────────
    // KPI
    // ──────────────────────────────────────────────
    public int TotalCount => FilteredOrdonnances.Count;
    public int OrdonnancesImminentes => Ordonnances.Count(o => o.Statut == StatutOrdonnance.Bientôt);
    public int OrdonnancesEnRetard => Ordonnances.Count(o => o.EstEnRetard);

    // ──────────────────────────────────────────────
    // Titre panneau
    // ──────────────────────────────────────────────
    private string _panelTitle = "Nouvelle ordonnance";
    public string PanelTitle
    {
        get => _panelTitle;
        set => SetProperty(ref _panelTitle, value);
    }

    // ──────────────────────────────────────────────
    // Helpers : string ↔ enum  (casse exacte !)
    // ──────────────────────────────────────────────
    private static StatutOrdonnance StatutFromString(string s) => s switch
    {
        "Préparé" => StatutOrdonnance.Prepare,
        "Bientôt" => StatutOrdonnance.Bientôt,
        "Livré" => StatutOrdonnance.Livre,      // ← majuscule corrigée
        _ => StatutOrdonnance.EnAttente
    };

    private static string StatutToString(StatutOrdonnance s) => s switch
    {
        StatutOrdonnance.Prepare => "Préparé",
        StatutOrdonnance.Bientôt => "Bientôt",
        StatutOrdonnance.Livre => "Livré",
        _ => "En attente"
    };

    // ──────────────────────────────────────────────
    // Commandes
    // ──────────────────────────────────────────────
    public IRelayCommand OpenNewPanelCommand { get; }
    public IRelayCommand SaveOrdonnanceCommand { get; }
    public IRelayCommand CancelCommand { get; }
    public IRelayCommand<Ordonnance> EditOrdonnanceCommand { get; }
    public IRelayCommand<Ordonnance> DeleteOrdonnanceCommand { get; }
    public IRelayCommand<Patient> SelectPatientCommand { get; }
    public IRelayCommand ClearPatientCommand { get; }

    // ──────────────────────────────────────────────
    // Constructeur — reçoit IOrdonnanceService
    // ──────────────────────────────────────────────
    public OrdonnanceViewModel(IPatientService patientService, IOrdonnanceService ordonnanceService)
    {
        _patientService = patientService;
        _ordonnanceService = ordonnanceService;   // ← INJECTÉ

        OpenNewPanelCommand = new RelayCommand(OpenNewPanel);
        SaveOrdonnanceCommand = new RelayCommand(async () => await SaveOrdonnanceAsync(), CanSave);
        CancelCommand = new RelayCommand(Cancel);
        EditOrdonnanceCommand = new RelayCommand<Ordonnance>(EditOrdonnance);
        DeleteOrdonnanceCommand = new RelayCommand<Ordonnance>(async o => await DeleteOrdonnanceAsync(o));
        SelectPatientCommand = new RelayCommand<Patient>(SelectPatient);
        ClearPatientCommand = new RelayCommand(ClearPatient);

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(SelectedPatient))
                ((RelayCommand)SaveOrdonnanceCommand).NotifyCanExecuteChanged();
        };

        _ = LoadDataAsync();
    }

    // ──────────────────────────────────────────────
    // Chargement initial (patients + ordonnances BD)
    // ──────────────────────────────────────────────
    private async Task LoadDataAsync()
    {
        // Patients
        var patients = await _patientService.GetAllPatientsAsync();
        Patients.Clear();
        foreach (var p in patients) Patients.Add(p);
        FilterPatients();

        // Ordonnances depuis la BD
        await LoadOrdonnancesFromDbAsync();
    }

    private async Task LoadOrdonnancesFromDbAsync()
    {
        var from = DateTime.Today.AddYears(-2);
        var to = DateTime.Today.AddYears(2);
        var list = await _ordonnanceService.GetByDateRangeAsync(from, to);

        Ordonnances.Clear();
        foreach (var o in list)
        {
            // Rattacher l'objet Patient si disponible
            if (o.Patient is null)
                o.Patient = Patients.FirstOrDefault(p => p.Id == o.PatientId);
            Ordonnances.Add(o);
        }
        ApplyFilter();
    }

    // ──────────────────────────────────────────────
    // Ouvrir panneau nouvelle ordonnance
    // ──────────────────────────────────────────────
    private void OpenNewPanel()
    {
        _isEditing = false;
        _editingId = null;
        PanelTitle = "Nouvelle ordonnance";
        ResetForm();
        IsPanelVisible = true;
    }

    // ──────────────────────────────────────────────
    // Charger une ordonnance dans le formulaire
    // ──────────────────────────────────────────────
    private void LoadOrdonnanceIntoForm(Ordonnance o)
    {
        _isEditing = true;
        _editingId = o.Id;
        PanelTitle = "Modifier l'ordonnance";

        SelectedPatient = Patients.FirstOrDefault(p => p.Id == o.PatientId);
        DateFacture = o.DateFacture;
        ProchainRenouvellement = o.ProchainRenouvellement;
        SelectedStatut = StatutToString(o.Statut);   // ← converti correctement
        Notes = o.Notes ?? string.Empty;

        IsPanelVisible = true;
        ((RelayCommand)SaveOrdonnanceCommand).NotifyCanExecuteChanged();
    }

    // ──────────────────────────────────────────────
    // Modifier
    // ──────────────────────────────────────────────
    private void EditOrdonnance(Ordonnance? o)
    {
        if (o is null) return;
        SelectedOrdonnance = o;
    }

    // ──────────────────────────────────────────────
    // Enregistrer → BD via service
    // ──────────────────────────────────────────────
    private bool CanSave() => SelectedPatient is not null;

    private async Task SaveOrdonnanceAsync()
    {
        if (SelectedPatient is null) return;

        var statut = StatutFromString(SelectedStatut);   // ← conversion correcte

        if (_isEditing && _editingId is not null)
        {
            // Mise à jour statut via service
            await _ordonnanceService.UpdateStatutAsync(_editingId.Value, statut);

            // Mettre à jour l'objet en mémoire
            var existing = Ordonnances.FirstOrDefault(o => o.Id == _editingId);
            if (existing is not null)
            {
                existing.PatientId = SelectedPatient.Id;
                existing.Patient = SelectedPatient;
                existing.DateFacture = DateFacture;
                existing.ProchainRenouvellement = ProchainRenouvellement;
                existing.Statut = statut;
                existing.Notes = Notes;
            }
        }
        else
        {
            // Ajout via service → enregistrement BD
            var nouvelle = new Ordonnance
            {
                PatientId = SelectedPatient.Id,
                Patient = SelectedPatient,
                DateFacture = DateFacture,
                ProchainRenouvellement = ProchainRenouvellement,
                Statut = statut,
                Notes = Notes
            };

            var saved = await _ordonnanceService.AddOrdonnanceAsync(nouvelle);
            Ordonnances.Add(saved);
        }

        ApplyFilter();
        Cancel();
    }

    // ──────────────────────────────────────────────
    // Supprimer → BD via service
    // ──────────────────────────────────────────────
    private async Task DeleteOrdonnanceAsync(Ordonnance? o)
    {
        if (o is null) return;

        await _ordonnanceService.DeleteOrdonnanceAsync(o.Id);   // ← BD
        Ordonnances.Remove(o);

        if (_editingId == o.Id) Cancel();
        ApplyFilter();
    }

    // ──────────────────────────────────────────────
    // Annuler
    // ──────────────────────────────────────────────
    private void Cancel()
    {
        IsPanelVisible = false;
        _isEditing = false;
        _editingId = null;
        _selectedOrdonnance = null;
        OnPropertyChanged(nameof(SelectedOrdonnance));
        ResetForm();
    }

    // ──────────────────────────────────────────────
    // Reset formulaire — toujours "En attente" par défaut
    // ──────────────────────────────────────────────
    private void ResetForm()
    {
        SelectedPatient = null;
        SearchPatient = string.Empty;
        DateFacture = DateTime.Today;
        ProchainRenouvellement = DateTime.Today.AddDays(30);
        SelectedStatut = "En attente";
        Notes = string.Empty;
        PanelTitle = "Nouvelle ordonnance";
        FilterPatients();
    }

    // ──────────────────────────────────────────────
    // Filtre liste — aligné avec FilterOptions
    // ──────────────────────────────────────────────
    private void ApplyFilter()
    {
        FilteredOrdonnances.Clear();

        IEnumerable<Ordonnance> source = FilterType switch
        {
            "En attente" => Ordonnances.Where(o => o.Statut == StatutOrdonnance.EnAttente),
            "Préparé" => Ordonnances.Where(o => o.Statut == StatutOrdonnance.Prepare),
            "Bientôt" => Ordonnances.Where(o => o.EstImminente),
            "Livré" => Ordonnances.Where(o => o.Statut == StatutOrdonnance.Livre),
            _ => Ordonnances   // "Tous les statuts"
        };

        if (!string.IsNullOrWhiteSpace(SearchQuery))
            source = source.Where(o =>
                o.Patient?.Nom.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true ||
                o.Patient?.Prenom.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) == true);

        foreach (var o in source) FilteredOrdonnances.Add(o);

        OnPropertyChanged(nameof(OrdonnancesImminentes));
        OnPropertyChanged(nameof(OrdonnancesEnRetard));
        OnPropertyChanged(nameof(TotalCount));
    }

    // ──────────────────────────────────────────────
    // Filtre patients (autocomplete)
    // ──────────────────────────────────────────────
    private void FilterPatients()
    {
        FilteredPatients.Clear();

        var result = string.IsNullOrWhiteSpace(SearchPatient)
            ? Patients
            : (IEnumerable<Patient>)Patients.Where(p =>
                p.Nom.Contains(SearchPatient, StringComparison.OrdinalIgnoreCase) ||
                p.Prenom.Contains(SearchPatient, StringComparison.OrdinalIgnoreCase));

        foreach (var p in result) FilteredPatients.Add(p);

        OnPropertyChanged(nameof(HasFilteredPatients));
    }

    // ──────────────────────────────────────────────
    // Sélection / effacement patient
    // ──────────────────────────────────────────────
    private void SelectPatient(Patient? patient)
    {
        if (patient is null) return;
        SelectedPatient = patient;
        _searchPatient = string.Empty;
        OnPropertyChanged(nameof(SearchPatient));
        FilteredPatients.Clear();
        OnPropertyChanged(nameof(HasFilteredPatients));
    }

    private void ClearPatient()
    {
        SelectedPatient = null;
        SearchPatient = string.Empty;
    }
}