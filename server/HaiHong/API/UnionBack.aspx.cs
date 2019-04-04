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
    public partial class UnionBack : System.Web.UI.Page
    {
        /// <summary>
        /// 错误数据输出格式
        /// </summary>
        protected string falseTips = string.Empty;
        /// <summary>
        /// 返回正确输出格式
        /// </summary>
        protected string trueTips = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            /************************************************************************************************************************
             * 获取请求参数并验证其合法性
             * **********************************************************************************************************************/
            string UnionID = RequestHelper.GetRequest("UnionID").toInt();
            if (UnionID == "0") { Response.Write("获取联盟信息错误,请重试!"); Response.End(); }
            string UnionKey = RequestHelper.GetRequest("UnionKey").ToString();
            if (string.IsNullOrEmpty(UnionKey)) { Response.Write("渠道签名信息验证失败,请重试!"); Response.End(); }
            else if (UnionKey.Length <= 10) { Response.Write("渠道签名信息验证失败,请重试!"); Response.End(); }
            else if (UnionKey.Length >= 30) { Response.Write("渠道签名信息验证失败,请重试!"); Response.End(); }
            /************************************************************************************************************************
             * 获取联盟渠道任务数据信息
             * **********************************************************************************************************************/
            DataRow UnionRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUnionChannel]", new Dictionary<string, object>() { 
                {"UnionID",UnionID}
            });
            if (UnionRs == null) { Response.Write("获取联盟数据信息失败!"); Response.End(); }
            else if (UnionRs["isDisplay"].ToString() != "1") { Response.Write("false"); Response.End(); }
            /***************************************************************************************************************
             * 保存数据库回调探针的数据信息
             * *************************************************************************************************************/
            DbHelper.Connection.ExecuteProc("[Stored_SaveUnionBackDaily]", new Dictionary<string, object>() {
                {"UnionID",UnionRs["UnionID"].ToString()},
                {"strUnion",UnionRs["strUnion"].ToString()},
                {"strXml",this.GetParameter()},
                {"strIP",FunctionCenter.GetCustomerIP()},
            });
            /************************************************************************************************************************
             * 开始执行网页数据存储功能
             * **********************************************************************************************************************/
            SaveUnion(UnionRs);
            Response.End();
        }
        /// <summary>
        /// 保存任务信息
        /// </summary>
        protected void SaveUnion(DataRow UnionRs)
        {
            /*************************************************************************************************************
             * 声明系统参数配置项目信息
             * ***********************************************************************************************************/
            ConfigureCenter thisConfigure = new ConfigureCenter();
            if (thisConfigure == null) { Response.Write("获取系统参数配置信息失败,请联系客服！"); Response.End(); }
            /***************************************************************************************************************
             * 开启回调数据
             * *************************************************************************************************************/
            string strParamster = this.GetParameter();
            /***************************************************************************************************************
             * 数据防刷处理
             * *************************************************************************************************************/
            if (strParamster.ToLower().Contains("select")) { Response.Write("非法请求,已关闭数据链接！"); Response.End(); }
            else if (strParamster.ToLower().Contains("die")) { Response.Write("非法请求,已关闭数据链接！"); Response.End(); }
            else if (strParamster.ToLower().Contains("print")) { Response.Write("非法请求,已关闭数据链接！"); Response.End(); }
            else if (strParamster.ToLower().Contains("null,null")) { Response.Write("非法请求,已关闭数据链接！"); Response.End(); }
            else if (strParamster.ToLower().Contains("md5")) { Response.Write("非法请求,已关闭数据链接！"); Response.End(); }
            else if (strParamster.ToLower().Contains("delete")) { Response.Write("非法请求,已关闭数据链接！"); Response.End(); }
            /***************************************************************************************************************
             * 验证联盟渠道配置参数的合法性
             * *************************************************************************************************************/
            if (string.IsNullOrEmpty(UnionRs["strXml"].ToString())) { Response.Write("获取联盟配置信息失败!"); Response.End(); }
            else if (UnionRs["strXml"].ToString().Length <= 0) { Response.Write("获取联盟配置信息失败"); Response.End(); }
            else if (!UnionRs["strXml"].ToString().StartsWith("<configurationRoot>")) { Response.Write("获取联盟配置信息失败"); Response.End(); }
            else if (!UnionRs["strXml"].ToString().EndsWith("</configurationRoot>")) { Response.Write("获取联盟配置信息失败"); Response.End(); }
            ConfigurationHelper cfgHelper = new ConfigurationHelper(UnionRs["strXml"].ToString());
            if (cfgHelper == null) { Response.Write("获取联盟配置信息失败"); Response.End(); }
            else if (cfgHelper.Length <= 0) { Response.Write("获取联盟配置信息失败"); Response.End(); }
            /***************************************************************************************************************
             * 验证渠道回调请求地址合法性
             * *************************************************************************************************************/
            string VerificationIP = new Fooke.Function.String(cfgHelper["strIP"].toString()).ToString();
            if (VerificationIP.Length != 0 && VerificationIP.Length >= 7
            && !VerificationIP.Contains(FunctionCenter.GetCustomerIP()))
            { Response.Write("非法请求地址,已关闭数据请求!"); Response.End(); }
            /***************************************************************************************************************
             * 获取成功或失败输出结果参数
             * *************************************************************************************************************/
            string trueBack = new Fooke.Function.String(cfgHelper["trueBack"]).toString("success");
            string falseBack = new Fooke.Function.String(cfgHelper["falseBack"]).toString("false");
            /***************************************************************************************************************
             * 获取配置用户请求参数数据信息
             * *************************************************************************************************************/
            string OpenName = new Fooke.Function.String(cfgHelper["OpenName"]).toString();
            if (OpenName == "") { this.ErrorMessage("获取参数配置错误,无法查询用户标识!", falseBack); Response.End(); }
            else if (OpenName.Length <= 0) { this.ErrorMessage("获取参数配置错误,无法查询用户标识!", falseBack); Response.End(); }
            else if (OpenName.Length >= 32) { this.ErrorMessage("获取参数配置错误,无法查询用户标识!", falseBack); Response.End(); }
            string UserIdentifier = RequestHelper.GetRequest(OpenName).toString();
            if (string.IsNullOrEmpty(UserIdentifier)) { this.ErrorMessage("获取请求用户参数信息失败！", falseBack); Response.End(); }
            else if (UserIdentifier.Length <= 0) { this.ErrorMessage("获取请求用户参数信息失败！", falseBack); Response.End(); }
            else if (UserIdentifier.Length <= 5) { this.ErrorMessage("获取请求用户参数信息失败！", falseBack); Response.End(); }
            DataRow MemberRs = GetMemberRs(UserIdentifier);
            if (MemberRs == null) { this.ErrorMessage("获取用户数据信息失败!", falseBack); Response.End(); }
            /***************************************************************************************************************
             * 获取奖励积分数据配置信息
             * *************************************************************************************************************/
            string NumberName = new Fooke.Function.String(cfgHelper["NumberName"]).toString();
            if (string.IsNullOrEmpty(NumberName)) { this.ErrorMessage("获取奖励积分配置参数信息失败！", falseBack); Response.End(); }
            else if (NumberName.Length <= 0) { this.ErrorMessage("获取奖励积分配置参数信息失败！", falseBack); Response.End(); }
            else if (NumberName.Length >= 20) { this.ErrorMessage("获取奖励积分配置参数信息失败！", falseBack); Response.End(); }
            double Points = RequestHelper.GetRequest(NumberName).cDouble();
            if (Points <= 0) { this.ErrorMessage("获取奖励积分数据信息失败!", falseBack); Response.End(); }
            else if (Points >= 10000) { this.ErrorMessage("奖励积分数据超过系统设置上限!", falseBack); Response.End(); }
            /***************************************************************************************************************
             * 获取下载应用名称数据信息
             * *************************************************************************************************************/
            string ApplicationName = new Fooke.Function.String(cfgHelper["ApplicationName"]).toString();
            if (string.IsNullOrEmpty(ApplicationName)) { this.ErrorMessage("获取应用名称配置信息失败！", falseBack); Response.End(); }
            else if (ApplicationName.Length <= 0) { this.ErrorMessage("获取应用名称配置信息失败！", falseBack); Response.End(); }
            string Appname = RequestHelper.GetRequest(ApplicationName).toString();
            if (string.IsNullOrEmpty(Appname)) { this.ErrorMessage("获取下载应用名称失败！", falseBack); Response.End(); }
            else if (Appname.Length <= 0) { this.ErrorMessage("获取下载应用名称失败！", falseBack); Response.End(); }
            else if (Appname.Length >= 30) { this.ErrorMessage("获取下载应用名称失败！", falseBack); Response.End(); }
            /***************************************************************************************************************
             * 获取下载应用包名ID数据信息
             * *************************************************************************************************************/
            string ApplicationID = new Fooke.Function.String(cfgHelper["ApplicationID"]).toString();
            if (ApplicationID == "") { ApplicationID = "AppID"; }
            string AppID = RequestHelper.GetRequest(ApplicationID).toString();
            /***************************************************************************************************************
             * 获取设备编号IDFA数据信息
             * *************************************************************************************************************/
            string Idfa = RequestHelper.GetRequest("Idfa").toString();
            if (string.IsNullOrEmpty(Idfa)) { Idfa = RequestHelper.GetRequest("udid").toString(); }
            if (string.IsNullOrEmpty(Idfa)) { Idfa = RequestHelper.GetRequest("deviceid").toString(); }
            if (string.IsNullOrEmpty(Idfa)) { Idfa = RequestHelper.GetRequest("device").toString(); }
            if (string.IsNullOrEmpty(Idfa)) { Idfa = RequestHelper.GetRequest("imie").toString(); }
            if (string.IsNullOrEmpty(Idfa)) { Idfa = MemberRs["DeviceCode"].ToString(); }
            /***************************************************************************************************************
             * 生成任务唯一标示数据信息
             * *************************************************************************************************************/
            string DutyKey = string.Format("联盟任务-|-|-{0}-|-|-{1}-|-|-{2}-|-|-联盟任务",
                MemberRs["UserID"].ToString(), UnionRs["UnionID"].ToString(), Appname, Points);
            DutyKey = string.Format("RW{0}", new Fooke.Function.String(DutyKey).ToMD5().Substring(0, 24).ToUpper());
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserDuty]", new Dictionary<string, object>() {
                {"DutyKey",DutyKey}
            });
            if (cRs != null) { this.ErrorMessage("回调记录已经存在，请返回重试！", falseBack); Response.End(); }
            /***************************************************************************************************************
             * 开始保存请求任务数据信息
             * *************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["DutyKey"] = DutyKey;
            thisDictionary["UnionID"] = UnionRs["UnionID"].ToString();
            thisDictionary["strUnion"] = UnionRs["strUnion"].ToString();
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["nickname"].ToString();
            thisDictionary["DeviceType"] = MemberRs["DeviceType"].ToString();
            thisDictionary["DeviceCode"] = Idfa;
            thisDictionary["AppID"] = AppID;
            thisDictionary["Appname"] = Appname;
            thisDictionary["strIP"] = MemberRs["strIP"].ToString();
            thisDictionary["strCity"] = MemberRs["strCity"].ToString();
            thisDictionary["Points"] = Points.ToString("0.00");
            thisDictionary["Amount"] = Points.ToString("0.00");
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserDuty]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试!", falseBack); Response.End(); }
            /*************************************************************************************************************
             * 发放用户奖励提成数据信息
             * ***********************************************************************************************************/
            string isTask = thisConfigure.GetParameter("isTask", "shareXml").toInt();
            int CTTimer = thisConfigure.GetParameter("CTTimer", "shareXml").cInt();
            if (isTask == "1" && MemberRs["ParentID"].ToString() != "0"
            && (new Fooke.Function.String(MemberRs["BonusTimer"].ToString()).cInt() <= CTTimer || CTTimer <= 0))
            {
                new BonusHelper().TaskBonus(UserID: MemberRs["UserID"].ToString(),
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
            {
                new Fooke.Code.PushCenter().Start(Configure: new Fooke.Code.ConfigureCenter(),
                    content: string.Format("恭喜完成任务获得奖励{0}元", Points.ToString("0.00")),
                    identify: MemberRs["userID"].ToString());
            }
            catch { }
            /***************************************************************************************************************
             * 输出数据处理结果信息
             * *************************************************************************************************************/
            Response.Write(trueBack);
            Response.End();
        }

        public DataRow GetMemberRs(string UserIdentifier)
        {
            DataRow MemberRs = null;
            if (UserIdentifier.Length >= 5 && UserIdentifier.Length <= 10
            && new Fooke.Function.String(UserIdentifier).cInt() != 0)
            {
                MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                    {"UserID",UserIdentifier}
                });
            }
            else if (UserIdentifier.Length >= 11 && UserIdentifier.Length <= 40)
            {
                MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUnionMember]", new Dictionary<string, object>() {
                    {"DeviceIdentifier",UserIdentifier}
                });
            }
            return MemberRs;
        }
        /// <summary>
        /// 输出数据处理结果
        /// </summary>
        /// <param name="strTips"></param>
        /// <param name="falseBack"></param>
        protected void ErrorMessage(string strTips, string falseBack)
        {
            falseBack = falseBack.Replace("{$tips}", strTips);
            Response.Write(falseBack);
            Response.End();
        }

        /// <summary>
        /// 获取网页请求参数内容
        /// </summary>
        /// <returns></returns>
        public string GetParameter()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<div>");
            try
            {
                foreach (string vChar in Request.Form)
                {
                    strBuilder.Append("<span>" + vChar + ":" + RequestHelper.GetRequest(vChar) + "</li>");
                }
                foreach (string vChar in Request.QueryString)
                {
                    strBuilder.Append("<span>" + vChar + ":" + RequestHelper.GetRequest(vChar) + "</li>");
                }
            }
            catch { }
            strBuilder.Append("</div>");
            return strBuilder.ToString();
        }
    }
}