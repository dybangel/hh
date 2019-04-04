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
    public partial class Class : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /***********************************************************************
             * 页面异步请求处理
             * *********************************************************************/
            switch (strRequest)
            {
                case "getclass": this.VerificationRole("内容管理"); this.GetClassOptions(); Response.End(); break;
                case "getchild": this.VerificationRole("内容管理"); this.GetClassChild(); Response.End(); break;
                case "savest": this.VerificationRole("内容管理"); this.SaveEditor(); Response.End(); break;
                case "save": this.VerificationRole("内容管理"); this.AddSave(); Response.End(); ; break;
                case "add": this.VerificationRole("内容管理"); this.Add(); Response.End(); break;
                case "del": this.VerificationRole("内容管理"); this.Delete(); Response.End(); break;
                case "edit": this.VerificationRole("内容管理"); this.Update(); Response.End(); break;
                case "editsave": this.VerificationRole("内容管理"); this.SaveUpdate(); Response.End(); break;
                case "display": this.VerificationRole("内容管理"); this.Display(); Response.End(); break;
                case "isOper": this.VerificationRole("内容管理"); this.SaveOper(); Response.End(); break;
                default: this.VerificationRole("内容管理"); strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 异步获取到当前栏目下的子栏目
        /// </summary>
        protected void GetClassChild()
        {
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            string cuteTxt = RequestHelper.GetRequest("cuteTxt", false).toString();
            string ParentName = RequestHelper.GetRequest("ParentName").toString();
            string strParams = " and ParentID=" + ParentID + " order by sortid desc,classid desc";
            string showTxt = "&nbsp;&nbsp;" + cuteTxt;
            string columns = "classId,SortID,className,channelid,channelname,identify,isDisplay,isChild,isOper";
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Class, columns: columns, Params: strParams);
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table1\">");
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td width=\"15\"><input type=\"checkbox\" name=\"classId\" value=\"" + Rs["classId"] + "\" /></td>");
                strText.Append("<td width=\"200\">" + showTxt);
                if (Rs["isChild"].ToString() != "0") { strText.Append("<img  title=\"点击展开子分类\" onclick=\"GetChild(" + Rs["classId"].ToString() + ",'" + Rs["className"] + "','" + showTxt + "',this)\" src=\"template/images/b.gif\" />"); }
                else { strText.Append("<img title=\"已展开全部子分类\" src=\"template/images/s.gif\" />"); }
                strText.Append("" + Rs["className"] + "");
                switch (Rs["isOper"].ToString())
                {
                    case "0": strText.Append("<font color=\"#F00\">[动]</font>"); break;
                    case "1": strText.Append("<font color=\"#F00\">[伪]</font>"); break;
                    case "2": strText.Append("<font color=\"#F00\">[动,静]</font>"); break;
                    case "3": strText.Append("<font color=\"#F00\">[静]</font>"); break;
                }
                strText.Append("</td>");
                strText.Append("<td width=\"120\">" + ParentName + "</td>");
                strText.Append("<td width=\"120\"><a href=\"?action=default&channelid=" + Rs["channelid"] + "\">" + Rs["channelname"] + "</a></td>");
                strText.Append("<td width=\"80\"><input type=\"text\" isnumeric=\"true\" onblur=\"SaveEditor(this," + Rs["classId"].ToString() + ")\" size=\"5\" class=\"inputtext center\" value=\"" + Rs["SortID"] + "\" /></td>");
                strText.Append("<td width=\"80\">");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&classId=" + Rs["classId"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&classId=" + Rs["classId"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=add&channelId=" + Rs["channelId"] + "&ParentID=" + Rs["classId"] + "\" title=\"添加子栏目\"><img src=\"images/ico/add.png\" /></a>");
                strText.Append("<a href=\"?action=edit&classId=" + Rs["classId"] + "\" title=\"修改栏目\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=del&classId=" + Rs["classId"] + "\" title=\"删除栏目\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
                if (Rs["isChild"].ToString() != "0") { strText.Append("<tr style=\"display:none\" class=\"hback\"><td style=\"padding:0px;\" id=\"_child_" + Rs["classid"] + "\" colspan=\"7\"></td></tr>"); }
            }
            strText.Append("</table>");
            Response.Write(strText.ToString());
            Response.End();
        }

        protected void GetClassOptions()
        {
            string ChannelID = RequestHelper.GetRequest("channelid").toInt();
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<select name=\"ParentID\">");
            strBuilder.Append("<option name=\"0\">作为一级分类[根目录]</option>");
            strBuilder.Append(ClassHelper.Options(ChannelID: ChannelID));
            strBuilder.Append("</select>");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 栏目列表
        /// </summary>
        protected void strDefault()
        {
            string isBrowse = CookieHelper.Get("fooke_jsapi_isBrowse").toInt();
            string ChannelID = RequestHelper.GetRequest("channelID").toInt();
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"7\">栏目管理 >> 栏目列表</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"15\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"200\">栏目名称</td>");
            strText.Append("<td width=\"120\">上级目录</td>");
            strText.Append("<td width=\"120\">所属模型</td>");
            strText.Append("<td width=\"80\">栏目排序</td>");
            strText.Append("<td width=\"80\">是否使用</td>");
            strText.Append("<td>操作选项</td>");
            strText.Append("</tr>");
            string Params = " and ParentID=0";
            if (ChannelID != "0") { Params += " and ChannelID = " + ChannelID + ""; }
            string columns = "classId,SortID,className,channelid,channelname,Identify,isDisplay,isChild,isOper";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = columns;
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "classId";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID Desc,classId desc";
            PageCenterConfig.Tablename = TableCenter.Class;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Class, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr style=\"background:#fef0f0\" class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"classId\" value=\"" + Rs["classId"] + "\" /></td>");
                strText.Append("<td>");
                if (Rs["isChild"].ToString() != "0" && isBrowse != "1") { strText.Append("<img  title=\"点击展开子分类\" onclick=\"GetChild(" + Rs["classId"].ToString() + ",'" + Rs["className"] + "','',this)\" src=\"template/images/b.gif\" />"); }
                else { strText.Append("<img title=\"已展开全部子分类\" src=\"template/images/s.gif\" />"); }
                strText.Append("" + Rs["className"] + "");
                switch (Rs["isOper"].ToString())
                {
                    case "0": strText.Append("<font color=\"#F00\">[动]</font>"); break;
                    case "1": strText.Append("<font color=\"#F00\">[伪]</font>"); break;
                    case "2": strText.Append("<font color=\"#F00\">[动,静]</font>"); break;
                    case "3": strText.Append("<font color=\"#F00\">[静]</font>"); break;
                }
                strText.Append("</td>");
                strText.Append("<td>一级分类</td>");
                strText.Append("<td><a href=\"?action=default&channelid=" + Rs["channelid"] + "\">" + Rs["channelname"] + "</a></td>");
                strText.Append("<td><input type=\"text\" isnumeric=\"true\" onblur=\"SaveEditor(this," + Rs["classId"].ToString() + ")\" size=\"5\" class=\"inputtext center\" value=\"" + Rs["SortID"] + "\" /></td>");
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&classId=" + Rs["classId"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&classId=" + Rs["classId"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=add&channelId=" + Rs["channelId"] + "&ParentID=" + Rs["classId"] + "\" title=\"添加子栏目\"><img src=\"images/ico/add.png\" /></a>");
                strText.Append("<a href=\"?action=edit&classId=" + Rs["classId"] + "\" title=\"修改栏目\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=del&classId=" + Rs["classId"] + "\" title=\"删除栏目\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
                if (Rs["isChild"].ToString() != "0" && isBrowse == "1") { strText.Append(this.GetChildList(Rs["classId"].ToString(), Rs["className"].ToString(), "")); }
                else { strText.Append("<tr style=\"display:none\" class=\"hback\"><td style=\"padding:0px;\" id=\"_child_" + Rs["classid"] + "\" colspan=\"7\"></td></tr>"); }
            }
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"7\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"7\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"使用\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"关闭\" onclick=\"commandOperate(this)\" />");
            if (isBrowse != "1") { strText.Append("<input type=\"button\" class=\"button\" value=\"展开所有栏目\" onclick=\"OpenOfAll()\" />"); }
            else { strText.Append("<input type=\"button\" class=\"button\" value=\"折叠所有栏目\" onclick=\"CloseOfAll()\" />"); }

            strText.Append("<select name=\"isOper\">");
            strText.Append("<option value=\"-1\">设置运行模式</option>");
            strText.Append("<option value=\"0\">动态aspx</option>");
            strText.Append("<option value=\"1\">伪静态</option>");
            strText.Append("<option value=\"2\">栏目页动态</option>");
            strText.Append("<option value=\"3\">全静态运行</option>");
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"isOper\" value=\"保存设置\" onclick=\"commandOperate(this)\" />");
            strText.Append("<select onchange=\"window.location='?action=default&channelid='+this.value\">");
            strText.Append("<option value=\"0\">按模型查看</option>");
            strText.Append(ChannelHelper.Options(ChannelID));
            strText.Append("</select>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************
            * 开始输出网页数据
            * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/class/default.html");
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
        /// 获取到栏目的子栏目
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="ParentName"></param>
        /// <param name="cuteTxt"></param>
        /// <returns></returns>
        public string GetChildList(string ParentID, string ParentName, string cuteTxt)
        {
            StringBuilder strText = new StringBuilder();
            string strParams = " and ParentID=" + ParentID + " order by sortid desc,classid desc";
            string showTxt = "　" + cuteTxt;
            string columns = "classId,SortID,className,channelid,channelname,identify,isDisplay,isChild,isOper";
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Class, columns: columns, Params: strParams);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"classId\" value=\"" + Rs["classId"] + "\" /></td>");
                strText.Append("<td title=\"已展开全部子分类\">");
                strText.Append(showTxt);
                strText.Append("<img src=\"template/images/s.gif\" />");
                strText.Append(Rs["className"].ToString());
                switch (Rs["isOper"].ToString())
                {
                    case "0": strText.Append("<font color=\"#F00\">[动]</font>"); break;
                    case "1": strText.Append("<font color=\"#F00\">[伪]</font>"); break;
                    case "2": strText.Append("<font color=\"#F00\">[动,静]</font>"); break;
                    case "3": strText.Append("<font color=\"#F00\">[静]</font>"); break;
                }
                strText.Append("</td>");
                strText.Append("<td>" + ParentName + "</td>");
                strText.Append("<td><a href=\"?action=default&channelid=" + Rs["channelid"] + "\">" + Rs["channelname"] + "</a></td>");
                strText.Append("<td><input type=\"text\" isnumeric=\"true\" onblur=\"SaveEditor(this," + Rs["classId"].ToString() + ")\" size=\"5\" class=\"inputtext center\" value=\"" + Rs["SortID"] + "\" /></td>");
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&classId=" + Rs["classId"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&classId=" + Rs["classId"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=add&channelId=" + Rs["channelId"] + "&ParentID=" + Rs["classId"] + "\" title=\"添加子栏目\"><img src=\"images/ico/add.png\" /></a>");
                strText.Append("<a href=\"?action=edit&classId=" + Rs["classId"] + "\" title=\"修改栏目\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=del&classId=" + Rs["classId"] + "\" title=\"删除栏目\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
                if (Rs["isChild"].ToString() != "0") { strText.Append(this.GetChildList(Rs["classId"].ToString(), Rs["className"].ToString(), showTxt)); }
            }
            return strText.ToString();
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        protected void Add()
        {
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            string Template = string.Empty;
            string cTemplate = string.Empty;
            if (ChannelID != "0")
            {
                DataRow channelRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                    {"ChannelID",ChannelID}
                });
                if (channelRs != null)
                {
                    Template = "{@dir}/" + channelRs["channelname"] + "/栏目页.html";
                    cTemplate = "{@dir}/" + channelRs["channelname"] + "/内容页.html";
                }
            }
            /******************************************************************************************************
             * 输出网页内容
             * ****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/class/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "channel": strValue = new ChannelHelper().Options(ChannelID, value: "channelid", text: "channelname"); break;
                    case "options": if (ChannelID != "0") { strValue = ClassHelper.Options(ChannelID, "0", ParentID); } break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() { 
                        new RadioMode(){Name="isdisplay",Value="1",Text="开启"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "file": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "thumb",
                        tips = "请上传分类图片",
                        selector = true
                    }); break;
                    case "template": strValue = Template; break;
                    case "ctemplate": strValue = cTemplate; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        protected void Update()
        {
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindClass", new Dictionary<string, object>() {
                {"classId",classId}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"ChannelID",Rs["ChannelID"].ToString()}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            ConfigurationReader xReader = new ConfigurationReader(Rs["strXML"].ToString());
            /***********************************************************************************************
             * 输出网页内容
             * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/class/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "channel": strValue = new ChannelHelper().Options(cRs["channelId"].ToString(), value: "channelid", text: "channelname"); break;
                    case "options": strValue = ClassHelper.Options(Rs["channelId"].ToString(), "0", Rs["ParentID"].ToString()); break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() { 
                        new RadioMode(){Name="isdisplay",Value="1",Text="开启"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, Rs["isDisplay"].ToString()); break;
                    case "file": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "thumb",
                        tips = "请上传分类图片",
                        fileValue = Rs["thumb"].ToString(),
                        selector = true
                    }); break;
                    case "template": strValue = Rs["Template"].ToString(); break;
                    case "ctemplate": strValue = Rs["cTemplate"].ToString(); break;
                    case "isoper0": if (Rs["isOper"].ToString() == "0") { strValue = "selected"; } break;
                    case "isoper1": if (Rs["isOper"].ToString() == "1") { strValue = "selected"; } break;
                    case "isoper2": if (Rs["isOper"].ToString() == "2") { strValue = "selected"; } break;
                    case "isoper3": if (Rs["isOper"].ToString() == "3") { strValue = "selected"; } break;
                    default:
                        try { strValue = Rs[funName].ToString(); }
                        catch { strValue = xReader.GetParameter(funName).ToString(); }
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
        /// 添加模型
        /// </summary>
        protected void AddSave()
        {
            /***************************************************************
             * 验证模型名称
             * *************************************************************/
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            if (ChannelID == "0") { this.ErrorMessage("请选择栏目所属的模型！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"ChannelID",ChannelID}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的模型不存在！"); Response.End(); }
            /***************************************************************
             * 验证上级分类信息
             * *************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID != "0")
            {
                DataRow eRs = DbHelper.Connection.ExecuteFindRow("Stored_FindClass", new Dictionary<string, object>() {
                    {"ClassId",ParentID}
                });
                if (eRs == null) { this.ErrorMessage("你选择上级栏目不存在，请重新选择！"); Response.End(); }
                if (eRs["channelid"].ToString() != ChannelID) { this.ErrorMessage("分类模型设置错误，请返回重试！"); Response.End(); }
            }
            /***************************************************************
             * 验证分类名称
             * *************************************************************/
            string className = RequestHelper.GetRequest("className").toString();
            if (string.IsNullOrEmpty(className)) { this.ErrorMessage("请输入栏目名称！"); Response.End(); }
            if (className.Length > 50) { this.ErrorMessage("栏目名称请保持在50个汉字以内！"); Response.End(); }
            /***************************************************************
             * 检查栏目名称是否存在
             * *************************************************************/
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"className",className},
                {"ParentID",ParentID}
            });
            if (oRs != null) { this.ErrorMessage("同平道同目录下栏目名称已经存在！"); Response.End(); }
            /********************************************************************************
             * 检查分类标识是否合法
             * ******************************************************************************/
            string Identify = RequestHelper.GetRequest("Identify").toString();
            if (string.IsNullOrEmpty(Identify)) { this.ErrorMessage("请输入栏目的英文名称！"); Response.End(); }
            if (Identify.Length <= 2) { this.ErrorMessage("栏目的英文名称至少需要2个字符！"); Response.End(); }
            if (Identify.Length >= 50) { this.ErrorMessage("栏目的英文名称请限制在50个字符以内！"); Response.End(); }
            if (VerifyCenter.VerifyChina(Identify)) { this.ErrorMessage("栏目英文名中不允许包含中文字符！"); Response.End(); }
            oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"Identify",Identify}
            });
            if (oRs != null) { this.ErrorMessage("栏目英文名称已经存在，请另外选择一个吧！"); Response.End(); }
            /********************************************************************************
             * 设置模版已经其它数据信息
             * ******************************************************************************/
            string Template = RequestHelper.GetRequest("Template", false).toString();
            if (string.IsNullOrEmpty(Template)) { this.ErrorMessage("请选择栏目页模版地址"); Response.End(); }
            if (!string.IsNullOrEmpty(Template) && Template.Length > 50) { this.ErrorMessage("模板地址长度请限制在50个汉字以内!"); Response.End(); }
            string cTemplate = RequestHelper.GetRequest("cTemplate", false).toString();
            if (string.IsNullOrEmpty(cTemplate)) { this.ErrorMessage("请选择内容页模版地址"); Response.End(); }
            if (!string.IsNullOrEmpty(cTemplate) && cTemplate.Length > 50) { this.ErrorMessage("内容模板地址长度请限制在50个汉字以内!"); Response.End(); }
            string Thumb = RequestHelper.GetRequest("Thumb").toString();
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length > 150) { this.ErrorMessage("图片地址长度请限制在150个汉字以内！"); Response.End(); }
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            if (!string.IsNullOrEmpty(Keywords) && Keywords.Length > 100) { this.ErrorMessage("栏目关键词请保持在100个字符以内！"); Response.End(); }
            string strDesc = RequestHelper.GetRequest("strDesc").toString();
            if (!string.IsNullOrEmpty(strDesc) && strDesc.Length > 200) { this.ErrorMessage("栏目描述请保持在200个字符以内！"); Response.End(); }
            /*******************************************************************
             * 远程存图,抓取编辑器中的一张图片作为缩略图
             * *****************************************************************/
            string intro = RequestHelper.GetBodyContent("intro").ToString();
            string SaveThumbPic = RequestHelper.GetRequest("SaveThumbPic").toInt();
            if (string.IsNullOrEmpty(Thumb) && SaveThumbPic == "1" && intro != "") { Thumb = FunctionCenter.GetThumbPic(intro, 0); }
            string strXML = RequestHelper.GetPrametersXML(notTarget: "intro|strdesc");
            /****************************************************************
            * 获取其他不需要验证的数据信息
            * **************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string isOper = RequestHelper.GetRequest("isOper").toInt();
            /****************************************************************
             * 开始插入数据
             * **************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["classId"] = "0";
            oDictionary["ParentID"] = ParentID;
            oDictionary["OldParentID"] = "0";
            oDictionary["ChannelID"] = cRs["ChannelID"].ToString();
            oDictionary["ChannelName"] = cRs["ChannelName"].ToString();
            oDictionary["className"] = className;
            oDictionary["Identify"] = Identify;
            oDictionary["isOper"] = isOper;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["SortID"] = SortID;
            oDictionary["cTemplate"] = cTemplate;
            oDictionary["Template"] = Template;
            oDictionary["Thumb"] = Thumb;
            oDictionary["Keywords"] = Keywords;
            oDictionary["strDesc"] = strDesc;
            oDictionary["intro"] = intro;
            oDictionary["strXML"] = strXML;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveClass", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /********************************************************************
             * 更新页面缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Class"); }
            catch { }
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
            this.ConfirmMessage("栏目分类保存成功，点击确定将继续停留在当前页面！");
            Response.End();
        }

        protected void SaveUpdate()
        {
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindClass", new Dictionary<string, object>() {
                {"classId",classId}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /***************************************************************
             * 验证模型名称
             * *************************************************************/
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            if (ChannelID == "0") { this.ErrorMessage("请选择栏目所属的模型！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"ChannelID",ChannelID}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的模型不存在！"); Response.End(); }
            /***************************************************************
             * 验证上级分类信息
             * *************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID != "0")
            {
                DataRow eRs = DbHelper.Connection.ExecuteFindRow("Stored_FindClass", new Dictionary<string, object>() {
                    {"ClassId",ParentID}
                });
                if (eRs == null) { this.ErrorMessage("你选择上级栏目不存在，请重新选择！"); Response.End(); }
                if (eRs["channelid"].ToString() != ChannelID) { this.ErrorMessage("分类模型设置错误，请返回重试！"); Response.End(); }
            }
            if (ParentID != "0" && classId == ParentID) { this.ErrorMessage("上级分类设置错误！"); Response.End(); }
            /***************************************************************
             * 验证分类名称
             * *************************************************************/
            string className = RequestHelper.GetRequest("className").toString();
            if (string.IsNullOrEmpty(className)) { this.ErrorMessage("请输入栏目名称！"); Response.End(); }
            if (className.Length > 50) { this.ErrorMessage("栏目名称请保持在50个汉字以内！"); Response.End(); }
            /***************************************************************
             * 检查栏目名称是否存在
             * *************************************************************/
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"className",className},
                {"ParentID",ParentID}
            });
            if (oRs != null && oRs["classId"].ToString() != Rs["classId"].ToString()) { this.ErrorMessage("同平道同目录下栏目名称已经存在！"); Response.End(); }
            /********************************************************************************
             * 设置模版已经其它数据信息
             * ******************************************************************************/
            string Template = RequestHelper.GetRequest("Template", false).toString();
            if (string.IsNullOrEmpty(Template)) { this.ErrorMessage("请选择栏目页模版地址"); Response.End(); }
            if (!string.IsNullOrEmpty(Template) && Template.Length > 50) { this.ErrorMessage("模板地址长度请限制在50个汉字以内!"); Response.End(); }
            string cTemplate = RequestHelper.GetRequest("cTemplate", false).toString();
            if (string.IsNullOrEmpty(cTemplate)) { this.ErrorMessage("请选择内容页模版地址"); Response.End(); }
            if (!string.IsNullOrEmpty(cTemplate) && cTemplate.Length > 50) { this.ErrorMessage("内容模板地址长度请限制在50个汉字以内!"); Response.End(); }
            string Thumb = RequestHelper.GetRequest("Thumb").toString();
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length > 150) { this.ErrorMessage("图片地址长度请限制在150个汉字以内！"); Response.End(); }
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            if (!string.IsNullOrEmpty(Keywords) && Keywords.Length > 100) { this.ErrorMessage("栏目关键词请保持在100个字符以内！"); Response.End(); }
            string strDesc = RequestHelper.GetRequest("strDesc").toString();
            if (!string.IsNullOrEmpty(strDesc) && strDesc.Length > 200) { this.ErrorMessage("栏目描述请保持在200个字符以内！"); Response.End(); }
            /*******************************************************************
             * 远程存图,抓取编辑器中的一张图片作为缩略图
             * *****************************************************************/
            string intro = RequestHelper.GetBodyContent("intro").ToString();
            string SaveThumbPic = RequestHelper.GetRequest("SaveThumbPic").toInt();
            if (string.IsNullOrEmpty(Thumb) && SaveThumbPic == "1" && intro != "") { Thumb = FunctionCenter.GetThumbPic(intro, 0); }
            string strXML = RequestHelper.GetPrametersXML(notTarget: "intro|strdesc");
            /****************************************************************
            * 获取其他不需要验证的数据信息
            * **************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string isOper = RequestHelper.GetRequest("isOper").toInt();
            /****************************************************************
             * 开始插入数据
             * **************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["classId"] = Rs["ClassID"].ToString();
            oDictionary["ParentID"] = ParentID;
            oDictionary["OldParentID"] = Rs["ParentID"].ToString();
            oDictionary["ChannelID"] = cRs["ChannelID"].ToString();
            oDictionary["ChannelName"] = cRs["ChannelName"].ToString();
            oDictionary["className"] = className;
            oDictionary["Identify"] = Rs["Identify"].ToString();
            oDictionary["isOper"] = isOper;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["SortID"] = SortID;
            oDictionary["cTemplate"] = cTemplate;
            oDictionary["Template"] = Template;
            oDictionary["Thumb"] = Thumb;
            oDictionary["Keywords"] = Keywords;
            oDictionary["strDesc"] = strDesc;
            oDictionary["intro"] = intro;
            oDictionary["strXML"] = strXML;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveClass", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /********************************************************************
             * 更新页面缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Class"); }
            catch { }
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
            this.ConfirmMessage("栏目分类保存成功，点击确定将继续停留在当前页面！");
            Response.End();
        }
        /// <summary>
        /// 删除管理员
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
            * 验证删除数据的合法性
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_Class", Params: " and classId in (" + strList + ") and isDisplay=0");
            if (Tab == null) { this.ErrorMessage("没有要删除的数据！"); Response.End(); }
            else if (Tab.Rows.Count<=0) { this.ErrorMessage("没有要删除的数据！"); Response.End(); }
            foreach (DataRow Rs in Tab.Rows)
            {
                DbHelper.Connection.ExecuteProc("[Stored_DeleteClass]", new Dictionary<string, object>()
                {
                    {"ClassID",Rs["ClassID"].ToString()},
                    {"ParentID",Rs["ParentID"].ToString()}
                });
            }
            /********************************************************************
             * 更新页面缓存
             * ******************************************************************/
            try { BufferHelper.Delete("Fooke_Class"); }
            catch { }
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
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
            string strList = RequestHelper.GetRequest("ClassID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_Class", Params: " and ClassID in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_Class", new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and ClassID in (" + strList + ")");
            /***********************************************************************************************
            * 更新页面缓存
            * *********************************************************************************************/
            try { BufferHelper.Delete("Fooke_Class"); }
            catch { }
            /***********************************************************************************************
            * 开始输出数据
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
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_Class", Params: " and ClassID in (" + strList + ")");
            if (Tab == null) { Response.Write("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { Response.Write("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证请求参数值信息
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("isOrder").toInt();
            /***********************************************************************************************
            * 开始保存数据
            * *********************************************************************************************/
            DbHelper.Connection.Update("Fooke_Class", new Dictionary<string, string>() {
                {"SortID",strValue}
            }, Params: " and ClassID in (" + strList + ")");
            /***********************************************************************************************
            * 更新页面缓存
            * *********************************************************************************************/
            try { BufferHelper.Delete("Fooke_Class"); }
            catch { }
            /***********************************************************************************************
            * 开始输出数据
            * *********************************************************************************************/
            Response.Write("success");
            Response.End();
        }
        /// <summary>
        /// 保存设置的运行模式
        /// </summary>
        protected void SaveOper()
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
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_Class", Params: " and ClassID in (" + strList + ")");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证请求参数值信息
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("isOper").toInt();
            /***********************************************************************************************
            * 开始保存数据
            * *********************************************************************************************/
            DbHelper.Connection.Update("Fooke_Class", new Dictionary<string, string>() {
                {"isOper",strValue}
            }, Params: " and ClassID in (" + strList + ")");
            /***********************************************************************************************
            * 更新网页页面缓存
            * *********************************************************************************************/
            try { BufferHelper.Delete("Fooke_Class"); }
            catch { }
            /***********************************************************************************************
            * 输出数据处理结果
            * *********************************************************************************************/
            this.History(); 
            Response.End();
        }
    }
}