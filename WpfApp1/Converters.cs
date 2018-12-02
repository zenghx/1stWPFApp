using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml;

namespace WpfApp1
{
    class getURL : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "http://localhost:8080/ReportServer/Pages/ReportViewer.aspx?%2f%E6%8A%A5%E8%A1%A8%E9%A1%B9%E7%9B%AE1%2fReport1&rs:Command=Render";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return UserInfo.UsrGroup != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (UserInfo.UsrGroup != 0)
                return "Hidden";
            return "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class AuthorsPresentationConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return "[" + values[0].ToString().Replace(" ", "") + "]" + values[1].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class CountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "共查找到" + value.ToString() + "条结果";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class ISBN2Imgage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.douban.com/book/subject/isbn/" + value.ToString());
                var response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string xmlUrl = reader.ReadToEnd();
                reader.Close();
                response.Close();
                //实例Xml文档
                XmlDocument xmlDoc = new XmlDocument();
                //从指定字符串载入xml文档
                xmlDoc.LoadXml(xmlUrl);
                //实例解析、加入并移除集合的命名空间及范围管理
                XmlNamespaceManager xmlNM = new XmlNamespaceManager(xmlDoc.NameTable);
                //将给定命名空间加入到集合
                xmlNM.AddNamespace("db", "http://www.w3.org/2005/Atom");
                //获取文档根元素
                XmlElement root = xmlDoc.DocumentElement;
                //选择匹配Xpath(内容)表达式的结点列表
                //函数原型:SelectNodes(string xpath,XmlNamespaceManger nsmgr)
                XmlNodeList nodes = root.SelectNodes("/db:entry", xmlNM);

                //获取子节点信息
                foreach (XmlNode nodeData in nodes)
                {
                    foreach (XmlNode childnode in nodeData.ChildNodes)
                    {
                        string str = childnode.Name;
                        if (str == "link")
                        {
                            if (childnode.Attributes[1].Value == "image")
                            {
                                //获取image路径 <link rel="image" href="http://xxx.jpg"/>
                                string imagePath = childnode.Attributes[0].Value;
                                return imagePath;
                                //下载图片
                                /* string imageName = "local.jpg";
                                 System.Net.WebClient client = new System.Net.WebClient();
                                 client.DownloadFile(imagePath, imageName);
                                 //从本地文件里载入图片
                                 //下载指定URL资源到本地目录
                                 //函数原型 DownloadFile(string address,string fileName)
                                 break;*/
                            }
                        }
                    }
                }
                return "/Resources/Images/noImg.jpg";
            }
            catch (Exception)
            {

                return "/Resources/Images/noImg.jpg";
            }

            
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
