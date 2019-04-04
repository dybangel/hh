using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using Fooke.Function;
using Fooke.Code;
namespace Fooke.Web.Member
{
    public partial class Home : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "message": SendMessage(); Response.End(); break;
                case "password": strPassword(); Response.End(); break;
                case "passwordto": strPasswordTo(); Response.End(); break;
                case "savepasswordto": SavePasswordTo(); Response.End(); break;
                case "savepassword": SavePassword(); Response.End(); break;
                case "profile": strProfile(); Response.End(); break;
                case "saveprofile": SaveProfile(); Response.End(); break;
                case "filesave": SaveThumb(); Response.End(); break;
                case "access": strAccess(); Response.End(); break;
                case "mobile": ShowMobile(); Response.End(); break;
                case "savemobile": SaveMobile(); Response.End(); break;
                case "alipay": ShowAlipay(); Response.End(); break;
                case "helper": strHelper(); Response.End(); break;
                case "wechat": ShowWeChat(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /********************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 帮助中心
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ******************************************************************************************************/
        #region 帮助中心
        protected void strHelper()
        {
            /**************************************************************************************
             * 开始输出网页数据
             * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/helper.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }
        #endregion
        /// <summary>
        /// 修改账号
        /// </summary>
        protected void strAccess()
        {
            /**************************************************************************************
             * 开始输出网页数据
             * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/access.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }
        /********************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 用户绑定的手机号码
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ******************************************************************************************************/
        #region 用户绑定的手机号码
        protected void ShowMobile()
        {
            /**************************************************************************************
             * 判断手机号码是否已经绑定,否则跳转到绑定手机号码界面
             * ************************************************************************************/
            if (string.IsNullOrEmpty(MemberRs["strMobile"].ToString())) { strMobile(); Response.End(); }
            else if (MemberRs["strMobile"].ToString().Length <= 0) { strMobile(); Response.End(); }
            else if (MemberRs["strMobile"].ToString().Length != 11) { strMobile(); Response.End(); }
            else if (!MemberRs["strMobile"].ToString().isMobile()) { strMobile(); Response.End(); }
            /**************************************************************************************
             * 开始输出网页数据
             * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/showmobile.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 绑定手机号
        /// </summary>
        protected void strMobile()
        {
            /**************************************************************************************
             * 开始输出网页数据
             * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/mobile.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        #endregion
        /********************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 支付宝账号信息
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ******************************************************************************************************/
        #region 支付宝账号信息
        /// <summary>
        /// 显示支付宝
        /// </summary>
        protected void ShowAlipay()
        {
            /**************************************************************************************
            * 判断是否还没有绑定支付宝,否则跳转到绑定支付宝界面
            * ************************************************************************************/
            if (string.IsNullOrEmpty(MemberRs["AlipayChar"].ToString())) { strAlipay(); Response.End(); }
            else if (MemberRs["AlipayChar"].ToString().Length<=0) { strAlipay(); Response.End(); }
            else if (string.IsNullOrEmpty(MemberRs["Alipayname"].ToString())) { strAlipay(); Response.End(); }
            else if (MemberRs["Alipayname"].ToString().Length <= 0) { strAlipay(); Response.End(); }
            /**************************************************************************************
            * 开始输出网页数据
            * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/showalipay.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 绑定支付宝
        /// </summary>
        protected void strAlipay()
        {
            
            /**************************************************************************************
            * 开始输出网页数据
            * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/alipay.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }
        #endregion

        /********************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 微信账号信息
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ******************************************************************************************************/
        #region 微信账号信息 
        /// <summary>
        /// 微信账号信息
        /// </summary>
        protected void ShowWeChat()
        {
            /**************************************************************************************
            * 判断用户是否绑定微信,如果已绑定则跳转到显示界面
            * ************************************************************************************/
            //线上还是线下提现
            string WeIsOnLine = this.GetParameter("WeIsOnLine", "AlipayXml").toInt();
            if (WeIsOnLine == "1")
            {
                //（关注公众号绑定微信）
                if (string.IsNullOrEmpty(MemberRs["AuthorizationType"].ToString())) { strWeChat(); Response.End(); }
                else if (MemberRs["AuthorizationType"].ToString().Length <= 0) { strWeChat(); Response.End(); }
                else if (MemberRs["AuthorizationType"].ToString().ToLower() == "define") { strWeChat(); Response.End(); }
                else if (string.IsNullOrEmpty(MemberRs["AuthorizationKey"].ToString())) { strWeChat(); Response.End(); }
                else if (MemberRs["AuthorizationKey"].ToString().Length <= 0) { strWeChat(); Response.End(); }
                else if (MemberRs["AuthorizationKey"].ToString().StartsWith("DM")) { strWeChat(); Response.End(); }
            }
            else
            {
                //线下方式绑定
                if (string.IsNullOrEmpty(MemberRs["Fullname"].ToString())) { strFullname(); Response.End(); }
                else if (MemberRs["Fullname"].ToString().Length <= 0) { strFullname(); Response.End(); }
                if (string.IsNullOrEmpty(MemberRs["strWechat"].ToString())) { strFullname(); Response.End(); }
                else if (MemberRs["strWechat"].ToString().Length <= 0) { strFullname(); Response.End(); }
                else if (MemberRs["strWechat"].ToString().Length <= 4) { strFullname(); Response.End(); }
            }
            /**************************************************************************************
            * 开始输出网页数据
            * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/showchat.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 绑定微信
        /// </summary>
        protected void strWeChat()
        {
            /**************************************************************************************
            * 开始输出网页数据
            * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/wechat.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    case "qrcode": strValue = new Fooke.WeChat.ConfigHelper().GetParameter("DimensionalPath").toString(); break;
                    case "wechatname": strValue = new Fooke.WeChat.ConfigHelper().GetParameter("Wechatname").toString(); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 填写实名信息
        /// </summary>
        protected void strFullname()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/home/fullname.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    case "amount": strValue = new Fooke.Function.String(MemberRs["Amount"].ToString()).cDouble().ToString("0.00"); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        #endregion

        /********************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 修改登陆密码安全密码信息
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ******************************************************************************************************/
        #region 修改登陆密码安全密码信息
        /// <summary>
        /// 修改登陆密码
        /// </summary>
        protected void strPassword()
        {
            /**************************************************************************************
             * 开始输出网页数据
             * ************************************************************************************/
            SimpleMaster.SimpleMaster sMaster = new SimpleMaster.SimpleMaster();
            string strResponse = sMaster.Reader("template/home/password.html");
            strResponse = sMaster.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 修改登陆密码
        /// </summary>
        protected void strPasswordTo()
        {
            /***********************************************************************
             * 开始输出网页数据
             * *********************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/home/passwordTo.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    case "username": strValue = MemberRs["username"].ToString().hideText(3, 6); break;
                    case "isFirst":
                        if (MemberRs["PasswordTo"].ToString() == "2d13c3247c6219415d093926") { strValue = "1"; }
                        else if (MemberRs["Password"].ToString() == MemberRs["PasswordTo"].ToString()) { strValue = "1"; }
                        else { strValue = "0"; }; break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 修改个人资料
        /// </summary>
        protected void strProfile()
        {
            /***********************************************************************
             * 开始输出网页数据
             * *********************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/home/profile.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()); break;
                    case "user": strValue = MemberRs["DeviceIdentifier"].ToString().Substring(0,8); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        #endregion

        /********************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 会员中心,默认主页
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ******************************************************************************************************/
        #region 会员中心,默认主页
        /// <summary>
        /// 会员中心,默认主页
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 统计用户今日收益信息
             * ************************************************************************************/
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserTodayComputer]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"DateKey",DateTime.Now.ToString("yyyyMMdd")}
            });
            if (sRs == null) { this.ErrorMessage("获取今日收益信息失败,请重试！"); Response.End(); }
            //线上还是线下提现
            string WeIsOnLine = this.GetParameter("WeIsOnLine", "AlipayXml").toInt();
            string bgImg = "template/images/";
            string bgColor = "background:";
            string bgVimg = "template/images/";
            int Level = GetUserLevel();
            switch (Level)
            {
                case 0: bgImg += "bgg.png"; bgVimg += "v0.png"; bgColor += "#7EA6A6"; break;
                case 1: bgImg += "bgr.png"; bgVimg += "v1.png"; bgColor += "#D87588"; break;
                case 2: bgImg += "bgj.png"; bgVimg += "v2.png"; bgColor += "#F6B946"; break;
                case 3: bgImg += "bgb.png"; bgVimg += "v3.png"; bgColor += "#757575"; break;
                case 4: bgImg += "bgb.png"; bgVimg += "v4.png"; bgColor += "#757575"; break;
            }
            /**************************************************************************************
            * 输出网页内容信息
            * ************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/home/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "iReset": if (MemberRs["Password"].ToString() != MemberRs["PasswordTo"].ToString())
                        { strValue = "none"; }; break;
                    case "totalamount": strValue = new Fooke.Function.String(sRs["TotalAmount"].ToString()).cDouble().ToString("0.00"); break;
                    case "todayamount": strValue = new Fooke.Function.String(sRs["Todayamount"].ToString()).cDouble().ToString("0.00"); break;
                    case "weisonline": strValue = WeIsOnLine; break;
                    case "bgimg": strValue = bgImg; break;
                    case "bgvimg": strValue = bgVimg; break;
                    case "bgcolor": strValue = bgColor; break;
                    case "level": strValue = Level.ToString(); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        #endregion
        /********************************************************************************************
         * 数据处理区域
         * ******************************************************************************************/
        /// <summary>
        /// 保存编辑我的登录密码
        /// </summary>
        protected void SavePassword()
        {
            /*****************************************************************************************************
             * 判断二级密码是否填写正确
             * ***************************************************************************************************/
            string OldPassword = RequestHelper.GetRequest("OldPassword").toString();
            if (string.IsNullOrEmpty(OldPassword)) { this.ErrorMessage("请填写您上次的登陆密码！"); Response.End(); }
            else if (OldPassword.Length <= 5) { this.ErrorMessage("旧密码长度不能少于6个字符！"); Response.End(); }
            else if (OldPassword.Length >= 16) { this.ErrorMessage("旧密码长度不能大于16个字符！"); Response.End(); }
            if (MemberHelper.toPassword(OldPassword) != MemberRs["Password"].ToString())
            { this.ErrorMessage("旧密码填写错误,请重试！"); Response.End(); }
            /******************************************************************************************************
             * 判断新密码是否合法
             * ****************************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请填写你要设置的登录密码！"); Response.End(); }
            else if (Password.Length <= 5) { this.ErrorMessage("登录密码长度至少在6位数以上！"); Response.End(); }
            else if (Password.Length >= 16) { this.ErrorMessage("登录密码长度请限制在16位数以内！"); Response.End(); }
            string SurePassword = RequestHelper.GetRequest("SurePassword").toString();
            if (string.IsNullOrEmpty(SurePassword)) { this.ErrorMessage("请填写确认登录密码！"); Response.End(); }
            else if (SurePassword != Password) { this.ErrorMessage("确认密码填写错误！"); Response.End(); }
            else if (MemberHelper.toPassword(Password) == MemberRs["Password"].ToString())
            { this.ErrorMessage("登陆密码没有发生更改,无需修改！"); Response.End(); }
            if (MemberHelper.toPassword(Password) == MemberRs["PasswordTo"].ToString())
            { this.ErrorMessage("登陆密码与交易密码不能相同！"); Response.End(); }
            /*****************************************************************************************************
             * 开始保存数据
             * ***************************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, new Dictionary<string, string>() {
                {"Password",MemberHelper.toPassword(Password)}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /*****************************************************************************************************
             * 开始保存数据
             * ***************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"type\":\"altRedirect\"");
            strBuilder.Append(",\"tips\":\"登陆密码修改成功\"");
            strBuilder.Append(",\"url\":\"Home.aspx\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 更新我的二级密码
        /// </summary>
        protected void SavePasswordTo()
        {
            /*****************************************************************************************************
             * 验证新交易密码的合法性
             * ***************************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请填写你要设置的交易密码！"); Response.End(); }
            else if (Password.Length < 6) { this.ErrorMessage("交易密码长度至少在6位数以上！"); Response.End(); }
            else if (Password.Length > 16) { this.ErrorMessage("交易密码长度请限制在16位数以内！"); Response.End(); }
            string SurePassword = RequestHelper.GetRequest("SurePassword").toString();
            if (string.IsNullOrEmpty(SurePassword)) { this.ErrorMessage("请填写确认交易密码！"); Response.End(); }
            else if (SurePassword != Password) { this.ErrorMessage("确认交易密码填写错误！"); Response.End(); }
            /*****************************************************************************************************
             * 判断二级密码是否与登录密码相同
             * ***************************************************************************************************/
            if (MemberHelper.toPassword(Password) == MemberRs["Password"].ToString()) { this.ErrorMessage("交易密码与登录密码一致,请重新更换二级密码吧！"); Response.End(); }
            /*****************************************************************************************************
             * 判断验证码是否合法
             * ***************************************************************************************************/
            if (MemberRs["PasswordTo"].ToString() != MemberRs["Password"].ToString()
             && MemberRs["PasswordTo"].ToString() != "2d13c3247c6219415d093926")
            {
                string Captcha = RequestHelper.GetRequest("Captcha").toString();
                if (string.IsNullOrEmpty(Captcha)) { this.ErrorMessage("请填写验证码！"); Response.End(); }
                else if (Captcha.Length != 6) { this.ErrorMessage("验证码长度太长！"); Response.End(); }
                else if (MemberRs["SessionDate"].ToString().cDate() <= DateTime.Now.AddMinutes(-30)) { this.ErrorMessage("短信验证码已过期！"); Response.End(); }
                else if (Captcha != MemberRs["SessionCode"].ToString()) { this.ErrorMessage("短信验证码错误,请重试！"); Response.End(); }
            }
            /*****************************************************************************************************
             * 开始保存数据
             * ***************************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, new Dictionary<string, string>() {
                {"PasswordTo",MemberHelper.toPassword(Password)}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /*****************************************************************************************************
             * 开始保存数据
             * ***************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"type\":\"altRedirect\"");
            strBuilder.Append(",\"tips\":\"支付密码修改成功\"");
            strBuilder.Append(",\"url\":\"Home.aspx\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 保存我上传的头像
        /// </summary>
        protected void SaveThumb()
        {
            /*************************************************************************************************
             *验证图片的合法性
             * ***********************************************************************************************/
            if (Request.Files == null) { JSONMessage("请上传一张图片作为头像！"); Response.End(); }
            else if (Request.Files.Count <= 0) { JSONMessage("请上传一张图片作为头像！"); Response.End(); }
            string fileName = string.Format("用户头像-|-|-{0}-|-|-{0}", MemberRs["UserID"].ToString());
            fileName = new Fooke.Function.String(fileName).ToMD5().Substring(0, 16).ToLower();
            fileName = fileName + ".{exc}";
            /*************************************************************************************************
             *开始上传图片,获取到上传地址
             * ***********************************************************************************************/
            string strResponse = new PostedHelper().SaveTo(Posted: Request.Files[0],
                directory: "~/file/user",
                fileName: fileName,
                fileSize: 3,
                fileExc: "jpg|png|bmp|jpeg|xmp|gif|JPEG|JPG|png+xmp",
                Err: (msg) =>
                {
                    JSONMessage(msg); Response.End();
                });
            /*****************************************************************************************************
             * 保存上传图片信息
             * ***************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { JSONMessage("获取上传头像截图失败,请重试！"); Response.End(); }
            else if (strResponse.Contains("error")) { JSONMessage("获取上传头像截图失败,请重试！"); Response.End(); }
            /**************************************************************************************************
             * 将图片资源保存到数据库
             * ***********************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, dictionary: new Dictionary<string, string>() {
                    {"Thumb",strResponse}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /*************************************************************************************************
             *输出数据处理结果
             * ***********************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"头像上传成功!\"");
            strBuilder.Append(",\"type\":\"alert\"");
            strBuilder.Append(",\"url\":\"Home.aspx\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 保存用户手机号码
        /// </summary>
        protected void SaveMobile()
        {
            /********************************************************************************************************************
             * 验证手机号码的合法性
             * ******************************************************************************************************************/
            string strMobile = RequestHelper.GetRequest("strMobile").ToString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("请填写您的手机号码！"); Response.End(); }
            else if (strMobile.Length != 11) { this.ErrorMessage("手机号码格式错误!"); Response.End(); }
            else if (!strMobile.isMobile()) { this.ErrorMessage("手机号码格式错误,请重试！"); Response.End(); }
            else if (MemberRs["strMobile"].ToString() == strMobile) { this.ErrorMessage("手机号码未发生更改！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"Username",strMobile}
            });
            if (cRs != null) { this.ErrorMessage("对不起,你绑定的手机号码已经存在！"); Response.End(); }
            /********************************************************************************************************************
             * 验证验证码的合法性
             * ******************************************************************************************************************/
            string Captcha = RequestHelper.GetRequest("Captcha").toString();
            if (string.IsNullOrEmpty(Captcha)) { this.ErrorMessage("请填写验证码！"); Response.End(); }
            else if (Captcha.Length != 6) { this.ErrorMessage("验证码长度太长！"); Response.End(); }
            else if (MemberRs["SessionDate"].ToString().cDate() <= DateTime.Now.AddMinutes(-30)) { this.ErrorMessage("短信验证码已过期！"); Response.End(); }
            else if (Captcha != MemberRs["SessionCode"].ToString()) { this.ErrorMessage("短信验证码错误,请重试！"); Response.End(); }
            /********************************************************************************************************************
            * 开始保存数据
            * ******************************************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, new Dictionary<string, string>() {
                {"Username",strMobile},
                {"strMobile",strMobile}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /*****************************************************************************************************
             * 开始保存数据
             * ***************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"type\":\"altRedirect\"");
            strBuilder.Append(",\"tips\":\"手机号码绑定成功\"");
            strBuilder.Append(",\"url\":\"Home.aspx\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 保存个人资料
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
        /// 发送短信验证码
        /// </summary>
        protected void SendMessage()
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
            string SendText = "你的短信验证码是 " + SessionCode + "";
            if (!string.IsNullOrEmpty(strMode) && strMode.ToLower() == ("SaveAccess").ToLower())
            { SendText = "您正在更改手机号，验证码：" + SessionCode + "，请于3分钟内正确输入。请勿向任何人透露本验证码。如非本人操作，请联系客服"; }
            else if (!string.IsNullOrEmpty(strMode) && strMode.ToLower() == ("SavePasswordTo").ToLower())
            { SendText = "您正在设置交易密码，验证码：" + SessionCode + "，请于3分钟内正确输入。请勿向任何人透露本验证码。"; }
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
        /// 获取当前用户等级
        /// </summary>
        protected int GetUserLevel()
        {
            int level = 0;
            try
            {
                //查询当前用户的所有邀请人（所有下级）
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserChildrenAll]", new Dictionary<string, object>() {
                    {"NodeID",MemberRs["UserID"].ToString()}
                });
                int ChildrenNum = string.IsNullOrEmpty(sRs[0].ToString()) ? 0 : Convert.ToInt32(sRs[0].ToString());
                //第一个等级
                int shareGrade1 = this.GetParameter("shareGrade1", "shareXml").cInt();
                //第二个等级 
                int shareGrade2 = this.GetParameter("shareGrade2", "shareXml").cInt();
                //第三个等级 
                int shareGrade3 = this.GetParameter("shareGrade3", "shareXml").cInt();
                //第四个等级 
                int shareGrade4 = this.GetParameter("shareGrade4", "shareXml").cInt();
                if (ChildrenNum < shareGrade1 ) { level = 0; }
                if (ChildrenNum >= shareGrade1 && shareGrade2 > shareGrade1 && ChildrenNum < shareGrade2) { level = 1; }
                if (ChildrenNum >= shareGrade2 && shareGrade2 < shareGrade3 && ChildrenNum < shareGrade3) { level = 2; }
                if (ChildrenNum >= shareGrade3 && shareGrade3 > shareGrade2 && ChildrenNum < shareGrade4) { level = 3; }
                if (ChildrenNum >= shareGrade4 && shareGrade4 > shareGrade3) { level = 4; }
            }
            catch (Exception ex) { }
            return level;
        }

    }
}