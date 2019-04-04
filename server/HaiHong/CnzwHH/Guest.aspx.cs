using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using Fooke.Function;
using Fooke.Code;
namespace Fooke.Web.Admin
{
    public partial class Guest : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "editsave": this.VerificationRole("留言反馈"); SaveUpdate(); Response.End(); break;
                case "edit": this.VerificationRole("留言反馈"); strUpdate(); Response.End(); break;
                case "looker": this.VerificationRole("留言反馈"); Looker(); Response.End(); break;
                case "stor": this.VerificationRole("留言反馈"); SelectedList(); Response.End(); break;
                case "del": this.VerificationRole("留言反馈"); this.Delete(); Response.End(); break;
                case "list": this.VerificationRole("留言反馈"); this.strList(); Response.End(); break;
                case "listsave": this.VerificationRole("留言反馈"); SaveList(); Response.End(); break;
                case "saveimage": this.VerificationRole("留言反馈"); SaveListImage(); Response.End(); break;
                default: this.VerificationRole("留言反馈"); this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 查看列表
        /// </summary>
        protected void SelectedList() {

            /********************************************************************************************
             * 获取分页查询参数
             * ******************************************************************************************/
            string SearchType = RequestHelper.GetRequest("searchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            /********************************************************************************************
             * 构建网页内容信息
             * ******************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"3\">");
            strText.Append("<form action=\"?\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="title",Text="搜反馈类型"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写要搜索的关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            /********************************************************************************************
             * 构建网页内容
             * ******************************************************************************************/
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"80\">用户昵称</td>");
            strText.Append("<td>反馈内容</td>");
            strText.Append("<td width=\"120\">反馈日期</td>");
            strText.Append("</tr>");
            /********************************************************************************************
             * 构建分页查询条件
             * ******************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                    case "title": Params += " and strtitle like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            /********************************************************************************************
             * 构建分页参数语句
             * ******************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "GuestID,UserID,Nickname,Addtime,strTitle,isReply";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "GuestID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "GuestID desc";
            PageCenterConfig.Tablename = TableCenter.Guest;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Guest, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">", JSONHelper.ToString(Rs));
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{1}</a></td>",Rs["UserID"], Rs["Nickname"]);
                strText.AppendFormat("<td>{0}{1}</td>",
                    (Rs["isReply"].ToString() == "1" ? "<a class=\"vbtn\">已复</a>" : ""),
                    Rs["strTitle"]);
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"].ToString().cDate().ToString("yyyy-MM-dd HH:mm"));
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /********************************************************************************************
             * 输出网页内容
             * ******************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Guest/stor.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                    case "pagebar": strValue = PageCenter.Often2(Record, 12); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 关键词回复列表
        /// </summary>
        protected void strDefault()
        {
            /********************************************************************************************
             * 获取分页查询条件
             * *****************************************************************************************/
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            /********************************************************************************************
             * 构建网页内容
             * *****************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">反馈信息 >> 反馈列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="title",Text="搜反馈类型"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写关键词\" type=\"text\" size=\"15\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            /********************************************************************************************
             * 构建网页内容信息
             * *****************************************************************************************/
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"60\">反馈用户</td>");
            strText.Append("<td width=\"200\">反馈类型</td>");
            strText.Append("<td>备注信息</td>");
            strText.Append("<td width=\"120\">反馈日期</td>");
            strText.Append("<td width=\"100\">电话</td>");
            strText.Append("<td width=\"60\">回复</td>");
            strText.Append("<td width=\"60\">操作选项</td>");
            strText.Append("</tr>");
            /********************************************************************************************
             * 构建分页查询条件
             * *****************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "orderid": Params += " and orderid like '%" + Keywords + "%'"; break;
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                    case "title": Params += " and title like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate.cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate.cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            /********************************************************************************************
             * 构建分页查询语句
             * *****************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "GuestID,UserID,Nickname,Addtime,strTitle,strContent,strMobile,isReply";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "GuestID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "GuestID desc";
            PageCenterConfig.Tablename = TableCenter.Guest;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Guest, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /********************************************************************************************
             * 循环遍历网页内容
             * *****************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"GuestID\" value=\"{0}\" /></td>", Rs["GuestID"]);
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>",Rs["strTitle"]);
                strText.AppendFormat("<td>{0}</td>", Rs["strContent"]);
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"].ToString().cDate().ToString("yyyy-MM-dd HH:mm"));
                strText.AppendFormat("<td>{0}</td>", Rs["strMobile"]);
                strText.AppendFormat("<td>{0}</td>", (Rs["isReply"].ToString() == "1" ? "<a class=\"vbtn\">已回复</a>" : "<a class=\"vbtnRed\">未回复</a>"));
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=looker&GuestID={0}\" title=\"查看\"><img src=\"template/images/ico/chart.png\" /></a>", Rs["GuestID"]);
                strText.AppendFormat("<a href=\"?action=del&GuestID={0}\"  title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["GuestID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            /********************************************************************************************
             * 构建分页内容信息
             * *****************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<select name=\"target\">");
            strText.Append("<option value=\"sel\">删除选中的数据</option>");
            strText.Append("<option value=\"days\">删除一天前的记录</option>");
            strText.Append("<option value=\"week\">删除一周前的记录</option>");
            strText.Append("<option value=\"month\">删除一月前的记录</option>");
            strText.Append("<option value=\"byear\">删除半年前的记录</option>");
            strText.Append("<option value=\"year\">删除一年前的记录</option>");
            strText.Append("<option value=\"all\">删除所有记录</option>");
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" class=\"button\" value=\"删除选中\" onclick=\"deleteOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /********************************************************************************************
             * 输出网页参数内容
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Guest/default.html");
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
        /// 查看讨论列表信息
        /// </summary>
        protected void strList()
        {
            /************************************************************************************************
             * 获取网页请求参数
             * **********************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { Response.Write("获取反馈信息失败！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { Response.Write("拉取数据失败,请刷新重试！"); Response.End(); }
            /************************************************************************************************
            * 开始查询请求数据
            * **********************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindGuestTotalList]", new Dictionary<string, object>() {
                {"GuestID",cRs["GuestID"].ToString()}
            });
            StringBuilder strBuilder = new StringBuilder();
            /************************************************************************************************
            * 输出遍历网页内容
            * **********************************************************************************************/
            foreach (DataRow Rs in thisTab.Rows)
            {
                strBuilder.AppendFormat("<div operate=\"{0}\" class=\"items\">",
                    (Rs["UserID"].ToString() != "0" ? "user" : "service"));
                strBuilder.AppendFormat("<div class=\"foter\">");
                strBuilder.AppendFormat("<font class=\"name\">" + Rs["Nickname"] + "</font>");
                strBuilder.AppendFormat("<font class=\"date\">" + Rs["Addtime"] + "</font>");
                strBuilder.AppendFormat("</div>");
                strBuilder.AppendFormat("<div class=\"content\">" + Resolution(Rs["strContent"].ToString()) + "</div>");
                strBuilder.AppendFormat("</div>");
            }
            /********************************************************************************************
             * 输出网页参数内容
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Guest/list.html");
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
        /// 解析留言内容信息
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public string Resolution(string strContent)
        {
            try { strContent = strContent.Replace("<text>", "<div>").Replace("</text>", "</div>"); }
            catch { }
            /**************************************************************************************
             * 解析图片内容信息
             * ************************************************************************************/
            if (strContent.Contains("<image>") && strContent.Contains("</image>"))
            {
                try
                {
                    System.Text.RegularExpressions.Regex Rex = new System.Text.RegularExpressions.Regex(@"<image>(.+?)</image>");
                    foreach (Match thisMatch in Rex.Matches(strContent))
                    {
                        try
                        {
                            string strValue = thisMatch.Result("$1");
                            strValue = strValue.Replace("<![CDATA[", "");
                            strValue = strValue.Replace("]]>", "");
                            strValue = "<img src=\"" + strValue + "\" />";
                            strContent = strContent.Replace(thisMatch.Value, strValue);
                        }
                        catch { }
                    }
                }
                catch { }
            }
            strContent = strContent.Replace("<![CDATA[", "");
            strContent = strContent.Replace("]]>", "");
            strContent = strContent.Replace("<configurationRoot>", "");
            strContent = strContent.Replace("</configurationRoot>", "");
            return strContent;
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        protected void Looker()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Guest/looker.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 查看详情
        /// </summary>
        protected void strUpdate()
        {
            /************************************************************************************************
             * 获取网页请求参数
             * **********************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { Response.Write("获取反馈信息失败！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { Response.Write("拉取数据失败,请刷新重试！"); Response.End(); }
            /************************************************************************************************
             * 输出网页内容信息
             * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Guest/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                { 
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 保存管理员回复内容
        /// </summary>
        protected void SaveUpdate()
        {
            /************************************************************************************************
             * 获取网页请求参数
             * **********************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { Response.Write("获取反馈信息失败！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { Response.Write("拉取数据失败,请刷新重试！"); Response.End(); }
            /*******************************************************************************
             * 获取回复内容信息
             * *****************************************************************************/
            string strReply = RequestHelper.GetRequest("strReply").toString();
            if (string.IsNullOrEmpty(strReply)) { this.ErrorMessage("请填写回复内容！"); Response.End(); }
            if (strReply.Length > 400) { this.ErrorMessage("回复内容请限制在400个汉字以内！"); Response.End(); }
            /*******************************************************************************
             * 开始保存数据
             * *****************************************************************************/
            DbHelper.Connection.Update("Fooke_Guest", new Dictionary<string, string>() {
                {"strReply",strReply},
                {"isReply","1"},
                {"LastDate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
            }, Params: " and GuestID=" + GuestID + "");
            /******************************************************************************
             * 输出执行结果
             * ****************************************************************************/
            this.ErrorMessage("回复成功!", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 保存留言回复信息
        /// </summary>
        protected void SaveList()
        {
            /************************************************************************************************
             * 验证留言ID是否合法
             * **********************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { this.ErrorMessage("获取反馈信息失败！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { this.ErrorMessage("拉取数据失败,请刷新重试！"); Response.End(); }
            /*******************************************************************************
             * 获取回复内容信息
             * *****************************************************************************/
            string strReply = RequestHelper.GetRequest("strReply").toString();
            if (string.IsNullOrEmpty(strReply)) { this.ErrorMessage("请填写回复内容！"); Response.End(); }
            if (strReply.Length > 400) { this.ErrorMessage("回复内容请限制在400个汉字以内！"); Response.End(); }
            string strContent = "<configurationRoot><text>" + strReply + "</text></configurationRoot>";
            /*******************************************************************************
             * 开始保存数据
             * *****************************************************************************/
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveGuestList]", new Dictionary<string, object>() {
                {"GuestID",cRs["GuestID"].ToString()},
                {"UserID","0"},
                {"Nickname","客服"},
                {"strContent",strContent}
            });
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /***************************************************************************************************************
             * 给用户发送一条推送消息
             * ***************************************************************************************************************/
            try
            {
                new PushCenter().Start(Configure: this.Configure,
                content: "您有新的反馈回复:" + strContent + "",
                identify: cRs["UserID"].ToString(),
                appMode: "guest",
                appId: cRs["GuestID"].ToString());
            }
            catch { }
            /******************************************************************************
             * 输出执行结果
             * ****************************************************************************/
            this.ErrorMessage("回复成功!", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 保存留言图片回复
        /// </summary>
        protected void SaveListImage()
        {
            /************************************************************************************************
             * 验证留言ID是否合法
             * **********************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { this.ErrorMessage("获取反馈信息失败！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { this.ErrorMessage("拉取数据失败,请刷新重试！"); Response.End(); }
            /*******************************************************************************
             * 获取回复内容信息
             * *****************************************************************************/
            if (Request.Files == null) { this.ErrorMessage("请上传图片文件!"); Response.End(); }
            else if (Request.Files.Count <= 0) { this.ErrorMessage("请上传图片!"); Response.End(); }
            else if (Request.Files["frmFile"] == null) { this.ErrorMessage("请上传图片!"); Response.End(); }
            /*******************************************************************************
             * 准备上传图片,获取图片内容信息
             * *****************************************************************************/
            string strResponse = string.Empty;
            try
            {
                string fileName = string.Format("留言反馈-|-|-{0}-|-|-{0}", DateTime.Now.Ticks.ToString(), cRs["GuestID"].ToString());
                fileName = new Fooke.Function.String(fileName).ToMD5().Substring(0, 16).ToLower();
                fileName = fileName + ".{exc}";
                new PostedHelper().SaveAs(Request.Files[0], new Fooke.Function.PostedHelper.FileMode()
                {
                    fileName = fileName,
                    fileDirectory = "Guest",
                    fileExt = "jpg|png|bmp|jpeg",
                    fileSize = 1024 * 1024 * 2,
                    Success = (Thumb) => { strResponse = Thumb; },
                    Error = (Exp) => { this.JSONMessage(Exp); Response.End(); }
                });
            }
            catch { }
            /*****************************************************************************************************
             * 保存上传图片信息
             * ***************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("支付截图上传失败为空,请重试！"); Response.End(); }
            if (strResponse.Contains("error")) { this.ErrorMessage("支付截图上传失败地址错误,请重试！"); Response.End(); }
            /*******************************************************************************
             * 生成图片内容信息
             * *****************************************************************************/
            string strContent = "<configurationRoot><image><![CDATA[" + strResponse + "]]></image></configurationRoot>";
            /*******************************************************************************
             * 开始保存数据
             * *****************************************************************************/
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveGuestList]", new Dictionary<string, object>() {
                {"GuestID",cRs["GuestID"].ToString()},
                {"UserID","0"},
                {"Nickname","客服"},
                {"strContent",strContent}
            });
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /***************************************************************************************************************
             * 给用户发送一条推送消息
             * ***************************************************************************************************************/
            try
            {
                new PushCenter().Start(Configure: this.Configure,
                content: "您有新的反馈回复:[图片]",
                identify: cRs["UserID"].ToString(),
                appMode: "guest",
                appId: cRs["GuestID"].ToString());
            }
            catch { }
            /******************************************************************************
             * 输出执行结果
             * ****************************************************************************/
            this.ErrorMessage("回复成功!", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 删除用户张变记录信息
        /// </summary>
        protected void Delete()
        {
            string GuestID = RequestHelper.GetRequest("GuestID").toString();
            string target = RequestHelper.GetRequest("target").toString();
            if (string.IsNullOrEmpty(target)) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            if (target == "sel" && string.IsNullOrEmpty(GuestID)) { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            string strParamter = string.Empty;
            switch (target.ToLower())
            {
                case "sel": strParamter += " and GuestID in (" + GuestID + ")"; break;
                case "days": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and Addtime<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and Addtime<='" + DateTime.Now.AddYears(-1) + "'"; break;
                case "all": strParamter += " and 1=1"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DbHelper.Connection.Delete(TableCenter.Guest, Params: strParamter);
            /********************************************************************
             * 输出返回数据
             * ******************************************************************/
            this.History();
            Response.End();
        }
    }
}