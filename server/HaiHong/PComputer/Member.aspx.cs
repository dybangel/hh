using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using Fooke.Code;
using Fooke.Function;
using Fooke.SimpleMaster;
namespace Fooke.Web.Member
{
    public partial class Member : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "list": strList(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 获取请求用户列表
        /// </summary>
        protected void strList()
        {
            /************************************************************************************************************
             * 构建分页查询条件信息
             * **********************************************************************************************************/
            string Params = " and ParentID = " + MemberRs["UserID"] + " ";
            /************************************************************************************************************
             * 获取分页显示数量
             * **********************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(20);
            /************************************************************************************************************
             * 构建分页查询语句
             * **********************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UserID,ParentID,UserModel,UserName,Thumb,amtWallet,Nickname,Amount,isDisplay,Addtime,LastDate,strEmail,strMobile";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " UserID Desc";
            PageCenterConfig.Tablename = "Fooke_User";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_User", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            if (Tab == null || Tab.Rows.Count <= 0) { Response.Write(""); Response.End(); }
            /************************************************************************************************************
             * 输出网页内容信息
             * **********************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendFormat("<table class=\"tabs\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
            foreach (DataRow Rs in Tab.Rows)
            {
                strBuilder.AppendFormat("<tr class=\"hback\">");
                strBuilder.AppendFormat("<td class=\"img\"><img src=\"{0}\" /></td>", FunctionCenter.ConvertPath(Rs["Thumb"].ToString()));
                strBuilder.AppendFormat("<td class=\"textor\">");
                strBuilder.AppendFormat("<div>手机号:{0}</div>", Rs["Username"]);
                strBuilder.AppendFormat("<div>持币量:{0}</div>", Rs["Amount"]);
                strBuilder.AppendFormat("</td>");
                strBuilder.AppendFormat("<td class=\"btn\"><input class=\"transfer\" type=\"button\" onClick=\"window.location='wallet.aspx?action=transfer&strwallet={0}'\" value=\"转账\" /></td>", Rs["amtWallet"]);
                strBuilder.AppendFormat("</tr>");
            }
            strBuilder.AppendFormat("</table>");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 默认主页
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 获取用户团队业绩信息
             * ************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAchieve]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            if (cRs == null) { this.ErrorMessage("获取用户团队业绩信息失败,请重试！"); Response.End(); }
            else if (cRs.Table.Columns["Achieve"] == null) { this.ErrorMessage("获取请求数据内容信息失败,请重试!"); Response.End(); }
            /**************************************************************************************
             * 解析网页模板输出内容
             * ************************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader("template/member/default.html");
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "weekday": strValue = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek); break;
                    case "today": strValue = DateTime.Now.ToString("yyyy年MM月dd日"); break;
                    case "Achieve": strValue = new Fooke.Function.String(cRs["Achieve"].ToString()).cDouble().ToString("0.00"); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strResponse);
            Response.End();
        }
    }
}