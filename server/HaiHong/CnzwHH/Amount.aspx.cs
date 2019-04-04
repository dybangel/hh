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
    public partial class Amount : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "add": this.VerificationRole("账户余额充值"); this.Add(); Response.End(); break;
                case "adddemo": this.VerificationRole("账户余额充值"); this.AddDemo(); Response.End(); break;
                case "save": this.VerificationRole("账户余额充值"); this.AddSave(); Response.End(); break;
                case "stor": this.VerificationRole("余额明细"); SelectedList(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); this.Delete(); Response.End(); break;
                case "computer": this.VerificationRole("余额明细"); strComputer(); Response.End(); break;
                default: this.VerificationRole("余额明细"); this.strDefault(); Response.End(); break;
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
            DataTable Tab = DbHelper.Connection.ExecuteFindTable("[Stored_FindAmountComputerTable]", new Dictionary<string, object>() {
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
            string strResponse = Master.Reader("template/amount/computer.html");
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
        /// 快速查看选择
        /// </summary>
        protected void SelectedList() 
        {
            /*****************************************************************************************************************
             * 获取查询条件
             * ***************************************************************************************************************/
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            string Mode = RequestHelper.GetRequest("Mode").toString();
            /*****************************************************************************************************************
             * 网页内容
             * ***************************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"5\">");
            strText.Append("<form id=\"SearchForm\" OnSubmit=\"return _doPost(this)\" action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<input type=\"hidden\" name=\"userid\" value=\"" + UserID + "\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写要搜索的关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"80\">账变类型</td>");
            strText.Append("<td width=\"80\">账变金额</td>");
            strText.Append("<td width=\"80\">账户余额</td>");
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
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            if (!string.IsNullOrEmpty(Mode)) { Params += " and Mode='" + Mode + "'"; }
            /************************************************************************************************************
             * 获取分页数量信息
             * **********************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ID,UserID,Nickname,Affairs,Mode,Amount,Balance,Addtime,Remark";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "ID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "ID desc";
            PageCenterConfig.Tablename = "Fooke_Amount";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Amount", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /************************************************************************************************************
             * 输出查询内容
             * **********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">",JSONHelper.ToString(Rs));
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td><a href=\"?mode={0}\">{0}</a></td>", Rs["mode"]);
                strText.AppendFormat("<td style=\"color:#{0}\">{1}</td>", (Rs["Affairs"].ToString() == "0" ? "FF0000" : "00CC00"), Rs["Amount"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Balance"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Remark"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /************************************************************************************************************
             * 解析网页模板内容信息
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/amount/stor.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                    case "pagebar": strValue = PageCenter.Often2(Record, PageSize); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
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
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            string FormID = RequestHelper.GetRequest("FormID").toInt("-1");
            string Mode = RequestHelper.GetRequest("Mode").toString();
            /****************************************************************************************
             * 构建网页内容
             * **************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"9\">余额明细 >> 明细记录</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append("<form id=\"SearchForm\" OnSubmit=\"return _doPost(this)\" action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"},
                new OptionMode(){Value="Remark",Text="备注信息"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写要搜索的关键词\" type=\"text\" size=\"15\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <select name=\"mode\">");
            strText.Append("<option value=\"\">明细类型</option>");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="任务奖励",Text="任务奖励"},
                new OptionMode(){Value="任务提成",Text="任务提成"},
                new OptionMode(){Value="级差提成",Text="级差提成"},                
                new OptionMode(){Value="提现退款",Text="提现退款"},
                new OptionMode(){Value="新手红包",Text="新手红包"},
                new OptionMode(){Value="新手奖励",Text="新手奖励"},
                new OptionMode(){Value="邀请奖励",Text="邀请奖励"},
                new OptionMode(){Value="签到红包",Text="签到红包"},
                new OptionMode(){Value="充值",Text="充值"},
                new OptionMode(){Value="扣除",Text="扣除"},
                new OptionMode(){Value="提现",Text="提现"},
                
            }, Mode));
            strText.Append("</select>");
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
            strText.Append("<td width=\"120\">用户昵称</td>");
            strText.Append("<td width=\"100\">变动类型</td>");
            strText.Append("<td width=\"120\">来源用户</td>");
            strText.Append("<td width=\"100\">变动金额</td>");
            strText.Append("<td width=\"100\">账户余额</td>");
            strText.Append("<td width=\"160\">变动日期</td>");
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
                    case "remark": Params += " and Remark like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:000:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            if (FormID != "-1") { Params += " and FormID=" + FormID + ""; }
            if (!string.IsNullOrEmpty(Mode)) { Params += " and Mode='" + Mode + "'"; }
            /************************************************************************************************************
             * 获取分页数量信息
             * **********************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ID,UserID,Nickname,FormID,Formname,Affairs,Mode,Amount,Balance,Addtime,Remark";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "ID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "ID desc";
            PageCenterConfig.Tablename = "Fooke_Amount";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Amount", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /************************************************************************************************************
             * 输出查询内容
             * **********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"Id\" value=\"{0}\" /></td>", Rs["Id"]);
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{0}</a></td>", Rs["userid"]);
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}&StarDate=" + StarDate + "&EndDate=" + EndDate + "\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td><a href=\"?mode={0}&StarDate="+StarDate+"&EndDate="+EndDate+"\">{0}</a></td>", Rs["mode"]);
                strText.AppendFormat("<td><a href=\"?action=default&formid={0}&StarDate=" + StarDate + "&EndDate=" + EndDate + "\">{1}</a></td>", Rs["formid"], Rs["Formname"]);
                strText.AppendFormat("<td style=\"color:#{0}\">{1}</td>", (Rs["Affairs"].ToString() == "0" ? "FF0000" : "00CC00"), Rs["Amount"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Balance"]);
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Remark"]);
                strText.AppendFormat("</tr>");
            }
            /************************************************************************************************************
             * 构建分页控件与操作按钮
             * **********************************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"9\">");
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
            string strResponse = Master.Reader("template/amount/default.html");
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
        /// 充值界面
        /// </summary>
        protected void Add()
        {
            /***********************************************************************************************
            * 输出网页参数内容
            * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/amount/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 充值子页面
        /// </summary>
        protected void AddDemo()
        {
            /***********************************************************************************************
            * 获取用户信息
            * *********************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { Response.Write("请选择要充值的用户！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { Response.Write("获取用户信息,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 解析网页模板内容信息
             * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/amount/addDemo.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
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
            string strList = RequestHelper.GetRequest("ID").ToString();
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
                case "sel": strParamter += " and Id in (" + strList + ")"; break;
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
            try { DbHelper.Connection.Delete("Fooke_Amount", Params: strParamter); }
            catch { }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了余额明细数据ID(" + strParamter + ")"); }
            catch { }
            /**********************************************************************************************************************
             * 返回数据处理结果
             * ********************************************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        protected void AddSave()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("UserID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
             * 判断充值类型数据信息
             * *********************************************************************************************/
            int Affairs = RequestHelper.GetRequest("Affairs").cInt();
            if (Affairs != 0 && Affairs != 1) { this.ErrorMessage("请求参数错误,请选择充值类型！"); Response.End(); }
            /***********************************************************************************************
             * 判断充值金额信息
             * *********************************************************************************************/
            double Amount = RequestHelper.GetRequest("amount").cDouble();
            if (Amount == 0) { this.ErrorMessage("请填写充值金额！"); Response.End(); }
            /***********************************************************************************************
             * 验证描述内容信息
             * *********************************************************************************************/
            string Remark = RequestHelper.GetRequest("Remark").toString();
            string Mode = string.Empty;
            if (Affairs == 0)
            {
                Mode = "扣除";
                Remark = string.Format("管理员扣除{0}", Amount.ToString("0.00"));
            }
            else
            {
                Mode = "充值";
                Remark = string.Format("管理员充值{0}", Amount.ToString("0.00"));
            }
            if (string.IsNullOrEmpty(Remark)) { Remark = string.Format("管理员充值{0}", Amount.ToString("0.00")); }
            if (Remark.Length >= 100) { this.ErrorMessage("备注信息长度请限制在100个字符以内！"); Response.End(); }
            /***********************************************************************************************
             * 查询是否存在用户信息
             * *********************************************************************************************/
            DataTable thisTab = DbHelper.Connection.FindTable(TableCenter.User, columns: "UserID,Nickname", Params: " and UserID in (" + strList + ")");
            if (thisTab == null) { this.ErrorMessage("没有要执行处理的用户！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("没有要执行处理的用户！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据处理信息
             * *********************************************************************************************/
            foreach (DataRow Rs in thisTab.Rows)
            {
                /***********************************************************************************************
                * 生成数据流水单号
                * *********************************************************************************************/
                string strKey = string.Format("用户充值-|-|-{0}-|-|-{1}-|-|-{2}-|-|-管理员充值",
                    Rs["UserID"].ToString(), DateTime.Now.ToString("yyyyMMddHHmmss"), Guid.NewGuid().ToString());
                strKey = new Fooke.Function.String(strKey).ToMD5().Substring(0, 24).ToUpper();
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAmount]", new Dictionary<string, object>() {
                    {"UserID",Rs["UserID"].ToString()},
                    {"Mode",Mode},
                    {"strKey",strKey}
                });
                if (sRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
                /***********************************************************************************************
                * 开始保存数据
                * *********************************************************************************************/
                Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
                thisDictionary["UserID"] = Rs["UserID"].ToString();
                thisDictionary["Nickname"] = Rs["Nickname"].ToString();
                thisDictionary["Mode"] = Mode;
                thisDictionary["Amount"] = Amount;
                thisDictionary["Remark"] = Remark;
                thisDictionary["strKey"] = strKey;
                thisDictionary["Affairs"] = Affairs;
                thisDictionary["FormID"] = Rs["UserID"].ToString();
                thisDictionary["Formname"] = Rs["Nickname"].ToString();
                DbHelper.Connection.ExecuteProc("[Stored_SaveAmount]", thisDictionary);
                /******************************************************************************************
                * 保存充值日志
                * ****************************************************************************************/
                try { SaveOperation("为用户" + Rs["Nickname"] + "(" + Rs["UserID"] + ")充值" + Amount + "元"); }
                catch { }
            };
            /******************************************************************************************
             * 输出数据处理信息
             * ****************************************************************************************/
            this.ErrorMessage("账户充值记录保存成功！", iSuccess: true);
            Response.End();
        }
    }
}