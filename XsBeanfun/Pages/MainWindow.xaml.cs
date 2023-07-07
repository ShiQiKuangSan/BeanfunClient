using Beanfun.Api;
using Beanfun.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Beanfun.Pages
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //var result = await BeanfunApi.Client().GetAccountList(BeanfunConst.Instance.Token);

            //if (!result.IsSuccess)
            //{
            //    return;
            //}

            //var res = await BeanfunApi.Client().GetDynamicPassword(result.Data.AccountList[0], BeanfunConst.Instance.Token);
        }
    }
}