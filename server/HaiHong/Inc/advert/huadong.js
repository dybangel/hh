var AdvHuadong{$id}=function(options)
{
	/**************************************************************************************
	*渲染网页广告图片
	***************************************************************************************/
	var render = function()
	{
		var strTemplate ="";
		strTemplate += '<table cellpadding="3" cellspacing="1" >';
		strTemplate += '<tr>';
		$(options).each(function(k,items){
				strTemplate += "<td onclick=\"window.location='"+items['url']+"'\">";
				strTemplate += '<img src="'+items["thumb"]+'">';
				strTemplate += '</td>';
		});
		strTemplate += '</tr>';
		strTemplate += '</table>';
		/**************************************************************************************
		*为变量赋值
		***************************************************************************************/
		if(strTemplate!=undefined && strTemplate!=null 
		&& strTemplate!="" && document)
		{
			document.write(strTemplate);
		}
	}
	/**************************************************************************************
	*预设网页内容
	***************************************************************************************/
	options = options || [];
	/**************************************************************************************
	*准备构建网页内容信息
	***************************************************************************************/
	if(options!=undefined && options!=null 
	&& typeof(options)=='object')
	{
		try{render();}catch(err){}
	}
};
/********************************************************************************************
*构建广告内容信息
*********************************************************************************************/
var HuadongOption{$id} = {$json};
/********************************************************************************************
*生成数据信息
*********************************************************************************************/
try{
	if(HuadongOption{$id}!=undefined && HuadongOption{$id}!=null 
	&& typeof(HuadongOption{$id})=='object')
	{
		AdvHuadong{$id}({$json});	
	}
}catch(err){}
