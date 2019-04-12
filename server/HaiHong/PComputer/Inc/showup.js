/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*任务详情页JS控件信息
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var ShowTasker = function(tasker)
{
	console.log(tasker["processname"]);

	/***********************************************************************************
	*显示任务倒计时信息
	************************************************************************************/
	var showTimer = function()
	{
	//alert("this is show up.js");
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
			return year+"-"+month+"-"+day+" "+h+":"+m+":"+s+"";
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
			}
			return parseInt((sTime.getTime() - eTime.getTime()) / parseInt(timeType));
		};
		/***********************************************************************************
		*开始执行请求数据
		************************************************************************************/
		var objTimer = setInterval(function(){
			var sTimer = GetDateDiff(tasker["expire"],getDateTime(new Date()),"second");
			if(sTimer>=1){
				sTimer = sTimer -1;
				var Minute = parseInt(sTimer/60);
				var Second = parseInt(sTimer%60);
				if(Minute<10){Minute=0+""+Minute;}
				if(Second<10){Second=0+""+Second;}
				$("#FRMInterval").html(Minute+":"+Second);
			}else{
				try{clearInterval(objTimer);}
				catch(err){}
				try{AbortTask(tasker["appid"]);}
				catch(err){}
			}
		},1000);
	};
	

	
	var FormatProcess = function(Process){
		if(Process=="1"){return "第一步";}
		else if(Process=="2"){return "第二步";}	
		else if(Process=="3"){return "第二步";}	
		else if(Process=="4"){return "第三步";}	
		else if(Process=="5"){return "第五步";}	
		else if(Process=="6"){return "第六步";}	
		else if(Process=="7"){return "第七步";}	
		else if(Process=="8"){return "第八步";}	
		else if(Process=="9"){return "第九步";}	
		else if(Process=="10"){return "第十步";}	
	}
	/***********************************************************************************
	*声明当前的步骤信息
	************************************************************************************/
	var thisProcess = 1;
	/***********************************************************************************
	*将Xml内容转换成数据变量
	************************************************************************************/
	var strJson = XmlFormatJson(tasker["strcontext"]);
	/***********************************************************************************
	*声明数据处理变量信息
	************************************************************************************/
	var strTemplate="";
	strTemplate+="<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" id=\"TABContianer\">";
	
	try{
		strTemplate+="<tr class=\"hback\">";
		strTemplate+="<td class=\"appinfo\">";
		strTemplate+="<div>任务奖励</div>";
		strTemplate+="<div class=\"num\">"+tasker["amount"]+"</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:0px;\"></div>";
		strTemplate+="<div>";
		strTemplate+="<span>剩余时间</span>";
		strTemplate+="<span id=\"FRMInterval\" style=\"padding:0px 2px\">00:00</span>";
		strTemplate+="</div>";
		strTemplate+="</td>";
		strTemplate+="</tr>";
	}catch(err){}
	
	/***********************************************************************************
	*第一步,显示下载按钮
	************************************************************************************/
	try{
		var starContext="如果您没有下载app请先下载,如果下载过,点击开始任务后,请返回本页面第二步,点击唤醒APP";
		if(strJson["getValue"]("starContext")!=undefined 
		&& strJson["getValue"]("starContext")!=null 
		&& strJson["getValue"]("starContext")!=""){
			starContext = strJson["getValue"]("starContext");
		}
		strTemplate+="<tr operate=\"step01\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+FormatProcess(thisProcess)+"</div>";
		strTemplate+="<div class=\"remark\">"+starContext+"</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		if(tasker["iskeyword"]!=undefined && tasker["iskeyword"]!=null 
		&& tasker["strkeyword"]!=undefined && tasker["strkeyword"]!=null
		&& tasker["strkeyword"]!="" && tasker["iskeyword"]=="1")
		{
			strTemplate+="<div>";	
			strTemplate+="<input readonly=\"readonly\" operate=\"copy\" value=\""+tasker["strkeyword"]+"\" type=\"text\" id=\"FRMKeyword\" onCopy=\"StartDownload();\"/>";
			strTemplate+="</div>";
			strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";	
			strTemplate+="<div>";	
			strTemplate+="<input operate=\"button\" onClick=\"StartDownload();\" id=\"KSButton\" value=\"开始下载\" type=\"button\"/>";
			strTemplate+="</div>";	
		}else {
			strTemplate+="<div>";	
			strTemplate+="<input operate=\"button\" onClick=\"StartDownload();\" id=\"KSButton\" value=\"开始任务\" type=\"button\"/>";
			strTemplate+="</div>";	
		}
		strTemplate+="</td>";
		strTemplate+="</tr>";
	}catch(err){}
	/***********************************************************************************
	*第二步,显示下载按钮
	************************************************************************************/
try{thisProcess = thisProcess + 1;}catch(err){};
	if(tasker["appmodel"]!="5")
	{
		// try{thisProcess = thisProcess + 1;}
		// catch(err){}
		
		// var SortContext="找到与下方图标相对应的app并下载";
		// if(strJson["getValue"]("SortContext")!=undefined 
		// && strJson["getValue"]("SortContext")!=null 
		// && strJson["getValue"]("SortContext")!=""){
		// 	SortContext = strJson["getValue"]("SortContext");
		// }
		
		// strTemplate+="<tr operate=\"step02\" class=\"step\">";
		// strTemplate+="<td class=\"textor\">";
		// strTemplate+="<div class=\"process\">"+FormatProcess(thisProcess)+"</div>";
		// strTemplate+="<div class=\"remark\">"+SortContext+"</div>";
		// strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		// strTemplate+="<div style=\"text-align:center\">";	
		// strTemplate += "<img onclick=\"imgs('" + tasker["thumb"] + "')\" style=\"width:80px\" src=\"" + tasker["thumb"] + "\"/>";
		// strTemplate+="</div>";
		// strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";	
		// strTemplate+="<div style=\"text-align:center;color:#999\">";
		// strTemplate+="约排在"+tasker["softrank"]+"位";
		// strTemplate+="</div>";	
		// strTemplate+="</td>";
		// strTemplate+="</tr>";
	};
	/***********************************************************************************
	*第二步,显示下载按钮
	************************************************************************************/
	
	if(tasker["appmodel"]!="5" && tasker["advmodel"]!="截图广告")
	{
		try{thisProcess = thisProcess + 1;}
		catch(err){}
		
		var TryContext="下载完成后回到本页面,点击唤醒APP,试玩10秒";
		if(strJson["getValue"]("TryContext")!=undefined 
		&& strJson["getValue"]("TryContext")!=null 
		&& strJson["getValue"]("TryContext")!=""){
			TryContext = strJson["getValue"]("TryContext");
		}
		strTemplate+="<tr operate=\"step02\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+"第二步"+"</div>";
		strTemplate+="<div class=\"remark\">"+TryContext+"</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div>";	
		strTemplate+="<input operate=\"button\" onclick=\"wakeupapp()\" id=\"TryButton\" value=\"唤醒APP\" type=\"button\"/>";
		strTemplate+="</div>";	
		strTemplate+="</td>";
		strTemplate+="</tr>";

		strTemplate+="<tr operate=\"step03\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+"第三步"+"</div>";
		strTemplate+="<div class=\"remark\">"+"二次唤醒APP,试玩5秒"+"</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div>";	
		strTemplate+="<input operate=\"button\" onclick=\"TryApplication();\" id=\"TryButton\" value=\"二次唤醒APP\" type=\"button\"/>";
		strTemplate+="</div>";	
		strTemplate+="</td>";
		strTemplate+="</tr>";


	}
	/***********************************************************************************
	*第二步,显示下载按钮
	************************************************************************************/
	if(tasker["advmodel"]=="普通广告"){
		try{thisProcess = thisProcess + 1;}
		catch(err){}
		var FishContext="试玩时间到了以后,点击下方领取奖励,就可以获得奖励啦";
		if(strJson["getValue"]("FishContext")!=undefined 
		&& strJson["getValue"]("FishContext")!=null 
		&& strJson["getValue"]("FishContext")!=""){
			FishContext = strJson["getValue"]("FishContext");
		}
		strTemplate+="<tr operate=\"step04\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+"第四步"+"</div>";
		strTemplate+="<div class=\"remark\">"+FishContext+"</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div>";	
		strTemplate+="<input operate=\"button\" onclick=\"FinishApplication();\" id=\"FSButton\" value=\"领取奖励\" type=\"button\"/>";
		strTemplate+="</div>";	
		strTemplate+="</td>";
		strTemplate+="</tr>";
	};
	/***********************************************************************************
	*上传广告截图功能
	************************************************************************************/
	if(tasker["advmodel"]=="评论广告")
	{
		try{thisProcess = thisProcess + 1;}
		catch(err){}
		var ULDContext="选择五星好评,分别复制标题,内容粘贴于对应位置,完成好评操作!";
		var DiscussTitle = cfgParams(tasker["strxml"],"DiscussTitle");
		var DiscussRemark = cfgParams(tasker["strxml"],"DiscussRemark");
		strTemplate+="<tr operate=\"step05\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+FormatProcess(thisProcess)+"</div>";
		strTemplate+="<div class=\"remark\">"+ULDContext+"</div>";
		strTemplate+="<div id=\"FRMCOPYTitle\" operate=\"copy\">标题:"+DiscussTitle+"</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div>";	
		strTemplate+="<input operate=\"button\" id=\"FRMCOPYTbutton\" value=\"复制标题\" type=\"button\" style=\"width:100px;height:36px;\"/>";
		strTemplate+="</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div id=\"FRMCOPYContext\" operate=\"copy\">内容："+DiscussRemark+"</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div>";	
		strTemplate+="<input operate=\"button\" id=\"FRMCOPYCbutton\" value=\"复制内容\" type=\"button\" style=\"width:100px;height:36px;\"/>";
		strTemplate+="</div>";	
		
		strTemplate+="</td>";
		strTemplate+="</tr>";
	};
	/***********************************************************************************
	*截图广告显示截图要求
	************************************************************************************/
	if(tasker["advmodel"]=="评论广告")
	{
		try{thisProcess = thisProcess + 1;}
		catch(err){}
		var HotContext="截图完成后,按照一下截图要求上传图片";
		if(strJson["getValue"]("HotContext")!=undefined 
		&& strJson["getValue"]("HotContext")!=null 
		&& strJson["getValue"]("HotContext")!=""){
			HotContext = strJson["getValue"]("HotContext");
		}
		strTemplate+="<tr operate=\"step05\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+FormatProcess(thisProcess)+"</div>";
		strTemplate+="<div class=\"remark\">"+HotContext+"</div>";
		strTemplate+="<div style=\"color:#fe6c00\">截图要求:</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div id=\"HOTContianer\">";
		strTemplate+="<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr>";
		if(strJson["getValue"]("SAMPThumb1")!=undefined && strJson["getValue"]("SAMPThumb1")!=null
		&& strJson["getValue"]("SAMPThumb1")!=""){
		    strTemplate += "<td><img  onclick=\"imgs('" + strJson["getValue"]("SAMPThumb1") + "')\" src=\"" + strJson["getValue"]("SAMPThumb1") + "\" /></td>";
		}
		if(strJson["getValue"]("SAMPThumb2")!=undefined && strJson["getValue"]("SAMPThumb2")!=null
		&& strJson["getValue"]("SAMPThumb2")!=""){
		    strTemplate += "<td><img onclick=\"imgs('" + strJson["getValue"]("SAMPThumb2") + "')\" src=\"" + strJson["getValue"]("SAMPThumb2") + "\" /></td>";
		}
		if(strJson["getValue"]("SAMPThumb3")!=undefined && strJson["getValue"]("SAMPThumb3")!=null
		&& strJson["getValue"]("SAMPThumb3")!=""){
		    strTemplate += "<td><img onclick=\"imgs('" + strJson["getValue"]("SAMPThumb3") + "')\" src=\"" + strJson["getValue"]("SAMPThumb3") + "\" /></td>";
		}
		strTemplate+="</tr></table>";
		strTemplate+="</div>";
		strTemplate+="</td>";
		strTemplate+="</tr>";	
	};
	/***********************************************************************************
	*上传广告截图功能
	************************************************************************************/
	if(tasker["advmodel"]=="评论广告")
	{
		try{thisProcess = thisProcess + 1;}
		catch(err){}
		var ULDContext="返回到本页面,按要求提交相关截图与信息，为顺利得到奖励,请不要提交错误";
		if(strJson["getValue"]("ULDContext")!=undefined 
		&& strJson["getValue"]("ULDContext")!=null 
		&& strJson["getValue"]("ULDContext")!=""){
			ULDContext = strJson["getValue"]("ULDContext");
		}
		strTemplate+="<tr operate=\"step05\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+FormatProcess(thisProcess)+"</div>";
		strTemplate+="<div class=\"remark\">"+ULDContext+"</div>";
		strTemplate+="<div style=\"color:#fe6c00\">上传截图:</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<input style=\"display:none\" onchange=\"doUpload(this)\" type=\"file\" operate=\"file\" id=\"doFile\"/>";	
		strTemplate+="<div id=\"UPLContianer\">";
		strTemplate+="<div operate=\"upload\"><img src=\"template/images/upload.png\" onclick=\"document.querySelector('#doFile').click();\" /></div>";
		strTemplate+="</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div><input type=\"text\" operate=\"text\" id=\"frmMobile\" name=\"frmMobile\" placeholder=\"请填写您的手机号\"/></div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div><input type=\"text\" operate=\"text\" id=\"frmName\" name=\"frmName\" placeholder=\"请填写您的姓名\"/></div>";	
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div>";	
		strTemplate+="<input operate=\"button\" onclick=\"FinishHOT()\" id=\"TJButton\" value=\"提交审核\" type=\"button\"/>";
		strTemplate+="</div>";	
		
		strTemplate+="</td>";
		strTemplate+="</tr>";
	};
	/***********************************************************************************
	*截图广告显示截图要求
	************************************************************************************/
	if(tasker["advmodel"]=="截图广告")
	{
		try{thisProcess = thisProcess + 1;}
		catch(err){}
		var HotContext="截图完成后,按照一下截图要求上传图片";
		if(strJson["getValue"]("HotContext")!=undefined 
		&& strJson["getValue"]("HotContext")!=null 
		&& strJson["getValue"]("HotContext")!=""){
			HotContext = strJson["getValue"]("HotContext");
		}
		strTemplate+="<tr operate=\"step05\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+FormatProcess(thisProcess)+"</div>";
		strTemplate+="<div class=\"remark\">"+HotContext+"</div>";
		strTemplate+="<div style=\"color:#fe6c00\">截图要求:</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div id=\"HOTContianer\">";
		strTemplate+="<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr>";
		if(strJson["getValue"]("SAMPThumb1")!=undefined && strJson["getValue"]("SAMPThumb1")!=null
		&& strJson["getValue"]("SAMPThumb1")!=""){
		    strTemplate += "<td><img onclick=\"imgs('" + strJson["getValue"]("SAMPThumb1") + "')\" src=\"" + strJson["getValue"]("SAMPThumb1") + "\" /></td>";
		}
		if(strJson["getValue"]("SAMPThumb2")!=undefined && strJson["getValue"]("SAMPThumb2")!=null
		&& strJson["getValue"]("SAMPThumb2")!=""){
		    strTemplate += "<td><img onclick=\"imgs('" + strJson["getValue"]("SAMPThumb2") + "')\" src=\"" + strJson["getValue"]("SAMPThumb2") + "\" /></td>";
		}
		if(strJson["getValue"]("SAMPThumb3")!=undefined && strJson["getValue"]("SAMPThumb3")!=null
		&& strJson["getValue"]("SAMPThumb3")!=""){
		    strTemplate += "<td><img onclick=\"imgs('" + strJson["getValue"]("SAMPThumb3") + "')\" src=\"" + strJson["getValue"]("SAMPThumb3") + "\" /></td>";
		}
		strTemplate+="</tr></table>";
		strTemplate+="</div>";
		strTemplate+="</td>";
		strTemplate+="</tr>";	
	};
	/***********************************************************************************
	*上传广告截图功能
	************************************************************************************/
	if(tasker["advmodel"]=="截图广告")
	{
		try{thisProcess = thisProcess + 1;}
		catch(err){}
		var ULDContext="返回到本页面,按要求提交相关截图与信息，为顺利得到奖励,请不要提交错误";
		if(strJson["getValue"]("ULDContext")!=undefined 
		&& strJson["getValue"]("ULDContext")!=null 
		&& strJson["getValue"]("ULDContext")!=""){
			ULDContext = strJson["getValue"]("ULDContext");
		}
		strTemplate+="<tr operate=\"step05\" class=\"step\">";
		strTemplate+="<td class=\"textor\">";
		strTemplate+="<div class=\"process\">"+FormatProcess(thisProcess)+"</div>";
		strTemplate+="<div class=\"remark\">"+ULDContext+"</div>";
		strTemplate+="<div style=\"color:#fe6c00\">上传截图:</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<input style=\"display:none\" onchange=\"doUpload(this)\" type=\"file\" operate=\"file\" id=\"doFile\"/>";	
		strTemplate+="<div id=\"UPLContianer\">";
		strTemplate+="<div operate=\"upload\"><img src=\"template/images/upload.png\" onclick=\"document.querySelector('#doFile').click();\" /></div>";
		strTemplate+="</div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div><input type=\"text\" operate=\"text\" id=\"frmMobile\" name=\"frmMobile\" placeholder=\"请填写您的手机号\"/></div>";
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div><input type=\"text\" operate=\"text\" id=\"frmName\" name=\"frmName\" placeholder=\"请填写您的姓名\"/></div>";	
		strTemplate+="<div style=\"display:block;width:100%;font-size:0px;height:10px;\"></div>";
		strTemplate+="<div>";	
		strTemplate+="<input operate=\"button\" onclick=\"FinishHOT()\" id=\"TJButton\" value=\"提交审核\" type=\"button\"/>";
		strTemplate+="</div>";	
		
		strTemplate+="</td>";
		strTemplate+="</tr>";
	};
	
	strTemplate+="</table>";
	/***********************************************************************************
	*将数据变量赋值
	************************************************************************************/
	try{$("#SHOWContianer").html(strTemplate);}
	catch(err){}
	/***********************************************************************************
	*复制关键词信息
	************************************************************************************/
	if(document.querySelector("#FRMKeyword")!=undefined 
	&& document.querySelector("#FRMKeyword")!=null)
	{
		try{
			var clipboard = new ClipboardJS(document.querySelector("#FRMKeyword"), {
				text: function() {
					return tasker["strkeyword"];
				},
				target:function() {
					return document.querySelector("#FRMKeyword");
				}
			});
			clipboard.on('success', function(e) {
				try{StartDownload();}
				catch(err){}
			});
			clipboard.on('error', function(e) {
				try{ShowMessager({"text":"关键词复制失败,请选择长按复制方式！"});}
				catch(err){}
			});	
		}catch(err){}
	};
	/***********************************************************************************
	*复制评论标题信息
	************************************************************************************/
	if(document.querySelector("#FRMCOPYTbutton")!=undefined 
	&& document.querySelector("#FRMCOPYTbutton")!=null)
	{
		try{
			var clipboard = new ClipboardJS(document.querySelector("#FRMCOPYTbutton"), {
				text: function() {
					return (cfgParams(tasker["strxml"],"DiscussTitle") || "");
				},
				target:function() {
					return document.querySelector("#FRMCOPYTbutton");
				}
			});
			clipboard.on('success', function(e) {
				try{$("#frm-alert").tips({"tips":"评论标题复制成功"});}
				catch(err){}
			});
			clipboard.on('error', function(e) {
				try{ShowMessager({"text":"评论标题复制失败,请选择长按复制方式！"});}
				catch(err){}
			});	
		}catch(err){}
	};
	/***********************************************************************************
	*复制评论标题信息
	************************************************************************************/
	if(document.querySelector("#FRMCOPYCbutton")!=undefined 
	&& document.querySelector("#FRMCOPYCbutton")!=null)
	{
		try{
			var clipboard = new ClipboardJS(document.querySelector("#FRMCOPYCbutton"), {
				text: function() {
					return (cfgParams(tasker["strxml"],"DiscussRemark") || "");
				},
				target:function() {
					return document.querySelector("#FRMCOPYCbutton");
				}
			});
			clipboard.on('success', function(e) {
				try{$("#frm-alert").tips({"tips":"评论内容复制成功"});}
				catch(err){}
			});
			clipboard.on('error', function(e) {
				try{ShowMessager({"text":"评论内容复制失败,请选择长按复制方式！"});}
				catch(err){}
			});	
		}catch(err){}
	};
	/***********************************************************************************
	*加载网页倒计时信息
	************************************************************************************/
	try{showTimer();}
	catch(err){}
};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*点击开始任务,跳转到指定的API数据接口
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var StartDownload=function()
{
	if(tasker==undefined){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(tasker==null){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(typeof(tasker)!='object'){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(tasker['appmodel']==undefined){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else if(tasker['appmodel']==null){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	/************************************************************************************
	*网页端小助手调试?
	*************************************************************************************/
	if(cfg["aide"]["isWeb"]=="true")
	{
		try{window.location="aide.aspx?action=download&appid="+tasker["appid"]+"&appkey="+tasker["appkey"]+"";}
		catch(err){}	
	}
	else{
		try{
			var rspJson = "{\"success\":\"true\"";
			rspJson += ",\"type\":\"download\"";
			rspJson += ",\"appid\":\""+tasker["appid"]+"\"";
			rspJson += ",\"identify\":\""+tasker["packername"]+"\"";
			rspJson += ",\"packagename\":\""+tasker["processname"]+"\"";
			rspJson += ",\"appkey\":\""+tasker["appkey"]+"\"";
			rspJson += ",\"myurl\":\""+tasker["url"]+"\"";
			rspJson += "}";
			try{
				if(cfg["aide"]["devicetype"]!="android"){
					try{tasker['isDown']="1";}
					catch(err){}
					try{window.location = cfg["aide"]["packer"]+"?"+escape(rspJson);}
					catch(err){}
				}else{
					try{tasker['isDown']="1";}
					catch(err){}
					try{window.DmqbWebView.StartTask(rspJson);}
					catch(err){}
				};
			}catch(err){}
		}catch(err){}
	}
};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*开始试玩,打开APP
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var TryApplication = function()
{
	if(tasker==undefined){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(tasker==null){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(typeof(tasker)!='object'){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(tasker['isDown']==undefined){ShowMessager({"text":"您还没有开始任务,请点击开始任务!"});return false;}
	else if(tasker['isDown']==null){ShowMessager({"text":"您还没有开始任务,请点击开始任务!"});return false;}
	else if(tasker['isDown']!="1"){ShowMessager({"text":"您还没有开始任务,请点击开始任务!"});return false;}
	else if(tasker['appmodel']==undefined){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else if(tasker['appmodel']==null){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else{
		if(cfg["aide"]["devicetype"]!="android")
		{
			try{
				var options = {};
				options["data"] = {
					"identify":tasker["packername"],
					"packagename":tasker["processname"],
					"appid":tasker["appid"],
					"istype":tasker["appmodel"],
					"appkey": tasker["appkey"]
				};
				options["back"] = function(){}
				Assistant["ajax"]("wake",options);
			}catch(err){}
			try{tasker['isTry']=true;}
			catch(err){}
		}
		else{
			if(window.DmqbWebView==undefined){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else if(window.DmqbWebView==null){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else if(typeof(window.DmqbWebView)!='object'){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else if(window.DmqbWebView.OpenApp==undefined){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else if(window.DmqbWebView.OpenApp==null){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else{
				try{window.DmqbWebView.OpenApp("{\"appkey\":"+tasker["appkey"]+",\"packername\":"+tasker["packername"]+",\"package\":"+tasker["processname"]+",\"trydate\":"+tasker["trydate"]+"}");}
				catch(err){ShowMessager({"text":"打开APP过程中发生错误,请重试！"});return false;}
				try{tasker['isTry']=true;}
				catch(err){}
			}
		}	
	}
};
	var wakeupapp=function(){
	//tasker['isTry']=true;
	//alert(tasker["packername"]);
	window.location.href=tasker["processname"];
	
				//StartDownload();
	//TryApplication();
	};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*点击领取奖励事件
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var FinishApplication = function()
{
	/***********************************************************************************
	*将数据发送到小助手
	************************************************************************************/
	var SaveVerification = function()
	{
		var options = {};
		options["data"] = {
			"identify":tasker["packername"],
			"packagename":tasker["processname"],
			"appid":tasker["appid"],
			"istype":tasker["appmodel"],
			"appkey": tasker["appkey"]
		};
		options["error"] = function(rspJson)
		{
			try{
				if(rspJson['tips'].indexOf('还未达到任务时间')!=-1)
				{
					try{
						Assistant["dialog"]({
							"tips":"试玩时间还不够,请按照规定的时间要求进行试玩！",
							"icon":"wait",
							"confirmText":"继续试玩",
							"click":function(){TryApplication();}
						});	
					}catch(err){}
				}
				else if((rspJson['tips'].indexOf('到达要求时间')!=-1 
				|| rspJson['tips'].indexOf('使用到达规定时间')!=-1)){
					try{
						Assistant["dialog"]({
							"tips":"您刚才的操作未达到任务要求,任务时间已重置请务必按照任务要求(打开时长/注册)！",
							"icon":"waiting",
							"confirmText":"继续试玩",
							"click":function(){TryApplication();}
						});	
					}catch(err){}
				}else if((rspJson['tips'].indexOf('请尝试其它任务')!=-1 
				|| rspJson['tips'].indexOf('请尝试其他任务')!=-1)){
					try{
						Assistant["dialog"]({
							"tips":"您的操作和信息仍然无法达到开发商要求完成任务失败，请尝试其他任务",
							"icon":"fail",
							"confirmText":"继续做任务",
							"click": function () { OpenFrame('app.aspx?token=' + cfg["user"]["strtokey"] + '&taskerModel=' + encodeURI(jQuery.cookies("taskerModel"))); }
						});	
					}catch(err){}
				}
				else{ShowMessager({"text":rspJson['tips']});return false;}
			}catch(err){}
		};
		options["back"] = function(rspJson)
		{
			if(tasker['appmodel']=="4"){
				try{HOTSubmit();}
				catch(err){}
			}else{
				Assistant["dialog"]({
					"tips":"提交成功，请等待任务奖励发放!",
					"icon":"finish",
					"confirmText":"继续做任务",
					"click": function () { OpenFrame('app.aspx?token=' + cfg["user"]["strtokey"] + '&taskerModel=' + encodeURI(jQuery.cookies("taskerModel"))); }
				});	
			};	
		}
		/***********************************************************************************
		*发送异步请求数据信息
		************************************************************************************/
		if(Assistant!=undefined && Assistant!=null && typeof(Assistant)=='object'
		&& Assistant["ajax"]!=undefined && Assistant["ajax"]!=null){
			try{Assistant["ajax"]("verification",options);}
			catch(err){}	
		}
	};
	/***********************************************************************************
	*开始验证提交数据的合法性
	************************************************************************************/
	if(tasker==undefined){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(tasker==null){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(typeof(tasker)!='object'){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(tasker['isDown']==undefined){ShowMessager({"text":"您还没有开始任务或二次唤醒APP,请点击开始任务!"});return false;}
	else if(tasker['isDown']==null){ShowMessager({"text":"您还没有开始任务或二次唤醒APP,请点击开始任务!"});return false;}
	else if(tasker['isDown']!="1"){ShowMessager({"text":"您还没有开始任务或二次唤醒APP,请点击开始任务!"});return false;}
	else if(tasker['appmodel']==undefined){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else if(tasker['appmodel']==null){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else{
		if(cfg["aide"]["devicetype"]!="android")
		{
			try{SaveVerification();}
			catch(err){}
		}
		else{
			if(window.DmqbWebView==undefined){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else if(window.DmqbWebView==null){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else if(typeof(window.DmqbWebView)!='object'){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else if(window.DmqbWebView.SubmitTask==undefined){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else if(window.DmqbWebView.SubmitTask==null){ShowMessager({"text":"SDK调用方法不存在！"});return false;}
			else{
			try{window.DmqbWebView.SubmitTask("{\"appkey\":"+tasker["appkey"]+",\"packername\":"+tasker["packername"]+",\"package\":"+tasker["processname"]+"}");}
			catch(err){ShowMessager({"text":"打开APP过程中发生错误,请重试！"});return false;}
			}
		}	
	};
};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*用户点击提交审核按钮事件
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var FinishHOT = function()
{
	if(tasker==undefined){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(tasker==null){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(typeof(tasker)!='object'){ShowMessager({"text":"获取应用配置错误,请重试！"});return false;}
	else if(tasker['isDown']==undefined){ShowMessager({"text":"您还没有开始任务,请点击开始任务!"});return false;}
	else if(tasker['isDown']==null){ShowMessager({"text":"您还没有开始任务,请点击开始任务!"});return false;}
	else if(tasker['isDown']!="1"){ShowMessager({"text":"您还没有开始任务,请点击开始任务!"});return false;}
	else if(tasker['appmodel']==undefined){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else if(tasker['appmodel']==null){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else if(tasker['advmodel']==undefined){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else if(tasker['advmodel']==null){ShowMessager({"text":"获取下载任务应用类型失败,请重试!"});return false;}
	else{
		if(tasker['appmodel']=="4"){
			try{FinishApplication();}
			catch(err){}
		}else{
			try{HOTSubmit();}
			catch(err){}	
		}
	};
}

/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*用户上传任务交易截图
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var doUpload = function(tar)
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
	/***********************************************************************************************
	*开始上传图片
	************************************************************************************************/
	var upload = function()
	{
 		var FormerData = new FormData();
		FormerData.append("Filename","blob.png");
        FormerData.append("File",tar.files[0]);
		var SendOptions = {};
		SendOptions['url'] = "../api/app.aspx?action=upload&token="+cfg["user"]["strtokey"];
		SendOptions['type'] = 'POST';
		SendOptions['dataType'] = 'json';
		SendOptions['data'] = FormerData;
		SendOptions['async'] = false;
		SendOptions['cache'] = false;
		SendOptions['contentType'] = false;
		SendOptions['processData'] = false;
		SendOptions['success'] = function(rspJson)
		{
			try{animation("hide");}catch(err){}
			if(rspJson==undefined){putMessager('数据返回格式不合法,请重试！');return false;}
			else if(rspJson==null){putMessager('数据返回格式不合法,请重试！');return false;}
			else if(typeof(rspJson)!='object'){putMessager('数据返回格式不合法,请重试！');return false;}
			else if(rspJson['success']==undefined){putMessager('数据返回格式不合法,请重试！');return false;}
			else if(rspJson['success']==null){putMessager('数据返回格式不合法,请重试！');return false;}
			else if(rspJson['success']!='true'){putMessager('数据返回格式不合法,请重试！');return false;}
			else if(rspJson['url']==undefined){putMessager('获取返回图片地址失败,请重试！');return false;}
			else if(rspJson['url']==null){putMessager('获取返回图片地址失败,请重试！');return false;}
			else if(rspJson['url']==""){putMessager('获取返回图片地址失败,请重试！');return false;}
			else{
				var frmTemplate=$("<div file=\""+rspJson["url"]+"\" operate=\"show\"><img class=\"thumb\" src=\""+rspJson["url"]+"\" /><span operate=\"del\">&times;</span></div>")[0];
				/**********************************************************************************
				*为图片添加删除事件
				***********************************************************************************/
				try{
					$(frmTemplate).find("span[operate=\"del\"]").click(function(){
						ShowConfirm({"text":"您确定要移除当前上传的截图吗?","back":function(isConfirm){
							if(isConfirm){
								try{$(frmTemplate).remove();}
								catch(err){}
							};												  
						}});												
					});
					/**********************************************************************************
					*为图片增加点击高亮事件
					***********************************************************************************/
					$(frmTemplate).click(function(){
						try{$("#UPLContianer").find("div[operate=\"show\"]").removeClass("cur");}
						catch(err){}
						try{$(frmTemplate).addClass("cur");}
						catch(err){}	
					});
				}catch(err){}
				
				/**********************************************************************************
				*将图片绑定到网页当中
				***********************************************************************************/
				try{$("#UPLContianer").prepend(frmTemplate);}
				catch(err){}
			}
		};
		SendOptions['error'] = function(errJson)
		{
			try{animation("hide");}catch(err){}
			putMessager('上传过程中发生错误,请重试！');
			return false;
		};
		/********************************************************************************************
		*开始上传数据
		*********************************************************************************************/
		if(SendOptions!=undefined && SendOptions!=null && typeof(SendOptions)=='object'
		&& window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
		&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null 
		&& typeof(window.jQuery.ajax)=='function')
		{
			try{window.jQuery.ajax(SendOptions);}
			catch(err){}
		}
		 
	};
	/********************************************************************************************
	*输出错误处理数据信息
	*********************************************************************************************/
	var putMessager = function(errMessager)
	{
		try{animation("hide");}catch(err){}
		try{ShowMessager({'text':errMessager});}
		catch(err){}
	};
	/******************************************************************************
	*验证上传文件列表合法性多个,所有文件
	*******************************************************************************/
	var VerificationFiles = function(arrFormat)
	{
		var errMessager = 'success';
		/**************************************************************************
		*开始执行数据处理
		***************************************************************************/
		try{
			$(arrFormat).each(function(k,arrJson){
				try{
					if(arrJson['type']==undefined){errMessager='获取文件格式失败';return false;}
					else if(arrJson['type']==null){errMessager='获取文件格式失败';return false;}
					else if(arrJson['type']==''){errMessager='获取文件格式失败';return false;}
					else if(!arrJson['type'].match('image.*')){errMessager='必须上传图片格式文件';return false;}
				}
				catch(err){}
				var Filesize = parseInt(arrJson['size']) || 0;
				if(Filesize<0){errMessager='获取文件大小失败,请重试!';return false;}
				else if(Filesize>=(1024*1024*5)){errMessager='单张图片大小不能超过2M!';return false;}
			});
		}
		catch(err){errMessager=err.message;}
		/**************************************************************************
		*返回数据处理结果
		***************************************************************************/
		return errMessager;
	};
	/******************************************************************************
	*显示上传文件动画
	*******************************************************************************/
	try{animation();}catch(err){}
	/******************************************************************************
	*开始验证上传文件基本信息
	*******************************************************************************/
	if(tar.files==undefined){putMessager('请选择需要上传的文件！');return false;}
	else if(tar.files==null){putMessager('请选择需要上传的文件！');return false;}
	else if(typeof(tar.files)!='object'){putMessager('请选择需要上传的文件！');return false;}
	else if(tar.files.length==undefined){putMessager('请选择需要上传的文件！');return false;}
	else if(tar.files.length==null){putMessager('请选择需要上传的文件！');return false;}
	else if(tar.files.length<=0){putMessager('请选择需要上传的文件！');return false;}
	else if(tar.files.length>=2){putMessager('单次最多只能上传1张图片！');return false;}
	else if(tar.files[0]==undefined){putMessager('获取上传文件信息失败,请重试！');return false;}
	else if(tar.files[0]==null){putMessager('获取上传文件信息失败,请重试！');return false;}
	else if(typeof(tar.files[0])!='object'){putMessager('获取上传文件信息失败,请重试！');return false;}
	/******************************************************************************
	*验证图片格式以及尺寸信息
	*******************************************************************************/
	var errMessager = VerificationFiles(tar.files);
	if(errMessager==undefined){putMessager('获取文件验证信息失败！');return false;}
	else if(errMessager==null){putMessager('获取文件验证信息失败！');return false;}
	else if(errMessager==''){putMessager('获取文件验证信息失败！');return false;}
	else if(errMessager!="success"){putMessager(errMessager);return false;}
	/******************************************************************************
	*验证上传图片数量
	*******************************************************************************/
	var length = $("#UPLContianer").find("div[operate=\"show\"]").length;
	if(length==undefined){putMessager('获取上传文件数量失败！');return false;}
	else if(length==null){putMessager('获取上传文件数量失败！');return false;}
	else if(length>=3){putMessager('您已经上传了超过3张图片,不允许再上传了！');return false;}
	/******************************************************************************
	*开始上传图片信息
	*******************************************************************************/
	try{upload();}
	catch(err){}	
};
/****************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*用户提交截图审核功能
☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*****************************************************************************************/
var HOTSubmit = function()
{
	/********************************************************************************************
	*输出错误处理数据信息
	*********************************************************************************************/
	var putMessager = function(errMessager)
	{
		try{animation("hide");}catch(err){}
		try{ShowMessager({'text':errMessager});}
		catch(err){}
	};
	/***********************************************************************************
	*验证手机号码的合法性
	************************************************************************************/
	var frmMobile = document.querySelector("#frmMobile").value;
	if(frmMobile==undefined){putMessager("请填写您的手机号！");return false;}
	else if(frmMobile==null){putMessager("请填写您的手机号！");return false;}
	else if(frmMobile==""){putMessager("请填写您的手机号！");return false;}
	else if(frmMobile.length<=0){putMessager("请填写您的手机号！");return false;}
	else if(frmMobile.length>=30){putMessager("手机号长度不能超过30个字符！");return false;}
	/***********************************************************************************
	*验证用户名的合法性
	************************************************************************************/
	var frmName = document.querySelector("#frmName").value;
	if(frmName==undefined){putMessager("请填写您的姓名！");return false;}
	else if(frmName==null){putMessager("请填写您的姓名！");return false;}
	else if(frmName==""){putMessager("请填写您的姓名！");return false;}
	else if(frmName.length<=0){putMessager("请填写您的姓名！");return false;}
	else if(frmName.length>=30){putMessager("姓名长度不能超过30个字符！");return false;}
	/***********************************************************************************
	*获取用户上传的截图认证信息
	************************************************************************************/
	var length = $("#UPLContianer").find("div[operate=\"show\"]").length;
	if(length==undefined){putMessager("您还没有上传截图！");return false;}
	else if(length==undefined){putMessager("您还没有上传截图！");return false;}
	else if(length<=0){putMessager("您还没有上传截图！");return false;}
	else if(length>=4){putMessager("最多只允许上传3张截图！");return false;}
	var strXml = "";
	strXml += "<configurationRoot>";
	$("#UPLContianer").find("div[operate=\"show\"]").each(function(){
		var fileUrl = $(this).attr("file") || "";
		if(fileUrl!=undefined && fileUrl!=null && fileUrl!="")
		{strXml += "<file value=\""+fileUrl+"\" />";}
	});
	strXml += "</configurationRoot>";
	/***********************************************************************************
	*开始保存请求数据
	************************************************************************************/
	var options = {};
	options["url"] = "../api/app.aspx?action=submission&token="+cfg["user"]["strtokey"];
	options["type"] = "POST";
	options["data"] = {
		"strMobile":frmMobile,
		"strname":frmName,
		"strxml":strXml,
		"appid":tasker["appid"],
		"appkey":tasker["appkey"]
	};
	options["back"] = function(rspJson){
		if(rspJson==undefined){putMessager('数据返回格式不合法,请重试！');return false;}
		else if(rspJson==null){putMessager('数据返回格式不合法,请重试！');return false;}
		else if(typeof(rspJson)!='object'){putMessager('数据返回格式不合法,请重试！');return false;}
		else if(rspJson['success']==undefined){putMessager('数据返回格式不合法,请重试！');return false;}
		else if(rspJson['success']==null){putMessager('数据返回格式不合法,请重试！');return false;}
		else if(rspJson['success']!='true'){putMessager('数据返回格式不合法,请重试！');return false;}
		else{
			try{
				ToMessager({
				   "text":"您的截图信息已经提交,请等待开发商审核！",
				   "type":"success",
				   "url": 'app.aspx?token=' + cfg["user"]["strtokey"] + '&taskerModel=' + jQuery.cookies("taskerModel")
				});
			}catch(err){}
		};
	};
	/***********************************************************************************
	*开始提交数据请求
	************************************************************************************/
	if(options!=undefined && options!=null && typeof(options)=="object"
	&& options["url"]!=undefined && options["url"]!=null 
	&& options["url"]!=""){
		try{getResponse(options);}
		catch(err){}	
	}
};
/***********************************************************************************
*提示放弃任务信息
************************************************************************************/
var ShowCancel = function()
{
	try{
		ShowConfirm({
			"text":"任务放弃后要重新抢夺,你确定要放弃任务吗?",
			"back":function(isConfirm){
				try{if(isConfirm){AbortTask(tasker["appid"]);}}
				catch(err){}
			}
		});	
	}catch(err){}
};

/***********************************************************************************
*获取指定Xml
************************************************************************************/
var cfgParams=function(strXml,domName)
{
	try{
		var strValue = $(strXml).find(domName).html();
		strValue = strValue.replace("<!--[CDATA[","");
		strValue = strValue.replace("]]-->","");
		return strValue;
	}catch(err){return "";}
};

/**********************************************************************
*获取指定号码的请求数据信息
***********************************************************************/
var XmlFormatJson = function(strXml)
{
	/**********************************************************************
	*获取到节点的值
	***********************************************************************/
	var getValue = function(node)
	{
		var strValue = node.innerHTML || "";
		strValue = strValue.replace("<!--[CDATA[","");
		strValue = strValue.replace("]]-->","");
		return strValue;	
	};
	var domJson = {};
	domJson["map"] = {};
	domJson["getValue"] = function(strname){
		try{
			try{strname = (strname.toString().toLowerCase() || "");}
			catch(err){}
			var strvalue = domJson["map"][strname] || "";
			if(strvalue==undefined){strvalue="";}
			else if(strvalue==null){strvalue="";}
			return strvalue;
		}
		catch(err){return "";}
	};
	domJson["setValue"] = function(strname,strvalue)
	{
		try{strname = (strname.toString().toLowerCase() || "");}
		catch(err){}
		try{domJson["map"][strname] =(strvalue || "");}
		catch(err){}
	};
	/**********************************************************************
	*开始执行数据请求
	***********************************************************************/
	try{
		if(strXml!=undefined && strXml!=null && strXml!="" 
		&& strXml.indexOf('<configurationRoot>')!=-1 
		&& strXml.indexOf('</configurationRoot>')!=-1)
		{
			$(strXml).children().each(function(k,node){
				try{
					var nodeName = 	node.tagName || "";
					var nodeValue = getValue(node) || "";
					if(nodeName!=undefined && nodeName!=null && nodeName!=""
					&& nodeValue!=undefined && nodeValue!=null && nodeValue!=""){
						try{domJson["setValue"](nodeName,nodeValue);}
						catch(err){} 
					}
				}catch(err){}
			});
		};
	}catch(err){}
	/**********************************************************************
	*返回数据处理结果
	***********************************************************************/
	return domJson;
};