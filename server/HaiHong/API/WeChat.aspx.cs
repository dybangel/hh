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
using Fooke.WeChat;
namespace Fooke.Web.API
{
    public partial class WeChat : Fooke.Code.BaseHelper
    {
        private ConfigurationReader respConfigure = null;
        private string siteKey = string.Empty;
        private Fooke.WeChat.ConfigHelper WConfig = new ConfigHelper();
        protected void Page_Load(object sender, EventArgs e)
        {
            /****************************************************************************
             * 加载数据信息配置
             * ***************************************************************************/
            if (this.WConfig == null) { WConfig = new ConfigHelper(); }
            if (this.WConfig != null) { siteKey = WConfig.GetParameter("strkey", "DefaultXml").toString(); }
            Fooke.Code.LogsHelper.Add("sss");
            if (string.IsNullOrEmpty(siteKey)) { Response.Write("公众号验证信息失败！"); Response.End(); }
            else if (siteKey.Length <= 0) { Response.Write("公众号验证信息失败！"); Response.End(); }
            else if (siteKey.Length <= 5) { Response.Write("公众号验证信息失败！"); Response.End(); }
            
            /***************************************************************************
             * 开始验证数据
             * *************************************************************************/
            if (Request.HttpMethod.ToLower() != "post") { this.AuthorizationWeChat(); Response.End(); }
            else
            {
                this.AuthorizationRequest((respCfg) =>
                    {
                        if (respCfg != null) { this.respConfigure = respCfg; }
                        if (respCfg != null && respConfigure != null)
                        {
                            string SendRequest = this.GetRequest("msgType").toString();
                            string Event = this.GetRequest("Event").toString();
                            switch (SendRequest.ToLower())
                            {
                                case "text": this.AuthorizationText(); Response.End(); break;
                                //case "image": this.ImageRestore(xReader); Response.End(); break;
                                case "event":
                                    if (!string.IsNullOrEmpty(Event) && Event.ToLower() == "click") { MenuClick(); Response.End(); }
                                    else if (!string.IsNullOrEmpty(Event) && Event.ToLower() == "subscribe") { FollowClick(); Response.End(); }
                                    break;
                            }
                        }
                    });
            }
            Response.End();
        }
        /// <summary>
        /// 关注回复
        /// </summary>
        protected void FollowClick()
        {
            string FromUserName = this.GetRequest("FromUserName").toString();
            string ToUserName = this.GetRequest("ToUserName").toString();
            /**********************************************************************************************
             * 保存代理账号信息
             * ********************************************************************************************/
            string FormatKey = this.GetRequest("EventKey").toString("0");
            /**********************************************************************************************
             * 输出回复信息
             * ********************************************************************************************/
            try
            {
                string isOpen = this.Paramter("isOpen", "FollowXml").toInt();
                if (isOpen == "0") { Response.Write(""); Response.End(); }
                string RestoreMode = this.Paramter("RestoreMode", "FollowXml").toInt();
                string MaterID = this.Paramter("MaterID", "FollowXml").toInt();
                string strDesc = this.Paramter("Remark", "FollowXml").toString("亲爱的用户欢迎您的关注!");
                if (RestoreMode == "0" && MaterID != "0") { this.ResponseThumb(FromUserName, ToUserName, MaterID); Response.End(); }
                else { this.ResponseText(FromUserName, ToUserName, strDesc); Response.End(); }
            }
            catch { }
            /**********************************************************************************************
             * 网页请求完成
             * ********************************************************************************************/
            Response.End();
        }

        /// <summary>
        /// 菜单按钮事件
        /// </summary>
        protected void MenuClick()
        {
            /**********************************************************************************************************************
             * 获取微信请求参数信息
             * ********************************************************************************************************************/
            string ToUserName = this.GetRequest("ToUserName").toString();
            string FromUserName = this.GetRequest("FromUserName").toString();
            string EventKey = this.GetRequest("EventKey").toString();
            /**********************************************************************************************************************
             * 获取微信菜单数据信息
             * ********************************************************************************************************************/
            DataRow MenuRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatMenu]", new Dictionary<string, object>() {
                {"MenuID",EventKey}
            });
            if (MenuRs == null) { this.ResponseDefault(ToUserName, FromUserName); Response.End(); }
            /**********************************************************************************************************************
             * 处理系统API接口数据信息
             * ********************************************************************************************************************/
            if (MenuRs["strDesc"].ToString().Contains("余额")) { QueryBalance(); Response.End(); }
            else if (MenuRs["strDesc"].ToString().Contains("验证码")) { GetCaptcha(); Response.End(); }
            else if (MenuRs != null && MenuRs["strRequest"] == "api")
            {
                string strResponse = string.Empty;
                using (System.Net.WebClient thisClient = new System.Net.WebClient())
                {
                    try { strResponse = thisClient.DownloadString(MenuRs["APIurl"].ToString()); }
                    finally { thisClient.Dispose(); }
                    this.ResponseText(FromUserName, ToUserName, strResponse); Response.End();
                }
                Response.End();
            }
            else if (MenuRs["strRequest"].ToString() != "link")
            {
                if (MenuRs["strRequest"].ToString() == "mater" && MenuRs["MaterID"].ToString() != "0")
                { this.ResponseThumb(FromUserName, ToUserName, MenuRs["MaterID"].ToString()); Response.End(); }
                else { this.ResponseText(FromUserName, ToUserName, MenuRs["strDesc"].ToString()); Response.End(); }
            }
            /*************************************************************************
            * 截断网页内容
            * ***********************************************************************/
            Response.End();
        }
        /// <summary>
        /// 查询用户余额
        /// </summary>
        protected void QueryBalance()
        {
            /************************************************************************************************
             * 获取请求用户标识数据
             * ***********************************************************************************************/
            string ToUserName = this.GetRequest("ToUserName").toString();
            string FromUserName = this.GetRequest("FromUserName").toString();
            /************************************************************************************************
             * 获取请求用户数据信息
             * ***********************************************************************************************/
            DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"AuthorizationType","weixin"},
                {"AuthorizationKey",FromUserName}
            });
            if (MemberRs == null) { this.ResponseText(FromUserName, ToUserName, "获取用户信息失败,请先获取验证码绑定用户！"); Response.End(); }
            /************************************************************************************************
            * 输出用户账户余额信息
            * ***********************************************************************************************/
            string SendText = string.Format("亲爱的{0}您好∶\n", MemberRs["nickname"]);
            SendText += string.Format("您当前剩余金额为{0}元\n", MemberRs["Amount"]);
            SendText += string.Format("你拥有的积分为{0}分\n", MemberRs["Amount"]);
            this.ResponseText(FromUserName, ToUserName, SendText);
            Response.End();
        }
        /// <summary>
        /// 输出数据验证码
        /// </summary>
        protected void GetCaptcha()
        {
            /************************************************************************************************
             * 获取请求用户标识数据
             * ***********************************************************************************************/
            string ToUserName = this.GetRequest("ToUserName").toString();
            string FromUserName = this.GetRequest("FromUserName").toString();
            /************************************************************************************************
             * 获取请求用户数据信息
             * ***********************************************************************************************/
            DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"AuthorizationType","weixin"},
                {"AuthorizationKey",FromUserName}
            });
            if (MemberRs != null) { this.ResponseText(FromUserName, ToUserName, "您已经绑定了微信账号,无需重复绑定!"); Response.End(); }
            /************************************************************************************************
            * 保存用户邀请码数据请求
            * ***********************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserAuthorization]", new Dictionary<string, object>() {
                {"AuthorizationKey",FromUserName}
            });
            if (cRs == null) { this.ResponseText(FromUserName, ToUserName, "获取微信验证码信息失败,请重试!"); Response.End(); }
            /************************************************************************************************
            * 生成微信微信验证码数据
            * ***********************************************************************************************/
            string AuthorizationCode = string.Format("{0}{1}", cRs["AuthorID"].ToString(),
                new Fooke.Function.String(FromUserName).Substring(0, 6).ToUpper());
            /************************************************************************************************
            * 输出微信验证码数据信息
            * ***********************************************************************************************/
            string SendText = string.Format("亲爱的用户您好,请复制下方的验证码到{0}绑定微信界面即可完成验证", this.GetParameter("sitename", "siteXml").toString());
            SendText += string.Format(",您的微信验证码是:\n\n\n{0}\n\n\n", FromUserName);
            this.ResponseText(FromUserName, ToUserName, SendText);
            Response.End();
        }

        /// <summary>
        /// 处理文本回复数据信息
        /// </summary>
        protected void AuthorizationText()
        {
            /**************************************************************************
             * 获取请求参数信息
             * *************************************************************************/
            string ToUserName = this.GetRequest("ToUserName").toString();
            string FromUserName = this.GetRequest("FromUserName").toString();
            string Keyword = this.GetRequest("Content").toString();
            /**************************************************************************
             * 验证关键词回复内容信息
             * *************************************************************************/
            if (string.IsNullOrEmpty(Keyword)) { this.ResponseDefault(FromUserName, ToUserName); Response.End(); }
            else if (Keyword.Length <= 0) { this.ResponseDefault(FromUserName, ToUserName); Response.End(); }
            else if (Keyword.Contains("余额")) { QueryBalance(); Response.End(); }
            else if (Keyword.Contains("验证码")) { GetCaptcha(); Response.End(); }
            /**************************************************************************
             * 处理关键字回复功能
             * *************************************************************************/
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatKeywords]", new Dictionary<string, object>() {
                {"Title",Keyword},
                {"isMatch","1"}
            });
            if (Rs == null) { this.ResponseDefault(FromUserName, ToUserName); Response.End(); }
            else if (Rs["Modal"].ToString() == "0" && Rs["MaterID"].ToString() != "0")
            { this.ResponseThumb(FromUserName, ToUserName, Rs["MaterID"].ToString()); }
            else { this.ResponseText(FromUserName, ToUserName, Rs["strDesc"].ToString()); }
            /**************************************************************************
             * 截断输出功能
             * *************************************************************************/
            Response.End();
        }



        /// <summary>
        /// 回复文本内容
        /// </summary>
        /// <param name="ToUserName"></param>
        /// <param name="FromUserName"></param>
        /// <param name="Text"></param>
        public void ResponseText(string ToUserName, string FromUserName, string Text)
        {
            /********************************************************************************
             * 替换变量字段信息
             * ******************************************************************************/
            if (!string.IsNullOrEmpty(Text) && Text.Contains("{$") && Text.Contains("}"))
            { Text = this.FormatText(ToUserName, Text); }
            /********************************************************************************
             * 替换API请求接口信息
             * ******************************************************************************/
            if (Text.ToLower().Contains("apis||"))
            {
                try
                {
                    Text = Text.Replace("apis||", "").Replace("&amp;", "&");
                    using (System.Net.WebClient thisClient = new System.Net.WebClient())
                    {
                        thisClient.Encoding = Encoding.UTF8;
                        try { Text = thisClient.DownloadString(Text); }
                        catch { thisClient.Dispose(); }
                    };
                }
                catch { Text = "发生未知错误，请稍后重试！"; }
            }
            /********************************************************************************
             * 输出回复信息
             * ******************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<xml>");
            strBuilder.Append("<ToUserName><![CDATA[" + ToUserName + "]]></ToUserName>");
            strBuilder.Append("<FromUserName><![CDATA[" + FromUserName + "]]></FromUserName>");
            strBuilder.Append("<CreateTime>" + FunctionCenter.ConvertDateTimeInt(DateTime.Now) + "</CreateTime>");
            strBuilder.Append("<MsgType><![CDATA[text]]></MsgType>");
            strBuilder.Append("<Content><![CDATA[" + Text + "]]></Content>");
            strBuilder.Append("<FuncFlag>0</FuncFlag>");
            strBuilder.Append("</xml>");
            HttpContext.Current.Response.Write(strBuilder.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 回复图文内容
        /// </summary>
        /// <param name="FromUserName"></param>
        /// <param name="ToUserName"></param>
        /// <param name="StuffID"></param>
        public void ResponseThumb(string ToUserName, string FromUserName, string ParentID)
        {
            /**************************************************************************************************************************************
             * 获取网页请求参数内容信息
             * ************************************************************************************************************************************/
            System.Data.DataTable Tab = DbHelper.Connection.ExecuteFindTable("[Stored_FindWeChatMaterList]", new Dictionary<string, object>() {
                {"ParentID",ParentID}
            });
            if (Tab == null) { HttpContext.Current.Response.End(); }
            else if (Tab.Rows.Count <= 0) { HttpContext.Current.Response.End(); }
            /**************************************************************************************************************************************
             * 构建网页输出内容数据信息
             * ************************************************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<xml>");
            strBuilder.Append("<ToUserName><![CDATA[" + ToUserName + "]]></ToUserName>");
            strBuilder.Append("<FromUserName><![CDATA[" + FromUserName + "]]></FromUserName>");
            strBuilder.Append("<CreateTime>" + FunctionCenter.ConvertDateTimeInt(DateTime.Now) + "</CreateTime>");
            strBuilder.Append("<MsgType><![CDATA[news]]></MsgType>");
            strBuilder.Append("<ArticleCount><![CDATA[" + Tab.Rows.Count + "]]></ArticleCount>");
            strBuilder.Append("<FuncFlag>1</FuncFlag>");
            strBuilder.Append("<Articles>");
            foreach (DataRow Rs in Tab.Rows)
            {
                try
                {
                    string strUrl = FunctionCenter.SiteUrl() + Win.ApplicationPath + string.Format("/wechat/mater.aspx?action=show&Id={0}&AuthorizationKey={1}", Rs["materid"], ToUserName);
                    if (Rs["isUrl"].ToString() == "1" && !string.IsNullOrEmpty(Rs["toUrl"].ToString()))
                    { strUrl = Rs["toUrl"].ToString().Replace("{$openid}", FromUserName); }
                    strBuilder.AppendFormat("<item>");
                    strBuilder.AppendFormat("<Title><![CDATA[{0}]]></Title> ", Rs["title"]);
                    strBuilder.AppendFormat("<Description><![CDATA[{0}]]></Description>", Rs["Remark"]);
                    strBuilder.AppendFormat("<PicUrl><![CDATA[{0}]]></PicUrl>", (FunctionCenter.SiteUrl() + FunctionCenter.ConvertPath(Rs["thumb"].ToString())));
                    strBuilder.AppendFormat("<Url><![CDATA[{0}]]></Url>", strUrl);
                    strBuilder.AppendFormat("</item>");
                }
                catch { }
            }
            strBuilder.Append("</Articles>");
            strBuilder.Append("</xml>");
            /**************************************************************************************************************************************
             * 输出数据处理结果
             * ************************************************************************************************************************************/
            HttpContext.Current.Response.Write(strBuilder.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 默认回复
        /// </summary>
        /// <param name="FromUserName"></param>
        /// <param name="ToUserName"></param>
        /// <param name="ChartID"></param>
        public void ResponseDefault(string FromUserName, string ToUserName)
        {
            string isOpen = this.Paramter("isOpen", "demoXml").toInt();
            if (isOpen != "1") { Response.Write(""); Response.End(); }
            string RestoreMode = this.Paramter("RestoreMode", "demoXml").toInt();
            string MaterID = this.Paramter("MaterID", "demoXml").toInt();
            string Remark = this.Paramter("Remark", "demoXml").toString();
            if (RestoreMode == "0" && MaterID != "0")
            { this.ResponseThumb(FromUserName, ToUserName, MaterID); Response.End(); }
            else { this.ResponseText(FromUserName, ToUserName, Remark); Response.End(); }
        }
        /// <summary>
        /// 格式化回复内容
        /// </summary>
        /// <param name="Identify"></param>
        /// <param name="Text"></param>
        /// <returns></returns>
        private string FormatText(string AuthorizationKey, string strFormat)
        {
            try
            {
                strFormat = strFormat.Replace("{$openid}", AuthorizationKey);
                if (strFormat.Contains("{$") && strFormat.Contains("}"))
                {
                    DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                        {"AuthorizationType","weixin"},
                        {"AuthorizationKey",AuthorizationKey}
                    });
                    if (MemberRs != null)
                    {
                        strFormat = strFormat.Replace("{$nickname}", MemberRs["Nickname"].ToString());
                        strFormat = strFormat.Replace("{$userid}", MemberRs["UserID"].ToString());
                        strFormat = strFormat.Replace("{$point}", MemberRs["Points"].ToString());
                        strFormat = strFormat.Replace("{$amount}", MemberRs["Amount"].ToString());
                    }
                }
            }
            catch { }
            /***************************************************************************************************************
             * 返回数据处理结果信息
             * *************************************************************************************************************/
            return strFormat;
        }


        /// <summary>
        /// 获取到微信请求过来的数据信息
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public Fooke.Function.String GetRequest(string strName)
        {
            string strValue = string.Empty;
            try
            {
                if (this.respConfigure != null) { strValue = this.respConfigure.GetParameter(strName).toString(); }
            }
            catch { }
            return new Fooke.Function.String(strValue);
        }


        /// <summary>
        /// 开始处理微信公众号请求
        /// </summary>
        protected void AuthorizationRequest(Action<ConfigurationReader> Fun)
        {
            string strResponse = string.Empty;
            using (System.IO.Stream stream = System.Web.HttpContext.Current.Request.InputStream)
            {
                try
                {
                    byte[] requestByte = new byte[stream.Length];
                    stream.Read(requestByte, 0, (int)stream.Length);
                    strResponse = System.Text.Encoding.UTF8.GetString(requestByte);
                }
                finally { stream.Dispose(); }
            }
            if (!string.IsNullOrEmpty(strResponse) && strResponse.ToLower().Contains("msgtype"))
            {
                try
                {
                    ConfigurationReader Configuration = new ConfigurationReader(strResponse);
                    if (Fun != null && Configuration != null && Configuration.dictionary.Count > 0)
                    {
                        Fun(Configuration);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 获取微信参数配置
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public Fooke.Function.String Paramter(string strName, string configName)
        {
            string strValue = string.Empty;
            try
            {
                if (this.WConfig == null) { this.WConfig = new ConfigHelper(); }
                if (this.WConfig != null) { strValue = this.WConfig.GetParameter(strName, configName).toString(); }
            }
            catch { }
            return new Fooke.Function.String(strValue);
        }


        /*****************************************************************************************************************
         * 验证微信请求参数错误,判断当前请求数据是否合法
         * ****************************************************************************************************************/
        /// <summary>
        /// 验证微信请求是否合法
        /// </summary>
        protected void AuthorizationWeChat()
        {
            string echoStr = Request.QueryString["echoStr"];
            if (!string.IsNullOrEmpty(echoStr) && echoStr.Length > 10)
            {
                AuthorizationSignature((iSuccess) =>
                {
                    if (iSuccess) { Response.Write(echoStr); }
                });
            }
            Response.End();
        }

        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// <returns></returns>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        private void AuthorizationSignature(Action<bool> Fun)
        {
            bool iSuccess = false;
            try
            {
                string signature = RequestHelper.GetRequest("signature").toString();
                string timestamp = RequestHelper.GetRequest("timestamp").toString();
                string nonce = RequestHelper.GetRequest("nonce").toString();
                string[] ArrTmp = { siteKey, timestamp, nonce };
                Array.Sort(ArrTmp);//字典排序
                string tmpStr = string.Join("", ArrTmp);
                tmpStr = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
                tmpStr = tmpStr.ToLower();
                iSuccess = (tmpStr == signature);
            }
            catch { }
            /*********************************************************************************************
             * 输出返回验证
             * *******************************************************************************************/
            try { if (Fun != null) { Fun(iSuccess); } }
            catch { }
        }

    }
}