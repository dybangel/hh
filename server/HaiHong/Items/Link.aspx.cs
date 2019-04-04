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
    public partial class Link : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "redirect": toRedirect(); Response.End(); break;
                default: this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 跳转到指定的网址去
        /// </summary>
        protected void toRedirect()
        {
            try
            {
                string Id = RequestHelper.GetRequest("linksId").toString();
                if (Id == "0") { this.ErrorMessage("请求参数错误！"); Response.End(); }
                DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Link, Params: " and Id = " + Id + "");
                if (cRs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
                int Hits = new Fooke.Function.String(cRs["OnClick"].ToString()).cInt();
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary["onClick"] = (Hits + 1).ToString();
                DbHelper.Connection.Update(TableCenter.Link, dictionary, Params: " and Id = " + cRs["Id"] + "");
                string toUrl = cRs["siteURL"].ToString();
                if (!toUrl.ToLower().Contains("http")) { toUrl = "http://" + toUrl; }
                Response.Redirect(toUrl);
                Response.End();
            }
            catch { }
        }

        /// <summary>
        /// 单页默认页面
        /// </summary>
        protected void strDefault()
        {

        }
    }
}