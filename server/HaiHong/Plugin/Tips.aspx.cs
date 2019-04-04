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
namespace Fooke.Web.Plugin
{
    public partial class Tips : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "err": strDefault(); Response.End(); break;
                case "confirm": strConfirm(); Response.End(); break;
                default: break;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        protected void strConfirm()
        {
            string trueUrl = RequestHelper.GetRequest("trueUrl").ToEncryptionText().toString();
            if (string.IsNullOrEmpty(trueUrl)) { trueUrl = "javascript:history.go(-1);"; }
            if (!trueUrl.Contains("history.")) { trueUrl = "javascript:" + trueUrl + ";"; }

            string falseUrl = RequestHelper.GetRequest("falseUrl").ToEncryptionText().toString();
            if (string.IsNullOrEmpty(falseUrl)) { falseUrl = "javascript:history.go(-1);"; }
            if (!falseUrl.Contains("history.")) { falseUrl = "javascript:" + trueUrl + ";"; }

            string MessageText = RequestHelper.GetRequest("tips").ToEncryptionText().toString();
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/tips/confirm.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "falseUrl": strValue = falseUrl; break;
                    case "trueUrl": strValue = trueUrl; break;
                    case "tips": strValue = MessageText; break;
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
            string returnUrl = RequestHelper.GetRequest("returnUrl").ToEncryptionText().toString();
            if (string.IsNullOrEmpty(returnUrl)) { returnUrl = "javascript:history.go(-1);"; }
            if (!returnUrl.Contains("history.")) { returnUrl = "javascript:" + returnUrl + ";"; }
            string MessageText = RequestHelper.GetRequest("tips").ToEncryptionText().toString();
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/tips/error.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "url": strValue = returnUrl; break;
                    case "tips": strValue = MessageText; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }
}