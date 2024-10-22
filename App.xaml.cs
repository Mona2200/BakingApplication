using BakingApplication.Data;
using BakingApplication.Data.Interfaces;
using BakingApplication.Data.Repositories;
using BakingApplication.Models;
using BakingApplication.ViewModels;
using BakingApplication.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Windows;

namespace BakingApplication;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private ServiceProvider serviceProvider;

    public App()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddDbContext<BakingApplicationContext>(options => options.UseSqlite(ConfigurationManager.AppSettings["DbConnectString"]), ServiceLifetime.Singleton);

        services.AddSingleton<IBakingRepository, BakingRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IExpenseRepository, ExpenseRepository>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<CatalogViewModel>();
        services.AddSingleton<AddBakingViewModel>();
        services.AddSingleton<OrderViewModel>();
        services.AddSingleton<RemoveBakingViewModel>();
        services.AddSingleton<OrderListViewModel>();
        services.AddSingleton<ExpenseViewModel>();
        services.AddSingleton<AddExpenseViewModel>();
        services.AddSingleton<StatisticsViewModel>();

        services.AddSingleton<MainWindow>();

        services.AddSingleton<OrderModel>();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        MainViewModel mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.DataContext = mainViewModel;
        mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        mainWindow.Height = SystemParameters.WorkArea.Height;
        mainWindow.Width = SystemParameters.WorkArea.Width;
        mainWindow.Show();
    }
}
