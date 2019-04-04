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
    public partial class Rotary : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "del": this.VerificationRole("转盘抽奖"); this.Delete(); Response.End(); break;
                default: this.VerificationRole("转盘抽奖"); strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 关键词回复列表
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
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            string Mode = RequestHelper.GetRequest("Mode").toString();
            /****************************************************************************************
             * 构建网页内容
             * **************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"7\">转盘抽奖 >> 抽奖记录</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="nickname",Text="搜用户昵称"},
                new OptionMode(){Value="userid",Text="搜用户编号"},
                new OptionMode(){Value="username",Text="搜用户账号"}
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
            strText.Append("<td width=\"60\">用户编号</td>");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"80\">有效下注</td>");
            strText.Append("<td width=\"100\">中奖金额</td>");
            strText.Append("<td width=\"160\">账变日期</td>");
            strText.Append("<td>备注信息</td>");
            strText.Append("</tr>");
            /************************************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "userid": Params += " and UserID=" + Keywords + ""; break;
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                    case "username": Params += " and exists(select userid from fooke_user where userid=fooke_userrotary.userid and username like '%" + Keywords + "%')"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            if (!string.IsNullOrEmpty(Mode)) { Params += " and Mode='" + Mode + "'"; }
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "RotaryID,RotaryKey,UserID,Nickname,Rotaryname,BetAmount,thisAmount,Addtime,Remark";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "RotaryID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "RotaryID desc";
            PageCenterConfig.Tablename = "Fooke_UserRotary";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserRotary", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /************************************************************************************************************
             * 输出查询内容
             * **********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"RotaryID\" value=\"{0}\" /></td>", Rs["RotaryID"]);
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{0}</a></td>", Rs["userid"]);
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td style=\"color:green\">￥ {0} 元</td>", Rs["BetAmount"]);
                strText.AppendFormat("<td style=\"color:#ff0000\">￥ {0} 元</td>", Rs["thisAmount"]);
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Remark"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"7\">");
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
            string strResponse = Master.Reader("template/rotary/default.html");
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
            /**********************************************************************************************
             * 验证请求参数合法性
             * ********************************************************************************************/
            string strTarget = RequestHelper.GetRequest("target").toString("sel");
            string strList = RequestHelper.GetRequest("RotaryID").ToString();
            if (strTarget == "sel")
            {
                if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            }
            /**********************************************************************************************
             * 构建删除条件
             * ********************************************************************************************/
            string strParamter = string.Empty;
            switch (strTarget.ToLower())
            {
                case "sel": strParamter += " and RotaryID in (" + strList + ")"; break;
                case "days": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and Addtime<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and Addtime<='" + DateTime.Now.AddYears(-1) + "'"; break;
                case "all": strParamter += " and 1=1"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_UserRotary", Params: strParamter);
            /**********************************************************************************************
             * 输出返回数据
             * ********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}