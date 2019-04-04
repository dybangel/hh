;(function($) {
"use strict";
	$.fn.eKaoshi = function(options,arrList)
	{
		var contianer = this;
		/********************************************************************************************
		*定义网页控件集合信息
		*********************************************************************************************/
		var Operate = {
			"SelectedIndex":0,
			"thisOption":{},
			"thisFalse":"false",
			"FalseNumber":0,
			"arrOptions":[]
		};
		/********************************************************************************************
		*定义用户已经答题的数据集合
		*********************************************************************************************/
		var eKsList = {};
		/********************************************************************************************
		*跳转到指定的考试题目当中
		*********************************************************************************************/
		var GotoKaoshi = function(SelectedIndex)
		{
			try{
				var SelectedIndex = SelectedIndex || 0;
				SelectedIndex = SelectedIndex-1;
				if(SelectedIndex<=0){SelectedIndex=0;}
				if(SelectedIndex>=arrList['items'].length)
				{SelectedIndex = parseInt(arrList['items'].length) || 0;}
				try{ShowKaoshi(SelectedIndex);}catch(err){}
				/****************************************************************************
				*关闭答题选项卡
				*****************************************************************************/
				try{$(Operate['frmTabsMaster']).hide();}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*提醒客户购买VIP
		*********************************************************************************************/
		var ShowCustomer = function()
		{
			if(WindowsConfirm!=undefined && WindowsConfirm!=null 
			&& typeof(WindowsConfirm)=='function'){
				WindowsConfirm('VIP用户可以开启语音读题和答题技巧等所有功能,是否开启VIP？',function(){
					window.location='Service.aspx';return false;
				},function(){return false;});
				return false;
			}else if(window.confirm!=undefined && window.confirm!=null 
			&& typeof(window.confirm)=='function'){
				if(window.confirm('VIP用户可以开启语音读题和答题技巧等所有功能,是否开启VIP？')){
					window.location='Service.aspx';return false;
				};
			};
		};
		/********************************************************************************************
		*显示指定的考试试题
		*********************************************************************************************/
		var ShowKaoshi = function(SelectedIndex)
		{
			/******************************************************************
			*加载动画数据信息
			*******************************************************************/
			try{$("#frmQuestionContianer").css({"left":"100%"});}catch(err){}
			try{$("#frmLoadMaster").show();}catch(err){}
			/******************************************************************
			*关闭语音播放功能
			*******************************************************************/
			try{ClosePlayer();}catch(err){}
			/******************************************************************
			*将当前试题回答设置为正确试题
			*******************************************************************/
			Operate["thisFalse"] = "false";
			/******************************************************************
			*验证当前答题信息的合法性
			*******************************************************************/
			if(SelectedIndex==undefined){alert('加载试题序号错误！');return false;}
			else if(SelectedIndex==null){alert('加载试题序号错误！');return false;}
			else if(isNaN(parseInt(SelectedIndex))){alert('加载试题序号错误！');return false;}
			else if(parseInt(SelectedIndex)<=-1){alert('加载试题序号错误！');return false;}
			/******************************************************************
			*判断是否需要验证VIP会员信息,以下功能暂时屏蔽
			*******************************************************************/
			if(SelectedIndex>=9999 && options['isVip']!=undefined && options['isVip']!='True'){
				/*try{
					if(top.WindowsConfirm!=undefined && top.WindowsConfirm!=null 
					&& typeof(top.WindowsConfirm)=='function'){
						top.WindowsConfirm('你需要购买VIP会员才能继续答题,是否立即购买VIP？',function(isOK){
							if(isOK){window.open('Index.aspx?to=service');return false;}
						});
					}else if(window.confirm!=undefined && window.confirm!=null 
					&& typeof(window.confirm)=='function'){
						if(window.confirm('你需要购买VIP会员才能继续答题,是否立即购买VIP？')){
							window.open('Index.aspx?to=service');return false;
						};
					};
					return false;
				}catch(err){};
				return false;*/
			};
			/******************************************************************
			*验证答题卡信息是否存在
			*******************************************************************/
			Operate["SelectedIndex"] = parseInt(SelectedIndex);
			Operate['thisOption'] = arrList['items'][SelectedIndex] || {};
			/******************************************************************
			*验证答题信息的合法性
			*******************************************************************/
			if(Operate['thisOption']==undefined){alert('加载答卷试题错误！');return false;}
			else if(Operate['thisOption']==null){alert('加载答卷试题错误！');return false;}
			else if(typeof(Operate['thisOption'])!='object'){alert('加载答卷试题错误！');return false;}
			else if(Operate['thisOption']['strTitle']==undefined){alert('无法加载试题标题！');return false;}
			else if(Operate['thisOption']['strTitle']==null){alert('无法加载试题标题！');return false;}
			else if(Operate['thisOption']['strTitle']==""){alert('无法加载试题标题！');return false;}
			else if(Operate['thisOption']['QuestionMode']==undefined){alert('无法加载试题类型！');return false;}
			else if(Operate['thisOption']['QuestionMode']==null){alert('无法加载试题类型！');return false;}
			else if(Operate['thisOption']['QuestionMode']==""){alert('无法加载试题类型！');return false;}
			else if(Operate['thisOption']['AnswerText']==undefined){alert('无法加载试题答案！');return false;}
			else if(Operate['thisOption']['AnswerText']==null){alert('无法加载试题答案！');return false;}
			else if(Operate['thisOption']['AnswerText']==""){alert('无法加载试题答案！');return false;}
			else if(Operate['thisOption']['strOptions']==undefined){alert('无法加载试题选项！');return false;}
			else if(Operate['thisOption']['strOptions']==null){alert('无法加载试题选项！');return false;}
			else if(Operate['thisOption']['QuestionMode']!="判断" 
			&& Operate['thisOption']['strOptions']==""){alert('无法加载试题选项！');return false;}
			else if(Operate['thisOption']['QuestionMode']=="判断" 
			&& Operate['thisOption']['strOptions']==""){Operate['thisOption']['strOptions']='正确|错误';}
			/******************************************************************
			*生成数组,并且将数组乱序显示出来
			*******************************************************************/
			var arrOptions = [];
			try{
				arrOptions = Operate['thisOption']['strOptions'].split("|") || [];
				arrOptions.sort(function(){ return 0.5 - Math.random();});
				Operate['arrOptions'] = arrOptions;
			}catch(err){}
			/******************************************************************
			*判断数组信息是否合法
			*******************************************************************/
			if(Operate['arrOptions']==undefined){alert('加载试题选项失败,请重试!');return false;}
			else if(Operate['arrOptions']==null){alert('加载试题选项失败,请重试!');return false;}
			else if(Operate['arrOptions'].length<=0 && Operate['thisOption']['QuestionMode']!="判断")
			{alert('加载试题选项失败,请重试!');return false;}
			/******************************************************************
			*开始加载试题信息
			*******************************************************************/
			try{
				/******************************************************************
				*显示主题
				*******************************************************************/
				if(Operate['frmQuestionTitle']!=undefined && Operate['frmQuestionTitle']!=null
				&& Operate['thisOption']['strTitle']!=undefined && Operate['thisOption']['strTitle']!=null
				&& Operate['thisOption']['QuestionMode']!=undefined 
				&& Operate['thisOption']['QuestionMode']!=null)
				{
					try{
						var strTitle = "";
						strTitle += "【"+Operate['thisOption']['QuestionMode']+"】";
						strTitle += Operate['thisOption']['strTitle'];
						$(Operate['frmQuestionTitle']).html(strTitle);
					}catch(err){}
				}
				/******************************************************************
				*显示图片
				*******************************************************************/
				try{
					var strThumb = "";
					if(Operate['thisOption']['strThumb']!=undefined 
					&& Operate['thisOption']['strThumb']!=null 
					&& Operate['thisOption']['strThumb']!="")
					{
						try{
							strThumb += "<img alt=\"点击查看大图\" title=\"点击查看大图\"";
							strThumb +=" src=\""+Operate['thisOption']['strThumb']+"\"";
							strThumb +=" onclick=\"ShowThumb()\" />";
						}catch(err){}
					};
					$(Operate['frmQuestionThumb']).html(strThumb);
				}catch(err){}
				/******************************************************************
				*将上一题的我的答案消除,并且判断我上一次是否有答题，如果有则将我的答案贴上
				*******************************************************************/
				var thisAnswerValue = "";
				try{
					var thisAnswer = ReplaceKeywords(Operate['thisOption']['thisAnswer']) || "";
					if(Operate['thisOption']['QuestionMode']!="" 
					&& Operate['thisOption']['QuestionMode']!="判断"
					&& thisAnswer!=undefined && thisAnswer!="")
					{
						var ArrAnswer = thisAnswer.split('|');
						for(var i in arrOptions){
							for(var j in ArrAnswer){
								if(ArrAnswer[j]==arrOptions[i])
								{
									var abcText = "";
									if(i==0){abcText="A";}
									else if(i==1){abcText="B";}
									else if(i==2){abcText="C";}
									else if(i==3){abcText="D";}
									else if(i==4){abcText="E";}
									else if(i==5){abcText="F";}
									if(thisAnswerValue!=""){thisAnswerValue=thisAnswerValue+abcText;}
									else{thisAnswerValue=abcText;}
								}
							}
						}
					}else{thisAnswerValue = thisAnswer;}
					/******************************************************************
					*复制正确答案
					*******************************************************************/
					try{$(Operate['frmMAtext']).html(thisAnswerValue);}
					catch(err){}
				}catch(err){alert('加载答案选项错误,请重试!');return false;}
				
				/******************************************************************
				*计算正确答案信息,以及我回答的答案选项信息
				*******************************************************************/
				try{
					var AnswerText = Operate['thisOption']['AnswerText'] || "";
					var AnswerValue = "";
					if(Operate['thisOption']['QuestionMode']=="判断")
					{
						try{
							if(AnswerText!="正确" && AnswerText!="对"){AnswerValue="错";}
							else{AnswerValue = "对";}
						}catch(err){}
					}
					else if(Operate['thisOption']['QuestionMode']!="判断")
					{
						try{
							var ArrAnswer = AnswerText.split("|");
							for(var i in arrOptions){
								for(var j in ArrAnswer){
									if(ArrAnswer[j]==arrOptions[i])
									{
										var abcText = "";
										if(i==0){abcText="A";}
										else if(i==1){abcText="B";}
										else if(i==2){abcText="C";}
										else if(i==3){abcText="D";}
										else if(i==4){abcText="E";}
										else if(i==5){abcText="F";}
										
										if(AnswerValue!=""){AnswerValue=AnswerValue+abcText;}
										else{AnswerValue=abcText;}
									}	
								}
							}
						}catch(err){}
					};
					/******************************************************************
					*将正确答案复制到当前的选项卡当中
					*******************************************************************/
					try{Operate['thisOption']['abcText']=AnswerValue;}
					catch(err){alert('设置答案信息失败');return false;}
				}catch(err){}
				/******************************************************************
				*生成答题选项,判断题将不显示选项
				*******************************************************************/
				var strTemplate = "";
				strTemplate +="<div id=\"frmList\" operate=\"optionsList\">";
				if(Operate['thisOption']['QuestionMode']!=undefined 
				&& Operate['thisOption']['QuestionMode']!=null 
				&& Operate['thisOption']['QuestionMode']!="\u5224\u65ad")
				{
					
					for(var n in arrOptions)
					{
						var autoText = GetAbcText(n);
						/*******************************************************************
						*开始加载选项题
						********************************************************************/
						strTemplate +="<label abs=\""+autoText+"\" id=\"label"+autoText+"\"";
						strTemplate +=" ians=\"under\" class=\"items\">";
						strTemplate +="<input name=\"autoABCD\" id=\"auto"+autoText+"\" class=\"autoABCD\"";
						if(Operate['thisOption']['QuestionMode']!="\u591a\u9009")
						{strTemplate+=" type=\"radio\"";}
						else{strTemplate+=" type=\"checkbox\"";}
						strTemplate +=" iAns=\"under\" abs=\""+autoText+"\" ";
						strTemplate +=" value=\""+autoText+"\" operate=\"autoMul\" />";
						strTemplate +="<span class=\"text\">"+arrOptions[n]+"</span>";
						strTemplate +="</label>";
					};
				}
				else if(Operate['thisOption']['QuestionMode']!=undefined 
				&& Operate['thisOption']['QuestionMode']!=null 
				&& Operate['thisOption']['QuestionMode']=="\u5224\u65ad")
				{
					/******************************************************************
					*显示正确选项
					*******************************************************************/
					try{
						strTemplate +="<label id=\"labelA\" ians=\"under\" ";
						strTemplate +=" abs=\"A\" class=\"items\" >";
						strTemplate +="<input id=\"autoTrue\" class=\"autoJudge\" type=\"radio\"";
						strTemplate +=" name=\"autoJudge\" iAns=\"under\" abs=\"A\" ";
						strTemplate +=" value=\"\u5bf9\" operate=\"autoMul\" />";
						strTemplate +="<span style=\"font-size:20px;font-family:Geneva, Arial, Helvetica\" class=\"text\">\u221a</span>";
						strTemplate +="</label>";
					}catch(err){}
					/******************************************************************
					*显示错误选项
					*******************************************************************/
					try{
						strTemplate +="<label id=\"labelB\" ians=\"under\" ";
						strTemplate +=" abs=\"B\" class=\"items\" >";
						strTemplate +="<input name=\"autoJudge\" id=\"autoFalse\" ";
						strTemplate +=" class=\"autoJudge\" type=\"radio\""
						strTemplate +=" iAns=\"under\" abs=\"B\" value=\"\u9519\" ";
						strTemplate +=" operate=\"autoMul\" />";
						strTemplate +="<span style=\"font-size:20px;font-family:Geneva, Arial, Helvetica\" class=\"text\">\u0026\u0074\u0069\u006d\u0065\u0073\u003b</span>";
						strTemplate +="</label>";
					}catch(err){}
				};
				strTemplate +="</div>";
				/******************************************************************
				*将Html字符转换成DOM对象
				*******************************************************************/
				var frmTemplate = $(strTemplate)[0]; 
				/******************************************************************
				*定义选项事件信息,多选单选判断
				*******************************************************************/
				if(frmTemplate!=undefined && frmTemplate!=null 
				&& Operate['thisOption']['QuestionMode']!="" 
				&& Operate['thisOption']['QuestionMode']=="\u591a\u9009")
				{
					try{
						$(frmTemplate).find("input[operate=\"autoMul\"]").click(function(){
							if(Operate['mp3Button']!=undefined && Operate['mp3Button'].value!="" 
							&& GetPlayerValue()!='关闭语音' && Operate['thisOption']['thisAnswer']=="")
							{
								var thisValue = this.value;	
								if(thisValue!=undefined && thisValue=="A"){MultipleA(true);}
								else if(thisValue!=undefined && thisValue=="B"){MultipleB(true);}
								else if(thisValue!=undefined && thisValue=="C"){MultipleC(true);}
								else if(thisValue!=undefined && thisValue=="D"){MultipleD(true);}	
							}else{this.checked = false;}
						});	
					}catch(err){}
				}else if(frmTemplate!=undefined && frmTemplate!=null 
				&& Operate['thisOption']['QuestionMode']!="" 
				&& Operate['thisOption']['QuestionMode']=="\u5355\u9009")
				{
					try{
						$(frmTemplate).find("input[operate=\"autoMul\"]").click(function(){
							if(Operate['mp3Button']!=undefined && Operate['mp3Button'].value!="" 
							&& GetPlayerValue()!='关闭语音' && Operate['thisOption']['thisAnswer']=="")
							{
								var thisValue = this.value;	
								if(thisValue!=undefined && thisValue=="A"){SingleA();}
								else if(thisValue!=undefined && thisValue=="B"){SingleB();}
								else if(thisValue!=undefined && thisValue=="C"){SingleC();}
								else if(thisValue!=undefined && thisValue=="D"){SingleD();}	
							}else{this.checked = false;}
						});	
					}catch(err){}
				}else if(frmTemplate!=undefined && frmTemplate!=null 
				&& Operate['thisOption']['QuestionMode']!="" 
				&& Operate['thisOption']['QuestionMode']=="\u5224\u65ad"
				&& thisAnswer!=undefined && thisAnswer!=null && thisAnswer=="")
				{
					/*************************************************************
					*勾选正确答案
					**************************************************************/
					$(frmTemplate).find("#autoTrue").click(function(){
						if(Operate['mp3Button']!=undefined && Operate['mp3Button'].value!="" 
						&& GetPlayerValue()!='关闭语音' && Operate['thisOption']['thisAnswer']=="")
						{
							try{VerificationSelectionJudge("对");}catch(err){}
						}
					});	
					/*************************************************************
					*勾选错误答案
					**************************************************************/
					$(frmTemplate).find("#autoFalse").click(function(){
						if(Operate['mp3Button']!=undefined && Operate['mp3Button'].value!="" 
						&& GetPlayerValue()!='关闭语音' && Operate['thisOption']['thisAnswer']=="")
						{
							try{VerificationSelectionJudge("错");}catch(err){}
						}
					});	
				};
				
				/******************************************************************
				*判断是否为多选题,否则将提交答案按钮关闭
				*******************************************************************/
				try{
					if(Operate['thisOption']['QuestionMode']!=undefined 
					&& Operate['thisOption']['QuestionMode']!=null 
					&& Operate['thisOption']['QuestionMode']!="\u591a\u9009")
					{
						$("#FrmOkDiv").hide();
					}
					else if(Operate['thisOption']['QuestionMode']!=undefined 
					&& Operate['thisOption']['QuestionMode']!=null 
					&& Operate['thisOption']['QuestionMode']=="\u591a\u9009"){
						$("#FrmOkDiv").show();	
					};
				}catch(err){}
				/******************************************************************
				*将答案选项赋值到控件当中
				*******************************************************************/
				$(Operate['frmQuestionOption']).html(frmTemplate);
				/***************************************************************
				*设置已选中的数据信息
				****************************************************************/
				try{
					if(Operate['thisOption']['thisAnswer']!=undefined 
					&& Operate['thisOption']['thisAnswer']!=null 
					&& Operate['thisOption']['thisAnswer']!="" 
					&& Operate['thisOption']['abcText']!=undefined 
					&& Operate['thisOption']['abcText']!=null 
					&& Operate['thisOption']['abcText']!="" 
					&& Operate['thisOption']['QuestionMode']=="单选")
					{
						try{MarkerSingle(Operate['thisOption']['abcText'],thisAnswerValue);}
						catch(err){}
					}
					else if(Operate['thisOption']['thisAnswer']!=undefined 
					&& Operate['thisOption']['thisAnswer']!=null 
					&& Operate['thisOption']['thisAnswer']!="" 
					&& Operate['thisOption']['abcText']!=undefined 
					&& Operate['thisOption']['abcText']!=null 
					&& Operate['thisOption']['abcText']!="" 
					&& Operate['thisOption']['QuestionMode']=="多选")
					{
						try{MarkerMultiple(Operate['thisOption']['abcText'],thisAnswerValue);}
						catch(err){}
					}
					else if(Operate['thisOption']['thisAnswer']!=undefined 
					&& Operate['thisOption']['thisAnswer']!=null 
					&& Operate['thisOption']['thisAnswer']!="" 
					&& Operate['thisOption']['abcText']!=undefined 
					&& Operate['thisOption']['abcText']!=null 
					&& Operate['thisOption']['abcText']!="" 
					&& Operate['thisOption']['QuestionMode']=="判断")
					{
						try{MarkerJudge(Operate['thisOption']['abcText'],thisAnswerValue);}
						catch(err){}
					};
				}catch(err){}
				
				/******************************************************************
				*判断当前试题的收藏信息
				*******************************************************************/
				try{
					if(Operate['thisOption']['isFav']=="1"){$(Operate['FrmFavAbs']).attr("fav","true");}
					else{$(Operate['FrmFavAbs']).attr("fav","false");}	
				}catch(err){}
				
				/******************************************************************
				*加载考题分析
				*******************************************************************/
				try{
					if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
					&& typeof(Operate['thisOption'])=='object' 
					&& Operate['thisOption']['officialText']!=undefined 
					&& Operate['thisOption']['officialText']!=null 
					&& Operate['thisOption']['officialText']!="")
					{
						$("#frmFenxiText").html(Operate['thisOption']['officialText']);
					}
				}catch(err){}
				/******************************************************************
				*判断是否需要关闭考题分析
				*******************************************************************/
				if(GetOfficialValue()!="关闭分析"){
					try{CloseOfficial();}catch(err){}
				}
				/******************************************************************
				*判断是否需要开启语音
				*******************************************************************/
				if(GetPlayerValue()=="关闭语音"){
					try{ShowPlayer();}catch(err){}
				};
				/******************************************************************
				*将当前试题设置为选中模式,并且将滚动条跳转到此处
				*******************************************************************/
				try{GotoPaint();}catch(err){}
				try{GotoProcess();}catch(err){}
				/******************************************************************
				*保存当前答题记录信息
				*******************************************************************/
				if(options!=undefined && options!=null && typeof(options)=='object' 
				&& options['channel']!=undefined && options['channel']=='\u987a\u5e8f\u7ec3\u4e60')
				{
					try{CookieRecord((parseInt(SelectedIndex)+1));}catch(err){}
				}
				
			}catch(err){}
			/******************************************************************
			*加载动画数据信息
			*******************************************************************/
			var timeout = setTimeout(function(){
				try{$("#frmQuestionContianer").animate({"left":"0px"},400);}catch(err){};
				try{$("#frmLoadMaster").hide();}catch(err){}
				try{clearTimeout(timeout);}catch(err){}
			},500);
			
		};
		/********************************************************************************************
		*将当前试题设置为选中模式,并且将滚动条跳转到此处
		*********************************************************************************************/
		var GotoPaint = function()
		{
			try{
				var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
				SelectedIndex=SelectedIndex+1;
				/************************************************************************************
				*标注试题选中状态
				*************************************************************************************/
				try{
					var rowsIndex = parseInt(Math.floor((SelectedIndex/6)));
					var scrollHeight = parseInt($(Operate['frmOptionsTabs']).outerHeight());
					var gotoHeight = parseInt(((rowsIndex-1)*43));
					if(gotoHeight<=0){gotoHeight=0;}
					if(gotoHeight>=scrollHeight){gotoHeight=scrollHeight;}
					$(Operate['frmTabs']).scrollTop(gotoHeight);
				}catch(err){}
				/************************************************************************************
				*标注试题选中状态
				*************************************************************************************/
				try{
					if(document.querySelector('#frmCurrent')){
						$(document.querySelector('#frmCurrent')).removeAttr('id');	
					};
					$("#frmTabs").find("td[value=\""+SelectedIndex+"\"]").attr("id","frmCurrent");
				}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*显示考题分析信息
		*********************************************************************************************/
		var ShowOfficial = function()
		{
			try{SetOfficialValue("关闭分析");}catch(err){}
			try{$("#frmFenxi").show();}catch(err){}
		};
		/********************************************************************************************
		*关闭考题分析信息
		*********************************************************************************************/
		var CloseOfficial = function(){
			try{SetOfficialValue("考题分析");}catch(err){}
			try{$("#frmFenxi").hide();}catch(err){}
		};
		/********************************************************************************************
		*获取考题分析文本信息
		*********************************************************************************************/
		var GetOfficialValue = function()
		{
			var strValue = "";
			if(Operate['frmOfficial']!=undefined && Operate['frmOfficial']!=null)
			{
				try{strValue = $(Operate['frmOfficial']).attr("value") || "考题分析";}
				catch(err){}
				try{
					if(strValue==undefined){strValue="考题分析";}
					else if(strValue==null){strValue="考题分析";}
					else if(strValue==""){strValue="考题分析";}
				}catch(err){}
			}
			return strValue;
		};
		/********************************************************************************************
		*设置考题分析文本信息
		*********************************************************************************************/
		var SetOfficialValue = function(strValue)
		{
			if(Operate['frmOfficial']!=undefined && Operate['frmOfficial']!=null 
			&& strValue!=undefined && strValue!=null && strValue!="")
			{
				try{$(Operate['frmOfficial']).attr("value",strValue);}
				catch(err){}
				try{$("#frmOffName").html(strValue);}catch(err){}
			};
		};
		/********************************************************************************************
		*获取到开启Mp3的按钮文本信息
		*********************************************************************************************/
		var GetPlayerValue = function()
		{
			var strValue = "";
			if(Operate['mp3Button']!=undefined && Operate['mp3Button']!=null)
			{
				try{strValue = $(Operate['mp3Button']).attr("value") || "语音讲解";}
				catch(err){}
				try{
					if(strValue==undefined){strValue="语音讲解";}
					else if(strValue==null){strValue="语音讲解";}
					else if(strValue==""){strValue="语音讲解";}
				}catch(err){}
			}
			return strValue;
		};
		/********************************************************************************************
		*设置开启Mp3的按钮文本信息
		*********************************************************************************************/
		var SetPlayerValue = function(strValue)
		{
			if(Operate['mp3Button']!=undefined && Operate['mp3Button']!=null 
			&& strValue!=undefined && strValue!=null && strValue!="")
			{
				try{$(Operate['mp3Button']).attr("value",strValue);}
				catch(err){}
			}
		}
		/********************************************************************************************
		*开启Mp3
		*********************************************************************************************/
		var ShowPlayer = function()
		{
			try{
				var falseNumber = parseInt(Operate['FalseNumber']) || 0;
				var isVip = cfg['isVip'] || "False";
				if(isVip!="True" && falseNumber>=6){
					try{ShowCustomer();return false;}catch(err){};
					return false;	
				}
				else if(isVip=="True"){
					try{
						if(Operate['thisOption']!=undefined 
						&& Operate['thisOption']!=null 
						&& typeof(Operate['thisOption'])=='object' 
						&& Operate['frmPlayer']!=undefined 
						&& Operate['frmPlayer']!=null
						&& Operate['thisOption']['strMP3']!=undefined 
						&& Operate['thisOption']['strMP3']!=null 
						&& Operate['thisOption']['strMP3']!="")
						{
							try{$(Operate['frmPlayer']).attr("src",Operate['thisOption']['strMP3']);}
							catch(err){}
							try{$(Operate['frmPlayer'])[0].play();}catch(err){}
							try{ShowTechniques();}catch(err){}
							try{SetPlayerValue('关闭语音');}catch(err){}
						}
						else{alert('加载语音失败,请重试!');return false;};
					}catch(err){}			
				}
			}catch(err){}
		};
		/********************************************************************************************
		*关闭Mp3
		*********************************************************************************************/
		var ClosePlayer = function()
		{
			try{
				if(Operate['frmPlayer']!=undefined 
				&& Operate['frmPlayer']!=null)
				{
					try{Operate['frmPlayer'].pause();}catch(err){}
					try{CloseTechniques();}catch(err){}
					try{SetPlayerValue('语音讲解');}catch(err){}
				}
			}catch(err){alert(err.message);return false;}
		};
		/********************************************************************************************
		*开启答题技巧
		*********************************************************************************************/
		var ShowTechniques = function(){
			try{
				if(Operate['thisOption']!=undefined 
				&& Operate['thisOption']!=null 
				&& typeof(Operate['thisOption'])=='object' 
				&& Operate['frmTechniques']!=undefined 
				&& Operate['frmTechniques']!=null
				&& Operate['thisOption']['strTechniques']!=undefined 
				&& Operate['thisOption']['strTechniques']!=null 
				&& Operate['thisOption']['strTechniques']!="" 
				&& Operate['frmTechThumb']!=undefined && Operate['frmTechThumb']!=null)
				{
					$(Operate['frmTechThumb']).attr("src",Operate['thisOption']['strTechniques']);
					$(Operate['frmTechMaster']).show();
				}
				else{alert('当前试题没有分析!');return false;};	
			}catch(err){}	
		};
		/********************************************************************************************
		*关闭答题技巧
		*********************************************************************************************/
		var CloseTechniques = function()
		{
			try{
				if(Operate['frmTechMaster']!=undefined 
				&& Operate['frmTechMaster']!=null)
				{
					$(Operate['frmTechMaster']).hide();
				}
			}catch(err){}
		};
		/********************************************************************************************
		*渲染答题卡选项个数信息
		*********************************************************************************************/
		var renderOptions = function()
		{
			var frmTemplate = "";
			try{
				frmTemplate += "<table width=\"100%\" id=\"frmOptionsTabs\" cellpadding=\"3\" cellspacing=\"3\">";
				frmTemplate += "<tr class=\"hback\">";
				var Spaction = 6;
				var SelectedIndex = 1;var Length = parseInt(arrList['items'].length) || 0;
				try{
					$(arrList['items']).each(function(k,json){
						k=k+1;
						frmTemplate +="<td value=\""+k+"\"";
						if(json['isTrue']=='1'){frmTemplate +=" right=\"true\"";}
						else if(json['isTrue']=='0'){frmTemplate +=" right=\"false\"";}
						frmTemplate +=" operate=\"dbclick\">";
						frmTemplate +=""+k+"";
						frmTemplate +="</td>";
						if(SelectedIndex>=Spaction){frmTemplate+="</tr><tr class=\"hback\">";SelectedIndex=1;}
						else{SelectedIndex=SelectedIndex+1;}								  
					});
				}catch(err){}
				/********************************************************************************************
				*数据补位,将空的TD补足
				*********************************************************************************************/
				if(Length!=undefined && Length!=0 && Spaction!=0 
				&& (Length % Spaction) !=0)
				{
					try{
						for(var j=0;j<(Spaction-(Length % Spaction));j++)
						{frmTemplate+="<td></td>";}
					}catch(err){}
				}
				/********************************************************************************************
				*输出表格TR信息
				*********************************************************************************************/
				frmTemplate += "</tr>";
				frmTemplate += "</table>";
			}catch(err){}
			/********************************************************************************************
			*计算首正率
			*********************************************************************************************/
			try{CalculationAnswer();}catch(err){}
			/********************************************************************************************
			*将内容复制到控件中
			*********************************************************************************************/
			if(Operate['frmTabs']!=undefined && Operate['frmTabs']!=null 
			&& Operate['frmTabsMaster']!=undefined && Operate['frmTabsMaster']!=null)
			{
				try{$(Operate['frmTabs']).html(frmTemplate);}catch(err){};
				/********************************************************************************************
				*增加选项卡点击事件
				*********************************************************************************************/
				try{
					$(Operate['frmTabs']).find("td[operate=\"dbclick\"]").click(function(){
						var SelectedIndex = parseInt($(this).attr("value")) || 0;
						if(SelectedIndex!=undefined && SelectedIndex!=null && SelectedIndex!=0)
						{
							GotoKaoshi(SelectedIndex);	
						}
					});	
				}catch(err){}
			};
		};
		/********************************************************************************************
		*渲染控件进度条信息
		*********************************************************************************************/
		var GotoProcess = function()
		{
			try{
				var thisLength = parseInt(arrList['items'].length) || 0;
				var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
				SelectedIndex=SelectedIndex+1;
				if(SelectedIndex<=0){SelectedIndex=0;}
				else if(SelectedIndex>=thisLength){SelectedIndex=thisLength;}
				$("#frmProcess").html(SelectedIndex+"/"+thisLength);
			}catch(err){}
		};
		/********************************************************************************************
		*关闭错题弹出菜单信息
		*********************************************************************************************/
		var closeEmpty = function()
		{
			try{
				$(Operate['frmEmpty']).hide();
				$(Operate['FrmEmptyButton']).attr("isHide","false");		
			}catch(err){}
		};
		/********************************************************************************************
		*打开错题弹出菜单
		*********************************************************************************************/
		var openEmpty = function()
		{
			try{
				$(Operate['frmEmpty']).show();
				$(Operate['FrmEmptyButton']).attr("isHide","true");		
			}catch(err){}
		}
		/********************************************************************************************
		*定义渲染控件
		*********************************************************************************************/
		var render = function()
		{
			var strTemplate = "";
			strTemplate += "<div operate=\"\" id=\"Kscontianer\">";
			/***************************************************************************************
			*选项栏以及图片栏
			****************************************************************************************/
			strTemplate += "<div id=\"frmQuestionContianer\">";
			strTemplate += "<div id=\"frmQuestion\">";
			strTemplate += "<div id=\"frmQuestionTitle\"></div>";
			strTemplate += "<div id=\"frmQuestionThumb\"></div>";
			strTemplate += "<div id=\"frmQuestionOption\"></div>";
			strTemplate += "</div>";
			strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:1px;\"></div>";
			strTemplate += "<div id=\"FrmOkDiv\">";
			strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:5px;\"></div>";
			strTemplate += "<input class=\"button\" type=\"button\" id=\"FrmOk\" value=\"提交答案\">";
			strTemplate += "</div>";
			strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:5px;\"></div>";
			strTemplate += "<div style=\"display:none\" id=\"frmFenxi\">";
			strTemplate += "<div id=\"frmFenxiTitle\"></div>";
			strTemplate += "<div id=\"frmFenxiText\"></div>";
			strTemplate += "</div>";
			strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
			strTemplate += "</div>";
			/***************************************************************************************
			*答题卡
			****************************************************************************************/
			strTemplate += "<div id=\"frmButtons\">";
			strTemplate += "<div id=\"FrmUpper\"><span class=\"name\">上一题</span></div>";
			strTemplate += "<div id=\"frmOptionsBtn\"><span id=\"frmProcess\" class=\"name\">0</span></div>";
			strTemplate += "<div value=\"语音讲解\" id=\"mp3Button\"><span class=\"name\">技巧讲解</span></div>";
			strTemplate += "<div value=\"考题分析\" id=\"frmOfficial\"><span id=\"frmOffName\" class=\"name\">考题分析</span></div>";
			strTemplate += "<div id=\"FrmNext\"><span class=\"name\">下一题</span></div>";
			strTemplate += "</div>";
			/**********************************************************************************
			*播放语音控件
			***********************************************************************************/
			strTemplate += "<audio title=\"播放语音控件\" autoplay id=\"frmPlayer\"></audio>";
			strTemplate += "<audio title=\"播放语音控件\" autoplay id=\"frmRemind\"></audio>";
			/**********************************************************************************
			*答题控件遮罩层
			***********************************************************************************/
			strTemplate += "<div title=\"答题技巧遮罩层\" id=\"frmTechMaster\">";
			strTemplate += "<div title=\"答题技巧控件\" id=\"frmTechniques\">";
			strTemplate += "<div id=\"frmTechTitle\">技巧讲解</div>";
			strTemplate += "<img id=\"frmTechThumb\"/>";
			strTemplate += "<div id=\"frmTechClose\">关闭</div>";
			strTemplate += "</div>";
			strTemplate += "</div>";
			/**********************************************************************************
			*提示正在加载信息
			***********************************************************************************/
			strTemplate += "<div title=\"答题技巧遮罩层\" id=\"frmLoadMaster\">";
			strTemplate += "<div title=\"正在载入\" id=\"frmLoad\">";
			strTemplate += "<div><img style=\"width:40px\" src=\"template/images/load/load.gif\"/></div>";
			strTemplate += "<div style=\"font-size:0px;height:10px;clear:both;width:100%\"></div>";
			strTemplate += "<div>正在载入,请稍后...</div>";
			strTemplate += "</div>";
			strTemplate += "</div>";
			
			/**********************************************************************************
			*保存答题信息提示
			***********************************************************************************/
			strTemplate += "<div title=\"保存答题信息提示\" id=\"frmSaveMaster\">";
			strTemplate += "<div title=\"正在保存\" id=\"frmSaveContianer\">";
			strTemplate += "<div><img style=\"width:40px\" src=\"template/images/load/load.gif\"/></div>";
			strTemplate += "<div style=\"font-size:0px;height:10px;clear:both;width:100%\"></div>";
			strTemplate += "<div>正在保存答题信息,请等待...</div>";
			strTemplate += "</div>";
			strTemplate += "</div>";
			/**********************************************************************************
			*显示答题卡选项信息
			***********************************************************************************/
			strTemplate += "<div id=\"frmTabsMaster\">";
			strTemplate += "<div id=\"frmTabsTools\">";
			strTemplate += "<div id=\"FrmTrueAbs\">对0</div>";
			strTemplate += "<div id=\"FrmFalseAbs\">错0</div>";
			strTemplate += "<div id=\"FrmUnderAbs\">未答"+(parseInt(arrList['items'].length) || 0)+"</div>";
			strTemplate += "<div id=\"FrmFavAbs\"></div>";
			strTemplate += "<div id=\"FrmEmptyAbs\">清空</div>";
			strTemplate += "</div>";
			strTemplate += "<div id=\"frmTabs\"></div>";
			strTemplate += "</div>";
			/**********************************************************************************
			*结束考题答题界面
			***********************************************************************************/
			strTemplate += "</div>";
			
			$(contianer).html(strTemplate);
			
			/***************************************************************************************
			*设置答题模块控件
			****************************************************************************************/
			Operate['frmQuestionTitle'] = document.querySelector("#frmQuestionTitle") || null;
			Operate['frmQuestionOption'] = document.querySelector("#frmQuestionOption") || null;
			Operate['frmQuestionThumb'] = document.querySelector("#frmQuestionThumb") || null;
			/***************************************************************************************
			*设置答题选择项信息
			****************************************************************************************/
			Operate['FrmOk'] = document.querySelector("#FrmOk") || null;
			Operate['FrmUpper'] = document.querySelector("#FrmUpper") || null;
			Operate['FrmNext'] = document.querySelector("#FrmNext") || null;
			/***************************************************************************************
			*设置选项卡控件
			****************************************************************************************/
			Operate['frmTabsMaster'] = document.querySelector("#frmTabsMaster") || null;
			Operate['frmTabsTools'] = document.querySelector("#frmTabsTools") || null;
			Operate['FrmTrueAbs'] = document.querySelector("#FrmTrueAbs") || null;
			Operate['FrmFalseAbs'] = document.querySelector("#FrmFalseAbs") || null;
			Operate['FrmUnderAbs'] = document.querySelector("#FrmUnderAbs") || null;
			Operate['FrmFavAbs'] = document.querySelector("#FrmFavAbs") || null;
			Operate['FrmEmptyAbs'] = document.querySelector("#FrmEmptyAbs") || null;
			Operate['frmTabs'] = document.querySelector("#frmTabs") || null;
			/***************************************************************************************
			*显示进度条
			****************************************************************************************/
			Operate['frmProcess'] = document.querySelector("#frmProcess") || null;
			/***************************************************************************************
			*答题技巧
			****************************************************************************************/
			Operate['frmTechMaster'] = document.querySelector("#frmTechMaster") || null;
			Operate['frmTechniques'] = document.querySelector("#frmTechniques") || null;
			Operate['frmTechThumb'] = document.querySelector("#frmTechThumb") || null;
			Operate['frmTechClose'] = document.querySelector("#frmTechClose") || null;
			/***************************************************************************************
			*播放语音
			****************************************************************************************/
			Operate['frmPlayer'] = document.querySelector("#frmPlayer") || null;
			Operate['frmRemind'] = document.querySelector("#frmRemind") || null;
			
			/***************************************************************************************
			*考题分析,显示考题分析的控件
			****************************************************************************************/
			Operate['frmOfficial'] = document.querySelector("#frmOfficial") || null;
			Operate['frmOfficialControls'] = document.querySelector("#frmOfficialControls") || null;
			/***************************************************************************************
			*错题处理信息
			****************************************************************************************/
			Operate['FrmEmptyButton'] = document.querySelector("#FrmEmptyButton") || null;
			Operate['frmEmpty'] = document.querySelector("#frmEmpty") || null;
			Operate['FrmEmptyThis'] = document.querySelector("#FrmEmptyThis") || null;
			Operate['FrmEmptyAll'] = document.querySelector("#FrmEmptyAll") || null;
			Operate['FrmEmptybox'] = document.querySelector("#FrmEmptybox") || null;
			/***************************************************************************************
			*设置为错题,显示答案
			****************************************************************************************/
			Operate['sErrButton'] = document.querySelector("#sErrButton") || null;
			Operate['showAnsButton'] = document.querySelector("#showAnsButton") || null;
			
			/***************************************************************************************
			*设置底部菜单导航卡信息
			****************************************************************************************/
			Operate['frmButtons'] = document.querySelector("#frmButtons") || null;
			Operate['FrmUpper'] = document.querySelector("#FrmUpper") || null;
			Operate['frmOptionsBtn'] = document.querySelector("#frmOptionsBtn") || null;
			Operate['mp3Button'] = document.querySelector("#mp3Button") || null;
			Operate['frmOfficial'] = document.querySelector("#frmOfficial") || null;
			Operate['FrmNext'] = document.querySelector("#FrmNext") || null;
			/***************************************************************************************
			*加载数据事件
			****************************************************************************************/
			try{
				if(Operate['frmTabs']!=undefined && Operate['frmTabs']!=null 
				&& Operate['frmTabsMaster']!=undefined && Operate['frmTabsMaster']!=null
				&& arrList!=undefined && arrList!=null && typeof(arrList)=='object' 
				&& arrList["items"].length>=0)
				{renderOptions();}
			}catch(err){}
			/***************************************************************************************
			*加载答题选项卡事件信息
			****************************************************************************************/
			try{
				if(Operate['frmTabsMaster']!=undefined && Operate['frmTabsMaster']!=null 
				&& Operate['frmOptionsBtn']!=undefined && Operate['frmOptionsBtn']!=null)
				{
					try{
						$(Operate['frmOptionsBtn']).click(function(){
							$(Operate['frmTabsMaster']).show();
							try{GotoPaint();}catch(err){}
						});
						$(Operate['frmTabsMaster']).click(function(){
							if(event.target!=undefined && event.target!=null 
							&& event.target==this){
								$(Operate['frmTabsMaster']).hide();	
							};										   
						});
					}catch(err){}
				};
			}catch(err){}
			/***************************************************************************************
			*清空按钮事件信息
			****************************************************************************************/
			if(Operate['FrmEmptyAbs']!=undefined && Operate['FrmEmptyAbs']!=null){
				try{
					$(Operate['FrmEmptyAbs']).click(function(){
						try{
							if(window.WindowsConfirm!=undefined && window.WindowsConfirm!=null 
							&& typeof(window.WindowsConfirm)=='function'){
								window.WindowsConfirm('你确定要清空当前的答题记录?',function(){
									if(cfg['channel']=="错题练习"){
										window.location='?action=clearwrong';	
									}else if(cfg['channel']=="收藏题练习"){
										window.location='?action=emptyFav';	
									}else if(cfg['channel']=="顺序练习"){
										try{EmptyPaperLogs();}catch(err){};	
									}else if(cfg['channel']=="专项练习"){
										try{EmptyPaperLogs();}catch(err){};	
									}else{window.location.reload();}
								},function(){return false;});	
							}else if(confirm('你确定要清空当前的答题记录?')){
								if(cfg['channel']=="错题练习"){
									window.location='?action=clearwrong';	
								}else if(cfg['channel']=="收藏题练习"){
									window.location='?action=emptyFav';	
								}else if(cfg['channel']=="顺序练习"){
									try{EmptyPaperLogs();}catch(err){};	
								}else if(cfg['channel']=="专项练习"){
									try{EmptyPaperLogs();}catch(err){};	
								}else{window.location.reload();}
							};	
						}catch(err){}
					});	
				}catch(err){}
			};
			/***************************************************************************************
			*关闭答题技巧事件信息
			****************************************************************************************/
			try{
				if(Operate['frmTabsMaster']!=undefined && Operate['frmTabsMaster']!=null 
				&& Operate['frmTechClose']!=undefined && Operate['frmTechClose']!=null)
				{
					$(Operate['frmTechClose']).click(function(){
						try{ClosePlayer();}catch(err){}							   
					});
				};
			}catch(err){}
			/***************************************************************************************
			*上一题按钮点击事件
			****************************************************************************************/
			if(Operate['FrmUpper']!=undefined && Operate['FrmUpper']!=null)
			{
				try{
					$(Operate['FrmUpper']).click(function(){
						try{
							var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
							if(SelectedIndex<=0){alert('当前已经是第一题!');return false;}
							else{SelectedIndex=SelectedIndex-1;}
							if(SelectedIndex<=0){SelectedIndex=0;}
							ShowKaoshi(SelectedIndex);
						}catch(err){}
					});	
				}catch(err){}
			};
			/***************************************************************************************
			*下一题按钮点击事件
			****************************************************************************************/
			if(Operate['FrmNext']!=undefined && Operate['FrmNext']!=null)
			{
				try{
					$(Operate['FrmNext']).click(function(){
						try{
							var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
							if(SelectedIndex>=arrList['items'].length)
							{alert('当前已经是最后一题!');return false;}
							else{SelectedIndex=SelectedIndex+1;}
							if(SelectedIndex>=arrList['items'].length){alert('当前已经是最后一题!');return false;}
							else{ShowKaoshi(SelectedIndex);}
						}catch(err){}
					});	
				}catch(err){}
			};
			/***************************************************************************************
			*显示考题分析
			****************************************************************************************/
			if(Operate['frmOfficial']!=undefined && Operate['frmOfficial']!=null)
			{
				try{
					$(Operate['frmOfficial']).click(function(){
						if(GetOfficialValue()=='考题分析'){ShowOfficial();}
						else{CloseOfficial();}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*开启语音
			****************************************************************************************/
			if(Operate['mp3Button']!=undefined && Operate['mp3Button']!=null)
			{
				try{
					$(Operate['mp3Button']).click(function(){
						try{
							var isVip = cfg['isVip'] || "False";
							var SelectionIndex = parseInt(Operate['SelectedIndex']) || 0;
							if(isVip!="True" && SelectionIndex>=6){
								try{ShowCustomer();return false;}catch(err){};
								return false;	
							}else{
								if(GetPlayerValue()=='语音讲解'){try{ShowPlayer();}catch(err){};}
								else{try{ClosePlayer();}catch(err){}}
							}
						}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*监听语音播放完成事件
			****************************************************************************************/
			if(Operate['frmPlayer']!=undefined && Operate['frmPlayer']!=null)
			{
				try{
					$(Operate['frmPlayer']).on("ended",function(){
						if(GetPlayerValue()!='语音讲解'){
							var timer = 3;
							var inter = setInterval(function(){
								if(timer>=0){timer=timer-1;}
								else{
									try{
										clearInterval(inter);
										if(GetPlayerValue()!='语音讲解')
										{ShowPlayer();}	
									}catch(err){}
								}
							},500);
						}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*设置收藏题
			****************************************************************************************/
			if(Operate['FrmFavAbs']!=undefined && Operate['FrmFavAbs']!=null)
			{
				try{
					$(Operate['FrmFavAbs']).click(function(){
						/*********************************************************************
						*获取当前试题是否已收藏
						**********************************************************************/
						var isFav =$(this).attr('fav') || "false";
						if(isFav==undefined){isFav="false";}
						else if(isFav==null){isFav="false";}
						else if(isFav==""){isFav="false";}
						/*********************************************************************
						*开始判断保存信息
						**********************************************************************/
						try{
							if(Operate["thisOption"]!=undefined 
							&& Operate["thisOption"]!=null 
							&& isFav!="true")
							{
								SaveFav(Operate["thisOption"],function(strMsg){
									if(strMsg!="success"){alert(strMsg);return false;}
									else{try{$(Operate['FrmFavAbs']).attr("fav","true");}catch(err){}};
								});
							}
							else if(Operate["thisOption"]!=undefined 
							&& Operate["thisOption"]!=null 
							&& isFav=="true")
							{
								DeleteFav(Operate["thisOption"],function(strMsg){
									if(strMsg!="success"){alert(strMsg);return false;}
									else{try{$(Operate['FrmFavAbs']).attr("fav","false");}catch(err){}};
								});
							};
						}catch(err){}
						/********************************************************************
						*设置当前试题可收藏信息
						*********************************************************************/
						try{
							var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
							if(SelectedIndex<=0){SelectedIndex=0;}
							else if(SelectedIndex>=arrList['items'].length)
							{SelectedIndex=arrList['items'].length;};
							if(arrList['items'][SelectedIndex]['isFav']=="1")
							{arrList['items'][SelectedIndex]['isFav']="0";}
							else{arrList['items'][SelectedIndex]['isFav']="1";}
						}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*多选题下选好了的按钮点击事件
			****************************************************************************************/
			if(Operate['FrmOk']!=undefined && Operate['FrmOk']!=null)
			{
				try{
					$(Operate['FrmOk']).click(function(){
						/*************************************************************
						*标注选项信息
						**************************************************************/
						var absText = GetThisAnswer();
						/*************************************************************
						*判断答题是否正确
						**************************************************************/
						if(absText==""){alert('未选择答案!');return false;}
						else if(absText.length<=0){alert('获取选中项信息失败!');return false;}
						else if(absText.length>=5){alert('获取选中项信息失败!');return false;}
						else{
							try{VerificationSelectionMultiple(absText);}catch(err){}	
						};
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*加载动画数据信息
			****************************************************************************************/
			try{$("#frmQuestionContianer").css({"left":"100%"});}catch(err){}
			try{$("#frmLoadMaster").hide();}catch(err){}
			/***************************************************************************************
			*加载默认试题信息
			****************************************************************************************/
			try{
				if(options['thisRecord']!=undefined && options['thisRecord']!=null 
				&& options['thisRecord']!=0 && options['thisRecord']!=1 
				&& !isNaN(parseInt(options['thisRecord'])) && WindowsConfirm!=undefined 
				&& WindowsConfirm!=null && typeof(WindowsConfirm)=='function')
				{
					try{
						WindowsConfirm("你上次练习到"+options['thisRecord']+"题,是否继续?",function(){
							var thisRecord = parseInt(options['thisRecord'])-1;
							if(thisRecord<=0){thisRecord=0;}
							ShowKaoshi(thisRecord);																	 
						},function(){
							try{EmptyPaperLogs();}catch(err){};	
						});
					}catch(err){}
				}else{try{ShowKaoshi(0);}catch(err){};}
			}catch(err){}
			/********************************************************************************************
			*监听网页返回事件
			*********************************************************************************************/
			try{
				var pushHistory = function(){ 
					window.history.pushState({"foo":"地址"}, "title", "#");
				};
				
				try{pushHistory(); }catch(err){}
				/***********************************************************************************
				*开始监听网页动作
				************************************************************************************/
				window.addEventListener("popstate", function(e) { 
					if(confirm('你确定要离开当前页面吗?')){
						try{
							SavePaper(function(){
								try{
									if(cfg!=undefined && cfg!=null && typeof(cfg)=='object' 
									&& cfg['Refre']!=undefined && cfg['Refre']!=null && cfg['Refre']!="")
									{window.location=cfg['Refre'];}
									else{window.location='Index.aspx';}
								}catch(err){}
							});
						}catch(err){};
						return false;
					}else{
						try{
							SavePaper(function(){
								try{pushHistory(); }catch(err){}
							});
						}catch(err){};
						return false;	
					};return false;
				}, false);
			}catch(err){}
			/********************************************************************************************
			*监听网页关闭事件
			*********************************************************************************************/
			/*window.addEventListener("beforeunload", function (e) {
				var confirmationMessage = '确定离开此页吗？本页不需要刷新或后退';
				(e || window.event).returnValue = confirmationMessage;     // Gecko and Trident
				return confirmationMessage;                         
		    });*/
			/********************************************************************************************
			*去掉微信的莫某一些炒作工具
			*********************************************************************************************/
			try{
				document.addEventListener('WeixinJSBridgeReady', function onBridgeReady() {
					WeixinJSBridge.call('hideToolbar');
					WeixinJSBridge.call('hideMenuItems');
					WeixinJSBridge.call('hideOptionMenu');
				});
			}catch(err){}
			
		};
		/********************************************************************************************
		*多选情况下获取我选择的答案信息
		*********************************************************************************************/
		var GetThisAnswer = function()
		{
			var absText = "";
			try{
				$(Operate['frmQuestionOption']).find("input[operate=\"autoMul\"]").each(function(){
					if(this.checked!=undefined && this.checked!=null && this.checked){
						if(absText!=""){absText=absText+this.value;}
						else{absText=this.value;}
					}																	
				});
			}catch(err){}
			return absText;
		};
		/********************************************************************************************
		*将我选择的答案信息保存记录
		*********************************************************************************************/
		var SetThisAnswer = function(absText)
		{
			try{Operate['thisOption']['thisAnswer'] = absText;}catch(err){}
		};
		/********************************************************************************************
		*答题卡选项按钮多选题选择A时的情况
		*********************************************************************************************/
		var MultipleA = function(iChecked)
		{
			
			/****************************************************************************************
			*标记我的答案信息
			*****************************************************************************************/
			try{
				/***************************************************************************************
				*判断题打钩信息
				****************************************************************************************/
				var autoChecked = document.querySelector('#autoA');
				if(autoChecked!=undefined && autoChecked!=null && iChecked!=true)
				{autoChecked.checked = !autoChecked.checked;};
				/***************************************************************************************
				*标注当前选项卡的信息
				****************************************************************************************/
				if(autoChecked.checked)
				{$("#frmList").find("label[id=\"labelA\"]").attr("selection","true");}
				else{$("#frmList").find("label[id=\"labelA\"]").attr("selection","false");}
			}catch(err){}
			/****************************************************************************************
			*设置我选择的答案信息
			*****************************************************************************************/
			//try{SetThisAnswer(GetThisAnswer());}
			//catch(err){}
		};
		/********************************************************************************************
		*单选题选择A时的情况
		*********************************************************************************************/
		var SingleA = function()
		{
			try{VerificationSelectionSingle("A");}
			catch(err){}
		};
		
		/********************************************************************************************
		*答题卡选项按钮多选题选择A时的情况
		*********************************************************************************************/
		var MultipleB = function(iChecked)
		{
			
			/****************************************************************************************
			*标记我的答案信息
			*****************************************************************************************/
			try{
				/***************************************************************************************
				*判断题打钩信息
				****************************************************************************************/
				var autoChecked = document.querySelector('#autoB');
				if(autoChecked!=undefined && autoChecked!=null && iChecked!=true)
				{autoChecked.checked = !autoChecked.checked;};
				/***************************************************************************************
				*标注当前选项卡的信息
				****************************************************************************************/
				if(autoChecked.checked)
				{$("#frmList").find("label[id=\"labelB\"]").attr("selection","true");}
				else{$("#frmList").find("label[id=\"labelB\"]").attr("selection","false");}
			}catch(err){}
			/****************************************************************************************
			*设置我选择的答案信息
			*****************************************************************************************/
			//try{SetThisAnswer(GetThisAnswer());}
			//catch(err){}
		};
		/********************************************************************************************
		*单选题选择A时的情况
		*********************************************************************************************/
		var SingleB = function()
		{
			try{VerificationSelectionSingle("B");}
			catch(err){}
		};
		
		/********************************************************************************************
		*答题卡选项按钮多选题选择A时的情况
		*********************************************************************************************/
		var MultipleC = function(iChecked)
		{
			/****************************************************************************************
			*标记我的答案信息
			*****************************************************************************************/
			try{
				/***************************************************************************************
				*判断题打钩信息
				****************************************************************************************/
				var autoChecked = document.querySelector('#autoC');
				if(autoChecked!=undefined && autoChecked!=null && iChecked!=true)
				{autoChecked.checked = !autoChecked.checked;};
				/***************************************************************************************
				*标注当前选项卡的信息
				****************************************************************************************/
				if(autoChecked.checked){$("#frmList").find("label[id=\"labelC\"]").attr("selection","true");}
				else{$("#frmList").find("label[id=\"labelC\"]").attr("selection","false");}
			}catch(err){alert(err.message);return false;}
			/****************************************************************************************
			*设置我选择的答案信息
			*****************************************************************************************/
			//try{SetThisAnswer(GetThisAnswer());}
			//catch(err){}
		};
		/********************************************************************************************
		*单选题选择A时的情况
		*********************************************************************************************/
		var SingleC = function()
		{
			try{VerificationSelectionSingle("C");}
			catch(err){}
		};
		
		/********************************************************************************************
		*答题卡选项按钮多选题选择A时的情况
		*********************************************************************************************/
		var MultipleD = function(iChecked)
		{
			/****************************************************************************************
			*标记我的答案信息
			*****************************************************************************************/
			try{
				/***************************************************************************************
				*判断题打钩信息
				****************************************************************************************/
				var autoChecked = document.querySelector('#autoD');
				if(autoChecked!=undefined && autoChecked!=null && iChecked!=true)
				{autoChecked.checked = !autoChecked.checked;};
				/***************************************************************************************
				*标注当前选项卡的信息
				****************************************************************************************/
				if(autoChecked.checked)
				{$("#frmList").find("label[id=\"labelD\"]").attr("selection","true");}
				else{$("#frmList").find("label[id=\"labelD\"]").attr("selection","false");}
			}catch(err){}
			/****************************************************************************************
			*设置我选择的答案信息
			*****************************************************************************************/
			//try{SetThisAnswer(GetThisAnswer());}
			//catch(err){}
		};
		/********************************************************************************************
		*单选题选择A时的情况
		*********************************************************************************************/
		var SingleD = function()
		{
			try{VerificationSelectionSingle("D");}
			catch(err){}
		};
		
		/****************************************************************************************
		*标记用户选择的答案和正确答案
		*****************************************************************************************/
		var MarkerSingle = function(trueAbs,absText)
		{
			/***********************************************************************
			*撤销用户选中的标记信息
			************************************************************************/
			try{
				$("#frmList").find("input[operate=\"autoMul\"]").attr("ians","under");
				$("#frmList").find("label[class=\"items\"]").attr("ians","under");
			}catch(err){}
			/***********************************************************************
			*标记用户选择的选项信息
			************************************************************************/
			try{
				if(trueAbs!=undefined && trueAbs!=null && trueAbs!="" 
				&& absText!=undefined && absText!=null && absText!="" 
				&& absText==trueAbs)
				{
					$(document.querySelector("#auto"+absText+"")).attr("ians","true");
					$(document.querySelector("#label"+absText+"")).attr("ians","true");
				}else if(trueAbs!=undefined && trueAbs!=null && trueAbs!="" 
				&& absText!=undefined && absText!=null && absText!="" 
				&& absText!=trueAbs)
				{
					$(document.querySelector("#auto"+absText+"")).attr("ians","false");
					$(document.querySelector("#label"+absText+"")).attr("ians","false");
					$(document.querySelector("#auto"+trueAbs+"")).attr("ians","true");
					$(document.querySelector("#label"+trueAbs+"")).attr("ians","true");
				}
			}catch(err){}
		};
		/********************************************************************************************
		*验证用户选择的答案信息,单项选择
		*********************************************************************************************/
		var VerificationSelectionSingle = function(SelectionABC)
		{
			/****************************************************************************************
			*设置我选择的答案信息
			*****************************************************************************************/
			try{SetThisAnswer(SelectionABC);}
			catch(err){}
			/****************************************************************************************
			*验证我的答案是否正确
			*****************************************************************************************/
			try
			{
				var absText = Operate['thisOption']['abcText'] || "";
				if(absText==undefined){absText="";}
				else if(absText==null){absText="";}
				try{
					if(absText!=undefined && absText!=null && absText!="" 
					&& SelectionABC!=undefined && SelectionABC!=null 
					&& SelectionABC!="" && SelectionABC==absText)
					{
						try{AnswerTrue(SelectionABC);}catch(err){}
					}else{
						try{AnswerFalse(SelectionABC);}catch(err){}
					}
				}catch(err){}
				/****************************************************************************************
				*标注选项当中的正确与错误试题信息
				*****************************************************************************************/
				try{MarkerSingle(absText,SelectionABC);}catch(err){}
				
			}catch(err){}
			/********************************************************************************************
			*计算首正率
			*********************************************************************************************/
			try{CalculationAnswer();}catch(err){}
		};
		
		/****************************************************************************************
		*标记用户选择的答案和正确答案
		*****************************************************************************************/
		var MarkerJudge = function(trueAbs,absText)
		{
			if(trueAbs!=undefined && trueAbs!=null && trueAbs!="" 
			&& absText!=undefined && absText!=null && absText!="" && absText==trueAbs)
			{
				if(trueAbs!="对" && trueAbs!="\u5bf9" && trueAbs!="正确"){
					$("#frmList").find("label[id=\"labelB\"]").attr("ians","true");
					$("#frmList").find("input[id=\"autoFalse\"]").attr("ians","true");	
				}else{
					$("#frmList").find("label[id=\"labelA\"]").attr("ians","true");
					$("#frmList").find("input[id=\"autoTrue\"]").attr("ians","true");	
				}
			}else if(trueAbs!=undefined && trueAbs!=null && trueAbs!="" 
			&& absText!=undefined && absText!=null && absText!="" && absText!=trueAbs)
			{
				if(trueAbs!="对" && trueAbs!="\u5bf9" && trueAbs!="正确")
				{
					$("#frmList").find("label[id=\"labelB\"]").attr("ians","true");
					$("#frmList").find("label[id=\"labelA\"]").attr("ians","false");
					$("#frmList").find("input[id=\"autoFalse\"]").attr("ians","true");	
					$("#frmList").find("input[id=\"autoTrue\"]").attr("ians","false");	
				}else{
					$("#frmList").find("label[id=\"labelB\"]").attr("ians","false");
					$("#frmList").find("label[id=\"labelA\"]").attr("ians","true");
					$("#frmList").find("input[id=\"autoFalse\"]").attr("ians","false");	
					$("#frmList").find("input[id=\"autoTrue\"]").attr("ians","true");	
				};	
			};
		};
		/********************************************************************************************
		*验证用户选择的答案信息,单项选择
		*********************************************************************************************/
		var VerificationSelectionJudge = function(JudgeText)
		{
			/****************************************************************************************
			*设置我选择的答案信息
			*****************************************************************************************/
			try{SetThisAnswer(JudgeText);}
			catch(err){}
			/****************************************************************************************
			*验证我的答案是否正确
			*****************************************************************************************/
			try
			{
				var absText = Operate['thisOption']['abcText'] || "";
				if(absText==undefined){absText="";}
				else if(absText==null){absText="";}
				if(Operate['thisOption']['abcText']!=undefined 
				&& Operate['thisOption']['abcText']!=null 
				&& Operate['thisOption']['abcText']!="" 
				&& JudgeText!=undefined && JudgeText!=null && JudgeText!="" 
				&& JudgeText==Operate['thisOption']['abcText'])
				{try{AnswerTrue(JudgeText);}catch(err){}}
				else{try{AnswerFalse(JudgeText);}catch(err){}}
				/****************************************************************************************
				*标注选项当中的正确与错误试题信息
				*****************************************************************************************/
				try{MarkerJudge(absText,JudgeText);}catch(err){}
				
			}catch(err){}
			/********************************************************************************************
			*计算首正率
			*********************************************************************************************/
			try{CalculationAnswer();}catch(err){}
		};
		/****************************************************************************************
		*标记用户的选择答案和正确答案 trueAbs=正确答案,absText=选择答案
		*****************************************************************************************/
		var MarkerMultiple = function(trueAbs,absText)
		{
			try{
				$("#frmList").find("input[operate=\"autoMul\"]").attr("ians","under");
				$("#frmList").find("label[class=\"items\"]").attr("ians","under");
			}catch(err){}
			if(trueAbs!=undefined && trueAbs!=null && trueAbs!="" 
			&& absText!=undefined && absText!=null && absText!="" 
			&& absText!=trueAbs)
			{
				var sTemp = "ABCD";
				for(var k in trueAbs){
					for(var j in sTemp)
					{
						if(trueAbs[k]!=undefined && trueAbs[k]!=null && trueAbs[k]!=""
						&& sTemp[j]!=undefined && sTemp[j]!=null && sTemp[j]!="" 
						&& sTemp[j]!=trueAbs[k])
						{
							$("#auto"+sTemp[j]+"").attr("ians","false");
							$("#label"+sTemp[j]+"").attr("ians","false");	
						}else{
							$("#auto"+sTemp[j]+"").attr("ians","true");
							$("#label"+sTemp[j]+"").attr("ians","true");
						}	
					}
				};
				for(var k in trueAbs)
				{
					$("#auto"+trueAbs[k]+"").attr("ians","true");
					$("#label"+trueAbs[k]+"").attr("ians","true");
				};
				
			}else if(trueAbs!=undefined && trueAbs!=null && trueAbs!="" 
			&& absText!=undefined && absText!=null && absText!=""){
				for(var k in trueAbs)
				{
					$(document.querySelector("#auto"+trueAbs[k]+"")).attr("ians","true");
					$(document.querySelector("#label"+trueAbs[k]+"")).attr("ians","true");
				};	
			}
		};
		
		/********************************************************************************************
		*验证用户多选题回答的答案信息
		*********************************************************************************************/
		var VerificationSelectionMultiple = function(absText)
		{
			try{
				if(Operate['thisOption']['abcText']!=undefined 
				&& Operate['thisOption']['abcText']!=null 
				&& Operate['thisOption']['abcText']!="" 
				&& absText!=undefined && absText!=null && absText!="" 
				&& absText==Operate['thisOption']['abcText'])
				{try{AnswerTrue(absText);}catch(err){};}
				else{try{AnswerFalse(absText);}catch(err){};}
			}catch(err){}
			/****************************************************************************************
			*标注选项当中的正确与错误试题信息
			*****************************************************************************************/
			try{MarkerMultiple(Operate['thisOption']['abcText'],absText);}
			catch(err){alert(err.message);}
			
			/********************************************************************************************
			*计算首正率
			*********************************************************************************************/
			try{CalculationAnswer();}catch(err){}
		};
		/********************************************************************************************
		*获取到我选择的答案的正确内容
		*********************************************************************************************/
		var GetThisAnswerText = function(absText)
		{
			var textContent = "";
			try{
				if(absText=="对"){textContent='对';}
				else if(absText=="错"){textContent='错';}
				else if(Operate['arrOptions']!=undefined && Operate['arrOptions']!=null 
				&& Operate['arrOptions'].length>=1)
				{
					for(var k in Operate['arrOptions']){
						var abc = "A";
						if(k==0){abc="A";}
						else if(k==1){abc="B";}
						else if(k==2){abc="C";}
						else if(k==3){abc="D";}
						else if(k==4){abc="E";}
						if(absText.indexOf(abc)!=-1){
							if(textContent!=""){textContent=textContent+"|"+Operate['arrOptions'][k];}
							else{textContent=Operate['arrOptions'][k];}
						}
					}
				}
			}catch(err){}
			return textContent;
		}
		/********************************************************************************************
		*计算答题首正率信息,可以暂时屏蔽
		*********************************************************************************************/
		var CalculationFirst = function()
		{
			try{
				var thisTotal = 0;var trueNumber = 0;var falseNumber = 0;
				try{
					$(arrList['items']).each(function(k,json){
						if(json['Firsttrue']=='0'){
							thisTotal=thisTotal+1;
							falseNumber = falseNumber+1;
						}else if(json['Firsttrue']=='1'){
							thisTotal=thisTotal+1;
							trueNumber = trueNumber+1;	
						};					  
					});	
				}catch(err){}
				if(thisTotal<=0){thisTotal=1;}
				var Points = ((trueNumber / thisTotal)*100).toFixed(2);
				if(parseFloat(Points)>=100){Points="100.00";}
				$('#frmPointValue').html(Points+"%");
			}catch(err){}
		};
		/********************************************************************************************
		*统计用户答题标准信息
		*********************************************************************************************/
		var CalculationAnswer = function()
		{
			try{
				var thisTotal = 0;var trueNumber = 0;var falseNumber = 0;
				var UnderNumber = 0;
				try{
					$(arrList['items']).each(function(k,json){
						try{
							if(json['isTrue']=='-1'){
								thisTotal=thisTotal+1;
								UnderNumber = UnderNumber+1;
							}else if(json['isTrue']=='1'){
								thisTotal=thisTotal+1;
								trueNumber = trueNumber+1;	
							}else if(json['isTrue']=='0'){
								thisTotal=thisTotal+1;
								falseNumber = falseNumber+1;	
							};
						}catch(err){}					  
					});	
				}catch(err){}
				/********************************************************************************************
				*开始设置数据信息
				*********************************************************************************************/
				try{$("#FrmTrueAbs").html("对"+trueNumber);}catch(err){};
				try{$("#FrmFalseAbs").html("错"+falseNumber);}catch(err){};
				try{$("#FrmUnderAbs").html("未答"+UnderNumber);}catch(err){}
				try{Operate['FalseNumber']=falseNumber;}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*用户回答正确以后的提示信息
		*********************************************************************************************/
		var AnswerTrue = function(absText)
		{
			/****************************************************************************************
			*将当前试题标注为已答题
			*****************************************************************************************/
			try{$("#frmCurrent").attr("Right","true");}
			catch(err){}
			/****************************************************************************************
			*记录当前答题的日志记录信息
			*****************************************************************************************/
			var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
			if(SelectedIndex<=0){SelectedIndex=0;}
			/****************************************************************************************
			*记录当前答题的日志记录信息
			*****************************************************************************************/
			try{
				if(arrList['items']!=undefined && arrList['items']!=null && typeof(arrList['items'])=='object' 
				&& arrList['items'][SelectedIndex]!=undefined && arrList['items'][SelectedIndex]!=null 
				&& Operate['thisOption']!=undefined && Operate['thisOption']!=null)
				{
					try{arrList['items'][SelectedIndex]['isTrue']='1';}catch(err){}
					try{arrList['items'][SelectedIndex]['thisAnswer']=GetThisAnswerText(absText);}
					catch(err){}
					try{
						if(arrList['items'][SelectedIndex]['Firsttrue']!="0" 
						&& arrList['items'][SelectedIndex]['Firsttrue']!="1")
						{arrList['items'][SelectedIndex]['Firsttrue']='1';}
					}catch(err){}
				}
			}catch(err){}
			/****************************************************************************************
			*首先关闭播放语音
			*****************************************************************************************/
			if(GetPlayerValue()=="语音讲解" && Operate['thisFalse']!=undefined 
			&& Operate['thisFalse']!=null && Operate['thisFalse']!="false")
			{try{ClosePlayer();}catch(err){};}
			/****************************************************************************************
			*播放提示语音
			*****************************************************************************************/
			try{
				$(Operate['frmRemind']).attr("src","inc/true.mp3");
				$(Operate['frmRemind'])[0].play();
			}
			catch(err){}
			/****************************************************************************************
			*错题回答正确以后将删除错题信息
			*****************************************************************************************/
			try{
				if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object'
				&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']=="错题练习")
				{
					try{DeleteFalse(Operate['thisOption'],function(){});}catch(err){}
				};
			}catch(err){}
			/****************************************************************************************
			*执行延时跳转信息
			*****************************************************************************************/
			try{
				var timerout = setTimeout(function(){
					try{							 
						clearTimeout(timerout);
						$(Operate['FrmNext']).click();
					}catch(err){}
				},1500);
			}
			catch(err){}
		};
		/********************************************************************************************
		*用户回答错误以后的提示信息
		*********************************************************************************************/
		var AnswerFalse = function(absText)
		{
			/****************************************************************************************
			*将当前试题标注为已答题
			*****************************************************************************************/
			try{$("#frmCurrent").attr("Right","false");}
			catch(err){}
			/****************************************************************************************
			*记录当前答题的日志记录信息
			*****************************************************************************************/
			var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
			if(SelectedIndex<=0){SelectedIndex=0;}
			/****************************************************************************************
			*记录当前答题的日志记录信息
			*****************************************************************************************/
			try{
				if(arrList['items']!=undefined && arrList['items']!=null && typeof(arrList['items'])=='object' 
				&& arrList['items'][SelectedIndex]!=undefined && arrList['items'][SelectedIndex]!=null 
				&& Operate['thisOption']!=undefined && Operate['thisOption']!=null)
				{
					try{arrList['items'][SelectedIndex]['isTrue']='0';}catch(err){}
					try{arrList['items'][SelectedIndex]['thisAnswer']=GetThisAnswerText(absText);}
					catch(err){}
					try{
						if(arrList['items'][SelectedIndex]['Firsttrue']!="0" 
						&& arrList['items'][SelectedIndex]['Firsttrue']!="1")
						{arrList['items'][SelectedIndex]['Firsttrue']='0';}
					}catch(err){}
				}
			}catch(err){}
			/****************************************************************************************
			*将正确答案展示出来
			*****************************************************************************************/
			try{ShowAnswer();}catch(err){}
			/****************************************************************************************
			*播放语音信息
			*****************************************************************************************/
			try{ShowPlayer();}catch(err){}
			/****************************************************************************************
			*开始组织答题错误后的处理信息
			*****************************************************************************************/
			try{
				if(Operate['thisFalse']!="true"){
					try{Operate['thisFalse']="true";}catch(err){}
					try{SaveFalse(Operate['thisOption']);}catch(err){}
				}
			}catch(err){}
		};
		/********************************************************************************************
		*开始渲染网页内容信息
		*********************************************************************************************/
		try{
			if(arrList!=undefined && arrList!=null && arrList['items']!=undefined 
			&& arrList['items']!=null && typeof(arrList['items'])=='object' 
			&& arrList['items'].length!=0)
			{
				try{render();}catch(err){}	
			};
		}catch(err){}
		/********************************************************************************************
		*存储用户答题卡信息
		*********************************************************************************************/
		var SavePaper = function(back)
		{
			try{
				if(arrList!=undefined && arrList!=null && typeof(eKsList)=='object' 
				&& arrList['items']!=undefined && arrList['items']!=null
				&& Operate['SelectedIndex']!=undefined && Operate['SelectedIndex']!=""
				&& cfg['strUnion']!=undefined && cfg['strUnion']!=null 
				&& (cfg['strUnion']=='顺序练习' || cfg['strUnion']=='专项练习')
				&& cfg['UnionID']!=undefined && cfg['UnionID']!=null && cfg['UnionID']!='0')
				{
					try{
						var thisRecord = parseInt(Operate['SelectedIndex']) || 0;
						SavePaperLogs(arrList['items'],thisRecord,function(iSuccess){
							if(back!=undefined && back!=null 
							&& typeof(back)=='function'){
								back();	
							};
						});	
					}catch(err){}
				}else if(back!=undefined && back!=null 
				&& typeof(back)=='function'){
					try{back();	}catch(err){}
				};
			}catch(err){}
		}
		
	};

})(jQuery);
/**************************************************************************************
*清空当前的答题选项信息
***************************************************************************************/
var EmptyAbs = function(){
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object'
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!="")
		{
			try{
				var SendOption = {};
				try{
					SendOption["url"]="eKaoshi.aspx?action=emptyAbs&classId="+cfg['classId']+"&strChannel="+cfg['channel']+"";
					SendOption["success"] = function(strResponse){
						try{
							if(strResponse!=undefined && strResponse!=null && strResponse!="" 
							&& back!=undefined && back!=null 
							&& typeof(back)=='function')
							{back(strResponse);}
						}catch(err){}
					};
					SendOption["error"] = function(){
						alert('数据请求中发生错误,请重试！');return false;
					};
				}catch(err){}
				/*************************************************************************
				*开始请求数据
				**************************************************************************/
				if(window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
				&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null)
				{
					try{window.jQuery.ajax(SendOption);}catch(err){}
				};
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};	
}
/**************************************************************************************
*收藏当前考题
***************************************************************************************/
var SaveFav = function(options,back)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& options!=undefined && options!=null && typeof(options)=='object'
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!=""
		&& options['QuetID']!=undefined && options['QuetID']!=null && options['QuetID']!="")
		{
			try{
				var SendOption = {};
				try{
					SendOption["url"]="eKaoshi.aspx?action=savefav&Quetid="+options["QuetID"]+"&classId="+cfg['classId']+"&strChannel="+cfg['channel']+"";
					SendOption["success"] = function(strResponse){
						try{
							if(strResponse!=undefined && strResponse!=null && strResponse!="" 
							&& back!=undefined && back!=null && typeof(back)=='function')
							{back(strResponse);}
						}catch(err){}
					};
					SendOption["error"] = function(){
						alert('数据请求中发生错误,请重试！');return false;
					};
				}catch(err){}
				/*************************************************************************
				*开始请求数据
				**************************************************************************/
				if(window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
				&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null)
				{
					try{window.jQuery.ajax(SendOption);}catch(err){}
				}
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};
};
/**************************************************************************************
*取消当前的收藏
***************************************************************************************/
var DeleteFav = function(options,back)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& options!=undefined && options!=null && typeof(options)=='object'
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!=""
		&& options['QuetID']!=undefined && options['QuetID']!=null && options['QuetID']!="")
		{
			try{
				var SendOption = {};
				try{
					SendOption["url"]="eKaoshi.aspx?action=delfav&Quetid="+options["QuetID"]+"&classId="+cfg['classId']+"&strChannel="+cfg['channel']+"";
					SendOption["success"] = function(strResponse){
						try{
							if(strResponse!=undefined && strResponse!=null && strResponse!="" 
							&& back!=undefined && back!=null && typeof(back)=='function')
							{back(strResponse);}
						}catch(err){}
					};
					SendOption["error"] = function(){
						alert('数据请求中发生错误,请重试！');return false;
					};
				}catch(err){}
				/*************************************************************************
				*开始请求数据
				**************************************************************************/
				if(window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
				&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null)
				{
					try{window.jQuery.ajax(SendOption);}catch(err){}
				}
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};
};
/**************************************************************************************
*保存为错题或者将当前题目设置为错题
***************************************************************************************/
var SaveFalse = function(options,back)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& options!=undefined && options!=null && typeof(options)=='object'
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!=""
		&& options['QuetID']!=undefined && options['QuetID']!=null && options['QuetID']!="")
		{
			try{
				var SendOption = {};
				try{
					SendOption["url"]="eKaoshi.aspx?action=savewrong&Quetid="+options["QuetID"]+"&classId="+cfg['classId']+"&strChannel="+cfg['channel']+"";
					SendOption["success"] = function(strResponse){
						try{
							if(strResponse!=undefined && strResponse!=null && strResponse!="" 
							&& back!=undefined && back!=null 
							&& typeof(back)=='function')
							{back(strResponse);}
						}catch(err){}
					};
					SendOption["error"] = function(){
						alert('数据请求中发生错误,请重试！');return false;
					};
				}catch(err){}
				/*************************************************************************
				*开始请求数据
				**************************************************************************/
				if(window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
				&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null)
				{
					try{window.jQuery.ajax(SendOption);}catch(err){}
				}
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};
};
/**************************************************************************************
*删除当前错题信息
***************************************************************************************/
var DeleteFalse = function(options,back)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& options!=undefined && options!=null && typeof(options)=='object'
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!=""
		&& options['QuetID']!=undefined && options['QuetID']!=null && options['QuetID']!="")
		{
			try{
				var SendOption = {};
				try{
					SendOption["url"]="eKaoshi.aspx?action=delwrong&Quetid="+options["QuetID"]+"&classId="+cfg['classId']+"&&strChannel="+cfg['channel']+"";
					SendOption["success"] = function(strResponse){
						try{
							if(strResponse!=undefined && strResponse!=null && strResponse!="" 
							&& back!=undefined && back!=null 
							&& typeof(back)=='function')
							{back(strResponse);}
						}catch(err){}
					};
					SendOption["error"] = function(){
						alert('数据请求中发生错误,请重试！');return false;
					};
				}catch(err){}
				/*************************************************************************
				*开始请求数据
				**************************************************************************/
				if(window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
				&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null)
				{
					try{window.jQuery.ajax(SendOption);}catch(err){}
				}
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};		
};
/**************************************************************************************
*清空所有的错题信息
***************************************************************************************/
var EmptyWrong = function(back)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!="")
		{
			try{
				var SendOption = {};
				try{
					SendOption["url"]="eKaoshi.aspx?action=clearwrong&classId="+cfg['classId']+"";
					SendOption["success"] = function(strResponse){
						try{
							if(strResponse!=undefined && strResponse!=null && strResponse!="" 
							&& back!=undefined && back!=null 
							&& typeof(back)=='function')
							{back(strResponse);}
						}catch(err){}
					};
					SendOption["error"] = function(){
						alert('数据请求中发生错误,请重试！');return false;
					};
				}catch(err){}
				/*************************************************************************
				*开始请求数据
				**************************************************************************/
				if(window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
				&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null)
				{
					try{window.jQuery.ajax(SendOption);}catch(err){}
				}
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};		
};
/**************************************************************************************
*保存答题记录日志信息
***************************************************************************************/
var CookieRecord = function(thisRecord)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!="" 
		&& window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function'
		&& window.jQuery.cookies!=undefined && typeof(window.jQuery.cookies)=='function')
		{
			var CookieName = ("FookeOrder"+cfg['classId']);
			window.jQuery.cookies(CookieName,thisRecord);	
		};
	}catch(err){alert(err.message);return false;};	
};
/**************************************************************************************
*保存答题记录日志信息
***************************************************************************************/
var SaveRecord = function(thisRecord)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!="")
		{
			try{
				var SendOption = {};
				try{
					SendOption["url"]="eKaoshi.aspx?action=SaveRecord";
					SendOption['type']="post";
					SendOption["data"] = {
						"thisRecord":(parseInt(thisRecord) || 0),
						"unionid":cfg['classId'],
						"strUnion":cfg['channel']
					};
					SendOption["success"] = function(strResponse)
					{
						if(strResponse!="success")
						{console.log('记录操作日志:'+strResponse);return false;}
					};
					SendOption["error"] = function()
					{
						console.log('请求出错:'+strResponse);return false;
					};
				}catch(err){}
				/************************************************************************
				*开始请求数据
				*************************************************************************/
				if(window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
				&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null)
				{
					try{window.jQuery.ajax(SendOption);}catch(err){}
				}
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};	
};
/**************************************************************************************
*保存答题记录日志信息
***************************************************************************************/
var SavePaperLogs = function(options,thisRecord,back)
{
	try{$("#frmSaveMaster").show();}catch(err){}
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& options!=undefined && options!=null && typeof(options)=='object'
		&& cfg['strUnion']!=undefined && cfg['strUnion']!=null && cfg['strUnion']!=""
		&& cfg['UnionID']!=undefined && cfg['UnionID']!=null && cfg['UnionID']!="0"
		&& options!=undefined && options!=null && typeof(options)=="object")
		{
			try{
				var SendOption = {};
				SendOption["url"]="eKaoshi.aspx?action=saveLogs";
				SendOption['type']="post";
				SendOption['async'] = false;
				SendOption["data"] = {
					"thisRecord":(parseInt(thisRecord) || 0),
					"strContext":JSON.stringify(options),
					"unionid":cfg['UnionID'],
					"strUnion":cfg['strUnion']
				};
				SendOption["success"] = function(strResponse){
					try{$("#frmSaveMaster").hide();}catch(err){}
					try{
						if(strResponse!="success"){console.log('记录操作日志:'+strResponse);return false;}
						else if(back!=undefined && back!=null && typeof(back)=='function')
						{try{back(true);}catch(err){};}
					}catch(err){}
				};
				SendOption["error"] = function(){
					try{$("#frmSaveMaster").hide();}catch(err){}
					console.log('请求出错:'+strResponse);return false;
				};
				/*************************************************************************
				*开始请求数据
				**************************************************************************/
				if(window.jQuery!=undefined && window.jQuery!=null && typeof(window.jQuery)=='function' 
				&& window.jQuery.ajax!=undefined && window.jQuery.ajax!=null)
				{
					try{window.jQuery.ajax(SendOption);}catch(err){}
				}
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};	
};
/**************************************************************************************
*清空答题选项信息,主要是顺序练习和专项练习
***************************************************************************************/
var EmptyPaperLogs = function(){
	try{
		if(cfg['strUnion']!=undefined && cfg['strUnion']!=null && cfg['strUnion']!="" 
		&& cfg['UnionID']!=undefined && cfg['UnionID']!=null && cfg['UnionID']!="0")
		{
			window.location='eKaoshi.aspx?action=EmptyLogs&strUnion='+cfg['strUnion']+'&UnionID='+cfg['UnionID']+'';	
		};
	}catch(err){}
};
/**************************************************************************************
*获取选项前缀标记
***************************************************************************************/
var ABC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
var GetAbcText = function(Selected)
{
	try{
		var abcText = "";
		if(parseInt(Selected)<ABC.length)
		{abcText = ABC.substr(Selected,1);}
		return abcText;
	}catch(err){return "";}
};
/**************************************************************************************
*替换特殊关键字
***************************************************************************************/
var ReplaceKeywords = function(strValue)
{
	var strValue = strValue || "";
	try{
		strValue = strValue.replace(/\r\n/g,"|");
		strValue = strValue.replace(/\r/g,"|");
		strValue = strValue.replace(/\n/g,"|");
	}catch(err){}
	return strValue;
};

/**************************************************************************
*提示网页错误信息
***************************************************************************/
window.alert = function(strValue,back){
	try{
		if(window.WindowsAlert!=undefined && window.WindowsAlert!=null 
		&& typeof(window.WindowsAlert)=='function'){
			WindowsAlert(strValue,function(){
				if(back!=undefined && back!=null 
				&& typeof(back)=='function'){
					back();	
				}							   
			});
			return false;	
		};
	}catch(err){}
}