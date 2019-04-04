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
    public partial class AppTasker : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "add": this.VerificationRole("计划任务"); this.Add(); Response.End(); break;
                case "adddemo": this.VerificationRole("计划任务"); this.AddDemo(); Response.End(); break;
                case "save": this.VerificationRole("计划任务"); SaveUpdate(); Response.End(); break;
                case "stor": this.VerificationRole("计划任务"); SelectedList(); Response.End(); break;
                case "looker": this.VerificationRole("计划任务"); strLooker(); Response.End(); break;
                case "edit": this.VerificationRole("计划任务"); Update(); Response.End(); break;
                case "display": this.VerificationRole("计划任务"); SaveDisplay(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); this.Delete(); Response.End(); break;
                case "excute": this.VerificationRole("超级管理员权限"); SaveExecute(); Response.End(); break;
                default: this.VerificationRole("计划任务"); this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 快速查看选择
        /// </summary>
        protected void SelectedList()
        {
            /****************************************************************************************
             * 获取查询参数条件信息
             * **************************************************************************************/
            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            /**************************************************************************************************
            * 显示网页输出内容
            * **************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"4\">");
            strText.Append("<form id=\"frmForm\" OnSubmit=\"return _doPost(this);\" action=\"appTasker.aspx\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Appname",Text="搜任务名称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写关键词\" type=\"text\" size=\"12\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td>应用名称</td>");
            strText.Append("<td width=\"80\">状态</td>");
            strText.Append("<td width=\"130\">下次执行</td>");
            strText.Append("</tr>");
            /************************************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType)
                {
                    case "Appname": Params += " and Appname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Nextdate>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Nextdate<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:00") + "'"; }
            /************************************************************************************************************
             * 获取分页数量信息
             * **********************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "AppId,Appname,Kucun,Amount,StarDate,EndDate,NextDate,Interval,isDisplay";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "AppId";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "AppId desc";
            PageCenterConfig.Tablename = "Fooke_appTasker";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_appTasker", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /**************************************************************************************************
            * 遍历网页内容
            * **************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">", JSONHelper.ToString(Rs));
                strText.AppendFormat("<td>{0}({1}份)</td>", Rs["Appname"], Rs["Kucun"]);
                strText.AppendFormat("<td>{0}</td>", (Rs["isDisplay"].ToString() == "1" ? "<a class=\"vbtn\">已禁用(否)</a>" : "<a class=\"vbtnRed\">已禁用(是)</a>"));
                strText.AppendFormat("<td>{0}</td>", Rs["Nextdate"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************************************
             * 输出网页内容
             * *****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appTasker/stor.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                    case "pagebar": strValue = PageCenter.Often2(Record, 10); break;
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
            /****************************************************************************************
             * 构建网页内容
             * **************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"11\">计划任务 >> 计划列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"11\">");
            strText.Append("<form id=\"SearchForm\" OnSubmit=\"return _doPost(this)\" action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Appname",Text="搜任务名称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写关键词\" type=\"text\" size=\"12\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
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
            strText.Append("<td>任务名称</td>");
            strText.Append("<td width=\"140\">开始时间</td>");
            strText.Append("<td width=\"140\">结束时间</td>");
            strText.Append("<td width=\"80\">执行间隔</td>");
            strText.Append("<td width=\"140\">下次执行</td>");
            strText.Append("<td width=\"80\">增加份数</td>");
            strText.Append("<td width=\"80\">任务奖励</td>");
            strText.Append("<td width=\"60\">状态</td>");
            strText.Append("<td width=\"100\">执行计划</td>");
            strText.Append("<td width=\"100\">选项</td>");
            strText.Append("</tr>");
            /************************************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType)
                {
                    case "Appname": Params += " and Appname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Nextdate>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Nextdate<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:00") + "'"; }
            /************************************************************************************************************
             * 获取分页数量信息
             * **********************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /************************************************************************************************************
             * 生成分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "AppId,Appname,Kucun,Amount,StarDate,EndDate,NextDate,Interval,isDisplay";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "AppId";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "AppId desc";
            PageCenterConfig.Tablename = "Fooke_appTasker";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_appTasker", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /************************************************************************************************************
             * 输出查询内容
             * **********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"AppId\" value=\"{0}\" /></td>", Rs["AppId"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Appname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Stardate"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Enddate"]);
                strText.AppendFormat("<td style=\"color:#CD0000\">{0}分</td>", Rs["Interval"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Nextdate"]);
                strText.AppendFormat("<td style=\"color:#ff0000\">{0}份</td>", Rs["Kucun"]);
                strText.AppendFormat("<td style=\"color:#009900\">{0}元</td>", Rs["Amount"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.AppendFormat("<a href=\"?action=display&val=0&AppId=" + Rs["AppId"] + "\"><img src=\"template/images/ico/yes.gif\"/></a>"); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&AppId=" + Rs["AppId"] + "\"><img src=\"template/images/ico/no.gif\"/></a>"); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td><input type=\"button\" value=\"执行计划\" OnClick=\"ExecuteTasker({0})\" /></td>", Rs["Appid"]);
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=looker&appid={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["AppId"]);
                strText.AppendFormat("<a href=\"?action=del&appid={0}\"  title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["AppId"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            /************************************************************************************************************
             * 构建分页控件与操作按钮
             * **********************************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"11\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"11\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除选中\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"禁用计划(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"禁用计划(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /************************************************************************************************************
             * 输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appTasker/default.html");
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
        /// 添加计划
        /// </summary>
        protected void Add()
        {
            /***********************************************************************************************
            * 输出网页参数内容
            * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appTasker/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 添加计划
        /// </summary>
        protected void AddDemo()
        {
            /***********************************************************************************************
            * 获取用户信息
            * *********************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { Response.Write("请选择计划任务应用！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (cRs == null) { Response.Write("获取请求数据失败,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 获取当前任务是否已有计划选项
             * *********************************************************************************************/
            DataRow tRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppTasker]", new Dictionary<string, object>() {
                {"AppId",AppID}
            });
            if (tRs != null) { Response.Redirect("appTasker.aspx?action=edit&appid=" + AppID + ""); Response.End(); }
            /***********************************************************************************************
             * 解析网页模板内容信息
             * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appTasker/addDemo.html");
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
        /// 编辑
        /// </summary>
        protected void strLooker()
        {
            /***********************************************************************************************
            * 输出网页参数内容
            * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appTasker/looker.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑计划信息
        /// </summary>
        protected void Update()
        {
            /***********************************************************************************************
            * 获取并验证请求参数信息
            * *********************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { Response.Write("请选择计划任务应用！"); Response.End(); }
            DataRow tRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppTasker]", new Dictionary<string, object>() {
                {"AppId",AppID}
            });
            if (tRs == null) { Response.Redirect("appTasker.aspx?action=adddemo&appid=" + AppID + ""); Response.End(); }
            /***********************************************************************************************
             * 解析网页模板内容信息
             * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appTasker/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = tRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /*******************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 数据处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *****************************************************************************************************************/
        /// <summary>
        /// 保存数据
        /// </summary>
        protected void SaveUpdate()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string AppId = RequestHelper.GetRequest("AppId").toInt();
            if (AppId == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppId}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 获取计划任务开始时间
             * *********************************************************************************************/
            string Stardate = RequestHelper.GetRequest("StarDate").toString();
            if (string.IsNullOrEmpty(Stardate)) { this.ErrorMessage("请选择任务计划开始时间！"); Response.End(); }
            else if (Stardate.Length <= 0) { this.ErrorMessage("请选择任务计划开始时间！"); Response.End(); }
            else if (Stardate.Length <= 10) { this.ErrorMessage("请选择任务计划开始时间！"); Response.End(); }
            else if (Stardate.Length >= 20) { this.ErrorMessage("请选择任务计划开始时间！"); Response.End(); }
            else if (!Stardate.isDate()) { this.ErrorMessage("计划任务开始时间格式错误！"); Response.End(); }
            /***********************************************************************************************
             * 验证计划开始时间是否小于当前时间
             * *********************************************************************************************/
            if (new Fooke.Function.String(Stardate).cDate() <= DateTime.Now)
            { this.ErrorMessage("计划开始时间不能小于当前时间！"); Response.End(); }
            /***********************************************************************************************
             * 获取计划任务结束时间
             * *********************************************************************************************/
            string EndDate = RequestHelper.GetRequest("EndDate").toString("9999-01-01 00:00:00");
            if (string.IsNullOrEmpty(EndDate)) { this.ErrorMessage("请选择任务计划结束时间！"); Response.End(); }
            else if (EndDate.Length <= 0) { this.ErrorMessage("请选择任务计划结束时间！"); Response.End(); }
            else if (EndDate.Length <= 10) { this.ErrorMessage("请选择任务计划结束时间！"); Response.End(); }
            else if (EndDate.Length >= 20) { this.ErrorMessage("请选择任务计划结束时间！"); Response.End(); }
            else if (!EndDate.isDate()) { this.ErrorMessage("计划任务结束时间格式错误！"); Response.End(); }
            /***********************************************************************************************
             * 验证计划开始时间是否小于当前时间
             * *********************************************************************************************/
            if (new Fooke.Function.String(EndDate).cDate() <= DateTime.Now)
            { this.ErrorMessage("计划任务结束时间不能小于当前时间！"); Response.End(); }
            /***********************************************************************************************
             * 获取计划任务奖励金额信息
             * *********************************************************************************************/
            double Amount = RequestHelper.GetRequest("Amount").cDouble();
            if (Amount <= 0) { this.ErrorMessage("计划任务奖励金额不能小于0！"); Response.End(); }
            else if (Amount >= 1000) { this.ErrorMessage("计划任务奖励金额不能大于1000"); Response.End(); }
            /***********************************************************************************************
             * 获取计划任务更新库存信息
             * *********************************************************************************************/
            int Kucun = RequestHelper.GetRequest("Kucun").cInt();
            if (Kucun < 0) { this.ErrorMessage("计划任务库存份数不能小于0！"); Response.End(); }
            else if (Kucun >= 1000000) { this.ErrorMessage("计划任务库存份数不能大于1000000"); Response.End(); }
            /***********************************************************************************************
             * 获取任务执行间隔,0表示值执行一次
             * *********************************************************************************************/
            int Interval = RequestHelper.GetRequest("Interval").cInt(1440);
            if (Interval < 0) { this.ErrorMessage("计划执行间隔时间不能小于0！"); Response.End(); }
            else if (Interval >= 14400) { this.ErrorMessage("计划执行间隔时间不能大于14400分钟"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据处理信息
             * *********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["AppId"] = cRs["AppId"].ToString();
            thisDictionary["Appname"] = cRs["Appname"].ToString();
            thisDictionary["Kucun"] = Kucun.ToString("0");
            thisDictionary["Amount"] = Amount.ToString("0.00");
            thisDictionary["StarDate"] = new Fooke.Function.String(Stardate).cDate().ToString("yyyy-MM-dd HH:mm:ss");
            thisDictionary["EndDate"] = new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd HH:mm:ss");
            thisDictionary["Interval"] = Interval.ToString("0");
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAppTasker]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /******************************************************************************************
            * 保存充值日志
            * ****************************************************************************************/
            try { SaveOperation("新增加了" + cRs["Appname"] + "计划任务(" + Kucun + "份," + Amount + "元)"); }
            catch { }
            /******************************************************************************************
             * 输出数据处理信息
             * ****************************************************************************************/
            this.ErrorMessage("计划任务更新成功！", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        protected void Delete()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AppID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始删除请求数据
            * *********************************************************************************************/
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_AppTasker", Params: " and AppID in (" + strList + ") and isDisplay=0");
            if (sTab == null) { this.ErrorMessage("没有需要删除的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { this.ErrorMessage("没有需要删除的数据！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_AppTasker", Params: " and AppID in (" + strList + ") and isDisplay=0");
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了计划任务ID(" + strList + ")"); }
            catch { }
            /**********************************************************************************************************************
             * 返回数据处理结果
             * ********************************************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 设置状态
        /// </summary>
        protected void SaveDisplay()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AppID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_AppTasker", Params: " and AppID in (" + strList + ")");
            if (sTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证参数值的合法性
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            if (strValue != "0" && strValue != "1") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据
             * *********************************************************************************************/
            DbHelper.Connection.Update("Fooke_AppTasker", dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and AppID in (" + strList + ")");
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 执行计划任务
        /// </summary>
        protected void SaveExecute()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AppID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_AppTasker", Params: " and AppID in (" + strList + ")");
            if (sTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证参数值的合法性
             * *********************************************************************************************/
            foreach (DataRow sRs in sTab.Rows)
            {
                DbHelper.Connection.Update("Fooke_Application", dictionary: new Dictionary<string, string>() { 
                        {"Kucun",sRs["Kucun"].ToString()},
                        {"Amount",sRs["Amount"].ToString()},
                    }, Params: " and AppID=" + sRs["AppId"] + "");
                if (new Fooke.Function.String(sRs["Interval"].ToString()).cInt() != 0)
                {
                    DateTime Nextdate = new Fooke.Function.String(sRs["Nextdate"].ToString()).cDate();
                    int Interval = new Fooke.Function.String(sRs["Interval"].ToString()).cInt();
                    DbHelper.Connection.Update("Fooke_AppTasker", dictionary: new Dictionary<string, string>() { 
                            {"NextDate",Nextdate.AddMinutes(Interval).ToString("yyyy-MM-dd HH:mm:ss")}
                        }, Params: " and AppID=" + sRs["AppId"] + "");
                }
            }
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}