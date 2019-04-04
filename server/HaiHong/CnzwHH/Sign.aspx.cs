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
    public partial class Sign : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "del": this.VerificationRole("超级管理员权限"); this.Delete(); Response.End(); break;
                case "delogs": this.VerificationRole("超级管理员权限"); DeleteLogs(); Response.End(); break;
                case "delred": this.VerificationRole("超级管理员权限"); DeleteRed(); Response.End(); break;
                case "list": this.VerificationRole("用户签到"); strList(); Response.End(); break;
                case "red": this.VerificationRole("用户签到"); strRed(); Response.End(); break;
                default: this.VerificationRole("用户签到"); strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 签到红包奖励
        /// </summary>
        protected void strRed()
        {
            /****************************************************************************************
             * 获取查询参数条件信息
             * **************************************************************************************/
            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            /****************************************************************************************
             * 构建网页内容
             * **************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">用户签到 >> 红包奖励</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append("<form action=\"Sign.aspx\" id=\"frmForm\" OnSubmit=\"return _doPost(this);\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"red\" />");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
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
            strText.Append("<td width=\"100\">用户编号</td>");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"100\">抢夺日期</td>");
            strText.Append("<td width=\"100\">获得奖励</td>");
            strText.Append("<td>抢夺时间</td>");
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
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and thisDate>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and thisDate<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "RedID,RedKey,DateKey,UserID,Nickname,Amount,Addtime";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "RedID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "RedID desc";
            PageCenterConfig.Tablename = "Fooke_UserSignRed";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserSignRed", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /************************************************************************************************************
             * 输出查询内容
             * **********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"RedID\" value=\"{0}\" /></td>", Rs["RedID"]);
                strText.AppendFormat("<td><a href=\"?action=list&userid={0}\">{0}</a></td>", Rs["userid"]);
                strText.AppendFormat("<td><a href=\"?action=list&userid={0}\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["DateKey"]);
                strText.AppendFormat("<td style=\"color:green\">￥ {0} 元</td>", Rs["Amount"]);                
                strText.AppendFormat("<td>{0}</td>", Rs["Addtime"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, 10));
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
            strText.Append(" <input type=\"button\" sendText=\"0\" cmdText=\"delred\" class=\"button\" value=\"删除选中\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /************************************************************************************************************
             * 输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/sign/red.html");
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
        /// 签到日志记录
        /// </summary>
        protected void strList()
        {
            /****************************************************************************************
             * 获取查询参数条件信息
             * **************************************************************************************/
            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            /****************************************************************************************
             * 构建网页内容
             * **************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">用户签到 >> 签到日志</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form action=\"Sign.aspx\" id=\"frmForm\" OnSubmit=\"return _doPost(this);\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"list\" />");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
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
            strText.Append("<td width=\"100\">用户编号</td>");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"100\">下注金额</td>");
            strText.Append("<td width=\"100\">获得奖励</td>");
            strText.Append("<td width=\"100\">连续签到赠送</td>");
            strText.Append("<td width=\"160\">签到日期</td>");
            strText.Append("<td>备注说明</td>");
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
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and thisDate>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and thisDate<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "SignID,SignKey,UserID,Nickname,DateKey,ThisDate,BetAmount,SignAmount,RepeatAmount,thisAmount,RepeatNum,Remark";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "SignID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "SignID desc";
            PageCenterConfig.Tablename = "Fooke_UserSignLogs";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserSignLogs", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /************************************************************************************************************
             * 输出查询内容
             * **********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"SignID\" value=\"{0}\" /></td>", Rs["SignID"]);
                strText.AppendFormat("<td><a href=\"?action=list&userid={0}\">{0}</a></td>", Rs["userid"]);
                strText.AppendFormat("<td><a href=\"?action=list&userid={0}\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td style=\"color:green\">￥ {0} 元</td>", Rs["BetAmount"]);
                strText.AppendFormat("<td style=\"color:#ff0000\">￥ {0} 元</td>", Rs["SignAmount"]);
                strText.AppendFormat("<td style=\"color:#ff0000\">￥ {0} 元</td>", Rs["RepeatAmount"]);
                strText.AppendFormat("<td>{0}</td>", Rs["DateKey"]);
                strText.AppendFormat("<td>{0}</td>",Rs["Remark"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<select name=\"target\">");
            strText.Append("<option value=\"sel\">删除选中的数据</option>");
            strText.Append("<option value=\"days\">删除一天前的记录</option>");
            strText.Append("<option value=\"week\">删除一周前的记录</option>");
            strText.Append("<option value=\"month\">删除一月前的记录</option>");
            strText.Append("<option value=\"byear\">删除半年前的记录</option>");
            strText.Append("<option value=\"year\">删除一年前的记录</option>");
            strText.Append("<option value=\"all\">删除所有记录</option>");
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" sendText=\"0\" cmdText=\"delogs\" class=\"button\" value=\"删除选中\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /************************************************************************************************************
             * 输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/sign/list.html");
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
        /// 用户签到列表信息
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
            /****************************************************************************************
             * 构建网页内容
             * **************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">用户签到 >> 签到信息</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form action=\"Sign.aspx\" id=\"frmForm\" OnSubmit=\"return _doPost(this);\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
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
            strText.Append("<td width=\"100\">用户编号</td>");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"100\">获得总额</td>");
            strText.Append("<td width=\"100\">最后签到金额</td>");
            strText.Append("<td width=\"160\">最后签到日期</td>");
            strText.Append("<td width=\"100\">连续签到次数</td>");
            strText.Append("<td>查看明细</td>");
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
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and LastDate>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and LastDate<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UserID,Nickname,AmountTotal,LastAmount,thisAmount,DateKey,LastDate,ThisDate,Repeatnum";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "UserID desc";
            PageCenterConfig.Tablename = "Fooke_UserSign";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserSign", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /************************************************************************************************************
             * 输出查询内容
             * **********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"UserID\" value=\"{0}\" /></td>", Rs["UserID"]);
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{0}</a></td>", Rs["userid"]);
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td style=\"color:green\">￥ {0} 元</td>", Rs["AmountTotal"]);
                strText.AppendFormat("<td style=\"color:#ff0000\">￥ {0} 元</td>", Rs["thisAmount"]);
                strText.AppendFormat("<td>{0}</td>", Rs["LastDate"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Repeatnum"]);
                strText.AppendFormat("<td><input type=\"button\" class=\"button\" value=\"查看明细\" Onclick=\"window.location='?action=list&userid="+Rs["UserID"]+"'\" /></td>");
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(" <input type=\"button\" class=\"button\" value=\"删除选中数据\" onclick=\"deleteOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /************************************************************************************************************
             * 输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/sign/default.html");
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
            string strList = RequestHelper.GetRequest("UserID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /**********************************************************************************************
             * 构建删除条件
             * ********************************************************************************************/
            DbHelper.Connection.Delete("Fooke_UserSign", Params: " and UserID in (" + strList + ")");
            /************************************************************************************************************
             * 保存操作日志
             * **********************************************************************************************************/
            try { SaveOperation("删除了签到记录(" + strList + ")"); }
            catch { }
            /************************************************************************************************************
            * 输出数据处理结果
            * **********************************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除签到日志信息
        /// </summary>
        protected void DeleteLogs()
        {
            /************************************************************************************************************
             * 验证请求参数的合法性
             * **********************************************************************************************************/
            string strList = RequestHelper.GetRequest("SignID").ToString();
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
                case "sel": strParamter += " and SignID in (" + strList + ")"; break;
                case "days": strParamter += " and ThisDate<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and ThisDate<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and ThisDate<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and ThisDate<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and ThisDate<='" + DateTime.Now.AddYears(-1) + "'"; break;
                case "all": strParamter += " and 1=1"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_UserSignLogs", Params: strParamter);
            /************************************************************************************************************
             * 保存操作日志
             * **********************************************************************************************************/
            try { SaveOperation("删除了签到日志记录(" + strParamter + ")"); }
            catch { }
            /************************************************************************************************************
            * 输出数据处理结果
            * **********************************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除红包奖励记录
        /// </summary>
        protected void DeleteRed()
        {
            /************************************************************************************************************
             * 验证请求参数的合法性
             * **********************************************************************************************************/
            string strList = RequestHelper.GetRequest("RedID").ToString();
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
                case "sel": strParamter += " and RedID in (" + strList + ")"; break;
                case "days": strParamter += " and ThisDate<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and ThisDate<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and ThisDate<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and ThisDate<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and ThisDate<='" + DateTime.Now.AddYears(-1) + "'"; break;
                case "all": strParamter += " and 1=1"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_UserSignRed", Params: strParamter);
            /************************************************************************************************************
             * 保存操作日志
             * **********************************************************************************************************/
            try { SaveOperation("删除了签到红包记录(" + strParamter + ")"); }
            catch { }
            /************************************************************************************************************
             * 输出数据处理结果
             * **********************************************************************************************************/
            this.History();
            Response.End();
        }
    }
}