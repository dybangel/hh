using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using Fooke.Function;
using Fooke.Code;
using System.Security.Cryptography;
namespace Fooke.Web.API
{
    public partial class Dollar : System.Web.UI.Page
    {
        public string GoldName = string.Empty;
        public int AlipayPoint = 0;
        public bool isUnit = true;
        public DataRow MemberRs = null;
        public ConfigureCenter Configure = null;
        private static readonly string AlipayXML = "AlipayXml";
        /// <summary>
        /// 页面请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            /*****************************************************************************************
             * 获取并验证用户账号数据信息
             * ***************************************************************************************/
            string strTokey = RequestHelper.GetRequest("uid").toString();
            if (string.IsNullOrEmpty(strTokey)) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (strTokey.Length<=20) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (strTokey.Length >= 32) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() { 
                {"strTokey",strTokey}
            });
            if (MemberRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (MemberRs["isDisplay"].ToString() != "1") { this.ErrorMessage("账号已停止使用,请联系客服！"); Response.End(); }
            if (this.Configure == null) { this.Configure = new ConfigureCenter(); }
            /***************************************************************************************
             * 判断网页请求
             * **************************************************************************************/
            string strRequest = RequestHelper.GetRequest("action").toString();
            if (string.IsNullOrEmpty(strRequest)) { strRequest = "default"; }
            switch (strRequest.ToLower())
            {
                case "request": this.SaveRequest(); Response.End(); break;
                case "deal": this.SaveResult(); Response.End(); break;
                default: this.strDefault(); Response.End(); break;
            }
            Response.End();
        }
        /// <summary>
        /// 兑换成功或者兑换失败的结果处理
        /// </summary>
        protected void SaveResult()
        {
            /**************************************************************************************************************************
             * 获取并验证兑吧数据信息
             * ************************************************************************************************************************/
            string appKey = RequestHelper.GetRequest("appKey").toString();
            if (string.IsNullOrEmpty(appKey)) { this.ErrorMessage("获取请求参数失败,请重试！"); Response.End(); }
            else if (appKey.Length <= 0) { this.ErrorMessage("获取请求参数失败,请重试！"); Response.End(); }
            else if (appKey.Length >= 20) { this.ErrorMessage("获取请求参数失败,请重试！"); Response.End(); }
            string strKey = this.GetParameter("appKey", "AlipayXml").toString();
            if (strKey != appKey) { this.ErrorMessage("请求参数错误，数据验证失败！"); Response.End(); }
            /**************************************************************************************************************************
             * 验证处理时间戳数据信息
             * ************************************************************************************************************************/
            string timestamp = RequestHelper.GetRequest("timestamp").toString();
            if (string.IsNullOrEmpty(timestamp)) { ErrorMessage("false"); Response.End(); }
            /**************************************************************************************************************************
             * 获取订单备注字段数据信息
             * ************************************************************************************************************************/
            string BizKey = RequestHelper.GetRequest("bizId").toString();
            if (string.IsNullOrEmpty(BizKey)) { ErrorMessage("false"); Response.End(); }
            else if (BizKey.Length <= 0) { ErrorMessage("false"); Response.End(); }
            else if (BizKey.Length <= 20) { ErrorMessage("false"); Response.End(); }
            else if (BizKey.Length >= 46) { ErrorMessage("false"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAlipay]", new Dictionary<string, object>() {
                {"BizKey",BizKey}
            });
            if (cRs == null) { Response.Write("false"); Response.End(); }
            else if (cRs["Affairs"].ToString() == "1") { Response.Write("ok"); Response.End(); }
            else if (cRs["Affairs"].ToString() == "100") { Response.Write("ok"); Response.End(); }
            /**************************************************************************************************************************
             * 获取并验证兑吧数据信息
             * ************************************************************************************************************************/
            if (RequestHelper.GetRequest("success").toString() == "false")
            {
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAlipayDisaccord]", new Dictionary<string, object>() {
                    {"AlipayID",cRs["AlipayID"].ToString()},
                    {"UserID",cRs["UserID"].ToString()},
                    {"Nickname",cRs["Nickname"].ToString()},
                    {"thisAmount",cRs["thisAmount"].ToString()},
                    {"OrderKey",cRs["OrderKey"].ToString()}
                });
                if (sRs == null) { this.ErrorMessage("数据处理过程中发生错误,请重试！"); Response.End(); }
                else if (sRs["Affairs"].ToString() != "100") { this.ErrorMessage("数据处理过程中发生错误,请重试！"); Response.End(); }
                /**************************************************************************************************************************
                * 输出数据处理结果
                * ************************************************************************************************************************/
                Response.Write("ok");
                Response.End();
            }
            /**************************************************************************************************************************
             * 兑换成功处理信息
             * ************************************************************************************************************************/
            DbHelper.Connection.ExecuteProc("[Stored_SaveAlipayConfirm]", new Dictionary<string, object>() {
                {"AlipayID",cRs["AlipayID"].ToString()},
                {"UserID",cRs["UserID"].ToString()},
            });
            Response.Write("ok");
            Response.End();
        }

        /// <summary>
        /// 兑换请求
        /// </summary>
        protected void SaveRequest()
        {
            string appKey = RequestHelper.GetRequest("appKey").toString();
            string strKey = this.GetParameter("appKey", "AlipayXml").toString();
            if (strKey != appKey) { this.ErrorMessage("请求参数错误，数据验证失败！"); Response.End(); }
            /*************************************************************************************************
             * 验证兑吧请求参数是否合法
             * ***********************************************************************************************/
            string Biz = RequestHelper.GetRequest("orderNum").toString();
            if (string.IsNullOrEmpty(Biz)) { this.ErrorMessage("请求参数错误，订单号为空！"); Response.End(); }
            else if (Biz.Length <= 15) { this.ErrorMessage("订单号长度不能少于15个字符！"); Response.End(); }
            else if (Biz.Length >= 46) { this.ErrorMessage("订单号长度请限制在46个字符以内！"); Response.End(); }
            string description = RequestHelper.GetRequest("description").toString();
            if (string.IsNullOrEmpty(description)) { this.ErrorMessage("请求参数错误，描述信息不能为空！"); Response.End(); }
            else if (description.Length > 300) { this.ErrorMessage("描述信息内容长度请限制在300个汉字以内！"); Response.End(); }
            /************************************************************************************************
            * 获取兑吧兑换类型信息
            * **********************************************************************************************/
            string type = RequestHelper.GetRequest("type").toString();
            if (string.IsNullOrEmpty(type)) { this.ErrorMessage("参数错误，兑换类型不能为空！"); Response.End(); }
            if (type.Length > 50) { this.ErrorMessage("兑换类型长度请限制在60个汉字以内！"); Response.End(); }
            /************************************************************************************************
             * 验证提现金额信息
             * **********************************************************************************************/
            double thisAmount = RequestHelper.GetRequest("credits").cDouble();
            if (thisAmount <= 0) { this.ErrorMessage("获取兑换金额信息失败,请重试！"); Response.End(); }
            double MinAmount = this.GetParameter("MinAmount", "AlipayXMl").cDouble();
            double MaxAmount = this.GetParameter("MaxAmount", "AlipayXMl").cDouble();
            if (thisAmount < MinAmount) { this.ErrorMessage("最少" + MinAmount + "元起提！"); Response.End(); }
            else if (thisAmount > MaxAmount) { this.ErrorMessage("每次最多只能提现" + MaxAmount + "元！"); Response.End(); }
            if (new Fooke.Function.String(MemberRs["Amount"].ToString()).cDouble() < thisAmount)
            { this.ErrorMessage("账户余额不足,请减少提现金额!"); Response.End(); }
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
            thisDictionary["FokeMode"] = "兑吧兑换";
            thisDictionary["BizKey"] = OrderKey;
            thisDictionary["DateKey"] = DateTime.Now.ToString("yyyyMMdd");
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["thisAmount"] = thisAmount.ToString("0.00");
            thisDictionary["AccessMode"] = "兑吧兑换";
            thisDictionary["AccessName"] = type;
            thisDictionary["AccessHolder"] = description;
            thisDictionary["strRemark"] = "";
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAlipay]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /***********************************************************************************
             * 输出兑换成功结果
             * *********************************************************************************/
            string toNumber = new Fooke.Function.String(thisRs["Amount"].ToString()).cDouble().ToString("0.00");
            double oPoint = new Fooke.Function.String(toNumber).cDouble();
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("'status': 'ok',");
            strBuilder.Append("'message': '操作成功',");
            strBuilder.Append("'errorMessage': '',");
            strBuilder.Append("'bizId': '" + OrderKey + "',");
            strBuilder.Append("'credits': '" + oPoint.ToString("0") + "'");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 积分墙数据
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************************************************
             * 判断系统是否有开启兑吧
             * ************************************************************************************************************************/
            string isDollar = this.GetParameter("isDollar", "AlipayXml").toInt();
            if (isDollar != "1") { this.ErrorMessage("已关闭兑吧兑换功能！"); Response.End(); }
            /**************************************************************************************************************************
             * 获取兑吧配置参数信息
             * ************************************************************************************************************************/
            string AppKey = this.GetParameter("DollarAppKey", "AlipayXml").toString();
            if (string.IsNullOrEmpty(AppKey)) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            else if (AppKey.Length <= 16) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            else if (AppKey.Length >= 40) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            string AppSecret = this.GetParameter("DollarAppSecret", "AlipayXml").toString();
            if (string.IsNullOrEmpty(AppSecret)) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            else if (AppSecret.Length <= 16) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            else if (AppSecret.Length >= 40) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            /**************************************************************************************************************************
             * 开始跳转到指定的网页
             * ************************************************************************************************************************/
            string toRedirect = DuibaHelper.BuildUrlWithSign(url: "http://www.duiba.com.cn/autoLogin/autologin",
                hshTable: new Hashtable(){
                    {"uid",MemberRs["strTokey"].ToString()},
                    {"credits",new Fooke.Function.String(MemberRs["Amount"].ToString()).cDouble().ToString("0")}
                }, appKey: AppKey, appSecret: AppSecret);
            /**************************************************************************************************************************
            * 开始跳转到指定的网页
            * ************************************************************************************************************************/
            Response.Redirect(toRedirect);
            Response.End();
        }
        /// <summary>
        /// 输出失败错误信息
        /// </summary>
        /// <param name="strTips"></param>
        /// <param name="number"></param>
        protected void ErrorMessage(string strTips)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("'status': 'fail',");
            strBuilder.Append("'message': '操作失败',");
            strBuilder.Append("'errorMessage': '" + strTips + "',");
            if (MemberRs != null) { strBuilder.Append("'credits': '" + MemberRs["Amount"].ToString() + "'"); }
            else { strBuilder.Append("'credits': '0'"); }
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 拉取配置信息
        /// </summary>
        /// <param name="xName"></param>
        /// <param name="configName"></param>
        /// <returns></returns>
        public Fooke.Function.String GetParameter(string strName, string configName = "siteXml")
        {
            try
            {
                if (this.Configure == null) { this.Configure = new ConfigureCenter(); }
                Function.String strValue = this.Configure.GetParameter(strName, configName);
                return strValue;
            }
            catch { return new Fooke.Function.String(string.Empty); }
        }

        /// <summary>
        /// 格式化用户资金，转换成人民币或者是积分
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        public double FormatAmount(double Point)
        {
            try
            {
                double AlipayPoint = this.GetParameter("AlipayPoint", "AlipayXML").cDouble();
                if (AlipayPoint <= 0) { AlipayPoint = 100; }
                int isUnit = this.GetParameter("isUnit", "AlipayXML").cInt();
                if (isUnit == 1) { return Point; }
                else
                {
                    double oNumber = Point;
                    try { oNumber = (Point / AlipayPoint); }
                    catch { }
                    return oNumber;
                }
            }
            catch { return Point; }
        }

        public double FormatAmount(object Point)
        {
            try
            {
                double AlipayPoint = this.GetParameter("AlipayPoint", "AlipayXML").cDouble();
                if (AlipayPoint <= 0) { AlipayPoint = 100; }
                int isUnit = this.GetParameter("isUnit", "AlipayXML").cInt();
                if (isUnit == 1) { return new Fooke.Function.String(Point.ToString()).cDouble(); }
                else
                {
                    double oNumber = new Fooke.Function.String(Point.ToString()).cDouble();
                    try { oNumber = (oNumber / AlipayPoint); }
                    catch { }
                    return oNumber;
                }
            }
            catch { return new Fooke.Function.String(Point.ToString()).cDouble(); }
        }

        /// <summary>
        /// 将用户积分明细转换成资金说明
        /// </summary>
        /// <param name="tips"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public string FormatTips(string tips, double number)
        {
            try
            {
                tips = string.Format(tips, number.ToString("0.00"));
                string iUnitName = this.GetParameter("xName", "AlipayXML").toString();
                if (string.IsNullOrEmpty(iUnitName)) { iUnitName = "积分"; }
                tips = tips + iUnitName;
            }
            catch { }
            return tips;
        }

    }
}