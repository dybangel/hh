using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Fooke.Code;
using Fooke.Function;
namespace Fooke.Web.Admin
{
    public partial class Login : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "exit": LoginOut(); Response.End(); break;
                case "chk": AdminLogin(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 默认登录页
        /// </summary>
        protected void strDefault()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/login.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {

                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        protected void LoginOut()
        {
            /************************************************************************************
             * 拉取管理员登录信息
             * ***********************************************************************************/
            string Token = CookieHelper.Get("fooke_admin_token").toString();
            if (string.IsNullOrEmpty(Token)) { this.ErrorMessage("拉取管理员信息失败，请重新登录！"); System.Web.HttpContext.Current.Response.End(); }
            if (Token.Length != 32) { this.ErrorMessage("拉取管理员信息失败，请重新登录！"); System.Web.HttpContext.Current.Response.End(); }
            DataRow AdminRs = DbHelper.Connection.ExecuteFindRow("Stored_AdminLogin", new Dictionary<string, object>() {
                {"strKey",Token},{"strip",FunctionCenter.GetCustomerIP()}
            });
            if (AdminRs == null) { this.ErrorMessage("拉取管理员信息失败,请重新登录！"); System.Web.HttpContext.Current.Response.End(); }
            /********************************************************************
             * 保存Token日志记录
             * ******************************************************************/
            CookieHelper.Add("fooke_admin_token", "", 90);
            SessionHelper.Add("fooke_admin_token", "", 90);
            CookieHelper.Delete("fooke_admin_token");
            Response.Redirect("Login.aspx"); Response.End();
        }
        /// <summary>
        /// 检查管理员登录信息
        /// </summary>
        protected void AdminLogin()
        {
            string AdminName = RequestHelper.GetRequest("adminName").toString();
            if (string.IsNullOrEmpty(AdminName)) { this.JSONMessage("请输入管理员帐号！"); Response.End(); }
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { this.JSONMessage("请输入管理员登录密码！"); Response.End(); }
            string verifycode = RequestHelper.GetRequest("verifycode").toString();
            if (string.IsNullOrEmpty(verifycode)) { this.JSONMessage("请输入验证码！"); Response.End(); }
            string SessionCode = SessionHelper.Get("vCode").toString();
            if (string.IsNullOrEmpty(SessionCode)) { SessionCode = CookieHelper.Get("vCode").toString(); }
            if (!string.IsNullOrEmpty(SessionCode) && SessionCode.ToLower() != verifycode.ToLower()) { this.JSONMessage("登录验证码错误！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow("Fooke_Admin", Params: " and Adminname='" + AdminName + "'");
            if (cRs == null) { this.JSONMessage("帐号或者密码错误！"); Response.End(); }
            Password = new Fooke.Function.String(Password).ToMD5().ToMD5().Substring(0, 20).ToLower();
            if (cRs["password"].ToString().ToLower() != Password) { this.JSONMessage("帐号或者密码错误！"); Response.End(); }
            if (cRs["isLock"].ToString() != "0") { this.JSONMessage("当前帐号已被超级管理员锁定！"); Response.End(); }
            string strKey = "管理员登录-|-|-" + cRs["AdminID"].ToString() + "-|-|-";
            if (cRs["isMary"].ToString() == "0") { strKey += DateTime.Now.Ticks; }
            strKey = new Fooke.Function.String(strKey).ToMD5().ToLower();
            /********************************************************************
             * 保存Token日志记录
             * ******************************************************************/
            CookieHelper.Add("fooke_admin_token", strKey, 90);
            SessionHelper.Add("fooke_admin_token", strKey, 90);
            /********************************************************************
             * 保存数据记录
             * *******************************************************************/
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["LastDate"] = DateTime.Now.ToString();
            dictionary["strKey"] = strKey;
            dictionary["hits"] = (new Fooke.Function.String(cRs["hits"].ToString()).cInt() + 1).ToString();
            DbHelper.Connection.Update("Fooke_Admin", dictionary, Params: " and AdminID=" + cRs["AdminID"] + "");
            /********************************************************************
             * 输出数据处理结果信息
             * *******************************************************************/
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"登陆成功\"");
            strBuilder.Append(",\"type\":\"redirect\"");
            strBuilder.Append(",\"url\":\"Index.aspx\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}