using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fooke.Code;
using Fooke.Function;
using System.Data;
namespace Fooke.Web.API
{
    public partial class Forgot : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "sendMessage": SendMessage(); Response.End(); break;
                case "message": SendMessage(); Response.End(); break;
                case "ResetPassword": ResetPassword(); Response.End(); break;
            }
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        protected void ResetPassword()
        {
            /***************************************************************************************************
             * 验证用户信息的合法性
             * **************************************************************************************************/
            string Mobile = RequestHelper.GetRequest("Mobile").toString();
            if (string.IsNullOrEmpty(Mobile)) { this.ErrorMessage("请填写注册手机号码！"); Response.End(); }
            if (Mobile.Length < 6) { this.ErrorMessage("账户长度超出合法限定!"); Response.End(); }
            if (Mobile.Length > 16) { this.ErrorMessage("账户长度超出合法限定!"); Response.End(); }
            DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("Stored_FindMember", new Dictionary<string, object>() { 
                {"UserName",Mobile}
            });
            if (MemberRs == null) { this.ErrorMessage("账户不存在,请检查！"); Response.End(); }
            /***************************************************************************************************
             * 验证短信验证码的合法性
             * **************************************************************************************************/
            string SessionCode = RequestHelper.GetRequest("Captcha").toInt();
            if (SessionCode == "0") { this.ErrorMessage("请填写短信验证码！"); Response.End(); }
            if (MemberRs["SessionCode"].ToString() != SessionCode) { this.ErrorMessage("手机号验证码错误！"); Response.End(); }
            /***************************************************************************************************
             * 验证登陆密码的合法性
             * **************************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请填写您的新密码！"); Response.End(); }
            if (Password.Length < 6 || Password.Length > 16) { this.ErrorMessage("新密码长度设置请保持在6-16位数之间!"); Response.End(); }
            string surePassword = RequestHelper.GetRequest("surePassword").toString();
            if (string.IsNullOrEmpty(surePassword)) { this.ErrorMessage("请填写您的确认新密码！"); Response.End(); }
            if (surePassword != Password) { this.ErrorMessage("新密码与确认密码不一致！"); Response.End(); }
            /***************************************************************************************************
             * 开始保存登陆密码
             * **************************************************************************************************/
            Dictionary<string, string> oDic = new Dictionary<string, string>();
            oDic["Password"] = MemberHelper.toPassword(Password);
            DbHelper.Connection.Update(TableCenter.User, oDic, Params: " and UserID=" + MemberRs["UserID"] + "");
            /***************************************************************************************************
             * 输出数据处理结果
             * **************************************************************************************************/
            StringBuilder strXml = new StringBuilder();
            strXml.Append("{");
            strXml.Append("\"success\":\"true\"");
            strXml.Append(",\"tips\":\"密码重置成功\"");
            strXml.Append("}");
            Response.Write(strXml.ToString());
            Response.End();
        }
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        protected void SendMessage()
        {
            string Mobile = RequestHelper.GetRequest("Mobile").toString();
            if (string.IsNullOrEmpty(Mobile)) { this.ErrorMessage("请填写注册手机号码！"); Response.End(); }
            if (Mobile.Length < 6) { this.ErrorMessage("账户长度超出合法限定!"); Response.End(); }
            if (Mobile.Length > 16) { this.ErrorMessage("账户长度超出合法限定!"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() { 
                {"UserName",Mobile}
            });
            if (cRs == null) { this.ErrorMessage("账户不存在,请检查！"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("账户不存在,请检查！"); Response.End(); }
            /****************************************************************************************************
             * 保存短信验证码
             * **************************************************************************************************/
            string SessionCode = new Random().Next(111111, 999999).ToString();
            DbHelper.Connection.Update(TableCenter.User, new Dictionary<string, string>() {
                {"SessionCode",SessionCode}
            }, Params: " and UserID=" + cRs["UserID"] + "");
            /****************************************************************************************************
             * 开始发送短信消息
             * **************************************************************************************************/
            string SendText = "您的短信验证码是 " + SessionCode + "";
            new MessageHelper().Start(this.Configure, Mobile, SendText, (iSuccess, strResponse) =>
            {
                if (!iSuccess) { this.ErrorMessage(strResponse); Response.End(); }
            });
            /****************************************************************************************************
             * 输出短信发送结果信息
             * **************************************************************************************************/
            StringBuilder strXml = new StringBuilder();
            strXml.Append("{");
            strXml.Append("\"success\":\"true\"");
            strXml.Append(",\"tips\":\"短信验证码发送成功\"");
            strXml.Append(",\"code\":\"000000\"");
            strXml.Append("}");
            Response.Write(strXml.ToString());
            Response.End();
        }
    }
}