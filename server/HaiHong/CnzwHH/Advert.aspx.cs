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
    public partial class Advert : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "addjs": this.VerificationRole("广告系统"); AddJavaScript(); Response.End(); break;
                case "code": this.VerificationRole("广告系统"); strCode(); Response.End(); break;
                case "editListsave": this.VerificationRole("广告系统"); SaveUpdateList(); Response.End(); break;
                case "editList": this.VerificationRole("广告系统"); EditAdvertList(); Response.End(); break;
                case "displayList": this.VerificationRole("广告系统"); DisplayList(); Response.End(); break;
                case "delList": this.VerificationRole("广告系统"); DeleteList(); Response.End(); break;
                case "childlist": this.VerificationRole("广告系统"); AdvertList(); Response.End(); break;
                case "addListsave": this.VerificationRole("广告系统"); AddListSave(); Response.End(); break;
                case "addlist": this.VerificationRole("广告系统"); AddList(); Response.End(); break;
                case "del": this.VerificationRole("广告系统"); this.Delete(); Response.End(); break;
                case "editsave": this.VerificationRole("广告系统"); SaveUpdate(); Response.End(); break;
                case "edit": this.VerificationRole("广告系统"); Update(); Response.End(); break;
                case "display": this.VerificationRole("广告系统"); Display(); Response.End(); break;
                case "add": this.VerificationRole("广告系统"); Add(); Response.End(); break;
                case "save": this.VerificationRole("广告系统"); AddSave(); Response.End(); break;
                default: this.VerificationRole("广告系统"); strDefault(); Response.End(); break;
            }
            Response.End();
        }
        /// <summary>
        /// 展示子广告的列表
        /// </summary>
        protected void AdvertList()
        {
            /********************************************************************************************************
             * 验证数据合法性
             * ******************************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toInt();
            if (advId == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvert", new Dictionary<string, object>() {
                {"AdvID",advId}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /********************************************************************************************************
            * 获取数据查询条件
            * ******************************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            /********************************************************************************************************
             * 构建网页内容
             * ******************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"6\">广告位管理 >> " + cRs["advertName"] + " >> 广告管理</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"search\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append("<form action=\"?\" method=\"get\">");
            strBuilder.Append("<input type=\"hidden\" name=\"action\" value=\"childlist\" />");
            strBuilder.Append("<input type=\"hidden\" name=\"advId\" value=\"" + cRs["advId"] + "\" />");
            strBuilder.Append("<select name=\"SearchType\">");
            strBuilder.Append("<option value=\"title\">搜名称</option>");
            strBuilder.Append("</select>");
            strBuilder.Append(" <input type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strBuilder.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            /********************************************************************************************************
             * 构建表头信息
             * ******************************************************************************************************/
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<input type=\"hidden\" name=\"advId\" value=\"" + cRs["advId"] + "\" />");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strBuilder.Append("<td>广告位名称</td>");
            strBuilder.Append("<td width=\"60\">显示排序</td>");
            strBuilder.Append("<td width=\"320\">链接地址</td>");
            strBuilder.Append("<td width=\"60\">状态</td>");
            strBuilder.Append("<td width=\"100\">选项</td>");
            strBuilder.Append("</tr>");
            /********************************************************************************************************
             * 获取分页查询条件
             * ******************************************************************************************************/
            string Params = " and advId = " + cRs["advId"] + "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "title": Params += " and advertName like '%" + Keywords + "%'"; break;
                }
            }
            /********************************************************************************************************
             * 获取分页查询语句
             * ******************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "id,advid,advertName,title,intro,isDisplay,sortid,strLink";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID desc, Id desc";
            PageCenterConfig.Tablename = TableCenter.AdvertList;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.AdvertList, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /********************************************************************************************************
             * 循环遍历网页内容
             * ******************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td><input type=\"checkbox\" name=\"id\" value=\"" + Rs["id"] + "\" /></td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"" + Rs["strLink"] + "\" target=\"_blank\">" + Rs["title"] + "</a>");
                strBuilder.Append("</td>");
                strBuilder.Append("<td>" + Rs["SortID"] + "</td>");
                strBuilder.Append("<td>" + Rs["strLink"] + "</td>");
                strBuilder.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strBuilder.Append("<a href=\"?action=displayList&val=0&advId=" + Rs["advId"] + "&Id=" + Rs["Id"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strBuilder.Append("<a href=\"?action=displayList&val=1&Id=" + Rs["Id"] + "&advId=" + Rs["advId"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=editList&advId=" + Rs["advId"] + "&Id=" + Rs["Id"] + "\" title=\"编辑广告位\"><img src=\"template/images/ico/edit.png\" /></a>");
                strBuilder.Append("<a href=\"?action=delList&advId=" + Rs["advId"] + "&Id=" + Rs["Id"] + "\"  title=\"删除广告位\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
            }
            /********************************************************************************************************
             * 输出分页控件信息
             * ******************************************************************************************************/
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" value=\"删除\" cmdText=\"delList\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"displayList\" value=\"设为审核\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"displayList\" value=\"取消审核\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/advert/advertList.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strBuilder.ToString(); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 添加子广告
        /// </summary>
        protected void AddList()
        {
            string advId = RequestHelper.GetRequest("advId").toInt();
            if (advId == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvert", new Dictionary<string, object>() {
                {"AdvID",advId}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /*********************************************************************************************************
             * 输出网页数据信息
             * ********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/advert/addList.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "advid": strValue = cRs["advid"].ToString(); break;
                    case "advertname": strValue = cRs["advertName"].ToString(); break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "event":
                        if (cRs["modals"].ToString() != "code")
                        { strValue = FunctionBase.UploadControl("thumb", tips: "请上传一张广告图,尺寸请参照广告位尺寸"); }
                        else
                        {
                            strValue = "<textarea placeholder=\"在这里填写javascript代码\" name=\"strcode\" class=\"inputtext\" style=\"width:460px;height:60px;\"></textarea>";
                            strValue += "<br/>请填写广告代码,该代码以javascript模式运行";
                        }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        protected void EditAdvertList()
        {

            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvertList", new Dictionary<string, object>() {
                {"Id",Id}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /**************************************************************************************************
             * 验证广告位信息
             * ************************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toInt();
            if (advId == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow advRs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvert", new Dictionary<string, object>() {
                {"AdvID",cRs["advId"].ToString()}
            });
            if (advRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/advert/editList.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "advid": strValue = cRs["advid"].ToString(); break;
                    case "advertname": strValue = cRs["advertName"].ToString(); break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, cRs["isDisplay"].ToString()); break;
                    case "event":
                        if (advRs["modals"].ToString() != "code")
                        { strValue = FunctionBase.UploadControl("thumb", defaultText: cRs["thumb"].ToString(), tips: "请上传一张广告图,尺寸请参照广告位尺寸"); }
                        else
                        {
                            strValue = "<textarea placeholder=\"在这里填写javascript代码\" name=\"strcode\" class=\"inputtext\" style=\"width:460px;height:60px;\">" + cRs["strcode"] + "</textarea>";
                            strValue += "<br/>请填写广告代码,该代码以javascript模式运行";
                        }
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

        /// <summary>
        /// 站点管理
        /// </summary>
        protected void strDefault()
        {
            /******************************************************************************************
             * 构建数据查询参数信息
             * ****************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StartDate = RequestHelper.GetRequest("StartDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            /******************************************************************************************
             * 构建网络表视图
             * ****************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"6\">广告位管理 >> 广告位列表</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"search\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append("<form action=\"?action=default\" method=\"get\">");
            strBuilder.Append("<select name=\"SearchType\">");
            strBuilder.Append("<option value=\"title\">搜名称</option>");
            strBuilder.Append("</select>");
            strBuilder.Append(" <input type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strBuilder.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            /******************************************************************************************
             * 构建表格抬头信息
             * ****************************************************************************************/
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strBuilder.Append("<td>广告位名称</td>");
            strBuilder.Append("<td width=\"12%\">广告位类型</td>");
            strBuilder.Append("<td width=\"12%\">广告位尺寸</td>");
            strBuilder.Append("<td width=\"6%\">状态</td>");
            strBuilder.Append("<td width=\"12%\">选项</td>");
            strBuilder.Append("</tr>");
            /******************************************************************************************
             * 获取分页查询语句条件
             * ****************************************************************************************/
            string Params = string.Empty;
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "title": Params += " and Advertname like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StartDate) && VerifyCenter.VerifyDateTime(StartDate)) { Params += " and Addtime>='" + StartDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate + "'"; }
            /******************************************************************************************
             * 获取分页查询语句信息
             * ****************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "advid,advertName,width,height,picnumber,intro,isdisplay,modals";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "advId";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " advId desc";
            PageCenterConfig.Tablename = TableCenter.Advert;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Advert, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /******************************************************************************************
             * 循环遍历网页内容信息
             * ****************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td><input type=\"checkbox\" name=\"advId\" value=\"" + Rs["advId"] + "\" /></td>");
                strBuilder.Append("<td>" + Rs["advertName"] + "</td>");
                strBuilder.Append("<td>");
                switch (Rs["modals"].ToString())
                {
                    case "image": strBuilder.Append("图片类广告"); break;
                    case "swf": strBuilder.Append("动画类广告"); break;
                    case "code": strBuilder.Append("代码类广告"); break;
                    case "focus": strBuilder.Append("焦点图广告"); break;
                }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>" + Rs["width"] + "*" + Rs["height"] + "</td>");
                strBuilder.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strBuilder.Append("<a href=\"?action=display&val=0&advId=" + Rs["advId"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strBuilder.Append("<a href=\"?action=display&val=1&advId=" + Rs["advId"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=childlist&advId=" + Rs["advId"] + "\" title=\"查看广告列表\"><img src=\"template/images/ico/chart.png\" /></a>");
                strBuilder.Append("<a href=\"?action=addlist&advId=" + Rs["advId"] + "\"  title=\"添加子广告\"><img src=\"template/images/ico/add.png\" /></a>");
                strBuilder.Append("<a href=\"?action=edit&advId=" + Rs["advId"] + "\" title=\"编辑广告位\"><img src=\"template/images/ico/edit.png\" /></a>");
                strBuilder.Append("<a href=\"?action=del&advId=" + Rs["advId"] + "\"  title=\"删除广告位\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
            }
            /******************************************************************************************
             * 输出网页分页信息管理
             * ****************************************************************************************/
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td class=\"xingmu\" colspan=\"6\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"设为审核\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"取消审核\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"addjs\" value=\"生成广告\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/advert/default.html");
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
        /// 广告代码调用
        /// </summary>
        protected void strCode()
        {
            /******************************************************************************************
             * 构建网页查询条件
             * ****************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            /******************************************************************************************
             * 构建网页内容信息
             * ****************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"2\">广告位管理 >> 代码调用</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"search\">");
            strBuilder.Append("<td colspan=\"2\">");
            strBuilder.Append("<form action=\"?action=default\" method=\"get\">");
            strBuilder.Append("<select name=\"SearchType\">");
            strBuilder.Append("<option value=\"title\">搜名称</option>");
            strBuilder.Append("</select>");
            strBuilder.Append("&nbsp;<input type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strBuilder.Append("&nbsp;<input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"200\">广告位名称</td>");
            strBuilder.Append("<td>调用代码</td>");
            strBuilder.Append("</tr>");
            /******************************************************************************************
             * 构建分页查询条件
             * ****************************************************************************************/
            string Params = string.Empty;
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "title": Params += " and AdvertName like '%" + Keywords + "%'"; break;
                }
            }
            /******************************************************************************************
             * 构建分页查询语句
             * ****************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "advid,advertName,modals";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "advId";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " advId desc";
            PageCenterConfig.Tablename = TableCenter.Advert;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Advert, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /******************************************************************************************
             * 循环遍历网页内容
             * ****************************************************************************************/
            string codeText = "<script language=\"javascript\" src=\"/js/advert/{0}.js\"></script>";
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td>" + Rs["advertName"] + "</td>");
                strBuilder.Append("<td id=\"frm-code-adv" + Rs["advid"] + "\">");
                strBuilder.Append("<input type=\"text\" size=\"80\" class=\"inputtext\" value=\'" + string.Format(codeText, Rs["advId"].ToString()) + "\' />");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
            }
            /******************************************************************************************
             * 输出分页内容
             * ****************************************************************************************/
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"2\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td colspan=\"2\">");
            strBuilder.Append("<font color=\"#f00\">代码调用说明∶</font>单击复制代码,将复制内容粘贴到要使用的页面中即可.");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/advert/code.html");
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
        /// 添加广告位信息
        /// </summary>
        protected void Add()
        {
            /******************************************************************************************
             * 输出网页内容信息
             * ****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/advert/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "advertmode": strValue = FunctionBase.OptionList(new List<OptionMode>() {
                            new OptionMode(){Value="image",Text="图片类广告"},
                            new OptionMode(){Value="swf",Text="动画类广告(手机不支持)"},
                            new OptionMode(){Value="code",Text="代码类广告"},
                            new OptionMode(){Value="focus",Text="焦点图广告"},
                        }); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑分类
        /// </summary>
        protected void Update()
        {
            /******************************************************************************************
             * 验证请求参数合法性
             * ****************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toInt();
            if (advId == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAdvert]", new Dictionary<string, object>() {
                {"AdvID",advId}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /******************************************************************************************
             * 输出网页内容信息
             * ****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/advert/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, cRs["isDisplay"].ToString()); break;
                    case "advertmode": strValue = FunctionBase.OptionList(new List<OptionMode>() {
                            new OptionMode(){Value="image",Text="图片类广告"},
                            new OptionMode(){Value="swf",Text="动画类广告(手机不支持)"},
                            new OptionMode(){Value="code",Text="代码类广告"},
                            new OptionMode(){Value="focus",Text="焦点图广告"},
                        }, cRs["modals"].ToString()); break;
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
        /// <summary>
        /// 保存添加的子项
        /// </summary>
        protected void AddListSave()
        {
            /********************************************************************************************
             * 验证广告位是否存在
             * *******************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toInt();
            if (advId == "0") { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DataRow advRs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvert", new Dictionary<string, object>() {
                {"AdvID",advId}
            });
            if (advRs == null) { this.ErrorMessage("请求参数错误，你查找的数据不存在！"); Response.End(); }
            /********************************************************************************************
             * 验证其它的数据信息
             * ******************************************************************************************/
            string title = RequestHelper.GetRequest("title").toString();
            if (string.IsNullOrEmpty(title)) { this.ErrorMessage("请填写广告名称！"); Response.End(); }
            if (title.Length > 50) { this.ErrorMessage("广告名称长度请限制在50个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvertList", new Dictionary<string, object>() {
                {"Advid",advId},
                {"title",title}
            });
            if (cRs != null) { this.ErrorMessage("相同名称的广告已经存在了，请另外选择一个吧！"); Response.End(); }
            /********************************************************************************************
             * 验证其它数据信息
             * ******************************************************************************************/
            string strLink = RequestHelper.GetRequest("strLink").toString();
            if (strLink.Length > 150) { this.ErrorMessage("连接跳转地址请限制在150个字符内!"); Response.End(); }
            string strCode = RequestHelper.GetRequest("strCode", false).toString();
            if (strCode.Length > 3500) { this.ErrorMessage("广告代码请限制在3500个字符以内！"); Response.End(); }
            if (advRs["modals"].ToString() == "code" && string.IsNullOrEmpty(strCode)) { this.ErrorMessage("请输入广告代码！"); Response.End(); }
            string thumb = RequestHelper.GetRequest("thumb").toString();
            if (thumb.Length > 120) { this.ErrorMessage("广告图片地址请限制在120个字符以内！"); Response.End(); }
            if (advRs["modals"].ToString() != "code" && string.IsNullOrEmpty(thumb)) { this.ErrorMessage("请上传一个广告资源！"); Response.End(); }
            string intro = RequestHelper.GetRequest("intro").toString();
            if (intro.Length > 200) { this.ErrorMessage("广告描述信息请限制在200个汉字以内！"); Response.End(); }
            /******************************************************************************************************
             * 获取其它不需要验证的数据
             * ****************************************************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /******************************************************************************************************
             * 保存数据信息
             * ****************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["AdvID"] = advRs["advid"].ToString();
            thisDictionary["AdvertName"] = advRs["advertName"].ToString();
            thisDictionary["title"] = title;
            thisDictionary["strCode"] = strCode;
            thisDictionary["strLink"] = strLink;
            thisDictionary["Thumb"] = thumb;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            thisDictionary["intro"] = intro;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveAdvertList", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请刷新重试！"); Response.End(); }
            /*******************************************************************************************************
             * 重新生成新的广告
             * ******************************************************************************************************/
            try { new AdvertHelper().StartBuild(advRs); }
            catch { }
            /******************************************************************************************************
             * 开始输出数据
             * ****************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面.", falseUrl: "?action=childlist&advId=" + advId + "");
            Response.End();
        }
        /// <summary>
        /// 保存广告
        /// </summary>
        protected void SaveUpdateList()
        {
            /*********************************************************************************************
             * 保存数据信息
             * *******************************************************************************************/
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvertList", new Dictionary<string, object>() {
                {"Id",Id}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /********************************************************************************************
             * 验证广告位是否存在
             * *******************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toInt();
            if (advId == "0") { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DataRow advRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAdvert]", new Dictionary<string, object>() {
                {"AdvID",advId}
            });
            if (advRs == null) { this.ErrorMessage("请求参数错误，你查找的数据不存在！"); Response.End(); }
            /********************************************************************************************
             * 验证其它的数据信息
             * ******************************************************************************************/
            string title = RequestHelper.GetRequest("title").toString();
            if (string.IsNullOrEmpty(title)) { this.ErrorMessage("请填写广告名称！"); Response.End(); }
            if (title.Length > 50) { this.ErrorMessage("广告名称长度请限制在50个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAdvertList]", new Dictionary<string, object>() {
                {"Advid",advId},
                {"title",title}
            });
            if (cRs != null && cRs["id"].ToString() != Rs["id"].ToString()) { this.ErrorMessage("相同名称的广告已经存在了，请另外选择一个吧！"); Response.End(); }
            /********************************************************************************************
             * 验证其它数据信息
             * ******************************************************************************************/
            string strLink = RequestHelper.GetRequest("strLink").toString();
            if (strLink.Length > 150) { this.ErrorMessage("连接跳转地址请限制在150个字符内!"); Response.End(); }
            string strCode = RequestHelper.GetRequest("strCode", false).toString();
            if (strCode.Length > 3500) { this.ErrorMessage("广告代码请限制在3500个字符以内！"); Response.End(); }
            if (advRs["modals"].ToString() == "code" && string.IsNullOrEmpty(strCode)) { this.ErrorMessage("请输入广告代码！"); Response.End(); }
            string thumb = RequestHelper.GetRequest("thumb").toString();
            if (thumb.Length > 120) { this.ErrorMessage("广告图片地址请限制在120个字符以内！"); Response.End(); }
            if (advRs["modals"].ToString() != "code" && string.IsNullOrEmpty(thumb)) { this.ErrorMessage("请上传一个广告资源！"); Response.End(); }
            string intro = RequestHelper.GetRequest("intro").toString();
            if (intro.Length > 200) { this.ErrorMessage("广告描述信息请限制在200个汉字以内！"); Response.End(); }
            /******************************************************************************************************
             * 获取其它不需要验证的数据
             * ****************************************************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /******************************************************************************************************
             * 保存数据信息
             * ****************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["Id"] = Rs["Id"].ToString();
            thisDictionary["AdvID"] = advRs["advid"].ToString();
            thisDictionary["AdvertName"] = advRs["advertName"].ToString();
            thisDictionary["title"] = title;
            thisDictionary["strCode"] = strCode;
            thisDictionary["strLink"] = strLink;
            thisDictionary["Thumb"] = thumb;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            thisDictionary["intro"] = intro;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveAdvertList", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请刷新重试！"); Response.End(); }
            /*******************************************************************************************************
             * 重新生成新的广告
             * ******************************************************************************************************/
            try { new AdvertHelper().StartBuild(advRs); }
            catch { }
            /******************************************************************************************************
             * 开始输出数据
             * ****************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面.", falseUrl: "?action=childlist&advId=" + advId + "");
            Response.End();
        }

        /// <summary>
        /// 数据处理区域
        /// </summary>
        protected void AddSave()
        {
            /******************************************************************************************
             * 验证广告位信息
             * ****************************************************************************************/
            string advertName = RequestHelper.GetRequest("advertName").toString();
            if (string.IsNullOrEmpty(advertName)) { this.ErrorMessage("请填写广告位名称！"); Response.End(); }
            if (advertName.Length > 30) { this.ErrorMessage("广告位名称请限制在30个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvert", new Dictionary<string, object>() { 
                {"AdvertName",advertName}
            });
            if (cRs != null) { this.ErrorMessage("广告位名称已经存在！"); Response.End(); }
            /************************************************************************************************
             * 验证其他的数据信息
             * **********************************************************************************************/
            string modals = RequestHelper.GetRequest("modals").toString();
            if (string.IsNullOrEmpty(modals)) { this.ErrorMessage("请选择广告位类型！"); Response.End(); }
            string Template = RequestHelper.GetRequest("Template").toString();
            if (string.IsNullOrEmpty(Template)) { this.ErrorMessage("请填写广告位调用模版地址！"); Response.End(); }
            if (Template.Length > 50) { this.ErrorMessage("模版地址长度请限制在50个字符以内！"); Response.End(); }
            string width = RequestHelper.GetRequest("width").toString();
            if (string.IsNullOrEmpty(width)) { this.ErrorMessage("请填写广告位占尺寸宽度！"); Response.End(); }
            string height = RequestHelper.GetRequest("height").toString();
            if (string.IsNullOrEmpty(height)) { this.ErrorMessage("请填写广告位占尺寸高度！"); Response.End(); }
            string intro = RequestHelper.GetRequest("intro").toString();
            if (intro.Length > 150) { this.ErrorMessage("广告位描述内容请限制在150个汉字以内！"); Response.End(); }
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /*******************************************************************************************************
             * 开始保存数据
             * ******************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["advertName"] = advertName;
            thisDictionary["Width"] = width;
            thisDictionary["Height"] = height;
            thisDictionary["Template"] = Template;
            thisDictionary["Modals"] = modals;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["intro"] = intro;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveAdvert", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); Response.End(); }
            /*******************************************************************************************************
             * 输出数据处理结果
             * ******************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面.");
            Response.End();
        }

        /// <summary>
        /// 修改广告位
        /// </summary>
        protected void SaveUpdate()
        {
            /***********************************************************************************************
             * 验证数据是否存在
             * **********************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toInt();
            if (advId == "0") { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvert", new Dictionary<string, object>() {
                {"AdvID",advId}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /***********************************************************************************************
             * 验证广告位名称是否合法
             * **********************************************************************************************/
            string advertName = RequestHelper.GetRequest("advertName").toString();
            if (string.IsNullOrEmpty(advertName)) { this.ErrorMessage("请填写广告位名称！"); Response.End(); }
            if (advertName.Length > 30) { this.ErrorMessage("广告位名称请限制在30个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindAdvert", new Dictionary<string, object>() { 
                {"AdvertName",advertName}
            });
            if (cRs != null && cRs["AdvID"].ToString() != Rs["AdvID"].ToString()) { this.ErrorMessage("广告位名称已经存在！"); Response.End(); }
            /************************************************************************************************
             * 验证其他的数据信息
             * **********************************************************************************************/
            string modals = RequestHelper.GetRequest("modals").toString();
            if (string.IsNullOrEmpty(modals)) { this.ErrorMessage("请选择广告位类型！"); Response.End(); }
            string Template = RequestHelper.GetRequest("Template").toString();
            if (string.IsNullOrEmpty(Template)) { this.ErrorMessage("请填写广告位调用模版地址！"); Response.End(); }
            if (Template.Length > 50) { this.ErrorMessage("模版地址长度请限制在50个字符以内！"); Response.End(); }
            string width = RequestHelper.GetRequest("width").toString();
            if (string.IsNullOrEmpty(width)) { this.ErrorMessage("请填写广告位占尺寸宽度！"); Response.End(); }
            string height = RequestHelper.GetRequest("height").toString();
            if (string.IsNullOrEmpty(height)) { this.ErrorMessage("请填写广告位占尺寸高度！"); Response.End(); }
            string intro = RequestHelper.GetRequest("intro").toString();
            if (intro.Length > 150) { this.ErrorMessage("广告位描述内容请限制在150个汉字以内！"); Response.End(); }
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /*******************************************************************************************************
             * 开始保存数据
             * ******************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["advid"] = Rs["advid"].ToString();
            thisDictionary["advertName"] = advertName;
            thisDictionary["Width"] = width;
            thisDictionary["Height"] = height;
            thisDictionary["Template"] = Template;
            thisDictionary["Modals"] = modals;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["intro"] = intro;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveAdvert", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); Response.End(); }
            /*******************************************************************************************************
             * 重新生成新的广告
             * ******************************************************************************************************/
            try { new AdvertHelper().StartBuild(thisRs); }
            catch { }
            /*******************************************************************************************************
             * 输出数据处理结果
             * ******************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面.");
            Response.End();
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        protected void Display()
        {
            /******************************************************************************************
             * 获取请求参数信息
             * *****************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toString();
            if (string.IsNullOrEmpty(advId)) { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            string val = RequestHelper.GetRequest("val").toInt();
            /******************************************************************************************
             * 开始保存数据
             * *****************************************************************************************/
            DbHelper.Connection.Update(TableCenter.Advert, new Dictionary<string, string>() {
                {"isDisplay",val}
            }, Params: " and advId in (" + advId + ")");
            /******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 设置子广告状态
        /// </summary>
        protected void DisplayList()
        {

            /******************************************************************************************
             * 获取请求参数信息
             * *****************************************************************************************/
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            string val = RequestHelper.GetRequest("val").toInt();
            /******************************************************************************************
             * 开始保存数据
             * *****************************************************************************************/
            DbHelper.Connection.Update(TableCenter.AdvertList, new Dictionary<string, string>() {
                {"isDisplay",val}
            }, Params: " and Id in (" + Id + ")");
            /******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除广告位
        /// </summary>
        protected void Delete()
        {
            /**********************************************************************************************
             * 处理数据信息
             * ********************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toString();
            if (string.IsNullOrEmpty(advId)) { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Advert, Params: " and advId in (" + advId + ")");
            if (Tab == null || Tab.Rows.Count <= 0) { this.ErrorMessage("没有可删除的数据！"); Response.End(); }
            /**********************************************************************************************
             * 开始删除数据
             * ********************************************************************************************/
            foreach (DataRow cRs in Tab.Rows)
            {
                DbHelper.Connection.ExecuteProc("[Stored_DeleteAdvert]", new Dictionary<string, object>() {
                    {"AdvertID",cRs["AdvID"].ToString()}
                });
            }
            /**********************************************************************************************
             * 输出数据处理结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除广告
        /// </summary>
        protected void DeleteList()
        {
            /************************************************************************
             * 验证数据合法性
             * **********************************************************************/
            string advId = RequestHelper.GetRequest("advId").toInt();
            if (advId == "0") { this.ErrorMessage("参数错误，请刷新网页重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAdvert]", new Dictionary<string, object>() {
                {"AdvID",advId}
            });
            if (cRs == null) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            /************************************************************************
             * 开始删除数据
             * **********************************************************************/
            string strList = RequestHelper.GetRequest("Id").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DbHelper.Connection.ExecuteProc("[Stored_DeleteAdvertList]", new Dictionary<string, object>() {
                {"AdvertID",advId},
                {"ListID",strList}
            });
            /************************************************************************
             * 处理结果
             * **********************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 批量生成广告
        /// </summary>
        protected void AddJavaScript()
        {
            /***************************************************************************************************
             * 验证数据的合法性
             * *************************************************************************************************/
            string advId = RequestHelper.GetRequest("advId").toString();
            if (string.IsNullOrEmpty(advId)) { this.ErrorMessage("参数错误，请选择一条数据！"); Response.End(); }
            /***************************************************************************************************
             * 开始生成广告信息
             * *************************************************************************************************/
            //try
            //{
                DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Advert, Params: " and isDisplay=1 and advId in (" + advId + ")");
                if (Tab == null || Tab.Rows.Count <= 0) { this.ErrorMessage("没有可生成的广告位！"); Response.End(); }
                foreach (DataRow Rs in Tab.Rows)
                {
                    new AdvertHelper().StartBuild(Rs);
                }
            //}
            //catch { }
            /***************************************************************************************************
             * 返回数据处理结果
             * *************************************************************************************************/
            this.ErrorMessage("恭喜，你所选择的广告位已经生成成功！");
            Response.End();
        }
    }

    /// <summary>
    /// 开始生成广告
    /// </summary>
    public class AdvertHelper
    {
        /// <summary>
        /// 获取模板地址内容信息
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="Fun"></param>
        public void FileReader(string filePath, Action<string> Fun)
        {
            string strResponse = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    filePath = filePath.Replace("{app}", Win.ApplicationPath);
                    if (System.IO.File.Exists(ServerPath(filePath))) { strResponse = System.IO.File.ReadAllText(ServerPath(filePath), System.Text.Encoding.Default); }
                    else { strResponse = "not find template:" + filePath + ""; }
                    if (strResponse.Contains("{app}")) { strResponse = strResponse.Replace("{app}", Win.ApplicationPath); }
                }
            }
            catch { }
            if (Fun != null) { try { Fun(strResponse); } catch { } }
        }
        /// <summary>
        /// 将文件相对路径转换为绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string ServerPath(string filePath)
        {
            try
            {
                if (!filePath.Contains(":")) { filePath = System.Web.HttpContext.Current.Server.MapPath(filePath); }
                filePath = filePath.Replace("\\", "/").Replace("//", "/").Replace("///", "/");
            }
            catch { }
            return filePath;
        }

        /// <summary>
        /// 开始生成广告信息
        /// </summary>
        public void StartBuild(DataRow cRs)
        {
            try
            {
                FileReader(cRs["template"].ToString(), (strResponse) =>
                {
                    DataTable Tab = DbHelper.Connection.ExecuteFindTable("[Stored_FindAdvertList]", new Dictionary<string, object>() {
                        {"AdvID",cRs["AdvID"].ToString()},
                        {"isDisplay","1"}
                    });
                    /***********************************************************************************
                     * 替换默认的数据信息
                     * *********************************************************************************/
                    try
                    {
                        if (Tab != null && Tab.Rows.Count >= 1)
                        {
                            strResponse = strResponse.Replace("{$thumb}", Tab.Rows[0]["thumb"].ToString());
                            strResponse = strResponse.Replace("{$url}", Tab.Rows[0]["strlink"].ToString());
                            strResponse = strResponse.Replace("{$title}", Tab.Rows[0]["title"].ToString());
                            strResponse = strResponse.Replace("{$strcode}", Tab.Rows[0]["strcode"].ToString());
                        }
                    }
                    catch { }
                    /***********************************************************************************
                     * 替换广告位中出现的常用字段信息
                     * *********************************************************************************/
                    try
                    {
                        strResponse = strResponse.Replace("{$width}", cRs["width"].ToString());
                        strResponse = strResponse.Replace("{$height}", cRs["height"].ToString());
                        strResponse = strResponse.Replace("{$id}", cRs["advId"].ToString());
                        strResponse = strResponse.Replace("{$advid}", cRs["advId"].ToString());
                        if (!string.IsNullOrEmpty(strResponse) && strResponse.Contains("{$json}"))
                        {
                            getSelection(Tab, (strJson) => { strResponse = strResponse.Replace("{$json}", strJson); });
                        }
                    }
                    catch { }
                    /***********************************************************************************
                     * 开始保存文件信息
                     * *********************************************************************************/
                    try
                    {
                        string SaveDirectory = ServerPath("~/js/advert");
                        if (!System.IO.Directory.Exists(SaveDirectory))
                        {
                            System.IO.Directory.CreateDirectory(SaveDirectory);
                        }
                        string SaveFilename = string.Format("{0}/{1}.js", SaveDirectory, cRs["advid"]);
                        if (System.IO.File.Exists(SaveFilename)) { System.IO.File.Delete(SaveFilename); }
                        System.IO.File.AppendAllText(SaveFilename, strResponse, System.Text.Encoding.Default);
                    }
                    catch { }
                });
            }
            catch { }
        }
        /// <summary>
        /// 获取到图集中的JSON数据
        /// </summary>
        /// <param name="Tab"></param>
        public void getSelection(DataTable Tab, Action<string> Fun)
        {
            try
            {
                StringBuilder JsonBuilder = new StringBuilder();
                JsonBuilder.Append("[");
                int SelectedIndex = 0;
                foreach (DataRow sRs in Tab.Rows)
                {
                    if (SelectedIndex != 0) { JsonBuilder.Append(","); }
                    else { SelectedIndex = 1; }
                    JsonBuilder.Append("{");
                    JsonBuilder.Append("\"success\":\"true\"");
                    JsonBuilder.Append(",\"url\":\"" + sRs["strlink"] + "\"");
                    JsonBuilder.Append(",\"title\":\"" + sRs["title"] + "\"");
                    JsonBuilder.Append(",\"thumb\":\"" + sRs["thumb"] + "\"");
                    JsonBuilder.Append(",\"strcode\":\"" + sRs["strcode"] + "\"");
                    JsonBuilder.Append("}");
                }
                JsonBuilder.Append("]");
                /**********************************************************************
                * 执行返回函数
                * *********************************************************************/
                try { if (Fun != null) { Fun(JsonBuilder.ToString()); } }
                catch { }
            }
            catch { }

        }
    }

}