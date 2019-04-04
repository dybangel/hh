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
                case "default": strDefault(); Response.End(); break;
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
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"7\">选择文件 >> 文件列表</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td colspan=\"7\">");
            strBuilder.Append("<form action=\"?action=default\" method=\"get\">");
            strBuilder.Append("<select name=\"SearchType\">");
            strBuilder.Append("<option " + FunctionCenter.CheckSelectedIndex(SearchType, "title", "selected") + " value=\"title\">搜名称</option>");
            strBuilder.Append("</select>");
            strBuilder.Append("&nbsp;<input type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strBuilder.Append("&nbsp;<input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td>名称</td>");
            strBuilder.Append("<td width=\"12%\">类型</td>");
            strBuilder.Append("<td width=\"12%\">大小(KB)</td>");
            strBuilder.Append("<td width=\"12%\">选项</td>");
            strBuilder.Append("</tr>");
            string Params = string.Empty;
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            { Params += " and advertName='%" + Keywords + "%'"; }
            if (classId != "0") { Params += " and classId=" + classId + ""; }
            if (!string.IsNullOrEmpty(StartDate) && VerifyCenter.VerifyDateTime(StartDate)) { Params += " and Addtime>='" + StartDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate + "'"; }
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "Id,fileName,filePath,strKey,fileSize,fileDate";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Id desc";
            PageCenterConfig.Tablename = TableCenter.Files;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Files, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td>"+Rs["fileName"]+"</td>");
                strBuilder.Append("<td></td>");
                strBuilder.Append("<td>" + Rs["fileSize"] + "</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=del&Id=" + Rs["Id"] + "\"  title=\"删除广告位\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
            }
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td colspan=\"7\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"xingmu\" colspan=\"7\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"设为审核\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"取消审核\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"addjs\" value=\"生成广告\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/file/default.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName) {
                    case "list": strValue = strBuilder.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }
}