using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using Fooke.Code;
using Fooke.Function;
namespace Fooke.Web.Admin
{
    public partial class Defend : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "edit": this.VerificationRole("超级管理员权限"); Update(); Response.End(); break;
                case "add": this.VerificationRole("超级管理员权限"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("超级管理员权限"); UpdateSave(); Response.End(); break;
                case "save": this.VerificationRole("超级管理员权限"); AddSave(); Response.End(); break;
                case "default": this.VerificationRole("超级管理员权限"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "cln": this.VerificationRole("超级管理员权限"); FindColumns(); Response.End(); break;
                case "run": this.VerificationRole("超级管理员权限"); DefendRun(); Response.End(); break;
                case "runText": this.VerificationRole("超级管理员权限"); DefendText(); Response.End(); break;
                case "text": this.VerificationRole("超级管理员权限"); ExecuteText(); Response.End(); break;
                case "savetext": this.VerificationRole("超级管理员权限"); SaveExecute(); Response.End(); break;
            }
        }
        /// <summary>
        /// 显示字段内容
        /// </summary>
        protected void FindColumns()
        {
            if (string.IsNullOrEmpty(Request["Tablename"])) { Response.Write("请求参数错误！"); Response.End(); }
            if (Request["Tablename"].Length > 30) { this.ErrorMessage("数据表名称太长!"); Response.End(); }
            string Tablename = Request["Tablename"].ToString();
            try {
                Tablename = Tablename.ToLower().Replace("'", "");
                Tablename = Tablename.ToLower().Replace("\"", "");
                Tablename = Tablename.ToLower().Replace(";", "");
            }
            catch { }
            /***************************************************************************************
             * 开始查询字段信息
             * *************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                DataTable Tab = DbHelper.Connection.ExecuteFindTable("Stored_FindMaster", new Dictionary<string, object>() {
                    {"FindMode","Columns"},
                    {"Tablename",Tablename}
                });
                foreach (DataRow Rs in Tab.Rows)
                {
                    strBuilder.AppendFormat("<a value=\"{0}\">{0}</a>", Rs["name"]);
                }
            }
            catch { }
            /*********************************************************************************
             * 输出处理结果
             * *******************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取我的银行卡列表信息
        /// </summary>
        protected void strDefault()
        {
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            StringBuilder strText = new StringBuilder();
            /************************************************************************************************************
             * 输出网页内容
             * ***********************************************************************************************************/
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"7\">数据维护 >> 语句列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="defendname",Text="搜名称"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append("&nbsp;<input type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append("&nbsp;<input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td>语句名称</td>");
            strText.Append("<td width=\"120\">添加日期</td>");
            strText.Append("<td width=\"120\">最后执行日期</td>");
            strText.Append("<td width=\"120\">执行次数</td>");
            strText.Append("<td width=\"120\">操作选项</td>");
            strText.Append("</tr>");
            /****************************************************************************************
              * 构建查询条件
              * ***************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType)) 
            {strParameter += " and groupname like '%" + Keywords + "%'";}
            /****************************************************************************************
             * 构建分页查询语句
             * ***************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "DefendID,DefendName,Addtime,LastDate,ExecTimer,DefendMode";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "DefendID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " DefendID Desc";
            PageCenterConfig.Tablename = "Fooke_Defend";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Defend", strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /****************************************************************************************
             * 循环遍历内容
             * ***************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"GroupID\" value=\"{0}\" /></td>", Rs["DefendID"]);
                strText.AppendFormat("<td>" + Rs["DefendName"] + "</td>");
                strText.AppendFormat("<td>" + Rs["Addtime"] + "</td>");
                strText.AppendFormat("<td>" + Rs["LastDate"] + "</td>");
                strText.AppendFormat("<td>" + Rs["ExecTimer"] + "</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=run&DefendID={0}\" title=\"执行维护语句\"><img src=\"template/images/ico/chart.png\" /></a>", Rs["DefendID"]);
                strText.AppendFormat("<a href=\"?action=edit&DefendID={0}\" title=\"编辑维护语句\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["DefendID"]);
                strText.AppendFormat("<a href=\"?action=del&DefendID={0}\"  title=\"删除维护语句\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["DefendID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            /******************************************************************************************
             * 显示分页内容
             * ****************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /****************************************************************************************
             * 输出网页内容
             * ***************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/defend/default.html");
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
        /// 添加用户组
        /// </summary>
        protected void Add()
        {
            /********************************************************************
             * 添加数据维护语句,就是添加SQL语句
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/defend/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": FindTable((str) => { strValue = str; }); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 查询出指定的系统表
        /// </summary>
        /// <param name="Fun"></param>
        public void FindTable(Action<string> Fun)
        {
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                DataTable Tab = DbHelper.Connection.ExecuteFindTable("Stored_FindMaster", new Dictionary<string, object>() {
                    {"FindMode","Table"}
                });
                foreach (DataRow Rs in Tab.Rows)
                {
                    strBuilder.AppendFormat("<option value=\"{0}\">{0}</option>", Rs["name"]);
                }
            }
            catch { }
            try
            {
                if (Fun != null && !string.IsNullOrEmpty(strBuilder.ToString()))
                {
                    Fun(strBuilder.ToString());
                }
            }
            catch { }
        }

        /// <summary>
        /// 编辑用户组
        /// </summary>
        protected void Update()
        {
            string DefendID = RequestHelper.GetRequest("DefendID").toInt();
            if (DefendID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefend", new Dictionary<string, object>() {
                {"DefendID",DefendID}
            });
            if (Rs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/defend/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": FindTable((str) => { strValue = str; }); break;
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { } break;
                    
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        protected void ExecuteText()
        {
            /********************************************************************
             * 添加数据维护语句,就是添加SQL语句
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/defend/text.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": FindTable((str) => { strValue = str; }); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 执行数据维护语句
        /// </summary>
        protected void DefendRun()
        {
            string DefendID = RequestHelper.GetRequest("DefendID").toInt();
            if (DefendID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefend", new Dictionary<string, object>() {
                {"DefendID",DefendID}
            });
            if (Rs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            /********************************************************************
            * 解析defendText语句中需要使用的参数信息
            * ******************************************************************/
            StringBuilder strText = new StringBuilder();
            if (!string.IsNullOrEmpty(Rs["defendText"].ToString()) && Rs["defendText"].ToString().Contains("{@"))
            {
                strText.AppendFormat("<table cellpadding=\"3\" style=\"width:100%;margin:0px;\" cellspacing=\"1\" border=\"0\" class=\"table\">");
                strText.AppendFormat("<tr class=\"xingmu\">");
                strText.AppendFormat("<td style=\"width:120px\">参数名称</td>");
                strText.AppendFormat("<td>参数值</td>");
                strText.AppendFormat("</tr>");
                this.DecideDefend(Rs["defendText"].ToString(), (funName, funText) =>
                {
                    strText.AppendFormat("<tr class=\"hback\">");
                    strText.AppendFormat("<td>{0}</td>", funName);
                    strText.AppendFormat("<td><input type=\"text\" notkong=\"true\" placeholder=\"请传入参数\" class=\"inputtext\" name=\"{0}\" id=\"frm-{0}\" size=\"20\" /></td>", funName);
                    strText.AppendFormat("</tr>");
                });
                strText.AppendFormat("</table>");
            }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/defend/run.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "agrs": strValue = strText.ToString(); break;
                    case "defendText": strValue = Rs["defendText"].ToString().Replace(";", ";<br/><br/>"); break;
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 解析出执行语句中的变量体
        /// </summary>
        /// <param name="defendText"></param>
        /// <param name="Fun"></param>
        public void DecideDefend(string defendText, Action<string,string> Fun)
        {
            try
            {
                Regex Rex = new Regex(@"{@(?<funName>(.+?))(?<funText>([.](.*?)))?/}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                foreach (Match Matchx in Rex.Matches(defendText))
                {
                    if (Matchx.Groups["funName"] != null && !string.IsNullOrEmpty(Matchx.Groups["funName"].ToString()))
                    {
                        try
                        {
                            string funName = Matchx.Groups["funName"].ToString();
                            string funText = Matchx.Groups["funText"].ToString();
                            try
                            {
                                if (string.IsNullOrEmpty(funText)) { funText = ""; }
                                if (!funText.Contains("(") && funText.Contains(")")) { funText = ""; }
                            }
                            catch { }
                            if (!string.IsNullOrEmpty(funName) && Fun != null)
                            {
                                Fun(funName, funText);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 开始执行查询语句
        /// </summary>
        protected void DefendText()
        {
            /******************************************************************************************
             * 获取查询语句的内容
             * *****************************************************************************************/
            string DefendID = RequestHelper.GetRequest("DefendID").toInt();
            if (DefendID == "0") { Response.Write("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefend", new Dictionary<string, object>() {
                {"DefendID",DefendID}
            });
            if (Rs == null) { Response.Write("对不起,你查找的数据不存在！"); Response.End(); }
            /******************************************************************************************
             * 验证是否需要保存到Excel数据
             * ****************************************************************************************/
            int isExcel = RequestHelper.GetRequest("isExcel").cInt();
            string Excelname = RequestHelper.GetRequest("Excelname").ToString();
            if (isExcel == 1 && string.IsNullOrEmpty(Excelname)) { Response.Write("请填写要保存到的Excel的名称!"); Response.End(); }
            if (isExcel == 1 && Excelname.Length > 30) { Response.Write("Excel名称长度请限制在30个字符以内!"); Response.End(); }
            /******************************************************************************************
             * 获取查询语句需要执行的参数信息
             * *****************************************************************************************/
            string defendText = Rs["defendText"].ToString();
            if (!string.IsNullOrEmpty(defendText) && defendText.Contains("{@"))
            {
                this.DecideDefend(defendText, (funName, funText) =>
                {
                    string strValue = RequestHelper.GetRequest(funName).ToString();
                    if (string.IsNullOrEmpty(strValue)) { Response.Write(string.Format("参数{0}不能为空!", funName)); Response.End(); }
                    if (strValue.Length > 300) { Response.Write(string.Format("参数{0}内容太多,请限制在300个字符以内！", funName)); Response.End(); }
                    /*********************************************************************************
                     * 解析参数处理函数
                     * ********************************************************************************/
                    if (!string.IsNullOrEmpty(funText))
                    {
                        Fooke.SimpleMaster.StringSystemFunctionCenter StringObject = new Fooke.SimpleMaster.StringSystemFunctionCenter();
                        strValue = StringObject.Run(strValue, funText);
                    }
                    /*********************************************************************************
                    * 开始替换变量体
                    * ********************************************************************************/
                    if (!string.IsNullOrEmpty(funText) && funText.Contains("("))
                    { defendText = defendText.Replace("{@" + funName + "." + funText + "/}", strValue); }
                    else { defendText = defendText.Replace("{@" + funName + "/}", strValue); }
                });
            }
            /*******************************************************************************************
             * 开始执行语句
             * *****************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            DbHelper.Connection.ExecuteTableBack("Stored_SaveDefendRun", new Dictionary<string, object>() {
                {"DefendID",Rs["DefendID"].ToString()},
                {"DefendText",defendText}
            }, new MSSQL.DataTableFunction((Tab, K) =>
            {
                strBuilder.AppendFormat("<table border=\"1\" bordercolor=\"#c0c0c0\" cellpadding=\"3\" cellspacing=\"1\">");
                strBuilder.AppendFormat("<tr>");
                foreach (DataColumn cln in Tab.Columns)
                {
                    strBuilder.AppendFormat("<th>{0}</th>", cln.ColumnName);
                }
                strBuilder.AppendFormat("</tr>");
                foreach (DataRow cRs in Tab.Rows)
                {
                    strBuilder.AppendFormat("<tr>");
                    foreach (DataColumn cln in Tab.Columns)
                    {
                        strBuilder.AppendFormat("<td>{0}</td>", cRs[cln.ColumnName]);
                    }
                    strBuilder.AppendFormat("</tr>");
                }
                strBuilder.AppendFormat("</table>");
                strBuilder.AppendLine("<br/>");
            }));
            /*******************************************************************************************
             * 判断是否为Excel输出
             * *****************************************************************************************/
            if (isExcel == 1 && !string.IsNullOrEmpty(Excelname))
            {
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = System.Text.Encoding.Default.ToString();
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + Excelname + ".xls");
                Response.ContentEncoding = System.Text.Encoding.Default;
                Response.ContentType = "application/ms-excel";
                Response.Write(strBuilder);
                Response.End();
            }
            else
            {
                Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
                string strResponse = Master.Reader("template/defend/result.html");
                strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
                {
                    string strValue = string.Empty;
                    switch (funName)
                    {
                        case "result": strValue = strBuilder.ToString(); break;
                        default: try { strValue = Rs[funName].ToString(); }
                            catch { } break;
                    }
                    return strValue;
                }));
                Response.Write(strResponse);
                Response.End();
            }
        }
        /// <summary>
        /// 执行自定义SQL语句
        /// </summary>
        protected void SaveExecute()
        {
            /***************************************************************************************
             * 验证维护语句是否合法
             * *************************************************************************************/
            if (string.IsNullOrEmpty(Request["defendText"])) { Response.Write("请填写数据维护语句！"); Response.End(); }
            if (Request["defendText"].Length > 20000) { Response.Write("数据维护语句长度请限制在20000个字符以内！"); Response.End(); }
            string DefendText = Request["defendText"].ToString().Trim();
            if (DefendText.Contains("{@")) { Response.Write("SQL语句中不允许变量存在!"); Response.End(); }
            if (!DefendText.ToLower().StartsWith("select") &&
                !DefendText.ToLower().StartsWith("insert") &&
                !DefendText.ToLower().StartsWith("delete") &&
                !DefendText.ToLower().StartsWith("update"))
            {
                Response.Write("SQL语句格式错误,只允许执行增删查改语句!"); Response.End();
            }
            if (DefendText.ToLower().Contains("drop")) { Response.Write("不允许执行drop命令!"); Response.End(); }
            if (DefendText.ToLower().Contains("alter")) { Response.Write("不允许执行alter命令!"); Response.End(); }
            if (DefendText.ToLower().Contains("modify")) { Response.Write("不允许执行modify命令!"); Response.End(); }
            if (DefendText.ToLower().Contains("create")) { Response.Write("不允许执行create命令!"); Response.End(); }
            if (DefendText.Length <= 0) { Response.Write("请求参数错误,请重试！"); Response.End(); }
            /******************************************************************************************
             * 验证是否需要保存到Excel数据
             * ****************************************************************************************/
            int isExcel = RequestHelper.GetRequest("isExcel").cInt();
            string Excelname = RequestHelper.GetRequest("Excelname").ToString();
            if (isExcel == 1 && string.IsNullOrEmpty(Excelname)) { Response.Write("请填写要保存到的Excel的名称!"); Response.End(); }
            if (isExcel == 1 && Excelname.Length > 30) { Response.Write("Excel名称长度请限制在30个字符以内!"); Response.End(); }
            /*******************************************************************************************
             * 开始执行语句
             * *****************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            DbHelper.Connection.ExecuteTableBack("Stored_SaveDefendText", new Dictionary<string, object>() {
                {"DefendText",DefendText}
            }, new MSSQL.DataTableFunction((Tab, K) =>
            {
                strBuilder.AppendFormat("<table border=\"1\" bordercolor=\"#c0c0c0\" cellpadding=\"3\" cellspacing=\"1\">");
                strBuilder.AppendFormat("<tr>");
                foreach (DataColumn cln in Tab.Columns)
                {
                    strBuilder.AppendFormat("<th>{0}</th>", cln.ColumnName);
                }
                strBuilder.AppendFormat("</tr>");
                foreach (DataRow cRs in Tab.Rows)
                {
                    strBuilder.AppendFormat("<tr>");
                    foreach (DataColumn cln in Tab.Columns)
                    {
                        strBuilder.AppendFormat("<td>{0}</td>", cRs[cln.ColumnName]);
                    }
                    strBuilder.AppendFormat("</tr>");
                }
                strBuilder.AppendFormat("</table>");
                strBuilder.AppendLine("<br/>");
            }));
            /*******************************************************************************************
             * 判断是否为Excel输出
             * *****************************************************************************************/
            if (isExcel == 1 && !string.IsNullOrEmpty(Excelname))
            {
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = System.Text.Encoding.Default.ToString();
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + Excelname + ".xls");
                Response.ContentEncoding = System.Text.Encoding.Default;
                Response.ContentType = "application/ms-excel";
                Response.Write(strBuilder);
                Response.End();
            }
            else
            {
                Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
                string strResponse = Master.Reader("template/defend/result.html");
                strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
                {
                    string strValue = string.Empty;
                    switch (funName)
                    {
                        case "result": strValue = strBuilder.ToString(); break;
                    }
                    return strValue;
                }));
                Response.Write(strResponse);
                Response.End();
            }

        }

        /// <summary>
        /// 添加银行卡
        /// </summary>
        protected void AddSave()
        {
            /***************************************************************************************
             * 验证名称信息
             * *************************************************************************************/
            string DefendName = RequestHelper.GetRequest("DefendName").ToString();
            if (string.IsNullOrEmpty(DefendName)) { this.ErrorMessage("请填写维护语句名称!"); Response.End(); }
            if (DefendName.Length > 40) { this.ErrorMessage("维护语句名称长度请保持在40个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefend", new Dictionary<string, object>() { 
                {"DefendName",DefendName}
            });
            if (cRs != null) { this.ErrorMessage("维护语句名称已经存在,请重新命名！"); Response.End(); }
            /***************************************************************************************
            * 验证维护语句是否合法
            * *************************************************************************************/
            if (string.IsNullOrEmpty(Request["defendText"])) { this.ErrorMessage("请填写数据维护语句！"); Response.End(); }
            if (Request["defendText"].Length > 20000) { this.ErrorMessage("数据维护语句长度请限制在20000个字符以内！"); Response.End(); }
            string DefendText = Request["defendText"].ToString();
            /***************************************************************************************
             * 开始保存数据
             * *************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["DefendID"] = "0";
            thisDictionary["DefendName"] = DefendName;
            thisDictionary["DefendText"] = DefendText;
            thisDictionary["DefendMode"] = "0";
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveDefend]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************
             * 输出网页信息
             * ****************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }

        /// <summary>
        /// 添加银行卡
        /// </summary>
        protected void UpdateSave()
        {

            string DefendID = RequestHelper.GetRequest("DefendID").toInt();
            if (DefendID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefend", new Dictionary<string, object>() {
                {"DefendID",DefendID}
            });
            if (Rs == null) { this.ErrorMessage("拉取数据失败,你查找的信息不存在！"); Response.End(); }
            /***************************************************************************************
             * 验证名称信息
             * *************************************************************************************/
            string DefendName = RequestHelper.GetRequest("DefendName").ToString();
            if (string.IsNullOrEmpty(DefendName)) { this.ErrorMessage("请填写维护语句名称!"); Response.End(); }
            if (DefendName.Length > 40) { this.ErrorMessage("维护语句名称长度请保持在40个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefend", new Dictionary<string, object>() { 
                {"DefendName",DefendName}
            });
            if (cRs != null && cRs["DefendID"].ToString() != Rs["DefendID"].ToString()) { this.ErrorMessage("维护语句名称已经存在,请重新命名！"); Response.End(); }
            /***************************************************************************************
            * 验证维护语句是否合法
            * *************************************************************************************/
            if (string.IsNullOrEmpty(Request["defendText"])) { this.ErrorMessage("请填写数据维护语句！"); Response.End(); }
            if (Request["defendText"].Length > 20000) { this.ErrorMessage("数据维护语句长度请限制在20000个字符以内！"); Response.End(); }
            string DefendText = Request["defendText"].ToString();
            /***************************************************************************************
             * 开始保存数据
             * *************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["DefendID"] = Rs["DefendId"].ToString();
            thisDictionary["DefendName"] = DefendName;
            thisDictionary["DefendText"] = DefendText;
            thisDictionary["DefendMode"] = "0";
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveDefend]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************
             * 输出网页信息
             * ****************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }
        /// <summary>
        /// 删除用户等级信息
        /// </summary>
        protected void Delete()
        {
            string DefendID = RequestHelper.GetRequest("DefendID").toString();
            if (string.IsNullOrEmpty(DefendID)) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_Defend", Params: " and DefendID in (" + DefendID + ")");
            /****************************************************
             * 输出处理结果
             * ****************************************************/
            this.History();
            Response.End();
        }
    }
}