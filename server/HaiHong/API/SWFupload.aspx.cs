using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Fooke.Function;
namespace Fooke.Web.API
{
    public partial class SWFupload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strRequest = RequestHelper.GetRequest("action").toString();
            if (string.IsNullOrEmpty(strRequest)) { strRequest = "default"; }
            switch (strRequest)
            {
                case "start": StartUpload(); Response.End(); break;
            }
        }
        /// <summary>
        /// 禁止上传文件格式
        /// </summary>
        private static readonly string NotSuffix = "php|jsp|asp|aspx";
        /// <summary>
        /// 输出JSON格式数据
        /// </summary>
        /// <param name="responseText"></param>
        /// <param name="iSuccess"></param>
        /// <param name="otherText"></param>
        public void JSONMessage(string responseText, bool iSuccess = false, string otherText = "")
        {
            StringBuilder strXml = new StringBuilder();
            strXml.Append("{");
            strXml.Append("\"success\":\"" + iSuccess.ToString().ToLower() + "\"");
            strXml.Append(",\"type\":\"alert\"");
            strXml.Append(",\"tips\":\"" + responseText + "\"");
            if (!string.IsNullOrEmpty(otherText)) { strXml.Append("," + otherText); }
            strXml.Append("}");
            System.Web.HttpContext.Current.Response.Write(strXml.ToString());
            System.Web.HttpContext.Current.Response.End();
        }
        /// <summary>
        /// 允许上传的图片文件格式
        /// </summary>
        public static readonly string fileFlter = "jpg|png|bmp|gif|rar|zip|swf|flv|mp3";


        /// <summary>
        /// 开始上传文件
        /// </summary>
        protected void StartUpload()
        {
            string SaveDirercoty = RequestHelper.GetRequest("SaveDirercoty").toString();
            if (string.IsNullOrEmpty(SaveDirercoty)) { SaveDirercoty = DateTime.Now.ToString("yyyyMMdd"); }
            if (Request.Files == null) { this.JSONMessage("请选择要上传的文件！"); Response.End(); }
            if (Request.Files.Count <= 0) { this.JSONMessage("请选择要上传的文件！"); Response.End(); }
            /***********************************************************************************
             * 开始上传图片
             * *********************************************************************************/
            string strResponse = string.Empty;
            try
            {
                string fileName = string.Format("上传文件-|-|-{0}-|-|-{1}", DateTime.Now.Ticks.ToString(), new Random().NextDouble().ToString());
                fileName = new Fooke.Function.String(fileName).ToMD5().Substring(0, 16).ToLower();
                fileName = fileName + ".{exc}";
                new PostedHelper().SaveAs(Request.Files[0], new Fooke.Function.PostedHelper.FileMode()
                {
                    fileName = fileName,
                    fileDirectory = SaveDirercoty,
                    fileExt = "jpg|png|bmp",
                    fileSize = 1024 * 1024 * 2,
                    Success = (thumb) => { strResponse = thumb; },
                    Error = (Exp) => { this.JSONMessage(Exp); Response.End(); }
                });
            }
            catch { }
            if (string.IsNullOrEmpty(strResponse))
            { this.JSONMessage("图片保存过程中发生错误,请重试！"); Response.End(); }
            /***********************************************************************************
             * 输出数据处理结果信息
             * *********************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"url\":\"" + strResponse + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}