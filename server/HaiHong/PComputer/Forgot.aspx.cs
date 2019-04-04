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
namespace Fooke.Web.Member
{
    public partial class Forgot : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "message": SendMessage(); Response.End(); break;
                case "verification": SaveVerification(); Response.End(); break;
                case "fit": strPassword(); Response.End(); break;
                case "saveFrgot": SaveForgot(); Response.End(); break;
                case "success": ShowSuccess(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 登陆密码重置成功
        /// </summary>
        protected void ShowSuccess()
        {
            /*********************************************************************************************
             * 验证登陆用户信息
             * *******************************************************************************************/
            string strKey = RequestHelper.GetRequest("strKey").ToString();
            if (string.IsNullOrEmpty(strKey)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"thisKey",strKey}
            });
            if (cRs == null) { this.ErrorMessage("获取用户信息失败,请重试!"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("账户已被冻结,请联系客服!"); Response.End(); }
            /*********************************************************************************************
             * 输出网页界面
             * *******************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forgot/success.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            /**************************************************************************************
             * 解析语言包,语言版本
             * ************************************************************************************/
            strResponse = new Fooke.SimpleMaster.LanguageMaster("lang/forgot/success").StartResolution(strResponse);
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 重置用户登陆密码
        /// </summary>
        protected void strPassword()
        {
            /*********************************************************************************************
             * 验证登陆用户信息
             * *******************************************************************************************/
            string strKey = RequestHelper.GetRequest("strKey").ToString();
            if (string.IsNullOrEmpty(strKey)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strKey.Length < 24) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"thisKey",strKey}
            });
            if (cRs == null) { this.ErrorMessage("获取用户信息失败,请重试!"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("账户已被冻结,请联系客服!"); Response.End(); }
            /*********************************************************************************************
             * 输出网页界面
             * *******************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forgot/fit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            /**************************************************************************************
             * 解析语言包,语言版本
             * ************************************************************************************/
            strResponse = new Fooke.SimpleMaster.LanguageMaster("lang/forgot/fit").StartResolution(strResponse);
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 获取请求参数信息
        /// </summary>
        protected void strDefault()
        {
            /********************************************************************
             * 输出网页内容信息
             * *****************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forgot/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                return strValue;
            }));
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 验证手机号信息是否合法
        /// </summary>
        protected void SaveVerification()
        {
            /*********************************************************************************************
             * 验证登陆账号合法性
             * *******************************************************************************************/
            string strMobile = RequestHelper.GetRequest("strMobile").ToString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("请填写您的登陆账号！"); Response.End(); }
            else if (strMobile.Length <= 5) { this.ErrorMessage("登陆账号格式错误！"); Response.End(); }
            else if (strMobile.Length >= 16) { this.ErrorMessage("登陆账号格式错误！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"Username",strMobile}
            });
            if (cRs == null) { this.ErrorMessage("手机号不存在,请注册!"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("账号已停止使用,请联系客服!"); Response.End(); }
            /********************************************************************************************************************
             * 验证验证码的合法性
             * ******************************************************************************************************************/
            string Captcha = RequestHelper.GetRequest("Captcha").toString();
            if (string.IsNullOrEmpty(Captcha)) { this.ErrorMessage("请填写验证码！"); Response.End(); }
            else if (Captcha.Length != 6) { this.ErrorMessage("验证码长度太长！"); Response.End(); }
            else if (cRs["SessionDate"].ToString().cDate() <= DateTime.Now.AddMinutes(-5)) { this.ErrorMessage("短信验证码已过期！"); Response.End(); }
            else if (Captcha != cRs["SessionCode"].ToString()) { this.ErrorMessage("短信验证码错误,请重试！"); Response.End(); }
            /************************************************************************************************
            * 账号验证通过,跳转到设置新密码界面
            * ***********************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"type\":\"redirect\"");
            strBuilder.Append(",\"url\":\"Forgot.aspx?action=fit&strKey=" + cRs["strTokey"] + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 保存用户登陆密码信息
        /// </summary>
        protected void SaveForgot()
        {
            /*********************************************************************************************
             * 验证登陆用户信息
             * *******************************************************************************************/
            string strKey = RequestHelper.GetRequest("strKey").ToString();
            if (string.IsNullOrEmpty(strKey)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strKey.Length <= 20) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"thisKey",strKey}
            });
            if (cRs == null) { this.ErrorMessage("获取用户信息失败,请重试!"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("账户已被冻结,请联系客服!"); Response.End(); }
            /******************************************************************************************************
             * 判断新密码是否合法
             * ****************************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请填写你要设置的登录密码！"); Response.End(); }
            if (Password.Length < 6) { this.ErrorMessage("登录密码长度至少在6位数以上！"); Response.End(); }
            if (Password.Length > 16) { this.ErrorMessage("登录密码长度请限制在16位数以内！"); Response.End(); }
            string SurePassword = RequestHelper.GetRequest("SurePassword").toString();
            if (string.IsNullOrEmpty(SurePassword)) { this.ErrorMessage("请填写确认登录密码！"); Response.End(); }
            if (SurePassword != Password) { this.ErrorMessage("确认密码填写错误！"); Response.End(); }
            if (MemberHelper.toPassword(Password) == cRs["PasswordTo"].ToString()){ this.ErrorMessage("登陆密码与安全密码不能相同！"); Response.End(); }
            /*****************************************************************************************************
             * 开始保存用户登陆密码信息
             * ***************************************************************************************************/
            DbHelper.Connection.Update("Fooke_User", new Dictionary<string, string>() {
                {"Password",MemberHelper.toPassword(Password)}
            }, Params: " and UserID=" + cRs["UserID"] + "");
            /*********************************************************************************
             * 返回处理结果,密码重置成功
             * *******************************************************************************/
            Response.Redirect("Forgot.aspx?action=success&strkey=" + cRs["strTokey"] + "");
            Response.End();
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        protected void SendMessage()
        {
            string strMobile = RequestHelper.GetRequest("strMobile").ToString();
            if (string.IsNullOrEmpty(strMobile)) { this.JSONMessage("请填写您的登陆账号！"); Response.End(); }
            else if (strMobile.Length <= 5) { this.JSONMessage("登陆账号格式错误！"); Response.End(); }
            else if (strMobile.Length >= 16) { this.JSONMessage("登陆账号格式错误！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"Username",strMobile}
            });
            if (cRs == null) { this.JSONMessage("您填写的账号不存在,请检查!"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.JSONMessage("账号已停止使用,请联系客服!"); Response.End(); }
            /************************************************************************************************
              * 生成短信验证码并保存
              * ***********************************************************************************************/
            string SessionCode = new Random().Next(111111, 999999).ToString();
            DbHelper.Connection.Update("Fooke_User", dictionary: new Dictionary<string, string>() {
                {"SessionCode",SessionCode},
                {"SessionDate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            }, Params: " and UserID=" + cRs["UserID"] + "");
            /************************************************************************************************
             * 发送短信验证码
             * ***********************************************************************************************/
            string SendText = "您的短信验证码是" + SessionCode + "";
            new MessageHelper().Start(Configure: this.Configure,
                Mobile: strMobile,
                SendText: SendText,
                Fun: (iSuccess, strResponse) =>
                {
                    if (!iSuccess) { JSONMessage(strResponse); Response.End(); }
                });
            /************************************************************************************************
             * 输出数据处理结果
             * ***********************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"type\":\"alert\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}