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
    public partial class UserNotification : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "edit": this.VerificationRole("用户公告"); this.Update(); Response.End(); break;
                case "add": this.VerificationRole("用户公告"); this.Add(); Response.End(); break;
                case "save": this.VerificationRole("用户公告"); this.AddSave(); Response.End(); break;
                case "editsave": this.VerificationRole("用户公告"); this.SaveUpdate(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); this.Delete(); Response.End(); break;
                case "display": this.VerificationRole("用户公告"); SaveDisplay(); Response.End(); break;
                case "savesrt": this.VerificationRole("用户公告"); SaveEditor(); Response.End(); break;
                default: this.VerificationRole("用户公告"); this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 关键词回复列表
        /// </summary>
        protected void strDefault()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">用户公告 >> 公告列表</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td>公告内容</td>");
            strText.Append("<td width=\"120\">添加日期</td>");
            strText.Append("<td width=\"80\">显示排序</td>");
            strText.Append("<td width=\"80\">显示状态</td>");
            strText.Append("<td width=\"80\">操作选项</td>");
            strText.Append("</tr>");
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "NotificationId,strContext,Addtime,isDisplay,SortID";
            PageCenterConfig.Params = string.Empty;
            PageCenterConfig.Identify = "NotificationId";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "NotificationId desc";
            PageCenterConfig.Tablename = "Fooke_UserNotification";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserNotification", string.Empty);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"NotificationId\" value=\"" + Rs["NotificationId"] + "\" /></td>");
                strText.AppendFormat("<td>{0}</td>", Rs["strContext"]);
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"]);
                strText.AppendFormat("<td><input type=\"text\" isnumeric=\"true\" operate=\"editsort\" url=\"?action=savesrt&NotificationId={0}\" size=\"5\" class=\"inputtext center\" value=\"{1}\" /></td>", Rs["NotificationId"], Rs["SortID"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.AppendFormat("<a href=\"?action=display&val=0&NotificationId={0}\"><img src=\"images/ico/yes.gif\"/></a>", Rs["NotificationId"]); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&NotificationId={0}\"><img src=\"images/ico/no.gif\"/></a>", Rs["NotificationId"]); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=edit&NotificationId={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["NotificationId"]);
                strText.AppendFormat("<a href=\"?action=del&NotificationId={0}\"  title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["NotificationId"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            /********************************************************************************************************************
             * 数据处理信息
             * ******************************************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"6\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除数据\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"允许显示(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"允许显示(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /**************************************************************************************
             * 输出数据处理结果
             * ************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/notification/default.html");
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
        protected void Add()
        {

            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/notification/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                          new RadioMode(){Name="isDisplay",Text="立即显示(是)",Value="1"},
                          new RadioMode(){Name="isDisplay",Text="立即显示(否)",Value="0"},
                    }, "1"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 查看消息内容
        /// </summary>
        protected void Update()
        {
            /********************************************************************
             * 验证并查询参数信息
             * ******************************************************************/
            string NotificationId = RequestHelper.GetRequest("NotificationId").toInt();
            if (NotificationId == "0") { this.ErrorMessage("获取请求参数失败,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserNotification]", new Dictionary<string, object>() {
                {"NotificationId",NotificationId}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/notification/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                          new RadioMode(){Name="isDisplay",Text="立即显示(是)",Value="1"},
                          new RadioMode(){Name="isDisplay",Text="立即显示(否)",Value="0"},
                    },cRs["isDisplay"].ToString()); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
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
            /***************************************************************************************
             * 获取并验证公告内容
             * *************************************************************************************/
            string strContext = RequestHelper.GetRequest("strContext").ToString();
            if (string.IsNullOrEmpty(strContext)) { this.ErrorMessage("公告内容不能为空！"); Response.End(); }
            else if (strContext.Length <= 8) { this.ErrorMessage("公告内容不能少于6个汉字！"); Response.End(); }
            else if (strContext.Length >= 600) { this.ErrorMessage("公告内容长度不能超过600个汉字！"); Response.End(); }
            /***************************************************************************************
             * 获取其他不需要验证的数据信息
             * *************************************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /***************************************************************************************
             * 保存发送消息内容
             * ***************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["strContext"] = strContext;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            DbHelper.Connection.ExecuteProc("[Stored_SaveUserNotification]", thisDictionary);
            /***************************************************************************************
             * 开始输出数据结果
             * ***************************************************************************************/
            this.ConfirmMessage("数据发送成功，点击确定将继续停留在本页面！");
            Response.End();
        }


        protected void SaveUpdate()
        {
            /***************************************************************************************
             * 验证并查询参数信息
             * *************************************************************************************/
            string NotificationId = RequestHelper.GetRequest("NotificationId").toInt();
            if (NotificationId == "0") { this.ErrorMessage("获取请求参数失败,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserNotification]", new Dictionary<string, object>() {
                {"NotificationId",NotificationId}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /***************************************************************************************
             * 获取并验证公告内容
             * *************************************************************************************/
            string strContext = RequestHelper.GetRequest("strContext").ToString();
            if (string.IsNullOrEmpty(strContext)) { this.ErrorMessage("公告内容不能为空！"); Response.End(); }
            else if (strContext.Length <= 8) { this.ErrorMessage("公告内容不能少于6个汉字！"); Response.End(); }
            else if (strContext.Length >= 600) { this.ErrorMessage("公告内容长度不能超过600个汉字！"); Response.End(); }
            /***************************************************************************************
             * 获取其他不需要验证的数据信息
             * *************************************************************************************/
            string SortID = RequestHelper.GetRequest("SortID").toInt();
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            /***************************************************************************************
             * 保存发送消息内容
             * ***************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["NotificationId"] = cRs["NotificationId"].ToString();
            thisDictionary["strContext"] = strContext;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["SortID"] = SortID;
            DbHelper.Connection.ExecuteProc("[Stored_SaveUserNotification]", thisDictionary);
            /***************************************************************************************
             * 开始输出数据结果
             * ***************************************************************************************/
            this.ConfirmMessage("数据发送成功，点击确定将继续停留在本页面！");
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
            string strList = RequestHelper.GetRequest("NotificationId").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
            * 开始查询请求数据信息
            * *********************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_UserNotification", Params: " and NotificationId in (" + strList + ") and isDisplay=0");
            if (Tab == null || Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***********************************************************************************************
            * 开始删除彩种信息
            * *********************************************************************************************/
            DbHelper.Connection.Delete("Fooke_UserNotification",
                Params: " and NotificationId in (" + strList + ") and isDisplay=0");
            /***********************************************************************************************
            * 输出数据处理结果
            * *********************************************************************************************/
            this.History();
            Response.End();
        }


        /// <summary>
        /// 保存显示设置
        /// </summary>
        protected void SaveDisplay()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("NotificationId").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strList.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 10) { this.ErrorMessage("最多只能选择10个玩法信息！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***************************************************************************************************
             * 判断是否存在需要处理的数据
             * *************************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_UserNotification", Params: " and NotificationId in (" + strList + ")");
            if (Tab == null || Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***************************************************************************************************
            * 开始保存数据处理结果
            * *************************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            DbHelper.Connection.Update("Fooke_UserNotification", dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and NotificationId in (" + strList + ")");
            /**********************************************************************************************
             * 输出网页数据信息
             * ********************************************************************************************/
            this.History();
            Response.End();
        }

        protected void SaveEditor()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("NotificationId").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (strList.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 10) { this.ErrorMessage("最多只能选择10个玩法信息！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***************************************************************************************************
             * 判断是否存在需要处理的数据
             * *************************************************************************************************/
            DataTable Tab = DbHelper.Connection.FindTable("Fooke_UserNotification", Params: " and NotificationId in (" + strList + ")");
            if (Tab == null || Tab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /***************************************************************************************************
             * 开始保存数据处理结果
             * *************************************************************************************************/
            string strValue = RequestHelper.GetRequest("value").toInt();
            DbHelper.Connection.Update("Fooke_UserNotification", dictionary: new Dictionary<string, string>() { 
                {"SortID",strValue}
            }, Params: " and NotificationId in (" + strList + ")");
            /**********************************************************************************************
             * 输出网页数据信息
             * ********************************************************************************************/
            Response.Write("success");
            Response.End();
        }
    }
}