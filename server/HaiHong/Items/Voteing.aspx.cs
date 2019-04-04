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
using Fooke.SimpleMaster;
namespace Fooke.Web
{
    public partial class Voteing : Fooke.Code.BaseHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "start": StartVoteing(); Response.End(); break;
                case "js": Javascript(); Response.End(); break;
                case "default": strDefault(); Response.End(); break;
            }
            Response.End();
        }
        /// <summary>
        /// 显示投票页面
        /// </summary>
        protected void strDefault()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindVote]", new Dictionary<string, object>() {
                {"VoteID",Id}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            if (cRs["isDisplay"].ToString() != "1") { this.ErrorMessage("对不起，你查找的数据不存在！"); Response.End(); }
            /*********************************************************************************
             * 生成选项信息
             * ********************************************************************************/
            StringBuilder strOptions = new StringBuilder();
            for (int k = 1; k <= 10; k++)
            {
                if (!string.IsNullOrEmpty(cRs["options" + k].ToString()))
                {
                    strOptions.AppendFormat("<div class=\"items\" id=\"frm-option-{0}\">", k);
                    strOptions.AppendFormat("<label><input type=\"{1}\" name=\"vote\" value=\"vote{0}\" id=\"vote{0}\" />", k, (cRs["ischoose"].ToString() == "0" ? "checkbox" : "radio"));
                    strOptions.AppendFormat("{0}({1}票)", cRs["options" + k + ""], cRs["vote" + k]);
                    strOptions.AppendFormat("</label>");
                    strOptions.AppendFormat("</div>");
                }
            }
            /**********************************************************************************
             * 生成一个投票的token,保证数据请求有效
             * *********************************************************************************/
            string Tokey = string.Format("-|-|-投票系统-|-|-{0}-|-|-{1}", cRs["Id"], cRs["voteName"]).md5().ToLower();
            /*********************************************************************************
             * 显示网络数据
             * ********************************************************************************/
            string TemplateDirectory = this.GetParameter("TemplateDir", "siteXML").toString();
            if (string.IsNullOrEmpty(TemplateDirectory)) { TemplateDirectory = "template"; }
            string cTemplate = Win.ApplicationPath + "/" + TemplateDirectory + "/voteing.html";
            /*********************************************************************************
            * 解析模板,输出模板内容
            * ********************************************************************************/
            SimpleMaster.SimpleMaster Fooke = new SimpleMaster.SimpleMaster();
            string strResponse = Fooke.Reader(cTemplate);
            strResponse = Fooke.Start(strResponse, new SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "options": strValue = strOptions.toString(); break;
                    case "Tokey": strValue = Tokey.ToString(); break;
                    default: try { strValue = cRs[funName].ToString(); }
                        catch { } break;
                }
                return strValue;
            }), true);
            Response.Write(strResponse);
            Response.End();
        }

        protected void JsMessage(string msgTxt)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("document.write('" + msgTxt + "');");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /// <summary>
        /// Js调用投票项目显示
        /// </summary>
        protected void Javascript()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { this.JsMessage("请求参数错误！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindVote]", new Dictionary<string, object>() {
                {"VoteID",Id}
            });
            if (cRs == null) { this.JsMessage("对不起,你查找的数据不存在！"); Response.End(); }
            if (cRs["isDisplay"].ToString() != "1") { this.JsMessage("对不起,你查找的数据不存在！"); Response.End(); }
            string strKey = "用户投票-|-|-" + cRs["Id"].ToString() + "-|-|-投票数据";
            strKey = new Fooke.Function.String(strKey).ToMD5().ToLower();
            /********************************************************************************
             * 保存一个cookie识别是否为非法提交
             * ******************************************************************************/
            try { CookieHelper.Add("fooke_voteing", "true", 90); }
            catch { }
            /********************************************************************************
             * 显示输出数据
             * ******************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("document.write('<div class=\"frm-voteing-box\" id=\"frm-voteing-box-" + cRs["id"] + "\">');");
            strBuilder.AppendLine("document.write('<form action=\"" + Win.ApplicationPath + "/items/voteing.aspx\" id=\"frm-voteing-form\" id=\"frm-voteing-form\" method=\"method\">');");
            strBuilder.AppendLine("document.write('<input type=\"hidden\" name=\"Id\" value=\"" + cRs["Id"] + "\" />');");
            strBuilder.AppendLine("document.write('<input type=\"hidden\" name=\"action\" value=\"start\" />');");
            strBuilder.AppendLine("document.write('<input type=\"hidden\" name=\"token\" value=\"" + strKey + "\" />');");
            strBuilder.AppendLine("document.write('<div id=\"frm-voteing-title\">" + cRs["voteName"] + "</div>');");
            strBuilder.AppendLine("document.write('<div id=\"frm-voteing-descrption\">" + cRs["descrption"] + "</div>');");
            strBuilder.AppendLine("document.write('<div id=\"frm-voteing-options\">');");
            for (int k = 1; k <= 10; k++)
            {
                if (!string.IsNullOrEmpty(cRs["options" + k].ToString()))
                {
                    strBuilder.AppendLine("document.write('<div id=\"frm-voteing-options" + k + "\">');");
                    strBuilder.AppendLine("document.write('<label>');");
                    if (cRs["ischoose"].ToString() == "0") { strBuilder.AppendLine("document.write('<input type=\"checkbox\" value=\"vote" + k + "\" name=\"vote\" />');"); }
                    else { strBuilder.AppendLine("document.write('<input type=\"radio\" value=\"vote" + k + "\" name=\"vote\" />');"); }
                    strBuilder.AppendLine("document.write('" + cRs["options" + k + ""] + "(" + cRs["Vote" + k] + "票)');");
                    strBuilder.AppendLine("document.write('</label>');");
                    strBuilder.AppendLine("document.write('</div>');");
                }
            }
            strBuilder.AppendLine("document.write('</div>');");
            strBuilder.AppendLine("document.write('<div id=\"frm-voteing-btns\">');");
            strBuilder.AppendLine("document.write('<input type=\"submit\" id=\"frm-voteing-submit\" value=\"投票\" />');");
            strBuilder.AppendLine("document.write('</div>');");
            strBuilder.AppendLine("document.write('</form>');");
            strBuilder.AppendLine("document.write('</div>');");
            Response.Write(strBuilder.ToString());
            Response.End();
        }
        /********************************************************************
         * 数据处理区域
         * ******************************************************************/
        /// <summary>
        /// 开始保存投票信息
        /// </summary>
        protected void StartVoteing()
        {
            string Id = RequestHelper.GetRequest("Id").toInt();
            if (Id == "0") { Response.Write("请求参数错误，请返回重试！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindVote]", new Dictionary<string, object>() {
                {"VoteID",Id}
            });
            if (cRs == null) { Response.Write("对不起，你查找的数据不存在！"); Response.End(); }
            if (cRs["isDisplay"].ToString() != "1") { Response.Write("当前投票选项已被禁止！"); Response.End(); }
            /****************************************************************************
             * 验证用户投票权限，是否拥有投票权限
             * **************************************************************************/
            string token = RequestHelper.GetRequest("token").toString();
            if (string.IsNullOrEmpty(token)) { Response.Write("请求参数错误！"); Response.End(); }
            string strOkey = "用户投票-|-|-" + cRs["Id"].ToString() + "-|-|-投票数据";
            strOkey = new Fooke.Function.String(strOkey).ToMD5().ToLower();
            if (token != strOkey) { Response.Write("参数信息错误，请返回重试！"); Response.End(); }
            string strKey = "用户投票数据-|-|-" + cRs["id"].ToString();
            strKey = new Fooke.Function.String(strKey).ToMD5().Substring(0, 20).toString();
            string cookey = CookieHelper.Get(strKey).toString();
            if (!string.IsNullOrEmpty(cookey)) { Response.Write("已经投过票了，请勿重复投票！"); Response.End(); }
            /***************************************************************************
             * 验证用户是否为cookie提交
             * *************************************************************************/
            string isTrue = CookieHelper.Get("fooke_voteing").toString("true");
            if (isTrue.ToLower() != "true") { Response.Write("请求参数错误，请刷新网页重试！"); Response.End(); }
            /****************************************************************************
             *开始验证其它数据信息
             * **************************************************************************/
            string vote = RequestHelper.GetRequest("vote").toString();
            if (string.IsNullOrEmpty(vote)) { Response.Write("请至少选择一个选项！"); Response.End(); }
            string[] voteTemp = vote.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (voteTemp.Length <= 0) { Response.Write("请至少选择一个投票项目！"); Response.End(); }
            int maxOptions = new Fooke.Function.String(cRs["maxOptions"].ToString()).cInt();
            if (maxOptions != 0 && voteTemp.Length > maxOptions) { Response.Write("最多只能选择" + maxOptions + "个项目投票！"); Response.End(); }
            if (cRs["ischoose"].ToString() == "0" && vote.Contains(",")) { Response.Write("单选项只能选择一个数据！"); Response.End(); }
            if (cRs["isExpire"].ToString() == "1")
            {
                DateTime StarDate = new Fooke.Function.String(cRs["StarDate"].ToString()).cDate();
                DateTime EndDate = new Fooke.Function.String(cRs["EndDate"].ToString()).cDate();
                if ((DateTime.Now >= StarDate && DateTime.Now <= EndDate)) { }
                else { Response.Write("当前投票项目设置了投票日期" + StarDate.ToString("yyyy-MM-dd") + "到" + EndDate.ToString("yyyy-MM-dd") + ",请到指定的日期再来投票吧！"); Response.End(); }
            }
            /*******************************************************************************
             * 开始保存数据
             * *****************************************************************************/
            try
            {
                Dictionary<string, string> oDic = new Dictionary<string, string>();
                oDic["total"] = (new Fooke.Function.String(cRs["total"].ToString()).cInt() + voteTemp.Length).ToString();
                foreach (string Char in voteTemp)
                {
                    try { oDic[Char] = (new Fooke.Function.String(cRs[Char].ToString()).cInt() + 1).ToString(); }
                    catch { }
                }
                if (oDic.Count <= 0) { Response.Write("发生未知错误，请返回重试！"); Response.End(); }
                DbHelper.Connection.Update(TableCenter.Vote, oDic, Params: " and Id=" + Id + "");
            }
            catch { }
            /************************************************************************************
             * 保存Cookies数据
             * *********************************************************************************/
            CookieHelper.Add(strKey, "1", 90);
            /************************************************************************************
             * 开始输出其它的信息
             * **********************************************************************************/
            Response.Write("投票成功"); Response.End();
        }


    }
}