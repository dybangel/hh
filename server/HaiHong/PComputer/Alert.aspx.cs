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
namespace Fooke.Web.Member
{
    public partial class Alert :Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 用户消息默认页面
        /// </summary>
        protected void strDefault()
        {
            /*******************************************************************************************************************************
             * 输出网页内容信息
             * *****************************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/alert/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        } 
    }
}