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
    public partial class WeChatKeywords : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "edit": this.VerificationRole("微信素材"); Update(); Response.End(); break;
                case "add": this.VerificationRole("微信素材"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("微信素材"); SaveUpdate(); Response.End(); break;
                case "save": this.VerificationRole("微信素材"); AddSave(); Response.End(); break;
                case "default": this.VerificationRole("微信素材"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("微信素材"); Delete(); Response.End(); break;
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
            strBuilder.Append("<td class=\"Base\" colspan=\"7\">匹配回复 >> 回复列表</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"search\">");
            strBuilder.Append("<td colspan=\"7\">");
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
            strBuilder.Append("<td>关键字</td>");
            strBuilder.Append("<td width=\"100\">匹配类型</td>");
            strBuilder.Append("<td width=\"100\">回复类型</td>");
            strBuilder.Append("<td width=\"100\">触发次数</td>");
            strBuilder.Append("<td width=\"100\">状态</td>");
            strBuilder.Append("<td width=\"180\">选项</td>");
            strBuilder.Append("</tr>");
            /*******************************************************************************************************
             * 构建查询语句条件
             * *****************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(Keywords)) { Params += " and title='%" + Keywords + "%'"; }
            /*******************************************************************************************************
             * 构建分页查询语句
             * *****************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "KeywordsID,Title,MaterID,isMatch,Modal,Hits,isDisplay";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "KeywordsID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " KeywordsID desc";
            PageCenterConfig.Tablename = "Fooke_WeChatKeywords";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_WeChatKeywords", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.AppendFormat("<tr class=\"hback\">");
                strBuilder.AppendFormat("<td><input type=\"checkbox\" name=\"KeywordsID\" value=\"{0}\" /></td>", Rs["KeywordsID"]);
                strBuilder.AppendFormat("<td>{0}</td>", Rs["title"].ToString());
                strBuilder.AppendFormat("<td>{0}</td>", (Rs["isMatch"].ToString() == "1" ? "完全匹配" : "模糊匹配"));
                strBuilder.AppendFormat("<td>{0}</td>", (Rs["isMatch"].ToString() == "1" ? "文本回复" : "图文回复"));
                strBuilder.AppendFormat("<td>{0}</td>", Rs["Hits"].ToString());
                strBuilder.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strBuilder.AppendFormat("<a href=\"?action=display&val=0&KeywordsID={0}\"><img src=\"template/images/ico/yes.gif\"/></a>", Rs["KeywordsID"]); }
                else { strBuilder.AppendFormat("<a href=\"?action=display&val=1&KeywordsID={0}\"><img src=\"template/images/ico/no.gif\"/></a>", Rs["KeywordsID"]); }
                strBuilder.AppendFormat("</td>");
                strBuilder.AppendFormat("<td>");
                strBuilder.AppendFormat("<a href=\"?action=edit&KeywordsID={0}\" title=\"编辑素材\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["KeywordsID"]);
                strBuilder.AppendFormat("<a href=\"?action=del&KeywordsID={0}\"  title=\"删除素材\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["KeywordsID"]);
                strBuilder.AppendFormat("</td>");
                strBuilder.AppendFormat("</tr>");
            }
            strBuilder.Append("<tr class=\"pager\">");
            strBuilder.Append("<td colspan=\"7\">");
            strBuilder.Append(PageCenter.Often(Record, 10));
            strBuilder.Append("</td>");
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"operback\">");
            strBuilder.Append("<td colspan=\"7\">");
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
            string strResponse = Master.Reader("template/wechatKeywords/default.html");
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
            string strResponse = Master.Reader("template/wechatKeywords/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Value="1",Text="开启回复"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="关闭回复"}
                    }, "1"); break;
                    case "isMatch": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isMatch",Value="1",Text="完全匹配"},
                        new RadioMode(){Name="isMatch",Value="0",Text="模糊匹配"}
                    }, "1"); break;
                    case "Modal": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="Modal",Value="1",Text="文本回复"},
                        new RadioMode(){Name="Modal",Value="0",Text="图文回复"}
                    }, "1"); break;
                    case "MaterList": strValue = new MaterHelper().Options("0", ""); break;
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
            string KeywordsID = RequestHelper.GetRequest("KeywordsID").toInt();
            if (KeywordsID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatKeywords]", new Dictionary<string, object>() {
                {"KeywordsID",KeywordsID}
            });
            if (Rs == null) { this.ErrorMessage("获取数据失败,请刷新网页重试！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatKeywords/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Value="1",Text="开启回复"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="关闭回复"}
                    }, Rs["isDisplay"].ToString()); break;
                    case "isMatch": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isMatch",Value="1",Text="完全匹配"},
                        new RadioMode(){Name="isMatch",Value="0",Text="模糊匹配"}
                    }, Rs["isMatch"].ToString()); break;
                    case "Modal": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="Modal",Value="1",Text="文本回复"},
                        new RadioMode(){Name="Modal",Value="0",Text="图文回复"}
                    }, Rs["Modal"].ToString()); break;
                    case "mode": strValue = Rs["Modal"].ToString(); break;
                    case "MaterList": strValue = new MaterHelper().Options("0", Rs["MaterID"].ToString()); break;
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
             * 获取并验证关键词的合法性
             * ***********************************************************************************************/
            string Title = RequestHelper.GetRequest("Title").toString();
            if (string.IsNullOrEmpty(Title)) { this.ErrorMessage("请填写匹配关键字！"); Response.End(); }
            else if (Title.Length <= 0) { this.ErrorMessage("请填写匹配关键字！"); Response.End(); }
            else if (Title.Length >= 40) { this.ErrorMessage("匹配关键字长度请限制在40个汉字以内！"); Response.End(); }
            /*************************************************************************************************
             * 验证关键词是否存在
             * ***********************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatKeywords]", new Dictionary<string, object>() {
                {"Title",Title},
                {"isMatch","1"}
            });
            if (cRs != null) { this.ErrorMessage("匹配关键字已经存在了,请检查！"); Response.End(); }
            /*************************************************************************************************
             * 检查图片资源
             * ***********************************************************************************************/
            string isMatch = RequestHelper.GetRequest("isMatch").toInt();
            string Modal = RequestHelper.GetRequest("Modal").toInt();
            string MaterID = RequestHelper.GetRequest("MaterID").toInt();
            if (Modal == "0" && MaterID == "0") { this.ErrorMessage("请选择需要回复的图文素材！"); Response.End(); }
            /*************************************************************************************************
             * 文本回复
             * ***********************************************************************************************/
            string strDesc = RequestHelper.GetRequest("strDesc").toString();
            if (Modal == "1" && string.IsNullOrEmpty(strDesc)) { this.ErrorMessage("请填写文本回复内容!"); Response.End(); }
            if (strDesc.Length > 400) { this.ErrorMessage("文本回复内容长度请限制在400个汉字以内！"); Response.End(); }
            /*************************************************************************************************
             * 获取不需要验证的数据
             * ***********************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /*************************************************************************************************
            * 开始保存网页内容
            * ***********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["KeywordsID"] = "0";
            thisDictionary["Title"] = Title;
            thisDictionary["MaterID"] = MaterID;
            thisDictionary["strDesc"] = strDesc;
            thisDictionary["isMatch"] = isMatch;
            thisDictionary["Modal"] = Modal;
            thisDictionary["isDisplay"] = isDisplay;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveWeChatKeywords]", thisDictionary);
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
        protected void SaveUpdate()
        {
            /*************************************************************************************************
             * 获取并验证请求参数的合法性
             * ***********************************************************************************************/
            string KeywordsID = RequestHelper.GetRequest("KeywordsID").toInt();
            if (KeywordsID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindWeChatKeywords", new Dictionary<string, object>() {
                {"KeywordsID",KeywordsID}
            });
            if (Rs == null) { this.ErrorMessage("拉取数据失败,你查找的信息不存在！"); Response.End(); }
            /*************************************************************************************************
             * 获取并验证关键词的合法性
             * ***********************************************************************************************/
            string Title = RequestHelper.GetRequest("Title").toString();
            if (string.IsNullOrEmpty(Title)) { this.ErrorMessage("请填写匹配关键字！"); Response.End(); }
            else if (Title.Length <= 0) { this.ErrorMessage("请填写匹配关键字！"); Response.End(); }
            else if (Title.Length >= 40) { this.ErrorMessage("匹配关键字长度请限制在40个汉字以内！"); Response.End(); }
            /*************************************************************************************************
             * 验证关键词是否存在
             * ***********************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatKeywords]", new Dictionary<string, object>() {
                {"Title",Title},
                {"isMatch","1"}
            });
            if (cRs != null && cRs["KeywordsID"].ToString() != Rs["KeywordsID"].ToString()) { this.ErrorMessage("匹配关键字已经存在了,请检查！"); Response.End(); }
            /*************************************************************************************************
             * 检查图片资源
             * ***********************************************************************************************/
            string isMatch = RequestHelper.GetRequest("isMatch").toInt();
            string Modal = RequestHelper.GetRequest("Modal").toInt();
            string MaterID = RequestHelper.GetRequest("MaterID").toInt();
            if (Modal == "0" && MaterID == "0") { this.ErrorMessage("请选择需要回复的图文素材！"); Response.End(); }
            /*************************************************************************************************
             * 文本回复
             * ***********************************************************************************************/
            string strDesc = RequestHelper.GetRequest("strDesc").toString();
            if (Modal == "1" && string.IsNullOrEmpty(strDesc)) { this.ErrorMessage("请填写文本回复内容!"); Response.End(); }
            if (strDesc.Length > 400) { this.ErrorMessage("文本回复内容长度请限制在400个汉字以内！"); Response.End(); }
            /*************************************************************************************************
             * 获取不需要验证的数据
             * ***********************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /*************************************************************************************************
            * 开始保存网页内容
            * ***********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["KeywordsID"] = Rs["KeywordsID"].ToString();
            thisDictionary["Title"] = Title;
            thisDictionary["MaterID"] = MaterID;
            thisDictionary["strDesc"] = strDesc;
            thisDictionary["isMatch"] = isMatch;
            thisDictionary["Modal"] = Modal;
            thisDictionary["isDisplay"] = isDisplay;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveWeChatKeywords]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); }
            /*************************************************************************************************
            * 输出网页内容
            * ***********************************************************************************************/
            this.ConfirmMessage(Modal+"数据保存成功,点击确定将继续停留在当前界面!");
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
            string strList = RequestHelper.GetRequest("KeywordsID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_WeChatKeywords", Params: " and KeywordsID in (" + strList + ") and isDisplay=0");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始保存数据
            * *********************************************************************************************/
            DbHelper.Connection.Delete("Fooke_WeChatKeywords", Params: "  and KeywordsID in (" + strList + ") and isDisplay=0");
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
            string strList = RequestHelper.GetRequest("KeywordsID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_WeChatKeywords", Params: " and KeywordsID in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_WeChatKeywords", new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and KeywordsID in (" + strList + ")");
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}