// JavaScript Document
(function($){
	$.fn.myScroll = function(options){
		//默认配置
		var defaults = {
			speed:40,  //滚动速度,值越大速度越慢
			rowHeight:24 //每行的高度
		};
		var obj = this;
		var opts = $.extend({}, defaults, options),intId = [];
		
		function marquee(obj, step)
		{
			try{var copyThis = $(obj).find("ul:eq(0)").clone();}catch(err){}
			try{$(obj).append(copyThis);}catch(err){}
			try{
				var offsetHeight = parseFloat($(obj)[0].offsetHeight);
				var scrollHeight = parseFloat($(obj)[0].scrollHeight);
				var divHeight = parseInt(offsetHeight) + parseInt(scrollHeight);
				var scrollTop = 0;var timer = 0;
				var inter = setInterval(function(){
					if(scrollTop<scrollHeight){
						try{
							scrollTop = scrollTop + opts['rowHeight'];
							$(obj).animate({'scrollTop':scrollTop});
						}
						catch(err){}
						if(scrollTop>=scrollHeight){scrollTop=0;}
					}else if(scrollTop>=scrollHeight){scrollTop=0;}
				},3000);
			}catch(err){}
		}
		
		marquee(obj,50);
	};
})(jQuery);