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
    public partial class Share : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "editor": this.VerificationRole("投资等级"); SaveEditor(); Response.End(); break;
                case "display": this.VerificationRole("投资等级"); SaveDisplay(); Response.End(); break;
                case "edit": this.VerificationRole("投资等级"); Update(); Response.End(); break;
                case "add": this.VerificationRole("投资等级"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("投资等级"); SaveUpdate(); Response.End(); break;
                case "save": this.VerificationRole("投资等级"); AddSave(); Response.End(); break;
                case "default": this.VerificationRole("投资等级"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("投资等级"); Delete(); Response.End(); break;
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
            strText.Append("<td class=\"Base\" colspan=\"8\">分享链接 >> 分享列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="title",Text="搜标题"},
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
            strText.Append("<td width=\"240\">分享标题</td>");
            strText.Append("<td>分享内容</td>");
            strText.Append("<td width=\"70\">奖励积分</td>");
            strText.Append("<td width=\"70\">分享次数</td>");
            strText.Append("<td width=\"80\">显示排序</td>");
            strText.Append("<td width=\"40\">状态</td>");
            strText.Append("<td width=\"80\">操作选项</td>");
            strText.Append("</tr>");
            /****************************************************************************************
             * 生成查询条件
             * **************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType)){strParameter += " and sharetitle like '%" + Keywords + "%'";}
            /****************************************************************************************
             * 构建查询Sql语句
             * **************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ShareID,ShareTitle,ShareText,Thumb,isDisplay,SortID,Points,ShareTimer";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "ShareID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID desc,ShareID asc";
            PageCenterConfig.Tablename = "Fooke_Share";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Share", strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"ShareID\" value=\"{0}\" /></td>", Rs["ShareID"]);
                strText.AppendFormat("<td>{0}</td>",Rs["ShareTitle"]);
                strText.AppendFormat("<td>{0}天</td>", Rs["ShareText"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Points"]);
                strText.AppendFormat("<td>{0}天</td>", Rs["ShareTimer"]);
                strText.AppendFormat("<td><input type=\"text\" style=\"width:40px\" class=\"inputtext\" url=\"?action=editor&gradeid={1}\" operate=\"editsort\" value=\"{0}\" /></td>", Rs["SortID"], Rs["ShareID"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1")
                { strText.AppendFormat("<a href=\"?action=display&ShareID={0}&val=0\"><img src=\"images/ico/yes.gif\"></a>", Rs["ShareID"]); }
                else { strText.AppendFormat("<a href=\"?action=display&ShareID={0}&val=1\"><img src=\"images/ico/no.gif\"></a>", Rs["ShareID"]); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=edit&ShareID={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["ShareID"]);
                strText.AppendFormat("<a href=\"?action=del&ShareID={0}\"  title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["ShareID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }

            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除选中\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"显示(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"显示(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /****************************************************************************************
             * 输出网页内容
             * **************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/share/default.html");
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
            string strResponse = Master.Reader("template/share/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() { 
                        new RadioMode(){Name="isDisplay",Value="1",Text="显示(是)"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="显示(否)"}
                    }, "1"); break;
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "Thumb",
                        tips = "请上传一张图片资源"
                    }, "0"); break;
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
            string ShareID = RequestHelper.GetRequest("ShareID").toInt();
            if (ShareID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindShare", new Dictionary<string, object>() {
                {"ShareID",ShareID}
            });
            if (Rs == null) { this.ErrorMessage("拉取数据失败,你查找的信息不存在！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/share/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() { 
                        new RadioMode(){Name="isDisplay",Value="1",Text="显示(是)"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="显示(否)"}
                    }, Rs["isDisplay"].ToString()); break;
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "Thumb",
                        fileValue=Rs["Thumb"].ToString(),
                        tips = "请上传一张图片资源"
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
            /********************************************************************************************************
             * 验证分享标题名称
             * ********************************************************************************************************/
            string shareTitle = RequestHelper.GetRequest("ShareTitle").ToString();
            if (string.IsNullOrEmpty(shareTitle)) { this.ErrorMessage("请填写分享标题！"); Response.End(); }
            if (shareTitle.Length > 60) { this.ErrorMessage("分享标题长度请限制在60个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindShare", new Dictionary<string, object>() {
                {"ShareTitle",shareTitle}
            });
            if (cRs != null) { this.ErrorMessage("分享标题已经存在,请重试！"); Response.End(); }
            /********************************************************************************************************
             * 验证分享描述,分享图标
             * ********************************************************************************************************/
            string ShareText = RequestHelper.GetRequest("ShareText").ToString();
            if (string.IsNullOrEmpty(ShareText)) { this.ErrorMessage("请填写分享描述内容！"); Response.End(); }
            if (ShareText.Length > 300) { this.ErrorMessage("描述内容长度请限制在300字符以内！"); Response.End(); }
            string Thumb = RequestHelper.GetRequest("Thumb").ToString();
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("请上传分享图标！"); Response.End(); }
            if (Thumb.Length > 150) { this.ErrorMessage("图片地址长度请限制在150个字符以内！"); Response.End(); }
            string strUrl = RequestHelper.GetRequest("strUrl").ToString();
            if (strUrl.Length > 100) { this.ErrorMessage("跳转链接地址长度请限制在100个字符以内！"); Response.End(); }
            /********************************************************************************************************
             * 获取其他数据信息
             * ********************************************************************************************************/
            string ShareContent = RequestHelper.GetBodyContent("content").ToString();
            double Points = RequestHelper.GetRequest("Points").cDouble();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /********************************************************************************************************
             * 开始保存数据
             * ********************************************************************************************************/
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveShare]", new Dictionary<string, object>() {
                {"ShareID","0"},
                {"ShareTitle",shareTitle},
                {"ShareText",ShareText},
                {"ShareContent",ShareContent},
                {"Thumb",Thumb},
                {"isDisplay",isDisplay},
                {"SortID",SortID},
                {"Points",Points.ToString("0.0")},
                {"strUrl",strUrl}
            });
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /********************************************************************************************************
             * 输出网页信息
             * ********************************************************************************************************/
            this.ConfirmMessage("保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }

        /// <summary>
        /// 添加银行卡
        /// </summary>
        protected void SaveUpdate()
        {
            string ShareID = RequestHelper.GetRequest("ShareID").toInt();
            if (ShareID == "0") { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindShare", new Dictionary<string, object>() {
                {"ShareID",ShareID}
            });
            if (Rs == null) { this.ErrorMessage("分享标题已经存在,请重试！"); Response.End(); }
            /********************************************************************************************************
             * 验证分享标题名称
             * ********************************************************************************************************/
            string shareTitle = RequestHelper.GetRequest("ShareTitle").ToString();
            if (string.IsNullOrEmpty(shareTitle)) { this.ErrorMessage("请填写分享标题！"); Response.End(); }
            if (shareTitle.Length > 60) { this.ErrorMessage("分享标题长度请限制在60个汉字以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindShare", new Dictionary<string, object>() {
                {"ShareTitle",shareTitle}
            });
            if (cRs != null && cRs["ShareID"].ToString() != Rs["ShareID"].ToString()) 
            { this.ErrorMessage("分享标题已经存在,请重试！"); Response.End(); }
            /********************************************************************************************************
             * 验证分享描述,分享图标
             * ********************************************************************************************************/
            string ShareText = RequestHelper.GetRequest("ShareText").ToString();
            if (string.IsNullOrEmpty(ShareText)) { this.ErrorMessage("请填写分享描述内容！"); Response.End(); }
            if (ShareText.Length > 300) { this.ErrorMessage("描述内容长度请限制在300字符以内！"); Response.End(); }
            string Thumb = RequestHelper.GetRequest("Thumb").ToString();
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("请上传分享图标！"); Response.End(); }
            if (Thumb.Length > 150) { this.ErrorMessage("图片地址长度请限制在150个字符以内！"); Response.End(); }
            string strUrl = RequestHelper.GetRequest("strUrl").ToString();
            if (strUrl.Length > 100) { this.ErrorMessage("跳转链接地址长度请限制在100个字符以内！"); Response.End(); }
            /********************************************************************************************************
             * 获取其他数据信息
             * ********************************************************************************************************/
            string ShareContent = RequestHelper.GetBodyContent("content").ToString();
            double Points = RequestHelper.GetRequest("Points").cDouble();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /********************************************************************************************************
             * 开始保存数据
             * ********************************************************************************************************/
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveShare]", new Dictionary<string, object>() {
                {"ShareID",ShareID},
                {"ShareTitle",shareTitle},
                {"ShareText",ShareText},
                {"ShareContent",ShareContent},
                {"Thumb",Thumb},
                {"isDisplay",isDisplay},
                {"SortID",SortID},
                {"Points",Points.ToString("0.0")},
                {"strUrl",strUrl}
            });
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /********************************************************************************************************
             * 输出网页信息
             * ********************************************************************************************************/
            this.ConfirmMessage("保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }
        /// <summary>
        /// 删除用户等级信息
        /// </summary>
        protected void Delete()
        {
            string ShareID = RequestHelper.GetRequest("ShareID").toString();
            if (string.IsNullOrEmpty(ShareID)) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_Share", Params: " and ShareID in (" + ShareID + ")");
            /****************************************************
             * 输出网页处理结果
             * ****************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 更新等级状态
        /// </summary>
        protected void SaveDisplay()
        {
            string ShareID = RequestHelper.GetRequest("ShareID").toString();
            if (string.IsNullOrEmpty(ShareID)) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            string val = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> thisDictionary = new Dictionary<string, string>();
            thisDictionary["isDisplay"] = val;
            DbHelper.Connection.Update("Fooke_Share", thisDictionary, Params: " and ShareID in (" + ShareID + ")");
            /*******************************************************************************************
             * 输出数据处理结果
             * ******************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 保存用户排序信息
        /// </summary>
        protected void SaveEditor()
        {
            string ShareID = RequestHelper.GetRequest("ShareID").toInt();
            if (ShareID == "0") { Response.Write("请求参数错误,请至少选择一条数据！"); Response.End(); }
            string value = RequestHelper.GetRequest("value").toInt();
            Dictionary<string, string> thisDictionary = new Dictionary<string, string>();
            thisDictionary["SortID"] = value;
            DbHelper.Connection.Update("Fooke_Share", thisDictionary, Params: " and ShareID=" + ShareID + "");
            /*******************************************************************************************
             * 输出数据处理结果
             * ******************************************************************************************/
            Response.Write("success");
            Response.End();
        }
    }
}