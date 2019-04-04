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
using System.Drawing;
using System.Drawing.Drawing2D;
using ZXing;
namespace Fooke.Web.Pay
{
    public partial class Index : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /************************************************************************
             * 输出网页内容
             * **********************************************************************/
            switch (strRequest)
            {
                case "save": SaveResponse(); Response.End(); break;
                case "saveActive": SaveActive(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 显示默认网页内容
        /// </summary>
        protected void strDefault()
        {
            /**********************************************************************
             * 拉取用户验证信息保证用户当前处于登录状态
             * *********************************************************************/
            this.VerifyMemberLogin((str) =>
            {
                string returnUrl = string.Format(Win.ApplicationPath + "/pay/index.aspx");
                Response.Redirect(string.Format(Win.ApplicationPath + "/Member/Login.aspx?returnUrl={0}", new Fooke.Function.String(returnUrl).ToEncryptionDes()));
                Response.End();
            });
            if (this.MemberRs == null)
            {
                string returnUrl = string.Format(Win.ApplicationPath + "/pay/index.aspx");
                Response.Redirect(string.Format(Win.ApplicationPath + "/Member/Login.aspx?returnUrl={0}", new Fooke.Function.String(returnUrl).ToEncryptionDes()));
                Response.End();
            }
            /**********************************************************************
             * 输出网页内容
             * ********************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {

                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /***************************************************************************************
         * 数据处理区域
         * *************************************************************************************/
        protected void SaveResponse()
        {

            /**********************************************************************
            * 拉取用户验证信息保证用户当前处于登录状态
            * *********************************************************************/
            this.VerifyMemberLogin((str) =>
            {
                string returnUrl = string.Format(Win.ApplicationPath + "/pay/index.aspx");
                Response.Redirect(string.Format(Win.ApplicationPath + "/Member/Login.aspx?returnUrl={0}", new Fooke.Function.String(returnUrl).ToEncryptionDes()));
                Response.End();
            });
            if (this.MemberRs == null)
            {
                string returnUrl = string.Format(Win.ApplicationPath + "/pay/index.aspx");
                Response.Redirect(string.Format(Win.ApplicationPath + "/Member/Login.aspx?returnUrl={0}", new Fooke.Function.String(returnUrl).ToEncryptionDes()));
                Response.End();
            }
            /***********************************************************************************
             * 开始验证数据安全
             * *********************************************************************************/
            string isOpen = this.GetParameter("isOpen", "payxml").toString();
            if (isOpen != "1") { this.ErrorMessage("对不起,充值系统当前已被管理员关闭！"); Response.End(); }
            string PaymentID = RequestHelper.GetRequest("PaymentID").toInt();
            if (PaymentID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            /***********************************************************************
             * 验证充值方式是否存在
             * *********************************************************************/
            DataRow PaymentRs = DbHelper.Connection.ExecuteFindRow("Stored_FindPayment", new Dictionary<string, object>() {
                {"PaymentID",PaymentID}
            });
            if (PaymentRs == null) { this.ErrorMessage("拉取支付平台信息失败,请重试！"); Response.End(); }
            if (PaymentRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前支付方式暂时无法使用,请换过一种支付方式吧!"); Response.End(); }
            /**********************************************************************
             * 获取充值金额
             * ********************************************************************/
            double Amount = RequestHelper.GetRequest("Amount").cDouble();
            if (Amount <= 0) { this.ErrorMessage("请选择要充值的金额！"); Response.End(); }
            double MinNumber = this.GetParameter("MinNumber", "PayXml").cDouble();
            if (Amount < MinNumber) { this.ErrorMessage(string.Format("单笔充值最少充值{0}元", MinNumber)); Response.End(); }
            double MaxNumber = this.GetParameter("MaxNumber", "PayXml").cDouble();
            if (Amount > MaxNumber) { this.ErrorMessage(string.Format("单笔充值最多充值{0}元", MaxNumber)); Response.End(); }
            /***********************************************************************
             * 检查当日充值次数是否超过上线
             * *********************************************************************/
            int iTimer = this.GetParameter("iTimer", "PayXml").cInt();
            if (iTimer >= 1)
            {
                DataRow iRs = FindHelper.Connection.FindRow(TableCenter.UserPay, columns: "coun(0)", Params: " and UserID = " + MemberRs["UserID"] + " and addtime>='" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "'");
                if (iRs != null && new Fooke.Function.String(iRs[0].ToString()).cInt() >= iTimer) { this.ErrorMessage("对不起,你今日充值次数已经达到上限,每日最多允许充值" + iTimer + "次"); Response.End(); }
            }
            /************************************************************************
             * 生成充值标识
             * **********************************************************************/
            string thisKey = string.Format("在线充值-|-|-{0}-|-|-{1}", MemberRs["UserID"], DateTime.Now.ToString("yyyyMMddHHmmss"));
            thisKey = new Fooke.Function.String(thisKey).ToMD5().Substring(0, 24).ToUpper();
            /*************************************************************************
             * 保存充值资料信息
             * ***********************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["strKey"] = thisKey;
            oDictionary["PaymentID"] = PaymentRs["ID"].ToString();
            oDictionary["PaymentName"] = PaymentRs["Model"].ToString();
            oDictionary["UserID"] = MemberRs["UserID"].ToString();
            oDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            oDictionary["OrderID"] = "0";
            oDictionary["OrderMode"] = "用户充值";
            oDictionary["Intro"] = string.Format("使用{0}充值{1}元", PaymentRs["Model"].ToString(), Amount.ToString("0.00"));
            oDictionary["Number"] = Amount.ToString("0.00");
            oDictionary["iReward"] = "0";
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_AddRechargeableLog", oDictionary);
            if (thisRs == null) { this.ErrorMessage("保存数据过程中发生未知错误,请返回重试！"); Response.End(); }
            /*****************************************************************************
             * 开始保存数据信息
             * ***************************************************************************/
            Response.Redirect(Win.ApplicationPath + "/Pay/Redirect.aspx?thisKey=" + thisRs["strKey"] + "");
            Response.End();
        }
        /// <summary>
        /// 保存充值信息
        /// </summary>
        protected void SaveActive()
        {
            /**********************************************************************
            * 拉取用户验证信息保证用户当前处于登录状态
            * *********************************************************************/
            this.VerifyMemberLogin((str) =>
            {
                string returnUrl = string.Format(Win.ApplicationPath + "/pay/index.aspx");
                Response.Redirect(string.Format(Win.ApplicationPath + "/Member/Login.aspx?returnUrl={0}", new Fooke.Function.String(returnUrl).ToEncryptionDes()));
                Response.End();
            });
            if (this.MemberRs == null)
            {
                string returnUrl = string.Format(Win.ApplicationPath + "/pay/index.aspx");
                Response.Redirect(string.Format(Win.ApplicationPath + "/Member/Login.aspx?returnUrl={0}", new Fooke.Function.String(returnUrl).ToEncryptionDes()));
                Response.End();
            }
            /***********************************************************************************
             * 开始验证数据安全
             * *********************************************************************************/
            string isOpen = this.GetParameter("isOpen", "payxml").toString();
            if (isOpen != "1") { this.ErrorMessage("对不起,充值系统当前已被管理员关闭！"); Response.End(); }
            string PaymentID = RequestHelper.GetRequest("PaymentID").toInt();
            if (PaymentID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            /***********************************************************************
             * 验证充值方式是否存在
             * *********************************************************************/
            DataRow PaymentRs = DbHelper.Connection.ExecuteFindRow("Stored_FindPayment", new Dictionary<string, object>() {
                {"PaymentID",PaymentID}
            });
            if (PaymentRs == null) { this.ErrorMessage("拉取支付平台信息失败,请重试！"); Response.End(); }
            if (PaymentRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前支付方式暂时无法使用,请换过一种支付方式吧!"); Response.End(); }
            /**********************************************************************
             * 获取充值金额
             * ********************************************************************/
            double Amount = this.GetParameter("OpenAmount", "shareXml").cDouble();
            if (MemberRs["isActive"].ToString() == "1") { Amount = this.GetParameter("OpenAmountTo", "shareXml").cDouble(); }
            if (Amount <= 0) { this.ErrorMessage("获取充值金额信息失败,请重试！"); Response.End(); }
            int OpenDays = RequestHelper.GetRequest("OpenDays").cInt();
            if (OpenDays <= 0) { this.ErrorMessage("获取用户激活天数失败,请联系客服!"); Response.End(); }
            /************************************************************************
             * 生成充值标识
             * **********************************************************************/
            string thisKey = string.Format("购买VIP-|-|-{0}-|-|-{1}", MemberRs["UserID"], DateTime.Now.ToString("yyyyMMddHHmmss"));
            thisKey = new Fooke.Function.String(thisKey).ToMD5().Substring(0, 24).ToUpper();
            /*************************************************************************
             * 保存充值资料信息
             * ***********************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["strKey"] = thisKey;
            oDictionary["PaymentID"] = PaymentRs["ID"].ToString();
            oDictionary["PaymentName"] = PaymentRs["Model"].ToString();
            oDictionary["UserID"] = MemberRs["UserID"].ToString();
            oDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            oDictionary["OrderID"] = OpenDays;
            oDictionary["OrderMode"] = "用户激活";
            oDictionary["Intro"] = string.Format("使用{0}充值{1}元", PaymentRs["Model"].ToString(), Amount.ToString("0.00"));
            oDictionary["Number"] = Amount.ToString("0.00");
            oDictionary["iReward"] = "0";
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_AddRechargeableLog", oDictionary);
            if (thisRs == null) { this.ErrorMessage("保存数据过程中发生未知错误,请返回重试！"); Response.End(); }
            /*****************************************************************************
             * 开始保存数据信息
             * ***************************************************************************/
            Response.Redirect(Win.ApplicationPath + "/Pay/Redirect.aspx?thisKey=" + thisRs["strKey"] + "");
            Response.End();
        }
    }
}