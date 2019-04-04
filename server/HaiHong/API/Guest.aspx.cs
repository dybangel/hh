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
namespace Fooke.Web.API
{
    public partial class Guest : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "looker": strLooker(); Response.End(); break;
                case "del": Delete(); Response.End(); break;
                case "save": AddSave(); Response.End(); break;
                case "list": strList(); Response.End(); break;
                case "saveList": SaveList(); Response.End(); break;
                case "model": strModel(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 获取留言反馈列表信息
        /// </summary>
        protected void strModel()
        {
            string strTitle = this.GetParameter("titleList", "GuestXml").ToString();
            if (string.IsNullOrEmpty(strTitle)) { strTitle = "为什么我不能提现?|为什么我无法众筹?"; }
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"items\":[");
            int SelectionIndex = 0;
            foreach (string strChar in strTitle.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (SelectionIndex != 0) { strBuilder.Append(","); }
                strBuilder.Append("{\"title\":\"" + strChar + "\"}");
                SelectionIndex = SelectionIndex + 1;
            }
            strBuilder.Append("]");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// 回复列表
        /// </summary>
        protected void strList()
        {
            /************************************************************************************************
             * 获取网页请求参数
             * **********************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { this.ErrorMessage("获取反馈信息失败！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { this.ErrorMessage("拉取数据失败,请刷新重试！"); Response.End(); }
            /************************************************************************************************
            * 开始查询请求数据
            * **********************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindGuestTotalList]", new Dictionary<string, object>() {
                {"GuestID",cRs["GuestID"].ToString()}
            });
            /************************************************************************************************
            * 输出数据处理结果
            * **********************************************************************************************/
            ResponseDataTable(thisTab);
            Response.End();
        }
        /// <summary>
        /// 列表
        /// </summary>
        protected void strDefault()
        {
            /********************************************************************************************
             * 货物我的留言信息
             * *****************************************************************************************/
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string Keywords = RequestHelper.GetRequest("keywords").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            /********************************************************************************************
             * 构建分页查询条件
             * *****************************************************************************************/
            string Params = " and UserID = " + MemberRs["UserID"] + "";
            if (!string.IsNullOrEmpty(SearchType) && !string.IsNullOrEmpty(Keywords))
            {
                switch (SearchType.ToLower())
                {
                    case "orderid": Params += " and orderid like '%" + Keywords + "%'"; break;
                    case "nickname": Params += " and nickname like '%" + Keywords + "%'"; break;
                    case "title": Params += " and title like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { Params += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and Addtime<='" + EndDate + "'"; }
            if (UserID != "0") { Params += " and UserID=" + UserID + ""; }
            /********************************************************************************************
             * 获取分页显示数量
             * *****************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(30);
            /********************************************************************************************
             * 构建分页查询语句
             * *****************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "GuestID,UserID,Nickname,Addtime,strTitle,strContent,isReply,[strReply],Lastdate";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "GuestID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = "GuestID desc";
            PageCenterConfig.Tablename = TableCenter.Guest;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Guest, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /********************************************************************************************
             * 输出网页参数内容
             * *****************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        protected void strLooker()
        {
            /*******************************************************************************************
             * 验证请求参数信息
             * ******************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { Response.Write("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { Response.Write("获取请求数据失败,请重试！"); Response.End(); }
            if (cRs["UserID"].ToString() != MemberRs["UserID"].ToString()) { Response.Write("越权操作,请重试！"); Response.End(); }
            /*******************************************************************************************
             * 输出网页内容
             * ******************************************************************************************/
            ResponseDataRow(cRs);
            Response.End();
        }
        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("GuestID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /****************************************************************************************
             * 开始删除数据
             * **************************************************************************************/
            DbHelper.Connection.Delete(TableCenter.Guest, Params: " and GuestID in (" + strList + ") and UserID=" + MemberRs["UserID"] + "");
            DbHelper.Connection.Delete("Fooke_GuestList", Params: " and GuestID in (" + strList + ") and UserID=" + MemberRs["UserID"] + "");
            /****************************************************************************************
             * 输出返回数据
             * **************************************************************************************/
            this.ErrorMessage("数据删除成功!", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 保存投诉与建议信息
        /// </summary>
        protected void AddSave()
        {
            /***********************************************************************************************************************
             * 判断留言板功能是否已关闭
             * *********************************************************************************************************************/
            string isOpen = this.GetParameter("isOpen", "GuestXml").toInt();
            if (isOpen != "1") { this.ErrorMessage("留言板功能已经关闭,暂时无法使用！"); Response.End(); }
            /***********************************************************************************************************************
             * 验证时间与重复功能
             * *********************************************************************************************************************/
            int starTimer = this.GetParameter("starTimer", "GuestXml").cInt();
            if (starTimer <= 0) { starTimer = 0; }
            if (DateTime.Now.Hour < starTimer) { this.ErrorMessage("反馈时间未到,请等待！"); Response.End(); }
            int endTimer = this.GetParameter("endTimer", "GuestXml").cInt();
            if (endTimer >= 24) { endTimer = 24; }
            if (DateTime.Now.Hour > endTimer) { this.ErrorMessage("今日反馈时间已经过了,请明日再来吧！"); Response.End(); }
            /***********************************************************************************************************************
             * 获取用户提交参数
             * **********************************************************************************************************************/
            string strTitle = RequestHelper.GetRequest("strTitle").toString();
            if (string.IsNullOrEmpty(strTitle)) { this.ErrorMessage("请选择留言类型！"); Response.End(); }
            if (strTitle.Length > 50) { this.ErrorMessage("反馈类型长度请限制在50个汉字以内！"); Response.End(); }
            string strContent = RequestHelper.GetRequest("strContent", false).toString().Replace(System.Environment.NewLine, "<br/>");
            if (string.IsNullOrEmpty(strContent)) { this.ErrorMessage("请填写留言反馈内容！"); Response.End(); }
            if (strContent.Length > 1200) { this.ErrorMessage("反馈备注信息内容长度请限制在1200个汉字以内！"); Response.End(); }
            string strMobile = RequestHelper.GetRequest("strMobile").toString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("请填写您的联系电话！"); Response.End(); }
            if (strMobile.Length > 16) { this.ErrorMessage("电话号码字段长度请限制在16个字符以内！"); Response.End(); }
            if (!VerifyCenter.VerifyMobile(strMobile)) { this.ErrorMessage("手机号码格式错误！"); Response.End(); }
            /**********************************************************************************************************************
            * 验证留言内容合法性
            * *********************************************************************************************************************/
            DataRow iRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuestToday]", new Dictionary<string, object>() {
               {"UserID",MemberRs["UserID"].ToString()},
               {"strTitle",strTitle},
               {"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")}
            });
            if (iRs == null) { this.ErrorMessage("获取请求数据失败,请重试!"); Response.End(); }
            if (iRs["isExists"].ToString() != "0") { this.ErrorMessage("您的留言信息已提交,请等待客服处理！"); Response.End(); }
            int timer = this.GetParameter("timer", "GuestXml").cInt();
            if (timer != 0 && timer <= iRs["Totay"].ToString().cInt()) { this.ErrorMessage("今日留言次数已经达到上限,请明日再来吧！"); Response.End(); }
            /*************************************************************************************************************************
             * 开始保存数据信息
             * ***********************************************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["UserID"] = MemberRs["UserID"].ToString();
            oDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            oDictionary["strTitle"] = strTitle;
            oDictionary["strContent"] = strContent;
            oDictionary["strMobile"] = strMobile;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveGuest]", oDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /***************************************************************************
             * 输出数据处理结果信息
             * *************************************************************************/
            ResponseDataRow(thisRs);
            Response.End();
        }

        /// <summary>
        /// 保存留言回复信息
        /// </summary>
        protected void SaveList()
        {
            /************************************************************************************************
             * 验证留言ID是否合法
             * **********************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { this.ErrorMessage("获取反馈信息失败！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { this.ErrorMessage("拉取数据失败,请刷新重试！"); Response.End(); }
            /*******************************************************************************
             * 获取回复内容信息
             * *****************************************************************************/
            string strContent = RequestHelper.GetRequest("strContent", false).ToString();
            if (string.IsNullOrEmpty(strContent)) { this.ErrorMessage("请填写回复内容！"); Response.End(); }
            if (strContent.Length > 400) { this.ErrorMessage("回复内容请限制在400个汉字以内！"); Response.End(); }
            /*******************************************************************************
             * 开始保存数据
             * *****************************************************************************/
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveGuestList]", new Dictionary<string, object>() {
                {"GuestID",cRs["GuestID"].ToString()},
                {"UserID",MemberRs["UserID"].ToString()},
                {"Nickname",MemberRs["Nickname"].ToString()},
                {"strContent",strContent}
            });
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /******************************************************************************
             * 输出执行结果
             * ****************************************************************************/
            this.ErrorMessage("回复成功!", iSuccess: true);
            Response.End();
        }
    }
}