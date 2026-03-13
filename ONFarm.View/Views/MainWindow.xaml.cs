using ONFarm.ViewModel;
using System.Windows;

namespace ONFarm.View.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        Loaded += async (_, _) =>
        {
            await vm.DashboardVM.LoadDataAsync();
            await vm.PatientListVM.LoadAsync();
            await vm.AgendaVM.LoadAsync();
            await vm.RappelsVM.LoadAsync();
        };
    }
}