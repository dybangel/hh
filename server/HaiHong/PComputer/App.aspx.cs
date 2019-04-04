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
    public partial class App : Fooke.Code.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "show": ShowDetails(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 默认加载任务列表数据
        /// </summary>
        protected void strDefault()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            //检测是否有邀请码
            string display = "display:none";
            DataRow smRs = DbHelper.Connection.FindRow("Fooke_User", "UserID", Params: " order by UserID asc");
            if (smRs != null && smRs["UserID"].toString() != MemberRs["UserID"].toString() && MemberRs["ParentID"].ToString() == "0")
            {
                display = "display:block";
                Response.Redirect("index.aspx?token=" + MemberRs["strTokey"] + "&display='" + display + "'");Response.End();
            }
            if (new Fooke.Function.String(MemberRs["DeviceType"].ToString()).toString("define") != "android")
            {
                Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
                string strResponse = Master.Reader("template/app/default.html");
                strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
                {
                    string strValue = string.Empty;
                    switch (funName.ToLower())
                    {
                        case "showInvited": strValue = display; break;
                        default: try { strValue = MemberRs[funName].ToString(); }
                            catch { } break;
                    }
                    return strValue;
                }));
                Response.Write(strResponse);
                Response.End();
            }
            else
            {
                Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
                string strResponse = Master.Reader("template/app/android.html");
                strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
                {
                    string strValue = string.Empty;
                    switch (funName.ToLower())
                    {
                        default: try { strValue = MemberRs[funName].ToString(); }
                            catch { } break;
                    }
                    return strValue;
                }));
                Response.Write(strResponse);
                Response.End();
            }
        }
        /// <summary>
        /// 显示任务详情列表信息
        /// </summary>
        protected void ShowDetails()
        {
            /*****************************************************************************************************************************
             * 获取并验证抢夺应用数据信息
             * ***************************************************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { this.ErrorMessage("获取应用ID信息失败,请重试！"); Response.End(); }
            DataRow ApplicationRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (ApplicationRs == null) { Response.Redirect("App.aspx"); Response.End(); }
            else if (ApplicationRs["isDisplay"].ToString() != "1") { this.ErrorMessage("应用已下架,请抢夺其他的任务吧！"); Response.End(); }
            /*****************************************************************************************************************************
            * 查询当前用户是否已经在做任务了
            * ***************************************************************************************************************************/
            DataRow SessionRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplicationSession]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            if (SessionRs == null) { Response.Redirect("App.aspx"); Response.End(); }
            else if (SessionRs["AppID"].ToString() != ApplicationRs["AppID"].ToString())
            { Response.Redirect("App.aspx"); Response.End(); }
            /*****************************************************************************************************************************
            * 判断任务是否已经超时
            * ***************************************************************************************************************************/
            if (new Fooke.Function.String(SessionRs["ExpireDate"].ToString()).cDate() <= DateTime.Now)
            {
                DbHelper.Connection.ExecuteProc("[Stored_AbortApplicationSession]", new Dictionary<string, object>() {
                    {"UserID",MemberRs["UserID"].ToString()},
                    {"AppID",SessionRs["AppID"].ToString()}
                });
                Response.Redirect("App.aspx");
                Response.End();
            }
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            if (new Fooke.Function.String(MemberRs["DeviceType"].ToString()).toString("define") != "android")
            {
                Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
                string strResponse = Master.Reader("template/app/show.html");
                strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
                {
                    string strValue = string.Empty;
                    switch (funName.ToLower())
                    {
                        case "tasker": strValue = ShowConfig(ApplicationRs, SessionRs).ToString(); break;
                        default: try { strValue = ApplicationRs[funName].ToString(); }
                            catch { } break;
                    }
                    return strValue;
                }));
                Response.Write(strResponse);
                Response.End();
            }
            else
            {
                Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
                string strResponse = Master.Reader("template/app/Androidshow.html");
                strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
                {
                    string strValue = string.Empty;
                    switch (funName.ToLower())
                    {
                        case "tasker": strValue = ShowConfig(ApplicationRs, SessionRs).ToString(); break;
                        default: try { strValue = ApplicationRs[funName].ToString(); }
                            catch { } break;
                    }
                    return strValue;
                }));
                Response.Write(strResponse);
                Response.End();
            }
        }
        /*******************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 公用方法处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *****************************************************************************************/

        #region 公用方法处理区域
        /// <summary>
        /// 获取任务详情页配置信息
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="SessionRs"></param>
        /// <returns></returns>
        public string ShowConfig(DataRow ApplicationRs,DataRow SessionRs) 
        {
            StringBuilder cfgBuilder = new StringBuilder();
            cfgBuilder.Append("<script language=\"javascript\">");
            cfgBuilder.Append("var tasker={");
            /********************************************************************************************************
             * 开始执行数据处理
             * ******************************************************************************************************/
            try
            {
                cfgBuilder.Append("\"success\":\"true\"");
                cfgBuilder.Append(",\"appid\":\"" + ApplicationRs["appId"] + "\"");
                cfgBuilder.Append(",\"userid\":\"" + MemberRs["UserID"] + "\"");
                cfgBuilder.Append(",\"appkey\":\"" + SessionRs["appkey"] + "\"");
                cfgBuilder.Append(",\"expire\":\"" + new Fooke.Function.String(SessionRs["ExpireDate"].ToString()).cDate().ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                cfgBuilder.Append(",\"isDown\":\"" + SessionRs["isDown"] + "\"");
                cfgBuilder.Append(",\"classid\":\"" + ApplicationRs["classId"] + "\"");
                cfgBuilder.Append(",\"classname\":\"" + ApplicationRs["className"] + "\"");
                cfgBuilder.Append(",\"devicemodel\":\"" + ApplicationRs["DeviceModel"] + "\"");
                cfgBuilder.Append(",\"appmodel\":\"" + ApplicationRs["AppModel"] + "\"");
                cfgBuilder.Append(",\"unionmodel\":\"" + ApplicationRs["UnionModel"] + "\"");
                cfgBuilder.Append(",\"advmodel\":\"" + ApplicationRs["AdvModel"] + "\"");
                cfgBuilder.Append(",\"syschar\":\"" + ApplicationRs["sysChar"] + "\"");
                cfgBuilder.Append(",\"modechar\":\"" + ApplicationRs["modeChar"] + "\"");
                cfgBuilder.Append(",\"appname\":\"" + ApplicationRs["AppName"] + "\"");
                cfgBuilder.Append(",\"packername\":\"" + ApplicationRs["Packername"] + "\"");
                cfgBuilder.Append(",\"processname\":\"" + ApplicationRs["Processname"] + "\"");
                cfgBuilder.Append(",\"strinstall\":\"" + ApplicationRs["strInstall"] + "\"");
                cfgBuilder.Append(",\"iskeyword\":\"" + ApplicationRs["isKeyword"] + "\"");
                cfgBuilder.Append(",\"strkeyword\":\"" + ApplicationRs["strKeyword"] + "\"");
                cfgBuilder.Append(",\"thumb\":\"" + FunctionCenter.ConvertPath(ApplicationRs["Thumb"].ToString()) + "\"");
                cfgBuilder.Append(",\"amount\":\"" + ApplicationRs["Amount"] + "\"");
                cfgBuilder.Append(",\"trydate\":\"" + ApplicationRs["TryDate"] + "\"");
                cfgBuilder.Append(",\"softsize\":\"" + ApplicationRs["softsize"] + "\"");
                cfgBuilder.Append(",\"kucun\":\"" + ApplicationRs["Kucun"] + "\"");
                cfgBuilder.Append(",\"softid\":\"" + ApplicationRs["SoftID"] + "\"");
                cfgBuilder.Append(",\"softrank\":\"" + ApplicationRs["softrank"] + "\"");
                cfgBuilder.Append(",\"strcontext\":\"" + ApplicationRs["strContext"] + "\"");
                cfgBuilder.Append(",\"strxml\":\"" + ApplicationRs["strXml"] + "\"");
                cfgBuilder.Append(",\"url\":\"" + System.Web.HttpContext.Current.Request.Url.ToString() + "\"");
            }
            catch { }
            cfgBuilder.Append("}");
            cfgBuilder.Append("</script>");
            /********************************************************************************************************
             * 返回数据处理结果
             * ******************************************************************************************************/
            return cfgBuilder.ToString();
        }

        #endregion
    }
}