/******************************************************************************************
*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
*发送请求数据主入口函数信息
*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
*******************************************************************************************/
;var getResponse = function(options)
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
	/*********************************************************************
	*输出错误处理提示信息
	***********************************************************************/
	var doConfirm = function(options)
	{
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		try{rspJson['showCancelButton'] =  true;}catch(err){}
		try{rspJson['cancelButtonText'] = '取消';}catch(err){}
		try{rspJson['closeOnConfirm'] =  true;}catch(err){}
		try{rspJson['closeOnCancel'] =  true;}catch(err){}
		/*********************************************************************************
		*点击操作按钮事件信息
		**********************************************************************************/
		var doBack = function(confirmOK){
			if(confirmOK){
				if(rspJson['trueUrl']!=undefined 
				&& rspJson['trueUrl']!=null 
				&& rspJson['trueUrl']!='')
				{
					window.location=rspJson["trueUrl"];	
				}	
			}else{
				if(rspJson["falseUrl"]!=undefined 
				&& rspJson['trueUrl']!=null 
				&& rspJson["falseUrl"]!="")
				{
					window.location=rspJson["falseUrl"];
				}
				else{window.location="?action=default";}
			}
		}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(window.confirm!=undefined && window.confirm!=null 
		&& typeof(window.confirm)=='function')
		{
			if(window.confirm(rspJson['text']))
			{
				doBack(true);	
			}
			else{doBack(false);	};
		}
	};
	/******************************************************************************************
	*弹出数据处理提示框信息
	*******************************************************************************************/
	var doMessage = function(rspJson)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		
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
	
	/******************************************************************************************
	*弹出确认框跳转到指定的界面
	*******************************************************************************************/
	var altRedirect = function(rspJson)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		try{rspJson['showCancelButton'] = false;}catch(err){}
		try{rspJson['closeOnConfirm'] = true;}catch(err){}
		/*********************************************************************************
		*验证是否存在跳转的URL地址
		**********************************************************************************/
		if(rspJson['url']==undefined){PutMessage('获取跳转URL地址失败！');return false;}
		else if(rspJson['url']==null){PutMessage('获取跳转URL地址失败！');return false;}
		else if(rspJson['url']==''){PutMessage('获取跳转URL地址失败！');return false;}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
			window.location=rspJson['url'];
		}
	};
	/******************************************************************************************
	*执行数据跳转功能
	*******************************************************************************************/
	var doRedirect = function(rspJson)
	{
		try{
			if(rspJson['url']==undefined){PutMessage('获取跳转URL地址失败！');return false;}
			else if(rspJson['url']==null){PutMessage('获取跳转URL地址失败！');return false;}
			else if(rspJson['url']==''){PutMessage('获取跳转URL地址失败！');return false;}
			else{window.location=rspJson['url'];}	
		}
		catch(err){}
	};
	/******************************************************************************************
	*将服务器端返回的JSON数据格式转换
	*******************************************************************************************/
	var JSONResponse = function(rspJson)
	{
		try{
			var rspJson = rspJson || {};
			try{rspJson['type'] = rspJson['type'] || 'define';	}catch(err){};
			try{rspJson['success']=rspJson['success'] || 'false';}catch(err){};
			try{rspJson['tips'] = rspJson['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
			try{rspJson['text'] = rspJson['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
		}catch(err){}
		/******************************************************************************************
		*验证返回数据格式的合法性
		*******************************************************************************************/
		if(rspJson==undefined){PutMessage("获取请求数据返回格式失败！");return false;}
		else if(rspJson==null){PutMessage("获取请求数据返回格式失败！");return false;}
		else if(typeof(rspJson)!='object'){PutMessage("获取请求数据返回格式失败！");return false;}
		else if(rspJson['type']==undefined){PutMessage("返回请求返回数据类型失败！");return false;}
		else if(rspJson['type']==null){PutMessage("返回请求返回数据类型失败！");return false;}
		else if(rspJson['type']==''){PutMessage("返回请求返回数据类型失败！");return false;}
		else if(rspJson['tips']==undefined){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['tips']==null){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['tips']==''){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['success']==undefined){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['success']==null){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['success']!='true'){PutMessage(rspJson['tips']);return false;}
		/******************************************************************************************
		*开始处理网站数据信息
		*******************************************************************************************/
		if(rspJson['type']=='confirm'){doConfirm(rspJson);}
		else if(rspJson['type']=='alert'){doMessage(rspJson);}
		else if(rspJson['type']=='altRedirect'){altRedirect(rspJson);}
		else if(rspJson['type']=='redirect'){doRedirect(rspJson);}	
		else if(rspJson['type']=='define'){
			if(options['back']!=undefined 
			&& options['back']!=null 
			&& typeof(options['back'])=='function')
			{
				options['back'](rspJson);
			}
		};
		/******************************************************************************************
		*处理系统回调函数
		*******************************************************************************************/
		if((rspJson['type']=='confirm' || rspJson['type']=='alert') 
		&& options['back']!=undefined && options['back']!=null 
		&& typeof(options['back'])=='function'){
			options['back'](rspJson);
		};
	};
	/******************************************************************************************
	*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
	*将字符串转换为JSON格式对象
	*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
	*******************************************************************************************/
	function strToJson (strResponse) 
	{
		var rspJson = null;
		/***********************************************************************************
		*开始执行请求数据处理
		************************************************************************************/
		try{
			if(strResponse!=undefined && strResponse!=null 
			&& strResponse!="" && strResponse.match(/{(.*?)}/))
			{
				try{rspJson = jQuery.parseJSON(strResponse);}
				catch(err){PutMessage("转换JSON数据格式失败!");return null;}
			}
			else if(strResponse!=undefined && strResponse!=null 
			&& strResponse!="" && strResponse.match(/[(.*?)]/))
			{
				try{rspJson = jQuery.parseJSON(strResponse);}
				catch(err){PutMessage("转换JSON数据格式失败!");return null;}
			}
			else if(strResponse!=undefined && strResponse!=null && strResponse!="")
			{
				try{
					if (typeof(JSON.parse(strResponse)) == "object") 
					{
						try{rspJson = jQuery.parseJSON(strResponse);}
						catch(err){PutMessage("转换JSON数据格式失败!");return null;}
					}
				}catch(e) {}
			};
		}catch(err){}
		/***********************************************************************************
		*返回JSON数据处理结果
		************************************************************************************/
		return rspJson;
	};
	/****************************************************************************************
	*播放数据请求的动画信息
	*****************************************************************************************/
	try{animation("show");}
	catch(err){}
	/****************************************************************************************
	*实例化请求对象信息
	*****************************************************************************************/
	var options = options || {};
	options['url'] = options['url'] || '';
	options['type'] = options['type'] || 'get';
	options['dataType'] = options['dataType'] || 'json';
	options['async'] = options['async'] || false;
	/****************************************************************************************
	*处理错误请求数据
	*****************************************************************************************/
	options['error'] = function(err)
	{
		try{
			var timer = setTimeout(function(){
				try{clearTimeout(timer);}catch(err){}
				try{animation("hide");}catch(err){}
			},500);
		}catch(err){}
		try{PutMessage('请求过程中发生错误,请重试！');}
		catch(err){}
		return false;
	};
	options['complete'] = function(err)
	{
		try{
			var timer = setTimeout(function(){
				try{clearTimeout(timer);}catch(err){}
				try{animation("hide");}catch(err){}
			},500);
		}catch(err){}
		
	};
	options['success'] = function(strResponse)
	{
		if(options['dataType']!=undefined && options['dataType']!=null 
		&& options['dataType']=='json' && strResponse!=undefined 
		&& strResponse!=null && typeof(strResponse)=='object')
		{
			try{JSONResponse(strResponse);}
			catch(err){}
		}
		else if(strResponse!=undefined && strResponse!=null 
		&& strResponse!="" && strResponse.match(/{(.*?)}/) 
		&& strResponse.indexOf(":")!=-1)
		{
			try{JSONResponse(strToJson(strResponse));}
			catch(err){alert(err.message);return false;}
		}
		else if(strResponse!=undefined && strResponse!=null 
		&& strResponse!="" && strResponse.match(/[(.*?)]/))
		{
			try{JSONResponse(strToJson(strResponse));}
			catch(err){alert(err.message);return false;}
		}
		else{
			if(options['back']!=undefined 
			&& options['back']!=null 
			&& typeof(options['back'])=='function')
			{
				options['back'](strResponse);
			}	
		}
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
	if(jQuery==undefined){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(jQuery==null){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(typeof(jQuery)!="function"){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(jQuery["ajax"]==undefined){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(jQuery["ajax"]==null){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(jQuery["ajax"]==""){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	/****************************************************************************************
	*开始请求数据信息
	*****************************************************************************************/
	try{jQuery.ajax(options);}
	catch(err){}
};
