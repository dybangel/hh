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
    public partial class UnionChannel : Fooke.Code.AdminHelper
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "edit": this.VerificationRole("联盟渠道"); Update(); Response.End(); break;
                case "add": this.VerificationRole("联盟渠道"); Add(); Response.End(); break;
                case "editsave": this.VerificationRole("联盟渠道"); SaveUpdate(); Response.End(); break;
                case "save": this.VerificationRole("联盟渠道"); AddSave(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "display": this.VerificationRole("联盟渠道"); SaveDisplay(); Response.End(); break;
                case "top": this.VerificationRole("联盟渠道"); SaveRecommand(); Response.End(); break;
                case "editor": this.VerificationRole("联盟渠道"); SaveEditor(); Response.End(); break;
                case "default": this.VerificationRole("联盟渠道"); strDefault(); Response.End(); break;
            }
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
            /**************************************************************************************
             * 构建网页内容
             * *************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"10\">渠道管理 >> 渠道列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"10\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="strUnion",Text="搜名称"},
                new OptionMode(){Value="UnionModel",Text="搜渠道"},
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
            strText.Append("<td width=\"80\">渠道标识</td>");
            strText.Append("<td width=\"80\">设备类型</td>");
            strText.Append("<td>渠道名称</td>");
            strText.Append("<td width=\"260\">回调地址</td>");
            strText.Append("<td width=\"60\">审核通过</td>");
            strText.Append("<td width=\"60\">推荐渠道</td>");
            strText.Append("<td width=\"100\">显示排序</td>");
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
                    case "strUnion": strParams += " and strUnion like '%" + Keywords + "%'"; break;
                    case "UnionModel": strParams += " and UnionModel like '%" + Keywords + "%'"; break;
                }
            }
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UnionID,UnionKey,UnionModel,DeviceModel,strUnion,strThumb,strRemark,SortID,isDisplay,isTop";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "UnionID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID Desc,UnionID asc";
            PageCenterConfig.Tablename = "Fooke_UnionChannel";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UnionChannel", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************
            * 循环遍历网页内容
            * **********************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"UnionID\" value=\"" + Rs["UnionID"] + "\" /></td>");
                strText.AppendFormat("<td>{0}</td>", Rs["UnionID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["DeviceModel"]);
                strText.AppendFormat("<td><font style=\"color:#CD0000\">【{0}】</font>{1}<font style=\"color:#999\">({2})</font></td>", Rs["UnionModel"], Rs["strUnion"], Rs["strRemark"]);
                strText.AppendFormat("<td><input type=\"text\" operate=\"copy\" readonly size=\"30\" value=\"{0}/app/{1}/{2}.aspx\" /></td>", FunctionCenter.SiteUrl(), Rs["UnionID"],Rs["UnionKey"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.AppendFormat("<a href=\"?action=display&val=0&UnionID=" + Rs["UnionID"] + "\"><img src=\"template/images/ico/yes.gif\"/></a>"); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&UnionID=" + Rs["UnionID"] + "\"><img src=\"template/images/ico/no.gif\"/></a>"); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                if (Rs["isTop"].ToString() == "1") { strText.AppendFormat("<a href=\"?action=top&val=0&UnionID=" + Rs["UnionID"] + "\"><img src=\"template/images/ico/yes.gif\"/></a>"); }
                else { strText.AppendFormat("<a href=\"?action=top&val=1&UnionID=" + Rs["UnionID"] + "\"><img src=\"template/images/ico/no.gif\"/></a>"); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td><input type=\"text\" operate=\"edit\" isnumeric=\"true\" url=\"?action=editor&UnionID=" + Rs["UnionID"] + "\" size=\"5\" class=\"inputtext center\" value=\"" + Rs["SortID"] + "\" /></td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=edit&UnionID=" + Rs["UnionID"] + "\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>");
                strText.AppendFormat("<a href=\"?action=del&UnionID=" + Rs["UnionID"] + "\"  title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>");
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            /***********************************************************************************************
            * 构建分页控件信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"10\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /***********************************************************************************************
            * 构建操作按钮信息
            * **********************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"10\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除渠道\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"通过审核(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"通过审核(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"top\" value=\"推荐渠道(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"top\" value=\"推荐渠道(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /***********************************************************************************************
            * 输出网页信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/UnionChannel/default.html");
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
            string strResponse = Master.Reader("template/unionChannel/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "Property": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>(){
                        new CheckBoxMode(){Name="isDisplay",Value="1",Text="通过审核(是)",Checked="1"},
                        new CheckBoxMode(){Name="isTop",Value="1",Text="推荐渠道(是)",Checked="0"}});
                        break;
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "Thumb",
                        tips = "请选择文件上传",
                        notKong = true
                    }, "0"); break;
                    case "UnionModel": strValue = new UnionHelper().SDKOptions(); break;
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
            /***********************************************************************************************
            * 获取请求参数信息
            * **********************************************************************************************/
            string UnionID = RequestHelper.GetRequest("UnionID").toInt();
            if (UnionID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUnionChannel]", new Dictionary<string, object>() {
                {"UnionID",UnionID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /***********************************************************************************************
            * 获取系统cfg参数配置合集
            * **********************************************************************************************/
            ConfigurationHelper cfgHelper = new ConfigurationHelper(Rs["strXml"].ToString());
            if (cfgHelper == null) { this.ErrorMessage("获取系统配置参数信息失败,请重试！"); Response.End(); }
            else if (cfgHelper.Length <= 0) { this.ErrorMessage("获取系统配置参数信息失败,请重试！"); Response.End(); }
            /***********************************************************************************************
            * 解析网页模板信息
            * **********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/UnionChannel/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "Property": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>(){
                        new CheckBoxMode(){Name="isDisplay",Value="1",Text="通过审核(是)",Checked=Rs["isDisplay"].ToString()},
                        new CheckBoxMode(){Name="isTop",Value="1",Text="推荐渠道(是)",Checked=Rs["isTop"].ToString()}});
                        break;
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "Thumb",
                        tips = "请选择文件上传",
                        fileValue = Rs["strThumb"].ToString(),
                        notKong = true
                    }, "0"); break;
                    case "UnionModel": strValue = new UnionHelper().SDKOptions(Rs["UnionModel"].ToString()); break;
                    default:
                        if (Rs.Table.Columns[funName] != null)
                        {
                            try { strValue = Rs[funName].ToString(); }
                            catch { };
                        }
                        else
                        {
                            try { strValue = cfgHelper[funName].ToString(); }
                            catch { };
                        }
                        ;break;
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
            /****************************************************************************************************************************
             * 获取并验证渠道设备信息
             * ***************************************************************************************************************************/
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").toString();
            if (string.IsNullOrEmpty(DeviceModel)) { this.ErrorMessage("请选择联盟渠道设备类型信息！"); Response.End(); }
            else if (DeviceModel.Length <= 2) { this.ErrorMessage("获取联盟渠道设备类型信息失败,请重试！"); Response.End(); }
            else if (DeviceModel.Length >= 16) { this.ErrorMessage("联盟渠道设备类型字段长度不能超过16个汉字!"); Response.End(); }
            string UnionModel = RequestHelper.GetRequest("UnionModel").ToString();
            if (string.IsNullOrEmpty(UnionModel)) { this.ErrorMessage("请选择接入渠道类型！"); Response.End(); }
            else if (UnionModel.Length <= 0) { this.ErrorMessage("接入渠道类型长度不能少于1个汉字！"); Response.End(); }
            else if (UnionModel.Length >= 12) { this.ErrorMessage("接入渠道类型长度不能超过12个汉字！"); Response.End(); }
            string strUnion = RequestHelper.GetRequest("strUnion").ToString();
            if (string.IsNullOrEmpty(strUnion)) { this.ErrorMessage("接入渠道名称不能为空！"); Response.End(); }
            else if (strUnion.Length <= 0) { this.ErrorMessage("接入渠道名称信息不能少于1个汉字！"); Response.End(); }
            else if (strUnion.Length >= 16) { this.ErrorMessage("接入渠道名称信息长度不能超过16个汉字！"); Response.End(); }
            /****************************************************************************************************************************
             * 验证联盟名称是否重复
             * ***************************************************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUnionChannel]", new Dictionary<string, object>() {
                {"strUnion",strUnion}
            });
            if (cRs != null) { this.ErrorMessage("联盟渠道名称已经存在,请重试！"); Response.End(); }
            /****************************************************************************************************************************
             * 获取并验证渠道应用图标信息
             * ***************************************************************************************************************************/
            string strThumb = RequestHelper.GetRequest("Thumb").ToString();
            if (string.IsNullOrEmpty(strThumb)) { this.ErrorMessage("获取渠道标识图标失败！"); Response.End(); }
            else if (strThumb.Length <= 10) { this.ErrorMessage("渠道标识图标地址长度不能少于10个字符！"); Response.End(); }
            else if (strThumb.Length >= 120) { this.ErrorMessage("渠道标识图标地址长度不能超过120个字符！"); Response.End(); }
            string strRemark = RequestHelper.GetRequest("strRemark").ToString();
            if (strRemark.Length >= 60) { this.ErrorMessage("渠道描述说明信息长度不能超过60个汉字！"); Response.End(); }
            /****************************************************************************************************************************
             * 获取并验证回调参数信息,当前信息理论上为必填字段
             * ***************************************************************************************************************************/
            string OpenID = RequestHelper.GetRequest("OpenName").toString();
            if (string.IsNullOrEmpty(OpenID)) { this.ErrorMessage("请填写回调用户标识字段名！"); Response.End(); }
            else if (OpenID.Length<=0) { this.ErrorMessage("渠道回调用户字段参数长度不能少于1个字符！"); Response.End(); }
            else if (OpenID.Length>=20) { this.ErrorMessage("渠道回调用户字段参数长度不能超过20个字符！"); Response.End(); }
            string NumberName = RequestHelper.GetRequest("NumberName").toString();
            if (string.IsNullOrEmpty(NumberName)) { this.ErrorMessage("请填写回调积分参数字段名！"); Response.End(); }
            else if (NumberName.Length<=0) { this.ErrorMessage("请填写回调积分参数字段名！"); Response.End(); }
            else if (NumberName.Length>=20) { this.ErrorMessage("积分回调参数字段长度不能超过20个字符！"); Response.End(); }
            string ApplicationName = RequestHelper.GetRequest("ApplicationName").toString();
            if (string.IsNullOrEmpty(ApplicationName)) { this.ErrorMessage("请填写回调下载应用字段名！"); Response.End(); }
            else if (ApplicationName.Length <= 0) { this.ErrorMessage("回调下载应用名称参数不能为空！"); Response.End(); }
            else if (ApplicationName.Length >= 20) { this.ErrorMessage("回调下载应用名称参数长度不能超过20个汉字！"); Response.End(); }
            /****************************************************************************************************************************
             * 获取系统配置参数信息,联盟配置参数参数信息
             * ***************************************************************************************************************************/
            string strXml = RequestHelper.GetPrametersXML();
            /****************************************************************************************************************************
             * 获取不需要验证的请求参数
             * ***************************************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isTop = RequestHelper.GetRequest("isTop").toInt();
            /****************************************************************************************************************************
             * 生成渠道唯一标识数据信息
             * ***************************************************************************************************************************/
            string UnionKey = string.Format("渠道标识-|-|-{0}-|-|-{1}-|-|-渠道标识",
                strUnion, Guid.NewGuid().ToString());
            UnionKey = new Fooke.Function.String(UnionKey).ToMD5().Substring(0, 24).ToUpper();
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUniuonChannel]", new Dictionary<string, object>() {
                {"UnionKey",UnionKey}
            });
            if (sRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /****************************************************************************************************************************
             * 开始保存请求数据
             * ***************************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["UnionKey"] = UnionKey;
            thisDictionary["UnionModel"] = UnionModel;
            thisDictionary["DeviceModel"] = DeviceModel;
            thisDictionary["strUnion"] = strUnion;
            thisDictionary["strThumb"] = strThumb;
            thisDictionary["strRemark"] = strRemark;
            thisDictionary["strXml"] = strXml;
            thisDictionary["SortID"] = SortID;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["isTop"] = isTop;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUnionChannel]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************************************************************************************
             * 开始保存请求数据信息
             * ***************************************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面！");
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
            string UnionID = RequestHelper.GetRequest("UnionID").toInt();
            if (UnionID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUnionChannel]", new Dictionary<string, object>() {
                {"UnionID",UnionID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /****************************************************************************************************************************
             * 获取并验证渠道设备信息
             * ***************************************************************************************************************************/
            string DeviceModel = RequestHelper.GetRequest("DeviceModel").toString();
            if (string.IsNullOrEmpty(DeviceModel)) { this.ErrorMessage("请选择联盟渠道设备类型信息！"); Response.End(); }
            else if (DeviceModel.Length <= 2) { this.ErrorMessage("获取联盟渠道设备类型信息失败,请重试！"); Response.End(); }
            else if (DeviceModel.Length >= 16) { this.ErrorMessage("联盟渠道设备类型字段长度不能超过16个汉字!"); Response.End(); }
            string UnionModel = RequestHelper.GetRequest("UnionModel").ToString();
            if (string.IsNullOrEmpty(UnionModel)) { this.ErrorMessage("请选择接入渠道类型！"); Response.End(); }
            else if (UnionModel.Length <= 0) { this.ErrorMessage("接入渠道类型长度不能少于1个汉字！"); Response.End(); }
            else if (UnionModel.Length >= 12) { this.ErrorMessage("接入渠道类型长度不能超过12个汉字！"); Response.End(); }
            string strUnion = RequestHelper.GetRequest("strUnion").ToString();
            if (string.IsNullOrEmpty(strUnion)) { this.ErrorMessage("接入渠道名称不能为空！"); Response.End(); }
            else if (strUnion.Length <= 0) { this.ErrorMessage("接入渠道名称信息不能少于1个汉字！"); Response.End(); }
            else if (strUnion.Length >= 16) { this.ErrorMessage("接入渠道名称信息长度不能超过16个汉字！"); Response.End(); }
            /****************************************************************************************************************************
             * 验证联盟名称是否重复
             * ***************************************************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUnionChannel]", new Dictionary<string, object>() {
                {"strUnion",strUnion}
            });
            if (cRs != null && cRs["UnionID"].ToString()!=Rs["UnionID"].ToString()) { this.ErrorMessage("联盟渠道名称已经存在,请重试！"); Response.End(); }
            /****************************************************************************************************************************
             * 获取并验证渠道应用图标信息
             * ***************************************************************************************************************************/
            string strThumb = RequestHelper.GetRequest("Thumb").ToString();
            if (string.IsNullOrEmpty(strThumb)) { this.ErrorMessage("获取渠道标识图标失败！"); Response.End(); }
            else if (strThumb.Length <= 10) { this.ErrorMessage("渠道标识图标地址长度不能少于10个字符！"); Response.End(); }
            else if (strThumb.Length >= 120) { this.ErrorMessage("渠道标识图标地址长度不能超过120个字符！"); Response.End(); }
            string strRemark = RequestHelper.GetRequest("strRemark").ToString();
            if (strRemark.Length >= 60) { this.ErrorMessage("渠道描述说明信息长度不能超过60个汉字！"); Response.End(); }
            /****************************************************************************************************************************
             * 获取并验证回调参数信息,当前信息理论上为必填字段
             * ***************************************************************************************************************************/
            string OpenID = RequestHelper.GetRequest("OpenName").toString();
            if (string.IsNullOrEmpty(OpenID)) { this.ErrorMessage("请填写回调用户标识字段名！"); Response.End(); }
            else if (OpenID.Length <= 0) { this.ErrorMessage("渠道回调用户字段参数长度不能少于1个字符！"); Response.End(); }
            else if (OpenID.Length >= 20) { this.ErrorMessage("渠道回调用户字段参数长度不能超过20个字符！"); Response.End(); }
            string NumberName = RequestHelper.GetRequest("NumberName").toString();
            if (string.IsNullOrEmpty(NumberName)) { this.ErrorMessage("请填写回调积分参数字段名！"); Response.End(); }
            else if (NumberName.Length <= 0) { this.ErrorMessage("请填写回调积分参数字段名！"); Response.End(); }
            else if (NumberName.Length >= 20) { this.ErrorMessage("积分回调参数字段长度不能超过20个字符！"); Response.End(); }
            string ApplicationName = RequestHelper.GetRequest("ApplicationName").toString();
            if (string.IsNullOrEmpty(ApplicationName)) { this.ErrorMessage("请填写回调下载应用字段名！"); Response.End(); }
            else if (ApplicationName.Length <= 0) { this.ErrorMessage("回调下载应用名称参数不能为空！"); Response.End(); }
            else if (ApplicationName.Length >= 20) { this.ErrorMessage("回调下载应用名称参数长度不能超过20个汉字！"); Response.End(); }
            /****************************************************************************************************************************
             * 获取系统配置参数信息,联盟配置参数参数信息
             * ***************************************************************************************************************************/
            string strXml = RequestHelper.GetPrametersXML();
            /****************************************************************************************************************************
             * 获取不需要验证的请求参数
             * ***************************************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isTop = RequestHelper.GetRequest("isTop").toInt();
            /****************************************************************************************************************************
             * 开始保存请求数据
             * ***************************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["UnionID"] = Rs["UnionID"].ToString();
            thisDictionary["UnionKey"] = Rs["UnionKey"].ToString();
            thisDictionary["UnionModel"] = UnionModel;
            thisDictionary["DeviceModel"] = DeviceModel;
            thisDictionary["strUnion"] = strUnion;
            thisDictionary["strThumb"] = strThumb;
            thisDictionary["strRemark"] = strRemark;
            thisDictionary["strXml"] = strXml;
            thisDictionary["SortID"] = SortID;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["isTop"] = isTop;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUnionChannel]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************************************************************************************
             * 开始保存请求数据信息
             * ***************************************************************************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前界面！");
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
            string strList = RequestHelper.GetRequest("UnionID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始删除请求数据
            * *********************************************************************************************/
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_UnionChannel", Params: " and UnionID in (" + strList + ") and isDisplay=0");
            if (sTab == null) { this.ErrorMessage("没有需要删除的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { this.ErrorMessage("没有需要删除的数据！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_UnionChannel", Params: " and UnionID in (" + strList + ") and isDisplay=0");
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
            string strList = RequestHelper.GetRequest("UnionID").ToString();
            if (string.IsNullOrEmpty(strList)) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { Response.Write("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_UnionChannel", Params: " and UnionID in (" + strList + ")");
            if (sTab == null) { Response.Write("没有需要处理的数据！"); Response.End(); }
            else if (sTab.Rows.Count <= 0) { Response.Write("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据信息
             * *********************************************************************************************/
            string SortID = RequestHelper.GetRequest("value").toInt();
            DbHelper.Connection.Update("Fooke_UnionChannel", dictionary: new Dictionary<string, string>() {
                {"SortID",SortID}
            }, Params: " and UnionID in (" + strList + ")");
            /***********************************************************************************************
             * 输出出局处理结果
             * *********************************************************************************************/
            Response.Write("排序成功！");
            Response.End();
        }
        /// <summary>
        /// 推荐数据
        /// </summary>
        protected void SaveRecommand()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("UnionID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_UnionChannel", Params: " and UnionID in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_UnionChannel", dictionary: new Dictionary<string, string>() {
                {"isTop",strValue}
            }, Params: " and UnionID in (" + strList + ")");
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 审核数据
        /// </summary>
        protected void SaveDisplay()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("UnionID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable sTab = DbHelper.Connection.FindTable("Fooke_UnionChannel", Params: " and UnionID in (" + strList + ")");
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
            DbHelper.Connection.Update("Fooke_UnionChannel", dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and UnionID in (" + strList + ")");
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}