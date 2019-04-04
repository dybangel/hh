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
namespace Fooke.Web.Member
{
    public partial class Guest : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.strRequest)
            {
                case "looker": strLooker(); Response.End(); break;
                case "del": Delete(); Response.End(); break;
                case "add": Add(); Response.End(); break;
                case "save": AddSave(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 获取到留言反馈的选项
        /// </summary>
        /// <param name="Fun"></param>
        public void GetOptions(Action<string> Fun)
        {
            try
            {
                StringBuilder strOptions = new StringBuilder();
                string strList = this.GetParameter("titleList", "GuestXml").toString();
                string[] arrList = strList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string oChar in arrList)
                {
                    strOptions.AppendFormat("<option value=\"{0}\">{0}</option>", oChar);
                }
                if (Fun != null) { Fun(strOptions.ToString()); }
            }
            catch { }
        }
        /// <summary>
        /// 发送留言请求
        /// </summary>
        protected void Add()
        {
           
            /*******************************************************************************************
             * 输出网页内容信息
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/guest/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "strTitle": GetOptions((str) => { strValue = str; }); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 关键词回复列表
        /// </summary>
        protected void strDefault()
        {
            /********************************************************************************************
             * 输出网页参数内容
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Guest/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName) { }
                return strValue;
            }));
            Response.Write(strResponse);
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
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/Guest/looker.html");
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
        protected void Delete()
        {
            /*******************************************************************************************
             * 验证请求参数信息
             * ******************************************************************************************/
            string GuestID = RequestHelper.GetRequest("GuestID").toInt();
            if (GuestID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuest]", new Dictionary<string, object>() {
                {"GuestID",GuestID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            if (cRs["UserID"].ToString() != MemberRs["UserID"].ToString()) { this.ErrorMessage("越权操作,请重试！"); Response.End(); }
            /****************************************************************************************
             * 开始删除数据
             * **************************************************************************************/
            DbHelper.Connection.Delete(TableCenter.Guest, Params: " and GuestID=" + GuestID + "");
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
            try
            {
                int starTimer = this.GetParameter("starTimer", "GuestXml").cInt();
                if (starTimer <= 0) { starTimer = 0; }
                if (DateTime.Now.Hour < starTimer) { this.ErrorMessage("反馈时间未到,请等待！"); Response.End(); }
                int endTimer = this.GetParameter("endTimer", "GuestXml").cInt();
                if (endTimer >= 24) { endTimer = 24; }
                if (DateTime.Now.Hour > endTimer) { this.ErrorMessage("今日反馈时间已经过了,请明日再来吧！"); Response.End(); }
            }
            catch { }
            /***********************************************************************************************************************
             * 获取用户提交参数
             * **********************************************************************************************************************/
            string strTitle = RequestHelper.GetRequest("strTitle").toString();
            if (string.IsNullOrEmpty(strTitle)) { this.ErrorMessage("请选择留言类型！"); Response.End(); }
            if (strTitle.Length > 50) { this.ErrorMessage("反馈类型长度请限制在50个汉字以内！"); Response.End(); }
            string strContent = RequestHelper.GetRequest("strContent").toString();
            if (string.IsNullOrEmpty(strContent)) { this.ErrorMessage("请填写留言反馈内容！"); Response.End(); }
            if (strContent.Length > 400) { this.ErrorMessage("反馈备注信息内容长度请限制在400个汉字以内！"); Response.End(); }
            string strMobile = RequestHelper.GetRequest("strMobile").toString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("请填写您的联系电话！"); Response.End(); }
            if (strMobile.Length > 16) { this.ErrorMessage("电话号码字段长度请限制在16个字符以内！"); Response.End(); }
            if (!VerifyCenter.VerifyMobile(strMobile)) { this.ErrorMessage("手机号码格式错误！"); Response.End(); }
            /**********************************************************************************************************************
            * 验证留言内容合法性
            * *********************************************************************************************************************/
            try
            {
                DataRow iRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindGuestToday]", new Dictionary<string, object>() {
                    {"UserID",MemberRs["UserID"].ToString()},
                    {"strTitle",strTitle},
                    {"Today",DateTime.Now.ToString("yyyy-MM-dd 00:00:00")}
                });
                if (iRs == null) { this.ErrorMessage("获取请求数据失败,请重试!"); Response.End(); }
                if (iRs["isExists"].ToString() != "0") { this.ErrorMessage("您的留言信息已提交,请等待客服处理！"); Response.End(); }
                int timer = this.GetParameter("timer", "GuestXml").cInt();
                if (timer != 0 && timer <= iRs["Totay"].ToString().cInt()) { this.ErrorMessage("今日留言次数已经达到上限,请明日再来吧！"); Response.End(); }
            }
            catch { }
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
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"反馈提交成功,我们会尽快处理您的请求!\"");
            strBuilder.Append(",\"type\":\"redirect\"");
            strBuilder.Append(",\"url\":\"Guest.aspx?action=default\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}