using Beanfun.Common;
using Beanfun.Common.Models;
using Beanfun.Common.Models.Config;
using Beanfun.Common.Services;
using Beanfun.Models.Login;
using Beanfun.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.DependencyInjection;

using PuppeteerSharp;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using XsBeanfun;

namespace Beanfun.ViewModels
{
    internal partial class LoginViewModel : ObservableObject, IDisposable
    {
        #region APP显示问题

        [ObservableProperty]
        private Visibility _isInputAct = Visibility.Hidden;

        /// <summary>
        /// 是否在登录中
        /// </summary>
        [ObservableProperty]
        private Visibility _isLoding = Visibility.Hidden;

        /// <summary>
        /// app是否在初始化中
        /// </summary>
        [ObservableProperty]
        private Visibility _isAppInit = Visibility.Hidden;

        public void ShowInputAct()
        {
            IsInputAct = Visibility.Visible;

            IsLoding = Visibility.Hidden;

            IsAppInit = Visibility.Hidden;
        }

        public void ShowLoding()
        {
            IsLoding = Visibility.Visible;

            IsInputAct = Visibility.Hidden;

            IsAppInit = Visibility.Hidden;
        }

        public void ShowAppInit()
        {
            IsAppInit = Visibility.Visible;
            IsInputAct = Visibility.Hidden;
            IsLoding = Visibility.Hidden;
        }

        #endregion APP显示问题

        /// <summary>
        /// 记住密码
        /// </summary>
        [ObservableProperty] private bool _recordActPwd;

        /// <summary>
        /// 账号集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<AccountModel> _accountList;

        /// <summary>
        /// 当前需要登录的账号
        /// </summary>
        [ObservableProperty]
        private AccountModel? _cuAccount;

        private readonly Config _config;
        private readonly IConfigService? _configService;

        private BeanfunPage? _beanfunPage;

        public LoginViewModel()
        {
            _configService = BeanfunConst.Instance.ConfigService;
            _configService?.InitConfig();

            _config = _configService?.GetConfig() ?? new Config();

            WeakReferenceMessenger.Default.Register<DeleteAccountEvent>(this, DeleteAccount);

            _accountList = new ObservableCollection<AccountModel>();
            InitAccount();

            RecordActPwd = _config.RecordActPwd;
        }

        /// <summary>
        /// 初始化app
        /// </summary>
        /// <returns></returns>
        public async Task AppInit()
        {
            ShowAppInit();

            BeanfunConst.Instance.InitPlugin();

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

            _beanfunPage = BeanfunConst.Instance.Page;

            ShowInputAct();
        }

        public async Task<LoginResult> Login(string account, string password)
        {
            LoginResult loginResult = new();

            if (_beanfunPage == null)
            {
                return loginResult.Fail("初始化Browser 失败");
            }

            ShowLoding();

            var page = await _beanfunPage.GetPage("LoginPage");

            if (page == null)
                return loginResult.Fail("初始化Page 失败");

            await page.SetUserAgentAsync(BeanfunHeader.UserAgent);

            var result = await page.GoToAsync(HkBeanfunUrl.LogintUrl, WaitUntilNavigation.DOMContentLoaded);

            if (result.Status != HttpStatusCode.OK)
            {
                return loginResult.Fail("网络异常，请检查你的网络是都通畅");
            }

            var html = await result.TextAsync();

            if (string.IsNullOrEmpty(html))
            {
                await page.CloseAsync();
                return loginResult.Fail("网络异常，请检查你的网络是都通畅1000");
            }

            if (html.Contains("IP已自動被系統鎖定"))
            {
                await page.CloseAsync();
                return loginResult.Fail("IP被锁定,请关闭加速器或更换节点再重试!");
            }

            if (html.Contains("目前無法在您的國家或地區瀏覽此網站"))
            {
                await page.CloseAsync();
                return loginResult.Fail("您所在的区域不允许登录!");
            }

            await page.WaitForSelectorAsync("#ifmIdPassForm", new WaitForSelectorOptions { Timeout = 30000 });

            await Task.Delay(3000);

            var frame = page.Frames.FirstOrDefault(x => x.Name == "ifmIdPassForm");

            if (frame == null)
            {
                await page.CloseAsync();
                return loginResult.Fail("登录错误1000");
            }

            await frame.WaitForSelectorAsync("#token1", new WaitForSelectorOptions { Timeout = 30000 });

            await frame.WaitForSelectorAsync("#t_AccountID", new WaitForSelectorOptions { Timeout = 30000 });

            html = await frame.GetContentAsync();

            if (string.IsNullOrEmpty(html))
            {
                await page.CloseAsync();
                return loginResult.Fail("网络异常，请检查你的网络是都通畅2000");
            }

            await frame.FocusAsync("#t_AccountID");

            await page.Keyboard.SendCharacterAsync(account);

            await frame.FocusAsync("#t_Password");

            await page.Keyboard.SendCharacterAsync(password);

            await frame.ClickAsync("#btn_login");

            await page.WaitForSelectorAsync("#BF_btnLogout");

            var cookies = await page.GetCookiesAsync();

            var cookie = cookies.FirstOrDefault(x => x.Name == "bfWebToken");

            if (cookie == null)
            {
                //重试5次
                for (int i = 0; i < 4; i++)
                {
                    cookies = await page.GetCookiesAsync();
                    cookie = cookies.FirstOrDefault(x => x.Name == "bfWebToken");
                    Thread.Sleep(1000);
                    if (cookie != null)
                    {
                        break;
                    }
                }

                if (cookie == null)
                {
                    await page.CloseAsync();
                    return loginResult.Fail("获取WebToken失败");
                }
            }

            loginResult.Token = cookie.Value;

            SaveAccount(account, password, AccountType.HongKong);

            ShowInputAct();

            return loginResult.Success();
        }

        /// <summary>
        /// 记住密码
        /// </summary>
        [RelayCommand]
        public void RecordPwd()
        {
            _config.RecordActPwd = RecordActPwd;
            _configService?.SaveConfig(_config);
        }

        /// <summary>
        /// 删除当前的账号
        /// </summary>
        /// <param name="o"></param>
        /// <param name="message"></param>
        public void DeleteAccount(object o, DeleteAccountEvent evt)
        {
            var account = AccountList.FirstOrDefault(x => x.Account == evt.Value.Account);

            if (account == null) return;

            AccountList.Remove(account);

            //删除配置文件中的数据
            var acc = _config.Accounts.FirstOrDefault(x => x.Account == account.Account);

            if (acc == null) return;

            _config.Accounts.Remove(acc);

            _configService?.SaveConfig(_config);
        }

        private void InitAccount(AccountType type = AccountType.HongKong)
        {
            var list = _config.Accounts?
                .Where(x => x.AccountType == type)
                .ToList();

            for (int i = 0; i < list?.Count; i++)
            {
                ConfigAccount x = list[i];

                var account = new AccountModel() { Account = x.Account, Password = x.PassWord, IsDefault = x.IsDefault };
                AccountList.Add(account);

                if (CuAccount == null && x.IsDefault)
                {
                    CuAccount = account;
                }
            }
        }

        private async void SaveAccount(string account, string password, AccountType type)
        {
            if (!_config.Accounts.Any())
            {
                var item = new ConfigAccount
                {
                    Account = account,
                    PassWord = password,
                    AccountType = type,
                    IsDefault = true
                };

                if (!RecordActPwd)
                {
                    item.PassWord = string.Empty;
                }

                _config.Accounts = new List<ConfigAccount> { item };
            }
            else
            {
                var item = _config.Accounts.FirstOrDefault(x => x.Account == account);

                if (item == null)
                {
                    var acc = new ConfigAccount
                    {
                        Account = account,
                        PassWord = password,
                        AccountType = AccountType.HongKong,
                        IsDefault = true
                    };

                    if (!RecordActPwd)
                    {
                        acc.PassWord = string.Empty;
                    }

                    _config.Accounts.Add(acc);
                }
                else
                {
                    item.Account = account;

                    if (!RecordActPwd)
                    {
                        item.PassWord = string.Empty;
                    }
                    else
                    {
                        item.PassWord = password;
                    }

                    item.AccountType = AccountType.HongKong;
                    item.IsDefault = true;
                }

                SetNotDefault(account);
            }

            if (_configService == null)
            {
                return;
            }

            await _configService.SaveConfig(_config);
        }

        private void SetNotDefault(string account)
        {
            foreach (var item1 in _config.Accounts)
            {
                if (item1.Account != account)
                {
                    item1.IsDefault = false;
                }
            }
        }

        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}