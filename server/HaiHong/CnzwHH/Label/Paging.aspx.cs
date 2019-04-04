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
    public partial class Paging : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e){ }
        /// <summary>
        /// 添加管理员
        /// </summary>
        protected void Add()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<form id=\"frm-submit-forms\" action='../label.aspx?action=save' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<input type=\"hidden\" name=\"style\" value=\"2\" />");
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"change\">");
            strText.Append("<td class=\"Base\" colspan=\"4\">标签管理 >> 添加标签 >> 分页函数</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">标签名称</td>");
            strText.Append("<td colspan=\"3\" class=\"check_box\">");
            strText.Append(LabelHelper.LabelHeader());
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">选择模型</td>");
            strText.Append("<td class=\"check_box\" width=\"40%\">");
            strText.Append("<select name=\"ChannelID\">");
            strText.Append("<option value=\"0\">请选择标签调用模型</option>");
            strText.Append(ChannelHelper.Options("0"));
            strText.Append("</select>");
            strText.Append("&nbsp;<label><input type=\"checkbox\" name=\"isAll\" value=\"1\" />所有字段</label>");
            strText.Append("<span style=\"color:#F00\">请选择标签调用的模型</span>");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">选择分类</td>");
            strText.Append("<td id=\"class_box\">");
            strText.Append("<select style=\"width:220px\" name=\"classId\">");
            strText.Append("<option value=\"0\" style=\"color:#F00\">自适应当前栏目</option>");
            strText.Append("</select>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">排序方式</td>");
            strText.Append("<td width=\"40%\">");
            strText.Append("<select name=\"isSort\">");
            strText.Append("<option value=\"0\">文档降序</option>");
            strText.Append("<option value=\"1\">文档升序</option>");
            strText.Append("<option value=\"2\">日期降序</option>");
            strText.Append("<option value=\"3\">日期升序</option>");
            strText.Append("<option value=\"4\">随机排序</option>");
            strText.Append("<option value=\"5\">点击率降序</option>");
            strText.Append("<option value=\"6\">推荐指数降序</option>");
            strText.Append("<option value=\"7\">点赞指数降序</option>");
            strText.Append("</select>");
            strText.Append("<span>排序方式决定文档显示顺序</span>");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">属性控制</td>");
            strText.Append("<td class=\"check_box\">");
            strText.Append("<label><input type=\"checkbox\" name=\"isTop\" value=\"1\" />头条</label>");
            strText.Append("<label><input type=\"checkbox\" name=\"isRecommend\" value=\"1\" />推荐</label>");
            strText.Append("<label><input type=\"checkbox\" name=\"isHot\" value=\"1\" />热门</label>");
            strText.Append("<label><input type=\"checkbox\" name=\"isBook\" value=\"1\" />评论</label>");
            strText.Append("<label><input type=\"checkbox\" name=\"isUrl\" value=\"1\" />转向</label>");
            strText.Append("<label><input type=\"checkbox\" name=\"isPic\" value=\"1\" />图片</label>");
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
            strText.Append("<td width=\"40%\">");
            strText.Append("<input type=\"text\" size=\"8\" name=\"topNumber\" value=\"8\" class=\"inputtext center\" />");
            strText.Append("&nbsp;标题字数∶<input type=\"text\" size=\"8\" name=\"titleNumber\" value=\"30\" class=\"inputtext center\" />");
            strText.Append("&nbsp;导读字数∶<input type=\"text\" size=\"8\" name=\"descNumber\" value=\"60\" class=\"inputtext center\" />");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">日期格式</td>");
            strText.Append("<td class=\"check_box\">");
            string Year = DateTime.Now.Year.ToString("0000");
            string Month = DateTime.Now.Month.ToString("00");
            string Day = DateTime.Now.Day.ToString("00");
            strText.Append("<select name=\"fromatDate\">");
            strText.Append("<option value=\"yyyy-MM-dd HH:mm:ss\">无格式</option>");
            strText.Append("<option value=\"yyyy-MM-dd\">" + Year + "-" + Month + "-"+Day+"</option>");
            strText.Append("<option value=\"yyyy/MM/dd\">" + Year + "/" + Month + "/" + Day + "</option>");
            strText.Append("<option value=\"yyyy年MM月dd日\">" + Year + "年" + Month + "月" + Day + "日</option>");
            strText.Append("<option value=\"yyyy-MM\">" + Year + "-" + Month + "</option>");
            strText.Append("<option value=\"yyyy/MM\">" + Year + "/" + Month + "</option>");
            strText.Append("<option value=\"yyyy年MM月\">" + Year + "年" + Month + "月</option>");
            strText.Append("<option value=\"MM-dd\">" + Month + "-" + Day + "</option>");
            strText.Append("<option value=\"MM/dd\">" + Month + "/" + Day + "</option>");
            strText.Append("<option value=\"MM月dd日\">" + Month + "月" + Day + "日</option>");
            strText.Append("</select>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">分页样式</td>");
            strText.Append("<td colspan=\"3\">");
            strText.Append("<select name=\"PageStyle\">");
            strText.Append("<option value=\"0\">共N页,当前第N页 首页 上一页 下一页 尾页</option>");
            strText.Append("<option value=\"1\">首页 上一页 1 2 3 4 …… 下一页 尾页</option>");
            strText.Append("<option value=\"2\">1 2 3 4 5 6 7 8 9 N</option>");
            strText.Append("<option value=\"3\">first previous 1 2 3 4 5 6 7 8 9 next last</option>");
            strText.Append("<option value=\"4\">the page of N,this page N first previous next last</option>");
            strText.Append("</select>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">选择字段</td>");
            strText.Append("<td class=\"showcolumns\" colspan=\"3\">");
            strText.Append(LabelHelper.PagingColumns(""));
            strText.Append("</td>");
            strText.Append("</tr>");

            string LabelContent = "<li><a href=\"{$linkurl/}\">{$title/}</a></li>";

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
            /********************************************************
             * 保存数据
             * ******************************************************/
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
            string ChannelID = xReader.GetParameter("channelid").toInt();
           
            
            StringBuilder strText = new StringBuilder();
            strText.Append("<form id=\"frm-submit-forms\" action='../label.aspx?action=editsave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<input type=\"hidden\" name=\"style\" value=\"2\" />");
            strText.Append("<input type=\"hidden\" name=\"Id\" value=\"" + Id + "\" />");
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"change\">");
            strText.Append("<td class=\"Base\" colspan=\"4\">标签管理 >> 编辑标签 >> 分页函数</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">标签名称</td>");
            strText.Append("<td colspan=\"3\" class=\"check_box\">");
            strText.Append(LabelHelper.LabelHeader(cRs["labelName"].ToString(), cRs["isValid"].ToString(), cRs["isDelay"].ToString(), cRs["classId"].ToString()));
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">选择模型</td>");
            strText.Append("<td class=\"check_box\" width=\"40%\">");
            strText.Append("<select name=\"ChannelID\">");
            strText.Append("<option value=\"0\">请选择标签调用模型</option>");
            strText.Append(ChannelHelper.Options(ChannelID));
            strText.Append("</select>");
            string isAll = xReader.GetParameter("isAll").toInt();
            strText.Append("&nbsp;<label " + FunctionCenter.CheckSelectedIndex(isAll, "1", "class=\"current\"") + "><input type=\"checkbox\" " + FunctionCenter.CheckSelectedIndex(isAll, "1") + " name=\"isAll\" value=\"1\" />所有字段</label>");
            strText.Append("<span>请选择标签调用的模型</span>");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">选择分类</td>");
            strText.Append("<td id=\"class_box\">");
            string classId = xReader.GetParameter("classId").toInt();
            strText.Append("<select style=\"width:220px\" name=\"classId\">");
            strText.Append("<option value=\"0\" style=\"color:#F00\">自适应当前栏目</option>");
            if (ChannelID != "0") { strText.Append(ClassHelper.Options(ChannelID, "0", classId)); }
            strText.Append("</select>");
            strText.Append("</td>");
            strText.Append("</tr>");
            string isSort = xReader.GetParameter("isSort").toInt();
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">排序方式</td>");
            strText.Append("<td width=\"40%\">");
            strText.Append("<select name=\"isSort\">");
            strText.Append("<option value=\"0\">文档降序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort,"1","selected") + " value=\"1\">文档升序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort, "2", "selected") + "  value=\"2\">日期降序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort, "3", "selected") + "  value=\"3\">日期升序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort, "4", "selected") + "  value=\"4\">随机排序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort, "5", "selected") + "  value=\"5\">点击率降序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort, "6", "selected") + "  value=\"6\">推荐指数降序</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(isSort, "7", "selected") + "  value=\"7\">点赞指数降序</option>");
            strText.Append("</select>");
            strText.Append("<span>排序方式决定文档显示顺序</span>");
            strText.Append("</td>");
            string isTop = xReader.GetParameter("isTop").toInt();
            string isRecommend = xReader.GetParameter("isRecommend").toInt();
            string isHot = xReader.GetParameter("isHot").toInt();
            string isBook = xReader.GetParameter("isBook").toInt();
            string isUrl = xReader.GetParameter("isUrl").toInt();
            string isPic = xReader.GetParameter("isPic").toInt();
            strText.Append("<td class=\"tips\">属性控制</td>");
            strText.Append("<td class=\"check_box\">");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isTop,"1","class=\"current\"") + "><input type=\"checkbox\" "+FunctionCenter.CheckSelectedIndex(isTop,"1")+" name=\"isTop\" value=\"1\" />头条</label>");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isRecommend, "1", "class=\"current\"") + "><input type=\"checkbox\" " + FunctionCenter.CheckSelectedIndex(isRecommend, "1") + " name=\"isRecommend\" value=\"1\" />推荐</label>");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isHot, "1", "class=\"current\"") + "><input type=\"checkbox\" " + FunctionCenter.CheckSelectedIndex(isHot, "1") + " name=\"isHot\" value=\"1\" />热门</label>");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isBook, "1", "class=\"current\"") + "><input type=\"checkbox\" " + FunctionCenter.CheckSelectedIndex(isBook, "1") + " name=\"isBook\" value=\"1\" />评论</label>");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isUrl, "1", "class=\"current\"") + "><input type=\"checkbox\" " + FunctionCenter.CheckSelectedIndex(isUrl, "1") + " name=\"isUrl\" value=\"1\" />转向</label>");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isPic, "1", "class=\"current\"") + "><input type=\"checkbox\" " + FunctionCenter.CheckSelectedIndex(isPic, "1") + " name=\"isPic\" value=\"1\" />图片</label>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">限制条件</td>");
            strText.Append("<td class=\"singlebtn\" colspan=\"3\">");
            strText.Append("<input placeholder=\"格式 and 字段=条件,字符串用单引号,变量请使用{@变量名/}\" type=\"text\" size=\"60\" value=\"" + cRs["ParameterText"] + "\" class=\"inputtext\" name=\"ParameterText\" />");
            string isParameter = cRs["isParameter"].ToString();
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isParameter, "1", "class=\"current\"") + "><input type=\"radio\" " + FunctionCenter.CheckSelectedIndex(isParameter, "1") + " name=\"isParameter\" value=\"1\" />开启</label>");
            strText.Append("<label " + FunctionCenter.CheckSelectedIndex(isParameter, "0", "class=\"current\"") + "><input type=\"radio\"  " + FunctionCenter.CheckSelectedIndex(isParameter, "0") + " name=\"isParameter\" value=\"0\" />关闭</label>");
            strText.Append("<span>限制条件可更灵活的调用指定数据,需要使用时请开启.</span>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">调用数量</td>");
            strText.Append("<td width=\"40%\">");
            strText.Append("<input type=\"text\" size=\"8\" name=\"topNumber\" value=\""+xReader.GetParameter("topNumber").toInt()+"\" class=\"inputtext center\" />");
            strText.Append("&nbsp;标题字数∶<input type=\"text\" size=\"8\" name=\"titleNumber\" value=\"" + xReader.GetParameter("titleNumber").toInt() + "\" class=\"inputtext center\" />");
            strText.Append("&nbsp;导读字数∶<input type=\"text\" size=\"8\" name=\"descNumber\" value=\"" + xReader.GetParameter("descNumber").toInt() + "\" class=\"inputtext center\" />");
            strText.Append("</td>");
            strText.Append("<td class=\"tips\">日期格式</td>");
            strText.Append("<td class=\"check_box\">");
            string Year = DateTime.Now.Year.ToString("0000");
            string Month = DateTime.Now.Month.ToString("00");
            string Day = DateTime.Now.Day.ToString("00");
            string fromatDate = xReader.GetParameter("fromatDate").toString();
            strText.Append("<select name=\"fromatDate\">");
            strText.Append("<option value=\"" + fromatDate + "\">" + fromatDate + "</option>");
            strText.Append("<option value=\"yyyy-MM-dd HH:mm:ss\">无格式</option>");
            strText.Append("<option value=\"yyyy-MM-dd\">" + Year + "-" + Month + "-" + Day + "</option>");
            strText.Append("<option value=\"yyyy/MM/dd\">" + Year + "/" + Month + "/" + Day + "</option>");
            strText.Append("<option value=\"yyyy年MM月dd日\">" + Year + "年" + Month + "月" + Day + "日</option>");
            strText.Append("<option value=\"yyyy-MM\">" + Year + "-" + Month + "</option>");
            strText.Append("<option value=\"yyyy/MM\">" + Year + "/" + Month + "</option>");
            strText.Append("<option value=\"yyyy年MM月\">" + Year + "年" + Month + "月</option>");
            strText.Append("<option value=\"MM-dd\">" + Month + "-" + Day + "</option>");
            strText.Append("<option value=\"MM/dd\">" + Month + "/" + Day + "</option>");
            strText.Append("<option value=\"MM月dd日\">" + Month + "月" + Day + "日</option>");
            strText.Append("</select>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">分页样式</td>");
            strText.Append("<td colspan=\"3\">");
            string PageStyle = xReader.GetParameter("PageStyle").toInt();
            strText.Append("<select name=\"PageStyle\">");
            strText.Append("<option value=\"0\">共N页,当前第N页 首页 上一页 下一页 尾页</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(PageStyle,"1","selected") + " value=\"1\">首页 上一页 1 2 3 4 …… 下一页 尾页</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(PageStyle, "2", "selected") + " value=\"2\">1 2 3 4 5 6 7 8 9 N</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(PageStyle, "3", "selected") + " value=\"3\">first previous 1 2 3 4 5 6 7 8 9 next last</option>");
            strText.Append("<option " + FunctionCenter.CheckSelectedIndex(PageStyle, "4", "selected") + " value=\"4\">the page of N,this page N first previous next last</option>");
            strText.Append("</select>");
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"tips\">选择字段</td>");
            strText.Append("<td class=\"showcolumns\" colspan=\"3\">");
            strText.Append(LabelHelper.PagingColumns(""));
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
            /********************************************************
             * 保存数据
             * ******************************************************/
            strText.Append("</form>");
            Response.Write(strText.ToString());
        }
    }
}