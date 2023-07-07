using Beanfun.Services;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Windows;

namespace XsBeanfun
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            //StartupUri = new Uri("Pages/MainWindow.xaml", UriKind.Relative);
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; private set; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IConfigService, ConfigService>();

            return services.BuildServiceProvider();
        }
    }
}