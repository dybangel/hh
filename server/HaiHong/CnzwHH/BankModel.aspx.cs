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
    public partial class BankModel : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "edit": this.VerificationRole("用户绑卡"); Update(); Response.End(); break;
                case "add": this.VerificationRole("用户绑卡"); Add(); Response.End(); break;
                case "addsave": this.VerificationRole("用户绑卡"); AddSave(); Response.End(); break;
                case "editsave": this.VerificationRole("用户绑卡"); UpdateSave(); Response.End(); break;
                case "default": this.VerificationRole("用户绑卡"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("用户绑卡"); Delete(); Response.End(); break;
                case "display": this.VerificationRole("用户绑卡"); SaveDisplay(); Response.End(); break;
                case "editor": this.VerificationRole("用户绑卡"); SaveEditor(); Response.End(); break;
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
            strText.Append("<td class=\"Base\" colspan=\"7\">银行管理 >> 银行列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="modelname",Text="搜名称"},
                new OptionMode(){Value="shortname",Text="搜代码"}
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
            strText.Append("<td width=\"120\">银行名称</td>");
            strText.Append("<td>银行代码</td>");
            strText.Append("<td width=\"100\">网银账户</td>");
            strText.Append("<td width=\"160\">显示排序</td>");
            strText.Append("<td width=\"160\">状态</td>");
            strText.Append("<td width=\"100\">操作选项</td>");
            strText.Append("</tr>");
            /******************************************************************************************
             * 构建分页查询条件
             * *****************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "modelname": strParameter += " and modelname like '%" + Keywords + "%'"; break;
                    case "shortname": strParameter += " and shortname like '%" + Keywords + "%'"; break;
                }
            }
            /******************************************************************************************
              * 构建分页查询语句
              * *****************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ModelID,isPattern,ModelName,ShortName,isDisplay,SortID,Remark";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "ModelID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "SortID desc,ModelID Desc";
            PageCenterConfig.Tablename = "Fooke_BankModel";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_BankModel", strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /******************************************************************************************
             * 遍历网页内容
             * *****************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"ModelID\" value=\"{0}\" /></td>", Rs["ModelID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["ModelName"]);
                strText.AppendFormat("<td>{0}</td>", Rs["ShortName"]);
                strText.AppendFormat("<td>{0}</td>", (Rs["isPattern"].ToString() == "1" ? "是" : "否"));
                strText.AppendFormat("<td><input type=\"text\" operate=\"edit\" isnumeric=\"true\" url=\"?action=editor&modelid={1}&value=$value\" size=\"5\" class=\"inputtext center\" value=\"{0}\" /></td>", Rs["SortID"], Rs["ModelID"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1")
                { strText.AppendFormat("<a href=\"?action=display&val=0&ModelID={0}\"><img src=\"images/ico/yes.gif\"/></a>", Rs["ModelID"]); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&ModelID={0}\"><img src=\"images/ico/no.gif\"/></a>", Rs["ModelID"]); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=edit&ModelID={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["ModelID"]);
                strText.AppendFormat("<a operate=\"delete\" href=\"?action=del&ModelID={0}\" title=\"删除\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["ModelID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"正常显示(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"正常显示(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /******************************************************************************************
             * 输出网页内容
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/bankModel/default.html");
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
        /// 添加银行卡帐号
        /// </summary>
        protected void Add()
        {
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/bankModel/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isPattern": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isPattern",Value="0",Text="普通银行卡"},
                        new RadioMode(){Name="isPattern",Value="1",Text="网银账户(支付宝等)"}
                    }, "0"); break;
                    case "isDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Value="1",Text="通过审核(是)"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="通过审核(否)"}
                    }, "1"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑银行卡帐号
        /// </summary>
        protected void Update()
        {
            string ModelID = RequestHelper.GetRequest("ModelID").toInt();
            if (ModelID == "0") { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBankModel]", new Dictionary<string, object>() {
                {"ModelID",ModelID}
            });
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/bankModel/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isPattern": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isPattern",Value="0",Text="普通银行卡"},
                        new RadioMode(){Name="isPattern",Value="1",Text="网银账户(支付宝等)"}
                    }, cRs["isPattern"].ToString()); break;
                    case "isDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isDisplay",Value="1",Text="通过审核(是)"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="通过审核(否)"}
                    }, cRs["isDisplay"].ToString()); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }


        protected void AddSave()
        {
            string isPattern = RequestHelper.GetRequest("isPattern").toInt();
            if (isPattern != "0" && isPattern != "1") { this.ErrorMessage("请求参数错误,请返回重试！"); Response.End(); }
            string ModelName = RequestHelper.GetRequest("ModelName").toString();
            if (string.IsNullOrEmpty(ModelName)) { this.ErrorMessage("请填写银行名称!"); Response.End(); }
            if (ModelName.Length > 30) { this.ErrorMessage("银行名称长度请限制在30个字符以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBankModel]", new Dictionary<string, object>() {
                {"ModelName",ModelName}
            });
            if (cRs != null) { this.ErrorMessage("银行名称已经存在,请换一个名字吧！"); Response.End(); }
            string ShortName = RequestHelper.GetRequest("ShortName").ToString();
            if (string.IsNullOrEmpty(ShortName)) { this.ErrorMessage("请填写银行简写代码！"); Response.End(); }
            if (ShortName.Length > 30) { this.ErrorMessage("银行简写代码长度请限制在30个字符以内！"); Response.End(); }
            DataRow vRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBankModel]", new Dictionary<string, object>() {
                {"ShortName",ShortName}
            });
            if (vRs != null) { this.ErrorMessage("银行名称已经存在,请换一个名字吧！"); Response.End(); }
            string Remark = RequestHelper.GetRequest("Remark").ToString();
            if (!string.IsNullOrEmpty(Remark) && Remark.Length > 200) { this.ErrorMessage("备注长度请限制在200个汉字以内！"); Response.End(); }
            /*****************************************************************************************************
             * 获取不需要验证的数据
             * ***************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /*************************************************************************************
            * 开始保存数据
            * ************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["ModelID"] = "0";
            oDictionary["isPattern"] = isPattern;
            oDictionary["ModelName"] = ModelName;
            oDictionary["ShortName"] = ShortName;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["SortID"] = SortID;
            oDictionary["Remark"] = Remark;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveBankModel]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
            try { new BankModelHelper().ApplicationClaer(); }
            catch { }
            /*********************************************************************
             * 返回处理结果
             * *******************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前页面！");
            Response.End();
        }

        /// <summary>
        /// 添加银行卡
        /// </summary>
        protected void UpdateSave()
        {
            string ModelID = RequestHelper.GetRequest("ModelID").toInt();
            if (ModelID == "0") { this.ErrorMessage("拉取用户信息失败,请刷新网页重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBankModel]", new Dictionary<string, object>() {
                {"ModelID",ModelID}
            });
            if (Rs == null) { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            /************************************************************************************
             * 开始验证数据完整性
             * **********************************************************************************/
            string isPattern = RequestHelper.GetRequest("isPattern").toInt();
            if (isPattern != "0" && isPattern != "1") { this.ErrorMessage("请求参数错误,请返回重试！"); Response.End(); }
            string ModelName = RequestHelper.GetRequest("ModelName").toString();
            if (string.IsNullOrEmpty(ModelName)) { this.ErrorMessage("请填写银行名称!"); Response.End(); }
            if (ModelName.Length > 30) { this.ErrorMessage("银行名称长度请限制在30个字符以内！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBankModel]", new Dictionary<string, object>() {
                {"ModelName",ModelName}
            });
            if (cRs != null && cRs["ModelID"].ToString()!=Rs["ModelID"].ToString()) { this.ErrorMessage("银行名称已经存在,请换一个名字吧！"); Response.End(); }
            string ShortName = RequestHelper.GetRequest("ShortName").ToString();
            if (string.IsNullOrEmpty(ShortName)) { this.ErrorMessage("请填写银行简写代码！"); Response.End(); }
            if (ShortName.Length > 30) { this.ErrorMessage("银行简写代码长度请限制在30个字符以内！"); Response.End(); }
            DataRow vRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBankModel]", new Dictionary<string, object>() {
                {"ShortName",ShortName}
            });
            if (vRs != null && vRs["ModelID"].ToString() != Rs["ModelID"].ToString()) { this.ErrorMessage("银行名称已经存在,请换一个名字吧！"); Response.End(); }
            string Remark = RequestHelper.GetRequest("Remark").ToString();
            if (!string.IsNullOrEmpty(Remark) && Remark.Length > 200) { this.ErrorMessage("备注长度请限制在200个汉字以内！"); Response.End(); }
            /*****************************************************************************************************
             * 获取不需要验证的数据
             * ***************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /*************************************************************************************
            * 开始保存数据
            * ************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["ModelID"] = Rs["ModelID"].ToString();
            oDictionary["isPattern"] = isPattern;
            oDictionary["ModelName"] = ModelName;
            oDictionary["ShortName"] = ShortName;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["SortID"] = SortID;
            oDictionary["Remark"] = Remark;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveBankModel]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
            try { new BankModelHelper().ApplicationClaer(); }
            catch { }
            /*********************************************************************
             * 返回处理结果
             * *******************************************************************/
            this.ConfirmMessage("数据保存成功,点击确定将继续停留在当前页面！");
            Response.End();
        }
        /// <summary>
        /// 删除银行卡信息
        /// </summary>
        protected void Delete()
        {
            string ModelID = RequestHelper.GetRequest("ModelID").toString();
            if (string.IsNullOrEmpty(ModelID)) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            DbHelper.Connection.Delete("Fooke_BankModel", Params: " and ModelID in (" + ModelID + ")");
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
            try { new BankModelHelper().ApplicationClaer(); }
            catch { }
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 保存显示审核状态信息
        /// </summary>
        protected void SaveDisplay()
        {
            string ModelID = RequestHelper.GetRequest("ModelID").ToString();
            if (string.IsNullOrEmpty(ModelID)) { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            string strValue = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> thisDictionary = new Dictionary<string, string>();
            thisDictionary["isDisplay"] = strValue;
            DbHelper.Connection.Update("Fooke_BankModel", thisDictionary, Params: " and ModelID in (" + ModelID + ")");
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
            try { new BankModelHelper().ApplicationClaer(); }
            catch { }
            /**********************************************************************************************
             * 输出返回结果
             * ********************************************************************************************/
            this.History();
            Response.End();
        }

        protected void SaveEditor()
        {
            string ModelID = RequestHelper.GetRequest("ModelID").toInt();
            if (ModelID == "0") { Response.Write("请求参数错误,请至少选择一条数据！"); Response.End(); }
            string isOrder = RequestHelper.GetRequest("value").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["SortID"] = isOrder;
            DbHelper.Connection.Update("Fooke_BankModel", dictionary, Params: " and ModelID=" + ModelID + "");
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
            try { new BankModelHelper().ApplicationClaer(); }
            catch { }
            /*********************************************************************
             * 开始输出数据
             * *******************************************************************/
            Response.Write("排序成功！"); Response.End();
        }
    }
}