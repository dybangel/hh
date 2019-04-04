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
    public partial class Pay : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "add": this.VerificationRole("用户充值"); this.Add(); Response.End(); break;
                case "save": this.VerificationRole("用户充值"); this.AddSave(); Response.End(); break;
                case "del": this.VerificationRole("用户充值"); this.Delete(); Response.End(); break;
                default: this.VerificationRole("用户充值"); this.List(); Response.End(); break;
            }
        }
        /// <summary>
        /// 关键词回复列表
        /// </summary>
        protected void List()
        {
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            string PaymentID = RequestHelper.GetRequest("PaymentID").toString();
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"8\">用户充值 >> 充值记录</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"8\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"searchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="userid",Text="搜用户昵称"},
                new OptionMode(){Value="nickname",Text="搜用户编号"},
                new OptionMode(){Value="username",Text="搜登陆账号"},
                new OptionMode(){Value="numberid",Text="搜订单号"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input placeholder=\"请填写要搜索的关键词\" type=\"text\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 充值日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">用户编号</td>");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"140\">订单号</td>");
            strText.Append("<td width=\"80\">充值金额</td>");
            strText.Append("<td width=\"160\">充值日期</td>");
            strText.Append("<td width=\"60\">充值方式</td>");
            strText.Append("<td>备注信息</td>");
            strText.Append("</tr>");
            /*****************************************************************************************************
             * 构建分页语句查询条件
             * ***************************************************************************************************/
            string Params = "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "userid": Params += " and UserID=" + Keywords + ""; break;
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                    case "numberid": Params += " and numberid like '%" + Keywords + "%'"; break;
                    case "username": Params += " and exists(select userid from fooke_user where fooke_user.userid=fooke_pay.userid and username like '%" + Keywords + "%')"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            if (!string.IsNullOrEmpty(PaymentID) && PaymentID.isInt()) { Params += " and PaymentID='" + PaymentID + "'"; }
            /*****************************************************************************************************
            * 构建分页查询语句
            * ***************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ID,strKey,UserID,Nickname,Addtime,Number,Remark,PaymentName,PaymentID";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "Id desc";
            PageCenterConfig.Tablename = TableCenter.UserPay;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.UserPay, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
            * 输出网页内容信息
            * ***************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"Id\" value=\"{0}\" /></td>", Rs["Id"]);
                strText.AppendFormat("<td><a href=\"Transfer.aspx?action=default&userid={0}\" style=\"color:#f00\">【账变】</a>{0}</td>", Rs["userid"]);
                strText.AppendFormat("<td><a href=\"?action=default&userid={0}\">{1}</a></td>", Rs["userid"], Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["strKey"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Number"]);
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"]);
                strText.AppendFormat("<td><a href=\"?paymentid={0}\">{1}</a></td>",Rs["paymentid"],Rs["paymentname"]);
                strText.AppendFormat("<td>{0}</td>",Rs["Remark"]);
                strText.Append("</tr>");
            }
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
            /*****************************************************************************************
             * 数据统计信息
             * ****************************************************************************************/
            DataRow ComputerRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindComputerPay]", new Dictionary<string, object>() {
                {"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")}
            });
            if (ComputerRs != null && ComputerRs.Table != null)
            {
                foreach (DataColumn col in ComputerRs.Table.Columns)
                {
                    strText.AppendFormat("&nbsp;{0}∶<font style=\"color:#f00;margin:0px 4px; font-weight:bold;\">{1}</font>", col.ColumnName, ComputerRs[col.ColumnName].ToString());
                }
            }
            /*****************************************************************************************
             * 数据信息统计结束
             * ***************************************************************************************/
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /*****************************************************************************************************
            * 输出网页内容
            * ***************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/pay/default.html");
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
            string strResponse = Master.Reader("template/pay/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "mode": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="Mode",Value="Free",Text="手续费预存"},
                        new RadioMode(){Name="Mode",Value="Access",Text="静态钱包"}
                    }, "Free"); break;
                    case "UserID": strValue = RequestHelper.GetRequest("UserID").toString(); break;
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
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("请选择要充值用户！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindMember", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { this.ErrorMessage("对不起,你查找的用户不存在！"); Response.End(); }
            string Nickname = RequestHelper.GetRequest("Nickname").toString();
            if (cRs != null && !string.IsNullOrEmpty(cRs["nickName"].ToString())) { Nickname = cRs["nickName"].ToString(); }
            double Number = RequestHelper.GetRequest("Number").cDouble();
            if (Number <= 0) { this.ErrorMessage("请填写给用户充值的金额！"); Response.End(); }
            string Remark = RequestHelper.GetRequest("Remark").toString();
            if (Remark.Length > 100) { this.ErrorMessage("描述备注信息长度请限制在100个汉字以内！"); Response.End(); }
            if (string.IsNullOrEmpty(Remark)) { Remark = string.Format("Manual Recharge {0}", Number); }
            string Affairs = RequestHelper.GetRequest("Affairs").toInt();
            if (Affairs != "0" && Affairs != "1") { this.ErrorMessage("请选择充值金额正负！"); Response.End(); }
            /********************************************************************
             * 生成充值订单号
             * ******************************************************************/
            string strKey = "用户充值-|-|-" + UserID + "-|-|-" + DateTime.Now.Ticks.ToString();
            strKey = "SD" + new Fooke.Function.String(strKey).ToMD5().Substring(0, 22).ToUpper();
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["strKey"] = strKey;
            oDictionary["UserID"] = UserID;
            oDictionary["Nickname"] = Nickname;
            oDictionary["Number"] = Number.ToString("0.00");
            oDictionary["PaymentName"] = "Manual";
            oDictionary["PaymentID"] = "0";
            oDictionary["Affairs"] = Affairs;
            oDictionary["Remark"] = Remark;
            DbHelper.Connection.ExecuteProc("Stored_SaveAdminPay", oDictionary);
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
            string ID = RequestHelper.GetRequest("ID").toString();
            string target = RequestHelper.GetRequest("target").toString();
            if (string.IsNullOrEmpty(target)) { this.ErrorMessage("请求参数错误,请选择删除数据模式！"); Response.End(); }
            if (target == "sel" && string.IsNullOrEmpty(ID)) { this.ErrorMessage("请求参数错误，请至少选择一条数据！"); Response.End(); }
            string strParamter = string.Empty;
            switch (target.ToLower())
            {
                case "sel": strParamter += " and Id in (" + ID + ")"; break;
                case "days": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-1) + "'"; break;
                case "week": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-7) + "'"; break;
                case "month": strParamter += " and Addtime<='" + DateTime.Now.AddMonths(-1) + "'"; break;
                case "byear": strParamter += " and Addtime<='" + DateTime.Now.AddDays(-180) + "'"; break;
                case "year": strParamter += " and Addtime<='" + DateTime.Now.AddYears(-1) + "'"; break;
                case "all": strParamter += " and 1=1"; break;
            }
            if (string.IsNullOrEmpty(strParamter)) { this.ErrorMessage("请求参数错误，请刷新网页重试！"); Response.End(); }
            DbHelper.Connection.Delete(TableCenter.UserPay, Params: strParamter);
            /********************************************************************
             * 输出返回数据
             * ******************************************************************/
            this.History();
            Response.End();
        }
    }
}