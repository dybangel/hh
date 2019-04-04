var Basic = {
	SiteID : function(){;
		try{
			if(top.siteid !=undefined || !isNaN(top.siteid)){return top.siteid;}
		}catch(err){return 0;}
	},
	Location : function(){
		try{
			if(top.res!=undefined){return top.res;}
			else{return "";}
		}catch(err){return ""}
	},
	Root : function(){
		try{
			if(top.app !=undefined){return top.app;}
			else{return 0;}
		}catch(err){return "/";}
	}
};
/********************************************************************************
*查找数据记录行
*********************************************************************************/
$.findRow=function(url,callback){
	$.loadXml(url,function(xml){
		$(xml).find("item").each(function(){
			if(callback!=undefined){callback(this);}								  
		});					   
	});
};
$.loadXml=function(url,callback){
	$.ajax({url:Basic.Location()+url,async: false,type:"get",success:function(xml){																
		if(callback!=undefined && callback!=null){
			callback(xml);
		};																
	}});
};

$.loadXmlPage=function(url,callback,PageSize,thisPage){
	var Record = 0;
	var thisPage = thisPage || 1;
	if(thisPage<=0){thisPage=1;}
	try{
		$.ajax({url:Basic.Location()+url,async: false,type:"get",success:function(xml){
			var thisObject = $(xml).find("item");
			Record = thisObject.length;
			var StartPosition = ((thisPage-1)*PageSize);
			if(StartPosition<=0){StartPosition=0;}
			var EndPosition = ((thisPage)*PageSize);
			if(parseInt(EndPosition)>Record){EndPosition=Record}
			for(var k=StartPosition;k<EndPosition;k++){
				if(callback!=undefined && callback!=null){
					callback(thisObject[k]);	
				}
			}
		}});
	}catch(err){}
	return Record;
	
};
/******************************************************************
*遍历JSON数据格式JSON
*******************************************************************/
var EachOptions = function(List,options,showTxt){
	var strXml = "";
	if(showTxt!=undefined){showTxt=showTxt+"─";}
	try{
		var selectedIndex = 0;
		
		for(var k=0;k<List["item"].length;k++){
			var json = List["item"][k];
			var value =json[options["value"]] ? json[options["value"]] : "";
			var text =json[options["text"]] ? json[options["text"]] : "";
			strXml+="<option  value=\""+value+"\"";
			/**********************************************************************
			*设置选项
			***********************************************************************/
			var strJson = "{\"success\":\"true\"";
			for(var s in json){
				strJson+=",\""+s+"\":\""+json[s]+"\"";
			}
			strJson+="}";
			strXml+=" json='"+strJson+"'";
			/****************************************************************************
			*设置默认值
			******************************************************************************/
			if(options["demo"]!=undefined && options["demo"]==value){strXml+=" selected";}
			strXml+=">";
			if(showTxt!=undefined){strXml+="└"+showTxt;}
			strXml+=text;
			strXml+="</option>";
			if(json["item"]!=undefined){
				if(showTxt==undefined){showTxt="";}
				strXml += EachOptions(json,options,showTxt);	
			}
		}
	}catch(err){};
	return strXml;
}

;(function($){
	$.fn.options=function(options){
		var options = options || {};
		var thisObject = this;
		
		if(options["url"]!=undefined && options["url"]!=""){
			$.ajax({url:Basic.Location()+options["url"],async: true,type:"get",dataType:"json",
			   success:function(thisList){
					$(thisObject).append(EachOptions(thisList,options));
					$(thisObject).change(function(){
						if(options["back"]!=undefined && typeof(options["back"])=='function'){		
							var json = $(this.options[this.selectedIndex]).attr("json");
							if(json!=undefined && json!=""){
								try{
									options["back"]($.parseJSON(json));	
								}catch(err){alert(err.message);}	
							}	
						}
					});
				},error:function(){}
			});
		}
	};
	/******************************************************************************
	*选择用户信息
	*******************************************************************************/
	$.fn.SelectorUser=function(back)
	{
		try{
			if($(this).attr("load")==undefined || $(this).attr("load")!="true"){
				$(this).html("<iframe id=\"frm-stor-user-frame\" height=\"99%\" width=\"100%\" src=\"User.aspx?action=stor\" fromaborder=\"0\"></iframe>");
				$(this).attr("load","true");
			}
			$(this).dialog({width:600,height:400,modals:true});
			if(back!=undefined && back!=null){FindUserCallBackDelegate=back;}
		}catch(err){}
	}
	
	/******************************************************************************
	*选择用户信息
	*******************************************************************************/
	$.fn.SelectorPlayerClass=function(lotteryId,back)
	{
		try{
			var lotteryId = lotteryId || 0;
			if($(this).attr("load")==undefined || $(this).attr("load")!="true"){
				$(this).html("<iframe id=\"frm-stor-user-frame\" height=\"99%\" width=\"100%\" src=\"PlayerClass.aspx?action=stor&Lotteryid="+lotteryId+"\" fromaborder=\"0\"></iframe>");
				$(this).attr("load","true");
			}
			$(this).dialog({width:600,height:400,modals:true});
			if(back!=undefined && back!=null){FindPlayerClassCallBackDelegate=back;}
		}catch(err){}
	}
	
	/******************************************************************************
	*选择银行卡
	*******************************************************************************/
	$.fn.SelectorBank=function(userId,back)
	{
		try{
			var userId = userId || 0;
			if($(this).attr("load")==undefined || $(this).attr("load")!="true"){
				$(this).html("<iframe id=\"frm-stor-user-frame\" height=\"99%\" width=\"100%\" src=\"Bank.aspx?action=stor&userId="+userId+"\" fromaborder=\"0\"></iframe>");
				$(this).attr("load","true");
			}
			$(this).dialog({width:600,height:400,modals:true});
			if(back!=undefined && back!=null){FindBankCallBackDelegate=back;}
		}catch(err){}
	}
	
	/******************************************************************************
	*选择银行卡
	*******************************************************************************/
	$.fn.SelectorAlipay=function(userId,back)
	{
		try{
			var userId = userId || 0;
			if($(this).attr("load")==undefined || $(this).attr("load")!="true"){
				$(this).html("<iframe id=\"frm-stor-user-frame\" height=\"99%\" width=\"100%\" src=\"Alipay.aspx?action=stor&userId="+userId+"\" fromaborder=\"0\"></iframe>");
				$(this).attr("load","true");
			}
			$(this).dialog({width:600,height:400,modals:true});
			if(back!=undefined && back!=null){FindAlipayCallBackDelegate=back;}
		}catch(err){}
	}
	
	/******************************************************************************
	*选择银行卡
	*******************************************************************************/
	$.fn.SelectorGrade=function(back)
	{
		try{
			var userId = userId || 0;
			if($(this).attr("load")==undefined || $(this).attr("load")!="true"){
				$(this).html("<iframe id=\"frm-stor-user-frame\" height=\"99%\" width=\"100%\" src=\"Grade.aspx?action=stor\" fromaborder=\"0\"></iframe>");
				$(this).attr("load","true");
			}
			$(this).dialog({width:600,height:400,modals:true});
			if(back!=undefined && back!=null){FindGradeCallBackDelegate=back;}
		}catch(err){}
	};
	/******************************************************************************
	*选择投资记录
	*******************************************************************************/
	$.fn.SelectorInvest=function(back)
	{
		try{
			var userId = userId || 0;
			if($(this).attr("load")==undefined || $(this).attr("load")!="true"){
				$(this).html("<iframe id=\"frm-stor-user-frame\" height=\"99%\" width=\"100%\" src=\"invest.aspx?action=stor\" fromaborder=\"0\"></iframe>");
				$(this).attr("load","true");
			}
			$(this).dialog({width:600,height:400,modals:true});
			if(back!=undefined && back!=null){FindInvestCallBackDelegate=back;}
		}catch(err){}
	}
	
	/******************************************************************************
	*选择交易记录
	*******************************************************************************/
	$.fn.SelectorDeal=function(back)
	{
		try{
			var userId = userId || 0;
			if($(this).attr("load")==undefined || $(this).attr("load")!="true"){
				$(this).html("<iframe id=\"frm-stor-user-frame\" height=\"99%\" width=\"100%\" src=\"deal.aspx?action=stor\" fromaborder=\"0\"></iframe>");
				$(this).attr("load","true");
			}
			$(this).dialog({width:600,height:400,modals:true});
			if(back!=undefined && back!=null){FindDealCallBackDelegate=back;}
		}catch(err){}
	}
	
	/******************************************************************************
	*选择交易记录
	*******************************************************************************/
	$.fn.moder=function(url,options)
	{
		try{
			var options = options || {width:600,height:400,modal:true}
			$(this).html("<iframe id=\"frm-stor-user-frame\" height=\"99%\" width=\"100%\" src=\""+url+"\" fromaborder=\"0\"></iframe>");
			$(this).dialog(options);
		}catch(err){}
	}
})(jQuery);


/*************************************************************
*查看交易号数据记录
**************************************************************/
var FindDealCallBackDelegate = null;
/*************************************************************
*选择用户的回调函数
**************************************************************/
var FindDealCallBack = function(thisJSON){
	try{
		if(FindDealCallBackDelegate!=undefined && typeof(FindDealCallBackDelegate)=='function'){
			FindDealCallBackDelegate(thisJSON);	
		}
	}catch(err){alert(err.message);}
}

/*************************************************************
*选择投资记录回调方法
**************************************************************/
var FindInvestCallBackDelegate = null;
/*************************************************************
*选择用户的回调函数
**************************************************************/
var FindInvestCallBack = function(thisJSON){
	try{
		if(FindInvestCallBackDelegate!=undefined && typeof(FindInvestCallBackDelegate)=='function'){
			FindInvestCallBackDelegate(thisJSON);	
		}
	}catch(err){alert(err.message);}
}

/*************************************************************
*选择投资记录回调方法
**************************************************************/
var FindDealCallBackDelegate = null;
/*************************************************************
*选择用户的回调函数
**************************************************************/
var FindDealCallBack = function(thisJSON){
	try{
		if(FindDealCallBackDelegate!=undefined && typeof(FindDealCallBackDelegate)=='function'){
			FindDealCallBackDelegate(thisJSON);	
		}
	}catch(err){alert(err.message);}
}

/*************************************************************
*选择提现申请执行的委托函数
**************************************************************/
var FindGradeCallBackDelegate = null;
/*************************************************************
*选择用户的回调函数
**************************************************************/
var FindGradeCallBack = function(thisJSON){
	try{
		if(FindGradeCallBackDelegate!=undefined && typeof(FindGradeCallBackDelegate)=='function'){
			FindGradeCallBackDelegate(thisJSON);	
		}
	}catch(err){alert(err.message);}
}

/*************************************************************
*选择提现申请执行的委托函数
**************************************************************/
var FindAlipayCallBackDelegate = null;
/*************************************************************
*选择用户的回调函数
**************************************************************/
var FindAlipayCallBack = function(thisJSON){
	try{
		if(FindAlipayCallBackDelegate!=undefined && typeof(FindAlipayCallBackDelegate)=='function'){
			FindAlipayCallBackDelegate(thisJSON);	
		}
	}catch(err){alert(err.message);}
}

/*************************************************************
*选择银行卡执行的委托函数
**************************************************************/
var FindBankCallBackDelegate = null;
/*************************************************************
*选择用户的回调函数
**************************************************************/
var FindBankCallBack = function(thisJSON){
	try{
		if(FindBankCallBackDelegate!=undefined && typeof(FindBankCallBackDelegate)=='function'){
			FindBankCallBackDelegate(thisJSON);	
		}
	}catch(err){alert(err.message);}
}

/*************************************************************
*选择玩法分类执行的委托函数
**************************************************************/
var FindPlayerClassCallBackDelegate = null;
/*************************************************************
*选择用户的回调函数
**************************************************************/
var FindPlayerClassCallBack = function(thisJSON){
	try{
		if(FindPlayerClassCallBackDelegate!=undefined && typeof(FindPlayerClassCallBackDelegate)=='function'){
			FindPlayerClassCallBackDelegate(thisJSON);	
		}
	}catch(err){alert(err.message);}
}

/*************************************************************
*选择用户的回调委托
**************************************************************/
var FindUserCallBackDelegate = null;
/*************************************************************
*选择用户的回调函数
**************************************************************/
var FindUserCallBack = function(thisJSON){
	try{
		if(FindUserCallBackDelegate!=undefined && typeof(FindUserCallBackDelegate)=='function'){
			FindUserCallBackDelegate(thisJSON);	
		}
	}catch(err){alert(err.message);}
}

var XmlOptions=function(options,demo){
var options = options || {};
var strXml = "";
if(typeof(options)=='object' && options.url!=undefined){
	$.loadXml(options.url,function(xml){
		$(xml).find("item").each(function(){
			var strValue = 0;
			if($(this).find(options.value).text()){strValue=$(this).find(options.value).text();}
			var strText ="当前选项错误";
			if($(this).find(options.text).text()){strText=$(this).find(options.text).text();}
			if(demo!=undefined && demo==strValue){
				strXml+="<option selected value=\""+strValue+"\">"+strText+"</option>";	
			}else{strXml+="<option value=\""+strValue+"\">"+strText+"</option>";}
		});
	});	
}
return strXml
};


var ProviderOptions=function(){
	$.loadXml("provider.xml",function(xml){
		var options = "";
		$(xml).find("item").each(function(){
			options+="<option value=\""+$(this).find("providername").text()+"\">"+$(this).find("providername").text()+"</option>";
		});
		return options;
	});	
}