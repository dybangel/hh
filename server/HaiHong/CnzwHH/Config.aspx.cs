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
    public partial class Config : Fooke.Code.AdminHelper
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (strRequest == "cache")
            {
                try
                {
                    Application.RemoveAll();
                    this.ErrorMessage("系统缓存更新成功！");
                    Response.End();
                }
                catch { }
            }
            if (this.strRequest == "skey")
            {
                //this.VerificationRole("更新密钥");
                //string AppID = this.GetParameter("AppID", "chatXML").toString();
                //string AppSecret = this.GetParameter("AppSecret", "chatXML").toString();
                //if (string.IsNullOrEmpty(AppID)) { Response.End(); }
                //if (string.IsNullOrEmpty(AppSecret)) { Response.End(); }
                //Fooke.WeChat.API.AccessTokenCenter.UpdateAccessToken(AppID, AppSecret);
                //this.ErrorMessage("微信公众号授权密钥更新成功");
                //Response.End();
            }
            /***************************************************************
             * 生成轮播图片JS文件
             * *************************************************************/
            switch (strRequest)
            {
                case "app": this.VerificationRole("系统参数配置"); ApplicationConfig(); Response.End(); break;
                case "save": this.SaveConfig(); Response.End(); ; break;
                case "user": this.VerificationRole("用户参数配置"); MemberXML(); Response.End(); break;
                case "base": this.VerificationRole("系统参数配置"); this.ConfigXML(); Response.End(); break;
                case "pay": this.VerificationRole("在线支付配置"); this.PayXML(); Response.End(); break;
                case "notice": this.VerificationRole("系统公告"); this.NoticeXML(); Response.End(); break;
                case "Message": this.VerificationRole("系统参数配置"); this.MessageXML(); Response.End(); break;
                case "Guest": this.VerificationRole("留言设置"); Guest(); Response.End(); break;
                case "pushXml": this.VerificationRole("激光推送"); this.PushXml(); Response.End(); break;
                case "shareXml": this.VerificationRole("分享设置"); ShareXml(); Response.End(); break;
                case "invited": this.VerificationRole("邀请好友"); this.InvitedXml(); Response.End(); break;
                case "duiba": this.VerificationRole("兑吧商城"); this.DuibaXml(); Response.End(); break;
                case "bank": this.VerificationRole("绑卡参数设置"); this.BankXml(); Response.End(); break;
                case "alipay": this.VerificationRole("超级管理员权限"); this.AlipayXml(); Response.End(); break;
                case "Rotary": this.VerificationRole("转盘抽奖"); this.RotaryXml(); Response.End(); break;
                case "SignXml": this.VerificationRole("用户签到"); this.SignXml(); Response.End(); break;
            }
        }
        protected string ReaderXML(string configName)
        {
            try
            {
                string strXML = string.Empty;
                DataRow Rs = DbHelper.Connection.FindRow(TableCenter.Config, Params: "");
                if (Rs != null) { strXML = Rs[configName].ToString(); }
                return strXML;
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        protected void SaveConfig()
        {
            /***************************************************************************************
             * 检查操作权限
             * *************************************************************************************/
            //this.CheckPower();
            /***************************************************************************************
             * 权限处理完成了
             * *************************************************************************************/
            string configname = RequestHelper.GetRequest("configname").toString();
            if (configname == "") { DebugCenter.ErrorMessage("参数错误，请返回重试！"); }
            string configXML = RequestHelper.GetPrametersXML(false);
            DataRow Rs = DbHelper.Connection.FindRow(TableCenter.Config, Params: "");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary[configname] = configXML;
            if (Rs == null) { DbHelper.Connection.Insert(TableCenter.Config, dictionary); }
            else { DbHelper.Connection.Update(TableCenter.Config, dictionary, Params: ""); }
            /*****************************************************************
             * 更新服务器缓存
             * ***************************************************************/
            try
            {
                Rs = DbHelper.Connection.FindRow(TableCenter.Config, Params: string.Empty);
                if (Rs != null) { BufferHelper.Add("Fooke_Config", Rs); }
            }
            catch { }
            this.History();
            Response.End();
        }
        /// <summary>
        /// 系统基本参数配置
        /// </summary>
        protected void ConfigXML()
        {
            /********************************************************************
             * 开始程序
             * *****************************************************************/
            string configname = "siteXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/site.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "icon": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "oIcon",
                        fileValue = Reader.GetParameter("oIcon").toString()
                    }); break;
                    case "siteurl": strValue = FunctionCenter.SiteUrl(); break;
                    case "isViral": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isViral",Value="1",Text="检查"},
                        new RadioMode(){Name="isViral",Value="0",Text="跳过"}
                    }, Reader.GetParameter("isViral").toInt()); break;
                    case "isMap": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isMap",Value="1",Text="是"},
                        new RadioMode(){Name="isMap",Value="0",Text="否"}
                    }, Reader.GetParameter("autoDisplay").toInt()); break;
                    case "isMark": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isMark",Value="1",Text="是"},
                        new RadioMode(){Name="isMark",Value="0",Text="否"}
                    }, Reader.GetParameter("thisNce").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 激光推送
        /// </summary>
        protected void PushXml()
        {
            string configname = "PushXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/push.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 兑吧商城参数配置
        /// </summary>
        protected void DuibaXml()
        {
            string configname = "duibaXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/duiba.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "isopen": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isOpen",Value="1",Text="开启"},
                        new RadioMode(){Name="isOpen",Value="0",Text="关闭"}}, Reader.GetParameter("isopen").toInt()); break;
                    case "siteurl": strValue = FunctionCenter.SiteUrl(); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 绑卡参数设置
        /// </summary>
        protected void BankXml()
        {
            string configname = "bankXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/bank.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "isEdit": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isEdit",Value="1",Text="允许修改(是)"},
                        new RadioMode(){Name="isEdit",Value="0",Text="允许修改(否)"}},
                        Reader.GetParameter("isEdit").toInt()); break;
                    case "isRepeat": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isRepeat",Value="1",Text="允许重复(是)"},
                        new RadioMode(){Name="isRepeat",Value="0",Text="允许重复(否)"}},
                       Reader.GetParameter("isRepeat").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        protected void AlipayXml()
        {
            string configname = "AlipayXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/alipay.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "isopen": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isOpen",Value="1",Text="开启提现(是)"},
                        new RadioMode(){Name="isOpen",Value="0",Text="开启提现(否)"}},
                        Reader.GetParameter("isopen").toInt()); break;
                    case "AliDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="AliDisplay",Value="1",Text="需要审核(是)"},
                        new RadioMode(){Name="AliDisplay",Value="0",Text="需要审核(否)"},
                        new RadioMode(){Name="AliDisplay",Value="2",Text="设置金额"}},
                        Reader.GetParameter("AliDisplay").toInt()); break;
                    case "AlIsOnLine": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="AlIsOnLine",Value="1",Text="线上(是)"},
                        new RadioMode(){Name="AlIsOnLine",Value="0",Text="线上(否)"}},
                        Reader.GetParameter("AlIsOnLine").toInt()); break;
                    case "isAlipay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isAlipay",Value="1",Text="开启支付宝(是)"},
                        new RadioMode(){Name="isAlipay",Value="0",Text="开启支付宝(否)"}},
                        Reader.GetParameter("isAlipay").toInt()); break;
                    case "isWeChat": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isWeChat",Value="1",Text="开启微信(是)"},
                        new RadioMode(){Name="isWeChat",Value="0",Text="开启微信(否)"}},
                        Reader.GetParameter("isWeChat").toInt()); break;
                    case "WeDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="WeDisplay",Value="1",Text="需要审核(是)"},
                        new RadioMode(){Name="WeDisplay",Value="0",Text="需要审核(否)"},
                        new RadioMode(){Name="WeDisplay",Value="2",Text="设置金额"}},
                        Reader.GetParameter("WeDisplay").toInt()); break;
                    case "WeIsOnLine": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="WeIsOnLine",Value="1",Text="线上(是)"},
                        new RadioMode(){Name="WeIsOnLine",Value="0",Text="线上(否)"}},
                        Reader.GetParameter("WeIsOnLine").toInt()); break;
                    case "iRepeat": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="iRepeat",Value="1",Text="允许重复(是)"},
                        new RadioMode(){Name="iRepeat",Value="0",Text="允许重复(否)"}},
                         Reader.GetParameter("iRepeat").toInt()); break;
                    case "DbDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="DbDisplay",Value="1",Text="需要审核(是)"},
                        new RadioMode(){Name="DbDisplay",Value="0",Text="需要审核(否)"},
                        new RadioMode(){Name="DbDisplay",Value="2",Text="设置金额"}},
                         Reader.GetParameter("DbDisplay").toInt()); break;
                    case "isDollar": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isDollar",Value="1",Text="开启兑吧(是)"},
                        new RadioMode(){Name="isDollar",Value="0",Text="开启兑吧(否)"}
                        },Reader.GetParameter("isDollar").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 分享推广数据
        /// </summary>
        protected void ShareXml()
        {
            /*****************************************************************************************************************
             * 系统配置数据信息
             * ***************************************************************************************************************/
            string configname = "shareXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            /*************************************************************************************************************
             * 生成签到金额奖励列表
             * **********************************************************************************************************/
            string RankLevel = Reader.GetParameter("RankLevel").toInt("5");
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table style=\"margin:0px;\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"hback\">");
            for (int Selected = 1; Selected <= new Fooke.Function.String(RankLevel).cInt(); Selected++)
            {
                strBuilder.Append("<td>第" + Selected + "级</td>");
                strBuilder.Append("<td><input style=\"width:40px;\" placeholder=\"纯数字\" value=\"" + Reader.GetParameter("shareLevel" + Selected) + "\" name=\"shareLevel" + Selected + "\" notKong=\"true\" type=\"text\" class=\"inputtext\" /></td>");
            }
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            /*****************************************************************************************************************
             * 输出网页内容信息
             * ***************************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/share.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "isBonus": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isBonus",Value="1",Text="有奖励(是)"},
                        new RadioMode(){Name="isBonus",Value="0",Text="有奖励(否)"},
                    }, Reader.GetParameter("isBonus").toInt()); break;
                    case "shareModel": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="shareModel",Value="0",Text="注册成功"},
                        new RadioMode(){Name="shareModel",Value="1",Text="完成任务(首次)"},
                    }, Reader.GetParameter("shareModel").toInt()); break;
                    case "isTask": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isTask",Value="1",Text="开启提成(是)"},
                        new RadioMode(){Name="isTask",Value="0",Text="开启提成(否)"},
                    }, Reader.GetParameter("isTask").toInt()); break;
                    case "isRookie": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isRookie",Value="1",Text="开启红包(是)"},
                        new RadioMode(){Name="isRookie",Value="0",Text="开启红包(否)"},
                    }, Reader.GetParameter("isRookie").toInt()); break;
                    case "TModel": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="TModel",Value="1",Text="按百分比"},
                        new RadioMode(){Name="TModel",Value="0",Text="按固定额"},
                    }, Reader.GetParameter("TModel").toInt()); break;
                    case "shareLevel": strValue = strBuilder.ToString(); break;
                    case "configName": strValue = configname; break;
                    case "isGradeTask": strValue = FunctionBase.RadioButton(new List<RadioMode>() {
                        new RadioMode(){Name="isGradeTask",Value="1",Text="开启提成(是)"},
                        new RadioMode(){Name="isGradeTask",Value="0",Text="开启提成(否)"},
                    }, Reader.GetParameter("isGradeTask").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        
        /// <summary>
        /// 邀请好友设置
        /// </summary>
        protected void InvitedXml()
        {
            string configname = "InvitedXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/invited.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "wechat": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "wechat",
                        fileValue = Reader.GetParameter("wechat").toString(),
                        tips = "请上传微信公众号二维码图片"
                    }); break;
                    case "iOSThumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "iOSThumb",
                        fileValue = Reader.GetParameter("iOSThumb").toString(),
                        tips = "请上传iOS系统分享图标"
                    }); break;
                    case "AndroidThumb": strValue = FunctionBase.SiteFileControl(new FileMode()
                    {
                        fileName = "AndroidThumb",
                        fileValue = Reader.GetParameter("AndroidThumb").toString(),
                        tips = "请上传安卓系统分享图标"
                    }); break;
                    case "configName": strValue = configname; break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 短信配置
        /// </summary>
        protected void MessageXML()
        {
            /********************************************************************
             * 开始程序
             * *****************************************************************/
            string configname = "MessageXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/message.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "servername": strValue = FunctionBase.OptionList(new List<OptionMode>(){
                        new OptionMode(){Value="鲸鱼",Text="鲸鱼"},
                        new OptionMode(){Value="创蓝",Text="创蓝"},
                    }, Reader.GetParameter("servername").toString()); break;
                    case "encoding": strValue = FunctionBase.OptionList(new List<OptionMode>(){
                        new OptionMode(){Value="GBK",Text="GBK"},
                        new OptionMode(){Value="UTF8",Text="UTF8"},
                    }, Reader.GetParameter("encoding").toString()); break;
                    case "method": strValue = FunctionBase.OptionList(new List<OptionMode>(){
                        new OptionMode(){Value="get",Text="Get方式提交"},
                        new OptionMode(){Value="Post",Text="Post方式提交"},
                    }, Reader.GetParameter("method").toString()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 短信配置
        /// </summary>
        protected void Guest()
        {
            /********************************************************************
             * 开始程序
             * *****************************************************************/
            string configname = "GuestXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/Guest.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "isopen": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isOpen",Value="1",Text="开启"},
                        new RadioMode(){Name="isOpen",Value="0",Text="关闭"},
                    }, Reader.GetParameter("isOpen").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 用户参数信息配置
        /// </summary>
        protected void MemberXML()
        {
            /********************************************************************
             * 开始程序
             * *****************************************************************/
            string configname = "UserXML";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/user.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "isRegister": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isRegister",Value="1",Text="允许注册(是)"},
                        new RadioMode(){Name="isRegister",Value="0",Text="允许注册(否)"}
                    }, Reader.GetParameter("isRegister").toInt()); break;
                    case "autoDisplay": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="autoDisplay",Value="1",Text="自动审核(是)"},
                        new RadioMode(){Name="autoDisplay",Value="0",Text="自动审核(否)"}
                    }, Reader.GetParameter("autoDisplay").toInt()); break;
                    case "share": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="share",Value="1",Text="必须填写(是)"},
                        new RadioMode(){Name="share",Value="0",Text="必须填写(否)"}
                    }, Reader.GetParameter("share").toInt()); break;
                    case "thisNce": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="thisNce",Value="1",Text="允许登陆(是)"},
                        new RadioMode(){Name="thisNce",Value="0",Text="允许登陆(否)"}
                    }, Reader.GetParameter("thisNce").toInt()); break;
                    case "thisDevice": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="thisDevice",Value="1",Text="允许注册(是)"},
                        new RadioMode(){Name="thisDevice",Value="0",Text="允许注册(否)"}
                    }, Reader.GetParameter("thisDevice").toInt()); break;
                    case "AliRepeat": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="AliRepeat",Value="1",Text="允许重复(是)"},
                        new RadioMode(){Name="AliRepeat",Value="0",Text="允许重复(否)"}
                    }, Reader.GetParameter("AliRepeat").toInt()); break;
                    case "MobileRepeat": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="MobileRepeat",Value="1",Text="允许重复(是)"},
                        new RadioMode(){Name="MobileRepeat",Value="0",Text="允许重复(否)"}
                    }, Reader.GetParameter("MobileRepeat").toInt()); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 应用程序下载设置
        /// </summary>
        protected void ApplicationConfig()
        {
            string configname = "appXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/app.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
        /// <summary>
        /// 在线充值参数配置
        /// </summary>
        protected void PayXML()
        {
            string configname = "payxml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            /******************************************************************************************************
             * 生成充值等级赠送
             * ****************************************************************************************************/
            string AmountList = Reader.GetParameter("AmountList").toString("500|2000|5000|8000");
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table style=\"margin:0px;\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"xingmu\">");
            foreach (string strChar in AmountList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                strBuilder.Append("<td style=\"width:120px\">充值" + strChar + "元赠送</td>");
            }
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"hback\">");
            foreach (string strChar in AmountList.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                strBuilder.Append("<td><input style=\"width:80px;\" placeholder=\"纯数字\" value=\"" + Reader.GetParameter("Fee" + strChar) + "\" name=\"Fee" + strChar + "\" notKong=\"true\" type=\"text\" class=\"inputtext\" /></td>");
            }
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            /******************************************************************************************************
             * 输出网页内容信息
             * ****************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/pay.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "isOpen": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isOpen",Value="1",Text="允许充值(是)"},
                        new RadioMode(){Name="isOpen",Value="0",Text="允许充值(否)"}
                    }, Reader.GetParameter("isOpen").toInt()); break;
                    case "isActivity": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isActivity",Value="1",Text="开启活动(是)"},
                        new RadioMode(){Name="isActivity",Value="0",Text="开启活动(否)"}
                    }, Reader.GetParameter("isActivity").toInt()); break;
                    case "ActivityMode": strValue = FunctionBase.CheckBoxButton(new List<CheckBoxMode>(){
                        new CheckBoxMode(){Name="isFirst",Value="1",Text="首次充值(第一次充值)",Checked=Reader.GetParameter("isFirst").toInt()},
                        new CheckBoxMode(){Name="isEveryday",Value="1",Text="每日赠送(每日第一次充值)",Checked=Reader.GetParameter("isEveryday").toInt()}
                    }); break;
                    case "list": strValue = strBuilder.ToString(); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 在线充值参数配置
        /// </summary>
        protected void NoticeXML()
        {
            string configname = "NoticeXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/notice.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }


        /// <summary>
        /// 中奖数字区域显示设置
        /// </summary>
        public static readonly Dictionary<string, string> arrDictionary = new Dictionary<string, string>() {
                {"01","一等奖"},
                {"02","谢谢参与"},
                {"03","六等奖"},
                {"04","要加油哦"},
                {"05","五等奖"},
                {"06","运气先存着"},
                {"07","四等奖"},
                {"08","再接再厉"},
                {"09","三等奖"},
                {"10","祝您好运"},
                {"11","二等奖"},
                {"12","不要灰心"}
       };

        /// <summary>
        /// 转盘抽奖
        /// </summary>
        protected void RotaryXml()
        {
            string configname = "RotaryXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            /*************************************************************************************************************
             * 生成中奖概率区域
             * **********************************************************************************************************/
            StringBuilder strBuilder = new StringBuilder();
            string strMinimum = Reader.GetParameter("strMinimum").toString("500|1000|3000|5000|10000");
            strBuilder.Append("<table style=\"margin:0px;\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"xingmu\">");
            foreach (string strChar in strMinimum.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                strBuilder.Append("<td style=\"width:120px\">" + strChar + "抽奖次数</td>");
            }
            strBuilder.Append("</tr>");
            strBuilder.Append("<tr class=\"hback\">");
            foreach (string strChar in strMinimum.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                strBuilder.Append("<td><input style=\"width:80px;\" placeholder=\"纯数字\" value=\"" + Reader.GetParameter("Inval" + strChar) + "\" name=\"Inval" + strChar + "\" notKong=\"true\" type=\"text\" class=\"inputtext\" /></td>");
            }
            strBuilder.Append("</tr>");
            strBuilder.Append("</table>");
            /*************************************************************************************************************
             * 生成中奖概率区域
             * **********************************************************************************************************/
            StringBuilder strText = new StringBuilder();
            strText.Append("<table style=\"margin:0px;\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strText.Append("<tr class=\"xingmu\">");
            strText.Append("<td width=\"160\">转盘区域</td>");
            strText.Append("<td width=\"100\">抽中概率</td>");
            strText.Append("<td width=\"120\">中奖金额</td>");
            strText.Append("<td width=\"120\">最低消费</td>");
            strText.Append("</tr>");
            foreach (KeyValuePair<string, string> Pair in arrDictionary)
            {
                strText.Append("<tr class=\"hback\">");
                strText.Append("<td>" + Pair.Value + "</td>");
                strText.Append("<td><input style=\"width:80px;\" placeholder=\"纯数字\" value=\"" + Reader.GetParameter("Win" + Pair.Key) + "\" name=\"Win" + Pair.Key + "\" notKong=\"true\" type=\"text\" class=\"inputtext\" /></td>");
                strText.Append("<td><input style=\"width:80px;\" placeholder=\"纯数字\" value=\"" + Reader.GetParameter("Bon" + Pair.Key) + "\" name=\"Bon" + Pair.Key + "\" notKong=\"true\" type=\"text\" class=\"inputtext\" /></td>");
                strText.Append("<td><input style=\"width:80px;\" placeholder=\"纯数字\" value=\"" + Reader.GetParameter("Cos" + Pair.Key) + "\" name=\"Cos" + Pair.Key + "\" notKong=\"true\" type=\"text\" class=\"inputtext\" /></td>");
                strText.Append("</tr>");
            }
            strText.Append("</table>");
            /*************************************************************************************************************
             * 开始程序,输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/RotaryXml.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "isOpen": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isOpen",Value="1",Text="开启转盘(是)"},
                        new RadioMode(){Name="isOpen",Value="0",Text="开启转盘(否)"}
                    }, Reader.GetParameter("isOpen").toInt()); break;
                    case "strPoints": strValue = strText.ToString(); break;
                    case "muns": strValue = strBuilder.ToString(); break;
                    case "strMinimum": strValue = strMinimum.ToString(); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }

        /// <summary>
        /// 用户签到参数
        /// </summary>
        protected void SignXml()
        {
            string configname = "SignXml";
            string configXML = this.ReaderXML(configname);
            ConfigurationReader Reader = new ConfigurationReader(configXML);
            /*************************************************************************************************************
             * 生成签到金额奖励列表
             * **********************************************************************************************************/
            string strBonus = Reader.GetParameter("strBonus").toString("1000|3000|5000|8000|15000");
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<table style=\"margin:0px;\" border=\"0\" cellspacing=\"1\" cellpadding=\"3\" class=\"table\">");
            strBuilder.Append("<tr class=\"xingmu\">");
            strBuilder.Append("<td width=\"160\">有效消费</td>");
            strBuilder.Append("<td width=\"100\">奖励金额</td>");
            strBuilder.Append("<td width=\"120\">额外奖励</td>");
            strBuilder.Append("</tr>");
            foreach (string strChar in strBonus.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
            {
                strBuilder.Append("<tr class=\"hback\">");
                strBuilder.Append("<td>" + strChar + "</td>");
                strBuilder.Append("<td><input style=\"width:80px;\" placeholder=\"纯数字\" value=\"" + Reader.GetParameter("Win" + strChar) + "\" name=\"Win" + strChar + "\" notKong=\"true\" type=\"text\" class=\"inputtext\" /></td>");
                strBuilder.Append("<td><input style=\"width:80px;\" placeholder=\"纯数字\" value=\"" + Reader.GetParameter("Cos" + strChar) + "\" name=\"Cos" + strChar + "\" notKong=\"true\" type=\"text\" class=\"inputtext\" /></td>");
                strBuilder.Append("</tr>");
            }
            strBuilder.Append("</table>");
            /*************************************************************************************************************
             * 开始程序,输出网页内容
             * **********************************************************************************************************/
            Fooke.SimpleMaster.SimpleMaster Master = new SimpleMaster.SimpleMaster();
            string strResponse = Master.Reader("template/config/SignXml.html");
            strResponse = Master.Start(strResponse, new Fooke.SimpleMaster.Function((funName) =>
            {
                string strValue = string.Empty;
                switch (funName)
                {
                    case "configName": strValue = configname; break;
                    case "signOpen": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="signOpen",Value="1",Text="开启签到(是)"},
                        new RadioMode(){Name="signOpen",Value="0",Text="开启签到(否)"}
                    }, Reader.GetParameter("signOpen").toInt()); break;
                    case "isRepeat": strValue = FunctionBase.RadioButton(new List<RadioMode>(){
                        new RadioMode(){Name="isRepeat",Value="1",Text="开启(是)"},
                        new RadioMode(){Name="isRepeat",Value="0",Text="开启(否)"}
                    }, Reader.GetParameter("isRepeat").toInt()); break;
                    case "Foreach": strValue = strBuilder.ToString(); break;
                    default: try { strValue = Reader.GetParameter(funName).toString(); }
                        catch { } break;
                }
                return strValue;
            }));
            Response.Write(strResponse);
            Response.End();
        }
    }
}