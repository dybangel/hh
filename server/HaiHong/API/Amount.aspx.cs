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
namespace Fooke.Web.API
{
    public partial class Amount : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "duty": strDuty(); Response.End(); break;
                case "share": strShare(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 获取我的账变记录
        /// </summary>
        protected void strDefault()
        {
            /*****************************************************************************************************
             * 获取数据请求参数信息
             * ****************************************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string Mode = RequestHelper.GetRequest("Mode").toString();
            /*****************************************************************************************************
             * 构建分页查询数据表
             * ****************************************************************************************************/
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append("(");
            strTabs.Append("    select List.Id,List.strKey,List.Mode,List.UserID,List.Nickname,List.FormID,List.Formname,");
            strTabs.Append("    List.Amount,List.Balance,List.Remark,List.Addtime,List.Affairs,");
            strTabs.Append("    ISNULL(Foke.Thumb,'/file/user/default.png') as thumb");
            strTabs.Append("    from Fooke_Amount as List left join Fooke_User as Foke on ");
            strTabs.Append("    Foke.UserID = List.FormID");
            strTabs.Append(") as FokeApps");
            /*****************************************************************************************************
             * 获取数据查询条件信息
             * ****************************************************************************************************/
            string Params = " and UserID=" + MemberRs["UserID"] + "";
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:00") + "'"; }
            if (!string.IsNullOrEmpty(Mode)) { Params += " and Mode='" + Mode + "'"; }
            /*****************************************************************************************************
             * 获取分页数量信息
             * ****************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(20);
            /*****************************************************************************************************
             * 构建分页查询语句
             * ****************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "Id desc";
            PageCenterConfig.Tablename = strTabs.ToString();
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(strTabs.ToString(), Params);
            DataTable Tab = FindHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
             * 输出数据处理结果
             * ****************************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }

        /// <summary>
        /// 获取我的任务记录
        /// </summary>
        protected void strDuty()
        {
            /*****************************************************************************************************
             * 获取数据请求参数信息
             * ****************************************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            /*****************************************************************************************************
             * 获取数据查询条件信息
             * ****************************************************************************************************/
            string Params = " and UserID=" + MemberRs["UserID"] + "";
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:00") + "'"; }
            /*****************************************************************************************************
             * 获取分页数量信息
             * ****************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(20);
            /*****************************************************************************************************
             * 构建分页查询语句
             * ****************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "DutyID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "DutyID desc";
            PageCenterConfig.Tablename = "Fooke_UserDuty";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserDuty", Params);
            DataTable Tab = FindHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
             * 输出数据处理结果
             * ****************************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }

        /// <summary>
        /// 获取学徒记录信息
        /// </summary>
        protected void strShare()
        {
            /*****************************************************************************************************
             * 获取数据请求参数信息
             * ****************************************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string Mode = RequestHelper.GetRequest("Mode").toString();
            /*****************************************************************************************************
             * 构建分页查询数据表
             * ****************************************************************************************************/
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append("(");
            strTabs.Append("    select List.Id,List.strKey,List.Mode,List.UserID,List.Nickname,List.FormID,List.Formname,");
            strTabs.Append("    List.Amount,List.Balance,List.Remark,List.Addtime,List.Affairs,");
            strTabs.Append("    ISNULL(Foke.Thumb,'/file/user/default.png') as thumb");
            strTabs.Append("    from Fooke_Amount as List left join Fooke_User as Foke on ");
            strTabs.Append("    Foke.UserID = List.FormID");
            strTabs.Append(") as FokeApps");
            /*****************************************************************************************************
             * 获取数据查询条件信息
             * ****************************************************************************************************/
            string Params = " and UserID=" + MemberRs["UserID"] + " and Mode in ('邀请奖励','任务提成')";
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + new Fooke.Function.String(StarDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:00") + "'"; }
            /*****************************************************************************************************
             * 获取分页数量信息
             * ****************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(20);
            /*****************************************************************************************************
             * 构建分页查询语句
             * ****************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "*";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "Id desc";
            PageCenterConfig.Tablename = strTabs.ToString();
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(strTabs.ToString(), Params);
            DataTable Tab = FindHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
             * 输出数据处理结果
             * ****************************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }
    }
}