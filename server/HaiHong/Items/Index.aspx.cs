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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Fooke.Web
{
    public partial class Index : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "default": strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 默认主页面信息
        /// </summary>
        protected void strDefault()
        {
            /***********************************************************************
             * 解析标签网页内容
             * **********************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/Index.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "title": strValue = this.GetParameter("sitetitle").toString(); break;
                }
                return strValue;
            }), true);
            Response.Write(strResponse);
            Response.End();
        }
    }
}