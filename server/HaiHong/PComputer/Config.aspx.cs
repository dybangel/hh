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
    public partial class Config : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 输出系统配置参数信息
        /// </summary>
        protected void strDefault()
        {
            /*****************************************************************************************
             * 构建参数配置信息
             * ****************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("var cfg = {");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"user\":" + GetUserInfo(MemberRs) + "");
            strBuilder.Append(",\"aide\":" + GetAssistant(MemberRs) + "");
            strBuilder.Append("};");
            /*****************************************************************************************
             * 输出参数处理结果
             * ****************************************************************************************/
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        public string GetUserInfo(DataRow MemberRs)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"userid\":\"" + MemberRs["userid"] + "\"");
            strBuilder.Append(",\"parentid\":\"" + MemberRs["ParentID"] + "\"");
            strBuilder.Append(",\"nickname\":\"" + MemberRs["nickname"] + "\"");
            strBuilder.Append(",\"thumb\":\"" + FunctionCenter.ConvertPath(MemberRs["thumb"].ToString()) + "\"");
            strBuilder.Append(",\"devicetype\":\"" + MemberRs["devicetype"] + "\"");
            strBuilder.Append(",\"devicemodel\":\"" + ReplaceModel(MemberRs["devicemodel"].ToString()) + "\"");
            strBuilder.Append(",\"devicechar\":\"" + ReplaceEdition(MemberRs["devicechar"].ToString()) + "\"");
            strBuilder.Append(",\"devicecode\":\"" + MemberRs["DeviceCode"] + "\"");
            strBuilder.Append(",\"deviceidentifier\":\"" + MemberRs["DeviceIdentifier"] + "\"");
            strBuilder.Append(",\"tokey\":\"" + MemberRs["strTokey"] + "\"");
            strBuilder.Append(",\"strtokey\":\"" + MemberRs["strTokey"] + "\"");
            strBuilder.Append(",\"amount\":\"" + MemberRs["amount"] + "\"");
            strBuilder.Append(",\"points\":\"" + MemberRs["Points"] + "\"");
            strBuilder.Append(",\"strcity\":\"" + MemberRs["strCity"] + "\"");
            strBuilder.Append(",\"isbreak\":\"" + MemberRs["isBreak"] + "\"");
            strBuilder.Append("}");
            return strBuilder.ToString();
        }

        public string GetAssistant(DataRow MemberRs)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            if (new BrowserHelper().Browser() == BrowserHelper.BrowserType.iOS)
            {
                strBuilder.Append(",\"appname\":\"" + this.GetParameter("iOSName", "appXml").toString() + "\"");
                strBuilder.Append(",\"deposit\":\"" + this.GetParameter("iOSDeposit", "appXml").toString() + "\"");
                strBuilder.Append(",\"edition\":\"" + this.GetParameter("iOSEdition", "appXml").toString() + "\"");
                strBuilder.Append(",\"port\":\"" + this.GetParameter("iOSport", "appXml").toString() + "\"");
                strBuilder.Append(",\"server\":\"http://127.0.0.1:" + this.GetParameter("Androidport", "appXml").toString() + "\"");
                strBuilder.Append(",\"packer\":\"" + this.GetParameter("iOSPackerName", "appXml").toString() + "\"");
                strBuilder.Append(",\"devicetype\":\"ios\"");
                strBuilder.Append(",\"zhdevice\":\"苹果系统\"");
            }
            else
            {
                strBuilder.Append(",\"appname\":\"" + this.GetParameter("AndroidName", "appXml").toString() + "\"");
                strBuilder.Append(",\"deposit\":\"" + this.GetParameter("AndroidDeposit", "appXml").toString() + "\"");
                strBuilder.Append(",\"edition\":\"" + this.GetParameter("AndroidEdition", "appXml").toString() + "\"");
                strBuilder.Append(",\"port\":\"" + this.GetParameter("Androidport", "appXml").toString() + "\"");
                strBuilder.Append(",\"server\":\"http://127.0.0.1:" + this.GetParameter("Androidport", "appXml").toString() + "\"");
                strBuilder.Append(",\"packer\":\"" + this.GetParameter("AndroidPackerName", "appXml").toString() + "\"");
                strBuilder.Append(",\"devicetype\":\"android\"");
                strBuilder.Append(",\"zhdevice\":\"安卓系统\"");
            }
            strBuilder.Append(",\"isWeb\":\"false\"");
            strBuilder.Append(",\"isWeixin\":\"" + new BrowserHelper().IsMicroMessenger() + "\"");
            strBuilder.Append(",\"isTencent\":\"" + new BrowserHelper().IsQQBrowser() + "\"");
            strBuilder.Append(",\"isMobile\":\"" + new BrowserHelper().IsMoblie() + "\"");
            strBuilder.Append("}");
            return strBuilder.ToString();
        }

        /************************************************************************************************************************
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * 公用方法处理区域
         * ☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
         * **********************************************************************************************************************/
        #region 公用方法处理区域
        /// <summary>
        /// 转换用户系统设备型号
        /// </summary>
        /// <param name="strChar"></param>
        /// <returns></returns>
        public string ReplaceModel(string strChar)
        {
            /********************************************************************************************************
             * 开始执行数据处理
             * *******************************************************************************************************/
            try { strChar = strChar.Replace(" ", "").Replace("iOS", ""); }
            catch { }
            try { if (strChar.Contains(",")) { strChar = strChar.Substring(0, strChar.LastIndexOf(",") + 1); } }
            catch { }
            try
            {
                if (strChar.ToLower().Contains("iphone4")) { strChar = "iPhone4"; }
                
                else if (strChar.ToLower().Contains("iphone5s")) { strChar = "iPhone5S"; }
                else if (strChar.ToLower().Contains("iphone5c")) { strChar = "iPhone5C"; }
                else if (strChar.ToLower().Contains("iphone5")) { strChar = "iPhone5"; }
                else if (strChar.ToLower().Contains("iphone6splus")) { strChar = "iPhone6SP"; }
                else if (strChar.ToLower().Contains("iphone6plus")) { strChar = "iPhone6P"; }
                else if (strChar.ToLower().Contains("iphone6s")) { strChar = "iPhone6S"; }
                else if (strChar.ToLower().Contains("iphone6")) { strChar = "iPhone6"; }
                else if (strChar.ToLower().Contains("iphonese")) { strChar = "iPhoneSE"; }
                else if (strChar.ToLower().Contains("iphone7plus")) { strChar = "iPhone7P"; }
                else if (strChar.ToLower().Contains("iphone8")) { strChar = "iPhone8P"; }
                else if (strChar.ToLower().Contains("iphone8plus")) { strChar = "iPhoneX"; }
                else if (strChar.ToLower().Contains("iphone10")) { strChar = "iPhoneXS"; }
            }
            catch { }
            /********************************************************************************************************
             * 返回数据处理结果信息
             * *******************************************************************************************************/
            return strChar;
        }
        /// <summary>
        /// 将用户系统版本转化
        /// </summary>
        /// <param name="strChar"></param>
        /// <returns></returns>
        public string ReplaceEdition(string strChar)
        {
            string Edition = "0";
            try
            {
                try { strChar = strChar.Replace(",", ".").Replace(" ", ""); }
                catch { }
                string[] arrTemp = strChar.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTemp != null && arrTemp.Length >= 1) { Edition = arrTemp[0]; }
                return Edition;
            }
            catch { return strChar; }
        }
        #endregion
    }
}