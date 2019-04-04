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
    public partial class Columns : Fooke.Code.AdminHelper
    {
        protected string Tablename = RequestHelper.GetRequest("Tablename").toString();
        protected string chName = RequestHelper.GetRequest("chName").toString();
        protected string TableKey = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            /**********************************************************************
             * 验证必须要的数据参数
             * ********************************************************************/
            if (string.IsNullOrEmpty(Tablename)) { this.ErrorMessage("获取存储数据表失败,请重试！"); Response.End(); }
            else if (Tablename.Length <= 3) { this.ErrorMessage("存储数据表字段长度不能少于3个字符,请重试！"); Response.End(); }
            else if (Tablename.Length >= 32) { this.ErrorMessage("存储数据表字段长度不能大于20个字符,请重试！"); Response.End(); }
            if (string.IsNullOrEmpty(chName)) { this.ErrorMessage("获取数据表名称信息失败,请重试！"); Response.End(); }
            else if (chName.Length <= 1) { this.ErrorMessage("数据表名称不能少于1个汉字,请重试！"); Response.End(); }
            if (chName.Length > 16) { this.ErrorMessage("数据表名称不能大于16个汉字,请重试！"); Response.End(); }
            /***********************************************************************************************************
             * 验证数据表MDK是否合法
             * *********************************************************************************************************/
            TableKey = new Fooke.Function.String(Tablename.ToLower()).ToMD5().ToLower();
            if (string.IsNullOrEmpty(TableKey)) { this.ErrorMessage("请求参数错误，请返回页面重试！"); Response.End(); }
            else if (TableKey.Length != 32) { this.ErrorMessage("请求参数错误，请返回页面重试！"); Response.End(); }
            /***********************************************************************************************************
             * 执行页面级数据请求
             * *********************************************************************************************************/
            switch (this.strRequest)
            {
                case "save": this.VerificationRole("内容管理"); this.AddSave(); Response.End(); ; break;
                case "add": this.VerificationRole("内容管理"); this.Add(); Response.End(); break;
                case "del": this.VerificationRole("内容管理"); this.Delete(); Response.End(); break;
                case "edit": this.VerificationRole("内容管理"); this.Update(); Response.End(); break;
                case "editsave": this.VerificationRole("内容管理"); this.SaveUpdate(); Response.End(); break;
                case "display": this.VerificationRole("内容管理"); this.SaveDisplay(); Response.End(); break;
                default: this.VerificationRole("内容管理"); this.strDefault(); Response.End(); break;
            }
        }

        /// <summary>
        /// 管理员列表
        /// </summary>
        protected void strDefault()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"9\">字段管理 >> 字段列表</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<input type=\"hidden\" name=\"chName\" value=\"" + chName + "\" />");
            strText.Append("<input type=\"hidden\" name=\"Tablename\" value=\"" + Tablename + "\" />");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"10%\">字段名称</td>");
            strText.Append("<td width=\"10%\">字段中文</td>");
            strText.Append("<td width=\"10%\">所属系统</td>");
            strText.Append("<td width=\"10%\">字段类型</td>");
            strText.Append("<td>描述说明</td>");
            strText.Append("<td width=\"15%\">显示排序</td>");
            strText.Append("<td width=\"5%\">允许使用</td>");
            strText.Append("<td width=\"10%\">操作选项</td>");
            strText.Append("</tr>");
            string Params = "";
            if (!string.IsNullOrEmpty(Tablename)) { Params += " and Tablename='" + Tablename + "'"; }
            if (!string.IsNullOrEmpty(TableKey)) { Params += " and TableKey='" + TableKey + "'"; }
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ColumnsID,strKey,Tablename,TableKey,chName,ColumnsName,ColumnsType,ColumnsText,ColumnsTips,isDisplay,SortID";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "ColumnsID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID Desc,ColumnsID Desc";
            PageCenterConfig.Tablename = TableCenter.Columns;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Columns, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"ColumnsID\" value=\"" + Rs["ColumnsID"] + "\" /></td>");
                strText.Append("<td style=\"color:#f00\">" + Rs["ColumnsName"] + "</td>");
                strText.Append("<td>" + Rs["ColumnsText"] + "</td>");
                strText.Append("<td>" + Rs["chName"] + "</td>");
                strText.Append("<td style=\"color:#f00\">" + Rs["ColumnsType"] + "</td>");
                strText.Append("<td>" + Rs["ColumnsTips"] + "</td>");
                strText.Append("<td operate=\"sortid\" url=\"?action=stor&Tablename=" + Tablename + "&chName=" + chName + "&text={text}\">" + Rs["SortID"] + "</td>");
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&Tablename=" + Tablename + "&chName=" + chName + "&val=0&ColumnsID=" + Rs["ColumnsID"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=display&Tablename=" + Tablename + "&chName=" + chName + "&val=1&ColumnsID=" + Rs["ColumnsID"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=edit&Tablename=" + Tablename + "&chName=" + chName + "&ColumnsID=" + Rs["ColumnsID"] + "\" title=\"修改字段属性\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=del&Tablename=" + Tablename + "&chName=" + chName + "&ColumnsID=" + Rs["ColumnsID"] + "\" title=\"删除字段\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");

            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除选中数据\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"允许使用(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"允许使用(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");

            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/columns/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "tableName": strValue = Tablename; break;
                    case "chName": strValue = chName; break;
                    case "list": strValue = strText.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        protected void Add()
        {
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/columns/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "tableName": strValue = Tablename; break;
                    case "chName": strValue = chName; break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Text="开启",Value="1"},
                        new RadioMode(){Name="isDisplay",Text="关闭",Value="0"}
                    }, "1"); break;
                    case "isnull": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isNotNull",Text="允许空值",Value="1"},
                        new RadioMode(){Name="isNotNull",Text="不允许空值",Value="0"}
                    }, "1"); break;
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
            string ColumnsID = RequestHelper.GetRequest("ColumnsID").toInt();
            if (ColumnsID == "0") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Columns, Params: " and ColumnsID = " + ColumnsID + "");
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }

            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/columns/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "tableName": strValue = Tablename; break;
                    case "chName": strValue = chName; break;
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Text="开启",Value="1"},
                        new RadioMode(){Name="isDisplay",Text="关闭",Value="0"}
                    }, cRs["isDisplay"].ToString()); break;
                    case "isnull":
                        strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isNotNull",Text="允许空值",Value="1"},
                        new RadioMode(){Name="isNotNull",Text="不允许空值",Value="0"}
                    }, cRs["isNotNull"].ToString()); break;
                    case "columnstype":
                        strValue = FunctionBase.OptionList(new List<OptionMode>()
                        {
                            new OptionMode(){Value="int",Text="数字型(int)"},
                            new OptionMode(){Value="float",Text="小数型(float)"},
                            new OptionMode(){Value="datetime",Text="日期(datetime)"},
                            new OptionMode(){Value="nvarchar",Text="普通文本(nvarchar)"},
                            new OptionMode(){Value="varchar[max]",Text="长文本(nvarchar[max])"},
                            new OptionMode(){Value="checkbox",Text="复选按钮(checkbox)"},
                            new OptionMode(){Value="options",Text="下拉菜单(select)"},
                            new OptionMode(){Value="radio",Text="单选按钮(radio)"},
                            new OptionMode(){Value="file",Text="文件上传(file)"},
                            new OptionMode(){Value="ntext",Text="文本域(编辑器)"},
                        }, cRs["ColumnsType"].ToString()); break;
                    case "disOptions":
                        if (cRs["ColumnsType"].ToString() != "options" && cRs["ColumnsType"].ToString() != "checkbox" && cRs["ColumnsType"].ToString() != "radio")
                        { strValue = "style=\"display:none\""; }
                        break;
                    case "options": this.FindOptions(cRs["Options"].ToString(), (strText) => { strValue = strText; }); break;
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

        protected void FindOptions(string strOptions, Action<string> Fun)
        {
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                DataTable Tab = DbHelper.Connection.ExecuteFindTable("Stored_FindColumnsOptions", new Dictionary<string, object>() {
                    {"strXml",strOptions}
                });
                foreach (DataRow cRs in Tab.Rows)
                {
                    strBuilder.AppendFormat("<tr class=\"hback\">");
                    strBuilder.AppendFormat("<td><input type=\"text\" value=\"{0}\" style=\"width:200px\" placeholder=\"请填写选项值\" name=\"opValue\" class=\"inputtext\" /></td>", cRs["strValue"].ToString());
                    strBuilder.AppendFormat("<td><input type=\"text\" value=\"{0}\" style=\"width:200px\" placeholder=\"请填写选项值\" name=\"opText\" class=\"inputtext\" /></td>", cRs["strText"].ToString());
                    strBuilder.AppendFormat("<td><img src=\"template/images/ico/jian.png\" operate=\"remove\" /></td>");
                    strBuilder.AppendFormat("</tr>");
                }
            }
            catch { }
            /*************************************************************************
             * 开始执行返回回调函数
             * ***********************************************************************/
            try
            {
                if (!string.IsNullOrEmpty(strBuilder.ToString()) && Fun != null)
                {
                    Fun(strBuilder.ToString());
                }
            }
            catch { }
        }

        /**************************************************************************
         * 数据处理区域
         * ************************************************************************/
        /// <summary>
        /// 保存添加的用户字段
        /// </summary>
        protected void AddSave()
        {
            /***********************************************************************************************
             * 获取字段类型信息
             * *********************************************************************************************/
            string ColumnsType = RequestHelper.GetRequest("ColumnsType").toString();
            if (string.IsNullOrEmpty(ColumnsType)) { this.ErrorMessage("请选择字段类型！"); Response.End(); }
            else if (ColumnsType.Length <= 2) { this.ErrorMessage("字段类型长度不能少于2个字符,请重试！"); Response.End(); }
            else if (ColumnsType.Length >= 16) { this.ErrorMessage("字段类型长度不能大于16个字符,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 验证字段名称的合法性
             * *********************************************************************************************/
            string ColumnsName = RequestHelper.GetRequest("ColumnsName").toString();
            if (string.IsNullOrEmpty(ColumnsName)) { this.ErrorMessage("请输入字段名称！"); Response.End(); }
            else if (ColumnsName.Length <= 2) { this.ErrorMessage("自定义字段名称长度至少三个字符！"); Response.End(); }
            else if (ColumnsName.Length > 20) { this.ErrorMessage("自定义字段长度请保持在20个字符以内！"); Response.End(); }
            else if (VerifyCenter.VerifyChina(ColumnsName)) { this.ErrorMessage("自定义字段名称不允许出现中文字符！"); Response.End(); }
            else if (!ColumnsName.ToLower().Contains("Define_")) { ColumnsName = "Define_" + ColumnsName; }
            /***********************************************************************************************
             * 检查新建字段名称是否存在
             * *********************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindColumns]", new Dictionary<string, object>() {
                {"Tablename",Tablename},
                {"ColumnsName",ColumnsName}
            });
            if (cRs != null) { this.ErrorMessage("相同的字段名称已经存在，请另外选择一个名字吧！"); Response.End(); }
            /***********************************************************************************************
             * 数据整合，检查字段名称是否重复
             * *********************************************************************************************/
            DataTable cTab = DbHelper.Connection.FindTable(Tablename, columns: "*", Params: " and 1=0");
            if (cTab == null) { this.ErrorMessage("发生未知错误,请返回重试！"); Response.End(); }
            foreach (DataColumn oColumns in cTab.Columns)
            {
                if (oColumns.ColumnName.ToLower() == ColumnsName.ToLower())
                { this.ErrorMessage("该数据字段已经存在了!请另外选择一个名称吧！"); Response.End(); break; }
            }
            /***********************************************************************************************
             * 验证字段中文名称
             * *********************************************************************************************/
            string ColumnsText = RequestHelper.GetRequest("ColumnsText").toString();
            if (string.IsNullOrEmpty(ColumnsText)) { this.ErrorMessage("请输入字段的中文名称！"); Response.End(); }
            else if (ColumnsText.Length <= 1) { this.ErrorMessage("字段的中文名称不能小于1个汉字！"); Response.End(); }
            else if (ColumnsText.Length > 20) { this.ErrorMessage("字段的中文名称不允许超过20个字符！"); Response.End(); }
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindColumns]", new Dictionary<string, object>() {
                {"Tablename",Tablename},
                {"ColumnsText",ColumnsText}
            });
            if (sRs != null) { this.ErrorMessage("相同的字段中文名称已经存在,请另外取名字吧"); Response.End(); }
            /***********************************************************************************************
             * 验证字段说明,默认值信息等
             * *********************************************************************************************/
            string ColumnsTips = RequestHelper.GetRequest("ColumnsTips").toString();
            if (!string.IsNullOrEmpty(ColumnsTips) && ColumnsTips.Length <= 1) { this.ErrorMessage("字段说明文字内容不能少于1个汉字!"); Response.End(); }
            else if (!string.IsNullOrEmpty(ColumnsTips) && ColumnsTips.Length >= 80) { this.ErrorMessage("字段描述信息请保持在80个字符以内！"); Response.End(); }
            string defaultTxt = RequestHelper.GetRequest("defaultTxt").toString();
            if (string.IsNullOrEmpty(defaultTxt) && defaultTxt.Length >= 300)
            { this.ErrorMessage("自定义字段的默认长度不允许超过300个字符！"); Response.End(); }
            /*************************************************************************
             * 数据整合，验证字段类型与选项
             * ***********************************************************************/
            StringBuilder Options = new StringBuilder();
            Options.Append("<configurationRoot>");
            if (ColumnsType == "options" || ColumnsType == "radio" || ColumnsType == "checkbox")
            {
                string opValue = RequestHelper.GetRequest("opValue").ToString();
                if (string.IsNullOrEmpty(opValue)) { this.ErrorMessage("选项值不能为空！"); Response.End(); }
                else if (opValue.Length >= 10000) { this.ErrorMessage("选项值总长度不能超过10000个字符！"); Response.End(); }
                string opText = RequestHelper.GetRequest("opText").ToString();
                if (string.IsNullOrEmpty(opText)) { this.ErrorMessage("选项文本不能为空！"); Response.End(); }
                else if (opText.Length >= 10000) { this.ErrorMessage("选项文本总长度不能超过10000个字符！"); Response.End(); }
                string[] valueTemp = opValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (valueTemp.Length <= 0) { this.ErrorMessage("选项值不能为空！"); Response.End(); }
                else if (valueTemp.Length >= 100) { this.ErrorMessage("选项值总个数不能超过100个！"); Response.End(); }
                foreach (string strChar in valueTemp)
                {
                    if (strChar.Length >= 60) { this.ErrorMessage("单个选项值长度不能超过60个字符！"); Response.End(); }
                }
                string[] textTemp = opText.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (textTemp.Length <= 0) { this.ErrorMessage("选项文本不能为空！"); Response.End(); }
                else if (valueTemp.Length != textTemp.Length) { this.ErrorMessage("选项值与选项文本个数不匹配！"); Response.End(); }
                foreach (string strChar in textTemp)
                {
                    if (strChar.Length >= 60) { this.ErrorMessage("单个选项文本长度不能超过60个字符！"); Response.End(); }
                }
                /************************************************************************************
                 * 构建选项信息
                 * **********************************************************************************/
                for (int k = 0; k < textTemp.Length; k++)
                {
                    Options.AppendFormat("<item value=\"{0}\" text=\"{1}\" />", valueTemp[k], textTemp[k]);
                }
            }
            Options.Append("</configurationRoot>");
            /**************************************************************************************************************
             * 验证选项信息
             * ************************************************************************************************************/
            if (ColumnsType == "options" && !Options.ToString().Contains("<item")) { this.ErrorMessage("请输入下拉菜单的选项！"); Response.End(); }
            else if (ColumnsType == "radio" && !Options.ToString().Contains("<item")) { this.ErrorMessage("请输入下拉菜单的选项！"); Response.End(); }
            else if (ColumnsType == "checkbox" && !Options.ToString().Contains("<item")) { this.ErrorMessage("请输入下拉菜单的选项！"); Response.End(); }
            /**************************************************************************************************************
             * 获取其它数据验证消息
             * ************************************************************************************************************/
            string ExtendCode = RequestHelper.GetRequest("ExtendCode", false).toString();
            if (ExtendCode.Length >= 300) { this.ErrorMessage("拓展代码的长度不允许超过300个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 验证字段长度信息
             * ************************************************************************************************************/
            string MinLength = RequestHelper.GetRequest("MinLength").toInt();
            if (MinLength.cInt() < 0) { this.ErrorMessage("最小字段长度不能小于0！"); Response.End(); }
            else if (MinLength.cInt() >= 20) { this.ErrorMessage("最小字段长度不能大于0！"); Response.End(); }
            string MaxLength = RequestHelper.GetRequest("MaxLength").toInt();
            if (MinLength.cInt() < 0) { this.ErrorMessage("最小字段长度不能小于0！"); Response.End(); }
            else if (MinLength.cInt() >= 20) { this.ErrorMessage("最小字段长度不能大于0！"); Response.End(); }
            if (Convert.ToInt32(MaxLength) <= 0) { MaxLength = "255"; }
            /**************************************************************************************************************
             * 无需验证的数据信息
             * ************************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isNotNull = RequestHelper.GetRequest("isNotNull").toInt();
            /**************************************************************************************************
             * 生成自定义字段标识
             * *************************************************************************************************/
            string strKey = string.Format("自定义字段-|-|-{0}-|-|-{1}-|-|-{2}-|-|-自定义字段",
                Tablename, ColumnsName, ColumnsText);
            strKey = new Fooke.Function.String(strKey).ToMD5().ToLower();
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindColumns]", new Dictionary<string, object>() {
                {"strKey",strKey}
            });
            if (oRs != null) { this.ErrorMessage("相同的字段数据已经存在，请另外选择一个名字吧！"); Response.End(); }
            /*************************************************************************************************
             * 开始保存数据
             * ***********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["ColumnsID"] = "0";
            thisDictionary["strKey"] = strKey;
            thisDictionary["Tablename"] = Tablename;
            thisDictionary["TableKey"] = TableKey;
            thisDictionary["chName"] = chName;
            thisDictionary["ColumnsName"] = ColumnsName;
            thisDictionary["ColumnsType"] = ColumnsType;
            thisDictionary["ColumnsText"] = ColumnsText;
            thisDictionary["ColumnsTips"] = ColumnsTips;
            thisDictionary["Options"] = Options.ToString();
            thisDictionary["isNotNull"] = isNotNull;
            thisDictionary["MaxLength"] = MaxLength;
            thisDictionary["MinLength"] = MinLength;
            thisDictionary["defaultTxt"] = defaultTxt;
            thisDictionary["ExtendCode"] = ExtendCode;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveColumns]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); }
            /**********************************************************************
             * 清空服务器缓存
             * ********************************************************************/
            try { new ColumnsHelper().ApplicationClaer(TableKey); }
            catch { }
            /**********************************************************************
             * 开始输出数据
             * ********************************************************************/
            this.ConfirmMessage("自定义字段添加成功，点击确定将继续停留在当前页面！", falseUrl: "?action=default&Tablename=" + Tablename + "&chName=" + chName + "");
            Response.End();
        }
        /// <summary>
        /// 保存编辑
        /// </summary>
        protected void SaveUpdate()
        {
            /***********************************************************************************************
             * 验证字段名称信息
             * *********************************************************************************************/
            string ColumnsID = RequestHelper.GetRequest("ColumnsID").toInt();
            if (ColumnsID == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("Stored_FindColumns", new Dictionary<string, object>() {
                {"ColumnsID",ColumnsID}
            });
            if (Rs == null) { this.ErrorMessage("对不起，你查找的数据不存在，请返回重试！"); Response.End(); }
            /***********************************************************************************************
             * 验证字段中文名称
             * *********************************************************************************************/
            string ColumnsText = RequestHelper.GetRequest("ColumnsText").toString();
            if (string.IsNullOrEmpty(ColumnsText)) { this.ErrorMessage("请输入字段的中文名称！"); Response.End(); }
            else if (ColumnsText.Length <= 1) { this.ErrorMessage("字段的中文名称不能小于1个汉字！"); Response.End(); }
            else if (ColumnsText.Length > 20) { this.ErrorMessage("字段的中文名称不允许超过20个字符！"); Response.End(); }
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindColumns]", new Dictionary<string, object>() {
                {"Tablename",Tablename},
                {"ColumnsText",ColumnsText}
            });
            if (sRs != null && sRs["ColumnsID"].ToString() != Rs["ColumnsID"].ToString()) { this.ErrorMessage("相同的字段中文名称已经存在,请另外取名字吧"); Response.End(); }
            /***********************************************************************************************
             * 验证字段说明,默认值信息等
             * *********************************************************************************************/
            string ColumnsTips = RequestHelper.GetRequest("ColumnsTips").toString();
            if (!string.IsNullOrEmpty(ColumnsTips) && ColumnsTips.Length <= 1) { this.ErrorMessage("字段说明文字内容不能少于1个汉字!"); Response.End(); }
            else if (!string.IsNullOrEmpty(ColumnsTips) && ColumnsTips.Length >= 80) { this.ErrorMessage("字段描述信息请保持在80个字符以内！"); Response.End(); }
            string defaultTxt = RequestHelper.GetRequest("defaultTxt").toString();
            if (string.IsNullOrEmpty(defaultTxt) && defaultTxt.Length >= 300)
            { this.ErrorMessage("自定义字段的默认长度不允许超过300个字符！"); Response.End(); }
            /*************************************************************************
             * 数据整合，验证字段类型与选项
             * ***********************************************************************/
            StringBuilder Options = new StringBuilder();
            Options.Append("<configurationRoot>");
            if (Rs["ColumnsType"].ToString() == "options" || Rs["ColumnsType"].ToString() == "radio" || Rs["ColumnsType"].ToString() == "checkbox")
            {
                string opValue = RequestHelper.GetRequest("opValue").ToString();
                if (string.IsNullOrEmpty(opValue)) { this.ErrorMessage("选项值不能为空！"); Response.End(); }
                else if (opValue.Length >= 10000) { this.ErrorMessage("选项值总长度不能超过10000个字符！"); Response.End(); }
                string opText = RequestHelper.GetRequest("opText").ToString();
                if (string.IsNullOrEmpty(opText)) { this.ErrorMessage("选项文本不能为空！"); Response.End(); }
                else if (opText.Length >= 10000) { this.ErrorMessage("选项文本总长度不能超过10000个字符！"); Response.End(); }
                string[] valueTemp = opValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (valueTemp.Length <= 0) { this.ErrorMessage("选项值不能为空！"); Response.End(); }
                else if (valueTemp.Length >= 100) { this.ErrorMessage("选项值总个数不能超过100个！"); Response.End(); }
                foreach (string strChar in valueTemp)
                {
                    if (strChar.Length >= 60) { this.ErrorMessage("单个选项值长度不能超过60个字符！"); Response.End(); }
                }
                string[] textTemp = opText.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (textTemp.Length <= 0) { this.ErrorMessage("选项文本不能为空！"); Response.End(); }
                else if (valueTemp.Length != textTemp.Length) { this.ErrorMessage("选项值与选项文本个数不匹配！"); Response.End(); }
                foreach (string strChar in textTemp)
                {
                    if (strChar.Length >= 60) { this.ErrorMessage("单个选项文本长度不能超过60个字符！"); Response.End(); }
                }
                /************************************************************************************
                 * 构建选项信息
                 * **********************************************************************************/
                for (int k = 0; k < textTemp.Length; k++)
                {
                    Options.AppendFormat("<item value=\"{0}\" text=\"{1}\" />", valueTemp[k], textTemp[k]);
                }
            }
            Options.Append("</configurationRoot>");
            /**************************************************************************************************************
             * 验证选项信息
             * ************************************************************************************************************/
            if (Rs["ColumnsType"].ToString() == "options" && !Options.ToString().Contains("<item")) { this.ErrorMessage("请输入下拉菜单的选项！"); Response.End(); }
            else if (Rs["ColumnsType"].ToString() == "radio" && !Options.ToString().Contains("<item")) { this.ErrorMessage("请输入下拉菜单的选项！"); Response.End(); }
            else if (Rs["ColumnsType"].ToString() == "checkbox" && !Options.ToString().Contains("<item")) { this.ErrorMessage("请输入下拉菜单的选项！"); Response.End(); }
            /**************************************************************************************************************
             * 获取其它数据验证消息
             * ************************************************************************************************************/
            string ExtendCode = RequestHelper.GetRequest("ExtendCode", false).toString();
            if (ExtendCode.Length >= 300) { this.ErrorMessage("拓展代码的长度不允许超过300个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 验证字段长度信息
             * ************************************************************************************************************/
            string MinLength = RequestHelper.GetRequest("MinLength").toInt();
            if (MinLength.cInt() < 0) { this.ErrorMessage("最小字段长度不能小于0！"); Response.End(); }
            else if (MinLength.cInt() >= 20) { this.ErrorMessage("最小字段长度不能大于0！"); Response.End(); }
            string MaxLength = RequestHelper.GetRequest("MaxLength").toInt();
            if (MinLength.cInt() < 0) { this.ErrorMessage("最小字段长度不能小于0！"); Response.End(); }
            else if (MinLength.cInt() >= 20) { this.ErrorMessage("最小字段长度不能大于0！"); Response.End(); }
            if (Convert.ToInt32(MaxLength) <= 0) { MaxLength = "255"; }
            /**************************************************************************************************************
             * 无需验证的数据信息
             * ************************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isNotNull = RequestHelper.GetRequest("isNotNull").toInt();
            /*************************************************************************************************
             * 开始保存数据
             * ***********************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["ColumnsID"] = Rs["ColumnsID"].ToString();
            thisDictionary["strKey"] = Rs["strKey"].ToString();
            thisDictionary["Tablename"] = Rs["Tablename"].ToString();
            thisDictionary["TableKey"] = Rs["TableKey"].ToString();
            thisDictionary["chName"] = Rs["chName"].ToString();
            thisDictionary["ColumnsName"] = Rs["ColumnsName"].ToString();
            thisDictionary["ColumnsType"] = Rs["ColumnsType"].ToString();
            thisDictionary["ColumnsText"] = ColumnsText;
            thisDictionary["ColumnsTips"] = ColumnsTips;
            thisDictionary["Options"] = Options.ToString();
            thisDictionary["isNotNull"] = isNotNull;
            thisDictionary["MaxLength"] = MaxLength;
            thisDictionary["MinLength"] = MinLength;
            thisDictionary["defaultTxt"] = defaultTxt;
            thisDictionary["ExtendCode"] = ExtendCode;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveColumns]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); }
            /**********************************************************************
             * 清空服务器缓存
             * ********************************************************************/
            try { new ColumnsHelper().ApplicationClaer(TableKey); }
            catch { }
            /**********************************************************************
             * 开始输出数据
             * ********************************************************************/
            this.ConfirmMessage("自定义字段添加成功，点击确定将继续停留在当前页面！", falseUrl: "?action=default&Tablename=" + Tablename + "&chName=" + chName + "");
            Response.End();
        }

        /// <summary>
        /// 删除字段
        /// </summary>
        protected void Delete()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("ColumnsID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
             * 开始查询请求数据信息
             * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Columns, Params: " and ColumnsID in (" + strList + ")");
            foreach (DataRow Rs in Tab.Rows)
            {
                DbHelper.Connection.ExecuteProc("[Stored_DeleteColumns]", new Dictionary<string, object>() {
                    {"Tablename",Rs["Tablename"].ToString()},
                    {"ColumnsName",Rs["ColumnsName"].ToString()},
                    {"ColumnsID",Rs["ColumnsID"].ToString()},
                });
            }
            /**********************************************************************
             * 清空服务器缓存
             * ********************************************************************/
            try { new ColumnsHelper().ApplicationClaer(TableKey); }
            catch { }
            /**********************************************************************
             * 开始输出数据
             * ********************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 审核字段
        /// </summary>
        protected void SaveDisplay()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("ColumnsID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
             * 开始验证请求数据系想你
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            if (strValue != "0" && strValue != "1")
            { this.ErrorMessage("获取数据参数错误,请重试！"); Response.End(); }
            /***********************************************************************************************
             * 开始保存数据
             * *********************************************************************************************/
            DbHelper.Connection.Update(TableCenter.Columns, dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and ColumnsID in (" + strList + ")");
            /**********************************************************************
             * 清空服务器缓存
             * ********************************************************************/
            try { new ColumnsHelper().ApplicationClaer(TableKey); }
            catch { }
            /**********************************************************************
             * 开始输出数据
             * ********************************************************************/
            this.History();
            Response.End();
        }
    }
}