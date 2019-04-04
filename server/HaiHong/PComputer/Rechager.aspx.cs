using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using ZXing;
using Fooke.Code;
using Fooke.Function;
using Fooke.SimpleMaster;
namespace Fooke.Web.Member
{
    public partial class Rechager : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "do": doRechager(); Response.End(); break;
                case "scavenging": Scavenging(); Response.End(); break;
                case "qrcode": QrCode(); Response.End(); break;
                case "finish": VerificationFinish(); Response.End(); break;
                case "success": Success(); Response.End(); break;
            }
        }

        /// <summary>
        /// 开始扫码
        /// </summary>
        protected void Scavenging()
        {
            /************************************************************************************************
             * 获取请求参数信息
             * **********************************************************************************************/
            string RechID = RequestHelper.GetRequest("RechID").toString();
            if (RechID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["isFinish"].ToString() == "1") { this.ErrorMessage("当前充值记录已完成,不需要重试充值!"); Response.End(); }
            /************************************************************************************************
             * 解析支付二维码数据信息
             * **********************************************************************************************/
            ConfigurationHelper cfgHelper = new ConfigurationHelper(cRs["strXml"].ToString());
            if (cfgHelper == null) { this.ErrorMessage("获取支付配置参数信息失败,请重试！"); Response.End(); }
            else if (cfgHelper.Length <= 0) { this.ErrorMessage("获取支付配置参数信息失败,请重试！"); Response.End(); }
            /************************************************************************************************
             * 生成二维码加密的数据参数
             * **********************************************************************************************/
            string FormatKey = string.Format("扫码支付-|-|-{0}-|-|-{1}-|-|-扫码支付",
                cfgHelper["code_url"], DateTime.Now.ToString("yyyyMMdd"));
            FormatKey = new Fooke.Function.String(FormatKey).ToMD5().ToLower();
            /************************************************************************************************
             * 解析网页模板输出内容
             * **********************************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/rechager/scavenging.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "key": strValue = FormatKey.ToLower(); break;
                    case "token": strValue = new Fooke.Function.String(cfgHelper["code_url"]).ToEncryptionDes().ToString(); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 支付成功
        /// </summary>
        protected void Success()
        {
            /************************************************************************************************
             * 获取请求参数信息
             * **********************************************************************************************/
            string RechID = RequestHelper.GetRequest("RechID").toString();
            if (RechID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            //else if (cRs["isFinish"].ToString() != "1") { this.ErrorMessage("验证充值记录信息失败,请重试!"); Response.End(); }
            /************************************************************************************************
             * 解析网页模板输出内容
             * **********************************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/rechager/success.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }

        /**********************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 数据处理区域功能
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ********************************************************************************************/

        /// <summary>
        /// 开始执行充值信息
        /// </summary>
        protected void doRechager()
        {
            /*******************************************************************************************
             * 获取并验证请求参数的合法性
             * ******************************************************************************************/
            string RechID = RequestHelper.GetRequest("RechID").toString();
            if (RechID == "0") { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { Response.Write("拉取充值信息失败,请返回重试！"); Response.End(); }
            else if (cRs["isFinish"].ToString() != "0") { Response.Write("支付未完成,可能是出错了,请联系客服!"); Response.End(); }
            /*******************************************************************************************
             * 输出数据处理结果信息
             * ******************************************************************************************/
            if (cRs["Model"].ToString() == "支付宝")
            {
                string strResponse = new PaymentHelper().Alipay(cRs);
                Response.Write(strResponse);
                Response.End();
            }
            else if (cRs["Model"].ToString() == "微信付款")
            {
                /***********************************************************************
                 * 获取用户支付数据信息
                 * **********************************************************************/
                string strResponse = new PaymentHelper().WeiXin(cRs, tradeType: "NATIVE", sParams: new Dictionary<string, string>() {
                   {"attach","购买VIP"},
                   {"body","订单编号:"+cRs["strKey"]+""},
                   {"spbill_create_ip","59.52.66.196"}
                });
                if (strResponse.Length <= 0) { this.ErrorMessage("支付微信统一下单失败,请联系管理员！"); Response.End(); }
                if (!strResponse.Contains("xml")) { this.ErrorMessage("支付微信统一下单失败,请联系管理员！"); Response.End(); }
                else if (!strResponse.Contains("<return_msg><![CDATA[OK]]></return_msg>")) { this.ErrorMessage("支付微信统一下单失败,请联系客服！"); Response.End(); }
                /***********************************************************************
                 * 保存用户支付记录数据信息
                 * **********************************************************************/
                DbHelper.Connection.ExecuteProc("[Stored_SaveRechargeableXml]", new Dictionary<string, object>() {
                  {"RechID",cRs["RechID"].ToString()},
                  {"strXml",strResponse}
                });
                /***********************************************************************
                 * 开始跳转到指定的支付页面
                 * **********************************************************************/
                Response.Redirect("Rechager.aspx?action=scavenging&rechid=" + cRs["RechID"] + "");
                Response.End();
            }
        }
        

        /// <summary>
        /// 生成二维码
        /// </summary>
        protected void QrCode()
        {
            /*******************************************************************************************
            * 获取并验证生成二维码的地址
            * *****************************************************************************************/
            string token = RequestHelper.GetRequest("qrcode").ToEncryptionText().toString();
            if (token.Length <= 0) { Response.Write("请求参数错误！"); Response.End(); }
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
            if (key != FormatKey) { Response.Write("请求参数错误,可能数据已经被非法串改！"); Response.End(); }
            /*******************************************************************************************
            * 开始生成一张支付二维码的图片
            * *****************************************************************************************/
            BarcodeWriter writer = new BarcodeWriter();
            writer.Renderer = new ZXing.Rendering.BitmapRenderer { Background = Color.Wheat, Foreground = Color.Black };
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");//编码问题        
            writer.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            const int codeSizeInPixels = 250;   //设置图片长宽
            writer.Options.Height = writer.Options.Width = codeSizeInPixels;
            writer.Options.Margin = 0;//设置边框
            ZXing.Common.BitMatrix bm = writer.Encode(token);
            Bitmap Bit = writer.Write(bm);
            Bit.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            Bit.Dispose();
            Response.End();
        }
        /// <summary>
        /// 验证支付是否成功
        /// </summary>
        protected void VerificationFinish()
        {
            /************************************************************************************************
             * 获取请求参数信息
             * **********************************************************************************************/
            string RechID = RequestHelper.GetRequest("RechID").toString();
            if (RechID == "0") { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { Response.Write("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["isFinish"].ToString() != "1") { Response.Write("continue"); Response.End(); }
            /************************************************************************************************
             * 输出数据处理结果
             * **********************************************************************************************/
            Response.Write("success");
            Response.End();
        }
    }
}