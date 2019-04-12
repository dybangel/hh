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
    public partial class Index : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "menu": strMenu(); Response.End(); break;
                case "rookie": SaveRookie(); Response.End(); break;
                case "shareConfirm": ShareConfirm(); Response.End(); break;
                case "shareCancel": ShareCancel(); Response.End(); break;
                case "saveFinger": SaveFinger(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 获取网页指纹识别信息
        /// </summary>
        protected void ShowFinger()
        {
            /**************************************************************************************
             * 开始加载网页内容
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strReader = Master.Reader("template/finger/index.html");
            strReader = Master.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName) { }
                return strValue;
            }), true);
            Response.Write(strReader);
            Response.End();
        }

        /// <summary>
        /// 默认主页
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 获取并保存用户邀请信息
             * ************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID == "0") { ParentID = RequestHelper.GetRequest("uid").toInt(); }
            if (ParentID == "0") { ParentID = CookieHelper.Get("FookeUID").toInt(); }
            if (ParentID == "0") { ParentID = CookieHelper.Get("ParentID").toInt(); }
            if (ParentID == "0") { ParentID = SessionHelper.Get("FookeUID").toInt(); }
            //if (new Fooke.Function.String(MemberRs["ParentID"].ToString()).cInt() <= 0
            //&& MemberRs["UserID"].ToString() != ParentID && ParentID != "0"
            //&& new Fooke.Function.String(MemberRs["UserID"].ToString()).cInt() > new Fooke.Function.String(ParentID).cInt())
            //{
            //    Response.Redirect("index.aspx?action=shareConfirm&token=" + MemberRs["strTokey"] + "&ParentID=" + ParentID + "&auto=true");
            //    Response.End();
            //}
            if (new Fooke.Function.String(MemberRs["ParentID"].ToString()).cInt() <= 0
               && MemberRs["UserID"].ToString() != ParentID && ParentID != "0")
            {
                Response.Redirect("index.aspx?action=shareConfirm&token=" + MemberRs["strTokey"] + "&ParentID=" + ParentID + "&auto=true");
                Response.End();
            }
            /**************************************************************************************
             * 指纹方式获取用户邀请？
             * ************************************************************************************/
            //string strFinger = RequestHelper.GetRequest("strFinger").toString();
            //if (new Fooke.Function.String(MemberRs["ParentID"].ToString()).cInt() <= 0
            //&& new Fooke.Function.String(MemberRs["shareCancel"].ToString()).cInt() <= 0
            //&& RequestHelper.GetRequest("Fingerprint").toString() != "false")
            //{
            //    if (string.IsNullOrEmpty(strFinger)) { ShowFinger(); Response.End(); }
            //    else if (strFinger.Length <= 0) { ShowFinger(); Response.End(); }
            //    else if (strFinger.Length <= 5) { ShowFinger(); Response.End(); }
            //    else if (strFinger.Length >= 30) { ShowFinger(); Response.End(); }
            //}
            /**************************************************************************************
             * 统计用户今日收益信息
             * ************************************************************************************/
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserTodayComputer]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"DateKey",DateTime.Now.ToString("yyyyMMdd")}
            });
            if (sRs == null) { this.ErrorMessage("获取今日收益信息失败,请重试！"); Response.End(); }
            /**************************************************************************************
             * 解析网页模板输出内容
             * ************************************************************************************/
            string display = RequestHelper.GetRequest("display").toString();
            if (string.IsNullOrEmpty(display) && !ShowInvited()) { display = "display:none";}
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/default.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "weekday": strValue = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek); break;
                    case "today": strValue = DateTime.Now.ToString("yyyy年MM月dd日"); break;
                    case "notification": strValue = GetNotificationXml(); break;
                    case "showRookie": if (!ShowRookie()) { strValue = "style=\"display:none\""; } break;
                    case "showInvited": strValue = "style=\"" + display + "\""; break;
                    case "todayamount": strValue = new Fooke.Function.String(sRs["TodayAmount"].ToString()).cDouble().ToString("0.00"); break;
                    case "totalamount": strValue = new Fooke.Function.String(sRs["TotalAmount"].ToString()).cDouble().ToString("0.00"); break;
                    case "balance": strValue = new Fooke.Function.String(sRs["Balance"].ToString()).cDouble().ToString("0.00"); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 弹出菜单栏页面
        /// </summary>
        protected void strMenu()
        {
            /**************************************************************************************
             * 解析网页内容,网页模板信息
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/menu/menu.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        /************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 填写邀请人处理信息
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * **********************************************************************************************************/
        #region 填写邀请人处理信息
        /// <summary>
        /// 邀请人处理信息
        /// </summary>
        protected void ShareConfirm()
        {
            /***************************************************************************************************
             * 判断是否开启新手红包功能
             * *************************************************************************************************/
            if (MemberRs["ParentID"].ToString() != "0") { JSONMessage("你已经填写了邀请人ID,无需重复填写！"); Response.End(); }
            /***************************************************************************************************
             * 获取并验证用户邀请ID并判断合法性
             * *************************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID == "0") { JSONMessage("请填写邀请人ID!"); Response.End(); }
            else if (ParentID == MemberRs["UserID"].ToString()) { JSONMessage("您不能填写自己的ID作为邀请人ID!"); Response.End(); }
            else if (ParentID.Length <= 5) { JSONMessage("邀请人ID账号信息不合法!"); Response.End(); }
            else if (new Fooke.Function.String(ParentID).cInt() <= 99999) { JSONMessage("邀请人ID账号信息不合法!"); Response.End(); }
            //else if (new Fooke.Function.String(ParentID).cInt() >= new Fooke.Function.String(MemberRs["UserID"].ToString()).cInt())
            //{ JSONMessage("邀请人ID不合法,请重试!"); Response.End(); }
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",ParentID}
            });
            if (sRs == null) { this.ErrorMessage("邀请人ID不存在,请检查！"); Response.End(); }
            else if (sRs["isDisplay"].ToString() != "1") { this.ErrorMessage("邀请人ID账号已停止使用！"); Response.End(); }
            /***************************************************************************************************
             * 开始更新保存用户邀请信息
             * *************************************************************************************************/
            DbHelper.Connection.Update("Fooke_User", dictionary: new Dictionary<string, string>() { 
                   {"ParentID",ParentID}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /***************************************************************************************************
             * 发放邀请奖励信息
             * *************************************************************************************************/
            string isBonus = this.GetParameter("isBonus", "shareXml").toInt();
            string shareModel = this.GetParameter("shareModel", "shareXml").toInt();
            if (isBonus == "1" && shareModel == "0")
            {
                try { new BonusHelper().ShareBonus(MemberRs["UserID"].ToString()); }
                catch { }
            }
            /***************************************************************************************************
             * 跳转到指定的首页
             * *************************************************************************************************/
            if (RequestHelper.GetRequest("auto").toString() == "true")
            {
                Response.Redirect("Index.aspx");
                Response.End();
            }
            /***************************************************************************************************
             * 输出数据处理结果
             * *************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 取消邀请信息
        /// </summary>
        protected void ShareCancel()
        {
            /***************************************************************************************************
             * 更新用户放弃邀请ID信息
             * *************************************************************************************************/
            DbHelper.Connection.Update("Fooke_User", dictionary: new Dictionary<string, string>() { 
                  {"shareCancel","1"}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /***************************************************************************************************
             * 输出数据处理结果
             * *************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 判断是否显示邀请
        /// </summary>
        /// <returns></returns>
        protected bool ShowInvited()
        {
            bool ShowInvitedContianer = false;
            /***************************************************************************************************
             * 更新用户放弃邀请ID信息
             * *************************************************************************************************/
            if (MemberRs["ParentID"].ToString() == "0"
            && MemberRs["shareCancel"].ToString() == "0")
            { ShowInvitedContianer = true; }
            /***************************************************************************************************
             * 输出数据处理结果
             * *************************************************************************************************/
            return ShowInvitedContianer;
        }
        /// <summary>
        /// 获取并保存用户指纹数据信息
        /// </summary>
        protected void SaveFinger()
        {
            /****************************************************************************************
             * 获取扫码指纹数据数据信息
             * **************************************************************************************/
            string strFinger = RequestHelper.GetRequest("strFinger").toString();
            if (string.IsNullOrEmpty(strFinger)) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            else if (strFinger.Length <= 0) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            else if (strFinger.Length <= 5) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            else if (strFinger.Length >= 30) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            /****************************************************************************************
             * 获取用户分享邀请ID数据
             * **************************************************************************************/
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_shareLogs]", new Dictionary<string, object>() {
                 {"strFinger",strFinger},
                 {"isFind","1"}
            });
            /****************************************************************************************
             * 判断邀请指纹请求数据是否存在
             * **************************************************************************************/
            if (sRs == null) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            else if (new Fooke.Function.String(sRs["ParentID"].ToString()).cInt() <= 0) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            else if (sRs["ParentID"].ToString() == MemberRs["UserID"].ToString()) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            //else if (new Fooke.Function.String(sRs["ParentID"].ToString()).cInt() >= new Fooke.Function.String(MemberRs["UserID"].ToString()).cInt())
            //{ Response.Redirect("index.aspx?Fingerprint=false"); Response.End(); }
            /****************************************************************************************
             * 判断是否为无效的邀请ID
             * **************************************************************************************/
            DataRow UserRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() { 
                {"UserID",sRs["ParentID"].ToString()}
            });
            if (UserRs == null) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            else if (UserRs["UserID"].ToString() == MemberRs["UserID"].ToString()) { Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&Fingerprint=false"); Response.End(); }
            /****************************************************************************************
             * 跳转到用户完善分享信息数据界面
             * **************************************************************************************/
            Response.Redirect(string.Format("index.aspx?token=" + MemberRs["strTokey"] + "&strFinger=" + strFinger + "&action=shareConfirm&ParentID={0}&auto=true",
                new Fooke.Function.String(sRs["ParentID"].toString()).toInt()));
            Response.End();
        }

        #endregion

        /************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 新手红包处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * **********************************************************************************************************/
        #region 新手红包处理区域
        /// <summary>
        /// 是否显示红包?
        /// </summary>
        /// <returns></returns>
        public bool ShowRookie()
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
            return ShowRookieContianer;
        }
        /// <summary>
        /// 领取新手红包
        /// </summary>
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
        #endregion
        /************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 网页公告处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * **********************************************************************************************************/
        #region 网页公告处理区域
        public string GetNotificationXml()
        {
            StringBuilder strBuilder = new StringBuilder();
            /**************************************************************************************************
             * 开始执行数据处理
             * ************************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindUserNotificationList]");
            strBuilder.Append("<table><tr>");
            foreach (DataRow cRs in thisTab.Rows)
            {
                strBuilder.Append("<td>" + cRs["strContext"] + "</td>");
            }
            strBuilder.Append("</tr></table>");
            /**************************************************************************************************
             * 返回数据处理结果
             * ************************************************************************************************/
            return strBuilder.ToString();
        }

        #endregion
    }
}