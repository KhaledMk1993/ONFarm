using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ONFarm.Domain.Entities;

namespace ONFarm.ViewModel;

public class MainViewModel : ObservableObject
{
    private ObservableObject _currentView;
    private string _currentPage = "Dashboard";
    private ObservableObject? _previousView;
    private string _previousPage = "Dashboard";

    private readonly PatientFormViewModel _patientFormVM;

    public ObservableObject CurrentView { get => _currentView; set => SetProperty(ref _currentView, value); }
    public string CurrentPage { get => _currentPage; set => SetProperty(ref _currentPage, value); }

    public DashboardViewModel DashboardVM { get; }
    public PatientListViewModel PatientListVM { get; }
    public AgendaViewModel AgendaVM { get; }
    public RappelsViewModel RappelsVM { get; }

    public IRelayCommand NavigateToDashboardCommand { get; }
    public IRelayCommand NavigateToPatientsCommand { get; }
    public IRelayCommand NavigateToAgendaCommand { get; }
    public IRelayCommand NavigateToRappelsCommand { get; }

    public MainViewModel(
        DashboardViewModel dashboardVM,
        PatientListViewModel patientListVM,
        AgendaViewModel agendaVM,
        RappelsViewModel rappelsVM,
        PatientFormViewModel patientFormVM)
    {
        DashboardVM = dashboardVM;
        PatientListVM = patientListVM;
        AgendaVM = agendaVM;
        RappelsVM = rappelsVM;
        _patientFormVM = patientFormVM;
        _currentView = dashboardVM;

        PatientListVM.AddPatientRequested += OnAddPatientRequested;
        PatientListVM.EditPatientRequested += OnEditPatientRequested;
        PatientListVM.OpenDetailRequested += OnOpenPatientRequested;
        _patientFormVM.SaveCompleted += OnFormSaveCompleted;
        _patientFormVM.Cancelled += OnFormCancelled;

        NavigateToDashboardCommand = new RelayCommand(() => Navigate(DashboardVM, "Dashboard"));
        NavigateToPatientsCommand = new RelayCommand(() => Navigate(PatientListVM, "Patients"));
        NavigateToAgendaCommand = new RelayCommand(() => Navigate(AgendaVM, "Agenda"));
        NavigateToRappelsCommand = new RelayCommand(() => Navigate(RappelsVM, "Rappels"));
    }

    private void OnAddPatientRequested()
    {
        _patientFormVM.Nom = string.Empty;
        _patientFormVM.Prenom = string.Empty;
        _patientFormVM.Telephone = string.Empty;
        _patientFormVM.Adresse = string.Empty;
        _patientFormVM.Notes = string.Empty;
        _patientFormVM.DateNaissance = null;
        _patientFormVM.IsEditMode = false;

        _previousView = CurrentView;
        _previousPage = CurrentPage;
        Navigate(_patientFormVM, "Nouveau patient");
    }

    private void OnEditPatientRequested(Patient patient)
    {
        _patientFormVM.LoadPatient(patient);
        _previousView = CurrentView;
        _previousPage = CurrentPage;
        Navigate(_patientFormVM, "Modifier patient");
    }

    private void OnOpenPatientRequested(Patient patient)
    {
        Navigate(PatientListVM, "Patients");
        _ = PatientListVM.LoadAsync();
    }

    private void OnFormSaveCompleted()
    {
        RetourVuePrecedente();
        if (_previousView is DashboardViewModel) _ = DashboardVM.LoadDataAsync();
        else if (_previousView is PatientListViewModel) _ = PatientListVM.LoadAsync();
    }

    private void OnFormCancelled() => RetourVuePrecedente();

    private void RetourVuePrecedente()
    {
        if (_previousView is not null) Navigate(_previousView, _previousPage);
        else Navigate(DashboardVM, "Dashboard");
    }

    private void Navigate(ObservableObject vm, string page)
    {
        CurrentView = vm;
        CurrentPage = page;
    }
}