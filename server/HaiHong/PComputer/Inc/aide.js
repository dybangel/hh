/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
小助手管理工具类
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var Assistant={};
/****************************************************************************************
*关闭网页提示控件
*****************************************************************************************/
Assistant["clsMaster"] = function()
{
	try{$("#AIDEMaster").remove();}
	catch(err){}
	try{$("#AIDEMaster").hide();}
	catch(err){}
};
var AssistantUpdate = function()
{
	if(window.cfg==undefined){ShowMessager({"text":"获取系统配置参数错误！"});return false;}
	else if(window.cfg==null){ShowMessager({"text":"获取系统配置参数错误！"});return false;}
	else if(typeof(window.cfg)!='object'){ShowMessager({"text":"获取系统配置参数错误！"});return false;}
	else if(window.cfg["aide"]==undefined){ShowMessager({"text":"获取系统配置参数错误！"});return false;}
	else if(window.cfg["aide"]==null){ShowMessager({"text":"获取系统配置参数错误！"});return false;}
	else{
		var options = {};
		options['title'] = '系统提示';
		options['text'] = '你确定要重新下载小助手吗?';
		options['back']=function(isOK){
			if(isOK){if(window.cfg["aide"]["devicetype"]=="android"){try{window.location=window.cfg['aide']['deposit'];}catch(err){}}else{try{window.location="itms-services://?action=download-manifest&url="+window.cfg['aide']['deposit'];}catch(err){};};}
		};
		try{ShowConfirm(options);}
		catch(err){}
	}
}
/****************************************************************************************
*小助手未打开的提示
*****************************************************************************************/
Assistant["underfind"] = function()
{
	/************************************************************************************
	*申明提示框关闭事件
	*************************************************************************************/
	var hideMaster = function()
	{
		try{
			if(document.querySelector('#AIDEMaster')){
				try{$("#AIDEMaster").remove();}
				catch(err){}	
			}
		}catch(err){}
	};
	/************************************************************************************
	*申明提示框开启事件
	*************************************************************************************/
	var showMaster = function()
	{
		var strTemplate = "";
		try{
			strTemplate+="<div id=\"AIDEMaster\">";
			strTemplate+="<div id=\"AIDEContianer\">";
			strTemplate+="<div id=\"AIDEThumb\"><img src=\"template/images/aide/underfind.png\"/></div>";
			strTemplate+="<div id=\"AIDETips\">小助手未打开,请先打开小助手吧</div>";
			strTemplate+="<div id=\"AIDEBtns\"><input type=\"button\" id=\"AIDETRUEButton\" value=\"打开小助手\" /></div>";
			strTemplate+="<div id=\"AIDEDownload\">";
			strTemplate+="<span>如遇问题</span>";
			strTemplate+="<a>重新下载助手</a>";
			strTemplate+="</div>";
			strTemplate+="</div>";
			strTemplate+="</div>";
		}catch(err){}
		/************************************************************************************
		*将内容赋值到网页当中
		*************************************************************************************/
		try{
			var frmTemplate = $(strTemplate)[0] || strTemplate;
			try{$(document.body).append(frmTemplate);}
			catch(err){}
			if(frmTemplate!=undefined && frmTemplate!=null && 
			document.querySelector("#AIDETRUEButton"))
			{
				$(frmTemplate).find("#AIDETRUEButton").click(function(){
					try{Assistant["open"]();}
					catch(err){}
					try{hideMaster();}
					catch(err){}
				});
			};
			
			if(frmTemplate!=undefined && frmTemplate!=null && 
			document.querySelector("#AIDEDownload"))
			{
				$(frmTemplate).find("#AIDEDownload").click(function(){
					try{AssistantUpdate();}
					catch(err){}
					try{hideMaster();}
					catch(err){}
				});
			}
			
		}catch(err){}
	};
	/************************************************************************************
	*设置提示框默认内容信息
	*************************************************************************************/
	if(!document.querySelector("#AIDETRUEButton")){
		try{showMaster();}
		catch(err){}
	};
};
/****************************************************************************************
*用户试玩的时间还没有到达
*****************************************************************************************/
Assistant["dialog"] = function(options)
{
	/************************************************************************************
	*申明提示框关闭事件
	*************************************************************************************/
	var hideMaster = function()
	{
		try{
			if(document.querySelector('#AIDEMaster')){
				try{$("#AIDEMaster").remove();}
				catch(err){}	
			}
		}catch(err){}
	};
	/************************************************************************************
	*申明提示框开启事件
	*************************************************************************************/
	var showMaster = function()
	{
		var strTemplate="";
		try{
			strTemplate+="<div id=\"AIDEMaster\">";
			strTemplate+="<div id=\"AIDEContianer\">";
			strTemplate+="<div id=\"AIDEThumb\">";
			strTemplate+="<img src=\"template/images/aide/"+options["icon"]+".png\"/>";
			strTemplate+="</div>";
			strTemplate+="<div id=\"AIDETips\">"+options["tips"]+"</div>";
			strTemplate+="<div id=\"AIDEBtns\">";
			if(options["confirmHide"]=="false"){
				strTemplate+="<input type=\"button\" ";
				strTemplate+=" value=\""+options["confirmText"]+"\"";
				strTemplate+=" id=\"AIDETRUEButton\"/>";
				strTemplate+="<div style=\"font-size:0px;height:4px;\"></div>";
			};
			if(options["cancelHide"]=="false"){
				strTemplate+="<input type=\"button\" ";
				strTemplate+=" value=\""+options["cancelText"]+"\"";
				strTemplate+=" id=\"AIDEFALSEButton\"/>";
				strTemplate+="<div style=\"font-size:0px;height:4px;\"></div>";
			};
			strTemplate+="</div>";
			strTemplate+="<div id=\"AIDEDownload\">";
			strTemplate+="<span>如遇问题</span>";
			strTemplate+="<a>重新下载助手</a>";
			strTemplate+="</div>";
			strTemplate+="</div>";
			strTemplate+="</div>";
		}catch(err){}
		/************************************************************************************
		*将内容赋值到网页当中
		*************************************************************************************/
		var frmTemplate = $(strTemplate)[0] || strTemplate;
		try{$(document.body).append(frmTemplate);}
		catch(err){}
		/************************************************************************************
		*设置点击事件数据信息
		*************************************************************************************/
		if(frmTemplate!=undefined && frmTemplate!=null && 
		document.querySelector("#AIDEFALSEButton"))
		{
			$(frmTemplate).find("#AIDEFALSEButton").click(function(){
				try{												  
					if(options["click"]!=undefined && options["click"]!=null 
					&& typeof(options["click"])=="function"){
						options["click"](true);	
					}
				}catch(err){}
				try{hideMaster();}
				catch(err){}
			});	
		}
		if(frmTemplate!=undefined && frmTemplate!=null && 
		document.querySelector("#AIDETRUEButton"))
		{
			$(frmTemplate).find("#AIDETRUEButton").click(function(){
				try{												  
					if(options["click"]!=undefined && options["click"]!=null 
					&& typeof(options["click"])=="function"){
						options["click"](false);	
					}
				}catch(err){}
				try{hideMaster();}
				catch(err){}
			});
		}
	}
	/************************************************************************************
	*设置提示框默认内容信息
	*************************************************************************************/
	var options = options || {};
	options["tips"] = options["tips"] || "来自客户端的对话信息获取失败";
	options["icon"] = options["icon"] || "underfind";
	options["confirmText"] = options["confirmText"] || "确定";
	options["confirmHide"] = options["confirmHide"] || "false";
	options["cancelText"] = options["cancelText"] || "取消";
	options["cancelHide"] = options["cancelHide"] || "true";
	/************************************************************************************
	*开始显示提示框信息
	*************************************************************************************/
	if(options!=undefined && options!=null 
	&& typeof(options)=="object")
	{
		try{showMaster();}
		catch(err){}	
	}
	
};

/****************************************************************************************
*打开小助手操作方法
*****************************************************************************************/
Assistant["open"] = function()
{
	try{Assistant["clsMaster"]();}
	catch(err){}
	try{
		if(window.cfg["aide"]["deviceType"]=="ios"){
			try{window.location = window.cfg["aide"]["packer"]+"?{\"type\":\"https\"}";	}
			catch(err){}
		}else{
			try{window.location = window.cfg["aide"]["packer"]+"?type=https";}
			catch(err){}	
		}
	}catch(err){}
	
};

/****************************************************************************************
*检查小助手是否开启
*****************************************************************************************/
Assistant["getAssistant"] = function()
{
	/*********************************************************************
	*输出错误处理提示信息
	***********************************************************************/
	var PutMessage = function(strMessage)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (strMessage || "未知的系统提示信息");}catch(err){}
		try{rspJson['type'] = 'error';}catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson);}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
		}
	};
	/*********************************************************************
	*开始申明数据处理信息
	***********************************************************************/
	var options = {};
	options["url"] = cfg["aide"]["server"]+"/start";
	options["type"] = "get";
	options["dataType"] = "jsonp";
	options["timeout"] = 5000;
	options["async"] = true;
	options["cache"] = false;
	options["success"] = function(){
		var tiemr = setTimeout(function(){
			try{clearTimeout(tiemr);}
			catch(err){}
			try{Assistant["getAssistant"]();}
			catch(err){}
		},10000);
	};
	options["error"] = function(xhr,e){
		try{Assistant["underfind"]();}
		catch(err){}
		try{
			var tiemr = setTimeout(function(){
				try{clearTimeout(tiemr);}
				catch(err){}
				try{Assistant["getAssistant"]();}
				catch(err){}
			},15000);
		}catch(err){}
	};
	/****************************************************************************************
	*验证请求数据的合法性
	*****************************************************************************************/
	if(options==undefined){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(options==null){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(typeof(options)!="object"){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(options["url"]==undefined){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(options["url"]==null){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(options["url"]==""){PutMessage('构建数据请求错误,请重试！');return false;}
	/****************************************************************************************
	*验证ajax对象的合法性
	*****************************************************************************************/
	if(Zepto==undefined){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(Zepto==null){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(typeof(Zepto)!="function"){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(Zepto["ajax"]==undefined){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(Zepto["ajax"]==null){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(Zepto["ajax"]==""){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	/****************************************************************************************
	*开始请求数据信息
	*****************************************************************************************/
	try{Zepto["ajax"](options);}
	catch(err){}
};

/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*小助手异步请求数据信息
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
Assistant["ajax"] = function(strAction,options)
{
	/****************************************************************************************
	*数据加载请求的动画信息
	*****************************************************************************************/
	var animation = function(operateText)
	{
		var closeAnimation = function()
		{
			if(document.querySelector("#rspLoading")){
				try{$(document.querySelector("#rspLoading")).remove();}
				catch(err){}
			}	
		};
		var openAnimation=function()
		{
			if(!document.querySelector("#rspLoading")){
				try{$(document.body).append($("<div id=\"rspLoading\"></div>")[0]);	}
				catch(err){}
			}else{
				try{$(document.querySelector("#rspLoading")).show();}
				catch(err){}	
			}
		};
		/**********************************************************************
		*开始执行数据请求处理
		***********************************************************************/
		try{
			if(operateText=="hide"){try{closeAnimation();}catch(err){};}
			else{try{openAnimation();}catch(err){};}	
		}catch(err){}
	};
	/*********************************************************************
	*输出错误处理提示信息
	***********************************************************************/
	var PutMessage = function(strMessage)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (strMessage || "未知的系统提示信息");}catch(err){}
		try{rspJson['type'] = 'error';}catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson);}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
		}
	};
	var stringFormat = function(args)
	{
		if(arguments==undefined){return "";}
		else if(arguments==null){return "";}
		else if(arguments.length==0){return "";}
		else if(arguments.length==1){return arguments[0];}
		else if (arguments.length ==2)
		{
			var strValue = arguments[0] || "";
			if (typeof (arguments[1]) == "object") {
				for (var key in arguments[1]) {
					if(arguments[1][key]!=undefined){
						var reg = new RegExp("({" + key + "})", "g"); 
						strValue = strValue.replace(reg, arguments[1][key]);
					}
				}
			}
			else {
				for (var i = 1; i < arguments.length; i++) 
				{
					if (arguments[i] != undefined) 
					{
						var reg= new RegExp("({)" + (i - 1) + "(})", "g");
						strValue = strValue.replace(reg, arguments[i]);
					}
				}
			}
			return strValue;
		}else if (arguments.length >=3){
			var strValue = arguments[0] || "";
			for (var i = 1; i < arguments.length; i++) 
			{
				if (arguments[i] != undefined) 
				{
					var reg= new RegExp("({)" + (i - 1) + "(})", "g");
					strValue = strValue.replace(reg, arguments[i]);
				}
			}
			return strValue;
		}
	};
	options = options || {};
	/*********************************************************************
	*开始申明数据处理信息
	***********************************************************************/
	var SendOption = {};
	SendOption["url"] = stringFormat("{0}/{1}?",cfg["aide"]["server"],strAction);
	SendOption["type"] = "get";
	if(options["data"]!=undefined && options["data"]!=null)
	{SendOption["data"] = options["data"];}
	SendOption["dataType"] = "jsonp";
	SendOption["timeout"] = 5000;
	SendOption["async"] = true;
	SendOption["cache"] = false;
	SendOption["success"] = function(rspJson)
	{
		try{animation("hide");}catch(err){};
		if(rspJson==undefined){PutMessage("获取数据通讯信息失败！");return false;}
		else if(rspJson==null){PutMessage("获取数据通讯信息失败！");return false;}
		else if(typeof(rspJson)!="object"){PutMessage("获取数据通讯信息失败！");return false;}
		else if(rspJson["tips"]==undefined){PutMessage("获取数据通讯信息失败！");return false;}
		else if(rspJson["tips"]==null){PutMessage("获取数据通讯信息失败！");return false;}
		else if(rspJson["success"]==undefined){PutMessage("获取数据通讯信息失败！");return false;}
		else if(rspJson["success"]==null){PutMessage("获取数据通讯信息失败！");return false;}
		else if(rspJson["success"]==""){PutMessage("获取数据通讯信息失败！");return false;}
		else if(rspJson["success"]!="true")
		{
			if(options["error"]!=undefined && options["error"]!=null 
			&& typeof(options["error"])=='function'){
				options["error"](rspJson);
			}else{
				PutMessage(rspJson["tips"]);
				return false;	
			}
		}
		else if(options["back"]!= undefined && options["back"]!=null 
		&& typeof(options["back"])=="function")
		{ 
			try{animation("hide");}catch(err){};
			try{options["back"](rspJson);}catch(err){}
			return false;
		}
	};
	SendOption["error"] = function(xhr,e){
		try{animation("hide");}catch(err){};
		if(options["isError"]!=true){
			try{PutMessage("数据请求过程中发生错误,请判断小助手是否打开！");	}
			catch(err){}
		}else if(options["error"]!=undefined && options["error"]!=null 
		&& typeof(options["error"])=='function'){
			try{options["error"]({"success":"false","tips":"数据请求过程中发生错误,请判断小助手是否打开！"});}
			catch(err){}
		};
		return false;
	};
	/****************************************************************************************
	*验证请求数据的合法性
	*****************************************************************************************/
	if(SendOption==undefined){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(SendOption==null){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(typeof(SendOption)!="object"){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(SendOption["url"]==undefined){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(SendOption["url"]==null){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(SendOption["url"]==""){PutMessage('构建数据请求错误,请重试！');return false;}
	/****************************************************************************************
	*验证ajax对象的合法性
	*****************************************************************************************/
	if(Zepto==undefined){PutMessage('无法获取Zepto对象,请引入Zepto包！');return false;}
	else if(Zepto==null){PutMessage('无法获取Zepto对象,请引入Zepto包！');return false;}
	else if(typeof(Zepto)!="function"){PutMessage('无法获取Zepto对象,Zepto包！');return false;}
	else if(Zepto["ajax"]==undefined){PutMessage('无法获取Zepto对象,请引入Zepto包！');return false;}
	else if(Zepto["ajax"]==null){PutMessage('无法获取Zepto对象,请引入Zepto包！');return false;}
	else if(Zepto["ajax"]==""){PutMessage('无法获取Zepto对象,请引入Zepto包！');return false;}
	/****************************************************************************************
	*开始请求数据信息
	*****************************************************************************************/
	try{animation();}catch(err){};
	try{Zepto["ajax"](SendOption);}
	catch(err){}
};