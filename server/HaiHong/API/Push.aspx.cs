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
    /// <summary>
    /// 给指定的用户发送推送
    /// </summary>
    public partial class Push : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "save": SavePusher(); Response.End(); break;
            }
        }
        
        /// <summary>
        /// 开始发送
        /// </summary>
        protected void SavePusher()
        {
            /****************************************************************************************************************
             * 获取并验证收货地址信息
             * **************************************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /****************************************************************************************************************
             * 获取手机号码数据信息
             * **************************************************************************************************************/
            string strTitle = RequestHelper.GetRequest("strTitle").toString();
            if (string.IsNullOrEmpty(strTitle)) { this.ErrorMessage("推送内容主题不能为空!"); Response.End(); }
            else if (strTitle.Length <= 0) { this.ErrorMessage("推送内容主题不能为空!"); Response.End(); }
            else if (strTitle.Length >= 40) { this.ErrorMessage("推送内容主题长度不能超过40个汉字!"); Response.End(); }
            /****************************************************************************************************************
             * 获取手机号码数据信息
             * **************************************************************************************************************/
            string strContext = RequestHelper.GetRequest("strContext").toString();
            if (string.IsNullOrEmpty(strContext)) { this.ErrorMessage("推送内容不能为空!"); Response.End(); }
            else if (strContext.Length <= 0) { this.ErrorMessage("推送内容不能为空!"); Response.End(); }
            else if (strContext.Length >= 100) { this.ErrorMessage("推送内容长度不能超过100个汉字!"); Response.End(); }
            /****************************************************************************************************************
             * 开始发送推送
             * **************************************************************************************************************/
            new PushCenter().Start(Configure: this.Configure,
                    content: strContext,
                    identify: cRs["UserID"].ToString(),
                    appMode: RequestHelper.GetRequest("appMode").toString("default"),
                    appId: RequestHelper.GetRequest("appId").toInt());
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            this.ErrorMessage("推送内容发送成功!", iSuccess: true);
            Response.End();
        }
    }
}