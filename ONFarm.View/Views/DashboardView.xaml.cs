using ONFarm.Domain.Entities;
using ONFarm.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ONFarm.View.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is DashboardViewModel vm && sender is DataGrid dg && dg.SelectedItem is Patient patient)
            vm.OpenPatientCommand.Execute(patient);
    }
}
