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
    public partial class Quiescent :Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e){ }
        /// <summary>
        /// 添加标签
        /// </summary>
        protected void Add()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<form id=\"frm-submit-forms\" action='../label.aspx?action=save' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<input type=\"hidden\" name=\"style\" value=\"5\" />");
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"change\">");
            strText.Append("<td class=\"Base\" colspan=\"4\">标签管理 >> 添加标签 >> 静态标签</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">标签名称</td>");
            strText.Append("<td colspan=\"3\" class=\"check_box\">");
            strText.Append(LabelHelper.LabelHeader());
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\" valign=\"top\">标签内容</td>");
            strText.Append("<td class=\"singlebtn\" colspan=\"3\">");
            strText.Append("<textarea class=\"inputtext\" style=\"height:300px\" name=\"loopTxt\" placeholder=\"请输入静态标签内容,支持标签嵌套,不允许嵌套自身\" id=\"LabelContent\"></textarea>");
            strText.Append("<br/>静态标签为纯html标签，支持标签嵌套！");
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
            /********************************************************
             * 保存数据
             * ******************************************************/
            strText.Append("</form>");
            Response.Write(strText.ToString());
        }
        /// <summary>
        /// 编辑静态标签
        /// </summary>
        protected void Update() {

            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindLabel", new Dictionary<string, object>() {
                {"Id",Id}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }

            StringBuilder strText = new StringBuilder();
            strText.Append("<form id=\"frm-submit-forms\" action='../label.aspx?action=editsave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<input type=\"hidden\" name=\"style\" value=\"5\" />");
            strText.Append("<input type=\"hidden\" name=\"Id\" value=\""+cRs["Id"]+"\" />");
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"change\">");
            strText.Append("<td class=\"Base\" colspan=\"4\">添加标签</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">标签名称</td>");
            strText.Append("<td colspan=\"3\" class=\"check_box\">");
            strText.Append(LabelHelper.LabelHeader(cRs["labelName"].ToString(), cRs["isValid"].ToString(), cRs["isDelay"].ToString(), cRs["classId"].ToString()));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\" valign=\"top\">标签内容</td>");
            strText.Append("<td class=\"singlebtn\" colspan=\"3\">");
            strText.Append("<textarea class=\"inputtext\" style=\"height:300px\" name=\"loopTxt\" placeholder=\"请输入静态标签内容,支持标签嵌套,不允许嵌套自身\" id=\"LabelContent\">" + cRs["LoopTxt"] + "</textarea>");
            strText.Append("<br/>静态标签为纯html标签，支持标签嵌套！");
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
            /********************************************************
             * 保存数据
             * ******************************************************/
            strText.Append("</form>");
            Response.Write(strText.ToString());
        }

    }
}