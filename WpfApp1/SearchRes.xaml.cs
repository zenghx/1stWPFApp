using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
        public string Key { get; set; }
        public Window1 ParentWindow { get; set; }
        
        public SearchRes()
        {
            
            InitializeComponent();
            Bind();
        }

        public class Countobj : INotifyPropertyChanged
        {
            public int Count { set; get; }
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Bind()
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlcn = new SqlConnection(Config.SqlCredentials))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISBN,AName,ANationality,BName,PubName FROM BOOKS,Authors,Publishers Where Books.AID=Authors.AID AND Books.PubID=Publishers.PubID AND ISBN=9787539982830", sqlcn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    sqlcn.Open();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds);
                    listBox.DataContext = ds;
                }
            }
            DataTable DT = ds.Tables[0];
            Countobj countobj = new Countobj { Count = DT.Rows.Count };
            countS.DataContext = countobj;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Index index = new Index { ParentWindow = ParentWindow };
            ParentWindow.frmMain.Navigate(index);
        }

        private void ListBox_Selected(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListBox).SelectedItem;
            var temp = (item as DataRowView).Row.ItemArray[0];
            string ISBN=temp.ToString();
            Detail detail = new Detail { _ISBN = ISBN, Res=this,ParentWindow=ParentWindow };
            ParentWindow.frmMain.Navigate(detail);
            return;
        }
    }
}
