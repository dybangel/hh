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
    public partial class Search : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "start": SearchStart(); Response.End(); break;
                case "default": strDefault(); Response.End(); break;
            }
            Response.End();
        }

        /// <summary>
        /// 显示投票页面
        /// </summary>
        protected void strDefault()
        {
            string TemplateDirectory = this.GetParameter("TemplateDir", "siteXML").toString();
            if (string.IsNullOrEmpty(TemplateDirectory)) { TemplateDirectory = "template"; }
            string cTemplate = Win.ApplicationPath + "/" + TemplateDirectory + "/search.html";
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader(cTemplate);
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "channel": break;
                }
                return strValue;
            }), true);
            Response.Write(strReader);
            Response.End();
        }

        /// <summary>
        /// 显示投票页面
        /// </summary>
        protected void SearchStart()
        {
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string classId = RequestHelper.GetRequest("classId").toInt();
            string channelId = RequestHelper.GetRequest("channelId").toInt();
            string StartDate = RequestHelper.GetRequest("StartDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            int PageSize = RequestHelper.GetRequest("PageSize").cInt();
            if (PageSize <= 0) { PageSize = 20; }
            if (string.IsNullOrEmpty(Keywords)) { Response.Redirect("search.aspx"); Response.End(); }
            /************************************************************************
             * 开始查询语句
             * **********************************************************************/
            string Params = " and class.classid=item.classid";
            if (!string.IsNullOrEmpty(Keywords)) { Params += " and item.title like '%" + Keywords + "%' or item.keywords like '" + Keywords + "'"; }
            if (classId != "0") { Params += " and item.classId = " + classId + ""; }
            if (channelId != "0") { Params += " and item.channelid=" + channelId + ""; }
            if (!string.IsNullOrEmpty(StartDate) && VerifyCenter.VerifyDateTime(StartDate)) { Params += " and item.Addtime>='" + StartDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and item.addtime<='" + EndDate + "'"; }
            string Tablename = "Fooke_class as class,Fooke_Items as Item";
            string columns = "Item.ItemsID,Item.ChannelID,Item.classId,Item.className,Item.Title,Item.Thumb,Item.Addtime,Item.Hits,Item.fileName,Item.strDesc,Item.Keywords,Item.Star,Item.isLike,class.isOper,class.Identify";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = columns;
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Item.ItemsID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Item.ItemsID";
            PageCenterConfig.Tablename = Tablename;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(Tablename, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /**********************************************************************
             * 遍历查找结果，返回JSON格式数据
             * ********************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\"success\":\"true\"");
            strBuilder.Append(",\"items\":[");
            int K = 0;
            foreach (DataRow cRs in Tab.Rows)
            {
                if (K != 0) { strBuilder.Append(","); }
                K = K + 1;
                strBuilder.Append("{");
                strBuilder.Append("\"autoid\":\"" + K + "\"");
                strBuilder.Append(",\"title\":\"" + cRs["title"] + "\"");
                strBuilder.Append(",\"classid\":\"" + cRs["classid"] + "\"");
                strBuilder.Append(",\"classname\":\"" + cRs["classname"] + "\"");
                strBuilder.Append(",\"thumb\":\"" + cRs["thumb"] + "\"");
                strBuilder.Append(",\"addtime\":\"" + cRs["addtime"] + "\"");
                strBuilder.Append(",\"hits\":\"" + cRs["hits"] + "\"");
                strBuilder.Append(",\"strdesc\":\"" + cRs["strdesc"] + "\"");
                strBuilder.Append(",\"keywords\":\"" + cRs["keywords"] + "\"");
                strBuilder.Append(",\"star\":\"" + cRs["star"] + "\"");
                strBuilder.Append(",\"islike\":\"" + cRs["islike"] + "\"");
                strBuilder.Append(",\"linkurl\":\"" + SimpleMaster.ResolutionMaster.GetFileUrl(cRs) + "\"");
                strBuilder.Append(",\"classurl\":\"" + SimpleMaster.ResolutionMaster.GetListUrl(cRs) + "\"");
                strBuilder.Append("}");
            }
            strBuilder.Append("]");
            strBuilder.Append(",\"record\":\"" + Record + "\"}");
            /*********************************************************************
             * 开始显示模版内容
             * ******************************************************************/
            string TemplateDirectory = this.GetParameter("TemplateDir", "siteXML").toString();
            if (string.IsNullOrEmpty(TemplateDirectory)) { TemplateDirectory = "template"; }
            string cTemplate = Win.ApplicationPath + "/" + TemplateDirectory + "/search.html";
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader(cTemplate);
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    case "result": strValue = strBuilder.ToString(); break;
                    case "pagebar": strValue = PageCenter.Often(Record, PageSize); break;
                    case "keywords": strValue = Keywords.ToLower(); break;
                    case "stardate": strValue = StartDate.ToLower(); break;
                    case "enddate": strValue = EndDate; break;
                    case "classid": strValue = classId; break;
                    case "channelid": strValue = channelId; break;
                }
                return strValue;
            }), true);
            Response.Write(strReader);
            Response.End();
        }
    }
}