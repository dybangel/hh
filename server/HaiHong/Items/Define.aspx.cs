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
    public partial class Define : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (strRequest == "default") { this.strDefault(); Response.End(); }
        }
        /// <summary>
        /// 自定义界面默认页
        /// </summary>
        protected void strDefault()
        {
            try
            {
                /***********************************************************************
                 * 获取系统目录地址
                 * **********************************************************************/
                string TemplateDirectory = this.GetParameter("TemplateDir", "siteXML").toString();
                if (string.IsNullOrEmpty(TemplateDirectory)) { TemplateDirectory = "template"; }
                TemplateDirectory = Win.ApplicationPath + "/" + TemplateDirectory;
                /***********************************************************************
                 * 开始加载自定义模版数据
                 * **********************************************************************/
                string define = RequestHelper.GetRequest("define").toString();
                if (string.IsNullOrEmpty(define)) { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
                string sPath = TemplateDirectory + "/define/";
                string strTemplate = sPath + define + ".html";
                if (!System.IO.File.Exists(FunctionCenter.ServerPath(strTemplate))) { this.ErrorMessage("对不起，你查找的页面不存在！"); Response.End(); }
                SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
                string strReader = Fooke.Reader(strTemplate);
                strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
                {
                    string strValue = string.Empty;
                    return strValue;
                }), isLabel: true);
                Response.Write(strReader);
                Response.End();
            }
            catch { }
        }

        
    }
}