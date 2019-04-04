using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using Fooke.Code;
using System.Security.Cryptography;
using Fooke.Function;
namespace Fooke.Web.API
{
    public partial class Effective : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strRequest = RequestHelper.GetRequest("action").toString();
            if (string.IsNullOrEmpty(strRequest)) { strRequest = "default"; }
            if (strRequest == "effkey") { this.EffectiveSave(); Response.End(); }
            Response.End();
        }
        /// <summary>
        /// 验证IP地址是否合法
        /// </summary>
        protected void EffectiveSave()
        {
            /*********************************************************************************************************
             * 获取并验证请求数据信息
             * *******************************************************************************************************/
            string strKey = RequestHelper.GetRequest("strKey").toString();
            if (string.IsNullOrEmpty(strKey)) { Response.Write("验证错误！"); Response.End(); }
            else if (strKey.Length != 32) { Response.Write("验证错误！"); Response.End(); }
            /*********************************************************************************************************
             * 验证管理员账号密码信息
             * *******************************************************************************************************/
            string AdminName = RequestHelper.GetRequest("AdminName").toString();
            if (string.IsNullOrEmpty(AdminName)) { Response.Write("参数未填写完整！"); Response.End(); }
            else if (AdminName.Length <= 4) { Response.Write("参数未填写完整！"); Response.End(); }
            else if (AdminName.Length >= 16) { Response.Write("参数未填写完整！"); Response.End(); }
            string Password = RequestHelper.GetRequest("Password").toString();
            if (string.IsNullOrEmpty(Password)) { Response.Write("参数未填写完整！"); Response.End(); }
            else if (Password.Length <= 5) { Response.Write("参数错误！"); Response.End(); }
            else if (Password.Length >= 16) { Response.Write("参数错误！"); Response.End(); }
            /*********************************************************************************************************
             * 获取并验证解锁密码合法性
             * *******************************************************************************************************/
            string vPassword = RequestHelper.GetRequest("vPassword").toString();
            if (string.IsNullOrEmpty(vPassword)) { Response.Write("参数未填写完整！"); Response.End(); }
            else if (vPassword.Length <= 5) { Response.Write("参数错误！"); Response.End(); }
            else if (vPassword.Length >= 16) { Response.Write("参数错误！"); Response.End(); }
            /*********************************************************************************************************
             * 获取并验证随机认证码信息
             * *******************************************************************************************************/
            string rnd = RequestHelper.GetRequest("rnd").toString();
            if (string.IsNullOrEmpty(rnd)) { Response.Write("参数错误！"); Response.End(); }
            if (rnd.Length != 6) { Response.Write("参数错误！"); Response.End(); }
            /*********************************************************************************************************
             * 验证加密参数合法性
             * *******************************************************************************************************/
            string oKey = string.Format("数据加密-|-|-{0}-|-|-{1}-|-|-{2}-|-|-{3}-|-|-数据加密",
                AdminName, Password, vPassword, rnd);
            oKey = ConvertToMD5(ConvertToMD5(oKey) + "-AAA-" + AdminName);
            if (strKey.ToLower() != oKey.ToLower()) { Response.Write(strKey + "数据验证不成功！" + oKey); Response.End(); }
            /*********************************************************************************************************
             * 加密管理员登陆密码
             * *******************************************************************************************************/
            Password = new Fooke.Function.String(Password).ToMD5().ToMD5().Substring(0, 20).ToLower();
            /*********************************************************************************************************
             * 开始保存请求IP地址数据
             * *******************************************************************************************************/
            DataRow cRs = DbHelper.Connection.FindRow("Fooke_Admin", Params: " and AdminName='" + AdminName + "'");
            if (cRs == null) { Response.Write("帐号或者密码错误！"); Response.End(); }
            else if (cRs["Password"].ToString().ToLower() != Password.ToLower()) { Response.Write("帐号或者密码错误！"); Response.End(); }
            else if (cRs["vPassword"].ToString().ToLower() != vPassword.ToLower()) { Response.Write("验证密码错误！"); Response.End(); }
            else if (cRs["isMat"].ToString() != "1") { Response.Write("该帐号无IP限制，无需认证！"); Response.End(); }
            /*********************************************************************************************************
             * 开始保存请求IP地址数据
             * *******************************************************************************************************/
            DbHelper.Connection.Update("Fooke_Admin", dictionary: new Dictionary<string, string>() {
                {"effIP",FunctionCenter.GetCustomerIP()}
            }, Params: " and AdminID=" + cRs["AdminID"] + "");
            /*********************************************************************************************************
             * 输出数据处理结果
             * *******************************************************************************************************/
            Response.Write("IP认证成功,现在可以登陆啦！");
            Response.End();
        }

        /// <summary>
        /// 将字符串进行MD5加密
        /// </summary>
        /// <param name="sDataIn"></param>
        /// <returns></returns>
        public string ConvertToMD5(string sDataIn)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = System.Text.Encoding.UTF8.GetBytes(sDataIn);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp;
        }
    }
}