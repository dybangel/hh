using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using Fooke.Code;
using Fooke.Function;
using Fooke.SimpleMaster;
namespace Fooke.Web
{
    public partial class List : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                default: strDefault(); Response.End(); break;
            }
        }

        protected void strDefault()
        {
            string ListID = RequestHelper.GetRequest("classId").toInt();
            string Identify = RequestHelper.GetRequest("identify").toString();
            if (ListID == "0" && string.IsNullOrEmpty(Identify)) { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            DataRow ListRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"ClassID",ListID},
                {"Identify",Identify}
            });
            if (ListRs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            if (ListRs["isDisplay"].ToString() != "1") { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            DataRow channelRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"channelID",ListRs["channelId"].ToString()}
            });
            if (channelRs == null) { this.ErrorMessage("对不起，你查找的页面不存在！"); Response.End(); }
            if (channelRs["isDisplay"].ToString() != "1") { this.ErrorMessage("越权操作！"); Response.End(); }
            /*****************************************************************************************************************
             * 开始加载模板解析内容
             * ***************************************************************************************************************/
            string cTemplate = ListRs["Template"].ToString();
            if (string.IsNullOrEmpty(cTemplate)) { cTemplate = "{@dir}/index.html"; }
            string TemplateDirectory = this.GetParameter("TemplateDir", "siteXML").toString();
            if (string.IsNullOrEmpty(TemplateDirectory)) { TemplateDirectory = "template"; }
            TemplateDirectory = Win.ApplicationPath + "/" + TemplateDirectory;
            cTemplate = cTemplate.Replace("{@dir}", TemplateDirectory);
            /*****************************************************************************************************************
            * 解析网页内容
            * ***************************************************************************************************************/
            Fooke.Release.ReleaseHelper ReleaseMaster = new Release.ReleaseHelper();
            string strReader = ReleaseMaster.ReleaseList(cTemplate, ListRs, channelRs);
            /*****************************************************************************************************************
             * 输出数据处理结果
             * ***************************************************************************************************************/
            Response.Write(strReader);
            Response.End();
        }
    }
}