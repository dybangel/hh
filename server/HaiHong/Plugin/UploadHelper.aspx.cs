using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fooke.Function;
using Fooke.SimpleMaster;
namespace Fooke.Web.Plugin
{
    public partial class UploadHelper : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "default": strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        protected void strDefault()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/upload/default.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;

                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }
}