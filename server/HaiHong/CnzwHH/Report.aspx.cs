using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using Fooke.Function;
using Fooke.Code;
namespace Fooke.Web.Admin
{
    public partial class Report : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {

                case "list": this.VerificationRole("用户报表"); List(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); this.Delete(); Response.End(); break;
                default: this.VerificationRole("用户报表"); this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 报表明细
        /// </summary>
        protected void List()
        {
            /******************************************************************************************
             * 获取查询条件
             * ****************************************************************************************/
            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarKey = RequestHelper.GetRequest("StarKey").toString();
            string endKey = RequestHelper.GetRequest("endKey").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            /******************************************************************************************
             * 获取网页内容
             * ****************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table id=\"frm-list\" width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"10\">用户报表 >> 报表明细</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"10\">");
            strText.Append("<form action=\"?\" id=\"frmForm\" onSubmit=\"return _doPost(this);\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"list\" />");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写要搜索的关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker({dateFmt:'yyyyMMdd'})\" type=\"text\" value=\"" + StarKey + "\" name=\"StarKey\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker({dateFmt:'yyyyMMdd'})\" type=\"text\" value=\"" + endKey + "\" name=\"endKey\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">用户名称</td>");
            strText.Append("<td width=\"100\">报表日期</td>");
            strText.Append("<td width=\"100\">金额收入</td>");
            strText.Append("<td width=\"100\">金额支出</td>");
            strText.Append("<td width=\"100\">任务数量</td>");
            strText.Append("<td width=\"100\">任务奖励</td>");
            strText.Append("<td width=\"100\">分享奖励</td>");
            strText.Append("</tr>");
            /******************************************************************************************
             * 构建分页查询条件
             * ****************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType)
                {
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                    case "userid": Params += " and userid =" + Keywords; break;
                }
            }
            if (!string.IsNullOrEmpty(StarKey)) { Params += " and dateKey>=" + StarKey + ""; }
            if (!string.IsNullOrEmpty(endKey)) { Params += " and dateKey<=" + endKey + ""; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            /******************************************************************************************
             * 构建分页查询语句
             * ****************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "Id desc";
            PageCenterConfig.Tablename = "Fooke_Report";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Report", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /******************************************************************************************
             * 输出网页内容信息
             * ****************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"list\" class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"Id\" value=\"{0}\" /></td>", Rs["Id"]);
                strText.AppendFormat("<td><a style=\"color:#f00\" href=\"Amount.aspx?action=default&userid={0}\">【明细】</a><a href=\"?action=list&userid={0}\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["dateKey"]);
                strText.AppendFormat("<td>{0} </td>", Rs["AmountIn"]);
                strText.AppendFormat("<td>{0} </td>", Rs["AmountOut"]);
                strText.AppendFormat("<td>{0} </td>", Rs["Dutynumber"]);
                strText.AppendFormat("<td>{0} </td>", Rs["DutyAmount"]);
                strText.AppendFormat("<td>{0} </td>", Rs["ShareAmount"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"10\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
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
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /*********************************************************************************
             * 输出网页数据信息
             * *******************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/report/list.html");
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
        /// 关键词回复列表
        /// </summary>
        protected void strDefault()
        {
            /********************************************************************************************
             * 获取查询语句条件
             * ******************************************************************************************/
            double StarKey = RequestHelper.GetRequest("StarKey").cDouble();
            if (StarKey <= 0) { StarKey = new Fooke.Function.String(DateTime.Now.AddDays(-30).ToString("yyyyMMdd")).cDouble(); }
            double endKey = RequestHelper.GetRequest("endKey").cDouble();
            if (endKey <= 0) { endKey = new Fooke.Function.String(DateTime.Now.AddDays(+1).ToString("yyyyMMdd")).cDouble(); }
            /********************************************************************************************
             * 获取查询请求数据信息
             * ******************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").ToString();
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                string strParams = string.Empty;
                switch (SearchType.ToLower())
                {
                    case "nickname": strParams = " and nickname='" + Keywords + "'"; break;
                    case "userid": strParams = " and userid=" + Keywords + ""; break;
                }
                if (string.IsNullOrEmpty(strParams)) { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
                DataRow cRs = DbHelper.Connection.FindRow(TableCenter.User, columns: "nickname,userid", Params: strParams);
                if (cRs == null) { this.ErrorMessage("对不起,你查找的用户不存在！"); Response.End(); }
                if (cRs != null) { ParentID = cRs["UserID"].ToString(); }
            }
            /********************************************************************************************
             * 构建网页输出内容
             * ******************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">报表查询 >> 报表统计</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form action=\"?\" id=\"frmForm\" onSubmit=\"return _doPost(this);\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<input type=\"hidden\" name=\"ParentID\" value=\"" + ParentID + "\" />");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请输入用户名\" type=\"text\" size=\"15\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker({dateFmt:'yyyyMMdd'})\" type=\"text\" value=\"" + StarKey + "\" name=\"starKey\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker({dateFmt:'yyyyMMdd'})\" type=\"text\" value=\"" + endKey + "\" name=\"endKey\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("当前位置∶<a href=\"?action=default&parentid=0&starKey=" + StarKey + "&endKey=" + endKey + "\">顶级用户</a> ");
            if (ParentID != "0")
            {
                MemberHelper.FindParent(ParentID, cRs =>
                {
                    strText.Append(" << <a href=\"?action=default&parentid=" + cRs["UserID"] + "&starKey=" + StarKey + "&endKey=" + endKey + "\">" + cRs["Nickname"] + "</a>");
                });
            }
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"100\">金额收入</td>");
            strText.Append("<td width=\"100\">金额支出</td>");
            strText.Append("<td width=\"100\">任务数量</td>");
            strText.Append("<td width=\"100\">任务奖励</td>");
            strText.Append("<td width=\"100\">分享奖励</td>");
            strText.Append("</tr>");
            /********************************************************************************************
             * 否建分页查询语句条件
             * ******************************************************************************************/
            string Tablename = "Fooke_User as Foke,FindReport(" + ParentID + "," + StarKey + "," + endKey + ") as Find";
            string Params = " and Foke.UserID = Find.UserID";
            /********************************************************************************************
             * 构建分页查询语句
             * ******************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "Foke.Nickname,Find.*";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Foke.UserID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "Foke.UserID asc";
            PageCenterConfig.Tablename = Tablename;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(Tablename, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"list\" class=\"hback\">");
                strText.AppendFormat("<td><a style=\"color:#f00\" href=\"Amount.aspx?action=default&userid={0}\">【明细】</a><a href=\"?action=default&parentid={0}&starKey=" + StarKey + "&endKey=" + endKey + "\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td>{0} </td>", Rs["AmountIn"]);
                strText.AppendFormat("<td>{0} </td>", Rs["AmountOut"]);
                strText.AppendFormat("<td>{0} </td>", Rs["Dutynumber"]);
                strText.AppendFormat("<td>{0} </td>", Rs["DutyAmount"]);
                strText.AppendFormat("<td>{0} </td>", Rs["ShareAmount"]);
                strText.AppendFormat("</tr>");
            }
            /*******************************************************************************************
             * 报表统计，资金报表统计
             * ******************************************************************************************/
            DataRow TotalRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserReportThis]", new Dictionary<string, object>() {
                {"UserID",ParentID},
                {"StarKey",StarKey},
                {"EndKey",endKey}
            });
            if (TotalRs != null)
            {
                strText.AppendFormat("<tr style=\"background:#fefef5;color:#cd0000;font-weight:900;\" class=\"hback\">");
                strText.AppendFormat("<td>数据统计</td>");
                strText.AppendFormat("<td>{0} </td>", TotalRs["AmountIn"]);
                strText.AppendFormat("<td>{0} </td>", TotalRs["AmountOut"]);
                strText.AppendFormat("<td>{0} </td>", TotalRs["Dutynumber"]);
                strText.AppendFormat("<td>{0} </td>", TotalRs["DutyAmount"]);
                strText.AppendFormat("<td>{0} </td>", TotalRs["ShareAmount"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /***************************************************************************************************************
             * 输出网络数据内容
             * *************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/report/default.html");
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
        /// 删除用户报表数据
        /// </summary>
        protected void Delete()
        {
            /************************************************************************************************************
             * 验证请求参数的合法性
             * **********************************************************************************************************/
            string strList = RequestHelper.GetRequest("Id").ToString();
            string strTarget = RequestHelper.GetRequest("target").toString("sel");
            if (strTarget == "sel")
            {
                if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            }
            /************************************************************************************************************
             * 开始删除请求数据信息
             * **********************************************************************************************************/
            string strParamter = string.Empty;
            switch (strTarget.ToLower())
            {
                case "sel": strParamter += " and Id in (" + strList + ")"; break;
                case "days": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and Addtime<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and Addtime<='" + DateTime.Now.AddYears(-1) + "'"; break;
                case "all": strParamter += " and 1=1"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_Report", Params: strParamter);
            /********************************************************************
             * 输出返回数据
             * ******************************************************************/
            this.History();
            Response.End();
        }
    }
}