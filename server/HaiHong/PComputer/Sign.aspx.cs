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
using Fooke.SimpleMaster;
namespace Fooke.Member
{
    public partial class Sign : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (strRequest == "default") { this.strDefault(); Response.End(); }
        }
        /// <summary>
        /// 单页默认页面
        /// </summary>
        protected void strDefault()
        {
            /*******************************************************************************************************
             * 获取我的签到记录信息
             * ******************************************************************************************************/
            DataRow SignRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindSignThis]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"Nickname",MemberRs["Nickname"].ToString()},
                {"Yesday",DateTime.Now.AddDays(-1).ToString("yyyyMMdd")},
                {"isClear",(isBlank() ? "0" : "1")}
            });
            if (SignRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (SignRs["UserID"].ToString() != MemberRs["UserID"].ToString())
            { this.ErrorMessage("越权操作,请重试！"); Response.End(); }
            /************************************************************************************************************
             * 获取用户连续签到次数
             * **********************************************************************************************************/
            double Continuous = new Fooke.Function.String(SignRs["Repeatnum"].ToString()).cDouble();
            DateTime LastDate = new Fooke.Function.String(SignRs["lastDate"].ToString()).cDate();
            /************************************************************************************************************
             * 输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/sign/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "ispacker": if (!isBlank()) { strValue = "disabled=\"disabled\""; }
                        else if (Continuous <= 4) { strValue = "disabled=\"disabled\""; }; break;
                    case "issign": if (isBlank()) { strValue = "disabled=\"disabled\""; }; break;
                    case "weekday": strValue = DateTime.Now.DayOfWeek.ToString(); break;
                    case "datekey": strValue = DateTime.Now.ToString("yyyyMMdd"); break;
                    case "lastkey": strValue = LastDate.ToString("yyyyMMdd"); break;
                    case "continuous": strValue = Continuous.ToString("0"); break;
                    case "hour": strValue = DateTime.Now.Hour.ToString("00"); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 是否为签到空白期
        /// </summary>
        /// <returns></returns>
        protected bool isBlank()
        {
            bool isClear = false;
            /**************************************************************************************
             * 开始执行数据查询
             * ************************************************************************************/
            try
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) { isClear = true; }
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday) { isClear = true; }
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday
                && new Fooke.Function.String(DateTime.Now.Hour.ToString("00")).cInt() >= 16)
                { isClear = true; }
            }
            catch { }
            /**************************************************************************************
             * 返回数据处理结果
             * ************************************************************************************/
            return isClear;
        }
    }
}