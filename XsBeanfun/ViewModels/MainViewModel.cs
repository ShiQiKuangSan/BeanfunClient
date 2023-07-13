using Beanfun.Api;
using Beanfun.Api.Models;
using Beanfun.Common;
using Beanfun.Common.Models.Config;
using Beanfun.Common.Services;
using Beanfun.Models.Main;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Beanfun.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region APP显示问题

        /// <summary>
        /// 是否显示账号
        /// </summary>
        [ObservableProperty]
        private Visibility _isShowAccount = Visibility.Visible;

        /// <summary>
        /// 是否在获取账号中
        /// </summary>
        [ObservableProperty]
        private Visibility _isLoding = Visibility.Hidden;

        [ObservableProperty]
        private string _lodingName;

        public void ShowAccount()
        {
            IsShowAccount = Visibility.Visible;

            IsLoding = Visibility.Hidden;
        }

        public void ShowLoding(string name = "获取账号中..")
        {
            IsLoding = Visibility.Visible;

            IsShowAccount = Visibility.Hidden;

            LodingName = name;
        }

        #endregion APP显示问题

        private readonly IMessageService? _messageService;
        private readonly IConfigService? _configService;

        /// <summary>
        /// 账号集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<AccountInfoModel> _accountList;

        [ObservableProperty]
        private AccountInfoModel? _cuAccount;

        [ObservableProperty]
        private string? _cuAccountStr;

        [ObservableProperty]
        private string? _cuPassword;

        [ObservableProperty]
        private string? _gamePath;

        [ObservableProperty]
        private Brush? _cuAccountStatusColor;

        [ObservableProperty]
        private Config? _config;

        public MainViewModel()
        {
            _messageService = BeanfunConst.Instance.MessageService;
            _configService = BeanfunConst.Instance.ConfigService;

            _config = _configService?.GetConfig();

            LodingName = "获取账号中..";

            AccountList = new();

            ShowAccount();
        }

        /// <summary>
        /// 窗口加载完成
        /// </summary>
        /// <returns></returns>
        public async Task Loaded()
        {
            ShowLoding();

            BeanfunResult<BeanfunAccountResult> result = await BeanfunApi.Client()
                .GetAccountList(BeanfunConst.Instance.Token);

            if (!result.IsSuccess)
            {
                _messageService?.Show("加载账号失败", "提示");
                ShowAccount();
                return;
            }

            if (result.Data == null)
            {
                _messageService?.Show("获取账号列表失败", "提示");
                ShowAccount();
                return;
            }

            if (result.Data.CertStatus == false)
            {
                _messageService?.Show("账号还未进阶认证，请自行认证后再登录", "提示");
                ShowAccount();
                return;
            }

            foreach (var item in result.Data.AccountList)
            {
                AccountList.Add(new AccountInfoModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    StatusStr = item.Status ? "正常" : "锁定",
                    Status = item.Status,
                    Sn = item.Sn,
                    CreateTime = item.CreateTime,
                });
            }

            if (AccountList.Any())
            {
                CuAccount = AccountList[0];

                CuAccountStr = CuAccount?.Id;
                CuPassword = CuAccount?.PassWord;
                SetStatusColor();
            }

            GamePath = Config?.GamePath;

            ShowAccount();
        }


        public void SetStatusColor()
        {

            if (CuAccount?.StatusStr == "正常")
            {
                CuAccountStatusColor = Brushes.Green;
            }
            else
            {
                CuAccountStatusColor = Brushes.Red;
            }
        }

        /// <summary>
        /// 获取游戏路径
        /// </summary>
        [RelayCommand]
        public void GetGamePath()
        {
            OpenFileDialog dialog = new();

            dialog.DefaultExt = ".exe";
            dialog.Filter = "Executable Files (.exe)|*.exe";

            var result = dialog.ShowDialog();
            if (result == true)
            {
                var path = dialog.FileName;

                //保存游戏路径
                if (Config != null)
                {
                    GamePath = path;
                    Config.GamePath = path;
                    _configService?.SaveConfig(Config);
                }
            }
        }

        /// <summary>
        /// 启动游戏
        /// </summary>
        [RelayCommand]
        public async void GameStart()
        {
            if (Config == null)
            {
                _messageService?.Show("初始化app配置失败!");
                return;
            }

            if (CuAccount == null)
            {
                _messageService?.Show("选择账号后再启动游戏!");
                return;
            }

            if (string.IsNullOrEmpty(Config.GamePath))
            {
                _messageService?.Show("请配置游戏路径!");
                GetGamePath();
                return;
            }

            if (!File.Exists(Config.GamePath))
            {
                _messageService?.Show("配置的游戏文件不存在!");
                GetGamePath();
                return;
            }

            if (Config.AutoInput)
            {
                //自动输入密码模式
                ShowLoding("正在获取动态密码..");

                var result = await BeanfunApi.Client().GetDynamicPassword(new BeanfunAccount
                {
                    Id = CuAccount.Id,
                    Name = CuAccount.Name,
                    CreateTime = CuAccount.CreateTime,
                    Sn = CuAccount.Sn,
                    Status = CuAccount.Status
                }, BeanfunConst.Instance.Token);

                if (!result.IsSuccess)
                {
                    _messageService?.Show("获取动态密码失败");
                    ShowAccount();
                    return;
                }

                if (string.IsNullOrEmpty(result.Data))
                {
                    _messageService?.Show("获取动态密码失败，请检测您的网络是否通畅");
                    ShowAccount();
                    return;
                }

                //设置密码到文本框
                CuAccount.PassWord = result.Data;
                CuPassword = result.Data;

                GameHandler.StartGame(Config.GamePath, CuAccount.Id, CuAccount.PassWord);
            }
            else
            {
                GameHandler.StartGame(Config.GamePath, null, null);
            }
        }

        /// <summary>
        /// 获取动态密码
        /// </summary>
        [RelayCommand]
        public async void GetDynamicPassword()
        {
            if (CuAccount == null)
            {
                _messageService?.Show("选择账号后再启动游戏!");
                return;
            }

            ShowLoding("获取动态密码中..");

            var result = await BeanfunApi.Client().GetDynamicPassword(new BeanfunAccount
            {
                Id = CuAccount.Id,
                Name = CuAccount.Name,
                CreateTime = CuAccount.CreateTime,
                Sn = CuAccount.Sn,
                Status = CuAccount.Status
            }, BeanfunConst.Instance.Token);

            if (!result.IsSuccess)
            {
                _messageService?.Show("获取动态密码失败");
                ShowAccount();
                return;
            }

            if (string.IsNullOrEmpty(result.Data))
            {
                _messageService?.Show("获取动态密码失败，请检测您的网络是否通畅");
                ShowAccount();
                return;
            }

            //设置密码到文本框
            CuAccount.PassWord = result.Data;
            CuPassword = result.Data;
            ShowAccount();
        }

        public void LoginOut()
        {
            BeanfunConst.Instance.Page.Dispose();

            BeanfunConst.Instance.Page.LaunchAsync();
        }


        public void OpenInBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
        }
    }
}