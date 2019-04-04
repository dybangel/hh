;(function($) {
"use strict";
	$.fn.Kaoshi = function(options,arrList)
	{
		var contianer = this;
		/********************************************************************************************
		*定义网页控件集合信息
		*********************************************************************************************/
		var Operate = {
			"SelectedIndex":0,
			"thisOption":{},
			"thisFalse":"false",
			"isTimeup" : "false",
			"iSubmit" : "false"
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
				/******************************************************************
				*当前试题以回答,并且已答错的情况下
				*******************************************************************/
				try{
					var thisAnswerTrue = GetThisAnswer();
					if(thisAnswerTrue==-1){
						try{ShowKaoshi(SelectedIndex);}catch(err){}	
					}
					else if(thisAnswerTrue==0)
					{
						ShowErrorKaoshi(Operate['thisOption'],function(){
							try{setWrongTager();}catch(err){}
							try{ShowKaoshi(SelectedIndex);}catch(err){}
						});
					}
					else{
						try{setTrueTager();}catch(err){};
						try{ShowKaoshi(SelectedIndex);}catch(err){}	
					}
				}catch(err){}
			}catch(err){}
		};
		
		/********************************************************************************************
		*显示指定的考试试题
		*********************************************************************************************/
		var ShowKaoshi = function(SelectedIndex)
		{
			/******************************************************************
			*开始加载新题信息
			*******************************************************************/
			try{ClosePlayer();}catch(err){alert("语音关闭失败,请重试");}
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
			/******************************************************************
			*标注当前试题允许回答
			*******************************************************************/
			try{
				if(Operate['isTimeup']=='true' || Operate['iSubmit']=='true')
				{
					$(Operate['FrmTrue']).attr("disabled","disabled");
					$(Operate['FrmFalse']).attr("disabled","disabled");
					$(Operate['FrmA']).attr("disabled","disabled");
					$(Operate['FrmB']).attr("disabled","disabled");
					$(Operate['FrmC']).attr("disabled","disabled");
					$(Operate['FrmD']).attr("disabled","disabled");	
				}
				else if(Operate['thisOption']['isTrue']!=undefined 
				&& Operate['thisOption']['isTrue']!=null 
				&& Operate['thisOption']['isTrue']!="" 
				&& Operate['thisOption']['isTrue']!="-1")
				{
					$(Operate['FrmTrue']).attr("disabled","disabled");
					$(Operate['FrmFalse']).attr("disabled","disabled");
					$(Operate['FrmA']).attr("disabled","disabled");
					$(Operate['FrmB']).attr("disabled","disabled");
					$(Operate['FrmC']).attr("disabled","disabled");
					$(Operate['FrmD']).attr("disabled","disabled");
				}else{
					$(Operate['FrmTrue']).removeAttr("disabled");
					$(Operate['FrmFalse']).removeAttr("disabled");
					$(Operate['FrmA']).removeAttr("disabled");
					$(Operate['FrmB']).removeAttr("disabled");
					$(Operate['FrmC']).removeAttr("disabled");
					$(Operate['FrmD']).removeAttr("disabled");
				}
			}catch(err){}
			/******************************************************************
			*输出试题信息
			*******************************************************************/
			try{
				
				/******************************************************************
				*显示主题
				*******************************************************************/
				try{
					var Subject = ""+(SelectedIndex+1)+"："+Operate['thisOption']['strTitle'];
					$(Operate['frmQuestionTitle']).html(Subject);
				}catch(err){}	
				/******************************************************************
				*显示图片
				*******************************************************************/
				try{
					if(Operate['thisOption']['strThumb']!=undefined 
					&& Operate['thisOption']['strThumb']!=null 
					&& Operate['thisOption']['strThumb']!="")
					{
						$(Operate['frmThumb']).html("<img alt=\"点击查看大图\" title=\"点击查看大图\" src=\""+Operate['thisOption']['strThumb']+"\" style=\"max-width:100%;max-height:100%;display: inline-block; vertical-align: middle;\" />");
					}
					else{$(Operate['frmThumb']).html("");}
				}catch(err){}
				/******************************************************************
				*生成数组,并且将数组乱序显示出来
				*******************************************************************/
				var arrOptions = ReplaceKeywords(Operate['thisOption']['strOptions']).split("|") || [];
				//arrOptions.sort(function(){ return 0.5 - Math.random();});
				
				/******************************************************************
				*生成答题选项,判断题将不显示选项
				*******************************************************************/
				if(Operate['thisOption']['QuestionMode']!="判断")
				{
					var strTemplate = "";
					strTemplate +="<div operate=\"optionsList\">";
					for(var n in arrOptions)
					{
						var autoText = "A";
						try{
							if(n==0){autoText="A";}
							else if(n==1){autoText="B";}
							else if(n==2){autoText="C";}
							else if(n==3){autoText="D";}
							else if(n==4){autoText="E";}
							else if(n==5){autoText="F";}
							else if(n==6){autoText="G";}
						}catch(err){}
						/*******************************************************************
						*开始加载选项题
						********************************************************************/
						strTemplate +="<label class=\"items\">";
						if(Operate['thisOption']['QuestionMode']=="多选")
						{
							strTemplate +="<input id=\"auto"+autoText+"\" class=\"autoABCD\" type=\"checkbox\"";
							if(Operate['thisOption']['isTrue']!=undefined 
							&& Operate['thisOption']['isTrue']!="-1")
							{strTemplate +=" disabled=\"disabled\" ";}
							if(Operate['iSubmit']='true'){strTemplate +=" disabled=\"disabled\" ";}
							if(Operate['thisOption']["thisAnswer"]!=undefined 
							&& Operate['thisOption']["thisAnswer"]!=null 
							&& Operate['thisOption']["thisAnswer"]!="" 
							&& Operate['thisOption']["thisAnswer"].indexOf(autoText)!=-1)
							{
								strTemplate +=" checked ";
							}
							strTemplate +=" value=\""+autoText+"\" operate=\"autoMul\" />";
						}
						strTemplate +="<span class=\"text\">"+autoText+":"+arrOptions[n]+"</span>";
						strTemplate +="</label>";
					};
					
					strTemplate +="</div>";
					var frmTemplate = $(strTemplate)[0]; 
					/******************************************************************
					*将选项内容赋值到数据当中
					*******************************************************************/
					$(Operate['frmQuestionOptions']).html(frmTemplate);
					/******************************************************************
					*定义选项事件信息
					*******************************************************************/
					if(Operate['thisOption']['QuestionMode']=="多选" 
					&& frmTemplate!=undefined && frmTemplate!=null)
					{
						try{
							$(frmTemplate).find("input[operate=\"autoMul\"]").click(function(){
								if(Operate['mp3Button']!=undefined && Operate['mp3Button'].value!="" 
								&& Operate['mp3Button'].value!='关闭语音')
								{
									var thisValue = this.value;	
									if(thisValue!=undefined && thisValue=="A"){MultipleA(true);}
									else if(thisValue!=undefined && thisValue=="B"){MultipleB(true);}
									else if(thisValue!=undefined && thisValue=="C"){MultipleC(true);}
									else if(thisValue!=undefined && thisValue=="D"){MultipleD(true);}	
								}else{this.checked = false;}
							});	
						}catch(err){}
					};
					
				}
				else{$(Operate['frmQuestionOptions']).html("");}
				/******************************************************************
				*计算正确答案信息
				*******************************************************************/
				try{
					/******************************************************************
					*定义正确答案文本信息
					*******************************************************************/
					var AnswerValue = "";
					/******************************************************************
					*开始获取正确答案内容
					*******************************************************************/
					var AnswerText = Operate['thisOption']['AnswerText'] || "";
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
					/******************************************************************
					*我的答案控件内容清空
					*******************************************************************/
					try{$("#frmMAtext").html("");}catch(err){}
					/******************************************************************
					*将我的答案填写到复选框当中
					*******************************************************************/
					try{
						if(Operate['thisOption']['thisAnswer']!=undefined 
						&& Operate['thisOption']['thisAnswer']!=null 
						&& Operate['thisOption']['thisAnswer']!=''){
							$("#frmMAtext").html(Operate['thisOption']['thisAnswer']);	
						}
					}catch(err){}
				}
				catch(err){}
				/******************************************************************
				*显示进度条
				*******************************************************************/
				try{$(Operate['frmProcess']).attr("value",Operate["SelectedIndex"]);}
				catch(err){};
				/******************************************************************
				*将用户选项信息还原
				*******************************************************************/
				try{
					$(Operate['FrmA']).attr("iSelection","false");
					$(Operate['FrmB']).attr("iSelection","false");
					$(Operate['FrmC']).attr("iSelection","false");
					$(Operate['FrmD']).attr("iSelection","false");
					$(Operate['FrmTrue']).attr("iSelection","false");
					$(Operate['FrmFalse']).attr("iSelection","false");
				}catch(err){}
				/******************************************************************
				*判断显示的答题按钮信息
				*******************************************************************/
				try{
					if(Operate['thisOption']['QuestionMode']!=undefined 
					&& Operate['thisOption']['QuestionMode']!=null 
					&& Operate['thisOption']['QuestionMode']!="" 
					&& Operate['thisOption']['QuestionMode']=="判断")
					{
						try{
							$(Operate['FrmA']).hide();
							$(Operate['FrmB']).hide();
							$(Operate['FrmC']).hide();
							$(Operate['FrmD']).hide();
							$(Operate['FrmTrue']).show();
							$(Operate['FrmFalse']).show();	
						}catch(err){}
						/******************************************************************
						*输出提示信息
						*******************************************************************/
						try{
							$(Operate['frmSecondTips']).html("判断题,判断对错");	
						}catch(err){}
					}
					else if(Operate['thisOption']['QuestionMode']!=undefined 
					&& Operate['thisOption']['QuestionMode']!=null 
					&& Operate['thisOption']['QuestionMode']!="" 
					&& Operate['thisOption']['QuestionMode']=="单选")
					{
						try{
							$(Operate['FrmA']).show();
							$(Operate['FrmB']).show();
							$(Operate['FrmC']).show();
							$(Operate['FrmD']).show();
							$(Operate['FrmTrue']).hide();
							$(Operate['FrmFalse']).hide();	
						}catch(err){}
						/******************************************************************
						*输出提示信息
						*******************************************************************/
						try{
							$(Operate['frmSecondTips']).html("单选题,选择你认为正确的答案");	
						}catch(err){}
					}
					else if(Operate['thisOption']['QuestionMode']!=undefined 
					&& Operate['thisOption']['QuestionMode']!=null 
					&& Operate['thisOption']['QuestionMode']!="" 
					&& Operate['thisOption']['QuestionMode']=="多选")
					{
						try{
							$(Operate['FrmA']).show();
							$(Operate['FrmB']).show();
							$(Operate['FrmC']).show();
							$(Operate['FrmD']).show();
							$(Operate['FrmTrue']).hide();
							$(Operate['FrmFalse']).hide();	
						}catch(err){}
						/******************************************************************
						*输出提示信息
						*******************************************************************/
						try{
							$(Operate['frmSecondTips']).html("多选题,选择你认为正确的答案");	
						}catch(err){}
					}
				}catch(err){}
				/******************************************************************
				*判断是否需要开启语音
				*******************************************************************/
				if($(Operate['mp3Button']).val()=="关闭语音"){
					try{ShowPlayer();}catch(err){}
				};
				/******************************************************************
				*将当前试题设置为选中模式,并且将滚动条跳转到此处
				*******************************************************************/
				try{GotoPaint();}catch(err){}
				
			}catch(err){}
		};
		/********************************************************************************************
		*答错以后给出错误提示信息
		*********************************************************************************************/
		var ShowErrorKaoshi = function(Json,back)
		{
			if(!Operate['ShowErrorMasker']){alert('获取配置信息错误,请重试');return false;}
			if(!Operate['ShowError']){alert('获取配置信息错误,请重试');return false;}
			var strTemplate ="";
			/*************************************************************************************
			*开始加载弹出框内容信息
			**************************************************************************************/
			try{
				strTemplate+="<div id=\"frmWindowtitle\"><span class=\"name\">驾考理论考试系统</span></div>";
				strTemplate+="<div id=\"frmWindowclose\">关闭</div>";
				strTemplate+="<div id=\"errContianer\">";
				/*************************************************************************************
				*展示标题
				**************************************************************************************/
				strTemplate+="<div id=\"errTitle\">"+Json['strTitle']+"</div>";
				strTemplate+="<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
				/*************************************************************************************
				*展示选项
				**************************************************************************************/
				try{
					if(Json['QuestionMode']!=undefined && Json['QuestionMode']!=null 
					&& Json['QuestionMode']!="" && Json['QuestionMode']!="判断")
					{
						var arrOptions = ReplaceKeywords(Json['strOptions']).split("|") || [];
						strTemplate +="<div id=\"errOption\">";
						for(var k in arrOptions){
							strTemplate +="<div>";
							if(k==0){strTemplate +="A:";}
							else if(k==1){strTemplate +="B:";}
							else if(k==2){strTemplate +="C:";}
							else if(k==3){strTemplate +="D:";}
							else if(k==4){strTemplate +="E:";}
							strTemplate +=""+arrOptions[k]+"";
							strTemplate +="</div>";
						};
						strTemplate +="</div>";
						strTemplate+="<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
					}
				}catch(err){}
				/*************************************************************************************
				*展示正确答案
				**************************************************************************************/
				try{
					strTemplate +="<div id=\"errAnswer\">";
					strTemplate +="<span id=\"trueAnswer\">正确答案："+((Json['abcText']) || "")+"</span>";
					strTemplate +="<span id=\"thisAnswer\">我的答案："+((Json['thisAnswer']) || "")+"</span>";
					strTemplate +="</div>";
				}catch(err){}
				/*************************************************************************************
				*展示图片
				**************************************************************************************/
				try{
					strTemplate +="<div id=\"errThumb\">";
					if(Json['strThumb']!=undefined && Json['strThumb']!=undefined!=null 
					&& Json['strThumb']!=undefined!="")
					{
						strTemplate+="<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
						strTemplate +="<img src=\""+Json['strThumb']+"\" style=\"max-width:300px;max-height:200px\" />";	
					}
					strTemplate +="</div>";
				}catch(err){}
				/*************************************************************************************
				*展示横线
				**************************************************************************************/
				strTemplate +="<div id=\"errDoline\"></div>";
				
				strTemplate +="<div id=\"errTools\">";
				strTemplate +="<div><input type=\"button\" operate=\"close\" id=\"frmClose\" value=\"继续答题\" /></div>";
				strTemplate +="<div>以上正确答案确认无误后点击继续考试，返回主界面</div>";
				strTemplate +="<div>页面将在<span id=\"frmtimer\">9</span>秒后自动关闭,返回主界面!</div>";
				strTemplate +="</div>";
				strTemplate+="</div>";
			}catch(err){}
			/*************************************************************************************
			*以下是操作按钮信息
			**************************************************************************************/
			$(Operate['ShowError']).html(strTemplate);
			$(Operate['ShowErrorMasker']).show();
			/*************************************************************************************
			*关闭弹出框信息内容
			**************************************************************************************/
			var closeWindows = function(){
				try{clearInterval(inter);}catch(err){}
				$(Operate['ShowErrorMasker']).hide();
				
				if(back!=undefined && back!=null 
				&& typeof(back)=='function'){
					try{back();}catch(err){}	
				}
			};
			/*************************************************************************************
			*设置点击事件信息
			**************************************************************************************/
			$(Operate['ShowError']).find("#frmWindowclose").click(function(){
				try{closeWindows();}catch(err){}												
			});
			$(Operate['ShowError']).find("#frmClose").click(function(){
				try{closeWindows();}catch(err){}																
			});
			/*************************************************************************************
			*设置点击事件信息
			**************************************************************************************/
			var timer = 10;
			var inter = setInterval(function(){
				try{
					if(timer>=1){
						timer=timer-1;
						$("#frmtimer").html(timer);
					}
					else{
						try{clearInterval(inter);}catch(err){}
						try{closeWindows();}catch(err){}
					}
				}catch(err){}
			},1000);
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
					var rowsIndex = parseInt(Math.floor((SelectedIndex/10)));
					var scrollHeight = $("#frmOptionsTabs").outerHeight();
					var gotoHeight = parseInt(((rowsIndex-1)*33));
					if(gotoHeight<=0){gotoHeight=0;}
					if(gotoHeight>=scrollHeight){gotoHeight=scrollHeight;}
					$(Operate['frmOptions']).scrollTop(gotoHeight);
				}catch(err){}
				/************************************************************************************
				*标注试题选中状态
				*************************************************************************************/
				try{
					if(document.querySelector('#frmCurrent')){
						$(document.querySelector('#frmCurrent')).removeAttr('id');	
					};
					$("#frmOptionsTabs").find("td[value=\""+SelectedIndex+"\"]").attr("id","frmCurrent");
				}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*显示正确答案信息
		*********************************************************************************************/
		var ShowAnswer = function()
		{
			if(Operate['thisOption']!=undefined 
			&& Operate['thisOption']!=null 
			&& typeof(Operate['thisOption'])=='object' 
			&& Operate['thisOption']['AnswerText']!=undefined 
			&& Operate['thisOption']['AnswerText']!=null 
			&& Operate['thisOption']['AnswerText']!="")
			{
				
			};		
		};
		
		/***************************************************************************************
		*开启倒计时功能
		****************************************************************************************/
		var StartInterval = function(){
			try{
				Operate['timer'] = setInterval(function(){
					var interval = parseInt(options['Interval']) || 0;
					if(interval>=1)
					{
						try{
							interval = interval-1;
							options['Interval'] = interval;
							var hour = parseInt((interval % 86400) / 3600);
							if(hour<10){hour="0"+hour;}
							var Minute = parseInt((interval % 3600) / 60);
							if(Minute<10){Minute="0"+Minute;}
							var Second = parseInt(((interval % 3600) % 60 % 60));
							if(Second<10){Second="0"+Second;}
							$(Operate['frmInterval']).html(""+hour+":"+Minute+":"+Second+"");
						}catch(err){}	
					}
					else
					{
						try{StopInterval();}catch(err){}
						Operate['isTimeup'] = true;
						try{
							$(Operate['FrmTrue']).attr("disabled","disabled");
							$(Operate['FrmFalse']).attr("disabled","disabled");
							$(Operate['FrmA']).attr("disabled","disabled");
							$(Operate['FrmB']).attr("disabled","disabled");
							$(Operate['FrmC']).attr("disabled","disabled");
							$(Operate['FrmD']).attr("disabled","disabled");	
						}catch(err){}
						try{ShowSubmit(true);}catch(err){}	
					}
				},1000);	
			}catch(err){}
		};
		/***************************************************************************************
		*关闭倒计时功能
		****************************************************************************************/
		var StopInterval = function()
		{
			try{
				if(Operate['timer']!=undefined && Operate['timer']!=null)
				{clearInterval(Operate['timer']);}
			}catch(err){}
		}
		/********************************************************************************************
		*开启Mp3
		*********************************************************************************************/
		var ShowPlayer = function(){
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
					$(Operate['frmPlayer']).attr("src",Operate['thisOption']['strMP3']);
					try{ShowTechniques();}catch(err){}
					/*****************************************************************************
					*设置答案选项卡是否可以选择
					******************************************************************************/
					if(Operate["thisFalse"]!="true"){
						try
						{
							$(Operate['FrmTrue']).attr("disabled","disabled");
							$(Operate['FrmFalse']).attr("disabled","disabled");
							$(Operate['FrmA']).attr("disabled","disabled");
							$(Operate['FrmB']).attr("disabled","disabled");
							$(Operate['FrmC']).attr("disabled","disabled");
							$(Operate['FrmD']).attr("disabled","disabled");
							$("input[operate='autoMul']").attr("disabled","disabled");
						}catch(err){}	
					};
					/*******************************************************************************
					*关闭考试倒计时信息
					********************************************************************************/
					if(Operate['isTimeup']!='true' && Operate['iSubmit']!='true')
					{
						try{StopInterval();	}catch(err){}
					}
				}
				else{alert('加载语音失败,请重试!');return false;};
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
					Operate['frmPlayer'].pause();
					try{CloseTechniques();}catch(err){}
					try{
						if(Operate['isTimeup']!='true' && Operate['iSubmit']!='true'){
							$(Operate['FrmTrue']).removeAttr("disabled");
							$(Operate['FrmFalse']).removeAttr("disabled");
							$(Operate['FrmA']).removeAttr("disabled");
							$(Operate['FrmB']).removeAttr("disabled");
							$(Operate['FrmC']).removeAttr("disabled");
							$(Operate['FrmD']).removeAttr("disabled");
							$("input[operate='autoMul']").removeAttr("disabled");
						}
					}catch(err){};
				}
			}catch(err){alert(err.message)}
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
				&& Operate['thisOption']['strTechniques']!="")
				{
					var strTemplate = "<img src=\""+Operate['thisOption']['strTechniques']+"\"";
					strTemplate +=" style=\"max-width:100%;max-height:300px;display:line;margin:0px;\"/>";
					$(Operate['frmTechniques']).html(strTemplate);
					$(Operate['frmTechniques']).show();
					$(Operate['frmThumb']).hide();
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
				if(Operate['frmTechniques']!=undefined 
				&& Operate['frmTechniques']!=null)
				{
					$(Operate['frmTechniques']).html("");
					$(Operate['frmTechniques']).hide();
					$(Operate['frmThumb']).show();
				}
			}catch(err){}
		};
		/********************************************************************************************
		*显示正确答案
		*********************************************************************************************/
		var ShowAnswer = function(){
			try{$(Operate['frmTAtext']).show();}catch(err){}
		};
		/********************************************************************************************
		*关闭正确答案显示
		*********************************************************************************************/
		var CloseAnswer = function(){
			try{$(Operate['frmTAtext']).hide();}catch(err){}
		};
		/********************************************************************************************
		*渲染答题卡选项个数信息
		*********************************************************************************************/
		var renderOptions = function()
		{
			try{
				var frmTemplate = "<table width=\"100%\" border=\"0\" id=\"frmOptionsTabs\" cellpadding=\"3\" cellspacing=\"1\">";
				frmTemplate += "<tr class=\"hback\">";
				var SelectedIndex = 1;var Length = parseInt(arrList['items'].length) || 0;
				$(arrList['items']).each(function(k,JSON){	
					try{
						frmTemplate +="<td value=\""+(k+1)+"\"";
						try{
							if(JSON['isTrue']=="1"){frmTemplate +=" answer=\"true\"";}
							else if(JSON['isTrue']=="0")
							{frmTemplate +=" answer=\"false\"";}
						}catch(err){}
						frmTemplate +=" operate=\"dbclick\">";
						if(JSON['thisAnswer']!=undefined && JSON['thisAnswer']!=null 
						&& JSON['thisAnswer']!=""){
							frmTemplate +="<span>"+(k+1)+"</span>";
							frmTemplate +="<span>"+JSON['thisAnswer']+"</span>";
						}else{
							frmTemplate +=""+(k+1)+"";	
						}
						frmTemplate +="</td>";
					}catch(err){}
					if(SelectedIndex>=10){
						frmTemplate+="</tr><tr class=\"hback\">";
						SelectedIndex=1;
					}
					else{SelectedIndex=SelectedIndex+1;}								  
				});
				/********************************************************************************************
				*数据补位,将空的TD补足
				*********************************************************************************************/
				if((Length % 10) !=0){
					try{
						for(var j=0;j<(10-(Length % 10));j++)
						{frmTemplate+="<td></td>";}
					}catch(err){}
				}
				/********************************************************************************************
				*输出表格TR信息
				*********************************************************************************************/
				frmTemplate += "</tr>";
				frmTemplate += "</table>";
				$(Operate['frmKOption']).html(frmTemplate);
			}catch(err){}
			/********************************************************************************************
			*增加选项卡点击事件
			*********************************************************************************************/
			try{
				$(Operate['frmKOption']).find("td[operate=\"dbclick\"]").click(function(){
					var SelectedIndex = parseInt($(this).attr("value")) || 0;
					if(SelectedIndex!=undefined && SelectedIndex!=null && SelectedIndex!=0)
					{
						GotoKaoshi(SelectedIndex);	
					}
				});	
			}catch(err){}
		};
		var renderProcess = function()
		{
			
			frmTemplate += "<tr class=\"hback\">";
			var SelectedIndex = 1;
			for(var k=1;k<=arrList['items'].length;k++)
			{
				frmTemplate+="<td>"+k+"</td>";
				if(SelectedIndex>=10){
					SelectedIndex=1;
					frmTemplate+="</tr><tr class=\"hback\">";
				}
				else{SelectedIndex=SelectedIndex+1;}
			};
			frmTemplate += "</tr>";
			frmTemplate += "</table>";
			$(Operate['frmOptions']).html(frmTemplate);
		};
		/********************************************************************************************
		*定义渲染控件
		*********************************************************************************************/
		var render = function()
		{
			var strTemplate = "";
			strTemplate += "<div operate=\"Kscontianer\" id=\"Kscontianer\">";
			/***************************************************************************************
			*错题提示界面信息,
			****************************************************************************************/
			strTemplate += "<div id=\"ShowErrorMasker\">"
			strTemplate += "<div title=\"驾考理论考试系统\" operate=\"ShowError\" id=\"ShowError\">";
			strTemplate += "</div>";
			strTemplate += "</div>";
			/***************************************************************************************
			*视频交卷提示信息,
			****************************************************************************************/
			strTemplate += "<div id=\"ShowSubmitMasker\">"
			strTemplate += "<div title=\"驾考理论考试系统\" operate=\"ShowSubmit\" id=\"ShowSubmit\">";
			strTemplate += "</div>";
			strTemplate += "</div>";
			
			/***************************************************************************************
			*全真模拟第一屏内容,
			****************************************************************************************/
			strTemplate += "<div operate=\"Kscontianer\" id=\"frmFirstKaoshi\">";
			
			/***************************************************************************************
			*左边考试信息栏框架,考生信息
			****************************************************************************************/
			strTemplate += "<div operate=\"frmFirstLeft\" id=\"frmFirstLeft\">";
			strTemplate += "<fieldset id=\"frmKaoshitai\">";
			strTemplate += "<legend id=\"frmKaoshitaiTitle\">考试场</legend>";
			strTemplate += "<div id=\"frmKaoshitaiText\">第001号考试台</div>";
			strTemplate += "</fieldset>";
			strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
			/***************************************************************************************
			*考生信息
			****************************************************************************************/
			strTemplate += "<fieldset id=\"frmKaosheng\">";
			strTemplate += "<legend id=\"frmKaoshengTitle\">考生信息</legend>";
			strTemplate += "<div id=\"frmKaoshengBox\">";
			strTemplate += "<div><img src=\"template/images/xuesheng.jpg\" /></div>";
			strTemplate += "<div>姓名：001考生</div>";
			strTemplate += "<div>性别：男</div>";
			strTemplate += "<div>考试科目：XXX</div>";
			strTemplate += "<div>考试车型：XXX</div>";
			strTemplate += "<div>考试原因：XXX</div>";
			strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
			strTemplate += "<div id=\"frmMP3contianer\">";
			strTemplate += "<input class=\"button\" type=\"button\" isAuto=\"false\" id=\"mp3Button\" value=\"语音讲解\">";
			strTemplate += "</div>";
			strTemplate += "</div>";
			strTemplate += "</fieldset>";
			strTemplate += "</div>";
			/***************************************************************************************
			*左边栏考生信息完成,开启中间题目框架栏
			****************************************************************************************/
			strTemplate += "<fieldset id=\"frmFirstMiddle\">";
			strTemplate += "<legend id=\"frmDaticontianerTitle\">考试题目</legend>";
			strTemplate += "<div id=\"frmQuestion\">";
			strTemplate += "<div id=\"frmQuestionTitle\"></div>";
			strTemplate += "<div id=\"frmQuestionOptions\"></div>";
			strTemplate += "</div>";
			/**************************************************************************************
			*答题栏
			***************************************************************************************/
			strTemplate += "<div id=\"frmDomit\">";
			strTemplate += "<div id=\"frmMyAnswer\"><span>您的答案:</span><span id=\"frmMAtext\"></span></div>";
			strTemplate += "<div id=\"FrmAbcdmit\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmTrue\" value=\"正确\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmFalse\" value=\"错误\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmA\" value=\"A\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmB\" value=\"B\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmC\" value=\"C\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmD\" value=\"D\">";
			strTemplate += "</div>";
			strTemplate += "</div>";
			strTemplate += "</fieldset>";
			/***************************************************************************************
			*中间题目框架加载完成,开启右边答题选项卡
			****************************************************************************************/
			strTemplate += "<fieldset id=\"frmFirstRight\">";
			strTemplate += "<legend id=\"frmKaoshitaiTitle\">答题信息</legend>";
			strTemplate +="<div id=\"frmKOption\"></div>";
			strTemplate += "</fieldset>";
			
			strTemplate += "</div>";
			/***************************************************************************************
			*第一屏考试信息加载完成
			****************************************************************************************/
			strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
			/***************************************************************************************
			*绘制第二屏考试信息
			****************************************************************************************/
			strTemplate += "<div operate=\"frmSecondKaoshi\" id=\"frmSecondKaoshi\">";
			strTemplate += "<fieldset id=\"frmIntervalBar\">";
			strTemplate += "<legend id=\"frmIntervalTitle\">剩余时间</legend>";
			strTemplate += "<div id=\"frmInterval\">45:00:00</div>";
			strTemplate += "</fieldset>";
			
			strTemplate += "<fieldset id=\"frmSecondTipsbar\">";
			strTemplate += "<legend>提示信息</legend>";
			strTemplate += "<div id=\"frmSecondTips\">判断题,判断对错</div>";
			strTemplate += "</fieldset>";
			/***************************************************************************************
			*第二排开始按钮信息
			****************************************************************************************/
			strTemplate += "<div id=\"frmSecondButtons\">";
			strTemplate += "<input class=\"button\" type=\"button\" id=\"FrmUpper\" value=\"上一题\">";
			strTemplate += "<input class=\"button\" type=\"button\" id=\"FrmNext\" value=\"下一题\">";
			strTemplate += "<input class=\"button\" type=\"button\" id=\"FrmSubmit\" value=\"交卷\">";
			strTemplate += "<input class=\"button\" type=\"button\" id=\"FrmWrong\" value=\"错题练习\">";
			strTemplate += "<span id=\"FrmResult\"></span>";
			strTemplate += "</div>";
			
			strTemplate += "</div>";
			strTemplate += "<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
			/***************************************************************************************
			*第三屏答题图片资源信息
			****************************************************************************************/
			strTemplate += "<div operate=\"frmThirdKaoshi\" id=\"frmThirdKaoshi\">";
			strTemplate += "<div title=\"答题技巧控件\" id=\"frmTechniques\"></div>";
			strTemplate += "<div title=\"考试答题图片信息\" id=\"frmThumb\"></div>";
			strTemplate += "</div>";
			strTemplate += "<audio title=\"播放语音控件\" autoplay id=\"frmPlayer\"></audio>";

			/***************************************************************************************
			*完成渲染考试答题模块界面信息
			****************************************************************************************/
			strTemplate += "</div>";
			$(contianer).html(strTemplate);
			/**************************************************************************************
			*设置答题模块控件
			***************************************************************************************/
			Operate['frmQuestionTitle'] = document.querySelector("#frmQuestionTitle") || null;
			Operate['frmQuestion'] = document.querySelector("#frmQuestion") || null;
			Operate['frmQuestionOptions'] = document.querySelector("#frmQuestionOptions") || null;
			Operate['frmThumb'] = document.querySelector("#frmThumb") || null;
			/**************************************************************************************
			*设置答题模块控件
			***************************************************************************************/
			Operate['frmMAtext'] = document.querySelector("#frmMAtext") || null;
			/**************************************************************************************
			*设置答题选择项信息
			***************************************************************************************/
			Operate['FrmTrue'] = document.querySelector("#FrmTrue") || null;
			Operate['FrmFalse'] = document.querySelector("#FrmFalse") || null;
			Operate['FrmA'] = document.querySelector("#FrmA") || null;
			Operate['FrmB'] = document.querySelector("#FrmB") || null;
			Operate['FrmC'] = document.querySelector("#FrmC") || null;
			Operate['FrmD'] = document.querySelector("#FrmD") || null;
			Operate['FrmOk'] = document.querySelector("#FrmOk") || null;
			Operate['FrmUpper'] = document.querySelector("#FrmUpper") || null;
			Operate['FrmNext'] = document.querySelector("#FrmNext") || null;
			/**************************************************************************************
			*设置选项卡控件
			***************************************************************************************/
			Operate['frmKOption'] = document.querySelector("#frmKOption") || null;
			/**************************************************************************************
			*显示题型信息
			***************************************************************************************/
			Operate['frmSecondTips'] = document.querySelector("#frmSecondTips") || null;
			/**************************************************************************************
			*答题技巧
			***************************************************************************************/
			Operate['frmTechniques'] = document.querySelector("#frmTechniques") || null;
			/**************************************************************************************
			*播放语音
			***************************************************************************************/
			Operate['frmPlayer'] = document.querySelector("#frmPlayer") || null;
			Operate['mp3Button'] = document.querySelector("#mp3Button") || null;
			Operate['frmRemind'] = document.querySelector("#frmRemind") || null;
			/**************************************************************************************
			*几个弹出提示框控件信息
			***************************************************************************************/
			Operate['ShowError'] = document.querySelector("#ShowError") || null;
			Operate['ShowErrorMasker'] = document.querySelector("#ShowErrorMasker") || null;
			Operate['ShowSubmit'] = document.querySelector("#ShowSubmit") || null;
			Operate['FrmSubmit'] = document.querySelector("#FrmSubmit") || null;
			Operate['ShowSubmitMasker'] = document.querySelector("#ShowSubmitMasker") || null;
			Operate['frmInterval'] = document.querySelector("#frmInterval") || null;
			Operate['FrmWrong'] = document.querySelector("#FrmWrong") || null;
			/**************************************************************************************
			*加载数据事件
			***************************************************************************************/
			try{
				if(Operate['frmKOption']!=undefined && Operate['frmKOption']!=null 
				&& arrList!=undefined && arrList!=null && typeof(arrList)=='object')
				{
					renderOptions();
				}
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
							/******************************************************************
							*当前试题以回答,并且已答错的情况下
							*******************************************************************/
							try{
								var thisAnswerTrue = GetThisAnswer();
								if(thisAnswerTrue==-1){
									try{ShowKaoshi(SelectedIndex);}catch(err){}	
								}
								else if(thisAnswerTrue==0)
								{
									ShowErrorKaoshi(Operate['thisOption'],function(){
										try{setWrongTager();}catch(err){}
										try{ShowKaoshi(SelectedIndex);}catch(err){}
									});
								}
								else{
									try{setTrueTager();}catch(err){};
									try{ShowKaoshi(SelectedIndex);}catch(err){}	
								}
							}catch(err){}
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
							/******************************************************************
							*当前试题以回答,并且已答错的情况下
							*******************************************************************/
							try{
								var thisAnswerTrue = GetThisAnswer();
								if(thisAnswerTrue==-1){
									try{ShowKaoshi(SelectedIndex);}catch(err){}	
								}
								else if(thisAnswerTrue==0)
								{
									ShowErrorKaoshi(Operate['thisOption'],function(){
										try{setWrongTager();}catch(err){}
										try{ShowKaoshi(SelectedIndex);}catch(err){}
									});
								}
								else{
									try{setTrueTager();}catch(err){};
									try{ShowKaoshi(SelectedIndex);}catch(err){}	
								}
							}catch(err){}
						}catch(err){}
					});	
				}catch(err){}
			};
			/**************************************************************************************
			*开启语音
			***************************************************************************************/
			if(Operate['mp3Button']!=undefined && Operate['mp3Button']!=null)
			{
				try{
					$(Operate['mp3Button']).click(function(){
						if(Operate['thisFalse']!="true"){
							if(this.value=='语音讲解'){
								this.value="关闭语音";
								ShowPlayer();
							}
							else{
								this.value="语音讲解";
								ClosePlayer();
								/***********************************************
								*开始考试倒计时信息
								************************************************/
								if(Operate['isTimeup']!='true' 
								&& Operate['iSubmit']!='true')
								{try{StartInterval();}catch(err){};}
							}
						}
					});	
				}catch(err){}	
			};
			/**************************************************************************************
			*监听语音播放完成事件
			***************************************************************************************/
			if(Operate['frmPlayer']!=undefined && Operate['frmPlayer']!=null)
			{
				try{
					$(Operate['frmPlayer']).on("ended",function(){
						if($(Operate['mp3Button']).val()=='关闭语音'){
							var timer = 3;
							var inter = setInterval(function(){
								if(timer>=0){timer=timer-1;}
								else{
									clearInterval(inter);
									ShowPlayer();
								}
							},500);
						}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*设置答题卡选项A按钮事件
			****************************************************************************************/
			if(Operate['FrmA']!=undefined && Operate['FrmA']!=null)
			{
				try{
					$(Operate['FrmA']).click(function(){
						try{
							if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['QuestionMode']!=undefined
							&& Operate['thisOption']['QuestionMode']=="单选")
							{
								try{SingleA();}catch(err){};
							}
							else if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['QuestionMode']!=undefined
							&& Operate['thisOption']['QuestionMode']=="多选")
							{
								try{MultipleA();}catch(err){};	
							}
						}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*设置答题卡选项B按钮事件
			****************************************************************************************/
			if(Operate['FrmB']!=undefined && Operate['FrmB']!=null)
			{
				try{
					$(Operate['FrmB']).click(function(){
						try{
							if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['QuestionMode']!=undefined
							&& Operate['thisOption']['QuestionMode']=="单选")
							{
								try{SingleB();}catch(err){};
							}
							else if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['QuestionMode']!=undefined
							&& Operate['thisOption']['QuestionMode']=="多选")
							{
								try{MultipleB();}catch(err){};	
							}
						}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*设置答题卡选项C按钮事件
			****************************************************************************************/
			if(Operate['FrmC']!=undefined && Operate['FrmC']!=null)
			{
				try{
					$(Operate['FrmC']).click(function(){
						try{
							if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['QuestionMode']!=undefined
							&& Operate['thisOption']['QuestionMode']=="单选")
							{
								try{SingleC();}catch(err){};
							}
							else if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['QuestionMode']!=undefined
							&& Operate['thisOption']['QuestionMode']=="多选")
							{
								try{MultipleC();}catch(err){};	
							}
						}catch(err){}
					});	
				}catch(err){}	
			};
			/**************************************************************************************
			*设置答题卡选项D按钮事件
			***************************************************************************************/
			if(Operate['FrmD']!=undefined && Operate['FrmD']!=null)
			{
				try{
					$(Operate['FrmD']).click(function(){
						try{
							if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['QuestionMode']!=undefined
							&& Operate['thisOption']['QuestionMode']=="单选")
							{
								try{SingleD();}catch(err){};
							}
							else if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['QuestionMode']!=undefined
							&& Operate['thisOption']['QuestionMode']=="多选")
							{
								try{MultipleD();}catch(err){};	
							}
						}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*用户点击正确答题按钮事件
			****************************************************************************************/
			if(Operate['FrmTrue']!=undefined && Operate['FrmTrue']!=null)
			{
				try{
					$(Operate['FrmTrue']).click(function(){
						/************************************************************
						*标注选项信息
						*************************************************************/
						try{$(Operate['FrmTrue']).attr("iSelection","true");}
						catch(err){}
						try{$(Operate['FrmFalse']).attr("iSelection","false");}
						catch(err){}
						/************************************************************
						*判断答题信息
						*************************************************************/
						try{VerificationSelectionJudge("对");}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*用户点击错误答题按钮事件
			****************************************************************************************/
			if(Operate['FrmFalse']!=undefined && Operate['FrmFalse']!=null)
			{
				try{
					$(Operate['FrmFalse']).click(function(){
						/************************************************************
						*标注选项信息
						*************************************************************/
						try{$(Operate['FrmTrue']).attr("iSelection","false");}
						catch(err){}
						try{$(Operate['FrmFalse']).attr("iSelection","true");}
						catch(err){}
						/************************************************************
						*判断答题信息
						*************************************************************/
						try{VerificationSelectionJudge("错");}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*点击交卷按钮
			****************************************************************************************/
			if(Operate['FrmSubmit']!=undefined && Operate['FrmSubmit']!=null)
			{
				try{
					$(Operate['FrmSubmit']).click(function(){
						if(this.value=="交卷")
						{
							try{
								ShowSubmit(false,function(){
														 
								});
							}catch(err){};
						}else
						{
							if(confirm('你确定要关闭当前窗口，重新考试')){window.close();	}
						}								   
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*设置数据倒计时信息
			****************************************************************************************/
			if(Operate['frmInterval']!=undefined && Operate['frmInterval']!=null)
			{
				try{StartInterval();}catch(err){}
			};
			if(Operate['FrmWrong']!=undefined && Operate['FrmWrong']!=null){
				$(Operate['FrmWrong']).click(function(){
					alert(1);		  
				});	
			}
			/***************************************************************************************
			*加载默认试题信息
			****************************************************************************************/
			try{	
				var thisRecord = parseInt(options['thisRecord'])-1;
				if(thisRecord<=0){thisRecord=0;}
				ShowKaoshi(thisRecord);	
			}catch(err){}
			
		};
		/********************************************************************************************
		*提交试卷信息
		*********************************************************************************************/
		var ShowSubmit = function(iClosed,back)
		{
			if(!Operate['ShowSubmitMasker']){alert('获取配置信息错误,请重试');return false;}
			if(!Operate['ShowSubmit']){alert('获取配置信息错误,请重试');return false;}
			var strTemplate ="";
			/*************************************************************************************
			*开始加载弹出框内容信息
			**************************************************************************************/
			try{
				strTemplate +="<div id=\"frmWindowtitle\"><span class=\"name\">交卷</span></div>";
				strTemplate +="<div id=\"frmWindowclose\">关闭</div>";
				strTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:6px;\"></div>";
				strTemplate +="<div style=\"width:528px;margin:0px auto;\">";
				/*************************************************************************************
				*展示标题
				**************************************************************************************/
				strTemplate +="<div style=\"width:100%;background:#128782;height:42px;line-height:42px;color:#FF0000;font-size:32px;font-weight:900;text-align:center\">考试确认信息</div>";
				strTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:0px;\"></div>";
				/*************************************************************************************
				*操作提示
				**************************************************************************************/
				strTemplate +="<div style=\"padding:12px 8px;background:#3ec180;color:#323259;font-size:20px;font-weight:900;text-align:left\">操作提示</div>";
				strTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:6px;\"></div>";
				var Tuernumber = 0;var Undernumber = 0;var Falsenumber=0;
				var Achievement = 0;
				try{
					$(arrList['items']).each(function(k,Json){
						if(Json['isTrue']=="1"){Tuernumber=Tuernumber+1;}
						else if(Json['isTrue']=="0"){Falsenumber=Falsenumber+1;}
						else{Undernumber=Undernumber+1;}
					});
				}catch(err){}
				try{
					var Lengthnumber = parseInt(arrList['items'].length) || 0;
					var Points = parseFloat((100/Lengthnumber));
					Achievement  = parseInt(Tuernumber * Points);
				}catch(rr){}
				/*************************************************************************************
				*展示考试结果
				**************************************************************************************/
				strTemplate +="<div style=\"padding:12px 8px;background:#3ec180;color:#323259;font-size:20px;font-weight:900;text-align:left\">";
				strTemplate +="<div style=\"padding:6px 0px;font-size:18px;\">";
				strTemplate +="你本次考试当前得分"+Achievement+"分，答错"+Falsenumber+"题,还有"+Undernumber+"题没回答."
				strTemplate +="</div>";
				strTemplate +="<div style=\"padding:6px 0px;font-size:18px;\">";
				strTemplate +="1、点击【确认交卷】将提交考试成绩,考试结束!"
				strTemplate +="</div>";
				strTemplate +="<div style=\"padding:6px 0px;font-size:18px;\">";
				strTemplate +="2、点击【继续考试】将关闭本窗口，考试继续!"
				strTemplate +="</div>";
				strTemplate +="</div>";
				strTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:6px;\"></div>";
				/*************************************************************************************
				*展示操作按钮
				**************************************************************************************/
				strTemplate +="<div style=\"padding:12px 8px;background:#8080c0;color:#323259;font-size:20px;font-weight:900;text-align:center; position:relative\">";
				strTemplate +="<input id=\"frmConfirm\" type=\"button\" style=\"width:140px;height:46px;font-size:14px;\" value=\"确认交卷\" />";
				strTemplate +="<input id=\"frmSubmitClose\" type=\"button\" style=\"width:140px;font-size:14px;;margin-left:20px;;height:46px;\" value=\"继续考试\" />";
				strTemplate +="</div>";
				strTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
				strTemplate +="</div>";
				
			}catch(err){}
			/*************************************************************************************
			*以下是操作按钮信息
			**************************************************************************************/
			$(Operate['ShowSubmit']).html(strTemplate);
			$(Operate['ShowSubmitMasker']).show();
			/*************************************************************************************
			*关闭弹出框信息内容
			**************************************************************************************/
			var closeWindows = function(){
				if(iClosed!=true){
					$(Operate['ShowSubmitMasker']).hide();
					if(back!=undefined && back!=null 
					&& typeof(back)=='function'){
						try{back();}catch(err){}	
					};
				}
			};
			/*************************************************************************************
			*设置点击事件信息
			**************************************************************************************/
			$(Operate['ShowSubmit']).find("#frmWindowclose").click(function(){
				try{closeWindows();}catch(err){}												
			});
			$(Operate['ShowSubmit']).find("#frmSubmitClose").click(function(){
				try{closeWindows();}catch(err){}																
			});
			/*************************************************************************************
			*点击确认交卷事件
			**************************************************************************************/
			$(Operate['ShowSubmit']).find("#frmConfirm").click(function(){
				/***************************************************************************************
				*将当前试题更改为不可答题状态
				****************************************************************************************/
				try{
					$(Operate['FrmTrue']).attr("disabled","disabled");
					$(Operate['FrmFalse']).attr("disabled","disabled");
					$(Operate['FrmA']).attr("disabled","disabled");
					$(Operate['FrmB']).attr("disabled","disabled");
					$(Operate['FrmC']).attr("disabled","disabled");
					$(Operate['FrmD']).attr("disabled","disabled");	
				}catch(err){}
				/*********************************************************************************
				* 关闭交卷按钮信息
				**********************************************************************************/
				try{$(Operate['FrmSubmit']).val('重新考试');}catch(err){}
				try{Operate['iSubmit']='true';}catch(err){}
				var frmTemplate = "";
				try{
					frmTemplate +="<div id=\"frmWindowtitle\"><span class=\"name\">查看成绩</span></div>";
					frmTemplate +="<div id=\"frmWindowclose\">关闭</div>";
					frmTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:6px;\"></div>";
					frmTemplate +="<div style=\"width:528px;margin:0px auto;\">";
					/*************************************************************************************
					*展示标题
					**************************************************************************************/
					frmTemplate +="<div style=\"width:100%;background:#128782;height:42px;line-height:42px;color:#FF0000;font-size:32px;font-weight:900;text-align:center\">考试确认信息</div>";
					frmTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:0px;\"></div>";
					/*************************************************************************************
					*操作提示
					**************************************************************************************/
					frmTemplate +="<div style=\"padding:12px 8px;background:#3ec180;color:#323259;font-size:20px;font-weight:900;text-align:left\">操作提示</div>";
					frmTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:6px;\"></div>";
					var Tuernumber = 0;var Undernumber = 0;var Falsenumber=0;
					var Achievement = 0;
					try{
						$(arrList['items']).each(function(k,Json){
							if(Json['isTrue']=="1"){Tuernumber=Tuernumber+1;}
							else if(Json['isTrue']=="0"){Falsenumber=Falsenumber+1;}
							else{Undernumber=Undernumber+1;}
						});
					}catch(err){}
					try{
						var Lengthnumber = parseInt(arrList['items'].length) || 0;
						var Points = parseFloat((100/Lengthnumber));
						Achievement  = parseInt(Tuernumber * Points);
					}catch(rr){}
					/*************************************************************************************
					*展示考试结果
					**************************************************************************************/
					try{
						$("#FrmResult").html("得分:"+Achievement+"");
					}catch(err){}
					/*************************************************************************************
					*展示考试结果
					**************************************************************************************/
					frmTemplate +="<div style=\"padding:12px 8px;background:#3ec180;color:#323259;font-size:20px;height:100px;font-weight:900;text-align:left\">";
					if(Achievement>=90){
						frmTemplate +="<div style=\"padding:6px 0px;font-size:18px;\">";
						frmTemplate +="恭喜你,你本次的考试成绩为"+Achievement+"分,考试合格";
						frmTemplate +="</div>";	
					}else{
						frmTemplate +="<div style=\"padding:6px 0px;font-size:18px;\">";
						frmTemplate +="很遗憾,你本次的考试成绩为"+Achievement+"分,考试不合格";
						frmTemplate +="</div>";	
					}
					frmTemplate +="</div>";
					frmTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:6px;\"></div>";
					/*************************************************************************************
					*展示操作按钮
					**************************************************************************************/
					frmTemplate +="<div style=\"padding:12px 8px;background:#8080c0;color:#323259;font-size:20px;font-weight:900;text-align:center; position:relative\">";
					frmTemplate +="<input id=\"frmOk\" type=\"button\" style=\"width:140px;height:46px;font-size:14px;\" value=\"确认\" />";
					frmTemplate +="</div>";
					frmTemplate +="<div style=\"clear:both;width:100%;font-size:0px;height:10px;\"></div>";
					frmTemplate +="</div>";
					
				}catch(err){}
				
				$(Operate['ShowSubmit']).html(frmTemplate);
				/************************************************************************************
				*关闭窗口
				*************************************************************************************/
				$(Operate['ShowSubmit']).find("#frmOk").click(function(){
					try{$(Operate['ShowSubmitMasker']).hide();}
					catch(err){}															
				});
				
			});
		}
		/********************************************************************************************
		*获取当前试卷下的答题信息
		*********************************************************************************************/
		var GetThisAnswer = function()
		{
			var thisAnswerTrue = -1;
			if(Operate['thisOption']['isTrue']!=undefined 
			&& Operate['thisOption']['isTrue']!=null 
			&& Operate['thisOption']['isTrue']!="")
			{thisAnswerTrue = parseInt(Operate['thisOption']['isTrue']);	};
			if(thisAnswerTrue!=0 && thisAnswerTrue!=1){thisAnswerTrue=-1;}
			if(isNaN(thisAnswerTrue)){thisAnswerTrue=-1;}
			return thisAnswerTrue;
		};
		/********************************************************************************************
		*获取多选题用户选择的答案信息
		*********************************************************************************************/
		var GetMultiple = function()
		{
			var absText = "";
			try{
				if(Operate['frmQuestionOptions']!=undefined && Operate['frmQuestionOptions']!=null){
					$(Operate['frmQuestionOptions']).find("input[operate=\"autoMul\"]").each(function(){
						if(this.checked!=undefined && this.checked!=null && this.checked){
							if(absText!=""){absText=absText+this.value;}
							else{absText=this.value;}
						}																	
					});	
				}
			}catch(err){}
			try{VerificationSelectionMultiple(absText);}catch(err){}	
		}
		/********************************************************************************************
		*设置用户选择的答案信息
		*********************************************************************************************/
		var SetAnswer = function(absText)
		{
			try{$(Operate['frmMAtext']).html(absText);}catch(err){}
			try{
				if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
				&& typeof(Operate['thisOption'])=='object')
				{Operate['thisOption']['thisAnswer'] = absText;}
			}catch(err){}
			/***************************************************************************************
			*设置选项卡当中选择答案信息
			****************************************************************************************/
			try{
				var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
				SelectedIndex=SelectedIndex+1;
				var strTemplate ="";
				strTemplate +="<span>"+SelectedIndex+"</span>";
				strTemplate +="<span>"+absText+"</span>";
				$("#frmOptionsTabs").find("td[value=\""+SelectedIndex+"\"]").html(strTemplate);
			}catch(err){}
		};
		/********************************************************************************************
		*标注错误信息
		*********************************************************************************************/
		var setWrongTager = function(){
			try{
				var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
				/****************************************************************************************
				*记录当前答题的日志记录信息
				*****************************************************************************************/
				try{
					if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
					&& Operate['thisOption']['QuetID']!=undefined && Operate['thisOption']['QuetID']!=null 
					&& Operate['thisOption']['QuetID']!="" && Operate['thisOption']['thisAnswer']!=undefined 
					&& Operate['thisOption']['thisAnswer']!=null && Operate['thisOption']['thisAnswer']!="" 
					&& Operate['thisOption']['abcText']!=undefined && Operate['thisOption']['abcText']!=null
					&& Operate['thisOption']['abcText']!="")
					{
						var QuetID = parseInt(Operate['thisOption']['QuetID']) || 0;
						var thisAnswer = Operate['thisOption']['thisAnswer'] || "";
						var isTrue = GetThisAnswer ();
						eKsList[QuetID] = {"thisAnswer":thisAnswer,"isTrue":isTrue};
						try{SavePaperLogs(arrList,SelectedIndex);}
						catch(err){}
					};
				}catch(err){alert(err.message);}
				/********************************************************************************************
				*显示用户错误与正确信息
				*********************************************************************************************/
				try{
					var length = parseInt(arrList['items'].length) || 0;
					SelectedIndex=SelectedIndex+1;
					if(SelectedIndex>=length){SelectedIndex=length;}
					$("#frmOptionsTabs").find("td[value=\""+SelectedIndex+"\"]").attr("answer","false");
				}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*标注错误信息
		*********************************************************************************************/
		var setTrueTager = function(){
			try{
				var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
				/****************************************************************************************
				*记录当前答题的日志记录信息
				*****************************************************************************************/
				try{
					if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
					&& Operate['thisOption']['QuetID']!=undefined && Operate['thisOption']['QuetID']!=null 
					&& Operate['thisOption']['QuetID']!="" && Operate['thisOption']['thisAnswer']!=undefined 
					&& Operate['thisOption']['thisAnswer']!=null && Operate['thisOption']['thisAnswer']!="" 
					&& Operate['thisOption']['abcText']!=undefined && Operate['thisOption']['abcText']!=null
					&& Operate['thisOption']['abcText']!="")
					{
						var QuetID = parseInt(Operate['thisOption']['QuetID']) || 0;
						var thisAnswer = Operate['thisOption']['thisAnswer'] || "";
						var isTrue = GetAnswerTrue();
						eKsList[QuetID] = {"thisAnswer":thisAnswer,"isTrue":"true"};
						try{SavePaperLogs(arrList,SelectedIndex);}
						catch(err){}
					};
				}catch(err){;}
				/********************************************************************************************
				*显示用户错误与正确信息
				*********************************************************************************************/
				try{
					var length = parseInt(arrList['items'].length) || 0;
					SelectedIndex=SelectedIndex+1;
					if(SelectedIndex>=length){SelectedIndex=length;}
					$("#frmOptionsTabs").find("td[value=\""+SelectedIndex+"\"]").attr("answer","true");
				}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*答题卡选项按钮多选题选择A时的情况
		*********************************************************************************************/
		var MultipleA = function(iChecked)
		{
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
				if(autoChecked.checked){$(Operate['FrmA']).attr("iSelection","true");}
				else{$(Operate['FrmA']).attr("iSelection","false");}
				/***************************************************************************************
				*标注用户选择的答案
				****************************************************************************************/
				try{GetMultiple();}catch(err){}
				
			}catch(err){}
		};
		/********************************************************************************************
		*单选题选择A时的情况
		*********************************************************************************************/
		var SingleA = function()
		{
			/***************************************************************************************
			*标注选择答案信息
			****************************************************************************************/
			try{
				$(Operate['FrmB']).attr("iSelection","false");
				$(Operate['FrmC']).attr("iSelection","false");
				$(Operate['FrmD']).attr("iSelection","false");
				$(Operate['FrmA']).attr("iSelection","true");
			}catch(err){alert('获取试题信息错误,请重试!');}
			/***************************************************************************************
			*验证选择答案是否正确
			****************************************************************************************/
			try{VerificationSelectionSingle("A");}
			catch(err){}
		};
		
		/********************************************************************************************
		*答题卡选项按钮多选题选择A时的情况
		*********************************************************************************************/
		var MultipleB = function(iChecked)
		{
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
				if(autoChecked.checked){$(Operate['FrmB']).attr("iSelection","true");}
				else{$(Operate['FrmB']).attr("iSelection","false");}
				/***************************************************************************************
				*标注用户选择的答案
				****************************************************************************************/
				try{GetMultiple();}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*单选题选择A时的情况
		*********************************************************************************************/
		var SingleB = function()
		{
			/***************************************************************************************
			*标注选择答案信息
			****************************************************************************************/
			try{
				$(Operate['FrmB']).attr("iSelection","true");
				$(Operate['FrmC']).attr("iSelection","false");
				$(Operate['FrmD']).attr("iSelection","false");
				$(Operate['FrmA']).attr("iSelection","false");
			}catch(err){alert('获取试题信息错误,请重试!');}
			/***************************************************************************************
			*验证选择答案是否正确
			****************************************************************************************/
			try{VerificationSelectionSingle("B");}
			catch(err){}
		};
		
		/********************************************************************************************
		*答题卡选项按钮多选题选择A时的情况
		*********************************************************************************************/
		var MultipleC = function(iChecked)
		{
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
				if(autoChecked.checked){$(Operate['FrmC']).attr("iSelection","true");}
				else{$(Operate['FrmC']).attr("iSelection","false");}
				/***************************************************************************************
				*标注用户选择的答案
				****************************************************************************************/
				try{GetMultiple();}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*单选题选择A时的情况
		*********************************************************************************************/
		var SingleC = function()
		{
			/***************************************************************************************
			*标注选择答案信息
			****************************************************************************************/
			try{
				$(Operate['FrmB']).attr("iSelection","false");
				$(Operate['FrmC']).attr("iSelection","true");
				$(Operate['FrmD']).attr("iSelection","false");
				$(Operate['FrmA']).attr("iSelection","false");
			}catch(err){alert('获取试题信息错误,请重试!');}
			/***************************************************************************************
			*验证选择答案是否正确
			****************************************************************************************/
			try{VerificationSelectionSingle("C");}
			catch(err){}
		};
		
		/********************************************************************************************
		*答题卡选项按钮多选题选择A时的情况
		*********************************************************************************************/
		var MultipleD = function(iChecked)
		{
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
				if(autoChecked.checked){$(Operate['FrmD']).attr("iSelection","true");}
				else{$(Operate['FrmD']).attr("iSelection","false");}
				/***************************************************************************************
				*标注用户选择的答案
				****************************************************************************************/
				try{GetMultiple();}catch(err){}
			}catch(err){}
		};
		/********************************************************************************************
		*单选题选择A时的情况
		*********************************************************************************************/
		var SingleD = function()
		{
			/***************************************************************************************
			*标注选择答案信息
			****************************************************************************************/
			try{
				$(Operate['FrmB']).attr("iSelection","false");
				$(Operate['FrmC']).attr("iSelection","");
				$(Operate['FrmD']).attr("iSelection","true");
				$(Operate['FrmA']).attr("iSelection","false");
			}catch(err){alert('获取试题信息错误,请重试!');}
			/***************************************************************************************
			*验证选择答案是否正确
			****************************************************************************************/
			try{VerificationSelectionSingle("D");}
			catch(err){}
		};
		
		/********************************************************************************************
		*验证用户选择的答案信息,单项选择
		*********************************************************************************************/
		var VerificationSelectionSingle = function(SelectionABC)
		{
			try{
				var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
				if(SelectedIndex<=0){SelectedIndex=0;}
				if(Operate['thisOption']['abcText']!=undefined 
				&& Operate['thisOption']['abcText']!=null 
				&& Operate['thisOption']['abcText']!="" 
				&& SelectionABC!=undefined && SelectionABC!=null 
				&& SelectionABC!="" && typeof(arrList)=='object' 
				&& typeof(arrList['items'])=='object' && arrList['items'][SelectedIndex])
				{
					
					try{
						if(SelectionABC==Operate['thisOption']['abcText']){
							arrList['items'][SelectedIndex]['isTrue'] = "1";
							Operate['thisOption']['isTrue']="1";
						}else{
							arrList['items'][SelectedIndex]['isTrue'] = "0";
							Operate['thisOption']['isTrue']="0";
						}
					}catch(err){}
				}
				else{arrList['items'][SelectedIndex]['isTrue'] = "-1";}
			}catch(err){}
			/*******************************************************************************
			*设置我选择的答案信息
			********************************************************************************/
			try{SetAnswer(SelectionABC);}catch(err){}
			
		};
		/********************************************************************************************
		*验证用户选择的答案信息,单项选择
		*********************************************************************************************/
		var VerificationSelectionJudge = function(SelectionABC)
		{
			try{
				var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
				if(SelectedIndex<=0){SelectedIndex=0;}
				if(Operate['thisOption']['abcText']!=undefined 
				&& Operate['thisOption']['abcText']!=null 
				&& Operate['thisOption']['abcText']!="" 
				&& SelectionABC!=undefined && SelectionABC!=null 
				&& SelectionABC!="" && typeof(arrList)=='object' 
				&& typeof(arrList['items'])=='object' && arrList['items'][SelectedIndex])
				{
					try{
						if(SelectionABC==Operate['thisOption']['abcText']){
							arrList['items'][SelectedIndex]['isTrue'] = "1";
							Operate['thisOption']['isTrue']="1";
						}else{
							arrList['items'][SelectedIndex]['isTrue'] = "0";
							Operate['thisOption']['isTrue']="0";
						}
					}catch(err){}
				}
				else{arrList['items'][SelectedIndex]['isTrue'] = "-1";}
			}catch(err){}
			/*******************************************************************************
			*设置我选择的答案信息
			********************************************************************************/
			try{SetAnswer(SelectionABC);}catch(err){}
			
		};
		
		/********************************************************************************************
		*验证用户多选题回答的答案信息
		*********************************************************************************************/
		var VerificationSelectionMultiple = function(SelectionABC)
		{
			try{
				var SelectedIndex = parseInt(Operate['SelectedIndex']) || 0;
				if(SelectedIndex<=0){SelectedIndex=0;}
				if(Operate['thisOption']['abcText']!=undefined 
				&& Operate['thisOption']['abcText']!=null 
				&& Operate['thisOption']['abcText']!="" 
				&& SelectionABC!=undefined && SelectionABC!=null 
				&& SelectionABC!="" && typeof(arrList)=='object' 
				&& typeof(arrList['items'])=='object' && arrList['items'][SelectedIndex])
				{
					try{
						if(SelectionABC==Operate['thisOption']['abcText']){
							arrList['items'][SelectedIndex]['isTrue'] = "1";
							Operate['thisOption']['isTrue']="1";
						}else{
							arrList['items'][SelectedIndex]['isTrue'] = "0";
							Operate['thisOption']['isTrue']="0";
						}
					}catch(err){}
				}
				else{arrList['items'][SelectedIndex]['isTrue'] = "-1";}
			}catch(err){}
			/*******************************************************************************
			*设置我选择的答案信息
			********************************************************************************/
			try{SetAnswer(SelectionABC);}catch(err){}
		};
		
		/********************************************************************************************
		*用户回答正确以后的提示信息
		*********************************************************************************************/
		var AnswerTrue = function()
		{
			/****************************************************************************************
			*将当前试题标注为已答题
			*****************************************************************************************/
			try{$("#frmCurrent").attr("Right","true");}
			catch(err){}
			/****************************************************************************************
			*记录当前答题的日志记录信息
			*****************************************************************************************/
			try{
				if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
				&& Operate['thisOption']['QuetID']!=undefined && Operate['thisOption']['QuetID']!=null 
				&& Operate['thisOption']['QuetID']!="" && Operate['thisOption']['AnswerText']!=undefined 
				&& Operate['thisOption']['AnswerText']!=null && Operate['thisOption']['AnswerText']!="")
				{
					var QuetID = parseInt(Operate['thisOption']['QuetID']) || 0;
					var AnswerText = Operate['thisOption']['AnswerText'] || "";
					var FirTrue = "false";
					if(eKsList[QuetID]!=undefined && eKsList[QuetID]!=null 
					&& typeof(eKsList[QuetID])=='object')
					{FirTrue = eKsList[QuetID]["FirTrue"] || "false";}
					if(FirTrue==undefined || FirTrue==null || FirTrue=="")
					{FirTrue="true";}
					try{
						var eOption = {
							"AnswerText":AnswerText,
							"isTrue":"true",
							"FirTrue":FirTrue
						};
						eKsList[QuetID] = eOption;
					}catch(err){}	
				};
			}catch(err){;}
			/****************************************************************************************
			*首先关闭播放语音
			*****************************************************************************************/
			if(Operate['mp3Button'].value=="语音讲解" && Operate['thisFalse']!=undefined 
			&& Operate['thisFalse']!=null && Operate['thisFalse']!="false")
			{try{ClosePlayer();}catch(err){};}
			/****************************************************************************************
			*播放提示语音
			*****************************************************************************************/
			try{$(Operate['frmRemind']).attr("src","inc/true.mp3");}
			catch(err){}
			/****************************************************************************************
			*执行延时跳转信息
			*****************************************************************************************/
			try{
				if(document.querySelector('#frmGoto').checked){
					var timerout = setTimeout(function(){
						try{							 
							clearTimeout(timerout);
							$(Operate['FrmNext']).click();
						}catch(err){}
					},1500);
				}
			}
			catch(err){}
		};
		/********************************************************************************************
		*用户回答错误以后的提示信息
		*********************************************************************************************/
		var AnswerFalse = function()
		{
			/****************************************************************************************
			*将当前试题标注为已答题
			*****************************************************************************************/
			try{$("#frmCurrent").attr("Right","true");}
			catch(err){}
			/****************************************************************************************
			*记录当前答题的日志记录信息
			*****************************************************************************************/
			try{
				if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
				&& Operate['thisOption']['QuetID']!=undefined && Operate['thisOption']['QuetID']!=null 
				&& Operate['thisOption']['QuetID']!="" && Operate['thisOption']['AnswerText']!=undefined 
				&& Operate['thisOption']['AnswerText']!=null && Operate['thisOption']['AnswerText']!="")
				{
					var QuetID = parseInt(Operate['thisOption']['QuetID']) || 0;
					var AnswerText = Operate['thisOption']['AnswerText'] || "";
					var FirTrue = "false";
					if(eKsList[QuetID]!=undefined && eKsList[QuetID]!=null 
					&& typeof(eKsList[QuetID])=='object')
					{FirTrue = eKsList[QuetID]["FirTrue"] || "false";}
					if(FirTrue==undefined || FirTrue==null || FirTrue=="")
					{FirTrue="true";}
					try{
						var eOption = {
							"AnswerText":AnswerText,
							"isTrue":"true",
							"FirTrue":FirTrue
						};
						eKsList[QuetID] = eOption;
					}catch(err){}	
				};
			}catch(err){;}
			/****************************************************************************************
			*开始组织
			*****************************************************************************************/
			try{
				if(Operate['thisFalse']!="true"){
					try{Operate['thisFalse']="true";}catch(err){}
					try{ShowPlayer();}catch(err){}
					try{SaveFalse(Operate['thisOption']);}catch(err){}
				}
			}catch(err){}
		};
		
		/********************************************************************************************
		*开始执行数据
		*********************************************************************************************/
		try{
			var strXml = options['xml'] || "<configurationRoot></configurationRoot>";
			var arrList = arrList || window.thisList;
			try{render();}catch(err){}
		}catch(err){};
		/********************************************************************************************
		*存储用户答题卡信息
		*********************************************************************************************/
		var SavePaper = function()
		{
			try{
				if(eKsList!=undefined && eKsList!=null && typeof(eKsList)=='object' 
				&& Operate['SelectedIndex']!=undefined && Operate['SelectedIndex']!="")
				{
					var thisRecord = parseInt(Operate['SelectedIndex']) || 0;
					SavePaperLogs(eKsList,thisRecord,function(iSuccess){
						try{
							var timer = 120;
							var inter = setInterval(function(){
								if(timer>=0){timer=timer-1;}
								else{
									clearInterval(inter);
									console.log('重新计时存储');
									SavePaper();
								}
							},1000);
						}catch(err){}
					});		
				};
			}catch(err){}
		}
		
	};

})(jQuery);
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
}
/**************************************************************************************
*保存答题记录日志信息
***************************************************************************************/
var SavePaperLogs = function(options,thisRecord,back)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& options!=undefined && options!=null && typeof(options)=='object'
		&& cfg['KaoshiID']!=undefined && cfg['KaoshiID']!=null && cfg['KaoshiID']!=""
		&& cfg['PaperID']!=undefined && cfg['PaperID']!=null && cfg['PaperID']!=""
		&& options!=undefined && options!=null && typeof(options)=="object")
		{
			var SendOption = {};
			SendOption["url"]="Kaoshi.aspx?action=saveLogs";
			SendOption['type']="post";
			SendOption["data"] = {
				"thisRecord":(parseInt(thisRecord) || 0),
				"AnswerText":JSON.stringify(options),
				"KaoshiID":cfg['KaoshiID']
			};
			SendOption["success"] = function(strResponse){
				try{
					if(strResponse!="success")
					{console.log('记录操作日志:'+strResponse);return false;}
					else if(back!=undefined && back!=null 
					&& typeof(back)=='function'){
						back(true);
					}
				}catch(err){}
				
			};
			SendOption["error"] = function(){
				console.log('请求出错:'+strResponse);return false;
			};
			/*************************************************************************
			*开始请求数据
			**************************************************************************/
			try{
				if($.ajax!=undefined && $.ajax!=null){
					jQuery.ajax(SendOption);
				}
			}catch(err){}
		};
	}catch(err){alert(err.message);return false;};	
}