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
                case "send": SendMessage(); Response.End(); break;
                case "message": SendMessage(); Response.End(); break;
                case "start": SaveRegister(); Response.End(); break;
            }
        }
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        protected void SendMessage()
        {
            /**********************************************************************************************************
             * 获取用户电话号码格式数据
             * ********************************************************************************************************/
            string strMobile = RequestHelper.GetRequest("Mobile").toString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("请填写用户登陆账号！"); Response.End(); }
            else if (strMobile.Length <= 5) { this.ErrorMessage("用户登陆账号不能少于6位数！"); Response.End(); }
            else if (strMobile.Length >= 16) { this.ErrorMessage("用户登陆账号不能超过16位数！"); Response.End(); }
            else if (strMobile.Length != 11) { this.ErrorMessage("手机号码格式错误！"); Response.End(); }
            else if (!strMobile.isMobile()) { this.ErrorMessage("手机号码格式错误！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"Username",strMobile}
            });
            if (cRs != null) { this.ErrorMessage("用户登陆账号已经存在了,您可以直接登陆！"); Response.End(); }
            /**********************************************************************************************************
             * 判断提交数据请求合法性
             * ********************************************************************************************************/
            string strKey = RequestHelper.GetRequest("strKey").toString();
            if (string.IsNullOrEmpty(strKey)) { this.ErrorMessage("数据验证失败！"); Response.End(); }
            else if (strKey.Length != 32) { this.ErrorMessage("验证数据信息错误！"); Response.End(); }
            string thisKey = string.Format("注册短信-|-|-{0}-|-|-注册短信", strMobile);
            thisKey = new Fooke.Function.String(thisKey).ToMD5().ToLower();
            if (thisKey.ToLower() != strKey.ToLower()) { this.ErrorMessage("数据验证失败,请重试！"); Response.End(); }
            /**********************************************************************************************************
             * 生成短信验证码并发送
             * ********************************************************************************************************/
            string SessionCode = new Random().Next(111111, 999999).ToString();
            string SendText = "您的验证码是:" + SessionCode + "";
            new MessageHelper().Start(Configure: this.Configure,
                    Mobile: strMobile,
                    SendText: SendText,
                    Fun: (iSuccess, strResponse) =>
                    {
                        if (!iSuccess) { this.ErrorMessage(strResponse); Response.End(); }
                    });
            /**********************************************************************************************************
             * 输出数据处理结果
             * ********************************************************************************************************/
            StringBuilder strXml = new StringBuilder();
            strXml.Append("{");
            strXml.Append("\"success\":\"true\"");
            strXml.Append(",\"tips\":\"短信发送成功\"");
            strXml.Append(",\"code\":\"" + SessionCode + "\"");
            strXml.Append("}");
            Response.Write(strXml.ToString());
            Response.End();
        }
        /// <summary>
        /// 用户注册
        /// </summary>
        protected void SaveRegister()
        {
            string isRegister = this.GetParameter("isRegister", "userXml").toInt();
            if (isRegister != "1") { this.ErrorMessage("当前用户系统不支持帐号注册！"); Response.End(); }
            /************************************************************************************************************
            * 判断IP地址是否已加入黑名单
            * ***********************************************************************************************************/
            string strBlack = this.GetParameter("strBlack", "userXml").toString();
            if (!string.IsNullOrEmpty(strBlack) && strBlack.Contains(FunctionCenter.GetCustomerIP()))
            { this.ErrorMessage("该IP登录地址已经被禁止！"); Response.End(); }
            /**************************************************************************************************************
             * 获取微信授权账户信息
             * ************************************************************************************************************/
            string AuthorizationType = RequestHelper.GetRequest("AuthorizationType").toString("Define");
            if (string.IsNullOrEmpty(AuthorizationType)) { this.ErrorMessage("获取第三方授权信息失败,请重试！"); Response.End(); }
            else if (AuthorizationType.Length <= 1) { this.ErrorMessage("第三方授权类型字段长度不能少于2个字符！"); Response.End(); }
            else if (AuthorizationType.Length >= 12) { this.ErrorMessage("第三方授权类型字段长度不能超过12个字符！"); Response.End(); }
            string AuthorizationKey = RequestHelper.GetRequest("AuthorizationKey").toString("DM" + Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(AuthorizationKey)) { this.ErrorMessage("获取第三方授权信息失败,请重试！"); Response.End(); }
            else if (AuthorizationKey.Length <= 12) { this.ErrorMessage("第三方授权标识字段长度不能少于12个字符！"); Response.End(); }
            else if (AuthorizationKey.Length >= 40) { this.ErrorMessage("第三方授权标识字段长度不能超过40个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 获取设备型号系统版本
             * ************************************************************************************************************/
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").toString("Define");
            if (string.IsNullOrEmpty(DeviceModel)) { this.ErrorMessage("获取手机设备信号信息失败,请重试！"); Response.End(); }
            else if (DeviceModel.Length <= 0) { this.ErrorMessage("手机设备型号字段不能少于0个字符！"); Response.End(); }
            else if (DeviceModel.Length >= 16) { this.ErrorMessage("手机设备型号字段不能大于16个字符！"); Response.End(); }
            string DeviceChar = RequestHelper.GetRequest("DeviceChar").toString("Define");
            if (string.IsNullOrEmpty(DeviceChar)) { this.ErrorMessage("获取手机系统版本信息失败,请重试！"); Response.End(); }
            else if (DeviceChar.Length <= 0) { this.ErrorMessage("手机系统版本字段不能少于1个字符！"); Response.End(); }
            else if (DeviceChar.Length >= 16) { this.ErrorMessage("手机系统版本字段不能大于16个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 获取手机设备数据信息
             * ************************************************************************************************************/
            string DeviceType = RequestHelper.GetRequest("DeviceType").toString("Define");
            if (string.IsNullOrEmpty(DeviceType)) { this.ErrorMessage("获取设备类型信息失败,请重试！"); Response.End(); }
            else if (DeviceType.Length <= 2) { this.ErrorMessage("设备类型字段不能少于2个字符！"); Response.End(); }
            else if (DeviceType.Length >= 12) { this.ErrorMessage("设备类型字段不能大于12个字符！"); Response.End(); }
            string DeviceCode = RequestHelper.GetRequest("DeviceCode").toString("DM" + Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(DeviceCode)) { this.ErrorMessage("获取设备编号信息失败,请重试！"); Response.End(); }
            else if (DeviceCode.Length <= 12) { this.ErrorMessage("设备编号信息字段不能少于12个字符！"); Response.End(); }
            else if (DeviceCode.Length >= 40) { this.ErrorMessage("设备编号信息字段不能大于40个字符！"); Response.End(); }
            string DeviceIdentifier = RequestHelper.GetRequest("DeviceIdentifier").toString("DM" + Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(DeviceIdentifier)) { this.ErrorMessage("获取系统唯一设备编号失败,请重试!"); Response.End(); }
            else if (DeviceIdentifier.Length <= 10) { this.ErrorMessage("设备系统唯一设备编码长度不能少于10个字符！"); Response.End(); }
            else if (DeviceIdentifier.Length >= 46) { this.ErrorMessage("设备系统唯一设备编码长度不能超过46个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 获取验证请求注册的合法性
             * ************************************************************************************************************/
            string VerificationKey = string.Format("注册验证-|-|-{0}-|-|-{1}-|-|-注册验证", DeviceType, DeviceCode);
            VerificationKey = new Fooke.Function.String(VerificationKey).ToMD5().ToLower();
            string force = RequestHelper.GetRequest("force").ToString();
            if (string.IsNullOrEmpty(force)) { this.ErrorMessage("非法提交请求数据,请重试！"); Response.End(); }
            else if (force.Length <= 0) { this.ErrorMessage("非法提交请求数据,请重试！"); Response.End(); }
            else if (VerificationKey != force) { this.ErrorMessage("数据验证错误,请重试！"); Response.End(); }
            /**************************************************************************************************************
             * 判断微信设备信息是否存在
             * ************************************************************************************************************/
            DataRow aRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"AuthorizationType",AuthorizationType},
                {"AuthorizationKey",AuthorizationKey}
            });
            if (aRs != null) { SaveUserReplacement(aRs); ResponseUserInfo(aRs); Response.End(); }
            /**************************************************************************************************************
             * 判断手机设备信息是否存在
             * ************************************************************************************************************/
            DataRow dRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"DeviceType",DeviceType},
                {"DeviceIdentifier",DeviceIdentifier}
            });
            if (dRs != null) { SaveUserReplacement(dRs); ResponseUserInfo(dRs); Response.End(); }
            /**************************************************************************************************************
             * 获取并验证MAC地址合法性
             * ************************************************************************************************************/
            string MacChar = RequestHelper.GetRequest("MacChar").toString(string.Format("DF{0}", DateTime.Now.Ticks.ToString()));
            if (string.IsNullOrEmpty(MacChar)) { this.ErrorMessage("获取设备MAC地址失败,请重试!"); Response.End(); }
            else if (MacChar.Length <= 10) { this.ErrorMessage("设备MAC地址长度不能少于10个字符！"); Response.End(); }
            else if (MacChar.Length >= 40) { this.ErrorMessage("设备MAC地址长度不能超过40个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 开始验证用户账户信息,并且验证登陆账号是否存在
             * ************************************************************************************************************/
            string UserName = RequestHelper.GetRequest("UserName").toString(getUsername());
            if (string.IsNullOrEmpty(UserName)) { this.ErrorMessage("请填写用户登陆账号！"); Response.End(); }
            else if (UserName.Length <= 5) { this.ErrorMessage("登陆账号太短,不能少于5个字符！"); Response.End(); }
            else if (UserName.Length >= 20) { this.ErrorMessage("登陆账号太长,不能多于20个字符！"); Response.End(); }
            else if (VerifyCenter.VerifySpecific(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            else if (VerifyCenter.VerifyChina(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserName",UserName}
            });
            if (cRs != null) { this.ErrorMessage("登陆手机号码已经被注册了!"); Response.End(); }
            /**************************************************************************************************************
             * 验证上级用户,顶级帐号
             * ************************************************************************************************************/
            string ParentID = GetParent(RequestHelper.GetRequest("ParentID").toInt());
            /**************************************************************************************************************
             * 验证其它的数据信息
             * ************************************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString("DuomiCMS");
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请填写你的登陆密码!"); Response.End(); }
            else if (Password.Length <= 5) { this.ErrorMessage("登陆密码不能少于6个字符!"); Response.End(); }
            else if (Password.Length > 16) { this.ErrorMessage("登陆密码不能大于16个字符"); Response.End(); }
            else { Password = MemberHelper.toPassword(Password); }
            /**************************************************************************************************************
             * 二级密码,资金密码
             * ************************************************************************************************************/
            string PasswordTo = RequestHelper.GetRequest("PasswordTo").toString("DuomiCMS");
            if (string.IsNullOrEmpty(PasswordTo)) { PasswordTo = RequestHelper.GetRequest("Password").toString(); }
            else if (PasswordTo.Length <= 5) { this.ErrorMessage("二级密码不能小于6个字符!"); Response.End(); }
            else if (PasswordTo.Length > 36) { this.ErrorMessage("二级密码不能大于36个字符!"); Response.End(); }
            else { PasswordTo = MemberHelper.toPassword(PasswordTo); }
            /**************************************************************************************************************
             * 验证用户昵称帐号是否唯一
             * ************************************************************************************************************/
            string Nickname = RequestHelper.GetRequest("Nickname").toString();
            if (string.IsNullOrEmpty(Nickname)) { Nickname ="HH"+ new Random().Next(11111111, 99999999).ToString(); }
            else if (Nickname.Length <= 0) { this.ErrorMessage("账户昵称不能少于0个字符!"); Response.End(); }
            else if (Nickname.Length >= 32) { this.ErrorMessage("账户昵称不能大于32个字符!"); Response.End(); }
            DataRow nRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"Nickname",Nickname} });
            if (nRs != null) { Nickname = "HH" + new Random().Next(11111111, 99999999).ToString(); }
            /**************************************************************************************************************
             * 验证用户资料
             * ************************************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString("/file/user/default.png");
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("获取用户头像地址信息失败,请重试！"); Response.End(); }
            else if (Thumb.Length <= 10) { this.ErrorMessage("获取用户头像地址信息失败,请重试！"); Response.End(); }
            else if (Thumb.Length >= 255) { this.ErrorMessage("用户头像地址太长,请限制在255个字符以内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证用户微信昵称，真实姓名等
             * ************************************************************************************************************/
            string strWeChat = RequestHelper.GetRequest("strWeChat").ToString();
            if (strWeChat.Length >= 24) { this.ErrorMessage("微信昵称长度不能超过24个字符！"); Response.End(); }
            string Fullname = RequestHelper.GetRequest("Fullname").ToString();
            if (Fullname.Length >= 12) { this.ErrorMessage("真实姓名长度不能超过12个汉字！"); Response.End(); }
            /**************************************************************************************************************
             * 获取支付宝账号昵称数据信息
             * ************************************************************************************************************/
            string AlipayChar = RequestHelper.GetRequest("AlipayChar").ToString();
            if (AlipayChar.Length >= 24) { this.ErrorMessage("支付宝账号长度不能超过24个字符!"); Response.End(); }
            string Alipayname = RequestHelper.GetRequest("Alipayname").ToString();
            if (Alipayname.Length >= 16) { this.ErrorMessage("支付宝昵称长度不能超过16个汉字！"); Response.End(); }
            /**************************************************************************************************************
             * 获取并验证用户账号信息
             * ************************************************************************************************************/
            string strEmail = RequestHelper.GetRequest("strEmail").ToString();
            if (strEmail.Length > 26) { this.ErrorMessage("邮箱地址字段长度请限制在26个字符以内！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strEmail) && !strEmail.isEmail()) { this.ErrorMessage("邮箱地址字段格式错误,请重试！"); Response.End(); }
            string strMobile = RequestHelper.GetRequest("strMobile").toString();
            if (!string.IsNullOrEmpty(strMobile) && strMobile.Length != 11) { this.ErrorMessage("手机号格式错误,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strMobile) && !strMobile.isMobile()) { this.ErrorMessage("手机号格式错误！"); Response.End(); }
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
            oDictionary["AuthorizationType"] = AuthorizationType;
            oDictionary["AuthorizationKey"] = AuthorizationKey;
            oDictionary["DeviceType"] = DeviceType;
            oDictionary["DeviceCode"] = DeviceCode;
            oDictionary["DeviceIdentifier"] = DeviceIdentifier;
            oDictionary["DeviceModel"] = DeviceModel;
            oDictionary["DeviceChar"] = DeviceChar;
            oDictionary["MacChar"] = MacChar;
            oDictionary["strTokey"] = strTokey;
            oDictionary["UserName"] = UserName;
            oDictionary["Password"] = Password;
            oDictionary["PasswordTo"] = PasswordTo;
            oDictionary["Nickname"] = Nickname;
            oDictionary["Thumb"] = Thumb;
            oDictionary["strMobile"] = strMobile;
            oDictionary["strEmail"] = strEmail;
            oDictionary["strWeChat"] = strWeChat;
            oDictionary["Fullname"] = Fullname;
            oDictionary["AlipayChar"] = AlipayChar;
            oDictionary["Alipayname"] = Alipayname;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUsers]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*************************************************************************************************************
            * 发放邀请奖励信息
            * ***********************************************************************************************************/
            string isBonus = this.GetParameter("isBonus", "shareXml").toInt();
            string shareModel = this.GetParameter("shareModel", "shareXml").toInt();
            if (isBonus == "1" && shareModel == "0")
            {
                try { new BonusHelper().ShareBonus(thisRs["UserID"].ToString()); }
                catch { }
            }
            /*************************************************************************************************************
            * 保存用户登陆记录信息
            * ***********************************************************************************************************/
            CookieHelper.Add("Fooke_MemberKey", thisRs["strTokey"].ToString(), 90);
            CookieHelper.Add("Fooke_MemberID", thisRs["UserID"].ToString(), 90);
            CookieHelper.Add("Fooke_MemberName", thisRs["UserName"].ToString(), 90);
            /************************************************************************************************************
             * 开始保存用户数据信息
             * ***********************************************************************************************************/
            ResponseUserInfo(thisRs);
            Response.End();
        }

        /**************************************************************************************************************
        * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
        * 输出用户处理数据信息
        * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
        * *************************************************************************************************************/
        public void ResponseUserInfo(DataRow MemberRs)
        {
            /***********************************************************************************************************
             * 声明数据输出变量
             * *********************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            /***********************************************************************************************************
             * 构建输出内容信息
             * *********************************************************************************************************/
            strBuilder.Append(",\"userid\":\"" + MemberRs["userid"] + "\"");
            strBuilder.Append(",\"parentid\":\"" + MemberRs["ParentID"] + "\"");
            strBuilder.Append(",\"authorizationtype\":\"" + MemberRs["AuthorizationType"] + "\"");
            strBuilder.Append(",\"authorizationkey\":\"" + MemberRs["AuthorizationKey"] + "\"");
            strBuilder.Append(",\"devicetype\":\"" + MemberRs["DeviceType"] + "\"");
            strBuilder.Append(",\"devicemodel\":\"" + MemberRs["DeviceModel"] + "\"");
            strBuilder.Append(",\"devicecode\":\"" + MemberRs["DeviceCode"] + "\"");
            strBuilder.Append(",\"deviceidentifier\":\"" + MemberRs["DeviceIdentifier"] + "\"");
            strBuilder.Append(",\"macchar\":\"" + MemberRs["MacChar"] + "\"");
            strBuilder.Append(",\"strtokey\":\"" + MemberRs["strTokey"] + "\"");
            strBuilder.Append(",\"groupid\":\"" + MemberRs["GroupID"] + "\"");
            strBuilder.Append(",\"groupname\":\"" + MemberRs["Groupname"] + "\"");
            strBuilder.Append(",\"username\":\"" + MemberRs["UserName"] + "\"");
            strBuilder.Append(",\"thumb\":\"" + MemberRs["Thumb"] + "\"");
            strBuilder.Append(",\"nickname\":\"" + MemberRs["Nickname"] + "\"");
            strBuilder.Append(",\"amount\":\"" + MemberRs["Amount"] + "\"");
            strBuilder.Append(",\"points\":\"" + MemberRs["Points"] + "\"");
            strBuilder.Append(",\"stremail\":\"" + MemberRs["strEmail"] + "\"");
            strBuilder.Append(",\"strmobile\":\"" + MemberRs["strMobile"] + "\"");
            strBuilder.Append(",\"strwechat\":\"" + MemberRs["strWeChat"] + "\"");
            strBuilder.Append(",\"strip\":\"" + MemberRs["strIP"] + "\"");
            strBuilder.Append(",\"strcity\":\"" + MemberRs["strcity"] + "\"");
            strBuilder.Append(",\"isdisplay\":\"" + MemberRs["isdisplay"] + "\"");
            /***********************************************************************************************************
             * 输出数据处理结果
             * *********************************************************************************************************/
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
        public string GetParent(string ParentID)
        {
            /**************************************************************************************************************
             * 开始执行数据请求
             * ************************************************************************************************************/
            string ShareMust = this.GetParameter("share", "userXml").toInt();
            if (ShareMust == "1" && ParentID=="0") { this.ErrorMessage("请填写注册邀请码！"); Response.End(); }
            else if (ShareMust == "1" && ParentID.Length<=5) { this.ErrorMessage("请填写注册邀请码！"); Response.End(); }
            else if (ParentID.Length>=10) { this.ErrorMessage("邀请码格式错误,请重试！"); Response.End(); }
            if (ParentID != "0")
            {
                DataRow FormatRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                   {"UserID",ParentID}
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

        /// <summary>
        /// 更新用户登陆设备账号信息
        /// </summary>
        /// <param name="MemberRs"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public DataRow SaveUserReplacement(DataRow MemberRs)
        {
            /**************************************************************************************************************
             * 获取微信授权账户信息
             * ************************************************************************************************************/
            string AuthorizationType = RequestHelper.GetRequest("AuthorizationType").toString("Define");
            if (string.IsNullOrEmpty(AuthorizationType)) { this.ErrorMessage("获取第三方授权信息失败,请重试！"); Response.End(); }
            else if (AuthorizationType.Length <= 2) { this.ErrorMessage("第三方授权类型字段长度不能少于2个字符！"); Response.End(); }
            else if (AuthorizationType.Length >= 12) { this.ErrorMessage("第三方授权类型字段长度不能超过12个字符！"); Response.End(); }
            string AuthorizationKey = RequestHelper.GetRequest("AuthorizationKey").toString("DM" + Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(AuthorizationKey)) { this.ErrorMessage("获取第三方授权信息失败,请重试！"); Response.End(); }
            else if (AuthorizationKey.Length <= 12) { this.ErrorMessage("第三方授权标识字段长度不能少于12个字符！"); Response.End(); }
            else if (AuthorizationKey.Length >= 40) { this.ErrorMessage("第三方授权标识字段长度不能超过40个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 获取设备型号系统版本
             * ************************************************************************************************************/
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").toString("Define");
            if (string.IsNullOrEmpty(DeviceModel)) { this.ErrorMessage("获取手机设备信号信息失败,请重试！"); Response.End(); }
            else if (DeviceModel.Length <= 0) { this.ErrorMessage("手机设备型号字段不能少于2个字符！"); Response.End(); }
            else if (DeviceModel.Length >= 16) { this.ErrorMessage("手机设备型号字段不能大于16个字符！"); Response.End(); }
            string DeviceChar = RequestHelper.GetRequest("DeviceChar").toString("Define");
            if (string.IsNullOrEmpty(DeviceChar)) { this.ErrorMessage("获取手机系统版本信息失败,请重试！"); Response.End(); }
            else if (DeviceChar.Length <= 0) { this.ErrorMessage("手机系统版本字段不能少于2个字符！"); Response.End(); }
            else if (DeviceChar.Length >= 16) { this.ErrorMessage("手机系统版本字段不能大于16个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 获取手机设备数据信息
             * ************************************************************************************************************/
            string DeviceType = RequestHelper.GetRequest("DeviceType").toString("Define");
            if (string.IsNullOrEmpty(DeviceType)) { this.ErrorMessage("获取设备类型信息失败,请重试！"); Response.End(); }
            else if (DeviceType.Length <= 2) { this.ErrorMessage("设备类型字段不能少于2个字符！"); Response.End(); }
            else if (DeviceType.Length >= 12) { this.ErrorMessage("设备类型字段不能大于12个字符！"); Response.End(); }
            string DeviceCode = RequestHelper.GetRequest("DeviceCode").toString("DM" + Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(DeviceCode)) { this.ErrorMessage("获取设备编号信息失败,请重试！"); Response.End(); }
            else if (DeviceCode.Length <= 12) { this.ErrorMessage("设备编号信息字段不能少于12个字符！"); Response.End(); }
            else if (DeviceCode.Length >= 40) { this.ErrorMessage("设备编号信息字段不能大于40个字符！"); Response.End(); }
            string DeviceIdentifier = RequestHelper.GetRequest("DeviceIdentifier").toString("DM" + Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(DeviceIdentifier)) { this.ErrorMessage("获取系统唯一设备编号失败,请重试!"); Response.End(); }
            else if (DeviceIdentifier.Length <= 10) { this.ErrorMessage("设备系统唯一设备编码长度不能少于10个字符！"); Response.End(); }
            else if (DeviceIdentifier.Length >= 46) { this.ErrorMessage("设备系统唯一设备编码长度不能超过46个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 验证需要保存的请求数据项目信息
             * ************************************************************************************************************/
            Dictionary<string, string> thisDictionary = new Dictionary<string, string>();
            if (MemberRs["DeviceType"].ToString() != DeviceType && DeviceType != "Define") { thisDictionary["DeviceType"] = DeviceType; }
            if (MemberRs["DeviceCode"].ToString() != DeviceCode && !DeviceCode.StartsWith("DM")) { thisDictionary["DeviceCode"] = DeviceCode; }
            if (MemberRs["DeviceIdentifier"].ToString() != DeviceIdentifier && !DeviceIdentifier.StartsWith("DM")) { thisDictionary["DeviceIdentifier"] = DeviceIdentifier; }
            if (MemberRs["DeviceModel"].ToString() != DeviceModel && DeviceModel != "Define") { thisDictionary["DeviceModel"] = DeviceModel; }
            if (MemberRs["DeviceChar"].ToString() != DeviceChar && DeviceChar != "Define") { thisDictionary["DeviceChar"] = DeviceChar; }
            if (MemberRs["AuthorizationType"].ToString() != AuthorizationType && AuthorizationType != "Define") { thisDictionary["AuthorizationType"] = AuthorizationType; }
            if (MemberRs["AuthorizationKey"].ToString() != AuthorizationKey && !AuthorizationKey.StartsWith("DM")) { thisDictionary["AuthorizationKey"] = AuthorizationKey; }
            /**************************************************************************************************************
             * 判断是否存在数据保存项目,否则执行数据保存
             * ************************************************************************************************************/
            if (thisDictionary != null && thisDictionary.Count != 0)
            {
                DbHelper.Connection.Update(tablename: "Fooke_User",
                    dictionary: thisDictionary,
                    Params: " and UserID=" + MemberRs["UserID"] + "");
            }
            /**************************************************************************************************************
             * 返回数据处理结果
             * ************************************************************************************************************/
            return MemberRs;
        }
        /// <summary>
        /// 获取用户账号信息
        /// </summary>
        /// <returns></returns>
        public string getUsername()
        {
            string userChar = string.Format("DM{0}{1}",
                    DateTime.Now.ToString("yyMMddHHmm"),
                    new Random().Next(111111, 999999).ToString());
            return new Fooke.Function.String(userChar).Substring(0, 16).ToUpper();
        }

        #endregion

    }
}