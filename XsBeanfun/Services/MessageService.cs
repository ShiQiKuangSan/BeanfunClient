using Beanfun.Common.Services;

using System.Windows;

using XsBeanfun;

namespace Beanfun.Services
{
    public class MessageService : IMessageService
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