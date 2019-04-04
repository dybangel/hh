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
    public partial class Index : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "start": StarDefault(); Response.End(); break;
                case "alipay": strAlipay(); Response.End(); break;
                case "saveFinger": SaveFinger(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }

        protected void StarDefault()
        {
            /**************************************************************************************
            * 获取请求参数信息
            * ************************************************************************************/
            string ParentID = RequestHelper.GetRequest("uid").toInt();
            string strKey = string.Format("邀请好友-|-|-{0}-|-|-{0}-|-|-邀请好友", ParentID);
            strKey = new Fooke.Function.String(strKey).ToMD5().Substring(0, 24).ToUpper();
            string Key = RequestHelper.GetRequest("key").toString();
            if (ParentID != "0" && string.IsNullOrEmpty(Key)) { this.ErrorMessage("获取请求参数信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0" && Key.Length <= 0) { this.ErrorMessage("获取请求参数信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0" && Key.Length != 24) { this.ErrorMessage("获取请求参数信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0" && Key != strKey) { this.ErrorMessage("获取请求参数信息失败,请重试！"); Response.End(); }
            /**************************************************************************************
            * 将获取到的邀请信息保存到Cookie
            * ************************************************************************************/
            try { CookieHelper.Add("FookeUID", ParentID, 90); }catch { }
            try { CookieHelper.Add("FookeKey", strKey, 90); }catch { }
            try { SessionHelper.Add("FookeUID", ParentID, 90); }catch { }
            /**************************************************************************************
            * 获取请求配置参数信息
            * ************************************************************************************/
            string strName = this.GetParameter("AndroidName", "appXml").toString();
            string strDeposit = this.GetParameter("AndroidDeposit", "appXml").toString();
            string strEdition = this.GetParameter("AndroidEdition", "appXml").toString();
            string strPackerName = this.GetParameter("AndroidPackerName", "appXml").toString();
            string strPort = this.GetParameter("AndroidPort", "appXml").toString();
            if (new BrowserHelper().Browser() == BrowserHelper.BrowserType.iOS)
            {
                strName = this.GetParameter("iOSName", "appXml").toString();
                strDeposit = this.GetParameter("iOSDeposit", "appXml").toString();
                strEdition = this.GetParameter("iOSEdition", "appXml").toString();
                strPackerName = this.GetParameter("iOSPackerName", "appXml").toString();
                strPort = this.GetParameter("iOSPort", "appXml").toString();
            }
            System.Text.StringBuilder cfgBuilder = new System.Text.StringBuilder();
            cfgBuilder.Append("<script language=\"javascript\">");
            cfgBuilder.Append("var thisConfig={");
            cfgBuilder.AppendFormat("\"{0}\":\"{1}\"", "sitename", this.GetParameter("sitename").toString());
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strName", strName);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strDeposit", strDeposit);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strEdition", strEdition);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strPackerName", strPackerName);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strPort", strPort);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "IsMicroMessenger", new BrowserHelper().IsMicroMessenger().ToString().ToLower());
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "IsQQ", new BrowserHelper().IsQQBrowser().ToString().ToLower());
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "Browser", new BrowserHelper().Browser().ToString());
            cfgBuilder.Append("};");
            cfgBuilder.Append("</script>");
            /**************************************************************************************
             * 开始加载网页内容
             * ************************************************************************************/
            if (new BrowserHelper().Browser() == BrowserHelper.BrowserType.iOS) 
            {
                SimpleMaster.SimpleMaster iOSMaster = new SimpleMaster.SimpleMaster();
                string iOSResponse = iOSMaster.Reader("template/start.html");
                iOSResponse = iOSMaster.Start(iOSResponse, new SimpleMaster.Function((funName) =>
                {
                    string strValue = string.Empty;
                    switch (funName)
                    {
                        case "config": strValue = cfgBuilder.ToString(); break;
                    }
                    return strValue;
                }), true);
                Response.Write(iOSResponse);
                Response.End();
            }
            /**************************************************************************************
             * 安卓手机或其他设备
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strReader = Master.Reader("template/android.html");
            strReader = Master.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "config": strValue = cfgBuilder.ToString(); break;
                }
                return strValue;
            }), true);
            Response.Write(strReader);
            Response.End();
        }

        protected void ShowFinger()
        {
            /**************************************************************************************
             * 开始加载网页内容
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strReader = Master.Reader("template/finger.html");
            strReader = Master.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName) { }
                return strValue;
            }), true);
            Response.Write(strReader);
            Response.End();
        }

        /// <summary>
        /// 显示默认数据信息
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
            * 获取请求参数信息
            * ************************************************************************************/
            string ParentID = RequestHelper.GetRequest("uid").toInt();
            string strKey = string.Format("邀请好友-|-|-{0}-|-|-{0}-|-|-邀请好友", ParentID);
            strKey = new Fooke.Function.String(strKey).ToMD5().Substring(0, 24).ToUpper();
            string Key = RequestHelper.GetRequest("key").toString();
            if (ParentID != "0" && string.IsNullOrEmpty(Key)) { this.ErrorMessage("获取请求参数信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0" && Key.Length <= 0) { this.ErrorMessage("获取请求参数信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0" && Key.Length != 24) { this.ErrorMessage("获取请求参数信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0" && Key != strKey) { this.ErrorMessage("获取请求参数信息失败,请重试！"); Response.End(); }
            /**************************************************************************************
            * 判断是否获取到网页指纹代码,如果没获取到则跳转到指纹代码界面
            * ************************************************************************************/
            string strFinger = RequestHelper.GetRequest("strFinger").toString();
            if (RequestHelper.GetRequest("Fingerprint").toString() != "false"
            && RequestHelper.GetRequest("uid").toInt() != "0"
            && !new BrowserHelper().IsMicroMessenger()
            && !new BrowserHelper().IsQQBrowser())
            {
                if (string.IsNullOrEmpty(strFinger)) { ShowFinger(); Response.End(); }
                else if (strFinger.Length <= 0) { ShowFinger(); Response.End(); }
                else if (strFinger.Length <= 5) { ShowFinger(); Response.End(); }
                else if (strFinger.Length >= 30) { ShowFinger(); Response.End(); }
            }
            /**************************************************************************************
            * 将获取到的邀请信息保存到Cookie
            * ************************************************************************************/
            try { CookieHelper.Add("FookeUID", ParentID, 90); }catch { }
            try { CookieHelper.Add("FookeKey", strKey, 90); } catch { }
            try { SessionHelper.Add("FookeUID", ParentID, 90); }
            catch { }
            /**************************************************************************************
            * 获取请求配置参数信息
            * ************************************************************************************/
            string strName = this.GetParameter("AndroidName", "appXml").toString();
            string strDeposit = this.GetParameter("AndroidDeposit", "appXml").toString();
            string strEdition = this.GetParameter("AndroidEdition", "appXml").toString();
            string strPackerName = this.GetParameter("AndroidPackerName", "appXml").toString();
            string strPort = this.GetParameter("AndroidPort", "appXml").toString();
            if (new BrowserHelper().Browser() == BrowserHelper.BrowserType.iOS)
            {
                strName = this.GetParameter("iOSName", "appXml").toString();
                strDeposit = this.GetParameter("iOSDeposit", "appXml").toString();
                strEdition = this.GetParameter("iOSEdition", "appXml").toString();
                strPackerName = this.GetParameter("iOSPackerName", "appXml").toString();
                strPort = this.GetParameter("iOSPort", "appXml").toString();
            }
            System.Text.StringBuilder cfgBuilder = new System.Text.StringBuilder();
            cfgBuilder.Append("<script language=\"javascript\">");
            cfgBuilder.Append("var thisConfig={");
            cfgBuilder.AppendFormat("\"{0}\":\"{1}\"", "sitename", this.GetParameter("sitename").toString());
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strName", strName);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strDeposit", strDeposit);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strEdition", strEdition);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strPackerName", strPackerName);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "strPort", strPort);
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "IsMicroMessenger", new BrowserHelper().IsMicroMessenger().ToString().ToLower());
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "IsQQ", new BrowserHelper().IsQQBrowser().ToString().ToLower());
            cfgBuilder.AppendFormat(",\"{0}\":\"{1}\"", "Browser", new BrowserHelper().Browser().ToString());
            cfgBuilder.Append("};");
            cfgBuilder.Append("</script>");
            /**************************************************************************************
             * 开始加载网页内容
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strReader = Master.Reader("template/default.html");
            strReader = Master.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "config": strValue = cfgBuilder.ToString(); break;
                }
                return strValue;
            }), true);
            Response.Write(strReader);
            Response.End();
        }

        /// <summary>
        /// 查询最新的提现信息
        /// </summary>
        protected void strAlipay()
        {
            /*****************************************************************************************
             * 构建查询语句条件
             * ***************************************************************************************/
            string strParams = "";
            /*****************************************************************************************
             * 获取分页显示数量
             * ***************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(20);
            /*****************************************************************************************
             * 构建分页查询数据表
             * ***************************************************************************************/
            System.Text.StringBuilder strTabs = new System.Text.StringBuilder();
            strTabs.Append("(");
            strTabs.Append("    select ");
            strTabs.Append("    ali.fokeMode,ali.alipayid,ali.thisAmount,ali.Addtime,foke.userid,foke.nickname,foke.username,foke.thumb");
            strTabs.Append("    from Fooke_Alipay as ali inner join fooke_user as foke");
            strTabs.Append("    on ali.userid = foke.userid");
            strTabs.Append(") as fokeApps");
            /*****************************************************************************************
             * 创建分页sql语句
             * ***************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "alipayid";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " alipayid desc";
            PageCenterConfig.Tablename = strTabs.toString();
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(strTabs.toString(), strParams);
            System.Data.DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            System.Data.DataRow cRs = DbHelper.Connection.FindRow(tablename: "Fooke_Alipay",
                columns: "isnull(sum(thisAmount)+13215716,0) as total",
                Params: " and 1=1");
            /**************************************************************************************
            * 输出网页内容信息
            * *************************************************************************************/
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"record\":\"" + Record + "\"");
            strBuilder.Append(",\"count\":\"" + cRs["total"].ToString() + "\"");
            strBuilder.Append(",\"result\":["); int SelectedIndex = 0;
            foreach (System.Data.DataRow sRs in Tab.Rows)
            {
                if (SelectedIndex != 0) { strBuilder.Append(","); }
                strBuilder.Append("{");
                strBuilder.Append("\"alipayid\":\"" + sRs["alipayid"] + "\"");
                strBuilder.Append(",\"thisamount\":\"" + sRs["thisAmount"] + "\"");
                strBuilder.Append(",\"fokemode\":\"" + sRs["fokeMode"] + "\"");
                strBuilder.Append(",\"userid\":\"" + sRs["userid"] + "\"");
                strBuilder.Append(",\"nickname\":\"" + sRs["nickname"] + "\"");
                strBuilder.Append(",\"username\":\"" + sRs["username"] + "\"");
                strBuilder.Append(",\"addtime\":\"" + new Fooke.Function.String(sRs["Addtime"].ToString()).DateToChinese(sRs["Addtime"].ToString()) + "\"");
                strBuilder.Append(",\"thumb\":\"" + sRs["thumb"] + "\"");
                strBuilder.Append("}");
                SelectedIndex = SelectedIndex + 1;
            }
            strBuilder.Append("]");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 保存邀请指纹识别信息
        /// </summary>
        protected void SaveFinger()
        {
            /****************************************************************************************
             * 获取扫码指纹数据数据信息
             * **************************************************************************************/
            string strFinger = RequestHelper.GetRequest("strFinger").toString();
            if (string.IsNullOrEmpty(strFinger)) { Response.Redirect("index.aspx?Fingerprint=false"); Response.End(); }
            else if (strFinger.Length <= 0) { Response.Redirect("index.aspx?Fingerprint=false"); Response.End(); }
            else if (strFinger.Length <= 5) { Response.Redirect("index.aspx?Fingerprint=false"); Response.End(); }
            else if (strFinger.Length >= 30) { Response.Redirect("index.aspx?Fingerprint=false"); Response.End(); }
            /****************************************************************************************
             * 获取用户分享邀请ID数据
             * **************************************************************************************/
            string ParentID = RequestHelper.GetRequest("uid").toInt();
            if (ParentID == "0") { ParentID = RequestHelper.GetRequest("ParentID").toInt(); }
            if (ParentID == "0") { ParentID = CookieHelper.Get("FookeUID").toInt(); }
            if (ParentID == "0") { ParentID = CookieHelper.Get("ParentID").toInt(); }
            if (ParentID == "0") { ParentID = SessionHelper.Get("FookeUID").toInt(); }
            /****************************************************************************************
             * 判断邀请ID请求数据是否合法,否则跳转到无邀请界面
             * **************************************************************************************/
            if (ParentID == "0") { Response.Redirect("index.aspx?Fingerprint=false"); Response.End(); }
            else if (new Fooke.Function.String(ParentID).cInt() <= 99999) { Response.Redirect("index.aspx?Fingerprint=false"); Response.End(); }
            /****************************************************************************************
             * 保存用户分享ID数据信息
             * **************************************************************************************/
            System.Data.DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_shareLogs]", new Dictionary<string, object>() {
               {"ParentID",ParentID},
               {"strFinger",strFinger}
            });
            if (sRs == null) { Response.Redirect("index.aspx?Fingerprint=false"); Response.End(); }
            /****************************************************************************************
             * 跳转到分享下载界面网页
             * **************************************************************************************/
            Response.Redirect(string.Format("index.aspx?uid={0}&key={1}&strFinger={2}",
                RequestHelper.GetRequest("uid").toInt(),
                RequestHelper.GetRequest("key").toString(),
                RequestHelper.GetRequest("strFinger").toString()));
            Response.End();
        }
    }
}