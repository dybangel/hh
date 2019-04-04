var ZipDictionary = {
    "data": [
        {
            "countryName": "America",
            "phoneCode": "1",
            "countryCode": "USA"
        },
        {
            "countryName": "United Kiongdom",
            "phoneCode": "44",
            "countryCode": "Kiongdom"
        },
		{
            "countryName": "China",
            "phoneCode": "086",
            "countryCode": "China"
        },
        {
            "countryName": "France",
            "phoneCode": "33",
            "countryCode": "France"
        },
        {
            "countryName": "Russia",
            "phoneCode": "7",
            "countryCode": "Russia"
        },
        {
            "countryName": "Malaysia",
            "phoneCode": "60",
            "countryCode": "马来西亚"
        },
        {
            "countryName": "Singapore",
            "phoneCode": "65",
            "countryCode": "Singapore"
        },
        {
            "countryName": "Thailand",
            "phoneCode": "66",
            "countryCode": "Thailand"
        },
        {
            "countryName": "Japan",
            "phoneCode": "81",
            "countryCode": "Japan"
        },
        {
            "countryName": "Korea",
            "phoneCode": "82",
            "countryCode": "Korea"
        },
        {
            "countryName": "HK,China",
            "phoneCode": "852",
            "countryCode": "Hongkong"
        },
        {
            "countryName": "Taiwan",
            "phoneCode": "886",
            "countryCode": "Taiwan"
        }
    ]
};
/********************************************************************************************
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*county zipcode functions
*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
*********************************************************************************************/
;(function($){
	/********************************************************************************************
	*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
	* loading select options
	*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
	*********************************************************************************************/
	$.fn.Ziption = function(options)
	{
		
		
		var GetOptions = function()
		{
			/********************************************************************************
			*开始创建数据返回
			*********************************************************************************/
			var strTemplate = "<option value=\"\">Select national</option>";
			/********************************************************************************
			*构建网络显示列表
			*********************************************************************************/
			if(window.ZipDictionary!=undefined && window.ZipDictionary!=null 
			&& typeof(window.ZipDictionary)=='object' && ZipDictionary['data']!=undefined
			&& ZipDictionary['data']!=null && typeof(ZipDictionary['data']=='object'))
			{
				$(window.ZipDictionary['data']).each(function(k,json){
					
					var strValue = json['phoneCode'];
					try{
						if(options['value']!=undefined && options['value']!=null && options['value']!="" 
						&& json[options['value']]!=undefined && json[options['value']]!=null){
							strValue = json[options['value']];
						};
					}catch(err){}
					
					var strText = json['countryName'];
					try{
						if(options['text']!=undefined && options['text']!=null && options['text']!="" 
						&& json[options['text']]!=undefined && json[options['text']]!=null){
							strValue = json[options['text']];
						};
					}catch(err){}
					try{
						strTemplate += "<option zip=\""+json['phoneCode']+"\"";
						strTemplate += " text=\""+json['countryName']+"\"";
						strTemplate += " value=\""+strValue+"\"";
						if(options['default']!=undefined && options['default']!=null 
						&& options['default']!="" && options['default']==strValue)
						{strTemplate += " selected";}
						strTemplate += ">";
						strTemplate += ""+strText+"";
						strTemplate += "</option>";
					}catch(err){}
				});
			};
			/********************************************************************************
			*返回数据处理结果
			*********************************************************************************/
			return strTemplate;
		};
		/*******************************************************************************
		*设置容器信息
		********************************************************************************/
		var thisContianer = this;
		var options = options || {
			"value":"phoneCode",
			"text":"countryName",
			"default":""
		};
		/********************************************************************************
		*清空请求数据信息
		*********************************************************************************/
		if(thisContianer!=undefined && thisContianer!=null){
			try{$(thisContianer).empty();}catch(err){}	
		}
		/********************************************************************************
		*监听点击时间信息
		*********************************************************************************/
		try{$(thisContianer).html(GetOptions());}catch(err){}
		/********************************************************************************
		*监听点击时间信息
		*********************************************************************************/
		if(thisContianer!=undefined && thisContianer!=null 
		&& options!=undefined && options!=null && typeof(options)=='object'
		&& options['callback']!=undefined && typeof(options['callback'])=='function')
		{
			try{
				$(thisContianer).change(function(){
					
					if(thisContianer[0]!=undefined && thisContianer[0]!=null 
					&& this.options.selectedIndex!=undefined && this.options.selectedIndex!=null
					&& this.value!=undefined && this.value!=null && this.value!="")
					{
						try{
							var opts = thisContianer[0].options[this.options.selectedIndex];
							var value = $(opts).attr("value") || "";
							var text = $(opts).attr("text") || "";
							var zip = $(opts).attr("zip") || "";
							options['callback']({"zip":zip,'value':value,'text':text});	
						}catch(err){}
					}								 
				});
			}catch(err){}
		};
		
	}	   
		   
})(jQuery);