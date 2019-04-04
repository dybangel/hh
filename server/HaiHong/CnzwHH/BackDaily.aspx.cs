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
    public partial class BackDaily : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "del": this.VerificationRole("超级管理员权限"); this.Delete(); Response.End(); break;
                default: this.VerificationRole("回调探针"); this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 数据列表
        /// </summary>
        protected void strDefault()
        {
            /****************************************************************************************
             * 获取查询参数条件信息
             * **************************************************************************************/
            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string UnionID = RequestHelper.GetRequest("UnionID").toInt();
            /****************************************************************************************
             * 构建网页内容
             * **************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">回调探针 >> 回调记录</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append("<form id=\"SearchForm\" OnSubmit=\"return _doPost(this)\" action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="strUnion",Text="搜渠道名称"},
                new OptionMode(){Value="strIP",Text="搜IP地址"},
                new OptionMode(){Value="strXml",Text="搜回调内容"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写要搜索的关键词\" type=\"text\" size=\"15\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            /****************************************************************************************
             * 构建表格信息
             * **************************************************************************************/
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"60\">联盟ID</td>");
            strText.Append("<td width=\"120\">联盟名称</td>");
            strText.Append("<td width=\"140\">时间</td>");
            strText.Append("<td width=\"120\">IP地址</td>");
            strText.Append("<td>请求参数</td>");
            strText.Append("</tr>");
            /************************************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType)
                {
                    case "strUnion": Params += " and strUnion like '%" + Keywords + "%'"; break;
                    case "strIP": Params += " and strIP like '%" + Keywords + "%'"; break;
                    case "strXml": Params += " and strXml like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate + "'"; }
            if (UnionID != "0") { Params += " and UnionID=" + UnionID + ""; }
            /************************************************************************************************************
             * 获取分页数量信息
             * **********************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "DailyID,UnionID,strUnion,strXml,Addtime,strIP";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "DailyID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "DailyID desc";
            PageCenterConfig.Tablename = "Fooke_UnionBackDaily";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UnionBackDaily", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /************************************************************************************************************
             * 输出查询内容
             * **********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"DailyID\" value=\"{0}\" /></td>", Rs["DailyID"]);
                strText.AppendFormat("<td><a href=\"?action=default&UnionID={0}\">{0}</a></td>", Rs["UnionID"]);
                strText.AppendFormat("<td><a href=\"?action=default&UnionID={0}&StarDate=" + StarDate + "&EndDate=" + EndDate + "\">{1}</a></td>", Rs["UnionID"], Rs["strUnion"]);
                strText.AppendFormat("<td>{0}</td>", new Fooke.Function.String(Rs["Addtime"].ToString()).cDate().ToString("yyyy-MM-dd HH:mm:ss"));
                strText.AppendFormat("<td>{0}</td>", Rs["strIP"]);
                strText.AppendFormat("<td>{0}</td>", Rs["strXml"]);
                strText.AppendFormat("</tr>");
            }
            /************************************************************************************************************
             * 构建分页控件与操作按钮
             * **********************************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"6\">");
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
            /************************************************************************************************************
             * 输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/backdaily/default.html");
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
        /// 删除用户张变记录信息
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
            string strList = RequestHelper.GetRequest("DailyID").ToString();
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
                case "sel": strParamter += " and DailyID in (" + strList + ")"; break;
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
            try { DbHelper.Connection.Delete("Fooke_UnionBackDaily", Params: strParamter); }
            catch { }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了回调探针数据ID(" + strParamter + ")"); }
            catch { }
            /**********************************************************************************************************************
             * 返回数据处理结果
             * ********************************************************************************************************************/
            this.History();
            Response.End();
        }
    }
}