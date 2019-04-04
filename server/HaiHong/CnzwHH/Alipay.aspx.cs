using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.IO;
using System.Xml;
using Fooke.Code;
using Fooke.Function;
namespace Fooke.Web.Admin
{
    public partial class Alipay : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "stor": SelectionList(); Response.End(); break;
                case "looker": this.VerificationRole("用户提现"); Looker(); Response.End(); break;
                case "edit": this.VerificationRole("用户提现"); Update(); Response.End(); break;
                case "confirm": this.VerificationRole("用户提现"); SaveConfirm(); Response.End(); break;
                case "dismiss": this.VerificationRole("用户提现"); SaveDismiss(); Response.End(); break;
                case "confirmbetch": this.VerificationRole("用户提现"); SaveConfirmBetch(); Response.End(); break;
                case "dismissbetch": this.VerificationRole("用户提现"); SaveDismissBetch(); Response.End(); break;
                case "del": this.VerificationRole("用户提现"); ; Delete(); Response.End(); break;
                case "computer": this.VerificationRole("用户提现"); strComputer(); Response.End(); break;
                case "default": this.VerificationRole("用户提现"); ; strDefault(); Response.End(); break;
            }
        }

        /// <summary>
        /// 数据统计
        /// </summary>
        protected void strComputer()
        {
            /*****************************************************************************************
             * 数据统计信息
             * ****************************************************************************************/
            DataTable Tab = DbHelper.Connection.ExecuteFindTable("[Stored_FindAlipayComputerTable]", new Dictionary<string, object>() {
                {"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")},
                {"Yesterday",DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00")},
                {"Weekday",FunctionCenter.GetWeekUpOfDate(DateTime.Now,DayOfWeek.Monday,0).ToString("yyyy-MM-dd 00:00:00")}
            });
            /*****************************************************************************************
             * 输出数据统计结果
             * ****************************************************************************************/
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
            string strResponse = Master.Reader("template/Alipay/computer.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strBuilder.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }


        /// <summary>
        /// 快速选择提现订单插件
        /// </summary>
        protected void SelectionList()
        {
            /**********************************************************************************
             * 搜索项需要参数
             * ********************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string Affairs = RequestHelper.GetRequest("Affairs").ToString();
            /*********************************************************************************
            * 开始输出网页信息
            * *******************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"4\">");
            strText.Append("<form action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="orderkey",Text="搜订单号"},
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" size=\"12\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <select name=\"Affairs\">");
            strText.Append("<option value=\"\">搜状态</option>");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="0",Text="待处理"},
                new OptionMode(){Value="1",Text="已提现"},
                new OptionMode(){Value="99",Text="处理中"},
                new OptionMode(){Value="100",Text="已驳回"}
            }, Affairs));
            strText.Append("</select> ");
            strText.Append("<input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"120\">交易号</td>");
            strText.Append("<td>交易用户</td>");
            strText.Append("<td width=\"60\">交易金额</td>");
            strText.Append("<td width=\"60\">状态</td>");
            strText.Append("</tr>");
            /********************************************************************************************
             * 构建分页查询条件
             * ******************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "orderkey": strParameter += " and orderkey like '%" + Keywords + "%'"; break;
                    case "userid": strParameter += " and userid like '%" + Keywords + "%'"; break;
                    case "nickname": strParameter += " and nickname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParameter += " and Addtime>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParameter += " and Addtime<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (!string.IsNullOrEmpty(Affairs) && Affairs.isInt()) { strParameter += " and Affairs=" + Affairs + ""; }
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID != "0") { strParameter += " and UserID=" + UserID + ""; }
            /*****************************************************************************
             * 开始显示分页数据
             * ***************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "AlipayID,OrderKey,FokeMode,BizKey,UserID,Nickname,thisAmount,thisBalance,thisInterval,Addtime,LastDate,strRemark,Affairs";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "AlipayID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " AlipayID Desc";
            PageCenterConfig.Tablename = "Fooke_Alipay";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Alipay", strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*******************************************************************************************
             * 输出网页内容
             * *****************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">", JSONHelper.ToString(Rs));
                strText.AppendFormat("<td>{0}</td>", Rs["OrderKey"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strText.AppendFormat("<td>{0}元</td>", Rs["thisAmount"]);
                strText.AppendFormat("<td>");
                switch (Rs["affairs"].ToString())
                {
                    case "0": strText.Append("<font affairs=\"0\">待处理</font>"); break;
                    case "1": strText.Append("<font affairs=\"1\">已同意</font>"); break;
                    case "99": strText.Append("<font affairs=\"99\">处理中</font>"); break;
                    case "100": strText.Append("<font affairs=\"100\">已驳回</font>"); break;
                }
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /********************************************************************************************
             * 输出数据处理结果
             * ******************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Alipay/stor.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                    case "pagebar": strValue = PageCenter.Often2(Record, 12); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 获取我的银行卡列表信息
        /// </summary>
        protected void strDefault()
        {
            /**********************************************************************************
             * 搜索项需要参数
             * ********************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string Affairs = RequestHelper.GetRequest("Affairs").ToString();
            /*********************************************************************************
             * 开始输出网页信息
             * *******************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"13\">用户提现 >> 提现记录</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"13\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="orderkey",Text="搜订单号"},
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <select name=\"Affairs\">");
            strText.Append("<option value=\"\">搜状态</option>");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="0",Text="待审核"},
                new OptionMode(){Value="1",Text="已提现"},
                new OptionMode(){Value="99",Text="处理中"},
                new OptionMode(){Value="100",Text="已驳回"}
            }, Affairs));
            strText.Append("</select> ");
            strText.Append("<input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            /*********************************************************************************
             * 输出网页列表信息
             * *******************************************************************************/
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">提现用户</td>");
            strText.Append("<td width=\"145\">订单编号</td>");
            strText.Append("<td width=\"70\">提现金额</td>");
            strText.Append("<td width=\"70\">提后余额</td>");
            strText.Append("<td width=\"70\">提现次数</td>");
            strText.Append("<td width=\"120\">提现日期</td>");
            strText.Append("<td width=\"120\">处理日期</td>");
            strText.Append("<td width=\"120\">收款方式</td>");
            strText.Append("<td width=\"120\">账号</td>");
            strText.Append("<td width=\"80\">订单状态</td>");
            strText.Append("<td width=\"80\">操作选项</td>");
            strText.Append("</tr>");
            /********************************************************************************************
             * 构建分页查询条件
             * ******************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "orderkey": strParameter += " and orderkey like '%" + Keywords + "%'"; break;
                    case "userid": strParameter += " and userid like '%" + Keywords + "%'"; break;
                    case "nickname": strParameter += " and nickname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParameter += " and Addtime>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParameter += " and Addtime<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (!string.IsNullOrEmpty(Affairs) && Affairs.isInt()) { strParameter += " and Affairs=" + Affairs + ""; }
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID != "0") { strParameter += " and UserID=" + UserID + ""; }
            /*****************************************************************************
             * 开始显示分页数据
             * ***************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "AlipayID,OrderKey,FokeMode,UserID,Nickname,thisAmount,thisBalance,thisInterval,AccessMode,AccessName,Addtime,LastDate,strRemark,Affairs";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "AlipayID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " AlipayID Desc";
            PageCenterConfig.Tablename = "Fooke_Alipay";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Alipay", strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"AlipayID\" value=\"{0}\" /></td>", Rs["AlipayID"].ToString());
                strText.AppendFormat("<td><a href=\"?action=default&UserID={0}\">{1}</a></td>", Rs["UserID"], Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["OrderKey"].ToString());
                strText.AppendFormat("<td style=\"color:green\">{0}元</td>", Rs["thisAmount"].ToString());
                strText.AppendFormat("<td style=\"color:#cd0000\">{0}元</td>", Rs["thisBalance"].ToString());
                strText.AppendFormat("<td style=\"color:green\">{0}次</td>", Rs["thisInterval"].ToString());
                strText.AppendFormat("<td>{0}</td>", Rs["Addtime"].ToString().cDate().ToString("yyyy-MM-dd HH:mm"));
                strText.AppendFormat("<td>{0}</td>", Rs["LastDate"].ToString().cDate().ToString("yyyy-MM-dd HH:mm"));
                strText.AppendFormat("<td>{0}</td>", Rs["AccessMode"].ToString());
                strText.AppendFormat("<td>{0}</td>", Rs["AccessName"].ToString());
                strText.AppendFormat("<td>");
                switch (Rs["affairs"].ToString())
                {
                    case "0": strText.Append("<font affairs=\"0\">待理处</font>"); break;
                    case "1": strText.Append("<font affairs=\"1\">已同意</font>"); break;
                    case "99": strText.Append("<font affairs=\"99\">处理中</font>"); break;
                    case "100": strText.Append("<font affairs=\"100\">已驳回</font>"); break;
                }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=looker&AlipayID=" + Rs["AlipayID"] + "\" title=\"查看详情\"><img src=\"template/images/ico/chart.png\" /></a>");
                strText.AppendFormat("<a href=\"?action=del&target=sel&AlipayID=" + Rs["AlipayID"] + "\" operate=\"delete\" title=\"删除订单\"><img src=\"template/images/ico/delete.png\" /></a>");
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            /********************************************************************************************
             * 输出分页内容信息
             * ******************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"13\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            /********************************************************************************************
             * 输出处理工具栏信息
             * ******************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"13\">");
            strText.Append("<select name=\"target\">");
            strText.Append("<option value=\"sel\">删除选中的数据</option>");
            strText.Append("<option value=\"days\">删除一天前的记录</option>");
            strText.Append("<option value=\"week\">删除一周前的记录</option>");
            strText.Append("<option value=\"month\">删除一月前的记录</option>");
            strText.Append("<option value=\"byear\">删除半年前的记录</option>");
            strText.Append("<option value=\"year\">删除一年前的记录</option>");
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" class=\"button\" value=\"删除数据\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"confirmbetch\" value=\"同意提现(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"100\" cmdText=\"dismissbetch\" value=\"同意提现(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /********************************************************************************
             * 输出网络数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Alipay/default.html");
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
        /// 查看处理提现申请
        /// </summary>
        protected void Looker()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Alipay/looker.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                { }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 查看交易信息详情
        /// </summary>
        protected void Update()
        {
            string AlipayID = RequestHelper.GetRequest("AlipayID").toInt();
            if (AlipayID == "0") { Response.Write("获取请求参数错误,请选择一个提现订单！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAlipay]", new Dictionary<string, object>() {
                {"AlipayID",AlipayID}
            });
            if (cRs == null) { Response.Write("获取请求数据错误,请刷新重试！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Alipay/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "affairs":
                        switch (cRs["affairs"].ToString())
                        {
                            case "0": strValue = ("<font affairs=\"0\">待处理</font>"); break;
                            case "1": strValue = ("<font affairs=\"1\">已同意</font>"); break;
                            case "99": strValue = ("<font affairs=\"99\">处理中</font>"); break;
                            case "100": strValue = ("<font affairs=\"100\">已驳回</font>"); break;
                        }; break;
                    case "FokeMode":
                        if (cRs["FokeMode"].ToString() == "Bank") { strValue = "银行卡"; }
                        else { strValue = cRs["AccessMode"].ToString(); }; break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 删除用户提现申请信息
        /// </summary>
        protected void Delete()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AlipayID").ToString();
            string strTarget = RequestHelper.GetRequest("target").toString();
            if (string.IsNullOrEmpty(strTarget)) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            if (strTarget == "sel" && string.IsNullOrEmpty(strList)) { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            if (strTarget == "sel" && !string.IsNullOrEmpty(strList))
            {
                if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            }
            /***********************************************************************************************
            * 构建查询删除条件信息
            * *********************************************************************************************/
            string strParamter = string.Empty;
            switch (strTarget.ToLower())
            {
                case "sel": strParamter += " and AlipayID in (" + strList + ") and Affairs in (1,100)"; break;
                case "days": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-1) + "' and Affairs in (1,100)"; break;
                case "week": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-7) + "' and Affairs in (1,100)"; break;
                case "month": strParamter += " and Addtime<='" + DateTime.Now.AddMonths(-1) + "' and Affairs in (1,100)"; break;
                case "byear": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-180) + "' and Affairs in (1,100)"; break;
                case "year": strParamter += " and Addtime<='" + DateTime.Now.AddYears(-1) + "' and Affairs in (1,100)"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            /***********************************************************************************************
            * 开始执行数据删除
            * *********************************************************************************************/
            DbHelper.Connection.Delete("Fooke_Alipay",
                Params: strParamter);
            /***********************************************************************************************
            * 输出数据返回结果
            * *********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 确认提现
        /// </summary>
        protected void SaveConfirm()
        {

            string AlipayID = RequestHelper.GetRequest("AlipayID").toInt();
            if (AlipayID == "0") { this.ErrorMessage("请求参数错误,请至少传入一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAlipay]", new Dictionary<string, object>() {
                {"AlipayID",AlipayID}
            });
            if (cRs == null) { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            else if (cRs["Affairs"].ToString() != "0") { this.ErrorMessage("订单已经处理,不允许重复处理！"); Response.End(); }
            /******************************************************************************************
             * 执行企业付款信息
             * ****************************************************************************************/
            //微信是否线上提现
            string WeIsOnLine = this.GetParameter("WeIsOnLine", "AlipayXml").toInt();
            //支付宝是否线上提现
            string AlIsOnLine = this.GetParameter("AlIsOnLine", "AlipayXml").toInt();
            if (cRs["FokeMode"].ToString() == "支付宝提现")
            {
                try 
                {
                    if (AlIsOnLine == "1")
                    {
                       //线上体现
                       new AliBusinessHelper().SaveTransfer(cRs, this.Configure);
                    }
                    else
                    {
                        //线下提现
                        SaveConfirmUnLine(cRs);
                    }
                }
                catch { }
            }
            else if (cRs["FokeMode"].ToString() == "微信提现")
            {
                try
                {
                    if (WeIsOnLine == "1")
                    {
                        //线上提现
                        string WeChatBusinessID = this.GetParameter("WeChatBusinessID", "AlipayXml").toString();
                        string WeChatAppId = this.GetParameter("WeChatAppId", "AlipayXml").toString();
                        string WeChatBusinessKey = this.GetParameter("WeChatBusinessKey", "AlipayXml").toString();
                        new Fooke.Code.WCBusinessHelper().SaveBusiness(cRs: cRs,
                            BusinessID: WeChatBusinessID,
                            BusinessKey: WeChatBusinessKey,
                            AppID: WeChatAppId);
                    }
                    else
                    {
                        //线下提现
                        SaveConfirmUnLine(cRs);
                    }
                }
                catch { }
            }
            else if (cRs["FokeMode"].ToString() == "兑吧兑换")
            {
                try { this.DuibaAPI(cRs, true); }
                catch { }
            }
            else
            {
                /******************************************************************************************
                * 开始处理请求订单信息
                * ****************************************************************************************/
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAlipayConfirm]", new Dictionary<string, object>() {
                    {"AlipayID",cRs["AlipayID"].ToString()},
                    {"UserID",cRs["UserID"].ToString()}
                });
                if (sRs == null) { this.ErrorMessage("数据处理过程中发生错误,请重试！"); Response.End(); }
            }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("同意了提现请求(" + AlipayID + ")"); }
            catch { }
            /******************************************************************************************
             * 返回处理结果
             * ****************************************************************************************/
            this.ErrorMessage("订单处理成功！", iSuccess: true);
            Response.End();
        }

        protected void SaveConfirmBetch()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AlipayID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            //线上还是线下提现
            string WeIsOnLine = this.GetParameter("WeIsOnLine", "AlipayXml").toInt();
            //支付宝是否线上提现
            string AlIsOnLine = this.GetParameter("AlIsOnLine", "AlipayXml").toInt();
            /***********************************************************************************************
            * 开始查询请求参数信息
            * *********************************************************************************************/
            DataTable thisTab = DbHelper.Connection.FindTable("Fooke_Alipay", Params: " and AlipayID in (" + strList + ") and Affairs=0");
            if (thisTab == null || thisTab.Rows.Count <= 0) { this.ErrorMessage("未获取到需要处理的数据,请重试！"); Response.End(); }
            foreach (DataRow cRs in thisTab.Rows)
            {
                /******************************************************************************************
                * 执行企业付款信息
                * ****************************************************************************************/
                if (cRs["FokeMode"].ToString() == "支付宝提现")
                {
                    try 
                    {
                        if (AlIsOnLine == "1")
                        {
                           //线上体现
                           new AliBusinessHelper().SaveTransfer(cRs, this.Configure); 
                        }
                        else
                        {
                            //线下提现
                            SaveConfirmUnLine(cRs);
                        }
                    }
                    catch { }
                }
                else if (cRs["FokeMode"].ToString() == "微信提现")
                {
                    try
                    {
                        if (WeIsOnLine == "1")
                        {
                            //线上体现
                            string WeChatBusinessID = this.GetParameter("WeChatBusinessID", "AlipayXml").toString();
                            string WeChatAppId = this.GetParameter("WeChatAppId", "AlipayXml").toString();
                            string WeChatBusinessKey = this.GetParameter("WeChatBusinessKey", "AlipayXml").toString();
                            new Fooke.Code.WCBusinessHelper().SaveBusiness(cRs: cRs,
                                BusinessID: WeChatBusinessID,
                                BusinessKey: WeChatBusinessKey,
                                AppID: WeChatAppId);
                        }
                        else
                        {
                            //线下提现
                            SaveConfirmUnLine(cRs);
                        }
                    }
                    catch { }
                }
                else if (cRs["FokeMode"].ToString() == "兑吧兑换")
                {
                    try { this.DuibaAPI(cRs, true); }
                    catch { }
                }
                else
                {
                    DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAlipayConfirm]", new Dictionary<string, object>() {
                        {"AlipayID",cRs["AlipayID"].ToString()},
                        {"UserID",cRs["UserID"].ToString()}
                    });
                    if (sRs == null) { this.ErrorMessage("数据处理过程中发生错误,请重试！"); Response.End(); }
                }
            }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("同意了提现请求(" + strList + ")"); }
            catch { }
            /******************************************************************************************
             * 返回处理结果
             * ****************************************************************************************/
            this.ErrorMessage("订单处理成功！", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 拒绝提现
        /// </summary>
        protected void SaveDismiss()
        {
            string AlipayID = RequestHelper.GetRequest("AlipayID").toInt();
            if (AlipayID == "0") { this.ErrorMessage("请求参数错误,请至少传入一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAlipay]", new Dictionary<string, object>() {
                {"AlipayID",AlipayID}
            });
            if (cRs == null) { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            else if (cRs["Affairs"].ToString() != "0") { this.ErrorMessage("订单已经处理,不允许重复处理！"); Response.End(); }
            /******************************************************************************************
             * 开始处理请求订单信息
             * ****************************************************************************************/
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAlipayDisaccord]", new Dictionary<string, object>() {
                {"AlipayID",cRs["AlipayID"].ToString()},
                {"UserID",cRs["UserID"].ToString()},
                {"Nickname",cRs["Nickname"].ToString()},
                {"thisAmount",cRs["thisAmount"].ToString()},
                {"OrderKey",cRs["OrderKey"].ToString()}
            });
            if (sRs == null) { this.ErrorMessage("数据处理过程中发生错误,请重试！"); Response.End(); }
            else if (sRs["Affairs"].ToString() != "100") { this.ErrorMessage("数据处理过程中发生错误,请重试！"); Response.End(); }
            /******************************************************************************************
             * 兑吧兑换处理结果
             * ****************************************************************************************/
            if (cRs["FokeMode"].ToString() == "兑吧兑换")
            {
                try { this.DuibaAPI(cRs, false); }
                catch { }
            }
            /******************************************************************************************
             * 返回处理结果
             * ****************************************************************************************/
            this.ErrorMessage("订单处理成功！", iSuccess: true);
            Response.End();
        }

        /// <summary>
        /// 批量拒绝提现
        /// </summary>
        protected void SaveDismissBetch()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AlipayID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求参数信息
            * *********************************************************************************************/
            DataTable thisTab = DbHelper.Connection.FindTable("Fooke_Alipay", Params: " and AlipayID in (" + strList + ") and Affairs in (0,99)");
            if (thisTab == null || thisTab.Rows.Count <= 0) { this.ErrorMessage("未获取到需要处理的数据,请重试！"); Response.End(); }
            foreach (DataRow cRs in thisTab.Rows)
            {
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAlipayDisaccord]", new Dictionary<string, object>() {
                    {"AlipayID",cRs["AlipayID"].ToString()},
                    {"UserID",cRs["UserID"].ToString()},
                    {"Nickname",cRs["Nickname"].ToString()},
                    {"thisAmount",cRs["thisAmount"].ToString()},
                    {"OrderKey",cRs["OrderKey"].ToString()}
                });
                if (sRs == null) { this.ErrorMessage("数据处理过程中发生错误,请重试！"); Response.End(); }
                else if (sRs["Affairs"].ToString() != "100") { this.ErrorMessage("数据处理过程中发生错误,请重试！"); Response.End(); }
                /******************************************************************************************
                * 兑吧兑换处理结果
                * ****************************************************************************************/
                if (cRs["FokeMode"].ToString() == "兑吧兑换")
                {
                    try { this.DuibaAPI(cRs, false); }
                    catch { }
                }
            }
            /******************************************************************************************
             * 返回处理结果
             * ****************************************************************************************/
            this.ErrorMessage("订单处理成功！", iSuccess: true);
            Response.End();
        }

        /// <summary>
        /// 兑吧审核接口API
        /// </summary>
        /// <param name="Rs"></param>
        protected bool DuibaAPI(DataRow Rs, bool isTrue = true)
        {
            /***********************************************************************************************************
             * 获取系统参数配置信息
             * *********************************************************************************************************/
            string isDollar = this.GetParameter("isDollar", "alipayXml").toInt();
            if (isDollar != "1") { this.ErrorMessage("兑吧功能已经被管理员关闭！"); Response.End(); }
            string AppKey = this.GetParameter("DollarAppKey", "AlipayXML").toString();
            if (string.IsNullOrEmpty(AppKey)) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            else if (AppKey.Length <= 0) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            else if (AppKey.Length <= 10) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            string AppSecret = this.GetParameter("DollarAppSecret", "AlipayXML").toString();
            if (string.IsNullOrEmpty(AppSecret)) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            else if (AppSecret.Length <= 0) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            else if (AppSecret.Length <= 10) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
            /***********************************************************************************************************
             * 声明数据返回变量
             * *********************************************************************************************************/
            bool iSuccess = false;
            /***********************************************************************************************************
             * 开始执行数据处理
             * *********************************************************************************************************/
            try
            {
                Hashtable hshTable = new Hashtable();
                if (isTrue && Rs != null) { hshTable.Add("passOrderNums", Rs["BizKey"].ToString()); }
                if (!isTrue && Rs != null) { hshTable.Add("rejectOrderNums", Rs["BizKey"].ToString()); }
                string url = Fooke.Code.DuibaHelper.BuildUrlWithSign("http://www.duiba.com.cn/audit/apiAudit"
                    , hshTable, AppKey, AppSecret);
                using (System.Net.WebClient thisClient = new System.Net.WebClient())
                {
                    try { iSuccess = thisClient.DownloadString(url).Contains("errorMessage"); }
                    finally { thisClient.Dispose(); }
                }
            }
            catch { return false; }
            /***********************************************************************************************************
             * 返回数据处理结果信息
             * *********************************************************************************************************/
            return iSuccess;
        }
        /// <summary>
        /// 线下提现
        /// </summary>
        /// <param name="cRs"></param>
        protected void SaveConfirmUnLine(DataRow cRs)
        {
            try
            {
                //同意提现
                DbHelper.Connection.ExecuteFindRow("[Stored_SaveAlipayConfirm]", new Dictionary<string, object>() {
                            {"AlipayID",cRs["AlipayID"].ToString()},
                            {"UserID",cRs["UserID"].ToString()}
                  }); 
            }
            catch (Exception ex) { }
        }
    }
}