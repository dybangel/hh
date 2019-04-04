using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fooke.Code;
using Fooke.Function;
using Fooke.SimpleMaster;
namespace Fooke.Web
{
    public partial class Identifier : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (RequestHelper.GetRequest("action").toString() == "do")
            {
                /********************************************************************************************************************************
                 * 获取网页参数信息
                 * ******************************************************************************************************************************/
                string strResponse = string.Empty;
                using (System.IO.Stream stream = System.Web.HttpContext.Current.Request.InputStream)
                {
                    try
                    {
                        byte[] requestByte = new byte[stream.Length];
                        stream.Read(requestByte, 0, (int)stream.Length);
                        strResponse = System.Text.Encoding.GetEncoding("UTF-8").GetString(requestByte);
                    }
                    finally { stream.Dispose(); }
                }
                /********************************************************************************************************************************
                 * 声明UDID字段信息
                 * ******************************************************************************************************************************/
                string UniqueKey = string.Empty;
                try
                {
                    System.Text.RegularExpressions.Regex Rex = new System.Text.RegularExpressions.Regex(@"<key>UDID</key>(?<Text>(?s)(.*?)(?s))<key>VERSION</key>",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);

                    if (Rex.IsMatch(strResponse))
                    {
                        UniqueKey = (Rex.Match(strResponse).Groups["Text"].ToString());
                        UniqueKey = UniqueKey.Replace(System.Environment.NewLine, "");
                        UniqueKey = UniqueKey.Replace("\r\n", "");
                        UniqueKey = UniqueKey.Replace("\n", "");
                        UniqueKey = UniqueKey.Replace("\t", "");
                        UniqueKey = UniqueKey.Replace("<string>", "");
                        UniqueKey = UniqueKey.Replace("</string>", "");
                        UniqueKey = UniqueKey.Replace(" ", "");
                    }
                }
                catch { }
                /********************************************************************************************************************************
                 * 输出数据处理结果
                 * ******************************************************************************************************************************/
                HttpContext.Current.Response.StatusCode = 301;
                HttpContext.Current.Response.Status = "301 Moved Permanently";
                HttpContext.Current.Response.AddHeader("Location", "Identifier.aspx?action=a&udid=" + UniqueKey);
                HttpContext.Current.Response.End();
            }
            else if (RequestHelper.GetRequest("action").toString() == "a")
            {
                Response.Write("<script language=\"javascript\">");
                Response.Write("window.location=\"" + this.GetParameter("iOSPackerName", "appXml") + "?udid:" + RequestHelper.GetRequest("udid") + "\"");
                Response.Write("</script>");
                Response.End();
            }
        }
    }
}