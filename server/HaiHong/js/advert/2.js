var AdvSlider2=function(options)
{
	/**************************************************************************************
	*start render advert
	*date 20170914
	*author liu123028
	***************************************************************************************/
	var render = function()
	{
		var strTemplate = '';
		var iSlider = false;
		if(options!=undefined && options!=null 
		&& typeof(options)=='object' && options.length>=2)
		{
			iSlider = true;
			strTemplate += '<div class="main_visual">';
			strTemplate += '<div class="flicking_con" id="conThis">';
			$(options).each(function(k,items){
				strTemplate += '<a href="javascript:void(0)"></a>';						 
			});
			strTemplate += '</div>';
			strTemplate += '<div class="main_image">';
			strTemplate += '<ul id="imgThis">';
			$(options).each(function(k,items){
				strTemplate += '<li style="background:url('+items["thumb"]+') center center no-repeat;background-size:100% 100%"></li>';
			});
			strTemplate += '</ul>';
			strTemplate += '<a href="javascript:;" style="display:none;" id="btn_prev"></a>';
			strTemplate += '<a href="javascript:;" style="display:none;" id="btn_next"></a>';
			strTemplate += '</div>';
			strTemplate += '</div>';	
		}
		else if(options!=undefined && options!=null 
		&& typeof(options)=='object' && options.length==1)
		{
			strTemplate += '<li style="width:100%;height:190px;;background:url('+options[0]["thumb"]+') center center no-repeat;background-size:100% 100%"></li>';	
		};
		
		/**************************************************************************************
		*start run replace
		*date 20170914
		*author liu123028
		***************************************************************************************/
		if(strTemplate!=undefined && strTemplate!=null 
		&& strTemplate!="" && document)
		{
			document.write(strTemplate);
			try{
				if(iSlider){StartSlider();}
			}catch(err){}
		}
	}
	/**************************************************************************************
	*start run slider
	*date 20170914
	*author liu123028
	***************************************************************************************/
	var StartSlider = function()
	{
		try
		{
			$(".main_visual").hover(function(){$("#btn_prev,#btn_next").fadeIn();},
						function(){$("#btn_prev,#btn_next").fadeOut();});
			$dragBln = false;
			$(".main_image").touchSlider({
				flexible : true,speed : 200,
				btn_prev : $("#btn_prev"),
				btn_next : $("#btn_next"),
				paging : $(".flicking_con a"),
				counter : function (e){$(".flicking_con a").removeClass("on").eq(e.current-1).addClass("on");}
			});
			$(".main_image").bind("mousedown", function() {$dragBln = false;});
			$(".main_image").bind("dragstart", function() {$dragBln = true;});
			$(".main_image a").click(function(){if($dragBln) {return false;}});
			timer = setInterval(function(){$("#btn_next").click();}, 5000);
			$(".main_visual").hover(function(){clearInterval(timer);},function(){
				timer = setInterval(function(){$("#btn_next").click();},5000);
			});
			$(".main_image").bind("touchstart",function(){clearInterval(timer);}).bind("touchend", function(){
				timer = setInterval(function(){$("#btn_next").click();}, 5000);
			});
		}catch(err){}	
	}
	/**************************************************************************************
	*start set defaults
	*date 20170914
	*author liu123028
	***************************************************************************************/
	options = options || [];
	/**************************************************************************************
	*start build advert
	*date 20170914
	*author liu123028
	***************************************************************************************/
	if(options!=undefined && options!=null 
	&& typeof(options)=='object')
	{
		try{render();}catch(err){}
	}
};
/**************************************************************************************
*start build defaults options as json
*date 20170914
*author liu123028
***************************************************************************************/
var advOption2 = [{"success":"true","url":"#","title":"�����Ž���ͼ","thumb":"/file/banner/5.png","strcode":""},{"success":"true","url":"#","title":"�����Ž���ͼ","thumb":"/file/banner/4.png","strcode":""},{"success":"true","url":"#","title":"�����Ž���ͼ","thumb":"/file/banner/3.png","strcode":""},{"success":"true","url":"#","title":"�ڶ��Ž���ͼ","thumb":"/file/banner/2.png","strcode":""},{"success":"true","url":"#","title":"��һ�Ž���ͼ","thumb":"/file/banner/1.png","strcode":""}];
/**************************************************************************************
*start advert
*date 20170914
*author liu123028
***************************************************************************************/
try{
	if(advOption2!=undefined && advOption2!=null 
	&& typeof(advOption2)=='object')
	{
		AdvSlider2([{"success":"true","url":"#","title":"�����Ž���ͼ","thumb":"/file/banner/5.png","strcode":""},{"success":"true","url":"#","title":"�����Ž���ͼ","thumb":"/file/banner/4.png","strcode":""},{"success":"true","url":"#","title":"�����Ž���ͼ","thumb":"/file/banner/3.png","strcode":""},{"success":"true","url":"#","title":"�ڶ��Ž���ͼ","thumb":"/file/banner/2.png","strcode":""},{"success":"true","url":"#","title":"��һ�Ž���ͼ","thumb":"/file/banner/1.png","strcode":""}]);	
	}
}catch(err){}
