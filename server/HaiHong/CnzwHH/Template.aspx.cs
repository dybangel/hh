using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fooke.Code;
using Fooke.Function;
using System.IO;
using System.Text;
namespace Fooke.Web.Admin
{
    public partial class Template : Fooke.Code.AdminHelper
    {
        protected string targetText = string.Empty;
        protected string thisDir = string.Empty;
        protected string tempDir = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            /****************************************************************
             * 获取当前页面所需要的系统配置参数
             * **************************************************************/
            thisDir = RequestHelper.GetRequest("dir").toString();
            tempDir = Win.ApplicationPath + "/" + this.GetParameter("templateDir").toString("template");
            if (string.IsNullOrEmpty(thisDir)) { thisDir = tempDir; }
            if (!System.IO.Directory.Exists(this.MapPath(thisDir))) { System.IO.Directory.CreateDirectory(this.MapPath(thisDir)); }
            targetText = RequestHelper.GetRequest("target").toString();
            if (string.IsNullOrEmpty(targetText)) { targetText = "Template"; }
            /****************************************************************
             * 开始执行页面请求
             * **************************************************************/
            switch (this.strRequest)
            {
                case "default": this.strDefault(); Response.End(); break;
                case "deld": this.DeleteDirectory(); Response.End(); break;
                case "delf": this.DeleteFile(); Response.End(); break;
                case "addf": this.AddDirectory(); Response.End(); break;
                case "adddsave": this.AddDirectorySave(); Response.End(); break;
                case "editf": this.UpdateFile(); Response.End(); break;
                case "addfsave": this.AddFileSave(); Response.End(); break;
                case "stor": strSelector(); Response.End(); break;
                default: this.strDefault(); Response.End(); break;
            }
        }

        protected void strSelector() {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table width=\"99%\" cellpadding=\"3\" class=\"table\" cellspacing=\"1\" border=\"0\">");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td>名称</td>");
            strBuilder.Append("<td>文件类型</td>");
            strBuilder.Append("<td>大小</td>");
            strBuilder.Append("<td width=\"120\">选项</td>");
            strBuilder.Append("</tr>");
            try
            {
                DirectoryInfo direct = new DirectoryInfo(this.MapPath(this.thisDir));
                foreach (DirectoryInfo dir in direct.GetDirectories())
                {
                    strBuilder.Append("<tr title=\"双击打开目录\" ondblclick=\"window.location='?action=stor&dir=" + thisDir + "/" + dir.Name + "&target=" + targetText + "'\" class=\"hback\">");
                    strBuilder.Append("<td>" + dir.Name + "</td>");
                    strBuilder.Append("<td>文件夹</td>");
                    strBuilder.Append("<td>0</td>");
                    strBuilder.Append("<td>");
                    strBuilder.Append("<a href=\"?action=list&dir=" + thisDir + "/" + dir.Name + "&target=" + targetText + "\" title=\"打开\"><img src=\"template/images/ico/chart.png\" /></a>");
                    strBuilder.Append("<a href=\"?action=editf&dir=" + thisDir + "/" + dir.Name + "&target=" + targetText + "\" title=\"添加子文件\"><img src=\"template/images/ico/add.png\" /></a>");
                    strBuilder.Append("<a operate=\"delete\" operate=\"delete\" href=\"?action=deld&dir=" + thisDir + "&name=" + dir.Name + "&target=" + targetText + "\" title=\"删除\"><img src=\"template/images/ico/delete.png\" /></a>");
                    strBuilder.Append("</td>");
                    strBuilder.Append("</tr>");
                }
                foreach (FileInfo cFile in direct.GetFiles())
                {
                    string tempTxt = string.Empty;
                    try { tempTxt = thisDir + "/" + cFile.Name; tempTxt = tempTxt.Replace(tempDir, "{@dir}"); }
                    catch { }
                    strBuilder.Append("<tr title=\"双击选择模版\" operate=\"dblselected\" url=\"" + tempTxt + "\" class=\"hback\">");
                    strBuilder.Append("<td>" + cFile.Name + "</td>");
                    strBuilder.Append("<td>" + cFile.Extension.ToString() + "</td>");
                    strBuilder.Append("<td>" + cFile.Length + "</td>");
                    strBuilder.Append("<td>");
                    strBuilder.Append("<a href=\"?action=editf&dir=" + thisDir + "&name=" + cFile.Name + "&target=" + targetText + "\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>");
                    strBuilder.Append("<a operate=\"delete\" href=\"?action=delf&dir=" + thisDir + "&name=" + cFile.Name + "&target=" + targetText + "\" title=\"删除\"><img src=\"template/images/ico/delete.png\" /></a>");
                    strBuilder.Append("</td>");
                    strBuilder.Append("</tr>");
                }
            }
            catch { }
            strBuilder.Append("</table>");
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/template/selector.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "target": strValue = targetText; break;
                    case "tempdir": strValue = tempDir; break;
                    case "thisdir": strValue = thisDir; break;
                    case "Directory": strValue = this.GetDirectory(); break;
                    case "list": strValue = strBuilder.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        protected void strDefault()
        {
            StringBuilder strBuilder = new StringBuilder();
            //strBuilder.Append("<table width=\"99%\" cellpadding=\"3\" class=\"table\" cellspacing=\"1\" border=\"0\">");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td>名称</td>");
            strBuilder.Append("<td>文件类型</td>");
            strBuilder.Append("<td>大小</td>");
            strBuilder.Append("<td width=\"120\">选项</td>");
            strBuilder.Append("</tr>");
            try
            {
                DirectoryInfo direct = new DirectoryInfo(this.MapPath(this.thisDir));
                foreach (DirectoryInfo dir in direct.GetDirectories())
                {
                    strBuilder.Append("<tr title=\"双击打开目录\" ondblclick=\"window.location='?action=default&dir=" + thisDir + "/" + dir.Name + "&target=" + targetText + "'\" class=\"hback\">");
                    strBuilder.Append("<td>" + dir.Name + "</td>");
                    strBuilder.Append("<td>文件夹</td>");
                    strBuilder.Append("<td>0</td>");
                    strBuilder.Append("<td>");
                    strBuilder.Append("<a href=\"?action=list&dir=" + thisDir + "/" + dir.Name + "&target=" + targetText + "\" title=\"打开\"><img src=\"template/images/ico/chart.png\" /></a>");
                    strBuilder.Append("<a href=\"?action=editf&dir=" + thisDir + "/" + dir.Name + "&target=" + targetText + "\" title=\"添加子文件\"><img src=\"template/images/ico/add.png\" /></a>");
                    strBuilder.Append("<a operate=\"delete\" operate=\"delete\" href=\"?action=deld&dir=" + thisDir + "&name=" + dir.Name + "&target=" + targetText + "\" title=\"删除\"><img src=\"template/images/ico/delete.png\" /></a>");
                    strBuilder.Append("</td>");
                    strBuilder.Append("</tr>");
                }
                foreach (FileInfo cFile in direct.GetFiles())
                {
                    string tempTxt = string.Empty;
                    try { tempTxt = thisDir + "/" + cFile.Name; tempTxt = tempTxt.Replace(tempDir, "{@dir}"); }
                    catch { }
                    strBuilder.Append("<tr title=\"双击选择模版\" operate=\"dblselected\" url=\"" + tempTxt + "\" class=\"hback\">");
                    strBuilder.Append("<td>" + cFile.Name + "</td>");
                    strBuilder.Append("<td>" + cFile.Extension.ToString() + "</td>");
                    strBuilder.Append("<td>" + cFile.Length + "</td>");
                    strBuilder.Append("<td>");
                    strBuilder.Append("<a href=\"?action=editf&dir=" + thisDir + "&name=" + cFile.Name + "&target=" + targetText + "\" title=\"编辑\"><img src=\"template/images/ico/edit.png\" /></a>");
                    strBuilder.Append("<a operate=\"delete\" href=\"?action=delf&dir=" + thisDir + "&name=" + cFile.Name + "&target=" + targetText + "\" title=\"删除\"><img src=\"template/images/ico/delete.png\" /></a>");
                    strBuilder.Append("</td>");
                    strBuilder.Append("</tr>");
                }
            }
            catch { }
            strBuilder.Append("</table>");
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/template/default.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "target": strValue = targetText; break;
                    case "tempdir": strValue = tempDir; break;
                    case "thisdir": strValue = thisDir; break;
                    case "Directory": strValue = this.GetDirectory(); break;
                    case "list": strValue = strBuilder.ToString(); break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 添加文件夹
        /// </summary>
        protected void AddDirectory()
        {

            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/template/AddDirectory.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "target": strValue = targetText; break;
                    case "tempdir": strValue = tempDir; break;
                    case "thisdir": strValue = thisDir; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 编辑文件
        /// </summary>
        protected void UpdateFile() {
            string name = RequestHelper.GetRequest("name").toString();
            string sPath = thisDir + "/" + name;
            string strReader = string.Empty;
            try { if (File.Exists(MapPath(sPath))) { strReader = FileHelper.FileReader(sPath); } }
            catch { }
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/template/add.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "target": strValue = targetText; break;
                    case "thisdir": strValue = thisDir; break;
                    case "strreader": strValue = strReader; break;
                    case "name": strValue = name; break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /*********************************************************************
         * 数据处理区域
         * *******************************************************************/
        /// <summary>
        /// 删除文件夹
        /// </summary>
        protected void DeleteDirectory()
        {
            try
            {
                string name = RequestHelper.GetRequest("name").toString();
                if (name == "") { this.Alert("请求参数错误！"); Response.End(); }
                string DirPath = thisDir + "/" + name;
                if (!Directory.Exists(MapPath(DirPath))) { this.Alert("文件目录不存在！"); Response.End(); }
                Directory.Delete(MapPath(DirPath), true);
                this.History();
                Response.End();
            }
            catch { }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        protected void DeleteFile()
        {
            try
            {
                string name = RequestHelper.GetRequest("name").toString();
                if (name == "") { this.Alert("请求参数错误！"); Response.End(); }
                string DirPath = thisDir + "/" + name;
                if (!File.Exists(MapPath(DirPath))) { this.Alert("文件不存在！"); Response.End(); }
                File.Delete(MapPath(DirPath));
                this.History();
                Response.End();
            }
            catch { }
        }
        /// <summary>
        /// 保存添加目录
        /// </summary>
        protected void AddDirectorySave()
        {
            try
            {
                string name = RequestHelper.GetRequest("name").toString();
                if (name == "") { this.Alert("请求参数错误！"); Response.End(); }
                if (name.Length > 15) { this.Alert("目录名称长度限制在15个字符以内！"); Response.End(); }
                if (VerifyCenter.VerifySpecific(name)) { this.Alert("目录名称不允许出现特殊字符！"); Response.End(); }
                string DirPath = thisDir + "/" + name;
                if (Directory.Exists(MapPath(DirPath))) { this.Alert("目录已经存在！"); Response.End(); }
                Directory.CreateDirectory(MapPath(DirPath));
                this.ConfirmMessage("目录添加成功，点击确定将停留在当前页面！", falseUrl: "?action=default&dir=" + thisDir + "");
                Response.End();
            }
            catch { }
        }
        /// <summary>
        /// 保存添加文件
        /// </summary>
        protected void AddFileSave()
        {
            string oldName = RequestHelper.GetRequest("oldName").toString();
            string Name = RequestHelper.GetRequest("name").toString();
            if (Name == "") { this.Alert("请输入文件名称！"); Response.End(); }
            if (Name.Contains("?") || Name.Contains(":") || Name.Contains("!") || Name.Contains("/") || Name.Contains("\\")) { this.Alert("文件名称不允许包含特殊字符！"); Response.End(); }
            if (Name.Length > 20) { this.Alert("文件名称最大长度请限制在20个字符以内！"); Response.End(); }
            if (!Name.Contains(".html")) { this.Alert("模版文件只允许添加html格式文件！"); Response.End(); }
            if (!Name.EndsWith(".html")) { this.Alert("文件格式错误，值允许上传html格式文件！"); Response.End(); }
            string cTxt = RequestHelper.GetRequest("cTxt", false).toString();
            /*************************************************************************
             * 删除原有文件，创建一个新的文件
             * **********************************************************************/
            if (oldName != "" && !string.IsNullOrEmpty(oldName))
            {
                try { if (File.Exists(MapPath(thisDir + "/" + oldName))) { File.Delete(MapPath(thisDir + "/" + oldName)); } }
                catch { }
            }
            /*************************************************************************
             * 创建新的文件
             * **********************************************************************/
            try
            {
                if (File.Exists(MapPath(thisDir + "/" + Name))) { File.Delete(MapPath(thisDir + "/" + Name)); }
                File.AppendAllText(MapPath(thisDir + "/" + Name), cTxt, Encoding.Default);
            }
            catch { }
            this.ConfirmMessage("文件保存成功，点击确定将继续停留在当前页面！", falseUrl: "?action=default&dir=" + thisDir + "");
            Response.End();
        }

        /**********************************************************************
         * 公用函数区域
         * ********************************************************************/

        /// <summary>
        /// 获取到当前目录同级目录
        /// </summary>
        /// <returns></returns>
        private string GetDirectory()
        {
            StringBuilder strBuilder = new StringBuilder();
            try
            {
                DirectoryInfo direct = new DirectoryInfo(this.MapPath(this.thisDir));
                DirectoryInfo cParent = new DirectoryInfo(this.MapPath(this.tempDir));
                if (thisDir != tempDir)
                {
                    foreach (DirectoryInfo dir in cParent.GetDirectories())
                    {
                        strBuilder.Append("<option value=\"" + tempDir + "/" + dir.Name + "\">");
                        strBuilder.Append(tempDir + "/" + dir.Name);
                        strBuilder.Append("</option>");
                    }
                }
                strBuilder.Append("<option value=\"" + thisDir + "\" selected>" + thisDir + "</option>");
                foreach (DirectoryInfo dir in direct.GetDirectories())
                {
                    strBuilder.Append("<option value=\"" + thisDir + "/" + dir.Name + "\">");
                    strBuilder.Append("┗━" + thisDir + "/" + dir.Name);
                    strBuilder.Append("</option>");
                }
            }
            catch { }
            return strBuilder.ToString();
        }
        /// <summary>
        /// 将相对路径转换成绝对路径
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string MapPath(string Path)
        {
            try
            {
                if (!Path.Contains(":")) { Path =System.Web.HttpContext.Current.Server.MapPath(Path); }
                Path = Path.Replace("/", "\\");
                Path = Path.Replace("\\\\", "\\");
                return Path;
            }
            catch { return Path; }
        }
    }
}