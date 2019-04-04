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
using Newtonsoft.Json.Linq;
namespace Fooke.Web.API
{
    /// <summary>
    /// 用户登录接口信息
    /// </summary>
    public partial class Verification : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strRequest = RequestHelper.GetRequest("action").toString();
            if (string.IsNullOrEmpty(strRequest)) { strRequest = "default"; }
            if (strRequest == "start") { StartVerification(); Response.End(); }
            else if (strRequest == "back") { SaveCallBack(); Response.End(); }
        }
        /// <summary>
        /// 保存渠道回调审核信息
        /// </summary>
        protected void SaveCallBack()
        {
            /*************************************************************************************************************
             * 获取并验证回调应用数据信息
             * ***********************************************************************************************************/
            string appKey = RequestHelper.GetRequest("appKey").toString();
            if (string.IsNullOrEmpty(appKey)) { this.ErrorMessage("获取请求参数错误，请重试！"); Response.End(); }
            else if (appKey.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (appKey.Length <= 20) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (appKey.Length >= 32) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppDown]", new Dictionary<string, object>() {
                {"appKey",appKey}
            });
            if (cRs == null) { this.ErrorMessage("请求参数错误，你查找的数据不存在！"); Response.End(); }
            else if (cRs["isFinish"].ToString() == "1") { this.ErrorMessage("success", true); Response.End(); }
            /*************************************************************************************************************
             * 更新用户回调数据记录信息
             * ***********************************************************************************************************/
            DbHelper.Connection.Update("Fooke_AppDown", dictionary: new Dictionary<string, string>() { 
                {"isCallback","1"}
            }, Params: " and TokenID=" + cRs["TokenID"] + "");
            /*************************************************************************************************************
            * 输出数据处理结果数据信息
            * ***********************************************************************************************************/
            Response.Write("{");
            Response.Write("\"success\":\"true\"");
            Response.Write(",\"type\":\"define\"");
            Response.Write(",\"tips\":\"success\"");
            Response.Write("}");
            Response.End();
        }

        /// <summary>
        /// 验证用户下载任务
        /// </summary>
        protected void StartVerification()
        {
            /*************************************************************************************************************
             * 声明系统参数配置项目信息
             * ***********************************************************************************************************/
            ConfigureCenter thisConfigure = new ConfigureCenter();
            if (thisConfigure == null) { this.ErrorMessage("获取系统参数配置信息失败,请联系客服！"); Response.End(); }
            /*************************************************************************************************************
             * 获取并验证回调应用数据信息
             * ***********************************************************************************************************/
            string appKey = RequestHelper.GetRequest("appKey").toString();
            if (string.IsNullOrEmpty(appKey)) { this.ErrorMessage("获取请求参数错误，请重试！"); Response.End(); }
            else if (appKey.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (appKey.Length <= 20) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            else if (appKey.Length >= 32) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindAppDown]", new Dictionary<string, object>() {
                {"appKey",appKey}
            });
            if (cRs == null) { this.ErrorMessage("请求参数错误，你查找的数据不存在！"); Response.End(); }
            else if (cRs["isFinish"].ToString() == "1") { this.ErrorMessage("success", true); Response.End(); }
            /*************************************************************************************************************
             * 开始验证自有渠道应用信息
             * ***********************************************************************************************************/
            DataRow ApplicationRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindApplication]", new Dictionary<string, object>() {
                {"AppID",cRs["AppID"].ToString()}
            });
            if (ApplicationRs == null) { this.ErrorMessage("获取自有渠道应用信息失败！"); Response.End(); }
            /*************************************************************************************************************
             * 开始获取做任务的用户数据
             * ***********************************************************************************************************/
            DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",cRs["UserID"].ToString()}
            });
            if (MemberRs == null) { this.ErrorMessage("获取用户数据信息失败！"); Response.End(); }
            else if (MemberRs == null) { this.ErrorMessage("对不起,您的账号已经被管理员禁止使用！"); Response.End(); }
            /*************************************************************************************************************
             * 检查当前任务是否完成或是否回调
             * ***********************************************************************************************************/
            #region 检查当前任务是否完成或是否回调
            if (cRs["appModel"].ToString() == "6" && cRs["isCallback"].ToString() == "0")
            {
                if (new Fooke.Function.String(cRs["isRound"].ToString()).cInt() <= 0)
                {
                    DbHelper.Connection.Update("Fooke_AppDown", dictionary: new Dictionary<string, string>() {
                        {"isRound","1"}
                    }, Params: " and TokenID=" + cRs["TokenID"] + "");
                    ErrorMessage("请务必打开app使用到达规定时间，您的试玩时间不够，将重新计时3分钟");
                    Response.End();
                }
                /*************************************************************************************************************
                * 将任务设置为失败的任务信息
                * ***********************************************************************************************************/
                DbHelper.Connection.ExecuteProc("[Stored_SaveAppDownFail]", new Dictionary<string, object>() {
                    {"TokenID",cRs["TokenID"].ToString()},
                    {"UserID",cRs["UserID"].ToString()},
                    {"AppId",cRs["AppId"].ToString()}
                });
                this.ErrorMessage("您的信息未通过开发商审核，完成任务失败，请尝试其它任务", false);
                Response.End();
            }
            else if (cRs["appModel"].ToString() == "2" && cRs["isCallback"].ToString() == "0")
            {
                if (new Fooke.Function.String(cRs["isRound"].ToString()).cInt() <= 0)
                {
                    DbHelper.Connection.Update("Fooke_AppDown", dictionary: new Dictionary<string, string>() {
                        {"isRound","1"}
                    }, Params: " and TokenID=" + cRs["TokenID"] + "");
                    ErrorMessage("请务必打开app使用到达规定时间，您的试玩时间不够，将重新计时3分钟");
                    Response.End();
                }
                this.ErrorMessage("您已提交任务完成请求，请等待开发商审核。", true);
                Response.End();
            }
            #endregion
            /*************************************************************************************************************
            * 数据上报,将用户提交的数据上报到指定平台
            * ***********************************************************************************************************/
            if (ApplicationRs["appModel"].ToString() == "3") { DoReport(ApplicationRs, cRs, MemberRs); }
            else if (ApplicationRs["appModel"].ToString() == "4") { DoReport(ApplicationRs, cRs, MemberRs); }
            //获取城市
            string strIp = string.IsNullOrEmpty(cRs["strIP"].ToString()) ? FunctionCenter.GetCustomerIP() : cRs["strIP"].ToString();
            string strCity = cRs["cityname"].ToString();
            if (string.IsNullOrEmpty(strCity)) { strCity = new IPHelper().ConvertToCity(strIp).Country;; }
            /*************************************************************************************************************
            * 开始保存用户做任务的数据记录
            * ***********************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["AppID"] = cRs["AppID"].ToString();
            thisDictionary["Appname"] = cRs["Appname"].ToString();
            thisDictionary["AppKey"] = cRs["AppKey"].ToString();
            thisDictionary["DeviceType"] = cRs["DeviceType"].ToString();
            thisDictionary["DeviceCode"] = cRs["DeviceCode"].ToString();
            thisDictionary["MacChar"] = cRs["MacChar"].ToString();
            thisDictionary["Amount"] = cRs["Amount"].ToString();
            thisDictionary["strIP"] = strIp;
            thisDictionary["cityname"] = strCity;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveFinishDown]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /*************************************************************************************************************
             * 发放用户奖励提成数据信息
             * ***********************************************************************************************************/
            //是否开启提成 1开启 0未开启
            string isTask = thisConfigure.GetParameter("isTask", "shareXml").toInt();
            //提成次数
            int CTTimer = thisConfigure.GetParameter("CTTimer", "shareXml").cInt();
            if (isTask == "1" && MemberRs["ParentID"].ToString() != "0"
            && (new Fooke.Function.String(MemberRs["BonusTimer"].ToString()).cInt() <= CTTimer || CTTimer <= 0))
            {
                new BonusHelper().TaskBonus(UserID: MemberRs["UserID"].ToString(),
                        Amount: new Fooke.Function.String(cRs["Amount"].ToString()).cDouble(),
                        FormatKey: cRs["AppKey"].ToString());
            }
            /*************************************************************************************************************
             * 发放用户奖励提成数据信息（级差提成奖励）
             * ***********************************************************************************************************/
            //是否开启级差提成 1开启 0未开启
            string isGradeTask = thisConfigure.GetParameter("isGradeTask", "shareXml").toInt();
            if (isGradeTask == "1" && MemberRs["ParentID"].ToString() != "0")
            {
                new BonusHelper().GradeBonus(UserID: MemberRs["UserID"].ToString(),
                        Amount: new Fooke.Function.String(cRs["Amount"].ToString()).cDouble(),
                        FormatKey: cRs["AppKey"].ToString());
            }
            /*************************************************************************************************************
             * 判断用户是否为第一次做任务并且
             * ***********************************************************************************************************/
            string isBonus = thisConfigure.GetParameter("isBonus", "shareXml").toInt();
            string shareModel = thisConfigure.GetParameter("shareModel", "shareXml").toInt();
            if (isBonus == "1" && shareModel == "1" && MemberRs["ParentID"].ToString() != "0"
            && new Fooke.Function.String(MemberRs["BonusTimer"].ToString()).cInt() <= 0)
            {
                try { new BonusHelper().ShareBonus(MemberRs["UserID"].ToString()); }
                catch { }
            }
            /***************************************************************************************************************
             * 给用户发送一条推送消息
             * *************************************************************************************************************/
            try
            {   string tempID = string.Empty;
                if (MemberRs["DeviceType"].ToString().ToLower() == "android") 
                { tempID = thisConfigure.GetParameter("AndroidTempID", "PushXml").ToString(); }
                else{tempID= thisConfigure.GetParameter("iOSTempID", "PushXml").ToString();}
                if (string.IsNullOrEmpty(tempID)) { tempID = "恭喜完成{0}任务获得{1}元"; }
                new Fooke.Code.PushCenter().Start(Configure: new Fooke.Code.ConfigureCenter(),
                    content: string.Format(tempID,cRs["Appname"].ToString(),cRs["Amount"].ToString()),
                    identify: MemberRs["UserID"].ToString());
            }
            catch { }
            /*************************************************************************************************************
             * 数据保存成功,输出数据处理结果
             * ***********************************************************************************************************/
            this.ErrorMessage("success", true);
            Response.End();
        }
        /*****************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 系统公用方法处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ***************************************************************************************************************/
        #region 系统公用方法处理区域
        /// <summary>
        /// 输出错误提示信息
        /// </summary>
        /// <param name="strTips"></param>
        /// <param name="isTrue"></param>
        protected void ErrorMessage(string strTips, bool isTrue = false)
        {
            Response.Write("{");
            Response.Write("\"success\":\"" + isTrue.ToString().ToLower() + "\"");
            Response.Write(",\"type\":\"define\"");
            Response.Write(",\"tips\":\"" + strTips + "\"");
            Response.Write("}");
            Response.End();
        }

        #endregion
        /*****************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 数据上报接口区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * ***************************************************************************************************************/
        #region 数据上报接口区域
        /// <summary>
        /// 数据上报
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="cRs"></param>
        public void DoReport(DataRow ApplicationRs, DataRow cRs, DataRow MemberRs)
        {
            if (ApplicationRs["UnionModel"].ToString() == "微网") { WNReport(ApplicationRs, cRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "巨掌") { JUZReport(ApplicationRs, cRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "博睿") { RBSBReport(ApplicationRs, cRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "来赚") { LAIZReport(ApplicationRs, cRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "有为") { YWReport(ApplicationRs, cRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "掌上互动") { ZSHDReport(ApplicationRs, cRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "壹狗") { YiGouReport(ApplicationRs, cRs, MemberRs); }
            else if (ApplicationRs["UnionModel"].ToString() == "海尔") { HaiErReport(ApplicationRs, cRs, MemberRs); }
        }

        /// <summary>
        /// 微网数据上报
        /// </summary>
        /// <param name="ApplicationRs">任务记录集</param>
        /// <param name="cRs">下载记录集</param>
        public void WNReport(DataRow ApplicationRs, DataRow cRs)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://centralapi.51welink.com/api/Welink/downstream_report_notice?AppleId={0}&Source={4}&IDFA={1}&IP={2}&MAC={3}",
                ApplicationRs["SoftID"].ToString(), cRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(), cRs["MacChar"].ToString(),
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
            else if (rspJson["status"].toString() == "") { this.ErrorMessage("微网:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"].toString() != "true") { this.ErrorMessage("微网:数据上报过程中返回接口请求错误！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 微网上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 巨掌数据上报
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="cRs"></param>
        public void JUZReport(DataRow ApplicationRs, DataRow cRs)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://api.plat.adjuz.net/activate?adid={0}&sourceid={3}&idfa={1}&ip={2}&mac=02:00:00:00:00:00",
                ApplicationRs["ThirdID"].ToString(), cRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
                cRs["MacChar"].ToString(), Fooke.DMFTasker.UnionConfig.Juzhang.sourceid);
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
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("巨掌:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("巨掌:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("巨掌:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("巨掌:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("巨掌:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("State")) { this.ErrorMessage("巨掌:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("巨掌:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["State"] == null) { this.ErrorMessage("巨掌:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["State"].toString() == "101") { this.ErrorMessage("巨掌:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            else if (rspJson["State"].toString() == "102") { this.ErrorMessage("巨掌:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            else if (rspJson["State"].toString() != "102") { this.ErrorMessage("巨掌:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 巨掌上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 来赚
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="cRs"></param>
        public void LAIZReport(DataRow ApplicationRs, DataRow cRs)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://es2.laizhuan.com/module/offer1/data.php?idfa={0}&ip={1}&token={2}&aff_id={3}&app_id={4}&act=active",
                cRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(),
                Fooke.DMFTasker.UnionConfig.Laizhuan.token,
                Fooke.DMFTasker.UnionConfig.Laizhuan.aff_id,
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
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("success")) { this.ErrorMessage("来赚:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("来赚:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["success"] == null) { this.ErrorMessage("来赚:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["success"].toString() != "1") { this.ErrorMessage("来赚:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 来赚上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 掌上互动
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="cRs"></param>
        public void ZSHDReport(DataRow ApplicationRs, DataRow cRs)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://api.adzshd.com/submit.ashx?adid={0}&appid={7}&mac={1}&idfa={2}&ip={3}&os={4}&osversion={5}&KeyWords={6}",
                ApplicationRs["ThirdID"].ToString(), cRs["MacChar"].ToString(), cRs["DeviceCode"].ToString(),
                FunctionCenter.GetCustomerIP(), cRs["DeviceType"].ToString(), cRs["strEdition"].ToString(),
                cRs["strInstall"].ToString(), Fooke.DMFTasker.UnionConfig.Zhangshanghudong.appid);
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
            else if (!strResponse.Contains("status")) { this.ErrorMessage("掌上互动:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("掌上互动:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"] == null) { this.ErrorMessage("掌上互动:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["status"].toString() == "101") { this.ErrorMessage("掌上互动:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            else if (rspJson["status"].toString() == "102") { this.ErrorMessage("掌上互动:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            else if (rspJson["status"].toString() != "1") { this.ErrorMessage("掌上互动:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 巨掌上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 有为
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="cRs"></param>
        public void YWReport(DataRow ApplicationRs, DataRow cRs)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://47.93.70.4/channel/submitIdfa?cid={4}&appid={0}&idfa={1}&ip={2}&word={3}",
                ApplicationRs["SoftID"].ToString(), cRs["DeviceCode"].ToString(),
                FunctionCenter.GetCustomerIP(), HttpUtility.UrlEncode(cRs["strInstall"].ToString()),
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
            else if (rspJson["status"].toString() != "20000") { this.ErrorMessage("有为:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 有为上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 博睿上报
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="cRs"></param>
        public void RBSBReport(DataRow ApplicationRs, DataRow cRs,DataRow MemberRs)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://api.iwxiao.net/app/ck.do?source={3}&appid={2}&idfa={0}&ip={1}&device={4}&osv={5}",
                cRs["DeviceCode"].ToString(), FunctionCenter.GetCustomerIP(), ApplicationRs["SoftID"].ToString(),
                ApplicationRs["Thirdname"].ToString(), MemberRs["DeviceModel"].ToString(), cRs["strEdition"].ToString());
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
            if (string.IsNullOrEmpty(strResponse)) { this.ErrorMessage("博睿上报:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.Length <= 0) { this.ErrorMessage("博睿上报:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (strResponse.StartsWith("error:")) { this.ErrorMessage("博睿上报:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.StartsWith("{")) { this.ErrorMessage("博睿上报:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.EndsWith("}")) { this.ErrorMessage("博睿上报:上报接口请求错误,未返回数据！"); Response.End(); }
            else if (!strResponse.Contains("code")) { this.ErrorMessage("博睿上报:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("博睿上报:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["code"] == null) { this.ErrorMessage("博睿上报:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["code"].toString() != "200") { this.ErrorMessage("博睿上报:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 博睿上报接口执行完成
             * ****************************************************************************************************************************************************/
        }

        /// <summary>
        /// 壹狗试玩
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="cRs"></param>
        public void YiGouReport(DataRow ApplicationRs, DataRow cRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://api.gzyigo.com/api/access/Appactive?adid={0}&ch={1}&idfa={2}&ip={3}&kid={4}&os={5}&devicemodel={6}&mac={7}",
               ApplicationRs["ThirdID"].ToString(), ApplicationRs["Thirdname"].ToString(), cRs["DeviceCode"].ToString(),
               FunctionCenter.GetCustomerIP(), ApplicationRs["strKeyword"].ToString(), MemberRs["DeviceModel"].ToString(), cRs["strEdition"].ToString(),
               cRs["MacChar"].ToString());
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
            else if (!strResponse.Contains("Code")) { this.ErrorMessage("壹狗试玩:上报接口请求错误,未返回数据！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 验证请求接口返回数据的合法性
             * ****************************************************************************************************************************************************/
            Newtonsoft.Json.Linq.JObject rspJson = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResponse);
            if (rspJson == null) { this.ErrorMessage("壹狗试玩:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["Code"] == null) { this.ErrorMessage("壹狗试玩:数据上报过程中返回接口请求错误！"); Response.End(); }
            else if (rspJson["Code"].toString() != "0") { this.ErrorMessage("壹狗试玩:您的任务信息无法提交成功，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 博睿上报接口执行完成
             * ****************************************************************************************************************************************************/
        }
        /// <summary>
        /// 海尔
        /// </summary>
        /// <param name="ApplicationRs"></param>
        /// <param name="cRs"></param>
        public void HaiErReport(DataRow ApplicationRs, DataRow cRs, DataRow MemberRs)
        {
            string strResponse = "error:";
            /******************************************************************************************************************************************************
             * 申明微网上报请求地址信息
             * ****************************************************************************************************************************************************/
            string url = string.Format("http://47.107.90.67/ad/active.php?channelID={0}&projID={1}&idfa={2}",
               ApplicationRs["ThirdID"].ToString(), ApplicationRs["Thirdname"].ToString(),cRs["DeviceCode"].ToString());
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
            else if (rspJson["resCode"].toString() != "1" || rspJson["msg"].toString() != "success") { this.ErrorMessage("海尔:您的任务上报激活失败，请重试任务，或放弃该任务！"); Response.End(); }
            /******************************************************************************************************************************************************
             * 海尔上报接口执行完成
             * ****************************************************************************************************************************************************/
        }
        #endregion
    }
}