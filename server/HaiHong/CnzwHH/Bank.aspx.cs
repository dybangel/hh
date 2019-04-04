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
    public partial class Bank : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "edit": this.VerificationRole("添加银行卡"); Update(); Response.End(); break;
                case "add": this.VerificationRole("添加银行卡"); Add(); Response.End(); break;
                case "save": this.VerificationRole("添加银行卡"); AddSave(); Response.End(); break;
                case "editsave": this.VerificationRole("添加银行卡"); SaveUpdate(); Response.End(); break;
                case "default": this.VerificationRole("用户绑卡"); strDefault(); Response.End(); break;
                case "del": this.VerificationRole("超级管理员权限"); Delete(); Response.End(); break;
                case "adddemo": this.VerificationRole("用户绑卡"); AddDemo(); Response.End(); break;
                case "display": this.VerificationRole("用户绑卡"); SaveDisplay(); Response.End(); break;
                case "stor": SelectedList(); Response.End(); break;
                case "looker": this.VerificationRole("用户绑卡"); strLooker(); Response.End(); break;
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
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
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
                new OptionMode(){Value="account",Text="搜银行卡号"},
                new OptionMode(){Value="bankname",Text="搜银行名称"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"},
                new OptionMode(){Value="username",Text="搜用户账号"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"100\">银行名称</td>");
            strText.Append("<td>银行卡号</td>");
            strText.Append("</tr>");
            /*****************************************************************************************************
            * 构建分页查询语句条件
            * ***************************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "username": strParameter += " and exists(select userid from Fooke_user where userName like '%" + Keywords + "%' and userid = fooke_bank.userid)"; break;
                    case "nickname": strParameter += " and nickname like '%" + Keywords + "%'"; break;
                    case "account": strParameter += " and bankaccount like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParameter += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParameter += " and Addtime<='" + EndDate + "'"; }
            string bankname = RequestHelper.GetRequest("modelId").ToString();
            if (!string.IsNullOrEmpty(bankname)) { strParameter += " and bankname = '" + bankname + "'"; }
            /*****************************************************************************************************
            * 构建分页查询语句
            * ***************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "BankID,UserID,Nickname,Bankname,BankAccount";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "UserID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " UserID Desc";
            PageCenterConfig.Tablename = TableCenter.Bank;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Bank, strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /**************************************************************************************************
            * 遍历网页内容
            * **************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr operate=\"selector\" json=\'{0}\' class=\"hback\">", JSONHelper.ToString(Rs));
                strText.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Bankname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Bankaccount"]);
                strText.AppendFormat("</tr>");
            }
            strText.Append("</table>");
            strText.Append("</form>");
            /*******************************************************************************************************
             * 输出网页内容
             * *****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/bank/stor.html");
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
        /// 获取我的银行卡列表信息
        /// </summary>
        protected void strDefault()
        {
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string StarDate = RequestHelper.GetRequest("StarDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            /***********************************************************************************************************
             * 构建网页内容
             * *********************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"9\">银行卡管理 >> 银行卡列表</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"search\">");
            strText.Append("<td colspan=\"9\">");
            strText.Append("<form action=\"?action=default\" method=\"get\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="account",Text="搜银行卡号"},
                new OptionMode(){Value="bankname",Text="搜银行名称"},
                new OptionMode(){Value="nickname",Text="搜用户昵称"},
                new OptionMode(){Value="username",Text="搜用户账号"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" placeholder=\"请填写搜索关键词\" value=\"" + Keywords + "\" name=\"Keywords\" class=\"inputtext\" />");
            strText.Append(" 查询日期 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + StarDate + "\" name=\"StarDate\" class=\"inputtext\" />");
            strText.Append(" 到 <input placeholder=\"请选择开始日期\" size=\"12\" onClick=\"WdatePicker()\" type=\"text\" value=\"" + EndDate + "\" name=\"EndDate\" class=\"inputtext\" />");
            strText.Append(" <input type=\"submit\" value=\"快速查找\" class=\"button\" />");
            strText.Append("</form>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"12\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"100\">用户昵称</td>");
            strText.Append("<td width=\"200\">银行名称</td>");
            strText.Append("<td width=\"200\">银行卡号</td>");
            strText.Append("<td width=\"200\">持卡人姓名</td>");
            strText.Append("<td width=\"200\">开户行</td>");
            strText.Append("<td width=\"40\">状态</td>");
            strText.Append("<td width=\"120\">添加日期</td>");
            strText.Append("<td>操作选项</td>");
            strText.Append("</tr>");
           /*****************************************************************************************************
            * 构建分页查询语句条件
            * ***************************************************************************************************/
            string strParameter = "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType)
                {
                    case "username": strParameter += " and exists(select userid from Fooke_user where userName like '%" + Keywords + "%' and userid = fooke_bank.userid)"; break;
                    case "nickname": strParameter += " and nickname like '%" + Keywords + "%'"; break;
                    case "account": strParameter += " and bankaccount like '%" + Keywords + "%'"; break;
                }
            }
            if (!string.IsNullOrEmpty(StarDate) && VerifyCenter.VerifyDateTime(StarDate)) { strParameter += " and Addtime>='" + StarDate + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { strParameter += " and Addtime<='" + EndDate + "'"; }
            string bankname = RequestHelper.GetRequest("modelId").ToString();
            if (!string.IsNullOrEmpty(bankname)) { strParameter += " and bankname = '" + bankname + "'"; }
            /*****************************************************************************************************
            * 构建分页查询语句
            * ***************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "BankID,UserID,Nickname,Bankname,BankAccount,Holdername,Branch,Addtime,isDisplay";
            PageCenterConfig.Params = strParameter;
            PageCenterConfig.Identify = "BankID";
            PageCenterConfig.PageSize = 10;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " BankID Desc";
            PageCenterConfig.Tablename = TableCenter.Bank;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Bank, strParameter);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*****************************************************************************************************
            * 循环遍历网页内容
            * ***************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"BankID\" value=\"{0}\" /></td>", Rs["BankID"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Nickname"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Bankname"]);
                strText.AppendFormat("<td>{0}</td>",Rs["BankAccount"]);
                strText.AppendFormat("<td>{0}</td>",Rs["Holdername"]);
                strText.AppendFormat("<td>{0}</td>", Rs["Branch"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1")
                { strText.AppendFormat("<a href=\"?action=display&val=0&bankid={0}\"><img src=\"images/ico/yes.gif\"/></a>", Rs["bankid"]); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&bankid={0}\"><img src=\"images/ico/no.gif\"/></a>", Rs["bankid"]); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"].ToString().cDate().ToString("yyyy-MM-dd HH:mm"));
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=looker&bankid={0}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["bankid"]);
                strText.AppendFormat("<a operate=\"delete\" href=\"?action=del&bankid={0}\" title=\"删除\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["bankid"]);
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
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"正常显示(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"正常显示(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("</table>");
            /*****************************************************************************************************
            * 输出网页内容
            * ***************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/bank/default.html");
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
        /// 添加银行卡帐号
        /// </summary>
        protected void Add()
        {
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/bank/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {

                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 添加银行卡帐号
        /// </summary>
        protected void AddDemo()
        {
            /**************************************************************************************
             * 获取请求参数信息是否合法
             * ************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { Response.Write("请选择一个用户！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindMember", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (cRs == null) { Response.Write("获取用户信息失败,请刷新重试！"); Response.End(); }
            /**************************************************************************************
             * 构建显示输出网页数据
             * ************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/bank/adddemo.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isDisplay",Value="1",Text="通过审核(是)"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="通过审核(否)"}
                    }, "1"); break;
                    case "bankModel": strValue = new BankModelHelper().Options(defaultText: "",
                        text: "modelname",
                        value: "modelname");
                        break;
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
            string strResponse = Master.Reader("template/bank/looker.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {

                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 编辑银行卡帐号
        /// </summary>
        protected void Update()
        {
            /*******************************************************************************************
             * 开始查询请求数据信息
             * ******************************************************************************************/
            string BankID = RequestHelper.GetRequest("BankID").toInt();
            if (BankID == "0") { Response.Write("请求参数错误,请刷新网页重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("Stored_FindBank", new Dictionary<string, object>() {
                {"BankID",BankID}
            });
            if (cRs == null) { Response.Write("获取请求数据失败,请重试!"); Response.End(); }
            /********************************************************************
             * 显示输出数据
             * ******************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/bank/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isDisplay",Value="1",Text="通过审核(是)"},
                        new RadioMode(){Name="isDisplay",Value="0",Text="通过审核(否)"}
                    }, cRs["isDisplay"].ToString()); break;
                    case "bankModel": strValue = new BankModelHelper().Options(defaultText: cRs["NetMode"].ToString(),
                        text: "modelname",
                        value: "modelname");
                        break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        protected void AddSave()
        {
            /*************************************************************************************************
             * 获取用户信息
             * ***********************************************************************************************/
            string UserID = RequestHelper.GetRequest("UserID").toInt();
            if (UserID == "0") { this.ErrorMessage("请求参数错误,请选择用户！"); Response.End(); }
            DataRow MemberRs = DbHelper.Connection.ExecuteFindRow("Stored_FindMember", new Dictionary<string, object>() {
                {"UserID",UserID}
            });
            if (MemberRs == null) { this.ErrorMessage("请求参数错误,你查找的数据不存在！"); Response.End(); }
            /****************************************************************************************************************
             * 获取所属银行名称并验证
             * **************************************************************************************************************/
            string Bankname = RequestHelper.GetRequest("Bankname").ToString();
            if (string.IsNullOrEmpty(Bankname)) { this.ErrorMessage("请选择要绑卡的银行！"); Response.End(); }
            else if (Bankname.Length <= 1) { this.ErrorMessage("获取银行名称信息失败,请重试！"); Response.End(); }
            else if (Bankname.Length >= 20) { this.ErrorMessage("银行名称字段长度请限制在20个字符内！"); Response.End(); }
            /****************************************************************************************************************
             * 获取用户开户行名称并验证
             * **************************************************************************************************************/
            string Branch = RequestHelper.GetRequest("Branch").ToString();
            if (string.IsNullOrEmpty(Branch)) { this.ErrorMessage("开户行名称不能为空！"); Response.End(); }
            else if (Branch.Length <= 4) { this.ErrorMessage("开户行名称不能少于4个字符！"); Response.End(); }
            else if (Branch.Length >= 30) { this.ErrorMessage("开户行名称长度请限制在30个汉字内！"); Response.End(); }
            /****************************************************************************************************************
             * 获取银行卡账号并验证
             * **************************************************************************************************************/
            string BankAccount = RequestHelper.GetRequest("BankAccount").ToString();
            if (string.IsNullOrEmpty(BankAccount)) { this.ErrorMessage("银行卡账号不能为空!"); Response.End(); }
            else if (BankAccount.Length <= 10) { this.ErrorMessage("银行卡账号不能少于10个字符！"); Response.End(); }
            else if (BankAccount.Length >= 30) { this.ErrorMessage("银行卡账号不能大于30个字符！"); Response.End(); }
            /****************************************************************************************************************
             * 验证持卡人姓名
             * **************************************************************************************************************/
            string Holdername = RequestHelper.GetRequest("Holdername").ToString();
            if (string.IsNullOrEmpty(Holdername)) { this.ErrorMessage("请填写持卡人姓名！"); Response.End(); }
            else if (Holdername.Length <= 1) { this.ErrorMessage("持卡人姓名字段长度不能少于1个汉字！"); Response.End(); }
            else if (Holdername.Length >= 12) { this.ErrorMessage("持卡人姓名字段长度请限制在12个汉字内！"); Response.End(); }
            /****************************************************************************************************************
             * 判断银行卡是否已被绑定
             * **************************************************************************************************************/
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBank]", new Dictionary<string, object>() {
                {"Bankname",Bankname},
                {"BankAccount",BankAccount}
            });
            if (oRs != null) { this.ErrorMessage("你已经绑定了相同的银行卡,不需要重复绑定！"); Response.End(); }
            /****************************************************************************************************************
             * 开始保存用户绑定银行卡信息
             * **************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["BankID"] = "0";
            thisDictionary["UserID"] = MemberRs["UserID"].ToString();
            thisDictionary["Nickname"] = MemberRs["Nickname"].ToString();
            thisDictionary["Bankname"] = Bankname;
            thisDictionary["BankAccount"] = BankAccount;
            thisDictionary["Holdername"] = Holdername;
            thisDictionary["Branch"] = Branch;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveBank]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            this.ErrorMessage("数据保存成功!", iSuccess: true);
            Response.End();
        }
       

        /// <summary>
        /// 添加银行卡
        /// </summary>
        protected void SaveUpdate()
        {
            /****************************************************************************************************************
             * 验证请求数据的合法性
             * **************************************************************************************************************/
            string BankID = RequestHelper.GetRequest("BankID").toInt();
            if (BankID == "0") { this.ErrorMessage("获取请求参数失败,请重试!"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBank]", new Dictionary<string, object>() {
                {"BankID",BankID}
            });
            if (cRs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            /****************************************************************************************************************
             * 获取所属银行名称并验证
             * **************************************************************************************************************/
            string Bankname = RequestHelper.GetRequest("Bankname").ToString();
            if (string.IsNullOrEmpty(Bankname)) { this.ErrorMessage("请选择要绑卡的银行！"); Response.End(); }
            else if (Bankname.Length <= 1) { this.ErrorMessage("获取银行名称信息失败,请重试！"); Response.End(); }
            else if (Bankname.Length >= 20) { this.ErrorMessage("银行名称字段长度请限制在20个字符内！"); Response.End(); }
            /****************************************************************************************************************
             * 获取用户开户行名称并验证
             * **************************************************************************************************************/
            string Branch = RequestHelper.GetRequest("Branch").ToString();
            if (string.IsNullOrEmpty(Branch)) { this.ErrorMessage("开户行名称不能为空！"); Response.End(); }
            else if (Branch.Length <= 4) { this.ErrorMessage("开户行名称不能少于4个字符！"); Response.End(); }
            else if (Branch.Length >= 30) { this.ErrorMessage("开户行名称长度请限制在30个汉字内！"); Response.End(); }
            /****************************************************************************************************************
             * 获取银行卡账号并验证
             * **************************************************************************************************************/
            string BankAccount = RequestHelper.GetRequest("BankAccount").ToString();
            if (string.IsNullOrEmpty(BankAccount)) { this.ErrorMessage("银行卡账号不能为空!"); Response.End(); }
            else if (BankAccount.Length <= 10) { this.ErrorMessage("银行卡账号不能少于10个字符！"); Response.End(); }
            else if (BankAccount.Length >= 30) { this.ErrorMessage("银行卡账号不能大于30个字符！"); Response.End(); }
            /****************************************************************************************************************
             * 验证持卡人姓名
             * **************************************************************************************************************/
            string Holdername = RequestHelper.GetRequest("Holdername").ToString();
            if (string.IsNullOrEmpty(Holdername)) { this.ErrorMessage("请填写持卡人姓名！"); Response.End(); }
            else if (Holdername.Length <=1) { this.ErrorMessage("持卡人姓名字段长度不能少于1个汉字！"); Response.End(); }
            else if (Holdername.Length >= 12) { this.ErrorMessage("持卡人姓名字段长度请限制在12个汉字内！"); Response.End(); }
            /****************************************************************************************************************
             * 判断银行卡是否已被绑定
             * **************************************************************************************************************/
            DataRow oRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindBank]", new Dictionary<string, object>() {
                {"Bankname",Bankname},
                {"BankAccount",BankAccount}
            });
            if (oRs != null && oRs["BankID"].ToString() != cRs["BankID"].ToString()) { this.ErrorMessage("你已经绑定了相同的银行卡,不需要重复绑定！"); Response.End(); }
            /****************************************************************************************************************
             * 开始保存用户绑定银行卡信息
             * **************************************************************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["BankID"] = cRs["BankID"].ToString();
            thisDictionary["UserID"] = cRs["UserID"].ToString();
            thisDictionary["Nickname"] = cRs["Nickname"].ToString();
            thisDictionary["Bankname"] = Bankname;
            thisDictionary["BankAccount"] = BankAccount;
            thisDictionary["Holdername"] = Holdername;
            thisDictionary["Branch"] = Branch;
            DataRow thisRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveBank]", thisDictionary);
            if (thisRs == null) { this.ErrorMessage("数据保存过程中发生未知错误,请重试！"); Response.End(); }
            /****************************************************************************************************************
            * 输出数据处理结果
            * **************************************************************************************************************/
            this.ErrorMessage("数据保存成功!", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 删除银行卡信息
        /// </summary>
        protected void Delete()
        {
            string strList = RequestHelper.GetRequest("BankID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /**************************************************************************************
             * 开始处理数据
             * ************************************************************************************/
            DbHelper.Connection.Delete(tablename: "Fooke_Bank",
                Params: " and BankID in (" + strList + ")");
            /**************************************************************************************
             * 返回处理结果
             * ************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 保存银行卡审核状态信息
        /// </summary>
        protected void SaveDisplay()
        {
            /***********************************************************************************************
             * 验证参数合法性
             * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("BankID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            /***********************************************************************************************
             * 验证请求参数值信息
             * *********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            if (strValue != "0" && strValue != "1") { this.ErrorMessage("请求参数错误,请重试！"); Response.End(); }
            /**************************************************************************************
            * 保存数据
            * ************************************************************************************/
            DbHelper.Connection.Update("Fooke_Bank", dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and BankID in (" + strList + ")");
            /**************************************************************************************
             * 返回处理结果
             * ************************************************************************************/
            this.History();
            Response.End();
        }
    }
}