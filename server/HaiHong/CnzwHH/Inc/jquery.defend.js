$(function(){
	$("#frm-table").change(function(){
		if(this.value!=undefined && this.value!=""){
			try{
				$.ajax({url:"?action=cln&Tablename="+this.value+"",type:"get",success:function(strResponse){
					$("#frm-columns").html(strResponse);
					$("#frm-columns").find("a").click(function(){
						if($(this).text()!=undefined && $(this).text()!=""){
							$("#frm-defendtext").insert($(this).text());	
						}									   
					});
				}});	
			}catch(err){}
		}								
	});	
	/**************************************************************************************************
	*插入数据表名称
	***************************************************************************************************/
	$("#frm-add").click(function(){
		if($("#frm-table")[0]!=undefined && $("#frm-table")[0]!=null && $("#frm-table").val()!=""){
			try{
				$("#frm-defendtext").insert($("#frm-table").val());
			}catch(err){}	
		}							 
	});
	/**************************************************************************************************
	*查询
	***************************************************************************************************/
	$("#frm-select").click(function(){
		try{
			FindKeywords(function(clnText,Tablename){
				var strSql = "select "+clnText+" from "+Tablename+";";
				$("#frm-defendtext").insert(strSql);	
			});
		}catch(err){}	
	});
	/**************************************************************************************************
	*查询
	***************************************************************************************************/
	$("#frm-insert").click(function(){
		try{
			FindKeywords(function(clnText,Tablename){
				if(clnText!=undefined && clnText!="" && Tablename!=undefined && Tablename!=""){
					var arrTemp = clnText.split(",");
					var strSql = "insert into "+Tablename+"\n(\n";
					for(var k in arrTemp){
						if(k!=arrTemp.length-1){
							strSql += arrTemp[k]+",\n";	
						}else{
							strSql += arrTemp[k]+"\n";
						}
					}										
					strSql+="\n)\n";
					strSql+=" values\n(\n"
					for(var k in arrTemp){
						if(k!=arrTemp.length-1){
							strSql += "{@"+arrTemp[k]+"},\n";	
						}else{
							strSql += "{@"+arrTemp[k]+"}\n";
						}
					}	
					strSql+=");"
					$("#frm-defendtext").insert(strSql);	
				}
			});
		}catch(err){}	
	});
	
	/**************************************************************************************************
	*查询
	***************************************************************************************************/
	$("#frm-delete").click(function(){
		try{
			FindKeywords(function(clnText,Tablename){
				var strSql = "delete from "+Tablename+" where 1=1 ;";
				$("#frm-defendtext").insert(strSql);	
			});
		}catch(err){}	
	});
	
	/**************************************************************************************************
	*查询
	***************************************************************************************************/
	$("#frm-update").click(function(){
		try{
			FindKeywords(function(clnText,Tablename){
				if(clnText!=undefined && clnText!="" && Tablename!=undefined && Tablename!=""){
					var strSql = "update "+Tablename+" set\n";
					var arrTemp = clnText.split(",");
					for(var k in arrTemp){
						if(k!=arrTemp.length-1){
							strSql += arrTemp[k]+"={@"+arrTemp[k]+"},\n";	
						}else{
							strSql += arrTemp[k]+"={@"+arrTemp[k]+"}\n";	
						}
					}
					strSql += " where 1=1 ;";
					$("#frm-defendtext").insert(strSql);
				}
			});
		}catch(err){}	
	});
	
});

var FindKeywords=function(back){
	/**********************************************************************************************
	*获取字段
	***********************************************************************************************/
	var clnText = "";
	try{
		$("#frm-columns").find("a").each(function(){
			var strValue = $(this).attr("value");
			if(strValue!=undefined && strValue!=""){
				if(clnText!=""){clnText=clnText+","+strValue;}
				else{clnText=strValue;}
			}
		});
	}catch(err){
		$.messager.alert('系统提示','发生未知错误,请重试！');
	}
	/**********************************************************************************************
	*获取数据表
	***********************************************************************************************/
	var Tablename = "";
	try{
		if($("#frm-table")[0]!=undefined && $("#frm-table")[0]!=null && $("#frm-table").val()!=""){
			Tablename = $("#frm-table").val();
		}
	}catch(err){
		$.messager.alert('系统提示','发生未知错误1,请重试！');
	}
	/**********************************************************************************************
	*返回
	***********************************************************************************************/
	try{
		if(clnText!=undefined && clnText!="" && Tablename!=undefined && Tablename!=""){
			if(back!=undefined && typeof(back)=='function'){
				back(clnText,Tablename);	
			}	
		}
	}catch(err){
		$.messager.alert('系统提示','发生未知错误,请重12试！');
	}
}

;(function($) {
"use strict";
	$.fn.insert=function(strValue){
		try{
			if(this[0]!=undefined && this[0]!=null){
				var $t=this[0];	
				if ($t!=undefined && $t!=null && $t.selection) {
					$t.focus();
					sel = $t.selection.createRange();
					sel.text = strValue;
					$t.focus();
				}
				else if ($t.selectionStart || $t.selectionStart == '0') {
					var startPos = $t.selectionStart;
					var endPos = $t.selectionEnd;
					var scrollTop = $t.scrollTop;
					$t.value = $t.value.substring(0, startPos) + strValue + $t.value.substring(endPos, $t.value.length);
					//this.focus();
					$t.selectionStart = startPos + strValue.length;
					$t.selectionEnd = startPos + strValue.length;
					$t.scrollTop = scrollTop;
				}
				else {
					this.value += strValue;
					this.focus();
				}
			}
		}catch(err){}
	}
	
})(jQuery);