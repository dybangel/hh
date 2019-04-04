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
using Fooke.SimpleMaster;
namespace Fooke.Web.Admin
{
    public partial class Link : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest.ToLower())
            {
                case "add": this.VerificationRole("友情连接"); this.AddLink(); Response.End(); break;
                case "addclass": this.VerificationRole("友情连接"); this.AddClass(); Response.End(); break;
                case "editclass": this.VerificationRole("友情连接"); this.UpdateClass(); Response.End(); break;
                case "addclasssave": this.VerificationRole("友情连接"); this.AddClassSave(); Response.End(); break;
                case "editclasssave": this.VerificationRole("友情连接"); this.UpdateClassSave(); Response.End(); break;
                case "classlist": this.VerificationRole("友情连接"); this.ListLinkClass(); Response.End(); break;
                case "delclass": this.VerificationRole("友情连接"); this.DeleteClass(); Response.End(); break;
                case "del": this.VerificationRole("友情连接"); this.DeleteLink(); Response.End(); break;
                case "displayclass": this.VerificationRole("友情连接"); this.DisplayClass(); Response.End(); break;
                case "display": this.VerificationRole("友情连接"); this.DisplayLink(); Response.End(); break;
                case "addlinksave": this.VerificationRole("友情连接"); this.AddLinkSave(); Response.End(); break;
                case "editlink": this.VerificationRole("友情连接"); this.UpdateLink(); Response.End(); break;
                case "editlinksave": this.VerificationRole("友情连接"); this.UpdateLinkSave(); Response.End(); break;
                default: this.VerificationRole("友情连接"); this.LinkList(); Response.End(); break;
            }

        }
        /// <summary>
        /// 链接列表
        /// </summary>
        protected void LinkList()
        {

            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            /***************************************************************************************************
             * 构建网页输出内容
             * *************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"6\">友情链接 >> 管理链接</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"search\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append("<form action=\"?action=default\" method=\"get\">");
            strBuilder.Append("<select name=\"searchType\">");
            strBuilder.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="sitename",Text="搜站点名称"},
                new OptionMode(){Value="classname",Text="搜分类名称"}
            }, SearchType));
            strBuilder.Append("</select>");
            strBuilder.Append(" <input placeholder=\"请填写要搜索的关键词\" type=\"text\" size=\"15\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />"); 
            strBuilder.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            /***************************************************************************************************
             * 构建网页输出内容
             * *************************************************************************************************/
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strBuilder.Append("<td>站点名称</td>");
            strBuilder.Append("<td width=\"18%\">站点目录</td>");
            strBuilder.Append("<td width=\"18%\">链接地址</td>");
            strBuilder.Append("<td width=\"18%\">审核状态</td>");
            strBuilder.Append("<td width=\"12%\">操作选项</td>");
            strBuilder.Append("</tr>");
            /******************************************************************************************************
             * 构建查询语句条件
             * ****************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "sitename": Params += " and sitename like '%" + Keywords + "%'"; break;
                    case "classname": Params += " and classname like '%" + Keywords + "%'"; break;
                }
            }
            /******************************************************************************************************
             * 构建分页查询语句
             * ****************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "id,classId,className,siteName,siteUrl,addtime,isOrder,isDisplay,thumb,isLogo";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " isOrder desc,Id desc";
            PageCenterConfig.Tablename = TableCenter.Link;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Link, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /******************************************************************************************************
             * 输出网页内容
             * ****************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td><input type=\"checkbox\" name=\"Id\" value=\"" + Rs["Id"] + "\" /></td>");
                strBuilder.Append("<td>");
                strBuilder.Append("" + Rs["sitename"] + "");
                if (Rs["isLogo"].ToString() == "1") { strBuilder.Append("<font color=\"red\">[图]</font>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>" + Rs["classname"] + "</td>");
                strBuilder.Append("<td>" + Rs["siteURL"] + "</td>");
                strBuilder.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strBuilder.Append("<a href=\"?action=display&Id=" + Rs["Id"] + "&val=0\"><img src=\"images/ico/yes.gif\"></a>"); }
                else { strBuilder.Append("<a href=\"?action=display&Id=" + Rs["Id"] + "&val=1\"><img src=\"images/ico/no.gif\"></a>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=editlink&Id=" + Rs["Id"] + "\"><img src=\"images/ico/edit.png\"></a>");
                strBuilder.Append("<a href=\"?action=del&Id=" + Rs["Id"] + "\" operate=\"delete\"><img src=\"images/ico/delete.png\"></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
            }

            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append(PageCenter.Often(Record, 12));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"审核通过(是)\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"审核通过(否)\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</form>");
            strBuilder.Append("</table>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new Fooke.SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/link/default.html");
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
        /// 添加链接
        /// </summary>
        protected void AddLink()
        {

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/link/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "options": strValue = LinkHelper.Options("0", ""); break;
                    case "file": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "thumb",
                        tips = "请选择一张照片上传"
                    }, "0"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑友情链接
        /// </summary>
        protected void UpdateLink()
        {
            /********************************************************************************************************
             * 获取数据记录
             * ******************************************************************************************************/
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("参数错误，请返回重试！"); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindLink", new Dictionary<string, object>() {
                {"LinkID",Id}
            });
            if (Rs == null) { this.ErrorMessage("没有找到你要编辑的数据！"); }
            /********************************************************************************************************
             * 输出网页数据信息
             * ******************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/link/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, Rs["isDisplay"].ToString()); break;
                    case "options": strValue = LinkHelper.Options("0", Rs["classId"].ToString()); break;
                    case "file": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "thumb",
                        fileValue = Rs["thumb"].ToString(),
                        tips = "请选择一张照片上传"
                    }, "0"); break;
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
        /// 目录列表
        /// </summary>
        protected void ListLinkClass()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<form id=\"iptforms\" action='?action=addclasssave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"6\">友情连接 >> 分类管理</td>");
            strBuilder.Append("</tr>");

            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strBuilder.Append("<td width=\"18%\">目录名称</td>");
            strBuilder.Append("<td width=\"18%\">父级目录</td>");
            strBuilder.Append("<td width=\"18%\">目录排序</td>");
            strBuilder.Append("<td width=\"18%\">目录状态</td>");
            strBuilder.Append("<td>操作选项</td>");
            strBuilder.Append("</tr>");
            string Params = " and ParentID=0";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "classId,ParentId,classname,isChild,isOrder,isDisplay";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "classId";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " classId desc";
            PageCenterConfig.Tablename = TableCenter.LinkClass;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.LinkClass, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td><input type=\"checkbox\" name=\"classId\" value=\"" + Rs["classId"] + "\" /></td>");
                strBuilder.Append("<td>" + Rs["classname"] + "</td>");
                strBuilder.Append("<td>一级目录</td>");
                strBuilder.Append("<td operate=\"isOrder\">" + Rs["isOrder"] + "</td>");
                strBuilder.Append("<td>");
                if (Rs["isDisplay"].ToString() == "0")
                { strBuilder.Append("<a href=\"?action=displayclass&classid=" + Rs["classid"] + "&val=1\"><img src=\"images/ico/no.gif\"/></a>"); }
                else { strBuilder.Append("<a href=\"?action=displayclass&classid=" + Rs["classid"] + "&val=0\"><img src=\"images/ico/yes.gif\"/></a>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=addclass&classid=" + Rs["classid"] + "\"><img src=\"images/ico/add.png\"/></a>");
                strBuilder.Append("<a href=\"?action=editclass&classid=" + Rs["classid"] + "\"><img src=\"images/ico/edit.png\"/></a>");
                strBuilder.Append("<a href=\"?action=delclass&classid=" + Rs["classid"] + "\" operate=\"delete\"><img src=\"images/ico/delete.png\"/></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
                if (Rs["isChild"].ToString() != "0") { strBuilder.Append(this.ChildList(Rs["classId"].ToString(), Rs["classname"].ToString(), "")); }
            }
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td colspan=\"7\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" value=\"删除\" cmdText=\"delclass\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"displayclass\" value=\"审核通过(是)\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"displayclass\" value=\"审核通过(否)\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");
            /****************************************************************************************************
             * 输出网页内容
             * **************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/link/classList.html");
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
        /// 显示子目录列表
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="ParentName"></param>
        /// <param name="CuteString"></param>
        /// <returns></returns>
        protected string ChildList(string ParentId, string ParentName, string CuteString)
        {
            StringBuilder strBuilder = new StringBuilder();
            string ShowCuteString = CuteString + "—";
            string Parameters = " and ParentId=" + ParentId + "";
            Parameters += " Order By isOrder Desc,classId Desc";
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.LinkClass, Params: Parameters);
            /*********************************************************************************************
             * 开始显示网页
             * ********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td><input type=\"checkbox\" name=\"classId\" value=\"" + Rs["classId"] + "\" /></td>");
                strBuilder.Append("<td>└" + ShowCuteString + Rs["classname"] + "</td>");
                strBuilder.Append("<td>" + ParentName + "</td>");
                strBuilder.Append("<td operate=\"isOrder\">" + Rs["isOrder"] + "</td>");
                strBuilder.Append("<td>");
                if (Rs["isDisplay"].ToString() == "0") { strBuilder.Append("<a href=\"?action=displayclass&classid=" + Rs["classid"] + "&val=1\"><img src=\"images/ico/no.gif\"/></a>"); }
                else { strBuilder.Append("<a href=\"?action=displayclass&classid=" + Rs["classid"] + "&val=0\"><img src=\"images/ico/yes.gif\"/></a>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=addclass&classid=" + Rs["classid"] + "\"><img src=\"images/ico/add.png\"/></a>");
                strBuilder.Append("<a href=\"?action=editclass&classid=" + Rs["classid"] + "\"><img src=\"images/ico/edit.png\"/></a>");
                strBuilder.Append("<a href=\"?action=delclass&classid=" + Rs["classid"] + "\" operate=\"delete\"><img src=\"images/ico/delete.png\"/></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
                if (Rs["isChild"].ToString() != "0") { strBuilder.Append(this.ChildList(Rs["classId"].ToString(), Rs["classname"].ToString(), ShowCuteString)); }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 添加友情链接栏目
        /// </summary>
        protected void AddClass()
        {
            string ParentId = RequestHelper.GetRequest("classId").toInt();
            if (ParentId != "0")
            {
                DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLinkClass]", new Dictionary<string, object>() {
                    {"classId",ParentId}
                });
                if (Rs == null) { this.ErrorMessage("系统没有找到你需要的数据！"); }
            }
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/link/addClass.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "options": strValue = LinkHelper.Options("0", ParentId); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑目录
        /// </summary>
        protected void UpdateClass()
        {
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("参数错误，找不到你需要的数据!"); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLinkClass]", new Dictionary<string, object>() {
                {"classId",classId}
            });
            if (Rs == null) { this.ErrorMessage("系统没有找到你需要的数据！"); }
            /**********************************************************************************************
             * 输出网页内容信息
             * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/link/editClass.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, Rs["isDisplay"].ToString()); break;
                    case "options": strValue = LinkHelper.Options("0", Rs["ParentID"].ToString()); break;
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



        /************************************************
         * 数据处理区域
         * **********************************************/
        /// <summary>
        /// 保存添加栏目
        /// </summary>
        protected void AddClassSave()
        {
            string ParentId = RequestHelper.GetRequest("ParentId").toInt();
            /*************************************************************************
             * 验证分类名称信息
             * ************************************************************************/
            string classname = RequestHelper.GetRequest("classname").toString();
            if (string.IsNullOrEmpty(classname)) { this.ErrorMessage("请输入目录名称！"); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLinkClass]", new Dictionary<string, object>() {
                {"className",classname},
                {"ParentID",ParentId}
            });
            if (Rs != null) { this.ErrorMessage("相同的目录名称已经存在！"); }
            /*************************************************************************
             * 获取其他不需要验证的数据
             * ************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string isOrder = RequestHelper.GetRequest("isOrder").toInt();
            /*************************************************************************
             * 开始保存数据信息
             * ************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["ClassID"] = "0";
            thisDictionary["classname"] = classname;
            thisDictionary["ParentId"] = ParentId;
            thisDictionary["OldParentID"] = "0";
            thisDictionary["isOrder"] = isOrder;
            thisDictionary["isDisplay"] = isDisplay;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveLinkClass]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据处理过程中发生未知错误,请刷新重试！"); Response.End(); }
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面！", "history.go(-1);", "?action=classlist");
            Response.End();
        }

        /// <summary>
        /// 保存修改栏目
        /// </summary>
        protected void UpdateClassSave()
        {
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindLinkClass", new Dictionary<string, object>() { 
                {"classId",classId}
            });
            if (Rs == null) { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            string ParentId = RequestHelper.GetRequest("ParentId").toInt();
            /*************************************************************************
             * 验证分类名称信息
             * ************************************************************************/
            string classname = RequestHelper.GetRequest("classname").toString();
            if (string.IsNullOrEmpty(classname)) { this.ErrorMessage("请输入目录名称！"); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindLinkClass]", new Dictionary<string, object>() {
                {"className",classname},
                {"ParentID",ParentId}
            });
            if (cRs != null && cRs["classId"].ToString() != Rs["classId"].ToString()) { this.ErrorMessage("相同的目录名称已经存在！"); }
            /*************************************************************************
             * 验证分类归档信息是否超出允许范围
             * ************************************************************************/
            if (classId == ParentId) { this.ErrorMessage("分类归档错误,请重新设置！"); Response.End(); }
            /*************************************************************************
             * 获取其他不需要验证的数据
             * ************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string isOrder = RequestHelper.GetRequest("isOrder").toInt();
            /*************************************************************************
             * 开始保存数据信息
             * ************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["ClassID"] = Rs["classId"].ToString();
            thisDictionary["classname"] = classname;
            thisDictionary["ParentId"] = ParentId;
            thisDictionary["OldParentID"] = Rs["ParentId"].ToString();
            thisDictionary["isOrder"] = isOrder;
            thisDictionary["isDisplay"] = isDisplay;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveLinkClass]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据处理过程中发生未知错误,请刷新重试！"); Response.End(); }
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面！", "history.go(-1);", "?action=classlist");
            Response.End();
        }
        /// <summary>
        /// 添加友情链接
        /// </summary>
        protected void AddLinkSave()
        {
            /****************************************************************************************************
             * 验证分类信息
             * **************************************************************************************************/
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请选择站点分类！"); Response.End(); }
            DataRow ClassRs = DbHelper.Connection.ExecuteFindRow("Stored_FindLinkClass", new Dictionary<string, object>() {
                {"classId",classId}
            });
            if (ClassRs == null) { this.ErrorMessage("获取分类信息失败,请重试！"); Response.End(); }
            /****************************************************************************************************
             * 验证站点名称是否合法
             * **************************************************************************************************/
            string siteName = RequestHelper.GetRequest("sitename").toString();
            if (string.IsNullOrEmpty(siteName)) { this.ErrorMessage("请填写站点名称！"); Response.End(); }
            if (siteName.Length > 40) { this.ErrorMessage("站点名称长度请限制在40个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindLink", new Dictionary<string, object>() {
                {"SiteName",siteName},
                {"classId",classId}
            });
            if (cRs != null) { this.ErrorMessage("同分类下站点名称已经存在,请重试！"); Response.End(); }
            string siteURL = RequestHelper.GetRequest("siteURL").toString();
            if (string.IsNullOrEmpty(siteURL)) { this.ErrorMessage("请填写站点链接地址！"); Response.End(); }
            if (siteURL.Length > 150) { this.ErrorMessage("站点链接地址长度请限制在150个字符以内！"); Response.End(); }
            string Thumb = RequestHelper.GetRequest("Thumb").toString();
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length > 150) { this.ErrorMessage("站点LOGO地址长度请限制在150个字符以内！"); Response.End(); }
            string isLogo = !string.IsNullOrEmpty(Thumb) ? "1" : "0";
            /****************************************************************************************************
             * 获取无需验证的数据
             * **************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string isOrder = RequestHelper.GetRequest("isOrder").toInt();
            /****************************************************************************************************
            * 开始保存站点数据
            * **************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["Id"] = "0";
            thisDictionary["classid"] = ClassRs["ClassID"].ToString();
            thisDictionary["classname"] = ClassRs["classname"].ToString();
            thisDictionary["sitename"] = siteName;
            thisDictionary["siteURL"] = siteURL;
            thisDictionary["isOrder"] = isOrder;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["Thumb"] = Thumb;
            thisDictionary["isLogo"] = isLogo;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveLink", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请返回重试！"); Response.End(); }
            /****************************************************************************************************
            * 输出网页数据信息
            * **************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面！", "history.go(-1);", "?action=default");
            Response.End();
        }

        /// <summary>
        /// 保存编辑的链接信息
        /// </summary>
        protected void UpdateLinkSave()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("参数错误，请返回重试！"); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindLink", new Dictionary<string, object>() {
                {"LinkID",Id}
            });
            if (Rs == null) { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            /****************************************************************************************************
             * 验证分类信息
             * **************************************************************************************************/
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请选择站点分类！"); Response.End(); }
            DataRow ClassRs = DbHelper.Connection.ExecuteFindRow("Stored_FindLinkClass", new Dictionary<string, object>() {
                {"classId",classId}
            });
            if (ClassRs == null) { this.ErrorMessage("获取分类信息失败,请重试！"); Response.End(); }
            /****************************************************************************************************
             * 验证站点名称是否合法
             * **************************************************************************************************/
            string siteName = RequestHelper.GetRequest("sitename").toString();
            if (string.IsNullOrEmpty(siteName)) { this.ErrorMessage("请填写站点名称！"); Response.End(); }
            if (siteName.Length > 40) { this.ErrorMessage("站点名称长度请限制在40个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindLink", new Dictionary<string, object>() {
                {"SiteName",siteName},
                {"classId",classId}
            });
            if (cRs != null && cRs["Id"].ToString() != Rs["Id"].ToString()) { this.ErrorMessage("同分类下站点名称已经存在,请重试！"); Response.End(); }
            string siteURL = RequestHelper.GetRequest("siteURL").toString();
            if (string.IsNullOrEmpty(siteURL)) { this.ErrorMessage("请填写站点链接地址！"); Response.End(); }
            if (siteURL.Length > 150) { this.ErrorMessage("站点链接地址长度请限制在150个字符以内！"); Response.End(); }
            string Thumb = RequestHelper.GetRequest("Thumb").toString();
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length > 150) { this.ErrorMessage("站点LOGO地址长度请限制在150个字符以内！"); Response.End(); }
            string isLogo = !string.IsNullOrEmpty(Thumb) ? "1" : "0";
            /****************************************************************************************************
             * 获取无需验证的数据
             * **************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string isOrder = RequestHelper.GetRequest("isOrder").toInt();
            /****************************************************************************************************
            * 开始保存站点数据
            * **************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["Id"] = Rs["Id"].ToString();
            thisDictionary["classid"] = ClassRs["ClassID"].ToString();
            thisDictionary["classname"] = ClassRs["classname"].ToString();
            thisDictionary["sitename"] = siteName;
            thisDictionary["siteURL"] = siteURL;
            thisDictionary["isOrder"] = isOrder;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["Thumb"] = Thumb;
            thisDictionary["isLogo"] = isLogo;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveLink", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请返回重试！"); Response.End(); }
            /****************************************************************************************************
            * 输出网页数据信息
            * **************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面！", "history.go(-1);", "?action=default");
            Response.End();
        }

        /// <summary>
        /// 删除链接目录
        /// </summary>
        protected void DeleteClass()
        {
            string classId = RequestHelper.GetRequest("classId").toString();
            if (string.IsNullOrEmpty(classId)) { this.ErrorMessage("参数错误，请选择一条数据！"); }
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_LinkClass", Params: " and classId in (" + classId + ")");
            if (Tab == null || Tab.Rows.Count <= 0) { this.ErrorMessage("请求参数错误,没有需要删除的数据！"); Response.End(); }
            foreach (DataRow cRs in Tab.Rows)
            {

                new LinkHelper().FindTable(cRs["classId"].ToString(), (oRs, Txt) =>
                {
                    DbHelper.Connection.ExecuteFindRow("[Stored_DeleteLinkClass]", new Dictionary<string, object>()
                    {
                        {"classId",oRs["classId"].ToString()},
                        {"ParentID",oRs["ParentID"].ToString()}
                    });
                }, "");
                /****************************************************************************************************
                * 删除当前数据
                * **************************************************************************************************/
                DbHelper.Connection.Delete("Fooke_LinkClass", Params: " and classId=" + cRs["ClassId"] + "");
            }
            /****************************************************************************************************
             * 返回数据处理结果
             * **************************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除友情链接
        /// </summary>
        protected void DeleteLink()
        {
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("参数错误，请选择一条数据！"); }
            DbHelper.Connection.Delete(TableCenter.Link, Params: " and Id in (" + Id + ")");
            this.History();
            Response.End();
        }

        /// <summary>
        /// 修改属性
        /// </summary>
        protected void DisplayClass()
        {
            string classId = RequestHelper.GetRequest("classId").toString();
            if (string.IsNullOrEmpty(classId)) { this.ErrorMessage("参数错误，请选择一条数据！"); }
            string val = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["isDisplay"] = val;
            DbHelper.Connection.Update(TableCenter.LinkClass, dictionary, " and classId in (" + classId + ")");
            this.History();
            Response.End();
        }

        protected void DisplayLink()
        {
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("参数错误，请选择一条数据！"); }
            string val = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["isDisplay"] = val;
            DbHelper.Connection.Update(TableCenter.Link, dictionary, " and Id in (" + Id + ")");
            this.History();
            Response.End();
        }
    }
}