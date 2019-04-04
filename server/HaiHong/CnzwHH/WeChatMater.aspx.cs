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
    public partial class WeChatMater : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "edit": this.VerificationRole("微信素材"); Update(); Response.End(); break;
                case "add": this.VerificationRole("微信素材"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("微信素材"); UpdateSave(); Response.End(); break;
                case "save": this.VerificationRole("微信素材"); AddSave(); Response.End(); break;
                case "default": this.VerificationRole("微信素材"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "display": this.VerificationRole("微信素材"); SaveDisplay(); Response.End(); break;
            }
        }
        /// <summary>
        /// 获取我的银行卡列表信息
        /// </summary>
        protected void strDefault()
        {
            /*******************************************************************************************************
             * 获取查询条件
             * *****************************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            /*******************************************************************************************************
             * 处理网页
             * *****************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            strBuilder.Append("<td class=\"Base\" colspan=\"5\">素材管理 >> 素材列表</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"search\">");
            strBuilder.Append("<td colspan=\"5\">");
            strBuilder.Append("<form action=\"?action=default\" method=\"get\">");
            strBuilder.Append("<select name=\"searchType\">");
            strBuilder.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="title",Text="搜标题"},
            }, SearchType));
            strBuilder.Append("</select>");
            strBuilder.Append(" <input type=\"text\" placeholder=\"请填写要搜寻的关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strBuilder.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strBuilder.Append("</form>");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<form id=\"iptforms\" action='?' onsubmit=\"return _doPost(this)\" method='post'>");
            strBuilder.Append("<tr class=\"navigation\">");
            strBuilder.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strBuilder.Append("<td>素材标题</td>");
            strBuilder.Append("<td width=\"120\">子素材个数</td>");
            strBuilder.Append("<td width=\"60\">状态</td>");
            strBuilder.Append("<td width=\"180\">选项</td>");
            strBuilder.Append("</tr>");
            /*******************************************************************************************************
             * 构建查询语句条件
             * *****************************************************************************************************/
            string Params = " and ParentID=" + ParentID + "";
            if (!string.IsNullOrEmpty(Keywords)) { Params += " and title='%" + Keywords + "%'"; }
            /*******************************************************************************************************
             * 构建分页查询语句
             * *****************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "MaterID,ParentID,isChild,Title,Thumb,isDisplay,SortID,isUrl";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "MaterID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " MaterID desc";
            PageCenterConfig.Tablename = "Fooke_WeChatMater";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_WeChatMater", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.AppendFormat("<tr class=\"hback\">");
                strBuilder.AppendFormat("<td><input type=\"checkbox\" name=\"MaterID\" value=\"{0}\" /></td>", Rs["MaterID"]);
                strBuilder.AppendFormat("<td>{0}{1}</td>", Rs["title"].ToString(), (Rs["isUrl"].ToString() == "1" ? "[转]" : ""));
                strBuilder.AppendFormat("<td>{0}</td>", Rs["isChild"]);
                strBuilder.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strBuilder.AppendFormat("<a href=\"?action=display&val=0&MaterID={0}\"><img src=\"template/images/ico/yes.gif\"/></a>", Rs["MaterID"]); }
                else { strBuilder.AppendFormat("<a href=\"?action=display&val=1&MaterID={0}\"><img src=\"template/images/ico/no.gif\"/></a>", Rs["MaterID"]); }
                strBuilder.AppendFormat("</td>");
                strBuilder.AppendFormat("<td>");
                if (ParentID == "0")
                {
                    strBuilder.AppendFormat("<a href=\"?action=add&ParentID={0}\"  title=\"添加子素材\"><img src=\"template/images/ico/add.png\" /></a>", Rs["MaterID"]);
                    strBuilder.AppendFormat("<a href=\"?action=default&ParentID={0}\" title=\"查看子素材\"><img src=\"template/images/ico/chart.png\" /></a>", Rs["MaterID"]);
                }
                strBuilder.AppendFormat("<a href=\"?action=edit&MaterID={0}\" title=\"编辑素材\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["MaterID"]);
                strBuilder.AppendFormat("<a href=\"?action=del&MaterID={0}\"  title=\"删除素材\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["MaterID"]);
                strBuilder.AppendFormat("</td>");
                strBuilder.AppendFormat("</tr>");
            }
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"5\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td colspan=\"5\">");
            strBuilder.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"审核(是)\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"审核(否)\" onclick=\"commandOperate(this)\" />");
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            strBuilder.Append("</form>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatmater/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strBuilder.ToString(); break;
                    case "parentid": strValue = ParentID; break;
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
            string strResponse = Master.Reader("template/wechatmater/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="立即显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="立即关闭"}
                    }, "1"); break;
                    case "file": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "Thumb",
                        notKong = true,
                    }, "0"); break;
                    case "parentid": strValue = RequestHelper.GetRequest("ParentID").toInt(); ; break;
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
            string MaterID = RequestHelper.GetRequest("MaterID").toInt();
            if (MaterID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatMater]", new Dictionary<string, object>() {
                {"MaterID",MaterID}
            });
            if (Rs == null) { this.ErrorMessage("获取数据失败,请刷新网页重试！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatmater/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="立即显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="立即关闭"}
                    }, Rs["isDisplay"].ToString()); break;
                    case "file": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileValue = Rs["thumb"].ToString(),
                        fileName = "Thumb",
                        notKong = true,
                    }, "0"); break;
                    default: try { strValue = Rs[funName].ToString(); }
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
        protected void AddSave()
        {
            /*************************************************************************************************
             * 获取并验证素材分类信息
             * ***********************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            /*************************************************************************************************
             * 获取并验证素材标题合法性
             * ***********************************************************************************************/
            string Title = RequestHelper.GetRequest("Title").toString();
            if (string.IsNullOrEmpty(Title)) { this.ErrorMessage("请填写素材标题！"); Response.End(); }
            else if (Title.Length <= 0) { this.ErrorMessage("请填写素材标题！"); Response.End(); }
            else if (Title.Length > 100) { this.ErrorMessage("素材标题长度请限制在100个汉字以内！"); Response.End(); }
            /*************************************************************************************************
             * 验证素材标题名称是否存在
             * ***********************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatMater]", new Dictionary<string, object>() {
                {"ParentID",ParentID},
                {"Title",Title}
            });
            if (cRs != null) { this.ErrorMessage("同目录下素材名称已经存在,请重新选择吧！"); Response.End(); }
            /*************************************************************************************************
             * 检查图片资源
             * ***********************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString("/file/file/default.png");
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("请上传一张缩略图！"); Response.End(); }
            else if (Thumb.Length <= 10) { this.ErrorMessage("缩略图URL地址长度请限制在150个字符以内！"); Response.End(); }
            else if (Thumb.Length >= 150) { this.ErrorMessage("缩略图URL地址长度请限制在150个字符以内！"); Response.End(); }
            /*************************************************************************************************
             * 描述备注信息
             * ***********************************************************************************************/
            string Remark = RequestHelper.GetRequest("Remark").toString();
            if (Remark.Length > 150) { this.ErrorMessage("素材描述内容长度请限制在150个汉字以内！"); Response.End(); }
            string ToUrl = RequestHelper.GetRequest("ToUrl").toString();
            if (!string.IsNullOrEmpty(ToUrl) && ToUrl.Length > 150) { this.ErrorMessage("素材跳转地址长度请限制在150个字符以内！"); Response.End(); }
            else if (!string.IsNullOrEmpty(ToUrl) && !ToUrl.ToLower().Contains("http")) { this.ErrorMessage("请填写完整的url跳转地址,带上http！"); Response.End(); }
            /*************************************************************************************************
             * 获取不需要验证的数据
             * ***********************************************************************************************/
            string strContent = RequestHelper.GetBodyContent("strContent");
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /*************************************************************************************************
            * 开始保存网页内容
            * ***********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["MaterID"] = "0";
            thisDictionary["ParentID"] = ParentID;
            thisDictionary["OldParentID"] = "0";
            thisDictionary["Title"] = Title;
            thisDictionary["Thumb"] = Thumb;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            thisDictionary["Remark"] = Remark;
            thisDictionary["strContent"] = strContent;
            if (!string.IsNullOrEmpty(ToUrl) && ToUrl.Contains("http")) { thisDictionary["isUrl"] = "1"; }
            else { thisDictionary["isUrl"] = "0"; }
            thisDictionary["toUrl"] = ToUrl;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveWeChatMater]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); }
            /*************************************************************************************************
            * 输出网页内容
            * ***********************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }

        /// <summary>
        /// 添加银行卡
        /// </summary>
        protected void UpdateSave()
        {
            string MaterID = RequestHelper.GetRequest("MaterID").toInt();
            if (MaterID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindWeChatMater", new Dictionary<string, object>() {
                {"MaterID",MaterID}
            });
            if (Rs == null) { this.ErrorMessage("拉取数据失败,你查找的信息不存在！"); Response.End(); }
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID == Rs["MaterID"].ToString()) { this.ErrorMessage("数据归档错误不能将自身设置为父级分类!"); Response.End(); }
            /*************************************************************************************************
             * 获取并验证素材标题合法性
             * ***********************************************************************************************/
            string Title = RequestHelper.GetRequest("Title").toString();
            if (string.IsNullOrEmpty(Title)) { this.ErrorMessage("请填写素材标题！"); Response.End(); }
            else if (Title.Length <= 0) { this.ErrorMessage("请填写素材标题！"); Response.End(); }
            else if (Title.Length > 100) { this.ErrorMessage("素材标题长度请限制在100个汉字以内！"); Response.End(); }
            /*************************************************************************************************
             * 验证素材标题名称是否存在
             * ***********************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatMater]", new Dictionary<string, object>() {
                {"ParentID",ParentID},    
                {"Title",Title}
            });
            if (cRs != null && cRs["MaterID"].ToString() != Rs["MaterID"].ToString()) { this.ErrorMessage("同目录下素材名称已经存在,请重新选择吧！"); Response.End(); }
            /*************************************************************************************************
             * 检查图片资源
             * ***********************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString("/file/file/default.png");
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("请上传一张缩略图！"); Response.End(); }
            else if (Thumb.Length <= 10) { this.ErrorMessage("缩略图URL地址长度请限制在150个字符以内！"); Response.End(); }
            else if (Thumb.Length >= 150) { this.ErrorMessage("缩略图URL地址长度请限制在150个字符以内！"); Response.End(); }
            /*************************************************************************************************
             * 描述备注信息
             * ***********************************************************************************************/
            string Remark = RequestHelper.GetRequest("Remark").toString();
            if (Remark.Length > 150) { this.ErrorMessage("素材描述内容长度请限制在150个汉字以内！"); Response.End(); }
            string ToUrl = RequestHelper.GetRequest("ToUrl").toString();
            if (!string.IsNullOrEmpty(ToUrl) && ToUrl.Length > 150) { this.ErrorMessage("素材跳转地址长度请限制在150个字符以内！"); Response.End(); }
            else if (!string.IsNullOrEmpty(ToUrl) && !ToUrl.ToLower().Contains("http")) { this.ErrorMessage("请填写完整的url跳转地址,带上http！"); Response.End(); }
            /*************************************************************************************************
             * 获取不需要验证的数据
             * ***********************************************************************************************/
            string strContent = RequestHelper.GetBodyContent("strContent");
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /*************************************************************************************************
            * 开始保存网页内容
            * ***********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["MaterID"] = Rs["MaterID"].ToString();
            thisDictionary["ParentID"] = ParentID;
            thisDictionary["OldParentID"] = Rs["ParentID"].ToString();
            thisDictionary["Title"] = Title;
            thisDictionary["Thumb"] = Thumb;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            thisDictionary["Remark"] = Remark;
            thisDictionary["strContent"] = strContent;
            if (!string.IsNullOrEmpty(ToUrl) && ToUrl.Contains("http")) { thisDictionary["isUrl"] = "1"; }
            else { thisDictionary["isUrl"] = "0"; }
            thisDictionary["toUrl"] = ToUrl;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveWeChatMater]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); }
            /*************************************************************************************************
            * 输出网页内容
            * ***********************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }
        /// <summary>
        /// 删除用户等级信息
        /// </summary>
        protected void Delete()
        {

            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("MaterID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_WeChatMater", Params: " and MaterID in (" + strList + ") and isDisplay=0");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证请求参数值信息
             * *********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                DbHelper.Connection.Delete("Fooke_WeChatMater",
                    Params: " and( MaterID =" + Rs["MaterID"] + " or ParentID = " + Rs["MaterID"] + ")");
            }
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 设置显示信息
        /// </summary>
        protected void SaveDisplay()
        {

            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("MaterID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_WeChatMater", Params: " and MaterID in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_WeChatMater", new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and MaterID in (" + strList + ")");
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}