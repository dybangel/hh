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
namespace Fooke.Web.Member
{
    public partial class Alipay : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /***********************************************************************************
             * 检测管理员是否设置开启兑吧兑换
             * *********************************************************************************/
            string isOpen = this.GetParameter("isOpen", "AlipayXml").toInt();
            if (isOpen != "1") { this.ErrorMessage("系统已关闭提现功能,请联系客服！"); Response.End(); }
            /***********************************************************************************
             * 判断用户提现是否绑定了资料
             * *********************************************************************************/
            if (MemberRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前账号已停止使用,请联系客服！"); Response.End(); }
            if (string.IsNullOrEmpty(MemberRs["strMobile"].ToString())) { this.ErrorMessage("您还没有绑定手机,请先绑定手机！"); Response.End(); }
            else if (MemberRs["strMobile"].ToString().Length <= 0) { this.ErrorMessage("您还没有绑定手机,请先绑定手机！"); Response.End(); }
            else if (!MemberRs["strMobile"].ToString().isMobile()) { this.ErrorMessage("您还没有绑定手机,请先绑定手机！"); Response.End(); }
            /***********************************************************************************
             * 验证用户是否绑定了支付宝
             * *********************************************************************************/
            //if (string.IsNullOrEmpty(MemberRs["AlipayChar"].ToString())) { AlipayBind(); Response.End(); }
            //else if (MemberRs["AlipayChar"].ToString().Length <= 0) { AlipayBind(); Response.End(); }
            //else if (MemberRs["AlipayChar"].ToString().Length <= 5) { AlipayBind(); Response.End(); }
            //if (string.IsNullOrEmpty(MemberRs["Alipayname"].ToString())) { AlipayBind(); Response.End(); }
            //else if (MemberRs["Alipayname"].ToString().Length <= 0) { AlipayBind(); Response.End(); }
            /***********************************************************************************
             * 检测管理员是否开启兑吧兑换
             * *********************************************************************************/
            string isDollar = this.GetParameter("isDollar", "AlipayXml").toInt();
            if (isDollar == "1") { Response.Redirect("../api/duiba.aspx?uid=" + MemberRs["strTokey"] + ""); Response.End(); }
            /***********************************************************************************
             * 普通提现方式输出提现界面
             * *********************************************************************************/
            switch (strRequest)
            {
                case "alipay": strAlipay(); Response.End(); break;
                case "wechat": strWeChat(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /***************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 微信提现模块
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *************************************************************************************************************/
        #region 微信提现模块
        /// <summary>
        /// 填写实名信息
        /// </summary>
        protected void strFullname()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/alipay/fullname.html");
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
        /// <summary>
        /// 微信提现
        /// </summary>
        protected void strWeChat()
        {
            /*******************************************************************************************
             * 判断提现方式是否开启
             * *****************************************************************************************/
            string isWeChat = this.GetParameter("isWeChat", "AlipayXml").toInt();
            if (isWeChat != "1") { this.ErrorMessage("已关闭微信提现功能,请联系客服或换过提现方式！"); Response.End(); }
            //线上还是线下提现
            string WeIsOnLine = this.GetParameter("WeIsOnLine", "AlipayXml").toInt();
            if (WeIsOnLine == "1")
            {
                /*******************************************************************************************
                 * 获取并验证用户绑定微信信息
                 * *****************************************************************************************/
                if (MemberRs["AuthorizationType"].toString().ToLower() == "define") { this.ErrorMessage("您还没有绑定微信,请先绑定微信！"); Response.End(); }
                else if (string.IsNullOrEmpty(MemberRs["AuthorizationType"].toString())) { this.ErrorMessage("您还没有绑定微信,请先绑定微信！"); Response.End(); }
                else if (MemberRs["AuthorizationType"].toString().Length <= 0) { this.ErrorMessage("您还没有绑定微信,请先绑定微信！"); Response.End(); }
                if (string.IsNullOrEmpty(MemberRs["AuthorizationKey"].toString())) { this.ErrorMessage("您还没有绑定微信,请先绑定微信！"); Response.End(); }
                else if (MemberRs["AuthorizationKey"].toString().Length <= 0) { this.ErrorMessage("您还没有绑定微信,请先绑定微信！"); Response.End(); }
            }
            else
            {
                /*******************************************************************************************
                 * 获取并验证用户资料是否绑定
                 * *****************************************************************************************/
                if (string.IsNullOrEmpty(MemberRs["Fullname"].ToString())) { strFullname(); Response.End(); }
                else if (MemberRs["Fullname"].ToString().Length <= 0) { strFullname(); Response.End(); }
                if (string.IsNullOrEmpty(MemberRs["strWechat"].ToString())) { strFullname(); Response.End(); }
                else if (MemberRs["strWechat"].ToString().Length <= 0) { strFullname(); Response.End(); }
                else if (MemberRs["strWechat"].ToString().Length > 11) { strFullname(); Response.End(); }
            }
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/alipay/wechat.html");
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

        /***************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 支付宝提现模块
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *************************************************************************************************************/
        #region 支付宝提现模块
        /// <summary>
        /// 提现到支付宝
        /// </summary>
        protected void strAlipay()
        {
            /*******************************************************************************************
             * 判断提现方式是否开启
             * *****************************************************************************************/
            string isAlipay = this.GetParameter("isAlipay", "AlipayXml").toInt();
            if (isAlipay != "1") { this.ErrorMessage("已关闭支付宝提现功能,请联系客服或换过提现方式！"); Response.End(); }
            /*******************************************************************************************
             * 获取并判断支付宝账号是否绑定
             * *****************************************************************************************/
            if (string.IsNullOrEmpty(MemberRs["AlipayChar"].ToString())) { AlipayBind(); Response.End(); }
            else if (MemberRs["AlipayChar"].ToString().Length <= 0) { AlipayBind(); Response.End(); }
            else if (MemberRs["AlipayChar"].ToString().Length <= 5) { AlipayBind(); Response.End(); }
            if (string.IsNullOrEmpty(MemberRs["Alipayname"].ToString())) { AlipayBind(); Response.End(); }
            else if (MemberRs["Alipayname"].ToString().Length <= 0) { AlipayBind(); Response.End(); }
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/alipay/alipay.html");
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
        /// <summary>
        /// 绑定支付宝账号
        /// </summary>
        protected void AlipayBind()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/alipay/alipaybind.html");
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
        /// <summary>
        /// 提现默认界面
        /// </summary>
        protected void strDefault()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/alipay/default.html");
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
    }
}