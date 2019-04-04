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
namespace Fooke.Web.Admin
{
    public partial class Address : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "edit": this.VerificationRole("收货地址"); Update(); Response.End(); break;
                case "add": this.VerificationRole("收货地址"); Add(); Response.End(); break;
                case "save": this.VerificationRole("收货地址"); AddSave(); Response.End(); break;
                case "editsave": this.VerificationRole("收货地址"); SaveUpdate(); Response.End(); break;
                case "default": this.VerificationRole("收货地址"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "adddemo": this.VerificationRole("收货地址"); AddDemo(); Response.End(); break;
                case "stor": SelectedList(); Response.End(); break;
                case "looker": this.VerificationRole("收货地址"); strLooker(); Response.End(); break;
            }
        }
        /// <summary>
        /// 快速选择器
        /// </summary>
        protected void SelectedList()
        {
            /**************************************************************************************
             * 获取查询预设参数信息
             * ************************************************************************************/
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            /**************************************************************************************************
            * 显示网页输出内容
            * **************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"100%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"3\">");
            strText.Append("<form id=\"frmForm\" OnSubmit=\"return _doPost(this);\" action=\"user.aspx\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"stor\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="address",Text="搜地址"},
                new OptionMode(){Value="fullname",Text="搜姓名"},
                new OptionMode(){Value="strmobile",Text="搜电话"},
                new OptionMode(){Value="username",Text="搜账号"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"100\">守护ID</td>");
            strText.Append("<td width=\"100\">收货人姓名</td>");
            strText.Append("<td>收货地址</td>");
            strText.Append("</tr>");
            /*****************************************************************************************************
            * 构建分页查询语句条件
            * ***************************************************************************************************/
            string strParams = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "username": strParams += " and exists(select userid from Fooke_user where userid = fooke_userdelivery.userid and userName like '%" + Keywords + "%')"; break;
                    case "fullname": strParams += " and fullname like '%" + Keywords + "%'"; break;
                    case "address": strParams += " and address like '%" + Keywords + "%'"; break;
                    case "strmobile": strParams += " and strmobile like '%" + Keywords + "%'"; break;
                }
            }
            if (UserID != "0") { strParams += " and UserID = " + UserID + ""; }
            /*****************************************************************************************************
            * 构建分页查询语句
            * ***************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "DeliveryID,UserID,Nickname,Fullname,Address";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "DeliveryID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " DeliveryID Desc";
            PageCenterConfig.Tablename = "Fooke_UserDelivery";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserDelivery", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /**************************************************************************************************
            * 遍历网页内容
            * **************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">", JSONHelper.ToString(Rs));
                strText.AppendFormat("<td>{0}</td>", Rs["UserID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Fullname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Address"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************************************
             * 输出网页内容
             * *****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/address/stor.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                    case "pagebar": strValue = PageCenter.Often2(Record, 12); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 默认列表信息
        /// </summary>
        protected void strDefault()
        {
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            /***********************************************************************************************************
             * 构建网页内容
             * *********************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"9\">收货地址 >> 地址列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append("<form id=\"FormContianer\" action=\"?\" OnSubmit=\"return _doPost(this)\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="address",Text="搜地址"},
                new OptionMode(){Value="fullname",Text="搜姓名"},
                new OptionMode(){Value="strmobile",Text="搜电话"},
                new OptionMode(){Value="username",Text="搜账号"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">用户ID</td>");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td>收货地址</td>");
            strText.Append("<td width=\"200\">联系电话</td>");
            strText.Append("<td width=\"200\">收货人姓名</td>");
            strText.Append("<td width=\"60\">默认地址</td>");
            strText.Append("<td width=\"120\">添加日期</td>");
            strText.Append("<td width=\"120\">操作选项</td>");
            strText.Append("</tr>");
            /*****************************************************************************************************
             * 构建分页查询语句条件
             * ***************************************************************************************************/
            string strParams = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "username": strParams += " and exists(select userid from Fooke_user where userid = fooke_userdelivery.userid and userName like '%" + Keywords + "%')"; break;
                    case "fullname": strParams += " and fullname like '%" + Keywords + "%'"; break;
                    case "address": strParams += " and address like '%" + Keywords + "%'"; break;
                    case "strmobile": strParams += " and strmobile like '%" + Keywords + "%'"; break;
                }
            }
            if (UserID != "0") { strParams += " and UserID = " + UserID + ""; }
            /*****************************************************************************************************
            * 构建分页查询语句
            * ***************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "DeliveryID,DeliveryKey,UserID,Nickname,Fullname,Address,strMobile,Addtime,isDefault";
            PageCenterConfig.Params = strParams;
            PageCenterConfig.Identify = "DeliveryID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " DeliveryID Desc";
            PageCenterConfig.Tablename = "Fooke_UserDelivery";
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record("Fooke_UserDelivery", strParams);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
            * 循环遍历网页内容
            * ***************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"DeliveryID\" value=\"{0}\" /></td>", Rs["DeliveryID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["UserID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Address"]);
                strText.AppendFormat("<td>{0}</td>", Rs["strMobile"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Fullname"]);
                strText.AppendFormat("<td>{0}</td>", (Rs["isDefault"].ToString() == "1" ? "<a class=\"vbtn\">是</a>" : "<a class=\"vbtnRed\">否</a>"));
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"].ToString().cDate().ToString("yyyy-MM-dd HH:mm"));
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=looker&DeliveryID={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["DeliveryID"]);
                strText.AppendFormat("<a operate=\"delete\" href=\"?action=del&DeliveryID={0}\" title=\"删除\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["DeliveryID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append(PageCenter.Often(Record, 10));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append(" <input type=\"button\" class=\"button\" value=\"删除数据\" onclick=\"deleteOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /*****************************************************************************************************
            * 输出网页内容
            * ***************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/address/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "list": strValue = strText.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 添加
        /// </summary>
        protected void Add()
        {
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/address/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName) { }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 添加详情
        /// </summary>
        protected void AddDemo()
        {
            /**************************************************************************************
             * 获取请求参数信息是否合法
             * ************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { Response.Write("请选择一个用户！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { Response.Write("获取用户信息失败,请刷新重试！"); Response.End(); }
            /**************************************************************************************
             * 构建显示输出网页数据
             * ************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/address/adddemo.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isDefault": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isDefault",Value="1",Text="默认地址(是)"},
                        new RadioMode(){Name="isDefault",Value="0",Text="默认地址(否)"}
                    }, "0"); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { }; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 编辑查看
        /// </summary>
        protected void strLooker()
        {
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/address/looker.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName) { }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 编辑数据
        /// </summary>
        protected void Update()
        {
            /*******************************************************************************************
             * 开始查询请求数据信息
             * ******************************************************************************************/
            string DeliveryID = RequestHelper.GetRequest("DeliveryID").toInt();
            if (DeliveryID == "0") { Response.Write("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindUserDelivery]", new Dictionary<string, object>() {
                {"DeliveryID",DeliveryID}
            });
            if (cRs == null) { Response.Write("获取请求数据失败,请重试!"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/address/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isDefault": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isDefault",Value="1",Text="默认地址(是)"},
                        new RadioMode(){Name="isDefault",Value="0",Text="默认地址(否)"}
                    }, cRs["isDefault"].ToString()); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 保存添加
        /// </summary>
        protected void AddSave()
        {
            /*************************************************************************************************
             * 获取用户信息
             * ***********************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("请求参数错误,请选择用户！"); Response.End(); }
            DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindMember]", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (MemberRs == null) { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
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
            this.ErrorMessage("数据保存成功!", iSuccess: true);
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
            this.ErrorMessage("数据保存成功!", iSuccess: true);
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
            DataTable thisTab = DbHelper.Connection.FindTable("[Fooke_UserDelivery]", Params: " and DeliveryID in (" + strList + ")");
            if (thisTab == null) { this.ErrorMessage("没有需要处理的数据信息！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据信息！"); Response.End(); }
            /**************************************************************************************
             * 开始处理数据
             * ************************************************************************************/
            DbHelper.Connection.Delete(tablename: "Fooke_UserDelivery",
                Params: " and DeliveryID in (" + strList + ")");
            /**************************************************************************************
             * 返回处理结果
             * ************************************************************************************/
            this.History();
            Response.End();
        }
    }
}