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
    public partial class WeChatConfig : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.strRequest == "to")
            {
                this.VerificationRole("更新密钥");
                new Fooke.WeChat.TokenHelper(new Fooke.WeChat.ConfigHelper()).Update((iSuccess) =>
                {
                    if (iSuccess) { this.ErrorMessage("微信公众号授权密钥更新成功"); Response.End(); }
                    else { this.ErrorMessage("微信公众号授权密钥更新失败,请检查配置信息是否合法!"); }
                });
                Response.End();
            }
            /***************************************************************
             * 生成轮播图片JS文件
             * *************************************************************/
            switch (strRequest)
            {
                case "save": this.VerificationRole("微信配置"); this.SaveUpdate(); Response.End(); ; break;
                case "demo": this.VerificationRole("微信回复"); strDemo(); Response.End(); break;
                case "Follow": this.VerificationRole("微信回复"); strFollow(); Response.End(); break;
                case "image": this.VerificationRole("微信回复"); strImage(); Response.End(); break;
                case "wechat": this.VerificationRole("微信配置"); strWeChat(); Response.End(); break;
            }
        }

        protected ConfigurationReader FindParamter(string clnName)
        {
            string strXml = string.Empty;
            try
            {

                DataRow Rs = DbHelper.Connection.ExecuteFindRow("[Stored_FindWeChatConfig]", new Dictionary<string, object>() {
                    {"clnName",clnName}
                });
                if (Rs != null) { strXml = Rs[clnName].ToString(); }
            }
            catch { }
            if (string.IsNullOrEmpty(strXml)) { strXml = "<configurationRoot></configurationRoot>"; }
            return new ConfigurationReader(strXml);
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        protected void SaveUpdate()
        {
            /***************************************************************************************
             * 权限处理完成了
             * *************************************************************************************/
            string ConfigName = RequestHelper.GetRequest("configName").toString();
            if (string.IsNullOrEmpty(ConfigName)) { DebugCenter.ErrorMessage("参数错误，请返回重试！"); }
            string ConfigXML = RequestHelper.GetPrametersXML(false);
            Dictionary<string, object> thisDictionary = new Dictionary<string, object>();
            thisDictionary["clnName"] = ConfigName;
            thisDictionary["clnValue"] = ConfigXML;
            DbHelper.Connection.ExecuteProc("[Stored_SaveWeChatConfig]", thisDictionary);
            /*****************************************************************
             * 更新服务器缓存
             * ***************************************************************/

            /*****************************************************************
             * 处理数据信息
             * ***************************************************************/
            this.ErrorMessage("参数配置成功", iSuccess: true);
            Response.End();
        }
        /// <summary>
        /// 交易系统参数配置
        /// </summary>
        protected void strDemo()
        {
            string configname = "demoXml";
            ConfigurationReader Reader = FindParamter(configname);
            /*************************************************************************************************
             * 输出网页参数内容
             * ***********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatconfig/demo.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "RestoreMode": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="RestoreMode",Value="1",Text="文本回复"},
                        new RadioMode(){Name="RestoreMode",Value="0",Text="图文回复"}
                    }, Reader.GetParameter("RestoreMode").toInt()); break;
                    case "isOpen": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isOpen",Value="1",Text="开启(是)"},
                        new RadioMode(){Name="isOpen",Value="0",Text="开启(否)"}
                    }, Reader.GetParameter("isOpen").toInt()); break;
                    case "configName": strValue = configname; break;
                    case "mode": strValue = Reader.GetParameter("RestoreMode").toInt(); break;
                    case "materList": strValue = new MaterHelper().Options("0", Reader.GetParameter("MaterID").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 交易系统参数配置
        /// </summary>
        protected void strFollow()
        {
            string configname = "FollowXml";
            ConfigurationReader Reader = FindParamter(configname);
            /*************************************************************************************************
             * 输出网页参数内容
             * ***********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatconfig/Follow.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "RestoreMode": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="RestoreMode",Value="1",Text="文本回复"},
                        new RadioMode(){Name="RestoreMode",Value="0",Text="图文回复"}
                    }, Reader.GetParameter("RestoreMode").toInt()); break;
                    case "isOpen": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isOpen",Value="1",Text="开启(是)"},
                        new RadioMode(){Name="isOpen",Value="0",Text="开启(否)"}
                    }, Reader.GetParameter("isOpen").toInt()); break;
                    case "configName": strValue = configname; break;
                    case "mode": strValue = Reader.GetParameter("RestoreMode").toInt(); break;
                    case "materList": strValue = new MaterHelper().Options("0", Reader.GetParameter("MaterID").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 交易系统参数配置
        /// </summary>
        protected void strImage()
        {
            string configname = "imageXml";
            ConfigurationReader Reader = FindParamter(configname);
            /*************************************************************************************************
             * 输出网页参数内容
             * ***********************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatconfig/image.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "RestoreMode": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="RestoreMode",Value="1",Text="文本回复"},
                        new RadioMode(){Name="RestoreMode",Value="0",Text="图文回复"}
                    }, Reader.GetParameter("RestoreMode").toInt()); break;
                    case "isOpen": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isOpen",Value="1",Text="开启(是)"},
                        new RadioMode(){Name="isOpen",Value="0",Text="开启(否)"}
                    }, Reader.GetParameter("isOpen").toInt()); break;
                    case "configName": strValue = configname; break;
                    case "mode": strValue = Reader.GetParameter("RestoreMode").toInt(); break;
                    case "materList": strValue = new MaterHelper().Options("0", Reader.GetParameter("MaterID").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        protected void strWeChat()
        {
            /********************************************************************
             * 开始程序
             * *****************************************************************/
            string configname = "defaultXml";
            ConfigurationReader Reader = FindParamter(configname);
            /********************************************************************
             * 输出网页参数内容
             * *****************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/wechatconfig/wechat.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "siteurl": strValue = FunctionCenter.SiteUrl(); break;
                    //case "strkey": strValue = ConfigRs["strKey"].ToString(); break;
                    case "dimensional": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileValue = Reader.GetParameter("DimensionalPath").toString(),
                        fileName = "DimensionalPath",
                        tips = "请上传微信公众号二维码图片"
                    }); break;
                    default:
                        try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { }
                        break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

    }
}