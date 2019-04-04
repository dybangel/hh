using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using Fooke.Code;
using Fooke.Function;
namespace Fooke.Web.Admin
{
    public partial class Duiba : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "stor": StorAlipay(); Response.End(); break;
                case "looker": this.VerificationRole("兑吧商城"); Looker(); Response.End(); break;
                case "edit": this.VerificationRole("兑吧商城"); Update(); Response.End(); break;
                case "editsave": this.VerificationRole("兑吧商城"); SaveUpdate(); Response.End(); break;
                case "default": this.VerificationRole("兑吧商城"); ; strDefault(); Response.End(); break;
                case "del": this.VerificationRole("兑吧商城"); ; Delete(); Response.End(); break;
                case "affaris": this.VerificationRole("兑吧商城"); SaveAffaris(); Response.End(); break;
            }
        }
        /// <summary>
        /// 快速选择提现订单插件
        /// </summary>
        protected void StorAlipay()
        {
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string Affairs = RequestHelper.GetRequest("Affairs").toInt("-1");
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"4\">");
            strText.Append("<form action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="nickname",Text="搜用户"},
                new OptionMode(){Value="thiskey",Text="搜单号"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append("&nbsp;<input placeholder=\"请输入搜索关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append("&nbsp;<input type=\"submit\" value=\"快速查找\" class=\"button\" />");
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
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "thiskey": strParameter += " and thiskey like '%" + Keywords + "%'"; break;
                    case "nickname": strParameter += " and nickname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParameter += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParameter += " and Addtime<='" + EndDate + "'"; }
            if (Affairs != "-1") { strParameter += " and Affairs=" + Affairs + ""; }
            /*****************************************************************************
             * 开始显示分页数据
             * ***************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ID,UserID,Nickname,thisKey,Addtime,Affairs,Remark,Amount";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Id Desc";
            PageCenterConfig.Tablename = TableCenter.Duiba;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Duiba, strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                StringBuilder strXml = new StringBuilder();
                strXml.Append("{");
                strXml.Append("\"auto\":\"0\"");
                foreach (DataColumn col in Tab.Columns)
                {
                    strXml.Append(",\"" + col.ColumnName.ToLower() + "\":\"" + Rs[col.ColumnName] + "\"");
                }
                strXml.Append("}");
                strText.Append("<tr operate=\"selector\" json=\'" + strXml.ToString() + "\' class=\"hback\">");
                strText.Append("<td>" + Rs["thisKey"] + "</td>");
                strText.Append("<td>" + Rs["nickname"] + "</td>");
                strText.Append("<td>￥" + Rs["Amount"] + "元</td>");
                strText.Append("<td>");
                switch (Rs["affairs"].ToString())
                {
                    case "0": strText.Append("<font affairs=\"0\">申请中</font>"); break;
                    case "1": strText.Append("<font affairs=\"1\">已兑换</font>"); break;
                    case "99": strText.Append("<font affairs=\"99\">已处理</font>"); break;
                    case "100": strText.Append("<font affairs=\"100\">已作废</font>"); break;
                }
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/duiba/stor.html");
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
            string Affairs = RequestHelper.GetRequest("Affairs").toInt("-1");
            /*********************************************************************************
             * 开始输出网页信息
             * *******************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">兑吧兑换 >> 兑换记录</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="thiskey",Text="搜单号"},
                new OptionMode(){Value="nickname",Text="搜用户"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append("&nbsp;<input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append("&nbsp;查询日期&nbsp;<input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append("&nbsp;到&nbsp;<input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append("&nbsp;<input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("&nbsp;<a href=\"?action=default\">查看全部</a>");
            strText.Append("&nbsp;<a href=\"?action=default&affairs=0\" " + FunctionCenter.CheckSelectedIndex(Affairs, "0", "class=\"highlig\"") + ">申请中</a>");
            strText.Append("&nbsp;<a href=\"?action=default&affairs=1\" " + FunctionCenter.CheckSelectedIndex(Affairs, "1", "class=\"highlig\"") + ">已兑换</a>");
            strText.Append("&nbsp;<a href=\"?action=default&affairs=100\" " + FunctionCenter.CheckSelectedIndex(Affairs, "100", "class=\"highlig\"") + ">作废订单</a>");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">兑换用户</td>");
            strText.Append("<td width=\"80\">兑换积分</td>");
            strText.Append("<td width=\"80\">订单号</td>");
            strText.Append("<td width=\"120\">兑换日期</td>");
            strText.Append("<td>交易单号</td>");
            strText.Append("<td width=\"80\">兑换状态</td>");
            strText.Append("<td width=\"80\">操作选项</td>");
            strText.Append("</tr>");
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "thiskey": strParameter += " and thiskey like '%" + Keywords + "%'"; break;
                    case "nickname": strParameter += " and nickname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParameter += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParameter += " and Addtime<='" + EndDate + "'"; }
            if (Affairs != "-1") { strParameter += " and Affairs=" + Affairs + ""; }
            /*****************************************************************************
             * 开始显示分页数据
             * ***************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ID,UserID,Nickname,thisKey,Addtime,Affairs,Remark,Amount";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Id Desc";
            PageCenterConfig.Tablename = TableCenter.Duiba;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Duiba, strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"ID\" value=\"" + Rs["ID"] + "\" /></td>");
                strText.Append("<td><a href=\"?UserID=" + Rs["UserID"] + "\">" + Rs["Nickname"] + "</a></td>");
                strText.Append("<td>" + Rs["Amount"] + "</td>");
                strText.Append("<td>" + Rs["thisKey"] + "</td>");
                strText.Append("<td>" + Rs["addtime"] + "</td>");
                strText.Append("<td>" + Rs["remark"] + "</td>");
                strText.Append("<td>");
                switch (Rs["affairs"].ToString()) {
                    case "0": strText.Append("<font affairs=\"0\">申请中</font>"); break;
                    case "1": strText.Append("<font affairs=\"1\">已兑换</font>"); break;
                    case "99": strText.Append("<font affairs=\"99\">已处理</font>"); break;
                    case "100": strText.Append("<font affairs=\"100\">已作废</font>"); break;
                }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=looker&Id=" + Rs["Id"] + "\" title=\"查看详情\"><img src=\"template/images/ico/chart.png\" /></a>");
                strText.Append("<a href=\"?action=del&Id=" + Rs["Id"] + "\" operate=\"delete\" title=\"删除订单\"><img src=\"template/images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }

            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"8\">");
            strText.Append("<select name=\"target\">");
            strText.Append("<option value=\"sel\">删除选中的数据</option>");
            strText.Append("<option value=\"days\">删除一天前的记录</option>");
            strText.Append("<option value=\"week\">删除一周前的记录</option>");
            strText.Append("<option value=\"month\">删除一月前的记录</option>");
            strText.Append("<option value=\"byear\">删除半年前的记录</option>");
            strText.Append("<option value=\"year\">删除一年前的记录</option>");
            strText.Append("</select>");
            strText.Append("&nbsp;<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"affaris\" value=\"已兑换\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"100\" cmdText=\"affaris\" value=\"订单作废\" onclick=\"commandOperate(this)\" />");
            /*****************************************************************************************
             * 数据统计信息
             * ****************************************************************************************/
            DataRow ComputerRs = DbHelper.Connection.ExecuteFindRow("Stored_FindDuibaComputer", new Dictionary<string, object>() {
                {"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")}
            });
            if (ComputerRs != null && ComputerRs.Table != null)
            {
                foreach (DataColumn col in ComputerRs.Table.Columns)
                {
                    strText.AppendFormat("&nbsp;{0}∶<font style=\"color:#f00;margin:0px 4px; font-weight:bold;\">{1}</font>", col.ColumnName, ComputerRs[col.ColumnName].ToString());
                }
            }
            /******************************************************************************************
             * 数据统计结束
             * ****************************************************************************************/


            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /********************************************************************************
             * 输出网络数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/duiba/default.html");
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
            string strResponse = Master.Reader("template/duiba/looker.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                { case "Id": strValue = RequestHelper.GetRequest("Id").toInt(); break; }
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
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { Response.Write("请求参数错误,请求ID错误！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.FindRow(TableCenter.Duiba, Params: " and id=" + Id + "");
            if (Rs == null) { Response.Write("拉取兑换记录信息失败！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/duiba/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "affairs":
                        switch (Rs["affairs"].ToString())
                        {
                            case "0": strValue = ("<font affairs=\"0\">申请中</font>"); break;
                            case "1": strValue = ("<font affairs=\"1\">已兑换</font>"); break;
                            case "99": strValue = ("<font affairs=\"99\">已处理</font>"); break;
                            case "100": strValue = ("<font affairs=\"100\">已作废</font>"); break;
                        }; break;
                    default: try { strValue = Rs[funName].ToString(); }
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
            string Id = RequestHelper.GetRequest("Id").toString();
            string target = RequestHelper.GetRequest("target").toString();
            if (string.IsNullOrEmpty(target)) { target="sel";}
            if (target == "sel" && string.IsNullOrEmpty(Id)) { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            string strParamter = string.Empty;
            switch (target.ToLower())
            {
                case "sel": strParamter += " and Id in (" + Id + ")"; break;
                case "days": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and Addtime<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and Addtime<='" + DateTime.Now.AddYears(-1) + "'"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            strParamter += " and(Affairs=1 or affairs=100) ";
            DbHelper.Connection.Delete(TableCenter.Duiba, Params: strParamter);
            /********************************************************************
             * 输出返回数据
             * ******************************************************************/
            this.History();
            Response.End();
        }
        
        /// <summary>
        /// 保存批量处理结果
        /// </summary>
        protected void SaveAffaris()
        {
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            string Affairs = RequestHelper.GetRequest("Affairs").toInt();
            if (Affairs == "0") { this.ErrorMessage("处理结果状态明确！"); Response.End(); }
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Duiba, Params: " and Id in (" + Id + ") and Affairs=0");
            if (Tab == null || Tab.Rows.Count <= 0) { this.ErrorMessage("没有要处理的兑换数据！"); Response.End(); }
            foreach (DataRow cRs in Tab.Rows)
            {
                if (cRs["Affairs"].ToString() == "0")
                {
                    try
                    {
                        DuibaAPI(cRs, Affairs, (iSuccess) =>
                        {
                            if (iSuccess)
                            {
                                Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
                                thisDictionary["Id"] = cRs["Id"].ToString();
                                if (Affairs == "1") { thisDictionary["Affairs"] = "99"; }
                                else { thisDictionary["Affairs"] = "100"; }
                                thisDictionary["thisKey"] = "T" + cRs["thisKey"].ToString().Remove(0, 1);
                                thisDictionary["UserID"] = cRs["UserID"].ToString();
                                thisDictionary["Nickname"] = cRs["Nickname"].ToString();
                                thisDictionary["Amount"] = cRs["Amount"].ToString();
                                thisDictionary["Remark"] = string.Format("兑换失败返回兑换积分{0}", cRs["Amount"].ToString());
                                DbHelper.Connection.ExecuteProc("Stored_SaveDuibaAffairs", thisDictionary);
                            }
                        });
                    }
                    catch { }
                }
            }
            /***********************************************************************
             * 输出处理成功后的结果数据
             * *********************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 兑吧审核接口API
        /// </summary>
        /// <param name="Rs"></param>
        protected void DuibaAPI(DataRow Rs, string Affairs, Action<bool> Fun)
        {
            try
            {
                string isOpen = this.GetParameter("isOpen", "DuibaXml").toInt();
                if (isOpen != "1") { this.ErrorMessage("兑吧功能已经被管理员关闭！"); Response.End(); }
                string AppKey = this.GetParameter("appKey", "DuibaXml").toString();
                if (string.IsNullOrEmpty(AppKey)) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
                string AppSecret = this.GetParameter("appSecret", "DuibaXml").toString();
                if (string.IsNullOrEmpty(AppSecret)) { this.ErrorMessage("对接参数错误，请联系管理员！"); Response.End(); }
                System.Collections.Hashtable hshTable = new System.Collections.Hashtable();
                if (Affairs=="1" && Rs != null) { hshTable.Add("passOrderNums", Rs["Biz"].ToString()); }
                else if (Affairs=="2" && Rs != null) { hshTable.Add("rejectOrderNums", Rs["Biz"].ToString()); }
                string url = DuibaHelper.BuildUrlWithSign("http://www.duiba.com.cn/audit/apiAudit", hshTable, AppKey, AppSecret);
                string strResponse = string.Empty;
                using (System.Net.WebClient thisClient = new System.Net.WebClient())
                {
                    try { strResponse = thisClient.DownloadString(url); ; }
                    finally { thisClient.Dispose(); }
                }
                if (!strResponse.Contains("errorMessage") && Fun != null) { Fun(true); }
                else if (Fun != null) { Fun(false); }
            }
            catch { }
        }

        /// <summary>
        /// 保存管理员处理的交易结果
        /// </summary>
        protected void SaveUpdate()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误,请至少传入一条数据！"); Response.End(); }
            string Affairs = RequestHelper.GetRequest("affairs").toInt();
            if (Affairs == "0") { this.ErrorMessage("请确定交易状态！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Duiba, Params: " and Id=" + Id + "");
            if (cRs == null) { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            if (cRs["affairs"].ToString() != "0") { this.ErrorMessage("当前兑换订单已经处理过了，不允许重复处理！"); Response.End(); }
            try
            {
                DuibaAPI(cRs, Affairs, (iSuccess) =>
                {
                    if (iSuccess)
                    {
                        Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
                        thisDictionary["Id"] = cRs["Id"].ToString();
                        if (Affairs == "1") { thisDictionary["Affairs"] = "99"; }
                        else { thisDictionary["Affairs"] = "100"; }
                        thisDictionary["thisKey"] = "T" + cRs["thisKey"].ToString().Remove(0, 1);
                        thisDictionary["UserID"] = cRs["UserID"].ToString();
                        thisDictionary["Nickname"] = cRs["Nickname"].ToString();
                        thisDictionary["Amount"] = cRs["Amount"].ToString();
                        thisDictionary["Remark"] = string.Format("兑换失败返回兑换积分{0}", cRs["Amount"].ToString());
                        DbHelper.Connection.ExecuteProc("Stored_SaveDuibaAffairs", thisDictionary);
                    }
                    else { this.ErrorMessage("兑换接口请求处理失败", iSuccess: false); }
                });
            }
            catch { }
            /******************************************************************************************
             * 返回处理结果
             * ****************************************************************************************/
            this.ErrorMessage("兑换订单处理成功！", iSuccess: true);
            Response.End();
        }
    }
}