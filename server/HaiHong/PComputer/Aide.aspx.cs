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
    /// <summary>
    /// 小助手模拟器
    /// </summary>
    public partial class Aide : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "download": SaveDownload(); Response.End(); break;
                case "verification": SaveVerification(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 提交审核信息
        /// </summary>
        protected void SaveVerification()
        {
            /*****************************************************************************************************************************
            * 开始验证appkey的合法性
            * ***************************************************************************************************************************/
            string appKey = RequestHelper.GetRequest("appKey").toString();
            if (string.IsNullOrEmpty(appKey)) { AideMessage("获取请求参数错误，请重试！"); Response.End(); }
            else if (appKey.Length <= 0) { AideMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (appKey.Length <= 20) { AideMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (appKey.Length >= 32) { AideMessage("获取请求参数错误,请重试！"); Response.End(); }
            /*****************************************************************************************************************************
            * 获取并验证下载的app参数信息
            * ***************************************************************************************************************************/
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppDown]", new Dictionary<string, object>() {
                {"appKey",appKey}
            });
            if (cRs == null) { this.ErrorMessage("请求参数错误，你查找的数据不存在！"); Response.End(); }
            else if (cRs["isFinish"].ToString() == "1") { AideMessage("success", true); Response.End(); }

            AideMessage("请尝试其他任务", false); Response.End();
            /*****************************************************************************************************************************
            * 输出数据处理结果信息
            * ***************************************************************************************************************************/
            AideMessage("任务已成功提交，请等待审核", success: true);
            Response.End();
        }

        public void AideMessage(string strTips, bool success = false)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"" + success.ToString().ToLower() + "\"");
            strBuilder.Append(",\"tips\":\"" + strTips + "\"");
            strBuilder.Append("}");
            Response.Write(RequestHelper.GetRequest("callback").toString());
            Response.Write("(" + strBuilder.ToString() + ")");
            Response.End();
        }

        /// <summary>
        /// 请求小助手数据信息
        /// </summary>
        protected void strDefault() {

            Response.Write(RequestHelper.GetRequest("callback").toString());
            Response.Write("(");
            Response.Write("{\"success\":\"true\"}");
            Response.Write(")");
            Response.End();
        }
        /// <summary>
        /// 下载任务
        /// </summary>
        protected void SaveDownload()
        {
            /*****************************************************************************************************************************
             * 获取并验证抢夺应用数据信息
             * ***************************************************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { this.ErrorMessage("获取应用ID信息失败,请重试！"); Response.End(); }
            DataRow ApplicationRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (ApplicationRs == null) { this.ErrorMessage("获取应用信息失败,请重试！"); Response.End(); }
            else if (ApplicationRs["isDisplay"].ToString() != "1") { this.ErrorMessage("应用已下架,请抢夺其他的任务吧！"); Response.End(); }
            /*****************************************************************************************************************************
            * 开始验证appkey的合法性
            * ***************************************************************************************************************************/
            string appKey = RequestHelper.GetRequest("appKey").toString();
            if (string.IsNullOrEmpty(appKey)) { this.ErrorMessage("获取请求参数错误，请重试！"); Response.End(); }
            else if (appKey.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (appKey.Length <= 20) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (appKey.Length >= 32) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }

            /*****************************************************************************************************************************
            * 查询当前用户是否已经在做任务了
            * ***************************************************************************************************************************/
            DataRow SessionRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplicationSession]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"AppKey",appKey}
            });
            if (SessionRs == null) { Response.Redirect("App.aspx"); Response.End(); }
            /*****************************************************************************************************************************
            * 开始请求下载数据
            * ***************************************************************************************************************************/
            string strResponse = "error";
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    string forceKey = string.Format("{0}-|-|-{0}-|-|-{2}-|-|-{3}-|-|-hzxiaz888999hz",
                    appKey, MemberRs["DeviceIdentifier"], ApplicationRs["AppId"].ToString(), MemberRs["UserID"].ToString());
                    forceKey = new Fooke.Function.String(forceKey).ToMD5().ToLower();

                    thisClient.Encoding = System.Text.Encoding.UTF8;
                    strResponse = thisClient.DownloadString(string.Format("{0}{1}/api/app.aspx?action=start&thisKey={4}&AppID={2}&appKey={3}&force={5}&strEdition={6}",
                    FunctionCenter.SiteUrl(), Win.ApplicationPath, AppID, appKey, MemberRs["strTokey"].ToString(), forceKey,MemberRs["DeviceModel"]));
                }
                catch (Exception exp) { strResponse = string.Format("error:{0}", exp.Message); }
                finally { thisClient.Dispose(); }
            }
            /*****************************************************************************************************************************
            * 验证返回数据结果的合法性
            * ***************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { Response.Write("发送数据下载请求发生未知错误！"); Response.End(); }
            else if (strResponse.Length<=0) { Response.Write("发送数据下载请求发生未知错误！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { Response.Write("发送数据下载请求发生未知错误！"); Response.End(); }
            Response.Write(strResponse); Response.End();
            /*****************************************************************************************************************************
            * 返回到下载跳转页面
            * ***************************************************************************************************************************/
            Response.Redirect("app.aspx?action=show&appid=" + ApplicationRs["appid"] + "");
            Response.End();
        }
    }
}