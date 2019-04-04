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
namespace Fooke.Member
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
            string SingleId = RequestHelper.GetRequest("Id").toInt();
            if (SingleId == "0" && string.IsNullOrEmpty(Identify)) { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            DataRow SingleRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindSingle]", new Dictionary<string, object>() {
                {"SignleId",SingleId},
                {"Identify",Identify}
            });
            if (SingleRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            else if (SingleRs["isDisplay"].ToString() != "1") { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /************************************************************************************************************
             * 输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/single/single.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = SingleRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }
}