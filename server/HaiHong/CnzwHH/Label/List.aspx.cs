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
    public partial class List : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e){ }
        /// <summary>
        /// 添加管理员
        /// </summary>
        protected void Add()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<form id=\"frm-submit-forms\" action='../label.aspx?action=save' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<input type=\"hidden\" name=\"style\" value=\"3\" />");
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"change\">");
            strText.Append("<td class=\"Base\" colspan=\"4\">标签管理 >> 添加标签 >> 栏目列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">标签名称</td>");
            strText.Append("<td colspan=\"3\" class=\"check_box\">");
            strText.Append(LabelHelper.LabelHeader());
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">调用范围</td>");
            strText.Append("<td width=\"40%\">");
            strText.Append("<select style=\"width:200px\" name=\"ParentID\">");
            strText.Append("<option value=\"-1\">所有栏目(不指定栏目)</option>");
            strText.Append("<option value=\"-2\" style=\"color:#F00;\">当前栏目通用</option>");
            strText.Append("<option value=\"0\">系统根目录</option>");
            strText.Append(ClassHelper.Options(ChannelID: "0", ParentID: "0", defaultTxt: "0"));
            strText.Append("</select>");
            strText.Append("<span>请选择要调用输出的友情连接的栏目</span>");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">排序方式</td>");
            strText.Append("<td>");
            strText.Append("<select name=\"isSort\">");
            strText.Append("<option value=\"0\">单页排序降序</option>");
            strText.Append("<option value=\"1\">单页排序升序</option>");
            strText.Append("<option value=\"2\">排序ID升序</option>");
            strText.Append("<option value=\"3\">排序ID降序</option>");
            strText.Append("</select>");
            strText.Append("<span>不同的排序方式可控制连接先后顺序</span>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">限制条件</td>");
            strText.Append("<td class=\"singlebtn\" colspan=\"3\">");
            strText.Append("<input placeholder=\"格式 and 字段=条件,字符串用单引号,变量请使用{@变量名/}\" type=\"text\" size=\"60\" class=\"inputtext\" name=\"ParameterText\" />");
            strText.Append("<label><input type=\"radio\" name=\"isParameter\" value=\"1\" />开启</label>");
            strText.Append("<label class=\"current\"><input type=\"radio\" checked name=\"isParameter\" value=\"0\" />关闭</label>");
            strText.Append("<span>限制条件可更灵活的调用指定数据,需要使用时请开启.</span>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">调用数量</td>");
            strText.Append("<td>");
            strText.Append("<input type=\"text\" size=\"12\" name=\"topNumber\" value=\"8\" class=\"inputtext center\" />");
            strText.Append("<span>请设置调用友情连接的数量0表示不限制数量</span>");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">导读字数</td>");
            strText.Append("<td>");
            strText.Append("<input type=\"text\" size=\"12\" name=\"descNumber\" value=\"0\" class=\"inputtext center\" />");
            strText.Append("<span>将截取单页面导读字数，0为不限制长度</span>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">选择字段</td>");
            strText.Append("<td class=\"showcolumns\" colspan=\"3\">");
            strText.Append(LabelHelper.ListColumns());
            strText.Append("</td>");
            strText.Append("</tr>");
            string LabelContent = "<a href=\"{$classurl/}\">{$classname/}</a>";
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\" valign=\"top\">标签内容</td>");
            strText.Append("<td class=\"singlebtn\" colspan=\"3\">");
            strText.Append("<textarea class=\"inputtext\" name=\"loopTxt\" id=\"LabelContent\">" + LabelContent + "</textarea>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td class=\"tips\"></td>");
            strText.Append("<td colspan=\"3\">");
            strText.Append("<input type=\"submit\" id=\"frm-submit-btns\" value=\"确认保存\" class=\"button\"/>");
            strText.Append("<input type=\"button\" value=\"返回上页\" onclick=\"history.go(-1);\" class=\"button\"/>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"attention\" colspan=\"4\">");
            strText.Append(LabelHelper.Attention());
            strText.Append("</font>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            Response.Write(strText.ToString());
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        protected void Update() {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindLabel", new Dictionary<string, object>() {
                {"Id",Id}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            ConfigurationReader xReader = new ConfigurationReader(cRs["strXML"].ToString());
            StringBuilder strText = new StringBuilder();
            strText.Append("<form id=\"frm-submit-forms\" action='../label.aspx?action=editsave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<input type=\"hidden\" name=\"style\" value=\"3\" />");
            strText.Append("<input type=\"hidden\" name=\"Id\" value=\""+cRs["Id"]+"\" />");
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"change\">");
            strText.Append("<td class=\"Base\" colspan=\"4\">标签管理 >> 编辑标签 >> 栏目列表<</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">标签名称</td>");
            strText.Append("<td colspan=\"3\" class=\"check_box\">");
            strText.Append(LabelHelper.LabelHeader(cRs["labelName"].ToString(), cRs["isValid"].ToString(), cRs["isDelay"].ToString(), cRs["classId"].ToString()));
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">调用范围</td>");
            strText.Append("<td width=\"40%\">");
            string ParentID = xReader.GetParameter("ParentID").toInt();
            strText.Append("<select style=\"width:200px\" name=\"ParentID\">");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(ParentID,"-1","selected") + " value=\"-1\">所有页面(不指定页面)</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(ParentID, "-2", "selected") + " value=\"-2\" style=\"color:#F00;\">当前页面通用</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(ParentID, "0", "selected") + " value=\"0\">系统根目录</option>");
            strText.Append(ClassHelper.Options(ChannelID: "0", ParentID: "0", defaultTxt: ParentID));
            strText.Append("</select>");
            strText.Append("<span>请选择要调用输出的友情连接的栏目</span>");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">排序方式</td>");
            strText.Append("<td>");
            strText.Append("<select name=\"isSort\">");
            string isSort = xReader.GetParameter("isSort").toInt();
            strText.Append("<option value=\"0\">单页排序降序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort,"1","selected") + " value=\"1\">单页排序升序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort, "2", "selected") + " value=\"2\">排序ID升序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort, "3", "selected") + " value=\"3\">排序ID降序</option>");
            strText.Append("</select>");
            strText.Append("<span>不同的排序方式可控制连接先后顺序</span>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">限制条件</td>");
            strText.Append("<td class=\"singlebtn\" colspan=\"3\">");
            string ParameterText = xReader.GetParameter("ParameterText").toString();
            string isParameter = xReader.GetParameter("isParameter").toInt();
            strText.Append("<input placeholder=\"格式 and 字段=条件,字符串用单引号,变量请使用{@变量名/}\" type=\"text\" size=\"60\" class=\"inputtext\" value=\"" + ParameterText + "\" name=\"ParameterText\" />");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isParameter, "1", "class=\"current\"") + "><input type=\"radio\" " + FunctionCenter.CheckSelectedIndex(isParameter, "1") + " name=\"isParameter\" value=\"1\" />开启</label>");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isParameter, "0", "class=\"current\"") + "><input type=\"radio\" " + FunctionCenter.CheckSelectedIndex(isParameter, "0") + " name=\"isParameter\" value=\"0\" />关闭</label>");
            strText.Append("<span>限制条件可更灵活的调用指定数据,需要使用时请开启.</span>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">调用数量</td>");
            strText.Append("<td>");
            string topNumber = xReader.GetParameter("topNumber").toString();
            strText.Append("<input type=\"text\" size=\"12\" name=\"topNumber\" value=\"" + topNumber + "\" class=\"inputtext center\" />");
            strText.Append("<span>请设置调用友情连接的数量0表示不限制数量</span>");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">导读字数</td>");
            strText.Append("<td>");
            string descNumber = xReader.GetParameter("descNumber").toString();
            strText.Append("<input type=\"text\" size=\"12\" name=\"descNumber\" value=\"" + descNumber + "\" class=\"inputtext center\" />");
            strText.Append("<span>将截取单页面导读字数，0为不限制长度</span>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">选择字段</td>");
            strText.Append("<td class=\"showcolumns\" colspan=\"3\">");
            strText.Append(LabelHelper.ListColumns());
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\" valign=\"top\">标签内容</td>");
            strText.Append("<td class=\"singlebtn\" colspan=\"3\">");
            strText.Append("<textarea class=\"inputtext\" name=\"loopTxt\" id=\"LabelContent\">" + cRs["LoopTxt"] + "</textarea>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td class=\"tips\"></td>");
            strText.Append("<td colspan=\"3\">");
            strText.Append("<input type=\"submit\" id=\"frm-submit-btns\" value=\"确认保存\" class=\"button\"/>");
            strText.Append("<input type=\"button\" value=\"返回上页\" onclick=\"history.go(-1);\" class=\"button\"/>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"attention\" colspan=\"4\">");
            strText.Append(LabelHelper.Attention());
            strText.Append("</font>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            Response.Write(strText.ToString());
        }
    }
}