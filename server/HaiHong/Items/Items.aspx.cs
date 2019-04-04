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
    public partial class Items : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (strRequest == "default") { this.strDefault(); Response.End(); }
            else if (strRequest == "hits") { this.GetHits(); Response.End(); }
        }
        /// <summary>
        /// 获取点击数
        /// </summary>
        protected void GetHits()
        {
            /***********************************************************************************************************
             * 获取并验证分类栏目参数信息
             * *********************************************************************************************************/
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            if (ChannelID == "0") { Response.Write("document.write('参数错误');"); Response.End(); }
            DataRow channelRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindChannel]", new Dictionary<string, object>() {
                {"ChannelID",ChannelID}
            });
            if (channelRs == null) { Response.Write("document.write('参数错误');"); Response.End(); }
            else if (channelRs["isDisplay"].ToString() != "1") { Response.Write("document.write('参数错误');"); Response.End(); }
            /***********************************************************************************************************
             * 获取并验证文档参数信息
             * *********************************************************************************************************/
            string ShowID = RequestHelper.GetRequest("ShowID").toInt(RequestHelper.GetRequest("ItemsID").toInt());
            if (ShowID == "0") { Response.Write("document.write('参数错误');"); Response.End(); }
            DataRow ShowRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindDocumentChannel]", new Dictionary<string, object>() {
                {"Tablename",channelRs["Tablename"].ToString()},
                {"ShowID",ShowID}
            });
            if (ShowRs == null) { Response.Write("document.write('参数错误');"); Response.End(); }
            else if (ShowRs["isDisplay"].ToString() != "1") { Response.Write("document.write('参数错误');"); Response.End(); }
            /***********************************************************************************************************
            * 开始保存请求数据信息
            * *********************************************************************************************************/
            DbHelper.Connection.Update(channelRs["Tablename"].ToString(), dictionary: new Dictionary<string, string>(){
                {"Hits",(new Function.String(ShowRs["Hits"].ToString()).cInt() + 1).ToString()}
            }, Params: " and ShowID=" + ShowRs["ShowID"] + "");
            /***********************************************************************************************************
            * 输出数据处理结果信息
            * *********************************************************************************************************/
            Response.Write("document.write('" + ShowRs["Hits"] + "');");
            Response.End();
        }
        /// <summary>
        /// 默认文档内容信息
        /// </summary>
        protected void strDefault()
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
            /************************************************************************************************************
             * 判断是否为转向文档,如果是则直接转向
             * **********************************************************************************************************/
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
            /************************************************************************************************************
             * 获取模板地址,开始加载网页内容
             * **********************************************************************************************************/
            string cTemplate = new Fooke.Function.String(ItemsRs["cTemplate"].ToString()).toString("{$Parents}");
            if (cTemplate.Length<=0) { cTemplate = classRs["cTemplate"].ToString(); }
            else if (cTemplate == "{$Parents}") { cTemplate = classRs["cTemplate"].ToString(); }
            string TemplateDirectory = this.GetParameter("TemplateDir", "siteXML").toString("template");
            TemplateDirectory = Win.ApplicationPath + "/" + TemplateDirectory;
            cTemplate = cTemplate.Replace("{@dir}", TemplateDirectory);
            /************************************************************************************************************
             * 解析模板内容信息
             * **********************************************************************************************************/
            Fooke.Release.ReleaseHelper ReleaseMaster = new Release.ReleaseHelper();
            string strResponse = ReleaseMaster.ReleaseContext(strTemplate: cTemplate,
                ShowRs: ItemsRs,
                classRs: classRs,
                channelRs: channelRs);
            Response.Write(strResponse);
            Response.End();
        }
    }
}