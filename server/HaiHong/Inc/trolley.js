/*******************************************************************************************
*更新购物车数量
********************************************************************************************/
var SaveUpdate = function(TrolleyID,number)
{
	if(window.Send!=undefined && window.Send!=null && typeof(window.Send)=='function' 
	&& TrolleyID!=undefined && TrolleyID!=null && !isNaN(TrolleyID) 
	&& number!=undefined && number!=null && !isNaN(number))
	{
		Send({
			 "url":"trolley.aspx?action=edit&trolleyid="+TrolleyID+"&number="+number+"",
			 "back":function(){
				
			 }
		});
	}
};
/*******************************************************************************************
*删除购物车中的商品信息
********************************************************************************************/
var DeleteTrolley = function(TrolleyId)
{
	if(window.Send!=undefined && window.Send!=null && typeof(window.Send)=='function' 
	&& TrolleyId!=undefined && TrolleyId!=null && TrolleyId!="")
	{
		Send({
			 "url":"trolley.aspx?action=del&TrolleyId="+TrolleyId+"",
			 "back":function(){
				window.location.reload();
			 }
		});
	}	
}

/****************************************************************************************
*扩展jQuery 方法展示
*****************************************************************************************/
;(function($) {
"use strict";
	$.fn.ShowTrolley = function(options)
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
					/*******************************************************************************
					*开始输出商品内容
					********************************************************************************/
					try{
						strProduct +="<tr productid=\""+json['productid']+"\" trolleyid=\""+json['trolleyid']+"\" class=\"product\">";
						strProduct +="<td class=\"selector\" style=\"height:30px;width:24px;padding:8px 8px !important;\" operate=\"selector\">";
						strProduct +="<input businessid=\""+json['businessid']+"\" operate=\"items\" type=\"checkbox\" amount=\""+amount+"\" name=\"strList\" value=\""+json['trolleyid']+"\" />";
						strProduct +="</td>";
						strProduct += "<td onclick=\"window.location='show.aspx?action=show&showid="+json['productid']+"'\" style=\"width:70px;padding:0px;\"><img style=\"max-width:60px;height:70px;border:#ccc solid 1px;padding:2px;background:#fff\" src=\""+json['thumb']+"\" /></td>";
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
						strProduct += "<div class=\"frmValue\">";
						strProduct += "<span class=\"reduce\">-</span>";
						strProduct += "<input type=\"number\" max=\"100\" step=\"1\" min=\"0\" value=\""+json['number']+"\" name=\"\" />";
						strProduct += "<span class=\"add\">+</span>";
						strProduct += "</div>";
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
							var amount = 0;
							if(json["icustomer"]=="1"){amount = parseFloat(json["wholeamount"]) || 0;}
							else{amount = parseFloat(json["retailamount"]) || 0;}
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
							strTemplate +="<td class=\"selector\" style=\"height:30px;width:24px;padding:8px 8px !important;\" operate=\"selector\">";
							strTemplate +="<input businessid=\""+json["businessid"]+"\" operate=\"business\" type=\"checkbox\" name=\"businessid\" bsn=\""+json["businessid"]+"\" value=\"0\" />";
							strTemplate +="</td>";
							strTemplate +="<td onclick=\"window.location='site.aspx?action=show&siteid="+json['businessid']+"'\" style=\"position:relative;height:30px;\" colspan=\"2\">";
							strTemplate +="<img style=\"width:20px;height:20px;border:#eee solid 1px;padding:2px;background:#fff;position:absolute;left:0px;top:10px;\" src=\""+json['strthumb']+"\" />";
							strTemplate +="<span style=\"position:absolute;left:30px;top:14px;\">"+json["businessname"]+"</span>";
							strTemplate +="</td>";
							strTemplate +="</tr>";
							/*************************************************************
							*显示商品信息
							**************************************************************/
							try{
								strTemplate +=(ShowProduct(json["businessid"]));
							}catch(err){}
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
			/******************************************************************************************
			*展示购物车商品信息
			*******************************************************************************************/
			if($contianer!=undefined && $contianer!=null 
			&& strTemplate!=undefined && strTemplate!="" 
			&& $(strTemplate)[0]!=undefined && $(strTemplate)[0]!=null)
			{
				$($contianer).html(strTemplate);
				/******************************************************************************************
				*购物车商品减数量
				*******************************************************************************************/
				$($contianer).find("span[class=\"reduce\"]").click(function(){
					try{
						var trolleyId = parseInt($(this).parents("tr.product").attr("trolleyid")) || 0;
						if(trolleyId!=undefined && trolleyId!=null && trolleyId!=0)
						{
							var frmInput = $(this.parentNode).find("input[type=\"number\"]")[0];
							if(frmInput!=undefined && frmInput!=null)
							{
								var strValue = parseInt($(frmInput).val()) || 0;
								strValue = strValue-1;
								if(strValue<=0){
									if(confirm('你确定要将商品移除到购物车?')){
										try{DeleteTrolley(trolleyId);}catch(err){}
									}	
								}else{
									$(frmInput).val(strValue);
									try{Calculation();}catch(err){}
								}
							}else{alert('获取购物车信息失败');return false;}
						}else{alert('获取购物车信息失败');return false;}
					}catch(err){}
				});
				/******************************************************************************************
				*购物车商品加数量
				*******************************************************************************************/
				$($contianer).find("span[class=\"add\"]").click(function(){
					try{
						var trolleyId = parseInt($(this).parents("tr.product").attr("trolleyid")) || 0;
						if(trolleyId!=undefined && trolleyId!=null && trolleyId!=0)
						{
							var frmInput = $(this.parentNode).find("input[type=\"number\"]")[0];
							if(frmInput!=undefined && frmInput!=null)
							{
								var strValue = parseInt($(frmInput).val()) || 0;
								var maxValue = parseInt($(frmInput).attr("max")) || 0;
								strValue = strValue+1;
								if(strValue>=maxValue){strValue=maxValue;}
								try{SaveUpdate(trolleyId,strValue);}
								catch(err){}
								try{$(frmInput).val(strValue);}catch(err){}
								try{Calculation();}catch(err){}
							}else{alert('获取购物车信息失败');return false;}
						}else{alert('获取购物车信息失败');return false;}
					}catch(err){}
				});
				/******************************************************************************************
				*购物车商品更改
				*******************************************************************************************/
				$($contianer).find("input[type=\"number\"]").change(function(){
					try{
						var trolleyId = parseInt($(this).parents("tr.product").attr("trolleyid")) || 0;
						if(trolleyId!=undefined && trolleyId!=null && trolleyId!=0)
						{
							var strValue = parseInt($(this).val()) || 0;
							var maxValue = parseInt($(this).attr("max")) || 0;
							if(strValue<=0){
								if(confirm('你确定要将商品移除到购物车?')){
									try{DeleteTrolley(trolleyId);}catch(err){}
								}	
							}else{
								if(strValue>=maxValue){strValue=maxValue;}
								try{SaveUpdate(trolleyId,strValue);}
								catch(err){}
								try{$(frmInput).val(strValue);}catch(err){}
								try{Calculation();}catch(err){}
							}
						}else{alert('获取购物车信息失败');}
					}catch(err){}
				});
				/******************************************************************************************
				*选中商品信息
				*******************************************************************************************/
				$($contianer).find("input[operate=\"items\"]").click(function(){
					try{
						var businessId = parseInt($(this).attr("businessid")) || 0;
						var checked = this.checked || false;
						if(businessId!=undefined && businessId!=null){
							var isAll = false;
							try{
								$($contianer).find("input[operate=\"items\"]").each(function(){
									var items = parseInt($(this).attr("businessid")) || 0;
									if(items!=undefined && items==businessId 
									&& this.checked!=checked)
									{isAll = true;}
								});
							}catch(err){alert('发生选择性错误,请重试！');return false;};
							/******************************************************************
							*验证店铺是否处于被选中状态
							*******************************************************************/
							try{
								var bck = $($contianer).find("input[bsn=\""+businessId+"\"]")[0];
								if(isAll!=true && bck && checked==true)
								{
									bck.checked = true;
								}
								else if(isAll!=true && bck && checked!=true)
								{
									bck.checked = false;	
								}
							}catch(err){}
						};
					}catch(err){}
					/***********************************************************************
					*判断商品是否需要全选
					************************************************************************/
					try{iSelectAll();}catch(err){}
				});
				/******************************************************************************************
				*选中商铺
				*******************************************************************************************/
				$($contianer).find("input[operate=\"business\"]").click(function(){
					try{
						var businessId = parseInt($(this).attr("businessid")) || 0;
						if(businessId!=undefined && businessId!=null && businessId!=0)
						{
							var $thus = this;
							$($contianer).find("input[operate=\"items\"]").each(function(){
								var items = parseInt($(this).attr("businessid")) || 0;
								if(businessId!=undefined && businessId==items){
									this.checked = 	$thus.checked;
								}
							});	
						}
					}catch(err){alert('发生选择性错误,请重试！');return false;}
					/***********************************************************************
					*判断商品是否需要全选
					************************************************************************/
					try{iSelectAll();}catch(err){}
					
				});
				/**************************************************************************
				*全选所有商品
				***************************************************************************/
				$("#frm-selectionAll").click(function(){
					try
					{
						var $thus = this;
						$($contianer).find("input[type=\"checkbox\"]").each(function(){
							this.checked = $thus.checked;														
						});
					}catch(err){alert(err.message);}
					/***********************************************************************
					*统计订单金额信息
					************************************************************************/
					try{Calculation();}catch(err){}
				});
				/**************************************************************************
				*点击商品结算按钮
				***************************************************************************/
				$("#frm-SettlementButton").click(function(){
					try{SettlementTrolley();}catch(err){}									  
				});
				
			};
		};
		/*******************************************************************************************
		*判断商品是否需要被全选
		********************************************************************************************/
		var iSelectAll = function()
		{
			try{
				var iSelected = false;
				$($contianer).find("input[type=\"checkbox\"]").each(function(){
					if(!this.checked){iSelected=true;}
				});
				if(document.getElementById('frm-selectionAll')){
					document.getElementById('frm-selectionAll').checked=!iSelected;	
				}
			}catch(err){}
			/*************************************************************************************
			*统计商品数量信息
			**************************************************************************************/
			try{Calculation();}catch(err){}
		};
		/*******************************************************************************************
		*计算购物车中商品选中的数量信息
		********************************************************************************************/
		var Calculation = function()
		{
			try
			{
				var Total = 0;var sCount = 0;
				$($contianer).find("input[operate=\"items\"]").each(function(){
					if(this.checked!=undefined && this.checked!=null && this.checked)
					{
						var objectTR = $(this).parents("tr.product");
						try{
							var amount = parseFloat($(this).attr("amount")) || 0;
							var number = parseInt($(objectTR).find("input[type=\"number\"]").val()) || 0;
							if(amount!=undefined && amount!=0 && number!=undefined && number!=0)
							{
								Total = Total + parseFloat((amount*number));
								sCount = sCount+1;	
							}
						}catch(err){}	
					}											   
				});
				$("#frm-zongji").html('总计 <span>￥<font>'+Total.toFixed(2)+'</font></span>');
				$("#frm-SettlementButton").html("结算("+sCount+")");
			}catch(err){}
		};
		
		/*******************************************************************************************
		*开始结算商品订单信息
		********************************************************************************************/
		var SettlementTrolley = function()
		{
			try{
				var ListID = "";
				try
				{
					$("#frm-tabs").find("input[name=\"strList\"]").each(function(){
						if(this.checked!=undefined && this.checked!=null 
						&& this.checked && this.value!=undefined && this.value!="")
						{
							var sId = parseInt($(this).val()) || 0;
							if(sId!=undefined && sId!=0 && ListID!="")
							{ListID=ListID+","+sId;}
							else if(sId!=undefined && sId!=0){ListID=sId;}
						};
					});
				}catch(err){};
				if(ListID!=undefined && ListID!=null && ListID!="")
				{window.location='Trolley.aspx?action=confirm&strList='+ListID;}
				else{alert('请选择要结算的商品!');return false;}
			}catch(err){}
		}

		
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