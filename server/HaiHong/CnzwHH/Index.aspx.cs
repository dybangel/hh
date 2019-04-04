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
namespace Fooke.Web.Admin
{
    public partial class Index : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "header": this.HeaderMain(); Response.End(); break;
                case "footer": this.footerMain(); Response.End(); break;
                case "tools": this.toolMain(); Response.End(); break;
                case "left": this.LeftMain(); Response.End(); break;
                case "right": this.RightMain(); Response.End(); break;
                case "saveTo": this.saveTo(); Response.End(); break;
                case "message": this.MessageTips(); Response.End(); break;
                default: strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 显示默认数据信息
        /// </summary>
        protected void strDefault()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/frame/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "sitename": strValue = this.GetParameter("siteName").toString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 获取用户的提现申请，给出提示
        /// </summary>
        protected void MessageTips()
        {
            DataTable Tab = DbHelper.Connection.ExecuteFindTable("Stored_FindAdminMessage", null);
            if (Tab == null || Tab.Rows.Count <= 0) { Response.End(); }
            StringBuilder strXml = new StringBuilder();
            foreach (DataRow Rs in Tab.Rows)
            {
                if (Rs["number"].ToString() != "0")
                {
                    strXml.Append("<a href=\"" + Rs["Links"] + "\">" + Rs["Remark"] + "(<font style=\"color:#f00; font-weight:bold; margin:0px 5px;\">" + Rs["number"] + "</font>条)</a>");
                }
            }
            Response.Write(strXml.ToString());
            Response.End();
        }
        /// <summary>
        /// 保存管理员的通讯录
        /// </summary>
        protected void saveTo()
        {
            string intro = RequestHelper.GetRequest("intro").toString();
            if (!string.IsNullOrEmpty(intro) && intro.Length > 500) { this.ErrorMessage("备忘录内容请限制在500个字符以内！"); Response.End(); }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["strDesc"] = intro;
            DbHelper.Connection.Update(TableCenter.Admin, dictionary, Params: " and AdminId = " + AdminRs["AdminID"] + "");
            Response.Redirect("Index.aspx?action=right");
            Response.End();
        }

        /// <summary>
        /// 生成右边工作区域
        /// </summary>
        protected void RightMain()
        {
            /**********************************************************************************************
             * 输出网页内容信息
             * *********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/frame/right.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "strdesc": strValue = AdminRs["strdesc"].ToString(); break;
                    case "adminName": strValue = AdminRs["adminName"].ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 左边页面代码
        /// </summary>
        protected void LeftMain()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/frame/left.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "thumb":
                        strValue = this.GetParameter("oIcon", "siteXML").toString();
                        if (!strValue.Contains("http")) { strValue = Win.ApplicationPath + strValue; }
                        break;
                    case "adminname": strValue = AdminRs["adminName"].ToString(); break;
                    case "menubar": strValue = FrmMenuBox(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        public string ShowAdministrator(bool Administrator)
        {
            if (!Administrator) { return "style=\"display:none\""; }
            else { return string.Empty; }
        }
        protected string FrmMenuBox()
        {

            string MenuBar = RequestHelper.GetRequest("MenuBar").ToString();
            if (string.IsNullOrEmpty(MenuBar)) { MenuBar = CookieHelper.Get("fooke_site_menubar").toString(); }
            if (string.IsNullOrEmpty(MenuBar)) { MenuBar = "content"; }
            if (!string.IsNullOrEmpty(MenuBar)) { CookieHelper.Add("fooke_site_menubar", MenuBar, 360); }
            bool Administrator = (AdminRs["PowerID"].ToString() == "0" ? true : false);
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<div id=\"frm-menubar-box\">");
            if (MenuBar == "content")
            {
                strBuilder.Append("<div class=\"MenusBar\">");
                strBuilder.Append("<div class=\"titles current\"><span class=\"ico\"></span><span class=\"name\">系统设置</span></div>");
                strBuilder.Append("<div class=\"MenusList\" style=\"display:!important\">");
                strBuilder.Append("<ul>");
                strBuilder.Append("<li><a href=\"config.aspx?action=base\">基本配置</a> <a class=\"frm-add\" href=\"config.aspx?action=Message\"><img src=\"template/images/duanxin.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"config.aspx?action=app\">应用下载</a> <a class=\"frm-add\" href=\"config.aspx?action=pushXml\"><img src=\"template/images/tuisong.gif\" /></a></li>");
                if (Administrator) { strBuilder.Append("<li><a href=\"Admin.aspx?action=default\">系统账户</a><a class=\"frm-add\" href=\"Admin.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>"); }
                strBuilder.Append("<li><a href=\"Defend.aspx?action=default\">数据维护</a><a class=\"frm-add\" href=\"Defend.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("</ul>");
                strBuilder.Append("</div>");
                strBuilder.Append("</div>");
                /*****************************************************************
                * 标签模版
                * ***************************************************************/
                if (RequestHelper.GetRequest("isFunction").toInt() == "1")
                {
                    strBuilder.Append("<div class=\"MenusBar\">");
                    strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">模型管理</span></div>");
                    strBuilder.Append("<div class=\"MenusList\">");
                    strBuilder.Append("<ul>");
                    strBuilder.Append("<li><a href=\"Channel.aspx?action=default\">模型管理</a><a class=\"frm-add\" href=\"channel.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                    foreach (DataRow cRs in ChannelHelper.FindTable().Rows)
                    {
                        strBuilder.Append("<li>");
                        strBuilder.Append("<a href=\"columns.aspx?action=default&chName=" + cRs["channelName"] + "&Tablename=" + cRs["Tablename"] + "&ChannelID=" + cRs["ChannelID"] + "\">");
                        strBuilder.Append("" + cRs["Unitname"] + "字段</a>");
                        strBuilder.Append("<a class=\"frm-add\" href=\"columns.aspx?action=add&chName=" + cRs["channelName"] + "&Tablename=" + cRs["Tablename"] + "&ChannelID=" + cRs["ChannelID"] + "\">");
                        strBuilder.Append("<img src=\"template/images/addnews.gif\" /></a>");
                        strBuilder.Append("</li>");
                    }
                    strBuilder.Append("</ul>");
                    strBuilder.Append("</div>");
                    strBuilder.Append("</div>");
                }
                /*****************************************************************
                * 内容管理
                * ***************************************************************/
                strBuilder.Append("<div class=\"MenusBar\">");
                strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">内容管理</span></div>");
                strBuilder.Append("<div style=\"display:block\" class=\"MenusList\">");
                strBuilder.Append("<ul>");
                strBuilder.Append("<li><a href=\"class.aspx?action=default\">栏目管理</a><a class=\"frm-add\" href=\"class.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                new ChannelHelper().FindTable((cRs) => {
                    string BaseUrl = "Article.aspx";
                    if (cRs["Basename"].ToString() == "文章系统") { BaseUrl = "Article.aspx"; }
                    else if (cRs["Basename"].ToString() == "空模型") { BaseUrl = "EmptyChannel.aspx"; }
                    strBuilder.Append("<li><a href=\"" + BaseUrl + "?action=default&ChannelID=" + cRs["ChannelID"] + "\">");
                    strBuilder.Append("" + cRs["channelName"] + "</a><a class=\"frm-add\" href=\"" + BaseUrl + "?action=add&ChannelID=" + cRs["ChannelID"] + "\"><img src=\"template/images/addnews.gif\" /></a></li>");
                });
                strBuilder.Append("<li><a href=\"Single.aspx?action=default\">单页管理</a><a class=\"frm-add\" href=\"single.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"UserNotification.aspx?action=default\">系统公告</a><a class=\"frm-add\" href=\"UserNotification.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("</ul>");
                strBuilder.Append("</div>");
                strBuilder.Append("</div>");
                if (RequestHelper.GetRequest("isFunction").toInt() == "1")
                {
                    strBuilder.Append("<div class=\"MenusBar\">");
                    strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">标签模版</span></div>");
                    strBuilder.Append("<div class=\"MenusList\">");
                    strBuilder.Append("<ul>");
                    strBuilder.Append("<li><a href=\"label.aspx?action=default\">系统函数标签</a><a class=\"frm-add\" href=\"label/items.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                    strBuilder.Append("<li><a href=\"label.aspx?action=default&style=5\">系统静态标签</a><a class=\"frm-add\" href=\"label/Quiescent.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                    strBuilder.Append("<li><a href=\"Template.aspx?action=default\">系统模版管理</a></li>");
                    strBuilder.Append("</ul>");
                    strBuilder.Append("</ul>");
                    strBuilder.Append("</div>");
                    strBuilder.Append("</div>");
                }
                if (RequestHelper.GetRequest("isFunction").toInt() == "1")
                {
                    strBuilder.Append("<div class=\"MenusBar\">");
                    strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">其它系统</span></div>");
                    strBuilder.Append("<div class=\"MenusList\">");
                    strBuilder.Append("<ul>");
                    strBuilder.Append("<li><a href=\"Advert.aspx?action=default\">广告系统</a><a class=\"frm-add\" href=\"advert.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                    strBuilder.Append("<li><a href=\"Link.aspx?action=default\">友情连接</a><a class=\"frm-add\" href=\"link.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                    strBuilder.Append("<li><a href=\"voteing.aspx?action=default\">投票系统</a><a class=\"frm-add\" href=\"voteing.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                    strBuilder.Append("<li><a href=\"forms.aspx?action=default\">在线表单</a><a class=\"frm-add\" href=\"forms.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                    strBuilder.Append("</ul>");
                    strBuilder.Append("</div>");
                    strBuilder.Append("</div>");
                }
                if (RequestHelper.GetRequest("isFunction").toInt() == "1")
                {
                    strBuilder.Append("<div class=\"MenusBar\">");
                    strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">发布管理</span></div>");
                    strBuilder.Append("<div class=\"MenusList\">");
                    strBuilder.Append("<ul>");
                    strBuilder.Append("<li><a href=\"release.aspx?action=default\">生成主页</a></li>");
                    strBuilder.Append("<li><a href=\"release.aspx?action=list\">生成栏目</a></li>");
                    strBuilder.Append("<li><a href=\"release.aspx?action=single\">生成单页</a></li>");
                    strBuilder.Append("<li><a href=\"release.aspx?action=context\">生成内容</a></li>");
                    strBuilder.Append("</ul>");
                    strBuilder.Append("</div>");
                    strBuilder.Append("</div>");
                }


                //strBuilder.Append("<div class=\"MenusBar\">");
                //strBuilder.Append("<div class=\"titles current\"><span class=\"ico\"></span><span class=\"name\">微信平台</span></div>");
                //strBuilder.Append("<div class=\"MenusList\">");
                //strBuilder.Append("<ul>");
                //strBuilder.Append("<li><a href=\"WechatConfig.aspx?action=demo\">回复设置</a> <a class=\"frm-add\" href=\"WeChatConfig.aspx?action=wechat\"><img src=\"template/images/canshu.gif\" /></a></li>");
                //strBuilder.Append("<li><a href=\"WeChatMenu.aspx?action=default\">微信菜单</a> <a class=\"frm-add\" href=\"WeChatMenu.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                //strBuilder.Append("<li><a href=\"WeChatKeywords.aspx?action=default\">关键词汇</a> <a class=\"frm-add\" href=\"WeChatKeywords.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                //strBuilder.Append("<li><a href=\"WechatMater.aspx?action=default\">素材管理</a> <a class=\"frm-add\" href=\"WechatMater.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                //strBuilder.Append("</ul>");
                //strBuilder.Append("</div>");
                //strBuilder.Append("</div>");
            }
            else if (MenuBar == "user")
            {
                /*********************************************************************************
                 * 用户信息
                 * *******************************************************************************/
                strBuilder.Append("<div class=\"MenusBar\">");
                strBuilder.Append("<div class=\"titles current\"><span class=\"ico\"></span><span class=\"name\">用户系统</span></div>");
                strBuilder.Append("<div class=\"MenusList\" style=\"display:block!important\">");
                strBuilder.Append("<ul>");
                strBuilder.Append("<li><a href=\"Config.aspx?action=user\">用户参数</a><a class=\"frm-add\" href=\"config.aspx?action=shareXml\"><img src=\"template/images/yaoqing.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"User.aspx?action=default\">用户管理</a><a class=\"frm-add\" href=\"User.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"Amount.aspx?action=default\">余额明细</a><a " + ShowAdministrator(Administrator) + " class=\"frm-add\" href=\"Amount.aspx?action=add\"><img src=\"template/images/chongzhi.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"Report.aspx?action=default\">数据报表</a> <a class=\"frm-add\" href=\"Report.aspx?action=list\"><img src=\"template/images/mingxi.gif\" /></a></li>");
                //strBuilder.Append("<li><a href=\"Short.aspx?action=default\">用户消息</a> <a class=\"frm-add\" href=\"Short.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"Daliy.aspx?action=default\">登陆历史记录</a></li>");
                strBuilder.Append("</ul>");
                strBuilder.Append("</div>");
                strBuilder.Append("</div>");

                /*********************************************************************************
                 * 用户充值
                 * *******************************************************************************/
                //strBuilder.Append("<div class=\"MenusBar\">");
                //strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">用户充值</span></div>");
                //strBuilder.Append("<div class=\"MenusList\" style=\"display:none!important\">");
                //strBuilder.Append("<ul>");
                //strBuilder.Append("<li><a href=\"Pay.aspx?action=default\">充值记录</a> <a class=\"frm-add\" href=\"Pay.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                //strBuilder.Append("<li><a href=\"Payment.aspx?action=default\">支付平台</a> <a class=\"frm-add\" href=\"Payment.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                //strBuilder.Append("<li><a href=\"Rechargeable.aspx?action=default\">日志记录</a> <a class=\"frm-add\" href=\"Config.aspx?action=pay\"><img src=\"template/images/canshu.gif\" /></a></li>");
                //strBuilder.Append("</ul>");
                //strBuilder.Append("</div>");
                //strBuilder.Append("</div>");
                /*********************************************************************************
                 * 用户提现
                 * *******************************************************************************/
                strBuilder.Append("<div class=\"MenusBar\">");
                strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">用户提现</span></div>");
                strBuilder.Append("<div class=\"MenusList\">");
                strBuilder.Append("<ul>");
                strBuilder.Append("<li><a href=\"Alipay.aspx?action=default\">提现记录</a> <a class=\"frm-add\" href=\"Alipay.aspx?action=looker\"><img src=\"template/images/mingxi.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"Alipay.aspx?action=computer\">数据统计</a> <a class=\"frm-add\" href=\"config.aspx?action=alipay\"><img src=\"template/images/canshu.gif\" /></a></li>");
                strBuilder.Append("</ul>");
                strBuilder.Append("</div>");
                strBuilder.Append("</div>");
                strBuilder.Append("<div class=\"MenusBar\">");
                strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">用户签到</span></div>");
                strBuilder.Append("<div class=\"MenusList\">");
                strBuilder.Append("<ul>");
                strBuilder.Append("<li><a href=\"Sign.aspx?action=default\">用户签到</a> <a class=\"frm-add\" href=\"Config.aspx?action=SignXml\"><img src=\"template/images/canshu.gif\" /></a></li>");
                strBuilder.Append("</ul>");
                strBuilder.Append("</div>");
                strBuilder.Append("</div>");
                /*********************************************************************************
                 * 帮助模块
                 * *******************************************************************************/
                strBuilder.Append("<div class=\"MenusBar\">");
                strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">帮助模块</span></div>");
                strBuilder.Append("<div class=\"MenusList\" style=\"display:none!important\">");
                strBuilder.Append("<ul>");
                strBuilder.Append("<li><a href=\"Guest.aspx?action=default\">留言反馈</a> <a class=\"frm-add\" href=\"Config.aspx?action=Guest\"><img src=\"template/images/canshu.gif\" /></a></li>");
                strBuilder.Append("</ul>");
                strBuilder.Append("</div>");
                strBuilder.Append("</div>");

            }
            else if (MenuBar == "union")
            {
                /*********************************************************************************
                 * 用户信息
                 * *******************************************************************************/
                strBuilder.Append("<div class=\"MenusBar\">");
                strBuilder.Append("<div class=\"titles\"><span class=\"ico\"></span><span class=\"name\">联盟渠道</span></div>");
                strBuilder.Append("<div class=\"MenusList\" style=\"display:block!important\">");
                strBuilder.Append("<ul>");
                strBuilder.Append("<li><a href=\"UnionChannel.aspx?action=default\">渠道管理</a><a class=\"frm-add\" href=\"UnionChannel.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"DutyReport.aspx?action=default\">统计报表</a><a class=\"frm-add\" href=\"DutyReport.aspx?action=list\"><img src=\"template/images/mingxi.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"UserDuty.aspx?action=default\">任务记录</a> <a class=\"frm-add\" href=\"BackDaily.aspx?action=default\"><img src=\"template/images/tanzhen.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"UserDuty.aspx?action=excel\">数据导出</a> <a class=\"frm-add\" href=\"UserDuty.aspx?action=computer\"><img src=\"template/images/tongji.gif\" /></a></li>");
                strBuilder.Append("</ul>");
                strBuilder.Append("</div>");
                strBuilder.Append("</div>");
                /*********************************************************************************
               * 交易系统
               * *******************************************************************************/
                strBuilder.Append("<div class=\"MenusBar\">");
                strBuilder.Append("<div class=\"titles current\"><span class=\"ico\"></span><span class=\"name\">自有渠道</span></div>");
                strBuilder.Append("<div class=\"MenusList\">");
                strBuilder.Append("<ul>");
                strBuilder.Append("<li><a href=\"appClass.aspx?action=default\">应用分类</a> <a class=\"frm-add\" href=\"appClass.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"Application.aspx?action=default\">应用管理</a> <a class=\"frm-add\" href=\"Application.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"AppTasker.aspx?action=default\">计划任务</a> <a class=\"frm-add\" href=\"AppTasker.aspx?action=add\"><img src=\"template/images/addnews.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"AppDown.aspx?action=default\">下载记录</a> <a class=\"frm-add\" href=\"AppDown.aspx?action=excel\"><img src=\"template/images/daochu.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"AppDownReport.aspx?action=default\">统计报表</a><a class=\"frm-add\" href=\"AppDownReport.aspx?action=list\"><img src=\"template/images/mingxi.gif\" /></a></li>");
                strBuilder.Append("<li><a href=\"AppHot.aspx?action=default\">截图审核</a> <a class=\"frm-add\" href=\"AppHot.aspx?action=excel\"><img src=\"template/images/daochu.gif\" /></a></li>");
                strBuilder.Append("</ul>");
                strBuilder.Append("</div>");
                strBuilder.Append("</div>");
            }
            strBuilder.Append("</div>");

            return strBuilder.ToString();
        }

        /// <summary>
        /// 显示顶部信息
        /// </summary>
        protected void HeaderMain()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/frame/top.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        protected void toolMain() {

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/frame/strip.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "sitename": strValue = this.GetParameter("siteName").toString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 底部版权信息
        /// </summary>
        protected void footerMain()
        {
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/frame/footer.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "datetime": strValue = DateTime.Now.ToString(); break;
                    case "sitename": strValue = this.GetParameter("siteName").toString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }


}