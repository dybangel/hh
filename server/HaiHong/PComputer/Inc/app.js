/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*获取小助手请求URL网址
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
;var getServerUrl = function(actionText)
{
	var serverUrl=cfg["aide"]["server"]+"action="+actionText;
	return serverUrl;
};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*开始抢夺任务列表信息
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var SnatchTask = function(appId,Kucun)
{
	
	/************************************************************************************
	*处理数据检查结果
	*************************************************************************************/
	var showRender = function(rspJson)
	{
		if(rspJson["tips"]==undefined){ShowMessager({"text":"任务抢夺失败,请刷新重试！"});return false;}
		else if(rspJson["tips"]==null){ShowMessager({"text":"任务抢夺失败,请刷新重试！"});return false;}
		else if(rspJson["tips"]==""){ShowMessager({"text":"任务抢夺失败,请刷新重试！"});return false;}
		else if(rspJson["tips"]=="exists"){
			ShowConfirm({
				"text":"您已经有一个任务在做了,是否放弃在做任务,抢夺当前任务?",
				"back":function(isConfirm){
					if(isConfirm){
						AbortTask(appId,function(){
							SnatchTask(appId,Kucun);						 
						});	
					}	
				}
			});	
		}else{
			try{window.location='app.aspx?action=show&token='+cfg["user"]["strtokey"]+'&appid='+appId+'';	}
			catch(err){}
		};	
	};
	/************************************************************************************
	*验证手机系统合法性
	*************************************************************************************/
	var VerificationSystem = function(sysChar)
	{
		var replaceIOS = function(strVakye){
			strVakye = strVakye.toLowerCase().replace("ios","");
			strVakye = strVakye.toLowerCase().replace("os","");
			return strVakye;
		}
		var isContinue = false;
		if(sysChar==undefined){return true	;}
		else if(sysChar==null){return true	;}
		else if(sysChar==""){return true;}
		else if(sysChar.length<=0){return true;}
		else {
			var arrTemp = sysChar.split(',');
			if(arrTemp==undefined){return true;}
			else if(arrTemp==null){return true;}
			else if(arrTemp.length<=0){return true;}
			else if(arrTemp.length>=1){
				var userChar = replaceIOS(cfg["user"]["devicechar"]) || "";
				for(var s in arrTemp){
					if(replaceIOS(arrTemp[s])==userChar)
					{
						isContinue = true;	
					}	
				}	
			}
		};
		return isContinue;
	};
	/************************************************************************************
	*验证手机型号合法性
	*************************************************************************************/
	var VerificationModel = function(sysChar)
	{
		var isContinue = false;
		if(sysChar==undefined){return true	;}
		else if(sysChar==null){return true	;}
		else if(sysChar==""){return true;}
		else if(sysChar.length<=0){return true;}
		else {
			var arrTemp = sysChar.split(',');
			if(arrTemp==undefined){return true;}
			else if(arrTemp==null){return true;}
			else if(arrTemp.length<=0){return true;}
			else if(arrTemp.length>=1){
				for(var s in arrTemp){
					if(arrTemp[s].toLowerCase()==cfg["user"]["devicemodel"].toLowerCase())
					{isContinue = true;break;}	
				}	
			}
		};
		return isContinue;
	};
	/************************************************************************************
	*判断抢夺任务的型号是否合法哦
	*************************************************************************************/
	if(standard[appId]==undefined){ShowMessager({"text":"获取任务配置信息失败,请刷新重试！"});return false;}
	else if(standard[appId]==null){ShowMessager({"text":"获取任务配置信息失败,请刷新重试！"});return false;}
	else if(typeof(standard[appId])!="object"){ShowMessager({"text":"获取任务配置信息失败,请刷新重试！"});return false;}
	else if(standard[appId]["modechar"]==undefined){ShowMessager({"text":"获取机型要求配置失败！"});return false;}
	else if(standard[appId]["modechar"]==null){ShowMessager({"text":"获取机型要求配置失败！"});return false;}
	else if(standard[appId]["syschar"]==undefined){ShowMessager({"text":"获取手机系统要求配置失败！"});return false;}
	else if(standard[appId]["syschar"]==null){ShowMessager({"text":"获取手机系统要求配置失败！"});return false;}
	else{
		try{
			if(!VerificationModel(standard[appId]["modechar"])){
				ShowMessager({"text":"您不满足系统或机型要求标准<br/>请升级您的系统或机型后再来!","html":true});
				return false;	
			}else if(!VerificationSystem(standard[appId]["syschar"])){
				ShowMessager({"text":"您不满足系统或机型要求标准<br/>请升级您的系统或机型后再来!","html":true});
				return false;	
			}
		}catch(err){}	
	};
	/************************************************************************************
	*检查当前用户是否已经存在任务
	*************************************************************************************/
	if(parseInt(Kucun)!=undefined && parseInt(Kucun)!=null && parseInt(Kucun)>=1){
		try{
			getResponse({
				"url":"../api/app.aspx?action=snatch",
				"data":{"appid":appId,"token":cfg["user"]["strtokey"]},
				"back":function(rspJson){
					if(rspJson!=undefined && rspJson!=null && typeof(rspJson)=='object'
					&& rspJson['success']!=undefined && rspJson['success']=='true')
					{
						try{showRender(rspJson);}
						catch(err){}
					}
				},
				"fail":function(rspJson){
					try{
						ShowMessager({"text":rspJson["tips"],'back':function(){
							$("#tr"+appId+"").fadeOut();												 
						}});
						return false;
					}catch(err){}
				}
			});
		}catch(err){}	
	}else{
		ShowMessager({"text":"任务已经被抢光了！"});
		return false;	
	};
};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*放弃当前正在做的任务
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var AbortTask = function(appId,callback)
{
	try{
		getResponse({
			"url":"../api/app.aspx?action=abort",
			"data":{"appid":appId,"token":cfg["user"]["strtokey"]},
			"back":function(rspJson)
			{
				if(rspJson!=undefined && rspJson!=null && typeof(rspJson)=='object'
				&& rspJson['success']!=undefined && rspJson['success']=='true')
				{
					try{
						if(callback!=undefined && callback!=null 
						&& typeof(callback)=="function"){callback();}
						else { window.location = 'app.aspx?token=' + cfg["user"]["strtokey"] + '&taskerModel=' + encodeURI(jQuery.cookies("taskerModel")); }
					}catch(err){}	
				}
			}
		});	
	}catch(err){}
};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
声明标准任务存储对象信息
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var standard = {};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
获取网页App任务列表信息
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var getApplication = function (taskerModel)
{
    jQuery.cookies("taskerModel", taskerModel);
	/************************************************************************************
	*判断当前标准任务是否在手机上已经存在
	*************************************************************************************/
	var Findexists = function(rspJson,callback)
	{
		var packText = "";
		$(rspJson["result"]).each(function(k,json){
			if(json['appmenu']=='标准任务'){
				if(packText==""){packText=json["packername"];}
				else{packText = packText+","+json["packername"];}	
		    }
		});
		/************************************************************************************
		*申明请求数据模块
		*************************************************************************************/
		var options = {};
		options["data"] = {"package":packText};
		options["isError"] = true;
		options["error"] = function(arr){
			if(callback!=undefined && callback!=null 
			&& typeof(callback)=="function"){
				try{callback("");}
				catch(err){}
			}
		};
		options["back"] = function(arr){
			if(callback!=undefined && callback!=null 
			&& typeof(callback)=="function"){
				try{callback(arr.tips);}
				catch(err){}
			}
		};
		/************************************************************************************
		*开始发送数据请求
		*************************************************************************************/
		try{Assistant["ajax"]("packagename",options);}
		catch(err){}
	}
	/************************************************************************************
	*正常任务,标准任务
	*************************************************************************************/
	var showRender = function(rspJson)
	{
		Findexists(rspJson,function(existsChar){
			var strTemplate="";
			strTemplate+="<div class=\"modename\">标准任务</div>";
			strTemplate+="<div class=\"box\">";
			try{
				strTemplate+="<table cellpadding=\"0\" cellspacing=\"0\" class=\"tab\">";
				/*********************************************************************************************
				*邀请好友获得奖励
				**********************************************************************************************/
				strTemplate+="<tr onclick=\"gotoInvited()\"";
				strTemplate+=" class=\"items\">";
				strTemplate+="<td class=\"thumb\">";
				strTemplate+="	<img class=\"icon\" src=\"template/images/logo.png\" />";
				strTemplate+="</td>";
				strTemplate+="<td class=\"textor\">";
				strTemplate+="	<div class=\"title\"><h4>邀请好友的奖励</h4></div>";
				strTemplate+="	<div class=\"text\">";
				strTemplate+="		<span class=\"classname\">任务</span>";
				strTemplate+="		<span class=\"balance\">剩余999+份</span>";
				strTemplate+="	</div>";
				strTemplate+="</td>";
				strTemplate+="<td class=\"amt\">";
				//strTemplate+="	<div class=\"amtnum\">";
				//strTemplate+="		+<font>20</font>元";
				//strTemplate+="	</div>";
				strTemplate+="</td>";
				strTemplate+="</tr>";
				/*********************************************************************************************
				*其他任务数据信息
				**********************************************************************************************/
				$(rspJson["result"]).each(function(k,json){
					if(json['appmenu']=='标准任务' 
					&& (existsChar.indexOf(json["packername"])==-1 || json["isare"]=="1"))
					{	
					    if (!(taskerModel != '所有任务' && taskerModel != json['taskermodel'])) {
							try{standard[json["appid"]] = json;}
							catch(err){}
							strTemplate+="<tr onclick=\"SnatchTask('"+json["appid"]+"','"+json["kucun"]+"')\"";
							strTemplate+="  id=\"tr"+json["appid"]+"\" class=\"items\">";
							strTemplate+="<td class=\"thumb\">";
							strTemplate+="	<img class=\"icon\" src=\""+json["thumb"]+"\" />";
							strTemplate+="</td>";
							strTemplate+="<td class=\"textor\">";
							strTemplate+="	<div class=\"title\"><h4>"+json["appname"]+"</h4></div>";
							strTemplate+="	<div class=\"text\">";
							strTemplate+="		<span class=\"classname\">"+json["classname"]+"</span>";
							strTemplate+="		<span class=\"balance\">剩余"+json["kucun"]+"份</span>";
							strTemplate+="	</div>";
							try{strTemplate+=getFormChar(json);}
							catch(err){}
							strTemplate+="</td>";
							strTemplate+="<td class=\"amt\">";
							if(json["isare"]=="1"){strTemplate+="<div class=\"arebtn\">进行中</div>";}
					
							strTemplate+="	<div class=\"amtnum\">";
							strTemplate+="		+<font>"+json["amount"]+"</font>元";
							strTemplate+="	</div>";
							strTemplate+="</td>";
							strTemplate+="</tr>";
						}
					}
				});							
				
				strTemplate+="</table>";
			}catch(err){}
			strTemplate+="</div>";
			/****************************************************************************************
			*选择控件赋值
			*****************************************************************************************/
			try{$("#TASKContianer").html(strTemplate);}
			catch(err){}
		});
	};
	/************************************************************************************
	*计划任务,预告任务
	*************************************************************************************/
	var appTasker = function(rspJson)
	{
		/****************************************************************************************
		*隐藏多余关键词
		*****************************************************************************************/
		var hideText = function(strValue){
			try{return strValue.substring(0,1)+"******";}
			catch(err){}	
		};
		/****************************************************************************************
		*判断是否存在计划任务
		*****************************************************************************************/
		var isFind = function(){
			var isChecked = false;
			try{
				$(rspJson["result"]).each(function(k,json){
					if(json['appmenu']=='计划任务'){
						isChecked = true;return true;	
					}						   
				});	
			}catch(err){}
			return isChecked;
		};
		/***********************************************************************************
		*将指定的时间标准化,格式化
		************************************************************************************/
		var getDateTime = function (dt) 
		{
			var year=dt.getFullYear();
			var month=dt.getMonth()+1;
			var day=dt.getDate();
			var h=dt.getHours();
			var m=dt.getMinutes();
			var s=dt.getSeconds();
			month=month<10?"0"+month:month;
			day=day<10?"0"+day:day;
			h=h<10 ? "0"+h:h;
			m=m<10 ? "0"+m:m;
			s=s<10 ? "0"+s:s;
			return year+"/"+month+"/"+day+" "+h+":"+m+":"+s+"";
		};
		var getDateHour = function (dt) 
		{
			var h=dt.getHours();
			var m=dt.getMinutes();
			m=m<10 ? "0"+m:m;
			h=h<10 ? "0"+h:h;
			return h+":"+m+"";
		};
		/***********************************************************************************
		*计算两个时间的时间差
		************************************************************************************/
		var  GetDateDiff = function(startTime, endTime, diffType) 
		{
			//将xxxx-xx-xx的时间格式，转换为 xxxx/xx/xx的格式 
			startTime = startTime.replace(/\-/g, "/");
			endTime = endTime.replace(/\-/g, "/");
			//将计算间隔类性字符转换为小写
			diffType = diffType.toLowerCase();
			var sTime =new Date(startTime); //开始时间
			var eTime =new Date(endTime); //结束时间
			//作为除数的数字
			var timeType =1;
			switch (diffType) {
				case"second":
					timeType =1000;
				break;
				case"minute":
					timeType =1000*60;
				break;
				case"hour":
					timeType =1000*3600;
				break;
				case"day":
					timeType =1000*3600*24;
				break;
				default:
				break;
			};
			return parseInt((sTime.getTime() - eTime.getTime()) / parseInt(timeType));
		};
		/****************************************************************************************
		*获取计划任务日期
		*****************************************************************************************/
		var ShowTime = function(addtime)
		{
			var toDateTime = getDateTime(new Date(addtime.replace(/\-/g, "/")));
			var nowDateTime = getDateTime(new Date());
			var days = GetDateDiff(toDateTime,nowDateTime,"day");
			var showHtml = '';
			showHtml+='<div class=\"showtime\">';
			showHtml+='<span class=\"days\">';
			if(days=="0"){showHtml+="今天";}
			else if(days=="1"){showHtml+="明天";}
			else if(days=="2"){showHtml+="后天";}
			else{showHtml+=""+new Date(toDateTime).getDate()+"日";}
			showHtml+='</span>';
			showHtml+='<span class=\"hour\">';
			showHtml+=""+getDateHour(new Date(toDateTime))+"";
			showHtml+='</span>';
			showHtml+='</div>';
			return showHtml;
		}
		/****************************************************************************************
		*显示任务列表信息
		*****************************************************************************************/
		var showList = function()
		{
			var strTemplate="";
			try{
				strTemplate+="<div class=\"modename\">任务预告(随时关注才可以做更多任务)</div>";
				strTemplate+="<div class=\"box\">";
				strTemplate+="<table cellpadding=\"0\" cellspacing=\"0\" class=\"tab\">";
				$(rspJson["result"]).each(function(k,json){
					if(json['appmenu']=='计划任务')
					{
						strTemplate+="<tr class=\"items\">";
						strTemplate+="<td class=\"thumb\">";
						strTemplate+=ShowTime(json["addtime"]);
						strTemplate+="</td>";
						strTemplate+="<td class=\"textor\">";
						strTemplate+="	<div class=\"title\"><h4>"+hideText(json["appname"])+"</h4></div>";
						strTemplate+="	<div class=\"text\">";
						strTemplate+="		<span class=\"balance\">库存"+json["kucun"]+"</span>";
						strTemplate+="	</div>";
						strTemplate+="</td>";
						strTemplate+="<td class=\"amt\">";
						strTemplate+="	<div class=\"amtnum\">";
						strTemplate+="		+<font>"+json["amount"]+"</font>元";
						strTemplate+="	</div>";
						strTemplate+="</td>";
						strTemplate+="</tr>";
						//strTemplate+="<tr class=\"foter\"><td colspan=\"3\"></td></tr>";
					}
				});
				strTemplate+="</table>";
				strTemplate+="</div>";
			}catch(err){}
			try{$("#PLANContianer").html(strTemplate);	}
			catch(err){}
		};
		
		if(rspJson!=undefined && rspJson!=null && typeof(rspJson)=='object' 
		&& rspJson["result"]!=undefined && rspJson["result"]!=null 
		&& typeof(rspJson["result"])=='object' && isFind())
		{
			try{showList();	}
			catch(err){}
		}
	};
	
	/************************************************************************************
	*已完成
	*************************************************************************************/
	var FinishTasker = function(rspJson)
	{
		/****************************************************************************************
		*隐藏多余关键词
		*****************************************************************************************/
		var hideText = function(strValue){
			try{return strValue.substring(0,1)+"******";}
			catch(err){}	
		};
		/****************************************************************************************
		*判断是否存在计划任务
		*****************************************************************************************/
		var isFind = function(){
			var isChecked = false;
			try{
				$(rspJson["result"]).each(function(k,json){
					if(json['appmenu']=='完成任务'){
						isChecked = true;return true;	
					}						   
				});	
			}catch(err){}
			return isChecked;
		};
		/****************************************************************************************
		*显示任务列表信息
		*****************************************************************************************/
		var showList = function()
		{
			var strTemplate="";
			try{
				strTemplate+="<div class=\"modename\">已完成任务</div>";
				strTemplate+="<div class=\"box\">";
				strTemplate+="<table cellpadding=\"0\" cellspacing=\"0\" class=\"tab\">";
				$(rspJson["result"]).each(function(k,json){
					if(json['appmenu']=='完成任务')
					{
						strTemplate+="<tr class=\"items\">";
						strTemplate+="<td class=\"thumb\">";
						strTemplate+="	<img class=\"icon\" src=\""+json["thumb"]+"\" />";
						strTemplate+="</td>";
						strTemplate+="<td class=\"textor\">";
						strTemplate+="	<div class=\"title\"><h4>"+json["appname"]+"</h4></div>";
						strTemplate+="	<div class=\"text\">";
						strTemplate+="		<span class=\"classname\">"+json["classname"]+"</span>";
						strTemplate+="		<span class=\"finish\">已完成</span>";
						strTemplate+="	</div>";
						strTemplate+="</td>";
						strTemplate+="<td class=\"amt\">";
						if(json["isare"]=="1"){
							strTemplate+="<div class=\"arebtn\">进行中</div>";	
						}
						strTemplate+="	<div class=\"amtnum\">";
						strTemplate+="		+<font>"+json["amount"]+"</font>元";
						strTemplate+="	</div>";
						strTemplate+="</td>";
						strTemplate+="</tr>";
						//strTemplate+="<tr class=\"foter\"><td colspan=\"3\"></td></tr>";
					}
				});
				strTemplate+="</table>";
				strTemplate+="</div>";
			}catch(err){}
			try{$("#FINISHContianer").html(strTemplate);}
			catch(err){}	
		};
		
		if(rspJson!=undefined && rspJson!=null && typeof(rspJson)=='object' 
		&& rspJson["result"]!=undefined && rspJson["result"]!=null 
		&& typeof(rspJson["result"])=='object' && isFind()){
			try{showList();	}
			catch(err){}
		}
	};
	/****************************************************************************************
	*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
	*开始获取任务数据
	☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
	*****************************************************************************************/
	getResponse({
		"url":"../api/app.aspx?token="+cfg["user"]["strtokey"],
		"back":function(rspJson){
			if(rspJson!=undefined && rspJson!=null && typeof(rspJson)=='object'
			&& rspJson["result"]!=undefined && rspJson["result"]!=null
			&& typeof(rspJson["result"])=="object")
			{
				try{showRender(rspJson);}
				catch(err){}	
				try{appTasker(rspJson);}
				catch(err){}
				try{FinishTasker(rspJson);}
				catch(err){}
			}
				
		}
	});
};

var gotoInvited = function()
{
	if(cfg["aide"]["devicetype"]!="android")
	{
		try{OpenFrame('Invited.aspx?token='+cfg["user"]['strtokey']);}
		catch(err){}
	}
	else{
		if(window.DmqbWebView==undefined){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
		else if(window.DmqbWebView==null){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
		else if(typeof(window.DmqbWebView)!='object'){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
		else if(window.DmqbWebView.startShareActivity==undefined){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
		else if(window.DmqbWebView.startShareActivity==null)
		{ShowMessager({"text":"SDK调用方法不存在！"});return false;}
		else{
			try{window.DmqbWebView.startShareActivity();}
			catch(err){ShowMessager({"text":"打开APP过程中发生错误,请重试！"});return false;}
		}
	}
};

var getSystem = function(sysChar)
{
	if(sysChar==undefined){return "无限制"	;}
	else if(sysChar==null){return "无限制"	;}
	else if(sysChar==""){return "无限制";}
	else if(sysChar.length<=0){return "无限制";}
	else {
		var arrTemp = sysChar.split(',');
		if(arrTemp==undefined){return "无限制";}
		else if(arrTemp==null){return "无限制";}
		else if(arrTemp.length<=0){return "无限制";}
		else if(arrTemp.length>=1){return arrTemp[0]+"+";}
	}
};

var getModel = function(sysChar)
{
	if(sysChar==undefined){return "无限制"	;}
	else if(sysChar==null){return "无限制"	;}
	else if(sysChar==""){return "无限制";}
	else if(sysChar.length<=0){return "无限制";}
	else {
		var arrTemp = sysChar.split(',');
		if(arrTemp==undefined){return "无限制";}
		else if(arrTemp==null){return "无限制";}
		else if(arrTemp.length<=0){return "无限制";}
		else if(arrTemp.length>=1){return arrTemp[0]+"+";}
	}
};


var getFormChar = function(rspJson)
{
	var strTemplate = "";
	try{
		if(rspJson!=undefined && rspJson!=null && typeof(rspJson)=="object"
		&& (rspJson["syschar"]!=undefined && rspJson["syschar"]!=null && rspJson["syschar"]!="") 
		|| (rspJson["modechar"]!=undefined && rspJson["modechar"]!=null && rspJson["modechar"]!="")){
			strTemplate+="<div class=\"text\">";
			strTemplate+="<span class=\"syschar\">系统:"+getSystem(rspJson["syschar"])+"</span>&nbsp;";
			strTemplate+="<span class=\"modechar\">机型:"+getModel(rspJson["modechar"])+"</span>";
			strTemplate+="</div>";	
		}
	}catch(err){}
	return strTemplate;
};