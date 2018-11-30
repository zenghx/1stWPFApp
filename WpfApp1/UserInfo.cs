using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{ 
    static class UserInfo
    {
        public static string UsrName { set; get; }
        public static int UsrGroup { set; get; }
    }

    static class Config
    {
        public static string SqlCredentials = "Server=ZENGHX-LAPTOP\\SQLEXPRESS;Integrated Security=SSPI;database=Chart";
    }
}
