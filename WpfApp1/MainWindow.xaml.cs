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
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            #region 从注册表里读取Windows版本号，低于Windows10 1803版本则报错
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion");
            var BuildNumber = registryKey.GetValue("CurrentBuildNumber").ToString();
            var build = Convert.ToInt32(BuildNumber);
            if (build < 17134)
            {
                OSWarning warning = new OSWarning(this);
                warning.ShowDialog();
            }
            #endregion
            InitializeComponent();
        }
        #region 按钮功能，通过认证则进入主窗口，否则弹出消息框
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PassedAuthentication())
                {
                    Window1 window1 = new Window1();
                    window1.Show();
                    Close();
                }
                else
                {
                    myMessageBox myMessage = new myMessageBox("用户名或密码无效", "提示");
                    myMessage.ShowDialog();
                }
            }
            catch (Exception excp)
            {

                myMessageBox messageBox = new myMessageBox("出现错误！" + Environment.NewLine + "详细信息：" + excp.Message, "警告");
                messageBox.ShowDialog();
            }

        }
        #endregion
        #region 从数据库中获取用户信息并进行验证，通过则返回true，并保存用户名和用户组
        private Boolean PassedAuthentication()
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlcn = new SqlConnection(Config.SqlCredentials))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT GUID,usrName,pswHash,usrGrp FROM Users Where usrName=@uName", sqlcn))
                {
                    cmd.Parameters.Add("uName", SqlDbType.NChar);
                    cmd.Parameters[0].Value = textBox.Text;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    sqlcn.Open();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(ds,"Users");
                    if (ds.Tables["Users"].Rows.Count != 0)
                    {
                        var hash = ds.Tables["Users"].Rows[0]["pswHash"].ToString();
                        var salt = ds.Tables["Users"].Rows[0]["GUID"].ToString();
                        byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(passwBox.Password + salt);
                        byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordAndSaltBytes);
                        string hashString = Convert.ToBase64String(hashBytes);
                        if (hashString == hash)
                        {
                            UserInfo.UsrName = ds.Tables["Users"].Rows[0]["usrName"].ToString();
                            UserInfo.UsrGroup = int.Parse(ds.Tables["Users"].Rows[0]["usrGrp"].ToString());
                            return true;
                        }
                            
                    }
                    return false;
                }
            }
        }
        #endregion
    }
}

