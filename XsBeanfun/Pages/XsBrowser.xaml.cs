using Beanfun.ViewModels;

using System;
using System.ComponentModel;
using System.Windows;

namespace Beanfun.Pages
{
    /// <summary>
    /// XsBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class XsBrowser : Window
    {
        private readonly XsBrowserViewModel _viewModel;

        public XsBrowser()
        {
            InitializeComponent();

            this.DataContext = _viewModel = new XsBrowserViewModel();
        }

        public void SetUrl(string url)
        {
            _viewModel.Url = url;
        }
    }
}