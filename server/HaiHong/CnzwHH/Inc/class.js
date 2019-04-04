$(function(){
$("select[operate=\"Channel\"]").change(function(){											 
	if(this.value!="0" && this.value!="" && !isNaN(this.value)){
		var toUrl = "class.aspx?action=getclass&channelid="+this.value+"&math="+Math.random();
			$.ajax({type:'Get',url:toUrl,success:function(strReader){$("#parent_box").html(strReader);}
		});
		try{
			var jsonText = $(this.options[this.selectedIndex]).attr("json");
			if(jsonText!=undefined && jsonText.indexOf('{')!=-1){
				var json = $.parseJSON(jsonText);
				if(json!=undefined && json["channelname"]!=undefined){
					$("#Template").val('{@dir}/'+json["channelname"]+'/列表页.html');
					$("#cTemplate").val('{@dir}/'+json["channelname"]+'/内容页.html');	
				}
			}
		}catch(e){}
	}
});
$("input[operate=\"convertChar\"]").keyup(function(){toConvert(this,'Identify');	});
$("td[operate=\"Open\"]").click(function(){
var ParentID=$(this).attr("ParentID");
if(ParentID!="" && ParentID!=undefined){
var toUrl = "class.aspx?action=getchild&parentid="+ParentID+"&math="+Math.random();
$.ajax({
	type:'Get',
	url:toUrl,
	success:function(strReader){
		$("#_child_"+ParentID+"").parents('tr').show();
		$("#_child_"+ParentID+"").html(strReader);
	}
});
}
});
});
/**********************************************************************
灞曞紑瀛愭爮鐩垎绫?
***********************************************************************/
var GetChild=function(ParentID,ParentName,cuteTxt,t){
try{
	var obj = $("#_child_"+ParentID+"");
	if($(obj).attr('isEnbile')!='1' || $(obj).attr('isEnbile')==undefined){
		$.ajax({
			type:'get',
			url:"class.aspx?action=getchild&parentid="+ParentID+"&ParentName="+ParentName+"&cuteTxt="+cuteTxt+"&math="+Math.random(),
			success:function(strReader){
				$($(obj)[0].parentNode).show();
				$(obj).attr('isEnbile','1');
				$(obj).html(strReader);
				$(t).attr("src","template/images/s.gif");
			}
		});	
	
	}else{
	$($(obj)[0].parentNode).hide();	
	$(obj).attr('isEnbile','0');
	$(t).attr("src","template/images/b.gif");
	}	
}catch(err){alert(err.message);}
}

var OpenOfAll = function(){
  try{
	  jQuery.cookies('fooke_jsapi_isBrowse','1');
	  window.location.reload();
  }catch(err){alert(err.Message);}
}

var CloseOfAll = function(){
  try{
	  jQuery.cookies('fooke_jsapi_isBrowse','0');
	  window.location.reload();
  }catch(err){alert(err.Message);}
}
var EditSort=function(obj,classId,SortID){
	if(!isNaN(obj.value) && obj.value!=SortID){
		var toUrl = "class.aspx?action=savest&classId="+classId+"&isOrder="+obj.value+"&math="+Math.random();
		$.ajax({
			type:'Get',
			url:toUrl,
			success:function(strReader){ThatTips(strReader);}
		});	
	}	
}
