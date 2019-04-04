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
    public partial class AppClass : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "edit": this.VerificationRole("应用分类"); Update(); Response.End(); break;
                case "add": this.VerificationRole("应用分类"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("应用分类"); SaveUpdate(); Response.End(); break;
                case "save": this.VerificationRole("应用分类"); AddSave(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "display": this.VerificationRole("应用分类"); SaveDisplay(); Response.End(); break;
                case "editor": this.VerificationRole("应用分类"); SaveEditor(); Response.End(); break;
                case "default": this.VerificationRole("应用分类"); strDefault(); Response.End(); break;
            }
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
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            /**************************************************************************************
             * 构建网页内容
             * *************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"7\">应用分类 >> 分类列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Classname",Text="搜名称"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"搜索关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">分类ID</td>");
            strText.Append("<td width=\"140\">分类名称</td>");
            strText.Append("<td width=\"100\">是否显示</td>");
            strText.Append("<td width=\"100\">显示排序</td>");
            strText.Append("<td>描述备注</td>");
            strText.Append("<td width=\"100\">操作选项</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************/
            string strParams = " and ParentID=" + ParentID + "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "Classname": strParams += " and classname like '%" + Keywords + "%'"; break;
                }
            }
            /***********************************************************************************************
            * 构建数据查询语句
            * **********************************************************************************************/
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append("(");
            strTabs.Append("    select ClassID,ParentID,Classname,isDisplay,SortID,Remark,");
            strTabs.Append("    (select count(0) from Fooke_AppClass where ParentID=AppClass.ClassID) as isChild");
            strTabs.Append("    from Fooke_AppClass as AppClass");
            strTabs.Append(") as FokeApps");
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
            PageCenterConfig.Identify = "ClassID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID Desc,ClassID asc";
            PageCenterConfig.Tablename = strTabs.ToString();
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(strTabs.ToString(), strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************
            * 循环遍历网页内容
            * **********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"ClassID\" value=\"" + Rs["ClassID"] + "\" /></td>");
                strText.Append("<td>" + Rs["ClassID"] + "</td>");
                strText.Append("<td>");
                if (Rs["isChild"].ToString() != "0") { strText.Append("<img onclick=\"window.location='?parentid="+Rs["classId"]+"'\" src=\"template/images/b.gif\" />"); }
                else { strText.Append("<img src=\"template/images/s.gif\" />"); }
                strText.Append("" + Rs["ClassName"] + "");
                strText.Append("</td>");
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&ClassID=" + Rs["ClassID"] + "\"><img src=\"template/images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&ClassID=" + Rs["ClassID"] + "\"><img src=\"template/images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                strText.Append("<td><input type=\"text\" operate=\"edit\" isnumeric=\"true\" url=\"?action=editor&ClassID=" + Rs["ClassID"] + "\" size=\"5\" class=\"inputtext center\" value=\"" + Rs["SortID"] + "\" /></td>");
                strText.Append("<td>" + Rs["Remark"] + "</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=edit&ClassID=" + Rs["ClassID"] + "\" title=\"编辑分组信息\"><img src=\"template/images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=del&ClassID=" + Rs["ClassID"] + "\"  title=\"删除分组信息\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            /***********************************************************************************************
            * 构建分页控件信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
            * 构建操作按钮信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除分类\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"正常显示(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"正常显示(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /***********************************************************************************************
            * 输出网页信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/AppClass/default.html");
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
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/AppClass/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isDisplay",Value="1",Text="允许显示(是)"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="允许显示(否)"}}, "1"); break;
                    case "options": strValue = new Fooke.Code.AppClassHelper().Options(defaultText:RequestHelper.GetRequest("ParentID").toInt()); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        
        /// <summary>
        /// 编辑用户组
        /// </summary>
        protected void Update()
        {
            /***********************************************************************************************
            * 获取请求参数信息
            * **********************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            if (ClassID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppClass]", new Dictionary<string, object>() {
                {"ClassID",ClassID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /***********************************************************************************************
            * 解析网页模板信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/appClass/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isDisplay",Value="1",Text="允许显示(是)"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="允许显示(否)"}},
                        Rs["isDisplay"].ToString()); break;
                    case "options": strValue = new Fooke.Code.AppClassHelper().Options(defaultText:Rs["ParentID"].ToString()); break;
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        
        /// <summary>
        /// 保存用户等级
        /// </summary>
        protected void AddSave()
        {
            /*******************************************************************************************************
             * 获取选择的上级分类ID
             * *****************************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (new Fooke.Function.String(ParentID).cInt() < 0) { this.ErrorMessage("获取上级分类ID信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0" && new Fooke.Function.String(ParentID).cInt() <= 99999) { this.ErrorMessage("获取上级分类ID信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0")
            {
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppClass]", new Dictionary<string, object>() { 
                    {"ClassID",ParentID}
                });
                if (sRs == null) { this.ErrorMessage("获取上级分类信息失败,请重试!"); Response.End(); }
            }
            /*******************************************************************************************************
             * 获取用户等级信息
             * *****************************************************************************************************/
            string Classname = RequestHelper.GetRequest("Classname").ToString();
            if (string.IsNullOrEmpty(Classname)) { this.ErrorMessage("请填写服务分类名称！"); Response.End(); }
            else if (Classname.Length <= 0) { this.ErrorMessage("请填写服务分类名称！"); Response.End(); }
            else if (Classname.Length >= 20) { this.ErrorMessage("分类名称长度不能超过20个汉字！"); Response.End(); }
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppClass]", new Dictionary<string, object>() { 
                {"ParentID",ParentID},
                {"Classname",Classname}
            });
            if (oRs != null) { this.ErrorMessage("同目录下分类名称已经存在！"); Response.End(); }
            /*******************************************************************************************************
             * 获取其他不需要验证的数据
             * *****************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string Remark = RequestHelper.GetRequest("Remark").ToString();
            if (Remark.Length >= 60) { this.ErrorMessage("描述备注字段长度不能超过60个汉字!"); Response.End(); }
            /*******************************************************************************************************
             * 开始保存请求数据
             * *****************************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["Classname"] = Classname;
            oDictionary["ParentID"] = ParentID;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["SortID"] = SortID;
            oDictionary["Remark"] = Remark;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAppClass]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*******************************************************************************************************
              * 输出数据处理结果
              * *****************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定继续设置分组信息!");
            Response.End();
        }

        /// <summary>
        /// 保存用户等级
        /// </summary>
        protected void SaveUpdate()
        {
            /*******************************************************************************************************
             * 验证请求参数信息
             * *****************************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            if (ClassID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppClass]", new Dictionary<string, object>() {
                {"ClassID",ClassID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /*******************************************************************************************************
            * 获取选择的上级分类ID
            * *****************************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (new Fooke.Function.String(ParentID).cInt() < 0) { this.ErrorMessage("获取上级分类ID信息失败,请重试！"); Response.End(); }
            else if (new Fooke.Function.String(ParentID).cInt() >= new Fooke.Function.String(ClassID).cInt()) { this.ErrorMessage("上级分类ID不能大于等于当前分类ID！"); Response.End(); }
            else if (ParentID != "0" && new Fooke.Function.String(ParentID).cInt() <= 99999) { this.ErrorMessage("获取上级分类ID信息失败,请重试！"); Response.End(); }
            else if (ParentID != "0")
            {
                DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppClass]", new Dictionary<string, object>() { 
                    {"ClassID",ParentID}
                });
                if (sRs == null) { this.ErrorMessage("获取上级分类信息失败,请重试!"); Response.End(); }
            }
            /*******************************************************************************************************
             * 获取用户等级信息
             * *****************************************************************************************************/
            string Classname = RequestHelper.GetRequest("Classname").ToString();
            if (string.IsNullOrEmpty(Classname)) { this.ErrorMessage("请填写服务分类名称！"); Response.End(); }
            else if (Classname.Length <= 0) { this.ErrorMessage("请填写服务分类名称！"); Response.End(); }
            else if (Classname.Length >= 20) { this.ErrorMessage("分类名称长度不能超过20个汉字！"); Response.End(); }
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppClass]", new Dictionary<string, object>() { 
                {"ParentID",ParentID},
                {"Classname",Classname}
            });
            if (oRs != null && oRs["ClassID"].ToString() != Rs["ClassID"].ToString())
            { this.ErrorMessage("同目录下分类名称已经存在！"); Response.End(); }
            /*******************************************************************************************************
             * 获取其他不需要验证的数据
             * *****************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string Remark = RequestHelper.GetRequest("Remark").ToString();
            if (Remark.Length >= 60) { this.ErrorMessage("描述备注字段长度不能超过60个汉字!"); Response.End(); }
            /*******************************************************************************************************
             * 开始保存请求数据
             * *****************************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["ClassID"] = ClassID;
            oDictionary["Classname"] = Classname;
            oDictionary["ParentID"] = ParentID;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["SortID"] = SortID;
            oDictionary["Remark"] = Remark;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAppClass]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*******************************************************************************************************
              * 输出数据处理结果
              * *****************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定继续设置分组信息!");
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
            string strList = RequestHelper.GetRequest("ClassID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始删除请求数据
            * *********************************************************************************************/
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_AppClass", Params: " and ClassID in (" + strList + ") and isDisplay=0");
            if (sTab == null) { this.ErrorMessage("没有需要删除的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { this.ErrorMessage("没有需要删除的数据！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_AppClass", Params: " and ClassID in (" + strList + ") and isDisplay=0");
            /***********************************************************************************************
            * 输出数据处理结果
            * *********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 保存排序
        /// </summary>
        protected void SaveEditor()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("ClassID").ToString();
            if (string.IsNullOrEmpty(strList)) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { Response.Write("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_AppClass", Params: " and ClassID in (" + strList + ")");
            if (sTab == null) { Response.Write("没有需要处理的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { Response.Write("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据信息
             * *********************************************************************************************/
            string SortID = RequestHelper.GetRequest("value").toInt();
            DbHelper.Connection.Update("Fooke_AppClass", dictionary: new Dictionary<string, string>() {
                {"SortID",SortID}
            }, Params: " and ClassID in (" + strList + ")");
            /***********************************************************************************************
             * 输出出局处理结果
             * *********************************************************************************************/
            Response.Write("排序成功！");
            Response.End();
        }

        protected void SaveDisplay()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("ClassID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_AppClass", Params: " and ClassID in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_AppClass", dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and ClassID in (" + strList + ")");
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}