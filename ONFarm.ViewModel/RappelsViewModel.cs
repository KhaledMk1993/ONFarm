using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ONFarm.Application.Interfaces;
using ONFarm.Domain.Entities;
using System.Collections.ObjectModel;

namespace ONFarm.ViewModel;

public class RappelsViewModel : ObservableObject
{
    private readonly IRappelService _rappelService;
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<Rappel> Rappels { get; } = new();

    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand<Guid> MarquerVuCommand { get; }
    public IAsyncRelayCommand<Guid> SupprimerCommand { get; }

    public RappelsViewModel(IRappelService rappelService)
    {
        _rappelService = rappelService;

        LoadCommand = new AsyncRelayCommand(LoadAsync);

        MarquerVuCommand = new AsyncRelayCommand<Guid>(async guid =>
        {
            await _rappelService.MarquerCommeVuAsync(guid);
            await LoadAsync();
        });

        SupprimerCommand = new AsyncRelayCommand<Guid>(async guid =>
        {
            await _rappelService.DeleteRappelAsync(guid);
            await LoadAsync();
        });
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var rappels = await _rappelService.GetRappelsNonVusAsync();
            Rappels.Clear();
            foreach (var r in rappels) Rappels.Add(r);
        }
        finally { IsLoading = false; }
    }
}