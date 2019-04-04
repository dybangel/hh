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
    public partial class Authentication : Fooke.Code.APIHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "save": SaveAuthentication(); Response.End(); break;
                case "editsave": SaveUpdate(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        protected void strDefault()
        {
            /*******************************************************************************************
             * 获取用户绑定的证件信息
             * ****************************************************************************************/
            DataRow AutRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAuthentication]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            /*******************************************************************************************
             * 判断用户是否发布了认证信息
             * ****************************************************************************************/
            if (AutRs == null)
            {
                StringBuilder strError = new StringBuilder();
                strError.Append("{");
                strError.Append("\"success\":\"false\"");
                strError.Append(",\"tips\":\"nobind\"");
                strError.Append(",\"userid\":\"0\"");
                strError.Append(",\"isauthentication\":\"0\"");
                strError.Append("}");
                Response.Write(strError.ToString());
                Response.End();
            }
            /**************************************************************************************
             * 输出数据处理结果
             * ************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"success\"");
            strBuilder.Append(",\"userid\":\"" + AutRs["UserID"] + "\"");
            strBuilder.Append(",\"nickname\":\"" + AutRs["Nickname"] + "\"");
            strBuilder.Append(",\"idcategory\":\"" + AutRs["IDCategory"] + "\"");
            strBuilder.Append(",\"idnumber\":\"" + AutRs["IDnumber"] + "\"");
            strBuilder.Append(",\"fullname\":\"" + AutRs["Fullname"] + "\"");
            strBuilder.Append(",\"facethumb\":\"" + AutRs["FaceThumb"] + "\"");
            strBuilder.Append(",\"sidethumb\":\"" + AutRs["SideThumb"] + "\"");
            strBuilder.Append(",\"isauthentication\":\"" + AutRs["isAuthentication"] + "\"");
            strBuilder.Append(",\"remark\":\"" + AutRs["Remark"] + "\"");
            strBuilder.Append(",\"lastdate\":\"" + new Fooke.Function.String(AutRs["LastDate"].ToString()).cDate().ToString("yyyy-MM-dd HH:mm:ss") + "\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /********************************************************************************************
         * 数据处理区域
         * ******************************************************************************************/
        /// <summary>
        /// 保存我上传的认证信息
        /// </summary>
        protected void SaveAuthentication()
        {
            /*****************************************************************************************************
             * 验证用户证件编号信息
             * ***************************************************************************************************/
            string IDCategory = RequestHelper.GetRequest("IDCategory").toString("身份证");
            if (string.IsNullOrEmpty(IDCategory)) { this.ErrorMessage("请选择证件类型！"); Response.End(); }
            else if (IDCategory.Length <= 1) { this.ErrorMessage("选择证件类型格式错误,请重试！"); Response.End(); }
            else if (IDCategory.Length >= 10) { this.ErrorMessage("选择证件类型格式错误,请重试！"); Response.End(); }
            string IDnumber = RequestHelper.GetRequest("IDnumber").ToString();
            if (string.IsNullOrEmpty(IDnumber)) { this.ErrorMessage("请填写证件编号信息！"); Response.End(); }
            else if (IDnumber.Length <= 10) { this.ErrorMessage("证件编号信息字段长度格式错误,请重试！"); Response.End(); }
            else if (IDnumber.Length >= 24) { this.ErrorMessage("证件编号信息字段长度格式错误,请重试！"); Response.End(); }
            else if (IDCategory == "身份证" && IDnumber.Length != 18) { this.ErrorMessage("证件编号信息字段长度格式错误,请重试！"); Response.End(); }
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAuthentication]", new Dictionary<string, object>() {
                {"IDCategory",IDCategory},
                {"IDnumber",IDnumber}
            });
            if (sRs != null && sRs["UserID"].ToString() != MemberRs["UserID"].ToString())
            { this.ErrorMessage("证件信息已被注册！"); Response.End(); }
            /******************************************************************************************************
             * 验证用户真实姓名信息
             * ****************************************************************************************************/
            string Fullname = RequestHelper.GetRequest("Fullname").ToString();
            if (string.IsNullOrEmpty(Fullname)) { this.ErrorMessage("请填写您的真实姓名！"); Response.End(); }
            else if (Fullname.Length <= 1) { this.ErrorMessage("真实姓名不能少于1个字符！"); Response.End(); }
            else if (Fullname.Length >= 16) { this.ErrorMessage("真实姓名不能大于16个字符！"); Response.End(); }
            /******************************************************************************************************
             * 验证用户上传图片信息
             * ****************************************************************************************************/
            if (Request.Files == null) { this.ErrorMessage("请上传证件照片信息！"); Response.End(); }
            else if (Request.Files.Count <= 0) { this.ErrorMessage("请上传证件照片信息！"); Response.End(); }
            else if (Request.Files["FaceThumb"] == null) { this.ErrorMessage("请上传证件正面照！"); Response.End(); }
            else if (Request.Files["SideThumb"] == null) { this.ErrorMessage("请上传证件手持照！"); Response.End(); }
            /******************************************************************************************************
             * 获取用户证件正面信息
             * ****************************************************************************************************/
            string FaceThumb = new PostedHelper().SaveTo(Posted: Request.Files["FaceThumb"],
                  directory: "~/file/certificate",
                  fileSize: 4,
                  fileExc: "jpg|png|bmp|jpeg|xmp|gif|JPEG|JPG|png+xmp",
                  Err: (err) =>
                  {
                      this.ErrorMessage(err); Response.End();
                  });
            if (string.IsNullOrEmpty(FaceThumb)) { this.ErrorMessage("获取证件正面照信息失败,请重试！"); Response.End(); }
            else if (FaceThumb.Length <= 30) { this.ErrorMessage("获取证件正面照信息失败,请重试！"); Response.End(); }
            else if (FaceThumb.Length >= 120) { this.ErrorMessage("获取证件正面照信息失败,请重试！"); Response.End(); }
            /******************************************************************************************************
             * 获取用户证件反面信息
             * ****************************************************************************************************/
            string SideThumb = new PostedHelper().SaveTo(Posted: Request.Files["SideThumb"],
                  directory: "~/file/certificate",
                  fileSize: 4,
                  fileExc: "jpg|png|bmp|jpeg|xmp|gif|JPEG|JPG|png+xmp",
                  Err: (err) =>
                  {
                      this.ErrorMessage(err); Response.End();
                  });
            if (string.IsNullOrEmpty(SideThumb)) { this.ErrorMessage("获取证件反面照信息失败,请重试！"); Response.End(); }
            else if (SideThumb.Length <= 30) { this.ErrorMessage("获取证件反面照信息失败,请重试！"); Response.End(); }
            else if (SideThumb.Length >= 120) { this.ErrorMessage("获取证件反面照信息失败,请重试！"); Response.End(); }
            /*****************************************************************************************************
             * 开始保存数据
             * ***************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["IDCategory"] = IDCategory;
            thisDictionary["IDnumber"] = IDnumber;
            thisDictionary["Fullname"] = Fullname;
            thisDictionary["FaceThumb"] = FaceThumb;
            thisDictionary["SideThumb"] = SideThumb;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserAuthentication]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试!"); Response.End(); }
            /*****************************************************************************************************
             * 输出数据处理结果
             * ***************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"您的证件资料已上传\"");
            strBuilder.Append(",\"type\":\"alert\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }

        /// <summary>
        /// 保存我要修改的认证信息
        /// </summary>
        protected void SaveUpdate()
        {
            /*****************************************************************************************************
            * 验证我的认证记录是否存在
            * ***************************************************************************************************/
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAuthentication]", new Dictionary<string, object>() {
                {"UserID",MemberRs["UserID"].ToString()}
            });
            if (oRs == null) { this.ErrorMessage("获取认证记录信息失败,请重试！"); Response.End(); }
            /*****************************************************************************************************
             * 验证用户证件编号信息
             * ***************************************************************************************************/
            string IDCategory = RequestHelper.GetRequest("IDCategory").ToString();
            if (string.IsNullOrEmpty(IDCategory)) { this.ErrorMessage("请选择证件类型！"); Response.End(); }
            else if (IDCategory.Length <= 1) { this.ErrorMessage("选择证件类型格式错误,请重试！"); Response.End(); }
            else if (IDCategory.Length >= 10) { this.ErrorMessage("选择证件类型格式错误,请重试！"); Response.End(); }
            string IDnumber = RequestHelper.GetRequest("IDnumber").ToString();
            if (string.IsNullOrEmpty(IDnumber)) { this.ErrorMessage("请填写证件编号信息！"); Response.End(); }
            else if (IDnumber.Length <= 10) { this.ErrorMessage("证件编号信息字段长度格式错误,请重试！"); Response.End(); }
            else if (IDnumber.Length >= 24) { this.ErrorMessage("证件编号信息字段长度格式错误,请重试！"); Response.End(); }
            else if (IDCategory == "身份证" && IDnumber.Length != 18) { this.ErrorMessage("证件编号信息字段长度格式错误,请重试！"); Response.End(); }
            DataRow sRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserAuthentication]", new Dictionary<string, object>() {
                {"IDCategory",IDCategory},
                {"IDnumber",IDnumber}
            });
            if (sRs != null && sRs["UserID"].ToString() != MemberRs["UserID"].ToString())
            { this.ErrorMessage("证件信息已被注册！"); Response.End(); }
            /******************************************************************************************************
             * 验证用户真实姓名信息
             * ****************************************************************************************************/
            string Fullname = RequestHelper.GetRequest("Fullname").ToString();
            if (string.IsNullOrEmpty(Fullname)) { this.ErrorMessage("请填写您的真实姓名！"); Response.End(); }
            else if (Fullname.Length <= 1) { this.ErrorMessage("真实姓名不能少于1个字符！"); Response.End(); }
            else if (Fullname.Length >= 16) { this.ErrorMessage("真实姓名不能大于16个字符！"); Response.End(); }
            /******************************************************************************************************
             * 获取用户证件正面信息
             * ****************************************************************************************************/
            string FaceThumb = oRs["FaceThumb"].ToString();
            if (Request.Files != null && Request.Files.Count != 0 && Request.Files["FaceThumb"] != null)
            {
                FaceThumb = new PostedHelper().SaveTo(Posted: Request.Files["FaceThumb"],
                  directory: "~/file/certificate",
                  fileSize: 4,
                  fileExc: "jpg|png|bmp|jpeg|xmp|gif|JPEG|JPG|png+xmp",
                  Err: (err) =>
                  {
                      this.ErrorMessage(err); Response.End();
                  });
            }
            if (string.IsNullOrEmpty(FaceThumb)) { this.ErrorMessage("获取证件正面照信息失败,请重试！"); Response.End(); }
            else if (FaceThumb.Length <= 30) { this.ErrorMessage("获取证件正面照信息失败,请重试！"); Response.End(); }
            else if (FaceThumb.Length >= 120) { this.ErrorMessage("获取证件正面照信息失败,请重试！"); Response.End(); }
            /******************************************************************************************************
             * 获取用户证件反面信息
             * ****************************************************************************************************/
            string SideThumb = oRs["FaceThumb"].ToString();
            if (Request.Files != null && Request.Files.Count != 0 && Request.Files["SideThumb"] != null)
            {
                SideThumb = new PostedHelper().SaveTo(Posted: Request.Files["SideThumb"],
                  directory: "~/file/certificate",
                  fileSize: 4,
                  fileExc: "jpg|png|bmp|jpeg|xmp|gif|JPEG|JPG|png+xmp",
                  Err: (err) =>
                  {
                      this.ErrorMessage(err); Response.End();
                  });
            }
            if (string.IsNullOrEmpty(SideThumb)) { this.ErrorMessage("获取证件反面照信息失败,请重试！"); Response.End(); }
            else if (SideThumb.Length <= 30) { this.ErrorMessage("获取证件反面照信息失败,请重试！"); Response.End(); }
            else if (SideThumb.Length >= 120) { this.ErrorMessage("获取证件反面照信息失败,请重试！"); Response.End(); }
            /*****************************************************************************************************
             * 开始保存数据
             * ***************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["IDCategory"] = IDCategory;
            thisDictionary["IDnumber"] = IDnumber;
            thisDictionary["Fullname"] = Fullname;
            thisDictionary["FaceThumb"] = FaceThumb;
            thisDictionary["SideThumb"] = SideThumb;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveUserAuthentication]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试!"); Response.End(); }
            /*****************************************************************************************************
             * 输出数据处理结果
             * ***************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("{");
            strBuilder.Append("\"success\":\"true\"");
            strBuilder.Append(",\"tips\":\"您的证件资料已上传\"");
            strBuilder.Append(",\"type\":\"alert\"");
            strBuilder.Append("}");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
    }
}