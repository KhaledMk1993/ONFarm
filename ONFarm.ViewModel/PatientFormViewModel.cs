using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;

namespace ONFarm.ViewModel;

public class PatientFormViewModel : ObservableObject
{
    private readonly IPatientService _patientService;

    private Guid _id;
    private string _nom = string.Empty;
    private string _prenom = string.Empty;
    private string _telephone = string.Empty;
    private string _adresse = string.Empty;
    private string _notes = string.Empty;
    private DateOnly? _dateNaissance;
    private bool _isEditMode;

    public Guid Id { get => _id; set => SetProperty(ref _id, value); }
    public string Nom { get => _nom; set { if (SetProperty(ref _nom, value)) SaveCommand.NotifyCanExecuteChanged(); } }
    public string Prenom { get => _prenom; set { if (SetProperty(ref _prenom, value)) SaveCommand.NotifyCanExecuteChanged(); } }
    public string Telephone { get => _telephone; set => SetProperty(ref _telephone, value); }
    public string Adresse { get => _adresse; set => SetProperty(ref _adresse, value); }
    public string Notes { get => _notes; set => SetProperty(ref _notes, value); }
    public DateOnly? DateNaissance { get => _dateNaissance; set => SetProperty(ref _dateNaissance, value); }

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (SetProperty(ref _isEditMode, value))
                OnPropertyChanged(nameof(Titre));
        }
    }

    public string Titre => IsEditMode ? "Modifier le patient" : "Nouveau patient";

    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }

    public event Action? SaveCompleted;
    public event Action? Cancelled;

    public PatientFormViewModel(IPatientService patientService)
    {
        _patientService = patientService;
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        CancelCommand = new RelayCommand(() => Cancelled?.Invoke());
    }

    public void LoadPatient(Patient patient)
    {
        Id = patient.Id;
        Nom = patient.Nom;
        Prenom = patient.Prenom;
        Telephone = patient.Telephone ?? string.Empty;
        Adresse = patient.Adresse ?? string.Empty;
        Notes = patient.Notes ?? string.Empty;
        DateNaissance = patient.DateNaissance;
        IsEditMode = true;
    }

    private bool CanSave() =>
        !string.IsNullOrWhiteSpace(Nom) && !string.IsNullOrWhiteSpace(Prenom);

    private async Task SaveAsync()
    {
        var patient = new Patient
        {
            Id = IsEditMode ? Id : Guid.NewGuid(),
            Nom = Nom.Trim(),
            Prenom = Prenom.Trim(),
            Telephone = Telephone.Trim(),
            Adresse = Adresse.Trim(),
            Notes = Notes.Trim(),
        };

        patient.SetDateNaissance(DateNaissance);

        if (IsEditMode) await _patientService.UpdatePatientAsync(patient);
        else await _patientService.AddPatientAsync(patient);

        SaveCompleted?.Invoke();
    }
}