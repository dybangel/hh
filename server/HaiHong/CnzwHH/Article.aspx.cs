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
    public partial class Article : Fooke.Code.AdminHelper
    {
        protected DataRow ChannelRs = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            /*******************************************************************************************************
            * 验证请求模型数据信息
            * *****************************************************************************************************/
            string ChannelID = RequestHelper.GetRequest("ChannelID").toInt();
            if (ChannelID == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            ChannelRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindChannel]", new Dictionary<string, object>() {
                {"ChannelID",ChannelID}
            });
            if (ChannelRs == null) { this.ErrorMessage("模型归类错误，请返回重试！"); Response.End(); }
            if (ChannelRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前模型已经关闭，需要在模型管理中开启才能使用！"); Response.End(); }
            /*******************************************************************************************************
             * 开始执行页面级请求处理
             * *****************************************************************************************************/
            switch (this.strRequest)
            {
                case "save": this.VerificationRole("内容管理"); this.AddSave(); Response.End(); ; break;
                case "add": this.VerificationRole("内容管理"); this.Add(); break;
                case "del": this.VerificationRole("内容管理"); this.SaveDelete(); Response.End(); break;
                case "edit": this.VerificationRole("内容管理"); this.Update(); break;
                case "editsave": this.VerificationRole("内容管理"); this.SaveUpdate(); Response.End(); break;
                case "display": this.VerificationRole("内容管理"); this.SaveDisplay(); Response.End(); break;
                case "saveclass": this.VerificationRole("内容管理"); this.SaveClass(); Response.End(); break;
                case "saveProperty": this.VerificationRole("内容管理"); this.SaveProperty(); Response.End(); break;
                default: this.VerificationRole("内容管理"); this.strDefault(); Response.End(); break;
            }
        }
        /// <summary>
        /// 文章管理
        /// </summary>
        protected void strDefault()
        {
            /*********************************************************************
             * 获取列表搜索参数信息
             * ******************************************************************/
            string SearchType = RequestHelper.GetRequest("SearchType").toString();
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            string classId = RequestHelper.GetRequest("classId").toInt();
            string StartDate = RequestHelper.GetRequest("StartDate").toString();
            string EndDate = RequestHelper.GetRequest("EndDate").toString();
            string isFast = RequestHelper.GetRequest("isFast").ToString();
            /********************************************************************
             * 加载当前模型下的分类信息
             * *****************************************************************/
            string ClassOption = ClassHelper.Options(ChannelRs["ChannelID"].ToString(), "0", classId);
            /********************************************************************
             * 构建网络输出内容
             * *****************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table width=\"99%\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"hback\">");
            strText.Append("<td class=\"Base\" colspan=\"6\">" + ChannelRs["channelName"] + " >> 管理" + ChannelRs["UnitName"] + "</td>");
            strText.Append("</tr>");
            strText.Append("<form id=\"SearchForms\" action=\"Article.aspx\" onsubmit=\"return _doPost(this)\" method=\"get\">");
            strText.Append("<input type=\"hidden\" name=\"action\" value=\"default\" />");
            strText.Append("<input type=\"hidden\" name=\"channelid\" value=\"" + ChannelRs["channelId"] + "\" />");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td colspan=\"5\">");
            strText.Append("<select name=\"SearchType\">");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() { 
                new OptionMode(){Text="搜文档标题",Value="title"},
                new OptionMode(){Text="搜文档分类",Value="classname"}
            }, SearchType));
            strText.Append("</select>");
            strText.Append(" <input type=\"text\" value=\"" + Keywords + "\" placeholder=\"请填写搜索关键词\" class=\"inputtext\" name=\"keywords\" />");
            strText.Append(" 日期 <input type=\"text\" onClick=\"WdatePicker()\" placeholder=\"选择开始日期\" value=\"" + StartDate + "\" name=\"StartDate\" size=\"12\" class=\"inputtext\" />");
            strText.Append(" - <input type=\"text\" onClick=\"WdatePicker()\" placeholder=\"选择结束日期\" value=\"" + EndDate + "\" name=\"EndDate\" size=\"12\" class=\"inputtext\" />");
            strText.Append(" <select name=\"isFast\">");
            strText.Append("<option value=\"\">搜属性</option>");
            strText.Append(FunctionBase.OptionList(new List<OptionMode>() {
                new OptionMode(){Value="isTop",Text="头条"},
                new OptionMode(){Value="isRec",Text="推荐"},
                new OptionMode(){Value="isHot",Text="热门"},
                new OptionMode(){Value="isBook",Text="评论"},
                new OptionMode(){Value="isTo",Text="转向"}
            }, isFast));
            strText.Append("</select>");
            strText.Append(" <input type=\"submit\" class=\"button\" value=\"快速查找\" />");
            strText.Append("</td>");
            strText.Append("<td>");
            strText.Append("<select style=\"width:100px\" name=\"classId\" onchange=\"window.location='?action=default&channelid=" + ChannelRs["channelid"] + "&classid='+this.value\">");
            strText.Append("<option value=\"0\">按栏目查看</option>");
            strText.Append(ClassOption);
            strText.Append("</select>");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</form>");
            strText.Append("<form id=\"iptforms\" action='?action=addlinksave' onsubmit=\"return _doPost(this)\" method='post'>");
            strText.Append("<input type=\"hidden\" name=\"channelid\" value=\"" + ChannelRs["channelId"] + "\" />");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"2%\"><input type=\"checkbox\" operate=\"selectList\" /></td>");
            strText.Append("<td>标题</td>");
            strText.Append("<td width=\"10%\">栏目</td>");
            strText.Append("<td width=\"5%\">状态</td>");
            strText.Append("<td width=\"12%\">日期</td>");
            strText.Append("<td width=\"10%\">操作选项</td>");
            strText.Append("</tr>");
            /*********************************************************************************************************
             * 构建分页查询条件
             * *******************************************************************************************************/
            string Params = " and ChannelID =" + ChannelRs["ChannelID"] + "";
            if (!string.IsNullOrEmpty(Keywords) && !string.IsNullOrEmpty(SearchType))
            {
                switch (SearchType.ToLower())
                {
                    case "title": Params += " and title like '%" + Keywords + "%'"; break;
                    case "classname": Params += " and classname = '" + Keywords + "'"; break;
                    case "author": Params += " and author = '" + Keywords + "'"; break;
                }
            }
            if (classId != "0") { Params += " and classId = " + classId + ""; }
            if (!string.IsNullOrEmpty(StartDate) && VerifyCenter.VerifyDateTime(StartDate)) { Params += " and Addtime>='" + new Fooke.Function.String(StartDate).cDate().ToString("yyyy-MM-dd 00:00:00") + "'"; }
            if (!string.IsNullOrEmpty(EndDate) && VerifyCenter.VerifyDateTime(EndDate)) { Params += " and addtime<='" + new Fooke.Function.String(EndDate).cDate().ToString("yyyy-MM-dd 23:59:59") + "'"; }
            if (!string.IsNullOrEmpty(isFast) && isFast == "isTop") { Params += " and isTop=1"; }
            else if (!string.IsNullOrEmpty(isFast) && isFast == "isRec") { Params += " and isRec=1"; }
            else if (!string.IsNullOrEmpty(isFast) && isFast == "isHot") { Params += " and isHot=1"; }
            else if (!string.IsNullOrEmpty(isFast) && isFast == "isBook") { Params += " and isBook=1"; }
            else if (!string.IsNullOrEmpty(isFast) && isFast == "isTo") { Params += " and isTo=1"; }
            /*********************************************************************************************************
             * 获取分页查询数量
             * *******************************************************************************************************/
            int PageSize = RequestHelper.GetRequest("PageSize").cInt(10);
            /*********************************************************************************************************
             * 构建分页查询语句
             * *******************************************************************************************************/
            PagingSetting PageCenterConfig = new PagingSetting();
            PageCenterConfig.Columns = "ShowID,ShowKey,ChannelID,ChannelName,ClassID,ClassName,Title,Thumb,isDisplay,isTop,isRec,isHot,isBook,isTo,Addtime";
            PageCenterConfig.Params = Params;
            PageCenterConfig.Identify = "ShowID";
            PageCenterConfig.PageSize = PageSize;
            PageCenterConfig.Page = RequestHelper.GetPage();
            PageCenterConfig.Sort = " ShowID Desc";
            PageCenterConfig.Tablename = ChannelRs["Tablename"].ToString();
            string strSQL = PageCenterHelper.MS2005(PageCenterConfig);
            int Record = PageCenterHelper.Record(ChannelRs["Tablename"].ToString(), Params);
            DataTable Tab = DbHelper.Connection.ExecuteDataTable(strSQL);
            /*********************************************************************************************************
             * 循环遍历网页内容信息
             * *******************************************************************************************************/
            foreach (DataRow Rs in Tab.Rows)
            {
                strText.AppendFormat("<tr class=\"hback\">");
                strText.AppendFormat("<td><input type=\"checkbox\" name=\"ShowID\" value=\"{0}\" /></td>", Rs["ShowID"]);
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a target=\"_blank\" href=\"../items/items.aspx?ItemsID={0}&classId={1}\">{2}</a>", Rs["ShowID"], Rs["classId"], Rs["title"]);
                if (!string.IsNullOrEmpty(Rs["Thumb"].ToString())) { strText.AppendFormat("<font color=\"#F00\">[图]</font>"); }
                if (Rs["isTo"].ToString() == "1") { strText.AppendFormat("<font color=\"#F00\">[转]</font>"); }
                if (Rs["isHot"].ToString() == "1") { strText.AppendFormat("<font color=\"#F00\">[热]</font>"); }
                if (Rs["isTop"].ToString() == "1") { strText.AppendFormat("<font color=\"#F00\">[顶]</font>"); }
                if (Rs["isRec"].ToString() == "1") { strText.AppendFormat("<font color=\"#F00\">[推]</font>"); }
                if (Rs["isBook"].ToString() == "1") { strText.AppendFormat("<font color=\"#F00\">[评]</font>"); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td><a href=\"?action=default&channelid={0}&classid={1}\">{2}</a></td>", Rs["channelid"], Rs["classId"], Rs["className"]);
                strText.AppendFormat("<td>");
                if (Rs["isDisplay"].ToString() == "1") { strText.AppendFormat("<a href=\"?action=display&val=0&channelid={0}&ShowID={1}\"><img src=\"images/ico/yes.gif\"/></a>", Rs["channelid"], Rs["ShowID"]); }
                else { strText.AppendFormat("<a href=\"?action=display&val=1&channelid={0}&ShowID={1}\"><img src=\"images/ico/no.gif\"/></a>", Rs["channelid"], Rs["ShowID"]); }
                strText.AppendFormat("</td>");
                strText.AppendFormat("<td>{0}</td>", Rs["addtime"].ToString());
                strText.AppendFormat("<td>");
                strText.AppendFormat("<a href=\"?action=edit&channelid={0}&ShowID={1}\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>", Rs["channelid"], Rs["ShowID"]);
                strText.AppendFormat("<a href=\"?action=del&channelid={0}&ShowID={1}\" title=\"删除\" operate=\"delete\"><img src=\"template/images/ico/delete.png\" /></a>", Rs["channelid"], Rs["ShowID"]);
                strText.AppendFormat("</td>");
                strText.AppendFormat("</tr>");
            }
            /*********************************************************************************************************
             * 构建网页分页语句内容
             * *******************************************************************************************************/
            strText.Append("<tr class=\"pager\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append(PageCenter.Often(Record, PageSize));
            strText.Append("</td>");
            strText.Append("</tr>");
            /*********************************************************************************************************
             * 设置文档属性信息
             * *******************************************************************************************************/
            strText.Append("<tr class=\"operback\">");
            strText.Append("<td colspan=\"6\">");
            strText.Append("<input type=\"button\" class=\"button\" value=\"删除选中\" onclick=\"deleteOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"1\" cmdText=\"display\" value=\"审核(是)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"display\" value=\"审核(否)\" onclick=\"commandOperate(this)\" />");
            strText.Append("<select name=\"Property\">");
            strText.Append("<option value=\"\">属性设置</option>");
            strText.Append("<option value=\"top\">设为置顶</option>");
            strText.Append("<option value=\"dtop\">取消置顶</option>");
            strText.Append("<option value=\"hot\">设为热门</option>");
            strText.Append("<option value=\"dhot\">取消热门</option>");
            strText.Append("<option value=\"rec\">设为推荐</option>");
            strText.Append("<option value=\"drec\">取消推荐</option>");
            strText.Append("<option value=\"book\">允许评论</option>");
            strText.Append("<option value=\"dbook\">取消评论</option>");
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"saveProperty\" value=\"保存设置\" onclick=\"commandOperate(this)\" />");
            strText.Append("<select name=\"classId\">");
            strText.Append("<option value=\"0\">选择栏目</option>");
            strText.Append(ClassOption);
            strText.Append("</select>");
            strText.Append(" <input type=\"button\" class=\"button\" sendText=\"0\" cmdText=\"saveclass\" value=\"移动文档\" onclick=\"commandOperate(this)\" />");
            strText.Append("</td>");
            strText.Append("</tr>");
            strText.Append("</table>");
            strText.Append("</form>");
            /*********************************************************************************************************
             * 输出网页数据内容
             * *******************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/article/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "unitname": strValue = ChannelRs["unitName"].ToString(); break;
                    case "channelid": strValue = ChannelRs["channelid"].ToString(); break;
                    case "list": strValue = strText.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 添加文章
        /// </summary>
        protected void Add()
        {
            /*********************************************************************************************************
             * 输出网页数据内容
             * *******************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/article/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    case "unitname": strValue = ChannelRs["unitName"].ToString(); break;
                    case "channelid": strValue = ChannelRs["channelid"].ToString(); break;
                    case "attr": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Name="isDisplay",Value="1",Text="开启",Checked="1"},
                        new CheckBoxMode(){Name="isRec",Value="1",Text="推荐",Checked="0"},
                        new CheckBoxMode(){Name="isTop",Value="1",Text="头条",Checked="0"},
                        new CheckBoxMode(){Name="isHot",Value="1",Text="热门",Checked="0"},
                        new CheckBoxMode(){Name="isBook",Value="1",Text="评论",Checked="0"}
                    }); break;
                    case "author": strValue = AdminRs["adminName"].ToString(); break;
                    case "class": strValue = ClassHelper.Options(ChannelRs["ChannelID"].ToString(), "0"); break;
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "Thumb",
                        tips = "请上传一张图片资源"
                    }, "0"); break;
                    case "define": strValue = new ColumnsHelper().ShowColumns(ChannelRs["Tablename"].ToString()); break;
                    case "addtime": strValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        protected void Update()
        {
            /********************************************************************************************************
            * 验证请求参数信息
            * ******************************************************************************************************/
            string ShowID = RequestHelper.GetRequest("ShowID").toInt();
            if (ShowID == "0") { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindDocumentChannel]", new Dictionary<string, object>() {
                {"Tablename",ChannelRs["Tablename"].ToString()},
                {"ShowID",ShowID}
            });
            if (Rs == null) { this.ErrorMessage("获取请求数据失败,请重试！"); Response.End(); }
            else if (Rs["ChannelID"].ToString() != ChannelRs["ChannelID"].ToString()) 
            { this.ErrorMessage("获取文档归属模型信息失败,请重试！"); Response.End(); }
            /********************************************************************************************************
            * 输出网页内容信息
            * ******************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/article/edit.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName.ToLower())
                {
                    case "unitname": strValue = ChannelRs["unitName"].ToString(); break;
                    case "channelid": strValue = ChannelRs["channelid"].ToString(); break;
                    case "attr": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>() {
                        new CheckBoxMode(){Name="isDisplay",Value="1",Text="开启",Checked=Rs["isDisplay"].ToString()},
                        new CheckBoxMode(){Name="isRec",Value="1",Text="推荐",Checked=Rs["isRec"].ToString()},
                        new CheckBoxMode(){Name="isTop",Value="1",Text="头条",Checked=Rs["isTop"].ToString()},
                        new CheckBoxMode(){Name="isHot",Value="1",Text="热门",Checked=Rs["isHot"].ToString()},
                        new CheckBoxMode(){Name="isBook",Value="1",Text="评论",Checked=Rs["isBook"].ToString()}
                    }); break;
                    case "class": strValue = ClassHelper.Options(ChannelRs["ChannelID"].ToString(), "0", defaultTxt: Rs["classId"].ToString()); break;
                    case "thumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "Thumb",
                        tips = "请上传一张图片资源",
                        collection = true,
                        selector = true,
                        fileValue = Rs["Thumb"].ToString()
                    }, "0"); break;
                    case "define": strValue = new ColumnsHelper().ShowColumns(ChannelRs["Tablename"].ToString(),Rs); break;
                    default: try { strValue = Rs[funName].ToString(); }
                        catch { }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /*********************************************************************************************************
         * ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
         * 数据处理区域
         * ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
         * *******************************************************************************************************/
        /// <summary>
        /// 添加文档内容
        /// </summary>
        protected void AddSave()
        {
            /**************************************************************************************************************
             * 验证文档栏目信息
             * ************************************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            if (ClassID == "0") { this.ErrorMessage("获取请求参数错误,请选择文档分类!！"); Response.End(); }
            DataRow ClassRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"ClassID",ClassID}
            });
            if (ClassRs == null) { this.ErrorMessage("获取分类信息错误,请重试！"); Response.End(); }
            else if (ClassRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前分类信息未通过审核,请重试！"); Response.End(); }
            /**************************************************************************************************************
             * 验证文档标题
             * ************************************************************************************************************/
            string Title = RequestHelper.GetRequest("Title").toString();
            if (string.IsNullOrEmpty(Title)) { this.ErrorMessage("文档标题不能为空！"); Response.End(); }
            else if (Title.Length <= 1) { this.ErrorMessage("标题字段长度不能少于2个汉字！"); Response.End(); }
            else if (Title.Length >= 60) { this.ErrorMessage("文档标题长度请限制在60个字符内！"); Response.End(); }
            /*************************************************************************************************************
             * 验证标题属性
             * ***********************************************************************************************************/
            string TitleType = RequestHelper.GetRequest("TitleType").toString();
            if (!string.IsNullOrEmpty(TitleType) && TitleType.Length <= 1) { this.ErrorMessage("标题类型长度不能少于2个汉字！"); Response.End(); }
            else if (!string.IsNullOrEmpty(TitleType) && TitleType.Length > 10) { this.ErrorMessage("标题类型长度请限制在10个字符以内！"); Response.End(); }
            string TitleColor = RequestHelper.GetRequest("TitleColor").toString();
            if (!string.IsNullOrEmpty(TitleColor) && TitleColor.Length <= 2) { this.ErrorMessage("标题颜色字段长度不能少于3个字符！"); Response.End(); }
            else if (!string.IsNullOrEmpty(TitleColor) && TitleColor.Length > 10) { this.ErrorMessage("标题颜色长度请限制在10个字符以内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证标题字体大小,字体样式信息
             * ************************************************************************************************************/
            string FontSize = RequestHelper.GetRequest("FontSize").toInt();
            string FontModel = RequestHelper.GetRequest("FontModel").toInt();
            /**************************************************************************************************************
             * 获取文章来源,文章作者等内容信息
             * ************************************************************************************************************/
            string Source = RequestHelper.GetRequest("source").toString("原创");
            if (string.IsNullOrEmpty(Source)) { this.ErrorMessage("获取文档来源信息错误,请重试！"); Response.End(); }
            else if (Source.Length >= 12) { this.ErrorMessage("文档来源请保持在20个字符以内！"); Response.End(); }
            string Author = RequestHelper.GetRequest("Author").toString("佚名");
            if (string.IsNullOrEmpty(Author)) { this.ErrorMessage("获取文档作者信息失败,请重试！"); Response.End(); }
            else if (Author.Length >= 12) { this.ErrorMessage("文档作者字段长度请限制在12个字符内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证图文信息,关键词描述等
             * ************************************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString();
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length <= 10) { this.ErrorMessage("图片文件格式地址错误,请重试！"); Response.End(); }
            else if (Thumb.Length > 120) { this.ErrorMessage("图片资源地址长度请限制在150个字符以内！否则无法保存！"); Response.End(); }
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            if (!string.IsNullOrEmpty(Keywords) && Keywords.Length <= 1) { this.ErrorMessage("关键词不能少于1个汉字！"); Response.End(); }
            else if (!string.IsNullOrEmpty(Keywords) && Keywords.Length <= 30) { this.ErrorMessage("关键词长度不允许超过50个字符！"); Response.End(); }
            string strDescrption = RequestHelper.GetRequest("strDescrption").toString();
            if (!string.IsNullOrEmpty(strDescrption) && strDescrption.Length <= 1) { this.ErrorMessage("描述内容信息不能少于1个汉字!"); Response.End(); }
            else if (strDescrption.Length >= 200) { this.ErrorMessage("描述内容信息长度请限制在200个汉字内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证跳转地址信息
             * ************************************************************************************************************/
            string toUrl = RequestHelper.GetRequest("toUrl").toString();
            if (toUrl.Length != 0 && toUrl.Length <= 7) { this.ErrorMessage("转向Url地址格式错误,请重试！"); Response.End(); }
            else if (toUrl.Length != 0 && !toUrl.ToLower().Contains("http")) { this.ErrorMessage("请填写完整的Url跳转地址！"); Response.End(); }
            else if (toUrl.Length != 0 && toUrl.Length > 120) { this.ErrorMessage("Url跳转地址长度请限制在120个汉字以内！"); Response.End(); }
            string isTo = (toUrl.Length != 0 ? "1" : "0");
            /**************************************************************************************************************
             * 验证内容发布时间
             * ************************************************************************************************************/
            string Addtime = RequestHelper.GetRequest("Addtime").ToString();
            if (string.IsNullOrEmpty(Addtime)) { this.ErrorMessage("请选择文档发布日期！"); Response.End(); }
            else if (!Addtime.isDate()) { this.ErrorMessage("文档发布日期格式错误！"); Response.End(); }
            /**************************************************************************************************************
             * 验证网页静态文件地址,文档名称
             * ************************************************************************************************************/
            string Filename = RequestHelper.GetRequest("fileName").toString();
            if (string.IsNullOrEmpty(Filename)) { this.ErrorMessage("文档文件名称不能为空！"); Response.End(); }
            else if (Filename.Length <= 3) { this.ErrorMessage("文档名称不能少于4个字符！"); Response.End(); }
            else if (Filename.Length > 20) { this.ErrorMessage("文件名称长度不允许超过20个字符！"); Response.End(); }
            string cTemplate = RequestHelper.GetRequest("cTemplate").toString();
            if (string.IsNullOrEmpty(cTemplate)) { this.ErrorMessage("文档调用模版不能为空！"); Response.End(); }
            else if (cTemplate.Length <= 3) { this.ErrorMessage("模板地址长度不能少于4个字符！"); Response.End(); }
            else if (cTemplate.Length > 36) { this.ErrorMessage("模版文件长度不允许超过36个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 获取文档属性信息
             * ************************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string isTop = RequestHelper.GetRequest("isTop").toInt();
            string isRec = RequestHelper.GetRequest("isRec").toInt();
            string isHot = RequestHelper.GetRequest("isHot").toInt();
            string isBook = RequestHelper.GetRequest("isBook").toInt();
            string Hits = RequestHelper.GetRequest("Hits").toInt();
            string Star = RequestHelper.GetRequest("Star").toInt();
            /**************************************************************************************************************
             * 获取文章内容信息
             * ************************************************************************************************************/
            string Content = RequestHelper.GetBodyContent("Content");
            /**************************************************************************************************************
             * 验证自定义字段信息
             * ************************************************************************************************************/
            string isDefine = RequestHelper.GetRequest("isDefine").ToString();
            if (string.IsNullOrEmpty(isDefine)) { this.ErrorMessage("获取自定义参数信息失败,请重试！"); Response.End(); }
            else if (isDefine != "true" && isDefine != "false") { this.ErrorMessage("获取自定义参数信息失败,请重试！"); Response.End(); }
            else if (isDefine == "true")
            {
                try
                {
                    new ColumnsHelper().Verification(ChannelRs["TableName"].ToString(), (errMessage) =>
                    {
                        this.ErrorMessage(errMessage); Response.End();
                    });
                }
                catch { }
            }
            /**************************************************************************************************************
             * 抓取文档缩略图信息
             * ************************************************************************************************************/
            string SaveThumbPic = RequestHelper.GetRequest("SaveThumbPic").toInt();
            if (string.IsNullOrEmpty(Thumb) && !string.IsNullOrEmpty(Content) && SaveThumbPic == "1")
            { Thumb = FunctionCenter.GetThumbPic(Content, 0); }
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length <= 7) { this.ErrorMessage("缩略图地址长度不能少于7个字符！"); Response.End(); }
            else if (!string.IsNullOrEmpty(Thumb) && Thumb.Length >= 120)
            { this.ErrorMessage("缩略图地址长度请限制在120个字符内！"); Response.End(); }
            /**************************************************************************************************************
             * 截取文档描述内容信息
             * ************************************************************************************************************/
            string isDescrption = RequestHelper.GetRequest("isDescrption").toInt();
            if (string.IsNullOrEmpty(strDescrption) && isDescrption == "1") { strDescrption = FunctionCenter.GetDescrption(Content, 120); }
            if (!string.IsNullOrEmpty(strDescrption) && strDescrption.Length > 200)
            { this.ErrorMessage("文档描述内容长度请限制在200汉字以内！"); Response.End(); }
            /**************************************************************************************************************
             * 生成文档标识内容信息
             * ************************************************************************************************************/
            string ShowKey = string.Format("模型文档-|-|-{0}-|-|-{1}-|-|-{2}-|-|-{3}",
                ChannelRs["ChannelID"].ToString(), ClassRs["ClassID"].ToString(),
                Guid.NewGuid().ToString(), DateTime.Now.ToString("yyyyMMddHHmmss"));
            ShowKey = new Fooke.Function.String(ShowKey).ToMD5().Substring(0, 24).ToUpper();
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindArticle]", new Dictionary<string, object>() {
                {"ShowKey",ShowKey}
            });
            if (cRs != null) { this.ErrorMessage("服务器系统繁忙,请稍后重试！"); Response.End(); }
            /************************************************************************
             * 开始保存数据,保存数据基本信息
             * **********************************************************************/
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["Tablename"] = ChannelRs["Tablename"].ToString();
            thisDictionary["ShowKey"] = ShowKey;
            thisDictionary["ChannelID"] = ChannelRs["ChannelID"].ToString();
            thisDictionary["ChannelName"] = ChannelRs["ChannelName"].ToString();
            thisDictionary["ClassID"] = ClassRs["ClassID"].ToString();
            thisDictionary["ClassName"] = ClassRs["ClassName"].ToString();
            thisDictionary["Title"] = Title;
            thisDictionary["TitleType"] = TitleType;
            thisDictionary["TitleColor"] = TitleColor;
            thisDictionary["Fontsize"] = FontSize;
            thisDictionary["FontModel"] = FontModel;
            thisDictionary["Thumb"] = Thumb;
            thisDictionary["isDisplay"] = isDisplay;
            thisDictionary["isTop"] = isTop;
            thisDictionary["isRec"] = isRec;
            thisDictionary["isHot"] = isHot;
            thisDictionary["isBook"] = isBook;
            thisDictionary["isTo"] = isTo;
            thisDictionary["toUrl"] = toUrl;
            thisDictionary["Keywords"] = Keywords;
            thisDictionary["strDescrption"] = strDescrption;
            thisDictionary["Content"] = Content;
            thisDictionary["Source"] = Source;
            thisDictionary["Author"] = Author;
            thisDictionary["cTemplate"] = cTemplate;
            thisDictionary["Filename"] = Filename;
            thisDictionary["Hits"] = Hits;
            thisDictionary["Star"] = Star;
            thisDictionary["Addtime"] = new Fooke.Function.String(Addtime).cDate().ToString("yyyy-MM-dd HH:mm:ss");
            DataRow ItemsRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveArticle]", thisDictionary);
            if (ItemsRs == null) { this.ErrorMessage("数据处理过程中发生未知错误,请重试！"); Response.End(); }
            /**********************************************************************************************************
             * 保存用户自定义字段
             * *******************************************************************************************************/
            try
            {
                if (isDefine == "true")
                {
                    new ColumnsHelper().SaveColumns(ChannelRs["Tablename"].ToString(), (oDictionary) =>
                    {
                        if (oDictionary != null && oDictionary.Count > 0)
                        {
                            DbHelper.Connection.Update(tablename: ChannelRs["Tablename"].ToString(),
                                dictionary: oDictionary,
                                Params: " and ShowID =" + ItemsRs["ShowID"] + "");
                        }
                    }, (err) => { this.ErrorMessage(err); Response.End(); });
                }
            }
            catch { }
            /****************************************************************************
             * 数据保存成功，执行跳转
             * ***************************************************************************/
            this.ConfirmMessage("数据保存成功，点击确定将继续停留在当前界面!", falseUrl: "?action=default&channelId=" + ChannelRs["channelid"] + "");
            Response.End();
        }
        /// <summary>
        /// 保存文档
        /// </summary>
        protected void SaveUpdate()
        {
            /*********************************************************************************************************
             * 获取文档信息
             * *******************************************************************************************************/
            string ShowID = RequestHelper.GetRequest("ShowID").toInt();
            if (ShowID == "0") { this.ErrorMessage("请求参数错误，请返回重试！"); Response.End(); }
            DataRow ItemsRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindDocumentChannel]", new Dictionary<string, object>() {
                {"Tablename",ChannelRs["Tablename"].ToString()},
                {"ShowID",ShowID}
            });
            if (ItemsRs == null) { this.ErrorMessage("对不起,你查找的数据不存在！"); Response.End(); }
            else if (ItemsRs["ChannelID"].ToString() != ChannelRs["ChannelID"].ToString())
            { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); }
            /**************************************************************************************************************
             * 验证文档栏目信息
             * ************************************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            if (ClassID == "0") { this.ErrorMessage("获取请求参数错误,请选择文档分类!！"); Response.End(); }
            DataRow ClassRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"ClassID",ClassID}
            });
            if (ClassRs == null) { this.ErrorMessage("获取分类信息错误,请重试！"); Response.End(); }
            else if (ClassRs["isDisplay"].ToString() != "1") { this.ErrorMessage("当前分类信息未通过审核,请重试！"); Response.End(); }
            /**************************************************************************************************************
             * 验证文档标题
             * ************************************************************************************************************/
            /**************************************************************************************************************
             * 验证文档标题
             * ************************************************************************************************************/
            string Title = RequestHelper.GetRequest("Title").toString();
            if (string.IsNullOrEmpty(Title)) { this.ErrorMessage("文档标题不能为空！"); Response.End(); }
            else if (Title.Length <= 1) { this.ErrorMessage("标题字段长度不能少于2个汉字！"); Response.End(); }
            else if (Title.Length >= 60) { this.ErrorMessage("文档标题长度请限制在60个字符内！"); Response.End(); }
            /*************************************************************************************************************
             * 验证标题属性
             * ***********************************************************************************************************/
            string TitleType = RequestHelper.GetRequest("TitleType").toString();
            if (!string.IsNullOrEmpty(TitleType) && TitleType.Length <= 1) { this.ErrorMessage("标题类型长度不能少于2个汉字！"); Response.End(); }
            else if (!string.IsNullOrEmpty(TitleType) && TitleType.Length > 10) { this.ErrorMessage("标题类型长度请限制在10个字符以内！"); Response.End(); }
            string TitleColor = RequestHelper.GetRequest("TitleColor").toString();
            if (!string.IsNullOrEmpty(TitleColor) && TitleColor.Length <= 2) { this.ErrorMessage("标题颜色字段长度不能少于3个字符！"); Response.End(); }
            else if (!string.IsNullOrEmpty(TitleColor) && TitleColor.Length > 10) { this.ErrorMessage("标题颜色长度请限制在10个字符以内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证标题字体大小,字体样式信息
             * ************************************************************************************************************/
            string FontSize = RequestHelper.GetRequest("FontSize").toInt();
            string FontModel = RequestHelper.GetRequest("FontModel").toInt();
            /**************************************************************************************************************
             * 获取文章来源,文章作者等内容信息
             * ************************************************************************************************************/
            string Source = RequestHelper.GetRequest("source").toString("原创");
            if (string.IsNullOrEmpty(Source)) { this.ErrorMessage("获取文档来源信息错误,请重试！"); Response.End(); }
            else if (Source.Length >= 12) { this.ErrorMessage("文档来源请保持在20个字符以内！"); Response.End(); }
            string Author = RequestHelper.GetRequest("Author").toString("佚名");
            if (string.IsNullOrEmpty(Author)) { this.ErrorMessage("获取文档作者信息失败,请重试！"); Response.End(); }
            else if (Author.Length >= 12) { this.ErrorMessage("文档作者字段长度请限制在12个字符内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证图文信息,关键词描述等
             * ************************************************************************************************************/
            string Thumb = RequestHelper.GetRequest("Thumb").toString();
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length <= 10) { this.ErrorMessage("图片文件格式地址错误,请重试！"); Response.End(); }
            else if (Thumb.Length > 120) { this.ErrorMessage("图片资源地址长度请限制在150个字符以内！否则无法保存！"); Response.End(); }
            string Keywords = RequestHelper.GetRequest("Keywords").toString();
            if (!string.IsNullOrEmpty(Keywords) && Keywords.Length <= 1) { this.ErrorMessage("关键词不能少于1个汉字！"); Response.End(); }
            else if (!string.IsNullOrEmpty(Keywords) && Keywords.Length <= 30) { this.ErrorMessage("关键词长度不允许超过50个字符！"); Response.End(); }
            string strDescrption = RequestHelper.GetRequest("strDescrption").toString();
            if (!string.IsNullOrEmpty(strDescrption) && strDescrption.Length <= 1) { this.ErrorMessage("描述内容信息不能少于1个汉字!"); Response.End(); }
            else if (strDescrption.Length >= 200) { this.ErrorMessage("描述内容信息长度请限制在200个汉字内！"); Response.End(); }
            /**************************************************************************************************************
             * 验证跳转地址信息
             * ************************************************************************************************************/
            string toUrl = RequestHelper.GetRequest("toUrl").toString();
            if (toUrl.Length != 0 && toUrl.Length <= 7) { this.ErrorMessage("转向Url地址格式错误,请重试！"); Response.End(); }
            else if (toUrl.Length != 0 && !toUrl.ToLower().Contains("http")) { this.ErrorMessage("请填写完整的Url跳转地址！"); Response.End(); }
            else if (toUrl.Length != 0 && toUrl.Length > 120) { this.ErrorMessage("Url跳转地址长度请限制在120个汉字以内！"); Response.End(); }
            string isTo = (toUrl.Length != 0 ? "1" : "0");
            /**************************************************************************************************************
             * 验证内容发布时间
             * ************************************************************************************************************/
            string Addtime = RequestHelper.GetRequest("Addtime").ToString();
            if (string.IsNullOrEmpty(Addtime)) { this.ErrorMessage("请选择文档发布日期！"); Response.End(); }
            else if (!Addtime.isDate()) { this.ErrorMessage("文档发布日期格式错误！"); Response.End(); }
            /**************************************************************************************************************
             * 验证网页静态文件地址,文档名称
             * ************************************************************************************************************/
            string Filename = RequestHelper.GetRequest("fileName").toString();
            if (string.IsNullOrEmpty(Filename)) { this.ErrorMessage("文档文件名称不能为空！"); Response.End(); }
            else if (Filename.Length <= 3) { this.ErrorMessage("文档名称不能少于4个字符！"); Response.End(); }
            else if (Filename.Length > 20) { this.ErrorMessage("文件名称长度不允许超过20个字符！"); Response.End(); }
            string cTemplate = RequestHelper.GetRequest("cTemplate").toString();
            if (string.IsNullOrEmpty(cTemplate)) { this.ErrorMessage("文档调用模版不能为空！"); Response.End(); }
            else if (cTemplate.Length <= 3) { this.ErrorMessage("模板地址长度不能少于4个字符！"); Response.End(); }
            else if (cTemplate.Length > 36) { this.ErrorMessage("模版文件长度不允许超过36个字符！"); Response.End(); }
            /**************************************************************************************************************
             * 获取文档属性信息
             * ************************************************************************************************************/
            string isDisplay = RequestHelper.GetRequest("isDisplay").toInt();
            string isTop = RequestHelper.GetRequest("isTop").toInt();
            string isRec = RequestHelper.GetRequest("isRec").toInt();
            string isHot = RequestHelper.GetRequest("isHot").toInt();
            string isBook = RequestHelper.GetRequest("isBook").toInt();
            string Hits = RequestHelper.GetRequest("Hits").toInt();
            string Star = RequestHelper.GetRequest("Star").toInt();
            /**************************************************************************************************************
             * 获取文章内容信息
             * ************************************************************************************************************/
            string Content = RequestHelper.GetBodyContent("Content");
            /**************************************************************************************************************
             * 验证自定义字段信息
             * ************************************************************************************************************/
            string isDefine = RequestHelper.GetRequest("isDefine").ToString();
            if (string.IsNullOrEmpty(isDefine)) { this.ErrorMessage("获取自定义参数信息失败,请重试！"); Response.End(); }
            else if (isDefine != "true" && isDefine != "false") { this.ErrorMessage("获取自定义参数信息失败,请重试！"); Response.End(); }
            else if (isDefine == "true")
            {
                try
                {
                    new ColumnsHelper().Verification(ChannelRs["TableName"].ToString(), (errMessage) =>
                    {
                        this.ErrorMessage(errMessage); Response.End();
                    });
                }
                catch { }
            }
            /**************************************************************************************************************
             * 抓取文档缩略图信息
             * ************************************************************************************************************/
            string SaveThumbPic = RequestHelper.GetRequest("SaveThumbPic").toInt();
            if (string.IsNullOrEmpty(Thumb) && !string.IsNullOrEmpty(Content) && SaveThumbPic == "1")
            { Thumb = FunctionCenter.GetThumbPic(Content, 0); }
            if (!string.IsNullOrEmpty(Thumb) && Thumb.Length <= 7) { this.ErrorMessage("缩略图地址长度不能少于7个字符！"); Response.End(); }
            else if (!string.IsNullOrEmpty(Thumb) && Thumb.Length >= 120)
            { this.ErrorMessage("缩略图地址长度请限制在120个字符内！"); Response.End(); }
            /**************************************************************************************************************
             * 截取文档描述内容信息
             * ************************************************************************************************************/
            string isDescrption = RequestHelper.GetRequest("isDescrption").toInt();
            if (string.IsNullOrEmpty(strDescrption) && isDescrption == "1") { strDescrption = FunctionCenter.GetDescrption(Content, 120); }
            if (!string.IsNullOrEmpty(strDescrption) && strDescrption.Length > 200)
            { this.ErrorMessage("文档描述内容长度请限制在200汉字以内！"); Response.End(); }
            /**************************************************************************************************************
             * 保存数据内容信息
             * ************************************************************************************************************/
            Dictionary<string, object> oDictionary = new Dictionary<string, object>();
            oDictionary["Tablename"] = ChannelRs["Tablename"].ToString();
            oDictionary["ShowID"] = ItemsRs["ShowID"].ToString();
            oDictionary["ShowKey"] = ItemsRs["ShowKey"].ToString();
            oDictionary["ChannelID"] = ChannelRs["ChannelID"].ToString();
            oDictionary["ChannelName"] = ChannelRs["ChannelName"].ToString();
            oDictionary["ClassID"] = ClassRs["ClassID"].ToString();
            oDictionary["ClassName"] = ClassRs["ClassName"].ToString();
            oDictionary["Title"] = Title;
            oDictionary["TitleType"] = TitleType;
            oDictionary["TitleColor"] = TitleColor;
            oDictionary["Fontsize"] = FontSize;
            oDictionary["FontModel"] = FontModel;
            oDictionary["Thumb"] = Thumb;
            oDictionary["isDisplay"] = isDisplay;
            oDictionary["isTop"] = isTop;
            oDictionary["isRec"] = isRec;
            oDictionary["isHot"] = isHot;
            oDictionary["isBook"] = isBook;
            oDictionary["isTo"] = isTo;
            oDictionary["toUrl"] = toUrl;
            oDictionary["Keywords"] = Keywords;
            oDictionary["strDescrption"] = strDescrption;
            oDictionary["Content"] = Content;
            oDictionary["Source"] = Source;
            oDictionary["Author"] = Author;
            oDictionary["cTemplate"] = cTemplate;
            oDictionary["Filename"] = Filename;
            oDictionary["Hits"] = Hits;
            oDictionary["Star"] = Star;
            oDictionary["Addtime"] = Addtime;
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_SaveArticle]", oDictionary);
            if (cRs == null) { this.ErrorMessage("数据处理过程中发生未知错误,请重试！"); Response.End(); }
            /**********************************************************************************************************
             * 保存用户自定义字段
             * *******************************************************************************************************/
            try
            {
                if (isDefine == "true")
                {
                    new ColumnsHelper().SaveColumns(ChannelRs["Tablename"].ToString(), (thisDictionary) =>
                    {
                        if (thisDictionary != null && thisDictionary.Count > 0)
                        {
                            DbHelper.Connection.Update(tablename: ChannelRs["Tablename"].ToString(),
                                dictionary: thisDictionary,
                                Params: " and ShowID =" + ItemsRs["ShowID"] + "");
                        }
                    }, (err) => { this.ErrorMessage(err); Response.End(); });
                }
            }
            catch { }
            /****************************************************************************
             * 数据保存成功，执行跳转
             * ***************************************************************************/
            this.ConfirmMessage("数据保存成功，点击确定将继续停留在当前界面!"+isDefine, falseUrl: "?action=default&channelId=" + ChannelRs["channelid"] + "");
            Response.End();
        }
        /// <summary>
        /// 删除用户数据
        /// </summary>
        protected void SaveDelete()
        {
            /***********************************************************************************************
            * 验证参数合法性
            * *********************************************************************************************/
            string strList = RequestHelper.GetRequest("ShowID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable thisTab = DbHelper.Connection.FindTable(ChannelRs["Tablename"].ToString(), Params: " and ShowID in (" + strList + ") and isDisplay=0");
            if (thisTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /*********************************************************************************************
            * 获取数据信息
            * ********************************************************************************************/
            DbHelper.Connection.Delete(ChannelRs["Tablename"].ToString(),
                Params: " and ShowID in (" + strList + ") and isDisplay=0");
            /*********************************************************************************************
            * 返回数据处理结果
            * ********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 审核用户信息
        /// </summary>
        protected void SaveDisplay()
        {
            /*********************************************************************************************
            * 获取处理数据
            * ********************************************************************************************/
            string strList = RequestHelper.GetRequest("ShowID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable thisTab = DbHelper.Connection.FindTable(ChannelRs["Tablename"].ToString(), Params: " and ShowID in (" + strList + ")");
            if (thisTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /*********************************************************************************************
            * 获取数据值并且保存
            * ********************************************************************************************/
            string strValue = RequestHelper.GetRequest("val").toInt();
            DbHelper.Connection.Update(ChannelRs["Tablename"].ToString(), dictionary: new Dictionary<string, string>() {
                {"isDisplay",strValue}
            }, Params: " and ShowID in (" + strList + ")");
            /*********************************************************************************************
            * 返回处理结果
            * ********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 设置文档属性
        /// </summary>
        protected void SaveProperty()
        {
            /*********************************************************************************************
            * 获取处理数据
            * ********************************************************************************************/
            string strList = RequestHelper.GetRequest("ShowID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable thisTab = DbHelper.Connection.FindTable(ChannelRs["Tablename"].ToString(), Params: " and ShowID in (" + strList + ")");
            if (thisTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /*********************************************************************************************
            * 构建处理信息
            * ********************************************************************************************/
            string Property = RequestHelper.GetRequest("Property").ToString();
            if (string.IsNullOrEmpty(Property)) { this.ErrorMessage("请选择需要设置的文档属性！"); Response.End(); }
            else if (Property == "0") { this.ErrorMessage("参数错误，请选择一个属性进行设置！"); Response.End(); }
            else if (Property.Length <= 2) { this.ErrorMessage("获取设置文档属性信息失败,请重试！"); Response.End(); }
            else if (Property.Length >= 6) { this.ErrorMessage("获取设置文档属性信息失败,请重试！"); Response.End(); }
            /*********************************************************************************************
            * 开始保存请求数据信息
            * ********************************************************************************************/
            Dictionary<string, string> thisDictionary = new Dictionary<string, string>();
            if (Property == "top") { thisDictionary["isTop"] = "1"; }
            else if (Property == "dtop") { thisDictionary["isTop"] = "0"; }
            else if (Property == "hot") { thisDictionary["isHot"] = "1"; }
            else if (Property == "dhot") { thisDictionary["isHot"] = "0"; }
            else if (Property == "rec") { thisDictionary["isRec"] = "1"; }
            else if (Property == "drec") { thisDictionary["isRec"] = "0"; }
            else if (Property == "book") { thisDictionary["isBook"] = "1"; }
            else if (Property == "dbook") { thisDictionary["isBook"] = "0"; }
            /*********************************************************************************************
            * 开始保存数据
            * ********************************************************************************************/
            if (thisDictionary.Count <= 0) { this.ErrorMessage("发生未知错误，请返回重试！"); Response.End(); }
            DbHelper.Connection.Update(ChannelRs["Tablename"].ToString(), thisDictionary, Params: " and ShowID in (" + strList + ")");
            /*********************************************************************************************
            * 输出处理结果
            * ********************************************************************************************/
            this.History();
            Response.End();
        }
        /// <summary>
        /// 开始移动文档
        /// </summary>
        protected void SaveClass()
        {
            /*********************************************************************************************
            * 获取处理数据
            * ********************************************************************************************/
            string strList = RequestHelper.GetRequest("ShowID").ToString();
            if (string.IsNullOrEmpty(strList)) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            var arrTemp = strList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTemp.Length <= 0) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            if (arrTemp.Length >= 100) { this.ErrorMessage("获取请求参数错误,请重试！"); Response.End(); }
            foreach (var strIn in arrTemp) { if (!strIn.isInt()) { this.ErrorMessage("发生未知错误,请重试！"); Response.End(); } }
            DataTable thisTab = DbHelper.Connection.FindTable(ChannelRs["Tablename"].ToString(), Params: " and ShowID in (" + strList + ")");
            if (thisTab == null) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            else if (thisTab.Rows.Count <= 0) { this.ErrorMessage("没有需要处理的数据！"); Response.End(); }
            /*********************************************************************************************
            * 获取分类信息
            * ********************************************************************************************/
            string ClassID = RequestHelper.GetRequest("ClassID").toInt();
            if (ClassID == "0") { this.ErrorMessage("请选择要移动到的栏目！"); Response.End(); }
            DataRow cRs = DbHelper.Connection.ExecuteFindRow("[Stored_FindClass]", new Dictionary<string, object>() {
                {"ClassID",ClassID}
            });
            if (cRs == null) { this.ErrorMessage("对不起，你查找的栏目分类不存在！"); Response.End(); }
            /*********************************************************************************************
             * 开始保存数据
             * ********************************************************************************************/
            DbHelper.Connection.Update(ChannelRs["Tablename"].ToString(), dictionary: new Dictionary<string, string>() {
                {"ClassID",cRs["ClassID"].ToString()},
                {"ClassName",cRs["ClassName"].ToString()}
            }, Params: " and ShowID in (" + strList + ")");
            /*********************************************************************************************
            * 输出处理结果
            * ********************************************************************************************/
            this.History();
            Response.End();
        }
    }
}