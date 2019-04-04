jQuery.alert=function(tips,okFunction,width,height){
	try{
		if(document.getElementById('frm-jquery-alert-box')){$("#frm-jquery-alert-box").remove();}
	}catch(e){}
	try{
		thisObject = document.createElement('div');
		thisObject.id = "frm-jquery-alert-box";
		$(document.body).append(thisObject);
		$(thisObject).attr("title","系统提示");
		$(thisObject).html(tips);
		if(width==undefined || isNaN(width)){width=450;}
		if(height==undefined || isNaN(height)){height=220;}
		$(thisObject).dialog({width:width,height:height,modal:true,buttons:{"确定":function(){
			if(okFunction!=undefined && typeof(okFunction)=='function'){okFunction();$(thisObject).remove();}	  
		}},close:function(){okFunction();$(thisObject).remove();}});
	}catch(e){alert(e.message);return false;}
};
jQuery.confirm=function(tips,okFunction,cancelFunction,width,height){
	
	try{
		if(document.getElementById('frm-jquery-confirm-box')){$("#frm-jquery-confirm-box").remove();}
	}catch(e){}
	try{
		thisObject = document.createElement('div');
		thisObject.id = "frm-jquery-confirm-box";
		$(document.body).append(thisObject);
		$(thisObject).attr("title","系统提示");
		$(thisObject).html(tips);
		if(width==undefined || isNaN(width)){width=450;}
		if(height==undefined || isNaN(height)){height=220;}
		$(thisObject).dialog({width:width,height:height,modal:true,buttons:
		{"确定":function(){
				if(okFunction!=undefined && typeof(okFunction)=='function'){okFunction();}
				$(thisObject).remove();},"取消":function(){
				if(cancelFunction!=undefined && typeof(cancelFunction)=='function'){cancelFunction();}
				$(thisObject).remove();
			}
		},close:function(){if(cancelFunction!=undefined && typeof(cancelFunction)=='function'){cancelFunction();}$(thisObject).remove();}});
	}catch(e){alert(e.message);}
};
jQuery.error=function(tips,back,timer){
	if(document.getElementById('this-jQuery-alert-box')){
		$("#this-jQuery-alert-box").remove();
	}
	try{
		var thisText = '<div id="this-jQuery-alert-box">{tips}</div>';
		thisText = thisText.format({tips:tips});
		$(document.body).append(thisText);
		$("#this-jQuery-alert-box").show()
		if(timer==undefined || isNaN(timer)){timer=10;}
		var k=0;
		var interval = setInterval(function(){
			if(k<timer){k=k+1;}
			else{
				clearInterval(interval);$("#this-jQuery-alert-box").remove();;k =0;
				if(back!=undefined){back();}
			}
		},200);
	}catch(e){}
};
jQuery.Interval=function(sTime,callback){var cTime = sTime;var obj = setInterval(function(){if(cTime>0){cTime=cTime-1;}else{clearInterval(obj);callback();cTime=sTime;}},1000);}