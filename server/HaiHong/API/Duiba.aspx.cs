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
    public partial class Duiba : System.Web.UI.Page
    {
        public string GoldName = string.Empty;
        public int AlipayPoint = 0;
        public bool isUnit = true;
        public DataRow MemberRs = null;
        public ConfigureCenter Configure = null;
        private static readonly string AlipayXML = "DuibaXml";
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
            string appKey = RequestHelper.GetRequest("appKey").toString();
            string thisKey = this.GetParameter("appKey", AlipayXML).toString();
            if (appKey != thisKey) { Response.Write("false"); Response.End(); }
            string timestamp = RequestHelper.GetRequest("timestamp").toString();
            if (string.IsNullOrEmpty(timestamp)) { Response.Write("false"); Response.End(); }
            string bizId = RequestHelper.GetRequest("bizId").toString();
            if (string.IsNullOrEmpty(bizId)) { Response.Write("false"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Duiba, Params: " and Biz = '" + bizId + "'");
            if (cRs == null) { Response.Write("false"); Response.End(); }
            if (cRs["Affairs"].ToString() != "99" && cRs["Affairs"].ToString() != "0") { Response.Write("ok"); Response.End(); }
            string success = RequestHelper.GetRequest("success").toString();
            if (success == "true" && (cRs["affairs"].ToString() == "99" || cRs["affairs"].ToString() == "0"))
            {
                Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
                thisDictionary["Id"] = cRs["Id"].ToString();
                thisDictionary["Affairs"] = "1";
                thisDictionary["thisKey"] = "T" + cRs["thisKey"].ToString().Remove(0, 1);
                thisDictionary["UserID"] = cRs["UserID"].ToString();
                thisDictionary["Nickname"] = cRs["Nickname"].ToString();
                thisDictionary["Amount"] = cRs["Amount"].ToString();
                thisDictionary["Remark"] = string.Format("兑换失败返回兑换积分{0}", cRs["Amount"].ToString());
                DbHelper.Connection.ExecuteProc("Stored_SaveDuibaAffairs", thisDictionary);
                Response.Write("ok"); Response.End();
            }
            else if (success == "false" && (cRs["affairs"].ToString() == "99" || cRs["affairs"].ToString() == "0"))
            {
                Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
                thisDictionary["Id"] = cRs["Id"].ToString();
                thisDictionary["Affairs"] = "100";
                thisDictionary["thisKey"] = "T" + cRs["thisKey"].ToString().Remove(0, 1);
                thisDictionary["UserID"] = cRs["UserID"].ToString();
                thisDictionary["Nickname"] = cRs["Nickname"].ToString();
                thisDictionary["Amount"] = cRs["Amount"].ToString();
                thisDictionary["Remark"] = string.Format("兑换失败返回兑换积分{0}", cRs["Amount"].ToString());
                DbHelper.Connection.ExecuteProc("Stored_SaveDuibaAffairs", thisDictionary);
                Response.Write("ok"); Response.End();
            }
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
            if (Biz.Length > 60) { this.ErrorMessage("订单号长度请限制在60个字符以内！"); Response.End(); }
            string description = RequestHelper.GetRequest("description").toString();
            if (string.IsNullOrEmpty(description)) { this.ErrorMessage("请求参数错误，描述信息不能为空！"); Response.End(); }
            if (description.Length > 300) { this.ErrorMessage("描述信息内容长度请限制在300个汉字以内！"); Response.End(); }
            string trandKey = RequestHelper.GetRequest("orderNum").toString();
            if (string.IsNullOrEmpty(trandKey)) { this.ErrorMessage("参数错误，订单号码为空！"); Response.End(); }
            if (trandKey.Length > 60) { this.ErrorMessage("订单号长度请限制在60个字符以内！"); Response.End(); }
            string type = RequestHelper.GetRequest("type").toString();
            if (string.IsNullOrEmpty(type)) { this.ErrorMessage("参数错误，兑换类型不能为空！"); Response.End(); }
            if (type.Length > 50) { this.ErrorMessage("兑换类型长度请限制在60个汉字以内！"); Response.End(); }
            /**********************************************************************************
             * 检查用户兑换金额是否超过上限
             * ********************************************************************************/
            double Amount = RequestHelper.GetRequest("credits").cDouble();
            if (Amount <= 0) { this.ErrorMessage("请求参数错误，兑换" + this.GoldName + "为0"); Response.End(); }
            try
            {
                double MinNumber = this.GetParameter("MinNumber", AlipayXML).cDouble();
                double MaxNumber = this.GetParameter("MaxNumber", AlipayXML).cDouble();
                if (Amount < MinNumber) { this.ErrorMessage("最少" + MinNumber + "元起兑！"); Response.End(); }
                if (Amount > MaxNumber) { this.ErrorMessage("每次最多只能兑换" + MaxNumber + "元！"); Response.End(); }
            }
            catch { }
            /**********************************************************************************
             * 检查用户当日兑换次数是否超过上限
             * ********************************************************************************/
            int AlipayTimes = this.GetParameter("timer", AlipayXML).cInt();
            if (AlipayTimes > 0)
            {
                DataRow vRs = DbHelper.Connection.ExecuteFindRow("Stored_FindDuibaFrequency", new Dictionary<string, object>() {
                        {"UserID",MemberRs["UserID"].ToString()},{"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")}
                    });
                if (vRs == null) { this.ErrorMessage("拉取兑换记录信息失败,请重试"); Response.End(); }
                int vNumber = new Fooke.Function.String(vRs["total"].ToString()).cInt();
                if (AlipayTimes <= vNumber) { this.ErrorMessage("兑换次数已超过上限，每日最多允许兑换" + AlipayTimes + "次"); Response.End(); }
            }
            /**************************************************************************************
             * 生成交易订单号
             * ************************************************************************************/
            string thisKey = "Duiba-|-|-" + MemberRs["UserID"].ToString() + "-|-|-" + trandKey;
            thisKey = new Fooke.Function.String(thisKey).ToMD5().Substring(0, 20).ToUpper();
            /**************************************************************************************
             * 开始保存兑换数据
             * *************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["UserID"] = MemberRs["UserID"].ToString();
            oDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            oDictionary["thisKey"] = thisKey;
            oDictionary["Biz"] = Biz;
            oDictionary["Remark"] = description;
            oDictionary["Amount"] = Amount;
            oDictionary["Remark2"] = string.Format("兑吧商城兑换使用{0}积分", Amount);
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveDuiba", oDictionary);
            if (thisRs == null) { this.ErrorMessage("保存兑换信息失败,请重试！"); Response.End(); }
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
            strBuilder.Append("'bizId': '" + thisKey + "',");
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
                    {"uid",MemberRs["UserId"].ToString()},
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