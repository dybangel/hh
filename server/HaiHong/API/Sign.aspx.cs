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
    public partial class Sign : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {

                case "save": AddSave(); Response.End(); break;
                case "starRed": StarRed(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }

        /// <summary>
        /// 是否为签到空白期
        /// </summary>
        /// <returns></returns>
        protected bool isBlank()
        {
            bool isClear = false;
            /**************************************************************************************
             * 开始执行数据查询
             * ************************************************************************************/
            try
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) { isClear = true; }
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday) { isClear = true; }
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday
                && new Fooke.Function.String(DateTime.Now.Hour.ToString("00")).cInt() >= 16)
                { isClear = true; }
            }
            catch { }
            /**************************************************************************************
             * 返回数据处理结果
             * ************************************************************************************/
            return isClear;
        }

        protected void strDefault()
        {
            /*******************************************************************************************************
             * 获取我的签到记录信息
             * ******************************************************************************************************/
            DataRow SignRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindSignThis]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"Nickname",MemberRs["Nickname"].ToString()},
                {"Yesday",DateTime.Now.AddDays(-1).ToString("yyyyMMdd")},
                {"isClear",(isBlank() ? "0" : "1")}
            });
            if (SignRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (SignRs["UserID"].ToString() != MemberRs["UserID"].ToString())
            { this.ErrorMessage("越权操作,请重试！"); Response.End(); }
            /************************************************************************************************************
             * 获取用户连续签到次数
             * **********************************************************************************************************/
            ResponseDataRow(SignRs);
            Response.End();
        }

        /// <summary>
        /// 确认签到信息
        /// </summary>
        protected void AddSave()
        {
            /*********************************************************************************************************
             * 验证签到功能是否开启
             * *******************************************************************************************************/
            string SignOpen = this.GetParameter("SignOpen", "SignXml").toInt();
            if (SignOpen != "1") { this.ErrorMessage("签到活动功能已经关闭,请联系客服！"); Response.End(); }
            /*********************************************************************************************************
             * 验证签到时间的合法性
             * *******************************************************************************************************/
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) { this.ErrorMessage("今天是星期天,不能签到！"); Response.End(); }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday) { this.ErrorMessage("今天是星期六,不能签到！"); Response.End(); }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday && new Fooke.Function.String(DateTime.Now.Hour.ToString("00")).cInt() >= 16)
            { this.ErrorMessage("已经超过签到时间,请下个礼拜再来吧！"); Response.End(); }
            /*******************************************************************************************************
             * 获取我的签到记录信息
             * ******************************************************************************************************/
            DataRow SignRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindSignThis]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"Nickname",MemberRs["Nickname"].ToString()},
                {"Yesday",DateTime.Now.AddDays(-1).ToString("yyyyMMdd")},
            });
            if (SignRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (SignRs["UserID"].ToString() != MemberRs["UserID"].ToString()) { this.ErrorMessage("越权操作,请重试！"); Response.End(); }
            else if (SignRs["DateKey"].ToString() == DateTime.Now.ToString("yyyyMMdd")) { this.ErrorMessage("您今日已经签到过了,无需重试签到！"); Response.End(); }
            /*******************************************************************************************************
             * 获取用户已经连续签到的天数
             * ******************************************************************************************************/
            double Continuous = new Fooke.Function.String(SignRs["Repeatnum"].ToString()).cDouble();
            /*******************************************************************************************************
             * 计算用户当日签到的金额信息,并且计算是否开启奖励
             * ******************************************************************************************************/
            double SignAmount = new Fooke.Function.String("0").cDouble();
            /*******************************************************************************************************
             * 生成用户签名标识
             * ******************************************************************************************************/
            string SignKey = string.Format("用户签名-|-|-{0}-|-|-{1}-|-|-用户签名", MemberRs["UserID"].ToString(), DateTime.Now.ToString("yyyyMMdd"));
            SignKey = new Fooke.Function.String(SignKey).ToMD5().Substring(0, 24).ToUpper();
            DataRow iRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserSignLogs]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"SignKey",SignKey}
            });
            if (iRs != null) { this.ErrorMessage("您已经签到过了,无需重复签到！"); Response.End(); }
            /*********************************************************************************************************
             * 开始保存请求数据
             * *******************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["SignKey"] = SignKey;
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["DateKey"] = DateTime.Now.ToString("yyyyMMdd");
            thisDictionary["SignAmount"] = SignAmount.ToString("0.00");
            thisDictionary["RepeatAmount"] = "0";
            thisDictionary["thisAmount"] = "0";
            thisDictionary["RepeatNum"] = (Continuous + 1).ToString();
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserSign]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生错误,请重试！"); Response.End(); }
            /*********************************************************************************************************
             * 输出数据处理结果
             * *******************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"签到成功,获得奖励积分\"");
            strBuilder.Append(",\"point\":\"0\"");
            strBuilder.Append(",\"type\":\"define\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 开启抢红包记录
        /// </summary>
        protected void StarRed()
        {
            /*********************************************************************************************************
             * 验证签到功能是否开启
             * *******************************************************************************************************/
            string SignOpen = this.GetParameter("SignOpen", "signXml").toInt();
            if (SignOpen != "1") { this.ErrorMessage("已关闭签到抢红包功能,请联系客服！"); Response.End(); }
            /*********************************************************************************************************
             * 验证签到时间的合法性
             * *******************************************************************************************************/
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday && DateTime.Now.DayOfWeek != DayOfWeek.Saturday
            && DateTime.Now.DayOfWeek != DayOfWeek.Friday) { this.ErrorMessage("抢红包的时间还没到哦！"); Response.End(); }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday && new Fooke.Function.String(DateTime.Now.Hour.ToString("00")).cInt() <= 15)
            { this.ErrorMessage(new Fooke.Function.String(DateTime.Now.Hour.ToString("00")).cInt()+"抢红包的时间还没到哦！"); Response.End(); }
            /*******************************************************************************************************
             * 获取我的签到记录信息
             * ******************************************************************************************************/
            DataRow SignRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindSignThis]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"Nickname",MemberRs["Nickname"].ToString()},
                {"Yesday",DateTime.Now.AddDays(-1).ToString("yyyyMMdd")},
                {"isClear","0"}
            });
            if (SignRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (SignRs["UserID"].ToString() != MemberRs["UserID"].ToString()) { this.ErrorMessage("越权操作,请重试！"); Response.End(); }
            /*******************************************************************************************************
             * 获取用户已经连续签到的天数
             * ******************************************************************************************************/
            double Continuous = new Fooke.Function.String(SignRs["Repeatnum"].ToString()).cDouble();
            if (Continuous <= 0) { this.ErrorMessage("连续签到次数不足,请下次再来吧！"); Response.End(); }
            else if (Continuous <= 4) { this.ErrorMessage("连续签到次数不足,请下次再来吧！"); Response.End(); }
            /*******************************************************************************************************
             * 生成用户领取红包的Datekey
             * ******************************************************************************************************/
            DateTime FridayDate = FunctionCenter.GetWeekUpOfDate(DateTime.Now, DayOfWeek.Friday, 0);
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) { FridayDate = FunctionCenter.GetWeekUpOfDate(DateTime.Now, DayOfWeek.Friday, -1); }
            /*******************************************************************************************************
            * 获取抢夺红包奖励金额
            * ******************************************************************************************************/
            double thisAmount = this.GetParameter("RedAmount", "signXml").cDouble();
            if (thisAmount <= 0) { this.ErrorMessage("系统参数配置错误,请联系客服！"); Response.End(); }
            else if (thisAmount >= 1000) { this.ErrorMessage("系统参数配置错误,请联系客服！"); Response.End(); }
            /*******************************************************************************************************
             * 判断用户是否拥有抢红包权限
             * ******************************************************************************************************/
            int Rednumber = this.GetParameter("Rednumber", "signXml").cInt();
            /*******************************************************************************************************
             * 获取系统配置参数数据信息
             * ******************************************************************************************************/
            DataRow iRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserSignRedThis]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"DateKey",FridayDate.ToString("yyyyMMdd")}
            });
            if (iRs == null) { this.ErrorMessage("获取系统配置信息失败,请联系客服！"); Response.End(); }
            else if (iRs.Table.Columns["Timer"] == null) { this.ErrorMessage("获取系统配置信息失败,请联系客服！"); Response.End(); }
            else if (iRs.Table.Columns["isUnder"] == null) { this.ErrorMessage("获取系统配置信息失败,请联系客服！"); Response.End(); }
            /*******************************************************************************************************
             * 验证用户是否已经抢过红包了
             * ******************************************************************************************************/
            if (new Fooke.Function.String(iRs["isUnder"].ToString()).cInt() >= 1)
            { this.ErrorMessage("您已经抢过红包了,请下载再来吧！"); Response.End(); }
            if (Rednumber != 0 && new Fooke.Function.String(iRs["Timer"].ToString()).cInt() >= Rednumber)
            { this.ErrorMessage("红包已经被抢完了,请下次再来吧！"); Response.End(); }
            /*******************************************************************************************************
             * 获取抢红包的Key值是否正确
             * ******************************************************************************************************/
            string RedKey = string.Format("签到红包-|-|-{0}-|-|-{1}-|-|-签到红包",
                MemberRs["UserID"].ToString(), FridayDate.ToString("yyyyMMdd"));
            RedKey = new Fooke.Function.String(RedKey).ToMD5().Substring(0, 24).ToUpper();
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserSignRed]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"RedKey",RedKey}
            });
            if (cRs != null) { this.ErrorMessage("您已经抢过红包了,不允许重复抢夺！"); Response.End(); }
            /*******************************************************************************************************
             * 开始保存请求数据信息
             * ******************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["RedKey"] = RedKey;
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["DateKey"] = FridayDate.ToString("yyyyMMdd");
            thisDictionary["thisAmount"] = thisAmount.ToString("0.00");
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserSignRed]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /*******************************************************************************************************
            * 输出数据处理结果信息
            * ******************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"抢夺成功,获得红包奖励" + thisAmount + "元\"");
            strBuilder.Append(",\"point\":\"" + thisAmount + "\"");
            strBuilder.Append(",\"type\":\"define\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}