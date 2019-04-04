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
namespace Fooke.Web.API
{
    /// <summary>
    /// 用户登录接口信息
    /// </summary>
    public partial class Login : Fooke.Code.BaseHelper
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "start": SaveLogin(); Response.End(); break;
            }
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        protected void SaveLogin()
        {
            /************************************************************************************************************
            * 判断IP地址是否已加入黑名单
            * ***********************************************************************************************************/
            string strBlack = this.GetParameter("strBlack", "UserXML").toString();
            if (!string.IsNullOrEmpty(strBlack) && strBlack.Contains(FunctionCenter.GetCustomerIP()))
            { this.ErrorMessage("该IP登录地址已经被禁止！"); Response.End(); }
            /**************************************************************************************************************
             * 获取用户设备类型信息
             * ************************************************************************************************************/
            string DeviceType = RequestHelper.GetRequest("DeviceType").toString("ios");
            if (string.IsNullOrEmpty(DeviceType)) { this.ErrorMessage("获取设备类型信息失败,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(DeviceType) && DeviceType.Length <= 2) { this.ErrorMessage("设备类型字段不能少于2个字符！"); Response.End(); }
            else if (!string.IsNullOrEmpty(DeviceType) && DeviceType.Length >= 12) { this.ErrorMessage("设备类型字段不能大于12个字符！"); Response.End(); }
            string DeviceCode = RequestHelper.GetRequest("DeviceCode").toString();
            if (string.IsNullOrEmpty(DeviceCode)) { this.ErrorMessage("获取设备编号信息失败,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(DeviceCode) && DeviceCode.Length <= 12) { this.ErrorMessage("设备编号信息字段不能少于12个字符！"); Response.End(); }
            else if (!string.IsNullOrEmpty(DeviceCode) && DeviceCode.Length >= 40) { this.ErrorMessage("设备编号信息字段不能大于40个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 验证用户登陆账号数据信息
             * ************************************************************************************************************/
            string UserName = RequestHelper.GetRequest("UserName").toString();
            if (string.IsNullOrEmpty(UserName)) { this.ErrorMessage("用户登陆账号不能为空！"); Response.End(); }
            else if (UserName.Length <= 5) { this.ErrorMessage("用户登陆账号不能少于6个字符！"); Response.End(); }
            else if (UserName.Length >= 16) { this.ErrorMessage("用户登陆账号不能超过16个字符！"); Response.End(); }
            else if (VerifyCenter.VerifyChina(UserName)) { this.ErrorMessage("用户登陆账号中不能包含中文字符"); Response.End(); }
            else if (VerifyCenter.VerifySpecific(UserName)) { this.ErrorMessage("用户登陆账号中不能包含特殊字符"); Response.End(); }
            /************************************************************************************************************
            * 验证用户账号登陆密码信息
            * ***********************************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请输入登录密码！"); Response.End(); }
            if (Password.Length <= 5) { this.ErrorMessage("登陆密码不能少于6位数！"); Response.End(); }
            if (Password.Length >= 16) { this.ErrorMessage("登陆密码不能超过16位数！"); Response.End(); }
            else { Password = MemberHelper.toPassword(Password); }
            /************************************************************************************************************
            * 更新用户账号请求数据信息
            * ***********************************************************************************************************/
            string thisNce = this.GetParameter("thisNce", "userXml").toInt();
            string strkey = string.Format("用户登录-|-|-|-{0}-|-|-{1}-|-|-用户登陆", UserName, Password);
            if (thisNce != "1") { strkey = strkey + "-|-|-" + Guid.NewGuid().ToString(); }
            strkey = new Fooke.Function.String(strkey).ToMD5().Substring(0, 24).ToUpper();
            /************************************************************************************************************
            * 验证用户账号密码的合法性
            ************************************************************************************************************/
            DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveUserLogin", new Dictionary<string, object>() {
                {"UserName",UserName},
                {"Password",Password},
                {"strTokey",strkey},
                {"strIP",FunctionCenter.GetCustomerIP()}
            });
            if (MemberRs == null) { this.ErrorMessage("账号或者登录密码错误！"); Response.End(); }
            else if (MemberRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前帐号不允许登录！"); Response.End(); }
            /*************************************************************************************************************
            * 更新账号设备信息
            * ***********************************************************************************************************/
            if (MemberRs.Table.Columns["deviceType"] != null && MemberRs["deviceType"].ToString() == "注册") { SaveUserReplacement(MemberRs, DeviceType, DeviceCode); }
            else if (MemberRs.Table.Columns["deviceType"] != null && MemberRs["deviceType"].ToString().ToLower() == "define") { SaveUserReplacement(MemberRs, DeviceType, DeviceCode); }
            else if (MemberRs.Table.Columns["deviceCode"] != null && MemberRs["deviceCode"].ToString().ToLower() != DeviceCode.ToLower()) { SaveUserReplacement(MemberRs, DeviceType, DeviceCode); }
            /*************************************************************************************************************
            * 保存用户登陆记录信息
            * ***********************************************************************************************************/
            CookieHelper.Add("Fooke_MemberKey", MemberRs["strTokey"].ToString(), 90);
            CookieHelper.Add("Fooke_MemberID", MemberRs["UserID"].ToString(), 90);
            CookieHelper.Add("Fooke_MemberName", MemberRs["UserName"].ToString(), 90);
            /*************************************************************************************************************
            * 输出数据处理结果
            * ***********************************************************************************************************/
            ResponseDataRow(MemberRs);
            Response.End();
        }

        /**************************************************************************************************************
        * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
        * 公用方法处理区域
        * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
        * *************************************************************************************************************/
        #region 公用方法处理区域
        /// <summary>
        /// 更新用户登陆设备账号信息
        /// </summary>
        /// <param name="MemberRs"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public DataRow SaveUserReplacement(DataRow MemberRs, string deviceType, string deviceCode)
        {
            /**************************************************************************************************************
             * 开始执行数据请求
             * ************************************************************************************************************/
            MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserReplacement]", new Dictionary<string, object>() {
               {"UserID",MemberRs["UserID"].ToString()},    
               {"DeviceType",deviceType},
               {"DeviceCode",deviceCode}
            });
            /**************************************************************************************************************
             * 返回数据处理结果
             * ************************************************************************************************************/
            return MemberRs;
        }

        #endregion
    }
}