namespace Beanfun.Common.Services
{
    public interface IMessageService
    {
        void Show(string message);

        void Show(string message, string title);
    }
}