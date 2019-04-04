using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Text;
using System.Data;
using Fooke.Code;
using Fooke.Function;
namespace Fooke.Web.Pay
{
    /// <summary>
    /// 微信回调地址
    /// </summary>
    public partial class WeChat : Fooke.Code.BaseHelper
    {

        /// <summary>
        /// 获取请求数据，转换成string数据类型
        /// </summary>
        /// <returns></returns>
        public void Start(Action<string> Fun)
        {
            System.IO.Stream requestStream = System.Web.HttpContext.Current.Request.InputStream;
            byte[] requestByte = new byte[requestStream.Length];
            requestStream.Read(requestByte, 0, (int)requestStream.Length);
            string strXml = Encoding.UTF8.GetString(requestByte);
            if (Fun != null && !string.IsNullOrEmpty(strXml)) { Fun(strXml); Response.End(); }
            Response.End();
        }

        /// <summary>
        /// 输出支付失败结果
        /// </summary>
        /// <param name="Tips"></param>
        protected void ErrorMessage(string Tips)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<xml>");
            strBuilder.Append("<return_code><![CDATA[FAIL]]></return_code>");
            strBuilder.Append("<return_code><![CDATA[" + Tips + "]]></return_code>");
            strBuilder.Append("</xml>");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 开始加载网页数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            /**************************************************************************
             * 开始获取数据信息
             * ************************************************************************/
            this.Start(strXml =>
            {
                if (string.IsNullOrEmpty(strXml) && !strXml.Contains("<")) { this.ErrorMessage("发生未知错误，数据获取失败！"); Response.End(); }
                ConfigurationReader xReader = new ConfigurationReader(strXml);
                string return_code = xReader.GetParameter("return_code").toString();
                if (return_code != "SUCCESS") { this.ErrorMessage("发生未知错误，微信支付失败！"); Response.End(); }
                /*************************************************************************
                 * 检查并处理数据订单
                 * ***********************************************************************/
                string thisKey = xReader.GetParameter("out_trade_no").toString();
                if (string.IsNullOrEmpty(thisKey)) { this.ErrorMessage("拉取订单编号失败!"); Response.End(); }
                DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindRechargeableLog]", new Dictionary<string, object>() { 
                    {"strKey", thisKey } 
                });
                if (thisRs == null) {  this.ErrorMessage("拉取订单号信息失败,请刷新网页重试！"); Response.End(); }
                if (thisRs["isFinish"].ToString() != "0") { this.ErrorMessage("当前支付订单已被处理！"); Response.End(); }
                else if (thisRs["Model"].ToString() != "微信付款") { this.ErrorMessage("越权操作,本地订单甄别失败！"); Response.End(); }
                /*************************************************************************
                 * 查询微信订单是否存在
                 * ***********************************************************************/
                string Transaction_id = xReader.GetParameter("transaction_id").toString();
                if (string.IsNullOrEmpty(Transaction_id)) { this.ErrorMessage("微信支付中订单号不存在！"); Response.End(); }
                /*****************************************************************************************
                 * 查询微信交易订单,并更改本地交易记录
                 * ***************************************************************************************/
                this.QueryOrder(Transaction_id, thisRs, iFind =>
                {
                    new PaymentHelper().SaveUpdate(thisRs, iSuccess =>
                    {
                        if (!iSuccess) { this.ErrorMessage("本地订单处理过程中发生意外！"); Response.End(); }
                        
                        StringBuilder strBuilder = new StringBuilder();
                        strBuilder.Append("<xml>");
                        strBuilder.Append("<return_code><![CDATA[SUCCESS]]></return_code>");
                        strBuilder.Append("<return_code><![CDATA[OK]]></return_code>");
                        strBuilder.Append("</xml>");
                        Response.Write(strBuilder.ToString());
                        Response.End();
                    });
                });
            });
            Response.End();
        }
        /// <summary>
        /// 微信签名
        /// </summary>
        /// <param name="thisDictionary"></param>
        /// <param name="thisKey"></param>
        /// <returns></returns>
        public string SignatureText(SortedDictionary<string, string> thisDictionary, string thisKey)
        {
            string Buffer = string.Empty;
            foreach (KeyValuePair<string, string> pair in thisDictionary)
            {
                if (pair.Key != "sign" && !string.IsNullOrEmpty(pair.Value.ToString())) { Buffer += pair.Key + "=" + pair.Value + "&"; }
            }
            Buffer = Buffer.Trim('&');
            Buffer += "&key=" + thisKey;
            var md5 = System.Security.Cryptography.MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(Buffer));
            var strBuilder = new StringBuilder();
            foreach (byte b in bs) { strBuilder.Append(b.ToString("x2")); }
            return strBuilder.ToString().ToUpper();
        }
        /// <summary>
        /// 查询微信中订单是否存在
        /// </summary>
        /// <param name="transaction_id"></param>
        /// <returns></returns>
        private void QueryOrder(string Transaction_id, DataRow thisRs, Action<bool> Fun)
        {
            //WeiXinHelper thisHelper = new WeiXinHelper(thisRs["BusinessID"].ToString(), thisRs["BusinessKey"].ToString(), thisRs["other"].ToString());
            SortedDictionary<string, string> thisDictionary = new SortedDictionary<string, string>();
            thisDictionary["appid"] = thisRs["other"].ToString();
            thisDictionary["mch_id"] = thisRs["BusinessID"].ToString();
            thisDictionary["nonce_str"] = new Fooke.Function.String("订单查询-|-|-" + DateTime.Now.Ticks.ToString()).ToMD5().toString();
            thisDictionary["transaction_id"] = Transaction_id;
            thisDictionary["sign"] = this.SignatureText(thisDictionary, thisRs["BusinessKey"].ToString());
            string strXml = DictionaryToXML(thisDictionary);
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    byte[] Buffer = thisClient.UploadData("https://api.mch.weixin.qq.com/pay/orderquery", "POST", Encoding.UTF8.GetBytes(strXml.ToString()));
                    strXml = Encoding.UTF8.GetString(Buffer);
                }
                finally { thisClient.Dispose(); }
            };
            if (strXml.Contains("<return_code><![CDATA[SUCCESS]]></return_code>")
                && strXml.Contains("<return_msg><![CDATA[OK]]></return_msg>")
                && Fun != null) { Fun(true); Response.End(); }
            Response.End();
        }

        /// <summary>
        /// 将字典转换为XML格式数据
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public string DictionaryToXML(SortedDictionary<string, string> dictionary)
        {
            StringBuilder strXml = new StringBuilder();
            strXml.Append("<xml>");
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                if (pair.Value == null || string.IsNullOrEmpty(pair.Value)) { continue; }
                strXml.Append("<" + pair.Key + ">");
                if (isInt(pair.Value)) { strXml.Append(pair.Value); }
                else { strXml.Append("<![CDATA[" + pair.Value + "]]>"); }
                strXml.Append("</" + pair.Key + ">");
            }
            strXml.Append("</xml>");
            return strXml.ToString();
        }
        /// <summary>
        /// 判断是否为数字类型
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public bool isInt(string strValue)
        {
            try { double cInt = Convert.ToDouble(strValue); return true; }
            catch { return false; }
        }
    }
}