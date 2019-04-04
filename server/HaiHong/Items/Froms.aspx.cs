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
using Fooke.SimpleMaster;
namespace Fooke.Web
{
    public partial class Froms : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "start": this.SaveFroms(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 输出表单内容信息
        /// </summary>
        protected void strDefault()
        {
            /**********************************************************************************
             * 获取表单数据信息
             * *********************************************************************************/
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误！请传入表单ID"); Response.End(); }
            DataRow fromsRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsId",fromsId}
            });
            if (fromsRs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            if (fromsRs["isDisplay"].ToString() != "1") { this.ErrorMessage("表单已经停止使用，请联系管理员！"); Response.End(); }
            /**********************************************************************************
             * 解析表单模板信息
             * *********************************************************************************/
            string cTemplate = fromsRs["Template"].ToString();
            if (string.IsNullOrEmpty(cTemplate)) { cTemplate = "{@dir}/froms.html"; }
            /**********************************************************************************
            * 模板路径地址
            * *********************************************************************************/
            string directoryPath = this.GetParameter("TemplateDir", "siteXML").toString();
            if (string.IsNullOrEmpty(directoryPath)) { directoryPath = "template"; }
            directoryPath = Win.ApplicationPath + "/" + directoryPath;
            /**********************************************************************************
            * 处理模板信息
            * *********************************************************************************/
            cTemplate = cTemplate.Replace("{@dir}", directoryPath);
            /***********************************************************************
             * 解析标签网页内容
             * **********************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader(cTemplate);
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = fromsRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), true);
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 保存表单
        /// </summary>
        protected void SaveFroms()
        {
            /**********************************************************************************
             * 获取表单数据信息
             * *********************************************************************************/
            string fromsId = RequestHelper.GetRequest("fromsId").toInt();
            if (fromsId == "0") { this.ErrorMessage("请求参数错误！请传入表单ID"); Response.End(); }
            DataRow fromsRs = DbHelper.Connection.ExecuteFindRow("Stored_FindForms", new Dictionary<string, object>() {
                {"fromsId",fromsId}
            });
            if (fromsRs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            if (fromsRs["isDisplay"].ToString() != "1") { this.ErrorMessage("表单已经停止使用，请联系管理员！"); Response.End(); }
            /**************************************************************************************
             * 获取数据是否为非法提交
             * ************************************************************************************/
            string Tokey = string.Format("-|-|-在线表单-|-|-{0}-|-|-{1}", fromsRs["fromsId"], fromsRs["fromsName"]).md5().ToLower();
            string thisKey = RequestHelper.GetRequest("tokey").ToString();
            if (string.IsNullOrEmpty(thisKey)) { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            if (thisKey.ToLower() != Tokey.ToLower()) { this.ErrorMessage("请求参数错误,数据可能为非法提交"); Response.End(); }
            /****************************************************************************************
             * 验证主题内容
             * ***************************************************************************************/
            string title = RequestHelper.GetRequest("title").toString();
            if (string.IsNullOrEmpty(title)) { this.ErrorMessage("请输入表单主题！"); Response.End(); }
            if (title.Length > 50) { this.ErrorMessage("表单主题字数请限制在50个汉字以内！"); Response.End(); }
            /****************************************************************************************
             * 检查有效期
             * **************************************************************************************/
            if (!string.IsNullOrEmpty(fromsRs["isDate"].ToString()) && fromsRs["isDate"].ToString() == "1")
            {
                DateTime StarDate = new Fooke.Function.String(fromsRs["StarDate"].ToString()).cDate();
                if (StarDate > DateTime.Now) { this.ErrorMessage("当前模块开启了日期限制！当前时间还未到设定日期!"); }
                DateTime EndDate = new Fooke.Function.String(fromsRs["EndDate"].ToString()).cDate();
                if (EndDate < DateTime.Now) { this.ErrorMessage("当前模块开启了日期限制！当前时间已超过设定日期!"); }
            }
            /**************************************************************************************
            * 解析表单Xml格式内容信息
            * ************************************************************************************/
            ConfigurationReader xReader = new ConfigurationReader(fromsRs["strXML"].ToString());
            /***************************************************************************
             * 检查验证码
             * *************************************************************************/
            if (xReader.GetParameter("isCode").toInt() == "1")
            {
                string codeText = RequestHelper.GetRequest("codeText").toString();
                if (string.IsNullOrEmpty(codeText)) { this.ErrorMessage("请输入验证码！"); Response.End(); }
                string sessionCode = SessionHelper.Get("vCode").toString();
                if (!string.IsNullOrEmpty(sessionCode) && sessionCode.ToLower() != codeText.ToLower()) { this.ErrorMessage("验证码错误！"); Response.End(); }
            }
            /***************************************************************************
             * 获取客户端提交IP地址
             * *************************************************************************/
            string strip = FunctionCenter.GetCustomerIP();
            /***************************************************************************
             * 检查表单的重复性
             * *************************************************************************/
            try
            {
                int Times = xReader.GetParameter("Times").cInt();
                if (Times != 0)
                {
                    DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefineForms", new Dictionary<string, object>() {
                        {"Tablename",fromsRs["Tablename"]},
                        {"Addtime",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")},
                        {"strIP",strip},
                        {"clnText","count(0) as number"}
                    });
                    if (cRs == null) { this.ErrorMessage("发生未知错误，请返回重试！"); Response.End(); }
                    int cNumber = new Fooke.Function.String(cRs[0].ToString()).cInt();
                    if (cNumber >= Times) { this.ErrorMessage("今日提交次数达到上线,请明日再来吧！"); Response.End(); }
                }
            }
            catch { }
            /************************************************************************
             * 检查自定义字段的合法性
             * ***********************************************************************/
            string frmDefine = RequestHelper.GetRequest("frmDefine").ToString();
            try
            {
                if (!string.IsNullOrEmpty(frmDefine) && frmDefine.Length >= 5)
                {
                    new ColumnsHelper().Verification(fromsRs["TableName"].ToString(), (errMessage) =>
                    {
                        this.ErrorMessage(errMessage); Response.End();
                    });
                }
            }
            catch { }
            /************************************************************************
             * 验证今日表单提交次数是否超过上线
             * ***********************************************************************/
            try
            {
                DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefineForms", new Dictionary<string, object>() {
                        {"Tablename",fromsRs["Tablename"]},
                        {"Addtime",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")},
                        {"clnText","count(0) as number"}
                });
                if (cRs == null) { this.ErrorMessage("发生未知错误，请返回重试！"); Response.End(); }
                int cNumber = new Fooke.Function.String(cRs[0].ToString()).cInt();
                if (cNumber >= 4) { this.ErrorMessage("今日提交次数达到上线,请明日再来吧！"); Response.End(); }
            }
            catch { }
            /***************************************************************************
             * 检查数据的重复性
             * *************************************************************************/
            string strKey = fromsId + "-|-|-" + title + "-|-|-" + strip + "-|-|-" + DateTime.Now.ToString("yyyyMMdd");
            strKey = new Fooke.Function.String(strKey).ToMD5().ToLower();
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("Stored_FindDefineForms", new Dictionary<string, object>() {
                {"Tablename",fromsRs["Tablename"]},
                {"strKey",strKey}
            });
            if (oRs != null) { this.ErrorMessage("数据已经存在,请勿重复提交！"); Response.End(); }
            /******************************************************************************
             * 获取无需验证的数据信息
             * ****************************************************************************/
            string isValid = xReader.GetParameter("isAuto").toInt();
            string strXML = RequestHelper.GetPrametersXML(false);
            /***************************************************************************
             * 开始保存数据信息
             * *************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["formsId"] = fromsId;
            oDictionary["Tablename"] = fromsRs["Tablename"].ToString();
            oDictionary["strKey"] = strKey;
            oDictionary["Title"] = title;
            oDictionary["isDisplay"] = isValid;
            oDictionary["strip"] = strip;
            oDictionary["strXML"] = strXML;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("Stored_SaveDefineForms", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误，请返回重试！"); Response.End(); }
            /*********************************************************************************************************
             * 保存自定义字段信息
             * *******************************************************************************************************/
            try
            {
                if (!string.IsNullOrEmpty(frmDefine) && frmDefine.Length >= 5)
                {
                    new ColumnsHelper().SaveColumns(fromsRs["Tablename"].ToString(), (thisDictionary) =>
                    {
                        if (thisDictionary != null && thisDictionary.Count > 0)
                        {
                            DbHelper.Connection.Update(fromsRs["Tablename"].ToString(), thisDictionary, Params: " and Id =" + thisRs["Id"] + "");
                        }
                    }, (err) => { this.ErrorMessage(err); Response.End(); });
                }
            }
            catch { }
            /*******************************************************************
             * 输出保存成功的提示信息
             * *****************************************************************/
            this.ErrorMessage(string.Format("{0}保存成功！我们会尽快处理你提交的信息！", fromsRs["fromsName"]));
            Response.End();
        }
    }
}