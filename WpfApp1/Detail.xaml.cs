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
    /// Detail.xaml 的交互逻辑
    /// </summary>
    public partial class Detail : Page
    {
        public string _ISBN { set; get; }
        public SearchRes Res { set; get; }
        public Window1 ParentWindow { get; set; }

        public Detail()
        {
            InitializeComponent();
            Bind();
        }

        private void Bind()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.frmMain.Navigate(Res);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
