using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using Fooke.Code;
using Fooke.Function;
namespace Fooke.Web.Member
{
    public partial class Amount : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                default: this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 显示我的地址信息
        /// </summary>
        protected void strDefault()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/amount/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    case "amount": strValue = new Fooke.Function.String(MemberRs["Amount"].ToString()).cDouble().ToString("0.00"); break;
                    case "default": if (RequestHelper.GetRequest("type").toString("default") == "default") { strValue = " class=\"current\""; }; break;
                    case "alipay": if (RequestHelper.GetRequest("type").toString() == "alipay") { strValue = " class=\"current\""; }; break;
                    case "share": if (RequestHelper.GetRequest("type").toString() == "share") { strValue = " class=\"current\""; }; break;
                    case "duty": if (RequestHelper.GetRequest("type").toString() == "duty") { strValue = " class=\"current\""; }; break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }
}