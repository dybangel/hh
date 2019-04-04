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
    public partial class UnionChannel : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "class": strClass(); Response.End(); break;
                case "toRedirect": SaveRedirect(); Response.End(); break;
                case "default": strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 跳转到指定的应用地址
        /// </summary>
        protected void SaveRedirect()
        {
            /*******************************************************************************************************
             * 验证请求参数信息
             * *****************************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /*******************************************************************************************************
             * 更新应用点击次数以及最后更新日期
             * *****************************************************************************************************/
            try
            {
                DbHelper.Connection.Update("Fooke_Application", dictionary: new Dictionary<string, string>() {
                    {"Hits",(new Fooke.Function.String(Rs["Hits"].ToString()).cInt()+1).ToString()},
                    {"Lastdate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
                }, Params: " and AppID=" + Rs["AppID"] + "");
            }
            catch { }
            /*******************************************************************************************************
             * 输出网页内容数据信息
             * *****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/application/show.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 获取生活服务分类数据信息
        /// </summary>
        protected void strClass()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("[");
            /*************************************************************************************************
             * 开始执行请求数据信息
             * ***********************************************************************************************/
            int SelectedIndex = 0;
            new Fooke.Code.AppClassHelper().Each(Fun: (cRs, rowIndex, sChar) =>
            {
                if (SelectedIndex != 0) { strBuilder.Append(","); }
                strBuilder.Append("{");
                strBuilder.Append("\"classid\":\"" + cRs["classId"] + "\"");
                strBuilder.Append(",\"classname\":\"" + cRs["classname"] + "\"");
                strBuilder.Append(",\"parentid\":\"" + cRs["parentid"] + "\"");
                strBuilder.Append(",\"ischild\":\"" + cRs["isChild"] + "\"");
                strBuilder.Append("}");
                SelectedIndex = SelectedIndex + 1;
            },
            ParentID: "0",
            SparChar: "");
            /*************************************************************************************************
             * 输出数据处理结果
             * ***********************************************************************************************/
            strBuilder.Append("]");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取服务应用列表
        /// </summary>
        protected void strDefault()
        {
            /**************************************************************************************
             * 获取筛选条件信息
             * *************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string isRec = RequestHelper.GetRequest("isRec").toInt();
            /***********************************************************************************************
             * 构建分页语句查询条件
             * **********************************************************************************************/
            string strParams = " and isDisplay=1";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "strUnion": strParams += " and strUnion like '%" + Keywords + "%'"; break;
                    case "UnionModel": strParams += " and UnionModel like '%" + Keywords + "%'"; break;
                }
            }
            if (isRec == "1") { strParams += " and isRec=" + isRec + ""; }
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(20);
            /***********************************************************************************************
            * 构建分页查询语句
            * **********************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "UnionID,UnionKey,UnionModel,DeviceModel,strUnion,strThumb,strRemark,SortID,isDisplay,isTop";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "UnionID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " SortID Desc,UnionID asc";
            PageCenterConfig.Tablename = "Fooke_UnionChannel";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UnionChannel", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************************
            * 输出网页信息
            * **********************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }

    }
}