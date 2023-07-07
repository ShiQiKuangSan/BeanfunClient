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
        public XsBrowser()
        {
            InitializeComponent();

            this.DataContext = new XsBrowserViewModel();
        }
    }
}