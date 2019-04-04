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
    public partial class WebChat : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "talk": strTalk(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }

        protected void strTalk()
        {
            /**************************************************************************************
             * 验证请求数据的合法性
             * ************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("获取请求参数失败,请重试!"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["UserID"].ToString() == MemberRs["UserID"].ToString())
            { this.ErrorMessage("越权操作,你不能自己与自己聊天！"); Response.End(); }
            /**************************************************************************************
             * 获取交易订单编号数据信息
             * ************************************************************************************/
            string OrderID = RequestHelper.GetRequest("OrderID").toInt();
            if (OrderID == "0") { this.ErrorMessage("获取交易参数信息失败,请重试！"); Response.End(); }
            /**************************************************************************************
             * 输出网页内容信息,解析网页模板
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/webchat/talk.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "orderid": strValue = OrderID; break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }), isLabel: true);
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 默认主页
        /// </summary>
        protected void strDefault()
        {
            StringBuilder strBuilder = new StringBuilder();
            /**************************************************************************************
            * 查询用户已存在的银行卡信息
            * ************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindBank]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            foreach (DataRow oRs in thisTab.Rows)
            {
                strBuilder.Append("<div onclick=\"window.location='?action=edit&bankid=" + oRs["BankID"] + "'\" class=\"hback\">");
                strBuilder.Append("<div class=\"mode\">" + oRs["Bankname"] + "</div>");
                strBuilder.Append("<div class=\"branch\">" + oRs["Branch"] + "</div>");
                strBuilder.Append("<div class=\"account\">" + oRs["BankAccount"].ToString().hideText(4, 4) + "</div>");
                strBuilder.Append("</div>");
            }
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/bank/default.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strBuilder.ToString(); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }), isLabel: true);
            /**************************************************************************************
             * 输出网页内容信息
             * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 处理请求数据
        /// </summary>
        protected void AddSave()
        {
            /****************************************************************************************************************
             * 验证账户是否已进行了实名认证
             * **************************************************************************************************************/
            DataRow AutRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAuthentication]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            if (AutRs == null) { this.ErrorMessage("账户未进行身份认证,请先认证身份！"); Response.End(); }
            else if (AutRs["isAuthentication"].ToString() != "1") { this.ErrorMessage("账户未进行身份认证,请先认证身份！"); Response.End(); }
            /****************************************************************************************************************
             * 获取所属银行名称并验证
             * **************************************************************************************************************/
            string Bankname = RequestHelper.GetRequest("Bankname").ToString();
            if (string.IsNullOrEmpty(Bankname)) { this.ErrorMessage("请选择要绑卡的银行！"); Response.End(); }
            else if (Bankname.Length <= 1) { this.ErrorMessage("获取银行名称信息失败,请重试！"); Response.End(); }
            else if (Bankname.Length >= 20) { this.ErrorMessage("银行名称字段长度请限制在20个字符内！"); Response.End(); }
            /****************************************************************************************************************
             * 获取用户开户行名称并验证
             * **************************************************************************************************************/
            string Branch = RequestHelper.GetRequest("Branch").ToString();
            if (string.IsNullOrEmpty(Branch)) { this.ErrorMessage("开户行名称不能为空！"); Response.End(); }
            else if (Branch.Length <= 4) { this.ErrorMessage("开户行名称不能少于4个字符！"); Response.End(); }
            else if (Branch.Length >= 30) { this.ErrorMessage("开户行名称长度请限制在30个汉字内！"); Response.End(); }
            /****************************************************************************************************************
             * 获取银行卡账号并验证
             * **************************************************************************************************************/
            string BankAccount = RequestHelper.GetRequest("BankAccount").ToString();
            if (string.IsNullOrEmpty(BankAccount)) { this.ErrorMessage("银行卡账号不能为空!"); Response.End(); }
            else if (BankAccount.Length <= 10) { this.ErrorMessage("银行卡账号不能少于10个字符！"); Response.End(); }
            else if (BankAccount.Length >= 30) { this.ErrorMessage("银行卡账号不能大于30个字符！"); Response.End(); }
            /****************************************************************************************************************
             * 判断银行卡是否已被绑定
             * **************************************************************************************************************/
            DataRow iRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBankToday]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"Bankname",Bankname},
                {"BankAccount",BankAccount}
            });
            if (iRs == null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            else if (iRs.Table.Columns["Total"] == null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            else if (iRs["isThis"].ToString() != "0") { this.ErrorMessage("你已经绑定了一张相同的银行卡,请稍后重试！"); Response.End(); }
            else if (iRs["Total"].ToString().cInt() >= 10) { this.ErrorMessage("用户绑卡数量已超过系统上限！"); Response.End(); }
            /****************************************************************************************************************
             * 处理用户处理信息
             * **************************************************************************************************************/
            string isRepeat = this.GetParameter("isRepeat", "BankXml").toInt();
            int MaxLimit = this.GetParameter("MaxLimit", "BankXml").cInt();
            if (isRepeat == "1" && iRs["isUnder"].ToString() != "1") { this.ErrorMessage("当前银行卡已被绑定！"); Response.End(); }
            else if (MaxLimit != 0 && MaxLimit <= iRs["Total"].ToString().cInt()) { this.ErrorMessage("用户绑卡数量已超过系统上限！"); Response.End(); }
            /****************************************************************************************************************
             * 开始保存用户绑定银行卡信息
             * **************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["BankID"] = "0";
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["Bankname"] = Bankname;
            thisDictionary["BankAccount"] = BankAccount;
            thisDictionary["Holdername"] = AutRs["Fullname"].ToString();
            thisDictionary["Branch"] = Branch;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveBank]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            this.ErrorMessage("银行卡添加成功!", iSuccess: true, iUrl: "Bank.aspx");
            Response.End();
        }
        /// <summary>
        /// 修改编辑
        /// </summary>
        protected void SaveUpdate()
        {
            /****************************************************************************************************************
             * 验证请求数据的合法性
             * **************************************************************************************************************/
            string BankID = RequestHelper.GetRequest("BankID").toInt();
            if (BankID == "0") { this.ErrorMessage("获取请求参数失败,请重试!"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBank]", new Dictionary<string, object>() {
                {"BankID",BankID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["UserID"].ToString() != MemberRs["UserID"].ToString()) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /****************************************************************************************************************
             * 验证银行卡是否允许修改,或修改日期是否合法
             * **************************************************************************************************************/
            string isEdit = this.GetParameter("isEdit", "BankXml").toInt();
            if (isEdit != "1") { this.ErrorMessage("已绑定的银行卡不允许修改！"); Response.End(); }
            int EditDate = this.GetParameter("EditDate", "bankXml").cInt();
            if (EditDate != 0 && cRs["LastDate"].ToString().cDate().AddDays(EditDate) >= DateTime.Now)
            { this.ErrorMessage("银行卡修改太频繁,暂时不允许修改!"); Response.End(); }
            /****************************************************************************************************************
             * 验证账户是否已进行了实名认证
             * **************************************************************************************************************/
            DataRow AutRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAuthentication]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            if (AutRs == null) { this.ErrorMessage("账户未进行身份认证,请先认证身份！"); Response.End(); }
            else if (AutRs["isAuthentication"].ToString() != "1") { this.ErrorMessage("账户未进行身份认证,请先认证身份！"); Response.End(); }
            /****************************************************************************************************************
             * 获取所属银行名称并验证
             * **************************************************************************************************************/
            string Bankname = RequestHelper.GetRequest("Bankname").ToString();
            if (string.IsNullOrEmpty(Bankname)) { this.ErrorMessage("请选择要绑卡的银行！"); Response.End(); }
            else if (Bankname.Length <= 1) { this.ErrorMessage("获取银行名称信息失败,请重试！"); Response.End(); }
            else if (Bankname.Length >= 20) { this.ErrorMessage("银行名称字段长度请限制在20个字符内！"); Response.End(); }
            /****************************************************************************************************************
             * 获取用户开户行名称并验证
             * **************************************************************************************************************/
            string Branch = RequestHelper.GetRequest("Branch").ToString();
            if (string.IsNullOrEmpty(Branch)) { this.ErrorMessage("开户行名称不能为空！"); Response.End(); }
            else if (Branch.Length <= 4) { this.ErrorMessage("开户行名称不能少于4个字符！"); Response.End(); }
            else if (Branch.Length >= 30) { this.ErrorMessage("开户行名称长度请限制在30个汉字内！"); Response.End(); }
            /****************************************************************************************************************
             * 获取银行卡账号并验证
             * **************************************************************************************************************/
            string BankAccount = RequestHelper.GetRequest("BankAccount").ToString();
            if (string.IsNullOrEmpty(BankAccount)) { this.ErrorMessage("银行卡账号不能为空!"); Response.End(); }
            else if (BankAccount.Length <= 10) { this.ErrorMessage("银行卡账号不能少于10个字符！"); Response.End(); }
            else if (BankAccount.Length >= 30) { this.ErrorMessage("银行卡账号不能大于30个字符！"); Response.End(); }
            /****************************************************************************************************************
             * 判断银行卡是否已被绑定,要求重复信息
             * **************************************************************************************************************/
            string isRepeat = this.GetParameter("isRepeat", "BankXml").toInt();
            if (isRepeat != "1")
            {
                DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBank]", new Dictionary<string, object>() {
                    {"Bankname",Bankname},
                    {"BankAccount",BankAccount}
                });
                if (oRs != null && oRs["BankID"].ToString() != cRs["BankID"].ToString())
                { this.ErrorMessage("你已经绑定了相同的银行卡,不需要重复绑定！"); Response.End(); }
            }
            /****************************************************************************************************************
             * 开始保存用户绑定银行卡信息
             * **************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["BankID"] = cRs["BankID"].ToString();
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["Bankname"] = Bankname;
            thisDictionary["BankAccount"] = BankAccount;
            thisDictionary["Holdername"] = AutRs["Fullname"].ToString();
            thisDictionary["Branch"] = Branch;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveBank]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            this.ErrorMessage("银行卡添加成功!", iSuccess: true, iUrl: "Bank.aspx");
            Response.End();
        }
    }
}