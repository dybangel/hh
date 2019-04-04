;(function($) {
"use strict";
	$.fn.Kucun = function(options)
	{
		
		/****************************************************************************************
		* 将一个多维数组子元素进行排列,迭代
		*****************************************************************************************/
		var doExchange = function (arr)
		{
			var len = arr.length;
			// 当数组大于等于2个的时候
			if(len >= 2){
				// 第一个数组的长度
				var len1 = arr[0].length;
				// 第二个数组的长度
				var len2 = arr[1].length;
				// 2个数组产生的组合数
				var lenBoth = len1 * len2;
				//  申明一个新数组
				var items = new Array(lenBoth);
				// 申明新数组的索引
				var index = 0;
				try{
					for(var i=0; i<len1; i++){
						for(var j=0; j<len2; j++){
							if(arr[0][i] instanceof Array){
								items[index] = arr[0][i].concat(arr[1][j]);
							}else{
								items[index] = [arr[0][i]].concat(arr[1][j]);
							}
							index++;
						}
					};
				}catch(err){}
				try{
					var newArr = new Array(len -1);
					for(var i=2;i<arr.length;i++){
						newArr[i-1] = arr[i];
					};
					newArr[0] = items;
					return doExchange(newArr);
				}catch(err){}
				
			}else{
				return arr[0];
			}
		};
		
		/****************************************************************************************
		* 遍历一个Xml集合中的元素
		*****************************************************************************************/
		var attrList = function(propertyXml,back){
			try{;
				$(propertyXml).find("items").each(function(i,attrib){
					var options = {};
					if(attrib!=undefined && attrib!=null && $(attrib).find("name")[0]!=undefined 
					&& $(attrib).find("name")[0]!="" && $(attrib).find("value")[0]!="")
					{
						var strName =$($(attrib).find("name")[0]).text();
						var strValue =$($(attrib).find("value")[0]).text();
						options["name"] = strName;
						options["value"] = strValue;
					}
					if(options!=undefined && options!=null && typeof(options)=='object'
					&& back!=undefined && back!=null && typeof(back)=='function')
					{
						back(options);
					}
				});
			}catch(err){}
		};
		/*******************************************************************************************
		*将一维数组格式化,截取数组concat(length-2)为前缀,(length-1)为后缀
		********************************************************************************************/
		var Listname = function(arr,back)
		{
			var prefixName = "";var suffixName ="";
			try{
				if(arr!=undefined && arr!=null && typeof(arr)=='object')
				{
					prefixName = arr[0];
					for(var s=1;s<=arr.length-1;s++){
						suffixName	= suffixName+arr[s];
					}
				}else if(arr!=undefined && typeof(arr)=='string'){
					suffixName = arr;	
				}
			}catch(err){};
			try{
				if(suffixName!=undefined && suffixName!="" 
				&& back!=undefined && typeof(back)=='function')
				{
					back(prefixName,suffixName);	
				}
			}catch(err){}
		};
		/*******************************************************************************************
		*计算用户选中的商品信息
		********************************************************************************************/
		var Calculation = function()
		{
			try{
				var SelectorNumber = 0;
				$("#SelectionListTab").find("div[class=\"selector\"]").each(function(){
					var strValue = parseInt($(this).attr("number")) || 0;
					if(strValue!=undefined && strValue!=null 
					&& strValue!="" && !isNaN(strValue) && strValue>=1)
					{
						SelectorNumber=SelectorNumber+parseInt(strValue);
					}
				});
				var Amount = parseInt(cfg['retailamount']) || 0;
				var Total = parseFloat(Amount * SelectorNumber);
				var strHtml = "<span>";
				strHtml +="已选择<font>"+SelectorNumber+"</font>件商品";
				strHtml +="共<font>"+Total+"</font>元";
				strHtml +="<span>";
				$("#frm-faceplateCalculation").html(strHtml);
			}catch(err){}	
		}
		/*******************************************************************************************
		* 渲染属性选择器信息
		********************************************************************************************/
		var ShowKucun = function()
		{
			var arrTemp = [];var firstArray = [];var isFirst = "";
			try{
				attrList(options['xml'],function(arr){
					try{
						if(arr!=undefined && typeof(arr)=='object'
						&& arr["value"]!=undefined && arr["value"]!="")
						{
							var newTemp = arr["value"].split(",");
							if(newTemp!=undefined && newTemp!=null){
								arrTemp.push(newTemp);	
							}
						}
					}catch(err){}
				});
			}catch(err){}
			if(arrTemp!=null && arrTemp.length>=1){firstArray=arrTemp[0];}
			if(arrTemp!=null && arrTemp.length>=2 
			&& firstArray && firstArray.length>=1){ isFirst = firstArray[0];}
			//alert(typeof(firstArray));
			/********************************************************************************************
			*输出抬头目录信息
			*********************************************************************************************/
			try
			{
				var strMenu ="";
				strMenu += "<div id=\"frm-faceplateSelectionMenu\">";
				if(arrTemp!=undefined && arrTemp!=null && arrTemp.length>=2 
				&& firstArray!=undefined && firstArray!=null 
				&& typeof(firstArray)=='object' && firstArray.length!=0)
				{
					strMenu += "<table cellpadding=\"3\" border=\"0\" id=\"SelectionMenuTab\" cellspacing=\"1\">";
					strMenu += "<tr>";
					for(var k in firstArray)
					{
						strMenu += "<td operate=\"menu\" value=\""+firstArray[k]+"\" "+(k==0 ? "class=\"current\"" : "")+">"+firstArray[k]+"</td>";
					}
					strMenu += "</tr>";
					strMenu += "</table>";
				}else{
					strMenu += "<table cellpadding=\"3\" border=\"0\" id=\"SelectionMenuTab\" cellspacing=\"1\">";
					strMenu += "<tr>";
					strMenu += "<td class=\"current\">默认</td>";
					strMenu += "</tr>";
					strMenu += "</table>";	
				}
				strMenu += "</div>";
				var frmMenu = $(strMenu)[0];
				/********************************************************************************************
				*定义数据点击事件
				*********************************************************************************************/
				$(frmMenu).find("#SelectionMenuTab").find("td[operate=\"menu\"]").click(function(){
					try
					{
						try{
							$(frmMenu).find("td[operate=\"menu\"]").removeClass("current");
							$(this).addClass("current");
						}catch(err){}
						try
						{
							var prefixText = $(this).attr("value") || "";
							if(prefixText!=undefined && prefixText!="")
							{
								$("#SelectionListTab").find("div[class=\"selector\"]").fadeOut();
								$("#SelectionListTab").find("div[value=\""+prefixText+"\"]").fadeIn("slow");
							}
						}catch(err){}
					}catch(err){}
				});
				$("#frm-faceplateSelection").html(frmMenu);
			}catch(err){}
			/************************************************************************************************
			*开始加载属性信息
			*************************************************************************************************/
			try
			{
				var strTemplate = "<div id=\"SelectionListTab\">";
				var sTemp = doExchange(arrTemp);
				if(sTemp!=undefined && sTemp!=null && sTemp.length>0)
				{
					try{
						var arrList = "";
						for(var k in sTemp)
						{
							Listname(sTemp[k],function(prefixName,suffixName){
								var isKucun = '无库存';var attrNumber = 0;
								try{
									if(options["kucun"]!=undefined && options["kucun"]!=null 
									&& typeof(options["kucun"])=='object')
									{
										attrNumber = parseInt(options["kucun"][""+prefixName+""+suffixName+""]) ||
										0;	
										if(attrNumber!=0){isKucun="有库存"}
									}
								}catch(err){}
								/************************************************************************
								*开始加载模板信息
								*************************************************************************/
								strTemplate += "<div "+(prefixName!=isFirst ? " style=\"display:none\"" :"")+" text=\""+prefixName+""+suffixName+"\" value=\""+prefixName+"\" number=\"0\" class=\"selector\">";
								strTemplate += "<div class=\"name\">"+suffixName+"</div>";
								strTemplate += "<div class=\"kucun\">"+isKucun+"</div>";
								strTemplate += "<div value=\""+attrNumber+"\" class=\"value\">";
								strTemplate += "<span class=\"subtraction\">-</span>";
								strTemplate += "<input min=\"0\" placeholder=\"数量\" type=\"number\" value=\"0\" operate=\"number\" />";
								strTemplate += "<span class=\"addition\">+</span>";
								strTemplate += "</div>";
								strTemplate += "</div>";
							});
						};
						
					}catch(err){}
				}
				strTemplate += "</div>";
				var frmTemplate = $(strTemplate)[0];
				/********************************************************************************************
				*增加购买数量或减少购买数量事件
				*********************************************************************************************/
				$(frmTemplate).find("span[class=\"subtraction\"]").click(function(){
					try
					{
						var tagItems = $(this).parents("div[class=\"selector\"]")[0];
						if(tagItems!=undefined && tagItems!=null)
						{
							var Kucun = parseInt($(tagItems).attr("value")) || 0;
							var SelectionNumber = parseInt($(tagItems).find("input[type=\"number\"]").val()) || 0;
							if(!SelectionNumber){SelectionNumber=0;}
							if(isNaN(SelectionNumber)){SelectionNumber=0;}
							if(SelectionNumber>=1){SelectionNumber=SelectionNumber-1;}
							$(tagItems).find("input[type=\"number\"]").val(SelectionNumber);
							$(tagItems).attr("number",SelectionNumber);
							/*********************************************************
							*计算商品选择数量
							**********************************************************/
							try{Calculation();}catch(err){}
							/*********************************************************
							*程序执行完成
							**********************************************************/
						}else{alert('发生错误,请重新加载应用！');return false;}
					}catch(err){}
				});
				/********************************************************************************************
				*点击增加事件
				*********************************************************************************************/
				$(frmTemplate).find("span[class=\"addition\"]").click(function(){
					try
					{
						var tagItems = $(this).parents("div[class=\"selector\"]")[0];
						if(tagItems!=undefined && tagItems!=null)
						{
							var Kucun = parseInt($(this.parentNode).attr("value")) || 0;
							var SelectionNumber = parseInt($(tagItems).find("input[type=\"number\"]").val()) || 0;
							if(!SelectionNumber){SelectionNumber=0;}
							if(isNaN(SelectionNumber)){SelectionNumber=0;}
							if(SelectionNumber<Kucun){SelectionNumber = SelectionNumber+1;}
							$(tagItems).find("input[type=\"number\"]").val(SelectionNumber);
							$(tagItems).attr("number",SelectionNumber);
							/*********************************************************
							*计算商品选择数量
							**********************************************************/
							try{Calculation();}catch(err){}
							/*********************************************************
							*程序执行完成
							**********************************************************/
						}else{alert('发生错误,请重新加载应用！');return false;}
						
					}catch(err){}
				});
				/********************************************************************************************
				*点击增加事件
				*********************************************************************************************/
				$(frmTemplate).find("input[type=\"number\"]").change(function(){
					try
					{
						var tagItems = $(this).parents("div[class=\"selector\"]")[0];
						if(tagItems!=undefined && tagItems!=null)
						{
							var Kucun = parseInt($(tagItems).attr("value")) || 0;
							var SelectionNumber = parseInt($(this).val()) || 0;
							if(!SelectionNumber){SelectionNumber=0;}
							if(isNaN(SelectionNumber)){SelectionNumber=0;}
							$(this).val(SelectionNumber);
							$(tagItems).attr("number",SelectionNumber);
							
							/*********************************************************
							*计算商品选择数量
							**********************************************************/
							try{Calculation();}catch(err){}
							/*********************************************************
							*程序执行完成
							**********************************************************/
							
						}else{alert('发生错误,请重新加载应用！');return false;}
					}catch(err){}
				});
				/********************************************************************************************
				*显示其他数据信息
				*********************************************************************************************/
				$("#frm-faceplateSelection").append(frmTemplate);
			}catch(err){}
		};
		/********************************************************************************************
		*开始加载参数对象信息
		*********************************************************************************************/
		try{
			var options = options || {};
			try{options["xml"] = options["xml"] || cfg["propertyXml"];	}catch(err){}
			if(options["xml"]!=undefined && options["xml"]!=null && options["xml"]!="" 
			&& options["xml"].indexOf('items')!=-1)
			{
				ShowKucun();
			}
		}catch(err){}
		
	};
	/****************************************************************************************
	*获取到用户选中的商品属性信息
	*****************************************************************************************/
	$.fn.Selection = function(back)
	{
		var strXml = "<configurationRoot>";
		try{
			$("#SelectionListTab").find("div[class=\"selector\"]").each(function(){
				var strName = $(this).attr("text") || "";
				var strValue = parseInt($(this).attr("number")) || 0;
				if(strName!=undefined && strName!=null && strName!="" 
				&& strValue!=undefined && strValue!=null && strValue!="" 
				&& !isNaN(strValue) && strValue>=1 
				&& cfg!=undefined && cfg!=null && typeof(cfg)=='object')
				{
					strXml += "<items>";
					strXml += "<brandname>"+cfg['brandname']+"</brandname>";
					strXml += "<productid>"+cfg['productid']+"</productid>";
					strXml += "<idnumber>"+cfg['idnumber']+"</idnumber>";
					strXml += "<property>"+strName+"</property>";
					strXml += "<number>"+strValue+"</number>";
					strXml += "</items>";
				}
			});
		}catch(err){}
		strXml += "</configurationRoot>";
		/*******************************************************************************
		*执行数据回调
		********************************************************************************/
		try{
			if(back!=undefined && back!=null && typeof(back) =='function' 
			&& strXml!=undefined && strXml!="" && strXml.indexOf('items')!=-1)
			{
				back(strXml);	
			}else
			{
				alert('请选择商品属性!');return false;
			}
		}catch(err){}
	}
	
})(jQuery);
/****************************************************************************************
*展示图片墙
*****************************************************************************************/
var Slider=function(options)
{
	var options = options || [];
	var strTemplate = '';
	if(options!=undefined && options!=null 
	&& typeof(options)=='object' && options.length>=2)
	{
		try{
			strTemplate += '<div class="main_visual">';
			strTemplate += '<div class="flicking_con" id="conThis">';
			$(options).each(function(k,items){
				strTemplate += '<a href="javascript:void(0)"></a>';						 
			});
			strTemplate += '</div>';
			strTemplate += '<div class="main_image">';
			strTemplate += '<ul id="imgThis">';
			$(options).each(function(k,items){
				strTemplate += '<li style=\"width:100%\"><img style=\"width:100%;\" src="'+items+'" /></li>';						
			});
			strTemplate += '</ul>';
			strTemplate += '<a href="javascript:;" style="display:none;" id="btn_prev"></a>';
			strTemplate += '<a href="javascript:;" style="display:none;" id="btn_next"></a>';
			strTemplate += '</div>';
			strTemplate += '</div>';	
		}catch(err){}
	}else
	{
		strTemplate += '<div style=\"width:100%;height:450px;\"><img style=\"width:100%;max-height:450px;\" src=\"'+options[0]+'\" /></div>';	
	}
	
	$("#frm-swipper").html(strTemplate);
	if(options!=undefined && options!=null 
	&& typeof(options)=='object' && options.length>=2)
	{
		try
		{
			$(".main_visual").hover(function(){$("#btn_prev,#btn_next").fadeIn();},
						function(){$("#btn_prev,#btn_next").fadeOut();});
			$dragBln = false;
			$(".main_image").touchSlider({
				flexible : true,speed : 200,
				btn_prev : $("#btn_prev"),
				btn_next : $("#btn_next"),
				paging : $(".flicking_con a"),
				counter : function (e){$(".flicking_con a").removeClass("on").eq(e.current-1).addClass("on");}
			});
			$(".main_image").bind("mousedown", function() {$dragBln = false;});
			$(".main_image").bind("dragstart", function() {$dragBln = true;});
			$(".main_image a").click(function(){if($dragBln) {return false;}});
			timer = setInterval(function(){$("#btn_next").click();}, 5000);
			$(".main_visual").hover(function(){clearInterval(timer);},function(){
				timer = setInterval(function(){$("#btn_next").click();},5000);
			});
			$(".main_image").bind("touchstart",function(){clearInterval(timer);}).bind("touchend", function(){
				timer = setInterval(function(){$("#btn_next").click();}, 5000);
			});
		}catch(err){}
	}
};

/****************************************************************************************
*定义当前页面中常用的数据方法
*****************************************************************************************/
var ShowFaceplateMaster = function(closed)
{
	try
	{
		if(closed && closed=="true"){
			try{
			$("#frm-faceplateMaster").hide();
			$('html,body').removeClass("visible");
			}catch(err){}
		}else{
			try{
			$("#frm-faceplateMaster").show();
			$('html,body').addClass("visible");
			}catch(err){}
			
		}
	}catch(err){}	
};
/****************************************************************************************
*新增商品到购物车
*****************************************************************************************/
var SendTrolley = function(strXml,isConfirm)
{
	/****************************************************************************************
	*开始请求数据信息
	*****************************************************************************************/
	if(strXml!=undefined && strXml!=null && strXml!="" 
	&& window.Send!=undefined && window.Send!=null 
	&& typeof(window.Send)=='function')
	{
		Send({
			"url":"trolley.aspx?action=add",
			"data":{"productid":cfg["productid"],
					"strxml":strXml,
					"tokey":$.md5(strXml)
				   },
			"back":function(json)
			{
				try{
					if(json!=undefined && json!=null && typeof(json)=='object' 
					&& json['trolleyid']!=undefined && json['trolleyid']!=null)
					{
						isConfirm = isConfirm || false;
						if(isConfirm && json['trolleyid']!="")
						{
							window.location = "Trolley.aspx?action=confirm&strList="+json["trolleyid"]+"";
						}
						else{
							try{
								var number = parseInt(json["cartnumber"]) || 0;
								if(number<=0){number=0;}
								if(number>=10){number="9+";}
								$("#frm-HomeBuyCart").attr("number",number);
								ShowFaceplateMaster('true');
							}catch(err){};	
						}
						
					}
				}catch(err){}
			}
		});	
	}
};
/****************************************************************************************
*改写异步Ajax请求数据信息
*****************************************************************************************/
var SendResponse = function(options)
{
	/****************************************************************************************
	*返回JSON格式数据信息
	*****************************************************************************************/
	var JSONResponse = function(strResponse)
	{
		try
		{
			if(strResponse!=undefined && strResponse!=null && typeof(strResponse)=='object' 
			&& strResponse['success']!=undefined && strResponse['success']!='true')
			{
				if(strResponse['type']!=undefined && strResponse['type']=='redirect' 
				&& strResponse['url']!=undefined && strResponse['url']!=null)
				{
					window.location=strResponse['url'];	
				}
				else if(strResponse['type']!=undefined && strResponse['type']!='redirect' 
				&& strResponse['tips']!=undefined && strResponse['tips']!=null)
				{
					alert(strResponse['tips']);
					return false;	
				};
			}
			else if(strResponse!=undefined && strResponse!=null && typeof(strResponse)=='object' 
			&& strResponse['success']!=undefined && strResponse['success']=='true')
			{
				if(strResponse['type']!=undefined && strResponse['type']=='redirect' 
				&& strResponse['url']!=undefined && strResponse['url']!=null)
				{
					window.location=strResponse['url'];	
				}
				else if(strResponse['type']!=undefined && strResponse['type']!='redirect' 
				&& strResponse['tips']!=undefined && strResponse['tips']!=null)
				{
					if(options!=undefined && options!=null 
					&& options['back']!=undefined && options['back']!=null 
					&& typeof(options['back'])=='function')
					{
						options['back'](strResponse);	
					}else{
						alert(strResponse['tips']);
						return false;	
					};
				};
			}
			else{
				if(options!=undefined && options!=null 
				&& options['back']!=undefined && options['back']!=null 
				&& typeof(options['back'])=='function')
				{
					options['back'](strResponse);	
				}else{
					alert(strResponse['tips']);
					return false;	
				};
			};
			
		}catch(err){alert(err.message);return false;};	
	}
	/****************************************************************************************
	*实例化请求对象信息
	*****************************************************************************************/
	var options = options || {};
	/****************************************************************************************
	*生成URL
	*****************************************************************************************/
	options['url'] = options['url'] || '';
	/****************************************************************************************
	*设置请求方式
	*****************************************************************************************/
	options['type'] = options['type'] || 'get';
	/****************************************************************************************
	*处理数据类型
	*****************************************************************************************/
	options['dataType'] = options['dataType'] || 'json';
	/****************************************************************************************
	*数据请求成功,返回执行函数信息
	*****************************************************************************************/
	options['success'] = options['success'] || function(strResponse)
	{
		if(options['dataType']!=undefined && options['dataType']=='json' 
		&& strResponse!=undefined && strResponse!=null)
		{
			JSONResponse(strResponse);	
		}
		else if(options['dataType']!=undefined && options['dataType']!='json' 
		&& strResponse!=undefined && strResponse!=null && strResponse!="" 
		&& strResponse.match(/{(.*?)}/) && strResponse.indexOf("success")!=-1)
		{
			try{
				var strJSON = {};
				try{strJSON = jQuery.parseJSON(strResponse);}catch(err){}
				JSONResponse(strJSON);	
			}catch(err){alert(err.message);return false;}
		}
		else if(options!=undefined && options!=null 
		&& options['back']!=undefined && options['back']!=null 
		&& typeof(options['back'])=='function')
		{
			options['back'](strResponse);
		}
	};
	/****************************************************************************************
	*处理错误请求数据
	*****************************************************************************************/
	options['error'] = options['error'] || function(err)
	{
		alert('请求过程中发生错误,请重试！');
		return false;
	};
	/****************************************************************************************
	*开始请求数据信息
	*****************************************************************************************/
	if(options!=undefined && options!=null && typeof(options)=='object' 
	&& options['url']!=undefined && options['url']!=null && options['url']!="" 
	&& jQuery!=undefined && jQuery!=null && jQuery.ajax!=undefined && typeof(jQuery.ajax)=='function')
	{
		jQuery.ajax(options);	
	};
}

/****************************************************************************************
*将商品加入搜藏
*****************************************************************************************/
var SaveCollection = function(frmContianer)
{
	try
	{
		var options = {};
		options['url']="Collection.aspx?action=fav&productid="+cfg["productid"]+"&isAsyn=1";
		options['dataType']='json';
		options['type']='get';
		options['back'] = function(strResponse)
		{
			try{
				if(strResponse!=undefined && strResponse!=null && typeof(strResponse)=='object' 
				&& strResponse['iscollection']!=undefined && strResponse['iscollection']!=null
				&& strResponse['iscollection']!="")
				{
					if((parseInt(strResponse['iscollection']))!=0)
					{
						$(frmContianer).attr("isExists","1");	
					}else{
						$(frmContianer).removeAttr("isExists");		
					}
					
				};
			}catch(err){}
		};
		/****************************************************************************************
		*开始请求数据信息
		*****************************************************************************************/
		if(options!=undefined && options!=null && typeof(options)=='object' 
		&& options['url']!=undefined && options['url']!=null && options['url']!="" 
		&& SendResponse!=undefined && SendResponse!=null && typeof(SendResponse)=='function')
		{
			SendResponse(options);	
		};
	}catch(err){}
}
/****************************************************************************************
*加载网页中的事件信息
*****************************************************************************************/
$(function(){
	/****************************************************************************************
	*展示关闭属性面板
	*****************************************************************************************/
	$("#frm-cartbutton").click(function(){
		try{
		ShowFaceplateMaster('false');
		}catch(err){}
	});	
	$("#frm-buybutton").click(function(){
		try{
		ShowFaceplateMaster('false');
		}catch(err){}
	});
	$("#frm-faceplateClose").click(function(){
		try{
		ShowFaceplateMaster('true');
		}catch(err){}
	});
	$("#frm-appModel").click(function(){
		try{
		ShowFaceplateMaster('false');
		}catch(err){}
	});
	
	/****************************************************************************************
	*将商品加入收藏
	*****************************************************************************************/
	$("#frm-collection").click(function(){
		try
		{
			if(cfg!=undefined && cfg!=null && typeof(cfg)=='object' 
			&& cfg['productid']!=undefined && cfg['productid']!="")
			{
				SaveCollection(this);	
			}	
		}catch(err){}
	});
	/****************************************************************************************
	*将商品加入购物车中
	*****************************************************************************************/
	$("#frm-faceplateCartButton").click(function(){
		try{
			$("#frm-faceplateContianer").Selection(function(strXml){
				if(strXml!=undefined && strXml!="" && strXml.indexOf('items')!=-1)
				{
					SendTrolley(strXml,false);	
				}
			});	
		}catch(err){}
	});
	/****************************************************************************************
	*立即购买商品
	*****************************************************************************************/
	$("#frm-faceplateNowButton").click(function(){
		try{
			$("#frm-faceplateContianer").Selection(function(strXml){
				if(strXml!=undefined && strXml!="" && strXml.indexOf('items')!=-1)
				{
					SendTrolley(strXml,true);	
				}
			});	
		}catch(err){}
	});
});