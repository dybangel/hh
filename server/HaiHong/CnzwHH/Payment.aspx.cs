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
    public partial class Payment : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "display": this.VerificationRole("充值提现"); Display(); Response.End(); break;
                case "editsave": this.VerificationRole("充值提现"); SaveUpdate(); Response.End(); break;
                case "edit": this.VerificationRole("充值提现"); Update(); Response.End(); break;
                case "add": this.VerificationRole("充值提现"); this.Add(); Response.End(); break;
                case "save": this.VerificationRole("充值提现"); this.AddSave(); Response.End(); break;
                case "del": this.VerificationRole("充值提现"); this.Delete(); Response.End(); break;
                default: this.VerificationRole("充值提现"); this.List(); Response.End(); break;
            }
        }
        /// <summary>
        /// 关键词回复列表
        /// </summary>
        protected void List()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">用户充值 >> 充值记录</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"120\">支付平台</td>");
            strText.Append("<td width=\"120\">名称</td>");
            strText.Append("<td>商户ID</td>");
            strText.Append("<td width=\"60\">状态</td>");
            strText.Append("<td width=\"80\">默认</td>");
            strText.Append("<td width=\"60\">排序</td>");
            strText.Append("<td width=\"80\">操作选项</td>");
            strText.Append("</tr>");
            /*****************************************************************************************
             * 构建数据查询语句
             * ***************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "PaymentID,Model,PaymentName,BusinessID,isDisplay,SortID,isDefault";
            PageCenterConfig.Params = string.Empty;
            PageCenterConfig.Identify = "PaymentID";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "SortID desc,PaymentID desc";
            PageCenterConfig.Tablename = TableCenter.Payment;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Payment, string.Empty);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************
             * 构建网页内容信息
             * ***************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"PaymentID\" value=\"" + Rs["PaymentID"] + "\" /></td>");
                strText.Append("<td>"+Rs["Model"]+"</td>");
                strText.Append("<td>" + Rs["PaymentName"] + "</td>");
                strText.Append("<td>" + Rs["BusinessID"] + "</td>");
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&PaymentID=" + Rs["PaymentID"] + "\"><img src=\"template/images/ico/yes.gif\" /></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&PaymentID=" + Rs["PaymentID"] + "\"><img src=\"template/images/ico/no.gif\" /></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                if (Rs["isDefault"].ToString() == "1") { strText.Append("<a class=\"vbtn\">默认支付</a>"); }
                else { strText.Append("<a class=\"vbtnRed\">普通支付</a>"); }
                strText.Append("</td>");
                strText.Append("<td><input type=\"text\" value=\""+Rs["SortID"]+"\" size=\"6\" class=\"inputtext\" /></td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=edit&PaymentID=" + Rs["PaymentID"] + "\"><img src=\"images/ico/edit.png\"></a>");
                strText.Append("<a href=\"?action=del&PaymentID=" + Rs["PaymentID"] + "\" operate=\"delete\"><img src=\"images/ico/delete.png\"></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            /*****************************************************************************************
             * 构建网页分页内容
             * ***************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"审核(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"审核(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /*****************************************************************************************
             * 输出网页内容信息
             * ***************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/payment/default.html");
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
        /// 添加关键词回复
        /// </summary>
        protected void Add() {
            
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/payment/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "thumb",
                        tips = "请上传一个图标"
                    }, "0"); break;
                    case "isDefault": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Text="设为默认",Value="1",Name="isDefault",Checked="0"},
                        new CheckBoxMode(){Text="审核",Value="1",Name="isDisplay",Checked="1"}
                    }); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 添加关键词回复
        /// </summary>
        protected void Update()
        {
            /********************************************************************
             * 获取请求参数信息
             * ******************************************************************/
            string PaymentID = RequestHelper.GetRequest("PaymentID").toInt();
            if (PaymentID == "0") { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindPayment]", new Dictionary<string, object>() {
                {"PaymentID",PaymentID}
            });
            if (Rs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/payment/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "thumb",
                        tips = "请上传一个图标",
                        fileValue = Rs["thumb"].ToString()
                    }, "0"); break;
                    case "isDefault": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Text="设为默认",Value="1",Name="isDefault",Checked=Rs["isDefault"].ToString()},
                        new CheckBoxMode(){Text="审核",Value="1",Name="isDisplay",Checked=Rs["isDisplay"].ToString()}
                    }); break;
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /******************************************************************************
         * 数据处理区域
         * ****************************************************************************/
        /// <summary>
        /// 保存配置的菜单
        /// </summary>
        protected void AddSave()
        {
            /************************************************************************************************
             * 检查支付平台信息
             * **********************************************************************************************/
            string Model = RequestHelper.GetRequest("Model").toString();
            if (string.IsNullOrEmpty(Model)) { this.ErrorMessage("请选择要配置的支付平台!"); Response.End(); }
            if (Model.Length > 30) { this.ErrorMessage("支付平台名称长度请限制在30个汉字以内！"); Response.End(); }
            string PaymentName = RequestHelper.GetRequest("PaymentName").toString();
            if (string.IsNullOrEmpty(PaymentName)) { this.ErrorMessage("请填写充值支付平台名称！"); Response.End(); }
            if (PaymentName.Length > 30) { this.ErrorMessage("支付平台名称长度请限制在30个汉字以内!"); Response.End(); }
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindPayment]", new Dictionary<string, object>() {
                {"PaymentName",PaymentName}
            });
            if (oRs != null) { this.ErrorMessage("相同的支付平台名称已经存在,请检查！"); Response.End(); }
            /************************************************************************************************
             * 检查商户ID信息
             * **********************************************************************************************/
            string BusinessID = RequestHelper.GetRequest("BusinessID").toString();
            if (string.IsNullOrEmpty(BusinessID)) { this.ErrorMessage("请填写商户ID"); Response.End(); }
            if (BusinessID.Length > 50) { this.ErrorMessage("商户ID长度请限制在50个字符以内！"); Response.End(); }
            string BusinessKey = RequestHelper.GetRequest("BusinessKey").toString();
            if (BusinessKey.Length > 3000) { this.ErrorMessage("商户支付密钥长度请限制在3000个字符以内！"); Response.End(); }
            string Other = RequestHelper.GetRequest("Other").toString();
            if (Other.Length > 500) { this.ErrorMessage("其它参数配置长度请限制在200个字符以内！"); Response.End(); }
            string Remark = RequestHelper.GetRequest("remark").toString();
            if (Remark.Length > 1000) { this.ErrorMessage("备注说明信息长度请限制在1000个汉字以内！"); Response.End(); }
            /***************************************************************************
             * 获取上传图片信息
             * *************************************************************************/
            string thumb = RequestHelper.GetRequest("thumb").toString();
            if (thumb.Length > 150) { this.ErrorMessage("支付平台LOGO地址长度请限制在150个字符以内！"); Response.End(); }
            /***************************************************************************
             * 获取其他不需要验证的参数
             * *************************************************************************/
            string isDefault = RequestHelper.GetRequest("isDefault").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /***************************************************************************
             * 开始保存数据
             * *************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["PaymentID"] = "0";
            oDictionary["PaymentName"] = PaymentName;
            oDictionary["Model"] = Model;
            oDictionary["BusinessID"] = BusinessID;
            oDictionary["BusinessKey"] = BusinessKey;
            oDictionary["Other"] = Other;
            oDictionary["isDefault"] = isDefault;
            oDictionary["Remark"] = Remark;
            oDictionary["thumb"] = thumb;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["SortID"] = SortID;
            DbHelper.Connection.ExecuteProc("[Stored_SavePayment]", oDictionary);
            /********************************************************************
             * 开始输出数据结果
             * ******************************************************************/
            this.ConfirmMessage("数据保存成功，点击确定将继续停留在本页面！");
            Response.End();
        }

        protected void SaveUpdate()
        {
            string PaymentID = RequestHelper.GetRequest("PaymentID").toInt();
            if (PaymentID == "0") { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindPayment]", new Dictionary<string, object>() {
                {"PaymentID",PaymentID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /************************************************************************************************
             * 检查支付平台信息
             * **********************************************************************************************/
            string Model = RequestHelper.GetRequest("Model").toString();
            if (string.IsNullOrEmpty(Model)) { this.ErrorMessage("请选择要配置的支付平台!"); Response.End(); }
            if (Model.Length > 30) { this.ErrorMessage("支付平台名称长度请限制在30个汉字以内！"); Response.End(); }
            string PaymentName = RequestHelper.GetRequest("PaymentName").toString();
            if (string.IsNullOrEmpty(PaymentName)) { this.ErrorMessage("请填写充值支付平台名称！"); Response.End(); }
            if (PaymentName.Length > 30) { this.ErrorMessage("支付平台名称长度请限制在30个汉字以内!"); Response.End(); }
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindPayment]", new Dictionary<string, object>() {
                {"PaymentName",PaymentName}
            });
            if (oRs != null && oRs["PaymentID"].ToString()!=Rs["PaymentID"].ToString()) { this.ErrorMessage("相同的支付平台名称已经存在,请检查！"); Response.End(); }
            /************************************************************************************************
             * 检查商户ID信息
             * **********************************************************************************************/
            string BusinessID = RequestHelper.GetRequest("BusinessID").toString();
            if (string.IsNullOrEmpty(BusinessID)) { this.ErrorMessage("请填写商户ID"); Response.End(); }
            if (BusinessID.Length > 50) { this.ErrorMessage("商户ID长度请限制在50个字符以内！"); Response.End(); }
            string BusinessKey = RequestHelper.GetRequest("BusinessKey").toString();
            if (BusinessKey.Length > 3000) { this.ErrorMessage("商户支付密钥长度请限制在3000个字符以内！"); Response.End(); }
            string Other = RequestHelper.GetRequest("Other").toString();
            if (Other.Length > 500) { this.ErrorMessage("其它参数配置长度请限制在200个字符以内！"); Response.End(); }
            string Remark = RequestHelper.GetRequest("remark").toString();
            if (Remark.Length > 1000) { this.ErrorMessage("备注说明信息长度请限制在1000个汉字以内！"); Response.End(); }
            /***************************************************************************
             * 获取上传图片信息
             * *************************************************************************/
            string thumb = RequestHelper.GetRequest("thumb").toString();
            if (thumb.Length > 150) { this.ErrorMessage("支付平台LOGO地址长度请限制在150个字符以内！"); Response.End(); }
            /***************************************************************************
             * 获取其他不需要验证的参数
             * *************************************************************************/
            string isDefault = RequestHelper.GetRequest("isDefault").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            /***************************************************************************
             * 开始保存数据
             * *************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["PaymentID"] = Rs["PaymentID"].ToString();
            oDictionary["PaymentName"] = PaymentName;
            oDictionary["Model"] = Model;
            oDictionary["BusinessID"] = BusinessID;
            oDictionary["BusinessKey"] = BusinessKey;
            oDictionary["Other"] = Other;
            oDictionary["isDefault"] = isDefault;
            oDictionary["Remark"] = Remark;
            oDictionary["thumb"] = thumb;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["SortID"] = SortID;
            DbHelper.Connection.ExecuteProc("[Stored_SavePayment]", oDictionary);
            /********************************************************************
             * 开始输出数据结果
             * ******************************************************************/
            this.ConfirmMessage("数据保存成功，点击确定将继续停留在本页面！");
            Response.End();
        }

        /// <summary>
        /// 删除部门信息
        /// </summary>
        protected void Delete()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("PaymentID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始保存数据信息
            * *********************************************************************************************/
            DbHelper.Connection.Delete(TableCenter.Payment, Params: " and PaymentID in (" + strList + ")");
            /***********************************************************************************************
            * 返回数据处理结果
            * *********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 设置审核
        /// </summary>
        protected void Display()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("PaymentID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 获取保存参数值
            * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            if (strValue != "0" && strValue != "1") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            /***********************************************************************************************
            * 开始保存数据信息
            * *********************************************************************************************/
            DbHelper.Connection.Update(TableCenter.Payment, dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and PaymentID in (" + strList + ")");
            /***********************************************************************************************
            * 输出数据处理结果
            * *********************************************************************************************/
            this.History(); Response.End();
        }
    }
}