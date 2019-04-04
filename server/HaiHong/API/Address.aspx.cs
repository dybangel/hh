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
    public partial class Address : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "del": Delete(); Response.End(); break;
                case "save": AddSave(); Response.End(); break;
                case "editsave": SaveUpdate(); Response.End(); break;
                case "default": strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 默认列表信息
        /// </summary>
        protected void strDefault()
        {
            /*****************************************************************************************************
             * 构建分页查询语句条件
             * ***************************************************************************************************/
            string strParams = " and UserID=" + MemberRs["UserID"] + "";
            /*****************************************************************************************************
            * 获取分页查询数据
            * ***************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /*****************************************************************************************************
            * 构建分页查询语句
            * ***************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "DeliveryID,DeliveryKey,UserID,Nickname,Fullname,Address,strMobile,Addtime,isDefault";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "DeliveryID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " DeliveryID Desc";
            PageCenterConfig.Tablename = "Fooke_UserDelivery";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserDelivery", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
            * 输出数据处理结果
            * ***************************************************************************************************/
            ResponseDataTable(Tab, Record);
            Response.End();
        }
       
        /// <summary>
        /// 保存添加
        /// </summary>
        protected void AddSave()
        {
            /****************************************************************************************************************
             * 获取并验证收货地址信息
             * **************************************************************************************************************/
            string Address = RequestHelper.GetRequest("Address").ToString();
            if (string.IsNullOrEmpty(Address)) { this.ErrorMessage("请填写收货详细地址！"); Response.End(); }
            else if (Address.Length <= 10) { this.ErrorMessage("收货详细地址不能少于10个汉字！"); Response.End(); }
            else if (Address.Length >= 60) { this.ErrorMessage("收货详细地址长度不能超过60个汉字！"); Response.End(); }
            /****************************************************************************************************************
             * 获取手机号码数据信息
             * **************************************************************************************************************/
            string strMobile = RequestHelper.GetRequest("strMobile").ToString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("请填写收货人联系电话！"); Response.End(); }
            else if (strMobile.Length <= 6) { this.ErrorMessage("收货人联系电话长度不能少于7个字符！"); Response.End(); }
            else if (strMobile.Length >= 24) { this.ErrorMessage("收货人联系电话长度不能超过24个字符！"); Response.End(); }
            /****************************************************************************************************************
             * 获取并验证收货人地址信息
             * **************************************************************************************************************/
            string Fullname = RequestHelper.GetRequest("Fullname").ToString();
            if (string.IsNullOrEmpty(Fullname)) { this.ErrorMessage("收货人姓名不能为空！"); Response.End(); }
            else if (Fullname.Length <= 1) { this.ErrorMessage("收货人姓名长度不能少于2个汉字！"); Response.End(); }
            else if (Fullname.Length >= 12) { this.ErrorMessage("收货人姓名长度不能超过12个汉字内！"); Response.End(); }
            /****************************************************************************************************************
            * 获取不需要验证的数据
            * **************************************************************************************************************/
            string isDefault = RequestHelper.GetRequest("isDefault").toInt();
            /****************************************************************************************************************
             * 生成收货地址唯一标示
             * **************************************************************************************************************/
            string DeliveryKey = string.Format("收货地址-|-|-{0}-|-|-{1}-|-|-{2}-|-|-收货地址",
                MemberRs["UserID"].ToString(), DateTime.Now.ToString("yyyyMMddHHmmss"), Guid.NewGuid().ToString());
            DeliveryKey = new Fooke.Function.String(DeliveryKey).ToMD5().Substring(0, 24).ToUpper();
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserDelivery]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()},
                {"DeliveryKey",DeliveryKey}
            });
            if (oRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /****************************************************************************************************************
             * 开始保存用户绑定银行卡信息
             * **************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["DeliveryKey"] = DeliveryKey;
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["Fullname"] = Fullname;
            thisDictionary["Address"] = Address;
            thisDictionary["strMobile"] = strMobile;
            thisDictionary["isDefault"] = isDefault;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserDelivery]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            ResponseDataRow(thisRs);
            Response.End();
        }


        /// <summary>
        /// 保存编辑
        /// </summary>
        protected void SaveUpdate()
        {
            /****************************************************************************************************************
             * 验证请求数据的合法性
             * **************************************************************************************************************/
            string DeliveryID = RequestHelper.GetRequest("DeliveryID").toInt();
            if (DeliveryID == "0") { this.ErrorMessage("获取请求参数失败,请重试!"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserDelivery]", new Dictionary<string, object>() {
                {"DeliveryID",DeliveryID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (cRs["UserID"].ToString() != MemberRs["UserID"].ToString())
            { this.ErrorMessage("越权操作,请重试！"); Response.End(); }
            /****************************************************************************************************************
             * 获取并验证收货地址信息
             * **************************************************************************************************************/
            string Address = RequestHelper.GetRequest("Address").ToString();
            if (string.IsNullOrEmpty(Address)) { this.ErrorMessage("请填写收货详细地址！"); Response.End(); }
            else if (Address.Length <= 10) { this.ErrorMessage("收货详细地址不能少于10个汉字！"); Response.End(); }
            else if (Address.Length >= 60) { this.ErrorMessage("收货详细地址长度不能超过60个汉字！"); Response.End(); }
            /****************************************************************************************************************
             * 获取手机号码数据信息
             * **************************************************************************************************************/
            string strMobile = RequestHelper.GetRequest("strMobile").ToString();
            if (string.IsNullOrEmpty(strMobile)) { this.ErrorMessage("请填写收货人联系电话！"); Response.End(); }
            else if (strMobile.Length <= 6) { this.ErrorMessage("收货人联系电话长度不能少于7个字符！"); Response.End(); }
            else if (strMobile.Length >= 24) { this.ErrorMessage("收货人联系电话长度不能超过24个字符！"); Response.End(); }
            /****************************************************************************************************************
             * 获取并验证收货人地址信息
             * **************************************************************************************************************/
            string Fullname = RequestHelper.GetRequest("Fullname").ToString();
            if (string.IsNullOrEmpty(Fullname)) { this.ErrorMessage("收货人姓名不能为空！"); Response.End(); }
            else if (Fullname.Length <= 1) { this.ErrorMessage("收货人姓名长度不能少于2个汉字！"); Response.End(); }
            else if (Fullname.Length >= 12) { this.ErrorMessage("收货人姓名长度不能超过12个汉字内！"); Response.End(); }
            /****************************************************************************************************************
            * 获取不需要验证的数据
            * **************************************************************************************************************/
            string isDefault = RequestHelper.GetRequest("isDefault").toInt();
            /****************************************************************************************************************
             * 开始保存用户绑定银行卡信息
             * **************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["DeliveryID"] = cRs["DeliveryID"].ToString();
            thisDictionary["DeliveryKey"] = cRs["DeliveryKey"].ToString();
            thisDictionary["UserID"] = cRs["UserID"].ToString();
            thisDictionary["Nickname"] = cRs["Nickname"].ToString();
            thisDictionary["Fullname"] = Fullname;
            thisDictionary["Address"] = Address;
            thisDictionary["strMobile"] = strMobile;
            thisDictionary["isDefault"] = isDefault;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserDelivery]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            ResponseDataRow(thisRs);
            Response.End();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        protected void Delete()
        {
            string strList = RequestHelper.GetRequest("DeliveryID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable thisTab = DbHelper.Connection.FindTable("[Fooke_UserDelivery]", Params: " and UserID=" + MemberRs["UserID"] + " and DeliveryID in (" + strList + ")");
            if (thisTab == null) { this.ErrorMessage("没有需要处理的数据信息！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据信息！"); Response.End(); }
            /**************************************************************************************
             * 开始处理数据
             * ************************************************************************************/
            DbHelper.Connection.Delete(tablename: "Fooke_UserDelivery",
                Params: " and UserID=" + MemberRs["UserID"] + " and DeliveryID in (" + strList + ")");
            /**************************************************************************************
             * 返回处理结果
             * ************************************************************************************/
            this.ErrorMessage("数据处理成功！", iSuccess: true);
            Response.End();
        }
    }
}