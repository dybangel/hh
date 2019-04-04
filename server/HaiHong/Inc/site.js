$(function(){
	/***********************************************************************************
	*切换选项
	************************************************************************************/
	$("#frm-menu").find("span[operate=\"menu\"]").click(function(){
		/**************************************************************************
		*设置展现样式信息
		***************************************************************************/
		try{$("#frm-menu").find("span[operate=\"menu\"]").removeClass('current');$(this).addClass('current');}catch(err){}
		/**************************************************************************
		*设置数据请求模块
		***************************************************************************/
		try
		{
			var title = $(this).attr("title") || "";
			var url = $(this).attr("url") || "";
			 if(url!=undefined && url!=null && url!="" 
			&& title!=undefined && title=='alliance')
			{
				$("#frm-alliance").pager({"url":url,"ipager":"true"});
				//$("#frmLayout").hide();
				//$("#frm-alliance").show();
			}
			else if(url!=undefined && url!=null && url!="")
			{
				$("#frm-alliance").pager({"url":url,"ipager":"true"});
				//$("#frmLayout").show();
			};
		}catch(err){}
	});	
	/***********************************************************************************
	*点击关注
	************************************************************************************/
	$("#frmFollow").click(function(){
		try
		{
			var businessId = parseInt($(this).attr("businessid")) || 0;
			if(businessId!=undefined && businessId!=0){SaveFollow(this,businessId);}
		}catch(err){}
	});
});
/***********************************************************************************
*关注店铺
************************************************************************************/
var SaveFollow = function(contianer,businessId)
{
	try{
		Send({
			 "url":"Follow.aspx?action=follow&businessId="+businessId+"",
			 "back":function(json){
				 try{
					if(json!=undefined && json!=null && typeof(json)=='object' 
					&& json['mode']!=undefined && json['mode']!=null){
						if(json['mode']=='add'){
							$(contianer).attr('value','已关注');	
						}else{
							$(contianer).attr('value','已取消');	
						}
					}
				 }catch(err){}
				 /*************************************************************************
				 *设置已关注人数
				 **************************************************************************/
				 try
				 {
					 var follow = parseInt(json['follow']) || 0;
					 if(follow!=undefined && follow!=null)
					 {
						$(contianer).attr('text',""+follow+"人关注");		 
					 }
				 }catch(err){}
			  }
		});
	}catch(err){}
}