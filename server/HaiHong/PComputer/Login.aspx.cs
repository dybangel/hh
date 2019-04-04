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
using Fooke.SimpleMaster;
namespace Fooke.Web.Member
{
    public partial class Login : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "exit": LoginOut(); Response.End(); break;
                case "start": CheckLogin(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 安全退出登录页面
        /// </summary>
        protected void LoginOut()
        {
            CookieHelper.Add("Fooke_MemberKey", "", 0);
            CookieHelper.Add("Fooke_MemberID", "0", 0);
            CookieHelper.Delete("Fooke_MemberKey");
            CookieHelper.Delete("Fooke_MemberID");
            Response.Redirect("Login.aspx");
            Response.End();
        }
        /// <summary>
        /// 登录页面
        /// </summary>
        protected void strDefault()
        {
            /***********************************************************************
             * 生成网页key值
             * *********************************************************************/
            string Tokey = string.Format("用户登录-|-|-{0}-|-|-{1}-|-|-用户登陆",
                DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("yyyyMMddHH0000"));
            Tokey = new Fooke.Function.String(Tokey).ToMD5().ToLower();
            /***********************************************************************
             * 开始输出网页数据
             * *********************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/login/default.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "tokey": strValue = Tokey; break;
                }
                return strValue;
            }), isLabel: true);
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 验证用户登录信息
        /// </summary>
        public void CheckLogin()
        {
            /*************************************************************************************************************
             * 验证用户账号以及登陆密码
             * ***********************************************************************************************************/
            string UserName = RequestHelper.GetRequest("userName").toString();
            if (string.IsNullOrEmpty(UserName)) { this.ErrorMessage("请填写您的登陆账号！"); Response.End(); }
            else if (UserName.Length <= 5) { this.ErrorMessage("登陆账号长度不能少于6个字符！"); Response.End(); }
            else if (UserName.Length >= 16) { this.ErrorMessage("登陆账号长度不能超过16个字符！"); Response.End(); }
            else if (VerifyCenter.VerifySpecific(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            else if (VerifyCenter.VerifyChina(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            /*************************************************************************************************************
             * 验证登录密码是否合法
             * ***********************************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请填写您的登陆密码！"); Response.End(); }
            else if (Password.Length <= 5) { this.ErrorMessage("登陆密码不能少于6个字符！"); Response.End(); }
            else if (Password.Length >= 16) { this.ErrorMessage("登陆密码长度不能少于16个字符！"); Response.End(); }
            /*************************************************************************************************************
             * 获取并验证用户登陆的合法性
             * ***********************************************************************************************************/
            string Tokey = string.Format("用户登录-|-|-{0}-|-|-{1}-|-|-用户登陆",
                 DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("yyyyMMddHH0000"));
            Tokey = new Fooke.Function.String(Tokey).ToMD5().ToLower();
            string strKey = RequestHelper.GetRequest(Tokey).toString();
            if (string.IsNullOrEmpty(strKey)) { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            else if (strKey.Length != 32) { this.ErrorMessage("请求参数错误,可能是页面已经过期,请刷新网页重试！"); Response.End(); }
            else if (Tokey != strKey) { this.ErrorMessage("请求参数错误,可能是页面已经过期,请刷新网页重试！"); Response.End(); }
            /************************************************************************************************************
             * 验证IP黑名单数据信息
             * ***********************************************************************************************************/
            string strBlack = this.GetParameter("strBlack", "UserXML").toString();
            if (!string.IsNullOrEmpty(strBlack) && strBlack.Contains(FunctionCenter.GetCustomerIP()))
            { this.ErrorMessage("该IP登录地址已经被禁止！"); Response.End(); }
            /************************************************************************************************************
             * 更新用户帐号Key值，保证当前用户只存在唯一账户登录
             * **********************************************************************************************************/
            string thisNce = this.GetParameter("thisNce", "userXml").toInt();
            string strTokey = string.Format("用户登陆-|-|-{0}-|-|-{1}-|-|-用户登陆", UserName, Password);
            if (thisNce != "1") { strTokey = strTokey + "-|-|-" + Guid.NewGuid().ToString(); }
            strTokey = new Fooke.Function.String(strTokey).ToMD5().Substring(0, 24).ToUpper();
            /*************************************************************************************************************
            * 发起用户账号登陆请求,验证账号密码的合法性
            * ***********************************************************************************************************/
            DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveUserLogin", new Dictionary<string, object>() {
                {"UserName",UserName},
                {"Password",MemberHelper.toPassword(Password)},
                {"strTokey",strTokey},
                {"strIP",FunctionCenter.GetCustomerIP()}
            });
            if (MemberRs == null) { this.ErrorMessage("账号或者登录密码错误！"); Response.End(); }
            else if (MemberRs["isDisplay"].ToString() != "1") { this.ErrorMessage("用户账号已停止使用,不能再登陆了！"); Response.End(); }
            /*************************************************************************************************************
            * 保存用户登录数据信息
            * ***********************************************************************************************************/
            CookieHelper.Add("Fooke_MemberKey", MemberRs["strTokey"].ToString(), 90);
            CookieHelper.Add("Fooke_MemberID", MemberRs["UserID"].ToString(), 90);
            /**************************************************************************************************************
             * 获取用户登陆跳转地址
             * ************************************************************************************************************/
            string returnUrl = RequestHelper.GetRequest("returnUrl").ToString();
            /**************************************************************************************************************
            * 输出出局处理结果
            * ************************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"type\":\"redirect\"");
            if (returnUrl.Length != 0) { strBuilder.Append(",\"url\":\"" + returnUrl.ToEncryptionText() + "\""); }
            else { strBuilder.Append(",\"url\":\"index.aspx\""); }
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}