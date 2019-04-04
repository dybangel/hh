using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using Fooke.Function;
using Fooke.Code;
namespace Fooke.Web.Admin
{
    public partial class Voteing : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e){

            switch (this.strRequest)
            {
                case "save": this.VerificationRole("投票系统"); this.AddSave(); Response.End(); ; break;
                case "add": this.VerificationRole("投票系统"); this.Add(); Response.End(); break;
                case "del": this.VerificationRole("投票系统"); this.Delete(); Response.End(); break;
                case "edit": this.VerificationRole("投票系统"); this.Update(); Response.End(); break;
                case "editsave": this.VerificationRole("投票系统"); this.SaveUpdate(); Response.End(); break;
                case "display": this.VerificationRole("投票系统"); this.Display(); Response.End(); break;
                case "code": this.VerificationRole("投票系统"); this.CodeList(); Response.End(); break;
                case "define": this.VerificationRole("投票系统"); DefineOptions(); Response.End(); break;
                default: this.VerificationRole("投票系统"); this.strDefault(); Response.End(); break;
            }
        }

        /// <summary>
        /// 管理员列表
        /// </summary>
        protected void strDefault()
        {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">投票系统 >> 投票项目</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td>项目名称</td>");
            strText.Append("<td width=\"15%\">总票数</td>");
            strText.Append("<td width=\"15%\">选项模式</td>");
            strText.Append("<td width=\"5%\">是否使用</td>");
            strText.Append("<td width=\"180\">操作选项</td>");
            strText.Append("</tr>");
            string Params = "";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "id,voteName,ischoose,total,isExpire,isDisplay,MaxOptions,StarDate,EndDate,Descrption";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Id Desc";
            PageCenterConfig.Tablename = TableCenter.Vote;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Vote, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"Id\" value=\"" + Rs["Id"] + "\" /></td>");
                strText.Append("<td>" + Rs["voteName"] + "</td>");
                strText.Append("<td style=\"color:#f00\">" + Rs["total"] + "</td>");
                strText.Append("<td>");
                if (Rs["isChoose"].ToString() == "1") { strText.Append("<a class=\"vbtn\">多选</a>"); }
                else { strText.Append("<a class=\"vbtnRed\">单选</a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.Append("<a href=\"?action=display&val=0&Id=" + Rs["Id"] + "\"><img src=\"images/ico/yes.gif\"/></a>"); }
                else { strText.Append("<a href=\"?action=display&val=1&Id=" + Rs["Id"] + "\"><img src=\"images/ico/no.gif\"/></a>"); }
                strText.Append("</td>");
                strText.Append("<td>");
                strText.Append("<a href=\"?action=define&Id=" + Rs["Id"] + "\" title=\"查看投票明细\"><img src=\"images/ico/chart.png\" /></a>");
                strText.Append("<a href=\"?action=edit&Id=" + Rs["Id"] + "\" title=\"编辑投票项目\"><img src=\"images/ico/edit.png\" /></a>");
                strText.Append("<a href=\"?action=del&Id=" + Rs["Id"] + "\" title=\"删除项目\" operate=\"delete\"><img src=\"images/ico/delete.png\" /></a>");
                strText.Append("</td>");
                strText.Append("</tr>");
            }
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"6\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"允许使用\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"禁止使用\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/voteing/default.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
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

        protected void CodeList() {
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"3\">投票系统 >> 代码调用</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td width=\"48%\">投票项目</td>");
            strText.Append("<td>调用代码(点击直接复制代码)</td>");
            strText.Append("</tr>");
            string Params = "";
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "id,voteName,ischoose,total,isExpire,isDisplay,MaxOptions,StarDate,EndDate,Descrption";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "Id";
            PageCenterConfig.PageSize = 12;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " Id Desc";
            PageCenterConfig.Tablename = TableCenter.Vote;
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(TableCenter.Forms, Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td><input type=\"checkbox\" name=\"Id\" value=\"" + Rs["Id"] + "\" /></td>");
                strText.Append("<td>" + Rs["voteName"] + "</td>");
                strText.Append("<td>&lt;script src=\"" + Win.ApplicationPath + "/Items/Voteing.aspx?action=js&Id=" + Rs["Id"] + "\" language=\"javascript\">&lt;/script></td>");
                strText.Append("</tr>");
            }
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"xingmu\" colspan=\"3\">");
            strText.Append(PageCenter.Often(Record, 12));
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/voteing/codelist.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
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


        protected void DefineOptions()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Vote, Params: " and Id = " + Id + "");
            if (cRs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/voteing/define.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    default:
                        try { strValue = cRs[funName].ToString(); }
                        catch { }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();

        }

        /// <summary>
        /// 添加自定义表单
        /// </summary>
        protected void Add()
        {

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/voteing/add.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, "1"); break;
                    case "isExpire": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isExpire",Value="1",Text="开启"},
                        new RadioMode(){Name="isExpire",Value="0",Text="关闭"}
                    }, "0"); break;
                    case "ischoose": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="ischoose",Value="1",Text="单选"},
                        new RadioMode(){Name="ischoose",Value="0",Text="多选"}
                    }, "1"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        protected void Update() {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Vote, Params: " and Id = " + Id + "");
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/voteing/edit.html");
            strResponse = Master.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isdisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isdisplay",Value="1",Text="显示"},
                        new RadioMode(){Name="isdisplay",Value="0",Text="关闭"}
                    }, cRs["isdisplay"].ToString()); break;
                    case "isExpire": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isExpire",Value="1",Text="开启"},
                        new RadioMode(){Name="isExpire",Value="0",Text="关闭"}
                    },cRs["isExpire"].ToString()); break;
                    case "ischoose": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="ischoose",Value="1",Text="单选"},
                        new RadioMode(){Name="ischoose",Value="0",Text="多选"}
                    }, cRs["ischoose"].ToString()); break;
                    default:
                        try { strValue = cRs[funName].ToString(); }
                        catch { }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();

        }
       

        /// <summary>
        /// 保存添加账户
        /// </summary>
        protected void AddSave()
        {
            string voteName = RequestHelper.GetRequest("voteName").toString();
            if (string.IsNullOrEmpty(voteName)) { this.ErrorMessage("请填写投票主题！"); Response.End(); }
            if (voteName.Length > 120) { this.ErrorMessage("投票主题的长度请限制在120个字符以内！"); Response.End(); }
            string isExpire = RequestHelper.GetRequest("isExpire").toInt();
            string starDate = RequestHelper.GetRequest("stardate").toString();
            if (isExpire == "1" && string.IsNullOrEmpty(starDate)) { this.ErrorMessage("请选择投票项目开始日期！"); Response.End(); }
            if (!string.IsNullOrEmpty(starDate) && !VerifyCenter.VerifyDateTime(starDate)) { this.ErrorMessage("开始日期格式错误！"); Response.End(); }
            string endDate = RequestHelper.GetRequest("endDate").toString();
            if (isExpire == "1" && string.IsNullOrEmpty(endDate)) { this.ErrorMessage("请选择投票项目结束日期！"); Response.End(); }
            if (!string.IsNullOrEmpty(endDate) && !VerifyCenter.VerifyDateTime(endDate)) { this.ErrorMessage("结束日期格式错误！"); Response.End(); }
            string descrption = RequestHelper.GetRequest("descrption").toString();
            if (descrption.Length > 200) { this.ErrorMessage("投票项目描述内容请限制在200个汉字以内！"); Response.End(); }
            string Options1 = RequestHelper.GetRequest("Options1").toString();
            if (string.IsNullOrEmpty(Options1)) { this.ErrorMessage("投票选项1不能为空！"); Response.End(); }
            if (Options1.Length > 120) { this.ErrorMessage("投票选项1长度请限制在120个汉字以内！"); Response.End(); }
            string Options2 = RequestHelper.GetRequest("Options2").toString();
            if (string.IsNullOrEmpty(Options2)) { this.ErrorMessage("投票选项2不能为空！"); Response.End(); }
            if (Options2.Length > 120) { this.ErrorMessage("投票选项2长度请限制在120个汉字以内！"); Response.End(); }
            string Options3 = RequestHelper.GetRequest("Options3").toString();
            if (Options3.Length > 120) { this.ErrorMessage("投票选项3的长度请限制在120个汉字以内！"); Response.End(); }
            string Options4 = RequestHelper.GetRequest("Options4").toString();
            if (Options4.Length > 120) { this.ErrorMessage("投票选项4的长度请限制在120个汉字以内！"); Response.End(); }
            string Options5 = RequestHelper.GetRequest("Options5").toString();
            if (Options5.Length > 120) { this.ErrorMessage("投票选项5的长度请限制在120个汉字以内！"); Response.End(); }
            string Options6 = RequestHelper.GetRequest("Options6").toString();
            if (Options6.Length > 120) { this.ErrorMessage("投票选项6的长度请限制在120个汉字以内！"); Response.End(); }
            string Options7 = RequestHelper.GetRequest("Options7").toString();
            if (Options7.Length > 120) { this.ErrorMessage("投票选项7的长度请限制在120个汉字以内！"); Response.End(); }
            string Options8 = RequestHelper.GetRequest("Options8").toString();
            if (Options8.Length > 120) { this.ErrorMessage("投票选项8的长度请限制在120个汉字以内！"); Response.End(); }
            string Options9 = RequestHelper.GetRequest("Options9").toString();
            if (Options9.Length > 120) { this.ErrorMessage("投票选项9的长度请限制在120个汉字以内！"); Response.End(); }
            string Options10 = RequestHelper.GetRequest("Options10").toString();
            if (Options10.Length > 120) { this.ErrorMessage("投票选项10的长度请限制在120个汉字以内！"); Response.End(); }
            string maxOptions = RequestHelper.GetRequest("maxOptions").toInt();
            string isChoose = RequestHelper.GetRequest("isChoose").toInt();
            if (Convert.ToInt32(maxOptions) > 10) { this.ErrorMessage("投票选项最多只有10个！"); Response.End(); }
            if (Convert.ToInt32(maxOptions) < 0) { this.ErrorMessage("最多选项不能小于0！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Vote, Params: " and voteName='" + voteName + "'");
            if (cRs != null) { this.ErrorMessage("投票项目已经存在了,请另外选择一个吧!"); Response.End(); }
            /********************************************************************
             * 以下为可以不做验证的选项
             * ******************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toString();
            string vote1 = RequestHelper.GetRequest("vote1").toInt();
            string vote2 = RequestHelper.GetRequest("vote2").toInt();
            string vote3 = RequestHelper.GetRequest("vote3").toInt();
            string vote4 = RequestHelper.GetRequest("vote4").toInt();
            string vote5 = RequestHelper.GetRequest("vote5").toInt();
            string vote6 = RequestHelper.GetRequest("vote6").toInt();
            string vote7 = RequestHelper.GetRequest("vote7").toInt();
            string vote8 = RequestHelper.GetRequest("vote8").toInt();
            string vote9 = RequestHelper.GetRequest("vote9").toInt();
            string vote10 = RequestHelper.GetRequest("vote10").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["PowerID"] = "0";
            dictionary["isDisplay"] = isDisplay;
            dictionary["VoteName"] = voteName;
            dictionary["isExpire"] = isExpire;
            dictionary["StarDate"] = starDate;
            dictionary["EndDate"] = endDate;
            dictionary["isChoose"] = isChoose;
            dictionary["Descrption"] = descrption;
            dictionary["Total"] = "0";
            dictionary["MaxOptions"] = maxOptions;
            dictionary["Options1"] = Options1;
            dictionary["Options2"] = Options2;
            dictionary["Options3"] = Options3;
            dictionary["Options4"] = Options4;
            dictionary["Options5"] = Options5;
            dictionary["Options6"] = Options6;
            dictionary["Options7"] = Options7;
            dictionary["Options8"] = Options8;
            dictionary["Options9"] = Options9;
            dictionary["Options10"] = Options10;
            dictionary["Vote1"] = vote1;
            dictionary["Vote2"] = vote2;
            dictionary["Vote3"] = vote3;
            dictionary["Vote4"] = vote4;
            dictionary["Vote5"] = vote5;
            dictionary["Vote6"] = vote6;
            dictionary["Vote7"] = vote7;
            dictionary["Vote8"] = vote8;
            dictionary["Vote9"] = vote9;
            dictionary["Vote10"] = vote10;
            DbHelper.Connection.Insert(TableCenter.Vote, dictionary);
            /********************************************************************
             * 开始输出相应代码
             * ******************************************************************/
            this.ConfirmMessage("投票项目保存成功,点击确定将继续停留在当前页面"); Response.End();
            Response.End();
        }

        protected void SaveUpdate() {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.FindRow(TableCenter.Vote, Params: " and Id = " + Id + "");
            if (Rs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            /****************************************************************************
             * 开始验证其它配置信息
             * **************************************************************************/
            string voteName = RequestHelper.GetRequest("voteName").toString();
            if (string.IsNullOrEmpty(voteName)) { this.ErrorMessage("请填写投票主题！"); Response.End(); }
            if (voteName.Length > 120) { this.ErrorMessage("投票主题的长度请限制在120个字符以内！"); Response.End(); }
            string isExpire = RequestHelper.GetRequest("isExpire").toInt();
            string starDate = RequestHelper.GetRequest("stardate").toString();
            if (isExpire == "1" && string.IsNullOrEmpty(starDate)) { this.ErrorMessage("请选择投票项目开始日期！"); Response.End(); }
            if (!string.IsNullOrEmpty(starDate) && !VerifyCenter.VerifyDateTime(starDate)) { this.ErrorMessage("开始日期格式错误！"); Response.End(); }
            string endDate = RequestHelper.GetRequest("endDate").toString();
            if (isExpire == "1" && string.IsNullOrEmpty(endDate)) { this.ErrorMessage("请选择投票项目结束日期！"); Response.End(); }
            if (!string.IsNullOrEmpty(endDate) && !VerifyCenter.VerifyDateTime(endDate)) { this.ErrorMessage("结束日期格式错误！"); Response.End(); }
            string descrption = RequestHelper.GetRequest("descrption").toString();
            if (descrption.Length > 200) { this.ErrorMessage("投票项目描述内容请限制在200个汉字以内！"); Response.End(); }
            string Options1 = RequestHelper.GetRequest("Options1").toString();
            if (string.IsNullOrEmpty(Options1)) { this.ErrorMessage("投票选项1不能为空！"); Response.End(); }
            if (Options1.Length > 120) { this.ErrorMessage("投票选项1长度请限制在120个汉字以内！"); Response.End(); }
            string Options2 = RequestHelper.GetRequest("Options2").toString();
            if (string.IsNullOrEmpty(Options2)) { this.ErrorMessage("投票选项2不能为空！"); Response.End(); }
            if (Options2.Length > 120) { this.ErrorMessage("投票选项2长度请限制在120个汉字以内！"); Response.End(); }
            string Options3 = RequestHelper.GetRequest("Options3").toString();
            if (Options3.Length > 120) { this.ErrorMessage("投票选项3的长度请限制在120个汉字以内！"); Response.End(); }
            string Options4 = RequestHelper.GetRequest("Options4").toString();
            if (Options4.Length > 120) { this.ErrorMessage("投票选项4的长度请限制在120个汉字以内！"); Response.End(); }
            string Options5 = RequestHelper.GetRequest("Options5").toString();
            if (Options5.Length > 120) { this.ErrorMessage("投票选项5的长度请限制在120个汉字以内！"); Response.End(); }
            string Options6 = RequestHelper.GetRequest("Options6").toString();
            if (Options6.Length > 120) { this.ErrorMessage("投票选项6的长度请限制在120个汉字以内！"); Response.End(); }
            string Options7 = RequestHelper.GetRequest("Options7").toString();
            if (Options7.Length > 120) { this.ErrorMessage("投票选项7的长度请限制在120个汉字以内！"); Response.End(); }
            string Options8 = RequestHelper.GetRequest("Options8").toString();
            if (Options8.Length > 120) { this.ErrorMessage("投票选项8的长度请限制在120个汉字以内！"); Response.End(); }
            string Options9 = RequestHelper.GetRequest("Options9").toString();
            if (Options9.Length > 120) { this.ErrorMessage("投票选项9的长度请限制在120个汉字以内！"); Response.End(); }
            string Options10 = RequestHelper.GetRequest("Options10").toString();
            if (Options10.Length > 120) { this.ErrorMessage("投票选项10的长度请限制在120个汉字以内！"); Response.End(); }
            string maxOptions = RequestHelper.GetRequest("maxOptions").toInt();
            string isChoose = RequestHelper.GetRequest("isChoose").toInt();
            if (Convert.ToInt32(maxOptions) > 10) { this.ErrorMessage("投票选项最多只有10个！"); Response.End(); }
            if (Convert.ToInt32(maxOptions) < 0) { this.ErrorMessage("最多选项不能小于0！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.FindRow(TableCenter.Vote, Params: " and voteName='" + voteName + "' and Id<>" + Id + "");
            if (cRs != null) { this.ErrorMessage("投票项目已经存在了,请另外选择一个吧!"); Response.End(); }
            /********************************************************************
             * 以下为可以不做验证的选项
             * ******************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toString();
            string vote1 = RequestHelper.GetRequest("vote1").toInt();
            string vote2 = RequestHelper.GetRequest("vote2").toInt();
            string vote3 = RequestHelper.GetRequest("vote3").toInt();
            string vote4 = RequestHelper.GetRequest("vote4").toInt();
            string vote5 = RequestHelper.GetRequest("vote5").toInt();
            string vote6 = RequestHelper.GetRequest("vote6").toInt();
            string vote7 = RequestHelper.GetRequest("vote7").toInt();
            string vote8 = RequestHelper.GetRequest("vote8").toInt();
            string vote9 = RequestHelper.GetRequest("vote9").toInt();
            string vote10 = RequestHelper.GetRequest("vote10").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["PowerID"] = "0";
            dictionary["isDisplay"] = isDisplay;
            dictionary["VoteName"] = voteName;
            dictionary["isExpire"] = isExpire;
            dictionary["StarDate"] = starDate;
            dictionary["EndDate"] = endDate;
            dictionary["isChoose"] = isChoose;
            dictionary["Descrption"] = descrption;
            dictionary["MaxOptions"] = maxOptions;
            dictionary["Options1"] = Options1;
            dictionary["Options2"] = Options2;
            dictionary["Options3"] = Options3;
            dictionary["Options4"] = Options4;
            dictionary["Options5"] = Options5;
            dictionary["Options6"] = Options6;
            dictionary["Options7"] = Options7;
            dictionary["Options8"] = Options8;
            dictionary["Options9"] = Options9;
            dictionary["Options10"] = Options10;
            dictionary["Vote1"] = vote1;
            dictionary["Vote2"] = vote2;
            dictionary["Vote3"] = vote3;
            dictionary["Vote4"] = vote4;
            dictionary["Vote5"] = vote5;
            dictionary["Vote6"] = vote6;
            dictionary["Vote7"] = vote7;
            dictionary["Vote8"] = vote8;
            dictionary["Vote9"] = vote9;
            dictionary["Vote10"] = vote10;
            DbHelper.Connection.Update(TableCenter.Vote, dictionary, Params: " and Id=" + Id + "");
            /********************************************************************
             * 开始输出相应代码
             * ******************************************************************/
            this.ConfirmMessage("投票项目保存成功,点击确定将继续停留在当前页面"); Response.End();
            Response.End();
        }
        /// <summary>
        /// 删除表单数据
        /// </summary>
        protected void Delete()
        {
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            DataTable Tab = DbHelper.Connection.FindTable(TableCenter.Vote, Params: " and Id in (" + Id + ")");
            foreach (DataRow Rs in Tab.Rows)
            {
                DbHelper.Connection.Delete(TableCenter.Vote, Params: " and Id = " + Rs["Id"] + "");
            }
            this.History();
            Response.End();
        }
        /// <summary>
        /// 审核投票项目
        /// </summary>
        protected void Display()
        {
            string Id = RequestHelper.GetRequest("Id").toString();
            if (string.IsNullOrEmpty(Id)) { this.ErrorMessage("参数错误，请至少选择一条数据！"); Response.End(); }
            string isDisplay = RequestHelper.GetRequest("val").toInt();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["isDisplay"] = isDisplay;
            DbHelper.Connection.Update(TableCenter.Vote, dictionary, Params: " and Id in (" + Id + ")");
            this.History();
            Response.End();
        }
    }
}