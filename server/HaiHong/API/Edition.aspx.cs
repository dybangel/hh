using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fooke.Function;
namespace Fooke.Web.API
{
    public partial class Edition : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 基本信息配置
        /// </summary>
        protected void strDefault()
        {
            string DeviceType = RequestHelper.GetRequest("DeviceType").toString("Android");
            StringBuilder strXml = new StringBuilder();
            try
            {
                string iOSName = this.GetParameter(DeviceType + "Name", "appXml").toString();
                string iOSDeposit = this.GetParameter(DeviceType + "Deposit", "appXml").toString();
                string iOSEdition = this.GetParameter(DeviceType + "Edition", "appXml").toString();
                string iOSport = this.GetParameter(DeviceType + "port", "appXml").toString();
                string iOSPackerName = this.GetParameter(DeviceType + "PackerName", "appXml").toString();
                strXml.Append("{\"success\":\"true\"");
                strXml.Append(",\"name\":\"" + iOSName + "\"");
                strXml.Append(",\"deposit\":\"" + iOSDeposit + "\"");
                strXml.Append(",\"edition\":\"" + iOSEdition + "\"");
                strXml.Append(",\"port\":\"" + iOSport + "\"");
                strXml.Append(",\"packerName\":\"" + iOSPackerName + "\"");
                strXml.Append("}");
            }
            catch { }
            Response.Write(strXml.ToString());
            Response.End();
        }
    }
}