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
namespace Fooke.Web.Admin
{
    public partial class Release : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (strRequest)
            {
                case "list": strList(); Response.End(); break;
                case "refreList": SaveList(); Response.End(); break;
                case "refreDefault": SaveDefault(); Response.End(); break;
                case "single": strSingle(); Response.End(); break;
                case "refreSingle": SaveSingle(); Response.End(); break;
                case "context": strContext(); Response.End(); break;
                case "refreContext": SaveContext(); Response.End(); break;
                default: strDefault(); Response.End(); break;

            }
        }
        /// <summary>
        /// 生成系统主页面
        /// </summary>
        protected void strDefault()
        {
            /*******************************************************************************
             * 开始输出网页数据
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/release/default.html");
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
        /// 生成系统主单页
        /// </summary>
        protected void strSingle()
        {

            /*******************************************************************************
             * 输出网页内容信息
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/release/single.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "option": strValue = SingleHelper.Options("0"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 生成系统栏目页
        /// </summary>
        protected void strList()
        {
            /*******************************************************************************
             * 输出网页内容信息
             * *****************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/release/list.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "channel": strValue = new Fooke.Code.ChannelHelper().Options(defaultText: "0",
                        value: "channelid", text: "channelname"); break;
                    case "class": strValue = Fooke.Code.ClassHelper.Options(ChannelID: "0", ParentID: "0"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();

        }
        /// <summary>
        /// 生成内容
        /// </summary>
        protected void strContext()
        {
            /*******************************************************************************************************************
             * 获取默认参数信息
             * ****************************************************************************************************************/
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            /*******************************************************************************************************************
             * 输出网页内容信息
             * ****************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/release/content.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "channel": strValue = new Fooke.Code.ChannelHelper().Options(defaultText: ChannelID,
                        value: "channelid", text: "channelname"); break;
                    case "class": strValue = Fooke.Code.ClassHelper.Options(ChannelID: "0", ParentID: "0"); break;
                    case "channelid": strValue = ChannelID; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /*************************************************************************************
         * 数据处理区域
         * ***********************************************************************************/
        /// <summary>
        /// 生成首页内容
        /// </summary>
        protected void SaveDefault()
        {
            /*******************************************************************************************************
             * 获取参数配置信息,主要是获取模板地址,模板目录等
             * *****************************************************************************************************/
            string cTemplate = this.GetParameter("IndexTemplate", "siteXML").toString("{@dir}/index.html");
            string TemplateDir = string.Format("{0}/{1}", Win.ApplicationPath,
                this.GetParameter("TemplateDir", "siteXML").toString("template"));
            cTemplate = cTemplate.Replace("{@dir}", TemplateDir);
            /*******************************************************************************************************
              * 开始生成PC版本主页
              * *****************************************************************************************************/
            string strReader = Fooke.Release.ReleaseHelper.ReleaseDefault(cTemplate);
            Fooke.Release.ReleaseHelper.Append("~/Index.html", strReader);
            /******************************************************************
             * 首页发布成功
             * ****************************************************************/
            this.ErrorMessage("首页生成成功！");
            Response.End();
        }
        /// <summary>
        /// 生成网站单页面
        /// </summary>
        protected void SaveSingle()
        {
            /*******************************************************************************************************
             * 构建数据查询条件
             * *****************************************************************************************************/
            string SingleID = RequestHelper.GetRequest("SingleID").toString();
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append("select * from Fooke_Single where isHtml = 1");
            if (!string.IsNullOrEmpty(SingleID)) { strTabs.Append(" and Id in (" + SingleID + ")"); }
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strTabs.ToString());
            if (Tab == null) { this.ErrorMessage("没有要生成的单页面！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有要生成的单页面！"); Response.End(); }
            /*******************************************************************************************************
             * 获取系统模板地址
             * *****************************************************************************************************/
            string TemplateDir = string.Format("{0}/{1}", Win.ApplicationPath,
                this.GetParameter("TemplateDir", "siteXML").toString("template"));
            /*******************************************************************************************************
             * 开始生成模版数据
             * *****************************************************************************************************/
            foreach (DataRow SingleRs in Tab.Rows)
            {
                SessionHelper.Add("SingleID", SingleRs["Id"].ToString());
                string strTemplate = SingleRs["cTemplate"].ToString();
                strTemplate = strTemplate.Replace("{@dir}", TemplateDir);

                Fooke.Release.ReleaseHelper ReleaseMaster = new Fooke.Release.ReleaseHelper();
                string strReader = ReleaseMaster.ReleaseSingle(strTemplate, SingleRs);
                ReleaseMaster.AppendText(string.Format("~/html/{0}.html", SingleRs["Identify"]), strReader);
            }
            /*******************************************************************************************************
             * 生成完成
             * *****************************************************************************************************/
            this.ErrorMessage("网页已生成成功！", iSuccess: true);
            Response.End();
        }

        /// <summary>
        /// 生成网站栏目页
        /// </summary>
        protected void SaveList()
        {
            /********************************************************************************************************************
             * 获取每个栏目生成的最大网页数
             * ******************************************************************************************************************/
            int MaxPage = RequestHelper.GetRequest("MaxPage").cInt(10);
            /********************************************************************************************************************
             * 获取并验证需要生成的模型信息
             * ******************************************************************************************************************/
            string ChannelID = RequestHelper.GetRequest("ChannelID").toString();
            if (!string.IsNullOrEmpty(ChannelID))
            {
                var arrTemp = ChannelID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            }
            /********************************************************************************************************************
             * 验证分类ID信息
             * ******************************************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toString();
            if (!string.IsNullOrEmpty(ClassID))
            {
                var arrTemp = ClassID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
                foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            }
            /********************************************************************************************************************
             * 构建查询条件信息
             * ******************************************************************************************************************/
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append(" select Chs.ChannelID,Chs.ChannelName,Chs.UnitName,Chs.BaseName,Chs.Tablename,Chs.intro");
            strTabs.Append(" ,Cls.classId,Cls.ParentID,Cls.className,Cls.Identify,Cls.isDisplay,Cls.SortID,Cls.isChild,Cls.cTemplate");
            strTabs.Append(" ,Cls.Template,Cls.Thumb,Cls.Keywords,Cls.strDesc,Cls.intro,Cls.isOper,Cls.strXML");
            strTabs.Append(" from Fooke_Class as Cls inner join Fooke_Channel as Chs ");
            strTabs.Append(" On Cls.ChannelID = Chs.ChannelID where Cls.isOper = 3");
            if (!string.IsNullOrEmpty(ChannelID)) { strTabs.Append(" and ChannelID in (" + ChannelID + ")"); }
            if (!string.IsNullOrEmpty(ClassID)) { strTabs.Append(" and ClassID in (" + ClassID + ")"); }
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strTabs.ToString());
            if (Tab == null) { this.ErrorMessage("没有要生成的栏目页！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有要生成的栏目页！"); Response.End(); }
            /********************************************************************************************************************
             * 获取系统模板地址
             * ******************************************************************************************************************/
            string TemplateDir = string.Format("{0}/{1}", Win.ApplicationPath,
                this.GetParameter("TemplateDir", "siteXML").toString("template"));
            /********************************************************************************************************************
             * 开始生成模板数据
             * ******************************************************************************************************************/
            Fooke.Release.ReleaseHelper ReleaseMaster = new Fooke.Release.ReleaseHelper();
            /********************************************************************************************************************
             * 开始解析网页内容
             * ******************************************************************************************************************/
            try
            {
                foreach (DataRow cRs in Tab.Rows)
                {
                    try
                    {
                        /************************************************************************************
                         * 保存服务器Session缓存
                         * **********************************************************************************/
                        SessionHelper.Add("ClassID", cRs["classId"].ToString());
                        SessionHelper.Add("session_fenye_list_pagecurrent", "0");

                        string strTemplate = cRs["Template"].ToString();
                        strTemplate = strTemplate.Replace("{@dir}", TemplateDir);
                        /**********************************************************************
                         * 开始生成PC
                         * *********************************************************************/
                        string strReader = ReleaseMaster.ReleaseList(strTemplate, cRs, cRs);
                        ReleaseMaster.AppendText(string.Format("~/html/{0}/default.html", cRs["Identify"]), strReader);
                        int PageCount = SessionHelper.Get("session_fenye_list_pagecount").cInt();
                        if (PageCount >= MaxPage) { PageCount = MaxPage; }
                        for (int k = 1; k <= PageCount; k++)
                        {
                            SessionHelper.Add("session_fenye_list_pagecurrent", k.ToString());
                            strReader = ReleaseMaster.ReleaseList(strTemplate, cRs, cRs);
                            ReleaseMaster.AppendText(strPath: string.Format("~/html/{0}/default_{1}.html", cRs["Identify"], k),
                                strReader: strReader);
                        }
                        /***********************************************************************
                         * 清空服务器Session缓存
                         * *********************************************************************/
                        SessionHelper.Add("session_fenye_list_pagecount", "0");
                        SessionHelper.Add("session_fenye_list_pagecurrent", "0");
                    }
                    catch (Exception err) { this.ErrorMessage(err.Message); }
                }
            }
            catch (Exception err) { this.ErrorMessage(err.Message); Response.End(); }
            /********************************************************************************************************************
             * 输出网页处理结果
             * ******************************************************************************************************************/
            this.ErrorMessage("已开启多线程生成数据！");
            Response.End();

        }

        /// <summary>
        /// 生成网站内容页
        /// </summary>
        protected void SaveContext()
        {
            /**************************************************************************************************
             * 获取请求参数信息
             * ************************************************************************************************/
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            if (ChannelID == "0") { this.ErrorMessage("请求参数错误，请选择一个模型进行发布！"); Response.End(); }
            DataRow ChannelRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindChannel]", new Dictionary<string, object>() {
                {"ChannelID",ChannelID}
            });
            if (ChannelRs == null) { this.ErrorMessage("请求参数错误，你选择的模型不存在！"); Response.End(); }
            else if (ChannelRs["isDisplay"].ToString() != "1") { this.ErrorMessage("模型已关闭,请重试！"); Response.End(); }
            /**************************************************************************************************
             * 获取其他请求参数信息
             * ************************************************************************************************/
            string classId = RequestHelper.GetRequest("classId").toInt();
            string newTop = RequestHelper.GetRequest("newTop").toInt();
            /**************************************************************************************************
             * 构建数据查询语句
             * ************************************************************************************************/
            StringBuilder strTabs = new StringBuilder();
            strTabs.Append("select   ");
            if (newTop != "0") { strTabs.Append(" top " + newTop + ""); }
            strTabs.Append(" Define.*,Cls.Identify,Cls.cTemplate as ShowTemplate,Cls.Template,Cls.Thumb as ClsThumb,Cls.ParentID, ");
            strTabs.Append(" Chs.UnitName,Chs.BaseName,Chs.Tablename,Chs.strKey as ChsKey from " + ChannelRs["Tablename"] + " as Define  ");
            strTabs.Append(" inner join Fooke_Class as Cls On Define.ClassID = Cls.ClassID");
            strTabs.Append(" inner join Fooke_Channel as Chs  On Chs.ChannelID = Define.ChannelID");
            strTabs.Append(" where Define.isDisplay=1 ");
            strTabs.Append(" and exists(select ClassID from Fooke_Class where Fooke_Class.ClassID=Define.ClassID and isOper in (2,3))");
            if (classId != "0") { strTabs.Append(" and Define.ClassID in (" + classId + ")"); }
            if (newTop != "0") { strTabs.Append(" order by define.showid desc"); }
            /**************************************************************************************************
             * 开始查询请求数据
             * ************************************************************************************************/
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strTabs.ToString());
            if (Tab == null) { this.ErrorMessage("没有要发布的文档！"); Response.End(); }
            else if (Tab.Rows.Count <= 0) { this.ErrorMessage("没有要发布的文档！"); Response.End(); }
            /**************************************************************************************************
              * 获取模板存放位置信息
              * ************************************************************************************************/
            string TemplateDir = string.Format("{0}/{1}", Win.ApplicationPath,
                this.GetParameter("TemplateDir", "siteXML").toString("template"));
            /**************************************************************************************************
             * 开始生成模板信息
             * ************************************************************************************************/
            Fooke.Release.ReleaseHelper ReleaseMaster = new Fooke.Release.ReleaseHelper();
            foreach (DataRow Rs in Tab.Rows)
            {
                DataRow classRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                    {"ClassID",Rs["ClassID"].ToString()}
                });
                if (classRs != null)
                {
                    string strTemplate = (Rs["cTemplate"].ToString() == "{$Parents}" ? classRs["cTemplate"].ToString() : Rs["cTemplate"].ToString());
                    strTemplate = strTemplate.Replace("{@dir}", TemplateDir);
                    string strReader = ReleaseMaster.ReleaseContext(strTemplate, Rs, classRs, ChannelRs);
                    string fileName = (Rs["fileName"].ToString().ToLower().Contains("{$showid}") ?
                        string.Format("{0}.aspx", Rs["ShowID"]) :
                        Rs["fileName"].ToString());
                    fileName = "~/html/" + classRs["Identify"] + "/" + fileName;
                    ReleaseMaster.AppendText(fileName, strReader);
                }
            }
            /**************************************************************************************************
            * 输出数据处理结果
            * ************************************************************************************************/
            this.ErrorMessage("文档内容生成成功！", iSuccess: true);
            Response.End();
        }
    }
}