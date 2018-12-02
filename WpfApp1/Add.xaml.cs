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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Add.xaml 的交互逻辑
    /// </summary>
    public partial class Add : Window
    {
        public Add()
        {
            InitializeComponent();
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataSet ds = new DataSet();
            try
            {
                #region 强制要求ISBN为13位或10位
                if (ISBN.Text.Length != 13&&ISBN.Text.Length!=7)
                    throw new FormatException("ISBN长度有误！");
                #endregion
                #region 检查所填记录是否有存在的部分，不存在则ds对应的table里的值为0
                using (SqlConnection sqlcn = new SqlConnection(Config.SqlCredentials))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Books Where ISBN=@ISBN;Select COUNT(*) from Authors Where AID=@AID;Select COUNT(*) from Publishers Where PubID=@PubID", sqlcn))
                    {
                        #region 添加参数并将参数格式化为所需类型
                        cmd.Parameters.Add("ISBN", SqlDbType.BigInt);
                        cmd.Parameters.Add("AID", SqlDbType.Int);
                        cmd.Parameters.Add("PubID", SqlDbType.Int);
                        cmd.Parameters[0].Value = long.Parse(ISBN.Text);
                        cmd.Parameters[1].Value = int.Parse(AID.Text);
                        cmd.Parameters[2].Value = int.Parse(PubID.Text);
                        #endregion
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        sqlcn.Open();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);
                        var d = ds;
                    }
                    #endregion
                #region 根据检查结果决定是否对每个表插入新数据
                    if (ds.Tables[1].Rows[0][0].ToString() == "0")
                        using (SqlCommand cmd = new SqlCommand("Insert into Publishers(PubID,PubName,PubTele,PubAddr) Values(@PubID,@PubName,@PubTele,@PubAddr) ", sqlcn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@PubID", int.Parse(PubID.Text)));
                            cmd.Parameters.Add(new SqlParameter("@PubName", PubName.Text));
                            cmd.Parameters.Add(new SqlParameter("@PubTele", PubTele.Text));
                            cmd.Parameters.Add(new SqlParameter("@PubAddr", PubAddr.Text));
                            cmd.ExecuteNonQuery();
                        }
                    if (ds.Tables[1].Rows[0][0].ToString() == "0")
                        using (SqlCommand cmd = new SqlCommand("Insert into Authors(AID,AName,ANationality) Values(@AID,@AName,@ANationality)", sqlcn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@AID", int.Parse(AID.Text)));
                            cmd.Parameters.Add(new SqlParameter("@AName", AName.Text));
                            cmd.Parameters.Add(new SqlParameter("@ANationality", ANationality.Text));
                            cmd.ExecuteNonQuery();
                        }
                    //由于外键约束要把对另两个表的操作放在Books表前
                    if (ds.Tables[0].Rows[0][0].ToString()=="0")
                        using (SqlCommand cmd = new SqlCommand("Insert into Books(ISBN,PubID,AID,BName,Sales) Values(@ISBN,@PubID,@AID,@BName,@Sales ) ", sqlcn))
                        {
                            cmd.Parameters.Add(new SqlParameter("@ISBN",long.Parse(ISBN.Text)));
                            cmd.Parameters.Add(new SqlParameter("@PubID",int.Parse(PubID.Text)));
                            cmd.Parameters.Add(new SqlParameter("@AID",int.Parse(AID.Text)));
                            cmd.Parameters.Add(new SqlParameter("BName", BName.Text));
                            cmd.Parameters.Add(new SqlParameter("@Sales", int.Parse(Sales.Text)));
                            cmd.ExecuteNonQuery();
                        }
                }
                #endregion
                #region 成功提示并关闭窗口
                myMessageBox myMessageBox = new myMessageBox("添加记录成功", "提示");
                myMessageBox.ShowDialog();
                Close();
                #endregion
            }
            catch (Exception excp)
            {
                myMessageBox messageBox = new myMessageBox("出现错误！"+Environment.NewLine+"详细信息："+excp.Message, "警告");
                messageBox.ShowDialog();
            }


        }
    }
}
