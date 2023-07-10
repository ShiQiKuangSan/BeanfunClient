using Beanfun.Common;
using Beanfun.Common.Services;
using Beanfun.Models.Main;
using Beanfun.ViewModels;

using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;

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

        public MainWindow(Window window)
        {
            InitializeComponent();
            _baseWindow = window;
            this.DataContext = viewModel = new MainViewModel();

            _configService = BeanfunConst.Instance.ConfigService;

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
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is AccountInfoModel model)
            {
                viewModel.CuAccount = model;
                viewModel.SetStatusColor();
            }
        }

        private void SetConfig_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menu && viewModel.Config != null)
            {
                var str = menu.Header.ToString();
                if (str == "阻止游戏自动更新")
                {
                    viewModel.Config.KillGamePatcher = menu.IsChecked;
                }
                else if (str == "自动跳过PLAY窗口")
                {
                    viewModel.Config.KillStartPalyWindow = menu.IsChecked;
                }
                else if (str == "输入法钩子(Win7)")
                {
                    viewModel.Config.HookInput = menu.IsChecked;
                }

                _configService?.SaveConfig(viewModel.Config);
            }
        }

        private void OpenUrl_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menu)
            {
                var str = menu.Header.ToString();
                var url = string.Empty;

                if (str == "新枫之谷台服官网")
                {
                    url = "https://maplestory.beanfun.com/main";
                }
                else if (str == "新香港橘子官网")
                {
                    url = "https://bfweb.hk.beanfun.com/";
                }
                else if (str == "透视镜")
                {
                    url = "http://gametsg.techbang.com/maplestory/";
                }
                else if (str == "巴哈姆特")
                {
                    url = "https://forum.gamer.com.tw/A.php?bsn=7650/";
                }
                else if (str == "新枫之谷贴吧")
                {
                    url = "https://tieba.baidu.com/f?kw=%E6%96%B0%E6%9E%AB%E4%B9%8B%E8%B0%B7";
                }

                if (!string.IsNullOrEmpty(url))
                {
                    viewModel.OpenInBrowser(url);
                }
            }
        }

        private void UserInfo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menu)
            {
                var browser = new XsBrowser();

                var str = menu.Header.ToString();
                string url = string.Empty;

                if (str == "会员中心")
                {
                    url = HkBeanfunUrl.GetWebUrlMemberCenter();
                }
                else if (str == "充值中心")
                {
                    url = HkBeanfunUrl.GetWebUrlMemberTopUp();
                }
                else if (str == "客服中心")
                {
                    url = "https://csp.hk.beanfun.com/";
                }

                if (!string.IsNullOrEmpty(url))
                {
                    browser.SetUrl(url);
                    browser.ShowDialog();
                }
            }
        }

        private void KillBlackXchg_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.KillBlackXchg();
        }

        /// <summary>
        /// 纸娃娃
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMapEmul_Click(object sender, RoutedEventArgs e)
        {
            BeanfunConst.Instance.StartMapEmul();
        }

        /// <summary>
        /// 联盟摆放模拟器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WarAlliance_Click(object sender, RoutedEventArgs e)
        {
            BeanfunConst.Instance.StartWarAlliance();
        }

        private void LoginOut_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LoginOut();

            this.Close();
        }
    }
}