using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using System.Collections.ObjectModel;

namespace ONFarm.ViewModel;

public class AgendaViewModel : ObservableObject
{
    private readonly IOrdonnanceService _ordonnanceService;
    private readonly IRappelService _rappelService;
    private DateTime _semaineDebut;

    public DateTime SemaineDebut
    {
        get => _semaineDebut;
        set
        {
            if (SetProperty(ref _semaineDebut, value))
                OnPropertyChanged(nameof(TitreSemaine));
        }
    }

    public string TitreSemaine =>
        $"Semaine du {_semaineDebut:dd MMMM} au {_semaineDebut.AddDays(6):dd MMMM yyyy}";

    public ObservableCollection<JourCalendrier> Jours { get; } = new();

    public IAsyncRelayCommand SemainePrecedenteCommand { get; }
    public IAsyncRelayCommand SemaineSuivanteCommand { get; }
    public IAsyncRelayCommand AujourdhiuCommand { get; }

    public AgendaViewModel(IOrdonnanceService ordonnanceService, IRappelService rappelService)
    {
        _ordonnanceService = ordonnanceService;
        _rappelService = rappelService;

        var today = DateTime.Today;
        _semaineDebut = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

        SemainePrecedenteCommand = new AsyncRelayCommand(async () =>
        {
            SemaineDebut = SemaineDebut.AddDays(-7);
            await LoadAsync();
        });

        SemaineSuivanteCommand = new AsyncRelayCommand(async () =>
        {
            SemaineDebut = SemaineDebut.AddDays(7);
            await LoadAsync();
        });

        AujourdhiuCommand = new AsyncRelayCommand(async () =>
        {
            var t = DateTime.Today;
            SemaineDebut = t.AddDays(-(int)t.DayOfWeek + (int)DayOfWeek.Monday);
            await LoadAsync();
        });
    }

    public async Task LoadAsync()
    {
        Jours.Clear();
        var fin = SemaineDebut.AddDays(6);
        var ordonnances = (await _ordonnanceService.GetByDateRangeAsync(SemaineDebut, fin)).ToList();
        var rappels = (await _rappelService.GetRappelsNonVusAsync()).ToList();

        for (int i = 0; i < 7; i++)
        {
            var date = SemaineDebut.AddDays(i);
            var jour = new JourCalendrier { Date = date };

            foreach (var o in ordonnances.Where(o => o.ProchainRenouvellement.Date == date.Date))
                jour.Ordonnances.Add(o);

            foreach (var r in rappels.Where(r => r.DateRappel.Date == date.Date))
                jour.Rappels.Add(r);

            Jours.Add(jour);
        }
    }
}