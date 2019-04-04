using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Fooke.Function;
namespace Fooke.Web.API
{
    public partial class Application : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /************************************************************************************
             * 拉取管理员登录信息
             * ***********************************************************************************/
            string token = RequestHelper.GetRequest("token").toString();
            if (string.IsNullOrEmpty(token)) { Response.Write("false"); Response.End(); }
            if (token.Length != 32) { Response.Write("拉取管理员信息失败，请重新登录！"); Response.End(); }
            DataRow AdminRs = DbHelper.Connection.ExecuteFindRow("Stored_AdminLogin", new Dictionary<string, object>() {
                {"strKey",token},{"strip",FunctionCenter.GetCustomerIP()}
            });
            if (AdminRs == null) { Response.Write("请求参数错误,请重新登陆！"); Response.End(); }
            if (AdminRs["isLock"].ToString() == "1") { Response.Write("该帐号已被锁定！请联系超级管理员！"); Response.End(); }
            /************************************************************************************
             * 开始处理网页数据
             * ***********************************************************************************/
            switch (strRequest)
            {
                case "all": ApplicationClearAll(); Response.End(); break;
                case "key": ApplicationClearKey(); Response.End(); break;
            }
        }
        /// <summary>
        /// 清空所有的缓存
        /// </summary>
        protected void ApplicationClearAll()
        {

            try { Application.RemoveAll(); }
            catch { Application.Clear(); }
            Response.Write("ok"); Response.End();
        }
        /// <summary>
        /// 清空指定的缓存
        /// </summary>
        protected void ApplicationClearKey()
        {
            string thisKey = RequestHelper.GetRequest("thisKey").toString();
            if (string.IsNullOrEmpty(thisKey)) { Response.Write("请求参数错误！"); Response.End(); }
            try { if (BufferHelper.Exists(thisKey)) { BufferHelper.Delete(thisKey); } }
            catch { Application.Clear(); }
            Response.Write("ok"); Response.End();
        }
    }
}