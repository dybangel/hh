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
namespace Fooke.Web.Admin
{
    public partial class AppDownReport : Fooke.Code.AdminHelper
    {
        /// <summary>
        /// 入口函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "list": this.VerificationRole("下载报表"); ListReport(); Response.End(); break;
                case "default": this.VerificationRole("下载报表"); strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 报表明细
        /// </summary>
        protected void ListReport()
        {
            /**************************************************************************************
             * 获取筛选条件信息
             * *************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string dateKey = RequestHelper.GetRequest("dateKey").toInt();
            string AppId = RequestHelper.GetRequest("AppId").toInt();
            /**************************************************************************************
             * 构建网页内容
             * *************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"28\">统计报表 >> 明细报表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"28\">");
            strText.Append("<form id=\"SearchForm\" OnSubmit=\"return _doPost(this)\" action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"list\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Appname",Text="搜应用名称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"搜索关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">应用</td>");
            strText.Append("<td width=\"60\">日期</td>");
            strText.Append("<td width=\"60\">合计</td>");
            strText.Append("<td>00点</td>");
            strText.Append("<td>01点</td>");
            strText.Append("<td>02点</td>");
            strText.Append("<td>03点</td>");
            strText.Append("<td>04点</td>");
            strText.Append("<td>05点</td>");
            strText.Append("<td>06点</td>");
            strText.Append("<td>07点</td>");
            strText.Append("<td>08点</td>");
            strText.Append("<td>09点</td>");
            strText.Append("<td>10点</td>");
            strText.Append("<td>11点</td>");
            strText.Append("<td>12点</td>");
            strText.Append("<td>13点</td>");
            strText.Append("<td>14点</td>");
            strText.Append("<td>15点</td>");
            strText.Append("<td>16点</td>");
            strText.Append("<td>17点</td>");
            strText.Append("<td>18点</td>");
            strText.Append("<td>19点</td>");
            strText.Append("<td>20点</td>");
            strText.Append("<td>21点</td>");
            strText.Append("<td>22点</td>");
            strText.Append("<td>23点</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************/
            string strParams = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "Appname": strParams += " and Appname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParams += " and dateKey>=" + new Fooke.Function.String(StarDate).cDate().ToString("yyyyMMdd") + ""; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParams += " and dateKey<=" + new Fooke.Function.String(EndDate).cDate().ToString("yyyyMMdd") + ""; }
            if (dateKey != "0") { strParams += " and dateKey=" + dateKey + ""; }
            if (AppId != "0") { strParams += " and AppId=" + AppId + ""; }
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "ReportID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " ReportID desc";
            PageCenterConfig.Tablename = "Fooke_AppDownReport";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_AppDownReport", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************
            * 生成数据导出Sql语句
            * **********************************************************************************************/
            string executeSql = "select * from Fooke_AppDownReport where 1=1 " + strParams + "";
            executeSql = new Fooke.Function.String(executeSql).ToEncryptionDes().ToString();
            /***********************************************************************************************
            * 循环遍历网页内容
            * **********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"ReportID\" value=\"{0}\" /></td>", Rs["ReportID"]);
                strText.AppendFormat("<td><a href=\"?action=list&AppId={0}\">{1}</a></td>", Rs["AppId"], Rs["Appname"]);
                strText.AppendFormat("<td><a href=\"?action=list&dateKey={0}\">{0}</a></td>", Rs["dateKey"]);
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["HourTotal"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0000"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0001"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0002"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0003"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0004"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0005"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0006"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0007"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0008"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0009"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0010"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0011"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0012"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0013"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0014"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0015"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0016"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0017"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0018"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0019"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0020"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0021"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0022"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0023"].ToString());
                strText.AppendFormat("</tr>");
            }
            /***********************************************************************************************
            * 构建分页控件信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"28\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
            * 构建操作按钮信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"28\">");
            strText.Append("<select name=\"target\">");
            strText.Append("<option value=\"sel\">删除选中的数据</option>");
            strText.Append("<option value=\"days\">删除一天前的记录</option>");
            strText.Append("<option value=\"week\">删除一周前的记录</option>");
            strText.Append("<option value=\"month\">删除一月前的记录</option>");
            strText.Append("<option value=\"byear\">删除半年前的记录</option>");
            strText.Append("<option value=\"year\">删除一年前的记录</option>");
            strText.Append("<option value=\"all\">删除所有记录</option>");
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" class=\"button\" value=\"删除选中\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" value=\"导出筛选数据\" onclick=\"window.location='excel.aspx?action=save&exeText=" + executeSql + "'\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /***********************************************************************************************
            * 输出网页信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appDownReport/list.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 统计报表
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 获取筛选条件信息
             * *************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string dateKey = RequestHelper.GetRequest("dateKey").toInt();
            string AppId = RequestHelper.GetRequest("AppId").toInt();
            /**************************************************************************************
             * 为请求参数默认赋值
             * *************************************************************************************/
            if (string.IsNullOrEmpty(StarDate)) { StarDate = DateTime.Now.AddDays(-30).ToString("yyyyMMdd"); }
            else if (StarDate.Length <= 0) { StarDate = DateTime.Now.AddDays(-30).ToString("yyyyMMdd"); }
            else if (StarDate.Length != 8) { StarDate = DateTime.Now.AddDays(-30).ToString("yyyyMMdd"); }
            if (string.IsNullOrEmpty(EndDate)) { EndDate = DateTime.Now.AddDays(+1).ToString("yyyyMMdd"); }
            else if (EndDate.Length <= 0) { EndDate = DateTime.Now.AddDays(+1).ToString("yyyyMMdd"); }
            else if (EndDate.Length != 8) { EndDate = DateTime.Now.AddDays(+1).ToString("yyyyMMdd"); }
            /**************************************************************************************
             * 构建网页内容
             * *************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"27\">统计报表 >> 明细报表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"27\">");
            strText.Append("<form id=\"SearchForm\" OnSubmit=\"return _doPost(this)\" action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Appname",Text="搜应用名称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"搜索关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker({dateFmt:'yyyyMMdd'})\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker({dateFmt:'yyyyMMdd'})\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"80\">应用</td>");
            strText.Append("<td width=\"120\">日期</td>");
            strText.Append("<td width=\"60\">合计</td>");
            strText.Append("<td>00点</td>");
            strText.Append("<td>01点</td>");
            strText.Append("<td>02点</td>");
            strText.Append("<td>03点</td>");
            strText.Append("<td>04点</td>");
            strText.Append("<td>05点</td>");
            strText.Append("<td>06点</td>");
            strText.Append("<td>07点</td>");
            strText.Append("<td>08点</td>");
            strText.Append("<td>09点</td>");
            strText.Append("<td>10点</td>");
            strText.Append("<td>11点</td>");
            strText.Append("<td>12点</td>");
            strText.Append("<td>13点</td>");
            strText.Append("<td>14点</td>");
            strText.Append("<td>15点</td>");
            strText.Append("<td>16点</td>");
            strText.Append("<td>17点</td>");
            strText.Append("<td>18点</td>");
            strText.Append("<td>19点</td>");
            strText.Append("<td>20点</td>");
            strText.Append("<td>21点</td>");
            strText.Append("<td>22点</td>");
            strText.Append("<td>23点</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************/
            string strParams = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "Appname": strParams += " and Appname like '%" + Keywords + "%'"; break;
                }
            }
            if (AppId != "0") { strParams += " and AppId=" + AppId + ""; }
            /***********************************************************************************************
            * 构建分页查询数据表
            * **********************************************************************************************/
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append("(");
            strTabs.Append("    select isNull(Chs.Appname,'未知应用') as Appname,Report.*");
            strTabs.Append("    from [GetAppDownReport](" + StarDate + "," + EndDate + ") as Report");
            strTabs.Append("    left join Fooke_Application as Chs On Report.AppId = Chs.AppId");
            strTabs.Append(") as FokeApps");
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "AppId";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " AppId asc";
            PageCenterConfig.Tablename = strTabs.ToString();
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(strTabs.ToString(), strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************
            * 生成数据导出Sql语句
            * **********************************************************************************************/
            string executeSql = "select * from " + strTabs.ToString() + " where 1=1 " + strParams + "";
            executeSql = new Fooke.Function.String(executeSql).ToEncryptionDes().ToString();
            /***********************************************************************************************
            * 循环遍历网页内容
            * **********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><a href=\"?action=default&AppId={0}&starDate=" + StarDate + "&endDate=" + EndDate + "\">{1}</a></td>", Rs["AppId"], Rs["Appname"]);
                strText.AppendFormat("<td>{0}-{1}</td>", StarDate, EndDate);
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["HourTotal"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0000"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0001"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0002"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0003"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0004"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0005"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0006"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0007"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0008"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0009"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0010"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0011"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0012"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0013"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0014"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0015"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0016"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0017"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0018"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0019"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0020"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0021"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0022"].ToString());
                strText.AppendFormat("<td class=\"value\" value=\"{0}\">{0}</td>", Rs["Hour0023"].ToString());
                strText.AppendFormat("</tr>");
            }
            /***********************************************************************************************
            * 构建分页控件信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"27\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
            * 构建操作按钮信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"27\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"导出筛选数据\" onclick=\"window.location='excel.aspx?action=save&exeText=" + executeSql + "'\" />");
            strText.Append("当前模块统计渠道指定时间段内的任务次数情况,所有数据均来源任务记录!");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /***********************************************************************************************
            * 输出网页信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appDownReport/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        protected void Delete()
        {
            /**********************************************************************************************************************
            * 获取数据删除模式信息
            * ********************************************************************************************************************/
            string strTarget = RequestHelper.GetRequest("target").toString("sel");
            if (string.IsNullOrEmpty(strTarget)) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            else if (strTarget.Length <= 0) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            /**********************************************************************************************************************
             * 验证参数合法性
             * ********************************************************************************************************************/
            string strList = RequestHelper.GetRequest("ReportID").ToString();
            if (strTarget == "sel")
            {
                if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            }
            /**********************************************************************************************************************
             * 构建数据删除语句
             * ********************************************************************************************************************/
            string strParamter = string.Empty;
            switch (strTarget.ToLower())
            {
                case "sel": strParamter += " and ReportID in (" + strList + ")"; break;
                case "days": strParamter += " and DateKey<=" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ""; break;
                case "week": strParamter += " and DateKey<=" + DateTime.Now.AddDays(-7).ToString("yyyyMMdd") + ""; break;
                case "month": strParamter += " and DateKey<=" + DateTime.Now.AddMonths(-1).ToString("yyyyMMdd") + ""; break;
                case "byear": strParamter += " and DateKey<=" + DateTime.Now.AddDays(-180).ToString("yyyyMMdd") + ""; break;
                case "year": strParamter += " and DateKey<=" + DateTime.Now.AddYears(-1).ToString("yyyyMMdd") + ""; break;
                case "all": strParamter += " and 1=1"; break;
            }
            /**********************************************************************************************************************
             * 判断删除请求书否合法
             * ********************************************************************************************************************/
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            else if (strParamter.Length <= 0) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            /**********************************************************************************************************************
             * 开始删除请求数据
             * ********************************************************************************************************************/
            try { DbHelper.Connection.Delete("Fooke_AppDownReport", Params: strParamter); }
            catch { }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了下载记录报表ID(" + strParamter + ")"); }
            catch { }
            /**********************************************************************************************************************
             * 返回数据处理结果
             * ********************************************************************************************************************/
            this.History();
            Response.End();
        }
    }
}