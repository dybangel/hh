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
    public partial class Ecpss : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strKey = RequestHelper.GetRequest("BillNo", false).toString();
            if (string.IsNullOrEmpty(strKey)) { Response.Write("Validation failed!"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindRechargeableLog", new Dictionary<string, object>() { { "strKey", strKey } });
            if (cRs == null) { Response.Write("Validation failed!"); Response.End(); }
            if (cRs["isFinish"].ToString() != "0") { Response.Write("Validation failed!"); Response.End(); }
            /******************************************************************************
             * 拉取回调数据验证签名,验证签名
             * *****************************************************************************/
            try
            {
                string Amount = RequestHelper.GetRequest("Amount").toString();
                string Succeed = RequestHelper.GetRequest("Succeed").toString();
                string Token = RequestHelper.GetRequest("SignMD5info").toString();
                string MD5key = cRs["BusinessKey"].ToString();
                string thisKey = strKey + "&" + Amount + "&" + Succeed + "&" + MD5key;
                thisKey = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(thisKey, "md5").ToUpper();
                if (Token != thisKey) { Response.Write("Validation failed!"); Response.End(); }
            }
            catch { }
            /********************************************************************************
             * 更新网络数据
             * ******************************************************************************/
            new PaymentHelper().SaveUpdate(cRs, iSuccess =>
            {
                if (iSuccess) { Response.Write("ok"); Response.End(); }
                else { Response.Write("fail"); Response.End(); }
            });
            /*******************************************************************************
             * 输出网络回传
             * *****************************************************************************/
            Response.Write("ok");
            Response.End();
        }
    }
}