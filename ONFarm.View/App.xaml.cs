using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ONFarm.Application.Interfaces;
using ONFarm.Application.Services;
using ONFarm.Domain.Interfaces;
using ONFarm.Infrastructure.Data;
using ONFarm.Infrastructure.Repositories;
using ONFarm.ViewModel;
using System.Windows;
using ONFarm.View.Views;

namespace ONFarm.View;

public partial class App
{
    private ServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        using var ctx = _serviceProvider
            .GetRequiredService<IDbContextFactory<ONFarmDbContext>>()
            .CreateDbContext();
        ctx.Database.Migrate();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextFactory<ONFarmDbContext>(options =>
            options.UseNpgsql(
                "Host=localhost;Port=5432;Database=onfarm_db;Username=postgres;Password=postgres"));

        services.AddTransient<IPatientRepository, PatientRepository>();
        services.AddTransient<IOrdonnanceRepository, OrdonnanceRepository>();
        services.AddTransient<IRappelRepository, RappelRepository>();
        services.AddTransient<IMedicamentRepository, MedicamentRepository>();

        services.AddTransient<IPatientService, PatientService>();
        services.AddTransient<IOrdonnanceService, OrdonnanceService>();
        services.AddTransient<IRappelService, RappelService>();
        services.AddTransient<IMedicamentService, MedicamentService>();
        services.AddTransient<IExcelService, ExcelService>();

        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<PatientListViewModel>();
        services.AddSingleton<PatientDetailViewModel>();
        services.AddSingleton<PatientFormViewModel>();
        services.AddSingleton<AgendaViewModel>();
        services.AddSingleton<RappelsViewModel>();
        services.AddSingleton<MainViewModel>();

        services.AddTransient<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnExit(e);
    }
}