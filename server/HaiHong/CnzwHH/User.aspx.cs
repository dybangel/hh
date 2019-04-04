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
    public partial class User : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "del": this.VerificationRole("超级管理员权限"); this.Delete(); Response.End(); break;
                case "editsave": this.VerificationRole("用户管理"); SaveUpdate(); Response.End(); break;
                case "edit": this.VerificationRole("用户管理"); Update(); Response.End(); break;
                case "display": this.VerificationRole("用户管理"); SaveDisplay(); Response.End(); break;
                case "add": this.VerificationRole("用户管理"); Add(); Response.End(); break;
                case "save": this.VerificationRole("用户管理"); AddSave(); Response.End(); break;
                case "stor": SelectedUser(); Response.End(); break;
                case "looker": this.VerificationRole("用户管理"); ShowDetails(); Response.End(); break;
                case "computer": this.VerificationRole("用户管理"); strComputer(); Response.End(); break;
                case "network": this.VerificationRole("用户管理"); strNetwork(); Response.End(); break;
                case "chd": this.VerificationRole("用户管理"); ShowChild(); Response.End(); break;
                default: this.VerificationRole("用户管理"); strDefault(); Response.End(); break;
            }
        }

        protected void ShowChild() 
        {
            /*****************************************************************************************
             * 获取当前展示的级数信息
             * ***************************************************************************************/
            int NodeLevel = RequestHelper.GetRequest("nLevel").cInt();
            string ShowChar = "";
            for (int sLevel = 0; sLevel <= NodeLevel; sLevel++) {
                ShowChar = ShowChar + "&nbsp;";
            }
            /*****************************************************************************************
             * 开始构建网页展示内容
             * ***************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            /************************************************************************************************
             * 构建分页查询语句条件
             * **********************************************************************************************/
            string Params = " and ParentID=" + RequestHelper.GetRequest("UserID").toInt() + "";
            /*************************************************************************************************************************
             * 获取分页数据信息
             * ************************************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(50);
            /*************************************************************************************************************************
             * 构建分页查询语句
             * ************************************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UserID,ParentID,isChild,DeviceType,DeviceModel,GroupID,Groupname,UserName,Nickname,Amount,Points,strMobile,strIP,strCity,BonusTimer,BonusAmount,AlipayTimer,AlipayAmount,Addtime,LastDate,isDisplay,isBreak";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " UserID desc";
            PageCenterConfig.Tablename = "[Fun_FindUsers]";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("[Fun_FindUsers]", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            strBuilder.Append("<table class=\"tabs2\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            /*************************************************************************************************************************
             * 循环遍历网页内容
             * ************************************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                if (Rs["isChild"].ToString() != "0")
                {
                    strBuilder.AppendFormat("<tr class=\"hback\">");
                    strBuilder.AppendFormat("<td style=\"width:120px\" operate=\"looker\" url=\"user.aspx?action=looker&userid={0}\">{0}</td>", Rs["userid"]);
                    strBuilder.AppendFormat("<td spread=\"false\" onclick=\"ShowChd(" + Rs["UserID"] + ",this," + (NodeLevel+1) + ")\">");
                    strBuilder.AppendFormat(ShowChar);
                    strBuilder.AppendFormat("<font class=\"ico\"></font>");
                    strBuilder.AppendFormat("<font>" + Rs["UserName"] + "</font>");
                    strBuilder.AppendFormat("</td>");
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", Rs["isChild"]);
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", Rs["Nickname"]);
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", new Fooke.Function.String(Rs["Addtime"].ToString()).cDate().ToString("yyyy-MM-dd"));
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", new Fooke.Function.String(Rs["lastDate"].ToString()).cDate().ToString("yyyy-MM-dd"));
                    strBuilder.AppendFormat("</tr>");
                    strBuilder.AppendFormat("<tr class=\"contianer\" id=\"contianer" + Rs["userId"] + "\"><td style=\"padding:0px;height:0px;\" colspan=\"6\"></td><tr>");
                }
                else
                {
                    strBuilder.AppendFormat("<tr class=\"hback\">");
                    strBuilder.AppendFormat("<td style=\"width:120px\" operate=\"looker\" url=\"user.aspx?action=looker&userid={0}\">{0}</td>", Rs["userid"]);
                    strBuilder.AppendFormat("<td spread=\"true\">");
                    strBuilder.AppendFormat(ShowChar);
                    strBuilder.AppendFormat("<font class=\"ico\"></font>");
                    strBuilder.AppendFormat("<font>" + Rs["UserName"] + "</font>");
                    strBuilder.AppendFormat("</td>");
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", Rs["isChild"]);
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", Rs["Nickname"]);
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", new Fooke.Function.String(Rs["Addtime"].ToString()).cDate().ToString("yyyy-MM-dd"));
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", new Fooke.Function.String(Rs["lastDate"].ToString()).cDate().ToString("yyyy-MM-dd"));
                    strBuilder.AppendFormat("</tr>");
                }
            }
            strBuilder.Append("</table>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 网络结构图
        /// </summary>
        protected void strNetwork()
        {
            /**************************************************************************************
            * 获取查询预设参数信息
            * ************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StartDate = RequestHelper.GetRequest("StartDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string isFast = RequestHelper.GetRequest("isFast").ToString();
            string ParentID = RequestHelper.GetRequest("ParentID").toString();
            /*****************************************************************************************
             * 开始构建网页展示内容
             * ***************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"6\">用户管理 >> 网络结构图</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"search\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append("<form action=\"?\"  id=\"SearchForms\" method=\"get\">");
            strBuilder.Append("<input type=\"hidden\" name=\"action\" value=\"network\">");
            strBuilder.Append("<select name=\"SearchType\">");
            strBuilder.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="UserID",Text="搜用户ID"},
                new OptionMode(){Value="Nickname",Text="搜用户昵称"}
            }, SearchType));
            strBuilder.Append("</select>");
            strBuilder.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strBuilder.Append(" 到期日期 <input type=\"text\" placeholder=\"请选择日期\" onClick=\"WdatePicker()\" isDate=\"true\" value=\"" + StartDate + "\" name=\"StartDate\" size=\"12\" class=\"inputtext\" />");
            strBuilder.Append(" - <input type=\"text\" placeholder=\"请选择日期\" onClick=\"WdatePicker()\" isDate=\"true\" value=\"" + EndDate + "\" name=\"EndDate\" size=\"12\" class=\"inputtext\" />");
            strBuilder.Append(" <select name=\"isFast\">");
            strBuilder.Append("<option value=\"\">所有用户</option>");
            strBuilder.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="today",Text="今日注册"},
                new OptionMode(){Value="online",Text="当前在线"},
                new OptionMode(){Value="disable",Text="禁用账户"},
            }, isFast));
            strBuilder.Append("</select>");
            strBuilder.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td style=\"width:120px\">用户ID</td>");
            strBuilder.Append("<td>登陆账号</td>");
            strBuilder.Append("<td style=\"width:120px\">直推</td>");
            strBuilder.Append("<td style=\"width:120px\">用户昵称</td>");
            strBuilder.Append("<td style=\"width:120px\">注册日期</td>");
            strBuilder.Append("<td style=\"width:120px\">最后登陆日期</td>");
            strBuilder.Append("</tr>");
            /************************************************************************************************
             * 构建分页查询语句条件
             * **********************************************************************************************/
            //string Params = " and ParentID=0";
            string Params = string.Empty;
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "userid": Params += " and userid like '%" + Keywords + "%'"; break;
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StartDate) && VerifyCenter.VerifyDateTime(StartDate)) { Params += " and Addtime>='" + StartDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (!string.IsNullOrEmpty(isFast) && isFast == "today") { Params += " and Addtime>='" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "'"; }
            else if (!string.IsNullOrEmpty(isFast) && isFast == "online") { Params += " and isOnline=1"; }
            else if (!string.IsNullOrEmpty(isFast) && isFast == "disable") { Params += " and isDisplay=0"; }
            if (!string.IsNullOrEmpty(ParentID) && ParentID.isInt()) { Params += " and ParentID=" + ParentID + ""; }
            /*************************************************************************************************************************
             * 获取分页数据信息
             * ************************************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /*************************************************************************************************************************
             * 构建分页查询语句
             * ************************************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UserID,ParentID,isChild,DeviceType,DeviceModel,GroupID,Groupname,UserName,Nickname,Amount,Points,strMobile,strIP,strCity,BonusTimer,BonusAmount,AlipayTimer,AlipayAmount,Addtime,LastDate,isDisplay,isBreak";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " UserID desc";
            PageCenterConfig.Tablename = "[Fun_FindUsers]";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("[Fun_FindUsers]", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            
            /*************************************************************************************************************************
             * 循环遍历网页内容
             * ************************************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                if (Rs["isChild"].ToString() != "0")
                {
                    strBuilder.AppendFormat("<tr class=\"hback\">");
                    strBuilder.AppendFormat("<td style=\"width:120px\" operate=\"looker\" url=\"user.aspx?action=looker&userid={0}\">{0}</td>", Rs["userid"]);
                    strBuilder.AppendFormat("<td spread=\"false\" onclick=\"ShowChd(" + Rs["UserID"] + ",this,0)\">");
                    strBuilder.AppendFormat("<font class=\"ico\"></font>");
                    strBuilder.AppendFormat("<font>" + Rs["UserName"] + "</font>");
                    strBuilder.AppendFormat("</td>");
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", Rs["isChild"]);
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", Rs["Nickname"]);
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", new Fooke.Function.String(Rs["Addtime"].ToString()).cDate().ToString("yyyy-MM-dd"));
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", new Fooke.Function.String(Rs["lastDate"].ToString()).cDate().ToString("yyyy-MM-dd"));
                    strBuilder.AppendFormat("</tr>");
                    strBuilder.AppendFormat("<tr class=\"contianer\" id=\"contianer" + Rs["userId"] + "\"><td style=\"padding:0px;height:0px;\" colspan=\"6\"></td><tr>");
                }
                else
                {
                    strBuilder.AppendFormat("<tr class=\"hback\">");
                    strBuilder.AppendFormat("<td style=\"width:120px\" operate=\"looker\" url=\"user.aspx?action=looker&userid={0}\">{0}</td>", Rs["userid"]);
                    strBuilder.AppendFormat("<td spread=\"true\">");
                    strBuilder.AppendFormat("<font class=\"ico\"></font>");
                    strBuilder.AppendFormat("<font>" + Rs["UserName"] + "</font>");
                    strBuilder.AppendFormat("</td>");
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", Rs["isChild"]);
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", Rs["Nickname"]);
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", new Fooke.Function.String(Rs["Addtime"].ToString()).cDate().ToString("yyyy-MM-dd"));
                    strBuilder.AppendFormat("<td style=\"width:120px\">{0}</td>", new Fooke.Function.String(Rs["lastDate"].ToString()).cDate().ToString("yyyy-MM-dd"));
                    strBuilder.AppendFormat("</tr>");
                }
            }
            
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append(PageCenter.Often(Record, PageSize));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</form>");
            strBuilder.Append("</table>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/user/network.html");
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
        /// 数据统计
        /// </summary>
        protected void strComputer()
        {
            /*****************************************************************************************
             * 数据统计信息
             * ****************************************************************************************/
            DataTable Tab = DbHelper.Connection.ExecuteFindTable("[Stored_FindUserComputerTable]", new Dictionary<string, object>() {
                {"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")},
                {"Yesterday",DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00")},
                {"thisMonth",DateTime.Now.ToString("yyyy-MM-01 00:00:00")},
                {"LastMonth",DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01 00:00:00")},
                {"Month",DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd 00:00:00")}
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
            string strResponse = Master.Reader("template/user/computer.html");
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

        public string GetPosition(string ParentID="0")
        {
            /****************************************************************************************************
             * 计算当前路径
             * **************************************************************************************************/
            StringBuilder dirBuilder = new StringBuilder();
            if (ParentID != "0")
            {
                try
                {
                    DataTable dirTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindUserParent]", new Dictionary<string, object>() {
                        {"NodeID",ParentID}
                    });
                    foreach (DataRow Rs in dirTab.Select("", sort: " nodeLevel desc"))
                    {
                        dirBuilder.AppendFormat("<a href=\"user.aspx?action=default&parentid={0}\">{1}</a> >> ", Rs["UserId"], Rs["Nickname"]);
                    }
                }
                catch { }
            }
            return dirBuilder.ToString();
        }

        public void dirPosition(DataRow cRs, Action<string> Fun)
        {
            /****************************************************************************************************
             * 计算当前路径
             * **************************************************************************************************/
            StringBuilder dirBuilder = new StringBuilder();
            if (cRs != null && cRs["ParentID"].ToString() != "0")
            {
                try
                {
                    DataTable dirTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindUserParent]", new Dictionary<string, object>() {
                        {"NodeID",cRs["ParentID"].ToString()}
                    });
                    foreach (DataRow Rs in dirTab.Rows)
                    {
                        dirBuilder.AppendFormat("<a href=\"user.aspx?action=looker&userid={0}\">{1}</a> >> ", Rs["UserId"], Rs["Nickname"]);
                    }
                }
                catch { }
            }
            try { if (Fun != null) { Fun(dirBuilder.ToString()); } }
            catch { }
        }
        /// <summary>
        /// 查看用户详情
        /// </summary>
        protected void ShowDetails()
        {
            /****************************************************************************************************
             * 验证参数合法性
             * **************************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { Response.Write("参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                { "UserID", UserID }
            });
            if (cRs == null) { Response.Write("拉取数据失败，你查找的信息不存在！"); Response.End(); }
            /****************************************************************************************************
             * 输出网页内容信息
             * **************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/user/looker.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "dir": dirPosition(cRs, (strResp) => { strValue = strResp; }); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 快速选择
        /// </summary>
        protected void SelectedUser()
        {
            /**************************************************************************************
             * 获取查询预设参数信息
             * ************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StartDate = RequestHelper.GetRequest("StartDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            /**************************************************************************************************
            * 显示网页输出内容
            * **************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"5\">");
            strText.Append("<form id=\"frmForm\" OnSubmit=\"return _doPost(this);\" action=\"user.aspx\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="UserID",Text="搜用户ID"},
                new OptionMode(){Value="Nickname",Text="搜用户昵称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input size=\"12\" type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"60\">用户编号</td>");
            strText.Append("<td width=\"100\">登录帐号</td>");
            strText.Append("<td>用户昵称</td>");
            strText.Append("<td width=\"40\">状态</td>");
            strText.Append("</tr>");
            /************************************************************************************************
             * 构建分页查询语句条件
             * **********************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "userid": Params += " and userid like '%" + Keywords + "%'"; break;
                    case "username": Params += " and username like '%" + Keywords + "%'"; break;
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StartDate) && VerifyCenter.VerifyDateTime(StartDate)) { Params += " and Addtime>='" + StartDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:00") + "'"; }
            /*************************************************************************************************************************
             * 构建分页查询语句
             * ************************************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UserID,UserName,Nickname,isOnline,isDisplay";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " UserID desc";
            PageCenterConfig.Tablename = TableCenter.User;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.User, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /**************************************************************************************************
            * 遍历网页内容
            * **************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">", JSONHelper.ToString(Rs));
                strText.AppendFormat("<td><input operate=\"selected\" type=\"checkbox\" name=\"UserID\" value=\"{0}\" /></td>", Rs["UserID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["UserID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["UserName"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strText.AppendFormat("<td><img src=\"template/images/ico/{0}.gif\" /></td>", (Rs["isDisplay"].ToString() == "1" ? "yes" : "no"));
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************************************
             * 输出网页内容
             * *****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/user/storall.html");
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
        /// 站点管理
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 获取查询预设参数信息
             * ************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StartDate = RequestHelper.GetRequest("StartDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string isFast = RequestHelper.GetRequest("isFast").ToString();
            string ParentID = RequestHelper.GetRequest("ParentID").toString();
            /*****************************************************************************************
             * 开始构建网页展示内容
             * ***************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"16\">用户管理 >> 用户列表</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"search\">");
            strBuilder.Append("<td colspan=\"16\">");
            strBuilder.Append("<form action=\"?action=default\" method=\"get\">");
            strBuilder.Append("<select name=\"SearchType\">");
            strBuilder.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="UserID",Text="搜用户ID"},
                new OptionMode(){Value="Nickname",Text="搜用户昵称"}
            }, SearchType));
            strBuilder.Append("</select>");
            strBuilder.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strBuilder.Append(" 到期日期 <input type=\"text\" placeholder=\"请选择日期\" onClick=\"WdatePicker()\" isDate=\"true\" value=\"" + StartDate + "\" name=\"StartDate\" size=\"12\" class=\"inputtext\" />");
            strBuilder.Append(" - <input type=\"text\" placeholder=\"请选择日期\" onClick=\"WdatePicker()\" isDate=\"true\" value=\"" + EndDate + "\" name=\"EndDate\" size=\"12\" class=\"inputtext\" />");
            strBuilder.Append(" <select name=\"isFast\">");
            strBuilder.Append("<option value=\"\">所有用户</option>");
            strBuilder.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="today",Text="今日注册"},
                new OptionMode(){Value="online",Text="当前在线"},
                new OptionMode(){Value="disable",Text="禁用账户"},
            }, isFast));
            strBuilder.Append("</select>");
            strBuilder.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strBuilder.Append("<td width=\"80\">赚钱号(UID)</td>");
            strBuilder.Append("<td width=\"80\">用户昵称</td>");
            strBuilder.Append("<td width=\"100\">手机号</td>");
            strBuilder.Append("<td width=\"60\">直推好友</td>");
            strBuilder.Append("<td width=\"80\">粉丝数量</td>");
            strBuilder.Append("<td width=\"80\">设备类型</td>");
            //strBuilder.Append("<td width=\"80\">支付宝账号</td>");
            strBuilder.Append("<td width=\"80\">账户余额</td>");
            strBuilder.Append("<td width=\"80\">任务奖励</td>");
            strBuilder.Append("<td width=\"80\">提现金额</td>");
            strBuilder.Append("<td width=\"100\">注册日期</td>");
            strBuilder.Append("<td width=\"40\">状态</td>");
            strBuilder.Append("<td width=\"40\">越狱</td>");
            strBuilder.Append("<td width=\"60\">在线</td>");
            strBuilder.Append("<td width=\"100\">操作选项</td>");
            strBuilder.Append("</tr>");
            /************************************************************************************************
             * 构建分页查询语句条件
             * **********************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "userid": Params += " and userid like '%" + Keywords + "%'"; break;
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StartDate) && VerifyCenter.VerifyDateTime(StartDate)) { Params += " and Addtime>='" + new Fooke.Function.String(StartDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (!string.IsNullOrEmpty(isFast) && isFast == "today") { Params += " and Addtime>='" + DateTime.Now.ToString("yyyy-MM-dd 00:00:00") + "'"; }
            else if (!string.IsNullOrEmpty(isFast) && isFast == "online") { Params += " and isOnline=1"; }
            else if (!string.IsNullOrEmpty(isFast) && isFast == "disable") { Params += " and isDisplay=0"; }
            if (!string.IsNullOrEmpty(ParentID) && ParentID.isInt()) { Params += " and ParentID=" + ParentID + ""; }
            /*************************************************************************************************************************
             * 获取分页数据信息
             * ************************************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /*************************************************************************************************************************
             * 构建分页查询语句
             * ************************************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UserID,ParentID,isChild,DeviceType,DeviceModel,GroupID,Groupname,UserName,Nickname,Amount,Points,strMobile,strIP,strCity,BonusTimer,BonusAmount,AlipayTimer,AlipayAmount,Addtime,LastDate,isDisplay,isBreak,AlipayChar";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " UserID desc";
            PageCenterConfig.Tablename = "[Fun_FindUsers]";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("[Fun_FindUsers]", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*************************************************************************************************************************
             * 循环遍历网页内容
             * ************************************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.AppendFormat("<tr class=\"hback\">");
                strBuilder.AppendFormat("<td><input type=\"checkbox\" name=\"UserID\" value=\"{0}\" /></td>", Rs["UserID"]);
                strBuilder.AppendFormat("<td operate=\"looker\" url=\"user.aspx?action=looker&userid={0}\">{0}</td>", Rs["userid"]);
                strBuilder.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strBuilder.AppendFormat("<td>{0}</td>", Rs["strMobile"]);
                strBuilder.AppendFormat("<td>{0}</td>", Rs["isChild"]);
                strBuilder.AppendFormat("<td>{0}</td>", GetAllChild(Rs["UserID"].ToString()));
                strBuilder.AppendFormat("<td>{0}</td>", Rs["DeviceType"]);
                //strBuilder.AppendFormat("<td>{0}</td>", Rs["AlipayChar"]);
                strBuilder.AppendFormat("<td style=\"color:#009900\">{0}</td>", Rs["Amount"]);
                strBuilder.AppendFormat("<td style=\"color:#CD0000\">{0}</td>", Rs["BonusAmount"]);
                strBuilder.AppendFormat("<td style=\"color:#009900\">{0}</td>", Rs["AlipayAmount"]);
                strBuilder.AppendFormat("<td>{0}</td>", new Fooke.Function.String(Rs["Addtime"].ToString()).cDate().ToString("yyyy-MM-dd"));
                strBuilder.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1")
                { strBuilder.AppendFormat("<a href=\"?action=display&val=0&UserID={0}\"><img src=\"images/ico/yes.gif\"/></a>", Rs["UserID"]); }
                else { strBuilder.AppendFormat("<a href=\"?action=display&val=1&UserID={0}\"><img src=\"images/ico/no.gif\"/></a>", Rs["UserID"]); }
                strBuilder.AppendFormat("</td>");
                strBuilder.AppendFormat("<td>{0}</td>", (Rs["isBreak"].ToString() == "1" ? "<a class=\"vbtnRed\">是</a>" : "<a class=\"vbtn\">否</a>"));
                strBuilder.AppendFormat("<td>{0}</td>", (Rs["LastDate"].ToString().cDate() >= DateTime.Now.AddMinutes(-20) ? "<a class=\"vbtn\">在线</a>" : "<a class=\"vbtnRed\">离线</a>"));
                strBuilder.AppendFormat("<td>");
                strBuilder.AppendFormat("<a href=\"?action=edit&UserID={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["UserID"]);
                strBuilder.AppendFormat("<a href=\"?action=del&UserID={0}\"  title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["UserID"]);
                strBuilder.AppendFormat("</td>");
                strBuilder.AppendFormat("</tr>");
            }
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"16\">");
            strBuilder.Append(PageCenter.Often(Record, PageSize));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td colspan=\"16\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"允许登录(是)\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"允许登录(否)\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</form>");
            strBuilder.Append("</table>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/user/default.html");
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
        ///  统计用户所有下级数量
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        protected int GetAllChild(string UserID)
        {
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserChildrenAll]", new Dictionary<string, object>() {
                    {"NodeID",UserID}
                });
           int ChildrenNum = string.IsNullOrEmpty(sRs[0].ToString()) ? 0 : Convert.ToInt32(sRs[0].ToString());
           return ChildrenNum;
        }
        /// <summary>
        /// 添加
        /// </summary>
        protected void Add()
        {
            /**************************************************************************************************
             * 输出网页参数信息
             * ************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/user/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Text="允许登录",Value="1"},
                        new RadioMode(){Name="isDisplay",Text="禁止登陆",Value="0"},
                    }, "1"); break;
                    case "file": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileValue = "/file/user/default.png",
                        fileName = "Thumb",
                        tips = "请上传一张本地资源"
                    }); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑
        /// </summary>
        protected void Update()
        {
            /*********************************************************************************************
             * 获取用户资料信息
             * ********************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindMember", new Dictionary<string, object>() { { "UserID", UserID } });
            if (cRs == null) { this.ErrorMessage("拉取数据失败，你查找的信息不存在！"); Response.End(); }
            /*********************************************************************************************
             * 输出网页内容信息
             * ********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/user/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                       new RadioMode(){Name="isDisplay",Text="允许登录",Value="1"},
                       new RadioMode(){Name="isDisplay",Text="禁止登录",Value="0"},
                    }, cRs["isDisplay"].ToString()); break;
                    case "file": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileValue = cRs["thumb"].ToString(),
                        fileName = "Thumb",
                        tips = "请上传一张本地资源"
                    }); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
       

        /// <summary>
        /// 数据处理区域
        /// </summary>
        protected void AddSave()
        {
            /**************************************************************************************************************
             * 获取微信授权账户信息
             * ************************************************************************************************************/
            string AuthorizationType = RequestHelper.GetRequest("AuthorizationType").toString("Define");
            if (string.IsNullOrEmpty(AuthorizationType)) { this.ErrorMessage("获取第三方授权信息失败,请重试！"); Response.End(); }
            else if (AuthorizationType.Length <= 1) { this.ErrorMessage("第三方授权类型字段长度不能少于2个字符！"); Response.End(); }
            else if (AuthorizationType.Length >= 12) { this.ErrorMessage("第三方授权类型字段长度不能超过12个字符！"); Response.End(); }
            string AuthorizationKey = RequestHelper.GetRequest("AuthorizationKey").toString("DM" + Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(AuthorizationKey)) { this.ErrorMessage("获取第三方授权信息失败,请重试！"); Response.End(); }
            else if (AuthorizationKey.Length <= 12) { this.ErrorMessage("第三方授权标识字段长度不能少于12个字符！"); Response.End(); }
            else if (AuthorizationKey.Length >= 40) { this.ErrorMessage("第三方授权标识字段长度不能超过40个字符！"); Response.End(); }
            DataRow aRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"AuthorizationType",AuthorizationType},
                {"AuthorizationKey",AuthorizationKey}
            });
            if (aRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /**************************************************************************************************************
             * 获取用户设备类型信息
             * ************************************************************************************************************/
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").toString("Define");
            if (string.IsNullOrEmpty(DeviceModel)) { this.ErrorMessage("获取设备系统版本失败,请重试！"); Response.End(); }
            else if (DeviceModel.Length <= 2) { this.ErrorMessage("设备系统版本字段不能少于2个字符！"); Response.End(); }
            else if (DeviceModel.Length >= 12) { this.ErrorMessage("设备系统版本字段不能大于12个字符！"); Response.End(); }
            string DeviceType = RequestHelper.GetRequest("DeviceType").toString("Define");
            if (string.IsNullOrEmpty(DeviceType)) { this.ErrorMessage("获取设备类型信息失败,请重试！"); Response.End(); }
            else if (DeviceType.Length <= 2) { this.ErrorMessage("设备类型字段不能少于2个字符！"); Response.End(); }
            else if (DeviceType.Length >= 12) { this.ErrorMessage("设备类型字段不能大于12个字符！"); Response.End(); }
            string DeviceCode = RequestHelper.GetRequest("DeviceCode").toString(Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(DeviceCode)) { this.ErrorMessage("获取设备编号信息失败,请重试！"); Response.End(); }
            else if (DeviceCode.Length <= 12) { this.ErrorMessage("设备编号信息字段不能少于12个字符！"); Response.End(); }
            else if (DeviceCode.Length >= 40) { this.ErrorMessage("设备编号信息字段不能大于40个字符！"); Response.End(); }
            string DeviceIdentifier = RequestHelper.GetRequest("DeviceIdentifier").toString(Guid.NewGuid().ToString());
            if (string.IsNullOrEmpty(DeviceIdentifier)) { this.ErrorMessage("获取系统唯一设备编号失败,请重试!"); Response.End(); }
            else if (DeviceIdentifier.Length <= 10) { this.ErrorMessage("设备系统唯一设备编码长度不能少于10个字符！"); Response.End(); }
            else if (DeviceIdentifier.Length >= 40) { this.ErrorMessage("设备系统唯一设备编码长度不能超过40个字符！"); Response.End(); }
            DataRow dRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"DeviceIdentifier",DeviceIdentifier}
            });
            if (dRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /**************************************************************************************************************
             * 获取并验证MAC地址合法性
             * ************************************************************************************************************/
            string MacChar = RequestHelper.GetRequest("MacChar").toString(string.Format("DF{0}", DateTime.Now.Ticks.ToString()));
            if (string.IsNullOrEmpty(MacChar)) { this.ErrorMessage("获取设备MAC地址失败,请重试!"); Response.End(); }
            else if (MacChar.Length <= 10) { this.ErrorMessage("设备MAC地址长度不能少于10个字符！"); Response.End(); }
            else if (MacChar.Length >= 40) { this.ErrorMessage("设备MAC地址长度不能超过40个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 开始验证用户账户信息,并且验证登陆账号是否存在
             * ************************************************************************************************************/
            string UserName = RequestHelper.GetRequest("UserName").toString();
            if (string.IsNullOrEmpty(UserName)) { this.ErrorMessage("请填写用户登陆账号！"); Response.End(); }
            else if (UserName.Length <= 5) { this.ErrorMessage("登陆账号太短,不能少于5个字符！"); Response.End(); }
            else if (UserName.Length >= 20) { this.ErrorMessage("登陆账号太长,不能多于20个字符！"); Response.End(); }
            else if (VerifyCenter.VerifySpecific(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            else if (VerifyCenter.VerifyChina(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserName",UserName}
            });
            if (cRs != null) { this.ErrorMessage("登陆手机号码已经被注册了!"); Response.End(); }
            /**************************************************************************************************************
             * 验证上级用户,顶级帐号
             * ************************************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID != "0")
            {
                DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                    {"UserID",ParentID}
                });
                if (oRs == null) { this.ErrorMessage("设置的上级账户信息不存在,请重试！"); Response.End(); }
            }
            /**************************************************************************************************************
             * 验证其它的数据信息
             * ************************************************************************************************************/
            string Password = RequestHelper.GetRequest("password").toString("DuomiCMS");
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请填写你的登陆密码!"); Response.End(); }
            else if (Password.Length <= 5) { this.ErrorMessage("登陆密码不能少于6个字符!"); Response.End(); }
            else if (Password.Length > 16) { this.ErrorMessage("登陆密码不能大于16个字符"); Response.End(); }
            else { Password = MemberHelper.toPassword(Password); }
            /**************************************************************************************************************
             * 二级密码,资金密码
             * ************************************************************************************************************/
            string PasswordTo = RequestHelper.GetRequest("PasswordTo").toString("DuomiCMS");
            if (string.IsNullOrEmpty(PasswordTo)) { PasswordTo = RequestHelper.GetRequest("Password").toString(); }
            else if (PasswordTo.Length <= 5) { this.ErrorMessage("二级密码不能小于6个字符!"); Response.End(); }
            else if (PasswordTo.Length > 16) { this.ErrorMessage("二级密码不能大于16个字符!"); Response.End(); }
            else { PasswordTo = MemberHelper.toPassword(PasswordTo); }
            /**************************************************************************************************************
             * 验证用户昵称帐号是否唯一
             * ************************************************************************************************************/
            string Nickname = RequestHelper.GetRequest("Nickname").toString();
            if (string.IsNullOrEmpty(Nickname)) { this.ErrorMessage("请填写你的账户昵称!"); Response.End(); }
            else if (Nickname.Length <= 1) { this.ErrorMessage("账户昵称不能少于1个字符!"); Response.End(); }
            else if (Nickname.Length >= 16) { this.ErrorMessage("账户昵称不能大于16个字符!"); Response.End(); }
            /**************************************************************************************************************
             * 验证用户资料
             * ************************************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString("/file/user/default.png");
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("获取用户头像地址信息失败,请重试！"); Response.End(); }
            else if (Thumb.Length <= 10) { this.ErrorMessage("获取用户头像地址信息失败,请重试！"); Response.End(); }
            else if (Thumb.Length >= 255) { this.ErrorMessage("用户头像地址太长,请限制在255个字符以内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证用户微信昵称，真实姓名等
             * ************************************************************************************************************/
            string strWeChat = RequestHelper.GetRequest("strWeChat").ToString();
            if (strWeChat.Length >= 24) { this.ErrorMessage("微信昵称长度不能超过24个字符！"); Response.End(); }
            string Fullname = RequestHelper.GetRequest("Fullname").ToString();
            if (Fullname.Length >= 12) { this.ErrorMessage("真实姓名长度不能超过12个汉字！"); Response.End(); }
            /**************************************************************************************************************
             * 获取支付宝账号昵称数据信息
             * ************************************************************************************************************/
            string AlipayChar = RequestHelper.GetRequest("AlipayChar").ToString();
            if (AlipayChar.Length>=24) { this.ErrorMessage("支付宝账号长度不能超过24个字符!"); Response.End(); }
            string Alipayname = RequestHelper.GetRequest("Alipayname").ToString();
            if (Alipayname.Length >= 16) { this.ErrorMessage("支付宝昵称长度不能超过16个汉字！"); Response.End(); }
            /**************************************************************************************************************
             * 获取并验证用户账号信息
             * ************************************************************************************************************/
            string strEmail = RequestHelper.GetRequest("strEmail").ToString();
            if (strEmail.Length > 26) { this.ErrorMessage("邮箱地址字段长度请限制在26个字符以内！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strEmail) && !strEmail.isEmail()) { this.ErrorMessage("邮箱地址字段格式错误,请重试！"); Response.End(); }
            string strMobile = RequestHelper.GetRequest("strMobile").toString();
            if (!string.IsNullOrEmpty(strMobile) && strMobile.Length != 11) { this.ErrorMessage("手机号格式错误,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strMobile) && !strMobile.isMobile()) { this.ErrorMessage("手机号格式错误！"); Response.End(); }
            /**************************************************************************************************************
             * 获取管理员针对用户的备注信息
             * ************************************************************************************************************/
            string strRemark = RequestHelper.GetRequest("strRemark").ToString();
            if (strRemark.Length > 60) { this.ErrorMessage("备注字段内容长度请限制在60个汉字内！"); Response.End(); }
            /************************************************************************************************************
            * 获取无需验证的数据信息
            * ***********************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /************************************************************************************************************
            * 生成用户登陆标识,该标识记录用户登陆信息必须唯一
            * ***********************************************************************************************************/
            string strTokey = string.Format("用户注册-|-|-|-{0}-|-|-{1}-|-|-{2}-|-|-用户注册",
                DateTime.Now.ToString("yyyyMMddHHmmss"), Password, Guid.NewGuid().ToString());
            strTokey = new Fooke.Function.String(strTokey).ToMD5().Substring(0, 24).ToUpper();
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"strTokey",strTokey}
            });
            if (sRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /************************************************************************************************************
            * 生成用户登陆标识,该标识记录用户登陆信息必须唯一
            * ***********************************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["ParentID"] = ParentID;
            oDictionary["AuthorizationType"] = AuthorizationType;
            oDictionary["AuthorizationKey"] = AuthorizationKey;
            oDictionary["DeviceType"] = DeviceType;
            oDictionary["DeviceCode"] = DeviceCode;
            oDictionary["DeviceModel"] = DeviceModel;
            oDictionary["DeviceIdentifier"] = DeviceIdentifier;
            oDictionary["MacChar"] = MacChar;
            oDictionary["strTokey"] = strTokey;
            oDictionary["UserName"] = UserName;
            oDictionary["Password"] = Password;
            oDictionary["PasswordTo"] = PasswordTo;
            oDictionary["Nickname"] = Nickname;
            oDictionary["Thumb"] = Thumb;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["strMobile"] = strMobile;
            oDictionary["strEmail"] = strEmail;
            oDictionary["strWeChat"] = strWeChat;
            oDictionary["Fullname"] = Fullname;
            oDictionary["AlipayChar"] = AlipayChar;
            oDictionary["Alipayname"] = Alipayname;
            oDictionary["strRemark"] = strRemark;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUsers]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /**************************************************************************************************************
             * 输出数据处理结果信息
             * ************************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前页面！");
            Response.End();
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        protected void SaveUpdate()
        {
            /**********************************************************************************************************
             * 获取需要编辑的用户资料信息
             * ********************************************************************************************************/
            string UserID = RequestHelper.GetRequest("userID").toInt();
            if (UserID == "0") { this.ErrorMessage("请求参数错误,请返回重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindMember", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (Rs == null) { this.ErrorMessage("拉取用户信息失败！"); Response.End(); }
            /**************************************************************************************************************
             * 开始验证用户账户信息,并且验证登陆账号是否存在
             * ************************************************************************************************************/
            string UserName = RequestHelper.GetRequest("UserName").toString();
            if (string.IsNullOrEmpty(UserName)) { this.ErrorMessage("请填写用户登陆账号！"); Response.End(); }
            else if (UserName.Length <= 5) { this.ErrorMessage("登陆账号太短,不能少于5个字符！"); Response.End(); }
            else if (UserName.Length >= 20) { this.ErrorMessage("登陆账号太长,不能多于20个字符！"); Response.End(); }
            else if (VerifyCenter.VerifySpecific(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            else if (VerifyCenter.VerifyChina(UserName)) { this.ErrorMessage("登陆账号不支持特殊字符！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserName",UserName}
            });
            if (cRs != null && cRs["UserID"].ToString() != Rs["UserID"].ToString()) { this.ErrorMessage("登陆手机号码已经被注册了!"); Response.End(); }
            /**************************************************************************************************************
             * 验证上级用户,顶级帐号
             * ************************************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            //if (new Fooke.Function.String(ParentID).cInt() >= new Fooke.Function.String(UserID).cInt())
            //{ this.ErrorMessage("用户上级邀请ID不能大于当前的用户ID"); Response.End(); }
            //else if (ParentID != "0")
            if (ParentID != "0")
            {
                DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                    {"UserID",ParentID}
                });
                if (oRs == null) { this.ErrorMessage("设置的上级账户信息不存在,请重试！"); Response.End(); }
            }
            /**************************************************************************************************************
             * 验证用户登陆密码信息,默认为用户自己的密码
             * ************************************************************************************************************/
            string Password = RequestHelper.GetRequest("Password").toString();
            if (!string.IsNullOrEmpty(Password) && Password.Length <= 5) { this.ErrorMessage("登陆密码不能少于6个字符!"); Response.End(); }
            if (!string.IsNullOrEmpty(Password) && Password.Length > 16) { this.ErrorMessage("登陆密码不能大于16个字符"); Response.End(); }
            if (!string.IsNullOrEmpty(Password)) { Password = Fooke.Code.MemberHelper.toPassword(Password); }
            else { Password = Rs["Password"].ToString(); }
            /**************************************************************************************************************
            * 验证用户登陆密码信息,默认为用户自己的密码
            * ************************************************************************************************************/
            string PasswordTo = RequestHelper.GetRequest("PasswordTo").toString();
            if (!string.IsNullOrEmpty(PasswordTo) && PasswordTo.Length <= 5) { this.ErrorMessage("二级密码不能小于6个字符!"); Response.End(); }
            if (!string.IsNullOrEmpty(PasswordTo) && PasswordTo.Length > 16) { this.ErrorMessage("二级密码不能大于16个字符!"); Response.End(); }
            if (!string.IsNullOrEmpty(PasswordTo)) { PasswordTo = Fooke.Code.MemberHelper.toPassword(PasswordTo); }
            else { PasswordTo = Rs["PasswordTo"].ToString(); }
            /**************************************************************************************************************
             * 验证用户昵称帐号是否唯一
             * ************************************************************************************************************/
            string Nickname = RequestHelper.GetRequest("Nickname").toString();
            if (string.IsNullOrEmpty(Nickname)) { this.ErrorMessage("请填写你的账户昵称!"); Response.End(); }
            else if (Nickname.Length <= 1) { this.ErrorMessage("账户昵称不能少于1个字符!"); Response.End(); }
            else if (Nickname.Length >= 16) { this.ErrorMessage("账户昵称不能大于16个字符!"); Response.End(); }
            /**************************************************************************************************************
             * 验证用户资料
             * ************************************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString("/file/user/default.png");
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("获取用户头像地址信息失败,请重试！"); Response.End(); }
            else if (Thumb.Length <= 10) { this.ErrorMessage("获取用户头像地址信息失败,请重试！"); Response.End(); }
            else if (Thumb.Length >= 255) { this.ErrorMessage("用户头像地址太长,请限制在255个字符以内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证用户微信昵称，真实姓名等
             * ************************************************************************************************************/
            string strWeChat = RequestHelper.GetRequest("strWeChat").ToString();
            if (strWeChat.Length >= 24) { this.ErrorMessage("微信昵称长度不能超过24个字符！"); Response.End(); }
            string Fullname = RequestHelper.GetRequest("Fullname").ToString();
            if (Fullname.Length >= 12) { this.ErrorMessage("真实姓名长度不能超过12个汉字！"); Response.End(); }
            /**************************************************************************************************************
             * 获取支付宝账号昵称数据信息
             * ************************************************************************************************************/
            string AlipayChar = RequestHelper.GetRequest("AlipayChar").ToString();
            if (AlipayChar.Length >= 24) { this.ErrorMessage("支付宝账号长度不能超过24个字符!"); Response.End(); }
            string Alipayname = RequestHelper.GetRequest("Alipayname").ToString();
            if (Alipayname.Length >= 16) { this.ErrorMessage("支付宝昵称长度不能超过16个汉字！"); Response.End(); }
            /**************************************************************************************************************
             * 获取并验证用户账号信息
             * ************************************************************************************************************/
            string strEmail = RequestHelper.GetRequest("strEmail").ToString();
            if (strEmail.Length > 26) { this.ErrorMessage("邮箱地址字段长度请限制在26个字符以内！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strEmail) && !strEmail.isEmail()) { this.ErrorMessage("邮箱地址字段格式错误,请重试！"); Response.End(); }
            string strMobile = RequestHelper.GetRequest("strMobile").toString();
            if (!string.IsNullOrEmpty(strMobile) && strMobile.Length != 11) { this.ErrorMessage("手机号格式错误,请重试！"); Response.End(); }
            else if (!string.IsNullOrEmpty(strMobile) && !strMobile.isMobile()) { this.ErrorMessage("手机号格式错误！"); Response.End(); }
            /**************************************************************************************************************
             * 获取管理员针对用户的备注信息
             * ************************************************************************************************************/
            string strRemark = RequestHelper.GetRequest("strRemark").ToString();
            if (strRemark.Length > 60) { this.ErrorMessage("备注字段内容长度请限制在60个汉字内！"); Response.End(); }
            /************************************************************************************************************
            * 获取无需验证的数据信息
            * ***********************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /************************************************************************************************************
            * 生成用户登陆标识,该标识记录用户登陆信息必须唯一
            * ***********************************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["UserID"] = Rs["UserID"].ToString();
            oDictionary["ParentID"] = ParentID;
            oDictionary["AuthorizationType"] = Rs["AuthorizationType"].ToString();
            oDictionary["AuthorizationKey"] = Rs["AuthorizationKey"].ToString(); ;
            oDictionary["DeviceType"] = Rs["DeviceType"].ToString(); ;
            oDictionary["DeviceCode"] = Rs["DeviceCode"].ToString(); ;
            oDictionary["DeviceModel"] = Rs["DeviceModel"].ToString(); ;
            oDictionary["DeviceIdentifier"] = Rs["DeviceIdentifier"].ToString(); ;
            oDictionary["MacChar"] = Rs["MacChar"].ToString(); ;
            oDictionary["strTokey"] = Rs["strTokey"].ToString();
            oDictionary["UserName"] = UserName;
            oDictionary["Password"] = Password;
            oDictionary["PasswordTo"] = PasswordTo;
            oDictionary["Nickname"] = Nickname;
            oDictionary["Thumb"] = Thumb;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["strMobile"] = strMobile;
            oDictionary["strEmail"] = strEmail;
            oDictionary["strWeChat"] = strWeChat;
            oDictionary["Fullname"] = Fullname;
            oDictionary["AlipayChar"] = AlipayChar;
            oDictionary["Alipayname"] = Alipayname;
            oDictionary["strRemark"] = strRemark;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUsers]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("修改了用户资料" + Rs["Nickname"] + "(" + Rs["UserID"] + ")"); }
            catch { }
            /***************************************************************************
             * 输出数据处理结果信息
             * **************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前页面！");
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
            string strList = RequestHelper.GetRequest("UserID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.User, Params: " and UserID in (" + strList + ")");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证请求参数值信息
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            if (strValue != "0" && strValue != "1") { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            /***********************************************************************************************
            * 开始保存数据
            * *********************************************************************************************/
            DbHelper.Connection.Update(TableCenter.User, new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and UserID in (" + strList + ")");
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }   
        /// <summary>
        /// 删除用户信息
        /// </summary>
        protected void Delete()
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
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.User, Params: " and UserID in (" + strList + ") and isDisplay=0");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count<=0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始删除数据信息
            * *********************************************************************************************/
            DbHelper.Connection.Delete("Fooke_User", Params: " and UserID in (" + strList + ") and isDisplay=0");
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了用户数据(" + strList + ")"); }
            catch { }
            /***********************************************************************************************
            * 输出数据处理结果
            * *********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}