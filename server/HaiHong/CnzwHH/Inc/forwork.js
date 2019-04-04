var App = null;
try{
App=parent.app;;	
}catch(err){App="/";}

var SiteID = 0;
try{
SiteID=top.app;
if(SiteID==undefined || isNaN(SiteID)){SiteID=0;}
}catch(err){SiteID=0;}
var Location = "";
try{
Location=top.res;
if(Location==undefined){Location="";}
}catch(err){Location="";}
/******************************************************************
*加载服务器缓存的查XML数据
*******************************************************************/
$.loadXml=function(url,callback){
	$.ajax({url:Location+url,async: false,type:"get",success:function(xml){																
		if(callback!=undefined && callback!=null){
			callback(xml);
		};																
	}});
}

/****************************************************************************
验证表单提交内容
******************************************************************************/
var _doPost=function(thisObject){
	var isTrue = true;
	for(var k=0;k<thisObject.elements.length;k++){
		var Element = thisObject.elements[k];
		if($(Element).attr("notkong")=="true" && $(Element).val()==""){
			$(Element).attr("complate","true");$(Element).focus();isTrue=false;break;
		}else{$(Element).removeAttr("complate");}
		if($(Element).attr("notkong")=="true" && $(Element).val()==$(Element).attr("char")){
			$(Element).attr("complate","true");;isTrue=false;break;
		}else{$(Element).removeAttr("complate");}
		if($(Element).attr("isnumeric")=="true" && isNaN($(Element).val())){
			$(Element).attr("complate","true");isTrue=false;break;
		}else{$(Element).removeAttr("complate");}
		if($(Element).attr("isdate")=="true" && !CheckDateTime($(Element).val())){
			$(Element).attr("complate","true");isTrue=false;break;
		}else{$(Element).removeAttr("complate");}
	}
	if(isTrue){$(thisObject).find("input[type=submit]").attr("disabled",'true');}
	return isTrue;
}

var GetFormersChecked=function(form)
{
	var Id="0";
	for(var n=0;n<form.elements.length;n++){
		if(form.elements[n].type=='checkbox' 
		&& form.elements[n].checked 
		&& form.elements[n].value!='on')
		{
			Id=Id+","+form.elements[n].value;	
		}
	}
	return Id;	
};

function CheckDateTime(str){
	var reg = /^(\d+)-(\d{1,2})-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$/;
	var r = str.match(reg);
	if(r==null)return false;
	r[2]=r[2]-1;
	var d= new Date(r[1], r[2],r[3], r[4],r[5], r[6]);
	if(d.getFullYear()!=r[1])return false;
	if(d.getMonth()!=r[2])return false;
	if(d.getDate()!=r[3])return false;
	if(d.getHours()!=r[4])return false;
	if(d.getMinutes()!=r[5])return false;
	if(d.getSeconds()!=r[6])return false;
	return true;
};

var deleteOperate=function(obj){
	var strText=$(obj).attr("textMessage");
	if(strText==undefined){strText='数据删除以后将无法恢复，你确定要删除？';}
	var cmdText=$(obj).attr("cmdText");
	if(cmdText==undefined){cmdText='del';}
	if(!confirm(strText)){return false;}else{
		var Id=GetFormersChecked(obj.form);
		if(Id==0){alert("请选择一条数据！");return false;}
		obj.form.action="?action="+cmdText;
		try{
			ajaxSubmit(obj.form,function(){
				for(var n=0;n<obj.form.elements.length;n++){
					if(obj.form.elements[n].type=='checkbox' && obj.form.elements[n].checked && obj.form.elements[n].value!='on'){
					    $(obj.form.elements[n]).parents("tr").remove();
					    window.location.reload();
					}
				}
				if($("tr.hback").length<=1){window.location.reload();}
			});
		}
		catch(err){obj.form.submit();}
	}
};

var commandOperate=function(obj){
	var strText=$(obj).attr("textMessage");
	if(strText==undefined){strText='你确定要执行该操作？';}
	var cmdText=$(obj).attr("cmdText");
	if(cmdText==undefined){alert("操作有误，请单独设置");return false;}
	var sendText=$(obj).attr("sendText") || "";
	if(sendText==undefined){sendText='';}
	var notselect = $(obj).attr("notselect") || "false";
	var theOne = $(obj).attr("theOne") || "false";
	if(!confirm(strText)){return false;}
	else{
		if(theOne!=undefined && theOne=="true")
		{
			var selected = "";
			try{
				var form = obj.form;
				for(var n=0;n<form.elements.length;n++){
					if(form.elements[n].type=='checkbox' 
					&& form.elements[n].checked 
					&& form.elements[n].value!='on')
					{
						if(selected!=""){selected=selected+","+form.elements[n].value;}
						else{selected=form.elements[n].value;	}
					}
				}
			}catch(err){}
			try{
				if(selected==""){alert("请选择一条数据！");return false;}
				if(selected!="" && selected.indexOf(',')!=-1){alert("只能选择一条数据！");return false;}
				var arr =selected.split(",");
				if(arr.length!=1){alert("只能选择一条数据！");return false;}
			}catch(err){}
			try{
				obj.form.action="?action="+cmdText+"&val="+sendText;
				obj.form.submit();
			}catch(err){}
			
		}else
		{
			var Id=GetFormersChecked(obj.form);
			if(Id==0 && notselect!="true"){
				alert("请选择一条数据！");
				return false;
			}
			obj.form.action="?action="+cmdText+"&val="+sendText;
			obj.form.submit();
		}
	}
};


$(function(){
	$("input[isnumeric='true']").keyup(function(){
		if(isNaN(this.value)){this.value=0;}	
	});
	$("input[notkong='true']").blur(function(){
		try{
			if($(this).val()==""){$(this).attr("complate","true");;
			}else{$(this).removeAttr("complate","true");}
		}catch(err){}
	});
	$("input[notkong='true']").focus(function(){
		try{
			if($(this).val()=="不能为空" || $(this).val()==$(this).attr("char")){
				this.value='';
			};
		}catch(err){}
	});
	$("input[operate='selectList']").click(function(){
		var form=this.form;
		for(var n=0;n<form.elements.length;n++){
			if(form.elements[n].type=='checkbox'){
				form.elements[n].checked=this.checked;
			}
		}
	});
	
	$("a[operate='delete']").click(function(){
		var strText=$(this).attr("textMessage");
		if(strText==undefined){strText='数据删除以后将无法恢复，你确定要删除？';}
		if(!confirm(strText)){return false;	}
		else{
			try{
				var thisTable = $(this).parents("table")[0]; 
				var thisObject = this;
				var url = $(thisObject).attr("href");
				if(url!=undefined && url!=""){
					url = url+"&isAsyn=1";
					$.ajax({url:url,type:"get",async: true,
						success:function(strXml){
							var strJson = $.parseJSON(strXml);
							if(strJson["success"]!=undefined && strJson["success"]=="true"){
								$(thisObject.parentNode.parentNode).remove();
								if(thisTable!=undefined && $(thisTable).find("tr.hback").length<=1){window.location.reload();}
							}else{
								alert(strJson["tips"]);return false;		
							}
							
						},
						error:function(){
							alert('发生未知错误!');return false;														
						}});
				}
				return false;
			}catch(err){return true;}
		}
	});
	$("input[operate='delete']").click(function(){
		/*****************************************************************
		*开始处理删除信息
		******************************************************************/
		var strMessage=$(this).attr("textMessage") || "";
		if(strMessage==undefined || strMessage==null || strMessage=="")
		{strMessage='数据删除以后将无法恢复，你确定要删除？';}
		var url = $(this).attr("url") || "";
		if(url==undefined || url=="" || url==null){alert('发生未知错误,请重试!');return false;}
		/*****************************************************************
		*开始判断删除信息
		******************************************************************/
		try{
			if(confirm(strMessage) && url!=undefined && url!="")
			{
				$.ajax({
					url:url+"&isAsyn=1&target=sel",type:"get",async: true,
					success:function(strResponse)
					{
						var options = {};
						try{options = jQuery.parseJSON(strResponse) || {};}catch(err){}
						try{messagerAlert(options);}catch(err){}
					},
					error:function(){
						try{
							messagerAlert({"success":"false",tips:"数据处理中发生错误,请重试"});
						}catch(err){}	
					}
				});
			}
		}catch(err){alert(err.message);}	
	});
	/*****************************************************************
	*弹出确认处理层
	******************************************************************/
	$("input[operate='confirm']").click(function(){
		/*****************************************************************
		*开始处理删除信息
		******************************************************************/
		var strMessage=$(this).attr("textMessage") || "";
		if(strMessage==undefined || strMessage==null || strMessage=="")
		{strMessage='数据操作后可能无法恢复,你确定要执行操作？';}
		var url = $(this).attr("url") || "";
		if(url==undefined || url=="" || url==null){alert('发生未知错误,请重试!');return false;}
		/*****************************************************************
		*开始判断删除信息
		******************************************************************/
		try{
			if(confirm(strMessage) && url!=undefined && url!="")
			{
				$.ajax({
					url:url+"&isAsyn=1&target=sel",type:"get",async: true,
					success:function(strResponse)
					{
						var options = {};
						try{options = jQuery.parseJSON(strResponse) || {};}catch(err){}
						try{messagerAlert(options);}catch(err){}
					},
					error:function(){
						try{
							messagerAlert({"success":"false",tips:"数据处理中发生错误,请重试"});
						}catch(err){}	
					}
				});
			}
		}catch(err){alert(err.message);}	
	});
	
	
	$("input[operate=\"history\"]").click(function(){
		history.go(-1);	
	});
	$("li[operate=\"tipsChange\"]").click(function(){
		var parent=this.parentNode;
		for(var n = 0;n<parent.childNodes.length;n++){
			
			var change=$(parent.childNodes[n]).attr("change");
			$("tr[operate="+change+"]").hide();
			$(parent.childNodes[n]).removeClass("current");
		}
		$("tr[operate="+$(this).attr("change")+"]").show()
		$(this).addClass("current");
	});
	/*********************************************************************************
	*将数据显示在父框架上面
	**********************************************************************************/
	$("#frm-input").find("td[operate=\"selector\"]").find("select").change(function(){
		try
		{
			var strValue = $(this.options[this.selectedIndex]).text();
			if(strValue!=undefined && strValue!=null)
			{
				$(this.parentNode).attr("text",strValue);	
			}
		}catch(err){}
	});
	/*********************************************************************************
	*将数据显示在父框架上面
	**********************************************************************************/
	$("#frm-input").find("td[operate=\"date\"]").find("input[type=\"date\"]").change(function(){
		try
		{
			var strValue = $(this).val();
			if(strValue!=undefined && strValue!=null)
			{
				$(this.parentNode).attr("text",strValue);	
			}
		}catch(err){}
	});
	
	$("input[operate=chooseTemp]").click(function(){
		var thisObject = document.getElementById('frm-template-selector-box');
		var targetText = "";
		try{targetText=$(this).parents("td").find("input[type=text]").attr("id");}catch(err){alert("无法识别目标,请刷新网页重试！");return false;}
		if(targetText=="" || targetText==undefined){targetText="Thumb";}
		if(!document.getElementById('frm-template-selector-box')){
			thisObject = document.createElement('div');
			thisObject.id = "frm-template-selector-box";
			$(document.body).append(thisObject);
			$(thisObject).attr("title","选择模版");
			$(thisObject).css({"margin":"0px","padding":"0px","background":"#f0f0f0"});
			$(thisObject).html("<iframe id=\"selector-frm-template\" name=\"selectorTemplate\" src=\"template.aspx?action=stor&target="+targetText+"\" scrolling=\"no\" frameborder=\"0\" width=\"100%\" height=\"420\"></iframe>");
			selectorTemplate = $(this).parents("td").find("input[type=text]");
		}
		$(thisObject).dialog({width:600,height:460,modal:true,close:function(){$(thisObject).remove();}});
		
	})
	var selectorTemplate = undefined;
	/*选择图库*/
	$("#frm-image-selector-btn").click(function(){
		var thisObject = document.getElementById('frm-image-selector-box');
		var targetText = "";
		try{targetText=$(this).parents("table.uploader").find("input[type=text]").attr("id");}catch(err){alert("无法识别目标,请刷新网页重试！");return false;}
		if(targetText=="" || targetText==undefined){targetText="Thumb";}
		if(!document.getElementById('frm-image-selector-box')){
			thisObject = document.createElement('div');
			thisObject.id = "frm-image-selector-box";
			$(document.body).append(thisObject);
			$(thisObject).attr("title","选择图库");
			$(thisObject).css({"margin":"0px","padding":"0px","background":"#f0f0f0"});
			$(thisObject).html("<iframe src=\"File.aspx?action=stor&target="+targetText+"\" scrolling=\"no\" frameborder=\"0\" width=\"100%\" height=\"420\"></iframe>");
		}
		$(thisObject).dialog({width:640,height:460,modal:true,close:function(){$(thisObject).remove();}});
	});
	/**********************************************************************************
	*图片选择器变换以后资源数据变换
	***********************************************************************************/
	$("input[operate=\"frm-resources-txt\"]").change(function(){
		
	});
	$("td[operate=\"shrink\"]").click(function(){
		var text = $(this).attr("text");
		if(text==undefined || text==""){text="system";}
		if($(this).attr("shrink")!=undefined && $(this).attr("shrink")=="true"){
			$(this).parents("table").find("tr[operate=\""+text+"\"]").hide();
			$(this).attr("shrink","false");	
		}else{
			$(this).parents("table").find("tr[operate=\""+text+"\"]").show();
			$(this).attr("shrink","true");	
		}
	});
	$("th[operate=\"shrink\"]").click(function(){
		var text = $(this).attr("text");
		if(text==undefined || text==""){text="system";}
		if($(this).attr("shrink")!=undefined && $(this).attr("shrink")=="true"){
			$(this).parents("table").find("tr[operate=\""+text+"\"]").hide();
			$(this).attr("shrink","false");	
		}else{
			$(this).parents("table").find("tr[operate=\""+text+"\"]").show();
			$(this).attr("shrink","true");	
		}
	});
	$("tr.hback").hover(
		function(){$(this).addClass("hovers");},
		function(){$(this).removeClass("hovers");}
	);
	$("td[operate=copyText]").click(function(){
		copyToClipboard(this.innerText)	;				 
	});
	$("td.singlebtn").find("label").click(function(){
		$(this).parents("td.singlebtn").find("label").each(function(){
			$(this).removeClass("current");													 
		});
		$(this).addClass("current"); 
	});
	
	$("td.check_box").find("label").click(function(){
		try{
			var falg = $(this).find("input")[0].checked;
			if(falg){$(this).addClass("current");}
			else{$(this).removeClass("current");}
		}catch(err){}
	});
	$("input#Color").click(function(){
		document.getElementById("colorPalette").style.display='';							
	});
	/*****************************************************************************
	*滑块控件信息
	******************************************************************************/
	$("div[operate=\"range\"]").find("input[type=\"range\"]").change(function(){
		try{
			var thisIpt = $(this.parentNode).find("input[operate=\"text\"]")[0];
			if(thisIpt!=undefined && thisIpt!=null){
				$(thisIpt).val(parseFloat(this.value) || 0);	
			}
		}catch(err){}
	});
	/*****************************************************************************
	*滑块控件信息
	******************************************************************************/
	$("div[operate=\"range\"]").find("input[operate=\"text\"]").change(function(){
		try{
			var thisValue = parseFloat(this.value) || 0;
			try{
				var maxValue = parseFloat($(this.parentNode).attr("max")) || 0;
				if(thisValue>=maxValue){thisValue=maxValue;}
			}catch(err){}
			try{
				var minValue = parseFloat($(this.parentNode).attr("min")) || 0;
				if(thisValue<=minValue){thisValue=minValue;}
			}catch(err){}
			/***********************************************************
			*设置滑块进度
			*************************************************************/
			var thisIpt = $(this.parentNode).find("input[operate=\"range\"]")[0];
			if(thisIpt!=undefined && thisIpt!=null){
				$(thisIpt).val(thisValue);	
			}
			/***********************************************************
			*设置当前控件的值
			*************************************************************/
			try{$(this).val(thisValue);	}catch(err){}
		}catch(err){}
	});
	
	/************************************************************************************
	*查看各种订单详情信息
	*************************************************************************************/
	$("td[operate=\"looker\"]").click(function(){
		try{
			var url=$(this).attr("url") || "";
			var title = $(this).attr("title") || '查看订单';
			if(url!=undefined && url!="" && title!=undefined && title!="")
			{
				try{
					/***********************************************************************
					*'funName':'looker',调用回调函数方法名称
					*'function':function(){alert('哈哈哈');},调用的回调函数对象
					************************************************************************/
					var options = {
							url:url,
						   	title:'查看订单',
						   	width:650,
							'funName':'looker',
							'function':function(){alert('哈哈哈');}
					}
					if(top.SliderWindows!=undefined && top.SliderWindows!=null
					&& typeof(top.SliderWindows)=='function')
					{
						top.SliderWindows(options);
					}else if(parent.SliderWindows!=undefined && parent.SliderWindows!=null
					&& typeof(parent.SliderWindows)=='function')
					{
						parent.SliderWindows(options);	
					}else
					{
						options["height"]=550;
						options["modal"]=true;
						PopupContianer(options);	
					}
				}catch(err)
				{
					alert('发生未知错误,请刷新网页或切换浏览器！');
					return false;
				}
			}else{
				$.messager.alert('系统提示','发生未知错误,请重试！');
				return false;	
			}
		}catch(err){}
	});
	
	$("input[operate=\"frmContent\"]").click(function(){
		var url = $(this).attr("url") || "";
		if(url!=undefined && url!="" && top.LoadContent!=undefined 
		&& top.LoadContent!=null && typeof(top.LoadContent)=='function')
		{
			try{top.LoadContent(url);}catch(err){}
		}
		else if(url!=undefined && url!=""  && 
		parent.frmContent!=undefined && parent.frmContent!=null 
		&& typeof(parent.frmContent)=='window')
		{
			try{parent.LoadContent(url);}catch(err){}
		}
		else if(url!=undefined && url!="" 
		&& parent.frmContent!=undefined 
		&& typeof(parent.frmContent)=='window')
		{
			try{
				parent.frmContent.document.url=url;	
			}catch(err){}
		}
	});
	
	$("td[operate=\"chContianer\"]").find("span[operate=\"change\"]").click(function(){
		
		try{$(this.parentNode).find("span[operate=\"change\"]").removeClass("current");}
		catch(err){}
		try{$(this).addClass("current");}
		catch(err){}
		try{
			$(this.parentNode).find("span[operate=\"change\"]").each(function(){
				var change=$(this).attr("text");
				$("tr[operate="+change+"]").hide();																			
			});
		}catch(err){}
		try{$("tr[operate="+$(this).attr("text")+"]").show();}
		catch(err){}
		
	});
	
	$("input[operate=\"close\"]").click(function(){
		
		try{
			if(window.SliderContianer!=undefined && window.SliderContianer!=null 
			&& typeof(window.SliderContianer)=='function')
			{
				SliderContianer('close');	
			}
		}catch(err){}

	});
	
	/*************************************************************************************
	*发送短信验证码按钮信息
	**************************************************************************************/
	$("input[operate='captcha']").click(function(){
		var text = $(this).attr("text") || 'reload';
		var url = $(this).attr('url');
		if(url==undefined || url==''){$("#frm-tipxbox").tips({'tips':'请求参数错误,请重试'});return false;}
		if(text!=undefined && text=='reload' 
		&& url!=undefined && url!='' && this!=undefined && this!=null)
		{
			var $this = this;
			try{
				$.ajax({
					url:url,type:'get',
					success:function(strResponse){
						if(strResponse!='success'){
							$("#frm-tipxbox").tips({'tips':strResponse});return false;
						}else{
							try{
								$($this).attr('text','unload');
								$($this).val('验证码发送成功');
								var $timer = 60;
								var $obj = setInterval(function(){
									if($timer>=1){
										$timer=$timer-1;
										$($this).val('等待'+$timer+'秒重新获取');
									}
									else{
										clearInterval($obj);
										$($this).attr('text','reload');
										$($this).val('重新获取验证码');
									}
								},1000);
							}catch(err){}
						}										
					}
				});
			}catch(err){}
		}
	});
	/*************************************************************************************
	*上传图片及时预览功能
	**************************************************************************************/
	$("div[operate=\"frmUpload\"]").find("input[operate=\"file\"]").change(function(evt){
		try{
			if(window.FileReader && this!=undefined && this!=null)
			{
				var $this = this;
				var files = evt.target.files;  
				for (var i = 0, f; f = files[i]; i++) {  
					if (!f.type.match('image.*')) {continue; }  
					var reader = new FileReader();
					reader.onload = (function(theFile) {  
						return function(e) 
						{
							if($this.parentNode!=undefined && $this.parentNode!=null)
							{
								try
								{
									$this.parentNode.style="background:url("+e.target.result+");background-repeat:repeat; background-position:center; background-size:100%;";
								}catch(err){}	
							}
						};  
					})(f);  
					reader.readAsDataURL(f);  
				}	
			}
		}catch(err){alert(err.message);}
	});
});
var CloseDialog=function(id){
	try{if(document.getElementById(id)!=null){$("#"+id).dialog("close");}
	}catch(err){alert(err.message);return false;}	
}

var browser = function(){
	var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
    var isOpera = userAgent.indexOf("Opera") > -1;
    if (isOpera) {
        return "Opera"
    }; //判断是否Opera浏览器
    if (userAgent.indexOf("Firefox") > -1) {
        return "FF";
    } //判断是否Firefox浏览器
    if (userAgent.indexOf("Chrome") > -1){
 		 return "Chrome";
 	}
    if (userAgent.indexOf("Safari") > -1) {
        return "Safari";
    } //判断是否Safari浏览器
    if (userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera) {
        return "IE";
    }; //判断是否IE浏览器
}


var CloseContianer = function()
{
	/*************************************************************************************************
	*开始调用网页对象信息
	**************************************************************************************************/
	try{
		if(parent!=undefined && parent.CloseMasker!=undefined && typeof(parent.CloseMasker)=='function'){
			parent.CloseMasker();	
		}else if(top!=undefined && top.CloseMasker!=undefined && typeof(top.CloseMasker)=='function'){
			top.CloseMasker();	
		}else if(ShowMasker!=undefined && typeof(ShowMasker)=='function'){
			CloseMasker();	
		}
	}catch(err){
		alert('关闭窗体过程中发生未知错误,'+err.message);
		return false;
	}
}
/*************************************************************************************************
*打开网页侧滑控制页面 options=close 关闭事件
**************************************************************************************************/
var SliderContianer = function(options)
{
	/*************************************************************************************************
	*预设格式化参数信息
	*'funName':'looker',调用回调函数方法名称
	*'function':function(){alert('哈哈哈');},调用的回调函象数对
	**************************************************************************************************/
	try{
		var options = options || {
			width:450,
			modal:true,
			closed:null,
			'funName':undefined,
			'function':function(){}
		};	
	}catch(err){}
	/*************************************************************************************************
	*开始调用网页对象信息
	**************************************************************************************************/
	try{
		if(top.SliderWindows!=undefined && top.SliderWindows!=null
		&& typeof(top.SliderWindows)=='function')
		{
			top.SliderWindows(options);
		}else if(parent.SliderWindows!=undefined && parent.SliderWindows!=null
		&& typeof(parent.SliderWindows)=='function')
		{
			parent.SliderWindows(options);	
		}else
		{
			options["height"]=550;
			options["modal"]=true;
			PopupContianer(options);	
		}
	}catch(err){
		$.messager.alert('系统提示','模块内容加载错误,请刷新网页重试！'+err.message);
		return false;
	}
};


var ShowDialog = function(Url,Width,Height,WindowObject)
{
	var my = browser();
	if(my=="IE"){
		var ReturnStr=showModalDialog(Url,WindowObj,'dialogWidth:'+Width+'pt;dialogHeight:'+Height+'pt;status:yes;help:no;scroll:yes;resize:yes');
		if (ReturnStr!='' && ReturnStr!=null) SetObj.value=ReturnStr;
		return ReturnStr;	
	}else{
		var newWindow=window.open (Url,'newwindow','height='+Height+',width='+Width+',toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no');	
	}
}

var setColor=function(color){
	try{
		$("#Color").val(color);
		$("#Color").css({"background":color});
		document.getElementById("colorPalette").style.display='none';	
	}catch(e){}	
};
/********************************************************************************
*设置返回地址
*********************************************************************************/
var SetHistory = function(url)
{
	try{
		pushHistory(); 
		window.addEventListener("popstate", function(e) { 
			window.location = url;
		}, false); 
		function pushHistory() 
		{ 
			window.history.pushState({"foo":"地址"}, "title", "#"); 
		};
	}catch(err){alert(111);}
};

var OpenFrame = function(url)
{
	try{
		if(window.ShowContianer!=undefined && window.ShowContianer!=null 
		&& typeof(window.ShowContianer)=='function'){
			try{window.ShowContianer({'url':url});	}
			catch(err){}
		}
		else if(parent.ShowContianer!=undefined && parent.ShowContianer!=null 
		&& typeof(parent.ShowContianer)=='function'){	
			try{parent.ShowContianer({'url':url});}
			catch(err){}
		}
		else if(top.ShowContianer!=undefined && top.ShowContianer!=null 
		&& typeof(top.ShowContianer)=='function'){	
			try{top.ShowContianer({'url':url});}
			catch(err){}
		}
		else{
			try{window.location = url;}
			catch(err){}
		}	
	}
	catch(err){}		
};


var PopupContianer = function(options)
{
	/*************************************************************************************************
	*开始调用网页对象信息
	**************************************************************************************************/
	if(options!=undefined && options!=null)
	{
		try{
			if(top.LayoutWindows!=undefined 
			&& top.LayoutWindows!=undefined 
			&& typeof(top.LayoutWindows)=='function')
			{
				top.LayoutWindows(options);	
			}
			else if(parent.LayoutWindows!=undefined 
			&& parent.LayoutWindows!=null 
			&& typeof(parent.LayoutWindows)=='function')
			{
				parent.LayoutWindows(options);	
			}
			else if(LayoutWindows!=undefined 
			&& LayoutWindows!=null 
			&& typeof(LayoutWindows)=='function'){
				LayoutWindows(options);	
			}
		}
		catch(err)
		{
			//ShowMessager({'text':'模块内容加载错误,请刷新网页重试！'+err.message});
			return false;
		}	
	}
};

var ShowLayout = function(options)
{
	try{PopupContianer(options);}
	catch(err){}
};
var ShowDislog = function(options)
{
	/*************************************************************************************************
	*预设格式化参数信息
	**************************************************************************************************/
	try{PopupContianer(options);}
	catch(err){}
};

var RightWindows = function(options)
{
	/*************************************************************************************************
	*开始调用网页对象信息
	**************************************************************************************************/
	if(options!=undefined && options!=null)
	{
		try{
			if(top.SliderWindows!=undefined 
			&& top.SliderWindows!=undefined 
			&& typeof(top.SliderWindows)=='function')
			{
				top.SliderWindows(options);	
			}
			else if(parent.SliderWindows!=undefined 
			&& parent.SliderWindows!=null 
			&& typeof(parent.SliderWindows)=='function')
			{
				parent.SliderWindows(options);	
			}
			else if(SliderWindows!=undefined 
			&& SliderWindows!=null 
			&& typeof(SliderWindows)=='function'){
				SliderWindows(options);	
			}
		}
		catch(err)
		{
			ShowMessager({'text':'模块内容加载错误,请刷新网页重试！'+err.message});
			return false;
		}		
	}
}

var ShowMessager = function(options)
{
	try{
		var options = options || {};
		try{options['title'] = options['title'] || '系统消息';}catch(err){}
		try{options['text'] = options['text'] || (options['text'] || '这里是系统消息');}catch(err){}
		try{options['icon'] = options['icon'] || 'warning';}catch(err){}
		try{options['fn'] = options['fn'] || function(){};}catch(err){}
		
		if(top.$.messager!=undefined && top.$.messager!=null 
		&& typeof(top.$.messager)=='object' && top.$.messager.alert!=undefined 
		&& top.$.messager.alert!=null && typeof(top.$.messager.alert)=='function')
		{
			try{top.$.messager.alert(options['title'],options['text'],options['icon'],options['fn']);}
			catch(err){}
		}
		else if(parent.$.messager!=undefined && parent.$.messager!=null 
		&& typeof(parent.$.messager)=='object' && parent.$.messager.alert!=undefined 
		&& parent.$.messager.alert!=null && typeof(parent.$.messager.alert)=='function')
		{
			try{parent.$.messager.alert(options['title'],options['text'],options['icon'],options['fn']);}
			catch(err){}
		}
		else if(window.$.messager!=undefined && window.$.messager!=null 
		&& typeof(window.$.messager)=='object' && window.$.messager.alert!=undefined 
		&& window.$.messager.alert!=null && typeof(window.$.messager.alert)=='function')
		{
			try{window.$.messager.alert(options['title'],options['text'],options['icon'],options['fn']);}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(options['text']);
		}
	}catch(err){alert(options['text']);return false;}
};

var ShowConfirm = function(options)
{
	try{
		var options = options || {};
		try{options['title'] = options['title'] || '系统消息';}catch(err){}
		try{options['text'] = options['text'] || '这里是系统消息';}catch(err){}
		try{options['fn'] = options['fn'] || function(isOK){};}catch(err){}
		
		if(top.$.messager!=undefined && top.$.messager!=null 
		&& typeof(top.$.messager)=='object' && top.$.messager.confirm!=undefined 
		&& top.$.messager.confirm!=null && typeof(top.$.messager.confirm)=='function')
		{
			try{top.$.messager.confirm(options['title'],options['text'],options['fn']);}
			catch(err){}
		}
		else if(parent.$.messager!=undefined && parent.$.messager!=null 
		&& typeof(parent.$.messager)=='object' && parent.$.messager.confirm!=undefined 
		&& parent.$.messager.confirm!=null && typeof(parent.$.messager.confirm)=='function')
		{
			try{parent.$.messager.confirm(options['title'],options['text'],options['fn']);}
			catch(err){}
		}
		else if(window.$.messager!=undefined && window.$.messager!=null 
		&& typeof(window.$.messager)=='object' && window.$.messager.confirm!=undefined 
		&& window.$.messager.confirm!=null && typeof(window.$.messager.confirm)=='function')
		{
			try{window.$.messager.confirm(options['title'],options['text'],options['fn']);}
			catch(err){}
		}
		else if(window.confirm!=undefined && window.confirm!=null 
		&& typeof(window.confirm)=='function')
		{
			try{
				if(window.confirm(options['text']))
				{options['fn'](true);}
				else{options['fn'](false);}
			}catch(err){}
		}
		
	}catch(err){alert(options);return false;}
};

var ShowPrompt = function(options)
{
	try{
		var options = options || {};
		try{options['title'] = options['title'] || '系统消息';}catch(err){}
		try{options['text'] = options['text'] || '这里是系统消息';}catch(err){}
		try{options['fn'] = options['fn'] || function(){};}catch(err){}
		
		if(top.$.messager!=undefined && top.$.messager!=null 
		&& typeof(top.$.messager)=='object' && top.$.messager.prompt!=undefined 
		&& top.$.messager.prompt!=null && typeof(top.$.messager.prompt)=='function')
		{
			try{top.$.messager.prompt(options['title'],options['text'],options['fn']);}
			catch(err){}
		}
		else if(parent.$.messager!=undefined && parent.$.messager!=null 
		&& typeof(parent.$.messager)=='object' && parent.$.messager.prompt!=undefined 
		&& parent.$.messager.prompt!=null && typeof(parent.$.messager.prompt)=='function')
		{
			try{parent.$.messager.prompt(options['title'],options['text'],options['fn']);}
			catch(err){}
		}
		else if(window.$.messager!=undefined && window.$.messager!=null 
		&& typeof(window.$.messager)=='object' && window.$.messager.prompt!=undefined 
		&& window.$.messager.prompt!=null && typeof(window.$.messager.prompt)=='function')
		{
			try{window.$.messager.prompt(options['title'],options['text'],options['fn']);}
			catch(err){}
		}
		else if(window.prompt!=undefined && window.prompt!=null 
		&& typeof(window.prompt)=='function' && options!=undefined 
		&& options!=null && typeof(options)=='object')
		{
			try{
				var inputValue = prompt(options['text']);
				if(inputValue!=undefined && inputValue!=null && typeof(inputValue)!='' 
				&& options['fn']!=undefined && options['fn']!=null 
				&& typeof(options['fn'])=='function'){
					options['fn'](inputValue);
				};
			}catch(err){}
		}
	}catch(err){alert(options);return false;}
};
/*********************************************************************************************
*选择下拉菜单,同步菜单文本到指定控件
**********************************************************************************************/
var doChange = function(obj,tar)
{
	/*********************************************************************************************
	*输出错误处理函数信息
	**********************************************************************************************/
	var PutMessage = function(errMessager)
	{
		try{ShowMessager({'text':errMessager});return false;}
		catch(err){}
	}
	/*********************************************************************************************
	*开始执行请求数据处理
	**********************************************************************************************/
	try{
		/*********************************************************************************************
		*获取指定的目标控件数据信息
		**********************************************************************************************/
		if(tar==undefined){PutMessage('获取目标控件信息失败,请重试！');return false;}
		else if(tar==null){PutMessage('获取目标控件信息失败,请重试！');return false;}
		else if(tar==''){PutMessage('获取目标控件信息失败,请重试！');return false;}
		if(typeof(tar)=='string'){tar = document.getElementById(tar);}
		if(tar==undefined){PutMessage('获取目标控件信息失败,请重试！');return false;}
		else if(tar==null){PutMessage('获取目标控件信息失败,请重试！');return false;}
		else if(typeof(tar)=='string'){PutMessage('获取目标控件信息失败,请重试！');return false;}
		/*********************************************************************************************
		*开始获取指定的文本内容信息
		**********************************************************************************************/
		if(obj==undefined){PutMessage('获取源目标控件信息失败,请重试！');return false;}
		else if(obj==null){PutMessage('获取源目标控件信息失败,请重试！');return false;}
		else if(obj['options']==undefined){PutMessage('获取源目标控件信息失败,请重试！');return false;}
		else if(obj['options']==null){PutMessage('获取源目标控件信息失败,请重试！');return false;}
		else if(obj.selectedIndex==undefined){PutMessage('获取源目标控件信息失败,请重试！');return false;}
		else if(obj.selectedIndex==null){PutMessage('获取源目标控件信息失败,请重试！');return false;}
		else if(obj['options'][obj.selectedIndex]==undefined)
		{PutMessage('获取源目标控件信息失败,请重试！');return false;}
		else if(obj['options'][obj.selectedIndex]==null)
		{PutMessage('获取源目标控件信息失败,请重试！');return false;}
		/*********************************************************************************************
		*开始执行数据处理
		**********************************************************************************************/
		try{
			var tarOptions = obj['options'][obj.selectedIndex];
			if(obj['value']!=undefined && obj['value']!=null 
			&& obj['value']!='' && tarOptions!=undefined 
			&& tarOptions!=null && tarOptions['text']!=undefined 
			&& tarOptions['text']!=null)
			{tar.value = tarOptions['text'];}
			else{tar.value='';}
		}
		catch(err){tar.value='';}
	}
	catch(err){}
}
