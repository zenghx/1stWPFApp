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

namespace WpfApp1
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class Index : Page
    {
        private Window1 ParentWindow { get; set; }
        public Index(Window1 window)
        {
            InitializeComponent();
            ParentWindow=window;
            #region 设置自动选定
            SearchBox.PreviewMouseDown +=new MouseButtonEventHandler(TextBox_PreviewMouseDown);
            SearchBox.GotFocus +=new RoutedEventHandler(TextBox_GotFocus);
            SearchBox.LostFocus +=new RoutedEventHandler(TextBox_LostFocus);
            #endregion
            SearchBox.Foreground = Brushes.Gray;
        }
        #region 设置自动选定
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.SelectAll();
            SearchBox.PreviewMouseDown-= new MouseButtonEventHandler(TextBox_PreviewMouseDown);
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Focus();
            e.Handled = true;
        }
        #endregion
        #region 设置文字变色
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            SearchBox.Foreground = Brushes.Black;
        }
        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var name = btn.Tag.ToString();
            #region 根据按钮进行功能跳转
            if (name == "add")
            {
                Add add = new Add();
                add.ShowDialog();
                return;
            }
            else if (name == "query")
            {
                SearchRes res = new SearchRes(ParentWindow, SearchBox.Text);
                ParentWindow.frmMain.Content = res;
            }
            else if (name == "report")
            {
                ReportView report = new ReportView();
                report.ShowDialog();
                return;
            }
            else return;
            #endregion
        }
    }
}
