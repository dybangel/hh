/*******************************************************************************************
* jQuery扩展器
********************************************************************************************/
;(function($) {
"use strict";
	/****************************************************************************************
	*加载用户订单列表
	*****************************************************************************************/
	$.fn.History = function(options)
	{
		var $contianer = this;
		/*******************************************************************************************
		* 渲染属性选择器信息
		********************************************************************************************/
		var render = function()
		{
			var strTemplate = "";
			strTemplate += "<table id=\"frm-tabs\" cellpadding=\"3\" cellspacing=\"1\" border=\"0\">";
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			$(options).each(function(k,json){
				/***************************************************************************************
				*输出分界线
				****************************************************************************************/
				strTemplate += "<tr><td style=\"font-size:0px;height:10px\" colspan=\"3\"></td></tr>";
				/***************************************************************************************
				*输出店铺信息
				****************************************************************************************/
				strTemplate += "<tr class=\"hback\">";
				strTemplate +="<td class=\"selector\" style=\"height:30px;width:24px;padding:8px 8px !important;\" operate=\"selector\">";
				strTemplate +="<input operate=\"items\" type=\"checkbox\" name=\"historyID\" value=\""+json['historyid']+"\" />";
				strTemplate +="</td>";
				strTemplate += "<td style=\"border-left:#f4f4f4 solid 1px;\" class=\"thumb\">";
				strTemplate += "<img style=\"max-width:60px;height:70px;border:#f4f4f4 solid 1px;padding:2px;background:#fff\" src=\""+json['thumb']+"\" />";
				
				strTemplate +="";
				strTemplate += "</td>";
				strTemplate += "<td class=\"text\" onclick=\"window.location='show.aspx?action=show&showid="+json["productid"]+"'\">";
				strTemplate += "<div class=\"title\">"+json['strtitle']+"</div>";
				strTemplate += "<div class=\"fot\">";
				strTemplate += "<span class=\"amt\"><font>￥</font>"+json['retailamount']+"</span>";
				strTemplate += "<span class=\"num\">"+json['salenumber']+"人付款</span>";
				strTemplate += "</div>";
				strTemplate += "</td>";
				strTemplate += "</tr>";					 
			});
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			strTemplate += "</table>";
			/***************************************************************************************
			*框架内容重新赋值
			****************************************************************************************/
			$($contianer!=undefined && $contianer!=null 
			&& $(strTemplate)[0]!=undefined && $(strTemplate)[0]!=null)
			{
				$($contianer).html(strTemplate);
				/************************************************************************************
				*定义全选事件
				*************************************************************************************/
				$("#frm-selectionAll").change(function(){
					try{
						var $thus = this;
						$($contianer).find("input[operate=\"items\"]").each(function(){
							this.checked = $thus.checked;														 
						});	
					}catch(err){}
				});
				/************************************************************************************
				*删除当前的历史记录
				*************************************************************************************/
				$("#frm-delButton").click(function(){
					try{
						if(confirm('你确定要删除浏览历史?')){
							getSelectionItems(function(historyID){
								window.location = "history.aspx?action=del&historyId="+historyID+"";						   
							});
						};
					}catch(err){alert('数据删除过程中发生错误!');return false;}
				});
			};
		};
		/********************************************************************************************
		*获取用户选择的选项
		*********************************************************************************************/
		var getSelectionItems = function(back)
		{
			var HistoryID = "";
			try{
				$($contianer).find("input[operate=\"items\"]").each(function(){
					if(this!=undefined && this!=null && this.checked){
						var strValue = parseInt($(this).val()) || 0;
						if(strValue!=undefined && strValue!=0 && HistoryID==""){
							HistoryID=strValue;
						}else if(strValue!=undefined && strValue!=0 && HistoryID!=""){
							HistoryID=HistoryID+","+strValue;	
						}
					}														 
				});	
			}catch(err){};
			/********************************************************************************************
			*返回回调对象
			*********************************************************************************************/
			try{
				if(HistoryID!=undefined && HistoryID!="" 
				&& back!=undefined && back!=null && typeof(back)=='function')
				{
					back(HistoryID)	
				}else{
					alert('未选中任何对象！');return false;	
				}
			}catch(err){}
			return HistoryID;
		}
		/********************************************************************************************
		*开始加载参数对象信息
		*********************************************************************************************/
		try{
			var options = options || {};
			//try{options["xml"] = options["xml"] || cfg["Productxml"];	}catch(err){}
			if(options!=undefined && options!=null 
			&& typeof(options)=='object' && options.length>=1 
			&& options[0]["historyid"]!=undefined 
			&& options[0]["historyid"]!="")
			{
				render();
			};
		}catch(err){}		
	};
})(jQuery);
/****************************************************************************************
*加载网页中的事件信息
*****************************************************************************************/
$(function(){
	/****************************************************************************************
	*展示关闭属性面板
	*****************************************************************************************/
	$("#frm-menu").find("td").click(function(){
		try{
			$("#frm-menu").find("td").removeClass("current");
			$(this).addClass('current');
		}catch(err){};
		try
		{
			var filter = $(this).attr("filter");
			if(filter!=undefined && filter!=null && filter!="")
			{
				try{
					var f = {};
					try{f = $.parseJSON(filter);}catch(err){}
					$("#frm-contianer").ShowOrder(orderList,f);	
				}catch(err){}
			}else{
				$("#frm-contianer").ShowOrder(orderList);	
			}
		}catch(err){}
	});
});