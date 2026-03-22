using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;

namespace ONFarm.ViewModel;

public class PatientListViewModel : ObservableObject
{
    private readonly IExcelService _excelService;
    private readonly IPatientService _patientService;
    private List<Patient> _allPatients = [];
    private DateTime? _filterDateDebut;
    private DateTime? _filterDateFin;
    private bool _isLoading;
    private bool _isUrgentFilterActive;
    private ObservableCollection<Patient> _patients = [];
    private string _searchQuery = string.Empty;


    public PatientListViewModel(IPatientService patientService, IExcelService excelService)
    {
        _patientService = patientService;
        _excelService = excelService;

        AddPatientCommand = new RelayCommand(() => AddPatientRequested?.Invoke());
        EditPatientCommand = new RelayCommand<Patient>(p => EditPatientRequested?.Invoke(p!));
        DeletePatientCommand = new AsyncRelayCommand<Patient>(DeletePatientAsync);
        OpenDetailCommand = new RelayCommand<Patient>(p =>
        {
            if (p is not null) OpenDetailRequested?.Invoke(p);
        });

        ImportExcelCommand = new AsyncRelayCommand(ImportExcelAsync);
        ExportExcelCommand = new AsyncRelayCommand(ExportExcelAsync);

        ResetDateFilterCommand = new RelayCommand(() =>
        {
            _filterDateDebut = null;
            _filterDateFin = null;
            OnPropertyChanged(nameof(FilterDateDebut));
            OnPropertyChanged(nameof(FilterDateFin));
            OnPropertyChanged(nameof(IsDateFilterActive));
            ApplyFilters();
        });

        ToggleUrgentFilterCommand = new RelayCommand(() => { IsUrgentFilterActive = !IsUrgentFilterActive; });

        LoadAsync().ConfigureAwait(false);
    }

    public ObservableCollection<Patient> Patients
    {
        get => _patients;
        private set => SetProperty(ref _patients, value);
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
                _ = SearchAsync();
        }
    }

    public DateTime? FilterDateDebut
    {
        get => _filterDateDebut;
        set
        {
            if (SetProperty(ref _filterDateDebut, value))
            {
                OnPropertyChanged(nameof(IsDateFilterActive));
                ApplyFilters();
            }
        }
    }

    public DateTime? FilterDateFin
    {
        get => _filterDateFin;
        set
        {
            if (SetProperty(ref _filterDateFin, value))
            {
                OnPropertyChanged(nameof(IsDateFilterActive));
                ApplyFilters();
            }
        }
    }

    public bool IsUrgentFilterActive
    {
        get => _isUrgentFilterActive;
        set
        {
            if (SetProperty(ref _isUrgentFilterActive, value))
                ApplyFilters();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool IsDateFilterActive => FilterDateDebut.HasValue || FilterDateFin.HasValue;

    public IRelayCommand AddPatientCommand { get; }
    public IRelayCommand<Patient> EditPatientCommand { get; }
    public IAsyncRelayCommand<Patient> DeletePatientCommand { get; }
    public IRelayCommand<Patient> OpenDetailCommand { get; }
    public IRelayCommand ToggleUrgentFilterCommand { get; }
    public IAsyncRelayCommand ImportExcelCommand { get; }
    public IAsyncRelayCommand ExportExcelCommand { get; }
    public IRelayCommand ResetDateFilterCommand { get; }

    public event Action? AddPatientRequested;
    public event Action<Patient>? EditPatientRequested;
    public event Action<Patient>? OpenDetailRequested;

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var list = await _patientService.GetAllPatientsAsync();
            _allPatients = list.ToList();
            ApplyFilters();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            await LoadAsync();
            return;
        }

        IsLoading = true;
        try
        {
            var results = await _patientService.SearchPatientsAsync(SearchQuery);
            _allPatients = results.ToList();
            ApplyFilters();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilters()
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

        if (IsUrgentFilterActive)
            filtered = filtered.Where(p => p.EstUrgentAujourdhui);

        filtered = filtered.OrderBy(p => p.StatutSuivi switch
        {
            "Rouge" => 0,
            "Orange" => 1,
            "Vert" => 2,
            _ => 3
        });

        Patients = new ObservableCollection<Patient>(filtered);
    }

    private void AddPatientToList(Patient patient)
    {
        _allPatients.Add(patient);
        ApplyFilters();
    }

    private async Task DeletePatientAsync(Patient? p)
    {
        if (p is null) return;
        await _patientService.DeletePatientAsync(p.Id);
        _allPatients.Remove(p);
        ApplyFilters();
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
            foreach (var patient in imported)
            {
                await _patientService.AddPatientAsync(patient);
                AddPatientToList(patient);
            }
        }
        finally
        {
            IsLoading = false;
        }
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
        finally
        {
            IsLoading = false;
        }
    }
}