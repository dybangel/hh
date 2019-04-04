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
	}catch(err){}
};
/*******************************************************************************************
* 延长收货
********************************************************************************************/
var SendDelay = function(orderId)
{
	try
	{
		if(orderId!=undefined && orderId!=null 
		&& orderId!="" && !isNaN(orderId))
		{
			Send({"url":"Order.aspx?action=delay&orderid="+orderId+""});
		}
	}catch(err){}	
};
/*******************************************************************************************
* 确认收货
********************************************************************************************/
var ConfirmDelivery = function(orderId)
{
	try
	{
		if(orderId!=undefined && orderId!=null 
		&& orderId!="" && !isNaN(orderId))
		{
			$("#frm-orderid").val(orderId);
			$("#frm-confirmMaster").show();
		}
	}catch(err){}		
};
/*******************************************************************************************
* 获取到退货订单状态信息
********************************************************************************************/
var getReturnAffairsText = function(affairs)
{
	var affairs = parseInt(affairs) || 0;
	var strValue = "等待处理";
	switch(affairs){
		case 0:strValue="等待处理";break;	
		case 1:strValue="拒绝退款";break;	
		case 2:strValue="同意退款";break;	
		case 3:strValue="买家发货";break;	
		case 98:strValue="买家投诉";break;	
		case 100:strValue="交易关闭";break;	
		case 255:strValue="交易完成";break;	
	};
	return strValue;
}
/*******************************************************************************************
* 当前网页中的事件
********************************************************************************************/
$(function(){
	$("#frm-close").click(function(){
		$("#frm-confirmMaster").hide();							   
	});		   
});
/*******************************************************************************************
* jQuery扩展器
********************************************************************************************/
;(function($) {
"use strict";
	/****************************************************************************************
	*加载用户订单列表
	*****************************************************************************************/
	$.fn.ShowOrder = function(options,Filter)
	{
		var $contianer = this;
		/*******************************************************************************************
		* 渲染属性选择器信息
		********************************************************************************************/
		var RenderContianer = function()
		{
			var strTemplate = "";
			strTemplate += "<table id=\"frm-tabs\" cellpadding=\"3\" cellspacing=\"1\" border=\"0\">";
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			$(options).each(function(k,json){
				/***************************************************************************************
				*过滤字符穿信息
				****************************************************************************************/
				if(Filter!=undefined && typeof(Filter)=='object' 
				&& Filter['affairs']!=undefined && Filter['affairs']!=""
				&& Filter['affairs']!=undefined && json['affairs']!="" 
				&& Filter['affairs']!=json['affairs'])
				{return true;}
				/***************************************************************************************
				*待评价信息
				****************************************************************************************/
				if(Filter!=undefined && typeof(Filter)=='object' 
				&& Filter['iscomment']!=undefined && Filter['iscomment']=="1" 
				&& json['iscomment']!=undefined && json['iscomment']==0)
				{
					if(json['affairs']!=3 && json['affairs']!=255){
						return true;	
					}
				}
				else if(Filter!=undefined && typeof(Filter)=='object' 
				&& Filter['iscomment']!=undefined && Filter['iscomment']=="1" 
				&& json['iscomment']!=undefined && json['iscomment']==1)
				{
					return true;
				};
				/***************************************************************************************
				*退货中
				****************************************************************************************/
				if(Filter!=undefined && typeof(Filter)=='object' 
				&& Filter['isreturn']!=undefined && json['isreturn']!=undefined 
				&& json['isreturn']!=1 && Filter['isreturn']==1)
				{return true;}
				/***************************************************************************************
				*输出分界线
				****************************************************************************************/
				strTemplate += "<tr><td style=\"font-size:0px;height:10px\" colspan=\"3\"></td></tr>";
				/***************************************************************************************
				*输出店铺信息
				****************************************************************************************/
				strTemplate += "<tr class=\"business\">";
				strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;\"></td>";
				strTemplate += "<td onclick=\"window.location='order.aspx?action=show&orderid="+json["orderid"]+"'\" colspan=\"2\">";
				strTemplate +="<img style=\"width:20px;height:20px;border:#eee solid 1px;padding:2px;background:#fff;position:absolute;left:2px;top:10px;\" src=\""+json['strthumb']+"\" />";
				strTemplate += "<span style=\"position:absolute;left:34px;top:12px;\">";
				strTemplate +=""+json["businessname"]+"";
				if(json['ordermode']!="0"){strTemplate +="(批发)";}
				else{strTemplate +="(零售)";}
				strTemplate +="</span>";
				strTemplate +="<label affairs=\""+json["affairs"]+"\">";
				switch(parseInt(json["affairs"])){
					case 0:strTemplate += "等待付款";break;
					case 1:strTemplate += "等待发货";break;
					case 2:strTemplate += "等待收货";break;
					case 3:strTemplate += "等待评价";break;
					case 255:strTemplate += "交易成功";break;
					case 100:strTemplate += "交易关闭";break;
				};
				strTemplate +="</label>";
				strTemplate += "</td>";
				strTemplate += "</tr>";
				/***************************************************************************************
				*输出订单中的商品信息
				****************************************************************************************/
				var orderNumber = 0;
				if(json["productxml"]!=undefined && json["productxml"]!=null 
				&& json["productxml"]!="" && json["productxml"].indexOf("items")!=-1)
				{
					var rootxml = $(json["productxml"])[0];
					$(rootxml).find("items").each(function(i,node){
						
						var amount = parseFloat($(node).find("amount").html()) || 0;
						var number = parseFloat($(node).find("number").html()) || 0;
						var isreturn = parseFloat($(node).find("isreturn").html()) || 0;
						var rnumber = parseFloat($(node).find("rnumber").html()) || 0;
						var isdiscount = parseFloat($(node).find("isdiscount").html()) || 0;
						orderNumber = orderNumber+number;
						strTemplate += "<tr class=\"product\">";
						strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;border-bottom:0px;\"></td>";
						strTemplate += "<td class=\"productinfo\" colspan=\"2\">";
						strTemplate +="<div class=\"thumb\"><img style=\"max-width:60px;height:70px;border:#ccc solid 1px;padding:2px;background:#fff\" src=\""+$(node).find("thumb").html()+"\" /></div>";
						strTemplate += "<div class=\"title\">"+$(node).find("strtitle").html()+"</div>";
						strTemplate += "<div class=\"property\">";
						strTemplate +=""+$(node).find("property").html()+"";
						if(isreturn!=undefined && isreturn!=null && isreturn==1 
						&& rnumber!=undefined && rnumber!=null && rnumber!=0)
						{
							var raff = parseFloat($(node).find("raff").html()) || 0;
							var rffText = getReturnAffairsText(raff);
							var returnId = parseFloat($(node).find("returnid").html()) || 0;
							if(returnId!=undefined && returnId!=0){
								
								strTemplate +="<font onclick=\"window.location='Process.aspx?action=show&orderid="+returnId+"'\" style=\"color:#cd0000\">(退货"+rnumber+"件,"+rffText+")</font>";	
							}else{
								strTemplate +="<font style=\"color:#cd0000\">(退货"+rnumber+"件,"+rffText+")</font>";			
							}
						};
						strTemplate +="</div>";
						strTemplate += "<div class=\"amount\">";
						strTemplate +="<font style=\"font-weight:100;font-size:10px;\">￥</font>";
						strTemplate +="<span style=\"margin:0px 4px;\">"+amount+"</span>";
						if(isdiscount=="1"){
							strTemplate +="<font style=\"color:#cd0000;font-size:10px;font-weight:100\">[特]</font>";
						}
						strTemplate +="</div>";
						strTemplate += "<div class=\"number\">×"+number+"</div>";
						strTemplate += "</td>";
						strTemplate += "</tr>";
						
					});
				};
				/***************************************************************************************
				*显示订单金额,订单数量
				****************************************************************************************/
				strTemplate += "<tr class=\"calculation\">";
				strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;border-bottom:0px;\"></td>";
				strTemplate += "<td colspan=\"2\">";
				strTemplate += "共"+json["goodsnumber"]+"件商品";
				strTemplate +="总金额：(含运费"+json["fareamount"]+"),<span><font style=\"font-size:10px;\">￥</font>"+json["orderamount"]+"元</span>,实际支付<span><font style=\"font-size:10px;\">￥</font>"+json["actualamount"]+"元</span>";
				strTemplate += "</td>";
				strTemplate += "</tr>";
				/***************************************************************************************
				*订单操作选项
				****************************************************************************************/
				var affairs = parseInt(json["affairs"]) || 0;
				var iscomment = parseInt(json["iscomment"]) || 0; 
				//if(affairs!=3 && affairs!=255 && iscomment!=1)
				//{
					strTemplate += "<tr class=\"buttons\">";
					strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;\"></td>";
					strTemplate += "<td colspan=\"2\">";
					
					if(affairs==0)
					{
						strTemplate += "<a onclick=\"if(!confirm('你确定要删除订单?')){return false;}\" href=\"order.aspx?action=del&orderid="+json['orderid']+"\">取消订单</a>";
						strTemplate += "<a href=\"order.aspx?action=defary&orderid="+json['orderid']+"\">立即付款</a>";
					}else if(affairs==1)
					{
						strTemplate += "<a href=\"order.aspx?action=return&orderid="+json['orderid']+"\">申请退款</a>";
						strTemplate += "<a onclick=\"SendNotify('"+json['orderid']+"')\">提醒发货</a>";		
					}else if(affairs==2){
						strTemplate += "<a href=\"Order.aspx?action=return&orderid="+json["orderid"]+"\">申请退款</a>";
						strTemplate += "<a onclick=\"SendDelay('"+json["orderid"]+"')\" href=\"javascript:void(0);\">延长收货</a>";
						strTemplate += "<a href=\"order.aspx?action=delivery&orderid="+json['orderid']+"\">查看物流</a>";
						strTemplate += "<a onclick=\"ConfirmDelivery('"+json["orderid"]+"')\" href=\"javascript:void(0);\">确认收货</a>";
					}else if(affairs==3){
						strTemplate += "<a href=\"order.aspx?action=comment&orderid="+json['orderid']+"\">发表评价</a>";	
					}else if(affairs==100){
						strTemplate += "<a onclick=\"if(!confirm('你确定要删除订单?')){return false;}\" href=\"order.aspx?action=del&orderid="+json['orderid']+"\">删除订单</a>";	
					}else if(affairs==255){
						strTemplate += "<a onclick=\"if(!confirm('你确定要删除订单?')){return false;}\" href=\"order.aspx?action=del&orderid="+json['orderid']+"\">删除订单</a>";	
						strTemplate += "<a href=\"order.aspx?action=comment&orderid="+json['orderid']+"\">追加评价</a>";	
					}
					strTemplate += "</td>";
					strTemplate += "</tr>";
				//}
									 
			});
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			strTemplate += "</table>";
			/***************************************************************************************
			*框架内容重新赋值
			****************************************************************************************/
			$($contianer).html(strTemplate);
		}
		/********************************************************************************************
		*开始加载参数对象信息
		*********************************************************************************************/
		try{
			var options = options || {};
			var Filter = Filter || {};
			//try{options["xml"] = options["xml"] || cfg["Productxml"];	}catch(err){}
			if(options!=undefined && options!=null && typeof(options)=='object' && options.length>=1 
			&& options[0]["orderkey"]!=undefined && options[0]["orderkey"]!=null && options[0]["orderkey"]!="" 
			&& options[0]["orderid"]!=undefined && options[0]["orderid"]!=null && options[0]["orderid"]!="")
			{
				RenderContianer();
			};
		}catch(err){}		
	};
	/****************************************************************************************
	*加载用户订单详情信息
	*****************************************************************************************/
	$.fn.ShowDetails = function(json)
	{
		var $contianer = this;
		var render = function()
		{
			var strTemplate = "";
			strTemplate += "<table id=\"frm-tabs\" cellpadding=\"3\" cellspacing=\"1\" border=\"0\">";
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			/***************************************************************************************
			*输出分界线
			****************************************************************************************/
			strTemplate += "<tr><td style=\"font-size:0px;height:10px\" colspan=\"3\"></td></tr>";
			/***************************************************************************************
			*输出店铺信息
			****************************************************************************************/
			strTemplate += "<tr class=\"business\">";
			strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;\"></td>";
			strTemplate += "<td onclick=\"window.location='site.aspx?action=show&siteid="+json["businessid"]+"'\" colspan=\"2\">";
			strTemplate += "<div class=\"ico\"><img style=\"width:16px;height:16px;\" src=\""+json["strthumb"]+"\"></div>";
			strTemplate += "<span class=\"name\">";
			strTemplate +=""+json["businessname"]+"";
			if(json['ordermode']!="0"){strTemplate +="(批发)";}
			else{strTemplate +="(零售)";}
			strTemplate +="</span>"
			strTemplate +="<label class=\"aff\" affairs=\""+json["affairs"]+"\">";
			switch(parseInt(json["affairs"]) || 0){
				case 0:strTemplate += "等待付款";break;
				case 1:strTemplate += "等待发货";break;
				case 2:strTemplate += "等待收货";break;
				case 98:strTemplate += "等待退款";break;
				case 99:strTemplate += "订单投诉";break;
				case 3:strTemplate += "等待评价";break;
				case 255:strTemplate += "交易成功";break;
				case 100:strTemplate += "交易关闭";break;
			};
			strTemplate +="</label>";
			strTemplate += "</td>";
			strTemplate += "</tr>";
			/***************************************************************************************
			*输出订单中的商品信息
			****************************************************************************************/
			try{
				if(json["productxml"]!=undefined && json["productxml"]!=null 
				&& json["productxml"]!="" && json["productxml"].indexOf("items")!=-1)
				{
					var rootxml = $(json["productxml"])[0];
					$(rootxml).find("items").each(function(i,node){
						
						var amount = parseFloat($(node).find("amount").html()) || 0;
						var number = parseFloat($(node).find("number").html()) || 0;
						var isreturn = parseFloat($(node).find("isreturn").html()) || 0;
						var rnumber = parseFloat($(node).find("rnumber").html()) || 0;
						var isdiscount = parseFloat($(node).find("isdiscount").html()) || 0;
						strTemplate += "<tr class=\"product\">";
						strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;border-bottom:0px;\"></td>";
						strTemplate += "<td class=\"productinfo\" colspan=\"2\">";
						strTemplate +="<div class=\"thumb\"><img style=\"max-width:60px;height:70px;border:#ccc solid 1px;padding:2px;background:#fff\" src=\""+$(node).find("thumb").html()+"\" /></div>";
						strTemplate += "<div class=\"title\">"+$(node).find("strtitle").html()+"</div>";
						strTemplate += "<div class=\"property\">";
						strTemplate +=""+$(node).find("property").html()+"";
						if(isreturn!=undefined && isreturn!=null && isreturn==1 
						&& rnumber!=undefined && rnumber!=null && rnumber!=0)
						{
							var raff = parseFloat($(node).find("raff").html()) || 0;
							var rffText = getReturnAffairsText(raff);
							var returnId = parseFloat($(node).find("returnid").html()) || 0;
							if(returnId!=undefined && returnId!=0){
								
								strTemplate +="<font onclick=\"window.location='Process.aspx?action=show&orderid="+returnId+"'\" style=\"color:#cd0000\">(退货"+rnumber+"件,"+rffText+")</font>";	
							}else{
								strTemplate +="<font style=\"color:#cd0000\">(退货"+rnumber+"件,"+rffText+")</font>";			
							}
						};
						strTemplate += "</div>";
						strTemplate += "<div class=\"amount\">";
						strTemplate +="<font style=\"font-weight:100;font-size:10px;\">￥</font>";
						strTemplate +="<span style=\"margin:0px 4px;\">"+amount+"</span>";
						if(isdiscount=="1"){
							strTemplate +="<font style=\"color:#cd0000;font-size:10px;font-weight:100\">[特]</font>";
						}
						strTemplate +="</div>";
						strTemplate += "<div class=\"number\">×"+number+"</div>";
						strTemplate += "</td>";
						strTemplate += "</tr>";
						
					});
				};
			}catch(err){};
			/***************************************************************************************
			*联系卖家信息
			****************************************************************************************/
			try{
				strTemplate += "<tr class=\"line\"><td style=\"font-size:0px;height:6px\" colspan=\"3\"></td></tr>";
				strTemplate += "<tr class=\"hback\">";
				strTemplate += "<td colspan=\"3\" style=\"padding:0px;border-top:#eee solid 1px;border-bottom:#eee solid 1px;\">";
				strTemplate += "<div id=\"frm-contact\">";
				strTemplate += "<div id=\"frm-chat\"><a href=\"mqqwpa://im/chat?chat_type=wpa&uin={$qq}&version=1&src_type=web&web_src=oicqzone.com\"><img style=\"width:18px;height:18px;position:relative;top:3px;left:-5px\" src=\"template/images/maijia.png\" />联系卖家</a></div>";
				strTemplate += "<div id=\"frm-call\"><a href=\"tel:"+json['strmobile']+"\"><img style=\"width:18px;height:18px;position:relative;top:3px;left:-5px\" src=\"template/images/maphone.png\" />拨打卖家电话</a></div>";
				strTemplate +="</div>";
				strTemplate += "</td>";
				strTemplate += "</tr>";
				strTemplate += "<tr class=\"line\"><td style=\"font-size:0px;height:6px\" colspan=\"3\"></td></tr>";
			}catch(err){}
			/***************************************************************************************
			*计算订单金额信息
			****************************************************************************************/
			try{
				strTemplate += "<tr class=\"calculation\">";
				strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;border-bottom:0px;\"></td>";
				strTemplate += "<td class=\"name\">";
				strTemplate += "<div>货品数量</div>";
				strTemplate += "<div>货款金额</div>";
				strTemplate += "<div>运费</div>";
				strTemplate += "<div>订单总额</div>";
				strTemplate += "<div>优惠金额</div>";
				strTemplate += "<div>应付金额(含运费)</div>";
				strTemplate += "</td>";
				strTemplate += "<td class=\"value\">";
				strTemplate += "<div>"+json['goodsnumber']+" 件</div>";
				strTemplate += "<div><font>￥</font>"+json['goodsamount']+"</div>";
				strTemplate += "<div><font>￥</font>"+json['fareamount']+"</div>";
				strTemplate += "<div><font>￥</font>"+json['orderamount']+"</div>";
				strTemplate += "<div><font>￥</font>"+json['breaksamount']+"</div>";
				strTemplate += "<div style=\"color:#ff6600;font-size:16px\"><font>￥</font>"+json['actualamount']+"</div>";
				strTemplate += "</td>";
				strTemplate += "</tr>";
			}catch(err){}
			/***************************************************************************************
			*显示其他选项信息
			****************************************************************************************/
			try{
				strTemplate += "<tr class=\"hback\">";
				strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;border-bottom:0px;\"></td>";
				strTemplate += "<td class=\"name\">订单号</td>";
				strTemplate += "<td class=\"value\">"+json['orderkey']+"</td>";
				strTemplate += "</tr>";
				
				strTemplate += "<tr class=\"hback\">";
				strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;\"></td>";
				strTemplate += "<td class=\"name\">创建日期</td>";
				strTemplate += "<td class=\"value\">"+json['addtime']+"</td>";
				strTemplate += "</tr>";
				}catch(err){}
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			strTemplate += "</table>";
			/****************************************************************************************
			*为控件赋值
			*****************************************************************************************/
			$($contianer).html(strTemplate);
		};
		/****************************************************************************************
		*渲染订单中的操作按钮
		*****************************************************************************************/
		var renderMenu = function()
		{
			var strTemplate = "";
			if(json!=undefined && json!=null && typeof(json)=='object' 
			&& json['orderid']!=undefined && json['orderid']!=null && json['orderid']!="" 
			&& json['affairs']!=undefined && json['affairs']!="")
			{
				var affairs = parseInt(json['affairs']) || 0;
				var iscomment = parseInt(json['iscomment']) || 0;
				if(affairs==0)
				{
					strTemplate += "<a onclick=\"if(!confirm('你确定要删除订单?')){return false;}\" href=\"order.aspx?action=del&orderid="+json['orderid']+"\">取消订单</a>";
					strTemplate += "<a href=\"order.aspx?action=defary&orderid="+json['orderid']+"\">立即付款</a>";
				}else if(affairs==1)
				{
					strTemplate += "<a href=\"order.aspx?action=return&orderid="+json['orderid']+"\">申请退款</a>";
					strTemplate += "<a onclick=\"SendNotify('"+json['orderid']+"')\">提醒发货</a>";		
				}else if(affairs==2){
					strTemplate += "<a href=\"Order.aspx?action=return&orderid="+json["orderid"]+"\">申请退款</a>";
					strTemplate += "<a onclick=\"SendDelay('"+json["orderid"]+"')\" href=\"javascript:void(0);\">延长收货</a>";
					strTemplate += "<a href=\"order.aspx?action=delivery&orderid="+json['orderid']+"\">查看物流</a>";
					strTemplate += "<a onclick=\"ConfirmDelivery('"+json["orderid"]+"')\" href=\"javascript:void(0);\">确认收货</a>";
				}else if(affairs==3){
					strTemplate += "<a href=\"order.aspx?action=comment&orderid="+json['orderid']+"\">发表评价</a>";	
				}else if(affairs==100){
					strTemplate += "<a onclick=\"if(!confirm('你确定要删除订单?')){return false;}\" href=\"order.aspx?action=del&orderid="+json['orderid']+"\">删除订单</a>";	
				}else if(affairs==255){
					strTemplate += "<a onclick=\"if(!confirm('你确定要删除订单?')){return false;}\" href=\"order.aspx?action=del&orderid="+json['orderid']+"\">删除订单</a>";	
					strTemplate += "<a href=\"order.aspx?action=comment&orderid="+json['orderid']+"\">追加评价</a>";	
				}
			};
			$("#frmMenuMaster").html(strTemplate);
		}
		
		/****************************************************************************************
		*加载用户订单详情信息
		*****************************************************************************************/
		if(json!=undefined && json!=null && typeof(json)=='object' 
		&& json["businessid"]!=undefined && json['businessid']!=null 
		&& json['businessid']!="" && json['orderid']!=undefined 
		&& json['orderid']!=null && json['userid']!=undefined)
		{
			try{
				render();
				renderMenu();
			}catch(err){}
		}
	};
	/***************************************************************************************
	*用户退款退货
	****************************************************************************************/
	$.fn.ShowReturn = function(json)
	{
		var $contianer = this;
		var render = function()
		{
			var strTemplate = "<table id=\"frm-tabs\" cellpadding=\"3\" cellspacing=\"1\" border=\"0\">";
			/***************************************************************************************
			*输出店铺信息
			****************************************************************************************/
			try{
				strTemplate += "<tr class=\"business\">";
				strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;\"></td>";
				strTemplate += "<td colspan=\"2\">";
				strTemplate += "<div class=\"ico\"><img style=\"width:16px;height:16px;\" src=\""+json["strthumb"]+"\"></div>";
				strTemplate += "<span class=\"name\">"+json["businessname"]+"</span>";
				strTemplate +="<label class=\"aff\" affairs=\""+json["affairs"]+"\">";
				switch(parseInt(json["affairs"]) || 0){
					case 0:strTemplate += "等待付款";break;
					case 1:strTemplate += "等待发货";break;
					case 2:strTemplate += "等待收货";break;
					case 98:strTemplate += "等待退款";break;
					case 99:strTemplate += "订单投诉";break;
					case 3:strTemplate += "等待评价";break;
					case 255:strTemplate += "交易成功";break;
					case 100:strTemplate += "交易关闭";break;
				};
				strTemplate +="</label>";
				strTemplate += "</td>";
				strTemplate += "</tr>";
			}catch(err){}
			/***************************************************************************************
			*输出订单中的商品信息
			****************************************************************************************/
			try{
				if(json["productxml"]!=undefined && json["productxml"]!=null 
				&& json["productxml"]!="" && json["productxml"].indexOf("items")!=-1)
				{
					var rootxml = $(json["productxml"])[0];
					$(rootxml).find("items").each(function(i,node){
						
						var amount = parseFloat($(node).find("amount").html()) || 0;
						var number = parseFloat($(node).find("number").html()) || 0;
						var rnumber = parseFloat($(node).find("rnumber").html()) || 0;
						var isdiscount = parseFloat($(node).find("isdiscount").html()) || 0;
						var property = $(node).find("property").html() || "";
						var productid = parseInt($(node).find("productid").html()) || "";
						var md5Key = $.md5(property+""+productid);
						if(rnumber==0 && number!=0)
						{
							strTemplate += "<tr class=\"product\">";
							strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;border-bottom:0px;\"></td>";
							strTemplate += "<td style=\"width:70px;padding:0px;\"><img style=\"max-width:60px;height:70px;border:#ccc solid 1px;padding:2px;background:#fff\" src=\""+$(node).find("thumb").html()+"\" /></td>";
							strTemplate += "<td class=\"productinfo\">";
							strTemplate += "<div class=\"title\">"+$(node).find("strtitle").html()+"</div>";
							strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:5px;\"></div>";
							strTemplate += "<div class=\"property\">";
							strTemplate += "<div class=\"text\">"+$(node).find("property").html()+"</div>"
							strTemplate += "<div class=\"amount\">";
							strTemplate += "<font style=\"font-weight:100;font-size:10px;\">￥</font>";
							strTemplate += "<span style=\"margin:0px 4px;\">"+amount+"</span>";
							if(isdiscount=="1"){
								strTemplate +="<font style=\"color:#cd0000;font-size:10px;font-weight:100\">[特]</font>";
							}
							strTemplate += "</div>";
							strTemplate += "<div class=\"frmValue\">";
							strTemplate += "<span class=\"reduce\">-</span>";
							strTemplate += "<input type=\"number\" max=\""+number+"\" step=\"1\" min=\"0\" value=\"0\" name=\""+md5Key+"\" />";
							strTemplate += "<span class=\"add\">+</span>";
							strTemplate += "</div>";
							strTemplate += "<div class=\"number\">×"+number+"</div>"
							strTemplate += "</div>"
							strTemplate += "</td>";
							strTemplate += "</tr>";
						}
						
					});
				};
			}catch(err){};
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			strTemplate += "</table>";
			/****************************************************************************************
			*为控件赋值
			*****************************************************************************************/
			if(strTemplate!=undefined && strTemplate!=null 
			&& $(strTemplate)[0]!=undefined && $(strTemplate)[0]!=null)
			{
				$($contianer).html(strTemplate);
				$($contianer).find("span[class=\"reduce\"]").click(function(){
					try{
						var frmInput = $(this.parentNode).find("input[type=\"number\"]")[0];
						if(frmInput!=undefined && frmInput!=null)
						{
							var strValue = parseInt($(frmInput).val()) || 0;
							strValue = strValue-1;
							if(strValue<=0){strValue=0;}
							$(frmInput).val(strValue);
						}
					}catch(err){}
				});
				$($contianer).find("span[class=\"add\"]").click(function(){
					try{
						var frmInput = $(this.parentNode).find("input[type=\"number\"]")[0];
						if(frmInput!=undefined && frmInput!=null)
						{
							var strValue = parseInt($(frmInput).val()) || 0;
							var maxValue = parseInt($(frmInput).attr("max")) || 0;
							strValue = strValue+1;
							if(strValue<=0){strValue=0;}
							if(strValue>=maxValue){strValue=maxValue;}
							$(frmInput).val(strValue);
						}
					}catch(err){}
				});
				$($contianer).find("input[type=\"number\"]").change(function(){
					try{
						var strValue = parseInt($(this).val()) || 0;
						var maxValue = parseInt($(this).attr("max")) || 0;
						if(strValue<=0){strValue=0;}
						if(strValue>=maxValue){strValue=maxValue;}
						$(this).val(strValue);
					}catch(err){}
				});
				/*************************************************************************************
				*订单全部退款
				**************************************************************************************/
				$("#frm-selectionall").click(function(){
					try{
						if(this!=undefined && this!=null && this.checked)
						{
							$($contianer).find("input[type=\"number\"]").each(function(){
									var maxValue = parseInt($(this).attr("max")) || 0;
									if(maxValue<=0){maxValue=0;}
									this.value = maxValue
							});
						}else{
							$($contianer).find("input[type=\"number\"]").each(function(){
								this.value = 0
							});	
						};
					}catch(err){}
				});
			}
			
			
		};
		/****************************************************************************************
		*构架退款表单中的xml集合信息
		*****************************************************************************************/
		var constructXML = function()
		{
				
		}
		/****************************************************************************************
		*加载用户订单详情信息
		*****************************************************************************************/
		if(json!=undefined && json!=null && typeof(json)=='object' 
		&& json["businessid"]!=undefined && json['businessid']!=null 
		&& json['businessid']!="" && json['orderid']!=undefined 
		&& json['orderid']!=null && json['userid']!=undefined)
		{
			try{render();}catch(err){}
		}
	};
	/***************************************************************************************
	*修改退货退款
	****************************************************************************************/
	$.fn.ShowUpdate = function(json)
	{
		var $contianer = this;
		var render = function()
		{
			var strTemplate = "<table id=\"frm-tabs\" cellpadding=\"3\" cellspacing=\"1\" border=\"0\">";
			/***************************************************************************************
			*输出店铺信息
			****************************************************************************************/
			try{
				strTemplate += "<tr class=\"business\">";
				strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;\"></td>";
				strTemplate += "<td colspan=\"2\">";
				strTemplate += "<span style=\"left:0px;\" class=\"name\">"+json["businessname"]+"</span>";
				strTemplate +="<label class=\"aff\" affairs=\""+json["affairs"]+"\">";
				switch(parseInt(json["orderaffairs"]) || 0){
					case 0:strTemplate += "等待付款";break;
					case 1:strTemplate += "等待发货";break;
					case 2:strTemplate += "等待收货";break;
					case 98:strTemplate += "等待退款";break;
					case 99:strTemplate += "订单投诉";break;
					case 3:strTemplate += "等待评价";break;
					case 255:strTemplate += "交易成功";break;
					case 100:strTemplate += "交易关闭";break;
				};
				strTemplate +="</label>";
				strTemplate += "</td>";
				strTemplate += "</tr>";
			}catch(err){}
			/***************************************************************************************
			*输出订单中的商品信息
			****************************************************************************************/
			try{
				if(json["productxml"]!=undefined && json["productxml"]!=null 
				&& json["productxml"]!="" && json["productxml"].indexOf("items")!=-1)
				{
					var rootxml = $(json["productxml"])[0];
					$(rootxml).find("items").each(function(i,node){
						
						var amount = parseFloat($(node).find("amount").html()) || 0;
						var number = parseFloat($(node).find("number").html()) || 0;
						var rnumber = parseFloat($(node).find("rnumber").html()) || 0;
						var property = $(node).find("property").html() || "";
						var productid = parseInt($(node).find("productid").html()) || "";
						var md5Key = $.md5(property+""+productid);
						if(rnumber==0 && number!=0)
						{
							strTemplate += "<tr class=\"product\">";
							strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;border-bottom:0px;\"></td>";
							strTemplate += "<td style=\"width:70px;padding:0px;\"><img style=\"max-width:60px;height:70px;border:#ccc solid 1px;padding:2px;background:#fff\" src=\""+$(node).find("thumb").html()+"\" /></td>";
							strTemplate += "<td class=\"productinfo\">";
							strTemplate += "<div class=\"title\">"+$(node).find("strtitle").html()+"</div>";
							strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:5px;\"></div>";
							strTemplate += "<div class=\"property\">";
							strTemplate += "<div class=\"text\">"+$(node).find("property").html()+"</div>"
							strTemplate += "<div class=\"amount\"><font>￥</font>"+amount+"</div>";
							strTemplate += "<div style=\"bottom:8px;\" class=\"number\">×"+number+"</div>"
							strTemplate += "</div>"
							strTemplate += "</td>";
							strTemplate += "</tr>";
						}
						
					});
				};
			}catch(err){};
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			strTemplate += "</table>";
			/****************************************************************************************
			*为控件赋值
			*****************************************************************************************/
			if(strTemplate!=undefined && strTemplate!=null 
			&& $(strTemplate)[0]!=undefined && $(strTemplate)[0]!=null)
			{
				$($contianer).html(strTemplate);
				$($contianer).find("span[class=\"reduce\"]").click(function(){
					try{
						var frmInput = $(this.parentNode).find("input[type=\"number\"]")[0];
						if(frmInput!=undefined && frmInput!=null)
						{
							var strValue = parseInt($(frmInput).val()) || 0;
							strValue = strValue-1;
							if(strValue<=0){strValue=0;}
							$(frmInput).val(strValue);
						}
					}catch(err){}
				});
				$($contianer).find("span[class=\"add\"]").click(function(){
					try{
						var frmInput = $(this.parentNode).find("input[type=\"number\"]")[0];
						if(frmInput!=undefined && frmInput!=null)
						{
							var strValue = parseInt($(frmInput).val()) || 0;
							var maxValue = parseInt($(frmInput).attr("max")) || 0;
							strValue = strValue+1;
							if(strValue<=0){strValue=0;}
							if(strValue>=maxValue){strValue=maxValue;}
							$(frmInput).val(strValue);
						}
					}catch(err){}
				});
				$($contianer).find("input[type=\"number\"]").change(function(){
					try{
						var strValue = parseInt($(this).val()) || 0;
						var maxValue = parseInt($(this).attr("max")) || 0;
						if(strValue<=0){strValue=0;}
						if(strValue>=maxValue){strValue=maxValue;}
						$(this).val(strValue);
					}catch(err){}
				});
				/*************************************************************************************
				*订单全部退款
				**************************************************************************************/
				$("#frm-selectionall").click(function(){
					try{
						if(this!=undefined && this!=null && this.checked)
						{
							$($contianer).find("input[type=\"number\"]").each(function(){
									var maxValue = parseInt($(this).attr("max")) || 0;
									if(maxValue<=0){maxValue=0;}
									this.value = maxValue
							});
						}else{
							$($contianer).find("input[type=\"number\"]").each(function(){
								this.value = 0
							});	
						};
					}catch(err){}
				});
			}
			
			
		};
		/****************************************************************************************
		*构架退款表单中的xml集合信息
		*****************************************************************************************/
		var constructXML = function()
		{
				
		}
		/****************************************************************************************
		*加载用户订单详情信息
		*****************************************************************************************/
		if(json!=undefined && json!=null && typeof(json)=='object' 
		&& json["businessid"]!=undefined && json['businessid']!=null 
		&& json['businessid']!="" && json['orderid']!=undefined 
		&& json['orderid']!=null && json['userid']!=undefined)
		{
			try{render();}catch(err){}
		}
	};
	/***************************************************************************************
	*发表评价
	****************************************************************************************/
	$.fn.ShowComment = function(json)
	{
		var $contianer = this;
		var render = function()
		{
			var strTemplate = "<table id=\"frm-tabs\" cellpadding=\"3\" cellspacing=\"1\" border=\"0\">";
			/***************************************************************************************
			*输出订单中的商品信息
			****************************************************************************************/
			try{
				if(json["productxml"]!=undefined && json["productxml"]!=null 
				&& json["productxml"]!="" && json["productxml"].indexOf("items")!=-1)
				{
					var defaultId = "0";
					var rootxml = $(json["productxml"])[0];
					$(rootxml).find("items").each(function(i,node){
						var productid = parseFloat($(node).find("productid").html()) || 0;
						if(defaultId!=productid)
						{
							defaultId = productid;
							var amount = parseFloat($(node).find("amount").html()) || 0;
							var number = parseFloat($(node).find("number").html()) || 0;
							var property = $(node).find("property").html() || "";
							strTemplate += "<tr class=\"product\">";
							strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;\"></td>";
							strTemplate += "<td style=\"width:60px;padding:0px;\"><img style=\"max-width:60px;height:70px;border:#ccc solid 1px;padding:2px;background:#fff\" src=\""+$(node).find("thumb").html()+"\" /></td>";
							strTemplate += "<td class=\"productinfo\">";
							strTemplate += "<div class=\"title\">"+$(node).find("strtitle").html()+"</div>";
							strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:5px;\"></div>";
							strTemplate += "<div class=\"property\">";
							//strTemplate += "<div class=\"text\">"+$(node).find("property").html()+"</div>"
							strTemplate += "<div class=\"amount\"><font>￥</font>"+amount+"</div>";
							strTemplate += "<div class=\"number\">×"+number+"</div>"
							strTemplate += "</div>"
							
							strTemplate += "</div>"
							strTemplate += "</td>";
							strTemplate += "</tr>";
							/********************************************************************
							*输出评价标签
							*********************************************************************/
							strTemplate += "<tr class=\"hback\">";
							strTemplate += "<td style=\"padding:0px;font-size:0px;width:10px;\"></td>";
							strTemplate += "<td>商品评价</td>";
							strTemplate += "<td class=\"starLevel\">";
							try{
								for(var i =1;i<=5;i++)
								{
									strTemplate += "<label>";
									strTemplate += "<input operate=\"star\" selector=\"false\"";
									strTemplate += " type=\"radio\" value=\""+i+"\"";
									strTemplate += " name=\"star"+productid+"\" />";
									strTemplate += "</label>";
								}
							}catch(err){}
							strTemplate += "</td>";
							strTemplate += "</tr>";
							
							/********************************************************************
							*推荐标签
							*********************************************************************/
							strTemplate += "<tr class=\"hback\">";
							strTemplate += "<td class=\"spacing\"></td>";
							strTemplate += "<td valign=\"top\">推荐标签</td>";
							strTemplate += "<td class=\"tagValue\">";
							try
							{
								var tagArr = ["性价比高","发货快","质量好","值得买","包装精美"];
								var tagName = ("tag"+(productid || "")).toString();
								for(var i in tagArr)
								{
									var strValue = tagArr[i];
									if(strValue!=undefined && strValue!=null && strValue!="")
									{
										strTemplate += "<label>";
										strTemplate += "<input name=\""+tagName+"\" type=\"checkbox\"";
										strTemplate += " operate=\"tag\" value=\""+strValue+"\"";
										strTemplate +=" />";
										strTemplate +=""+strValue+"";
										strTemplate += "</label>";
									}
									
								}
							}catch(err){}
							strTemplate += "</td>";
							strTemplate += "</tr>";
							
							/********************************************************************
							*推荐标签
							*********************************************************************/
							strTemplate += "<tr class=\"hback\">";
							strTemplate += "<td class=\"spacing\"></td>";
							strTemplate += "<td style=\"padding:2px 0px !important;\" colspan=\"2\">";
							strTemplate += "<textarea name=\"text"+productid+"\" style=\"width:90%;border:#eee solid 1px;height:80px;font-size:14px;;padding:8px 8px;border-radius:0px;\" placeholder=\"这件商品怎么样?点评一下吧！\"></textarea>";
							strTemplate += "</td>";
							strTemplate += "</tr>";
							
							/********************************************************************
							*上传截图
							*********************************************************************/
							strTemplate += "<tr class=\"hback\">";
							strTemplate += "<td class=\"spacing\"></td>";
							strTemplate += "<td name=\"thumb"+productid+"\" style=\"position:relative;padding:0px !important;height:70px;\" colspan=\"2\">";
							strTemplate += "<div class=\"maps\">";
							strTemplate += "<input multiple operate=\"file\" type=\"file\" value=\"\" name=\"file"+productid+"\">";
							strTemplate += "</div>";
							strTemplate += "<div class=\"fileControls\" operate=\"fileControls\">";
							
							strTemplate += "</div>";
							strTemplate += "</td>";
							strTemplate += "</tr>";
							
							strTemplate += "<tr><td style=\"width:100%;font-size:0px;height:10px;\" colspan=\"3\"></td></tr>";
							
						}
						
					});
				};
			}catch(err){};
			strTemplate += "<tr><td colspan=\"3\"></td></tr>";
			strTemplate += "</table>";
			/****************************************************************************************
			*为控件赋值
			*****************************************************************************************/
			if(strTemplate!=undefined && strTemplate!=null 
			&& $(strTemplate)[0]!=undefined && $(strTemplate)[0]!=null)
			{
				$($contianer).html(strTemplate);
				/****************************************************************************************
				*增加点击事件信息
				*****************************************************************************************/
				$($contianer).find("input[operate=\"tag\"]").click(function(){
					try{
						try{
							var tagName = $(this).attr("name") || "";
							if(tagName==undefined || tagName=="")
							{alert('获取标签错误,请重试！');return false;}
							var length = 0;
							$($contianer).find("input[name=\""+tagName+"\"]").each(function(){
								if(this!=undefined && this!=null && this.checked){
									length=length+1;	
								}													   
							});
							if(length>=6){alert('最多只能选择五个标签');return false;}
						}catch(err){}
						try{
							if(this!=undefined && this!=null && this.checked)
							{
								$(this).attr("selector","true");
								$(this.parentNode).attr("selector","true");
							}else if(this!=undefined && this!=null)
							{
								$(this).attr("selector","false");
								$(this.parentNode).attr("selector","false");	
							}
						}catch(err){}
					}catch(err){}
				});
				/****************************************************************************************
				*设置评价等级
				*****************************************************************************************/
				$($contianer).find("input[operate=\"star\"]").click(function(){
					try{
						var Length = parseInt($(this).val()) || 1;
						var objTD = $(this.parentNode.parentNode);
						$(objTD).find("input[operate=\"star\"]").each(function(){
							try{
								var strValue = parseInt(this.value) || 0;
								if(strValue<=Length){$(this).attr("selector","true");}
								else{$(this).attr("selector","false");}
							}catch(err){}
						});
					}catch(err){}
				});
				/****************************************************************************************
				*图片选择器
				*****************************************************************************************/
				$($contianer).find("input[operate=\"file\"]").change(function(event){
					try{
						if(window.FileReader && event!=undefined && event!=null 
						&& event.target.files!=undefined && event.target.files!=null 
						&& event.target.files[0]!=undefined && event.target.files[0]!=null)
						{
							var pN = $(this.parentNode.parentNode);
							var fileName = $(pN).attr("name") || "";
							if(fileName==undefined || fileName=="")
							{alert('获取图片地址失败,请重试!');return false;}
							var fileControls = $(pN).find("div[operate=\"fileControls\"]")[0];
							if(fileControls!=undefined && fileControls!=null){$(fileControls).html("");}
							/****************************************************************
							*开始获取图片集合信息
							*****************************************************************/
							var fileList = event.target.files;  
							if(fileList.length>=4){alert('最多只能选择3张图片');return false;}
							else{
								for (var i = 0, itemFile; itemFile = fileList[i]; i++) 
								{
									try{
										if (itemFile!=undefined && itemFile!=null 
										&& itemFile.type.match('image.*')) 
										{
											try{
												var reader = new FileReader();
												reader.onload = (function(theFile) {  
													return function(e) 
													{
														StartUpload(theFile,{"back":function(url){
															if(fileControls!=undefined && fileControls!=null)
															{
																
																$(fileControls).append("<img style=\"max-width:60px;height:56px;border:#ccc solid 1px;padding:2px;background:#fff\" src=\""+url+"\"/><input type=\"hidden\" name=\""+fileName+"\" value=\""+url+"\" />");	
															};									 
														}});
														try{
															
														
														}catch(err){}
													}; 
												})(itemFile);
												reader.readAsDataURL(itemFile);  
											}catch(err){alert(err.message);}
										}
									}catch(err){alert(err.message);}
								}
							};
						};
					}catch(err){alert(err.message);}
				});
			}
			
			
		};
		/****************************************************************************************
		*构架退款表单中的xml集合信息
		*****************************************************************************************/
		var constructXML = function()
		{
				
		}
		/****************************************************************************************
		*加载用户订单详情信息
		*****************************************************************************************/
		if(json!=undefined && json!=null && typeof(json)=='object' 
		&& json["businessid"]!=undefined && json['businessid']!=null 
		&& json['businessid']!="" && json['orderid']!=undefined 
		&& json['orderid']!=null && json['userid']!=undefined)
		{
			try{render();}catch(err){}
		}
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