using CommunityToolkit.Mvvm.ComponentModel;

namespace Beanfun.ViewModels
{
    public partial class XsBrowserViewModel : ObservableObject
    {
        /// <summary>
        /// 浏览的url
        /// </summary>
        [ObservableProperty]
        private string _url;
    }
}