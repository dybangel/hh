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
    public partial class Short : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "unread": UnreadList(); Response.End(); break;
                case "details": Details(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 计算统计我的各种帐变类型的统计信息
        /// </summary>
        protected void UnreadList()
        {
            /******************************************************************************************
             * 开始查询数据
             * ****************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindShort]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"Request","统计未读"}
            });
            /******************************************************************************************
             * 输出网页数据信息
             * ****************************************************************************************/
            ResponseDataRow(cRs);
            Response.End();
        }

        protected void Details()
        {
            /******************************************************************************************
             * 验证数据信息
             * ****************************************************************************************/
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误,请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindShort", new Dictionary<string, object>() {
                {"Id",Id}
            });
            if (cRs == null) { this.ErrorMessage("拉取短消息失败,请刷新重试！"); Response.End(); }
            /********************************************************************
             * 更新已读
             * ******************************************************************/
            if (cRs != null && cRs["iLooker"].ToString() != "1")
            {
                DbHelper.Connection.Update("Fooke_Short", new Dictionary<string, string>() {
                    {"iLooker","1"}
                }, Params: " and Id=" + Id + "");
            }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("/wap/short/show.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 获取我的账变记录
        /// </summary>
        protected void strDefault()
        {
            /***********************************************************************************
             * 获取查询条件
             * *********************************************************************************/
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            /***********************************************************************************
            * 构建分页查询条件
            * *********************************************************************************/
            string Params = " and UserID = " + MemberRs["UserID"] + "";
            int PageSize = RequestHelper.GetRequest("PageSize").cInt();
            if (PageSize <= 0) { PageSize = 10; }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate + "'"; }
            /***********************************************************************************
            * 构建分页查询语句
            * *********************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ID,UserID,Nickname,SendID,SendName,Addtime,Remark,Title,iLooker";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "Id desc";
            PageCenterConfig.Tablename = "Fooke_Short";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Short", Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /******************************************************************************************
             * 输出网页数据信息
             * ****************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }
        
    }
}