using Beanfun.Common.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Beanfun.Models.Login
{
    public partial class AccountModel : ObservableObject
    {
        /// <summary>
        /// 账号
        /// </summary>
        [ObservableProperty]
        private string _account;

        /// <summary>
        /// 密码
        /// </summary>
        [ObservableProperty]
        private string _password;

        /// <summary>
        /// 账号类型
        /// </summary>
        [ObservableProperty]
        private AccountType _accountType;

        public bool IsDefault { get; set; } = false;

        [RelayCommand]
        public void DeleteAccount()
        {
            WeakReferenceMessenger.Default.Send(new DeleteAccountEvent(this));
            WeakReferenceMessenger.Default.Send(new UiChangeEvent("DeleteAccount"));
        }
    }
}