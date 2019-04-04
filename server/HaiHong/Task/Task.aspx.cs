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
namespace Fooke.Web.API
{
    public partial class Task : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "appTasker": SaveTasker(); Response.End(); break;
                case "session": SaveSession(); Response.End(); break;
            }
        }
        /// <summary>
        /// 更新计划任务
        /// </summary>
        protected void SaveTasker()
        {
            /******************************************************************************************
             * 验证执行代码的合法性
             * ****************************************************************************************/
            string toKey = RequestHelper.GetRequest("tokey").ToString();
            if (string.IsNullOrEmpty(toKey)) { Response.Write("获取执行代码错误,请重试！"); Response.End(); }
            else if (toKey.Length != 16) { Response.Write("获取执行代码错误,请重试！"); Response.End(); }
            else if (toKey != "9E797DF579468058") { Response.Write("获取执行代码错误,请重试！"); Response.End(); }
            /******************************************************************************************
             * 更新币种每日数据信息
             * ****************************************************************************************/
            DbHelper.Connection.ExecuteProc("[Stored_ExecuteTasker]");
            /******************************************************************************************
             * 输出数据处理结果信息
             * ****************************************************************************************/
            Response.Write("success");
            Response.End();
        }
        /// <summary>
        /// 更新抢夺过期记录
        /// </summary>
        protected void SaveSession()
        {
            /******************************************************************************************
             * 验证执行代码的合法性
             * ****************************************************************************************/
            string toKey = RequestHelper.GetRequest("tokey").ToString();
            if (string.IsNullOrEmpty(toKey)) { Response.Write("获取执行代码错误,请重试！"); Response.End(); }
            else if (toKey.Length != 16) { Response.Write("获取执行代码错误,请重试！"); Response.End(); }
            else if (toKey != "9E797DF579468058") { Response.Write("获取执行代码错误,请重试！"); Response.End(); }
            /******************************************************************************************
             * 更新币种每日数据信息
             * ****************************************************************************************/
            DbHelper.Connection.ExecuteProc("[Stored_ExecuteAppTaskerSession]");
            /******************************************************************************************
             * 输出数据处理结果信息
             * ****************************************************************************************/
            Response.Write("success");
            Response.End();
        }
    }
}