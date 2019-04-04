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
    public partial class Buffer : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "buffer": this.VerificationRole("系统缓存"); SaveBuffer(); Response.End(); break;
                case "editor": this.VerificationRole("系统缓存"); SaveEditor(); Response.End(); break;
                case "display": this.VerificationRole("系统缓存"); SaveDisplay(); Response.End(); break;
                case "edit": this.VerificationRole("系统缓存"); Update(); Response.End(); break;
                case "add": this.VerificationRole("系统缓存"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("系统缓存"); UpdateSave(); Response.End(); break;
                case "save": this.VerificationRole("系统缓存"); AddSave(); Response.End(); break;
                case "default": this.VerificationRole("系统缓存"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("系统缓存"); Delete(); Response.End(); break;
            }
        }
        /// <summary>
        /// 获取我的银行卡列表信息
        /// </summary>
        protected void strDefault()
        {
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            StringBuilder strText = new StringBuilder();
            
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">系统缓存 >> 网络节点</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="buffername",Text="节点名称"},
                new OptionMode(){Value="bufferurl",Text="节点地址"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td>节点名称</td>");
            strText.Append("<td width=\"300\">网络地址</td>");
            strText.Append("<td width=\"80\">显示排序</td>");
            strText.Append("<td width=\"80\">状态</td>");
            strText.Append("<td width=\"80\">操作选项</td>");
            strText.Append("</tr>");
            /****************************************************************************************
             * 生成查询条件
             * **************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType)){
                switch (SearchType.ToLower()) {
                    case "buffername": strParameter += " and buffername like '%" + Keywords + "%'"; break;
                    case "bufferurl": strParameter += " and bufferurl like '%" + Keywords + "%'"; break;
                }
            }
            /****************************************************************************************
             * 构建查询Sql语句
             * **************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "BufferID,Buffername,BufferUrl,isDisplay,SortID";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "BufferID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID desc,BufferID asc";
            PageCenterConfig.Tablename = "Fooke_Buffer";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Buffer", strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"BufferID\" value=\"{0}\" /></td>", Rs["BufferID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["BufferName"]);
                strText.AppendFormat("<td>{0}</td>", Rs["BufferUrl"]);
                strText.AppendFormat("<td><input type=\"text\" style=\"width:40px\" class=\"inputtext\" url=\"?action=editor&bufferid={1}\" operate=\"editsort\" value=\"{0}\" /></td>", Rs["SortID"], Rs["BufferID"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.AppendFormat("<a href=\"?action=display&val=0&BufferID={0}\"><img src=\"template/images/ico/yes.gif\"/></a>", Rs["BufferID"]); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&BufferID={0}\"><img src=\"template/images/ico/no.gif\"/></a>", Rs["BufferID"]); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=edit&BufferID={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["BufferID"]);
                strText.AppendFormat("<a href=\"?action=del&BufferID={0}\"  title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["BufferID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }

            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"显示(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"显示(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"buffer\" value=\"更新缓存\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /****************************************************************************************
             * 输出网页内容
             * **************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/buffer/default.html");
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
            string strResponse = Master.Reader("template/buffer/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName){
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="关闭"}
                    }, "1"); break;
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
            string BufferID = RequestHelper.GetRequest("BufferID").toInt();
            if (BufferID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBuffer]", new Dictionary<string, object>() {
                {"BufferID",BufferID}
            });
            if (Rs == null) { this.ErrorMessage("拉取分组信息失败,请刷新网页重试！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/buffer/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="关闭"}
                    }, Rs["isDisplay"].ToString()); break;
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
            /*******************************************************************************************
             * 获取节点数据信息
             * *******************************************************************************************/
            string BufferName = RequestHelper.GetRequest("BufferName").toString();
            if (string.IsNullOrEmpty(BufferName)) { this.ErrorMessage("请填写节点名称！"); Response.End(); }
            if (BufferName.Length > 30) { this.ErrorMessage("BufferName名称长度请限制在50个字符以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBuffer]", new Dictionary<string, object>() {
                {"BufferName",BufferName}
            });
            if (cRs != null) { this.ErrorMessage("相同的节点名称已经存在,请重新选择！"); Response.End(); }
            /*******************************************************************************************
             * 获取节点地址信息
             * *******************************************************************************************/
            string BufferUrl = RequestHelper.GetRequest("BufferUrl").toString();
            if (string.IsNullOrEmpty(BufferUrl)) { this.ErrorMessage("请填写节点地址!"); Response.End(); }
            if (BufferUrl.Length > 50) { this.ErrorMessage("节点地址信息长度请限制在20个字符以内！"); Response.End(); }
            if (!BufferUrl.Contains("http")) { this.ErrorMessage("节点地址网络地址请填写完整!"); Response.End(); }
            /**********************************************************************************************
             * 获取其它不需要审核的数据信息
             * *********************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /**********************************************************************************************
            * 开始保存数据
            * *********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["BufferID"] = "0";
            thisDictionary["BufferName"] = BufferName;
            thisDictionary["BufferUrl"] = BufferUrl;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveBuffer]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); Response.End(); }
            /*******************************************************************************************
             * 输出网页信息
             * *******************************************************************************************/
            this.ConfirmMessage("保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }

        /// <summary>
        /// 添加银行卡
        /// </summary>
        protected void UpdateSave()
        {
            string BufferID = RequestHelper.GetRequest("BufferID").toInt();
            if (BufferID == "0") { this.ErrorMessage("请求参数错误,请刷新重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBuffer]", new Dictionary<string, object>() {
                {"BufferID",BufferID}
            });
            if (Rs == null) { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            /*******************************************************************************************
             * 获取节点数据信息
             * *******************************************************************************************/
            string BufferName = RequestHelper.GetRequest("BufferName").toString();
            if (string.IsNullOrEmpty(BufferName)) { this.ErrorMessage("请填写节点名称！"); Response.End(); }
            if (BufferName.Length > 30) { this.ErrorMessage("BufferName名称长度请限制在50个字符以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBuffer]", new Dictionary<string, object>() {
                {"BufferName",BufferName}
            });
            if (cRs != null && cRs["BufferID"].ToString()!=Rs["BufferID"].ToString()) { this.ErrorMessage("相同的节点名称已经存在,请重新选择！"); Response.End(); }
            /*******************************************************************************************
             * 获取节点地址信息
             * *******************************************************************************************/
            string BufferUrl = RequestHelper.GetRequest("BufferUrl").toString();
            if (string.IsNullOrEmpty(BufferUrl)) { this.ErrorMessage("请填写节点地址!"); Response.End(); }
            try
            {
                if (BufferUrl.Length > 50) { this.ErrorMessage("节点地址信息长度请限制在20个字符以内！"); Response.End(); }
                if (!BufferUrl.Contains("http")) { BufferUrl = "http://" + BufferUrl; }
                if (BufferUrl.EndsWith("/")) { BufferUrl = BufferUrl.Substring(0, BufferUrl.Length - 1); }
            }
            catch { }
            /**********************************************************************************************
             * 获取其它不需要审核的数据信息
             * *********************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /**********************************************************************************************
            * 开始保存数据
            * *********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["BufferID"] = Rs["BufferID"].ToString();
            thisDictionary["BufferName"] = BufferName;
            thisDictionary["BufferUrl"] = BufferUrl;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveBuffer]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); Response.End(); }
            /*******************************************************************************************
             * 输出网页信息
             * *******************************************************************************************/
            this.ConfirmMessage("保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }
        /// <summary>
        /// 删除用户等级信息
        /// </summary>
        protected void Delete()
        {
            string BufferID = RequestHelper.GetRequest("BufferID").toString();
            if (string.IsNullOrEmpty(BufferID)) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_Buffer", Params: " and BufferID in (" + BufferID + ")");
            /****************************************************
             * 输出网页处理结果
             * ****************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 保存设置数据信息
        /// </summary>
        protected void SaveDisplay()
        {
            string BufferID = RequestHelper.GetRequest("BufferID").toString();
            if (string.IsNullOrEmpty(BufferID)) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            string val = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> thisDictionary = new Dictionary<string, string>();
            thisDictionary["isDisplay"] = val;
            DbHelper.Connection.Update("Fooke_Buffer", thisDictionary, Params: " and BufferID in (" + BufferID + ")");
            /****************************************************
             * 输出网页处理结果
             * ****************************************************/
            this.History();
            Response.End();
        }

        /// <summary>
        /// 保存用户排序信息
        /// </summary>
        protected void SaveEditor()
        {
            string BufferID = RequestHelper.GetRequest("BufferID").toInt();
            if (BufferID == "0") { Response.Write("请求参数错误,请至少选择一条数据！"); Response.End(); }
            string value = RequestHelper.GetRequest("value").toInt();
            Dictionary<string, string> thisDictionary = new Dictionary<string, string>();
            thisDictionary["SortID"] = value;
            DbHelper.Connection.Update("Fooke_Buffer", thisDictionary, Params: " and BufferID=" + BufferID + "");
            /*******************************************************************************************
             * 输出数据处理结果
             * ******************************************************************************************/
            Response.Write("success");
            Response.End();
        }
        /// <summary>
        /// 更新系统缓存
        /// </summary>
        protected void SaveBuffer()
        {
            string BufferID = RequestHelper.GetRequest("BufferID").toString();
            DataTable Tab = DbHelper.Connection.ExecuteFindTable("[Stored_FindBuffer]", new Dictionary<string, object>() {
                {"BufferID",BufferID},
                {"isOptions","1"}
            });
            if (Tab == null || Tab.Rows.Count <= 0) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            /**************************************************************************************************************
             * 开始更新系统缓存
             * **************************************************************************************************************/
            int timer = 0;
            foreach (DataRow cRs in Tab.Rows)
            {
                try
                {
                    using (System.Net.WebClient thisClient = new System.Net.WebClient())
                    {
                        string strResponse = string.Empty;
                        try
                        {
                            thisClient.Encoding = System.Text.Encoding.Default;
                            string url = string.Format("{0}/api/application.aspx?action=all&token={1}", cRs["BufferUrl"].ToString(), AdminRs["strkey"].ToString());
                            if (!url.Contains("http")) { url = "http://" + url; }
                            strResponse = thisClient.DownloadString(url);
                            if (strResponse == "ok") { timer = timer + 1; }
                        }
                        catch { }
                        finally { thisClient.Dispose(); }
                    }
                }
                catch { }
            }
            /**************************************************************************************************************
             * 系统缓存更新结束
             * **************************************************************************************************************/
            this.ErrorMessage(string.Format("成功更新{0}个节点缓存,共更新{1}个节点缓存", timer, Tab.Rows.Count));
            this.History();
            Response.End();
        }
    }
}