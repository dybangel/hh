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
namespace Fooke.Web.Member
{
    public partial class UnionChannel : Fooke.Web.UserHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "wallet": strWallet(); Response.End(); break;
                case "transfer": strTransfer(); Response.End(); break;
                default: this.strDefault(); Response.End(); break;
            }
        }

        /// <summary>
        /// 转移资产
        /// </summary>
        protected void strWallet()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/amount/wallet.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    case "amount": strValue = new Fooke.Function.String(MemberRs["Amount"].ToString()).cDouble().ToString("0.00"); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 转移资产
        /// </summary>
        protected void strTransfer()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/amount/transfer.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    case "amount": strValue = new Fooke.Function.String(MemberRs["Amount"].ToString()).cDouble().ToString("0.00"); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 显示我的地址信息
        /// </summary>
        protected void strDefault()
        {
            /*******************************************************************************************
             * 输出数据处理结果
             * *****************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/UnionChannel/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    case "amount": strValue = new Fooke.Function.String(MemberRs["Amount"].ToString()).cDouble().ToString("0.00"); break;
                    default: try { strValue = MemberRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }
}