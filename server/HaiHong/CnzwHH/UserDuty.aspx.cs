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
    public partial class UserDuty : Fooke.Code.AdminHelper
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
                case "computer": this.VerificationRole("任务列表"); strComputer(); Response.End(); break;
                case "excel": this.VerificationRole("任务列表"); strExcel(); Response.End(); break;
                case "savexport": this.VerificationRole("任务列表"); SaveExport(); Response.End(); break;
                case "default": this.VerificationRole("任务列表"); strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 数据统计
        /// </summary>
        protected void strComputer()
        {
            /**************************************************************************************
            * 获取统计日期数据信息
            * ************************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").ToString();
            if (string.IsNullOrEmpty(StarDate)) { StarDate = DateTime.Now.AddDays(-30).ToString(); }
            else if (!StarDate.isDate()) { StarDate = DateTime.Now.AddDays(-30).ToString(); }
            string EndDate = RequestHelper.GetRequest("EndDate").ToString();
            if (string.IsNullOrEmpty(EndDate)) { EndDate = DateTime.Now.AddDays(+1).ToString(); }
            else if (!EndDate.isDate()) { EndDate = DateTime.Now.AddDays(+1).ToString(); }
            /*****************************************************************************************
             * 数据统计信息
             * ****************************************************************************************/
            DataTable Tab = DbHelper.Connection.ExecuteFindTable("[Stored_FindUserDutyComputerTable]", new Dictionary<string, object>() {
                {"StarDate",new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00")},
                {"EndDate",new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:59")}
            });
            StringBuilder strBuilder = new StringBuilder();
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.AppendFormat("<div class=\"items\">");
                strBuilder.AppendFormat("<div class=\"name\">{0}</div>", Rs["menu"]);
                strBuilder.AppendFormat("<div class=\"number\">{0}{1}</div>", Rs["number"], Rs["unit"]);
                strBuilder.AppendFormat("</div>");
            }
            /***********************************************************************************************************
             * 输出网页参数内容
             * *********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/userDuty/computer.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strBuilder.ToString(); break;
                    case "StarDate": strValue = new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd"); break;
                    case "EndDate": strValue = new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 数据导出
        /// </summary>
        protected void strExcel()
        {
            /***********************************************************************************************************
             * 输出网页参数内容
             * *********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/userDuty/excel.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                   
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 列表信息
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
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            string UnionID = RequestHelper.GetRequest("UnionID").toInt();
            string DeviceType = RequestHelper.GetRequest("DeviceType").ToString();
            /**************************************************************************************
             * 构建网页内容
             * *************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"10\">任务记录 >> 任务列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"10\">");
            strText.Append("<form id=\"SearchForm\" OnSubmit=\"return _doPost(this)\" action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Appname",Text="搜任务名称"},
                new OptionMode(){Value="strUnion",Text="搜渠道名称"},
                new OptionMode(){Value="UserID",Text="搜用户ID"},
                new OptionMode(){Value="strCity",Text="搜所在城市"},
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
            strText.Append("<td width=\"100\">下载用户</td>");
            strText.Append("<td width=\"100\">渠道名称</td>");
            strText.Append("<td width=\"140\">应用名称</td>");
            strText.Append("<td width=\"60\">奖励金额</td>");
            strText.Append("<td width=\"200\">设备标识</td>");
            strText.Append("<td width=\"80\">设备类型</td>");
            strText.Append("<td width=\"120\">IP地址</td>");
            strText.Append("<td>所在城市</td>");
            strText.Append("<td width=\"140\">日期</td>");
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
                    case "strUnion": strParams += " and strUnion like '%" + Keywords + "%'"; break;
                    case "UserID": strParams += " and UserID like '%" + Keywords + "%'"; break;
                    case "strCity": strParams += " and strCity like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParams += " and Addtime>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParams += " and Addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (UserID != "0") { strParams += " and UserID=" + UserID + ""; }
            if (UnionID != "0") { strParams += " and UnionID=" + UnionID + ""; }
            if (DeviceType.Length != 0) { strParams += " and DeviceType='" + DeviceType + "'"; }
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "DutyID,DutyKey,UnionID,strUnion,UserID,Nickname,DeviceType,DeviceCode,AppID,Appname,strIP,strCity,Points,Amount,Addtime";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "DutyID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " DutyID desc";
            PageCenterConfig.Tablename = "Fooke_UserDuty";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserDuty", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************
            * 循环遍历网页内容
            * **********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"DutyID\" value=\"{0}\" /></td>", Rs["DutyID"]);
                strText.AppendFormat("<td><a href=\"?action=default&UserID={0}\">{1}</a></td>", Rs["UserID"], Rs["Nickname"]);
                strText.AppendFormat("<td><a href=\"?action=default&UnionID={0}\">{1}</a></td>", Rs["UnionID"], Rs["strUnion"]);
                strText.AppendFormat("<td><a href=\"?action=default&Appname={0}\">{0}</a></td>", Rs["Appname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Points"]);
                strText.AppendFormat("<td>{0}</td>", Rs["DeviceCode"]);
                strText.AppendFormat("<td>{0}</td>", Rs["DeviceType"]);
                strText.AppendFormat("<td>{0}</td>", Rs["strip"]);
                strText.AppendFormat("<td>{0}</td>", Rs["strCity"]);
                strText.AppendFormat("<td>{0}</td>", new Fooke.Function.String(Rs["Addtime"].ToString()).cDate().ToString("yyyy-MM-dd HH:mm:ss"));
                strText.AppendFormat("</tr>");
            }
            /***********************************************************************************************
            * 构建分页控件信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"10\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
            * 构建操作按钮信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"10\">");
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
            strText.Append("<input type=\"button\" class=\"button\" value=\"数据导出\" onclick=\"window.location='?action=excel'\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /***********************************************************************************************
            * 输出网页信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/userDuty/default.html");
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
            string strList = RequestHelper.GetRequest("DutyID").ToString();
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
                case "sel": strParamter += " and DutyID in (" + strList + ")"; break;
                case "days": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and Addtime<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and Addtime<='" + DateTime.Now.AddYears(-1) + "'"; break;
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
            try { DbHelper.Connection.Delete("Fooke_UserDuty", Params: strParamter); }
            catch { }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了任务记录数据ID(" + strParamter + ")"); }
            catch { }
            /**********************************************************************************************************************
             * 返回数据处理结果
             * ********************************************************************************************************************/
            this.History();
            Response.End();
        }

        /// <summary>
        /// 导出网页数据
        /// </summary>
        protected void SaveExport()
        {
            /******************************************************************************************
             * 获取数据导出预设参数信息
             * ****************************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            if (StarDate.Length != 0 && !StarDate.isDate()) { this.ErrorMessage("查询开始日期格式错误！"); Response.End(); }
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            if (EndDate.Length != 0 && !EndDate.isDate()) { this.ErrorMessage("查询结束日期格式错误！"); Response.End(); }
            string strUnion = RequestHelper.GetRequest("strUnion").toString();
            if (strUnion.Length >= 20) { this.ErrorMessage("联盟渠道名称字段长度不能超过20个汉字！"); Response.End(); }
            string Appname = RequestHelper.GetRequest("Appname").toString();
            if (Appname.Length >= 20) { this.ErrorMessage("应用名称字段长度不能超过20个汉字！"); Response.End(); }
            string strCity = RequestHelper.GetRequest("strCity").toString();
            if (strCity.Length >= 20) { this.ErrorMessage("城市名称字段长度不能超过20个汉字！"); Response.End(); }
            string strColumns = RequestHelper.GetRequest("strColumns", false).toString();
            if (strColumns.Length <= 0) { strColumns = "*"; }
            /******************************************************************************************
            * 开始构建数据查询语句
            * ****************************************************************************************/
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append("select " + strColumns + " from [Fun_FindUserDuty] where 1=1");
            if (strUnion.Length != 0) { strTabs.Append(" and strUnion like '%" + strUnion + "%'"); }
            if (Appname.Length != 0) { strTabs.Append(" and Appname like '%" + Appname + "%'"); }
            if (strCity.Length != 0) { strTabs.Append(" and strCity like '%" + strCity + "%'"); }
            if (StarDate.Length != 0 && StarDate.isDate()) { strTabs.Append(" and Addtime>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"); }
            if (EndDate.Length != 0 && EndDate.isDate()) { strTabs.Append(" and Addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:59") + "'"); }
            /******************************************************************************************
            * 查询数据导出结果
            * ****************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteDataTable(strTabs.ToString());
            if (thisTab == null) { this.ErrorMessage("数据查询语句生成失败,请联系开发人员！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("未查询到需要导出的数据！"); Response.End(); }
            /******************************************************************************************
            * 输出数据处理结果
            * ****************************************************************************************/
            new ExcelHelper().TableOutput(thisTab, "excel");
            Response.End();
        }
    }
}