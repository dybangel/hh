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
namespace Fooke.Web.Plugin
{
    public partial class File : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                default: strDefault(); Response.End(); break;
                case "stor": strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        protected void strDefault()
        {
            string classId = RequestHelper.GetRequest("classId").toInt();
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StartDate = RequestHelper.GetRequest("StartDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td>");
            strBuilder.Append("<form action=\"?action=default\" method=\"get\">");
            strBuilder.Append("<select name=\"SearchType\">");
            strBuilder.Append("<option " + FunctionCenter.CheckSelectedIndex(SearchType, "title", "selected") + " value=\"title\">搜名称</option>");
            strBuilder.Append("</select>");
            strBuilder.Append("&nbsp;<input type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" size=\"15\" class=\"inputtext\" />");
            strBuilder.Append("&nbsp;日期&nbsp;<input type=\"text\" onClick=\"WdatePicker()\" isDate=\"true\" value=\"" + StartDate + "\" name=\"StartDate\" size=\"10\" class=\"inputtext\" />");
            strBuilder.Append("&nbsp;-&nbsp;<input type=\"text\" onClick=\"WdatePicker()\" isDate=\"true\" value=\"" + EndDate + "\" name=\"EndDate\" size=\"10\" class=\"inputtext\" />");
            strBuilder.Append("&nbsp;<input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td style=\"background:#fff\"><div id=\"frm-list-box\">");
            string Params = string.Empty;
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            { Params += " and fileName='%" + Keywords + "%'"; }
            if (!string.IsNullOrEmpty(StartDate) && VerifyCenter.VerifyDateTime(StartDate)) { Params += " and fileDate>='" + Convert.ToDateTime(StartDate).ToString("yyyyMMdd") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and fileDate<='" + Convert.ToDateTime(EndDate).ToString("yyyyMMdd") + "'"; }
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "Id,fileName,filePath,strKey,fileSize,fileDate";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 18;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Id desc";
            PageCenterConfig.Tablename = TableCenter.Files;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Files, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<a url=\"" + Rs["filePath"] + "\" operate=\"dbclick\">");
                strBuilder.Append("<img src=\"" + Win.ApplicationPath + Rs["filePath"] + "\" />");
                strBuilder.Append("<div>" + Rs["fileName"] + "</div>");
                strBuilder.Append("</a>");
            }
            strBuilder.Append("</div></td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/file/default.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "target": strValue = RequestHelper.GetRequest("target").toString("Thumb"); break;
                    case "list": strValue = strBuilder.ToString(); break;
                    case "pagebar": strValue = PageCenter.Often(Record, 18); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }
}