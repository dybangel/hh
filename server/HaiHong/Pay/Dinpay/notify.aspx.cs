using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using Fooke.Code;
using Fooke.Function;
namespace Fooke.Web.Pay
{
    public partial class Dinpay : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strKey = RequestHelper.GetRequest("order_no", false).toString();
            if (string.IsNullOrEmpty(strKey)) { Response.Write("fail"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() { { "strKey", strKey } });
            if (cRs == null) { Response.Write("fail"); Response.End(); }
            if (cRs["isFinish"].ToString() != "0") { Response.Write("fail"); Response.End(); }
            /************************************************************************************
             * 拉取回传参数用来做数据签名
             * **********************************************************************************/
            SortedDictionary<string, string> thisDictionary = new SortedDictionary<string, string>();
            try
            {
                foreach (string strName in Request.QueryString)
                {
                    if (!string.IsNullOrEmpty(strName) && !string.IsNullOrEmpty(Request.QueryString[strName]))
                    { thisDictionary[strName.ToLower()] = Request.QueryString[strName].Trim(); }
                }
            }
            catch { }
            if (thisDictionary == null || thisDictionary.Count <= 0) { Response.Write("fail"); Response.End(); }
            /******************************************************************************
             * 验证签名
             * *****************************************************************************/
            try
            {
                string Signature = SignatureText(thisDictionary, cRs["BusinessKey"].ToString());
                string dinpaySign = RequestHelper.GetRequest("sign", false).toString();
                if (Signature != dinpaySign) { Response.Write("fail"); Response.End(); }
                new PaymentHelper().SaveUpdate(cRs, iSuccess =>
                {
                    if (iSuccess) { Response.Write("SUCCESS"); Response.End(); }
                    else { Response.Write("fail"); Response.End(); }
                });
            }
            catch { }
            /*******************************************************************************
             * 输出网络回传
             * *****************************************************************************/
            Response.Write("SUCCESS");
            Response.End();
        }
        /// <summary>
        /// 数据签名
        /// </summary>
        /// <param name="thisDictionary"></param>
        /// <param name="thisKey"></param>
        /// <returns></returns>
        public string SignatureText(SortedDictionary<string, string> thisDictionary, string thisKey)
        {
            string thisSignature = string.Empty;
            try
            {
                foreach (KeyValuePair<string, string> Pair in thisDictionary) { if (Pair.Value.ToLower() != "sign") { thisSignature = thisSignature + "" + Pair.Key.ToLower() + "=" + Pair.Value + "&"; } }
                thisSignature = thisSignature + "key=" + thisKey;
                thisSignature = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(thisSignature, "md5").ToLower();
            }
            catch { }
            return thisSignature;
        }

    }
}