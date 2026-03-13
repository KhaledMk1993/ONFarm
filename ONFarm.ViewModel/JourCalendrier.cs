using CommunityToolkit.Mvvm.ComponentModel;
using ONFarm.Domain.Entities;
using System.Collections.ObjectModel;

namespace ONFarm.ViewModel;

public class JourCalendrier : ObservableObject
{
    public DateTime Date { get; set; }
    public string NomJour => Date.ToString("dddd");
    public string NumeroJour => Date.Day.ToString();
    public bool EstAujourdhui => Date.Date == DateTime.Today;
    public ObservableCollection<Ordonnance> Ordonnances { get; } = new();
    public ObservableCollection<Rappel> Rappels { get; } = new();
    public bool HasEvents => Ordonnances.Any() || Rappels.Any();
}