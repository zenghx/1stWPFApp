using System;
using System.Collections.Generic;
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
    /// Detail.xaml 的交互逻辑
    /// </summary>
    public partial class Detail : Page
    {
        private string _ISBN { set; get; }
        private SearchRes Res { set; get; }
        private Window1 ParentWindow { get; set; }

        public Detail(string ISBN,SearchRes res,Window1 window)
        {
            _ISBN = ISBN;
            Res = res;
            ParentWindow = window;
            InitializeComponent();
            Bind();
        }

        private void Bind()
        {
            using (SqlConnection sqlcn = new SqlConnection(Config.SqlCredentials))
            {
                try
                {
                    #region 通过传入的ISBN查询出Books表中的书籍信息，并绑定到TextBox
                    using (SqlCommand cmd = new SqlCommand("SELECT ISBN,AID,PubID,BName,Sales FROM BOOKS Where ISBN=@ISBN", sqlcn))
                    {
                        DataSet ds = new DataSet();
                        cmd.Parameters.Add(new SqlParameter("@ISBN", long.Parse(_ISBN)));
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        sqlcn.Open();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);
                        ISBN.Text = ds.Tables[0].Rows[0][0].ToString();
                        AID.Text = ds.Tables[0].Rows[0][1].ToString();
                        PubID.Text = ds.Tables[0].Rows[0][2].ToString();
                        BName.Text = ds.Tables[0].Rows[0][3].ToString();
                        Sales.Text = ds.Tables[0].Rows[0][4].ToString();
                    }
                    #endregion
                    #region 通过书籍信息中的AID获取作者信息，并绑定到TextBox
                    using (SqlCommand cmd = new SqlCommand("SELECT AName,ANationality FROM Authors Where AID=@AID", sqlcn))
                    {
                        DataSet ds = new DataSet();
                        cmd.Parameters.Add(new SqlParameter("@AID", int.Parse(AID.Text)));
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);
                        AName.Text = ds.Tables[0].Rows[0][0].ToString();
                        ANationality.Text = ds.Tables[0].Rows[0][1].ToString();
                    }
                    #endregion
                    #region 通过书籍信息获取出版社信息，并绑定到TextBox
                    using (SqlCommand cmd = new SqlCommand("SELECT PubName,PubTele,PubAddr FROM Publishers Where PubID=@PubID", sqlcn))
                    {
                        DataSet ds = new DataSet();
                        cmd.Parameters.Add(new SqlParameter("@PubID", int.Parse(PubID.Text)));
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);
                        PubName.Text = ds.Tables[0].Rows[0][0].ToString();
                        PubTele.Text = ds.Tables[0].Rows[0][1].ToString();
                        PubAddr.Text = ds.Tables[0].Rows[0][2].ToString();
                    }
                    #endregion
                }
                #region 错误处理
                catch (Exception ex)
                {
                    var message = new myMessageBox("出现错误！" + Environment.NewLine + "详细信息" + ex.Message, "警告");
                    message.ShowDialog();
                    ParentWindow.frmMain.Navigate(Res);
                }
                #endregion
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => ParentWindow.frmMain.Navigate(Res);

        private void Update_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            #region 删除当前图书记录
            using (SqlConnection sqlcn = new SqlConnection(Config.SqlCredentials))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM BOOKS Where ISBN=@ISBN", sqlcn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@ISBN", long.Parse(ISBN.Text)));
                        sqlcn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    myMessageBox myMessageBox = new myMessageBox("删除记录成功", "提示");
                    myMessageBox.ShowDialog();
                    ParentWindow.frmMain.Navigate(new Index(ParentWindow));
                }
                #endregion
                #region 出错时的处理
                catch (Exception excp)
                {
                    myMessageBox messageBox = new myMessageBox("出现错误！" + Environment.NewLine + "详细信息：" + excp.Message, "警告");
                    messageBox.ShowDialog();
                }
                #endregion
            }


        }
    }
}
