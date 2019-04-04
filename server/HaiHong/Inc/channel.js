;(function($) {
"use strict";
	/***********************************************************************************
	*显示商品分类
	************************************************************************************/
	$.fn.ShowTabChannel = function(back)
	{
		var $contianer = this;
		/***********************************************************************************
		*渲染商品种类模式
		************************************************************************************/
		var render = function()
		{
			try{
				$.getJSON("../buffer/channel.json",function(options){
					var strTemplate = "";
					strTemplate +="<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" >";
					strTemplate +="<tr>";
					strTemplate +="<td operate=\"items\" channelid=\"0\" class=\"current\">\u5168\u90e8\u5206\u7c7b</td>";
					if(options!=undefined && options!=null 
					&& typeof(options)=='object')
					{
						$(options).each(function(k,json){
							strTemplate += "<td operate=\"items\" channelid=\""+json["ChannelID"]+"\">"+json["Channelname"]+"</td>";					
						});	
					};
					strTemplate +="</tr>";
					strTemplate +="</table>";
					$($contianer).html(strTemplate);
					/***********************************************************************************
					*定义分类切换事件
					************************************************************************************/
					$($contianer).find("td[operate=\"items\"]").click(function(){
						try{
							$($contianer).find("td[operate=\"items\"]").removeClass('current');
							$(this).addClass('current');
						}catch(err){}
						/***********************************************************************************
						*设置数据回调函数
						************************************************************************************/
						try{
							var channelid = parseInt($(this).attr("channelid")) || 0;
							if(channelid!=undefined && channelid!=null 
							&& back!=undefined && back!=null && typeof(back)=='function'){
								back(channelid);
							}
						}catch(err){}
					});
				});
			}catch(err){alert('加载商品分类失败,请重试！');return false;}
		};
		/***********************************************************************************
		*开始显示商品种类信息
		************************************************************************************/
		if($contianer!=undefined && $contianer!=null)
		{
			try{
				render();	
			}catch(err){}
		}
	};
	/***********************************************************************************
	*显示商品分类
	************************************************************************************/
	$.fn.ShowOptionsChannel = function(back,defaultValue)
	{
		var $contianer = this;
		var defaultValue = defaultValue || "";
		/***********************************************************************************
		*渲染商品种类模式
		************************************************************************************/
		var render = function()
		{
			try{
				$.getJSON("../buffer/channel.json",function(options){
					var strTemplate = "";
					$(options).each(function(k,json){
						strTemplate += "<option value=\""+json["Channelname"]+"\"";
						if(defaultValue!=undefined && defaultValue!="" 
						&& defaultValue==json["Channelname"]){
							strTemplate += "selected";	
						}
						strTemplate += ">";
						strTemplate += ""+json["Channelname"]+"";	
						strTemplate += "</option>";	
					});	
					try{
						$($contianer).empty();
						$($contianer).append(strTemplate);	
					}catch(err){}
					/***********************************************************************************
					*定义分类切换事件
					************************************************************************************/
					$($contianer).change(function(){
						var selection = this.options[this.selecedIndex];
						if(back!=undefined && back!=null 
						&& typeof(back)=='function' && selection!=undefined 
						&& selection!=null)
						{
							back($contianer,selection)	
						}
					});
				});
			}catch(err){alert('加载商品分类失败,请重试！');return false;}
		};
		/***********************************************************************************
		*开始显示商品种类信息
		************************************************************************************/
		if($contianer!=undefined && $contianer!=null)
		{
			try{
				render();	
			}catch(err){}
		}
	};
})(jQuery);