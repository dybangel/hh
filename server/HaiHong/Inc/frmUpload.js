/********************************************************************************************
*构建frmUpload上传插件
*********************************************************************************************/
;(function($){
"use strict";
	$.fn.frmUpload = function(options)
	{
		var thisContianer = this;
		/********************************************************************************************
		*设置框架的默认样式
		*********************************************************************************************/
		if(thisContianer!=undefined && thisContianer!=null)
		{
			thisContianer.style="display:block; width:100%; height:220px; clear:both; border:#ccc solid 1px; margin:0px; position:relative; background-repeat:no-repeat; background-size:100%; background-position:center"	
		}
		/********************************************************************************************
		*准备上传文件
		*********************************************************************************************/
		var StartUpload = function(theFile,back)
		{
			var formData = new FormData();
			if(theFile!=undefined && theFile!=null)
			{
				formData.append("fileName",theFile);				
			}
			if(formData!=undefined && formData!=null 
			&& theFile!=undefined && theFile!=null)
			{
				/********************************************************************************************
				*给出上传准备提示
				*********************************************************************************************/
				try
				{
					if(back!=undefined && back!=null 
					&& typeof(back)=='function')
					{back('正在上传',0);}
				}catch(err){}
				/********************************************************************************************
				*准备上传文件
				*********************************************************************************************/
				$.ajax({url: options['url'],type: 'POST', data: formData,
					dataType:'json',async: false,cache: false,contentType: false,processData: false,  
					success: function (opt) 
					{  
					  		if(opt!=undefined && opt!=null && typeof(opt)=='object')
							{
								render(opt['url']);	
								/******************************************************************
								*渲染动画信息
								********************************************************************/
								try{
									var selectionIndex = parseInt(getLength()-1) || 0;
									if(selectionIndex<=0){selectionIndex=0;}
									try{SwitchBackground(selectionIndex);}catch(err){}
								}catch(err){}
							};
							try
							{
								if(back!=undefined && back!=null 
								&& typeof(back)=='function')
								{back('上传成功',100);}
							}catch(err){}
					},
					error: function (json) 
					{  
					  	try
						{
							if(back!=undefined && back!=null 
							&& typeof(back)=='function')
							{back('上传失败',0);}
						}catch(err){}
					}
				 });	
			}
		};	
		/********************************************************************************************
		*计算子数据个数,生成一个新的序列
		*********************************************************************************************/
		var frmSequence = null;
		var Calculate = function()
		{
			
			if(!frmSequence){
				var strSequence = "<div operate=\"frmSequence\"></div>";
				frmSequence = $(strSequence)[0];
				$(thisContianer).append(frmSequence);
			}
			var Length = parseInt($(thisContianer).find("div[operate=\"subitems\"]").length) || 0;
			if(frmSequence!=undefined && frmSequence!=null && Length>=1)
			{
				var strTemplate = "";
				for(var k = 1;k<=Length;k++){strTemplate += "<span>"+k+"</span>";};
				$(frmSequence).html(strTemplate);
			}
			$(frmSequence).find("span").click(function(){
				var selectionIndex = (parseInt($(this).text())-1) || 0;
				if(selectionIndex>=0){SwitchBackground(selectionIndex);}
			});
		};
		/********************************************************************************************
		*切换商品图片背景
		*********************************************************************************************/
		var SwitchBackground = function(selectionIndex)
		{
			try{
				if(selectionIndex>=0 && selectionIndex<=100){
					var subItems = $(thisContianer).find("div[operate=\"subitems\"]")[selectionIndex];	
					if(subItems!=undefined && subItems!=null)
					{
						var url = $(subItems).attr("data");
						thisContianer[0].style="background:url("+url+"); background-repeat:no-repeat; background-size:contain;background-position:center";
					}	
				}
			}catch(err){}
		}
		
		/********************************************************************************************
		*显示上传文件结果信息
		*********************************************************************************************/
		var render = function(fileValue)
		{
			try
			{
				//overflow:hidden;background:url("+fileValue+") #fff;background-repeat:no-repeat; background-position:center center; background-size:100%;
				if(fileValue!=undefined && fileValue!=null && fileValue!="")
				{
					var strControl = "<div data=\""+(fileValue)+"\" operate=\"subitems\" style=\"\"><input type=\"hidden\" value=\""+(fileValue)+"\" name=\""+(options["name"] || "thumb")+"\" /><span operate=\"close\" style=\"display:block; width:14px; height:14px; background:#cd0000;text-align:center; line-height:14px; color:#fff; border-radius:0px; cursor:pointer; font-size:14px; position:absolute;right:2px;top:2px;z-Index:1;\" title=\"delete\">-</span></div>";
					/********************************************************************************************
					*开始加载图片信息
					*********************************************************************************************/
					try
					{
						if(strControl!=undefined && strControl!="" 
						&& $(strControl)[0]!=undefined && $(strControl)[0]!=null){
							var frmControl = $(strControl)[0];
							$(thisContianer).append(frmControl);
							try{Calculate();}catch(err){alert(err.message);}
							$(frmControl).find("span[operate=\"close\"]").click(function(){
								if(confirm('你确定要移除当前图片?')){
									$(frmControl).remove();
									try{Calculate();}catch(err){}
								};													
							});	
						}
					}catch(err){}
				}
			}catch(err){};
			
		};
		
		/********************************************************************************************
		*获取当前上传
		*********************************************************************************************/
		var getLength = function()
		{
			try
			{
				var Length = 0;
				Length = parseInt($(thisContianer).find("div[operate=\"subitems\"]").length) || 0;
				return Length;
			}catch(err){}
		};
		/********************************************************************************************
		*渲染CSS的样式信息，背景动画信息
		*********************************************************************************************/
		var renderCSS = function()
		{
				
		}
		/********************************************************************************************
		*开始构建上传文件插件内容
		*********************************************************************************************/
		//var strForm = "<form id=\"frm-formContianer\" action=\"SWFupload.aspx\" enctype=\"multipart/form-data\" onSubmit=\"return _doPost(this)\" method=\"post\"></form>";
		var renderContianer = function()
		{
			/********************************************************************************************
			*初始化上传控件盒子
			*********************************************************************************************/
			var strContianer = "<div style=\"display:block; width:60px; position:absolute; height:60px; background:#fff; border:#ccc solid 1px; border-radius:3px;z-Index:100;top:-webkit-calc((100% - 60px) / 2); top:-moz-calc((100% - 60px) / 2);top:calc((100% - 60px) / 2);left:-webkit-calc((100% - 60px) / 2);left:calc((100% - 60px) / 2);left:-moz-calc((100% - 60px) / 2);;overflow:hidden;margin:5px 5px;padding:3px\"><span style=\"width:1px; height:50px;left:25px;top:5px; position:absolute; background:#ddd; display:block;filter:alpha(opacity=70);-moz-opacity:0.7;opacity:0.7;\"></span><span style=\"height:1px; width:50px;top:29px;left:2px; position:absolute; background:#ddd; display:block;filter:alpha(opacity=70);-moz-opacity:0.7;opacity:0.7;\"></span></div>";
			var frmContianer = $(strContianer)[0];
			$(thisContianer).append(frmContianer);
			/********************************************************************************************
			*初始化上传进度条信息
			*********************************************************************************************/
			var strProcess = "<div style=\"width:98%;height:20px;position:absolute;left:1%;top:24px;text-align:center;line-height:20px;\">选择文件</div>";
			var frmProcess = $(strProcess)[0];
			$(frmContianer).append(frmProcess);
			/********************************************************************************************
			*初始化上传file控件
			*********************************************************************************************/
			var strInput = "<input type=\"file\" multiple=\"multiple\" value=\"\" style=\"display:block; position:absolute;left:0px;top:0px; width:60px; height:60px;filter:alpha(opacity=0);-moz-opacity:0;opacity:0;\" />";
			var frmInput = $(strInput)[0];
			$(frmContianer).append(frmInput);
			$(frmInput).change(function(event){
				var Limit = parseInt(options['limit']) || 0;
				if(parseInt(getLength())>=Limit){alert('已超过上传数量');return false;}
				else{
					if(window.FileReader && event!=undefined && event!=null)
				{
					var fileList = event.target.files;  
					for (var i = 0, itemFile; itemFile = fileList[i]; i++) 
					{
						if (itemFile!=undefined && itemFile!=null 
						&& itemFile.type.match('image.*')) 
						{
							var reader = new FileReader();
							reader.onload = (function(theFile) {  
								return function(e) 
								{
									try
									{
										StartUpload(theFile,function(tips,number){
											$(frmProcess).html(tips);							 
										});
									}
									catch(err){alert(err.message);}
								}; 
							})(itemFile);
							reader.readAsDataURL(itemFile);  
						}
					}	
				}
				}
			});
		}
		/********************************************************************************************
		*初始化系统默认参数信息
		*********************************************************************************************/
		var options = options || {limit:8};
		options['limit'] = parseInt(options['limit']) || 8;
		options['url'] = options['url'] || 'SWFupload.aspx?action=start';
		/********************************************************************************************
		*输出网页内容信息
		*********************************************************************************************/
		if(options!=null && options!=undefined)
		{
			try{renderContianer();}catch(err){}	
		}
		/********************************************************************************************
		*设置默认值信息
		*********************************************************************************************/
		if(options!=null && options!=undefined 
		&& options['default']!=undefined && options['default']!="")
		{
			try
			{
				$(options['default']).find("items").each(function(){
					try{
						var fileValue =($(this).html()).toString()
						fileValue = fileValue.replace("<!--[CDATA[","");
						fileValue = fileValue.replace("]]-->","");
						fileValue = fileValue.replace("<![CDATA[","");
						fileValue = fileValue.replace("]]>","");
						render(fileValue);
					}catch(err){}
				});
				/******************************************************************
				*渲染动画信息
				********************************************************************/
				try{SwitchBackground(0);}catch(err){}
			}catch(err){}
		};
		
	};
	/********************************************************************************************
	*显示单图上传信息
	*********************************************************************************************/
	$.fn.fileUpload = function(options)
	{
		var thisContianer = this;
		/************************************************************************************************
		*开始上传文件信息
		*************************************************************************************************/
		var StartUpload = function(theFile,back)
		{
			var formData = new FormData();
			if(theFile!=undefined && theFile!=null)
			{
				formData.append("fileName",theFile);				
			}
			if(formData!=undefined && formData!=null 
			&& theFile!=undefined && theFile!=null)
			{
				/********************************************************************************************
				*给出上传准备提示
				*********************************************************************************************/
				try
				{
					if(back!=undefined && back!=null 
					&& typeof(back)=='function')
					{back('正在上传',0);}
				}catch(err){}
				/********************************************************************************************
				*准备上传文件
				*********************************************************************************************/
				$.ajax({url: options['url'],type: 'POST', data: formData,
					dataType:'json',async: false,cache: false,contentType: false,processData: false,  
					success: function (opt) 
					{  
						if(opt!=undefined && opt!=null && typeof(opt)=='object'
						&& opt['success']!=undefined  && opt['success']=="true"
						&& opt['url']!=undefined && opt['success']!="")
						{	
							render(opt["url"]);	
						};
						try
						{
							if(back!=undefined && back!=null 
							&& typeof(back)=='function')
							{back('上传成功',100);}
						}catch(err){}
					},
					error: function (json) 
					{  
					  	try
						{
							if(back!=undefined && back!=null 
							&& typeof(back)=='function')
							{back('上传失败',0);}
						}catch(err){}
					}
				 });	
			}
		};	
		var fileControls = null;
		/********************************************************************************************
		*显示上传文件结果信息,渲染新上传的文件
		*********************************************************************************************/
		var render = function(fileValue)
		{
			/*****************************************************************************************
			*检查控件是否存在,不存在则创建
			******************************************************************************************/
			try{options["name"]=(options["name"] || "thumb");}catch(err){}
			try{
				if(!fileControls && options["name"]!=undefined && options["name"]!="")
				{
					
					var strControl = "<input type=\"hidden\" value=\""+(fileValue)+"\" name=\""+(options["name"] || "thumb")+"\" />";
					fileControls = $(strControl)[0];
					if(fileControls!=undefined && fileControls!=null 
					&& thisContianer!=undefined && thisContianer!=null)
					{
						$(thisContianer).append(fileControls);	
					}
				}
			}catch(err){}
			/*****************************************************************************************
			*开始控件赋值
			******************************************************************************************/
			try
			{
				if(fileValue!=undefined && fileValue!=null 
				&& fileValue!="" && fileControls!=undefined 
				&& fileControls!=null)
				{
					$(fileControls).val(fileValue);
				}
			}catch(err){};
		};
		/********************************************************************************************
		*渲染初始控件信息
		*********************************************************************************************/
		var renderContianer = function()
		{
			/********************************************************************************************
			*初始化上传控件盒子
			*********************************************************************************************/
			var strContianer = "<div style=\"display:block; width:100%; position:relative; height:100%; background:url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADwAAAA8CAYAAAA6/NlyAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6MTBBRUQ1RDU1RkNCMTFFNzkxNTVCODI2Q0U1MUZBQ0IiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6MTBBRUQ1RDY1RkNCMTFFNzkxNTVCODI2Q0U1MUZBQ0IiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDoxMEFFRDVEMzVGQ0IxMUU3OTE1NUI4MjZDRTUxRkFDQiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDoxMEFFRDVENDVGQ0IxMUU3OTE1NUI4MjZDRTUxRkFDQiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pp1e7rcAAANVSURBVHja7JtZaBNBGMenraZeFZWKxeJR2wY8sLQqKhaxFEUUFBG0WkXwRZGCCj4URR8EqYiiIOKLPniUelTUKnigpXiRirXiBY0IoiI+eDxUVKoY/x/5ImHZTTZxdzM7mQ9+bDIzIfl1JjPfzKY5kUhEZFPkaGEtrIW1sK+Ewz09vhYoDwb/PX4VDj/EpQRUovy9WftcxTpwBigEZVYNckWWhRZWPfoZngfAJDBMws/6CHxzapYeBfaA1WCgpJ1TCZ6AdaAB9DdpU8HX1yZ/HFp/26mHx4N7oJgrfoMP3ECm6ONrHZiepG2pRfkEEj4cJ7sX7AY/JP4argVzaHSa1F3k607w3KT+JQ3pPh4ez8BUnycesVFZg/IOq1k69l34ko2ztN0YBGrBRH7+AtwGP1UUrgf7QZGh/CPYBppVSjxoOThtIiu4jOo2ZdDnD19/OdHDY7hnk8UBcAW8y4DwKjAWhJwQXgnybbQbAFawuNcz9jknh3QwhbalKkxan1No68USN4KTD9PPhTX5DT8Moefr0unhGym0veay7EzOq7shNsuizTimKN0h3cGTUbJoA/ddlN0M7vIkStyB9Ba3liVag68n6dl6l0QLQCs4ZNgp0eODkG4FQ50W7gWLwFLQAp4yLVy22Ik9q0lQjv8YLE/Qhuq6IF3hdKYV4WHb5tHEuh4c4eUuWdDhXQjSDU7n0l4EHUQc5Q1/KkF/mGN+E6Y1/wKYkulc2ougLK3LDVnZhAN8+nIWDJFtP+x0UIJwXkTvHEi1PUy0sZiW5mtpKev2QtYJ4XyeSc+AB2BjCq/NA03gKhgu6wFAfJQYJAMs32zjO0j5bTtolP3EIxZLOPOpMqmjw3y6bTnZ4rU1nPjP9cMRD01y+8Blkfh2DB3udYI1cWW0ldsBbononQ7p98Ojecmottl+MDjF7elw/zhYmOnlwG4P1/IwrE7jPTaAtzLI2hGm+l3gJhj5H++T54cjnkKecRcIhcJKeDZnPsVCsTAOaZpJt9KxiYqyxh4u4C3ZMqFwxAtXWSQSSg9poYW1sBb2nXCv4o5fjcInFRc+YRRu5M24itFUHgxeMgrTrZH5InrCTz9Q++5zyU8i+nuteZDdbqzU/wKghbWwFvZV/BVgAJf4v1YWZV5dAAAAAElFTkSuQmCC) center center no-repeat #fff; border:#ccc solid 1px; border-radius:3px;z-Index:100;overflow:hidden;padding:3px;\"></div>";
			var frmContianer = $(strContianer)[0];
			$(thisContianer).append(frmContianer);
			/********************************************************************************************
			*初始化上传file控件
			*********************************************************************************************/
			var frmInput = $("<input type=\"file\" value=\"\" style=\"display:block; position:absolute;left:0px;top:0px; width:100%; height:100%;filter:alpha(opacity=0);-moz-opacity:0;opacity:0;\" />")[0];
			$(frmContianer).append(frmInput);
			$(frmInput).change(function(event){
				if(window.FileReader && event!=undefined 
				&& event!=null && event.target.files!=undefined 
				&& event.target.files!=null && event.target.files[0]!=undefined 
				&& event.target.files[0]!=null && options!=undefined && options!=null)
				{
					var thisFile = event.target.files[0];
					/********************************************************************************
					*验证上传文件格式信息
					*********************************************************************************/
					try{
						var fileSize = parseInt(thisFile["size"]) || 0;
						var maxSize = parseInt(options["size"]) || 1024*1024*2;
						if(fileSize>maxSize){alert('上传文件大小超出限定');return false;}
					}catch(err){}
					/********************************************************************************
					*验证上传文件后缀名信息
					*********************************************************************************/
					try{
						var fileName = thisFile["name"] || "";
						if(fileName==undefined || fileName==""){alert('获取上传文件名称错误,请重试！');}
						var fileFlter = options["flter"] || "png|bmp|jpg|gif|rar|zip|doc"; 
						var fileTemp = fileName.split(".");
						if(fileTemp.length<=0){alert('上传文件格式错误,请重试！');return false;}
						var fileExc = fileTemp[fileTemp.length-1] || "";
						if(fileFlter.indexOf(fileExc.toLowerCase())==-1){alert('上传文件格式错误,请重试！');return false;}
					}catch(err){}
					/***************************************************************************
					*初始化文件信息,加载文件信息准备上传
					****************************************************************************/
					try
					{
						var reader = new FileReader();
						reader.onload = (function(theFile){  
							return function(e) 
							{
								/******************************************************************
								*渲染上传文件信息
								*******************************************************************/
								try{
									$(frmContianer).css({"background":"url("+e.target.result+") center center no-repeat #fff","background-size":"contain"});
								}catch(err){}
								/******************************************************************
								*开始上传文件
								*******************************************************************/
								try
								{
									StartUpload(theFile,function(tips,number){
										$(frmProcess).html(tips);							 
									});
								}
								catch(err){alert(err.message);}
							};
						})(thisFile);
						reader.readAsDataURL(thisFile);
					}catch(err){}
				}
			});
		};
		/********************************************************************************************
		*初始化系统默认参数信息
		*********************************************************************************************/
		var options = options || 
		{
			"url":"SWFupload.aspx?action=start",
			"flter":"gif|png|jpg|bmp|rar|zip|doc",
			"size":1024*1024
		};
		options['url'] = options['url'] || 'SWFupload.aspx?action=start';
		/********************************************************************************************
		*输出网页内容信息
		*********************************************************************************************/
		if(options!=null && options!=undefined)
		{
			try{renderContianer();}catch(err){}	
		}
		/********************************************************************************************
		*设置默认值信息
		*********************************************************************************************/
		if(options!=null && options!=undefined 
		&& options['default']!=undefined && options['default']!="")
		{
			try{render(options['default']);}catch(err){}
		};
	};
	
	/********************************************************************************************
	*显示图片上传控件,当前图片不上传,只显示出结果
	*********************************************************************************************/
	$.fn.fileContianer = function(options)
	{
		var thisContianer = this;
		/********************************************************************************************
		*显示上传文件结果信息,渲染新上传的文件
		*********************************************************************************************/
		var fileControls = null;
		var render = function(fileValue)
		{
			/*****************************************************************************************
			*检查控件是否存在,不存在则创建
			******************************************************************************************/
			try{options["name"]=(options["name"] || "thumb");}catch(err){}
			try{
				if(!fileControls && options["name"]!=undefined && options["name"]!="")
				{
					
					var strControl = "<input type=\"hidden\" value=\""+(fileValue)+"\" name=\""+(options["name"] || "thumb")+"\" />";
					fileControls = $(strControl)[0];
					if(fileControls!=undefined && fileControls!=null 
					&& thisContianer!=undefined && thisContianer!=null)
					{
						$(thisContianer).append(fileControls);	
					}
				}
			}catch(err){}
			/*****************************************************************************************
			*开始控件赋值
			******************************************************************************************/
			try
			{
				if(fileValue!=undefined && fileValue!=null 
				&& fileValue!="" && fileControls!=undefined 
				&& fileControls!=null)
				{
					$(fileControls).val(fileValue);
				}
			}catch(err){};
		};
		
		/********************************************************************************************
		*渲染初始控件信息
		*********************************************************************************************/
		var renderContianer = function()
		{
			/********************************************************************************************
			*初始化上传控件盒子
			*********************************************************************************************/
			var strContianer = "<div style=\"display:block; width:100%; position:relative; height:100%; background:url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADwAAAA8CAYAAAA6/NlyAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNiAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6MTBBRUQ1RDU1RkNCMTFFNzkxNTVCODI2Q0U1MUZBQ0IiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6MTBBRUQ1RDY1RkNCMTFFNzkxNTVCODI2Q0U1MUZBQ0IiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDoxMEFFRDVEMzVGQ0IxMUU3OTE1NUI4MjZDRTUxRkFDQiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDoxMEFFRDVENDVGQ0IxMUU3OTE1NUI4MjZDRTUxRkFDQiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pp1e7rcAAANVSURBVHja7JtZaBNBGMenraZeFZWKxeJR2wY8sLQqKhaxFEUUFBG0WkXwRZGCCj4URR8EqYiiIOKLPniUelTUKnigpXiRirXiBY0IoiI+eDxUVKoY/x/5ImHZTTZxdzM7mQ9+bDIzIfl1JjPfzKY5kUhEZFPkaGEtrIW1sK+Ewz09vhYoDwb/PX4VDj/EpQRUovy9WftcxTpwBigEZVYNckWWhRZWPfoZngfAJDBMws/6CHxzapYeBfaA1WCgpJ1TCZ6AdaAB9DdpU8HX1yZ/HFp/26mHx4N7oJgrfoMP3ECm6ONrHZiepG2pRfkEEj4cJ7sX7AY/JP4argVzaHSa1F3k607w3KT+JQ3pPh4ez8BUnycesVFZg/IOq1k69l34ko2ztN0YBGrBRH7+AtwGP1UUrgf7QZGh/CPYBppVSjxoOThtIiu4jOo2ZdDnD19/OdHDY7hnk8UBcAW8y4DwKjAWhJwQXgnybbQbAFawuNcz9jknh3QwhbalKkxan1No68USN4KTD9PPhTX5DT8Moefr0unhGym0veay7EzOq7shNsuizTimKN0h3cGTUbJoA/ddlN0M7vIkStyB9Ba3liVag68n6dl6l0QLQCs4ZNgp0eODkG4FQ50W7gWLwFLQAp4yLVy22Ik9q0lQjv8YLE/Qhuq6IF3hdKYV4WHb5tHEuh4c4eUuWdDhXQjSDU7n0l4EHUQc5Q1/KkF/mGN+E6Y1/wKYkulc2ougLK3LDVnZhAN8+nIWDJFtP+x0UIJwXkTvHEi1PUy0sZiW5mtpKev2QtYJ4XyeSc+AB2BjCq/NA03gKhgu6wFAfJQYJAMs32zjO0j5bTtolP3EIxZLOPOpMqmjw3y6bTnZ4rU1nPjP9cMRD01y+8Blkfh2DB3udYI1cWW0ldsBbononQ7p98Ojecmottl+MDjF7elw/zhYmOnlwG4P1/IwrE7jPTaAtzLI2hGm+l3gJhj5H++T54cjnkKecRcIhcJKeDZnPsVCsTAOaZpJt9KxiYqyxh4u4C3ZMqFwxAtXWSQSSg9poYW1sBb2nXCv4o5fjcInFRc+YRRu5M24itFUHgxeMgrTrZH5InrCTz9Q++5zyU8i+nuteZDdbqzU/wKghbWwFvZV/BVgAJf4v1YWZV5dAAAAAElFTkSuQmCC) center center no-repeat #fff; border:#ccc solid 1px; border-radius:3px;z-Index:100;overflow:hidden;padding:3px;\"></div>";
			var frmContianer = $(strContianer)[0];
			$(thisContianer).append(frmContianer);
			/********************************************************************************************
			*初始化上传file控件
			*********************************************************************************************/
			var frmInput = $("<input name=\""+options['name']+"\" type=\"file\" value=\"\" style=\"display:block; position:absolute;left:0px;top:0px; width:100%; height:100%;filter:alpha(opacity=0);-moz-opacity:0;opacity:0;\" />")[0];
			$(frmContianer).append(frmInput);
			$(frmInput).change(function(event){
				if(window.FileReader && event!=undefined 
				&& event!=null && event.target.files!=undefined 
				&& event.target.files!=null && event.target.files[0]!=undefined 
				&& event.target.files[0]!=null && options!=undefined && options!=null)
				{
					var thisFile = event.target.files[0];
					/********************************************************************************
					*验证上传文件格式信息
					*********************************************************************************/
					try{
						var fileSize = parseInt(thisFile["size"]) || 0;
						var maxSize = parseInt(options["size"]) || 1024*1024*2;
						if(fileSize>maxSize){alert('上传文件大小超出限定');return false;}
					}catch(err){}
					/********************************************************************************
					*验证上传文件后缀名信息
					*********************************************************************************/
					try{
						var fileName = thisFile["name"] || "";
						if(fileName==undefined || fileName==""){alert('获取上传文件名称错误,请重试！');}
						var fileFlter = options["flter"] || "png|bmp|jpg|gif|rar|zip|doc"; 
						var fileTemp = fileName.split(".");
						if(fileTemp.length<=0){alert('上传文件格式错误,请重试！');return false;}
						var fileExc = fileTemp[fileTemp.length-1] || "";
						if(fileFlter.indexOf(fileExc.toLowerCase())==-1){alert('上传文件格式错误,请重试！');return false;}
					}catch(err){}
					/***************************************************************************
					*初始化文件信息,加载文件信息准备上传
					****************************************************************************/
					try
					{
						var reader = new FileReader();
						reader.onload = (function(theFile){  
							return function(e) 
							{
								/******************************************************************
								*渲染上传文件信息
								*******************************************************************/
								try{
									$(frmContianer).css({"background":"url("+e.target.result+") center center no-repeat #fff","background-size":"contain"});
								}catch(err){}
							};
						})(thisFile);
						reader.readAsDataURL(thisFile);
					}catch(err){}
				}
			});
		};
		/********************************************************************************************
		*初始化系统默认参数信息
		*********************************************************************************************/
		var options = options || 
		{
			"url":"SWFupload.aspx?action=start",
			"flter":"gif|png|jpg|bmp|rar|zip|doc",
			"size":1024*1024
		};
		options['url'] = options['url'] || 'SWFupload.aspx?action=start';
		/********************************************************************************************
		*输出网页内容信息
		*********************************************************************************************/
		if(options!=null && options!=undefined)
		{
			try{renderContianer();}catch(err){}	
		}
		/********************************************************************************************
		*设置默认值信息
		*********************************************************************************************/
		if(options!=null && options!=undefined 
		&& options['default']!=undefined && options['default']!="")
		{
			try{render(options['default']);}catch(err){}
		};
		
	}
	
	
})(jQuery);
/********************************************************************************************
*准备上传文件
*********************************************************************************************/
var StartUpload = function(theFile,options)
{
	var options = options || {};
	if(theFile!=undefined && theFile!=null && typeof(theFile)=='object' 
	&& options!=undefined && options!=null && typeof(options)=='object')
	{
		/********************************************************************************
		*初始化默认值
		*********************************************************************************/
		try{
			options['url'] = options['url'] || "/Plugin/SWFupload/SWFupload.aspx?action=start";
			options['size'] = parseInt(options['size']) || (1024 * 1024 *2);
			options['fileExc'] = options['fileExc'] || "jpg|gif|png|bmp|rar|zip";
		}catch(err){};
		/********************************************************************************
		*验证数据信息
		*********************************************************************************/
		try{
			var fileSize = parseInt(thisFile["size"]) || 0;
			var maxSize = parseInt(options["size"]) || 1024*1024*2;
			if(fileSize>maxSize){alert('上传文件大小超出限定');return false;}
		}catch(err){};
		/********************************************************************************
		*验证上传文件后缀名信息
		*********************************************************************************/
		try{
			var fileName = thisFile["name"] || "";
			if(fileName==undefined || fileName==""){alert('获取上传文件名称错误,请重试！');}
			var fileFlter = options["fileExc"] || "png|bmp|jpg|gif|rar|zip|doc"; 
			var fileTemp = fileName.split(".");
			if(fileTemp.length<=0){alert('上传文件格式错误,请重试！');return false;}
			var fileExc = fileTemp[fileTemp.length-1] || "";
			if(fileFlter.indexOf(fileExc.toLowerCase())==-1){alert('上传文件格式错误,请重试！');return false;}
		}catch(err){};
		/********************************************************************************
		*准备上传文件
		*********************************************************************************/
		var formData = new FormData();
		if(theFile!=undefined && theFile!=null)
		{
			formData.append("fileName",theFile);				
		};
		if(formData!=undefined && formData!=null 
		&& theFile!=undefined && theFile!=null)
		{
			/********************************************************************************************
			*准备上传文件
			*********************************************************************************************/
			$.ajax({url: options['url'],type: 'POST', data: formData,
				dataType:'json',async: false,cache: false,contentType: false,processData: false,  
				success: function (opt) 
				{  
					try{
						if(opt!=undefined && opt!=null && typeof(opt)=='object'
						&& opt['success']!=undefined  && opt['success']=="true"
						&& opt['url']!=undefined && opt['url']!="")
						{	
							if(options['back']!=undefined && options['back']!=null 
							&& typeof(options['back'])=='function')
							{
								options['back'](opt['url']);
							}
						}
						else if(opt!=undefined && opt!=null && typeof(opt)=='object'
						&& opt['success']!=undefined  && opt['success']!="true")
						{
							try{
								alert('文件上传失败:'+opt['tips']+'');
								return false;
							}catch(err){}	
						};
					}catch(err){}
				},
				error: function (json) 
				{  
					try{
						alert('文件上传失败');
						return false;
					}catch(err){}
				}
			 });	
		}
			
	};
};
/********************************************************************************************
*加载数据信息
*********************************************************************************************/
$(function(){
	$("div[operate=\"fileUpload\"]").each(function(){
		var frmName = $(this).attr("name") || "thumb";
		$(this).fileUpload({"name":frmName});										   
	});		   
});
