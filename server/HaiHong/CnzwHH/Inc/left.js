﻿$(function(){
$("div.titles").click(function(){
	$("div.titles").each(function(){
		$(this).removeClass("current");	
		$(this).parents(".MenusBar").find(".MenusList").hide();
	});
	$(this).addClass("current");
	$(this).parents(".MenusBar").find(".MenusList").show();
})		   
})