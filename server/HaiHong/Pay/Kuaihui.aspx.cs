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
    public partial class Kuaihui : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /***********************************************************************************
             * 验证签名信息
             * *********************************************************************************/
            string Keys = RequestHelper.GetRequest("Keys").ToString();
            if (string.IsNullOrEmpty(Keys)) { Response.Write("false"); Response.End(); }
            if (Keys != "e491eb6y804") { Response.Write("false"); Response.End(); }
            /***********************************************************************************
             * 验证请求参数合法性
             * *********************************************************************************/
            string RechID = RequestHelper.GetRequest("Payuser").toInt();
            if (RechID == "0") { Response.Write("false"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindRechargeableLog]", new Dictionary<string, object>() {
                {"RechID",RechID}
            });
            if (cRs == null) { Response.Write("false"); Response.End(); }
            else if (cRs["isFinish"].ToString() != "0") { Response.Write("false"); Response.End(); }
            /***********************************************************************************
             * 验证用户充值金额信息
             * *********************************************************************************/
            double Amount = RequestHelper.GetRequest("money").cDouble();
            if (Amount <= 0) { Response.Write("false"); Response.End(); }
            /***********************************************************************************
             * 保存用户充值订单信息
             * *********************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["RechID"] = cRs["RechID"].ToString();
            oDictionary["strKey"] = cRs["strKey"].ToString();
            oDictionary["UserID"] = cRs["UserID"].ToString();
            oDictionary["Nickname"] = cRs["Nickname"].ToString();
            oDictionary["Amount"] = Amount.ToString("0.00");
            oDictionary["PaymentName"] = cRs["PaymentName"].ToString();
            oDictionary["PaymentID"] = cRs["PaymentID"].ToString();
            oDictionary["OrderID"] = cRs["OrderID"].ToString();
            oDictionary["OrderMode"] = cRs["OrderMode"].ToString();
            oDictionary["Bonus"] = "0";
            oDictionary["Remark"] = cRs["Remark"].ToString();
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveRechargeable]", oDictionary);
            if (oRs == null) { Response.Write("false"); Response.End(); }
            else if (oRs != null && oRs["isFinish"].ToString() != "1") { Response.Write("false"); Response.End(); }
            /********************************************************************************
             * 返回数据处理结果
             * ******************************************************************************/
            Response.Write("Success");
            Response.End();
        }
    }
}