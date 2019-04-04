;(function($) {
"use strict";
	/***********************************************************************************
	*显示商品分类
	************************************************************************************/
	$.fn.ShowCategory = function(options,channelid)
	{
		var $category = this;
		/********************************************************************************
		*显示分类信息
		*********************************************************************************/
		var ShowClass = function(parentid)
		{
			var clsTemplate = "";
			clsTemplate +="<table cellpadding=\"0\" class=\"frmclass\" cellspacing=\"6\">";
			clsTemplate +="<tr><td colspan=\"4\" cellspacing=\"0\" style=\"font-size:0px;padding:0px;height:0px;\"></td></tr>";
			clsTemplate +="<tr class=\"list\">";
			var SelectionIndex = 0;var iCount = 0;
			$(options).each(function(k,json){
					try{
						if(json["ParentID"]==parentid)
						{
							SelectionIndex=SelectionIndex+1;
							iCount = iCount+1;
							clsTemplate +="<td onclick=\"window.location='List.aspx?action=default&classid="+json['ClassID']+"'\" class=\"items\">";
							clsTemplate +="<div class=\"text\">"+json['ClassName']+"</div>";
							clsTemplate +="</td>";
							if(SelectionIndex>=4){
								clsTemplate +="</tr><tr class=\"list\">";	
								SelectionIndex=0;
							}
						};
					}catch(err){}
			});
			/***************************************************************************
			*补足空缺位置
			****************************************************************************/
			if(iCount%4!=0){
				for(var i=0;i<4-(iCount%4);i++)
				{
					clsTemplate +="<td class=\"items\"></td>";	
				}
			}
			clsTemplate +="</tr>";
			clsTemplate +="</table>";
			return clsTemplate;
		}
		/********************************************************************************
		*渲染显示内容信息
		*********************************************************************************/
		var render = function()
		{
			var strTemplate = "";
			try{
				strTemplate +="<table cellpadding=\"3\" cellspacing=\"1\" border=\"0\" id=\"frm-category\">";
				strTemplate +="<tr><td style=\"padding:0px;width:0px;font-size:0px;\"></td></tr>";
				var defaultId = "0";var demoId = "0";
				$(options).each(function(k,json){
					try{
						if(json["ChannelID"]==channelid && json['ParentID']=='0')
						{
							strTemplate +="<tr class=\"category\">";
							strTemplate +="<td>";
							strTemplate +="<div class=\"text\">————"+json["ClassName"]+"————</div>";
							strTemplate +="</td>";
							strTemplate +="</tr>";
							strTemplate +="<tr class=\"class\">";
							strTemplate +="<td>";
							strTemplate +=(ShowClass(json['ClassID']));
							strTemplate +="</td>";
							strTemplate +="</tr>";
						};
						defaultId = json["ChannelID"];
					}catch(err){}
				});
				strTemplate +="";
				strTemplate +="</table>";	
			}catch(err){}
			/***********************************************************************************
			*内容赋值
			************************************************************************************/
			if($category!=undefined && $category!=null 
			&& strTemplate!=undefined && strTemplate!="")
			{
				$($category).html(strTemplate);	
			}
		};
		
		render();
	};
	/***********************************************************************************
	*显示商品种类
	************************************************************************************/
	$.fn.ShowChannel = function(options)
	{
		var $continaer = this;
		var render = function()
		{
			var strTemplate = "";
			try{
				strTemplate +="<table cellpadding=\"3\" cellspacing=\"1\" border=\"0\" id=\"channel\">";
				strTemplate +="<tr><td style=\"padding:0px;width:0px;font-size:0px;\"></td>";
				var defaultId = "0";var demoId = "0";
				$(options).each(function(k,json){
					try{
						if(defaultId!=json["ChannelID"])
						{
							strTemplate +="<td operate=\"channel\" channelid=\""+json["ChannelID"]+"\"";
							if(defaultId=="0"){
								strTemplate +=" class=\"current\"";
								demoId = json["ChannelID"];
							}
							strTemplate +=">";
							strTemplate +="<div class=\"thumb\"><img onerror=\"this.src='template/images/photo.png'\" src=\""+json['chThumb']+"\" /></div>";
							strTemplate +="<div class=\"text\">"+json["Channelname"]+"</div>";
							strTemplate +="</td>";
						};
						defaultId = json["ChannelID"];
					}catch(err){}
				});
				strTemplate +="</tr>";
				strTemplate +="</table>";	
			}catch(err){}
			/***********************************************************************************
			*内容赋值
			************************************************************************************/
			if($continaer!=undefined && $continaer!=null 
			&& strTemplate!=undefined && strTemplate!="")
			{
				$($continaer).html(strTemplate);
			}
			/***********************************************************************************
			*显示分类
			************************************************************************************/
			try{
				if(demoId!=undefined && demoId!=0)
				{
					$("#frm-contianer").ShowCategory(options,demoId);	
				}
			}catch(err){}
			/***********************************************************************************
			*定义点击事件
			************************************************************************************/
			$($continaer).find("td[operate=\"channel\"]").click(function(){
				try{
					$($continaer).find("td[operate=\"channel\"]").removeClass('current');
					$(this).addClass('current');
				}catch(err){}
				/*******************************************************************************
				*显示品种下的分类
				********************************************************************************/
				try{
					var channelId = parseInt($(this).attr("ChannelID")) || 0;
					if(channelId!=undefined && channelId!=0)
					{$("#frm-contianer").ShowCategory(options,channelId);}
				}catch(err){}
			});
		};
		
		render();
	};
	
})(jQuery);