using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Fooke.Function;
namespace Fooke.Web.Plugin
{
    public partial class SWFupload : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "preview": strPreview(); Response.End(); break;
                case "default": strDefault(); Response.End(); break;
                case "start": StartUpload(); Response.End(); break;
            }
        }
        /// <summary>
        /// 上传图片自带预览功能
        /// </summary>
        protected void strPreview()
        {
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/SWFupload/preview.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "txt":
                        strValue = RequestHelper.GetRequest("txt").ToString();
                        if (string.IsNullOrEmpty(strValue)) { strValue = RequestHelper.GetRequest("input").ToString(); }
                        if (string.IsNullOrEmpty(strValue)) { strValue = "Thumb"; }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        protected void strDefault()
        {
            /***********************************************************************************************
             * 输出网页内容信息
             * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/SWFupload/upload.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "txt":
                        strValue = RequestHelper.GetRequest("txt").ToString();
                        if (string.IsNullOrEmpty(strValue)) { strValue = RequestHelper.GetRequest("input").ToString(); }
                        if (string.IsNullOrEmpty(strValue)) { strValue = "Thumb"; }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 禁止上传文件格式
        /// </summary>
        private static readonly string NotSuffix = "php|jsp|asp|aspx";
        /// <summary>
        /// 禁止上传文件格式
        /// </summary>
        public static readonly string fileFlter = "jpg|png|bmp|gif|rar|zip|swf|flv|mp3|mp4";
        /// <summary>
        /// 开始上传文件
        /// </summary>
        protected void StartUpload()
        {
            /***************************************************************************************
             * 创建数据存储目录
             * *************************************************************************************/
            string SaveDirercoty = RequestHelper.GetRequest("SaveDirercoty").toString();
            if (string.IsNullOrEmpty(SaveDirercoty)) { SaveDirercoty = "file"; }
            /***************************************************************************************
             * 验证文件上传是否为空
             * *************************************************************************************/
            if (Request.Files == null) { this.ErrorMessage("请选择要上传的文件！"); Response.End(); }
            if (Request.Files.Count <= 0) { this.ErrorMessage("请选择要上传的文件！"); Response.End(); }
            /***************************************************************************************
             * 开始上传图片,验证上传图片资源
             * **************************************************************************************/
            HttpPostedFile thisPosted = Request.Files[0];
            if (!thisPosted.FileName.Contains(".")) { this.ErrorMessage("拉取图片格式信息失败！"); Response.End(); }
            string Flter = thisPosted.FileName.Substring(thisPosted.FileName.LastIndexOf(".") + 1);
            if (!fileFlter.Contains(Flter.ToLower())) { this.ErrorMessage("上传图片格式错误,请上传JPG或者PNG格式图片!"); Response.End(); }
            if ((thisPosted.ContentLength / 1024 / 1024) > 20) { this.ErrorMessage("上传图片资源太大,最多只允许上传2M以内的图片！"); Response.End(); }
            string FileName = string.Format("上传图片-|-|-{0}-|-|-{1}", DateTime.Now.Ticks, SaveDirercoty);
            FileName = FileName.md5().Substring(0, 16) + ".{exc}";
            /***********************************************************************************
             * 开始上传图片
             * *********************************************************************************/
            string strResponse = string.Empty;
            new Fooke.Function.PostedHelper().SaveAs(thisPosted, new PostedHelper.FileMode()
            {
                fileName = FileName,
                fileDirectory = SaveDirercoty,
                fileExt = "png|jpg|gif|bmp|rar|zip|mp3|mp4",
                fileSize = 1024 * 1024 * 20,
                Success = (Thumb) => { strResponse = Thumb; },
                Error = (Exp) => { this.JSONMessage(Exp); Response.End(); }
            });
            /***********************************************************************************
            * 输出数据处理结果信息
            * *********************************************************************************/
            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
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