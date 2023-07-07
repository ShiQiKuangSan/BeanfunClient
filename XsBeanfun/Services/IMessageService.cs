using System.Windows;

using XsBeanfun;

namespace Beanfun.Services
{
    internal interface IMessageService
    {
        void Show(string message);

        void Show(string message, string title);
    }

    internal class MessageService : IMessageService
    {
        public void Show(string message)
        {
            Show(message, "提示");
        }

        public void Show(string message, string title)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message);
            });
        }
    }
}