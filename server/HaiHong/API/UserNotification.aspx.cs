using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fooke.Code;
using Fooke.Function;
using System.Data;
namespace Fooke.Web.API
{
    public partial class UserNotification : Fooke.Web.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                default: strDefault(); Response.End(); break;
            }
        }
        

        /// <summary>
        /// 默认主页
        /// </summary>
        protected void strDefault()
        {
            /***************************************************************************************************
             * 获取分页列表数量
             * *************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(20);
            /***************************************************************************************************
             * 构建分页查询语句
             * *************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "NotificationId,strContext,Addtime,isDisplay,SortID";
            PageCenterConfig.Params = string.Empty;
            PageCenterConfig.Identify = "NotificationId";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "NotificationId desc";
            PageCenterConfig.Tablename = "Fooke_UserNotification";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserNotification", string.Empty);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***************************************************************************************************
             * 输出数据处理结果
             * *************************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }
    }
}