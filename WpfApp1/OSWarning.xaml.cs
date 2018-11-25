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

namespace WpfApp1
{
    /// <summary>
    /// OSCheck.xaml 的交互逻辑
    /// </summary>
    public partial class OSWarning : Window
    {
        private MainWindow w;
        public OSWarning(MainWindow window )
        {
            w = window;
            InitializeComponent();
            this.Closing += OSWarning_Closing;
        }

        private void OSWarning_Closing(object sender, System.ComponentModel.CancelEventArgs e) => w.Close();
        private void Y_Click(object sender, RoutedEventArgs e)
        {
            Closing -= OSWarning_Closing;
            this.Close();
        }

        private void N_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
