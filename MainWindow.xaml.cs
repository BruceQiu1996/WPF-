using DevExpress.Xpf.Core;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SearchAnyWay
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            //设置样式
            ApplicationThemeHelper.UseLegacyDefaultTheme = true;
            ApplicationThemeHelper.ApplicationThemeName = Theme.VisualStudioCategory;
            this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            this.Icon = new BitmapImage(new Uri("../../debug.png",UriKind.Relative));
            this.BorderThickness = new Thickness(0);
            this.Margin = new Thickness(0);
            this.Padding = new Thickness(0);
            this.DataContext = new MainViewModel();
        }
    }
}
