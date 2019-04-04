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
    public partial class App : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "class": strClass(); Response.End(); break;
                case "toRedirect": SaveRedirect(); Response.End(); break;
                case "upload": SaveUpload(); Response.End(); break;
                case "submission": Submission(); Response.End(); break;
                case "abort": AbortSession(); Response.End(); break;
                case "snatch": SaveSession(); Response.End(); break;
                case "start": Start(); Response.End(); break;
                case "thiss": ApplicationWaiting(); Response.End(); break;
                case "default": strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 跳转到指定的应用地址
        /// </summary>
        protected void SaveRedirect()
        {
            /*******************************************************************************************************
             * 验证请求参数信息
             * *****************************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /*******************************************************************************************************
             * 更新应用点击次数以及最后更新日期
             * *****************************************************************************************************/
            try
            {
                DbHelper.Connection.Update("Fooke_Application", dictionary: new Dictionary<string, string>() {
                    {"Hits",(new Fooke.Function.String(Rs["Hits"].ToString()).cInt()+1).ToString()},
                    {"Lastdate",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}
                }, Params: " and AppID=" + Rs["AppID"] + "");
            }
            catch { }
            /*******************************************************************************************************
             * 输出网页内容数据信息
             * *****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/application/show.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 获取生活服务分类数据信息
        /// </summary>
        protected void strClass()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("[");
            /*************************************************************************************************
             * 开始执行请求数据信息
             * ***********************************************************************************************/
            int SelectedIndex = 0;
            new Fooke.Code.AppClassHelper().Each(Fun: (cRs, rowIndex, sChar) =>
            {
                if (SelectedIndex != 0) { strBuilder.Append(","); }
                strBuilder.Append("{");
                strBuilder.Append("\"classid\":\"" + cRs["classId"] + "\"");
                strBuilder.Append(",\"classname\":\"" + cRs["classname"] + "\"");
                strBuilder.Append(",\"parentid\":\"" + cRs["parentid"] + "\"");
                strBuilder.Append(",\"ischild\":\"" + cRs["isChild"] + "\"");
                strBuilder.Append("}");
                SelectedIndex = SelectedIndex + 1;
            },
            ParentID: "0",
            SparChar: "");
            /*************************************************************************************************
             * 输出数据处理结果
             * ***********************************************************************************************/
            strBuilder.Append("]");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取服务应用列表
        /// </summary>
        protected void strDefault()
        {
            /***********************************************************************************************
             * 开始执行数据查询
             * **********************************************************************************************/
            DataRow Rs = DbHelper.Connection.FindRow("Fooke_Udid",Params:" and Udid = '"+MemberRs["DeviceIdentifier"].ToString()+"'");
            if(Rs == null){Response.Write("{\"success\":\"true\",\"result\":\"[{}]\"}");Response.End();}
            DataTable thisTab = DbHelper.Connection.ExecuteFindTable("[Stored_FindApplicationList]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"DeviceModel",(MemberRs["DeviceType"].ToString().ToLower() == "ios" ? "苹果系统" : "安卓系统")}
            });
            /***********************************************************************************************
            * 开始遍历网页内容信息
            * **********************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\"success\":\"true\",\"result\":[");
            int SelectedIndex = 0;
            foreach (DataRow cRs in thisTab.Rows)
            {
                if (SelectedIndex != 0) { strBuilder.Append(","); }
                SelectedIndex = SelectedIndex + 1;
                strBuilder.Append("{");
                strBuilder.Append("\"appmenu\":\"" + cRs["appmenu"] + "\"");
                strBuilder.Append(",\"appid\":\"" + cRs["appid"] + "\"");
                strBuilder.Append(",\"classname\":\"" + cRs["classname"] + "\"");
                strBuilder.Append(",\"devicemodel\":\"" + cRs["DeviceModel"] + "\"");
                strBuilder.Append(",\"advmodel\":\"" + cRs["AdvModel"] + "\"");
                strBuilder.Append(",\"syschar\":\"" + cRs["sysChar"] + "\"");
                strBuilder.Append(",\"taskermodel\":\"" + cRs["Taskermodel"] + "\"");
                strBuilder.Append(",\"modechar\":\"" + cRs["modeChar"] + "\"");
                strBuilder.Append(",\"kucun\":\"" + cRs["Kucun"] + "\"");
                strBuilder.Append(",\"appname\":\"" + cRs["AppName"] + "\"");
                strBuilder.Append(",\"packername\":\"" + cRs["Packername"] + "\"");
                strBuilder.Append(",\"processname\":\"" + cRs["Processname"] + "\"");
                strBuilder.Append(",\"thumb\":\"" + cRs["Thumb"] + "\"");
                strBuilder.Append(",\"amount\":\"" + cRs["Amount"] + "\"");
                strBuilder.Append(",\"isrec\":\"" + cRs["isRec"] + "\"");
                strBuilder.Append(",\"addtime\":\"" + cRs["Addtime"] + "\"");
                strBuilder.Append(",\"isare\":\"" + cRs["isAre"] + "\"");
                strBuilder.Append("}");
            }
            strBuilder.Append("]}");
            /***********************************************************************************************
            * 输出数据处理结果信息
            * **********************************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 获取当前用户正在执行的任务
        /// </summary>
        protected void ApplicationWaiting()
        {
            /****************************************************************************************
             * 输出网页内容信息
             * ***************************************************************************************/
            ResponseDataTable(DbHelper.Connection.ExecuteFindTable("[Stored_FindApplicationWaiting]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            }));
            Response.End();
            /****************************************************************************************
             * 接口数据执行完成
             * ***************************************************************************************/
        }

        /*******************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 任务处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *****************************************************************************************/
        /// <summary>
        /// 开始下载任务
        /// </summary>
        protected void Start()
        {
            /****************************************************************************************
             * 获取并验证APPID的合法性
             * ***************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppId").toInt();
            if (AppID == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow ApplicationRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (ApplicationRs == null) { this.ErrorMessage("获取应用信息失败,请重试！"); Response.End(); }
            else if (ApplicationRs["isDisplay"].ToString() != "1") { this.ErrorMessage("应用已下架,请抢夺其他的任务吧！"); Response.End(); }
            /****************************************************************************************
            * 验证用户抢夺任务是否过期
            * ***************************************************************************************/
            DataRow SessionRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplicationSession]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            if (SessionRs == null) { this.ErrorMessage("任务已过期,请重新抢夺！"); Response.End(); }
            else if ((SessionRs != null && SessionRs["AppID"].ToString() != AppID)) { this.ErrorMessage("你还没有抢夺当前的任务哦！"); Response.End(); }
            else if (new Fooke.Function.String(SessionRs["ExpireDate"].ToString()).cDate() <= DateTime.Now.AddMinutes(-3))
            { this.ErrorMessage("任务已过期,请重新抢夺！"); Response.End(); }
            /****************************************************************************************
            * 获取下载应用APPKey值
            * ***************************************************************************************/
            string AppKey = RequestHelper.GetRequest("appKey").ToString();
            if (string.IsNullOrEmpty(AppKey)) { this.ErrorMessage("获取下载应用标识信息失败！"); Response.End(); }
            else if (AppKey.Length <= 0) { this.ErrorMessage("获取下载应用标识信息失败!"); Response.End(); }
            else if (AppKey.Length <= 20) { this.ErrorMessage("获取下载应用标识信息失败!"); Response.End(); }
            else if (AppKey.Length >= 32) { this.ErrorMessage("获取下载应用标识信息失败!"); Response.End(); }
            if (AppKey != SessionRs["AppKey"].ToString()) { this.ErrorMessage("数据验证失败,请重试!"); Response.End(); }
            /****************************************************************************************
            * 验证请求数据的合法性
            * ***************************************************************************************/
            string force = RequestHelper.GetRequest("force").toString();
            if (string.IsNullOrEmpty(force)) { this.ErrorMessage("请求参数错误，请返回重试?"); Response.End(); }
            else if (force.Length <= 0) { this.ErrorMessage("请求参数错误，请返回重试?"); Response.End(); }
            else if (force.Length != 32) { this.ErrorMessage("请求参数错误，请返回重试?"); Response.End(); }
            string forceKey = string.Format("{0}-|-|-{1}-|-|-{2}-|-|-{3}-|-|-hzxiaz888999hz",
                AppKey, MemberRs["DeviceIdentifier"], ApplicationRs["AppId"].ToString(), MemberRs["UserID"].ToString());
            forceKey = new Fooke.Function.String(forceKey).ToMD5().ToLower();
            if (force != forceKey) { this.ErrorMessage("请求参数错误，数据验证不通过"); Response.End(); }
            /****************************************************************************************
            * 获取用户操作系统版本
            * ***************************************************************************************/
            string strEdition = RequestHelper.GetRequest("strEdition").toString();
            if (strEdition.Length <= 0) { this.ErrorMessage("获取手机系统版本失败!"); Response.End(); }
            else if (strEdition.Length >= 20) { this.ErrorMessage("获取手机系统版本失败!"); Response.End(); }
            string strNetwork = RequestHelper.GetRequest("strNetwork").toString("未知");
            if (strNetwork.Length <= 0) { this.ErrorMessage("获取设备网卡类型失败!"); Response.End(); }
            else if (strNetwork.Length >= 20) { this.ErrorMessage("获取设备网卡类型失败!"); Response.End(); }
            string isBreak = RequestHelper.GetRequest("isBreak").toInt();
            //获取城市
            string strIp = FunctionCenter.GetCustomerIP();
            if (string.IsNullOrEmpty(strIp)) { strIp = FunctionCenter.GetCustomerIP(); }
            string City = new IPHelper().ConvertToCity(strIp).Country;
            /****************************************************************************************
            * 生成一个strToken验证用户是否已经下载应用
            * ***************************************************************************************/
            string strToken = string.Format("应用渠道-|-|-{0}-|-|-{1}-|-|-{2}-|-|-应用渠道",
                MemberRs["UserID"].ToString(), ApplicationRs["AppID"].ToString(), "0");
            strToken = new Fooke.Function.String(strToken).ToMD5().Substring(0, 24).ToUpper();
            /****************************************************************************************
            * 验证用户是否已经下载任务
            * ***************************************************************************************/
            DataRow dRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppDown]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"strToken",strToken}
            });
            if (dRs != null && dRs["isFinish"].ToString() == "1") { this.ErrorMessage("你已经完成过这个任务了！"); Response.End(); }
            else if (dRs != null) { StartRedirect(AppKey, ApplicationRs); Response.End(); }
            /****************************************************************************************
            * 点击通知信息
            * ***************************************************************************************/
            if (ApplicationRs["appModel"].ToString() == "2") { DoClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["appModel"].ToString() == "3") { DoClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["appModel"].ToString() == "4") { DoClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["appModel"].ToString() == "6") { DoClick(ApplicationRs, MemberRs, AppKey); }
            /****************************************************************************************
            * 开始保存请求数据下载信息
            * ***************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["strToken"] = strToken;
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["AppID"] = ApplicationRs["AppId"].ToString();
            thisDictionary["Appname"] = ApplicationRs["Appname"].ToString();
            thisDictionary["AppKey"] = AppKey;
            thisDictionary["AppModel"] = ApplicationRs["AppModel"].ToString();
            thisDictionary["UnionModel"] = ApplicationRs["UnionModel"].ToString();
            thisDictionary["DeviceType"] = MemberRs["DeviceType"].ToString();
            thisDictionary["DeviceCode"] = MemberRs["DeviceCode"].ToString();
            thisDictionary["MacChar"] = MemberRs["MacChar"].ToString();
            thisDictionary["Amount"] = ApplicationRs["Amount"].ToString();
            thisDictionary["strIP"] = strIp;
            thisDictionary["cityname"] = City.toString("未知城市");
            thisDictionary["strInstall"] = (ApplicationRs["isKeyword"].ToString() == "1" ? ApplicationRs["isKeyword"].ToString() : ApplicationRs["strInstall"].ToString());
            thisDictionary["strEdition"] = strEdition;
            thisDictionary["strNetwork"] = strNetwork;
            thisDictionary["isBreak"] = MemberRs["isBreak"].ToString();
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAppDown]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("服务器系统繁忙,请稍后重试!"); Response.End(); }
            /****************************************************************************************
            * 输出数据处理结果
            * ***************************************************************************************/
            StartRedirect(AppKey, ApplicationRs);
            Response.End();
        }

        /// <summary>
        /// 开始跳转下载
        /// </summary>
        /// <param name="Rs"></param>
        protected void StartRedirect(string appKey, DataRow ApplicationRs)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"请求成功\"");
            strBuilder.Append(",\"appkey\":\"" + appKey + "\"");
            strBuilder.Append(",\"appname\":\"" + ApplicationRs["appname"] + "\"");
            strBuilder.Append(",\"thumb\":\"" + ApplicationRs["thumb"] + "\"");
            strBuilder.Append(",\"processname\":\"" + ApplicationRs["Processname"] + "\"");
            strBuilder.Append(",\"isback\":\"0\"");
            strBuilder.Append(",\"package\":\"" + ApplicationRs["Packername"] + "\"");
            strBuilder.Append(",\"time\":\"" + ApplicationRs["TryDate"] + "\"");
            strBuilder.Append(",\"trydate\":\"" + ApplicationRs["TryDate"] + "\"");
            strBuilder.Append(",\"iskey\":\"" + ApplicationRs["isKeyword"] + "\"");
            strBuilder.Append(",\"iskeyword\":\"" + ApplicationRs["isKeyword"] + "\"");
            strBuilder.Append(",\"strkeyword\":\"" + ApplicationRs["strKeyword"] + "\"");
            strBuilder.Append(",\"strinstall\":\"" + ApplicationRs["strInstall"] + "\"");
            strBuilder.Append(",\"appmodel\":\"" + ApplicationRs["AppModel"] + "\"");
            strBuilder.Append(",\"unionmodel\":\"" + ApplicationRs["UnionModel"] + "\"");
            strBuilder.Append(",\"advmodel\":\"" + ApplicationRs["AdvModel"] + "\"");
            strBuilder.Append(",\"devicemodel\":\"" + ApplicationRs["DeviceModel"] + "\"");
            strBuilder.Append(",\"search\":\"itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/search?\"");
            string strBuild = "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/search?";
            if (ApplicationRs["isKeyword"].ToString() != "1") { strBuild = ApplicationRs["strInstall"].ToString(); }
            strBuilder.Append(",\"url\":\"" + strBuild + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /*******************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 显示数据处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *****************************************************************************************/
        /// <summary>
        /// 抢夺任务信息
        /// </summary>
        protected void SaveSession()
        {

            /*****************************************************************************************************************************
             * 获取并验证抢夺应用数据信息
             * ***************************************************************************************************************************/
            string AppID = RequestHelper.GetRequest("AppID").toInt();
            if (AppID == "0") { this.ErrorMessage("获取应用ID信息失败,请重试！"); Response.End(); }
            /*****************************************************************************************************************************
             * 获取并判断用户是否存在唯一应用请求,即同一用户同时只能抢夺一个任务
             * ***************************************************************************************************************************/
            DataRow SessionRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplicationSession]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            if (SessionRs != null)
            {
                if (new Fooke.Function.String(SessionRs["ExpireDate"].ToString()).cDate() <= DateTime.Now)
                {
                    DbHelper.Connection.ExecuteProc("[Stored_AbortApplicationSession]", new Dictionary<string, object>() {
                        {"UserID",MemberRs["UserID"].ToString()},
                        {"AppID",SessionRs["AppID"].ToString()}
                    });
                }
                else
                {
                    if (SessionRs["AppID"].ToString() == AppID)
                    {
                        ResponseDataRow(SessionRs); Response.End();
                    }
                    else
                    {
                        Response.Write("{\"success\":\"true\",\"type\":\"define\",\"tips\":\"exists\"}");
                        Response.End();
                    }
                }
            }
            /*****************************************************************************************************************************
             * 查询应用ID是否合法
             * ***************************************************************************************************************************/
            DataRow ApplicationRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",AppID}
            });
            if (ApplicationRs == null) { this.ErrorMessage("获取应用信息失败,请重试！"); Response.End(); }
            else if (ApplicationRs["isDisplay"].ToString() != "1") { this.ErrorMessage("应用已下架,请抢夺其他的任务吧！"); Response.End(); }
            /*****************************************************************************************************************************
             * 验证当前应用的库存信息是否合法
             * ***************************************************************************************************************************/
            int Kucun = new Fooke.Function.String(ApplicationRs["Kucun"].ToString()).cInt();
            if (Kucun <= 0) { this.ErrorMessage("任务已经被抢完了,你手慢了呀！"); Response.End(); }
            /*****************************************************************************************************************************
             * 生成指定的AppKey值数据
             * ***************************************************************************************************************************/
            string AppKey = new Fooke.DMFTasker.AppHelper().GetKey(MemberRs["UserID"].ToString(), AppID);
            if (string.IsNullOrEmpty(AppKey)) { this.ErrorMessage("获取应用标识信息失败,请重试!"); Response.End(); }
            else if (AppKey.Length <= 20) { this.ErrorMessage("获取应用标识信息失败,请重试!"); Response.End(); }
            else if (AppKey.Length >= 34) { this.ErrorMessage("获取应用标识信息失败,请重试!"); Response.End(); }
            /*****************************************************************************************************************************
             * 设置用户排重上报功能
             * ***************************************************************************************************************************/
            DataRow KeyRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplicationKey]", new Dictionary<string, object>() {
                {"appKey",AppKey}
            });
            if (KeyRs == null) { this.ErrorMessage("服务器系统繁忙,请稍后再试！"); Response.End(); }
            else if (new Fooke.Function.String(KeyRs["isFind"].ToString()).cInt() <= 0)
            {
                if (ApplicationRs["appModel"].ToString() == "2") { DoRepeat(ApplicationRs, MemberRs, AppKey); }
                else if (ApplicationRs["appModel"].ToString() == "3") { DoRepeat(ApplicationRs, MemberRs, AppKey); }
                else if (ApplicationRs["appModel"].ToString() == "4") { DoRepeat(ApplicationRs, MemberRs, AppKey); }
                else if (ApplicationRs["appModel"].ToString() == "6") { DoRepeat(ApplicationRs, MemberRs, AppKey); }
            }
            /*****************************************************************************************************************************
             * 开始保存抢夺数据信息
             * ***************************************************************************************************************************/
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveApplicationSession]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"AppID",AppID},
                {"AppKey",AppKey},
            });
            if (thisRs == null) { this.ErrorMessage("任务已经被抢光,你动手太慢了！"); Response.End(); }
            /*****************************************************************************************************************************
             * 开始执行数据跳转信息
             * ***************************************************************************************************************************/
            ResponseDataRow(thisRs);
            Response.End();
        }

        /// <summary>
        /// 终止应用信息
        /// </summary>
        protected void AbortSession()
        {
            /*****************************************************************************************************************************
             * 获取并判断用户是否存在唯一应用请求,即同一用户同时只能抢夺一个任务
             * ***************************************************************************************************************************/
            DataRow SessionRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplicationSession]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            if (SessionRs == null)
            {
                Response.Write("{");
                Response.Write("\"success\":\"true\"");
                Response.Write(",\"tips\":\"success\"");
                Response.Write(",\"type\":\"define\"");
                Response.Write("}");
                Response.End();
            }
            /*****************************************************************************************************************************
             * 开始终止当前的请求数据
             * ***************************************************************************************************************************/
            DbHelper.Connection.ExecuteProc("[Stored_AbortApplicationSession]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"AppID",SessionRs["AppID"].ToString()}
            });
            /*****************************************************************************************************************************
             * 输出数据处理结果信息
             * ***************************************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"type\":\"define\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /*******************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 抢夺任务接口排重
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *****************************************************************************************/
        #region 抢夺任务接口排重
        /// <summary>
        /// 任务排重
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void DoRepeat(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            if (ApplicationRs["UnionModel"].ToString() == "微网") { WNETRepeat(ApplicationRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "巨掌") { JUZRepeat(ApplicationRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "博睿") { BORRepeat(ApplicationRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "来赚") { LAIZRepeat(ApplicationRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "有为") { YOUWRepeat(ApplicationRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "掌上互动") { ZSHDRepeat(ApplicationRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "壹狗") { YIGOURepeat(ApplicationRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "海尔") { HaiErRepeat(ApplicationRs, MemberRs); }
        }

        /// <summary>
        /// 微网排重方式
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void WNETRepeat(DataRow ApplicationRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /**************************************************************************************************************************************************
             * 获取接口请求地址
             * ************************************************************************************************************************************************/
            string url = string.Format("http://centralapi.51welink.com/api/Welink/downstream_check_idfa?AppleId={0}&ChannelID={4}&IDFA={1}&IP={2}&MAC={3}",
                ApplicationRs["SoftID"].ToString(), MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
                MemberRs["MacChar"].ToString(), Fooke.DMFTasker.UnionConfig.Weiwang.source);
            if (new Fooke.Function.String(ApplicationRs["isWeight"].ToString()).toInt() == "2")
            {
                url = string.Format("http://centralapi.51welink.com/api/Welink/check_idfa_inreportdata?AppleId={0}&ChannelID={4}&IDFA={1}&IP={2}&MAC={3}",
                ApplicationRs["SoftID"].ToString(), MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
                MemberRs["MacChar"].ToString(), Fooke.DMFTasker.UnionConfig.Weiwang.source);
            }
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("status")) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("微网:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"] == null) { this.ErrorMessage("微网:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"].toString() == "false") { this.ErrorMessage("微网:该任务你已经下载过,请抢夺其它任务吧！"); Response.End(); }
            else if (rspJson["idfa"] == null) { this.ErrorMessage("微网:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["idfa"].toString() == "1") { this.ErrorMessage("微网:该任务你已经下载过,请抢夺其它任务吧！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网排重接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 巨掌排重方式
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void JUZRepeat(DataRow ApplicationRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /**************************************************************************************************************************************************
             * 获取接口请求地址
             * ************************************************************************************************************************************************/
            string url = string.Format("http://api.plat.adjuz.net/distinct?adid={0}&sourceid={3}&idfa={1}&ip={2}&mac=02:00:00:00:00:00",
                ApplicationRs["ThirdID"].ToString(), MemberRs["DeviceCode"].ToString(),
                FunctionCenter.GetCustomerIP(), Fooke.DMFTasker.UnionConfig.Juzhang.sourceid);
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains(MemberRs["DeviceCode"].ToString())) { this.ErrorMessage("微网:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("巨掌:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson[MemberRs["DeviceCode"]] == null) { this.ErrorMessage("巨掌:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson[MemberRs["DeviceCode"]].toString() == "1") { this.ErrorMessage("巨掌:该任务你已经下载过,请抢夺其它任务吧！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 巨掌排重接口执行完成
             * ****************************************************************************************************************************************************/
        }
        /// <summary>
        /// 博睿
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void BORRepeat(DataRow ApplicationRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /**************************************************************************************************************************************************
             * 获取接口请求地址
             * ************************************************************************************************************************************************/
            string url = string.Format("http://trimreport.iwxiao.net/trimreport/nwd?appid={1}&idfa={0}&source={2}",
                MemberRs["DeviceCode"].ToString(), ApplicationRs["ThirdID"].ToString(), ApplicationRs["Thirdname"].ToString());
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("博睿:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("博睿:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("博睿:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("博睿:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("博睿:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("result")) { this.ErrorMessage("博睿:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("博睿:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["result"] == null) { this.ErrorMessage("博睿:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["result"][MemberRs["DeviceCode"]] == null) { this.ErrorMessage("博睿:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["result"][MemberRs["DeviceCode"]].toString() == "1") { this.ErrorMessage("博睿:该任务你已经下载过,请抢夺其它任务吧！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 博睿排重接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 来赚
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void LAIZRepeat(DataRow ApplicationRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /**************************************************************************************************************************************************
             * 获取接口请求地址
             * ************************************************************************************************************************************************/
            string url = string.Format("http://es2.laizhuan.com/module/offer1/data.php?idfa={0}&ip={1}&token={3}&aff_id={4}&app_id={5}&act=unique",
                MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(), Fooke.DMFTasker.UnionConfig.Laizhuan.token,
                Fooke.DMFTasker.UnionConfig.Laizhuan.aff_id, Fooke.DMFTasker.UnionConfig.Laizhuan.app_id);
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("status")) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("来赚:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"] == null) { this.ErrorMessage("来赚:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"].toString() == "1") { this.ErrorMessage("来赚:该任务您已经做过了,请抢夺其它任务吧！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 博睿排重接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 有为
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void YOUWRepeat(DataRow ApplicationRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /**************************************************************************************************************************************************
             * 获取接口请求地址
             * ************************************************************************************************************************************************/
            string url = string.Format("http://47.93.70.4/channel/checkIdfa?cid={2}&appid={0}&idfa={1}",
                ApplicationRs["SortID"].ToString(), MemberRs["DeviceCode"].ToString(),
                Fooke.DMFTasker.UnionConfig.Youwei.cid);
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("有为:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("有为:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("有为:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("有为:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("有为:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("status")) { this.ErrorMessage("有为:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("有为:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"] == null) { this.ErrorMessage("有为:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"].toString() != "20000") { this.ErrorMessage("有为:该任务您已经做过了,请抢夺其它任务吧！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 有为排重接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 掌上互动
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void ZSHDRepeat(DataRow ApplicationRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /**************************************************************************************************************************************************
             * 获取接口请求地址
             * ************************************************************************************************************************************************/
            string url = string.Format("http://api.adzshd.com/RemoveEcho.ashx?adid={4}&idfa={0}&btype=1&ip={1}&os={2}&osversion={3}",
                MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(), MemberRs["DeviceType"].ToString(),
                MemberRs["DeviceModel"].ToString(), ApplicationRs["ThirdID"].ToString());
            if (new Fooke.Function.String(ApplicationRs["isWeight"].ToString()).toInt() == "2")
            {
                url = string.Format("http://api.adzshd.com/SourceSearchISActivate.ashx?cmd=getuserisacctivate&adid={1}&idfa={0}",
                MemberRs["DeviceCode"].ToString(), ApplicationRs["ThirdID"].ToString());
            }
            else if (new Fooke.Function.String(ApplicationRs["isWeight"].ToString()).toInt() == "3")
            {
                url = string.Format("http://api.adzshd.com/SourceSearchISActivate.ashx?cmd=isactivate&adid={1}&idfa={0}",
                MemberRs["DeviceCode"].ToString(), ApplicationRs["ThirdID"].ToString());
            }
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("掌上互动:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("掌上互动:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("掌上互动:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("掌上互动:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("掌上互动:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains(MemberRs["DeviceCode"].ToString())) { this.ErrorMessage("有为:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("掌上互动:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson[MemberRs["DeviceCode"].ToString()] == null) { this.ErrorMessage("掌上互动:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson[MemberRs["DeviceCode"].ToString()].toString() == "1") { this.ErrorMessage("掌上互动:该任务您已经做过了,请抢夺其它任务吧！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 掌上互动排重接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 壹狗试玩
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void YIGOURepeat(DataRow ApplicationRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /**************************************************************************************************************************************************
             * 获取接口请求地址
             * ************************************************************************************************************************************************/
            string url = string.Format("http://api.gzyigo.com/api/access/appquery?adid={2}&ch={3}&idfa={0}&ip={1}",
                MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
                ApplicationRs["ThirdID"].ToString(), ApplicationRs["Thirdname"].ToString());
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("壹狗试玩:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("壹狗试玩:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("壹狗试玩:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("壹狗试玩:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("壹狗试玩:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains(MemberRs["DeviceCode"].ToString())) { this.ErrorMessage("壹狗试玩:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("壹狗试玩:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson[MemberRs["DeviceCode"].ToString()] == null) { this.ErrorMessage("壹狗试玩:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson[MemberRs["DeviceCode"].ToString()].toString() == "1") { this.ErrorMessage("壹狗试玩:该任务您已经做过了,请抢夺其它任务吧！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 掌上互动排重接口执行完成
             * ****************************************************************************************************************************************************/
        }
        /// <summary>
        /// 海尔渠道
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void HaiErRepeat(DataRow ApplicationRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /**************************************************************************************************************************************************
             * 获取接口请求地址
             * ************************************************************************************************************************************************/
            string url = string.Format("http://47.107.90.67/ad/checkActive.php?projID={0}&idfa={1}&ip={2}",
                ApplicationRs["Thirdname"].ToString(), MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP());
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("海尔:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("海尔:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("海尔:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("海尔:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("海尔:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("resCode")) { this.ErrorMessage("海尔:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("海尔:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["resCode"] == null) { this.ErrorMessage("海尔:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["resCode"].toString() != "1" || rspJson["msg"].toString() != "success") { this.ErrorMessage("海尔:该任务您已经做过了,请抢夺其它任务吧！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 海尔排重接口执行完成
             * ****************************************************************************************************************************************************/
        }

        #endregion

        /*******************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 抢夺任务上报接口
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *****************************************************************************************/
        #region 抢夺任务上报接口
        /// <summary>
        /// 任务上报
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void DoClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            if (ApplicationRs["UnionModel"].ToString() == "微网") { WNETClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["UnionModel"].ToString() == "巨掌") { JUZClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["UnionModel"].ToString() == "博睿") { BORClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["UnionModel"].ToString() == "来赚") { LAIZClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["UnionModel"].ToString() == "有为") { YOUWClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["UnionModel"].ToString() == "掌上互动") { ZSHDClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["UnionModel"].ToString() == "壹狗") { YiGouClick(ApplicationRs, MemberRs, AppKey); }
            else if (ApplicationRs["UnionModel"].ToString() == "海尔") { HaiErClick(ApplicationRs, MemberRs, AppKey); }
        }
        /// <summary>
        /// 获取渠道回调地址
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public string tokenUrl(string appKey, string isBor = "0")
        {
            return string.Format("{0}{1}/api/verification.aspx?action=back&appkey={2}&isbor={3}",
                   FunctionCenter.SiteUrl(), Win.ApplicationPath, appKey, isBor);
        }

        /// <summary>
        /// 微网点击接口
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void WNETClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://centralapi.51welink.com/api/Welink/downstream_click_notice?AppleId={0}&ChannelID={5}&IDFA={1}&IP={2}&MAC={3}&CallbackURL={4}",
                ApplicationRs["SortID"].ToString(), MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
                MemberRs["MacChar"].ToString(), HttpUtility.UrlEncode(tokenUrl(AppKey)),
                Fooke.DMFTasker.UnionConfig.Weiwang.source);
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("微网:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("微网:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("微网:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("微网:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("微网:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("status")) { this.ErrorMessage("微网:任务通知请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("微网:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"] == null) { this.ErrorMessage("微网:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"].toString() == "false") { this.ErrorMessage("微网:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            else if (rspJson["status"].toString() != "true") { this.ErrorMessage("微网:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 巨掌
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        public void JUZClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://api.plat.adjuz.net/click?adid={0}&sourceid={4}&idfa={1}&ip={2}&mac=02:00:00:00:00:00&callbackurl={3}",
                ApplicationRs["ThirdID"].ToString(), MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
                HttpUtility.UrlEncode(tokenUrl(AppKey)), Fooke.DMFTasker.UnionConfig.Juzhang.sourceid);
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("巨掌:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("巨掌:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("巨掌:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("巨掌:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("巨掌:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("State")) { this.ErrorMessage("巨掌:任务通知请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("巨掌:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["State"] == null) { this.ErrorMessage("巨掌:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["State"].toString() != "100") { this.ErrorMessage("巨掌:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网上报接口执行完成
             * ****************************************************************************************************************************************************/
        }
        /// <summary>
        /// 博睿
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        /// <param name="AppKey"></param>
        public void BORClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://api.iwxiao.net/app/ck.do?source={3}&appid={2}&idfa={0}&client_ip={1}&callback={4}",
                MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(), ApplicationRs["ThirdID"].ToString(),
                ApplicationRs["Thirdname"].ToString(), HttpUtility.UrlEncode(tokenUrl(AppKey)));
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("博睿:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("博睿:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("博睿:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("博睿:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("博睿:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("code")) { this.ErrorMessage("博睿:任务通知请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("博睿:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["code"] == null) { this.ErrorMessage("博睿:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["code"].toString() != "200") { this.ErrorMessage("博睿:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 来赚
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        /// <param name="AppKey"></param>
        public void LAIZClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://es2.laizhuan.com/module/offer1/data.php?idfa={0}&ip={1}&token={3}&aff_id={4}&app_id={5}&act=click&callback={2}",
                MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(), HttpUtility.UrlEncode(tokenUrl(AppKey)),
                Fooke.DMFTasker.UnionConfig.Laizhuan.token, Fooke.DMFTasker.UnionConfig.Laizhuan.aff_id,
                Fooke.DMFTasker.UnionConfig.Laizhuan.app_id);
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("来赚:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("来赚:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("来赚:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("来赚:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("来赚:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("success")) { this.ErrorMessage("来赚:任务通知请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("来赚:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["success"] == null) { this.ErrorMessage("来赚:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["success"].toString() != "1") { this.ErrorMessage("来赚:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网上报接口执行完成
             * ****************************************************************************************************************************************************/
        }
        /// <summary>
        /// 有为
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        /// <param name="AppKey"></param>
        public void YOUWClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://47.93.70.4/channel/clickIdfa?cid={5}&appid={0}&idfa={1}&ip={2}&word={3}&callbackUrl={4}",
               ApplicationRs["SortID"].ToString(), MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
               HttpUtility.UrlEncode(ApplicationRs["strKeyword"].ToString()), HttpUtility.UrlEncode(tokenUrl(AppKey)),
               Fooke.DMFTasker.UnionConfig.Youwei.cid);
            if (ApplicationRs["Thirdname"].ToString().Length != 0)
            {
                url = string.Format("http://47.93.70.4/channel/clickIdfaAlias?cid={6}&appid={0}&alias={5}&idfa={1}&ip={2}&word={3}&callbackUrl={4}",
               ApplicationRs["SortID"].ToString(), MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
               HttpUtility.UrlEncode(ApplicationRs["strKeyword"].ToString()), HttpUtility.UrlEncode(tokenUrl(AppKey)),
               ApplicationRs["Thirdname"].ToString(), Fooke.DMFTasker.UnionConfig.Youwei.cid);
            }
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("有为:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("有为:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("有为:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("有为:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("有为:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("status")) { this.ErrorMessage("有为:任务通知请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("有为:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"] == null) { this.ErrorMessage("有为:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"].toString() != "20000") { this.ErrorMessage("有为:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网上报接口执行完成
             * ****************************************************************************************************************************************************/
        }
        /// <summary>
        /// 掌上互动
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        /// <param name="AppKey"></param>
        public void ZSHDClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://api.adzshd.com/SourceClick.ashx?adid={0}&appid={7}&mac=02:00:00:00:00:00&idfa={1}&ip={2}&os={3}&osversion={4}&KeyWords={5}&callback={6}",
               ApplicationRs["thirdid"].ToString(), MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
              MemberRs["DeviceType"].ToString(), MemberRs["DeviceModel"].ToString(), ApplicationRs["strKeyword"].ToString(),
              HttpUtility.UrlEncode(tokenUrl(AppKey)), Fooke.DMFTasker.UnionConfig.Zhangshanghudong.appid);
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("掌上互动:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("掌上互动:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("掌上互动:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("掌上互动:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("掌上互动:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("status")) { this.ErrorMessage("掌上互动:任务通知请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("掌上互动:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"] == null) { this.ErrorMessage("掌上互动:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"].toString() != "1") { this.ErrorMessage("掌上互动:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 壹狗试玩
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        /// <param name="AppKey"></param>
        public void YiGouClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://api.gzyigo.com/api/access/appclick?adid={7}&ch={8}&idfa={0}&ip={1}&kid={2}&os={3}&devicemodel={4}&mac={5}&callback={6}",
               MemberRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(), ApplicationRs["strKeyword"].ToString(),
               RequestHelper.GetRequest("strEdition").toString(), MemberRs["DeviceModel"].ToString(), MemberRs["MacChar"].ToString(),
               HttpUtility.UrlEncode(tokenUrl(AppKey)), ApplicationRs["ThirdID"].ToString(), ApplicationRs["Thirdname"].ToString());
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("壹狗试玩:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("壹狗试玩:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("壹狗试玩:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("壹狗试玩:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("壹狗试玩:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("Code")) { this.ErrorMessage("壹狗试玩:任务通知请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("壹狗试玩:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["Code"] == null) { this.ErrorMessage("壹狗试玩:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["Code"].toString() != "0") { this.ErrorMessage("壹狗试玩:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网上报接口执行完成
             * ****************************************************************************************************************************************************/
        }
        /// <summary>
        /// 海尔
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="MemberRs"></param>
        /// <param name="AppKey"></param>
        public void HaiErClick(DataRow ApplicationRs, DataRow MemberRs, string AppKey)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://47.107.90.67/ad/click.php?channelID={0}&projID={1}&idfa={2}&mac={3}&ip={4}&devicetype={5}&os={6}&callbackURL={7}",
               ApplicationRs["ThirdID"].ToString(), ApplicationRs["Thirdname"].ToString(), MemberRs["DeviceCode"].ToString(), MemberRs["MacChar"].ToString(), FunctionCenter.GetCustomerIP(), MemberRs["DeviceModel"].ToString(), MemberRs["DeviceChar"].ToString(), HttpUtility.UrlEncode(tokenUrl(AppKey)));
            using (System.Net.WebClient thisClient = new System.Net.WebClient())
            {
                try
                {
                    thisClient.Encoding = Encoding.UTF8;
                    strResponse = thisClient.DownloadString(url);
                }
                catch (Exception err) { strResponse = string.Format("error:{0}", err.Message); }
                finally { thisClient.Dispose(); }
            }
            /******************************************************************************************************************************************************
             * 判断数据请求过程中是否发生错误
             * ****************************************************************************************************************************************************/
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("海尔:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("海尔:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("海尔:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("海尔:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("海尔:任务通知请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("resCode")) { this.ErrorMessage("海尔:任务通知请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("海尔:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["resCode"] == null) { this.ErrorMessage("海尔:任务通知过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["resCode"].toString() != "1" || rspJson["msg"].toString() != "success") { this.ErrorMessage("海尔:任务通知广告主失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 海尔上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        #endregion
        /*******************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 用户截图审核任务
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * *****************************************************************************************/
        #region 用户截图审核任务
        protected void SaveUpload()
        {
            /***********************************************************************************
             * 验证获取上传图片合法性
             * *********************************************************************************/
            if (Request.Files == null) { this.JSONMessage("请选择要上传的文件！"); Response.End(); }
            if (Request.Files.Count <= 0) { this.JSONMessage("请选择要上传的文件！"); Response.End(); }
            /***********************************************************************************
             * 验证上传文件合法性
             * *********************************************************************************/
            HttpPostedFile thisPosted = ((HttpPostedFile)(Request.Files[0]));
            if (thisPosted.ContentLength <= 0) { this.JSONMessage("获取上传文件内容信息失败,请重试！"); Response.End(); }
            else if (thisPosted.ContentLength >= (1024 * 1024 * 5)) { this.JSONMessage("单个文件资源大小请限制在50M以内！"); Response.End(); }
            else if (string.IsNullOrEmpty(thisPosted.FileName)) { this.JSONMessage("获取上传文件格式失败,请重试！"); Response.End(); }
            else if (!thisPosted.FileName.Contains(".")) { this.JSONMessage("获取上传文件格式失败,请重试！"); Response.End(); }
            /***********************************************************************************
             * 获取文件后缀名,并且判断是否合法
             * *********************************************************************************/
            string strFlter = thisPosted.FileName.Substring(thisPosted.FileName.LastIndexOf(".") + 1);
            if (strFlter.Length <= 1) { this.JSONMessage("获取上传文件格式错误,请重试！"); Response.End(); }
            else if (strFlter.Length >= 12) { this.JSONMessage("获取上传文件格式错误,请重试！"); Response.End(); }
            else if (!("jpg|jpeg|png|bmp").Contains(strFlter.ToLower())) { this.JSONMessage("获取上传文件格式错误,只允许上传图片与视频文件！"); Response.End(); }
            /***********************************************************************************
             * 开始上传图片信息
             * *********************************************************************************/
            string strResponse = "error";
            string fileName = string.Format("上传文件-|-|-{0}-|-|-{1}", DateTime.Now.Ticks.ToString(), new Random().NextDouble().ToString());
            fileName = new Fooke.Function.String(fileName).ToMD5().Substring(0, 16).ToLower();
            fileName = fileName + ".{exc}";
            new PostedHelper().SaveAs(Request.Files[0], new Fooke.Function.PostedHelper.FileMode()
            {
                fileName = fileName,
                fileDirectory = "~/file/screen",
                fileExt = "jpg|png|bmp",
                fileSize = 1024 * 1024 * 5,
                Success = (thumb) => { strResponse = thumb; },
                Error = (Exp) => { this.JSONMessage(Exp); Response.End(); }
            });
            if (string.IsNullOrEmpty(strResponse)) { this.JSONMessage("图片保存过程中发生错误,请重试！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.JSONMessage("图片保存过程中发生错误,请重试！"); Response.End(); }
            else if (strResponse.Length <= 20) { this.JSONMessage("图片保存过程中发生错误,请重试！"); Response.End(); }
            else if (strResponse.Length >= 120) { this.JSONMessage("图片保存过程中发生错误,请重试！"); Response.End(); }
            else if (strResponse.StartsWith("error")) { this.JSONMessage("图片保存过程中发生错误,请重试！"); Response.End(); }
            /***********************************************************************************
             * 输出数据处理结果信息
             * *********************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"url\":\"" + strResponse + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 提交截图审核
        /// </summary>
        protected void Submission()
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
             * 获取并验证用户下载AppKey值
             * ***************************************************************************************************************************/
            string AppKey = RequestHelper.GetRequest("appKey").ToString();
            if (string.IsNullOrEmpty(AppKey)) { this.ErrorMessage("获取下载应用标识信息失败！"); Response.End(); }
            else if (AppKey.Length <= 0) { this.ErrorMessage("获取下载应用标识信息失败!"); Response.End(); }
            else if (AppKey.Length <= 20) { this.ErrorMessage("获取下载应用标识信息失败!"); Response.End(); }
            else if (AppKey.Length >= 32) { this.ErrorMessage("获取下载应用标识信息失败!"); Response.End(); }
            DataRow DownRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppDown]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"AppKey",AppKey}
            });
            if (DownRs == null) { this.ErrorMessage("您还没有开始任务,请先开始任务！"); Response.End(); }
            else if (DownRs["isFinish"].ToString() == "1") { this.ErrorMessage("您的截图已经通过开发商审核了,再去做其它的任务吧！"); Response.End(); }
            else if (DownRs["isFinish"].ToString() == "99") { this.ErrorMessage("您已经提交了截图审核,请等待开发商审核！"); Response.End(); }
            else if (DownRs["isFinish"].ToString() == "100") { this.ErrorMessage("很遗憾,您的截图没有通过开发商审核！"); Response.End(); }
            else if (DownRs["isFinish"].ToString() != "0") { this.ErrorMessage("未知的截图任务状态,请重试！"); Response.End(); }
            /*****************************************************************************************************************************
             * 获取并验证用户上传的其它数据信息
             * ***************************************************************************************************************************/
            string strMobile = RequestHelper.GetRequest("strMobile").ToString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("获取手机号信息失败,请填写您的手机号！"); Response.End(); }
            else if (strMobile.Length <= 0) { this.ErrorMessage("获取手机号信息失败,请填写您的手机号！"); Response.End(); }
            else if (strMobile.Length <= 6) { this.ErrorMessage("请填写正确的手机号码!"); Response.End(); }
            else if (strMobile.Length >= 16) { this.ErrorMessage("请填写正确的手机号码!"); Response.End(); }
            /*****************************************************************************************************************************
             * 获取并验证用户名称信息
             * ***************************************************************************************************************************/
            string strName = RequestHelper.GetRequest("strName").ToString();
            if (string.IsNullOrEmpty(strName)) { this.ErrorMessage("请填写您的名称信息！"); Response.End(); }
            else if (strName.Length <= 0) { this.ErrorMessage("请填写您的名称信息！"); Response.End(); }
            else if (strName.Length >= 16) { this.ErrorMessage("名称信息字段长度不能超过16个字符!"); Response.End(); }
            /*****************************************************************************************************************************
             * 获取用户上传的截图数据
             * ***************************************************************************************************************************/
            string strXml = RequestHelper.GetRequest("strXml", false).ToString();
            if (string.IsNullOrEmpty(strXml)) { this.ErrorMessage("获取上传截图信息失败！"); Response.End(); }
            else if (strXml.Length <= 0) { this.ErrorMessage("获取用户上传截图信息失败！"); Response.End(); }
            else if (!strXml.StartsWith("<configurationRoot>")) { this.ErrorMessage("获取用户上传截图信息失败！"); Response.End(); }
            else if (!strXml.EndsWith("</configurationRoot>")) { this.ErrorMessage("获取用户上传截图信息失败！"); Response.End(); }
            else if (!strXml.Contains("file")) { this.ErrorMessage("获取用户上传截图信息失败！"); Response.End(); }
            DataTable thisTab = FunctionCenter.XMLConvertToDataTable(strXml);
            if (thisTab == null) { this.ErrorMessage("1获取用户上传截图信息失败！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("获取用2户上传截图信息失败！"); Response.End(); }
            else if (thisTab.Rows.Count >= 4) { this.ErrorMessage("您最多只允许上传3张截图！"); Response.End(); }
            /*****************************************************************************************************************************
             * 开始保存请求数据
             * ***************************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["AppID"] = DownRs["AppID"].ToString();
            thisDictionary["AppKey"] = DownRs["AppKey"].ToString();
            thisDictionary["Appname"] = DownRs["Appname"].ToString();
            thisDictionary["strMobile"] = strMobile;
            thisDictionary["strName"] = strName;
            thisDictionary["HotXml"] = strXml.ToString();
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveAppHot]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("服务器系统繁忙,请稍后再试！"); Response.End(); }
            /*****************************************************************************************************************************
             * 输出数据处理结果
             * ***************************************************************************************************************************/
            Response.Write("{");
            Response.Write("\"success\":\"true\"");
            Response.Write(",\"type\":\"define\"");
            Response.Write(",\"tips\":\"success\"");
            Response.Write("}");
            Response.End();
        }

        #endregion
    }
}