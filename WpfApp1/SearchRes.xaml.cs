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
        private string Key { get; set; }
        private Window1 ParentWindow { get; set; }
        
        public SearchRes(Window1 window,string Key)
        {
            this.Key = Key;
            ParentWindow = window;
            InitializeComponent();
            Bind();
        }

        #region 将记录条数绑定到xaml
        public class Countobj : INotifyPropertyChanged
        {
            public int Count { set; get; }
            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region 进行数据库查询并进行数据绑定
        private void Bind()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection sqlcn = new SqlConnection(Config.SqlCredentials))
                {
                    #region 通过ISBN精准查询
                    try
                    {
                        if (Key.Length == 13 || Key.Length == 10)
                            using (SqlCommand cmd = new SqlCommand("SELECT ISBN,AName,ANationality,BName,PubName FROM BOOKS,Authors,Publishers Where Books.AID=Authors.AID AND Books.PubID=Publishers.PubID AND ISBN=@ISBN", sqlcn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@ISBN", long.Parse(Key)));
                                SqlDataAdapter adapter = new SqlDataAdapter();
                                sqlcn.Open();
                                adapter.SelectCommand = cmd;
                                adapter.Fill(ds);
                                dt = ds.Tables[0];
                            }
                        else throw new FormatException();
                    }
                    #endregion
                    #region 通过书名、作者名、出版社名模糊查询，并合并重复结果
                    catch (FormatException)
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT ISBN,AName,ANationality,BName,PubName FROM BOOKS,Authors,Publishers Where Books.AID=Authors.AID AND Books.PubID=Publishers.PubID AND BName like @BName;" +
                                                                "SELECT ISBN,AName,ANationality,BName,PubName FROM BOOKS,Authors,Publishers Where Books.AID=Authors.AID AND Books.PubID=Publishers.PubID AND AName like @AName;"+
                                                                "SELECT ISBN,AName,ANationality,BName,PubName FROM BOOKS,Authors,Publishers Where Books.AID=Authors.AID AND Books.PubID=Publishers.PubID AND PubName like @PubName",
                                                                sqlcn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@BName", "%" + Key + "%"));
                            cmd.Parameters.Add(new SqlParameter("@AName", "%" + Key + "%"));
                            cmd.Parameters.Add(new SqlParameter("@PubName", "%" + Key + "%"));
                            SqlDataAdapter adapter = new SqlDataAdapter { SelectCommand = cmd };
                            adapter.Fill(ds);
                            ds.Tables[0].Merge(ds.Tables[1]);
                            ds.Tables[0].Merge(ds.Tables[2]);
                            var dv = new DataView(ds.Tables[0]);
                            string[] strCols = { "ISBN", "AName", "ANationality", "BName", "PubName" };
                            dt = dv.ToTable(true, strCols);
                        }
                    }
                    #endregion
                }
            }
            #region 错误处理
            catch (Exception ex)
            {

                var message = new myMessageBox("出现错误！" + Environment.NewLine + "详细信息" + ex.Message, "警告");
                message.ShowDialog();
                ParentWindow.frmMain.Navigate(new Index(ParentWindow));
            }
            #endregion

            #region 获取查询到的结果条数，绑定到xaml
            listBox.DataContext = dt;
            Countobj countobj = new Countobj { Count = dt.Rows.Count };
            countS.DataContext = countobj;
            #endregion
        }
        #endregion
        #region 返回键
        private void Back_Click(object sender, RoutedEventArgs e) => ParentWindow.frmMain.Navigate(new Index(ParentWindow));
        #endregion
        #region 双击列表条目跳转至详细信息
        private void ListBox_Selected(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListBox).SelectedItem;
            var temp = (item as DataRowView).Row.ItemArray[0];
            string ISBN=temp.ToString();
            Detail detail = new Detail(ISBN, this, ParentWindow);
            ParentWindow.frmMain.Navigate(detail);
            return;
        }
        #endregion
    }
}
