;(function($) {
"use strict";
	$.fn.eKaoshi = function(options)
	{
		var contianer = this;
		/********************************************************************************************
		*定义网页控件集合信息
		*********************************************************************************************/
		var Operate = {
			"SelectedIndex":0,
			"thisOption":{},
			"thisFalse":"false",
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
			try{ClosePlayer();}
			catch(err){alert("语音关闭失败,请重试");}
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
			else if(Operate['thisOption']['questionMode']==undefined){alert('无法加载试题类型！');return false;}
			else if(Operate['thisOption']['questionMode']==null){alert('无法加载试题类型！');return false;}
			else if(Operate['thisOption']['questionMode']==""){alert('无法加载试题类型！');return false;}
			else if(Operate['thisOption']['answerText']==undefined){alert('无法加载试题答案！');return false;}
			else if(Operate['thisOption']['answerText']==null){alert('无法加载试题答案！');return false;}
			else if(Operate['thisOption']['answerText']==""){alert('无法加载试题答案！');return false;}
			else if(Operate['thisOption']['strOptions']==undefined){alert('无法加载试题选项！');return false;}
			else if(Operate['thisOption']['strOptions']==null){alert('无法加载试题选项！');return false;}
			else if(Operate['thisOption']['questionMode']!="判断" 
			&& Operate['thisOption']['strOptions']==""){alert('无法加载试题选项！');return false;}
			
			
			try{
				
				/******************************************************************
				*显示主题
				*******************************************************************/
				try{
					var Subject = ""+(SelectedIndex+1)+"："+Operate['thisOption']['strTitle'];
					$(Operate['frmQueSubjict']).html(Subject);
				}catch(err){}	
				/******************************************************************
				*显示图片
				*******************************************************************/
				try{
					if(Operate['thisOption']['strthumb']!=undefined 
					&& Operate['thisOption']['strthumb']!=null 
					&& Operate['thisOption']['strthumb']!="")
					{
						$(Operate['frmQueThumb']).html("<img alt=\"点击查看大图\" title=\"点击查看大图\" src=\""+Operate['thisOption']['strthumb']+"\" />");
					}
					else{$(Operate['frmQueThumb']).html("");}
				}catch(err){}
				
				
				/******************************************************************
				*生成数组,并且将数组乱序显示出来
				*******************************************************************/
				var arrOptions = Operate['thisOption']['strOptions'].split("|") || [];
				arrOptions.sort(function(){ return 0.5 - Math.random();});
				
				/******************************************************************
				*生成答题选项,判断题将不显示选项
				*******************************************************************/
				if(Operate['thisOption']['questionMode']!="判断")
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
						if(Operate['thisOption']['questionMode']=="多选")
						{
							strTemplate +="<input id=\"auto"+autoText+"\" class=\"autoABCD\" type=\"checkbox\"";
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
					$(Operate['frmQueOption']).html(frmTemplate);
					/******************************************************************
					*定义选项事件信息
					*******************************************************************/
					if(Operate['thisOption']['questionMode']=="多选" 
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
				else{$(Operate['frmQueOption']).html("");}
				
				/******************************************************************
				*计算正确答案信息
				*******************************************************************/
				try{
					var answerText = Operate['thisOption']['answerText'] || "";
					var AnswerValue = "";
					if(Operate['thisOption']['questionMode']=="判断")
					{
						try{
							if(answerText!="正确" && answerText!="对"){AnswerValue="错";}
							else{AnswerValue = "对";}
						}catch(err){}
					}
					else if(Operate['thisOption']['questionMode']!="判断")
					{
						try{
							var ArrAnswer = answerText.split("|");
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
					*复制正确答案
					*******************************************************************/
					try{$(Operate['frmTAtext']).html(AnswerValue);}
					catch(err){}
					/******************************************************************
					*将正确答案复制到当前的选项卡当中
					*******************************************************************/
					try{Operate['thisOption']['abcText']=AnswerValue;}
					catch(err){alert('设置答案信息失败');return false;}
					
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
					$(Operate['FrmOk']).attr("iSelection","false");
					$(Operate['FrmTrue']).attr("iSelection","false");
					$(Operate['FrmFalse']).attr("iSelection","false");
				}catch(err){}
				/******************************************************************
				*判断显示的答题按钮信息
				*******************************************************************/
				try{
					if(Operate['thisOption']['questionMode']!=undefined 
					&& Operate['thisOption']['questionMode']!=null 
					&& Operate['thisOption']['questionMode']!="" 
					&& Operate['thisOption']['questionMode']=="判断")
					{
						try{
							$(Operate['FrmA']).hide();
							$(Operate['FrmB']).hide();
							$(Operate['FrmC']).hide();
							$(Operate['FrmD']).hide();
							$(Operate['FrmOk']).hide();
							$(Operate['FrmTrue']).show();
							$(Operate['FrmFalse']).show();	
						}catch(err){}
					}else if(Operate['thisOption']['questionMode']!=undefined 
					&& Operate['thisOption']['questionMode']!=null 
					&& Operate['thisOption']['questionMode']!="" 
					&& Operate['thisOption']['questionMode']=="单选")
					{
						try{
							$(Operate['FrmA']).show();
							$(Operate['FrmB']).show();
							$(Operate['FrmC']).show();
							$(Operate['FrmD']).show();
							$(Operate['FrmOk']).hide();
							$(Operate['FrmTrue']).hide();
							$(Operate['FrmFalse']).hide();	
						}catch(err){}	
					}else if(Operate['thisOption']['questionMode']!=undefined 
					&& Operate['thisOption']['questionMode']!=null 
					&& Operate['thisOption']['questionMode']!="" 
					&& Operate['thisOption']['questionMode']=="多选")
					{
						try{
							$(Operate['FrmA']).show();
							$(Operate['FrmB']).show();
							$(Operate['FrmC']).show();
							$(Operate['FrmD']).show();
							$(Operate['FrmOk']).show();
							$(Operate['FrmTrue']).hide();
							$(Operate['FrmFalse']).hide();	
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
			&& Operate['thisOption']['answerText']!=undefined 
			&& Operate['thisOption']['answerText']!=null 
			&& Operate['thisOption']['answerText']!="")
			{
				
			};		
		};
		/********************************************************************************************
		*显示考题分析信息
		*********************************************************************************************/
		var ShowOfficial = function()
		{
			try{
				if(Operate['thisOption']!=undefined 
				&& Operate['thisOption']!=null 
				&& typeof(Operate['thisOption'])=='object' 
				&& Operate['frmTechniques']!=undefined 
				&& Operate['frmTechniques']!=null
				&& Operate['thisOption']['officialText']!=undefined 
				&& Operate['thisOption']['officialText']!=null 
				&& Operate['thisOption']['officialText']!="")
				{
					$(Operate['frmOfficial']).val("关闭分析");
					$(Operate['frmOfficialControls']).html(Operate['thisOption']['officialText']);
					$(Operate['frmOfficialControls']).show();
				}
				else{alert('当前试题没有分析!');return false;};	
			}catch(err){}
		};
		/********************************************************************************************
		*关闭考题分析信息
		*********************************************************************************************/
		var CloseOfficial = function(){
			try{
				$(Operate['frmOfficial']).val("考题分析");
				$(Operate['frmOfficialControls']).html("");
				$(Operate['frmOfficialControls']).hide();
			}catch(err){}
		};
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
							$(Operate['FrmOk']).attr("disabled","disabled");
						}catch(err){}	
					};
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
						$(Operate['FrmTrue']).removeAttr("disabled");
						$(Operate['FrmFalse']).removeAttr("disabled");
						$(Operate['FrmA']).removeAttr("disabled");
						$(Operate['FrmB']).removeAttr("disabled");
						$(Operate['FrmC']).removeAttr("disabled");
						$(Operate['FrmD']).removeAttr("disabled");
						$(Operate['FrmOk']).removeAttr("disabled");
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
					strTemplate +=" style=\"max-width:100%;max-height:360px;display:line;margin:0px;\"/>";
					$(Operate['frmTechniques']).html(strTemplate);
					$(Operate['frmTechniques']).show();
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
				for(var k=1;k<=Length;k++)
				{
					frmTemplate+="<td value=\""+k+"\" operate=\"dbclick\">"+k+"</td>";
					if(SelectedIndex>=10){
						frmTemplate+="</tr><tr class=\"hback\">";
						SelectedIndex=1;
					}
					else{SelectedIndex=SelectedIndex+1;}
				};
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
				$(Operate['frmOptions']).html(frmTemplate);
			}catch(err){}
			/********************************************************************************************
			*增加选项卡点击事件
			*********************************************************************************************/
			try{
				$(Operate['frmOptions']).find("td[operate=\"dbclick\"]").click(function(){
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
			strTemplate += "<div id=\"frmQuestion\">";
			strTemplate += "<div id=\"frmQueSubjict\"></div>";
			strTemplate += "<div id=\"frmQueOption\"></div>";
			strTemplate += "<div id=\"frmQueThumb\"></div>";
			strTemplate += "</div>";
			/***************************************************************************************
			*答题栏
			****************************************************************************************/
			strTemplate += "<div id=\"frmDomit\">";
			strTemplate += "<div id=\"frmTrueAnswer\"><span>正确答案:</span><span style=\"display:none\" id=\"frmTAtext\"></span></div>";
			strTemplate += "<div id=\"frmMyAnswer\"><span>我的答案:</span><span id=\"frmMAtext\"></span></div>";
			strTemplate += "<div id=\"FrmAbcdmit\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmTrue\" value=\"正确\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmFalse\" value=\"错误\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmA\" value=\"A\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmB\" value=\"B\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmC\" value=\"C\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmD\" value=\"D\">";
			strTemplate += "<input class=\"button\" style=\"display:none\" type=\"button\" id=\"FrmOk\" value=\"选好了\">";
			strTemplate += "<input class=\"button\" type=\"button\" id=\"FrmUpper\" value=\"上一题\">";
			strTemplate += "<input class=\"button\" type=\"button\" id=\"FrmNext\" value=\"下一题\">";
			strTemplate += "</div>";
			strTemplate += "</div>";
			/***************************************************************************************
			*进度条
			****************************************************************************************/
			strTemplate += "<progress id=\"frmProcess\" value=\"300\" min=\"1\" max=\""+arrList['items'].length+"\">";
			strTemplate += "</progress>";
			/***************************************************************************************
			*工具栏
			****************************************************************************************/
			strTemplate += "<div id=\"frmTools\">";
			strTemplate += "<div id=\"frmPoints\"><span>首 正 率:</span><span id=\"frmPointValue\">0.00%</span></div>";
			strTemplate += "<label id=\"frmGotoLabel\"><input type=\"checkbox\" checked id=\"frmGoto\" value=\"1\"/><span id=\"frmGotoText\">答对后自动跳转</span></label>";
			strTemplate += "<div id=\"Frmbuttons\">";
			strTemplate += "<input class=\"button\" type=\"button\" isAuto=\"false\" id=\"mp3Button\" value=\"语音讲解\">";
			if(options!=undefined && options['channel']!=undefined 
			&& options['channel']!=null && options['channel']!='错题练习')
			{
				strTemplate += "<input class=\"button\" type=\"button\" isAuto=\"false\" id=\"sErrButton\" value=\"设为错题\">";	
			}
			else{
				strTemplate += "<input class=\"button\" type=\"button\" isAuto=\"false\" id=\"FrmEmptyButton\" value=\"清空错题\">";
				strTemplate +="<div id=\"frmEmpty\">";
				strTemplate +="<label id=\"FrmEmptybox\"><span class=\"ico\">";
				strTemplate +="<input type=\"checkbox\" value=\"1\"";
				if(options['isDelete']!=undefined && options['isDelete']!=null 
				&& options['isDelete']=='1')
				{strTemplate +=" checked ";}
				strTemplate +=" id=\"isEmpty\" />";
				strTemplate +="</span>";
				strTemplate +="<span class=\"name\">答对以后自动去除</span>";
				strTemplate +="</label>";
				strTemplate +="<label id=\"FrmEmptyThis\"><span class=\"ico\"></span><span class=\"name\">去除当前错题</span></label>";
				strTemplate +="<label id=\"FrmEmptyAll\"><span class=\"ico\"></span><span class=\"name\">清空所有错题</span></label>";
				strTemplate +="</div>";
			}
			strTemplate += "<input class=\"button\" type=\"button\" isAuto=\"false\" id=\"frmOfficial\" value=\"考题分析\">";
			strTemplate += "<input class=\"button\" type=\"button\" isAuto=\"false\" id=\"showAnsButton\" value=\"显示答案\">";
			strTemplate += "</div>";
			strTemplate += "</div>";
			/***************************************************************************************
			*答题卡
			****************************************************************************************/
			strTemplate += "<div title=\"答题选项卡控件\"  id=\"frmOptions\"></div>";
			strTemplate += "<div title=\"答题技巧控件\"  id=\"frmTechniques\"></div>";
			strTemplate += "<div title=\"考题分析控件\" id=\"frmOfficialControls\"></div>";
			strTemplate += "<audio title=\"播放语音控件\" autoplay controls=\"controls\" id=\"frmPlayer\"></audio>";
			strTemplate += "<audio title=\"播放语音控件\" autoplay id=\"frmRemind\"></audio>";
			strTemplate += "</div>";
			
			$(contianer).html(strTemplate);
			
			/***************************************************************************************
			*设置答题模块控件
			****************************************************************************************/
			Operate['frmQueSubjict'] = document.querySelector("#frmQueSubjict") || null;
			Operate['frmQueOption'] = document.querySelector("#frmQueOption") || null;
			Operate['frmQueThumb'] = document.querySelector("#frmQueThumb") || null;
			/***************************************************************************************
			*设置答题模块控件
			****************************************************************************************/
			Operate['frmTAtext'] = document.querySelector("#frmTAtext") || null;
			Operate['frmMAtext'] = document.querySelector("#frmMAtext") || null;
			/***************************************************************************************
			*设置答题选择项信息
			****************************************************************************************/
			Operate['FrmTrue'] = document.querySelector("#FrmTrue") || null;
			Operate['FrmFalse'] = document.querySelector("#FrmFalse") || null;
			Operate['FrmA'] = document.querySelector("#FrmA") || null;
			Operate['FrmB'] = document.querySelector("#FrmB") || null;
			Operate['FrmC'] = document.querySelector("#FrmC") || null;
			Operate['FrmD'] = document.querySelector("#FrmD") || null;
			Operate['FrmOk'] = document.querySelector("#FrmOk") || null;
			Operate['FrmUpper'] = document.querySelector("#FrmUpper") || null;
			Operate['FrmNext'] = document.querySelector("#FrmNext") || null;
			/***************************************************************************************
			*设置选项卡控件
			****************************************************************************************/
			Operate['frmOptions'] = document.querySelector("#frmOptions") || null;
			/***************************************************************************************
			*显示进度条
			****************************************************************************************/
			Operate['frmProcess'] = document.querySelector("#frmProcess") || null;
			/***************************************************************************************
			*答题技巧
			****************************************************************************************/
			Operate['frmTechniques'] = document.querySelector("#frmTechniques") || null;
			/***************************************************************************************
			*播放语音
			****************************************************************************************/
			Operate['frmPlayer'] = document.querySelector("#frmPlayer") || null;
			Operate['mp3Button'] = document.querySelector("#mp3Button") || null;
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
			*加载数据事件
			****************************************************************************************/
			try{
				if(Operate['frmOptions']!=undefined && Operate['frmOptions']!=null 
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
						if(this.value=='考题分析'){ShowOfficial();}
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
						if(Operate['thisFalse']!="true"){
							if(this.value=='语音讲解'){
								this.value="关闭语音";
								ShowPlayer();
							}
							else{
								this.value="语音讲解";
								ClosePlayer();
							}
						}
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
			*设置错题信息
			****************************************************************************************/
			if(Operate['sErrButton']!=undefined && Operate['sErrButton']!=null)
			{
				try{
					$(Operate['sErrButton']).click(function(){
						try{
							if(Operate["thisOption"]!=undefined 
							&& Operate["thisOption"]!=null){
								SaveFalse(Operate["thisOption"],function(strMsg){
									try{
										if(strMsg!="success"){$.messager.show({title:'系统提示',msg:strMsg});}
										else{$.messager.show({title:'系统提示',msg:"错题设置成功"});};
									}catch(err){}
								});
							}	
						}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*答对后删除当前错题
			****************************************************************************************/
			if(Operate['FrmEmptybox']!=undefined && Operate['FrmEmptybox']!=null)
			{
				try{
					$(Operate['FrmEmptybox']).click(function(){									
						try{closeEmpty();}catch(err){};				
					});
				}catch(err){}	
			}
			
			/***************************************************************************************
			*清空错题按钮信息
			****************************************************************************************/
			if(Operate['FrmEmptyButton']!=undefined && Operate['FrmEmptyButton']!=null 
			&& Operate['frmEmpty']!=undefined && Operate['frmEmpty']!=null)
			{
				try{
					$(Operate['FrmEmptyButton']).click(function(){
																
						var isHide = $(Operate['FrmEmptyButton']).attr("isHide") || "false";
						if(isHide!="true"){try{openEmpty();}catch(err){};}
						else{try{closeEmpty();}catch(err){};}							
					});
				}catch(err){}
			};
			/***************************************************************************************
			*去除当前错题
			****************************************************************************************/
			if(Operate['FrmEmptyThis']!=undefined && Operate['FrmEmptyThis']!=null)
			{
				$(Operate['FrmEmptyThis']).click(function(){
					if(Operate["thisOption"]!=undefined 
					&& Operate["thisOption"]!=null){
						DeleteFalse(Operate["thisOption"],function(strMsg){
							try{
								if(strMsg!="success"){$.messager.show({title:'系统提示',msg:strMsg});}
								else{$.messager.show({title:'系统提示',msg:"错题设置成功"});};
							}catch(err){}
						});
					}	
					
				});	
			};
			/***************************************************************************************
			*去除当前错题
			****************************************************************************************/
			if(Operate['FrmEmptyAll']!=undefined && Operate['FrmEmptyAll']!=null)
			{
				$(Operate['FrmEmptyAll']).click(function(){
					try{closeEmpty();}catch(err){};
					$.messager.confirm('系统提示','错题清空后将无法恢复,你确定要清空所有错题？',function(isOK){
																					
					});
					/*if(Operate["thisOption"]!=undefined 
					&& Operate["thisOption"]!=null){
						DeleteFalse(Operate["thisOption"],function(strMsg){
							try{
								if(strMsg!="success"){$.messager.show({title:'系统提示',msg:strMsg});}
								else{$.messager.show({title:'系统提示',msg:"错题设置成功"});};
							}catch(err){}
						});
					}*/	
					
				});	
			}
			/***************************************************************************************
			*显示正确答案
			****************************************************************************************/
			if(Operate['showAnsButton']!=undefined && Operate['showAnsButton']!=null){
				$(Operate['showAnsButton']).click(function(){
					try{
						if(this.value=="显示答案"){
							this.value = "隐藏答案";
							try{ShowAnswer();}catch(err){}
						}else{
							this.value = "显示答案";
							try{CloseAnswer();}catch(err){}	
						}
					}catch(err){}										   
				});	
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
							&& Operate['thisOption']['questionMode']!=undefined
							&& Operate['thisOption']['questionMode']=="单选")
							{
								try{SingleA();}catch(err){};
							}
							else if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['questionMode']!=undefined
							&& Operate['thisOption']['questionMode']=="多选")
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
							&& Operate['thisOption']['questionMode']!=undefined
							&& Operate['thisOption']['questionMode']=="单选")
							{
								try{SingleB();}catch(err){};
							}
							else if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['questionMode']!=undefined
							&& Operate['thisOption']['questionMode']=="多选")
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
							&& Operate['thisOption']['questionMode']!=undefined
							&& Operate['thisOption']['questionMode']=="单选")
							{
								try{SingleC();}catch(err){};
							}
							else if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['questionMode']!=undefined
							&& Operate['thisOption']['questionMode']=="多选")
							{
								try{MultipleC();}catch(err){};	
							}
						}catch(err){}
					});	
				}catch(err){}	
			};
			/***************************************************************************************
			*设置答题卡选项D按钮事件
			****************************************************************************************/
			if(Operate['FrmD']!=undefined && Operate['FrmD']!=null)
			{
				try{
					$(Operate['FrmD']).click(function(){
						try{
							if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['questionMode']!=undefined
							&& Operate['thisOption']['questionMode']=="单选")
							{
								try{SingleD();}catch(err){};
							}
							else if(Operate['thisOption']!=undefined && Operate['thisOption']!=null 
							&& Operate['thisOption']['questionMode']!=undefined
							&& Operate['thisOption']['questionMode']=="多选")
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
						/*************************************************************
						*标注选项信息
						**************************************************************/
						try{$(Operate['FrmTrue']).attr("iSelection","true");}
						catch(err){}
						try{$(Operate['FrmFalse']).attr("iSelection","false");}
						catch(err){}
						/*************************************************************
						*判断答题信息
						**************************************************************/
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
						/*************************************************************
						*标注选项信息
						**************************************************************/
						try{$(Operate['FrmTrue']).attr("iSelection","false");}
						catch(err){}
						try{$(Operate['FrmFalse']).attr("iSelection","true");}
						catch(err){}
						/*************************************************************
						*判断答题信息
						**************************************************************/
						try{VerificationSelectionJudge("错");}catch(err){}
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
						var absText = "";
						$(Operate['frmQueOption']).find("input[operate=\"autoMul\"]").each(function(){
							if(this.checked!=undefined && this.checked!=null && this.checked){
								if(absText!=""){absText=absText+this.value;}
								else{absText=this.value;}
							}																	
						});
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
			*默认加载第一道试题
			****************************************************************************************/
			try{ShowKaoshi(0);}catch(err){}
			/***************************************************************************************
			*加载默认试题信息
			****************************************************************************************/
			try{
				if(options['thisRecord']!=undefined && options['thisRecord']!=null 
				&& options['thisRecord']!=0 && options['thisRecord']!=1 
				&& !isNaN(parseInt(options['thisRecord'])))
				{
					$.messager.confirm("系统提示","你上次练习到"+options['thisRecord']+"题,是否继续?",function(isOK){
						if(isOK){
							var thisRecord = parseInt(options['thisRecord'])-1;
							if(thisRecord<=0){thisRecord=0;}
							ShowKaoshi(thisRecord);		
						}else{}
					});
				}
			}catch(err){}
			/********************************************************************************************
			*存储用户答题卡信息内容
			*********************************************************************************************/
			try{
				var timer = 10;
				var inter = setInterval(function(){
					if(timer>=0){timer=timer-1;}
					else{clearInterval(inter);SavePaper();}
				},1000);
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
				if(Operate['thisOption']['abcText']!=undefined 
				&& Operate['thisOption']['abcText']!=null 
				&& Operate['thisOption']['abcText']!="" 
				&& SelectionABC!=undefined && SelectionABC!=null 
				&& SelectionABC!="" && SelectionABC==Operate['thisOption']['abcText'])
				{
					try{AnswerTrue();}catch(err){}
				}
				else{
					try{AnswerFalse();}catch(err){}
				}
			}catch(err){}
		};
		/********************************************************************************************
		*验证用户选择的答案信息,单项选择
		*********************************************************************************************/
		var VerificationSelectionJudge = function(JudgeText)
		{
			try{
				if(Operate['thisOption']['abcText']!=undefined 
				&& Operate['thisOption']['abcText']!=null 
				&& Operate['thisOption']['abcText']!="" 
				&& JudgeText!=undefined && JudgeText!=null && JudgeText!="" 
				&& JudgeText==Operate['thisOption']['abcText'])
				{
					try{AnswerTrue();}catch(err){}
				}
				else{
					try{AnswerFalse();}catch(err){}
				}
			}catch(err){}
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
				{
					try{AnswerTrue();}catch(err){}
				}
				else{
					try{AnswerFalse();}catch(err){}
				}
			}catch(err){}
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
				&& Operate['thisOption']['QuetID']!="" && Operate['thisOption']['answerText']!=undefined 
				&& Operate['thisOption']['answerText']!=null && Operate['thisOption']['answerText']!="")
				{
					var QuetID = parseInt(Operate['thisOption']['QuetID']) || 0;
					var AnswerText = Operate['thisOption']['answerText'] || "";
					var FirTrue = "false";
					if(eKsList[QuetID]!=undefined && eKsList[QuetID]!=null 
					&& typeof(eKsList[QuetID])=='object')
					{FirTrue = eKsList[QuetID]["FirTrue"] || "false";}
					if(FirTrue==undefined || FirTrue==null || FirTrue=="")
					{FirTrue="true";}
					try{
						var eOption = {
							"answerText":AnswerText,
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
				&& Operate['thisOption']['QuetID']!="" && Operate['thisOption']['answerText']!=undefined 
				&& Operate['thisOption']['answerText']!=null && Operate['thisOption']['answerText']!="")
				{
					var QuetID = parseInt(Operate['thisOption']['QuetID']) || 0;
					var AnswerText = Operate['thisOption']['answerText'] || "";
					var FirTrue = "false";
					if(eKsList[QuetID]!=undefined && eKsList[QuetID]!=null 
					&& typeof(eKsList[QuetID])=='object')
					{FirTrue = eKsList[QuetID]["FirTrue"] || "false";}
					if(FirTrue==undefined || FirTrue==null || FirTrue=="")
					{FirTrue="true";}
					try{
						var eOption = {
							"answerText":AnswerText,
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
			var arrList = $.xml2json(strXml);
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
			var SendOption = {};
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
			var SendOption = {};
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
};
/**************************************************************************************
*保存答题记录日志信息
***************************************************************************************/
var SavePaperLogs = function(options,thisRecord,back)
{
	try{
		if(window.cfg!=undefined && window.cfg!=null && typeof(window.cfg)=='object' 
		&& options!=undefined && options!=null && typeof(options)=='object'
		&& cfg['classId']!=undefined && cfg['classId']!=null && cfg['classId']!=""
		&& cfg['channel']!=undefined && cfg['channel']!=null && cfg['channel']!=""
		&& options!=undefined && options!=null && typeof(options)=="object")
		{
			var SendOption = {};
			SendOption["url"]="PaperQuence.aspx?action=saveLogs";
			SendOption['type']="post";
			SendOption["data"] = {
				"thisRecord":(parseInt(thisRecord) || 0),
				"AnswerText":JSON.stringify(options),
				"unionid":cfg['classId'],
				"strUnion":cfg['channel']
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