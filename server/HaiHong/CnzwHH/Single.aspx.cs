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
    public partial class Single : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "del": this.VerificationRole("内容管理"); this.Delete(); Response.End(); break;
                case "editsave": this.VerificationRole("内容管理"); UpdateSave(); Response.End(); break;
                case "edit": this.VerificationRole("内容管理"); Update(); Response.End(); break;
                case "display": this.VerificationRole("内容管理"); Display(); Response.End(); break;
                case "add": this.VerificationRole("内容管理"); Add(); Response.End(); break;
                case "save": this.VerificationRole("内容管理"); AddSave(); Response.End(); break;
                default: this.VerificationRole("内容管理"); strDefault(); Response.End(); break;
            }
            Response.End();
        }
        /// <summary>
        /// 站点管理
        /// </summary>
        protected void strDefault()
        {
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            string ParentName = RequestHelper.GetRequest("ParentName").toString();
            if (string.IsNullOrEmpty(ParentName)) { ParentName = "一级分类"; }
            string display = RequestHelper.GetRequest("display").toString();
            if (string.IsNullOrEmpty(display)) { display = "none"; }
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"7\">单页管理 >> 单页列表</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strBuilder.Append("<td>分类名称</td>");
            strBuilder.Append("<td width=\"8%\">上级目录</td>");
            strBuilder.Append("<td width=\"8%\">分类排序</td>");
            strBuilder.Append("<td width=\"8%\">分类ID</td>");
            strBuilder.Append("<td width=\"6%\">状态</td>");
            strBuilder.Append("<td width=\"12%\">选项</td>");
            strBuilder.Append("</tr>");
            string Params = " and ParentID=" + ParentID + "";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "Id,title,Identify,ParentID,isDisplay,SortID,isChild,isHtml";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID desc,Id Desc";
            PageCenterConfig.Tablename = TableCenter.Single;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Single, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr style=\"background:#fcc\" class=\"hback\">");
                strBuilder.Append("<td><input type=\"checkbox\" name=\"Id\" value=\"" + Rs["Id"] + "\" /></td>");
                strBuilder.Append("<td>");
                if (Rs["isChild"].ToString() != "0")
                { strBuilder.Append("<img src=\"template/images/b.gif\" /><a title=\"点击查看子分类\" href=\"?action=default&ParentId=" + Rs["Id"] + "\">" + Rs["title"] + "</a>"); }
                else { strBuilder.Append("<img src=\"template/images/s.gif\" /><a title=\"没有子分类\">" + Rs["title"] + "</a>"); }
                if (Rs["isHtml"].ToString() == "0") { strBuilder.Append("<font color=\"#f00\">[动]</font>"); }
                else { strBuilder.Append("<font color=\"#f00\">[静]</font>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>" + ParentName + "</td>");
                strBuilder.Append("<td>"+Rs["SortID"]+"</td>");
                strBuilder.Append("<td>"+Rs["Id"]+"</td>");
                strBuilder.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strBuilder.Append("<a href=\"?action=display&val=0&Id=" + Rs["Id"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strBuilder.Append("<a href=\"?action=display&val=1&Id=" + Rs["Id"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=add&ParentID=" + Rs["Id"] + "\" title=\"添加子分类\"><img src=\"template/images/ico/add.png\" /></a>");
                strBuilder.Append("<a href=\"?action=edit&Id=" + Rs["Id"] + "\" title=\"编辑分类\"><img src=\"template/images/ico/edit.png\" /></a>");
                strBuilder.Append("<a href=\"?action=del&Id=" + Rs["Id"] + "\"  title=\"删除分类\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
                if (display == "show" && Rs["isChild"].ToString() != "0") { strBuilder.Append(ShowChild(Rs["Id"].ToString(), Rs["title"].ToString(), "")); }
            }
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"7\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td class=\"xingmu\" colspan=\"7\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"锁定\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"解锁\" onclick=\"commandOperate(this)\" />");
            if (display == "none") { strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"全部展开\" onclick=\"window.location='?action=default&display=show'\" />"); }
            else { strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"全部折合\" onclick=\"window.location='?action=default&display=none'\" />"); }
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");

            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/single/default.html");
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
        /// 显示子栏目
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="ParentName"></param>
        /// <param name="ShowText"></param>
        /// <returns></returns>
        public string ShowChild(string ParentID, string ParentName, string ShowText)
        {
            StringBuilder strBuilder = new StringBuilder();
            ShowText = ShowText + "━";
            string Params = " and ParentID="+ParentID+" order by  SortID desc,Id Desc";
            string Columns = "Id,title,ParentID,isDisplay,SortID,isChild,isHtml";
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Single, columns: Columns, Params: Params);
            foreach (DataRow Rs in Tab.Rows)
            {

                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td><input type=\"checkbox\" name=\"Id\" value=\"" + Rs["Id"] + "\" /></td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a>┗" + ShowText + Rs["title"] + "</a>");
                if (Rs["isHtml"].ToString() == "0") { strBuilder.Append("<font color=\"#f00\">[动]</font>"); }
                else { strBuilder.Append("<font color=\"#f00\">[静]</font>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>" + ParentName + "</td>");
                strBuilder.Append("<td>" + Rs["SortID"] + "</td>");
                strBuilder.Append("<td>" + Rs["Id"] + "</td>");
                strBuilder.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strBuilder.Append("<a href=\"?action=display&val=0&Id=" + Rs["Id"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strBuilder.Append("<a href=\"?action=display&val=1&Id=" + Rs["Id"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=add&ParentID=" + Rs["Id"] + "\" title=\"添加子分类\"><img src=\"template/images/ico/add.png\" /></a>");
                strBuilder.Append("<a href=\"?action=edit&Id=" + Rs["Id"] + "\" title=\"编辑分类\"><img src=\"template/images/ico/edit.png\" /></a>");
                strBuilder.Append("<a href=\"?action=del&Id=" + Rs["Id"] + "\"  title=\"删除分类\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
                if (Rs["isChild"].ToString() != "0") { strBuilder.Append(ShowChild(Rs["Id"].ToString(), Rs["title"].ToString(), "")); }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 添加站点
        /// </summary>
        protected void Add()
        {
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID != "0")
            {
                DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Single, Params: " and Id=" + ParentID + "");
                if (cRs == null) { this.ErrorMessage("对不起,你查找的分类不存在！"); Response.End(); }
            }
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/single/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": strValue = SingleHelper.Options("0", ParentID); break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() { 
                        new RadioMode(){Name="isdisplay",Value="1",Text="开启"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "file": strValue = FunctionBase.UploadControl("Thumb", tips: "封面图片大小请参照前台UI设计大小",selector:true); break;
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
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Single, Params: " and Id=" + Id + "");
            if (cRs == null) { this.ErrorMessage("拉取数据失败，你查找的信息不存在！"); Response.End(); }
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/single/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "ishtml": if (cRs["isHtml"].ToString() == "1") { strValue = "selected"; } break;
                    case "options": strValue = SingleHelper.Options("0", cRs["ParentID"].ToString()); break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() { 
                        new RadioMode(){Name="isdisplay",Value="1",Text="开启"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    },cRs["isDisplay"].ToString()); break;
                    case "file": strValue = FunctionBase.UploadControl("Thumb",defaultText:cRs["thumb"].ToString(), tips: "封面图片大小请参照前台UI设计大小", selector: true); break;
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
        /// 数据处理区域
        /// </summary>
        protected void AddSave()
        {
            string title = RequestHelper.GetRequest("title").toString();
            if (string.IsNullOrEmpty(title)) { this.ErrorMessage("请填写页面标题!"); Response.End(); }
            if (title.Length > 150) { this.ErrorMessage("页面标题请限制在150个汉字以内！"); Response.End(); }
            string Identify = RequestHelper.GetRequest("Identify").toString();
            if (string.IsNullOrEmpty(Identify)) { this.ErrorMessage("请填写页面标识！"); Response.End(); }
            if (Identify.Length > 50) { this.ErrorMessage("页面标识请限制在50个字符以内！"); Response.End(); }
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            string Thumb = RequestHelper.GetRequest("Thumb").toString();
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length > 150) { this.ErrorMessage("封面图片地址请限制在150个字符以内！"); Response.End(); }
            string intro = RequestHelper.GetRequest("intro").toString();
            if (intro.Length > 200) { this.ErrorMessage("页面描述请限制在200个汉字以内！"); Response.End(); }
            string cTemplate = RequestHelper.GetRequest("cTemplate").toString();
            if (string.IsNullOrEmpty(cTemplate)) { this.ErrorMessage("请选择单页面模版！"); Response.End(); }
            if (cTemplate.Length > 120) { this.ErrorMessage("单页面模版地址请限制在120个字符以内！"); Response.End(); }
            /***********************************************************************
             * 检查标题或者标识是否已经存在
             * *********************************************************************/
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Single, columns: "Id", Params: " and title='" + title + "' and ParentID=" + ParentID + "");
            if (cRs != null) { this.ErrorMessage("同目录下标题名称已经存在，请另外选择一个吧！"); Response.End(); }
            DataRow oRs = DbHelper.Connection.FindRow(TableCenter.Single, columns: "Id", Params: " and Identify='" + Identify + "'");
            if (oRs != null) { this.ErrorMessage("页面标识已经存在了，请另外选择一个吧！"); Response.End(); }
            /***********************************************************************
             * 获取其它不用验证的内容
             * *********************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string content = RequestHelper.GetBodyContent("content");
            string isHtml = RequestHelper.GetRequest("isHtml").toInt();
            /***********************************************************************
             * 开始保存数据
             * *********************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["SingleID"] = "0";
            oDictionary["ParentID"] = ParentID;
            oDictionary["title"] = title;
            oDictionary["Identify"] = Identify;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["strDesc"] = intro;
            oDictionary["thumb"] = Thumb;
            oDictionary["content"] = content;
            oDictionary["SortID"] = SortID;
            oDictionary["cTemplate"] = cTemplate;
            oDictionary["isHtml"] = isHtml;
            DataRow SingleRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveSingle", oDictionary);
            if (SingleRs == null) { this.ErrorMessage("拉取数据信息失败,请刷新网页重试！"); }
            /********************************************************************************************
             * 更改父类的属性ID
             * *******************************************************************************************/
            if (ParentID != "0") { this.UpdateChild(ParentID); }
            /*********************************************************************************************
             * 更新网页内容
             * *******************************************************************************************/
            if (SingleRs["isHtml"].ToString()=="1" && SingleRs!=null)
            {
                string TemplateDir = this.GetParameter("TemplateDir", "siteXML").toString();
                if (string.IsNullOrEmpty(TemplateDir)) { TemplateDir = "template"; }
                TemplateDir = Win.ApplicationPath + "/" + TemplateDir;
                SessionHelper.Add("make_singleid", SingleRs["Id"].ToString());
                string strTemplate = SingleRs["cTemplate"].ToString();
                strTemplate = strTemplate.Replace("{@dir}", TemplateDir);
                Fooke.Release.ReleaseHelper ReleaseMaster = new Fooke.Release.ReleaseHelper();
                string strReader = ReleaseMaster.ReleaseSingle(strTemplate, SingleRs);
                ReleaseMaster.AppendText("~/html/" + SingleRs["Identify"] + ".html", strReader);
            }
            /********************************************************************************************
             * 开始输出结果
             * *******************************************************************************************/
            this.ConfirmMessage("单页面添加成功,点击确定将继续停留在当前界面.");
            Response.End();
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        protected void UpdateSave()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Single, Params: " and Id=" + Id + "");
            if (cRs == null) { this.ErrorMessage("拉取数据失败，你查找的数据不存在！"); Response.End(); }

            string title = RequestHelper.GetRequest("title").toString();
            if (string.IsNullOrEmpty(title)) { this.ErrorMessage("请填写页面标题!"); Response.End(); }
            if (title.Length > 150) { this.ErrorMessage("页面标题请限制在150个汉字以内！"); Response.End(); }
            string Identify = RequestHelper.GetRequest("Identify").toString();
            if (string.IsNullOrEmpty(Identify)) { this.ErrorMessage("请填写页面标识！"); Response.End(); }
            if (Identify.Length > 50) { this.ErrorMessage("页面标识请限制在50个字符以内！"); Response.End(); }
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID == cRs["Id"].ToString()) { this.ErrorMessage("保存失败,页面ID不能与ParentID相同"); Response.End(); }
            string Thumb = RequestHelper.GetRequest("Thumb").toString();
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length > 150) { this.ErrorMessage("封面图片地址请限制在150个字符以内！"); Response.End(); }
            string intro = RequestHelper.GetRequest("intro").toString();
            if (intro.Length > 200) { this.ErrorMessage("页面描述请限制在200个汉字以内！"); Response.End(); }
            string cTemplate = RequestHelper.GetRequest("cTemplate").toString();
            if (string.IsNullOrEmpty(cTemplate)) { this.ErrorMessage("请选择单页面模版！"); Response.End(); }
            if (cTemplate.Length > 120) { this.ErrorMessage("单页面模版地址请限制在120个字符以内！"); Response.End(); }
            /***********************************************************************
             * 检查标题或者标识是否已经存在
             * *********************************************************************/
            DataRow vRs = DbHelper.Connection.FindRow(TableCenter.Single, columns: "Id", Params: " and Id<>" + Id + " and title='" + title + "' and ParentID=" + ParentID + "");
            if (vRs != null) { this.ErrorMessage("同目录下标题名称已经存在，请另外选择一个吧！"); Response.End(); }
            DataRow oRs = DbHelper.Connection.FindRow(TableCenter.Single, columns: "Id", Params: " and Id<>" + Id + " and Identify='" + Identify + "'");
            if (oRs != null) { this.ErrorMessage("页面标识已经存在了，请另外选择一个吧！"); Response.End(); }
            /***********************************************************************
             * 获取其它不用验证的内容
             * *********************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string content = RequestHelper.GetBodyContent("content");
            string isHtml = RequestHelper.GetRequest("ishtml").toInt();
            /***********************************************************************
             * 开始保存数据
             * *********************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["SingleID"] = Id;
            oDictionary["ParentID"] = ParentID;
            oDictionary["title"] = title;
            oDictionary["Identify"] = Identify;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["strDesc"] = intro;
            oDictionary["thumb"] = Thumb;
            oDictionary["content"] = content;
            oDictionary["SortID"] = SortID;
            oDictionary["cTemplate"] = cTemplate;
            oDictionary["isHtml"] = isHtml;
            DataRow SingleRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveSingle", oDictionary);
            if (SingleRs == null) { this.ErrorMessage("拉取数据信息失败,请刷新网页重试！"); }
            /********************************************************************************************
             * 更改父类的属性ID
             * *******************************************************************************************/
            if (ParentID != "0" && cRs["ParentID"].ToString() != ParentID) { this.UpdateChild(ParentID); }
            if (cRs["ParentID"].ToString() != "0" && cRs["ParentID"].ToString() != ParentID) { this.UpdateChild(cRs["ParentID"].ToString()); }
            /*********************************************************************************************
             * 更新网页内容
             * *******************************************************************************************/
            if (SingleRs["isHtml"].ToString() == "1" && SingleRs != null)
            {
                string TemplateDir = this.GetParameter("TemplateDir", "siteXML").toString();
                if (string.IsNullOrEmpty(TemplateDir)) { TemplateDir = "template"; }
                TemplateDir = Win.ApplicationPath + "/" + TemplateDir;
                SessionHelper.Add("make_singleid", SingleRs["Id"].ToString());
                string strTemplate = SingleRs["cTemplate"].ToString();
                strTemplate = strTemplate.Replace("{@dir}", TemplateDir);
                Fooke.Release.ReleaseHelper ReleaseMaster = new Fooke.Release.ReleaseHelper();
                string strReader = ReleaseMaster.ReleaseSingle(strTemplate, SingleRs);
                ReleaseMaster.AppendText("~/html/" + SingleRs["Identify"] + ".html", strReader);
            }
            /*********************************************************************************************
             * 开始输出数据
             * ********************************************************************************************/
            this.ConfirmMessage("单页面编辑成功,点击确定将继续停留在当前界面.");
            Response.End();
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        protected void Display()
        {
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            string val = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["isDisplay"] = val;
            DbHelper.Connection.Update(TableCenter.Single, dictionary, Params: " and Id in(" + Id + ")");
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除文章分类
        /// </summary>
        protected void Delete()
        {
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Single, Params: " and Id in (" + Id + ")");
            foreach (DataRow Rs in Tab.Rows)
            {
                try
                {
                    DbHelper.Connection.Delete(TableCenter.Single, Params: " and Id in (select Id from GetSingleChild(" + Rs["Id"] + "))");
                    DbHelper.Connection.Delete(TableCenter.Single, Params: " and Id=" + Rs["Id"] + "");
                    if (Rs["ParentId"].ToString() != "0") { UpdateChild(Rs["ParentID"].ToString()); }
                }
                catch { }
            }
            this.History();
            Response.End();
        }
        /// <summary>
        /// 更新子分类个数
        /// </summary>
        /// <param name="ParentID"></param>
        public void UpdateChild(string ParentID)
        {
            try
            {
                DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Single, columns: "count(Id) as total", Params: " and ParentID=" + ParentID + "");
                if (cRs != null)
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary["isChild"] = cRs["total"].ToString();
                    DbHelper.Connection.Update(TableCenter.Single, dictionary, Params: " and Id=" + ParentID + "");
                }
            }
            catch { }
        }
    }
}