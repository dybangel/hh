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
    public partial class Label : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "getclass": this.VerificationRole("标签模版"); this.GetClassOption(); Response.End(); break;
                case "columns": this.VerificationRole("标签模版"); this.GetColumnsName(); Response.End(); break;
                case "getfroms": this.VerificationRole("标签模版"); this.GetFroms(); Response.End(); break;
                case "addclass": this.VerificationRole("标签模版"); this.AddClass(); Response.End(); break;
                case "addclassave": this.VerificationRole("标签模版"); this.SaveAddClass(); Response.End(); break;
                case "classlist": this.VerificationRole("标签模版"); this.ClassList(); Response.End(); break;
                case "editclass": this.VerificationRole("标签模版"); this.UpdateClass(); Response.End(); break;
                case "editclassave": this.VerificationRole("标签模版"); this.SaveUpdateClass(); Response.End(); break;
                case "delclass": this.VerificationRole("标签模版"); this.DeleteClass(); Response.End(); break;
                case "del": this.VerificationRole("标签模版"); this.Delete(); Response.End(); break;
                case "display": this.VerificationRole("标签模版"); this.Display(); Response.End(); break;
                case "save": this.VerificationRole("标签模版"); this.AddSave(); Response.End(); break;
                case "edit": this.VerificationRole("标签模版"); this.UpdateLabel(); Response.End(); break;
                case "editsave": this.VerificationRole("标签模版"); this.SaveUpdate(); Response.End(); break;
                case "saveProperty": this.VerificationRole("标签模版"); this.SaveProperty(); Response.End(); break;
                case "add": this.VerificationRole("标签模版"); this.CopyLabel(); Response.End(); break;
                default: this.VerificationRole("标签模版"); this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 调用输出表单列表的字段
        /// </summary>
        protected void GetFroms()
        {
            //string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            //if (fromsId == "0") { Response.Write("error:请求参数错误，请选择表单！"); Response.End(); }
            //DataRow fromsRs = DbHelper.Connection.FindRow(TableCenter.Froms, Params: " and fromsId=" + fromsId + "");
            //if (fromsRs == null) { Response.Write("error:表单不存在！"); Response.End(); }
            //StringBuilder strBuilder = new StringBuilder();
            ////strBuilder.Append(LabelCenter.FromsColumns());
            //DataTable cTab = DbHelper.Connection.FindTable(TableCenter.Columns, Params: " and cTable='" + fromsRs["cName"] + "' order by SortID desc,columnsId asc");
            //foreach (DataRow Rs in cTab.Rows)
            //{
            //    strBuilder.Append("<a onclick=\"setTxt('{$" + Rs["columnsName"] + "/}')\">" + Rs["columnsText"] + "</a>");
            //}
            //Response.Write(strBuilder.ToString());
            //Response.End();
        }

        /// <summary>
        /// 显示模型下的字段
        /// </summary>
        protected void GetColumnsName()
        {
            string channelId = RequestHelper.GetRequest("channelID").toInt();
            if (channelId == "0") { Response.Write("参数错误，请选择文档模型！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Channel, Params: " and channelId = " + channelId + "");
            if (cRs == null) { Response.Write("你查找的模型不存在！"); Response.End(); }
            Response.Write(LabelHelper.ChannelColumns(cRs["BaseName"].ToString()));
            Response.End();
        }

        /// <summary>
        /// 调用输出模型分类的options选项
        /// </summary>
        protected void GetClassOption()
        {
            string channelId = RequestHelper.GetRequest("channelId").toInt();
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<select style=\"width:220px;\" name=\"classId\">");
            strBuilder.Append("<option value=\"0\">所有分类</option>");
            strBuilder.Append("<option value=\"-1\" style=\"color:#F00\">自适应当前栏目</option>");
            strBuilder.Append(ClassHelper.Options(channelId));
            strBuilder.Append("</select>");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 标签管理列表
        /// </summary>
        protected void strDefault()
        {
            /*************************************************************************************
             * 获取查询信息
             * **********************************************************************************/
            string classId = RequestHelper.GetRequest("classId").toInt();
            string style = RequestHelper.GetRequest("style").toInt();
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            /*************************************************************************************
             * 获取查询信息
             * **********************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">标签管理 >> 标签列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="labelName",Text="标签名称"},
                new OptionMode(){Value="className",Text="分类名称"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" placeholder=\"请填写关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            /*************************************************************************************
            * 构建网页主体内容信息
            * **********************************************************************************/
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td>标签名称</td>");
            strText.Append("<td width=\"120\">标签分类</td>");
            strText.Append("<td width=\"120\">标签类型</td>");
            strText.Append("<td width=\"80\">状态</td>");
            strText.Append("<td width=\"140\">操作选项</td>");
            strText.Append("</tr>");
            /*************************************************************************************
            * 构建查询条件
            * **********************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType.ToLower())
                {
                    case "labelname": Params += " and labelname like '%" + Keywords + "%'"; break;
                    case "classname": Params += " and classname like '%" + Keywords + "%'"; break;
                }
            }
            if (classId != "0") { Params += " and classId = " + classId + ""; }
            if (style != "0") { Params += " and style = " + style + ""; }
            /*************************************************************************************
            * 获取分页显示数量
            * **********************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /*************************************************************************************
            * 构建查询分页语句
            * **********************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "Id,classId,className,labelName,style,isValid,isDelay";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Id Desc";
            PageCenterConfig.Tablename = TableCenter.Label;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Label, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                string styleName = "";
                switch (Rs["style"].ToString())
                {
                    case "1": styleName = "通用列表"; break;
                    case "2": styleName = "分页标签"; break;
                    case "3": styleName = "栏目列表"; break;
                    case "4": styleName = "单页列表"; break;
                    case "5": styleName = "静态标签"; break;
                    case "6": styleName = "友情连接"; break;
                    case "7": styleName = "表单列表"; break;
                }
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"Id\" value=\"" + Rs["Id"] + "\" /></td>");
                strText.Append("<td id=\"c_" + Rs["id"] + "\" title=\"点击复制标签名称\">" + Rs["labelName"] + "");
                if (Rs["isDelay"].ToString() == "1") { strText.Append("<font color=\"#F00\">[延时]</font>"); }
                strText.Append("</td>");
                strText.Append("<td><a href=\"?action=default&classId=" + Rs["classId"] + "\">" + Rs["className"] + "</a></td>");
                strText.Append("<td><a href=\"?action=default&style=" + Rs["style"] + "\">" + styleName + "</a></td>");
                strText.Append("<td>");
                if (Rs["isValid"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&id=" + Rs["Id"] + "\"><img src=\"images/ico/yes.gif\" /></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&id=" + Rs["Id"] + "\"><img src=\"images/ico/no.gif\" /></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a onclick=\"if(!confirm('你确定要复制标签？')){return false;}\" href=\"?action=add&Id=" + Rs["Id"] + "\" title=\"复制标签\"><img src=\"images/ico/add.png\" /></a>");
                strText.Append("<a href=\"?action=edit&style=" + Rs["style"] + "&Id=" + Rs["Id"] + "\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=del&Id=" + Rs["Id"] + "\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /*******************************************************************************
            * 设置操作选项
            * *****************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除选中\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"正常使用\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"关闭使用\" onclick=\"commandOperate(this)\" />");
            strText.Append("<select name=\"Property\">");
            strText.Append("<option value=\"0\">选择属性设置</option>");
            strText.Append("<option value=\"1\">延时加载</option>");
            strText.Append("<option value=\"2\">取消延时</option>");
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"saveProperty\" value=\"保存设置\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            
            /*******************************************************************************
            * 开始输出网页数据
            * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/label/default.html");
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
        /// 标签分类列表
        /// </summary>
        protected void ClassList()
        {
            /*******************************************************************************
            * 构建网页内容信息
            * *****************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"5\">标签管理 >> 标签分类 >> 分类列表</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"120\">分类名称</td>");
            strText.Append("<td width=\"120\">上级分类</td>");
            strText.Append("<td>分类说明</td>");
            strText.Append("<td width=\"120\">操作选项</td>");
            strText.Append("</tr>");
            /*******************************************************************************
            * 构建分页查询语句
            * *****************************************************************************/
            string Params = " and ParentID=0";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "classId";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " classId Desc";
            PageCenterConfig.Tablename = TableCenter.LabelClass;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.LabelClass, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*******************************************************************************
            * 构建分页内容信息
            * *****************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"classId\" value=\"" + Rs["classId"] + "\" /></td>");
                strText.Append("<td>" + Rs["className"] + "</td>");
                strText.Append("<td>一级分类</td>");
                strText.Append("<td>" + Rs["Descrption"] + "</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=addclass&ParentID=" + Rs["classId"] + "\" title=\"添加子栏目\"><img src=\"images/ico/add.png\" /></a>");
                strText.Append("<a href=\"?action=editclass&classId=" + Rs["classId"] + "\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=delclass&classId=" + Rs["classId"] + "\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
                if (Rs["isChild"].ToString() != "0") { strText.Append(this.GetChildList(Rs["classId"].ToString(), Rs["className"].ToString(), "")); }
            }
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"5\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************
            * 开始输出网页数据
            * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/label/classlist.html");
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
        /// 标签分类下级分类
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="ParentName"></param>
        /// <param name="cuteTxt"></param>
        /// <returns></returns>
        public string GetChildList(string ParentID, string ParentName, string cuteTxt)
        {
            StringBuilder strText = new StringBuilder();
            string strParams = " and ParentID=" + ParentID + " order by classid desc";
            string showTxt = "━" + cuteTxt;
            string columns = "*";
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.LabelClass, columns: columns, Params: strParams);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"classId\" value=\"" + Rs["classId"] + "\" /></td>");
                strText.Append("<td title=\"已展开全部子分类\">┗" + showTxt + Rs["className"] + "</td>");
                strText.Append("<td>" + ParentName + "</td>");
                strText.Append("<td>"+Rs["descrption"]+"</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=addclass&ParentID=" + Rs["classId"] + "\" title=\"添加子栏目\"><img src=\"images/ico/add.png\" /></a>");
                strText.Append("<a href=\"?action=editclass&classId=" + Rs["classId"] + "\" title=\"修改栏目\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=delclass&classId=" + Rs["classId"] + "\" title=\"删除栏目\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
                if (Rs["isChild"].ToString() != "0") { strText.Append(this.GetChildList(Rs["classId"].ToString(), Rs["className"].ToString(), showTxt)); }
            }
            return strText.ToString();
        }


        /// <summary>
        /// 添加标签目录
        /// </summary>
        protected void AddClass()
        {
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            /*******************************************************************************
            * 开始输出网页数据
            * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/label/addClass.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": strValue = new LabelHelper().Options(ParentID:"0",defaultText:ParentID); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 修改标签分类
        /// </summary>
        protected void UpdateClass()
        {
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请求参数参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLabelClass]", new Dictionary<string, object>() {
                {"classId",classId}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /*******************************************************************************
            * 开始输出网页数据
            * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/label/editClass.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": strValue =new  LabelHelper().Options(ParentID:"0",defaultText: Rs["ParentID"].ToString()); break;
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 保存添加分类
        /// </summary>
        protected void SaveAddClass()
        {
            string className = RequestHelper.GetRequest("className").toString();
            if (string.IsNullOrEmpty(className)) { this.ErrorMessage("标签分类名称不能为空！"); Response.End(); }
            if (className.Length > 20) { this.ErrorMessage("标签分类名称长度请限制在20个字符以内！"); Response.End(); }
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            string Descrption = RequestHelper.GetRequest("Descrption").toString();
            if (!string.IsNullOrEmpty(Descrption) && Descrption.Length > 50) { this.ErrorMessage("描述信息请限制在50个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindLabelClass", new Dictionary<string, object>() {
                {"ParentID",ParentID},
                {"className",className}
            });
            if (cRs != null) { this.ErrorMessage("同目录下分类名称已经存在，请另外选择一个名字吧！"); Response.End(); }
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["classId"] = "0";
            thisDictionary["className"] = className;
            thisDictionary["ParentID"] = ParentID;
            thisDictionary["OldParentID"] = "0";
            thisDictionary["descrption"] = Descrption;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveLabelClass", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 输出数据处理结果
             * *********************************************************************************************/
            this.ConfirmMessage("数据保存成功，点击确定将继续停留在当前页面！", falseUrl: "?action=classlist");
            Response.End();
        }
        /// <summary>
        /// 保存编辑
        /// </summary>
        protected void SaveUpdateClass()
        {
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindLabelClass", new Dictionary<string, object>() {
                {"ClassID",classId},
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /***********************************************************************************************
             * 数据验证
             * *********************************************************************************************/
            string className = RequestHelper.GetRequest("className").toString();
            if (string.IsNullOrEmpty(className)) { this.ErrorMessage("标签分类名称不能为空！"); Response.End(); }
            if (className.Length > 20) { this.ErrorMessage("标签分类名称长度请限制在20个字符以内！"); Response.End(); }
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID != "0" && ParentID == classId) { this.ErrorMessage("不能选择自身作为上级分类！"); Response.End(); }
            string Descrption = RequestHelper.GetRequest("Descrption").toString();
            if (!string.IsNullOrEmpty(Descrption) && Descrption.Length > 50) { this.ErrorMessage("描述信息请限制在50个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindLabelClass", new Dictionary<string, object>() {
                {"ParentID",ParentID},
                {"className",className}
            });
            if (cRs != null && cRs["classId"].ToString()!=Rs["classId"].ToString()) { this.ErrorMessage("同目录下分类名称已经存在，请另外选择一个名字吧！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据
             * *********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["classId"] = Rs["classId"].ToString();
            thisDictionary["className"] = className;
            thisDictionary["ParentID"] = ParentID;
            thisDictionary["OldParentID"] = Rs["ParentID"].ToString();
            thisDictionary["descrption"] = Descrption;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveLabelClass", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 输出数据处理结果
             * *********************************************************************************************/
            this.ConfirmMessage("数据保存成功，点击确定将继续停留在当前页面！", falseUrl: "?action=classlist");
            Response.End();
        }
        /// <summary>
        /// 编辑标签
        /// </summary>
        protected void UpdateLabel()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            string style = RequestHelper.GetRequest("style").toInt();
            if (style == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            string toUrl = string.Empty;
            switch (style)
            {
                case "1": toUrl = "label/items.aspx?action=edit&id=" + Id + ""; break;
                case "2": toUrl = "label/paging.aspx?action=edit&id=" + Id + ""; break;
                case "3": toUrl = "label/list.aspx?action=edit&id=" + Id + ""; break;
                case "4": toUrl = "label/single.aspx?action=edit&id=" + Id + ""; break;
                case "5": toUrl = "label/quiescent.aspx?action=edit&id=" + Id + ""; break;
                case "6": toUrl = "label/links.aspx?action=edit&id=" + Id + ""; break;
                case "7": toUrl = "label/froms.aspx?action=edit&id=" + Id + ""; break;
            }
            if (string.IsNullOrEmpty(toUrl)) { this.ErrorMessage("发生未知错误，标签编辑失败！"); Response.End(); }
            Response.Redirect(toUrl); Response.End();
        }

        /// <summary>
        /// 保存标签
        /// </summary>
        protected void AddSave()
        {
            /***********************************************************************************************************
             * 验证标签名称
             * *********************************************************************************************************/
            string LabelName = RequestHelper.GetRequest("LabelName").toString();
            if (string.IsNullOrEmpty(LabelName)) { this.ErrorMessage("标签名称不能为空！"); Response.End(); }
            if (LabelName.Length > 30) { this.ErrorMessage("标签名称请限制在30个字符以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLabel]", new Dictionary<string, object>() {
                {"labelname",LabelName}
            });
            if (cRs != null) { this.ErrorMessage("相同的标签名称已经存在，请另外选择一个吧！"); Response.End(); }
            string style = RequestHelper.GetRequest("style").toInt();
            if (style == "0") { this.ErrorMessage("请选择标签类型！"); Response.End(); }
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            if (style == "1" && ChannelID == "0") { this.ErrorMessage("通用列表标签必须选择模型！"); Response.End(); }
            if (style == "2" && ChannelID == "0") { this.ErrorMessage("分页标签必须选择模型！"); Response.End(); }
            /**********************************************************************************************
             * 设置获取标签的数据信息
             * ********************************************************************************************/
            string classId = RequestHelper.GetRequest("FolderID").toInt();
            string className = "标签根目录";
            DataRow classRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLabelClass]", new Dictionary<string, object>() {
                {"classId",classId}
            });
            if (classId != "0" && classRs == null) { this.ErrorMessage("标签存放目录不存在，请另外选择！"); Response.End(); }
            if (classRs != null) { className = classRs["className"].ToString(); }
            /************************************************************************************************
             * 获取其它的数据信息
             * **********************************************************************************************/
            string isValid = RequestHelper.GetRequest("isValid").toInt();
            string isDelay = RequestHelper.GetRequest("isDelay").toInt();
            string isParameter = RequestHelper.GetRequest("isParameter").toInt();
            string ParameterText = RequestHelper.GetRequest("ParameterText", false).toString();
            if (isParameter == "1" && string.IsNullOrEmpty(ParameterText)) { this.ErrorMessage("你选择了标签自定义条件，请输入查询条件！"); Response.End(); }
            if (ParameterText.Length >= 500) { this.ErrorMessage("查询限制条件长度请限制在500个字符以内！"); Response.End(); }
            string LoopTxt = RequestHelper.GetRequest("LoopTxt", false).toString();
            string strXML = RequestHelper.GetPrametersXML(false).ToString();
            /**************************************************************************************************
             * 开始保存数据信息
             * ************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["classId"] = classId;
            thisDictionary["className"] = className;
            thisDictionary["labelName"] = LabelName;
            thisDictionary["style"] = style;
            thisDictionary["LoopTxt"] = LoopTxt;
            thisDictionary["strXML"] = strXML;
            thisDictionary["isParameter"] = isParameter;
            thisDictionary["ParameterText"] = ParameterText;
            thisDictionary["isValid"] = isValid;
            thisDictionary["isDelay"] = isDelay;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveLabel]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请返回重试！"); Response.End(); }
            /*************************************************************************
             * 更新标签缓存
             * ***********************************************************************/
            try { new LabelHelper().ApplicationClear(); }
            catch { }
            /*************************************************************************
             * 开始执行输出
             * ***********************************************************************/
            this.ConfirmMessage("标签保存成功，点击确定将继续停留在当前页面！", falseUrl: "../label.aspx?action=default&classid=" + classId + "");
            Response.End();
        }
        /// <summary>
        /// 保存编辑标签
        /// </summary>
        protected void SaveUpdate()
        {
            /***********************************************************************************************************
             * 获取并验证请求参数的合法性
             * *********************************************************************************************************/
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLabel]", new Dictionary<string, object>() {
                {"Id",Id}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /***********************************************************************************************************
             * 验证标签名称
             * *********************************************************************************************************/
            string LabelName = RequestHelper.GetRequest("LabelName").toString();
            if (string.IsNullOrEmpty(LabelName)) { this.ErrorMessage("标签名称不能为空！"); Response.End(); }
            if (LabelName.Length > 30) { this.ErrorMessage("标签名称请限制在30个字符以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLabel]", new Dictionary<string, object>() {
                {"labelname",LabelName}
            });
            if (cRs != null && cRs["Id"].ToString() != Rs["Id"].ToString()) { this.ErrorMessage("相同的标签名称已经存在，请另外选择一个吧！"); Response.End(); }
            string style = RequestHelper.GetRequest("style").toInt();
            if (style == "0") { this.ErrorMessage("请选择标签类型！"); Response.End(); }
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            if (style == "1" && ChannelID == "0") { this.ErrorMessage("通用列表标签必须选择模型！"); Response.End(); }
            if (style == "2" && ChannelID == "0") { this.ErrorMessage("分页标签必须选择模型！"); Response.End(); }
            /**********************************************************************************************
             * 设置获取标签的数据信息
             * ********************************************************************************************/
            string classId = RequestHelper.GetRequest("FolderID").toInt();
            string className = "标签根目录";
            DataRow classRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLabelClass]", new Dictionary<string, object>() {
                {"classId",classId}
            });
            if (classId != "0" && classRs == null) { this.ErrorMessage("标签存放目录不存在，请另外选择！"); Response.End(); }
            if (classRs != null) { className = classRs["className"].ToString(); }
            /************************************************************************************************
             * 获取其它的数据信息
             * **********************************************************************************************/
            string isValid = RequestHelper.GetRequest("isValid").toInt();
            string isDelay = RequestHelper.GetRequest("isDelay").toInt();
            string isParameter = RequestHelper.GetRequest("isParameter").toInt();
            string ParameterText = RequestHelper.GetRequest("ParameterText", false).toString();
            if (string.IsNullOrEmpty(ParameterText) && isParameter == "1") { this.ErrorMessage("你选择了标签自定义条件，请输入查询条件！"); Response.End(); }
            string LoopTxt = RequestHelper.GetRequest("LoopTxt", false).toString();
            string strXML = RequestHelper.GetPrametersXML(false).ToString();
            /**************************************************************************************************
             * 开始保存数据信息
             * ************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["Id"] = Id;
            thisDictionary["classId"] = classId;
            thisDictionary["className"] = className;
            thisDictionary["labelName"] = LabelName;
            thisDictionary["style"] = Rs["style"].ToString();
            thisDictionary["LoopTxt"] = LoopTxt;
            thisDictionary["strXML"] = strXML;
            thisDictionary["isParameter"] = isParameter;
            thisDictionary["ParameterText"] = ParameterText;
            thisDictionary["isValid"] = isValid;
            thisDictionary["isDelay"] = isDelay;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveLabel]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请返回重试！"); Response.End(); }
            /*************************************************************************
             * 更新标签缓存
             * ***********************************************************************/
            try { new LabelHelper().ApplicationClear(); }
            catch { }
            /*************************************************************************
             * 开始执行输出
             * ***********************************************************************/
            this.ConfirmMessage("标签保存成功，点击确定将继续停留在当前页面！", falseUrl: "../label.aspx?action=default&classid=" + classId + "");
            Response.End();
        }

        /// <summary>
        /// 删除系统函数标签
        /// </summary>
        protected void Delete()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("Id").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_Label", Params: " and Id in (" + strList + ") and isValid=0");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始删除数据信息
            * *********************************************************************************************/
            DbHelper.Connection.Delete("Fooke_Label", Params: " and Id in (" + strList + ") and isValid=0");
            /***********************************************************************************************
            * 更新标签缓存
            * *********************************************************************************************/
            try { new LabelHelper().ApplicationClear(); }
            catch { }
            /***********************************************************************************************
            * 输出数据处理结果
            * *********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除标签分类
        /// </summary>
        protected void DeleteClass()
        {
            string classId = RequestHelper.GetRequest("classId").toString();
            if (classId == "") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.LabelClass, Params: " and classId in (" + classId + ")");
            foreach (DataRow Rs in Tab.Rows)
            {
                DbHelper.Connection.Delete(TableCenter.LabelClass, Params: " and classId = " + classId + "");
                if (Rs["ParentID"].ToString() != "0") { this.UpdateChildCount(Rs["ParentID"].ToString()); }
            }
            /*************************************************************************
             * 更新标签缓存
             * ***********************************************************************/
            try { new LabelHelper().ApplicationClear(); }
            catch { }
            /*************************************************************************
             * 开始执行输出
             * ***********************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 审核管理员状态
        /// </summary>
        protected void Display()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("Id").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_Label", Params: " and Id in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_Label", new Dictionary<string, string>() {
                {"isValid",strValue}
            }, Params: " and Id in (" + strList + ")");
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 设置延时加载数据
        /// </summary>
        protected void SaveProperty()
        {
            string Id = RequestHelper.GetRequest("Id").toString();
            if (Id == "") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            string Property = RequestHelper.GetRequest("Property").toInt();
            if (Property == "0") { this.ErrorMessage("参数错误，请选择一个属性进行设置！"); Response.End(); }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            switch (Property)
            {
                case "1": dictionary["isDelay"] = "1"; break;
                case "2": dictionary["isDelay"] = "0"; break;
            }
            if (dictionary.Count <= 0) { this.ErrorMessage("发生未知错误，请返回重试！"); Response.End(); }
            DbHelper.Connection.Update(TableCenter.Label, dictionary, Params: " and Id in (" + Id + ")");
            /*************************************************************************
             * 更新标签缓存
             * ***********************************************************************/
            try { new LabelHelper().ApplicationClear(); }
            catch { }
            /*************************************************************************
             * 开始执行输出
             * ***********************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 复制标签
        /// </summary>
        public void CopyLabel()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLabel]", new Dictionary<string, object>() {
                {"Id",Id}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /**************************************************************************************************
             * 开始复制标签信息
             * ************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["Id"] = "0";
            thisDictionary["classId"] = Rs["classId"].ToString();
            thisDictionary["className"] = Rs["className"].ToString();
            thisDictionary["labelName"] = string.Format("{0}_{1}", Rs["LabelName"].ToString(), DateTime.Now.ToString("HHmm"));
            thisDictionary["style"] = Rs["style"].ToString();
            thisDictionary["LoopTxt"] = Rs["LoopTxt"].ToString();
            thisDictionary["strXML"] = Rs["strXML"].ToString();
            thisDictionary["isParameter"] = Rs["isParameter"].ToString(); ;
            thisDictionary["ParameterText"] = Rs["ParameterText"].ToString();
            thisDictionary["isValid"] = Rs["isValid"].ToString();
            thisDictionary["isDelay"] = Rs["isDelay"].ToString();
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveLabel]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请返回重试！"); Response.End(); }
            /*************************************************************************
             * 更新标签缓存
             * ***********************************************************************/
            try { new LabelHelper().ApplicationClear(); }
            catch { }
            /*************************************************************************
             * 开始执行输出
             * ***********************************************************************/
            this.History();
            Response.End();
        }

        /// <summary>
        /// 更改标签分类子栏目个数
        /// </summary>
        /// <param name="ParentID"></param>
        public void UpdateChildCount(string ParentID)
        {
            try
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary["ParentID"] = ParentID;
                DbHelper.Connection.ExecuteProc("Stored_LabelClassUpdateChildCount", dictionary);
            }
            catch { }
        }
    }
}