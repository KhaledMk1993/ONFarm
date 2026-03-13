using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using System.Collections.ObjectModel;

namespace ONFarm.ViewModel;

public class PatientListViewModel : ObservableObject
{
    private readonly IPatientService _patientService;
    private readonly IExcelService _excelService;

    // Liste complète non filtrée
    private List<Patient> _allPatients = new();

    // Liste affichée (filtrée)
    private ObservableCollection<Patient> _patients = new();
    public ObservableCollection<Patient> Patients
    {
        get => _patients;
        private set => SetProperty(ref _patients, value);
    }

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
                _ = SearchAsync();
        }
    }

    // Filtre date début
    private DateTime? _filterDateDebut;
    public DateTime? FilterDateDebut
    {
        get => _filterDateDebut;
        set
        {
            if (SetProperty(ref _filterDateDebut, value))
            {
                OnPropertyChanged(nameof(IsDateFilterActive));
                ApplyDateFilter();
            }
        }
    }

    // Filtre date fin
    private DateTime? _filterDateFin;
    public DateTime? FilterDateFin
    {
        get => _filterDateFin;
        set
        {
            if (SetProperty(ref _filterDateFin, value))
            {
                OnPropertyChanged(nameof(IsDateFilterActive));
                ApplyDateFilter();
            }
        }
    }

    // Indicateur badge "Filtre actif"
    public bool IsDateFilterActive => FilterDateDebut.HasValue || FilterDateFin.HasValue;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public IRelayCommand AddPatientCommand { get; }
    public IRelayCommand<Patient> EditPatientCommand { get; }
    public IAsyncRelayCommand<Patient> DeletePatientCommand { get; }
    public IAsyncRelayCommand ImportExcelCommand { get; }
    public IAsyncRelayCommand ExportExcelCommand { get; }
    public IRelayCommand ResetDateFilterCommand { get; }

    public event Action? AddPatientRequested;
    public event Action<Patient>? EditPatientRequested;
    public event Action<Patient>? OpenDetailRequested;

    public PatientListViewModel(IPatientService patientService, IExcelService excelService)
    {
        _patientService = patientService;
        _excelService = excelService;

        AddPatientCommand = new RelayCommand(() => AddPatientRequested?.Invoke());
        EditPatientCommand = new RelayCommand<Patient>(p => EditPatientRequested?.Invoke(p!));
        DeletePatientCommand = new AsyncRelayCommand<Patient>(DeletePatientAsync);
        ImportExcelCommand = new AsyncRelayCommand(ImportExcelAsync);
        ExportExcelCommand = new AsyncRelayCommand(ExportExcelAsync);
        ResetDateFilterCommand = new RelayCommand(() =>
        {
            _filterDateDebut = null;
            _filterDateFin = null;
            OnPropertyChanged(nameof(FilterDateDebut));
            OnPropertyChanged(nameof(FilterDateFin));
            OnPropertyChanged(nameof(IsDateFilterActive));
            ApplyDateFilter();
        });

        LoadAsync().ConfigureAwait(false);
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _patientService.GetAllPatientsAsync();
            _allPatients = list.ToList();
            ApplyDateFilter();
        }
        finally { IsLoading = false; }
    }

    public async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) { await LoadAsync(); return; }

        IsLoading = true;
        try
        {
            var results = await _patientService.SearchPatientsAsync(SearchQuery);
            _allPatients = results.ToList();
            ApplyDateFilter();
        }
        finally { IsLoading = false; }
    }

    // Applique le filtre date sur _allPatients
    private void ApplyDateFilter()
    {
        var filtered = _allPatients.AsEnumerable();

        if (FilterDateDebut.HasValue)
        {
            var debut = DateOnly.FromDateTime(FilterDateDebut.Value);
            filtered = filtered.Where(p => p.DateNaissance.HasValue && p.DateNaissance.Value >= debut);
        }

        if (FilterDateFin.HasValue)
        {
            var fin = DateOnly.FromDateTime(FilterDateFin.Value);
            filtered = filtered.Where(p => p.DateNaissance.HasValue && p.DateNaissance.Value <= fin);
        }

        Patients = new ObservableCollection<Patient>(filtered);
    }

    public void AddPatientToList(Patient patient)
    {
        _allPatients.Add(patient);
        ApplyDateFilter();
    }

    private async Task DeletePatientAsync(Patient? p)
    {
        if (p == null) return;
        await _patientService.DeletePatientAsync(p.Id);
        _allPatients.Remove(p);
        ApplyDateFilter();
    }

    private async Task ImportExcelAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Importer des patients depuis Excel",
            Filter = "Fichiers Excel (*.xlsx;*.xls)|*.xlsx;*.xls",
            Multiselect = false
        };

        if (dialog.ShowDialog() != true) return;

        IsLoading = true;
        try
        {
            var imported = await _excelService.ImporterPatientsAsync(dialog.FileName);
            foreach (var p in imported)
            {
                await _patientService.AddPatientAsync(p);
                AddPatientToList(p);
            }
        }
        finally { IsLoading = false; }
    }

    private async Task ExportExcelAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Exporter les patients",
            Filter = "Fichiers Excel (*.xlsx)|*.xlsx",
            FileName = $"Patients_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
            DefaultExt = ".xlsx"
        };

        if (dialog.ShowDialog() != true) return;

        IsLoading = true;
        try
        {
            await _excelService.ExporterPatientsAsync(Patients, dialog.FileName);
        }
        finally { IsLoading = false; }
    }
}
