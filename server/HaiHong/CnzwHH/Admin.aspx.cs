using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using Fooke.Code;
using Fooke.Function;
namespace Fooke.Web.Admin
{
    public partial class Admin : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /****************************************************************
             * 验证操作权限
             * **************************************************************/
            if (AdminRs != null && AdminRs["PowerID"].ToString() != "0" && (strRequest != "password" && strRequest != "editpassword"))
            {this.ErrorMessage("越权操作！当前模块只允许超级管理员才能编辑！"); Response.End();}
            /******************************************************************
             * 开始执行页面输出
             * ****************************************************************/
            switch (this.strRequest)
            {
                case "add": this.AddAdmin(); Response.End(); break;
                case "addsave": this.AddAdminSave(); Response.End(); break;
                case "edit": this.UpdateAdmin(); Response.End(); break;
                case "editsave": this.UpdateAdminSave(); Response.End(); break;
                case "del": this.DeleteAdmin(); Response.End(); break;
                case "display": this.DisplayAdmin(); Response.End(); break;
                case "password": this.UpdatePassword(); Response.End(); break;
                case "editpassword": this.UpdatePasswordSave(); Response.End(); break;
                case "addpower": this.AddPower(); Response.End(); break;
                case "addpowersave": this.AddPowerSave(); Response.End(); break;
                case "editpower": this.UpdatePower(); Response.End(); break;
                case "delpower": this.DeletePower(); Response.End(); break;
                case "editpowersave": this.UpdatePowerSave(); Response.End(); break;
                case "power": PowerList(); Response.End(); break;
                case "operate": strOperate(); Response.End(); break;
                default: this.AdminList(); Response.End(); break;
            }
        }
        /// <summary>
        /// 系统操作日志
        /// </summary>
        protected void strOperate()
        {
            /****************************************************************************************
             * 获取查询参数条件信息
             * **************************************************************************************/
            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string AdminID = RequestHelper.GetRequest("AdminID").toInt();
            /****************************************************************************************************************
             * 构建网页内容信息
             * **************************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"3\">操作日志 >> 日志列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form id=\"SearchForm\" OnSubmit=\"return _doPost(this);\" action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"operate\">");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="adminname",Text="搜管理员账号"},
                new OptionMode(){Value="remark",Text="搜描述内容"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写要搜索的关键词\" type=\"text\" size=\"15\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            /****************************************************************************************************************
            * 网页主题内容信息
            * **************************************************************************************************************/
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"120\">管理员账号</td>");
            strText.Append("<td width=\"120\">操作时间</td>");
            strText.Append("<td>描述信息</td>");
            strText.Append("</tr>");
            /****************************************************************************************************************
            * 构建分页查询语句
            * **************************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "adminname": Params += " and adminname like '%" + Keywords + "%'"; break;
                    case "remark": Params += " and remark like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:00") + "'"; }
            if (AdminID != "0") { Params += " and AdminID=" + AdminID + ""; }
            /****************************************************************************************************************
             * 构建分页查询语句
             * **************************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "OperationID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " OperationID Desc";
            PageCenterConfig.Tablename = "[Fooke_AdminOperation]";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("[Fooke_AdminOperation]", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><a href=\"?action=default&adminId="+Rs["AdminID"]+"\">" + Rs["Adminname"] + "</a></td>");
                strText.Append("<td>" + Rs["addtime"] + "</td>");
                strText.Append("<td>"+Rs["Remark"]+"</td>");
                strText.Append("</tr>");
            }
            /****************************************************************************************************************
             * 构建分页查询语句
             * **************************************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"3\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            /****************************************************************************************************************
             * 解析并输出网页处理信息
             * **************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/admin/operate.html");
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
        /// 获取权限选项的options
        /// </summary>
        /// <param name="defaultTxt"></param>
        /// <returns></returns>
        protected string PowerOption(string defaultTxt="")
        {
            StringBuilder strBuilder = new StringBuilder();
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Power, columns: "PowerID,PowerName");
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<option value=\"" + Rs["PowerID"] + "\"");
                if (Rs["PowerID"].ToString() == defaultTxt) { strBuilder.Append(" selected"); }
                strBuilder.Append(">");
                strBuilder.Append(Rs["PowerName"].ToString());
                strBuilder.Append("</option>");
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 显示管理员列表
        /// </summary>
        protected void AdminList()
        {
            /***********************************************************************************************************
            * 显示网页内容数据信息
            * *********************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">账号管理 >> 帐号列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"140\">登录账号</td>");
            strText.Append("<td width=\"140\">注册时间</td>");
            strText.Append("<td width=\"140\">最后活动时间</td>");
            strText.Append("<td width=\"80\">登录次数</td>");
            strText.Append("<td width=\"140\">多人登录</td>");
            strText.Append("<td width=\"140\">允许登陆</td>");
            strText.Append("<td>操作选项</td>");
            strText.Append("</tr>");
            /***********************************************************************************************************
             * 构建分页查询语句
             * *********************************************************************************************************/
            int Page = RequestHelper.GetPage();
            int Record = 0;
            DataTable Tab = this.GetAdmin("", Page, out Record);
            /***********************************************************************************************************
            * 循环输出网页内容信息
            * *********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"AdminId\" value=\"" + Rs["AdminId"] + "\" /></td>");
                strText.Append("<td>");
                strText.Append("" + Rs["adminname"] + "");
                strText.Append("</td>");
                strText.Append("<td>" + Rs["addtime"] + "</td>");
                strText.Append("<td>" + Rs["LastDate"] + "</td>");
                strText.Append("<td>" + Rs["Hits"] + "</td>");
                strText.Append("<td>");
                if (Rs["isMary"].ToString() == "0") { strText.Append("<a class=\"vbtnRed\">允许多人(否)</a>"); }
                else { strText.Append("<a class=\"vbtn\">允许多人(是)</a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                if (Rs["isLock"].ToString() == "1") { strText.Append("<a href=\"?action=display&adminId=" + Rs["adminId"] + "&val=0\" class=\"vbtnRed\">允许登陆(否)</a>"); }
                else { strText.Append("<a href=\"?action=display&adminId=" + Rs["adminId"] + "&val=1\" class=\"vbtn\">允许登陆(是)</a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=edit&adminId=" + Rs["adminId"] + "\" class=\"vbtn\">编辑</a>");
                strText.Append("<a href=\"?action=del&adminId=" + Rs["adminId"] + "\" class=\"vbtnRed\" operate=\"delete\">删除</a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            /***********************************************************************************************************
            * 显示分页控件数据信息
            * *********************************************************************************************************/
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            /***********************************************************************************************************
            * 批量操作数据信息
            * *********************************************************************************************************/
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"8\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除选中账号\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"禁用账号(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"禁用账号(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /***********************************************************************************************************
            * 输出网页处理结果
            * *********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/admin/default.html");
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
        /// 角色管理列表
        /// </summary>
        protected void PowerList() 
        {
            /***********************************************************************************************************
            * 构建网页数据内容信息
            * *********************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"4\">角色管理</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"18%\">角色名称</td>");
            strText.Append("<td>管理权限</td>");
            strText.Append("<td width=\"10%\">操作选项</td>");
            strText.Append("</tr>");
            /***********************************************************************************************************
            * 构建分页数据查询语句
            * *********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = string.Empty;
            PageCenterConfig.Identify = "PowerID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " PowerID Desc";
            PageCenterConfig.Tablename =TableCenter.Power;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Power, string.Empty);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************************
            * 输出网页内容信息
            * *********************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"PowerID\" value=\"" + Rs["PowerID"] + "\" /></td>");
                strText.Append("<td>" + Rs["PowerName"] + "</td>");
                strText.Append("<td>" + Rs["PowerXML"] + "</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=editpower&PowerID=" + Rs["PowerID"] + "\" title=\"编辑角色\"><img src=\"template/images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=delpower&PowerID=" + Rs["PowerID"] + "\" title=\"删除角色\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            /***********************************************************************************************************
            * 构建网页处理操作信息
            * *********************************************************************************************************/
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td colspan=\"4\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"4\">");
            strText.Append("<input type=\"button\" class=\"button\" cmdText=\"delpower\" textMessage=\"数据删除以后将无法恢复,你确定要删除?\" value=\"删除\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /***********************************************************************************************************
            * 输出数据处理结果信息
            * *********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/admin/powerList.html");
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
        /// 添加管理员
        /// </summary>
        protected void AddAdmin() 
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/admin/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isLock": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Name="isLock",Value="1",Text="禁止登录",Checked="0"}
                    }); break;
                    case "isMary": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isMary",Value="1",Text="允许多人(是)"},
                        new RadioMode(){Name="isMary",Value="0",Text="允许多人(否)"}
                    }, "1"); break;
                    case "isMat": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isMat",Value="1",Text="限制"},
                        new RadioMode(){Name="isMat",Value="0",Text="不限制"}
                    }, "1"); break;
                    case "PowerOption": strValue = PowerOption(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        protected void AddPower()
        {

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/admin/addpower.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName) { }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑角色
        /// </summary>
        protected void UpdatePower()
        {
            string PowerID = RequestHelper.GetRequest("PowerID").toInt();
            if (PowerID == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Power, Params: " and PowerID = " + PowerID + "");
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/admin/editpower.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 编辑管理员
        /// </summary>
        protected void UpdateAdmin()
        {
            /*************************************************************************************************************
             * 获取管理员账号参数信息
             * ***********************************************************************************************************/
            string adminId = RequestHelper.GetRequest("adminId").toInt();
            if (adminId == "0") { this.ErrorMessage("参数错误，请返回重试！"); }
            DataRow Rs = DbHelper.Connection.FindRow(TableCenter.Admin, Params: " and adminId=" + adminId + "");
            if (Rs == null) { this.ErrorMessage("系统没有找到你需要的数据！"); }
            /*************************************************************************************************************
             * 输出数据处理结果
             * ***********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/admin/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isLock": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Name="isLock",Value="1",Text="禁止登录",Checked=Rs["isLock"].ToString()}
                    }); break;
                    case "isMary": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isMary",Value="1",Text="允许多人(是)"},
                        new RadioMode(){Name="isMary",Value="0",Text="允许多人(否)"}
                    }, Rs["isMary"].ToString()); break;
                    case "isMat": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isMat",Value="1",Text="限制"},
                        new RadioMode(){Name="isMat",Value="0",Text="不限制"}
                    }, Rs["isMat"].ToString()); break;
                    case "PowerOption": strValue = PowerOption(Rs["PowerID"].ToString()); break;
                    default:
                        try { strValue = Rs[funName].ToString(); }
                        catch { }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 修改登录密码
        /// </summary>
        protected void UpdatePassword() 
        {

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/admin/password.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default:
                        try { strValue = AdminRs[funName].ToString(); }
                        catch { }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 保存添加的角色
        /// </summary>
        protected void AddPowerSave()
        {
            string PowerName = RequestHelper.GetRequest("PowerName").toString();
            if (string.IsNullOrEmpty(PowerName)) { this.ErrorMessage("请输入角色名称！"); Response.End(); }
            if (PowerName.Length > 30) { this.ErrorMessage("角色名称请限制在30个字符以内！"); Response.End(); }
            string PowerXML = RequestHelper.GetRequest("PowerXML").toString();
            if (string.IsNullOrEmpty(PowerXML)) { this.ErrorMessage("请至少为该角色选择一个权限！"); Response.End(); }
            DataRow oRs = DbHelper.Connection.FindRow(TableCenter.Power, Params: " and PowerName='" + PowerName + "'");
            if (oRs != null) { this.ErrorMessage("相同的角色名称已经存在，请返回重试！"); Response.End(); }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["Powername"] = PowerName;
            dictionary["PowerXML"] = PowerXML;
            DbHelper.Connection.Insert(TableCenter.Power, dictionary);
            this.ErrorMessage("系统角色添加成功，点击确定将继续停留在当前页面！");
            Response.End();
        }

        protected void UpdatePowerSave() 
        {

            string PowerID = RequestHelper.GetRequest("PowerID").toInt();
            if (PowerID == "0") { this.ErrorMessage("请求参数错误，你查找的数据不存在！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Power, Params: " and PowerID = " + PowerID + "");
            if (cRs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }

            string PowerName = RequestHelper.GetRequest("PowerName").toString();
            if (string.IsNullOrEmpty(PowerName)) { this.ErrorMessage("请输入角色名称！"); Response.End(); }
            if (PowerName.Length > 30) { this.ErrorMessage("角色名称请限制在30个字符以内！"); Response.End(); }
            string PowerXML = RequestHelper.GetRequest("PowerXML").toString();
            if (string.IsNullOrEmpty(PowerXML)) { this.ErrorMessage("请至少为该角色选择一个权限！"); Response.End(); }
            DataRow oRs = DbHelper.Connection.FindRow(TableCenter.Power, Params: " and PowerID<>" + PowerID + " and PowerName='" + PowerName + "'");
            if (oRs != null) { this.ErrorMessage("相同的角色名称已经存在，请返回重试！"); Response.End(); }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["Powername"] = PowerName;
            dictionary["PowerXML"] = PowerXML;
            DbHelper.Connection.Update(TableCenter.Power, dictionary, Params: " and PowerID = " + cRs["PowerID"] + "");
            this.ErrorMessage("数据保存成功，点击确定将继续停留在当前页面！");
            Response.End(); 
        }

        /// <summary>
        /// 保存添加管理员
        /// </summary>
        protected void AddAdminSave() 
        {
           
            string adminname = RequestHelper.GetRequest("adminname").toString();
            string Password = RequestHelper.GetRequest("password").toString();
            string PasswordChar = RequestHelper.GetRequest("password1").toString();
            string isLock = RequestHelper.GetRequest("isLock").toInt();
            string isMary = RequestHelper.GetRequest("isMary").toInt();
            if (string.IsNullOrEmpty(adminname)) { this.ErrorMessage("请输入登录账号！"); }
            if (adminname.Length < 6 || adminname.Length > 16) { this.ErrorMessage("管理员登录帐号请限制在6-15个字符以内！"); Response.End(); }
            if (string.IsNullOrEmpty(Password)) { this.ErrorMessage("请输入登录密码！"); }
            if (Password.Length < 6 || Password.Length > 16) {this.ErrorMessage("管理员登录密码请设定在6-16个字符之间！");}
            if (Password != PasswordChar) { this.ErrorMessage("确认密码错误！"); }
            string PowerID = RequestHelper.GetRequest("PowerID").toInt();
            string PowerName = "超级管理员";
            if (PowerID != "0")
            {
                DataRow oRs = DbHelper.Connection.FindRow(TableCenter.Power, columns: "PowerName", Params: " and PowerID=" + PowerID + "");
                if (oRs != null) { PowerName = oRs["PowerName"].ToString(); }
                else { this.ErrorMessage("你选择的权限不存在，请重新选择！"); Response.End(); }
            }
            string isMat = RequestHelper.GetRequest("isMat").toInt();
            string effIP = RequestHelper.GetRequest("effIP").toString();
            string vPassword = RequestHelper.GetRequest("vPassword").toString();
            if (isMat == "1" && string.IsNullOrEmpty(effIP)) { this.ErrorMessage("请设置允许登录的IP地址！"); Response.End(); }
            if (isMat == "1" && string.IsNullOrEmpty(vPassword)) { this.ErrorMessage("请输入IP验证登录密码！"); Response.End(); }
            if (isMat == "1" && vPassword.Length < 6) { this.ErrorMessage("验证密码必须大于6位数！"); Response.End(); }
            /***********************************************************************
             * 验证登录Key
             * *********************************************************************/
            string strKey = "管理员-|-|-" + adminname + "-|-|-" + Password;
            strKey = new Fooke.Function.String(strKey).ToMD5().ToLower();
            /************************************************************************
             * 加密字符串
             * **********************************************************************/
            Password = new Fooke.Function.String(Password).ToMD5().ToMD5().Substring(0, 20).ToUpper();
            DataRow Rs = DbHelper.Connection.FindRow(TableCenter.Admin, Params: " and adminname='" + adminname + "'");
            if (Rs != null) { this.ErrorMessage("管理员账号已经存在！"); }
            /*************************************************************************
             * 开始插入数据
             * ***********************************************************************/
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["AdminName"] = adminname;
            dictionary["Password"] = Password;
            dictionary["strKey"] = strKey;
            dictionary["PowerID"] = PowerID;
            dictionary["PowerName"] = PowerName;
            dictionary["isMary"] = isMary;
            dictionary["isMat"] = isMat;
            dictionary["effIP"] = effIP;
            dictionary["vPassword"] = vPassword;
            dictionary["AddTime"] = DateTime.Now.ToString();
            dictionary["LastDate"] = DateTime.Now.ToString();
            dictionary["Hits"] = "0";
            dictionary["isLock"] = isLock;
            dictionary["strIP"] = FunctionCenter.GetCustomerIP();
            dictionary["strDesc"] = "";
            DbHelper.Connection.Insert(TableCenter.Admin, dictionary);
            this.ConfirmMessage("恭喜，管理员账号添加成功，点击确定继续添加！", "history.go(-1);", "?action=default");
            Response.End();
        }

        /// <summary>
        /// 保存编辑管理员
        /// </summary>
        protected void UpdateAdminSave()
        {
            
            string adminId = RequestHelper.GetRequest("adminId").toInt();
            string adminname = RequestHelper.GetRequest("adminname").toString();
            string Password = RequestHelper.GetRequest("password").toString();
            string PasswordChar = RequestHelper.GetRequest("password1").toString();
            string isLock = RequestHelper.GetRequest("isLock").toInt();
            string isMary = RequestHelper.GetRequest("isMary").toInt();
            if (adminId == "0") { this.ErrorMessage("参数错误，请返回重试！"); }
            if (string.IsNullOrEmpty(adminname)) { this.ErrorMessage("请输入登录账号！"); }
            if (!string.IsNullOrEmpty(Password))
            {
                if (Password.Length < 6 || Password.Length > 16) { this.ErrorMessage("管理员登录密码请设定在6-16个字符之间！"); }
                if (Password != PasswordChar) { this.ErrorMessage("确认密码错误！"); }
                Password = new Fooke.Function.String(Password).ToMD5().ToMD5().Substring(0, 20).ToUpper();
            }
            string PowerName = "超级管理员";
            string PowerID = RequestHelper.GetRequest("PowerID").toInt();
            if (PowerID != "0")
            {
                DataRow oRs = DbHelper.Connection.FindRow(TableCenter.Power,columns:"PowerName", Params: " and PowerID=" + PowerID + "");
                if (oRs != null) { PowerName = oRs["PowerName"].ToString(); }
                else { this.ErrorMessage("你选择的权限不存在，请重新选择！"); Response.End(); }
            }
            string isMat = RequestHelper.GetRequest("isMat").toInt();
            string effIP = RequestHelper.GetRequest("effIP").toString();
            string vPassword = RequestHelper.GetRequest("vPassword").toString();
            if (isMat == "1" && string.IsNullOrEmpty(effIP)) { this.ErrorMessage("请设置允许登录的IP地址！"); Response.End(); }
            if (isMat == "1" && string.IsNullOrEmpty(vPassword)) { this.ErrorMessage("请输入IP验证登录密码！"); Response.End(); }
            if (isMat == "1" && vPassword.Length<6) { this.ErrorMessage("验证密码必须大于6位数！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.FindRow(TableCenter.Admin, Params: " and adminname='" + adminname + "' and adminId<>"+adminId+"");
            if (Rs != null) { this.ErrorMessage("管理员账号已经存在！"); }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["AdminName"] = adminname;
            dictionary["isLock"] = isLock;
            dictionary["isMat"] = isMat;
            dictionary["effip"] = effIP;
            dictionary["vPassword"] = vPassword;
            dictionary["isMary"] = isMary;
            dictionary["PowerID"] = PowerID;
            dictionary["PowerName"] = PowerName;
            if (!string.IsNullOrEmpty(Password)) { dictionary["Password"] = Password; }
            DbHelper.Connection.Update(TableCenter.Admin, dictionary, Params: " and adminId=" + adminId + "");
            this.ConfirmMessage("恭喜，管理员账号编辑成功，点击确定继续编辑！", "history.go(-1);", "?action=default");
            Response.End();
        }
        /// <summary>
        /// 修改登录密码
        /// </summary>
        protected void UpdatePasswordSave()
        {
            string oldPassword = RequestHelper.GetRequest("oldPassword").toString();
            string password = RequestHelper.GetRequest("password").toString();
            string password1 = RequestHelper.GetRequest("password1").toString();
            if (oldPassword == "") { this.ErrorMessage("请输入旧密码！"); }
            oldPassword = new Fooke.Function.String(oldPassword).ToMD5().ToMD5().Substring(0, 20).ToUpper();
            if (oldPassword != this.AdminRs["Password"].ToString()) { this.ErrorMessage("旧密码错误！"); }
            if (password == "") { this.ErrorMessage("请输入要重新设置的密码！"); }
            if (password1 == "") { this.ErrorMessage("请输入确认密码！"); }
            if (password1 != password) { this.ErrorMessage("确认密码和新密码不一致！"); }
            password = new Fooke.Function.String(password).ToMD5().ToMD5().Substring(0, 20).ToUpper();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["password"] = password;
            DbHelper.Connection.Update(TableCenter.Admin, dictionary, Params: " and AdminId=" + this.AdminRs["AdminId"] + "");
            Response.Redirect("login.aspx");
            Response.End();
        }
        /// <summary>
        /// 删除角色权限
        /// </summary>
        protected void DeletePower()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("PowerID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_Power", Params: " and PowerID in (" + strList + ")");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始删除数据信息
            * *********************************************************************************************/
            DbHelper.Connection.Delete("Fooke_Power", Params: " and PowerID in (" + strList + ") and isLock=1");
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了管理员权限ID(" + strList + ")"); }
            catch { }
            /***********************************************************************************************
            * 输出数据处理结果
            * *********************************************************************************************/
            this.History();
            Response.End();
        }

        /// <summary>
        /// 删除管理员账号
        /// </summary>
        protected void DeleteAdmin()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AdminID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Admin, Params: " and AdminID in (" + strList + ") and isLock=1");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始删除数据信息
            * *********************************************************************************************/
            DbHelper.Connection.Delete("Fooke_Admin", Params: " and AdminID in (" + strList + ") and isLock=1");
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了管理员账号ID(" + strList + ")"); }
            catch { }
            /***********************************************************************************************
            * 输出数据处理结果
            * *********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 更改管理员状态
        /// </summary>
        protected void DisplayAdmin()
        {
            /***********************************************************************************************
              * 验证参数合法性
              * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AdminID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_Admin", Params: " and AdminID in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_Admin", new Dictionary<string, string>() {
                {"isLock",strValue}
            }, Params: " and AdminID in (" + strList + ")");
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}