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
    public partial class UserAuthentication : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "stor": SelectorList(); Response.End(); break;
                case "edit": this.VerificationRole("实名认证"); Update(); Response.End(); break;
                case "editsave": this.VerificationRole("实名认证"); SaveUpdate(); Response.End(); break;
                case "default": this.VerificationRole("实名认证"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "display": this.VerificationRole("实名认证"); SaveDisplay(); Response.End(); break;
                case "looker": this.VerificationRole("实名认证"); strLooker(); Response.End(); break;
            }
        }
        /// <summary>
        /// 选择列表
        /// </summary>
        protected void SelectorList() 
        {
            /**************************************************************************************
            * 获取查询预设参数信息
            * ************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string isAuthentication = RequestHelper.GetRequest("isAuthentication").ToString();
            /**************************************************************************************************
            * 显示网页输出内容
            * **************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"3\">");
            strText.Append("<form id=\"frmForm\" OnSubmit=\"return _doPost(this);\" action=\"UserAuthentication.aspx\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Fullname",Text="搜姓名"},
                new OptionMode(){Value="IDnumber",Text="搜编号"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <select name=\"isAuthentication\">");
            strText.Append("<option value=\"\">认证状态</option>");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="0",Text="等待认证"},
                new OptionMode(){Value="1",Text="通过认证"},
                new OptionMode(){Value="2",Text="等待认证"},
                new OptionMode(){Value="100",Text="认证失败"}
            }, isAuthentication));
            strText.Append("</select>");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"80\">用户昵称</td>");
            strText.Append("<td>证件编号</td>");
            strText.Append("<td width=\"80\">状态</td>");
            strText.Append("</tr>");
            /*****************************************************************************************************
            * 构建分页查询语句条件
            * ***************************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "Fullname": strParameter += " and Fullname like '%" + Keywords + "%'"; break;
                    case "IDnumber": strParameter += " and IDnumber like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParameter += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParameter += " and Addtime<='" + EndDate + "'"; }
            if (!string.IsNullOrEmpty(isAuthentication) && isAuthentication.isInt()) { strParameter += " and isAuthentication=" + isAuthentication + ""; }
            /*****************************************************************************************************
            * 构建分页查询语句
            * ***************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UserID,Nickname,IDCategory,IDnumber,Fullname,isAuthentication";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " UserID Desc";
            PageCenterConfig.Tablename = "Fooke_UserAuthentication";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserAuthentication", strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
            * 循环遍历网页内容
            * ***************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">", JSONHelper.ToString(Rs));
                strText.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strText.AppendFormat("<td>{0}({1})</td>", Rs["IDnumber"],Rs["Fullname"]);
                strText.AppendFormat("<td>");
                switch (Rs["isAuthentication"].ToString())
                {
                    case "0": strText.Append("<font affairs=\"0\">等待审核</font>"); break;
                    case "1": strText.Append("<font affairs=\"2\">通过认证</font>"); break;
                    case "2": strText.Append("<font affairs=\"0\">等待审核</font>"); break;
                    case "100": strText.Append("<font affairs=\"100\">认证失败</font>"); break;
                }
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************************************
             * 输出网页内容
             * *****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/UserAuthentication/stor.html");
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
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string isAuthentication = RequestHelper.GetRequest("isAuthentication").ToString();
            /***********************************************************************************************************
             * 构建网页内容
             * *********************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">实名认证 >> 认证资料</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Fullname",Text="搜姓名"},
                new OptionMode(){Value="IDnumber",Text="搜编号"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <select name=\"isAuthentication\">");
            strText.Append("<option value=\"\">认证状态</option>");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="0",Text="等待认证"},
                new OptionMode(){Value="1",Text="通过认证"},
                new OptionMode(){Value="2",Text="等待认证"},
                new OptionMode(){Value="100",Text="认证失败"}
            }, isAuthentication));
            strText.Append("</select>");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"80\">证件类型</td>");
            strText.Append("<td width=\"130\">证件编号</td>");
            strText.Append("<td width=\"100\">真实姓名</td>");
            strText.Append("<td width=\"80\">认证状态</td>");
            strText.Append("<td>描述说明</td>");
            strText.Append("<td width=\"80\">操作选项</td>");
            strText.Append("</tr>");
           /*****************************************************************************************************
            * 构建分页查询语句条件
            * ***************************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "Fullname": strParameter += " and Fullname like '%" + Keywords + "%'"; break;
                    case "IDnumber": strParameter += " and IDnumber like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParameter += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParameter += " and Addtime<='" + EndDate + "'"; }
            if (!string.IsNullOrEmpty(isAuthentication) && isAuthentication.isInt()) { strParameter += " and isAuthentication=" + isAuthentication + ""; }
            /*****************************************************************************************************
            * 构建分页查询语句
            * ***************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UserID,Nickname,IDCategory,IDnumber,Fullname,FaceThumb,SideThumb,isAuthentication,Remark,LastDate";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " UserID Desc";
            PageCenterConfig.Tablename = "Fooke_UserAuthentication";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserAuthentication", strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
            * 循环遍历网页内容
            * ***************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"UserID\" value=\"{0}\" /></td>", Rs["UserID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["IDCategory"]);
                strText.AppendFormat("<td>{0}</td>",Rs["IDnumber"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Fullname"]);
                strText.AppendFormat("<td>");
                switch (Rs["isAuthentication"].ToString()) {
                    case "0": strText.Append("<font affairs=\"0\">等待审核</font>"); break;
                    case "1": strText.Append("<font affairs=\"2\">通过认证</font>"); break;
                    case "2": strText.Append("<font affairs=\"0\">等待审核</font>"); break;
                    case "100": strText.Append("<font affairs=\"100\">认证失败</font>"); break;
                }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>{0}</td>", Rs["Remark"].ToString());
                strText.AppendFormat("<td><input type=\"button\" onclick=\"window.location='?action=looker&Userid="+Rs["Userid"]+"'\" value=\"审核认证\" /></td>");
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(" <input type=\"button\" class=\"button\" value=\"删除数据\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"通过认证(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"100\" cmdText=\"display\" value=\"通过认证(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /*****************************************************************************************************
            * 输出网页内容
            * ***************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/UserAuthentication/default.html");
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
        protected void strLooker()
        {
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/UserAuthentication/looker.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName){}
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 编辑银行卡帐号
        /// </summary>
        protected void Update()
        {
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { Response.Write("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAuthentication]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { Response.Write("请求参数错误,你查找的数据不存在！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/UserAuthentication/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isAuthentication":
                        switch (cRs["isAuthentication"].ToString())
                        {
                            case "0": strValue=("<font affairs=\"0\">等待审核</font>"); break;
                            case "1": strValue = ("<font affairs=\"2\">通过认证</font>"); break;
                            case "2": strValue = ("<font affairs=\"0\">等待审核</font>"); break;
                            case "100": strValue = ("<font affairs=\"100\">认证失败</font>"); break;
                        }; break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

       

        /// <summary>
        /// 添加银行卡
        /// </summary>
        protected void SaveUpdate()
        {
            /*************************************************************************************************
             * 获取请求参数以及请求数据
             * ***********************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("请求参数错误,请选择用户！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAuthentication]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            /*********************************************************************************************************
             * 获取用户设定的认证结果信息
             * *******************************************************************************************************/
            string isAuthentication = RequestHelper.GetRequest("Affairs").toInt();
            if (isAuthentication != "1" && isAuthentication != "100")
            { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            /*********************************************************************************************************
            * 获取备注请求数据信息
            * *******************************************************************************************************/
            string Remark = RequestHelper.GetRequest("Remark").ToString();
            if (Remark.Length >= 60) { this.ErrorMessage("备注字段信息长度请限制在60个字符内！"); Response.End(); }
            /*********************************************************************************************************
             * 开始保存请求数据信息
             * *******************************************************************************************************/
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserAuthenticationAffairs]", new Dictionary<string, object>()
            {
                {"UserID",cRs["UserID"].ToString()},
                {"Affairs",isAuthentication},
                {"Remark",Remark},
            });
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /********************************************************************************************************
             * 输出网页数据信息
             * ******************************************************************************************************/
            this.ErrorMessage("数据保存成功!", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 删除银行卡信息
        /// </summary>
        protected void Delete()
        {
            string strList = RequestHelper.GetRequest("UserID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /**************************************************************************************
             * 开始处理数据
             * ************************************************************************************/
            DbHelper.Connection.Delete(tablename: "Fooke_UserAuthentication",
                Params: " and UserID in (" + strList + ")");
            /**************************************************************************************
             * 返回处理结果
             * ************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 保存银行卡审核状态信息
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
             * 验证请求参数值信息
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            if (strValue != "1" && strValue != "100") { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            /**************************************************************************************
            * 查询出需要处理的请求数据
            * ************************************************************************************/
            DataTable thisTab = DbHelper.Connection.FindTable("Fooke_UserAuthentication", Params: " and UserID in (" + strList + ")");
            if (thisTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的请求数据！"); Response.End(); }
            /**************************************************************************************
            * 开始遍历保存请求的数据
            * ************************************************************************************/
            foreach (DataRow cRs in thisTab.Rows)
            {
                DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserAuthenticationAffairs]", new Dictionary<string, object>() {
                    {"UserID",cRs["UserID"].ToString()},
                    {"Affairs",strValue},
                    {"Remark",cRs["Remark"].ToString()}
                });
                if (oRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            }
            /**************************************************************************************
             * 返回处理结果
             * ************************************************************************************/
            this.History();
            Response.End();
        }
    }
}