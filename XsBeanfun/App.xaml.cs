using Beanfun.Common;
using Beanfun.Common.Services;
using Beanfun.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using SharpCompress.Common;

using System;
using System.IO;
using System.Windows;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XsBeanfun
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitAppService();
        }

        public new static App Current => (App)Application.Current;

        //public IServiceProvider Services { get; private set; }

        //private static IServiceProvider ConfigureServices()
        //{
        //    var services = new ServiceCollection();

        //    services.AddSingleton<IMessageService, MessageService>();
        //    services.AddSingleton<IConfigService, ConfigService>();

        //    return services.BuildServiceProvider();
        //}

        private static void InitAppService()
        {
            var msg = new MessageService();
            BeanfunConst.Instance.InitApp(new ConfigService(msg), msg);
            WindowManager.CloseChrome();
        }
    }
}