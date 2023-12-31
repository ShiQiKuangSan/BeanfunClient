﻿using Beanfun;
using Beanfun.Common;
using Beanfun.Common.Services;
using Beanfun.Models.Login;
using Beanfun.Pages;
using Beanfun.ViewModels;

using CommunityToolkit.Mvvm.Messaging;

using System;
using System.Windows;
using System.Windows.Input;

namespace XsBeanfun
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : BaseWindow
    {
        private XsBrowser? xsBrowser = null;
        private readonly LoginViewModel viewModel;
        private readonly IMessageService? _messageService;
        private readonly IConfigService? _configService;

        public LoginWindow()
        {
            InitializeComponent();

            this.DataContext = viewModel = new LoginViewModel();

            _configService = BeanfunConst.Instance.ConfigService;
            _messageService = BeanfunConst.Instance.MessageService;

            WeakReferenceMessenger.Default.Register<UiChangeEvent>(this, UiChange);

            this.Loaded += async (s, e) =>
            {
                await viewModel.AppInit();

                if (viewModel.CuAccount != null)
                {
                    LoginPwd.Password = viewModel.CuAccount.Password;
                }
            };
        }

        private void ForgotPwd_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            xsBrowser = new XsBrowser();
            if (xsBrowser.DataContext is XsBrowserViewModel viewModel)
            {
                viewModel.Url = HkBeanfunUrl.ForgotPwdUrl;
                xsBrowser.ShowDialog();
            }
        }

        private void Register_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            xsBrowser = new XsBrowser();
            if (xsBrowser.DataContext is XsBrowserViewModel viewModel)
            {
                viewModel.Url = HkBeanfunUrl.RegisteredUrl;
                xsBrowser.ShowDialog();
            }
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Login_OnClick(object sender, RoutedEventArgs e)
        {
            var accountTxt = LoginBox.Text;
            var pwd = LoginPwd.Password;

            if (string.IsNullOrEmpty(accountTxt))
            {
                _messageService?.Show("账号为空", "提示");
                return;
            }

            if (string.IsNullOrEmpty(pwd))
            {
                _messageService?.Show("密码为空", "提示");
                return;
            }

            var _config = _configService?.GetConfig();

            if (_config == null)
            {
                _messageService?.Show("读取账号配置失败");
                return;
            }

            var result = await viewModel.Login(accountTxt, pwd);

            if (result.Status == false)
            {
                _messageService?.Show(result.Msg, "提示");
                viewModel.ShowInputAct();
                return;
            }

            BeanfunConst.Instance.Token = result.Token;

            //跳转到登陆成功页
            MainWindow mainWindow = new(this);

            mainWindow.Show();

            this.Hide();
        }

        private void Image_WindowClose(object sender, MouseButtonEventArgs e)
        {
            BeanfunConst.Instance.Page.Dispose();
            this.Close();
        }

        private void UiChange(object recipient, UiChangeEvent message)
        {
            if (message.Value == "DeleteAccount")
            {
                LoginPwd.Password = string.Empty;
            }
        }


        private void BrowserStart_Click(object sender, RoutedEventArgs e)
        {
            BeanfunConst.Instance.Page.Dispose();

            if (BrowserStart.IsChecked == true)
            {
                viewModel.IsBrowserStart = false;
                BeanfunConst.Instance.Page.LaunchAsync(false);
            }
            else
            {
                viewModel.IsBrowserStart= true;
                BeanfunConst.Instance.Page.LaunchAsync(true);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            WeakReferenceMessenger.Default.Unregister<UiChangeEvent>(this);
            WindowManager.CloseChrome();
            BeanfunConst.Instance.Page.Dispose();

            GC.Collect();
        }
    }
}