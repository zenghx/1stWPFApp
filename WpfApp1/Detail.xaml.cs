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
        private int oldAID { set; get; }
        private int oldPubID { set; get; }
        private SearchRes Res { set; get; }
        private Window1 ParentWindow { get; set; }
        private HashSet<TextBox> ChangedTextBox { get; set; }
        private int TextBoxChangedCounter { get; set; }

        public Detail(string ISBN,SearchRes res,Window1 window)
        {
            _ISBN = ISBN;
            Res = res;
            ParentWindow = window;
            ChangedTextBox = new HashSet<TextBox>();
            TextBoxChangedCounter = 0;
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
                        oldAID = int.Parse(AID.Text);
                        oldPubID = int.Parse(PubID.Text);
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
                    var message = new myMessageBox("出现错误！" + Environment.NewLine + "详细信息：" + ex.Message, "警告");
                    message.ShowDialog();
                    ParentWindow.frmMain.Navigate(Res);
                }
                #endregion
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => ParentWindow.frmMain.Navigate(Res);

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlcn = new SqlConnection(Config.SqlCredentials))
                try
                {
                    sqlcn.Open();
                    foreach (TextBox textBox in ChangedTextBox)
                    {
                        #region 被修改内容为Authors表中内容，若PubID被修改，则级联修改到Books表
                        if (textBox.Name.StartsWith("A"))
                        {
                            using (SqlCommand cmd = new SqlCommand("UPDATE Authors SET "+textBox.Name+"=@value WHERE AID=@oldAID", sqlcn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@oldAID", oldAID));
                                if (textBox.Name == "AID")
                                    cmd.Parameters.Add(new SqlParameter("@value", int.Parse(textBox.Text)));
                                else
                                    cmd.Parameters.Add(new SqlParameter("@value", textBox.Text));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        #endregion
                        #region 被修改内容为Publishers表中内容，若PubID被修改，则级联修改到Books表
                        else if (textBox.Name.StartsWith("Pub"))
                        {
                            using (SqlCommand cmd = new SqlCommand("UPDATE Publishers SET "+textBox.Name+ "=@value WHERE PubID=@oldPubID", sqlcn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@oldPubID", oldPubID));
                                if (textBox.Name == "PubID")
                                    cmd.Parameters.Add(new SqlParameter("@value", int.Parse(textBox.Text)));
                                else
                                    cmd.Parameters.Add(new SqlParameter("@value", textBox.Text));
                                cmd.ExecuteNonQuery();
                            }
                        }
                        #endregion
                        #region 被修改内容为Books表中非外键内容
                        else
                        {
                               using (SqlCommand cmd = new SqlCommand("UPDATE Books SET "+textBox.Name+"=@value WHERE ISBN=@oldISBN", sqlcn))
                               {
                                cmd.Parameters.Add(new SqlParameter("@oldISBN", long.Parse(_ISBN)));
                                if (textBox.Name == "ISBN")
                                    if (textBox.Text.Length != 13 && textBox.Text.Length != 10)
                                        throw new FormatException("ISBN长度有误");
                                    else cmd.Parameters.Add(new SqlParameter("@value", long.Parse(textBox.Text)));
                                else if (textBox.Name == "Sales")
                                    cmd.Parameters.Add(new SqlParameter("@value", int.Parse(textBox.Text)));
                                else
                                    cmd.Parameters.Add(new SqlParameter("@value", textBox.Text));
                                    cmd.ExecuteNonQuery();
                                }
                        }
                        #endregion
                    }
                    var message = new myMessageBox("修改成功！", "提示");
                    message.ShowDialog();
                    ParentWindow.frmMain.Navigate(new Index(ParentWindow));
                }
                #region 错误处理
                catch (Exception excp)
                {
                    myMessageBox messageBox = new myMessageBox("出现错误！" + Environment.NewLine + "详细信息：" + excp.Message, "警告");
                    messageBox.ShowDialog();
                }
                #endregion
        }
        #region 删除记录
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
        #endregion
        #region 获取被修改过的TextBox
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxChangedCounter++;
            if (TextBoxChangedCounter > 10)
                ChangedTextBox.Add((TextBox)sender);
            else return;
        }
        #endregion
    }
}
