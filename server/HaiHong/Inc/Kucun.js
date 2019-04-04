/*!
 * select product number plugin
 * version: 201704111551
 * @requires jQuery v1.5 or later
 * Copyright (c) 2013 M. Alsup
 * Examples and documentation at: http://malsup.com/jquery/form/
 * Project repository: https://github.com/malsup/form
 * Dual licensed under the MIT and GPL licenses.
 * https://github.com/malsup/form#copyright-and-license
 */
/*global ActiveXObject */
;(function($) {
"use strict";
	$.fn.enter = function(options)
	{
		var options = options || {};
		try{options["template"] = options["template"] || "frmTemplate";	}catch(err){}
		var frmTemp = $(document.getElementById(options["template"]))[0];
		if(frmTemp==undefined || frmTemp==null){return;}
		
	};
	/****************************************************************************************
	*选择商品数量信息
	*****************************************************************************************/
	$.fn.attrTabs = function(options)
	{
		var frmTemp = null;
		var $thisContianer = this;
		try{
			var options = options || {};
			try{options["template"] = options["template"] || "frmTemplate";	}catch(err){}
			frmTemp = $(document.getElementById(options["template"])).html();
		}catch(err){}
		if(frmTemp!=undefined && frmTemp!=null && frmTemp!=""
		&& options!=undefined && typeof(options)=='object')
		{
			var arrTemp = [];
			try{
				$($(frmTemp)[0]).find("items").attrList(function(arr){
					if(arr!=undefined && typeof(arr)=='object'
					&& arr["value"]!=undefined && arr["value"]!="")
					{
						var newTemp = arr["value"].split(",");
						arrTemp.push(newTemp);
					}
				});
			}catch(err){}
			var strTemplate ="";
			strTemplate += "<table cellpadding=\"3\" border=\"0\" id=\"frm-arrTabs\" cellspacing=\"1\">";
			strTemplate += "<tr>";
			strTemplate += "<th width=\"80\">属性名称</th>";
			strTemplate += "<th width=\"80\">当前库存</th>";
			strTemplate += "<th>选择数量</th>";
			strTemplate += "</tr>";
			var arrList = "";
			var sTemp = $.doExchange(arrTemp);
			if(sTemp!=null && sTemp.length>0){
				try{
					var groupIndex = 0;
					for(var k in sTemp){
						$.Listname(sTemp[k],function(prefixName,suffixName){
							if(prefixName!=arrList){
								groupIndex=groupIndex+1;
								if(groupIndex!=1 && groupIndex!=0)
								{
									strTemplate += "<tr isclosed=\"false\" text=\""+prefixName+"\" operate=\"color\">";
									strTemplate += "<td style=\"background:#ffe2e2;color:#cd0000;font-weight:900\" colspan=\"3\">"+prefixName+"</td>";
								strTemplate += "</tr>";	
								}else
								{
									strTemplate += "<tr isclosed=\"true\" text=\""+prefixName+"\" operate=\"color\">";	
									strTemplate += "<td style=\"background:#ffe2e2;color:#cd0000;font-weight:900\" colspan=\"3\">"+prefixName+"</td>";
								strTemplate += "</tr>";	
								}	
							}
							arrList = prefixName;
							var attrNumber = 0;
							try{
								if(options["Kucun"]!=undefined && options["Kucun"]!=null)
								{
									attrNumber = parseInt(options["Kucun"][""+prefixName+""+suffixName+""]) || 0;	
								}
							}catch(err){}
							if(typeof(sTemp[k])=='object' && sTemp.length!=0){
								if(groupIndex!=1){
									strTemplate += "<tr text=\""+prefixName+""+suffixName+"\" style=\"display:none\" value=\""+prefixName+"\" operate=\"selector\">";
								}else{
									strTemplate += "<tr text=\""+prefixName+""+suffixName+"\" value=\""+prefixName+"\" operate=\"selector\">";
								}
								strTemplate += "<td>"+suffixName+"</td>";
								strTemplate += "<td>"+attrNumber+"</td>";
								strTemplate += "<td><input min=\"0\" placeholder=\"数量\" type=\"number\" value=\"\" operate=\"number\" /></td>";
								strTemplate += "</tr>";	
							}else if(typeof(sTemp[k])=='string'){
								strTemplate += "<tr text=\""+prefixName+""+suffixName+"\" value=\""+prefixName+"\" operate=\"selector\">";
								strTemplate += "<td>"+suffixName+"</td>";
								strTemplate += "<td>"+attrNumber+"</td>";
								strTemplate += "<td><input min=\"0\" placeholder=\"数量\" type=\"number\" value=\"\" operate=\"number\" /></td>";
								strTemplate += "</tr>";	
							}
						});
					}
				}catch(err){}
			}
			strTemplate += "</table>";
			$($thisContianer).html(strTemplate);
			/*******************************************************************************
			*加载数据点击事件
			********************************************************************************/
			$($thisContianer).find("tr[operate=\"color\"]").click(function(){
				try{
					var isClosed = $(this).attr("isclosed") || "false";
					if(isClosed!=undefined && isClosed!="" && isClosed=="false")
					{
						var prefixName = $(this).attr("text") || "";
						if(prefixName!=undefined && prefixName!="")
						{
							$(this).attr("isclosed","true");
							$($thisContianer).find("tr[value=\""+prefixName+"\"]").show();	
						}
					}
					else if(isClosed!=undefined && isClosed!="" && isClosed=="true"){
						var prefixName = $(this).attr("text") || "";
						if(prefixName!=undefined && prefixName!="")
						{
							$(this).attr("isclosed","false");
							$($thisContianer).find("tr[value=\""+prefixName+"\"]").hide();	
						}
					}
				}catch(err){}
			});
			$($thisContianer).find("input[operate=\"number\"]").change(function(){
				try{
					var strValue = parseInt(this.value) || 0;
					if(strValue<=0){strValue=0;}
					if(isNaN(strValue)){strValue=0;}
					this.value = strValue;	
					if(strValue!=0){$(this).attr("selected","true");}
				}catch(err){}
			});
		}
	};
	/*******************************************************************************************
	*将一维数组格式化,截取数组concat(length-2)为前缀,(length-1)为后缀
	********************************************************************************************/
	$.Listname = function(arr,back)
	{
		var prefixName = "";var suffixName ="";
		try{
			if(arr!=undefined && arr!=null && typeof(arr)=='object')
			{
				for(var s=0;s<=arr.length-2;s++){
					prefixName	= prefixName+arr[s];
				}
				suffixName = arr[arr.length-1];
			}else if(arr!=undefined && typeof(arr)=='string'){
				suffixName = arr;	
			}
		}catch(err){}
		if(suffixName!=undefined && suffixName!="" 
		&& back!=undefined && typeof(back)=='function')
		{
			back(prefixName,suffixName);	
		}
	}
	/*******************************************************************************************
	*获取items节点下所有的商品数据信息
	********************************************************************************************/
	$.fn.attrList = function(back){
		try{;
			$(this).each(function(i,attrib){
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
	*获取Xml结果信息返回一个数组
	********************************************************************************************/
	$.fn.attrListArray = function(back){
		var arrTemp = [];
		try{
			$(this).each(function(){
				var options = {};
				try{
					$(this.attributes).each(function(i,attrib){
						try{
							if(attrib["name"]!=undefined && attrib["name"]!=""
							&& attrib["value"]!=undefined)
							{
								options[attrib["name"]] = attrib["value"];
							}
						}catch(err){}
					}); 
				}catch(err){}
				/********************************************************************************
				*将结果加入数组中
				*********************************************************************************/
				if(options!=undefined && options!=null && typeof(options)=='object'
				&& arrTemp!=undefined && arrTemp!=null)
				{
					arrTemp.push(options);
				}
			});
		}catch(err){}
		/****************************************************************************************
		* 返回数据处理结果信息
		*****************************************************************************************/
		try{
			if(arrTemp!=undefined && arrTemp!=null 
			&& back!=undefined && back!=null && typeof(back)=='function')
			{
				back(arrTemp);
			}
		}catch(err){}
	};
	/****************************************************************************************
	* 将一个多维数组子元素进行排列,迭代
	*****************************************************************************************/
	$.doExchange = function (arr){
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
				return $.doExchange(newArr);
			}catch(err){}
            
        }else{
            return arr[0];
        }
    };
	/***************************************************************************************************
	*完成数量设置,数据筛选功能
	****************************************************************************************************/
	$.fn.selection = function(back)
	{
		try{
			var $thisContianer = this;
			$($thisContianer).find("input[operate=\"number\"]").each(function(i){
				try{
					var num = parseInt($(this).val()) || (-1);
					var attr = $(this.parentNode.parentNode).attr("text");
					if(num!=undefined && num!=null && parseInt(num)>=0 
					&& attr!=undefined && attr!="" && back!=undefined 
					&& back!=null && typeof(back)=='function')
					{
						try{
							back(num,attr);		
						}catch(err){}
					}
				}catch(err){}
			});
		}catch(err){}
	}
})(jQuery);

var GetAmount = function(cfg)
{
	var Amount = 0;
	try{
		var cfg = cfg || {};
		if(cfg!=undefined && cfg!=null && typeof(cfg)=='object' 
		&& cfg["ShowMode"]!=undefined && cfg["ShowMode"]!="")
		{
			if(cfg["ShowMode"]=="PF" && cfg["WholeAmount"]!=undefined && cfg["WholeAmount"]!="")
			{
				Amount = parseFloat(cfg["WholeAmount"]);	
			}else if(cfg["ShowMode"]=="LS" && cfg["RetailAmount"]!=undefined && cfg["RetailAmount"]!="")
			{
				Amount = parseFloat(cfg["RetailAmount"]);		
			}
			else if(cfg["ShowMode"]=="CG" && cfg["PurchaseAmount"]!=undefined && cfg["PurchaseAmount"]!="")
			{
				Amount = parseFloat(cfg["PurchaseAmount"]);		
			}else if(cfg["ShowMode"]=="PT" && cfg["WholeAmount"]!=undefined && cfg["WholeAmount"]!="")
			{
				Amount = parseFloat(cfg["WholeAmount"]);	
			}else if(cfg["ShowMode"]=="LT" && cfg["RetailAmount"]!=undefined && cfg["RetailAmount"]!="")
			{
				Amount = parseFloat(cfg["RetailAmount"]);		
			}
			else if(cfg["ShowMode"]=="CT" && cfg["PurchaseAmount"]!=undefined && cfg["PurchaseAmount"]!="")
			{
				Amount = parseFloat(cfg["PurchaseAmount"]);		
			}else if(cfg["ShowMode"]=="DH" && cfg["PurchaseAmount"]!=undefined && cfg["PurchaseAmount"]!="")
			{
				Amount = parseFloat(cfg["PurchaseAmount"]);		
			}else{
				Amount = parseFloat(cfg["PurchaseAmount"]);			
			}
		}
	}catch(err){}
	return Amount;
};