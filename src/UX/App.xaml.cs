﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Core.Services;
using Seemon.Vault.Services;
using Seemon.Vault.ViewModels;
using Seemon.Vault.Views;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace Seemon.Vault
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public T GetService<T>() where T : class => _host.Services.GetService(typeof(T)) as T;

        public App() { }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var activationArgs = new Dictionary<string, string>
            {

            };
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(appLocation);
                    c.AddInMemoryCollection(activationArgs);
                })
                .ConfigureServices(ConfigureServices)
                .Build();

            await _host.StartAsync();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Application Host
            services.AddHostedService<ApplicationHostService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Services
            services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();


            // Views and ViewModels
            services.AddTransient<IShellWindow, ShellWindow>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<WelcomeViewModel>();
            services.AddTransient<WelcomePage>();

            services.AddTransient<AboutViewModel>();
            services.AddTransient<AboutPage>();

            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();

            services.AddTransient<LicenseViewModel>();
            services.AddTransient<LicensePage>();

            // Configuration
            services.Configure<ApplicationConfig>(context.Configuration.GetSection(nameof(ApplicationConfig)));
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            _host = null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

        }
    }
}
