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
    public partial class Short : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "edit": this.VerificationRole("用户消息"); this.Update(); Response.End(); break;
                case "add": this.VerificationRole("用户消息"); this.Add(); Response.End(); break;
                case "save": this.VerificationRole("用户消息"); this.AddSave(); Response.End(); break;
                case "del": this.VerificationRole("用户消息"); this.Delete(); Response.End(); break;
                default: this.VerificationRole("用户消息"); this.List(); Response.End(); break;
            }
        }
        /// <summary>
        /// 列表
        /// </summary>
        protected void List()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"7\">用户消息 >> 消息列表</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"120\">接收用户</td>");
            strText.Append("<td width=\"120\">发送用户</td>");
            strText.Append("<td>消息名称</td>");
            strText.Append("<td width=\"120\">发送日期</td>");
            strText.Append("<td width=\"80\">消息已读</td>");
            strText.Append("<td width=\"80\">操作选项</td>");
            strText.Append("</tr>");
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ID,UserID,Nickname,SendID,SendName,Addtime,Title,iLooker";
            PageCenterConfig.Params = string.Empty;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "Id desc";
            PageCenterConfig.Tablename = "Fooke_Short";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Short", string.Empty);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"Id\" value=\"" + Rs["Id"] + "\" /></td>");
                strText.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["SendName"]);
                strText.AppendFormat("<td>{0}</td>", Rs["title"]);
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"]);
                strText.AppendFormat("<td>{0}</td>", Rs["iLooker"].ToString() == "0" ? "<a class=\"vbtnRed\">否</a>" : "<a class=\"vbtn\">是</a>");
                strText.Append("<td>");
                strText.AppendFormat("<input type=\"button\" value=\"查看消息\" operate=\"view\" url=\"?action=edit&Id={0}\" class=\"button\" />", Rs["Id"]);
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"7\">");
            strText.Append("<select name=\"target\">");
            strText.Append("<option value=\"sel\">删除选中的数据</option>");
            strText.Append("<option value=\"days\">删除一天前的记录</option>");
            strText.Append("<option value=\"week\">删除一周前的记录</option>");
            strText.Append("<option value=\"month\">删除一月前的记录</option>");
            strText.Append("<option value=\"byear\">删除半年前的记录</option>");
            strText.Append("<option value=\"year\">删除一年前的记录</option>");
            strText.Append("<option value=\"looker\">删除所有已读记录</option>");
            strText.Append("<option value=\"all\">删除所有记录</option>");
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" class=\"button\" value=\"删除消息\" onclick=\"deleteOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/short/default.html");
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
        /// 添加
        /// </summary>
        protected void Add()
        {

            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/short/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "targetName": strValue = RequestHelper.GetRequest("targetName").toString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 查看
        /// </summary>
        protected void Update()
        {
            /******************************************************************************************************************************
            * 获取并验证请求数据信息
            * ****************************************************************************************************************************/
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindShort]", new Dictionary<string, object>() {
                {"Id",Id}
            });
            if (cRs == null) { this.ErrorMessage("拉取短消息失败,请刷新重试！"); Response.End(); }
            /******************************************************************************************************************************
             * 输出数据处理结果信息
             * ****************************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/short/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "targetName": strValue = RequestHelper.GetRequest("targetName").toString(); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }break;
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
        /// 保存
        /// </summary>
        protected void AddSave()
        {
            string targetMode = RequestHelper.GetRequest("targetMode").toInt();
            if (targetMode != "0" && targetMode != "1" && targetMode != "2") { this.ErrorMessage("请选择短消息发送对象！"); Response.End(); }
            string targetName = RequestHelper.GetRequest("targetName").toString();
            if (targetMode == "0" && string.IsNullOrEmpty(targetName)) { this.ErrorMessage("请填写要发送短信的用户！"); Response.End(); }
            /***************************************************************************************
             * 获取短消息内容
             * *************************************************************************************/
            string Title = RequestHelper.GetRequest("Title").toString();
            if (string.IsNullOrEmpty(Title)) { this.ErrorMessage("请填写短消息名称！"); Response.End(); }
            if (Title.Length > 60) { this.ErrorMessage("短消息名称长度请限制在60个汉字以内！"); Response.End(); }
            string Remark = RequestHelper.GetRequest("Remark").toString();
            if (string.IsNullOrEmpty(Remark)) { this.ErrorMessage("请填写发送短消息内容！"); Response.End(); }
            if (Remark.Length > 600) { this.ErrorMessage("短消息内容长度请限制在600个汉字以内！"); Response.End(); }
            /***************************************************************************************
             * 保存发送消息内容
             * ***************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["Title"] = Title;
            thisDictionary["Remark"] = Remark;
            thisDictionary["TargetMode"] = targetMode;
            thisDictionary["TargetName"] = targetName;
            DbHelper.Connection.ExecuteProc("Stored_SaveAdminShort", thisDictionary);
            /***************************************************************************************
             * 开始输出数据结果
             * ***************************************************************************************/
            this.ConfirmMessage("短消息发送成功，点击确定将继续停留在本页面！");
            Response.End();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        protected void Delete()
        {
            /**********************************************************************************************************************
             * 获取数据删除模式信息
             * ********************************************************************************************************************/
            string strTarget = RequestHelper.GetRequest("target").toString("sel");
            if (string.IsNullOrEmpty(strTarget)) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            else if (strTarget.Length <= 0) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            /**********************************************************************************************************************
             * 验证参数合法性
             * ********************************************************************************************************************/
            string strList = RequestHelper.GetRequest("ID").ToString();
            if (strTarget == "sel")
            {
                if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            }
            /**********************************************************************************************************************
             * 构建数据删除语句
             * ********************************************************************************************************************/
            string strParamter = string.Empty;
            switch (strTarget.ToLower())
            {
                case "sel": strParamter += " and Id in (" + strList + ")"; break;
                case "days": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and Addtime<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and Addtime<='" + DateTime.Now.AddYears(-1) + "'"; break;
                case "all": strParamter += " and 1=1"; break;
            }
            /**********************************************************************************************************************
             * 判断删除请求书否合法
             * ********************************************************************************************************************/
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            else if (strParamter.Length <= 0) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            /**********************************************************************************************************************
             * 开始删除请求数据
             * ********************************************************************************************************************/
            try { DbHelper.Connection.Delete("Fooke_Short", Params: strParamter); }
            catch { }
            /******************************************************************************************
             * 保存操作日志
             * ****************************************************************************************/
            try { SaveOperation("删除了用户消息ID(" + strParamter + ")"); }
            catch { }
            /**********************************************************************************************************************
             * 返回数据处理结果
             * ********************************************************************************************************************/
            this.History();
            Response.End();
        }
    }
}