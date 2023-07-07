using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Beanfun.Models.Login
{
    internal class UiChangeEvent : ValueChangedMessage<string>
    {

        public UiChangeEvent(string value) : base(value)
        {
        }
    }
}
