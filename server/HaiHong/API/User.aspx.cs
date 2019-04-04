using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using Fooke.Function;
using Fooke.Code;
namespace Fooke.Web.API
{
    public partial class User : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "SaveAccess": SaveAccess(); Response.End(); break;
                case "SendMobile": SendMobile(); Response.End(); break;
                case "SaveThumb": SaveThumb(); Response.End(); break;
                case "SaveEmail": SaveEmail(); Response.End(); break;
                case "SaveMobile": SaveMobile(); Response.End(); break;
                case "SaveAlipay": SaveAlipay(); Response.End(); break;
                case "SavePasswordTo": SavePasswordTo(); Response.End(); break;
                case "SavePassword": SavePassword(); Response.End(); break;
                case "password": VerificationPassword(); Response.End(); break;
                case "showMember": ShowDetails(); Response.End(); break;
                case "savewechat": SaveWeChat(); Response.End(); break;
                case "saveprofile": SaveProfile(); Response.End(); break;
                case "computer": strComputer(); Response.End(); break;
                case "rookie": SaveRookie(); Response.End(); break;
                case "showrookie": ShowRookie(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }

        protected void strComputer()
        {
            /****************************************************************************************************************
             * 获取用户邀请奖励信息
             * **************************************************************************************************************/
            DataRow TotalRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserInvitedComputer]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")},
                {"DateKey",DateTime.Now.ToString("yyyyMMdd")}
            });
            if (TotalRs == null) { this.ErrorMessage("统计用户邀请数据信息失败!"); Response.End(); }
            /****************************************************************************************************************
             * 获取用户邀请奖励信息
             * **************************************************************************************************************/
            ResponseDataRow(TotalRs);
            Response.End();
        }

        /// <summary>
        /// 查看指定的用户信息
        /// </summary>
        protected void ShowDetails()
        {
            /********************************************************************************************
            * 查看用户请求参数信息
            * ******************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("获取请求数据错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs != null && cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("账号已停止使用！"); Response.End(); }
            /********************************************************************************************
             * 构建网页输出内容
             * ******************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"userid\":\"" + cRs["UserID"] + "\"");
            strBuilder.Append(",\"parentid\":\"" + cRs["ParentID"] + "\"");
            strBuilder.Append(",\"devicetype\":\"" + cRs["DeviceType"] + "\"");
            strBuilder.Append(",\"devicecode\":\"" + cRs["DeviceCode"] + "\"");
            strBuilder.Append(",\"username\":\"" + cRs["Username"] + "\"");
            strBuilder.Append(",\"organizationid\":\"" + cRs["OrganizationID"] + "\"");
            strBuilder.Append(",\"organizationname\":\"" + cRs["OrganizationName"] + "\"");
            strBuilder.Append(",\"usermodel\":\"" + cRs["UserModel"] + "\"");
            strBuilder.Append(",\"amount\":\"0\"");
            strBuilder.Append(",\"amtwallet\":\"" + cRs["AmtWallet"] + "\"");
            strBuilder.Append(",\"forceamount\":\"0\"");
            strBuilder.Append(",\"fcewallet\":\"" + cRs["FceWallet"] + "\"");
            strBuilder.Append(",\"appamount\":\"0\"");
            strBuilder.Append(",\"appwallet\":\"" + cRs["AppWallet"] + "\"");
            strBuilder.Append(",\"faith\":\"" + cRs["Faith"] + "\"");
            strBuilder.Append(",\"isauthentication\":\"0\"");
            strBuilder.Append(",\"isvip\":\"" + cRs["isVIP"] + "\"");
            strBuilder.Append(",\"thumb\":\"" + cRs["Thumb"] + "\"");
            strBuilder.Append(",\"nickname\":\"" + cRs["Nickname"] + "\"");
            strBuilder.Append(",\"stremail\":\"" + cRs["strEmail"] + "\"");
            strBuilder.Append(",\"strmobile\":\"" + cRs["strMobile"] + "\"");
            strBuilder.Append(",\"addtime\":\"" + cRs["Addtime"] + "\"");
            strBuilder.Append("}");
            /********************************************************************************************
             * 输出数据处理结果
             * ******************************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 更改手机号码,更改账户
        /// </summary>
        protected void SaveAccess()
        {
            /*********************************************************************************************
             * 验证请求参数的合法性
             * ********************************************************************************************/
            string Access = RequestHelper.GetRequest("thisAccess").toString();
            if (string.IsNullOrEmpty(Access)) { this.ErrorMessage("请填写新的手机号码！"); Response.End(); }
            else if (Access.Length != 11) { this.ErrorMessage("手机号格式错误,请检查！"); Response.End(); }
            else if (!Access.isMobile()) { this.ErrorMessage("新账户手机号码格式错误！"); Response.End(); }
            if (MemberRs["Username"].ToString() == Access) { this.ErrorMessage("手机号码没有发生更改,请检查！"); Response.End(); }
            /*********************************************************************************************
             * 验证短信验证码
             * ********************************************************************************************/
            string SessionCode = RequestHelper.GetRequest("Captcha").toString();
            if (string.IsNullOrEmpty(SessionCode)) { this.ErrorMessage("请填写短信验证码！"); Response.End(); }
            else if (MemberRs["SessionCode"].ToString() != SessionCode) { this.ErrorMessage("短信验证码错误！"); Response.End(); }
            /*********************************************************************************************
             * 验证请求账号是否存在
             * ********************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"Username",Access}
            });
            if (cRs != null) { this.ErrorMessage("您当前要绑定的帐号已经存在！"); Response.End(); }
            /*********************************************************************************************
             * 开始保存请求数据
             * ********************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, dictionary: new Dictionary<string, string>() {
                {"Username",Access},
                {"strMobile",Access}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /*********************************************************************************************
             * 输出数据处理结果
             * ********************************************************************************************/
            this.ErrorMessage("用户新手机号绑定成功！", iSuccess: true);
            Response.End();
        }

        /// <summary>
        /// 更改手机号码,重置手机号码
        /// </summary>
        protected void SendMobile()
        {
            /************************************************************************************************
             * 获取用户发送短信类型
             * ***********************************************************************************************/
            string strMode = RequestHelper.GetRequest("mode").toString("default");
            /************************************************************************************************
             * 设置用户默认账号与手机号信息?
             * ***********************************************************************************************/
            /************************************************************************************************
             * 获取接收短信的手机号码,并验证手机号是否合法
             * ***********************************************************************************************/
            string strMobile = RequestHelper.GetRequest("strMobile").toString();
            if (strMobile.Length <= 0 && MemberRs["strMobile"].ToString().Length == 11 && MemberRs["strMobile"].ToString().isMobile()) { strMobile = MemberRs["strMobile"].ToString(); }
            if (strMobile.Length <= 0 && MemberRs["Username"].ToString().Length == 11 && MemberRs["Username"].ToString().isMobile()) { strMobile = MemberRs["Username"].ToString(); }
            /************************************************************************************************
             * 验证接收短信手机号是否合法
             * ***********************************************************************************************/
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("您还没有绑定手机,请先绑定手机！"); Response.End(); }
            else if (strMobile.Length <= 5) { this.ErrorMessage("您还没有绑定手机,请先绑定手机！"); Response.End(); }
            else if (strMobile.Length >= 16) { this.ErrorMessage("绑定手机号码格式错误,请重新绑定！"); Response.End(); }
            else if (strMobile.Length != 11) { this.ErrorMessage("您还没有绑定手机,请先绑定手机！"); Response.End(); }
            else if (!strMobile.isMobile()) { this.ErrorMessage("绑定手机号码格式错误,请联系客服！"); Response.End(); }
            /************************************************************************************************
             * 验证用户绑定手机号码是否合法
             * ***********************************************************************************************/
            if ((MemberRs["Username"].ToString().Length == 11 && MemberRs["Username"].ToString().isMobile() && MemberRs["Username"].ToString() != strMobile)
            && (MemberRs["strMobile"].ToString().Length == 11 && MemberRs["strMobile"].ToString().isMobile() && MemberRs["strMobile"].ToString() != strMobile))
            {
                this.ErrorMessage("无效的手机号码,请重试!"); Response.End();
            }
            /************************************************************************************************
             * 生成用户新短信验证码
             * ***********************************************************************************************/
            string SessionCode = new Random().Next(111111, 999999).ToString();
            DbHelper.Connection.Update(TableCenter.User, dictionary: new Dictionary<string, string>() {
                {"SessionCode",SessionCode},
                {"SessionDate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /************************************************************************************************
            * 开始发送短信
            * ***********************************************************************************************/
            string servername = this.GetParameter("servername", "MessageXml").toString();
            string SendText = string.Empty;
            if (servername == "鲸鱼")
            {
                SendText = "你的短信验证码是 " + SessionCode + "";
                if (!string.IsNullOrEmpty(strMode) && strMode.ToLower() == ("SaveAccess").ToLower())
                { SendText = "您正在更改手机号，验证码：" + SessionCode + "，请于3分钟内正确输入。请勿向任何人透露本验证码。如非本人操作，请联系客服"; }
                else if (!string.IsNullOrEmpty(strMode) && strMode.ToLower() == ("SavePasswordTo").ToLower())
                { SendText = "您正在设置交易密码，验证码：" + SessionCode + "，请于3分钟内正确输入。请勿向任何人透露本验证码。"; }
            }
            else if (servername == "创蓝")
            {
               SendText = "您的验证码是：" + SessionCode + "，3分钟内有效。";
            }
            /************************************************************************************************
            * 准备发送短信验证码
            * ***********************************************************************************************/
            new MessageHelper().Start(Configure: this.Configure,
                   Mobile: strMobile,
                   SendText: SendText,
                   Fun: (iSuccess, strResponse) =>
                   {
                       if (!iSuccess) { this.ErrorMessage(strResponse); Response.End(); }
                   });
            /************************************************************************************************
            * 输出短信发送结果
            * ***********************************************************************************************/
            StringBuilder strXml = new StringBuilder();
            strXml.Append("{");
            strXml.Append("\"success\":\"true\"");
            strXml.Append(",\"tips\":\"短信验证码发送成功\"");
            strXml.Append(",\"code\":\"000000\"");
            strXml.Append("}");
            Response.Write(strXml.ToString());
            Response.End();
        }
        /// <summary>
        /// 允许上传的头像的文件格式
        /// </summary>
        private readonly string SuffixTxt = "jpg|png|gif";
        /// <summary>
        /// 设置我的用户头像
        /// </summary>
        protected void SaveThumb()
        {
            /***********************************************************************************
             * 验证获取上传图片合法性
             * *********************************************************************************/
            if (Request.Files == null) { this.JSONMessage("请选择要上传的文件！"); Response.End(); }
            if (Request.Files.Count <= 0) { this.JSONMessage("请选择要上传的文件！"); Response.End(); }
            /***********************************************************************************
             * 验证上传文件合法性
             * *********************************************************************************/
            HttpPostedFile thisPosted = ((HttpPostedFile)(Request.Files[0]));
            if (thisPosted.ContentLength <= 0) { this.JSONMessage("获取上传文件内容信息失败,请重试！"); Response.End(); }
            else if (thisPosted.ContentLength >= (1024 * 1024 * 50)) { this.JSONMessage("单个文件资源大小请限制在50M以内！"); Response.End(); }
            else if (string.IsNullOrEmpty(thisPosted.FileName)) { this.JSONMessage("获取上传文件格式失败,请重试！"); Response.End(); }
            else if (!thisPosted.FileName.Contains(".")) { this.JSONMessage("获取上传文件格式失败,请重试！"); Response.End(); }
            /***********************************************************************************
             * 获取文件后缀名,并且判断是否合法
             * *********************************************************************************/
            string strFlter = thisPosted.FileName.Substring(thisPosted.FileName.LastIndexOf(".") + 1);
            if (strFlter.Length <= 1) { this.JSONMessage("获取上传文件格式错误,请重试！"); Response.End(); }
            else if (strFlter.Length >= 12) { this.JSONMessage("获取上传文件格式错误,请重试！"); Response.End(); }
            else if (!("jpg|jpeg|png|bmp").Contains(strFlter.ToLower())) { this.JSONMessage("获取上传文件格式错误,只允许上传图片与视频文件！"); Response.End(); }

            string strResponse = MemberRs["Thumb"].ToString();
            /***********************************************************************************
             * 开始上传图片
             * *********************************************************************************/
            try
            {
                string fileName = string.Format("上传文件-|-|-{0}-|-|-{1}", DateTime.Now.ToString("yyyyMMddHHmm"), MemberRs["UserID"].ToString());
                fileName = new Fooke.Function.String(fileName).ToMD5().Substring(0, 16).ToLower();
                fileName = fileName + ".{exc}";
                new PostedHelper().SaveAs(Request.Files[0], new Fooke.Function.PostedHelper.FileMode()
                {
                    fileName = fileName,
                    fileDirectory = "fileuser",
                    fileExt = "jpg|png|bmp",
                    fileSize = 1024 * 1024 * 2,
                    Success = (Thumb) =>
                    {
                        DbHelper.Connection.Update(TableCenter.User, dictionary: new Dictionary<string, string>() {
                            {"Thumb",Thumb}
                        }, Params: " and UserID=" + MemberRs["UserID"] + "");
                        strResponse = Thumb;
                    },
                    Error = (Exp) => { this.JSONMessage(Exp); Response.End(); }
                });
            }
            catch { }
            /***********************************************************************************
             * 输出数据处理结果信息
             * *********************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"" + strResponse + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 设置我的手机号码
        /// </summary>
        protected void SaveMobile()
        {
            /************************************************************************************************************
            * 获取并验证手机号码信息
            * ***********************************************************************************************************/
            string strMobile = RequestHelper.GetRequest("strMobile").ToString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("手机号码格式错误,必须为11位纯数字!"); Response.End(); }
            else if (strMobile.Length != 11) { this.ErrorMessage("手机号码格式错误,必须为11位纯数字！"); Response.End(); }
            else if (!strMobile.isMobile()) { this.ErrorMessage("手机号码格式错误,必须为11位纯数字！"); Response.End(); }
            /************************************************************************************************************
            * 获取并验证短信验证码合法性
            * ***********************************************************************************************************/
            string SessionCode = RequestHelper.GetRequest("Captcha").toString();
            if (SessionCode.Length <= 0) { this.ErrorMessage("请填写短信验证码！"); Response.End(); }
            else if (SessionCode.Length != 6) { this.ErrorMessage("短信验证码格式错误！"); Response.End(); }
            else if (MemberRs["SessionCode"].ToString() != SessionCode) { this.ErrorMessage("短信验证码错误！"); Response.End(); }
            else if (new Fooke.Function.String(MemberRs["SessionDate"].ToString()).cDate() <= DateTime.Now.AddMinutes(-30))
            { this.ErrorMessage("短信验证码已经过期,请重新获取！"); Response.End(); }
            /*************************************************************************************************************
             * 验证支付宝账号是否已被绑定
             * ***********************************************************************************************************/
            string MobileRepeat = this.GetParameter("MobileRepeat", "userXml").toInt();
            if (MobileRepeat == "0")
            {
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                    {"strMobile",strMobile}
                });
                if (sRs != null && sRs["UserID"].ToString() != MemberRs["UserID"].ToString())
                { this.ErrorMessage("手机号已经被绑定了,请换一个手机号再绑定吧!"); Response.End(); }
            }
            /************************************************************************************************************
            * 开始保存数据信息
            * ***********************************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, dictionary: new Dictionary<string, string>() {
                {"strMobile",strMobile}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /************************************************************************************************************
            * 输出数据处理结果
            * ***********************************************************************************************************/
            this.ErrorMessage("手机号码绑定成功！", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 用户绑定支付宝账号
        /// </summary>
        protected void SaveAlipay()
        {
            /*************************************************************************************************************
             * 获取并验证支付宝账号合法性
             * ***********************************************************************************************************/
            string AlipayChar = RequestHelper.GetRequest("AlipayChar").ToString();
            if (string.IsNullOrEmpty(AlipayChar)) { this.ErrorMessage("支付宝账号不能为空！"); Response.End(); }
            else if (AlipayChar.Length <= 0) { this.ErrorMessage("支付宝账号不能为空！"); Response.End(); }
            else if (AlipayChar.Length <= 5) { this.ErrorMessage("支付宝账号长度不能少于6个字符！"); Response.End(); }
            else if (AlipayChar.Length >= 30) { this.ErrorMessage("支付宝账号长度不能超过30个字符！"); Response.End(); }
            /*************************************************************************************************************
             * 获取并验证支付宝密码的合法性
             * ***********************************************************************************************************/
            string Alipayname = RequestHelper.GetRequest("Alipayname").ToString();
            if (string.IsNullOrEmpty(Alipayname)) { this.ErrorMessage("支付宝昵称不能为空！"); Response.End(); }
            else if (Alipayname.Length <= 0) { this.ErrorMessage("支付宝昵称不能为空！"); Response.End(); }
            else if (Alipayname.Length >= 24) { this.ErrorMessage("支付宝昵称长度不能超过24个字符！"); Response.End(); }
            /*************************************************************************************************************
             * 验证支付宝账号是否已被绑定
             * ***********************************************************************************************************/
            string AliRepeat = this.GetParameter("AliRepeat", "userXml").toInt();
            if (AliRepeat == "0")
            {
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                    {"AlipayChar",AlipayChar}
                });
                if (sRs != null && sRs["UserID"].ToString() != MemberRs["UserID"].ToString())
                { this.ErrorMessage("支付宝账号已经被绑定了,请换一个支付宝绑定吧!"); Response.End(); }
            }
            /*************************************************************************************************************
             * 保存用户支付宝账号数据信息
             * ***********************************************************************************************************/
            DbHelper.Connection.Update("Fooke_User", dictionary: new Dictionary<string, string>() { 
                {"AlipayChar",AlipayChar},
                {"Alipayname",Alipayname}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /*************************************************************************************************************
             * 输出数据处理结果
             * ***********************************************************************************************************/
            this.ErrorMessage("支付宝绑定成功！", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 绑定用户微信账号信息
        /// </summary>
        protected void SaveWeChat()
        {
            /**************************************************************************************************************
             * 获取微信授权账户信息
             * ************************************************************************************************************/
            string AuthorizationType = RequestHelper.GetRequest("AuthorizationType").toString("weixin");
            if (string.IsNullOrEmpty(AuthorizationType)) { this.ErrorMessage("获取微信授权类型信息失败,请重试！"); Response.End(); }
            else if (AuthorizationType.Length <= 1) { this.ErrorMessage("微信授权类型字段长度不能少于2个字符！"); Response.End(); }
            else if (AuthorizationType.Length >= 12) { this.ErrorMessage("微信授权类型字段长度不能超过12个字符！"); Response.End(); }
            string AuthorizationKey = RequestHelper.GetRequest("AuthorizationKey", false).toString();
            if (string.IsNullOrEmpty(AuthorizationKey)) { this.ErrorMessage("获取第三方授权信息失败,请重试！"); Response.End(); }
            else if (AuthorizationKey.Length <= 8) { this.ErrorMessage("第三方授权标识字段长度不能少于12个字符！"); Response.End(); }
            else if (AuthorizationKey.Length >= 40) { this.ErrorMessage("第三方授权标识字段长度不能超过40个字符！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"AuthorizationType",AuthorizationType},
                {"AuthorizationKey",AuthorizationKey}
            });
            if (cRs != null) { this.ErrorMessage("微信账号信息已经被其他用户绑定了!"); Response.End(); }
            /**************************************************************************************************************
             * 开始保存请求数据信息
             * ************************************************************************************************************/
            DbHelper.Connection.Update("Fooke_User", dictionary: new Dictionary<string, string>() { 
                {"AuthorizationType",AuthorizationType},
                {"AuthorizationKey",AuthorizationKey}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /**************************************************************************************************************
             * 输出数据处理结果信息
             * ************************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"type\":\"define\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 设置我的电子邮箱
        /// </summary>
        protected void SaveEmail()
        {
            /************************************************************************************************************
            * 获取并验证手机号码信息
            * ***********************************************************************************************************/
            string strEmail = RequestHelper.GetRequest("strEmail").ToString();
            if (string.IsNullOrEmpty(strEmail)) { this.ErrorMessage("电子邮箱不能少于6个字符!"); Response.End(); }
            if (strEmail.Length <= 6) { this.ErrorMessage("电子邮箱不能少于6个字符!"); Response.End(); }
            else if (strEmail.Length >= 26) { this.ErrorMessage("电子邮箱字段长度请限制在26个字符内！"); Response.End(); }
            else if (!strEmail.Contains("@")) { this.ErrorMessage("电子邮箱格式错误,请重试！"); Response.End(); }
            else if (!strEmail.isEmail()) { this.ErrorMessage("电子邮箱格式错误,请重试！"); Response.End(); }
            /************************************************************************************************************
            * 开始保存数据信息
            * ***********************************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, dictionary: new Dictionary<string, string>() {
                {"strEmail",strEmail}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /************************************************************************************************************
            * 输出数据处理结果
            * ***********************************************************************************************************/
            this.ErrorMessage("电子邮箱设置成功！", iSuccess: true);
            Response.End();
        }

        /// <summary>
        /// 设置用户登录密码
        /// </summary>
        protected void SavePassword()
        {
            /************************************************************************************************
            * 验证旧的登陆密码的合法性
            * ***********************************************************************************************/
            string OldPassword = RequestHelper.GetRequest("OldPassword").toString();
            if (string.IsNullOrEmpty(OldPassword)) { this.ErrorMessage("请填写你上次登录的密码！"); Response.End(); }
            else if (OldPassword.Length <= 5) { this.ErrorMessage("旧登陆密码长度不能少于6位数！"); Response.End(); }
            else if (OldPassword.Length >= 16) { this.ErrorMessage("旧登陆密码长度不能大于16位数！"); Response.End(); }
            else if (MemberHelper.toPassword(OldPassword) != MemberRs["Password"].ToString()) { this.ErrorMessage("旧登录密码设置错误！"); Response.End(); }
            /************************************************************************************************
            * 验证用户设置的新的登陆密码信息
            * ***********************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请设置您的登录密码！"); Response.End(); }
            else if (Password.Length <= 5) { this.ErrorMessage("登录密码长度请限制在6-16位数之间！"); Response.End(); }
            else if (Password.Length > 16) { this.ErrorMessage("登录密码长度请设置在6-16位数之间！"); Response.End(); }
            else { Password = MemberHelper.toPassword(Password); }
            /************************************************************************************************
            * 验证确认密码数据信息
            * ***********************************************************************************************/
            string SurePassword = RequestHelper.GetRequest("SurePassword").toString();
            if (string.IsNullOrEmpty(SurePassword)) { this.ErrorMessage("请填写确认密码！"); Response.End(); }
            else if (SurePassword.Length <= 5) { this.ErrorMessage("确认密码与新密码长度不一致！"); Response.End(); }
            else if (SurePassword.Length > 16) { this.ErrorMessage("确认密码与新密码长度不一致！"); Response.End(); }
            else { SurePassword = MemberHelper.toPassword(SurePassword); }
            if (SurePassword != Password) { this.ErrorMessage("确认密码填写错误,请重试！"); Response.End(); }
            else if (Password.ToLower() == MemberRs["PasswordTo"].ToString().ToLower())
            { this.ErrorMessage("登录密码与二级密码不能相同！"); Response.End(); }
            /************************************************************************************************
            * 开始保存请求数据信息
            * ***********************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, dictionary: new Dictionary<string, string>() {
                {"Password",Password}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /************************************************************************************************
            * 输出数据处理结果
            * ***********************************************************************************************/
            this.ErrorMessage("设置成功！", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 设置交易密码
        /// </summary>
        protected void SavePasswordTo()
        {
            /************************************************************************************************
             * 验证新二级密码
             * **********************************************************************************************/
            string PasswordTo = RequestHelper.GetRequest("PasswordTo").toString();
            if (string.IsNullOrEmpty(PasswordTo)) { PasswordTo = RequestHelper.GetRequest("Password").toString(); }
            if (string.IsNullOrEmpty(PasswordTo)) { this.ErrorMessage("请填写您的交易密码！"); Response.End(); }
            else if (PasswordTo.Length <= 5) { this.ErrorMessage("交易密码长度设置在6-16位数之间！"); Response.End(); }
            else if (PasswordTo.Length >= 16) { this.ErrorMessage("交易密码长度设置在6-16位数之间！"); Response.End(); }
            else if (MemberHelper.toPassword(PasswordTo) == MemberRs["PasswordTo"].ToString())
            { this.ErrorMessage("旧密码与新密码相同,不需要设置！"); Response.End(); }
            else if (MemberHelper.toPassword(PasswordTo).ToLower() == MemberRs["Password"].ToString().ToLower())
            { this.ErrorMessage("二级密码与登录密码不能相同！"); Response.End(); }
            /************************************************************************************************
             * 验证短信验证码信息
             * **********************************************************************************************/
            string isCaptcha = RequestHelper.GetRequest("isCaptcha").toInt();
            string Captcha = RequestHelper.GetRequest("Captcha").ToString();
            if (isCaptcha != "1" && MemberRs["Password"].ToString() != MemberRs["PasswordTo"].ToString()) { this.ErrorMessage("请上传验证码标识!"); Response.End(); }
            else if (isCaptcha == "1" && string.IsNullOrEmpty(Captcha)) { this.ErrorMessage("请填写短信验证码！"); Response.End(); }
            else if (isCaptcha == "1" && Captcha.Length != 6) { this.ErrorMessage("短信验证码格式错误,必须为6位纯数字!"); Response.End(); }
            else if (isCaptcha == "1" && MemberRs["SessionDate"].ToString().cDate() <= DateTime.Now.AddMinutes(-5)) { this.ErrorMessage("短信验证码已过期,请重新获取!"); Response.End(); }
            else if (isCaptcha == "1" && MemberRs["SessionCode"].ToString() != Captcha) { this.ErrorMessage("短信验证码错误,请重试！"); Response.End(); }
            /************************************************************************************************
             * 开始保存数据
             * **********************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, dictionary: new Dictionary<string, string>() {
                {"PasswordTo",MemberHelper.toPassword(PasswordTo)}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /************************************************************************************************
             * 输出数据处理结果
             * **********************************************************************************************/
            this.ErrorMessage("设置成功！", iSuccess: true);
            Response.End();
        }

        /// <summary>
        /// 验证二级密码是否合法
        /// </summary>
        protected void VerificationPassword()
        {
            /*********************************************************************************************
             * 验证用户二级密码是否正确
             * *******************************************************************************************/
            string Password = RequestHelper.GetRequest("PasswordTo").ToString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("二级密码不能为空!"); Response.End(); }
            if (Password.Length <= 5) { this.ErrorMessage("二级密码长度必须大于6位数!"); Response.End(); }
            if (Password.Length >= 16) { this.ErrorMessage("二级密码长度必须小于16位数！"); Response.End(); }
            if (MemberHelper.toPassword(Password).ToLower() != MemberRs["PasswordTo"].ToString().ToLower())
            { this.ErrorMessage("二级密码错误,请重试！"); Response.End(); }
            /*********************************************************************************************
             * 二级密码验证成功,输出数据处理结果
             * *******************************************************************************************/
            this.ErrorMessage("success", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 保存昵称,邮箱
        /// </summary>
        protected void SaveProfile()
        {
            /****************************************************************************************
             * 验证昵称信息是否合法
             * **************************************************************************************/
            string Nickname = RequestHelper.GetRequest("Nickname").ToString();
            if (string.IsNullOrEmpty(Nickname)) { this.ErrorMessage("账户昵称不能为空！"); Response.End(); }
            else if (Nickname.Length <= 1) { this.ErrorMessage("昵称不能少于1个字符！"); Response.End(); }
            else if (Nickname.Length > 16) { this.ErrorMessage("用户昵称长度请限制在16个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"Nickname",Nickname}
            });
            if (cRs != null && cRs["UserID"].ToString() != MemberRs["UserID"].ToString()) { this.ErrorMessage("用户昵称已经存在,请重新设置!"); Response.End(); }
            /************************************************************************************************************
            * 验证用户其他信息,主要是联系信息,真实姓名等
            * ***********************************************************************************************************/
            string strEmail = RequestHelper.GetRequest("strEmail").ToString();
            if (!string.IsNullOrEmpty(strEmail) && strEmail.Length <= 6) { this.ErrorMessage("电子邮箱不能少于6个字符!"); Response.End(); }
            else if (!string.IsNullOrEmpty(strEmail) && strEmail.Length >= 26) { this.ErrorMessage("电子邮箱字段长度请限制在26个字符内！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strEmail) && !strEmail.Contains("@")) { this.ErrorMessage("电子邮箱格式错误,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strEmail) && !strEmail.isEmail()) { this.ErrorMessage("电子邮箱格式错误,请重试！"); Response.End(); }
            /****************************************************************************************
             * 开始保存数据
             * **************************************************************************************/
            Dictionary<string, string> thisDictionary = new Dictionary<string, string>();
            thisDictionary["Nickname"] = Nickname;
            thisDictionary["strEmail"] = strEmail;
            DbHelper.Connection.Update(TableCenter.User, thisDictionary,
                Params: " and UserID = " + MemberRs["UserID"] + "");
            /*****************************************************************************************
             * 输出保存数据结果
             * ***************************************************************************************/
            this.ErrorMessage("数据请求处理成功！", iSuccess: true);
            Response.End();
        }

        /// <summary>
        /// 输出当前的用户信息
        /// </summary>
        public void strDefault()
        {
            /********************************************************************************************
             * 统计用户账户总金额
             * ******************************************************************************************/
            DataRow CountRs = DbHelper.Connection.FindRow("Fooke_Report",
                columns: "Isnull(sum(AmountIn),0) as iAmount",
                Params: " and UserID=" + MemberRs["UserID"] + "");
            /********************************************************************************************
             * 构建网页输出内容
             * ******************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"userid\":\"" + MemberRs["UserID"] + "\"");
            strBuilder.Append(",\"parentid\":\"" + MemberRs["ParentID"] + "\"");
            strBuilder.Append(",\"authorizationtype\":\"" + MemberRs["AuthorizationType"] + "\"");
            strBuilder.Append(",\"authorizationkey\":\"" + MemberRs["AuthorizationKey"] + "\"");
            strBuilder.Append(",\"deviceidentifier\":\"" + MemberRs["DeviceIdentifier"] + "\"");
            strBuilder.Append(",\"devicemodel\":\"" + MemberRs["DeviceModel"] + "\"");
            strBuilder.Append(",\"devicechar\":\"" + MemberRs["DeviceChar"] + "\"");
            strBuilder.Append(",\"macchar\":\"" + MemberRs["MacChar"] + "\"");
            strBuilder.Append(",\"strcity\":\"" + MemberRs["strCity"] + "\"");
            strBuilder.Append(",\"strip\":\"" + MemberRs["strIP"] + "\"");
            strBuilder.Append(",\"alipayname\":\"" + MemberRs["Alipayname"] + "\"");
            strBuilder.Append(",\"alipaychar\":\"" + MemberRs["AlipayChar"] + "\"");
            strBuilder.Append(",\"fullname\":\"" + MemberRs["Fullname"] + "\"");
            strBuilder.Append(",\"strwechat\":\"" + MemberRs["strWeChat"] + "\"");
            strBuilder.Append(",\"devicetype\":\"" + MemberRs["DeviceType"] + "\"");
            strBuilder.Append(",\"devicecode\":\"" + MemberRs["DeviceCode"] + "\"");
            strBuilder.Append(",\"strtokey\":\"" + MemberRs["strTokey"] + "\"");
            strBuilder.Append(",\"password\":\"" + MemberRs["Password"] + "\"");
            strBuilder.Append(",\"passwordto\":\"" + MemberRs["PasswordTo"] + "\"");
            strBuilder.Append(",\"username\":\"" + MemberRs["Username"] + "\"");
            if (CountRs == null) { strBuilder.Append(",\"iamount\":\"0\""); }
            else if (CountRs != null) { strBuilder.Append(",\"iamount\":\"" + new Fooke.Function.String(CountRs["iAmount"].ToString()).cDouble().ToString("0.00") + "\""); }
            strBuilder.Append(",\"amount\":\"" + MemberRs["Amount"] + "\"");
            strBuilder.Append(",\"thumb\":\"" + MemberRs["Thumb"] + "\"");
            strBuilder.Append(",\"nickname\":\"" + MemberRs["Nickname"] + "\"");
            strBuilder.Append(",\"stremail\":\"" + MemberRs["strEmail"] + "\"");
            strBuilder.Append(",\"strmobile\":\"" + MemberRs["strMobile"] + "\"");
            strBuilder.Append(",\"addtime\":\"" + MemberRs["Addtime"] + "\"");
            strBuilder.Append("}");
            /********************************************************************************************
             * 输出数据处理结果
             * ******************************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 显示手否有红包
        /// </summary>
        protected void ShowRookie()
        {
            /**************************************************************************************************
             * 判断是否开启了新手红包
             * ************************************************************************************************/
            bool ShowRookieContianer = false;
            /**************************************************************************************************
             * 开始执行数据处理
             * ************************************************************************************************/
            try
            {
                string isRookie = this.GetParameter("isRookie", "shareXml").toInt();
                if (isRookie == "1")
                {
                    string RookieKey = string.Format("新手红包-|-|-{0}-|-|-{0}-|-|-新手红包", MemberRs["UserID"].ToString());
                    RookieKey = new Fooke.Function.String(RookieKey).ToMD5().Substring(0, 24).ToUpper();
                    DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAmount]", new Dictionary<string, object>() {
                        {"UserID",MemberRs["UserID"].ToString()},
                        {"strKey",RookieKey}
                    });
                    if (cRs == null) { ShowRookieContianer = true; }
                }
            }
            catch { }
            /**************************************************************************************************
             * 返回数据处理结果
             * ************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"type\":\"define\"");
            strBuilder.Append(",\"tips\":\"" + ShowRookieContianer.ToString().ToLower() + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        protected void SaveRookie()
        {
            /***************************************************************************************************
             * 判断是否开启新手红包功能
             * *************************************************************************************************/
            string isRookie = this.GetParameter("isRookie", "shareXml").toInt();
            if (isRookie != "1") { JSONMessage("已关闭新手红包功能,请联系客服！"); Response.End(); }
            /***************************************************************************************************
             * 获取用户领取红包金额信息
             * *************************************************************************************************/
            double AmtRookie = this.GetParameter("AmtRookie", "shareXml").cDouble();
            if (AmtRookie <= 0) { AmtRookie = Convert.ToDouble(((double)new Random().Next(5, 200) / 100).ToString("0.00")); }
            if (AmtRookie <= 0) { JSONMessage("获取红包奖励金额失败,请重试！"); Response.End(); }
            /***************************************************************************************************
             * 获取用户是否已领取新手红包
             * *************************************************************************************************/
            string RookieKey = string.Format("新手红包-|-|-{0}-|-|-{0}-|-|-新手红包", MemberRs["UserID"].ToString());
            RookieKey = new Fooke.Function.String(RookieKey).ToMD5().Substring(0, 24).ToUpper();
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAmount]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"strKey",RookieKey}
            });
            if (cRs != null) { JSONMessage("您已领取了新手红包,不需要重复领取！"); Response.End(); }
            /***************************************************************************************************
             * 获取用户是否已领取新手红包
             * *************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["FormID"] = MemberRs["UserID"].ToString();
            thisDictionary["Formname"] = MemberRs["Nickname"].ToString();
            thisDictionary["strKey"] = RookieKey;
            thisDictionary["Mode"] = "新手红包";
            thisDictionary["Affairs"] = "1";
            thisDictionary["Amount"] = AmtRookie.ToString("0.00");
            thisDictionary["Remark"] = string.Format("获取新手红包奖励{0}元", AmtRookie.ToString("0.00"));
            DbHelper.Connection.ExecuteProc("[Stored_SaveAmount]", thisDictionary);
            /***************************************************************************************************
             * 输出数据处理结果
             * *************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"amount\":\"" + AmtRookie.ToString("0.00") + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

    }
}