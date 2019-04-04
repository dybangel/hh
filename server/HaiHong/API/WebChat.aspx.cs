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
namespace Fooke.Web.API
{
    public partial class WebChat : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "save": SaveChat(); Response.End(); break;
                case "unalready": FindRlready(); Response.End(); break;
                case "unalreadyList": FindRlreadyList(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }

        protected void FindRlreadyList()
        {
            /****************************************************************************************************************
             * 获取交易订单号ID信息
             * **************************************************************************************************************/
            string OrderID = RequestHelper.GetRequest("OrderID").toString();
            if (string.IsNullOrEmpty(OrderID)) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (OrderID.Length <= 4) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (OrderID.Length >= 10) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /****************************************************************************************************************
             * 统计用户聊天咨询未读消息列表
             * **************************************************************************************************************/
            string OrderMode = RequestHelper.GetRequest("OrderMode").toString("聊天咨询");
            if (string.IsNullOrEmpty(OrderMode)) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            else if (OrderMode.Length <= 0) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            else if (OrderMode.Length >= 10) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            /****************************************************************************************************************
             * 开始查询请求数据信息
             * **************************************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindWebChatRlreadyList]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"OrderID",OrderID}
            });
            /****************************************************************************************************************
             * 输出数据处理结果信息
             * **************************************************************************************************************/
            ResponseDataTable(thisTab);
            Response.End();
        }

        /// <summary>
        /// 查看未读消息数量
        /// </summary>
        protected void FindRlready()
        {
            /****************************************************************************************************************
             * 获取交易订单号ID信息
             * **************************************************************************************************************/
            string OrderID = RequestHelper.GetRequest("OrderID").toString();
            if (string.IsNullOrEmpty(OrderID)) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (OrderID.Length <= 4) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (OrderID.Length >= 10) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }

            string OrderMode = RequestHelper.GetRequest("OrderMode").toString("聊天咨询");
            if (string.IsNullOrEmpty(OrderMode)) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            else if (OrderMode.Length <= 0) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            else if (OrderMode.Length >= 10) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            /****************************************************************************************************************
             * 开始查询请求数据信息
             * **************************************************************************************************************/
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWebChatRlready]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"OrderID",OrderID}
            });
            if (sRs == null) { this.ErrorMessage("获取查询数据信息失败,请重试！"); Response.End(); }
            /****************************************************************************************************************
             * 输出数据处理结果信息
             * **************************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"number\":\"" + sRs["number"] + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取聊天内容信息
        /// </summary>
        protected void strDefault()
        {
            /****************************************************************************************************************
             * 获取并验证接收消息用户信息
             * **************************************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (UserID == MemberRs["UserID"].ToString()) { this.ErrorMessage("您不能与自己聊天！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_Findmember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs == null) { this.ErrorMessage("当前账号已停止使用,请联系客服！"); Response.End(); }
            /****************************************************************************************************************
             * 获取交易订单号ID信息
             * **************************************************************************************************************/
            string OrderID = RequestHelper.GetRequest("OrderID").toString();
            if (string.IsNullOrEmpty(OrderID)) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (OrderID.Length <= 4) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (OrderID.Length >= 10) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }

            string OrderMode = RequestHelper.GetRequest("OrderMode").toString("聊天咨询");
            if (string.IsNullOrEmpty(OrderMode)) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            else if (OrderMode.Length <= 0) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            else if (OrderMode.Length >= 10) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            /****************************************************************************************************************
             * 获取交易订单号ID信息
             * **************************************************************************************************************/
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindWebChat]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"FormID",cRs["UserID"].ToString()},
                {"OrderID",OrderID}
            });
            /**************************************************************************************
            * 输出数据处理结果
            * ************************************************************************************/
            ResponseDataTable(thisTab);
            Response.End();
        }

        /// <summary>
        /// 处理请求数据
        /// </summary>
        protected void SaveChat()
        {
            /****************************************************************************************************************
             * 获取并验证接收消息用户信息
             * **************************************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (UserID == MemberRs["UserID"].ToString()) { this.ErrorMessage("您不能与自己聊天！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_Findmember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs == null) { this.ErrorMessage("当前账号已停止使用,请联系客服！"); Response.End(); }
            /****************************************************************************************************************
             * 获取交易订单号ID信息
             * **************************************************************************************************************/
            string OrderID = RequestHelper.GetRequest("OrderID").toString();
            if (string.IsNullOrEmpty(OrderID)) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (OrderID.Length <=4) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (OrderID.Length >= 10) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }

            string OrderMode = RequestHelper.GetRequest("OrderMode").toString("聊天咨询");
            if (string.IsNullOrEmpty(OrderMode)) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            else if (OrderMode.Length <= 0) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            else if (OrderMode.Length >= 10) { this.ErrorMessage("获取聊天类型失败！"); Response.End(); }
            /****************************************************************************************************************
             * 获取发送的聊天内容数据信息
             * **************************************************************************************************************/
            string strContent = RequestHelper.GetRequest("strContent", false).ToString();
            if (string.IsNullOrEmpty(strContent)) { this.ErrorMessage("请填写回复内容！"); Response.End(); }
            else if (strContent.Length > 1200) { this.ErrorMessage("回复内容请限制在1200个汉字以内！"); Response.End(); }
            else if (!strContent.StartsWith("<configurationRoot>")) { this.ErrorMessage("发送聊天内容格式信息错误！"); Response.End(); }
            else if (!strContent.EndsWith("</configurationRoot>")) { this.ErrorMessage("发送聊天内容格式信息错误！"); Response.End(); }
            /****************************************************************************************************************
             * 开始保存请求数据
             * **************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["UserID"] = cRs["UserID"].ToString();
            thisDictionary["Nickname"] = cRs["Nickname"].ToString();
            thisDictionary["FormID"] = MemberRs["UserID"].ToString();
            thisDictionary["Formname"] = MemberRs["Nickname"].ToString();
            thisDictionary["OrderID"] = OrderID;
            thisDictionary["OrderMode"] = OrderMode;
            thisDictionary["strContext"] = strContent;
            DbHelper.Connection.ExecuteProc("[Stored_SaveWebChat]", thisDictionary);
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"userid\":\"" + cRs["userid"] + "\"");
            strBuilder.Append(",\"nickname\":\"" + cRs["Nickname"] + "\"");
            strBuilder.Append(",\"formid\":\"" + MemberRs["UserID"] + "\"");
            strBuilder.Append(",\"formname\":\"" + MemberRs["Nickname"] + "\"");
            strBuilder.Append(",\"orderid\":\"" + OrderID + "\"");
            strBuilder.Append(",\"ordermode\":\"" + OrderMode + "\"");
            strBuilder.Append(",\"strcontext\":\"" + strContent + "\"");
            strBuilder.Append(",\"formthumb\":\"" + MemberRs["Thumb"] + "\"");
            strBuilder.Append(",\"addtime\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}