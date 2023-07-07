using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Beanfun.Models.Login
{
    public class DeleteAccountEvent : ValueChangedMessage<AccountModel>
    {
        public DeleteAccountEvent(AccountModel value) : base(value)
        {
        }
    }
}