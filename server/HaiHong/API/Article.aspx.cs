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
    public partial class Article : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "total": TotalComputer(); Response.End(); break;
                case "details": Show(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 统计消息数量
        /// </summary>
        protected void TotalComputer()
        {
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            if (ChannelID == "0") { this.ErrorMessage("请求参数错误,请传入渠道标识！"); Response.End(); }
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请求参数错误,请传入分类标识！"); Response.End(); }
            string Params = "";
            if (ChannelID != "0") { Params += " and ChannelID=" + ChannelID + ""; }
            if (classId != "0") { Params += " and classId = " + classId + ""; }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Items, columns: "count(0)", Params: Params);
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"请求成功\"");
            strBuilder.Append(",\"count\":\"" + cRs[0].ToString() + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 查看网页详情
        /// </summary>
        protected void Show()
        {
            /***********************************************************************************************************
             * 获取并验证分类栏目参数信息
             * *********************************************************************************************************/
            string classId = RequestHelper.GetRequest("classId").toInt();
            if (classId == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow classRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"ClassID",classId}
            });
            if (classRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            else if (classRs["isDisplay"].ToString() != "1") { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /***********************************************************************************************************
             * 获取并验证文档模型数据信息
             * *********************************************************************************************************/
            DataRow channelRs = DbHelper.Connection.ExecuteFindRow("Stored_FindChannel", new Dictionary<string, object>() {
                {"ChannelID",classRs["ChannelID"]}
            });
            if (channelRs == null) { this.ErrorMessage("对不起，你查找的页面不存在！"); Response.End(); }
            else if (channelRs["isDisplay"].ToString() != "1") { this.ErrorMessage("越权操作！"); Response.End(); }
            /***********************************************************************************************************
             * 获取并验证文档参数信息
             * *********************************************************************************************************/
            string ItemsID = RequestHelper.GetRequest("ItemsID").toInt();
            if (ItemsID == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow ItemsRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindDocumentChannel]", new Dictionary<string, object>() {
                {"Tablename",channelRs["Tablename"].ToString()},
                {"ShowID",ItemsID}
            });
            if (ItemsRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            else if (ItemsRs["isDisplay"].ToString() != "1") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            /***********************************************************************************************************
             * 判断是否为跳转地址,否则执行跳转
             * *********************************************************************************************************/
            if (ItemsRs["isTo"].ToString() == "1" && !string.IsNullOrEmpty(ItemsRs["toUrl"].ToString()))
            {
                try
                {
                    string thisUrl = ItemsRs["toUrl"].ToString().Replace("&amp;", "&");
                    if (!thisUrl.Contains("http")) { thisUrl = "http://" + thisUrl; }
                    Response.Redirect(thisUrl);
                    Response.End();
                }
                catch { }
            }
            /***********************************************************************************************************
             * 开始解析模版并且执行输出
             * *********************************************************************************************************/
            Fooke.Release.ReleaseHelper ReleaseMaster = new Release.ReleaseHelper();
            string strReader = ReleaseMaster.ReleaseContext(strTemplate: "template/article/show.html",
                ShowRs: ItemsRs,
                classRs: classRs,
                channelRs: channelRs);
            Response.Write(strReader);
            Response.End();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        protected void strDefault()
        {
            /******************************************************************************************
             * 获取文章分类数据信息
             * ****************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            if (ClassID == "0") { this.ErrorMessage("请求参数错误,请传入分类标识！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"ClassID",ClassID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据错误,请重试！"); Response.End(); }
            /******************************************************************************************
             * 构建分页数据请求条件
             * ****************************************************************************************/
            string Params = " and classId = " + ClassID + " and isDisplay=1";
            /******************************************************************************************
             * 构建分页显示页码信息
             * ****************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /******************************************************************************************
             * 构建分页查询语句信息
             * ****************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ShowID,ChannelID,classId,className,Title,strDescrption,Thumb,isDisplay,isTop,isRec,isHot,isBook,isTo,Addtime,Hits";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "ShowID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "isTop desc,ShowID Desc";
            PageCenterConfig.Tablename = "Fooke_Article";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_Article", Params);
            DataTable Tab = FindHelper.Connection.ExecuteDataTable(strSQL);
            /***********************************************************************************
             * 输出网页数据
             * **********************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }
    }
}