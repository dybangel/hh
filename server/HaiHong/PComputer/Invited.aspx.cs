using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using Fooke.Function;
using Fooke.Code;
namespace Fooke.Web.Member
{
    public partial class Invited : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "qrcode": SafeFile(); Response.End(); break;
                case "showcode": Showcode(); Response.End(); break;
                case "showtext": Showtext(); Response.End(); break; 
                default: strDefault(); Response.End(); break;
            }
        }

        /// <summary>
        /// 收徒赚钱
        /// </summary>
        protected void strDefault()
        {
            //收徒规则
            string InvitedRule = this.GetParameter("invitedRule", "UserXML").ToString();
            if (!string.IsNullOrEmpty(InvitedRule))
            {
                InvitedRule = InvitedRule.Replace("\n", "<br/>");
                InvitedRule = InvitedRule.Replace("\n\r", "<br/>");
                InvitedRule = InvitedRule.Replace("<br>", "<br/>");
            }else{InvitedRule = "想躺着赚钱嘛，快让身边的伙伴加入我们吧";}
            /****************************************************************************************************************
             * 获取用户邀请奖励信息
             * **************************************************************************************************************/
            DataRow TotalRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserInvitedComputer]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")},
                {"DateKey",DateTime.Now.ToString("yyyyMMdd")}
            });
            if (TotalRs == null) { this.ErrorMessage("统计用户邀请数据信息失败!"); Response.End(); }
            //邀请者（师傅）获得
            double SCAmount = this.GetParameter("SCAmount", "shareXml").cDouble();
            //新用户获得
            double STAmount = this.GetParameter("STAmount", "shareXml").cDouble();
            //计算当前用户的所有下级
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserChildrenAll]", new Dictionary<string, object>() {
                    {"NodeID",MemberRs["UserID"].ToString()}
                });
            int ChildrenNum = string.IsNullOrEmpty(sRs[0].ToString()) ? 0 : Convert.ToInt32(sRs[0].ToString());
            /****************************************************************************************************************
             * 输出网页内容数据信息
             * **************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/invited/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "shareurl": strValue = ShareUrl(); break;
                    case "ischild": strValue = ChildrenNum.ToString(); break;
                    case "todaychild": strValue = TotalRs["Todaynum"].ToString(); break;
                    case "todayamount": strValue = TotalRs["todayamount"].ToString(); break;
                    case "shareamount": strValue = TotalRs["shareamount"].ToString(); break;
                    case "scamount": strValue = SCAmount.ToString(); break;
                    case "stamount": strValue = STAmount.ToString(); break;
                    case "invitedrule": strValue = InvitedRule; break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 收徒二维码
        /// </summary>
        protected void Showcode()
        {
            /****************************************************************************************************************
            * 获取请求参数信息并验证
            * **************************************************************************************************************/
            string strToken = RequestHelper.GetRequest("token").toString();
            if (string.IsNullOrEmpty(strToken)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strToken.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strToken.Length <= 20) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strToken.Length >= 36) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"strTokey",strToken}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("用户账号已停止使用！"); Response.End(); }
            /****************************************************************************************************************
             * 输出网页内容数据信息
             * **************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/invited/showcode.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "shareurl": strValue = ShareUrl(); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 收徒链接地址
        /// </summary>
        protected void Showtext()
        {
            /****************************************************************************************************************
            * 获取请求参数信息并验证
            * **************************************************************************************************************/
            string strToken = RequestHelper.GetRequest("token").toString();
            if (string.IsNullOrEmpty(strToken)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strToken.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strToken.Length <= 20) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strToken.Length >= 36) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"strTokey",strToken}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("用户账号已停止使用！"); Response.End(); }
            /****************************************************************************************************************
             * 输出网页内容数据信息
             * **************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/invited/showtext.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "shareurl": string strKey = string.Format("邀请好友-|-|-{0}-|-|-{0}-|-|-邀请好友", MemberRs["UserID"].ToString());
                        strKey = new Fooke.Function.String(strKey).ToMD5().Substring(0, 24).ToUpper();
                        strValue = string.Format("{0}{1}/share/index.aspx?uid={2}&key={3}",
                            FunctionCenter.SiteUrl(), Win.ApplicationPath,
                            MemberRs["UserID"].ToString(), strKey); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }

        protected void SafeFile()
        {
            /****************************************************************************************************************
             * 获取请求参数信息并验证
             * **************************************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("用户账号已停止使用！"); Response.End(); }
            /****************************************************************************************************************
            * 生成二维码的Url内容信息
            * **************************************************************************************************************/
            string strKey = string.Format("邀请好友-|-|-{0}-|-|-{0}-|-|-邀请好友", MemberRs["UserID"].ToString());
            strKey = new Fooke.Function.String(strKey).ToMD5().Substring(0, 24).ToUpper();
            string shareUrl = string.Format("{0}{1}/share/index.aspx?uid={2}&key={3}",
                FunctionCenter.SiteUrl(), Win.ApplicationPath,
                MemberRs["UserID"].ToString(), strKey);
            /****************************************************************************************************************
            * 获取请求参数信息并验证
            * **************************************************************************************************************/
            new Fooke.Code.QRCodeHelper().Create(context: System.Web.HttpContext.Current,
                url: shareUrl);
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            Response.End();
        }

        /************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 公共方法处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * **********************************************************************************************/
        #region 公共方法处理区域
        /// <summary>
        /// 获取分享
        /// </summary>
        /// <returns></returns>
        protected string ShareUrl()
        {
            string strKey = string.Format("邀请好友-|-|-{0}-|-|-{0}-|-|-邀请好友", MemberRs["UserID"].ToString());
            strKey = new Fooke.Function.String(strKey).ToMD5().Substring(0, 24).ToUpper();
            string shareUrl = string.Format("{0}{1}/share/index.aspx?uid={2}&key={3}",
                FunctionCenter.SiteUrl(), Win.ApplicationPath,
                MemberRs["UserID"].ToString(), strKey);
            return HttpUtility.UrlEncode(shareUrl);
        }

        #endregion
    }

    
}