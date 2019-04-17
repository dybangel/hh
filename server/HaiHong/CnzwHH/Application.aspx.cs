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
    public partial class Application : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        { //Response.Write("<sciprt>document.getElementsByName('isKeyword')[0].parentElement.className=\"current\"</script>");

            switch (strRequest)
            {

                case "edit": this.VerificationRole("应用管理"); Update(); Response.End(); break;
                case "add": this.VerificationRole("应用管理"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("应用管理"); SaveUpdate(); Response.End(); break;
                case "save": this.VerificationRole("应用管理"); AddSave(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "display": this.VerificationRole("应用管理"); SaveDisplay(); Response.End(); break;
                case "rec": this.VerificationRole("应用管理"); SaveRecmend(); Response.End(); break;
                case "editor": this.VerificationRole("应用管理"); SaveEditor(); Response.End(); break;
                case "stor":  SelectorList(); Response.End(); break;
                case "default": this.VerificationRole("应用管理"); strDefault(); Response.End(); break;
            }
            
        }
       
        /// <summary>
        /// 快速选择
        /// </summary>
        protected void SelectorList() 
        {
            /**************************************************************************************************
            * 获取筛选条件信息
            * **************************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").toString();
            string AppModel = RequestHelper.GetRequest("AppModel").toString();
            string AdvModel = RequestHelper.GetRequest("AdvModel").toString();
            string UnionModel = RequestHelper.GetRequest("UnionModel").toString();
            /**************************************************************************************************
            * 显示网页输出内容
            * **************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"4\">");
            strText.Append("<form id=\"frmForm\" OnSubmit=\"return _doPost(this);\" action=\"application.aspx\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Appname",Text="搜名称"},
                new OptionMode(){Value="Classname",Text="搜分类"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"搜索关键词\" size=\"12\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"60\">应用类型</td>");
            strText.Append("<td>应用名称</td>");
            strText.Append("<td width=\"60\">奖励金额</td>");
            strText.Append("<td width=\"60\">剩余份数</td>");
            strText.Append("</tr>");
            /************************************************************************************************
             * 构建分页查询语句条件
             * **********************************************************************************************/
            string strParams = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "Appname": strParams += " and Appname like '%" + Keywords + "%'"; break;
                    case "Classname": strParams += " and classname like '%" + Keywords + "%'"; break;
                }
            }
            if (ClassID != "0") { strParams += " and ClassID=" + ClassID + ""; }
            if (!string.IsNullOrEmpty(DeviceModel)) { strParams += " and DeviceModel='" + DeviceModel + "'"; }
            if (!string.IsNullOrEmpty(AppModel)) { strParams += " and AppModel='" + AppModel + "'"; }
            if (!string.IsNullOrEmpty(AdvModel)) { strParams += " and DeviceModel='" + AdvModel + "'"; }
            if (!string.IsNullOrEmpty(UnionModel)) { strParams += " and DeviceModel='" + UnionModel + "'"; }
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /*************************************************************************************************
             * 构建分页查询语句
             * ***********************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "AppID,Classname,DeviceModel,UnionModel,AdvModel,AppName,Amount,Kucun";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "AppID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID Desc,AppID asc";
            PageCenterConfig.Tablename = "Fooke_Application";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Application", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /**************************************************************************************************
            * 遍历网页内容
            * **************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">", JSONHelper.ToString(Rs));
                strText.AppendFormat("<td>{0}</td>", Rs["AdvModel"]);
                strText.AppendFormat("<td><font style=\"color:#cd0000\">({1})</font>{0}</td>", Rs["Appname"], Rs["DeviceModel"]);
                strText.AppendFormat("<td style=\"color:#009900\">{0}元</td>", Rs["Amount"]);
                strText.AppendFormat("<td style=\"color:#FF0000\">{0}份</td>", Rs["Kucun"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************************************
             * 输出网页内容
             * *****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/application/stor.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                    case "pagebar": strValue = PageCenter.Often2(Record, 10); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 列表信息
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 获取筛选条件信息
             * *************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").toString();
            string AppModel = RequestHelper.GetRequest("AppModel").toString();
            string AdvModel = RequestHelper.GetRequest("AdvModel").toString();
            string UnionModel = RequestHelper.GetRequest("UnionModel").toString();
            /**************************************************************************************
             * 构建网页内容
             * *************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"13\">应用管理 >> 应用列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"13\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="Appname",Text="搜名称"},
                new OptionMode(){Value="Classname",Text="搜分类"},
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"搜索关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"80\">系统类型</td>");
            strText.Append("<td width=\"80\">广告类型</td>");
            strText.Append("<td width=\"80\">所属渠道</td>");
            strText.Append("<td width=\"220\">应用名称</td>");
            strText.Append("<td width=\"80\">所属分类</td>");
            strText.Append("<td width=\"140\">检测方式</td>");
            strText.Append("<td width=\"60\">任务奖励</td>");
            strText.Append("<td width=\"60\">剩余分数</td>");
            strText.Append("<td width=\"50\">是否显示</td>");
            //strText.Append("<td width=\"50\">推荐应用</td>");
            strText.Append("<td width=\"80\">显示排序</td>");
            strText.Append("<td width=\"100\">操作选项</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************/
            string strParams = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "Appname": strParams += " and Appname like '%" + Keywords + "%'"; break;
                    case "Classname": strParams += " and classname like '%" + Keywords + "%'"; break;
                }
            }
            if (ClassID != "0") { strParams += " and ClassID=" + ClassID + ""; }
            if (!string.IsNullOrEmpty(DeviceModel)) { strParams += " and DeviceModel='" + DeviceModel + "'"; }
            if (!string.IsNullOrEmpty(AppModel)) { strParams += " and AppModel='" + AppModel + "'"; }
            if (!string.IsNullOrEmpty(AdvModel)) { strParams += " and DeviceModel='" + AdvModel + "'"; }
            if (!string.IsNullOrEmpty(UnionModel)) { strParams += " and DeviceModel='" + UnionModel + "'"; }
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "AppID,AppKey,ClassID,Classname,DeviceModel,AppModel,UnionModel,AdvModel,isWeight,AppName,Amount,TryDate,Kucun,SortID,isRec,isDisplay,Addtime";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "AppID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID Desc,AppID asc";
            PageCenterConfig.Tablename = "Fooke_Application";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Application", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************
            * 循环遍历网页内容
            * **********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"AppID\" value=\"" + Rs["AppID"] + "\" /></td>");
                strText.AppendFormat("<td><a href=\"?action=default&deviceModel={0}\">{0}</a></td>", Rs["DeviceModel"]);
                strText.AppendFormat("<td><a href=\"?action=default&advModel={0}\">{0}</a></td>", Rs["AdvModel"]);
                strText.AppendFormat("<td><a href=\"?action=default&unionModel={0}\">{0}</a></td>", Rs["unionModel"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Appname"]);
                strText.AppendFormat("<td><a href=\"?action=default&classId={0}\">{1}</a></td>", Rs["classId"],Rs["className"]);
                strText.AppendFormat("<td>{0}</td>", ShowAppModel(Rs["appModel"].ToString()));
                strText.AppendFormat("<td style=\"color:#009900\">￥{0}</td>", Rs["Amount"]);
                strText.AppendFormat("<td style=\"color:#ff0000\">{0}</td>", Rs["Kucun"]);
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&AppId=" + Rs["AppId"] + "\"><img src=\"template/images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&AppId=" + Rs["AppId"] + "\"><img src=\"template/images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                //strText.Append("<td>");
                //if (Rs["isRec"].ToString() == "1") { strText.Append("<a href=\"?action=rec&val=0&AppId=" + Rs["AppId"] + "\"><img src=\"template/images/ico/yes.gif\"/></a>"); }
                //else { strText.Append("<a href=\"?action=rec&val=1&AppId=" + Rs["AppId"] + "\"><img src=\"template/images/ico/no.gif\"/></a>"); }
                //strText.AppendFormat("</td>");
                strText.AppendFormat("<td><input type=\"text\" operate=\"edit\" isnumeric=\"true\" url=\"?action=editor&AppId={0}\" size=\"4\" class=\"inputtext center\" value=\"{1}\" /></td>", Rs["AppId"], Rs["SortID"]);
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=edit&appid={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["AppId"]);
                strText.AppendFormat("<a href=\"?action=del&appid={0}\"  title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["AppId"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            /***********************************************************************************************
            * 构建分页控件信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"13\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
            * 构建操作按钮信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"13\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除应用\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"正常显示(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"正常显示(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"rec\" value=\"推荐应用(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"rec\" value=\"推荐应用(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /***********************************************************************************************
            * 输出网页信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/application/default.html");
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
        /// 添加数据
        /// </summary>
        protected void Add()
        {
            /********************************************************************************************
             * 显示输出数据
             * ******************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Application/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "Property": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Name="isDisplay",Text="允许显示(是)",Value="1",Checked="1"},
                        //new CheckBoxMode(){Name="isRec",Text="推荐应用(是)",Value="1",Checked="0"}
                    }); break;
                    case "isKeyword": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Name="isKeyword",Text="关键词搜索",Value="1",Checked="0"}
                    }); break;
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "thumb",
                        tips = "请上传应用图标"
                    }); break;
                    case "SAMPThumb1": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "SAMPThumb1",
                        tips = "上传截图任务图片示例"
                    }); break;
                    case "SAMPThumb2": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "SAMPThumb2",
                        tips = "上传截图任务图片示例"
                    }); break;
                    case "SAMPThumb3": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "SAMPThumb3",
                        tips = "上传截图任务图片示例"
                    }); break;
                    case "sysChar": strValue = GetSysDictionary(""); break;
                    case "modeChar": strValue = GetModeDictionary(""); break;
                    case "isweight": strValue = GetWeightDictionary(""); break;
                    case "appmodel": strValue = GetAppModelDictionary(""); break;
                    case "unionmodel": strValue = GetUnionDictionary(""); break;
                    case "options": strValue = new Fooke.Code.AppClassHelper().Options("0"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        protected void Update()
        {
            /***********************************************************************************************
            * 获取请求参数信息
            * **********************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            ConfigurationHelper cfgContext = new ConfigurationHelper(Rs["strContext"].ToString());
            ConfigurationHelper xmlConfig = new ConfigurationHelper(Rs["strXml"].ToString());
            /***********************************************************************************************
            * 解析网页模板信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/application/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {

                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "thumb",
                        fileValue = Rs["thumb"].ToString(),
                        tips = "请上传应用图标LOGO"
                    }); break;
                    case "Property": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Name="isDisplay",Text="允许显示(是)",Value="1",Checked=Rs["isDisplay"].ToString()},
                        //new CheckBoxMode(){Name="isRec",Text="推荐应用(是)",Value="1",Checked=Rs["isRec"].ToString()}
                    }); break;
                    case "isKeyword": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Name="isKeyword",Text="关键词搜索",Value="1",Checked=Rs["isKeyword"].ToString()}
                    }); break;
                    case "SAMPThumb1": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "SAMPThumb1",
                        tips = "上传截图任务图片示例",
                        fileValue = cfgContext["SAMPThumb1"].toString()
                    }); break;
                    case "SAMPThumb2": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "SAMPThumb2",
                        tips = "上传截图任务图片示例",
                        fileValue = cfgContext["SAMPThumb2"].toString()
                    }); break;
                    case "SAMPThumb3": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "SAMPThumb3",
                        tips = "上传截图任务图片示例",
                        fileValue = cfgContext["SAMPThumb3"].toString()
                    }); break;
                    case "sysChar": strValue = GetSysDictionary(Rs["sysChar"].ToString()); break;
                    case "modeChar": strValue = GetModeDictionary(Rs["modeChar"].ToString()); break;
                    case "isweight": strValue = GetWeightDictionary(Rs["isWeight"].ToString()); break;
                    case "appmodel": strValue = GetAppModelDictionary(Rs["AppModel"].ToString()); break;
                    case "unionmodel": strValue = GetUnionDictionary(Rs["UnionModel"].ToString()); break;
                    case "options": strValue = new Fooke.Code.AppClassHelper().Options(defaultText: Rs["ClassID"].ToString()); break;
                    case "DiscussTitle": strValue = xmlConfig.GetParameter("DiscussTitle").toString(); break;
                    case "DiscussRemark": strValue = xmlConfig.GetParameter("DiscussRemark").toString(); break;
                    case "starContext": strValue = xmlConfig.GetParameter("starContext").toString(); break;
                    case "SortContext": strValue = xmlConfig.GetParameter("SortContext").toString(); break;
                    case "TryContext": strValue = xmlConfig.GetParameter("TryContext").toString(); break;
                    case "FishContext": strValue = xmlConfig.GetParameter("FishContext").toString(); break;
                    case "HotContext": strValue = xmlConfig.GetParameter("HotContext").toString(); break;
                    case "ULDContext": strValue = xmlConfig.GetParameter("ULDContext").toString(); break;
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 保存用户等级
        /// </summary>
        protected void AddSave()
        {
            /*******************************************************************************************************
             * 获取并验证应用设备类型字段信息
             * *****************************************************************************************************/
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").ToString();
            if (string.IsNullOrEmpty(DeviceModel)) { this.ErrorMessage("请选择应用设备类型！"); Response.End(); }
            else if (DeviceModel.Length <= 2) { this.ErrorMessage("设备类型字段长度不能少于2个汉字！"); Response.End(); }
            else if (DeviceModel.Length >= 12) { this.ErrorMessage("设备类型字段长度不鞥超过12个汉字！"); Response.End(); }
            string AdvModel = RequestHelper.GetRequest("AdvModel").ToString();
            if (string.IsNullOrEmpty(AdvModel)) { this.ErrorMessage("请选择应用任务广告类型！"); Response.End(); }
            else if (AdvModel.Length <= 1) { this.ErrorMessage("应用任务广告类型字段长度不能少于2个汉字!"); Response.End(); }
            else if (AdvModel.Length >= 12) { this.ErrorMessage("应用任务广告类型字段长度不能超过12个汉字!"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证任务类型数据字段信息
             * *****************************************************************************************************/
            string TaskerModel = RequestHelper.GetRequest("TaskerModel").toString("快速任务");
            if (string.IsNullOrEmpty(TaskerModel)) { this.ErrorMessage("请选择任务类型!"); Response.End(); }
            else if (TaskerModel.Length <= 0) { this.ErrorMessage("任务类型字段长度不能少于1个汉字！"); Response.End(); }
            else if (TaskerModel.Length >= 16) { this.ErrorMessage("任务类型字段长度不能超过16个汉字！"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证任务广告名称字段信息
             * *****************************************************************************************************/
            string Appname = RequestHelper.GetRequest("Appname").ToString();
            if (string.IsNullOrEmpty(Appname)) { this.ErrorMessage("应用名称不能为空！"); Response.End(); }
            else if (Appname.Length <= 0) { this.ErrorMessage("应用名称不能少于1个汉字！"); Response.End(); }
            else if (Appname.Length >= 24) { this.ErrorMessage("应用名称长度不能超过24个汉字!"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证应用分类信息
             * *****************************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            if (ClassID == "0") { this.ErrorMessage("请选择应用所属分类！"); Response.End(); }
            DataRow classRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppClass]", new Dictionary<string, object>() {
                {"ClassID",ClassID}
            });
            if (classRs == null) { this.ErrorMessage("获取应用分类信息失败,请重试！"); Response.End(); }
            else if (classRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前的应用分类已停止使用！"); Response.End(); }
            /*******************************************************************************************************
             * 获取应用检测类型
             * *****************************************************************************************************/
            string AppModel = RequestHelper.GetRequest("AppModel").toInt();
            if (AppModel == "0") { this.ErrorMessage("请选择应用检测类型！"); Response.End(); }
            string UnionModel = RequestHelper.GetRequest("UnionModel").ToString();
            if (string.IsNullOrEmpty(UnionModel)) { this.ErrorMessage("请选择应用所属渠道！"); Response.End(); }
            else if (UnionModel.Length <= 1) { this.ErrorMessage("应用渠道字段长度不能少于2个汉字！"); Response.End(); }
            else if (UnionModel.Length >= 12) { this.ErrorMessage("应用渠道字段长度不能超过12个汉字！"); Response.End(); }
            /*******************************************************************************************************
            * 获取自有任务数据排重方式
            * *****************************************************************************************************/
            string isWeight = RequestHelper.GetRequest("isWeight").toInt();
            /*******************************************************************************************************
            * 获取并验证任务数量信息
            * *****************************************************************************************************/
            int Kucun = RequestHelper.GetRequest("Kucun").cInt();
            if (Kucun < 0) { this.ErrorMessage("任务数量不能少于0份！"); Response.End(); }
            double Amount = RequestHelper.GetRequest("Amount").cDouble();
            if (Amount <= 0) { this.ErrorMessage("应用任务奖励不能小于0元！"); Response.End(); }
            else if (Amount >= 200) { this.ErrorMessage("应用任务奖励不能超过200元！"); Response.End(); }
            /*******************************************************************************************************
            * 获取并验证应用包名
            * *****************************************************************************************************/
            string Packername = RequestHelper.GetRequest("Packername",false).ToString();
            if (string.IsNullOrEmpty(Packername)) { this.ErrorMessage("请填写应用包名！"); Response.End(); }
            else if (Packername.Length <= 1) { this.ErrorMessage("应用包名长度不能少于2个字符！"); Response.End(); }
            else if (Packername.Length >= 60) { this.ErrorMessage("应用包名字段长度不能超过60个字符！"); Response.End(); }
            string Processname = RequestHelper.GetRequest("Processname", false).ToString();
            if (string.IsNullOrEmpty(Processname)) { this.ErrorMessage("请填写应用进程名称！"); Response.End(); }
            else if (Processname.Length <= 1) { this.ErrorMessage("应用进程名称长度不能少于2个字符！"); Response.End(); }
            else if (Processname.Length >= 60) { /*this.ErrorMessage("应用进程名称长度不能超过60-506个字符！"); Response.End();*/ }
            /*******************************************************************************************************
            * 获取应用图片LOGO地址
            * *****************************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString("/file/app/default.png");
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("获取应用图标地址失败！"); Response.End(); }
            else if (Thumb.Length <= 10) { this.ErrorMessage("应用图标地址长度不能少于10个字符！"); Response.End(); }
            else if (Thumb.Length >= 120) { this.ErrorMessage("应用图标地址长度不能超过120个字符！"); Response.End(); }
            /*******************************************************************************************************
            * 获取应用下载安装地址或搜索关键词
            * *****************************************************************************************************/
            string isKeyword = RequestHelper.GetRequest("isKeyword").toInt();
            string strKeyword = RequestHelper.GetRequest("strKeyword").ToString();
            if (isKeyword == "1" && string.IsNullOrEmpty(strKeyword)) { this.ErrorMessage("您选择了关键词搜索,请填写关键词！"); Response.End(); }
            else if (isKeyword == "1" && strKeyword.Length <= 0) { this.ErrorMessage("您选择了关键词搜索,请填写关键词！"); Response.End(); }
            else if (strKeyword.Length >= 16) { this.ErrorMessage("搜索关键词长度不能超过16个汉字！"); Response.End(); }
            /*******************************************************************************************************
            * 获取并验证应用安装地址数据信息
            * *****************************************************************************************************/
            string strInstall = RequestHelper.GetRequest("strInstall", false).ToString();
            if (isKeyword == "0" && string.IsNullOrEmpty(strInstall)) { this.ErrorMessage("请填写应用下载安装地址！"); Response.End(); }
            else if (isKeyword == "0" && strInstall.Length <= 0) { this.ErrorMessage("请填写应用下载安装地址！"); Response.End(); }
            else if (strInstall.Length != 0 && strInstall.Length <= 10) { this.ErrorMessage("应用下载安装地址长度不能少于10个字符！"); Response.End(); }
            else if (strInstall.Length >= 500) { this.ErrorMessage("应用下载安装地址长度不能超过500个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取第三方应用ID,获取允许做任务的系统版本
             * *****************************************************************************************************/
            string sysChar = RequestHelper.GetRequest("sysChar").ToString();
            if (sysChar.Length != 0 && sysChar.Length <= 3) { this.ErrorMessage("系统版本字段长度不能少于3个字符！"); Response.End(); }
            else if (sysChar.Length >= 255) { this.ErrorMessage("系统版本字段长度不能超过255个字符！"); Response.End(); }
            string modeChar = RequestHelper.GetRequest("modeChar").ToString();
            if (modeChar.Length != 0 && modeChar.Length <= 3) { this.ErrorMessage("手机型号字段长度不能少于3个字符！"); Response.End(); }
            else if (modeChar.Length >= 255) { this.ErrorMessage("手机型号字段长度不能超过255个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取第三方应用ID,应用别名数据信息
             * *****************************************************************************************************/
            string ThirdID = RequestHelper.GetRequest("ThirdID").ToString();
            if (ThirdID.Length >= 36) { this.ErrorMessage("第三方应用ID字段长度不能超过36个字符！"); Response.End(); }
            string Thirdname = RequestHelper.GetRequest("Thirdname").ToString();
            if (Thirdname.Length >= 36) { this.ErrorMessage("第三方应用别名字段长度不能超过36个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取应用试玩时间,默认为180S
             * *****************************************************************************************************/
            int Trydate = RequestHelper.GetRequest("Trydate").cInt(180);
            if (Trydate <= 0) { this.ErrorMessage("应用试玩时间不能小于等于0秒！"); Response.End(); }
            else if (Trydate >= 7200) { this.ErrorMessage("应用试玩时间不能超过7200秒!"); Response.End(); }
            /*******************************************************************************************************
             * 获取应用安装包大小
             * *****************************************************************************************************/
            string Softsize = RequestHelper.GetRequest("Softsize").toString("未知");
            if (string.IsNullOrEmpty(Softsize)) { this.ErrorMessage("请填写应用安装包大小！"); Response.End(); }
            else if (Softsize.Length <= 0) { this.ErrorMessage("请填写应用安装包大小！"); Response.End(); }
            else if (Softsize.Length >= 12) { this.ErrorMessage("安装包大小字段长度不能超过12个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取appStore应用ID
             * *****************************************************************************************************/
            string SoftID = RequestHelper.GetRequest("SoftID").ToString();
            if (SoftID.Length >= 24) { this.ErrorMessage("应用ID字段长度不能超过24个字符！"); Response.End(); }
            string SoftRank = RequestHelper.GetRequest("SoftRank").toString("0");
            if (string.IsNullOrEmpty(SoftRank)) { this.ErrorMessage("请填写下载应用位置排名！"); Response.End(); }
            else if (SoftRank.Length <= 0) { this.ErrorMessage("请填写下载应用位置排名！"); Response.End(); }
            else if (SoftRank.Length >= 12) { this.ErrorMessage("应用排名字段长度不能超过12个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证任务步骤内容信息
             * *****************************************************************************************************/
            string strContext = GetBodyContext();
            if (string.IsNullOrEmpty(strContext)) { this.ErrorMessage("获取任务步骤信息失败!"); Response.End(); }
            else if (!strContext.StartsWith("<configurationRoot>")) { this.ErrorMessage("获取任务步骤信息失败!"); Response.End(); }
            else if (!strContext.EndsWith("</configurationRoot>")) { this.ErrorMessage("获取任务步骤信息失败!"); Response.End(); }
            ConfigurationHelper cfgContext = new ConfigurationHelper(strContext);
            if (cfgContext == null) { this.ErrorMessage("获取任务步骤配置信息失败!"); Response.End(); }
            else if (cfgContext.Length <= 0) { this.ErrorMessage("获取任务步骤配置信息失败!"); Response.End(); }
            /*******************************************************************************************************
             * 获取系统请求参数配置集合
             * *****************************************************************************************************/
            string strXml = RequestHelper.GetPrametersXML(loseTarget: true);
            if (string.IsNullOrEmpty(strXml)) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            else if (!strXml.StartsWith("<configurationRoot>")) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            else if (!strXml.EndsWith("</configurationRoot>")) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            ConfigurationHelper cfgXml = new ConfigurationHelper(strXml);
            if (cfgXml == null) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            else if (cfgXml.Length <= 0) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            /*******************************************************************************************************
             * 获取无需验证的请求数据信息
             * *****************************************************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isRec = RequestHelper.GetRequest("isRec").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /*******************************************************************************************************
             * 生成一个任务标识
             * *****************************************************************************************************/
            string appKey = string.Format("渠道应用-|-|-{0}-|-|-{1}-|-|-渠道应用",
                DateTime.Now.ToString("yyyyMMddHHmmss"), Guid.NewGuid().ToString());
            appKey = new Fooke.Function.String(appKey).ToMD5().Substring(0, 24).ToUpper();
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"appKey",appKey}
            });
            if (cRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /*******************************************************************************************************
             * 开始保存请求数据
             * *****************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["AppKey"] = appKey;
            thisDictionary["ClassID"] = classRs["classId"].ToString();
            thisDictionary["Classname"] = classRs["Classname"].ToString();
            thisDictionary["DeviceModel"] = DeviceModel;
            thisDictionary["AppModel"] = AppModel;
            thisDictionary["UnionModel"] = UnionModel;
            thisDictionary["TaskerModel"] = TaskerModel;
            thisDictionary["AdvModel"] = AdvModel;
            thisDictionary["isWeight"] = isWeight;
            thisDictionary["sysChar"] = sysChar;
            thisDictionary["modeChar"] = modeChar;
            thisDictionary["AppName"] = Appname;
            thisDictionary["Packername"] = Packername;
            thisDictionary["Processname"] = Processname;
            thisDictionary["strInstall"] = strInstall;
            thisDictionary["isKeyword"] = isKeyword;
            thisDictionary["strKeyword"] = strKeyword;
            thisDictionary["Thumb"] = Thumb;
            thisDictionary["Amount"] = Amount;
            thisDictionary["TryDate"] = Trydate;
            thisDictionary["Kucun"] = Kucun;
            thisDictionary["Softsize"] = Softsize;
            thisDictionary["SoftID"] = SoftID;
            thisDictionary["SoftRank"] = SoftRank;
            thisDictionary["ThirdID"] = ThirdID;
            thisDictionary["Thirdname"] = Thirdname;
            thisDictionary["strXml"] = strXml;
            thisDictionary["strContext"] = strContext;
            thisDictionary["SortID"] = SortID;
            thisDictionary["isRec"] = isRec;
            thisDictionary["isDisplay"] = isDisplay;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveApplication]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*******************************************************************************************************
              * 输出数据处理结果
              * *****************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }

        /// <summary>
        /// 保存用户等级
        /// </summary>
        protected void SaveUpdate()
        {
            /*******************************************************************************************************
             * 验证请求参数信息
             * *****************************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证应用设备类型字段信息
             * *****************************************************************************************************/
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").ToString();
            if (string.IsNullOrEmpty(DeviceModel)) { this.ErrorMessage("请选择应用设备类型！"); Response.End(); }
            else if (DeviceModel.Length <= 2) { this.ErrorMessage("设备类型字段长度不能少于2个汉字！"); Response.End(); }
            else if (DeviceModel.Length >= 12) { this.ErrorMessage("设备类型字段长度不鞥超过12个汉字！"); Response.End(); }
            string AdvModel = RequestHelper.GetRequest("AdvModel").ToString();
            if (string.IsNullOrEmpty(AdvModel)) { this.ErrorMessage("请选择应用任务广告类型！"); Response.End(); }
            else if (AdvModel.Length <= 1) { this.ErrorMessage("应用任务广告类型字段长度不能少于2个汉字!"); Response.End(); }
            else if (AdvModel.Length >= 12) { this.ErrorMessage("应用任务广告类型字段长度不能超过12个汉字!"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证任务类型数据字段信息
             * *****************************************************************************************************/
            string TaskerModel = RequestHelper.GetRequest("TaskerModel").toString("快速任务");
            if (string.IsNullOrEmpty(TaskerModel)) { this.ErrorMessage("请选择任务类型!"); Response.End(); }
            else if (TaskerModel.Length <= 0) { this.ErrorMessage("任务类型字段长度不能少于1个汉字！"); Response.End(); }
            else if (TaskerModel.Length >= 16) { this.ErrorMessage("任务类型字段长度不能超过16个汉字！"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证任务广告名称字段信息
             * *****************************************************************************************************/
            string Appname = RequestHelper.GetRequest("Appname").ToString();
            if (string.IsNullOrEmpty(Appname)) { this.ErrorMessage("应用名称不能为空！"); Response.End(); }
            else if (Appname.Length <= 0) { this.ErrorMessage("应用名称不能少于1个汉字！"); Response.End(); }
            else if (Appname.Length >= 24) { this.ErrorMessage("应用名称长度不能超过24个汉字!"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证应用分类信息
             * *****************************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            if (ClassID == "0") { this.ErrorMessage("请选择应用所属分类！"); Response.End(); }
            DataRow classRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppClass]", new Dictionary<string, object>() {
                {"ClassID",ClassID}
            });
            if (classRs == null) { this.ErrorMessage("获取应用分类信息失败,请重试！"); Response.End(); }
            else if (classRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前的应用分类已停止使用！"); Response.End(); }
            /*******************************************************************************************************
             * 获取应用检测类型
             * *****************************************************************************************************/
            string AppModel = RequestHelper.GetRequest("AppModel").toInt();
            if (AppModel == "0") { this.ErrorMessage("请选择应用检测类型！"); Response.End(); }
            string UnionModel = RequestHelper.GetRequest("UnionModel").ToString();
            if (string.IsNullOrEmpty(UnionModel)) { this.ErrorMessage("请选择应用所属渠道！"); Response.End(); }
            else if (UnionModel.Length <= 1) { this.ErrorMessage("应用渠道字段长度不能少于2个汉字！"); Response.End(); }
            else if (UnionModel.Length >= 12) { this.ErrorMessage("应用渠道字段长度不能超过12个汉字！"); Response.End(); }
            /*******************************************************************************************************
            * 获取自有任务数据排重方式
            * *****************************************************************************************************/
            string isWeight = RequestHelper.GetRequest("isWeight").toInt();
            /*******************************************************************************************************
            * 获取并验证任务数量信息
            * *****************************************************************************************************/
            int Kucun = RequestHelper.GetRequest("Kucun").cInt();
            if (Kucun < 0) { this.ErrorMessage("任务数量不能少于0份！"); Response.End(); }
            double Amount = RequestHelper.GetRequest("Amount").cDouble();
            if (Amount <= 0) { this.ErrorMessage("应用任务奖励不能小于0元！"); Response.End(); }
            else if (Amount >= 200) { this.ErrorMessage("应用任务奖励不能超过200元！"); Response.End(); }
            /*******************************************************************************************************
            * 获取并验证应用包名
            * *****************************************************************************************************/
            string Packername = RequestHelper.GetRequest("Packername",false).ToString();
            if (string.IsNullOrEmpty(Packername)) { this.ErrorMessage("请填写应用包名！"); Response.End(); }
            else if (Packername.Length <= 1) { this.ErrorMessage("应用包名长度不能少于2个字符！"); Response.End(); }
            else if (Packername.Length >= 60) { this.ErrorMessage("应用包名字段长度不能超过60个字符！"); Response.End(); }
            string Processname = RequestHelper.GetRequest("Processname",false).ToString();
            if (string.IsNullOrEmpty(Processname)) { this.ErrorMessage("请填写应用进程名称！"); Response.End(); }
            else if (Processname.Length <= 1) { this.ErrorMessage("应用进程名称长度不能少于2个字符！"); Response.End(); }
            else if (Processname.Length >= 60) { /*this.ErrorMessage("应用进程名称长度不能超过60-728个字符！"); Response.End(); */}
            /*******************************************************************************************************
            * 获取应用图片LOGO地址
            * *****************************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString("/file/app/default.png");
            if (string.IsNullOrEmpty(Thumb)) { this.ErrorMessage("获取应用图标地址失败！"); Response.End(); }
            else if (Thumb.Length <= 10) { this.ErrorMessage("应用图标地址长度不能少于10个字符！"); Response.End(); }
            else if (Thumb.Length >= 120) { this.ErrorMessage("应用图标地址长度不能超过120个字符！"); Response.End(); }
            /*******************************************************************************************************
            * 获取应用下载安装地址或搜索关键词
            * *****************************************************************************************************/
            string isKeyword = RequestHelper.GetRequest("isKeyword").toInt();
            string strKeyword = RequestHelper.GetRequest("strKeyword").ToString();
            if (isKeyword == "1" && string.IsNullOrEmpty(strKeyword)) { this.ErrorMessage("您选择了关键词搜索,请填写关键词！"); Response.End(); }
            else if (isKeyword == "1" && strKeyword.Length <= 0) { this.ErrorMessage("您选择了关键词搜索,请填写关键词！"); Response.End(); }
            else if (strKeyword.Length >= 16) { this.ErrorMessage("搜索关键词长度不能超过16个汉字！"); Response.End(); }
            /*******************************************************************************************************
            * 获取并验证应用安装地址数据信息
            * *****************************************************************************************************/
            string strInstall = RequestHelper.GetRequest("strInstall", false).ToString();
            if (isKeyword == "0" && string.IsNullOrEmpty(strInstall)) { this.ErrorMessage("请填写应用下载安装地址！"); Response.End(); }
            else if (isKeyword == "0" && strInstall.Length <= 0) { this.ErrorMessage("请填写应用下载安装地址！"); Response.End(); }
            else if (strInstall.Length != 0 && strInstall.Length <= 10) { this.ErrorMessage("应用下载安装地址长度不能少于10个字符！"); Response.End(); }
            else if (strInstall.Length >= 255) { this.ErrorMessage("应用下载安装地址长度不能超过255个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取第三方应用ID,获取允许做任务的系统版本
             * *****************************************************************************************************/
            string sysChar = RequestHelper.GetRequest("sysChar").ToString();
            if (sysChar.Length != 0 && sysChar.Length <= 3) { this.ErrorMessage("系统版本字段长度不能少于3个字符！"); Response.End(); }
            else if (sysChar.Length >= 255) { this.ErrorMessage("系统版本字段长度不能超过255个字符！"); Response.End(); }
            string modeChar = RequestHelper.GetRequest("modeChar").ToString();
            if (modeChar.Length != 0 && modeChar.Length <= 3) { this.ErrorMessage("手机型号字段长度不能少于3个字符！"); Response.End(); }
            else if (modeChar.Length >= 255) { this.ErrorMessage("手机型号字段长度不能超过255个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取第三方应用ID,应用别名数据信息
             * *****************************************************************************************************/
            string ThirdID = RequestHelper.GetRequest("ThirdID").ToString();
            if (ThirdID.Length >= 36) { this.ErrorMessage("第三方应用ID字段长度不能超过36个字符！"); Response.End(); }
            string Thirdname = RequestHelper.GetRequest("Thirdname").ToString();
            if (Thirdname.Length >= 36) { this.ErrorMessage("第三方应用别名字段长度不能超过36个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取应用试玩时间,默认为180S
             * *****************************************************************************************************/
            int Trydate = RequestHelper.GetRequest("Trydate").cInt(180);
            if (Trydate <= 0) { this.ErrorMessage("应用试玩时间不能小于等于0秒！"); Response.End(); }
            else if (Trydate >= 7200) { this.ErrorMessage("应用试玩时间不能超过7200秒!"); Response.End(); }
            /*******************************************************************************************************
             * 获取应用安装包大小
             * *****************************************************************************************************/
            string Softsize = RequestHelper.GetRequest("Softsize").toString("未知");
            if (string.IsNullOrEmpty(Softsize)) { this.ErrorMessage("请填写应用安装包大小！"); Response.End(); }
            else if (Softsize.Length <= 0) { this.ErrorMessage("请填写应用安装包大小！"); Response.End(); }
            else if (Softsize.Length >= 12) { this.ErrorMessage("安装包大小字段长度不能超过12个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取appStore应用ID
             * *****************************************************************************************************/
            string SoftID = RequestHelper.GetRequest("SoftID").ToString();
            if (SoftID.Length >= 24) { this.ErrorMessage("应用ID字段长度不能超过24个字符！"); Response.End(); }
            string SoftRank = RequestHelper.GetRequest("SoftRank").toString("0");
            if (string.IsNullOrEmpty(SoftRank)) { this.ErrorMessage("请填写下载应用位置排名！"); Response.End(); }
            else if (SoftRank.Length <= 0) { this.ErrorMessage("请填写下载应用位置排名！"); Response.End(); }
            else if (SoftRank.Length >= 12) { this.ErrorMessage("应用排名字段长度不能超过12个字符！"); Response.End(); }
            /*******************************************************************************************************
             * 获取并验证任务步骤内容信息
             * *****************************************************************************************************/
            string strContext = GetBodyContext();
            if (string.IsNullOrEmpty(strContext)) { this.ErrorMessage("获取任务步骤信息失败!"); Response.End(); }
            else if (!strContext.StartsWith("<configurationRoot>")) { this.ErrorMessage("获取任务步骤信息失败!"); Response.End(); }
            else if (!strContext.EndsWith("</configurationRoot>")) { this.ErrorMessage("获取任务步骤信息失败!"); Response.End(); }
            ConfigurationHelper cfgContext = new ConfigurationHelper(strContext);
            if (cfgContext == null) { this.ErrorMessage("获取任务步骤配置信息失败!"); Response.End(); }
            else if (cfgContext.Length <= 0) { this.ErrorMessage("获取任务步骤配置信息失败!"); Response.End(); }
            /*******************************************************************************************************
             * 获取系统请求参数配置集合
             * *****************************************************************************************************/
            string strXml = RequestHelper.GetPrametersXML(loseTarget: true);
            if (string.IsNullOrEmpty(strXml)) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            else if (!strXml.StartsWith("<configurationRoot>")) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            else if (!strXml.EndsWith("</configurationRoot>")) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            ConfigurationHelper cfgXml = new ConfigurationHelper(strXml);
            if (cfgXml == null) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            else if (cfgXml.Length <= 0) { this.ErrorMessage("获取请求参数配置集合信息失败!"); Response.End(); }
            /*******************************************************************************************************
             * 获取无需验证的请求数据信息
             * *****************************************************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isRec = RequestHelper.GetRequest("isRec").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /*******************************************************************************************************
             * 开始保存请求数据
             * *****************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["AppId"] = Rs["AppId"].ToString();
            thisDictionary["AppKey"] = Rs["AppKey"].ToString();
            thisDictionary["ClassID"] = classRs["classId"].ToString();
            thisDictionary["Classname"] = classRs["Classname"].ToString();
            thisDictionary["DeviceModel"] = DeviceModel;
            thisDictionary["AppModel"] = AppModel;
            thisDictionary["UnionModel"] = UnionModel;
            thisDictionary["TaskerModel"] = TaskerModel;
            thisDictionary["AdvModel"] = AdvModel;
            thisDictionary["isWeight"] = isWeight;
            thisDictionary["sysChar"] = sysChar;
            thisDictionary["modeChar"] = modeChar;
            thisDictionary["AppName"] = Appname;
            thisDictionary["Packername"] = Packername;
            thisDictionary["Processname"] = Processname;
            thisDictionary["strInstall"] = strInstall;
            thisDictionary["isKeyword"] = isKeyword;
            thisDictionary["strKeyword"] = strKeyword;
            thisDictionary["Thumb"] = Thumb;
            thisDictionary["Amount"] = Amount;
            thisDictionary["TryDate"] = Trydate;
            thisDictionary["Kucun"] = Kucun;
            thisDictionary["Softsize"] = Softsize;
            thisDictionary["SoftID"] = SoftID;
            thisDictionary["SoftRank"] = SoftRank;
            thisDictionary["ThirdID"] = ThirdID;
            thisDictionary["Thirdname"] = Thirdname;
            thisDictionary["strXml"] = strXml;
            thisDictionary["strContext"] = strContext;
            thisDictionary["SortID"] = SortID;
            thisDictionary["isRec"] = isRec;
            thisDictionary["isDisplay"] = isDisplay;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveApplication]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*******************************************************************************************************
              * 输出数据处理结果
              * *****************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面!");
            Response.End();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        protected void Delete()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AppID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始删除请求数据
            * *********************************************************************************************/
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_Application", Params: " and AppID in (" + strList + ") and isDisplay=0");
            if (sTab == null) { this.ErrorMessage("没有需要删除的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { this.ErrorMessage("没有需要删除的数据！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_Application", Params: " and AppID in (" + strList + ") and isDisplay=0");
            /***********************************************************************************************
            * 输出数据处理结果
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
            string strList = RequestHelper.GetRequest("AppID").ToString();
            if (string.IsNullOrEmpty(strList)) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { Response.Write("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_Application", Params: " and AppID in (" + strList + ")");
            if (sTab == null) { Response.Write("没有需要处理的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { Response.Write("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据信息
             * *********************************************************************************************/
            string SortID = RequestHelper.GetRequest("value").toInt();
            DbHelper.Connection.Update("Fooke_Application", dictionary: new Dictionary<string, string>() {
                {"SortID",SortID}
            }, Params: " and AppID in (" + strList + ")");
            /***********************************************************************************************
             * 输出出局处理结果
             * *********************************************************************************************/
            Response.Write("排序成功！");
            Response.End();
        }
        /// <summary>
        /// 保存推荐
        /// </summary>
        protected void SaveRecmend()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AppID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_Application", Params: " and AppID in (" + strList + ")");
            if (sTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证参数值的合法性
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            if (strValue != "0" && strValue != "1") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据
             * *********************************************************************************************/
            DbHelper.Connection.Update("Fooke_Application", dictionary: new Dictionary<string, string>() {
                {"isRec",strValue}
            }, Params: " and AppID in (" + strList + ")");
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }

        protected void SaveDisplay()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("AppID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_Application", Params: " and AppID in (" + strList + ")");
            if (sTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 验证参数值的合法性
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            if (strValue != "0" && strValue != "1") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据
             * *********************************************************************************************/
            DbHelper.Connection.Update("Fooke_Application", dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and AppID in (" + strList + ")");
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }

        /**********************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 申明系统公共变量
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ********************************************************************************************/
        #region 申明系统公共变量
        /// <summary>
        /// 手机型号选择
        /// </summary>
        public static readonly Dictionary<string, string> MODEDictionary = new Dictionary<string, string>()
        {
            {"iPhone4","iPhone4"},
            {"iPhone5","iPhone5"},
            {"iPhone5S","iPhone5S"},
            {"iPhone5C","iPhone5C"},
            {"iPhone6","iPhone6"},
            {"iPhone6S","iPhone6S"},
            {"iPhone6P","iPhone6P"},
            {"iPhone6SP","iPhone6SP"},
            {"iPhoneSE","iPhoneSE"},
            {"iPhone7","iPhone7"},
            {"iPhone7P","iPhone7P"},
            {"iPhone8","iPhone8"},
            {"iPhone8P","iPhone8P"},
            {"iPhoneX","iPhoneX"},
            {"iPhoneXS","iPhoneXS"},
        };
        /// <summary>
        /// 系统版本选择
        /// </summary>
        public static readonly Dictionary<string, string> SYSDictionary = new Dictionary<string, string>()
        {
            {"iOS7","iOS7"},
            {"iOS8","iOS8"},
            {"iOS9","iOS9"},
            {"iOS10","iOS10"},
            {"iOS11","iOS11"},
            {"iOS12","iOS12"}
        };
        /// <summary>
        /// 系统检测类型
        /// </summary>
        public static readonly Dictionary<string, string> APPMODELDictionary = new Dictionary<string, string>()
        {
            {"1","无接口对接-手动提取IDFA型"},
            {"2","有接口对接-激活回调型-计费以我们检测为准"},
            {"3","有接口对接-激活上报型"},
            {"4","审核型任务1-评论"},
            {"5","审核型任务2-通用型"},
            {"6","有接口对接-激活回调型-计费以广告主回调为准"}
        };

        /// <summary>
        /// 系统检测类型
        /// </summary>
        public static readonly Dictionary<string, string> WEIGHTDictionary = new Dictionary<string, string>()
        {
            {"0","默认排重"},
            {"1","广告主排重"},
            {"2","自排重"},
            {"3","特殊广告排重"}
        };

        /// <summary>
        /// 渠道列表
        /// </summary>
        public static readonly Dictionary<string, string> UNIONDictionary = new Dictionary<string, string>()
        {
            {"无渠道","无渠道(单独广告应用)"},
            {"海尔","海尔"},
            {"微网","微网"},
            {"巨掌","巨掌"},
            {"博睿","博睿"},
            {"来赚","来赚"},
            {"有为","有为"},
            {"掌上互动","掌上互动"},
            {"极北","极北"},
            {"博睿上报","博睿上报"},
            {"壹狗","壹狗"}
        };

        #endregion
        /**********************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 网页方法处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ********************************************************************************************/
        #region 网页方法处理区域
        /// <summary>
        /// 获取当前页面请求参数集合信息
        /// </summary>
        /// <returns></returns>
        public string GetParameterXml()
        {
            StringBuilder strXml = new StringBuilder();
            strXml.AppendFormat("<configurationRoot>");

            strXml.AppendFormat("</configurationRoot>");
            return strXml.ToString();
        }
        /// <summary>
        /// 显示app检测类型
        /// </summary>
        /// <param name="appModel"></param>
        /// <returns></returns>
        public string ShowAppModel(string appModel)
        {
            /*****************************************************************************************
             * 开始执行请求数据处理
             * ***************************************************************************************/
            string strReturn = "默认检测方式";
            switch (appModel)
            {
                case "1": strReturn = "无接口对接-手动提取IDFA型"; break;
                case "2": strReturn = "有接口对接-激活回调型-计费以我们检测为准"; break;
                case "3": strReturn = "有接口对接-激活上报型"; break;
                case "4": strReturn = "审核型任务1-评论"; break;
                case "5": strReturn = "审核型任务2-通用型"; break;
                case "6": strReturn = "有接口对接-激活回调型-计费以广告主回调为准"; break;
            }
            /*****************************************************************************************
             * 返回数据处理结果
             * ***************************************************************************************/
            return strReturn;
        }
        /// <summary>
        /// 显示手机型号列表
        /// </summary>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public string GetModeDictionary(string defaultText = "")
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> Pair in MODEDictionary)
            {
                strBuilder.Append("<label value=\"" + Pair.Key + "\"");
                strBuilder.Append(" text=\"" + Pair.Value + "\">");
                strBuilder.Append("<input type=\"checkbox\"");
                strBuilder.Append(" name=\"modeChar\" value=\"" + Pair.Key + "\"");
                if (defaultText.Contains(Pair.Key)) { strBuilder.Append(" checked"); }
                strBuilder.Append(" />");
                strBuilder.Append(Pair.Value);
                strBuilder.Append("</label>");
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 显示系统版本
        /// </summary>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public string GetSysDictionary(string defaultText = "")
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> Pair in SYSDictionary)
            {
                strBuilder.Append("<label value=\"" + Pair.Key + "\"");
                strBuilder.Append(" text=\"" + Pair.Value + "\">");
                strBuilder.Append("<input type=\"checkbox\"");
                strBuilder.Append(" name=\"sysChar\" value=\"" + Pair.Key + "\"");
                if (defaultText.Contains(Pair.Key)) { strBuilder.Append(" checked"); }
                strBuilder.Append(" />");
                strBuilder.Append(Pair.Value);
                strBuilder.Append("</label>");
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 获取排重方式
        /// </summary>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public string GetWeightDictionary(string defaultText = "")
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> Pair in WEIGHTDictionary)
            {
                strBuilder.Append("<option value=\"" + Pair.Key + "\"");
                if (defaultText.Contains(Pair.Key)) { strBuilder.Append(" selected"); }
                strBuilder.Append(" text=\"" + Pair.Value + "\">");
                strBuilder.Append(Pair.Value);
                strBuilder.Append("</option>");
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 获取检测类型
        /// </summary>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public string GetAppModelDictionary(string defaultText = "")
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> Pair in APPMODELDictionary)
            {
                strBuilder.Append("<option value=\"" + Pair.Key + "\"");
                if (defaultText.Contains(Pair.Key)) { strBuilder.Append(" selected"); }
                strBuilder.Append(" text=\"" + Pair.Value + "\">");
                strBuilder.Append(Pair.Value);
                strBuilder.Append("</option>");
            }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 获取渠道列表信息
        /// </summary>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public string GetUnionDictionary(string defaultText = "")
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> Pair in UNIONDictionary)
            {
                strBuilder.Append("<option value=\"" + Pair.Key + "\"");
                if (defaultText.Contains(Pair.Key)) { strBuilder.Append(" selected"); }
                strBuilder.Append(" text=\"" + Pair.Value + "\">");
                strBuilder.Append(Pair.Value);
                strBuilder.Append("</option>");
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 获取详情页面内容信息
        /// </summary>
        /// <returns></returns>
        public string GetBodyContext()
        {
            /*******************************************************************************************
             * 申明数据返回变量信息
             * *****************************************************************************************/
            StringBuilder strXml = new StringBuilder();
            /*******************************************************************************************
             * 声明数据
             * *****************************************************************************************/
            string starContext = RequestHelper.GetRequest("starContext").ToString();
            string SortContext = RequestHelper.GetRequest("SortContext").ToString();
            string TryContext = RequestHelper.GetRequest("TryContext").ToString();
            string FishContext = RequestHelper.GetRequest("FishContext").ToString();
            string HotContext = RequestHelper.GetRequest("HotContext").ToString();
            string ULDContext = RequestHelper.GetRequest("ULDContext").ToString();
            if (starContext.Length >= 500) { this.ErrorMessage("开始任务描述内容请限制在500汉字内！"); Response.End(); }
            if (SortContext.Length >= 500) { this.ErrorMessage("应用排名描述内容长度请限制在500汉字内！"); Response.End(); }
            if (TryContext.Length >= 500) { this.ErrorMessage("开始试玩描述内容长度请限制在500汉字内！"); Response.End(); }
            if (FishContext.Length >= 500) { this.ErrorMessage("领取奖励描述内容长度请限制在500汉字内！"); Response.End(); }
            if (HotContext.Length >= 500) { this.ErrorMessage("截图要求描述内容长度请限制在500汉字内！"); Response.End(); }
            if (ULDContext.Length >= 500) { this.ErrorMessage("上传截图描述内容长度请限制在500汉字内！"); Response.End(); }
            /*******************************************************************************************
             * 获取上传示例截图内容
             * *****************************************************************************************/
            string AdvModel = RequestHelper.GetRequest("AdvModel").toString("普通广告");
            if (string.IsNullOrEmpty(AdvModel)) { this.ErrorMessage("请选择应用任务广告类型！"); Response.End(); }
            else if (AdvModel.Length <= 1) { this.ErrorMessage("应用任务广告类型字段长度不能少于2个汉字!"); Response.End(); }
            else if (AdvModel.Length >= 12) { this.ErrorMessage("应用任务广告类型字段长度不能超过12个汉字!"); Response.End(); }
            /*******************************************************************************************
            * 获取第一张示例图片广告
            * *****************************************************************************************/
            string SAMPThumb1 = RequestHelper.GetRequest("SAMPThumb1").ToString();
            if (AdvModel == "截图广告" && SAMPThumb1.Length <= 0) { this.ErrorMessage("您选择了截图任务广告,请上传截图示例！"); Response.End(); }
            else if (SAMPThumb1.Length != 0 && SAMPThumb1.Length <= 10) { this.ErrorMessage("截图示例图片地址长度不能少于10个字符！"); Response.End(); }
            else if (SAMPThumb1.Length >= 120) { this.ErrorMessage("截图示例地址长度不能超过120个字符！"); Response.End(); }
            /*******************************************************************************************
            * 获取第二张示例图片广告
            * *****************************************************************************************/
            string SAMPThumb2 = RequestHelper.GetRequest("SAMPThumb2").ToString();
            if (SAMPThumb2.Length != 0 && SAMPThumb2.Length <= 10) { this.ErrorMessage("截图示例图片地址长度不能少于10个字符！"); Response.End(); }
            else if (SAMPThumb2.Length >= 120) { this.ErrorMessage("截图示例地址长度不能超过120个字符！"); Response.End(); }
            /*******************************************************************************************
            * 获取第三张示例图片广告
            * *****************************************************************************************/
            string SAMPThumb3 = RequestHelper.GetRequest("SAMPThumb3").ToString();
            if (SAMPThumb3.Length != 0 && SAMPThumb3.Length <= 10) { this.ErrorMessage("截图示例图片地址长度不能少于10个字符！"); Response.End(); }
            else if (SAMPThumb3.Length >= 120) { this.ErrorMessage("截图示例地址长度不能超过120个字符！"); Response.End(); }
            /*******************************************************************************************
             * 构建数据返回内容信息
             * *****************************************************************************************/
            strXml.AppendFormat("<configurationRoot>");
            strXml.AppendFormat("<starContext><![CDATA[" + starContext + "]]></starContext>");
            strXml.AppendFormat("<SortContext><![CDATA[" + SortContext + "]]></SortContext>");
            strXml.AppendFormat("<TryContext><![CDATA[" + TryContext + "]]></TryContext>");
            strXml.AppendFormat("<FishContext><![CDATA[" + FishContext + "]]></FishContext>");
            strXml.AppendFormat("<HotContext><![CDATA[" + HotContext + "]]></HotContext>");
            strXml.AppendFormat("<ULDContext><![CDATA[" + ULDContext + "]]></ULDContext>");
            strXml.AppendFormat("<SAMPThumb1><![CDATA[" + SAMPThumb1 + "]]></SAMPThumb1>");
            strXml.AppendFormat("<SAMPThumb2><![CDATA[" + SAMPThumb2 + "]]></SAMPThumb2>");
            strXml.AppendFormat("<SAMPThumb3><![CDATA[" + SAMPThumb3 + "]]></SAMPThumb3>");
            strXml.AppendFormat("</configurationRoot>");
            /*******************************************************************************************
             * 返回数据处理结果
             * *****************************************************************************************/
            return strXml.ToString();
        }

        #endregion
    }
}