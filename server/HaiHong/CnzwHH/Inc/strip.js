$(function(){
	$("#LeftMenu").click(function(){
		var isClosed = false;
		if($(top.document).find("#MainFrame").attr("cols")!='undefined' && $(top.document).find("#MainFrame").attr("cols")=='180,*'){isClosed=true;}
		if(isClosed){
			$(top.document).find("#MainFrame").attr("cols","0,*");
			$(this).find("img").attr("src","images/ico/right.png");
		}else{
			$(top.document).find("#MainFrame").attr("cols","180,*");
			$(this).find("img").attr("src","images/ico/left.png");
		};
		jQuery.cookie("leftClosed",isClosed);
		
	});
	$("#RefreshMenu").click(function(){top.Right.window.location.reload();});
	$("#HomeMenu").click(function(){top.Right.window.location='index.aspx?action=right';});
	$("#HistoryMenu").click(function(){top.Right.history.go(-1);});
	$("#ForwardMenu").click(function(){top.Right.history.go(1);});
});