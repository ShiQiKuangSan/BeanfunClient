using Beanfun.Common;
using Beanfun.Common.Services;
using Beanfun.Models.Main;
using Beanfun.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using System.Windows;
using System.Windows.Controls;

using XsBeanfun;

namespace Beanfun.Pages
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        private readonly MainViewModel viewModel;
        private readonly Window _baseWindow;
        private readonly IConfigService? _configService;
        private readonly IMessageService? _messageService;

        public MainWindow(Window window)
        {
            InitializeComponent();
            _baseWindow = window;
            this.DataContext = viewModel = new MainViewModel();

            _configService = BeanfunConst.Instance.ConfigService;
            _messageService = BeanfunConst.Instance.MessageService;

            this.Loaded += async (sender, e) =>
            {
                await viewModel.Loaded();
            };

            this.Closed += (sender, e) =>
            {
                _baseWindow.Show();
                this.Close();
            };

            xAccounts.SelectionChanged += XAccounts_SelectionChanged;
        }

        private void XAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is AccountInfoModel model1)
            {
                viewModel.CuAccount = model1;
            }
        }
    }
}