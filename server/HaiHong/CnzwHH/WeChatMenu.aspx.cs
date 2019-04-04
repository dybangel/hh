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
    public partial class WeChatMenu : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "to": this.VerificationRole("微信菜单"); ToUpdateMenu(); Response.End(); break;
                case "savesot": this.VerificationRole("微信菜单"); SaveEditor(); Response.End(); break;
                case "edit": this.VerificationRole("微信菜单"); Update(); Response.End(); break;
                case "add": this.VerificationRole("微信菜单"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("微信菜单"); SaveUpdate(); Response.End(); break;
                case "save": this.VerificationRole("微信菜单"); AddSave(); Response.End(); break;
                case "default": this.VerificationRole("微信菜单"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("微信菜单"); Delete(); Response.End(); break;
                case "display": this.VerificationRole("微信菜单"); SaveDisplay(); Response.End(); break;
            }
        }
        /// <summary>
        /// 获取我的银行卡列表信息
        /// </summary>
        protected void strDefault()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">菜单管理 >> 菜单列表</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"navigation\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td>菜单名称</td>");
            strText.Append("<td width=\"120\">排序</td>");
            strText.Append("<td width=\"100\">是否开启</td>");
            strText.Append("<td width=\"100\">动作</td>");
            strText.Append("<td width=\"120\">选项</td>");
            strText.Append("</tr>");
            string Params = " And ParentID=0";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "MenuID,ParentID,MenuName,strRequest,MaterID,strLinks,isDisplay,SortID";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "MenuID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID desc, MenuID asc";
            PageCenterConfig.Tablename = "Fooke_WeChatMenu";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_WeChatMenu", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr style=\"background:#fff9f3\" class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"MenuID\" value=\"{0}\" /></td>", Rs["MenuID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["MenuName"]);
                strText.AppendFormat("<td><input type=\"text\" class=\"inputtext\" operate=\"sortid\" url=\"?action=savesot&menuid={1}\" size=\"5\" value=\"{0}\" /></td>", Rs["SortID"],Rs["MenuID"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.AppendFormat("<a href=\"?action=display&val=0&MenuID={0}\"><img src=\"template/images/ico/yes.gif\"/></a>", Rs["MenuID"]); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&MenuID={0}\"><img src=\"template/images/ico/no.gif\"/></a>", Rs["MenuID"]); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=default&type={0}\">", Rs["strRequest"]);
                switch (Rs["strRequest"].ToString())
                {
                    case "text": strText.Append("文本回复"); break;
                    case "button": strText.Append("图文连接"); break;
                    case "link": strText.Append("连接网址"); break;
                    case "menu": strText.Append("弹出菜单"); break;
                    case "api": strText.Append("API接口"); break;
                }
                strText.AppendFormat("</a>");
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=add&MenuID={0}\" title=\"添加子菜单\"><img src=\"template/images/ico/add.png\" /></a>", Rs["MenuID"]);
                strText.AppendFormat("<a href=\"?action=edit&MenuID={0}\" title=\"编辑菜单\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["MenuID"]);
                strText.AppendFormat("<a href=\"?action=del&MenuID={0}\" title=\"删除菜单\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["MenuID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
                strText.Append(this.ChildList(Rs["MenuID"].ToString()));
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除菜单\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"禁用菜单(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"禁用菜单(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatmenu/default.html");
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
        /// 显示下级子菜单
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        protected string ChildList(string ParentID)
        {
            StringBuilder strText = new StringBuilder();
            string strParams = " and ParentID=" + ParentID + " order by SortID asc";
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_WeChatMenu", Params: strParams);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"MenuID\" value=\"{0}\" /></td>", Rs["MenuID"]);
                strText.AppendFormat("<td>└─{0}</td>", Rs["MenuName"]);
                strText.AppendFormat("<td><input type=\"text\" class=\"inputtext\" operate=\"sortid\" url=\"?action=savesot&menuid={1}\" size=\"5\" value=\"{0}\" /></td>", Rs["SortID"], Rs["MenuID"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.AppendFormat("<a href=\"?action=display&val=0&MenuID={0}\"><img src=\"template/images/ico/yes.gif\"/></a>", Rs["MenuID"]); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&MenuID={0}\"><img src=\"template/images/ico/no.gif\"/></a>", Rs["MenuID"]); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=default&type={0}\">", Rs["strRequest"]);
                switch (Rs["strRequest"].ToString().ToLower())
                {
                    case "text": strText.Append("文本回复"); break;
                    case "mater": strText.Append("图文连接"); break;
                    case "link": strText.Append("连接网址"); break;
                    case "menu": strText.Append("弹出菜单"); break;
                    case "api": strText.Append("API接口"); break;
                }
                strText.AppendFormat("</a>");
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=edit&MenuID={0}\" title=\"编辑菜单\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["MenuID"]);
                strText.AppendFormat("<a href=\"?action=del&MenuID={0}\" title=\"删除菜单\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["MenuID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            return strText.ToString();
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
            string strResponse = Master.Reader("template/wechatmenu/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="禁用菜单(否)"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="禁用菜单(是)"}
                    }, "1"); break;
                    case "options": strValue = new WeChatMenuHelper().Options("0", RequestHelper.GetRequest("MenuID").toInt()); break;
                    case "MaterList": strValue = new MaterHelper().Options("0", ""); break;
                    case "action": strValue = FunctionBase.OptionList(new List<OptionMode>() {
                        new OptionMode(){Value="menu",Text="展示子菜单(一级菜单)"},
                        new OptionMode(){Value="text",Text="文本回复"},
                        new OptionMode(){Value="mater",Text="图文回复"},
                        new OptionMode(){Value="link",Text="连接地址"},
                        new OptionMode(){Value="api",Text="api接口"}
                    }, (RequestHelper.GetRequest("MenuID").toInt() == "0" ? "menu" : "text")); break;
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
            string MenuID = RequestHelper.GetRequest("MenuID").toInt();
            if (MenuID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatMenu]", new Dictionary<string, object>() {
                {"MenuID",MenuID}
            });
            if (Rs == null) { this.ErrorMessage("获取数据失败,请刷新网页重试！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatmenu/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="禁用菜单(否)"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="禁用菜单(是)"}
                    }, Rs["isDisplay"].ToString()); break;
                    case "options": strValue = new WeChatMenuHelper().Options("0", Rs["ParentID"].ToString()); break;
                    case "MaterList": strValue = new MaterHelper().Options("0", Rs["MaterID"].ToString()); break;
                    case "action": strValue = FunctionBase.OptionList(new List<OptionMode>() {
                        new OptionMode(){Value="menu",Text="展示子菜单(一级菜单)"},
                        new OptionMode(){Value="text",Text="文本回复"},
                        new OptionMode(){Value="mater",Text="图文回复"},
                        new OptionMode(){Value="link",Text="连接地址"},
                        new OptionMode(){Value="api",Text="api接口"}
                    }, Rs["strRequest"].ToString()); break;
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
             * 获取并验证微信菜单信息
             * ************************************************************************************************/
            string MenuName = RequestHelper.GetRequest("MenuName").toString();
            if (string.IsNullOrEmpty(MenuName)) { this.ErrorMessage("请填写微信菜单名称！"); Response.End(); }
            else if (MenuName.Length <= 0) { this.ErrorMessage("菜单名称长度请限制在20个汉字以内！"); Response.End(); }
            else if (MenuName.Length >= 20) { this.ErrorMessage("菜单名称长度请限制在20个汉字以内！"); Response.End(); }
            /*************************************************************************************************
             * 获取微信菜单级别信息
             * ************************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatMenu]", new Dictionary<string, object>() {
                {"ParentID",ParentID},
                {"MenuName",MenuName}
            });
            if (cRs != null) { this.ErrorMessage("相同目录下菜单名称已经存在了,请换个名称吧！"); Response.End(); }
            /*************************************************************************************************
             * 检查微信菜单个数
             * ************************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindWeChatMenu]", new Dictionary<string, object>() {
                {"ParentID",ParentID}
            });
            if (thisTab == null) { this.ErrorMessage("获取微信菜单个数失败,请重试！"); Response.End(); }
            else if (ParentID == "0" && thisTab.Rows.Count >= 3) { this.ErrorMessage("一级菜单最多只允许添加3个!"); Response.End(); }
            else if (ParentID != "0" && thisTab.Rows.Count >= 5) { this.ErrorMessage("同目录下二级菜单最多只允许添加5个!"); Response.End(); }
            /*************************************************************************************************
            * 获取菜单动作
            * ***********************************************************************************************/
            string strRequest = RequestHelper.GetRequest("strRequest").toString();
            if (string.IsNullOrEmpty(strRequest)) { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            else if (strRequest.Length <= 0) { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            else if (strRequest.Length > 20) { this.ErrorMessage("非法提交数据,请重试！"); Response.End(); }
            /*************************************************************************************************
            * 获取并验证文本回复内容信息
            * ***********************************************************************************************/
            string strDesc = RequestHelper.GetRequest("strDesc").toString();
            if (strDesc.Length > 400) { this.ErrorMessage("文本回复内容长度400个汉字以内！"); Response.End(); }
            /*************************************************************************************************
            * 获取菜单的动作以及对应选项
            * ***********************************************************************************************/
            if (strRequest.ToLower() == "text" && string.IsNullOrEmpty(strDesc)) { this.ErrorMessage("请填写文本回复内容！"); Response.End(); }
            string MaterID = RequestHelper.GetRequest("MaterID").toInt();
            if (strRequest.ToLower() == "mater" && MaterID == "0") { this.ErrorMessage("请选择图文回复素材!"); Response.End(); }
            string strLinks = RequestHelper.GetRequest("strLinks").toString();
            if (strRequest.ToLower() == "link" && string.IsNullOrEmpty(strLinks)) { this.ErrorMessage("请填写跳转地址！"); Response.End(); }
            if (strRequest.ToLower() == "link" && strLinks.Length > 150) { this.ErrorMessage("跳转地址长度请限制在150个字符以内！"); Response.End(); }
            if (strRequest.ToLower() == "link" && !strLinks.Contains("://")) { this.ErrorMessage("请填写完整的Url地址！"); Response.End(); }
            string APIurl = RequestHelper.GetRequest("APIurl").toString();
            if (strRequest.ToLower() == "api" && string.IsNullOrEmpty(strLinks)) { this.ErrorMessage("请填写API接口地址！"); Response.End(); }
            if (strRequest.ToLower() == "api" && strLinks.Length > 150) { this.ErrorMessage("API接口地址长度请限制在150个字符以内！"); Response.End(); }
            if (strRequest.ToLower() == "api" && !strLinks.Contains("://")) { this.ErrorMessage("请填写完整的API接口地址！"); Response.End(); }
            if (strRequest == "menu" && ParentID != "0") { this.ErrorMessage("二级菜单不允许设置弹出子菜单！"); Response.End(); }
            /*************************************************************************************************
             * 获取不需要验证的数据
             * ***********************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /*************************************************************************************************
            * 开始保存网页内容
            * ***********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["MenuID"] = "0";
            thisDictionary["ParentID"] = ParentID;
            thisDictionary["MenuName"] = MenuName;
            thisDictionary["strRequest"] = strRequest;
            thisDictionary["strDesc"] = strDesc;
            thisDictionary["MaterID"] = MaterID;
            thisDictionary["strLinks"] = strLinks;
            thisDictionary["APIurl"] = APIurl;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveWeChatMenu]", thisDictionary);
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
            /*******************************************************************************************************
             * 获取并验证菜单ID数据信息
             * *****************************************************************************************************/
            string MenuID = RequestHelper.GetRequest("MenuID").toInt();
            if (MenuID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatMenu]", new Dictionary<string, object>() {
                {"MenuID",MenuID}
            });
            if (Rs == null) { this.ErrorMessage("拉取数据失败,你查找的信息不存在！"); Response.End(); }
            /*************************************************************************************************
             * 获取并验证微信菜单信息
             * ************************************************************************************************/
            string MenuName = RequestHelper.GetRequest("MenuName").toString();
            if (string.IsNullOrEmpty(MenuName)) { this.ErrorMessage("请填写微信菜单名称！"); Response.End(); }
            else if (MenuName.Length <= 0) { this.ErrorMessage("菜单名称长度请限制在20个汉字以内！"); Response.End(); }
            else if (MenuName.Length >= 20) { this.ErrorMessage("菜单名称长度请限制在20个汉字以内！"); Response.End(); }
            /*************************************************************************************************
             * 验证同目录下菜单名称是否存在
             * ************************************************************************************************/
            string ParentID = RequestHelper.GetRequest("ParentID").toInt();
            if (ParentID == MenuID) { this.ErrorMessage("请求参数错误,父级分类与菜单ID不能相同！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatMenu]", new Dictionary<string, object>() {
                {"ParentID",ParentID},
                {"MenuName",MenuName}
            });
            if (cRs != null && cRs["MenuID"].ToString() != Rs["MenuID"].ToString()) { this.ErrorMessage("相同目录下菜单名称已经存在了,请换个名称吧！"); Response.End(); }
            /*************************************************************************************************
             * 检查微信菜单个数
             * ************************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindWeChatMenu]", new Dictionary<string, object>() {
                {"ParentID",ParentID}
            });
            if (thisTab == null) { this.ErrorMessage("获取微信菜单个数失败,请重试！"); Response.End(); }
            else if (ParentID == "0" && thisTab.Rows.Count >= 4) { this.ErrorMessage("一级菜单最多只允许添加3个!"); Response.End(); }
            else if (ParentID != "0" && thisTab.Rows.Count >= 6) { this.ErrorMessage("同目录下二级菜单最多只允许添加5个!"); Response.End(); }
            /*************************************************************************************************
            * 获取菜单动作
            * ***********************************************************************************************/
            string strRequest = RequestHelper.GetRequest("strRequest").toString();
            if (string.IsNullOrEmpty(strRequest)) { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            else if (strRequest.Length <= 0) { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            else if (strRequest.Length > 20) { this.ErrorMessage("非法提交数据,请重试！"); Response.End(); }
            /*************************************************************************************************
            * 获取并验证文本回复内容信息
            * ***********************************************************************************************/
            string strDesc = RequestHelper.GetRequest("strDesc").toString();
            if (strDesc.Length > 400) { this.ErrorMessage("文本回复内容长度400个汉字以内！"); Response.End(); }
            /*************************************************************************************************
            * 获取菜单的动作以及对应选项
            * ***********************************************************************************************/
            if (strRequest.ToLower() == "text" && string.IsNullOrEmpty(strDesc)) { this.ErrorMessage("请填写文本回复内容！"); Response.End(); }
            string MaterID = RequestHelper.GetRequest("MaterID").toInt();
            if (strRequest.ToLower() == "mater" && MaterID == "0") { this.ErrorMessage("请选择图文回复素材!"); Response.End(); }
            string strLinks = RequestHelper.GetRequest("strLinks").toString();
            if (strRequest.ToLower() == "link" && string.IsNullOrEmpty(strLinks)) { this.ErrorMessage("请填写跳转地址！"); Response.End(); }
            if (strRequest.ToLower() == "link" && strLinks.Length > 150) { this.ErrorMessage("跳转地址长度请限制在150个字符以内！"); Response.End(); }
            if (strRequest.ToLower() == "link" && !strLinks.Contains("://")) { this.ErrorMessage("请填写完整的Url地址！"); Response.End(); }
            string APIurl = RequestHelper.GetRequest("APIurl").toString();
            if (strRequest.ToLower() == "api" && string.IsNullOrEmpty(strLinks)) { this.ErrorMessage("请填写API接口地址！"); Response.End(); }
            if (strRequest.ToLower() == "api" && strLinks.Length > 150) { this.ErrorMessage("API接口地址长度请限制在150个字符以内！"); Response.End(); }
            if (strRequest.ToLower() == "api" && !strLinks.Contains("://")) { this.ErrorMessage("请填写完整的API接口地址！"); Response.End(); }
            if (strRequest == "menu" && ParentID != "0") { this.ErrorMessage("二级菜单不允许设置弹出子菜单！"); Response.End(); }
            /*************************************************************************************************
             * 获取不需要验证的数据
             * ***********************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /*************************************************************************************************
            * 开始保存网页内容
            * ***********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["MenuID"] = Rs["MenuID"].ToString();
            thisDictionary["ParentID"] = ParentID;
            thisDictionary["MenuName"] = MenuName;
            thisDictionary["strRequest"] = strRequest;
            thisDictionary["strDesc"] = strDesc;
            thisDictionary["MaterID"] = MaterID;
            thisDictionary["strLinks"] = strLinks;
            thisDictionary["APIurl"] = APIurl;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveWeChatMenu]", thisDictionary);
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
            string strList = RequestHelper.GetRequest("MenuID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_WeChatMenu", Params: " and MenuID in (" + strList + ") and isDisplay=0");
            if (Tab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                DbHelper.Connection.Delete("Fooke_WeChatMenu", Params: " and( MenuID =" + Rs["MenuID"] + " or ParentID = " + Rs["MenuID"] + ")");
            }
            /***********************************************************************************************
            * 输出数据处理结果信息
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
            string strList = RequestHelper.GetRequest("MenuID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_WeChatMenu", Params: " and MenuID in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_WeChatMenu", new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and MenuID in (" + strList + ")");
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 保存排序信息
        /// </summary>
        protected void SaveEditor()
        {

            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("MenuID").ToString();
            if (string.IsNullOrEmpty(strList)) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { Response.Write("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_WeChatMenu", Params: " and MenuID in (" + strList + ")");
            if (Tab == null) { Response.Write("没有需要处理的数据！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { Response.Write("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证请求参数值信息
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("value").toInt();
            /***********************************************************************************************
            * 开始保存数据
            * *********************************************************************************************/
            DbHelper.Connection.Update("Fooke_WeChatMenu", new Dictionary<string, string>() {
                {"SortID",strValue}
            }, Params: " and MenuID in (" + strList + ")");
            /***********************************************************************************************
             * 输出网页处理结果信息
             * *********************************************************************************************/
            Response.Write("success");
            Response.End();
        }
        /// <summary>
        /// 更新微信菜单
        /// </summary>
        protected void ToUpdateMenu()
        {
            new Fooke.WeChat.MenuHelper().Update((iSuccess, strResponse) =>
            {
                if (iSuccess) { this.ErrorMessage("菜单更新成功!"); }
                else { this.ErrorMessage(strResponse); }
            });
            Response.End();
        }
    }
}