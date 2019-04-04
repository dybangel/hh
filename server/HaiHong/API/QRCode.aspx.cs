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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ZXing;
namespace Fooke.Web.API
{
    public partial class QRCode : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /***********************************************************************************
             * 判断当前请求类型
             * *********************************************************************************/
            switch (strRequest)
            {
                case "define": SaveDefine(); Response.End(); break;
                case "safetycode": Safetycode(); Response.End(); break;
            }
        }
        /// <summary>
        /// 生成自定义二维码信息
        /// </summary>
        protected void SaveDefine()
        {
            /***********************************************************************************************************
             *验证请求参数的合法性
             ************************************************************************************************************/
            if (RequestHelper.GetRequest("token").Length <= 0) { Response.Write("请求参数错误！"); Response.End(); }
            string token = RequestHelper.GetRequest("token").ToEncryptionText().toString();
            if (string.IsNullOrEmpty(token)) { Response.Write("请求参数错误！"); Response.End(); }
            else if (token.Length <= 0) { Response.Write("请求参数错误！"); Response.End(); }
            else if (token.Length >= 1024) { Response.Write("请求参数错误！"); Response.End(); }
            /***********************************************************************************************************
             * 验证请求数据是否合法
             ************************************************************************************************************/
            string Signaturekey = RequestHelper.GetRequest("key").toString();
            if (string.IsNullOrEmpty(Signaturekey)) { Response.Write("请求参数错误,可能数据已经被非法串改！"); Response.End(); }
            else if (Signaturekey.Length <= 0) { Response.Write("请求参数错误,可能数据已经被非法串改！"); Response.End(); }
            else if (Signaturekey.Length != 24) { Response.Write("请求参数错误,可能数据已经被非法串改！"); Response.End(); }
            string VerificationKey = string.Format("签名加密-|-|-{0}-|-|-签名加密", token);
            VerificationKey = new Fooke.Function.String(VerificationKey).ToMD5().Substring(0, 24).toString();
            if (VerificationKey != Signaturekey) { Response.Write("请求参数错误,可能数据已经被非法串改！"); Response.End(); }
            /***********************************************************************************************************
            * 开始生成二维码并且输出
            * ***********************************************************************************************************/
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
            /***********************************************************************************************************
            * 输出数据处理结果
            * ***********************************************************************************************************/
            Bit.Save(stream: Response.OutputStream,
                format: System.Drawing.Imaging.ImageFormat.Gif);
            Bit.Dispose();
            Response.End();
        }
        /// <summary>
        /// 生成安全码二维码信息
        /// </summary>
        protected void Safetycode()
        {
            /***********************************************************************************************************
             *验证请求参数的合法性
             ************************************************************************************************************/
            string SafetyID = RequestHelper.GetRequest("SafetyID").toInt();
            if (SafetyID == "0") { Response.Write("获取安全码信息失败！"); Response.End(); }
            string Fileurl = string.Format("{0}{1}/pcomputer/safecode.aspx?safetyid={2}",
                FunctionCenter.SiteUrl(), Win.ApplicationPath, SafetyID);
            /***********************************************************************************************************
            * 开始生成二维码并且输出
            * ***********************************************************************************************************/
            BarcodeWriter writer = new BarcodeWriter();
            //writer.Renderer = new ZXing.Rendering.BitmapRenderer { Background = Color.Wheat, Foreground = Color.SkyBlue };
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options.Hints.Add(EncodeHintType.CHARACTER_SET, "UTF-8");//编码问题        
            writer.Options.Hints.Add(EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);
            const int codeSizeInPixels = 250;   //设置图片长宽
            writer.Options.Height = writer.Options.Width = codeSizeInPixels;
            writer.Options.Margin = 0;//设置边框
            ZXing.Common.BitMatrix bm = writer.Encode(Fileurl);
            Bitmap Bit = writer.Write(bm);
            /***********************************************************************************************************
            * 输出数据处理结果
            * ***********************************************************************************************************/
            Bit.Save(stream: Response.OutputStream,
                format: System.Drawing.Imaging.ImageFormat.Gif);
            Bit.Dispose();
            Response.End();
        }
    }
}