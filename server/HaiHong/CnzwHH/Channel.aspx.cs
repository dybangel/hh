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
    public partial class Channel : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "del": this.VerificationRole("模型管理"); this.Delete(); Response.End(); break;
                case "editsave": this.VerificationRole("模型管理"); UpdateSave(); Response.End(); break;
                case "edit": this.VerificationRole("模型管理"); Update(); Response.End(); break;
                case "display": this.VerificationRole("模型管理"); Display(); Response.End(); break;
                case "add": this.VerificationRole("模型管理"); Add(); Response.End(); break;
                case "save": this.VerificationRole("模型管理"); AddSave(); Response.End(); break;
                default: this.VerificationRole("模型管理"); strDefault(); Response.End(); break;
            }
            Response.End();
        }
        /// <summary>
        /// 站点管理
        /// </summary>
        protected void strDefault()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"6\">模型管理 >> 模型列表</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strBuilder.Append("<td>模型名称</td>");
            strBuilder.Append("<td width=\"8%\">所属基类</td>");
            strBuilder.Append("<td width=\"8%\">模型数据表</td>");
            strBuilder.Append("<td width=\"6%\">状态</td>");
            strBuilder.Append("<td width=\"12%\">选项</td>");
            strBuilder.Append("</tr>");
            string Params = "";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "channelId,channelName,Basename,Tablename,SortID,UnitName,isDisplay";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "channelid";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID desc,channelid Desc";
            PageCenterConfig.Tablename = TableCenter.Channel;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Channel, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td><input type=\"checkbox\" name=\"channelId\" value=\"" + Rs["channelId"] + "\" /></td>");
                strBuilder.Append("<td>" + Rs["channelName"] + "</td>");
                strBuilder.Append("<td>" + Rs["Basename"] + "</td>");
                strBuilder.Append("<td>" + Rs["Tablename"] + "</td>");
                strBuilder.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strBuilder.Append("<a href=\"?action=display&val=0&channelId=" + Rs["channelId"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strBuilder.Append("<a href=\"?action=display&val=1&channelId=" + Rs["channelId"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strBuilder.Append("</td>");
                strBuilder.Append("<td>");
                strBuilder.Append("<a href=\"?action=edit&channelId=" + Rs["channelId"] + "\" title=\"编辑模型\"><img src=\"template/images/ico/edit.png\" /></a>");
                strBuilder.Append("<a href=\"?action=del&channelId=" + Rs["channelId"] + "\"  title=\"删除模型\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strBuilder.Append("</td>");
                strBuilder.Append("</tr>");
            }
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td colspan=\"6\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"锁定\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"解锁\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/channel/default.html");
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
        /// 添加站点
        /// </summary>
        protected void Add()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/channel/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": strValue = FunctionBase.OptionList(new List<OptionMode>() {
                        new OptionMode(){Text="文章系统",Value="文章系统"},
                        new OptionMode(){Text="空模型",Value="空模型"}
                    }, "文章系统"); break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() { 
                        new RadioMode(){Name="isdisplay",Value="1",Text="开启"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
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
            string channelId = RequestHelper.GetRequest("channelId").toInt();
            if (channelId == "0") { this.ErrorMessage("参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"channelid",channelId}
            });
            if (cRs == null) { this.ErrorMessage("拉取数据失败，你查找的信息不存在！"); Response.End(); }
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/channel/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": strValue = FunctionBase.OptionList(new List<OptionMode>() {
                        new OptionMode(){Text="文章系统",Value="文章系统"},
                        new OptionMode(){Text="空模型",Value="空模型"}
                    }, cRs["basename"].ToString()); break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() { 
                        new RadioMode(){Name="isdisplay",Value="1",Text="开启"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, cRs["isdisplay"].ToString()); break;
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
            string channelName = RequestHelper.GetRequest("channelname").toString();
            if (string.IsNullOrEmpty(channelName)) { this.ErrorMessage("请填写模型的名称！"); Response.End(); }
            if (channelName.Length > 20) { this.ErrorMessage("模型名称长度请限制在20个汉字以内！"); Response.End(); }
            string BaseName = RequestHelper.GetRequest("BaseName").toString();
            if (string.IsNullOrEmpty(BaseName)) { this.ErrorMessage("请选择模型基类型！"); Response.End(); }
            string UnitName = RequestHelper.GetRequest("UnitName").toString();
            if (string.IsNullOrEmpty(UnitName)) { this.ErrorMessage("请填写模型简称！"); Response.End(); }
            if (UnitName.Length > 10) { this.ErrorMessage("模型简称名称请限制在10个汉字以内！"); Response.End(); }
            string Tablename = RequestHelper.GetRequest("Tablename").toString();
            if (string.IsNullOrEmpty(Tablename)) { this.ErrorMessage("请填写数据表名称！不允许包含特殊字符！"); Response.End(); }
            if (Tablename.Length > 20) { this.ErrorMessage("数据表名称长度请限制在20个字符以内！不允许包含特殊字符！"); Response.End(); }
            if (VerifyCenter.VerifySpecific(Tablename)) { this.ErrorMessage("模型数据表名称不允许包含特殊字符！"); Response.End(); }
            if (VerifyCenter.VerifyChina(Tablename)) { this.ErrorMessage("模型数据表名称不允许为中文字符！"); Response.End(); }
            string intro = RequestHelper.GetRequest("intro").toString();
            if (intro.Length > 200) { this.ErrorMessage("描述内容请限制在200个汉字以内！"); Response.End(); }
            string strKey = new Fooke.Function.String("系统模型-|-|-" + Tablename).ToMD5().ToLower();
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindChannel]", new Dictionary<string, object>() {
                {"strKey",strKey}
            });
            if (cRs != null) { this.ErrorMessage("模型标识已经存在了，请另外选择吧！"); Response.End(); }
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"Tablename",Tablename}
            });
            if (oRs != null) { this.ErrorMessage("模型数据表已经存在了，请另外选择一个名称！"); Response.End(); }
            oRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"Channelname",channelName}
            });
            if (oRs != null) { this.ErrorMessage("模型名称已经存在，请另外选择一个名称！"); Response.End(); }
            /********************************************************************************************
             * 获取不需要验证的信息
             * ******************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string strXML = RequestHelper.GetPrametersXML(false);
            /********************************************************************************************
             * 重新定义模型数据表的名称
             * ******************************************************************************************/
            Tablename = "Fooke_Custom_" + Tablename;
            /********************************************************************************************
             * 开始插入数据
             * ******************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["ChannelID"] = "0";
            thisDictionary["ChannelName"] = channelName;
            thisDictionary["UnitName"] = UnitName;
            thisDictionary["BaseName"] = BaseName;
            thisDictionary["Tablename"] = Tablename;
            thisDictionary["strKey"] = strKey;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            thisDictionary["intro"] = intro;
            thisDictionary["strXML"] = strXML;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveChannel]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /***************************************************************************
            * 更新模型缓存数据
            * *************************************************************************/
            try { new ChannelHelper().ApplicationClaer(); }
            catch { }
            /********************************************************************************************
             * 开始输出结果
             * *******************************************************************************************/
            this.ConfirmMessage("模型添加成功,点击确定将继续停留在当前界面.");
            Response.End();
        }

        /// <summary>
        /// 修改分类
        /// </summary>
        protected void UpdateSave()
        {
            string ChannelId = RequestHelper.GetRequest("ChannelId").toInt();
            if (ChannelId == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow channelRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"channelid",ChannelId}
            });
            if (channelRs == null) { this.ErrorMessage("拉取数据失败，你查找的数据不存在！"); Response.End(); }

            string channelName = RequestHelper.GetRequest("channelname").toString();
            if (string.IsNullOrEmpty(channelName)) { this.ErrorMessage("请填写模型的名称！"); Response.End(); }
            if (channelName.Length > 20) { this.ErrorMessage("模型名称长度请限制在20个汉字以内！"); Response.End(); }
            string UnitName = RequestHelper.GetRequest("UnitName").toString();
            if (string.IsNullOrEmpty(UnitName)) { this.ErrorMessage("请填写模型简称！"); Response.End(); }
            if (UnitName.Length > 10) { this.ErrorMessage("模型简称名称请限制在10个汉字以内！"); Response.End(); }
            string intro = RequestHelper.GetRequest("intro").toString();
            if (intro.Length > 200) { this.ErrorMessage("描述内容请限制在200个汉字以内！"); Response.End(); }
            /********************************************************************************************
            * 检查模型名称是否存在
            * ******************************************************************************************/
            DataRow oRs = DbHelper.Connection.FindRow(TableCenter.Channel, Params: " and channelId<>" + ChannelId + " and channelName='" + channelName + "'");
            if (oRs != null) { this.ErrorMessage("模型名称已经存在，请另外选择一个名称！"); Response.End(); }
            /********************************************************************************************
             * 获取不需要验证的信息
             * ******************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string strXML = RequestHelper.GetPrametersXML(false);
            /********************************************************************************************
             * 开始插入数据
             * ******************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["ChannelID"] = channelRs["ChannelID"].ToString(); ;
            thisDictionary["ChannelName"] = channelName;
            thisDictionary["UnitName"] = UnitName;
            thisDictionary["BaseName"] = channelRs["BaseName"].ToString();
            thisDictionary["Tablename"] = channelRs["Tablename"].ToString();
            thisDictionary["strKey"] = channelRs["strKey"].ToString();
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            thisDictionary["intro"] = intro;
            thisDictionary["strXML"] = strXML;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveChannel]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /***************************************************************************
             * 更新模型缓存数据
             * *************************************************************************/
            try { new ChannelHelper().ApplicationClaer(); }
            catch { }
            /****************************************************************************
             * 开始执行页面输出
             * ***************************************************************************/
            this.ConfirmMessage("模型编辑成功,点击确定将继续停留在当前界面.");
            Response.End();
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        protected void Display()
        {
            string channelId = RequestHelper.GetRequest("channelId").toString();
            if (string.IsNullOrEmpty(channelId)) { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            string val = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["isDisplay"] = val;
            DbHelper.Connection.Update(TableCenter.Channel, dictionary, Params: " and channelId in(" + channelId + ")");
            /***************************************************************************
             * 更新模型缓存数据
             * *************************************************************************/
            try { new ChannelHelper().ApplicationClaer(); }
            catch { }
            /****************************************************************************
             * 开始执行页面输出
             * ***************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 删除文章分类
        /// </summary>
        protected void Delete()
        {
            string channelId = RequestHelper.GetRequest("channelId").toInt();
            if (channelId == "0") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"ChannelID",channelId}
            });
            if (!cRs["Tablename"].ToString().ToLower().Contains("fooke_custom_")) { this.ErrorMessage("系统数据表不允许删除！"); Response.End(); }
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["ChannelID"] = cRs["ChannelID"].ToString();
            dictionary["Tablename"] = cRs["Tablename"].ToString();
            DbHelper.Connection.ExecuteProc("[Stored_DeleteChannel]", dictionary);
            /***************************************************************************
            * 更新模型缓存数据
            * *************************************************************************/
            try { new ChannelHelper().ApplicationClaer(); }
            catch { }
            /****************************************************************************
             * 开始执行页面输出
             * ***************************************************************************/
            this.History();
            Response.End();
        }
    }
}