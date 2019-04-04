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
namespace Fooke.Web.Plugin
{
    public partial class Books : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "books": strBooks(); Response.End(); break;
                case "start": SaveBooks(); Response.End(); break;
                default: Response.End(); break;
            }
        }
        /// <summary>
        /// 展示评论页面,是否用JS展示？或者用ajax展示？
        /// </summary>
        protected void strBooks()
        {
            string ItemsID = RequestHelper.GetRequest("ItemsID").toInt();
            if (ItemsID == "0") { Response.Write("请求参数错误!"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindItems", new Dictionary<string, object>() { { "ItemsID", ItemsID } });
            if (cRs == null) { this.ErrorMessage("拉取数据信息失败,请重试！"); Response.End(); }
            if (cRs["isDisplay"].ToString() != "1") { Response.Write("数据为通过审核!"); Response.End(); }
            if (cRs["isBook"].ToString() != "1") { Response.Write("评论已经关闭!"); Response.End(); }
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strReader = Fooke.Reader("template/books/books.html");
            strReader = Fooke.Start(strReader, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), isLabel: true);
            Response.Write(strReader);
            Response.End();
        }
        /// <summary>
        /// 保存评论
        /// </summary>
        protected void SaveBooks()
        {
            string ItemsID = RequestHelper.GetRequest("ItemsID").toInt();
            if (ItemsID == "0") { this.JSONMessage("请求参数错误,请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindItems", new Dictionary<string, object>() { { "ItemsID", ItemsID } });
            if (cRs == null) { this.JSONMessage("拉取数据信息失败,请重试！"); Response.End(); }
            if (cRs["isDisplay"].ToString() != "1") { JSONMessage("数据为通过审核!"); Response.End(); }
            if (cRs["isBook"].ToString() != "1") { JSONMessage("评论已经关闭!"); Response.End(); }
            string booksText = RequestHelper.GetRequest("booksText").toString();
            if (string.IsNullOrEmpty(booksText)) { this.JSONMessage("评论内容不能为空！"); Response.End(); }
            if (booksText.Length > 500) { this.ErrorMessage("评论内容长度请限制在500个汉字以内！"); Response.End(); }
            int ParentID = RequestHelper.GetRequest("ParentID").cInt();
            if (ParentID < 0) { this.ErrorMessage("请求参数错误,请刷新网页重试！"); Response.End(); }
            /**************************************************************************
             * 验证用户信息是否存在且合法
             * ***********************************************************************/
            this.VerifyMemberLogin(err =>
            {
                if (!string.IsNullOrEmpty(err)) { JSONMessage(err); Response.End(); }
            });
            if (MemberRs == null) { JSONMessage("请先登录！"); Response.End(); }
            /**************************************************************************
             * 开始保存数据
             * *************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["ParentID"] = "0";
            oDictionary["ChannelID"] = cRs["ChannelID"].ToString();
            oDictionary["ItemsID"] = cRs["ItemsID"].ToString();
            oDictionary["UserID"] = cRs["UserID"].ToString();
            oDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            oDictionary["Title"] = cRs["Title"].ToString();
            oDictionary["BooksText"] = booksText;
            oDictionary["strIP"] = FunctionCenter.GetCustomerIP();
            DbHelper.Connection.ExecuteProc("Stored_SaveBooks", oDictionary);
            this.JSONMessage("评论发表成功", true); Response.End();
        }
    }
}