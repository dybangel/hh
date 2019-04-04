/*******************************************************************************************
* 发送消息通知接口信息
********************************************************************************************/
var SendNotify = function(orderId)
{
	try
	{
		if(orderId!=undefined && orderId!=null 
		&& orderId!="" && !isNaN(orderId))
		{
			Send({"url":"Order.aspx?action=notify&orderid="+orderId+""});
		}
	}catch(err){};
};
/*******************************************************************************************
* jQuery扩展器
********************************************************************************************/
;(function($) {
"use strict";
	/****************************************************************************************
	*加载用户订单列表
	*****************************************************************************************/
	$.fn.ShowProcess = function(options)
	{
		var $contianer = this;
		/****************************************************************************************
		*显示退货流程的内容信息
		*****************************************************************************************/
		var ShowProcess = function(ProcessMode,ProcessOpt)
		{
			var ProcessOpt = ProcessOpt || {};
			var ProcessMode = parseInt(ProcessMode) || 0;
			var strTemplate = "";
			if(ProcessMode==0)
			{
				try{
					strTemplate+="<div>退款类型:"+(ProcessOpt['RetuesMode']=="0" ? "仅退款" : "退货退款")+"</div>";
					strTemplate+="<div>退款原因:"+ProcessOpt['returnWhy']+"("+ProcessOpt['strRemark']+")"+"</div>";
					strTemplate+="<div>退款金额:<font style=\"color:#f00\">￥"+ProcessOpt['returnAmount']+"</font></div>";	
				}catch(err){}
				/***********************************************************************
				*显示加载图片信息
				************************************************************************/
				try{
					if(ProcessOpt['Maps']!=undefined && ProcessOpt['Maps']!=null 
					&& ProcessOpt['Maps']!="" && typeof(ProcessOpt['Maps']) && ProcessOpt['Maps']["items"]!=undefined && ProcessOpt['Maps']["items"]!=null)
					{
						try{
							strTemplate+="<div class=\"maps\" operate=\"maps\">";
							if(typeof(ProcessOpt['Maps']["items"])=='string'){
								strTemplate+="<img style=\"max-width:60px;height:60px;border:#ccc solid 1px;padding:2px;background:#fff;margin-right:5px;\" operate=\"preview\" src=\""+ProcessOpt['Maps']["items"]+"\">";	
							}else
							{
								$(ProcessOpt['Maps']["items"]).each(function(j,items){
									strTemplate+="<img style=\"max-width:60px;height:60px;border:#ccc solid 1px;padding:2px;background:#fff;margin-right:5px;\" operate=\"preview\" src=\""+items+"\">";							  
								});
							}
							strTemplate+="</div>";
						}catch(err){}
					}
				}catch(err){}
			}
			else if(ProcessMode==1)
			{
				strTemplate+="<div>拒绝原因:"+ProcessOpt['strRemark']+"</div>";
				
			}
			else if(ProcessMode==2)
			{
				try
				{
					strTemplate+="<div>";
					if(ProcessOpt["Fullname"]!=undefined && ProcessOpt["Fullname"]!=null 
					&& ProcessOpt["Fullname"]!=""){
						strTemplate +="退款地址:"+ProcessOpt["Fullname"]+"&nbsp;&nbsp;&nbsp;";
					};
					if(ProcessOpt["strMobile"]!=undefined && ProcessOpt["strMobile"]!=null 
					&& ProcessOpt["strMobile"]!=""){
						strTemplate +="电话:"+ProcessOpt["strMobile"]+"";
					};
					strTemplate +="</div>";
					/*********************************************************
					*收货地址
					***********************************************************/
					if(ProcessOpt["strCity"]!=undefined && ProcessOpt["strCity"]!="" 
					&& ProcessOpt["strCity"]!="")
					{
						strTemplate +="<div>";
						strTemplate +=""+ProcessOpt["strCity"]+"";
						strTemplate +="(请认真填写商品订单单号一并寄回)";
						strTemplate +="</div>";
					}
					/*********************************************************
					*商家备注信息
					***********************************************************/
					if(ProcessOpt["strRemark"]!=undefined && ProcessOpt["strRemark"]!="" 
					&& ProcessOpt["strRemark"]!="")
					{
						strTemplate +="<div>";
						strTemplate +="商家备注:";
						strTemplate +="<font style=\"color:#f00\">";
						strTemplate +=""+ProcessOpt["strRemark"]+"";
						strTemplate +="</font>";
						strTemplate +="</div>";
					}
				}catch(err){}
			}
			else if(ProcessMode==3)
			{
				try{
					strTemplate+="<div>物流公司:"+ProcessOpt['Deliveryname']+"</div>";;
					strTemplate+="<div>快递单号:"+ProcessOpt['ExpressKey']+"</div>";
					strTemplate+="<div>联系电话:"+ProcessOpt['strTel']+"</div>";
					strTemplate+="<div>买家备注:"+ProcessOpt['strRemark']+"</div>";
				}catch(err){}
				/***********************************************************************
				*显示加载图片信息
				************************************************************************/
				try{
					if(ProcessOpt['Maps']!=undefined && ProcessOpt['Maps']!=null 
					&& ProcessOpt['Maps']!="" && typeof(ProcessOpt['Maps']) && ProcessOpt['Maps']["items"]!=undefined && ProcessOpt['Maps']["items"]!=null)
					{
						try{
							strTemplate+="<div class=\"maps\" operate=\"maps\">";
							if(typeof(ProcessOpt['Maps']["items"])=='string'){
								strTemplate+="<img style=\"max-width:60px;height:60px;border:#ccc solid 1px;padding:2px;background:#fff;margin-right:5px;\" operate=\"preview\" src=\""+ProcessOpt['Maps']["items"]+"\">";	
							}else
							{
								$(ProcessOpt['Maps']["items"]).each(function(j,items){
									strTemplate+="<img style=\"max-width:60px;height:60px;border:#ccc solid 1px;padding:2px;background:#fff;margin-right:5px;\" operate=\"preview\" src=\""+items+"\">";							  
								});
							}
							strTemplate+="</div>";
						}catch(err){}
					}
				}catch(err){}
			}
			else if(ProcessMode==98)
			{
				try{
					strTemplate+="<div>投诉原因:"+ProcessOpt['ReportMode']+"</div>";;
					strTemplate+="<div>投诉内容:"+ProcessOpt['ReportText']+"</div>";
				}catch(err){}
				/***********************************************************************
				*显示加载图片信息
				************************************************************************/
				try{
					if(ProcessOpt['Maps']!=undefined && ProcessOpt['Maps']!=null 
					&& ProcessOpt['Maps']!="" && typeof(ProcessOpt['Maps']) && ProcessOpt['Maps']["items"]!=undefined && ProcessOpt['Maps']["items"]!=null)
					{
						try{
							strTemplate+="<div class=\"maps\" operate=\"maps\">";
							if(typeof(ProcessOpt['Maps']["items"])=='string'){
								strTemplate+="<img style=\"max-width:60px;height:60px;border:#ccc solid 1px;padding:2px;background:#fff;margin-right:5px;\" operate=\"preview\" src=\""+ProcessOpt['Maps']["items"]+"\">";	
							}else
							{
								$(ProcessOpt['Maps']["items"]).each(function(j,items){
									strTemplate+="<img style=\"max-width:60px;height:60px;border:#ccc solid 1px;padding:2px;background:#fff;margin-right:5px;\" operate=\"preview\" src=\""+items+"\">";							  
								});
							}
							strTemplate+="</div>";
						}catch(err){}
					}
				}catch(err){}
				
			};
			/*******************************************************************************************
			* 返回数据处理结果
			********************************************************************************************/
			return strTemplate ;
		};
		
		/*******************************************************************************************
		* 渲染属性选择器信息
		********************************************************************************************/
		var RenderContianer = function()
		{
			var strTemplate = "";
			var StarLength = parseInt(options.length-1) || 0;
			strTemplate += "<table id=\"frm-tabs\" cellpadding=\"3\" cellspacing=\"1\" border=\"0\">";
			$(options).each(function(k,json){
				/***************************************************************************************
				*输出分界线
				****************************************************************************************/
				strTemplate += "<tr><td style=\"font-size:0px;height:24px\" colspan=\"3\"></td></tr>";
				var ProcessMode = parseInt(json['processmode']) || 0;
				/***************************************************************************************
				*输出店铺信息
				****************************************************************************************/
				strTemplate += "<tr class=\"hback\">";
				strTemplate += "<td class=\"lines\"></td>";
				strTemplate += "<td class=\"process\" colspan=\"2\">";
				strTemplate += "<div class=\"title\">"+json["processtitle"]+"</div>";
				/********************************************************************************
				*显示倒计时
				*********************************************************************************/
				strTemplate += "<div class=\"interval\">";
				if(k==0 && json['affairs']!=100 && json['affairs']!=255){strTemplate += "<span style=\"color:#cd0000;\" value=\""+(parseInt(json['interval']) || 0)+"\" id=\"interval\">00 00:00:00</span>";}
				else{strTemplate += "<font style=\"color:#cd0000\">(已完成)</font>";}
				strTemplate += "系统将自动处理";
				strTemplate += "</div>";
				/********************************************************************************
				*显示备注信息
				*********************************************************************************/
				strTemplate += "<div class=\"remark\">";
				/********************************************************************************
				*将流程集合转换为JSON格式对象
				*********************************************************************************/
				var ProcessOpt = {};
				try{ProcessOpt = $.xml2json(json['processxml'] || "") || {};
				}catch(err){}
				if(ProcessOpt!=undefined && ProcessOpt!=null && typeof(ProcessOpt)=='object' 
				&& ProcessMode!=undefined && ProcessMode!=null && !isNaN(ProcessMode))
				{
					try{
						strTemplate +=ShowProcess(ProcessMode,ProcessOpt);	
					}catch(err){}
				}
				/********************************************************************************
				*显示流程集合信息
				*********************************************************************************/
				
				strTemplate += "</div>";
				/********************************************************************************
				*显示处理日期
				*********************************************************************************/
				strTemplate += "<div class=\"times\">"+json["addtime"]+"</div>";
				/********************************************************************************
				*显示操作选项按钮
				*********************************************************************************/
				/********************************************************************************
				*显示备注信息
				*********************************************************************************/
				if(k==0)
				{
					strTemplate += "<div class=\"button\">";
					if(ProcessMode==1)
					{
						strTemplate+="<input type=\"button\" onclick=\"if(confirm('你确定要投诉卖家?')){window.location='process.aspx?action=report&orderid="+json['orderid']+"';}\" value=\"投诉卖家\" />";
						strTemplate+="<input type=\"button\" onclick=\"window.location='process.aspx?action=edit&orderid="+json['orderid']+"';\" value=\"修改价格\" />";
						strTemplate+="<input onclick=\"if(!confirm('你确定要取消退货单?')){return false;}else{window.location='process.aspx?action=cancel&orderid="+json['orderid']+"'}\" type=\"button\" value=\"取消退货\" />";	
					}
					else if(ProcessMode==2){
						strTemplate+="<input type=\"button\" onclick=\"window.location='process.aspx?action=delivery&orderid="+json['orderid']+"'\" value=\"填写物流信息\" />";
						strTemplate+="<input onclick=\"if(!confirm('你确定要取消退货单?')){return false;}else{window.location='process.aspx?action=cancel&orderid="+json['orderid']+"'}\" type=\"button\" value=\"取消退货\" />";	
					}
					strTemplate += "</div>";
				}
				strTemplate += "</td>";
				strTemplate += "</tr>";					 
			});
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			strTemplate += "</table>";
			/***************************************************************************************
			*框架内容重新赋值
			****************************************************************************************/
			try{
				if(strTemplate!=undefined && strTemplate!=null 
				&& $(strTemplate)[0]!=undefined && $(strTemplate)[0]!=null)
				{
					$($contianer).html(strTemplate);
					try{
						if(document.getElementById('interval'))
						{
							$(document.getElementById('interval')).interval(function(s){
								//alert(s);														 
							});	
						};
					}catch(err){};
					/***************************************************************************************
					*点击图片预览
					****************************************************************************************/
					$("img[operate=\"preview\"]").click(function(){
						try
						{
							var src = $(this).attr("src") || "";
							if(src!=undefined && src!=null && src!="")
							{
								$("#frmPreviewContianer").html("<img src=\""+src+"\" />");
								$("#frmPreview").show();
							}
						}catch(err){}
					});
				};
			}catch(err){}
			
			
			
		}
		/********************************************************************************************
		*开始加载参数对象信息
		*********************************************************************************************/
		try{
			var options = options || {};
			if(options!=undefined && options!=null && typeof(options)=='object' && options.length>=1 
			&& options[0]["processid"]!=undefined && options[0]["processid"]!=null && options[0]["processid"]!="")
			{
				RenderContianer();
			}
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
	
});