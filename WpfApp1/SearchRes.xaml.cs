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
    /// SearchRes.xaml 的交互逻辑
    /// </summary>
    public partial class SearchRes : Page
    {
        private Window1 _parentWin;
        public Window1 ParentWindow
        {
            get { return _parentWin; }
            set { _parentWin = value; }
        }
        public SearchRes()
        {
            InitializeComponent();
        }
    }
}
