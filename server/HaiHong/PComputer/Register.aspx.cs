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
    public partial class Register : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "default": strDefault(); Response.End(); break;
                case "message": SendMessage(); Response.End(); break;
                case "start": SaveRegister(); Response.End(); break;
                case "signup": toRedirect(); Response.End(); break;
            }
        }

        protected void toRedirect()
        {
            /**************************************************************************************************************
             * 根据shareCode 换取到实际邀请码
             * ************************************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("邀请码错误,请填写您的邀请码!"); Response.End(); }
            else if (UserID.Length <= 5) { this.ErrorMessage("邀请码错误,请填写您的邀请码!"); Response.End(); }
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                 {"UserID",UserID}
            });
            if (oRs == null) { this.ErrorMessage("邀请码错误,请重试！"); Response.End(); }
            else if (oRs["isDisplay"].ToString() != "1") { this.ErrorMessage("邀请码错误,请重试！"); Response.End(); }
            /**************************************************************************************************************
             * 生成新的邀请码信息
             * ************************************************************************************************************/
            string ShareCode = string.Format("{0}{1}", oRs["Username"].ToString().Substring(0, 1), UserID);
            /**************************************************************************************************************
             * 输出数据处理结果
             * ************************************************************************************************************/
            Response.Redirect("~/member/register.aspx?code=" + ShareCode + "");
            Response.End();
        }
        /// <summary>
        /// 保存用户注册信息
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 验证请求参数的合法性
             * ************************************************************************************/
            string registerMode = RequestHelper.GetRequest("type").toString("default");
            string ShareCode = RequestHelper.GetRequest("ShareCode").ToString();
            string ShareKey = RequestHelper.GetRequest("ShareKey").ToString();
            if (registerMode == "invited" && ShareCode.Length<=0) { this.ErrorMessage("验证邀请数据信息失败,请重试！"); Response.End(); }
            else if (registerMode == "invited" && ShareCode.Length != 11) { this.ErrorMessage("验证邀请数据信息失败,请重试！"); Response.End(); }
            if (registerMode == "invited" && ShareKey.Length <= 0) { this.ErrorMessage("验证邀请数据信息失败,请重试！"); Response.End(); }
            else if (registerMode == "invited" && ShareKey.Length != 24) { this.ErrorMessage("验证邀请数据信息失败,请重试！"); Response.End(); }
            /**************************************************************************************
             * 验证邀请加密字符串的合法性
             * ************************************************************************************/
            if (registerMode == "invited")
            {
                string FormatKey = string.Format("邀请注册-|-|-{0}-|-|-{1}-|-|-邀请注册", ShareCode, ShareCode);
                FormatKey = new Fooke.Function.String(FormatKey).ToMD5().Substring(0, 24).ToLower();
                if (ShareKey != FormatKey) { this.ErrorMessage("验证邀请码数据信息失败,请重试！"); Response.End(); }
            }
            /**************************************************************************************
             * 生成注册加密的注册值
             * ************************************************************************************/
            string strKey = string.Format("用户注册-|-|-{0}-|-|-{1}-|-|-用户注册",
                DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMM"));
            strKey = new Fooke.Function.String(strKey).ToMD5().Substring(2, 24).ToLower();
            /**************************************************************************************
             * 开始加载网页内容信息
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/register/default.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "strKey": strValue = strKey; break;
                    case "readonly": if (ShareCode.Length != 0) { strValue = "readonly"; } break;
                }
                return strValue;
            }), true);
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        protected void SendMessage()
        {
            /************************************************************************************************
             * 获取请求用户信息
             * ***********************************************************************************************/
            string strMobile = RequestHelper.GetRequest("strMobile").ToString();
            if (string.IsNullOrEmpty(strMobile)) { this.JSONMessage("请填写您的手机号！"); Response.End(); }
            else if (strMobile.Length <= 5) { this.JSONMessage("手机号格式错误,请重试！"); Response.End(); }
            else if (strMobile.Length >= 16) { this.JSONMessage("手机号格式错误,请重试！"); Response.End(); }
            else if (strMobile.Length != 11) { this.JSONMessage("手机号码格式错误,请重试！"); Response.End(); }
            else if (!strMobile.isMobile()) { this.JSONMessage("手机号码格式错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                 {"Username",strMobile}
            });
            if (cRs != null) { this.JSONMessage("手机号已经存在,请直接登陆!"); Response.End(); }
            /************************************************************************************************
             * 生成短信验证码并保存
             * ***********************************************************************************************/
            string SessionCode = new Random().Next(111111, 999999).ToString();
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
             * 将短信号码保存到本地
             * ***********************************************************************************************/
            Fooke.Function.CookieHelper.Add("Fooke_SessionCode", SessionCode);
            Fooke.Function.SessionHelper.Add("Fooke_SessionCode", SessionCode);
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
        /// <summary>
        /// 用户注册
        /// </summary>
        protected void SaveRegister()
        {
            /************************************************************************************************************
             * 验证是否已开启账号注册功能
             * ***********************************************************************************************************/
            string isRegister = this.GetParameter("isRegister", "userXml").toInt();
            if (isRegister != "1") { this.ErrorMessage("当前用户系统不支持帐号注册！"); Response.End(); }
            /************************************************************************************************************
             * 验证IP黑名单数据信息
             * ***********************************************************************************************************/
            string strBlack = this.GetParameter("strBlack", "UserXML").toString();
            if (!string.IsNullOrEmpty(strBlack) && strBlack.Contains(FunctionCenter.GetCustomerIP()))
            { this.ErrorMessage("该IP登录地址已经被禁止！"); Response.End(); }
            /**************************************************************************************************************
             * 获取用户设备类型信息
             * ************************************************************************************************************/
            string DeviceType = RequestHelper.GetRequest("DeviceType").toString("Define");
            if (string.IsNullOrEmpty(DeviceType)) { this.ErrorMessage("获取设备类型信息失败,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(DeviceType) && DeviceType.Length <= 2) { this.ErrorMessage("设备类型字段不能少于2个字符！"); Response.End(); }
            else if (!string.IsNullOrEmpty(DeviceType) && DeviceType.Length >= 12) { this.ErrorMessage("设备类型字段不能大于12个字符！"); Response.End(); }
            string DeviceCode = RequestHelper.GetRequest("DeviceCode").toString(Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(DeviceCode)) { this.ErrorMessage("获取设备编号信息失败,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(DeviceCode) && DeviceCode.Length <= 12) { this.ErrorMessage("设备编号信息字段不能少于12个字符！"); Response.End(); }
            else if (!string.IsNullOrEmpty(DeviceCode) && DeviceCode.Length >= 40) { this.ErrorMessage("设备编号信息字段不能大于40个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 开始验证用户账户信息,并且验证登陆账号是否存在
             * ************************************************************************************************************/
            string UserName = RequestHelper.GetRequest("UserName").toString();
            if (string.IsNullOrEmpty(UserName)) { this.ErrorMessage("请填写用户登陆账号！"); Response.End(); }
            else if (UserName.Length <= 5) { this.ErrorMessage("登陆账号太短,不能少于5个字符！"); Response.End(); }
            else if (UserName.Length >= 16) { this.ErrorMessage("登陆账号太长,不能多于16个字符！"); Response.End(); }
            else if (VerifyCenter.VerifySpecific(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            else if (VerifyCenter.VerifyChina(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            else if (UserName.Length != 11) { this.ErrorMessage("登陆账号格式错误,请填写手机号!"); Response.End(); }
            else if (!UserName.isMobile()) { this.ErrorMessage("登陆账号格式错误,请填写手机号!"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserName",UserName}
            });
            if (cRs != null) { this.ErrorMessage("登陆手机号码已经被注册了!"); Response.End(); }
            /**************************************************************************************************************
             * 验证上级用户,顶级帐号
             * ************************************************************************************************************/
            string ParentID = GetParent(RequestHelper.GetRequest("ShareMobile").ToString());
            /**************************************************************************************************************
             * 验证其它的数据信息
             * ************************************************************************************************************/
            string Password = RequestHelper.GetRequest("password").toString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请填写你的登陆密码!"); Response.End(); }
            else if (Password.Length <= 5) { this.ErrorMessage("登陆密码不能少于6个字符!"); Response.End(); }
            else if (Password.Length > 16) { this.ErrorMessage("登陆密码不能大于16个字符"); Response.End(); }
            else { Password = MemberHelper.toPassword(Password); }
            /************************************************************************************************************
            * 二级密码,资金密码
            * **********************************************************************************************************/
            string PasswordTo = RequestHelper.GetRequest("PasswordTo").toString("FookeCMS");
            if (string.IsNullOrEmpty(PasswordTo)) { this.ErrorMessage("请填写您的支付密码!"); Response.End(); }
            else if (PasswordTo.Length <= 5) { this.ErrorMessage("二级密码不能小于6个字符!"); Response.End(); }
            else if (PasswordTo.Length > 16) { this.ErrorMessage("二级密码不能大于16个字符!"); Response.End(); }
            else { PasswordTo = MemberHelper.toPassword(PasswordTo); }
            /************************************************************************************************************
            * 获取短信验证码信息,验证账号是否合法
            * ***********************************************************************************************************/
            string Captcha = RequestHelper.GetRequest("Captcha").toString();
            if (string.IsNullOrEmpty(Captcha)) { this.ErrorMessage("请填写验证码！"); Response.End(); }
            else if (Captcha.Length != 6) { this.ErrorMessage("验证码长度太长！"); Response.End(); }
            string SessionCode = Fooke.Function.SessionHelper.Get("Fooke_SessionCode").ToString();
            if (string.IsNullOrEmpty(SessionCode)) { SessionCode = Fooke.Function.CookieHelper.Get("Fooke_SessionCode").ToString(); }
            if (!string.IsNullOrEmpty(SessionCode) && SessionCode.Length != 6) { this.ErrorMessage("获取短信验证码数据失败,请重试！"); Response.End(); }
            if (SessionCode.Length == 6 && SessionCode != Captcha) { this.ErrorMessage("短信验证码错误,请重试！"); Response.End(); }
            /************************************************************************************************************
             * 验证用户昵称帐号是否唯一
             * **********************************************************************************************************/
            string Nickname = RequestHelper.GetRequest("Nickname").toString(UserName);
            if (string.IsNullOrEmpty(Nickname)) { this.ErrorMessage("请填写你的账户昵称!"); Response.End(); }
            else if (Nickname.Length <= 1) { this.ErrorMessage("账户昵称不能少于1个字符!"); Response.End(); }
            else if (Nickname.Length >= 16) { this.ErrorMessage("账户昵称不能大于16个字符!"); Response.End(); }
            /************************************************************************************************************
             * 验证用户资料
             * **********************************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString("/file/user/default.png");
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("获取用户头像地址信息失败,请重试！"); Response.End(); }
            else if (Thumb.Length <= 10) { this.ErrorMessage("获取用户头像地址信息失败,请重试！"); Response.End(); }
            else if (Thumb.Length >= 120) { this.ErrorMessage("用户头像地址太长,请限制在120个字符以内！"); Response.End(); }
            /*************************************************************************************************************
             * 获取并验证用户账号信息
             * ***********************************************************************************************************/
            string strEmail = RequestHelper.GetRequest("strEmail").ToString();
            if (strEmail.Length > 26) { this.ErrorMessage("邮箱地址字段长度请限制在26个字符以内！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strEmail) && !strEmail.isEmail()) { this.ErrorMessage("邮箱地址字段格式错误,请重试！"); Response.End(); }
            string strMobile = RequestHelper.GetRequest("strMobile").toString(UserName);
            if (!string.IsNullOrEmpty(strMobile) && strMobile.Length != 11) { this.ErrorMessage("手机号格式错误,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strMobile) && !strMobile.isMobile()) { this.ErrorMessage("手机号格式错误！"); Response.End(); }
            /************************************************************************************************************
            * 获取无需验证的数据信息
            * ***********************************************************************************************************/
            string isDisplay = this.GetParameter("autoDisplay", "userXml").toInt();
            /************************************************************************************************************
            * 生成用户登陆标识,该标识记录用户登陆信息必须唯一
            * ***********************************************************************************************************/
            string strTokey = string.Format("用户注册-|-|-|-{0}-|-|-{1}-|-|-{2}-|-|-用户注册",
                DateTime.Now.ToString("yyyyMMddHHmmss"), Password, Guid.NewGuid().ToString());
            strTokey = new Fooke.Function.String(strTokey).ToMD5().Substring(0, 24).ToUpper();
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"strTokey",strTokey}
            });
            if (sRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /************************************************************************************************************
            * 生成用户登陆标识,该标识记录用户登陆信息必须唯一
            * ***********************************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["ParentID"] = ParentID;
            oDictionary["DeviceCode"] = DeviceCode;
            oDictionary["DeviceType"] = DeviceType;
            oDictionary["strTokey"] = strTokey;
            oDictionary["UserName"] = UserName;
            oDictionary["Password"] = Password;
            oDictionary["PasswordTo"] = PasswordTo;
            oDictionary["Nickname"] = Nickname;
            oDictionary["Thumb"] = Thumb;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["strMobile"] = strMobile;
            oDictionary["strEmail"] = strEmail;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUsers]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*************************************************************************************************************
             * 保存用户登录数据信息
             * ***********************************************************************************************************/
            CookieHelper.Add("Fooke_MemberKey", thisRs["strTokey"].ToString(), 90);
            CookieHelper.Add("Fooke_MemberID", thisRs["UserID"].ToString(), 90);
            /**************************************************************************************************************
             * 获取用户登陆跳转地址
             * ************************************************************************************************************/
            string returnUrl = RequestHelper.GetRequest("returnUrl").ToString();
            /**************************************************************************************************************
             * 输出数据处理结果
             * ************************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"type\":\"redirect\"");
            strBuilder.Append(",\"url\":\"Index.aspx\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /**************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 公用方法处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *************************************************************************************************************/
        #region 公用方法处理区域
        /// <summary>
        /// 获取到用户的邀请ID
        /// </summary>
        /// <returns></returns>
        public string GetParent(string ShareMobile)
        {
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            /**************************************************************************************************************
             * 开始执行数据请求
             * ************************************************************************************************************/
            string ShareMust = this.GetParameter("share", "userXml").toInt();
            if (ShareMust == "1" && string.IsNullOrEmpty(ShareMobile)) { this.ErrorMessage("请填写邀请人手机号！"); Response.End(); }
            else if (!string.IsNullOrEmpty(ShareMobile) && ShareMobile.Length <= 5) { this.ErrorMessage("邀请码格式错误,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(ShareMobile) && ShareMobile.Length >= 16) { this.ErrorMessage("邀请码格式错误,请重试！"); Response.End(); }
            if (!string.IsNullOrEmpty(ShareMobile))
            {
                DataRow FormatRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                   {"Username",ShareMobile}
                });
                if (FormatRs == null) { this.ErrorMessage("您填写的邀请码不存在,请重试！"); Response.End(); }
                else if (FormatRs["isDisplay"].ToString() != "1") { this.ErrorMessage("您填写的邀请码不存在,请重试！"); Response.End(); }
                /**************************************************************************************************************
                * 未邀请ID重新赋值
                * ************************************************************************************************************/
                ParentID = new Fooke.Function.String(FormatRs["UserID"].ToString()).toInt();
            }
            /**************************************************************************************************************
             * 判断邀请码的合法性
             * ************************************************************************************************************/
            if (ShareMust == "1" && ParentID == "0") { this.ErrorMessage("请填写邀请人手机号！"); Response.End(); }
            /**************************************************************************************************************
             * 返回数据处理结果
             * ************************************************************************************************************/
            return ParentID;
        }

        /// <summary>
        /// 检查设备编号是否存在
        /// </summary>
        /// <param name="deviceType">设备类型</param>
        /// <param name="deviceCode">设备编号</param>
        /// <returns></returns>
        public bool existsDevice(string deviceType, string deviceCode)
        {
            /*********************************************************************************************************
             * 申明数据返回变量
             * *******************************************************************************************************/
            bool isMatch = false;
            /*********************************************************************************************************
             * 开始处理请求数据
             * *******************************************************************************************************/
            try
            {
                DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() { 
                    {"deviceType",deviceType},
                    {"deviceCode",deviceCode}
                });
                if (cRs != null) { isMatch = true; }
            }
            catch { }
            /*********************************************************************************************************
             * 返回数据处理结果
             * *******************************************************************************************************/
            return isMatch;
        }
        #endregion
    }
}