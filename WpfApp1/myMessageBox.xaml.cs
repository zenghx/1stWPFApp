using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfApp1
{
    /// <summary>
    /// myMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class myMessageBox : Window
    {
        //public string Message { set; get; }
        //public string MyTitle { set; get; }
        public myMessageBox()
        {
            InitializeComponent();
           // MessageBlock.Text = Message;
            //Title = MyTitle;
        }

        public myMessageBox(string Message,string MyTitle)
        {
            InitializeComponent();
            MessageBlock.Text = Message;
            Title = MyTitle;
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Close();
    }
}
