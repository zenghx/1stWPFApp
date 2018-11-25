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
        public Index()
        {
            InitializeComponent();
            #region 设置自动选定
            TextBox.PreviewMouseDown +=new MouseButtonEventHandler(TextBox_PreviewMouseDown);
            TextBox.GotFocus +=new RoutedEventHandler(TextBox_GotFocus);
            TextBox.LostFocus +=new RoutedEventHandler(TextBox_LostFocus);
            #endregion
            TextBox.Foreground = Brushes.Gray;
        }
        #region 设置自动选定
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox.PreviewMouseDown += new MouseButtonEventHandler(TextBox_PreviewMouseDown);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox.SelectAll();
            TextBox.PreviewMouseDown-= new MouseButtonEventHandler(TextBox_PreviewMouseDown);
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox.Focus();
            e.Handled = true;
        }
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            TextBox.Foreground = Brushes.Black;
        }

        private Window1 _parentWin;
        public Window1 ParentWindow
        {
            get { return _parentWin; }
            set { _parentWin = value; }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var name = btn.Tag.ToString();
            object obj;
            if (name == "add")
            {
                Add add = new Add();
                add.ShowDialog();
                return;
            }
            else if (name == "query")
            {
                SearchRes res = new SearchRes { ParentWindow = _parentWin };
                obj = res;
            }
            else if (name == "report")
            { return; }
            else return;
            _parentWin.frmMain.Content = obj;
            
        }
    }
}
