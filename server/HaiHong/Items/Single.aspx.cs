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
    public partial class Single : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (strRequest == "default") { this.strDefault(); Response.End(); }
        }
        /// <summary>
        /// 单页默认页面
        /// </summary>
        protected void strDefault()
        {
            /********************************************************************************************
             * 加载单页数据
             * ******************************************************************************************/
            string Identify = RequestHelper.GetRequest("Identify").toString();
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0" && string.IsNullOrEmpty(Identify)) { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            DataRow SingleRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindSingle]", new Dictionary<string, object>() {
                {"SignleId",Id},
                {"Identify",Identify}
            });
            if (SingleRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            if (SingleRs["isDisplay"].ToString() != "1") { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /********************************************************************************************
             * 编译模板地址
             * ******************************************************************************************/
            string strTemplate = SingleRs["cTemplate"].ToString();
            string TemplateDirectory = this.GetParameter("TemplateDir", "siteXML").toString();
            if (string.IsNullOrEmpty(TemplateDirectory)) { TemplateDirectory = "template"; }
            TemplateDirectory = Win.ApplicationPath + "/" + TemplateDirectory;
            strTemplate = strTemplate.Replace("{@dir}", TemplateDirectory);
            /********************************************************************************************
             * 解析模板内容
             * ******************************************************************************************/
            Fooke.Release.ReleaseHelper ReleaseMaster = new Release.ReleaseHelper();
            string strReader = ReleaseMaster.ReleaseSingle(strTemplate, SingleRs);
            /********************************************************************************************
             * 输出网页内容
             * ******************************************************************************************/
            Response.Write(strReader);
            Response.End();
        }

        
    }
}