$(function(){
	$("select[name=ChannelID]").change(function(){
		var channelid = this.value;
		if(channelid!=0){
			
			var url = "../label.aspx?action=getclass&channelid="+channelid+"&math="+Math.random();
			$.ajax({
				url:url,
				type:"get",
				success:function(strReader){
					$("#class_box").html(strReader);
				}
			});
			
			var colurl = "../label.aspx?action=columns&channelid="+channelid+"&math="+Math.random();
			$.ajax({
				url:colurl,
				type:"get",
				success:function(strReader){
					$(".showcolumns").html(strReader);
				}
			});
		}
	});		
	
	$("select[name=fromsId]").change(function(){fromsChange(this);})
})

var fromsChange=function(obj){
if(obj.value!=0 && !isNaN(obj.value)){
	var url = "../label.aspx?action=getfroms&fromsId="+obj.value+"&math="+Math.random();
	$.ajax({
	  url:url,
	  type:"get",
	  success:function(strReader){
		  if(strReader.indexOf('error')==-1){
			$(".showcolumns").html(strReader);
		  }else{alert(strReader);}
	  }
	});
}
}

/*************************************************************************
浠ヤ笅涓烘爣绛捐嚜閫夊唴瀹瑰姛鑳?
**************************************************************************/

var setTxt = function(vTxt){
	insertAtCaret(vTxt);	
}

var insertAtCaret=function(myValue){
var $t=$("#LabelContent")[0];
if (document.getElementById("LabelContent").selection) {
document.getElementById("LabelContent").focus();
sel = document.getElementById("LabelContent").selection.createRange();
sel.text = myValue;
document.getElementById("LabelContent").focus();
}
else 
if ($t.selectionStart || $t.selectionStart == '0') {
var startPos = $t.selectionStart;
var endPos = $t.selectionEnd;
var scrollTop = $t.scrollTop;
$t.value = $t.value.substring(0, startPos) + myValue + $t.value.substring(endPos, $t.value.length);
this.focus();
$t.selectionStart = startPos + myValue.length;
$t.selectionEnd = startPos + myValue.length;
$t.scrollTop = scrollTop;
}
else {
this.value += myValue;
this.focus();
}
}