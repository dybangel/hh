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
namespace Fooke.Web.Member
{
    public partial class Service : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                default: strDefault(); Response.End(); break;
            }
        }
        
        /// <summary>
        /// 默认主页
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 开始输出网页内容信息
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/service/default.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }), isLabel: true);
            /**************************************************************************************
            * 输出网页处理结果
            * ************************************************************************************/
            Response.Write(strResponse);
            Response.End();
        }
    }
}