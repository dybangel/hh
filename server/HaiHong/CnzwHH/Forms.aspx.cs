using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using Fooke.Function;
using Fooke.Code;
namespace Fooke.Web.Admin
{
    public partial class Forms : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            switch (this.strRequest)
            {
                case "save": this.VerificationRole("在线表单"); this.AddSave(); Response.End(); ; break;
                case "add": this.VerificationRole("在线表单"); this.Add(); Response.End(); break;
                case "del": this.VerificationRole("在线表单"); this.Delete(); Response.End(); break;
                case "edit": this.VerificationRole("在线表单"); this.Update(); Response.End(); break;
                case "etxt": this.VerificationRole("在线表单"); this.Editor(); Response.End(); break;
                case "etxtsave": this.VerificationRole("在线表单"); this.SaveEditor(); Response.End(); break;
                case "editsave": this.VerificationRole("在线表单"); this.SaveUpdate(); Response.End(); break;
                case "display": this.VerificationRole("在线表单"); this.Display(); Response.End(); break;
                case "code": this.VerificationRole("在线表单"); this.CodeList(); Response.End(); break;
                case "build": this.VerificationRole("在线表单"); this.CreateBuild(); Response.End(); break;
                case "define": this.VerificationRole("在线表单"); this.DefineList(); Response.End(); break;
                case "disdefine": this.VerificationRole("在线表单"); this.disDefine(); Response.End(); break;
                case "deldefine": this.VerificationRole("在线表单"); this.delDefine(); Response.End(); break;
                case "viewdefine": this.VerificationRole("在线表单"); this.DetailsDefine(); Response.End(); break;
                case "saveTo": this.VerificationRole("在线表单"); this.saveTo(); Response.End(); break;
                default: this.VerificationRole("在线表单"); this.strDefault(); Response.End(); break;
            }
        }

        /// <summary>
        /// 管理员列表
        /// </summary>
        protected void strDefault()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">在线表单 >> 表单列表</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"15%\">表单名称</td>");
            strText.Append("<td width=\"15%\">数据表名</td>");
            strText.Append("<td>描述信息</td>");
            strText.Append("<td width=\"5%\">是否使用</td>");
            strText.Append("<td width=\"180\">操作选项</td>");
            strText.Append("</tr>");
            string Params = "";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "fromsId,fromsName,Tablename,isDisplay,isDate,StarDate,endDate,Descrption";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "fromsId";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " fromsId Desc";
            PageCenterConfig.Tablename = TableCenter.Forms;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Forms, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"fromsId\" value=\"" + Rs["fromsId"] + "\" /></td>");
                strText.Append("<td>" + Rs["fromsName"] + "</td>");
                strText.Append("<td style=\"color:#f00\">" + Rs["Tablename"] + "</td>");
                strText.Append("<td>" + Rs["Descrption"] + "</td>");
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&fromsId=" + Rs["fromsId"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&fromsId=" + Rs["fromsId"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=etxt&fromsId=" + Rs["fromsId"] + "\" title=\"修改表单内容\"><img src=\"images/ico/process.png\" /></a>");
                strText.Append("<a href=\"columns.aspx?action=add&chName=" + Rs["fromsName"] + "&Tablename=" + Rs["tablename"] + "\" title=\"为表单添加新的字段\"><img src=\"images/ico/add.png\" /></a>");
                strText.Append("<a href=\"?action=define&fromsId=" + Rs["fromsId"] + "\" title=\"查看表单数据\"><img src=\"images/ico/chart.png\" /></a>");
                strText.Append("<a href=\"?action=edit&fromsId=" + Rs["fromsId"] + "\" title=\"编辑表单项目\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=del&fromsId=" + Rs["fromsId"] + "\" title=\"删除表单\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"6\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"允许使用\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"禁止使用\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" textMessage='重新生成表单将覆盖原有的编辑，你确定要重新生成？' cmdText=\"build\" value=\"生成表单\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forms/default.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
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

        protected void CodeList() {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"3\">表单管理 >> 代码调用</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"20%\">表单名称</td>");
            strText.Append("<td>调用代码(点击直接复制代码)</td>");
            strText.Append("</tr>");
            string Params = "";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "fromsId,fromsName,Tablename,isDisplay,isDate,StarDate,endDate,Descrption";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "fromsId";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " fromsId Desc";
            PageCenterConfig.Tablename = TableCenter.Forms;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Forms, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"fromsId\" value=\"" + Rs["fromsId"] + "\" /></td>");
                strText.Append("<td>" + Rs["fromsName"] + "</td>");
                strText.Append("<td id=\"CodeString_" + Rs["fromsId"] + "\">{fooke.froms.code(" + Rs["fromsId"] + ")/}</td>");
                strText.Append("</tr>");
                strText.Append("<script language=\"javascript\">copyToClipboard(\"{fooke.froms.code(" + Rs["fromsId"] + ")/}\",\"CodeString_" + Rs["fromsId"] + "\");</script>");
            }
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"3\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /**********************************************************************************************************
             * 输出网络数据信息
             * ********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forms/codelist.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
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
        /// 查看表单数据列表
        /// </summary>
        protected void DefineList()
        {
            /*****************************************************************************************************
             * 获取指定的表单信息
             * ***************************************************************************************************/
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误，请选择一个表单！"); Response.End(); }
            DataRow fromsRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindForms]", new Dictionary<string, object>() {
                {"fromsId",fromsId}
            });
            if (fromsRs == null) { this.ErrorMessage("对不起，你查找的表单不存在！"); Response.End(); }
            /*****************************************************************************************************
            * 内容查询
            * ***************************************************************************************************/
            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            /*****************************************************************************************************
            * 开始显示表单的列表内容
            * ***************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"7\">表单管理 >> "+fromsRs["fromsName"]+" >> 数据列表</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<form action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"define\" />");
            strText.Append("<input type=\"hidden\" name=\"fromsId\" value=\"" + fromsId + "\" />");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="title",Text="搜标题"},
                new OptionMode(){Value="username",Text="搜用户账号"}
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
            strText.Append("<input type=\"hidden\" name=\"fromsId\" value=\"" + fromsId + "\" />");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"5%\">ID</td>");
            strText.Append("<td width=\"25%\">主题</td>");
            strText.Append("<td>回复备注</td>");
            strText.Append("<td width=\"12%\">时间</td>");
            strText.Append("<td width=\"5%\">审核</td>");
            strText.Append("<td width=\"180\">操作选项</td>");
            strText.Append("</tr>");
            /****************************************************************************************************
             * 构建分页语句查询条件
             * **************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "title": Params += " and title like '%" + Keywords + "%'"; break;
                    case "username": Params += " and exists(select userid from fooke_user where userid=" + fromsRs["Tablename"] + ".userid and username like '%" + Keywords + "%')"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate + "'"; }
            /****************************************************************************************************
            * 构建分页语句内容信息
            * **************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "Id,formsId,strKey,Title,Addtime,isDisplay,cmdText,cmdDate,strIP";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Id Desc";
            PageCenterConfig.Tablename = fromsRs["Tablename"].ToString();
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(fromsRs["Tablename"].ToString(), Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /****************************************************************************************************
            * 遍历网页内容信息
            * **************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"Id\" value=\"" + Rs["Id"] + "\" /></td>");
                strText.Append("<td>" + Rs["Id"] + "</td>");
                strText.Append("<td>" + Rs["title"] + "</td>");
                strText.Append("<td>" + Rs["cmdText"] + "</td>");
                strText.Append("<td>" + Rs["addtime"] + "</td>");
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=disdefine&val=0id=" + Rs["Id"] + "&fromsId=" + fromsId + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=disdefine&val=1&id="+Rs["Id"]+"&fromsId=" + fromsId + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=viewdefine&Id=" + Rs["Id"] + "&fromsId="+fromsId+"\" title=\"查看详情\"><img src=\"images/ico/chart.png\" /></a>");
                strText.Append("<a href=\"?action=deldefine&Id=" + Rs["Id"] + "&fromsId=" + fromsId + "\" title=\"删除\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"7\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"7\">");
            strText.Append("<select name=\"target\">");
            strText.Append("<option value=\"sel\">删除选中的数据</option>");
            strText.Append("<option value=\"days\">删除一天前的记录</option>");
            strText.Append("<option value=\"week\">删除一周前的记录</option>");
            strText.Append("<option value=\"month\">删除一月前的记录</option>");
            strText.Append("<option value=\"byear\">删除半年前的记录</option>");
            strText.Append("<option value=\"year\">删除一年前的记录</option>");
            strText.Append("<option value=\"all\">删除所有记录</option>");
            strText.Append("</select> ");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" value=\"删除\" textMessage=\"数据删除以后将无法恢复，请确定要删除？\" cmdText=\"deldefine\"  onclick=\"commandOperate(this)\"  />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"disdefine\" value=\"通过审核\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"disdefine\" value=\"设为未审\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /****************************************************************************************************
           * 输出网页内容
           * **************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forms/definelist.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
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
        /// 过滤保存数据信息
        /// </summary>
        private static readonly string NotFlter = "title|action|fromsid|codetext|frmdefine|tokey";
        /// <summary>
        /// 解析Xml集合中的内容数据
        /// </summary>
        /// <param name="xReader"></param>
        /// <returns></returns>
        public string formXML(ConfigurationReader xReader)
        {
            StringBuilder strText = new StringBuilder();
            try
            {
                foreach (KeyValuePair<string, string> oKey in xReader.dictionary)
                {
                    if (!NotFlter.Contains(oKey.Key.ToLower()))
                    {

                        strText.Append("<tr class=\"hback\">");
                        strText.Append("<td class=\"tips\">" + oKey.Key + "</td>");
                        strText.Append("<td>" + oKey.Value + "</td>");
                        strText.Append("</tr>");
                    }
                }
            }
            catch { }
            return strText.ToString();
        }

        /// <summary>
        /// 查看表单下单条数据详情
        /// </summary>
        protected void DetailsDefine()
        {
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow fromsRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindForms]", new Dictionary<string, object>() {
                {"fromsId",fromsId}
            });
            if (fromsRs == null) { this.ErrorMessage("表单不存在，或者已经被删除！"); Response.End(); }
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindDefineForms]", new Dictionary<string, object>() {
                {"Tablename",fromsRs["Tablename"]},
                {"Id",Id}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            ConfigurationReader xReader = new ConfigurationReader(Rs["strXML"].ToString());
            /*****************************************************************************************************
             * 输出网页内容信息
             * ***************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forms/details.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Value="1",Text="审核通过"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="拒绝通过"}
                    }, Rs["isdisplay"].ToString());break;
                    case "fromsId": strValue = fromsRs["fromsId"].ToString(); break;
                    case "fromsName": strValue = fromsRs["fromsName"].ToString(); break;
                    case "define": strValue = formXML(xReader); break;
                    default:
                        try { strValue = Rs[funName].ToString(); }
                        catch { strValue = xReader.GetParameter(funName).toString(); }; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 添加自定义表单
        /// </summary>
        protected void Add()
        {

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forms/add.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "isdate": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdate",Value="1",Text="开启"},
                        new RadioMode(){Name="isdate",Value="0",Text="关闭"}
                    }, "0"); break;
                    case "iscode": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="iscode",Value="1",Text="开启"},
                        new RadioMode(){Name="iscode",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "isauto": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isauto",Value="1",Text="自动审核"},
                        new RadioMode(){Name="isauto",Value="0",Text="手动审核"}
                    }, "1"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        protected void Update() {
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindForms]", new Dictionary<string, object>() {
                {"fromsId",fromsId}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            ConfigurationReader xReader = new ConfigurationReader(cRs["strXML"].ToString());
            /*************************************************************************************
             * 显示输出网页内容
             * ***********************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forms/edit.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, cRs["isDisplay"].ToString()); break;
                    case "isdate": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdate",Value="1",Text="开启"},
                        new RadioMode(){Name="isdate",Value="0",Text="关闭"}
                    },cRs["isDate"].ToString()); break;
                    case "iscode": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="iscode",Value="1",Text="开启"},
                        new RadioMode(){Name="iscode",Value="0",Text="关闭"}
                    }, xReader.GetParameter("iscode").toInt()); break;
                    case "isauto": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isauto",Value="1",Text="自动审核"},
                        new RadioMode(){Name="isauto",Value="0",Text="手动审核"}
                    }, xReader.GetParameter("isauto").toInt()); break;
                    default:
                        try { strValue = cRs[funName].ToString(); }
                        catch { strValue = xReader.GetParameter(funName).ToString(); }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();

        }
        /// <summary>
        /// 编辑表单内容
        /// </summary>
        protected void Editor()
        {
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsId",fromsId}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /***************************************************************************************************
             * 输出网页内容信息
             * *************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/forms/editor.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "BuildTxt":
                        strValue = cRs["buildTxt"].ToString();
                        if (!string.IsNullOrEmpty(strValue)) { strValue = HttpUtility.HtmlEncode(strValue); }
                        break;
                    default:
                        try { strValue = cRs[funName].ToString(); }
                        catch { }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /**************************************************************************
         * 数据处理区域
         * ************************************************************************/
        /// <summary>
        /// 生成表单的填写项目
        /// </summary>
        protected void CreateBuild()
        {
            string fromsId = RequestHelper.GetRequest("fromsId").toString();
            if (string.IsNullOrEmpty(fromsId)) { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            /***********************************************************************
             * 查询出要生成的表单信息
             * **********************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Forms, Params: " and fromsId in (" + fromsId + ")");
            foreach (DataRow Rs in Tab.Rows)
            {
                string Tokey = string.Format("-|-|-在线表单-|-|-{0}-|-|-{1}", Rs["fromsId"], Rs["fromsName"]).md5().ToLower();
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendLine("<form action=\"" + Win.ApplicationPath + "/items/froms.aspx\" method=\"post\" />");
                strBuilder.AppendLine("<input type=\"hidden\" name=\"action\" value=\"start\" />");
                strBuilder.AppendLine("<input type=\"hidden\" name=\"tokey\" value=\"" + Tokey + "\" />");
                strBuilder.AppendLine("<input type=\"hidden\" name=\"fromsId\" value=\"" + Rs["fromsId"] + "\" />");
                strBuilder.AppendLine("<table width=\"100%\" class=\"table\" cellspacing=\"1\" cellpadding=\"3\" border=\"0\">");
                /**********************************************************************************
                 * 生成主题
                 * ********************************************************************************/
                strBuilder.AppendLine("<tr>");
                strBuilder.AppendLine("<td class=\"tips\">主题</td>");
                strBuilder.AppendLine("<td><input type=\"text\" name=\"title\" id=\"frm-froms-title\" size=\"30\" /></td>");
                strBuilder.AppendLine("</tr>");
                /**********************************************************************************
                 * 解析自定义字段信息
                 * ********************************************************************************/
                new ColumnsHelper().Show(Rs["Tablename"].ToString(), (strValue, cRs) =>
                {
                    strBuilder.AppendLine("<tr>");
                    strBuilder.AppendLine("<td class=\"tips\">" + cRs["columnsText"] + "</td>");
                    strBuilder.AppendLine("<td>" + strValue + "</td>");
                    strBuilder.AppendLine("</tr>");
                }, null);
                /**********************************************************************************
                * 解析验证码信息
                * ********************************************************************************/
                ConfigurationReader xReader = new ConfigurationReader(Rs["strXML"].ToString());
                string isCode = xReader.GetParameter("isCode").toInt();
                if (isCode == "1")
                {
                    strBuilder.AppendLine("<tr>");
                    strBuilder.AppendLine("<td class=\"tips\">验证码</td>");
                    strBuilder.AppendLine("<td>");
                    strBuilder.AppendLine("<input type=\"text\" name=\"codeText\" id=\"frm-codeText\" size=\"12\" />");
                    strBuilder.AppendLine("<img src=\"" + Win.ApplicationPath + "/plugin/vcode.aspx\" valign=\"absmiddle\" onclick=\"this.src='" + Win.ApplicationPath + "/inc/vcode.aspx?math='+Math.random();\" title=\"看不清楚？点击刷新验证码!\" style=\"width:80px;height:28px;\" />");
                    strBuilder.AppendLine("</td>");
                    strBuilder.AppendLine("</tr>");
                }
                /**********************************************************************************
                * 提交表单
                * ********************************************************************************/
                strBuilder.AppendLine("<tr>");
                strBuilder.AppendLine("<td></td>");
                strBuilder.AppendLine("<td><input type=\"submit\" value=\"确认提交\" id=\"frm-button\" /></td>");
                strBuilder.AppendLine("</tr>");
                strBuilder.AppendLine("</table>");
                strBuilder.AppendLine("</form>");
                if (string.IsNullOrEmpty(strBuilder.ToString())) { this.ErrorMessage("构建表单过程中发生错误,请重试！"); Response.End(); }
                /*******************************************************************************************************
                 * 保存表单内容
                 * *****************************************************************************************************/
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary["buildTxt"] = strBuilder.ToString();
                DbHelper.Connection.Update(TableCenter.Forms, dictionary, Params: " and fromsId=" + Rs["fromsId"] + "");
            }
            /********************************************************************
             * 更新表单的缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Froms"); }
            catch { }
            /********************************************************************
             * 开始输出相应代码
             * ******************************************************************/
            this.ErrorMessage("恭喜，表单已成功生成！");
            Response.End();
        }
        /// <summary>
        /// 保存添加账户
        /// </summary>
        protected void AddSave()
        {
            /****************************************************************************
            * 获取表单名称
            * **************************************************************************/
            string fromsName = RequestHelper.GetRequest("fromsName").toString();
            if (fromsName == "") { this.ErrorMessage("请输入表单名称！"); Response.End(); }
            if (fromsName.Length > 20) { this.ErrorMessage("表单名称请限制在20个字符以内！"); Response.End(); }
            DataRow nRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsName",fromsName}
            });
            if (nRs != null) { this.ErrorMessage("表单名称已经存在了,请另外选择一个吧！"); Response.End(); }
            /*****************************************************************************
             * 获取表单数据表信息
             * ****************************************************************************/
            string Tablename = RequestHelper.GetRequest("Tablename").toString();
            if (string.IsNullOrEmpty(Tablename)) { this.ErrorMessage("请输入表单数据表名称！"); Response.End(); }
            if (Tablename.Length > 20) { this.ErrorMessage("表单数据表名称长度请限制在20个字符以内！"); Response.End(); }
            if (VerifyCenter.VerifyChina(Tablename)) { this.ErrorMessage("表单数据表名称不允许出现中文字符！"); Response.End(); }
            if (VerifyCenter.VerifySpecific(Tablename)) { this.ErrorMessage("表单数据表不允许出现特殊字符！"); Response.End(); }
            Tablename = "Fooke_Froms_Define_" + Tablename;
            /****************************************************************************
             * 检查自定义表单是否存在
             * **************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"Tablename",Tablename}
            });
            if (cRs != null) { this.ErrorMessage("表单数据表已经存在了！"); Response.End(); }
            /********************************************************************************
            * 模板地址
            * ******************************************************************************/
            string Template = RequestHelper.GetRequest("Template").ToString();
            if (string.IsNullOrEmpty(Template)) { this.ErrorMessage("请选择加载表单的模板地址！"); Response.End(); }
            if (Template.Length > 60) { this.ErrorMessage("模板地址的长度请限制在60个字符以内！"); Response.End(); }
            /********************************************************************************
             * 获取提交日期信息
             * ******************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").toDate();
            string EndDate = RequestHelper.GetRequest("EndDate").toDate();
            string isDate = RequestHelper.GetRequest("isDate").toInt();
            if (isDate == "1" && string.IsNullOrEmpty(StarDate) && !StarDate.isDate()) { this.ErrorMessage("请设置有效期开始日期！"); Response.End(); }
            if (isDate == "1" && string.IsNullOrEmpty(EndDate) && !EndDate.isDate()) { this.ErrorMessage("请设置有效期结束日期！"); Response.End(); }
            /*********************************************************************************
             * 获取其他的表单数据信息
             * *******************************************************************************/
            string isCode = RequestHelper.GetRequest("isCode").toInt();
            string isAuto = RequestHelper.GetRequest("isAuto").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string Descrption = RequestHelper.GetRequest("Descrption").toString();
            string strXML = RequestHelper.GetPrametersXML(notTarget: "intxt");
            /****************************************************************************
             * 检查数据表是否存在于数据库
             * **************************************************************************/
            DataRow oRs = DbHelper.Connection.FindRow("SysObjects", columns: "Name", Params: " and XType='U' And Name='" + Tablename + "'");
            if (oRs != null) { this.ErrorMessage("数据表在表单中不存在，却存在于数据库中，请维护你的数据库！"); Response.End(); }
            /****************************************************************************
             * 开始插入数据
             * **************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["fromsName"] = fromsName;
            thisDictionary["Tablename"] = Tablename;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["isDate"] = isDate;
            thisDictionary["StarDate"] = StarDate;
            thisDictionary["endDate"] = EndDate;
            thisDictionary["Descrption"] = Descrption;
            thisDictionary["Template"] = Template;
            thisDictionary["strXML"] = strXML;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveForms", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); }
            /********************************************************************
             * 更新表单的缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Froms"); }
            catch { }
            /********************************************************************
             * 开始输出相应代码
             * ******************************************************************/
            this.ConfirmMessage("保存成功，点击确定将继续停留在当前页面！"); Response.End();
            Response.End();
        }

        protected void SaveUpdate()
        {

            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsID",fromsId}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /****************************************************************************
             * 获取表单名称
             * **************************************************************************/
            string fromsName = RequestHelper.GetRequest("fromsName").toString();
            if (string.IsNullOrEmpty(fromsName)) { this.ErrorMessage("请输入表单名称！"); Response.End(); }
            if (fromsName.Length > 20) { this.ErrorMessage("表单名称请限制在20个字符以内！"); Response.End(); }
            /****************************************************************************
             * 检查表单名称是否已经存在
             * **************************************************************************/
            DataRow nRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsName",fromsName}
            });
            if (nRs != null && nRs["fromsId"].ToString() != Rs["fromsId"].ToString()) { this.ErrorMessage("表单名称已经存在了,请另外选择一个吧！"); Response.End(); }
            /********************************************************************************
           * 模板地址
           * ******************************************************************************/
            string Template = RequestHelper.GetRequest("Template").ToString();
            if (string.IsNullOrEmpty(Template)) { this.ErrorMessage("请选择加载表单的模板地址！"); Response.End(); }
            if (Template.Length > 60) { this.ErrorMessage("模板地址的长度请限制在60个字符以内！"); Response.End(); }
            /***************************************************************************
             * 验证其他的数据信息
             * **************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").toDate();
            string EndDate = RequestHelper.GetRequest("EndDate").toDate();
            string isDate = RequestHelper.GetRequest("isDate").toInt();
            if (isDate == "1" && string.IsNullOrEmpty(StarDate)) { this.ErrorMessage("请设置有效期开始日期！"); Response.End(); }
            if (isDate == "1" && string.IsNullOrEmpty(EndDate)) { this.ErrorMessage("请设置有效期结束日期！"); Response.End(); }
            string isCode = RequestHelper.GetRequest("isCode").toInt();
            string isAuto = RequestHelper.GetRequest("isAuto").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string Descrption = RequestHelper.GetRequest("Descrption").toString();
            string strXML = RequestHelper.GetPrametersXML(notTarget: "intxt");
            /****************************************************************************
             * 检查数据表是否存在于数据库
             * **************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["fromsId"] = Rs["fromsId"].ToString();
            thisDictionary["fromsName"] = fromsName;
            thisDictionary["Tablename"] = Rs["tablename"].ToString();
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["isDate"] = isDate;
            thisDictionary["StarDate"] = StarDate;
            thisDictionary["endDate"] = EndDate;
            thisDictionary["Descrption"] = Descrption;
            thisDictionary["Template"] = Template;
            thisDictionary["strXML"] = strXML;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveForms", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); }
            /********************************************************************
             * 更新表单的缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Froms"); }
            catch { }
            /********************************************************************
             * 开始输出相应代码
             * ******************************************************************/
            this.ConfirmMessage("表单编辑成功，点击确定将继续停留在当前页面！"); Response.End();
            Response.End();
        }
        /// <summary>
        /// 保存表单内容
        /// </summary>
        protected void SaveEditor()
        {
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsID",fromsId}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            string buildTxt = RequestHelper.GetRequest("buildTxt",false).toString();
            if (string.IsNullOrEmpty(buildTxt)) { this.ErrorMessage("表单内容不能为空！"); Response.End(); }
            try { buildTxt = System.Web.HttpUtility.HtmlDecode(buildTxt); }
            catch { }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["BuildTxt"] = buildTxt;
            DbHelper.Connection.Update(TableCenter.Forms, dictionary, Params: " and fromsId = " + fromsId + "");
            /********************************************************************
             * 更新表单的缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Froms"); }
            catch { }
            /********************************************************************
             * 开始输出相应代码
             * ******************************************************************/
            this.ConfirmMessage("表单内容编辑成功，点击确定继续编辑！");
            Response.End();
        }
        /// <summary>
        /// 保存单个的表单数据
        /// </summary>
        protected void saveTo()
        {
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow fromsRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsID",fromsId}
            });
            if (fromsRs == null) { this.ErrorMessage("对不起，你查找的表单不存在！"); Response.End(); }
            
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindDefineForms]", new Dictionary<string, object>() {
                {"Tablename",fromsRs["Tablename"]},
                {"Id",Id}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            string cmdText = RequestHelper.GetRequest("cmdText").toString();
            if (!string.IsNullOrEmpty(cmdText) && cmdText.Length > 200) { this.ErrorMessage("回复备注内容请保持在200个汉字以内！"); Response.End(); }
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["isDisplay"] = isDisplay;
            dictionary["cmdText"] = cmdText;
            dictionary["cmdDate"] = DateTime.Now.ToString();
            DbHelper.Connection.Update(fromsRs["Tablename"].ToString(), dictionary, Params: " and Id = " + cRs["Id"] + "");
            /******************************************************************************************************
             * 返回输出的内容信息
             * *****************************************************************************************************/
            this.ConfirmMessage("保存成功，点击确定将继续停留在当前页面！", falseUrl: "?action=define&fromsid=" + fromsId + "");
            Response.End();
        }

        /// <summary>
        /// 删除表单数据
        /// </summary>
        protected void Delete()
        {
            string fromsId = RequestHelper.GetRequest("fromsId").toString();
            if (string.IsNullOrEmpty(fromsId)) { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Forms, Params: " and fromsId in (" + fromsId + ")");
            foreach (DataRow Rs in Tab.Rows)
            {
                DbHelper.Connection.ExecuteText("drop table " + Rs["Tablename"] + "");
                DbHelper.Connection.Delete(TableCenter.Forms, Params: " and fromsId = " + Rs["fromsId"] + "");
            }
            /********************************************************************
             * 更新表单的缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Froms"); }
            catch { }
            /********************************************************************
             * 开始输出相应代码
             * ******************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 审核管理员状态
        /// </summary>
        protected void Display()
        {
            string fromsId = RequestHelper.GetRequest("fromsId").toString();
            if (string.IsNullOrEmpty(fromsId)) { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            string isDisplay = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["isDisplay"] = isDisplay;
            DbHelper.Connection.Update(TableCenter.Forms, dictionary, Params: " and fromsId in (" + fromsId + ")");
            /********************************************************************
             * 更新表单的缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Froms"); }
            catch { }
            /********************************************************************
             * 开始输出相应代码
             * ******************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 审核表单下的数据
        /// </summary>
        protected void disDefine()
        {
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误，请选择表单！"); Response.End(); }
            DataRow fromsRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsID",fromsId}
            });
            if (fromsRs == null) { this.ErrorMessage("请求参数错误，表单不存在！"); Response.End(); }
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("请求参数错误，请至少上传一条数据！"); }
            string val = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["isDisplay"] = val;
            DbHelper.Connection.Update(fromsRs["Tablename"].ToString(),dictionary,Params:" and Id in ("+Id+")");
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除表单下的数据
        /// </summary>
        protected void delDefine()
        {
            /************************************************************************************************
             * 获取表单查询信息
             * **********************************************************************************************/
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误，请选择表单！"); Response.End(); }
            DataRow fromsRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsID",fromsId}
            });
            if (fromsRs == null) { this.ErrorMessage("请求参数错误，表单不存在！"); Response.End(); }
            /***********************************************************************************************
             * 开始删除数据信息
             * *********************************************************************************************/
            string Id = RequestHelper.GetRequest("Id").toString();
            string target = RequestHelper.GetRequest("target").toString();
            if (string.IsNullOrEmpty(target)) { target = "sel"; }
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
                case "all": strParamter += " and 1=1"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DbHelper.Connection.Delete(fromsRs["Tablename"].ToString(), Params: " and Id in (" + Id + ")");
            /******************************************************************************************
             * 返回数据处理结果
             * ****************************************************************************************/
            this.History();
            Response.End();
        }
    }
}