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
    public partial class Alipay : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "savename": SaveFullname(); Response.End(); break;
                case "save": AddSave(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 默认列表接口
        /// </summary>
        protected void strDefault()
        {
            /*****************************************************************************************
             * 构建查询语句条件
             * ***************************************************************************************/
            string strParams = " and UserID=" + MemberRs["UserID"] + "";
            /*****************************************************************************************
            * 验证其他的条件信息
            * ***************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            if (!string.IsNullOrEmpty(Keywords)) { strParams += " and thisKey like '%" + Keywords + "%'"; }
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParams += " and Addtime>='" + StarDate + "'"; }
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParams += " and Addtime<='" + EndDate + "'"; }
            string iBonus = RequestHelper.GetRequest("iBonus").toInt();
            if (iBonus == "1") { strParams += " and iBonus=1"; }
            /*****************************************************************************************
             * 获取分页显示数量
             * ***************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(20);
            /*****************************************************************************************
             * 创建分页sql语句
             * ***************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "AlipayID,OrderKey,FokeMode,BizKey,UserID,Nickname,thisAmount,thisBalance,thisInterval,Addtime,LastDate,AccessMode,AccessName,AccessHolder,strRemark,Affairs";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "AlipayID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " AlipayID Desc";
            PageCenterConfig.Tablename = "Fooke_Alipay";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Alipay", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /**************************************************************************************
            * 输出网页内容信息
            * *************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }
        /********************************************************************************************
         * 数据处理区域功能
         * ******************************************************************************************/
        /// <summary>
        /// 保存提现信息
        /// </summary>
        protected void AddSave()
        {
            /************************************************************************************************
             * 验证提现参数配置信息,验证提现时间是否已经达到要求
             * **********************************************************************************************/
            string isOpen = this.GetParameter("isOpen", "AlipayXml").toInt();
            if (isOpen != "1") { this.ErrorMessage("提现功能已关闭,请在参数配置中开启！"); Response.End(); }
            double StartHour = this.GetParameter("StartHour", "AlipayXml").cDouble();
            if (StartHour <= 0) { this.ErrorMessage("获取提现时间设定失败,请联系客服"); Response.End(); }
            else if (StartHour >= new Fooke.Function.String(DateTime.Now.ToString("HHmm")).cDouble()) { this.ErrorMessage("提现的时间未到,每日提现的时间为" + StartHour + ",请等待！"); Response.End(); }
            double EndHour = this.GetParameter("EndHour", "AlipayXml").cDouble();
            if (EndHour <= 0) { this.ErrorMessage("获取提现时间设定失败,请联系客服"); Response.End(); }
            else if (EndHour <= new Fooke.Function.String(DateTime.Now.ToString("HHmm")).cDouble())
            { this.ErrorMessage("提现时间已经过了,请明日再来吧！"); Response.End(); }
            //微信是否线上提现
            string WeIsOnLine = this.GetParameter("WeIsOnLine", "AlipayXml").toInt();
            //支付宝是否线上提现
            string AlIsOnLine = this.GetParameter("AlIsOnLine", "AlipayXml").toInt();
            /************************************************************************************************
             * 验证用户信息
             * **********************************************************************************************/
            if (MemberRs == null) { this.ErrorMessage("获取请求数据错误,请重试！"); Response.End(); }
            else if (MemberRs["isDisplay"].ToString() != "1") { this.ErrorMessage("用户账号已停止使用！"); Response.End(); }
            /************************************************************************************************
             * 验证提现方式信息
             * **********************************************************************************************/
            string FokeMode = RequestHelper.GetRequest("FokeMode").ToString();
            if (string.IsNullOrEmpty(FokeMode)) { this.ErrorMessage("请选择提现方式！"); Response.End(); }
            else if (FokeMode.Length <= 1) { this.ErrorMessage("获取提现方式信息失败,请重试！"); Response.End(); }
            else if (FokeMode.Length >= 10) { this.ErrorMessage("提现方式字段长度请限制在10个汉字内!"); Response.End(); }
            /************************************************************************************************
             * 验证支付宝提现方式的合法性
             * **********************************************************************************************/
            string isAlipay = this.GetParameter("isAlipay", "AlipayXml").toInt();
            string isWeChat = this.GetParameter("isWeChat", "AlipayXml").toInt();
            if (FokeMode == "支付宝提现" && isAlipay!="1") { this.ErrorMessage("已关闭支付宝提现功能,请联系客服!"); Response.End(); }
            else if (FokeMode == "微信提现" && isWeChat != "1") { this.ErrorMessage("已关闭微信提现功能,请联系客服!"); Response.End(); }
            /************************************************************************************************
             * 验证支付宝提现方式的合法性
             * **********************************************************************************************/
            if (FokeMode == "支付宝提现" && string.IsNullOrEmpty(MemberRs["AlipayChar"].ToString())) { this.ErrorMessage("获取支付宝账号信息失败!"); Response.End(); }
            else if (FokeMode == "支付宝提现" && MemberRs["AlipayChar"].ToString().Length <= 0) { this.ErrorMessage("获取支付宝账号信息失败!"); Response.End(); }
            else if (FokeMode == "支付宝提现" && MemberRs["AlipayChar"].ToString().Length <= 5) { this.ErrorMessage("支付宝账号错误,不能少于6个字符!"); Response.End(); }
            else if (FokeMode == "支付宝提现" && MemberRs["AlipayChar"].ToString().Length >= 30) { this.ErrorMessage("支付宝账号错误,不能超过30个字符!"); Response.End(); }
            if (FokeMode == "支付宝提现" && string.IsNullOrEmpty(MemberRs["Alipayname"].ToString())) { this.ErrorMessage("获取支付宝昵称信息失败!"); Response.End(); }
            else if (FokeMode == "支付宝提现" && MemberRs["Alipayname"].ToString().Length <= 0) { this.ErrorMessage("获取支付宝昵称信息失败!"); Response.End(); }
            else if (FokeMode == "支付宝提现" && MemberRs["Alipayname"].ToString().Length >= 24) { this.ErrorMessage("支付宝昵称错误,不能超过24个字符!"); Response.End(); }
            /************************************************************************************************
            * 微信提现,需要绑定实名认证已经绑定微信账号
            * **********************************************************************************************/
            if (WeIsOnLine == "1" && FokeMode == "微信提现" && string.IsNullOrEmpty(MemberRs["AuthorizationType"].ToString())) { this.ErrorMessage("获取绑定微信授权信息失败!"); Response.End(); }
            else if (WeIsOnLine == "1" && FokeMode == "微信提现" && MemberRs["AuthorizationType"].ToString().Length <= 0) { this.ErrorMessage("获取绑定微信授权信息失败!"); Response.End(); }
            else if (WeIsOnLine == "1" && FokeMode == "微信提现" && MemberRs["AuthorizationType"].ToString() == "Define") { this.ErrorMessage("获取绑定微信授权信息失败!"); Response.End(); }
            if (WeIsOnLine == "1" && FokeMode == "微信提现" && string.IsNullOrEmpty(MemberRs["AuthorizationKey"].ToString())) { this.ErrorMessage("获取绑定微信授权信息失败!"); Response.End(); }
            else if (WeIsOnLine == "1" && FokeMode == "微信提现" && MemberRs["AuthorizationKey"].ToString().Length <= 0) { this.ErrorMessage("获取绑定微信授权信息失败!"); Response.End(); }
            else if (WeIsOnLine == "1" && FokeMode == "微信提现" && MemberRs["AuthorizationKey"].ToString().Length <= 20) { this.ErrorMessage("获取绑定微信授权信息失败,微信授权不能少于20个字符!"); Response.End(); }
            else if (WeIsOnLine == "1" && FokeMode == "微信提现" && MemberRs["AuthorizationKey"].ToString().Length >= 40) { this.ErrorMessage("获取绑定微信授权信息失败,微信授权不能少于40个字符!"); Response.End(); }

            if (WeIsOnLine == "0" && FokeMode == "微信提现" && string.IsNullOrEmpty(MemberRs["Fullname"].ToString())) { this.ErrorMessage("获取绑定实名认证信息失败!"); Response.End(); }
            else if (WeIsOnLine == "0" && FokeMode == "微信提现" && MemberRs["Fullname"].ToString().Length <= 0) { this.ErrorMessage("获取绑定实名认证信息失败!"); Response.End(); }
            else if (WeIsOnLine == "0" && FokeMode == "微信提现" && MemberRs["Fullname"].ToString().Length <= 1) { this.ErrorMessage("获取绑定实名认证信息失败,真实姓名不能少于1个汉字!"); Response.End(); }
            else if (WeIsOnLine == "0" && FokeMode == "微信提现" && MemberRs["Fullname"].ToString().Length >= 12) { this.ErrorMessage("获取绑定实名认证信息失败,真实姓名不能超过12个汉字!"); Response.End(); }
            if (WeIsOnLine == "0" && FokeMode == "微信提现" && string.IsNullOrEmpty(MemberRs["strWechat"].ToString())) { this.ErrorMessage("获取绑定账号认证信息失败,账号不能为空!"); Response.End(); }
            else if (WeIsOnLine == "0" && FokeMode == "微信提现" && MemberRs["strWechat"].ToString().Length <= 0) { this.ErrorMessage("获取绑定账号认证信息失败,账号不能少于1个字!"); Response.End(); }
            else if (WeIsOnLine == "0" && FokeMode == "微信提现" && MemberRs["strWechat"].ToString().Length > 11) { this.ErrorMessage("获取绑定账号认证信息失败,账号不能超过12个字!"); Response.End(); }
             /************************************************************************************************
             * 验证提现金额信息
             * **********************************************************************************************/
            double thisAmount = RequestHelper.GetRequest("Amount").cDouble();
            if (thisAmount <= 0) { this.ErrorMessage("获取兑换金额信息失败,请重试！"); Response.End(); }
            double MinAmount = this.GetParameter("MinAmount", "AlipayXMl").cDouble();
            double MaxAmount = this.GetParameter("MaxAmount", "AlipayXMl").cDouble();
            if (thisAmount < MinAmount) { this.ErrorMessage("最少" + MinAmount + "元起提！"); Response.End(); }
            else if (thisAmount > MaxAmount) { this.ErrorMessage("每次最多只能提现" + MaxAmount + "元！"); Response.End(); }
            if (new Fooke.Function.String(MemberRs["Amount"].ToString()).cDouble() < thisAmount)
            { this.ErrorMessage("账户余额不足,请减少提现金额!"); Response.End(); }
            /*****************************************************************************************************
             * 判断二级密码是否填写正确
             * ***************************************************************************************************/
            string PasswordTo = RequestHelper.GetRequest("PasswordTo").toString("DuomiCMS");
            if (string.IsNullOrEmpty(PasswordTo)) { this.ErrorMessage("请填写您的交易密码！"); Response.End(); }
            else if (PasswordTo.Length <= 5) { this.ErrorMessage("交易密码长度不能少于6个字符！"); Response.End(); }
            else if (PasswordTo.Length > 16) { this.ErrorMessage("交易密码长度不能大于16个字符！"); Response.End(); }
            //else if (MemberHelper.toPassword(PasswordTo) != MemberRs["PasswordTo"].ToString()) { this.ErrorMessage("交易密码填写错误！"); Response.End(); }
            /************************************************************************************************
             * 验证描述备注字段信息
             * **********************************************************************************************/
            string Remark = RequestHelper.GetRequest("Remark").ToString();
            if (Remark.Length >= 60) { this.ErrorMessage("描述备注字段长度请限制在60个汉字内！"); Response.End(); }
            /************************************************************************************************
             * 验证用户提现权限信息,处理提现请求
             * **********************************************************************************************/
            DataRow iRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAlipayToday]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"DateKey",DateTime.Now.ToString("yyyyMMdd").ToString()}
            });
            if (iRs == null) { this.ErrorMessage("获取系统验证信息失败,请重试！"); Response.End(); }
            /************************************************************************************************
             * 验证重复提现
             * **********************************************************************************************/
            string iRepeat = this.GetParameter("iRepeat", "AlipayXml").toInt();
            if (iRepeat != "1" && iRs["Under"].ToString() != "0") { this.ErrorMessage("您有提现订单未完成,请等待订单完成再提现！"); Response.End(); }
            int Alipaytimer = this.GetParameter("Alipaytimer", "alipayXml").cInt();
            if (Alipaytimer != 0 && Alipaytimer <= new Fooke.Function.String(iRs["Alipaytimer"].ToString()).cInt())
            { this.ErrorMessage("提现次数已超过限制,每日最多允许提现" + Alipaytimer + "次"); Response.End(); }
            double AlipayTotal = this.GetParameter("AlipayTotal", "AlipayXml").cDouble();
            if (AlipayTotal != 0 && AlipayTotal <= new Fooke.Function.String(iRs["AlipayAmount"].ToString()).cDouble())
            { this.ErrorMessage("提现金额已经超过了限制,每日最多允许提现" + AlipayTotal + "元!"); Response.End(); }
            else if (AlipayTotal != 0 && (AlipayTotal - thisAmount) <= new Fooke.Function.String(iRs["AlipayAmount"].ToString()).cDouble())
            { this.ErrorMessage("提现金额已经超过了限制,每日最多允许提现" + AlipayTotal + "元!"); Response.End(); }
            /************************************************************************************************
             * 生成提现订单号
             * **********************************************************************************************/
            string OrderKey = string.Format("用户提现-|-|-{0}-|-|-{1}-|-|-用户提现", MemberRs["UserID"].ToString(), DateTime.Now.Ticks.ToString());
            OrderKey = new Fooke.Function.String(OrderKey).ToMD5().Substring(0, 20).ToUpper();
            DataRow vRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAlipay]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"OrderKey",OrderKey}
            });
            if (vRs != null) { this.ErrorMessage("提现订单号已经存在,请重试！"); Response.End(); }
            /************************************************************************************************
             * 开始保存订单数据
             * **********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["OrderKey"] = OrderKey;
            thisDictionary["FokeMode"] = FokeMode;
            thisDictionary["BizKey"] = OrderKey;
            thisDictionary["DateKey"] = DateTime.Now.ToString("yyyyMMdd");
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["thisAmount"] = thisAmount.ToString("0.00");
            if (FokeMode == "支付宝提现")
            {
                thisDictionary["AccessMode"] = "支付宝提现";
                if (AlIsOnLine == "1") { thisDictionary["AccessName"] = MemberRs["AlipayChar"].ToString(); }
                else { thisDictionary["AccessName"] = "姓名：" + MemberRs["Alipayname"].ToString() + " 账号：" + MemberRs["AlipayChar"].ToString(); }
                thisDictionary["AccessHolder"] = MemberRs["Alipayname"].ToString();
            }
            else if (FokeMode == "微信提现")
            {
                thisDictionary["AccessMode"] = "微信提现";
                if (WeIsOnLine == "1") { thisDictionary["AccessName"] = MemberRs["AuthorizationKey"].ToString(); }
                else { thisDictionary["AccessName"] ="姓名：" + MemberRs["Fullname"].ToString() + " 账号：" + MemberRs["strWechat"].ToString(); }
                thisDictionary["AccessHolder"] = MemberRs["Fullname"].ToString();
            }
            thisDictionary["strRemark"] = Remark;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAlipay]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /************************************************************************************************
             * 验证自动到账功能
             * **********************************************************************************************/
            if (AlIsOnLine == "1" && FokeMode == "支付宝提现")
            {
                string AliDisplay = this.GetParameter("AliDisplay", "AlipayXml").toInt();
                double AliAmount = this.GetParameter("AliAmount", "AlipayXml").cDouble();
                if (AliDisplay == "0") { new AliBusinessHelper().SaveTransfer(thisRs, this.Configure); }
                else if (AliDisplay == "2" && thisAmount <= AliAmount) { new AliBusinessHelper().SaveTransfer(thisRs, this.Configure); }
            }
            else if (WeIsOnLine == "1" && FokeMode == "微信提现")
            {
                string WeDisplay = this.GetParameter("WeDisplay", "AlipayXml").toInt();
                double WeAmount = this.GetParameter("WeAmount", "AlipayXml").cDouble();
                string WeChatBusinessID = this.GetParameter("WeChatBusinessID", "AlipayXml").toString();
                string WeChatAppId = this.GetParameter("WeChatAppId", "AlipayXml").toString();
                string WeChatBusinessKey = this.GetParameter("WeChatBusinessKey", "AlipayXml").toString();
                if (WeDisplay == "0") { new Fooke.Code.WCBusinessHelper().SaveBusiness(cRs: thisRs, BusinessID: WeChatBusinessID, BusinessKey: WeChatBusinessKey, AppID: WeChatAppId); }
                else if (WeDisplay == "2" && thisAmount <= WeAmount) { new Fooke.Code.WCBusinessHelper().SaveBusiness(cRs: thisRs, BusinessID: WeChatBusinessID, BusinessKey: WeChatBusinessKey, AppID: WeChatAppId); }
            }
            /************************************************************************************************
             * 输出数据处理结果
             * **********************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"type\":\"redirect\"");
            strBuilder.Append(",\"url\":\"Alipay.aspx\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 验证真实姓名信息
        /// </summary>
        protected void SaveFullname()
        {
            /**********************************************************************************************************
             * 获取并验证用户微信账号信息
             * *********************************************************************************************************/
            string strWechat = RequestHelper.GetRequest("strWechat").ToString();
            if (string.IsNullOrEmpty(strWechat)) { this.ErrorMessage("请填写微信账号信息！"); Response.End(); }
            else if (strWechat.Length <= 4) { this.ErrorMessage("微信账号长度不能少于4个字符！"); Response.End(); }
            else if (strWechat.Length >= 30) { this.ErrorMessage("微信账号长度不能超过30个字符！"); Response.End(); }
            /**********************************************************************************************************
             * 获取并验证用户姓名
             * *********************************************************************************************************/
            string Fullname = RequestHelper.GetRequest("Fullname").ToString();
            if (string.IsNullOrEmpty(Fullname)) { this.ErrorMessage("请填写您的真实姓名！"); Response.End(); }
            else if (Fullname.Length <= 1) { this.ErrorMessage("真实姓名长度不能少于2个汉字！"); Response.End(); }
            else if (Fullname.Length >= 12) { this.ErrorMessage("真实姓名长度不能大于12个汉字！"); Response.End(); }
            /**********************************************************************************************************
             * 开始保存数据
             * *********************************************************************************************************/
            DbHelper.Connection.Update("Fooke_User", dictionary: new Dictionary<string, string>()
            {
                {"strWechat",strWechat},
                {"Fullname",Fullname}
            }, Params: " and UserID=" + MemberRs["UserID"] + "");
            /************************************************************************************************
             * 输出数据处理结果
             * **********************************************************************************************/
            this.ErrorMessage("用户真实姓名绑定成功！", iSuccess: true);
            Response.End();
        }
    }
}