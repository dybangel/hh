/****************************************************************************************
*切换收货地址
*****************************************************************************************/
var FindDeliveryCallBack = function(arr)
{
	try{
		if(arr!=undefined && arr!=null && typeof(arr)=='object')
		{
			var Fullname = arr["fullname"] || "";
			var strMobile = arr["strmobile"] || "";
			var strCity = arr["strcity"] || "";
			var Address = arr["address"] || "";	
			strCity = (strCity+Address).replace("-","");
			if(Fullname!=undefined && Fullname!="" 
			&& strMobile!=undefined && strMobile!="" 
			&& strCity!=undefined && strCity!="" 
			&& Address!=undefined && Address!="")
			{
				$("#frm-fullname").html(Fullname);
				$("#frm-mobile").html(strMobile);
				$("#frm-city").html(strCity);
				$("#frmFullname").val(Fullname);
				$("#frmMobile").val(strMobile);
				$("#frmCity").val(strCity);
				/****************************************************************************************
				*计算订单金额信息
				*****************************************************************************************/
				try{
					var strProvice = strCity;
					if(strProvice!=undefined && strProvice!="" 
					&& strProvice.length>=2)
					{strProvice = strProvice.substr(0,2);}
					
					if(strProvice!=undefined && strProvice!="")
					{OrderCalculation(strProvice);}
				}catch(err){}
			}
		}
	}catch(err){}
	try{$("#frm-deliveryMasker").contianer('close');}
	catch(err){};
	
};
/****************************************************************************************
*计算运费信息
*****************************************************************************************/
var FareCalculation = function(province,businessId)
{
	var fareAmount = 0;
	try{
		var province = province || "";
		if(province.length>=3){province=province.substr(0,2);}
	}catch(err){}
	/****************************************************************************************
	*开始计算
	*****************************************************************************************/
	if(optionList!=undefined && optionList!=null 
	&& typeof(optionList)=='object' && optionList.length>=1 
	&& optionList[0]['businessid']!=undefined && optionList[0]['businessid']!=""
	&& optionList[0]['farexml']!=undefined && optionList[0]['farexml']!=null 
	&& province!=undefined && province!="")
	{
		
		try{
			$(optionList).each(function(k,json){
				if(json!=undefined && json!=null && typeof(json)=='object' 
				&& json["businessid"]!=undefined && json["businessid"]!="" 
				&& !isNaN(json["businessid"]) && businessId!=undefined 
				&& businessId!=null && businessId!="" && businessId==json["businessid"])
				{
					var number = parseInt(json["number"]) || 0;
					var fareXml = $(json["farexml"])[0] || null;
					var defaultquantity = parseInt(json['defaultquantity']) || 0;
					var defaultamount = parseInt(json['defaultamount']) || 0;
					var superquantity = parseInt(json['superquantity']) || 0;
					var superamount = parseInt(json['superamount']) || 0;
					try{
						$(fareXml).find("items").each(function(i,$obj){
							try
							{
								var strCity = "";
								try{strCity = $($obj).find("city").text();}
								catch(err){}
								if(strCity!=undefined && strCity!=null && strCity!="" 
								&& province!=undefined && province!=null && province!="" 
								&& strCity.indexOf(province)!=-1)
								{
									defaultquantity = parseInt($($obj).find("quantity").text()) || 0;
									defaultamount = parseFloat($($obj).find("amount").text()) || 0;
									superquantity = parseInt($($obj).find("superquantity").text()) || 0;
									superamount = parseFloat($($obj).find("superamount").text()) || 0;	
								};
							}catch(err){}
						});
					}catch(err){}
					var amount = 0;
					if(superquantity<=0){superquantity=1;}
					if(number<=defaultquantity){amount=defaultamount;}
					else{amount=parseFloat(defaultamount+(((number-defaultquantity)*superamount/superquantity)));}
					fareAmount=parseFloat(fareAmount+amount);
				}
			});
		}catch(err){}
	};
	return fareAmount;		
}

/****************************************************************************************
*统计订单费用信息
*****************************************************************************************/
var OrderCalculation = function(strprovice)
{
	if(optionList!=undefined && optionList!=null 
	&& typeof(optionList)=='object' && optionList.length>=1 
	&& optionList[0]['businessid']!=undefined && optionList[0]['businessid']!=""
	&& optionList[0]['productid']!=undefined && optionList[0]['productid']!="")
	{
		var businessId = 0;var iError = false;var figureAmount = 0;
		$(optionList).each(function(k,json){
			try{
				if(businessId!=json["businessid"])
				{
					
					var fareAmount = FareCalculation(strprovice,json["businessid"]);
					var fareTemplate = "";  
					fareTemplate +="<div style=\"color:#FF6600\">"
					fareTemplate +="<font>￥</font>";
					fareTemplate +="<span style=\"color:#ff6600;font-size:16px;\">"+fareAmount+"</span>";
					fareTemplate +="元";
					fareTemplate +="</div>"
					$("#frm-fareTemplate"+json["businessid"]+"").html(fareTemplate);
					figureAmount = parseFloat(figureAmount + fareAmount);
					
				};
				businessId=json["businessid"];
			}catch(err){alert(err.message);}
			try
			{
				var number = parseInt(json["number"]) || 0;
				var amount = GetAmount(json) || 0;
				if(amount==undefined || amount<=0){iError=true;return true;}
				figureAmount = parseFloat(figureAmount + parseFloat(number*amount));
			}catch(err){};
		});
		/***********************************************************************
		*判断订单金额是否有计算错误
		************************************************************************/
		if(iError==true){
				alert('订单计算失败,请重试！');return false;	
		}
		/***********************************************************************
		*开始重新赋值订单信息
		************************************************************************/
		try{
			if(figureAmount!=undefined && figureAmount!=null 
			&& !isNaN(figureAmount) && figureAmount!=0){
				$("#frmCalculation").html("合计(<font>"+figureAmount.toFixed(2)+"</font>)元");
				$("#frmAmount").val(figureAmount.toFixed(2));	
			}else{
				alert('订单计算失败,请重试！')
			}
		}catch(err){}
	}	
};
/****************************************************************************************
*获取指定商品的金额信息
*****************************************************************************************/
var GetAmount = function(json)
{
	var amount = 0;
	try{
		if(json["icustomer"]=="1"){amount = parseFloat(json["wholeamount"]) || 0;}
		else{amount = parseFloat(json["retailamount"]) || 0;};
		if(json['isdiscount']!=undefined && json['isdiscount']=="1")
		{
			var discount = parseFloat(json['discount']) || 0;
			if(discount<=0){discount=100;}
			amount = parseFloat(((amount * discount)/100).toFixed(2));
		}
	}catch(err){}
	return amount;
}
/****************************************************************************************
*扩展jQuery 方法展示
*****************************************************************************************/
;(function($) {
"use strict";
	$.fn.ShowConfirm = function(options)
	{
		var $contianer = this;
		/****************************************************************************************
		*显示每个店铺下面的采购的商品信息
		*****************************************************************************************/
		var ShowProduct = function(BusinessID)
		{
			
			var strProduct = "";
			$(options).each(function(k,json){
				if(BusinessID==json["businessid"])
				{
					/*******************************************************************************
					*计算商品金额信息
					********************************************************************************/
					var amount = GetAmount(json) || 0;
					/*******************************************************************************
					*开始输出商品内容
					********************************************************************************/
					try{
						strProduct += "<tr productid=\""+json['productid']+"\" trolleyid=\""+json['trolleyid']+"\" class=\"product\">";
						strProduct += "<td class=\"spacing\"></td>";
						strProduct += "<td onclick=\"window.location='show.aspx?action=show&showid="+json['productid']+"'\" style=\"width:60px;\"><img style=\"max-width:60px;height:70px;border:#ccc solid 1px;padding:2px;background:#fff\" src=\""+json['thumb']+"\" /></td>";
						strProduct += "<td class=\"productinfo\">";
						strProduct += "<div class=\"title\">"+json['strtitle']+"</div>";
						strProduct += "<div style=\"clear:both;width:100%;font-size:0px;height:5px;\"></div>";
						strProduct += "<div class=\"property\">";
						strProduct += "<div class=\"text\">"+json['strproperty']+"</div>"
						strProduct += "<div class=\"amount\">";
						strProduct += "<font style=\"font-weight:100\">￥</font>";
						strProduct += "<span style=\"margin:0px 4px;\">"+amount+"</span>";
						if(json['isdiscount']=='1'){strProduct += "<font style=\"color:#cd0000;font-weight:100\">[特]</font>";}
						strProduct += "</div>";
						strProduct += "<div class=\"number\">×"+json["number"]+"</div>";
						strProduct += "</div>"
						strProduct += "</td>";
						strProduct +="</tr>";
						strProduct +="<tr><td colspan=\"3\" style=\"font-size:0px;height:1px;background:#fff\"></td></tr>";
					}catch(err){}
				};
			});
			return strProduct;
		};
		/*******************************************************************************
		* 计算商品运费信息
		********************************************************************************/
		var fareTemplate = function()
		{
			var fareAmount = 0;
			try
			{
				var province = province || "";
				if(province.length>=3){province=province.substr(0,2);}
			}catch(err){}
			for(var k in options)
			{
				if(businessid==options[k]["businessid"])
				{
					var json = options[k];
					var number = parseInt(json["number"]) || 0;
					var fareXml = $(json["farexml"])[0] || null;
					var defaultquantity = parseInt(json['defaultquantity']) || 0;
					var defaultamount = parseInt(json['defaultamount']) || 0;
					var superquantity = parseInt(json['superquantity']) || 0;
					var superamount = parseInt(json['superamount']) || 0;
					$(fareXml).find("items").each(function(){
						try
						{
							
							var strCity = "";
							try{strCity = $(this).find("city").text();}
							catch(err){}
							if(strCity!=undefined && strCity!=null && strCity!="" 
							&& province!=undefined && province!=null && province!="" 
							&& strCity.indexOf(province)!=-1)
							{
								defaultquantity = parseInt($(this).find("quantity").text()) || 0;
								defaultamount = parseFloat($(this).find("amount").text()) || 0;
								superquantity = parseInt($(this).find("superquantity").text()) || 0;
								superamount = parseFloat($(this).find("superamount").text()) || 0;	
							};
						}catch(err){}
					});
					var amount = 0;
					if(number<=defaultquantity){amount=defaultamount;}
					else{amount=defaultamount+(((number-defaultquantity)*superamount/superquantity));}
					fareAmount=fareAmount+amount;
				}	
			};
			return fareAmount;	
		};
		
		/*******************************************************************************
		* 合计商品金额与数量信息
		********************************************************************************/
		var FigureCalculation = function(SiteID,back)
		{
			var FigureAmount = 0;var FigureNumber = 0;
			try
			{
				$(options).each(function(k,json){
					if(SiteID==json["businessid"])
					{
						try
						{
							var number = parseInt(json["number"]) || 0;
							
							var amount = GetAmount(json) || 0;
							
							FigureNumber=FigureNumber + number;
							FigureAmount = FigureAmount + (amount*number);
						}catch(err){}
					};
				});
			}catch(err){};
			/*******************************************************************************
			* 执行返回回调函数
			********************************************************************************/
			try{
				if(back!=undefined && back!=null && typeof(back)=='function')
				{
					back({"number":FigureNumber,
						 "amount":FigureAmount
					});
				};
			}catch(err){}
		};
		/******************************************************************************************
		*渲染商品列表信息
		*******************************************************************************************/
		var render = function()
		{
			var strTemplate = "";
			strTemplate +="<table id=\"frm-tabs\" border=\"0\" cellpadding=\"3\" cellspacing=\"1\">";
			strTemplate +="<tr><td colspan=\"3\"></td></tr>";
			if(options!=undefined && options!=null && typeof(options)=='object')
			{
				var BusinessID = "0";
				$(options).each(function(k,json){
					if(BusinessID!=json["businessid"])
					{
						FigureCalculation(json["businessid"],function(arr){
							
							strTemplate +="<tr class=\"business\">";
							strTemplate +="<td style=\"border-bottom:#eeeeee solid 1px !important;\" class=\"spacing\"></td>";
							strTemplate +="<td onclick=\"window.location='site.aspx?action=show&siteid="+json['businessid']+"'\" style=\"position:relative;height:30px;\" colspan=\"2\">";
							strTemplate +="<img style=\"width:20px;height:20px;border:#eee solid 1px;padding:2px;background:#fff;position:absolute;left:0px;top:10px;\" src=\""+json['strthumb']+"\" />";
							strTemplate +="<span style=\"position:absolute;left:30px;top:14px;\">"+json["businessname"]+"</span>";
							strTemplate +="</td>";
							strTemplate +="</tr>";
							/*************************************************************
							*显示商品信息
							**************************************************************/
							try{strTemplate +=(ShowProduct(json["businessid"]));}catch(err){}
							/*************************************************************
							*显示留言备注
							**************************************************************/
							strTemplate +="<tr operate=\"fareTemplate\" class=\"hback\">";
							strTemplate +="<td class=\"spacing\"></td>";
							strTemplate +="<td class=\"name\">运费</td>";
							strTemplate +="<td id=\"frm-fareTemplate"+json["businessid"]+"\" class=\"value\">0元</td>";
							strTemplate +="</tr>";
							/*************************************************************
							*给卖家留言
							**************************************************************/
							strTemplate +="<tr class=\"hback\">";
							strTemplate +="<td class=\"spacing\"></td>";
							strTemplate +="<td class=\"name\">买家留言</td>";
							strTemplate +="<td class=\"value\"><input type=\"text\" name=\"strMessage"+json["businessid"]+"\" value=\"\" placeholder=\"对本次交易说明\" /></td>";
							strTemplate +="</tr>";
							/*************************************************************
							*合计商品信息
							**************************************************************/
							strTemplate +="<tr operate=\"calculation\" class=\"hback\">";
							strTemplate +="<td style=\"border-bottom:#eeeeee solid 1px !important;\" class=\"spacing\"></td>";
							strTemplate +="<td colspan=\"2\" class=\"value\">";
							strTemplate +="共"+arr["number"]+"件商品,小计:<font style=\"color:#ff6600;font-size:12px;\">¥</font><font style=\"color:#ff6600;font-size:16px;\">"+arr["amount"]+"</font>元";
							strTemplate +="</td>";
							strTemplate +="</tr>";
							/*************************************************************
							*标示分隔符
							**************************************************************/
							strTemplate +="<tr><td colspan=\"3\" style=\"background:#f4f4f4;height:8px;font-size:0px;\"></td></tr>";
							
							
						});
						
					};
					BusinessID=json["businessid"];	
				});
			};
			strTemplate +="</table>";
			$($contianer).html(strTemplate);
			/******************************************************************************************
			*统计订单金额,订单费用信息
			*******************************************************************************************/
			try
			{
				if(document.getElementById('frmCity'))
				{
					var strCity = document.getElementById('frmCity').value || "";
					if(strCity!=undefined && strCity!="" && strCity.length>=2){
						strCity = strCity.substr(0,2);	
					}
					OrderCalculation(strCity);
				}
			}catch(err){}
			/******************************************************************************************
			*显示商品数据信息
			*******************************************************************************************/
			$("#frm-deliveryTabs").click(function(){
				$("#frm-deliveryMasker").contianer({
					"url":"delivery.aspx?action=delivery",
					"reload":"false",
					"iszoom":"false"
				});					  
			});
		};
		/******************************************************************************************
		*开始处理数据信息
		*******************************************************************************************/
		try{
			if(options!=undefined && options!=null && typeof(options)=='object'
			&& $contianer!=undefined && $contianer!=null)
			{
				render();
			}
		}catch(err){}
	};
})(jQuery);

;$(function(){
		   
});