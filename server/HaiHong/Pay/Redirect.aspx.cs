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
    public partial class Redirect : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /************************************************************************
             * 输出网页内容
             * **********************************************************************/
            switch (strRequest)
            {
                case "qrcode": QrCode(); Response.End(); break;
                case "native": strNative(); Response.End(); break;
                case "wnative": strWNative(); Response.End(); break;
                case "jsapi": strJSAPI(); Response.End(); break;
                case "err": errMessage(); Response.End(); break;
                case "chk": ChkRechargeableLog(); Response.End(); break;
                case "success": ShowSuccess(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }

        protected void ShowSuccess()
        {
            /***********************************************************************
             * 获取请求参数信息
             * *********************************************************************/
            string RechID = RequestHelper.GetRequest("RechID").toString();
            if (RechID == "0") { strMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { strMessage("拉取充值信息失败,请返回重试！"); Response.End(); }
            /*********************************************************************************
             * 输出网页内容
             * ******************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader("template/success.html");
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            Response.Write(strReader);
            Response.End();
        }

        

        protected void errMessage()
        {
            /*********************************************************************************
             * 获取数据处理内容
             * ******************************************************************************/
            string respMessage = RequestHelper.GetRequest("respMessage").ToString();
            if (string.IsNullOrEmpty(respMessage)) { respMessage = "数据处理过程中发生错误,请重试！"; }
            /*********************************************************************************
             * 输出网页内容
             * ******************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader("template/message.html");
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "tips": strValue = respMessage; break;
                }
                return strValue;
            }));
            Response.Write(strReader);
            Response.End();
        }

        /// <summary>
        /// 检查支付是否到账
        /// </summary>
        protected void ChkRechargeableLog()
        {
            /***********************************************************************
             * 获取请求参数信息
             * *********************************************************************/
            string RechID = RequestHelper.GetRequest("RechID").toString();
            if (RechID == "0") { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { Response.Write("拉取充值信息失败,请返回重试！"); Response.End(); }
            else if (cRs["isFinish"].ToString() != "0") { Response.Write("支付未完成,可能是出错了,请联系客服!"); Response.End(); }
            /***********************************************************************
             * 充值已到账
             * *********************************************************************/
            Response.Write("success");
            Response.End();
        }
        /// <summary>
        /// 输出错误处理信息
        /// </summary>
        /// <param name="strTips"></param>
        public void strMessage(string respMessage)
        {
            /*********************************************************************************
             * 输出网页内容
             * ******************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader("template/message.html");
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "tips": strValue = respMessage; break;
                }
                return strValue;
            }));
            Response.Write(strReader);
            Response.End();
        }
        /// <summary>
        /// JSAPI支付
        /// </summary>
        protected void strJSAPI()
        {
            /***********************************************************************
             * 获取请求参数信息
             * *********************************************************************/
            string RechID = RequestHelper.GetRequest("RechID").toInt();
            if (RechID == "0") { this.strMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { this.strMessage("拉取充值信息失败,请返回重试！"); Response.End(); }
            else if (cRs["isFinish"].ToString() == "1") { this.strMessage("当前充值记录已完成,不需要重试充值!"); Response.End(); }
            else if (new Fooke.Function.String(cRs["addtime"].ToString()).cDate() <= DateTime.Now.AddHours(-2))
            { Response.Redirect("redirect.aspx?action=default&rechid=" + cRs["rechid"] + ""); Response.End(); }
            string BusinessID = cRs["Other"].ToString();
            string BusinessKey = cRs["BusinessKey"].ToString();
            string Other = cRs["Other"].ToString();
            /*********************************************************************************
            * 获取微信设置信息
            * ******************************************************************************/
            ConfigurationReader xReader = new ConfigurationReader(cRs["strXml"].ToString());
            if (xReader.GetParameter("return_code").toString() != "SUCCESS") { this.strMessage("调起统一支付信息错误！"); Response.End(); }
            if (xReader.GetParameter("result_code").toString() != "SUCCESS") { this.strMessage("调起统一支付信息错误！"); Response.End(); }
            string prepay_id = xReader.GetParameter("prepay_id").toString();
            string nonce_str = xReader.GetParameter("nonce_str").toString();
            /*********************************************************************************
            * 输出网页内容
            * ******************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader("template/jsapi.html");
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "parameter": strValue = new Fooke.Code.WXHelper(BusinessID, BusinessKey, Other).JSParamter(nonce_str, prepay_id); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strReader);
            Response.End();
        }
        /// <summary>
        /// 显示默认网页内容
        /// </summary>
        protected void strDefault()
        {
            /***********************************************************************
             * 获取请求参数信息
             * *********************************************************************/
            string RechID = RequestHelper.GetRequest("RechID").toString();
            if (RechID == "0") { this.strMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { this.strMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["isFinish"].ToString() == "1") { this.strMessage("当前充值记录已完成,不需要重试充值!"); Response.End(); }
            /***********************************************************************
             * 判断用户选择不同的充值方式跳转到不同的充值平台去
             * *********************************************************************/
            if (cRs["Model"].ToString() == "支付宝")
            {
                string strResponse = new PaymentHelper().Alipay(cRs);
                Response.Write(strResponse);
                Response.End();
            }
            else if (cRs["Model"].ToString() == "微信付款")
            {
                string strResponse = cRs["strXml"].ToString();
                bool isPost = false;
                if (string.IsNullOrEmpty(strResponse)) { isPost = true; }
                else if (!strResponse.Contains("xml")) { isPost = true; }
                else if (!strResponse.Contains("<return_msg><![CDATA[OK]]></return_msg>")) { isPost = true; }
                else if (new Fooke.Function.String(cRs["addtime"].ToString()).cDate() <= DateTime.Now.AddHours(-2)) { isPost = true; }
                if (isPost)
                {
                    strResponse = new PaymentHelper().WeiXin(cRs, tradeType: "NATIVE", sParams: new Dictionary<string, string>() {
                        {"attach","购买VIP"},
                        {"body","订单编号:"+cRs["strKey"]+""},
                        {"spbill_create_ip","59.52.66.196"}
                    });
                    DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveRechargeableXml]", new Dictionary<string, object>() {
                        {"RechID",cRs["RechID"].ToString()},
                        {"strXml",strResponse}
                    });
                    if (Rs == null) { this.ErrorMessage("支付请求处理失败,请重试！"); Response.End(); }
                }
                if (!strResponse.Contains("xml")) { strMessage("支付微信统一下单失败,请联系管理员！"); Response.End(); }
                if (!strResponse.Contains("<return_msg><![CDATA[OK]]></return_msg>"))
                { strMessage("支付微信统一下单失败,请联系客服！"); Response.End(); }
                /***********************************************************************
                 * 跳转到JSAPI当中去支付
                 * **********************************************************************/
                Response.Redirect("redirect.aspx?action=jsapi&RechID=" + cRs["RechID"] + "");
                Response.End();
            }
        }

        /// <summary>
        /// 微信扫码支付
        /// </summary>
        protected void strNative()
        {
            string thisKey = RequestHelper.GetRequest("thisKey").toString();
            if (string.IsNullOrEmpty(thisKey)) { this.ErrorMessage("请求参数错误,请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"strKey",thisKey}
            });
            if (cRs == null) { this.ErrorMessage("拉取充值信息失败,请返回重试！"); Response.End(); }
            if (cRs["isFinish"].ToString() == "1") { this.ErrorMessage("当前充值记录已完成,不需要重试充值!"); Response.End(); }
            if (cRs["Model"].ToString() != "微信付款") { this.ErrorMessage("当前支付方式不支持扫码！"); Response.End(); }
            /*********************************************************************************
             * 微信扫码付款数据信息验证
             * ******************************************************************************/
            ConfigurationReader xReader = new ConfigurationReader(cRs["strXml"].ToString());
            if (xReader.GetParameter("return_msg").toString() != "OK") { this.ErrorMessage("支付微信统一下单失败,请联系管理员！"); Response.End(); }
            if (xReader.GetParameter("result_code").toString() != "SUCCESS") { this.ErrorMessage("支付微信统一下单失败,请联系管理员！"); Response.End(); }
            string imageURL = xReader.GetParameter("code_url").toString();
            if (string.IsNullOrEmpty(imageURL)) { this.ErrorMessage("微信统一下单支付失败,请刷新网页重试!"); Response.End(); }
            /*********************************************************************************
             * 输出网页内容
             * ******************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader("template/native.html");
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "key": strValue = new Fooke.Function.String("-|-|-" + imageURL + "-|-|-").ToMD5().ToLower(); break;
                    case "char": strValue = new Fooke.Function.String(imageURL).ToEncryptionDes().toString(); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strReader);
            Response.End();
        }
        /// <summary>
        /// 微信扫码支付
        /// </summary>
        protected void strWNative()
        {
            string thisKey = RequestHelper.GetRequest("thisKey").toString();
            if (string.IsNullOrEmpty(thisKey)) { this.ErrorMessage("请求参数错误,请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"strKey",thisKey}
            });
            if (cRs == null) { this.ErrorMessage("拉取充值信息失败,请返回重试！"); Response.End(); }
            if (cRs["isFinish"].ToString() == "1") { this.ErrorMessage("当前充值记录已完成,不需要重试充值!"); Response.End(); }
            if (cRs["Model"].ToString() != "微信付款") { this.ErrorMessage("当前支付方式不支持扫码！"); Response.End(); }
            /*********************************************************************************
             * 微信扫码付款数据信息验证
             * ******************************************************************************/
            ConfigurationReader xReader = new ConfigurationReader(cRs["strXml"].ToString());
            if (xReader.GetParameter("return_msg").toString() != "OK") { this.ErrorMessage("支付微信统一下单失败,请联系管理员！"); Response.End(); }
            if (xReader.GetParameter("result_code").toString() != "SUCCESS") { this.ErrorMessage("支付微信统一下单失败,请联系管理员！"); Response.End(); }
            string imageURL = xReader.GetParameter("code_url").toString();
            if (string.IsNullOrEmpty(imageURL)) { this.ErrorMessage("微信统一下单支付失败,请刷新网页重试!"); Response.End(); }
            /*********************************************************************************
             * 输出网页内容
             * ******************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader("template/wnative.html");
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "key": strValue = new Fooke.Function.String("-|-|-" + imageURL + "-|-|-").ToMD5().ToLower(); break;
                    case "char": strValue = new Fooke.Function.String(imageURL).ToEncryptionDes().toString(); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strReader);
            Response.End();
        }
        /// <summary>
        /// 生成微信支付的二维码图片
        /// </summary>
        protected void QrCode()
        {
            /*******************************************************************************************
            * 获取并验证生成二维码的地址
            * *****************************************************************************************/
            string token = RequestHelper.GetRequest("token").ToEncryptionText().toString();
            if (token.Length<=0) { Response.Write("请求参数错误！"); Response.End(); }
            else if (token.Length >= 1000) { Response.Write("请求参数错误！"); Response.End(); }
            /*******************************************************************************************
            * 验证二维码地址的合法性
            * *****************************************************************************************/
            string key = RequestHelper.GetRequest("key").toString();
            if (string.IsNullOrEmpty(key)) { Response.Write("请求参数错误,请重试！"); Response.End(); }
            else if (key.Length != 32) { Response.Write("请求参数错误,请重试！"); Response.End(); }
            string FormatKey = string.Format("扫码支付-|-|-{0}-|-|-{1}-|-|-扫码支付",
                token, DateTime.Now.ToString("yyyyMMdd"));
            FormatKey = new Fooke.Function.String(FormatKey).ToMD5().ToLower();
            if (key!=FormatKey) { Response.Write("请求参数错误,可能数据已经被非法串改！"); Response.End(); }
            /*******************************************************************************************
            * 开始生成一张支付二维码的图片
            * *****************************************************************************************/
            BarcodeWriter writer = new BarcodeWriter();
            //writer.Renderer = new ZXing.Rendering.BitmapRenderer { Background = Color.Wheat, Foreground = Color.SkyBlue };
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");//编码问题        
            writer.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            const int codeSizeInPixels = 250;   //设置图片长宽
            writer.Options.Height = writer.Options.Width = codeSizeInPixels;
            writer.Options.Margin = 0;//设置边框
            ZXing.Common.BitMatrix bm = writer.Encode(token);
            Bitmap Bit = writer.Write(bm);
            Bit.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Gif);
            Bit.Dispose();
            Response.End();
        }
    }
}