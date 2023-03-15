using AllOverIt.ReactiveUI.Factories;
using CountdownTimerAppDemo.Views;
using CountdownTimerAppDemo.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using System.Windows;

namespace CountdownTimerAppDemo
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = new HostBuilder()
                        .ConfigureServices((context, services) =>
                        {
                            services.AddSingleton<MainWindow>();
                            services.AddSingleton<IViewFactory, ViewFactory>();
                            services.AddTransient<IViewFor<DoneWindowViewModel>, DoneWindow>();
                        })
                        .Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
