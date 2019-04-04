using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using Fooke.Code;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fooke.Function;
namespace Fooke.Web.API
{
    public partial class Often : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "notice": thisNotice(); Response.End(); break;
                case "config": thisConfig(); Response.End(); break;
                case "configName": FindConfigName(); Response.End(); break;
                case "share": shareXml(); Response.End(); break;
                case "advert": GetAdvert(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }

        /// <summary>
        /// 获取广告位列表信息
        /// </summary>
        protected void GetAdvert()
        {
            StringBuilder strBuilder = new StringBuilder();
            /*******************************************************************************************
             * 开始执行数据查询
             * *****************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindAdvertToList]", new Dictionary<string, object>() { });
            if (thisTab == null) { Response.Write("{}"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { Response.Write("{}"); Response.End(); }
            strBuilder.Append("{\"success\":\"true\"");
            string demoId = "0";
            foreach (DataRow cRs in thisTab.Rows)
            {
                if (demoId != cRs["AdvId"].ToString())
                {
                    strBuilder.Append(",\"" + cRs["advertName"] + "\":[");
                    int SelectedIndex = 0;
                    foreach (DataRow sRs in thisTab.Rows)
                    {
                        if (sRs["AdvId"].ToString() == cRs["Advid"].ToString())
                        {
                            if (SelectedIndex != 0) { strBuilder.Append(","); }
                            strBuilder.Append("{");
                            strBuilder.Append("\"advid\":\"" + sRs["AdvID"] + "\"");
                            strBuilder.Append(",\"advertname\":\"" + sRs["AdvertName"] + "\"");
                            strBuilder.Append(",\"modals\":\"" + sRs["Modals"] + "\"");
                            strBuilder.Append(",\"listId\":\"" + sRs["ID"] + "\"");
                            strBuilder.Append(",\"thumb\":\"" + sRs["Thumb"] + "\"");
                            strBuilder.Append(",\"strLink\":\"" + sRs["strLink"] + "\"");
                            strBuilder.Append(",\"title\":\"" + sRs["title"] + "\"");
                            strBuilder.Append("}");
                            SelectedIndex = SelectedIndex + 1;
                        }
                    }
                    strBuilder.Append("]");
                }
                demoId = cRs["AdvId"].ToString();
            }
            strBuilder.Append("}");
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取分享文本内容
        /// </summary>
        protected void shareXml()
        {

            /***************************************************************************************
             * 获取数据,设置默认值
             * *************************************************************************************/
            string AndroidText = this.GetParameter("AndroidText", "InviteXml").ToString();
            if (string.IsNullOrEmpty(AndroidText)) { AndroidText = "我发现一款可以赚钱的APP,赶快来玩吧"; }
            string AndroidThumb = this.GetParameter("AndroidThumb", "InviteXml").ToString();
            if (string.IsNullOrEmpty(AndroidThumb)) { AndroidThumb = string.Format("{0}/share.png", FunctionCenter.SiteUrl()); }
            string iOSText = this.GetParameter("iOSText", "InviteXml").ToString();
            if (string.IsNullOrEmpty(iOSText)) { iOSText = "我发现一款可以赚钱的APP,赶快来玩吧"; }
            string iOSThumb = this.GetParameter("iOSThumb", "InviteXml").ToString();
            if (string.IsNullOrEmpty(iOSThumb)) { iOSThumb = string.Format("{0}/share.png", FunctionCenter.SiteUrl()); }
            string Wechat = this.GetParameter("Wechat", "InviteXml").ToString();
            /***************************************************************************************
             * 输出数据结果信息
             * *************************************************************************************/
            StringBuilder strXml = new StringBuilder();
            strXml.Append("{");
            strXml.AppendFormat("\"success\":\"true\"");
            strXml.AppendFormat(",\"tips\":\"请求成功\"");
            strXml.AppendFormat(",\"androidtext\":\"{0}\"", AndroidText);
            strXml.AppendFormat(",\"androidthumb\":\"{0}\"", AndroidThumb);
            strXml.AppendFormat(",\"iostext\":\"{0}\"", iOSText);
            strXml.AppendFormat(",\"iosthumb\":\"{0}\"", iOSThumb);
            strXml.AppendFormat(",\"wechat\":\"{0}\"", Wechat);
            strXml.Append("}");
            Response.Write(strXml.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取指定数据的配置参数
        /// </summary>
        protected void FindConfigName()
        {
            /************************************************************************************
             * 获取参数配置信息
             * **********************************************************************************/
            string configName = RequestHelper.GetRequest("configName").toString();
            if (string.IsNullOrEmpty(configName)) { this.ErrorMessage("请输入配置项的名称！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Config, columns: configName, Params: string.Empty);
            if (cRs == null) { this.ErrorMessage("请检查配置项名称错误！"); Response.End(); }
            string strName = RequestHelper.GetRequest("strName").toString();
            if (string.IsNullOrEmpty(strName)) { this.ErrorMessage("请上传配置节点的名称！"); Response.End(); }
            /************************************************************************************
             * 遍历数据结果
             * **********************************************************************************/
            ConfigurationReader xReader = new ConfigurationReader(cRs[0].ToString());
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\"success\":\"true\"");
            foreach (string oName in strName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                strBuilder.AppendFormat(",\"{0}\":\"{1}\"", oName, xReader.GetParameter(oName));
            }
            strBuilder.Append("}");
            /************************************************************************************
             * 输出数据结果
             * **********************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取系统参数配置
        /// </summary>
        protected void thisConfig()
        {
            /************************************************************************************
             * 获取请求参数配置信息
             * **********************************************************************************/
            string configName = RequestHelper.GetRequest("configName").toString();
            if (string.IsNullOrEmpty(configName)) { configName = "siteXml"; }
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            foreach (KeyValuePair<string, string> Pari in this.Configure.GetDictionary(configName))
            {
                strBuilder.AppendFormat(",\"{0}\":\"{1}\"", Pari.Key.ToLower(), Pari.Value);
            }
            strBuilder.Append("}");
            /************************************************************************************
             * 输出数据处理结果
             * **********************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取系统公告
        /// </summary>
        protected void thisNotice()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"" + this.GetParameter("notice", "noticeXml").toString() + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 基本信息配置
        /// </summary>
        protected void strDefault()
        {
            string DeviceType = RequestHelper.GetRequest("DeviceType").toString();
            if (string.IsNullOrEmpty(DeviceType)) { DeviceType = "Android"; }
            StringBuilder strXml = new StringBuilder();
            try
            {
                string iOSName = this.GetParameter(DeviceType + "Name", "appXml").toString();
                string iOSDeposit = this.GetParameter(DeviceType + "Deposit", "appXml").toString();
                string iOSEdition = this.GetParameter(DeviceType + "Edition", "appXml").toString();
                string iOSport = this.GetParameter(DeviceType + "port", "appXml").toString();
                string iOSPackerName = this.GetParameter(DeviceType + "PackerName", "appXml").toString();
                strXml.Append("{\"success\":\"true\"");
                strXml.Append(",\"name\":\"" + iOSName + "\"");
                strXml.Append(",\"deposit\":\"" + iOSDeposit + "\"");
                strXml.Append(",\"edition\":\"" + iOSEdition + "\"");
                strXml.Append(",\"port\":\"" + iOSport + "\"");
                strXml.Append(",\"packerName\":\"" + iOSPackerName + "\"");
                strXml.Append("}");
            }
            catch { }
            Response.Write(strXml.ToString());
            Response.End();
        }
    }
}