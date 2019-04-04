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
    public partial class AppHot : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "confirm": this.VerificationRole("截图审核"); SaveConfirm(); Response.End(); break;
                case "dismiss": this.VerificationRole("截图审核"); SaveDismiss(); Response.End(); break;
                case "excel": this.VerificationRole("截图审核"); strExcel(); Response.End(); break;
                case "savexport": this.VerificationRole("截图审核"); SaveExport(); Response.End(); break;
                case "default": this.VerificationRole("截图审核"); strDefault(); Response.End(); break;
            }
        }

        protected void strExcel()
        {
            /***********************************************************************************************************
             * 输出网页参数内容
             * *********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appHot/excel.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {

                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 列表信息
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 获取筛选条件信息
             * *************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string isFast = RequestHelper.GetRequest("isFast").ToString();
            /**************************************************************************************
             * 构建网页内容
             * *************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"9\">截图审核 >> 审核列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append("<form id=\"SearchForm\" action=\"appHot.aspx\" method=\"get\">");
            strText.Append("<input type=\"hidden\" value=\"default\" name=\"action\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="appname",Text="搜应用名称"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"},
                new OptionMode(){Value="userid",Text="搜用户ID"},
                new OptionMode(){Value="strip",Text="搜IP地址"},
                new OptionMode(){Value="cityname",Text="搜所在城市"},
                new OptionMode(){Value="devicecode",Text="搜设备编号"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"搜索关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <select name=\"isFast\">");
            strText.Append("<option value=\"\">任务状态</option>");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="shenhe",Text="等待审核"},
                new OptionMode(){Value="complate",Text="任务完成"},
                new OptionMode(){Value="fail",Text="任务失败"},
            }, isFast));
            strText.Append("</select>");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"200\">应用名称</td>");
            strText.Append("<td width=\"200\">用户设备</td>");
            strText.Append("<td width=\"140\">IP地址</td>");
            strText.Append("<td width=\"140\">截图信息</td>");
            strText.Append("<td width=\"80\">任务状态</td>");
            strText.Append("<td width=\"80\">通过审核</td>");
            strText.Append("<td width=\"80\">驳回审核</td>");
            strText.Append("<td>任务截图</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************/
            string strParams = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "appname": strParams += " and appname like '%" + Keywords + "%'"; break;
                    case "nickname": strParams += " and nickname like '%" + Keywords + "%'"; break;
                    case "userid": strParams += " and userid like '%" + Keywords + "%'"; break;
                    case "strip": strParams += " and strip like '%" + Keywords + "%'"; break;
                    case "cityname": strParams += " and cityname like '%" + Keywords + "%'"; break;
                    case "devicecode": strParams += " and devicecode like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParams += " and Addtime>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParams += " and Addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (isFast == "complate") { strParams += " and isFinish=1"; }
            else if (isFast == "shenhe") { strParams += " and isFinish=99"; }
            else if (isFast == "fail") { strParams += " and isFinish=100"; }
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "HotID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " HotID desc";
            PageCenterConfig.Tablename = "[Fun_FindAppHotList]";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("[Fun_FindAppHotList]", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************
            * 循环遍历网页内容
            * **********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"tokenId\" value=\"" + Rs["tokenId"] + "\" /></td>");

                strText.AppendFormat("<td>");
                strText.AppendFormat("<div class=\"name\">任务:<a href=\"?action=default&appid={0}&starDate=" + StarDate + "&endDate=" + EndDate + "\">{1}</a>(奖励 {2} 元)</div>", Rs["appid"], Rs["Appname"], Rs["Amount"]);
                strText.AppendFormat("<div class=\"remark\">日期:{0}</div>", new Fooke.Function.String(Rs["Addtime"].ToString()).cDate().ToString("yyyy-MM-dd HH:mm:ss"));
                strText.AppendFormat("</td>");

                strText.AppendFormat("<td>");
                strText.AppendFormat("<div class=\"name\">用户:<a href=\"?action=default&userid={0}&starDate=" + StarDate + "&endDate=" + EndDate + "\">{1}</a></div>", Rs["userid"], Rs["nickname"]);
                strText.AppendFormat("<div class=\"remark\">设备:{0}</div>", Rs["DeviceCode"]);
                strText.AppendFormat("</td>");

                strText.AppendFormat("<td>");
                strText.AppendFormat("<div class=\"name\">城市:{0}</div>", Rs["Cityname"]);
                strText.AppendFormat("<div class=\"remark\">地址:{0}</div>", Rs["strIP"]);
                strText.AppendFormat("</td>");

                strText.AppendFormat("<td>");
                strText.AppendFormat("<div class=\"name\">手机:{0}</div>", Rs["strMobile"]);
                strText.AppendFormat("<div class=\"remark\">姓名:{0}</div>", Rs["strName"]);
                strText.AppendFormat("</td>");

                strText.AppendFormat("<td>{0}</td>", GetAffairsText(Rs["isFinish"].ToString()));
                strText.AppendFormat("<td><input class=\"button\" type=\"button\" onClick=\"ConfirmHot(" + Rs["HotID"] + ")\" value=\"通过审核\" /></td>");
                strText.AppendFormat("<td><input class=\"button\" type=\"button\" onClick=\"DismissHot(" + Rs["HotID"] + ")\" value=\"驳回审核\" /></td>");
                strText.AppendFormat("<td>{0}</td>", ShowThumb(Rs["HotXml"].ToString()));
                strText.AppendFormat("</tr>");
            }
            /***********************************************************************************************
            * 构建分页控件信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
            * 构建操作按钮信息
            * **********************************************************************************************/
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
            strText.Append("<input type=\"button\" class=\"button\" value=\"导出数据\" onclick=\"window.location='?action=excel'\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"confirm\" value=\"通过审核(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"100\" cmdText=\"dismiss\" value=\"通过审核(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /***********************************************************************************************
            * 输出网页信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appHot/default.html");
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
            /**********************************************************************************************************************
             * 获取数据删除模式信息
             * ********************************************************************************************************************/
            string strTarget = RequestHelper.GetRequest("target").toString("sel");
            if (string.IsNullOrEmpty(strTarget)) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            else if (strTarget.Length <= 0) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            /**********************************************************************************************************************
             * 验证参数合法性
             * ********************************************************************************************************************/
            string strList = RequestHelper.GetRequest("tokenId").ToString();
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
                case "sel": strParamter += " and tokenId in (" + strList + ")"; break;
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
            try { DbHelper.Connection.Delete("Fooke_AppDown", Params: strParamter); }
            catch { }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了任务下载记录(" + strParamter + ")"); }
            catch { }
            /**********************************************************************************************************************
             * 返回数据处理结果
             * ********************************************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 截图审核通过
        /// </summary>
        protected void SaveConfirm()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("hotId").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_AppHot", Params: " and hotId in (" + strList + ") and isFinish in (0,99)");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始批量处理数据信息
            * *********************************************************************************************/
            foreach (DataRow cRs in Tab.Rows)
            {
                Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
                thisDictionary["HotID"] = cRs["HotId"].ToString();
                thisDictionary["UserID"] = cRs["UserID"].ToString();
                thisDictionary["AppID"] = cRs["AppID"].ToString();
                thisDictionary["AppKey"] = cRs["AppKey"].ToString();
                thisDictionary["Appname"] = cRs["Appname"].ToString();
                thisDictionary["Nickname"] = cRs["Nickname"].ToString();
                thisDictionary["isFinish"] = "1";
                DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAppHotFinish]", thisDictionary);
                if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
                DataRow appDownRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppDown]", new Dictionary<string, object>() {
                    {"UserID",cRs["UserID"].ToString()},
                    {"AppID",cRs["AppID"].ToString()},
                    {"AppKey",cRs["AppKey"].ToString()},
                });
                /***********************************************************************************************
                * 发放邀请奖励信息
                * *********************************************************************************************/
                DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                    {"UserID",cRs["UserID"].ToString()}
                });
                if (MemberRs != null && appDownRs != null)
                {
                    /*************************************************************************************************************
                    * 发放用户奖励提成数据信息
                    * ***********************************************************************************************************/
                    //是否开启提成 1开启 0未开启
                    string isTask = this.GetParameter("isTask", "shareXml").toInt();
                    //提成次数
                    int CTTimer = this.GetParameter("CTTimer", "shareXml").cInt();
                    if (isTask == "1" && MemberRs["ParentID"].ToString() != "0"
                    && (new Fooke.Function.String(MemberRs["BonusTimer"].ToString()).cInt() <= CTTimer || CTTimer <= 0))
                    {
                        new BonusHelper().TaskBonus(UserID: MemberRs["UserID"].ToString(),
                                Amount: new Fooke.Function.String(appDownRs["Amount"].ToString()).cDouble(),
                                FormatKey: cRs["AppKey"].ToString());
                    }
                    /*************************************************************************************************************
                     * 发放用户奖励提成数据信息（级差提成奖励）
                     * ***********************************************************************************************************/
                    //是否开启级差提成 1开启 0未开启
                    string isGradeTask = this.GetParameter("isGradeTask", "shareXml").toInt();
                    if (isGradeTask == "1" && MemberRs["ParentID"].ToString() != "0")
                    {
                        new BonusHelper().GradeBonus(UserID: MemberRs["UserID"].ToString(),
                                Amount: new Fooke.Function.String(appDownRs["Amount"].ToString()).cDouble(),
                                FormatKey: cRs["AppKey"].ToString());
                    }
                    /*************************************************************************************************************
                    * 判断用户是否为第一次做任务并且
                    * ***********************************************************************************************************/
                    string isBonus = this.GetParameter("isBonus", "shareXml").toInt();
                    string shareModel = this.GetParameter("shareModel", "shareXml").toInt();
                    if (isBonus == "1" && shareModel == "1" && MemberRs["ParentID"].ToString() != "0"
                    && new Fooke.Function.String(MemberRs["BonusTimer"].ToString()).cInt() <= 1)
                    {
                        try { new BonusHelper().ShareBonus(MemberRs["UserID"].ToString()); }
                        catch { }
                    }
                    /***********************************************************************************************
                    * 给用户发送推送消息
                    * *********************************************************************************************/
                    new Fooke.Code.PushCenter().Start(Configure: new Fooke.Code.ConfigureCenter(),
                             content: string.Format("恭喜完成任务获得奖励{0}元", appDownRs["Amount"].ToString()),
                             identify: MemberRs["UserID"].ToString());
                }
            }
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }

        /// <summary>
        /// 截图审核被拒绝
        /// </summary>
        protected void SaveDismiss()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("hotId").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_AppHot", Params: " and hotId in (" + strList + ") and isFinish in (0,99)");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始批量处理数据信息
            * *********************************************************************************************/
            foreach (DataRow cRs in Tab.Rows)
            {
                Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
                thisDictionary["HotID"] = cRs["HotId"].ToString();
                thisDictionary["UserID"] = cRs["UserID"].ToString();
                thisDictionary["AppID"] = cRs["AppID"].ToString();
                thisDictionary["AppKey"] = cRs["AppKey"].ToString();
                thisDictionary["Appname"] = cRs["Appname"].ToString();
                thisDictionary["Nickname"] = cRs["Nickname"].ToString();
                thisDictionary["isFinish"] = "100";
                DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAppHotFinish]", thisDictionary);
                if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            }
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }

        /// <summary>
        /// 导出网页数据
        /// </summary>
        protected void SaveExport()
        {
            /******************************************************************************************
             * 获取数据导出预设参数信息
             * ****************************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            if (StarDate.Length != 0 && !StarDate.isDate()) { this.ErrorMessage("查询开始日期格式错误！"); Response.End(); }
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            if (EndDate.Length != 0 && !EndDate.isDate()) { this.ErrorMessage("查询结束日期格式错误！"); Response.End(); }
            string UnionModel = RequestHelper.GetRequest("UnionModel").toString();
            if (UnionModel.Length >= 20) { this.ErrorMessage("联盟渠道名称字段长度不能超过20个汉字！"); Response.End(); }
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            string Appname = RequestHelper.GetRequest("Appname").toString();
            if (Appname.Length >= 20) { this.ErrorMessage("应用名称字段长度不能超过20个汉字！"); Response.End(); }
            string strCity = RequestHelper.GetRequest("strCity").toString();
            if (strCity.Length >= 20) { this.ErrorMessage("城市名称字段长度不能超过20个汉字！"); Response.End(); }
            string strColumns = RequestHelper.GetRequest("strColumns", false).toString();
            if (strColumns.Length <= 0) { strColumns = "*"; }
            /******************************************************************************************
            * 开始构建数据查询语句
            * ****************************************************************************************/
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append("select " + strColumns + " from [Fun_FindAppHotList] where 1=1");
            if (UnionModel.Length != 0) { strTabs.Append(" and UnionModel like '%" + UnionModel + "%'"); }
            if (Appname.Length != 0) { strTabs.Append(" and Appname like '%" + Appname + "%'"); }
            if (strCity.Length != 0) { strTabs.Append(" and Cityname like '%" + strCity + "%'"); }
            if (AppID != "0") { strTabs.Append(" and AppID = " + AppID + ""); }
            if (StarDate.Length != 0 && StarDate.isDate()) { strTabs.Append(" and LastDate>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"); }
            if (EndDate.Length != 0 && EndDate.isDate()) { strTabs.Append(" and LastDate<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:59") + "'"); }
            /******************************************************************************************
            * 查询数据导出结果
            * ****************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteDataTable(strTabs.ToString());
            if (thisTab == null) { this.ErrorMessage("数据查询语句生成失败,请联系开发人员！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("未查询到需要导出的数据！"); Response.End(); }
            /******************************************************************************************
            * 输出数据处理结果
            * ****************************************************************************************/
            new ExcelHelper().TableOutput(thisTab, "excel");
            Response.End();
        }

        /**********************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 网页方法处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ********************************************************************************************/
        #region 网页方法处理区域
        /// <summary>
        /// 获取当前页面请求参数集合信息
        /// </summary>
        /// <returns></returns>
        public string GetAffairsText(string Affairs)
        {
            string strValue = "";
            switch (new Fooke.Function.String(Affairs).toInt())
            {
                case "0": strValue = "<font affairs=\"0\">等待完成</font>"; break;
                case "1": strValue = "<font affairs=\"2\">任务完成</font>"; break;
                case "99": strValue = "<font affairs=\"99\">等待审核</font>"; break;
                case "100": strValue = "<font affairs=\"100\">任务失败</font>"; break;
            }
            return strValue;
        }
        /// <summary>
        /// 显示上传截图信息
        /// </summary>
        /// <param name="hotXml"></param>
        /// <returns></returns>
        public string ShowThumb(string hotXml)
        {
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                if (hotXml.Length != 0 && hotXml.StartsWith("<configurationRoot>")
                && hotXml.EndsWith("</configurationRoot>") && hotXml.Contains("file"))
                {
                    DataTable thisTab = FunctionCenter.XMLConvertToDataTable(hotXml);
                    if (thisTab != null && thisTab.Rows.Count != 0)
                    {
                        foreach (DataRow cRs in thisTab.Rows)
                        {
                            strBuilder.Append("<img src=\"" + cRs[0] + "\" class=\"img\" onclick=\"ShowThumb()\" />");
                        }
                    }
                }
            }
            catch { }
            return strBuilder.ToString();
        }
        #endregion
    }
}