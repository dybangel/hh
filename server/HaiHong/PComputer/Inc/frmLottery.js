;(function($) {
"use strict";
	/************************************************************************************************************************
	*显示网页内容信息
	**************************************************************************************************************************/
	$.fn.ShowLottery=function(options,WinPlayer)
	{
		var Operate = {
			"player":{},
			"interval":0,
			'OrderAmount':0,
			'isAfterOK':false,
			'AfterNumber':0
		};
		var $thisContianer = this;
		var frmRender = function()
		{
			var strTemplate = "";
			strTemplate += "<div id=\"frmLottery\">";
			/****************************************************************************
			*开奖记录信息
			*****************************************************************************/
			strTemplate += "<div id=\"frmHistoryContianer\">";
			strTemplate += "<div id=\"frmHistoryText\">上期<br/>开奖</div>";
			strTemplate += "<div Key=\""+options['lotterykey']+"\" id=\"frmHistoryNums\"><b>0</b><b>0</b><b>0</b><b>0</b><b>0</b></div>";
			strTemplate += "<div Onclick=\"window.location='Lottery.aspx?action=history&lotteryId="+options['lotteryid']+"'\" id=\"frmHistoryBtns\">开奖历史</div>";
			strTemplate += "</div>";
			/****************************************************************************
			*获取期号信息,倒计时信息
			*****************************************************************************/
			strTemplate += "<div id=\"frmExpectContianer\">";
			strTemplate += "<div id=\"frmExpectText\">第00000000期</div>";
			strTemplate += "<div id=\"frmExpectInterval\">00:00:00</div>";
			strTemplate += "<div onclick=\"window.location='transfer.aspx'\" id=\"frmAmountBtns\">";
			strTemplate += "<div style=\"font-size:12px;padding:2px 0px;\">我的余额</div>";
			strTemplate += "<div>￥<font id=\"frmBalance\">"+options['balance']+"</font>元</div>";
			strTemplate += "</div>"
			strTemplate += "</div>";
			/****************************************************************************
			*选择玩法,设置倍数信息
			*****************************************************************************/
			strTemplate += "<div id=\"frmPlayerContianer\">";
			strTemplate += "<div text=\"选择玩法\" id=\"frmPlayerBox\">";
			strTemplate += "<select style=\"direction:ltr\" id=\"frmPlayerSetion\">";
			strTemplate += "<option value=\"\">选择玩法</option>";
			if(WinPlayer!=undefined && WinPlayer!=null && typeof(WinPlayer)=='object' 
			&& WinPlayer['items']!=undefined && WinPlayer['items']!=null 
			&& typeof(WinPlayer['items'])=='object' && WinPlayer['items'].length>=1){
				$(WinPlayer['items']).each(function(k,json){
					strTemplate += "<option Kid=\""+k+"\"";
					strTemplate += " value=\""+json['playermode']+"\">";
					strTemplate += ""+json['playername']+"(奖金"+json['maxbonus']+"元)";
					strTemplate += "</option>";
				});
			};
			strTemplate += "</select>";
			strTemplate += "</div>";
			strTemplate += "<div id=\"frmMultipleBoxs\">";
			strTemplate += "倍数<input readonly type=\"number\" id=\"frmMultiple\" value=\"1\" placeholder=\"数字\" />倍"
			strTemplate += "</div>";
			strTemplate += "</div>";
			/****************************************************************************
			*选号主体框架内容
			*****************************************************************************/
			strTemplate += "<div style=\"display:block;width:100%;clear:both;height:156px;\"></div>";
			strTemplate += "<div id=\"frmControl\"></div>";
			strTemplate += "<div style=\"display:block;width:100%;clear:both;height:145px;\"></div>";
			/****************************************************************************
			*选择号码展示内容
			*****************************************************************************/
			strTemplate += "<div id=\"frmChooseMaster\">";
			strTemplate += "<div id=\"frmChooseControls\">";
			strTemplate += "<div id=\"frmChooseTitle\">";
			strTemplate += "<div id=\"frmChooseTitleText\">查看选号</div>";
			strTemplate += "<div id=\"frmChooseTitleEmpty\">清空选号</div>";
			strTemplate += "</div>";
			strTemplate += "<div id=\"frmChooseContianer\"></div>";
			strTemplate += "<div id=\"frmChooseCalculation\">当前已选择0组号码,合计0元</div>";
			strTemplate += "</div>";
			strTemplate += "</div>";
			
			/****************************************************************************
			*展开追号面板内容
			*****************************************************************************/
			strTemplate += "<div id=\"frmAfterMaster\">";
			strTemplate += "<div id=\"frmAfterControls\">";
			strTemplate += "<div id=\"frmAfterTitle\">";
			strTemplate += "<div id=\"frmAfterTitleText\">追号计划</div>";
			strTemplate += "<div id=\"frmAfterOptions\">";
			strTemplate += "<input id=\"frmAfterOk\" type=\"checkbox\" value=\"0\" />";
			strTemplate += "<label style=\"padding-left:16px;\">";
			strTemplate += "<input id=\"frmAfterWintop\" type=\"checkbox\" checked />";
			strTemplate += "<span>中奖停追</span>";
			strTemplate += "</label>";
			strTemplate += "</div>";
			strTemplate += "<select id=\"frmAfterNumber\">";
			strTemplate += "<option value=\"5\">选择5期</option>";
			strTemplate += "<option value=\"10\">选择10期</option>";
			strTemplate += "<option value=\"15\">选择15期</option>";
			strTemplate += "<option value=\"20\">选择20期</option>";
			strTemplate += "</select>";
			strTemplate += "</div>";
			strTemplate += "<div id=\"frmAfterContianer\"></div>";
			strTemplate += "<div id=\"frmAfterCalculation\">当前已选择0期追号,合计0元</div>";
			strTemplate += "</div>";
			strTemplate += "</div>";
			
			/****************************************************************************
			*显示底部说明内容信息
			*****************************************************************************/
			strTemplate +="<div id=\"frmLotteryAmounts\">";
			strTemplate +="当前未选择任何号码";
			strTemplate +="</div>";
			
			/****************************************************************************
			*底部按钮信息
			*****************************************************************************/
			strTemplate +="<div id=\"frmLotteryButtons\">";
			strTemplate +="<div number=\"0\" id=\"frmLotteryAfterBtn\"><span class=\"name\">追号</span></div>";
			strTemplate +="<div number=\"0\" id=\"frmLotteryViewBtn\"><span class=\"name\">查看选号</span></div>";
			strTemplate +="<div id=\"frmLotteryAddBtn\"><span class=\"name\">添加选号</span></div>";
			strTemplate +="<div id=\"frmLotteryBuyBtn\"><span class=\"name\">立即下注</span></div>";
			strTemplate +="</div>";
			strTemplate +="</div>";
			/*****************************************************************************
			*将构建的内容加入的板块
			******************************************************************************/
			$($thisContianer).append(strTemplate);
			/*****************************************************************************
			*获取系统加载出来的控件信息
			******************************************************************************/
			Operate['frmLottery'] = document.querySelector('#frmLottery');
			Operate['frmHistoryContianer'] = document.querySelector('#frmHistoryContianer');
			Operate['frmHistoryNums'] = document.querySelector('#frmHistoryNums');
			Operate['frmHistoryBtns'] = document.querySelector('#frmHistoryBtns');
			
			Operate['frmExpectContianer'] = document.querySelector('#frmExpectContianer');
			Operate['frmExpectText'] = document.querySelector('#frmExpectText');
			Operate['frmExpectInterval'] = document.querySelector('#frmExpectInterval');
			Operate['frmAmountBtns'] = document.querySelector('#frmAmountBtns');
			
			Operate['frmPlayerContianer'] = document.querySelector('#frmPlayerContianer');
			Operate['frmPlayerBox'] = document.querySelector('#frmPlayerBox');
			Operate['frmPlayerSetion'] = document.querySelector('#frmPlayerSetion');
			Operate['frmMultipleBoxs'] = document.querySelector('#frmMultipleBoxs');
			Operate['frmMultiple'] = document.querySelector('#frmMultiple');
			
			Operate['frmSelection'] = document.querySelector('#frmSelection');
			Operate['frmLotteryButtons'] = document.querySelector('#frmLotteryButtons');
			Operate['frmLotteryAfterBtn'] = document.querySelector('#frmLotteryAfterBtn');
			Operate['frmLotteryViewBtn'] = document.querySelector('#frmLotteryViewBtn');
			Operate['frmLotteryAddBtn'] = document.querySelector('#frmLotteryAddBtn');
			Operate['frmLotteryBuyBtn'] = document.querySelector('#frmLotteryBuyBtn');
			/*********************************************************************************
			* 展开选号面板信息
			**********************************************************************************/
			Operate['frmChooseMaster'] = document.querySelector('#frmChooseMaster');
			Operate['frmChooseControls'] = document.querySelector('#frmChooseControls');
			Operate['frmChooseContianer'] = document.querySelector('#frmChooseContianer');
			Operate['frmChooseTitle'] = document.querySelector('#frmChooseTitle');
			Operate['frmChooseTitleText'] = document.querySelector('#frmChooseTitleText');
			Operate['frmChooseTitleEmpty'] = document.querySelector('#frmChooseTitleEmpty');
			Operate['frmChooseCalculation'] = document.querySelector('#frmChooseCalculation');
			/*********************************************************************************
			* 展开追号面板数据信息
			**********************************************************************************/
			Operate['frmAfterMaster'] = document.querySelector('#frmAfterMaster');
			Operate['frmAfterControls'] = document.querySelector('#frmAfterControls');
			Operate['frmAfterTitle'] = document.querySelector('#frmAfterTitle');
			Operate['frmAfterTitleText'] = document.querySelector('#frmAfterTitleText');
			Operate['frmAfterNumber'] = document.querySelector('#frmAfterNumber');
			Operate['frmAfterContianer'] = document.querySelector('#frmAfterContianer');
			Operate['frmAfterOk'] = document.querySelector('#frmAfterOk');
			Operate['frmAfterWintop'] = document.querySelector('#frmAfterWintop');
			Operate['frmAfterCalculation'] = document.querySelector('#frmAfterCalculation');
			/*********************************************************************************
			* 定义选号查看点击事件信息
			**********************************************************************************/
			if(Operate['frmChooseMaster']!=undefined && Operate['frmChooseMaster']!=null){
				try{
					$(Operate['frmChooseMaster']).click(function(){
						if(event.target==this){
							$(Operate['frmChooseMaster']).hide();				
						}								 
					});	
				}catch(err){}	
			};
			/*********************************************************************************
			* 点击购买按钮信息
			**********************************************************************************/
			if(Operate['frmLotteryBuyBtn']!=undefined && Operate['frmLotteryBuyBtn']!=null)
			{
				$(Operate['frmLotteryBuyBtn']).click(function(){
					var SelectionLength = parseInt(GetChooseLength()) || 0;
					if(SelectionLength!=undefined && SelectionLength!=null 
					&& !isNaN(SelectionLength) && SelectionLength>=1)
					{try{SaveLottery();}catch(err){};}
					else{alert('没有添加选号,请先添加选号');return false;}
				});	
			};
			/*********************************************************************************
			* 查看选号信息
			**********************************************************************************/
			if(Operate['frmLotteryViewBtn']!=undefined && Operate['frmLotteryViewBtn']!=null 
			&& Operate['frmChooseMaster']!=undefined && Operate['frmChooseMaster']!=null){
				try{
					$(Operate['frmLotteryViewBtn']).click(function(){										   
						$(Operate['frmChooseMaster']).show();					 
					});	
				}catch(err){}	
			};
			/*********************************************************************************
			* 清空选择的号码信息
			**********************************************************************************/
			if(Operate['frmChooseTitleEmpty']!=undefined && Operate['frmChooseTitleEmpty']!=null
			&& Operate['frmChooseContianer']!=undefined && Operate['frmChooseContianer']!=null)
			{
				try{
					$(Operate['frmChooseTitleEmpty']).click(function(){
						var SelectionLength = parseInt(GetChooseLength()) || 0;
						if(SelectionLength!=undefined && SelectionLength!=null 
						&& !isNaN(SelectionLength) && SelectionLength>=1){
							try{
								WindowsConfirm('您确定要清空所选择的号码组吗?',function(){
									try{EmptyClearanceSelection();}catch(err){};
								});
							}catch(err){}	
						}else{try{CloseChooseMaster();}catch(err){};}
					});	
				}catch(err){}
			};
			/*********************************************************************************
			* 点开追号面板信息
			**********************************************************************************/
			if(Operate['frmLotteryAfterBtn']!=undefined && Operate['frmLotteryAfterBtn']!=null 
			&& Operate['frmAfterMaster']!=undefined && Operate['frmAfterMaster']!=null){
				try{
					$(Operate['frmLotteryAfterBtn']).click(function(){
						try{
							if(Operate['isAfterOK']!=undefined && Operate['isAfterOK']!=null 
							&& Operate['isAfterOK']==true && Operate['AfterNumber']!=undefined 
							&& Operate['AfterNumber']!=null && Operate['AfterNumber']!=0)
							{try{$(Operate['frmAfterMaster']).show();}catch(err){}}
							else{try{afterLoading(5);}catch(err){};}
						}catch(err){}
					});	
					$(Operate['frmAfterMaster']).click(function(){
						try{
							if(event.target==this){
							$(this).hide();				
						}	
						}catch(err){}
					});	
					$(Operate['frmAfterNumber']).change(function(){
						try{
							var thisNumber = parseInt(this.value) || 5;	
							afterLoading(thisNumber);	
						}catch(err){}
					});
					
				}catch(err){}
			};
			if(Operate['frmMultiple']!=undefined && Operate['frmMultiple']!=null){
				var MulKey = cfg["lotterykey"] || "demo";
				/*************************************************************************
				*设置默认倍数信息
				**************************************************************************/
				try{
					var frmVal = parseFloat(jQuery.cookies("fooke_"+MulKey+"_mul")) || 1;
					if(frmVal!=undefined && frmVal!=null){
						try{$(Operate['frmMultiple']).val(frmVal);}catch(err){}
					}
				}catch(err){}
				/*************************************************************************
				*设置倍数更新事件
				**************************************************************************/
				$(Operate['frmMultiple']).click(function(){
					Keyboard(function(strV){
						try{$(Operate['frmMultiple']).val(strV);}catch(err){}
						try{jQuery.cookies("fooke_"+MulKey+"_mul",strV);}catch(err){}
					});										 
				});	
			}
			/*********************************************************************************
			* 定义处理操作事件信息
			**********************************************************************************/
			if(Operate['frmPlayerSetion']!=undefined && Operate['frmPlayerSetion']!=null){
				$(Operate['frmPlayerSetion']).change(function(){
					var SelectionIndex = parseInt($(this.options[this.selectedIndex]).attr('Kid')) || 0;
					if(SelectionIndex!=undefined && SelectionIndex!=null 
					&& WinPlayer!=undefined && WinPlayer!=null && typeof(WinPlayer)=='object' 
					&& WinPlayer['items']!=undefined && WinPlayer['items']!=null 
					&& typeof(WinPlayer['items'])=='object' 
					&& WinPlayer['items'].length>=SelectionIndex)
					{
						try{
							var sOptions = 	WinPlayer['items'][SelectionIndex];
							if(sOptions!=undefined && sOptions!=null 
							&& typeof(sOptions)=='object'){
								try{playerLoading(sOptions);}catch(err){}
								try{Operate['player'] = sOptions;}catch(err){}
								try{$(Operate['frmPlayerBox']).attr("text",sOptions['playername']);}
								catch(err){}
							}
						}catch(err){}
					}
					
				});
				/*****************************************************************************
				设置默认玩法信息
				******************************************************************************/
				if(WinPlayer!=undefined && WinPlayer!=null && typeof(WinPlayer)=='object' 
				&& WinPlayer['items']!=undefined && WinPlayer['items']!=null 
				&& typeof(WinPlayer['items'])=='object' && WinPlayer['items'].length>=1 
				&& WinPlayer['demo']!=undefined && WinPlayer['demo']!=null && WinPlayer['demo']!='')
				{
					$(WinPlayer['items']).each(function(j,json){
						if(json!=undefined && json!=null && typeof(json)=='object' 
						&& json['playermode']!=undefined && json['playermode']!="" 
						&& json['playermode']==WinPlayer['demo']){
							try{playerLoading(json);}catch(err){}
							try{Operate['player'] = json;}catch(err){}
							try{$(Operate['frmPlayerBox']).attr("text",json['playername']);}
							catch(err){}
							return true;	
						}								
					});
				};
			};
			/*****************************************************************************
			将选择的号码加入到账号当中
			******************************************************************************/
			if(Operate['frmLotteryAddBtn']!=undefined && Operate['frmLotteryAddBtn']!=null 
			&& Operate['player']!=undefined && typeof(Operate['player'])=='object' 
			&& Operate['player']["playermode"]!=undefined && Operate['player']["playermode"]){
				$(Operate['frmLotteryAddBtn']).click(function(){
					AddSelection(Operate['player']);									  
				});	
			}
			else{alert('获取彩种玩法列表信息失败,请重试！');return false;};
		};
		/***********************************************************************
		*关闭展开的选号查看器
		************************************************************************/
		var CloseChooseMaster = function(){
			try{$(Operate['frmChooseMaster']).hide();}
			catch(err){}	
		};
		/***********************************************************************
		*清空选号
		************************************************************************/
		var EmptyClearanceSelection = function()
		{
			/***********************************************************************
			*开始清空数据
			************************************************************************/
			try{
				if(Operate['frmChooseContianer']!=undefined 
				&& Operate['frmChooseContianer']!=null){
					$(Operate['frmChooseContianer']).find("div[operate=\"items\"]").remove();	
				};
			}catch(err){}
			/***********************************************************************
			*计算选择的订单号数量
			************************************************************************/
			try{CalculationChoose();}catch(err){}
			/***********************************************************************
			*关闭选号查看器
			************************************************************************/
			try{CloseChooseMaster();}catch(err){}
		};
		/***********************************************************************
		*开始加载追号列表信息
		************************************************************************/
		var afterLoading = function(number){
			console.log('loading after start');
			$.ajax({
				url:cfg["url"]+"&action=after&number="+number+"",
				success:function(strResponse){
					$("#frmAfterContianer").html(strResponse);
					try{$(Operate['frmAfterMaster']).show();}catch(err){}
					/****************************************************************************
					*快速选择追号数据
					*****************************************************************************/
					$("#frmAfterTabs").find("input[operate=\"checked\"]").click(function(){
						try{
							var thisTR = $(this.parentNode.parentNode)[0];
							if(thisTR!=undefined && thisTR!=null && thisTR.tagName=='TR'){
								var thisMul = $(thisTR).find("input[operate=\"multiple\"]")[0];
								if(this.checked && thisMul!=undefined && thisMul!=null){
									$(thisMul).removeAttr("disabled",'disabled');	
								}else if(thisMul!=undefined && thisMul!=null){
									$(thisMul).attr("disabled",'disabled');		
								}
							}
						}catch(err){}
						try{afterCalculation();	}catch(err){}
					});
					/****************************************************************************
					*设置倍数点击事件,弹出软件盘
					*****************************************************************************/
					$("#frmAfterTabs").find("input[operate=\"multiple\"]").click(function(){
						var $elms = this;
						Keyboard(function(strV){
							try{$($elms).val(strV);}catch(err){}
							try{afterCalculation();	}catch(err){}
						});		
					});
					/****************************************************************************
					*设置追号数据中的倍数
					*****************************************************************************/
					$("#frmAfterTabs").find("input[operate=\"multiple\"]").keyup(function(){
						try{this.value = this.value.replace(/[a-zA-Z]/,'');}catch(err){}
					});	
				}
			});
			console.log('loading after end');
		};
		
		/*********************************************************************************************
		*统计用户选择号码的注数
		**********************************************************************************************/
		var calculationAmount = function(){
			var total = 0;
			try{
				$("#frmChooseContianer").find("div[operate=\"items\"]").each(function(){
					var amount = parseFloat($(this).attr("amount")) || 0;														  					total = parseFloat(total + amount);
				});	
			}catch(err){}
			return parseFloat(total.toFixed(2));
		};
		
		/*********************************************************************************************
		*统计追号金额信息
		**********************************************************************************************/
		var afterCalculation = function()
		{
			var amount = parseFloat(calculationAmount()).toFixed(2) || 0;
			var total = 0;
			$("#frmAfterTabs").find("input[operate=\"checked\"]").each(function(k,elem){
				var thisTR = $(elem.parentNode.parentNode)[0];
				if(thisTR!=undefined && thisTR!=null && thisTR.tagName=='TR'){
					var amountTD = $(thisTR).find("td[operate=\"amount\"]")[0] || 0;
					var totalTD = $(thisTR).find("td[operate=\"total\"]")[0] || 0;
					if(amountTD!=undefined && amountTD!=null && totalTD!=undefined && totalTD!=null)
					{
						if(this.checked)
						{
							var thisMul = parseFloat($(thisTR).find("input[operate=\"multiple\"]").val()) || 0;
							try{
								if(thisMul<=0){thisMul=1;}
								if(thisMul>=999){thisMul=999;}
								thisMul = thisMul.toFixed(2);
							}catch(err){}
							try{
								var newAmount = parseFloat(amount * thisMul).toFixed(2);
								$(amountTD).html(newAmount);
							}catch(err){}
							try{
								total=(parseFloat(total)+parseFloat(newAmount)).toFixed(2);
								$(totalTD).html(total);
							}catch(err){}
						}else{
							try{$(amountTD).html("--");$(totalTD).html("--");}catch(err){}
						}
					}
				}
			});
			/***********************************************************************
			*统计追号数据信息
			************************************************************************/
			try{CalculationAfterNum();}catch(err){}
		};
		/***********************************************************************
		*统计用户设置的追号数据信息
		************************************************************************/
		var CalculationAfterNum = function()
		{
			var thisAmount = 0;var thisNumber = 0;
			try{
				$("#frmAfterTabs").find("input[operate=\"checked\"]:checked").each(function(k,elem){
					var thisTR = ($(elem).parents("tr[operate=\"items\"]"))[0];
					if(thisTR!=undefined && thisTR!=null && thisTR.tagName=='TR'){
						thisNumber=thisNumber+1;
						var amountTD = $(thisTR).find("td[operate=\"amount\"]")[0] || 0;
						var sAmount = parseFloat($(amountTD).html());
						thisAmount = thisAmount + sAmount
					}
				});	
			}catch(err){}
			try{
				if(thisNumber!=undefined && thisNumber!=null 
				&& !isNaN(thisNumber) && thisNumber!=0){
					Operate['isAfterOK']=true;
					Operate['AfterNumber']=parseInt(thisNumber) || 0;	
				}else{
					Operate['isAfterOK']=false;
					Operate['AfterNumber']=0;	
				}
			}catch(err){}
			/***********************************************************************
			*设置追号选择说明信息
			************************************************************************/
			try{
				$(Operate['frmAfterCalculation']).html("当前已选择"+thisNumber+"期追号,合计"+thisAmount+"元");	
			}catch(err){}
			/***********************************************************************
			*设置追号期数信息
			************************************************************************/
			try{$(Operate['frmLotteryAfterBtn']).attr("number",thisNumber);}
			catch(err){}
		};
		/***********************************************************************
		*获取用户选择的追号数量信息
		************************************************************************/
		var GetAfterLength = function()
		{
			var sCount = 0;
			try{
				if(document.getElementById('frmAfterTabs')!=undefined 
				&& document.getElementById('frmAfterTabs')!=null)
				{
					sCount = $("#frmAfterTabs").find("input[operate=\"checked\"]:checked").length;	
					sCount = parseInt(sCount) || 0;
				}
			}catch(err){}
			return sCount;
		};
		/***********************************************************************
		*清空追号信息
		************************************************************************/
		var EmptyAfterNum = function()
		{
			try{
				if(document.getElementById('frmAfterTabs')!=undefined 
				&& document.getElementById('frmAfterTabs')!=null)
				{$(document.getElementById('frmAfterTabs')).remove();}
			}catch(err){}
			/***********************************************************************
			*设置选项数据信息
			************************************************************************/
			try{Operate['isAfterOK']=true;}catch(err){}
			try{Operate['AfterNumber']=0;}catch(err){}
			/***********************************************************************
			*设置追号选择说明信息
			************************************************************************/
			try{
				$(Operate['frmAfterCalculation']).html("当前已选择0期追号,合计0元");	
			}catch(err){}
			/***********************************************************************
			*设置追号期数信息
			************************************************************************/
			try{$(Operate['frmLotteryAfterBtn']).attr("number",0);}
			catch(err){}
		};
		/***********************************************************************
		*获取到用户选择的号码组数
		************************************************************************/
		var GetChooseLength = function(){
			var sCount = 0;
			try{
				if(Operate['frmChooseContianer']!=undefined && Operate['frmChooseContianer']!=null)
				{
					sCount = $(Operate['frmChooseContianer']).find("div[operate='items']").length;
					sCount = parseInt(sCount) || 0;	
				}	
			}catch(err){}
			return sCount;
		};
		/*********************************************************************************
		* 统计选择号码注数以及需要花费的金额信息
		**********************************************************************************/
		var CalculationChoose = function()
		{
			/*************************************************************************
			* 开始统计请求数据信息
			**************************************************************************/
			try{
				var thisAmount = 0;var thisNumber = 0;
				try{
					$("#frmChooseContianer").find("div[operate=\"items\"]").each(function(j,elem){
						try{
							var sAmount = parseFloat($(elem).attr('amount')) || 0;
							thisAmount = thisAmount + sAmount;
							thisNumber = thisNumber + 1;
						}catch(err){}
					});
				}catch(err){}
				/*************************************************************************
				* 设置用户选择的组数与金额信息
				**************************************************************************/
				try{
					var strTips = "当前已选择"+thisNumber+"组号码,合计"+thisAmount+"元";
					$(Operate['frmChooseCalculation']).html(strTips);
				}catch(err){}
				/*************************************************************************
				* 设置用户选号组数信息
				**************************************************************************/
				try{$(Operate['frmLotteryViewBtn']).attr("number",thisNumber);}
				catch(err){}
				
			}catch(err){}
			/*************************************************************************
			* 已完成数据统计
			**************************************************************************/
		};
		/*********************************************************************************
		* 添加选号信息
		**********************************************************************************/
		var AddSelection = function(player)
		{
			
			try{
				if(Operate['player']!=undefined && typeof(Operate['player'])=='object' 
				&& Operate['player']["playermode"]!=undefined && Operate['player']["playermode"]!="")
				{
					calculationLoading(Operate['player'],function(number,chooseText){
						if(chooseText==""){alert('请选择投注号码！');return false;}
						if(chooseText==undefined){alert('请选择投注号码！');return false;}
						if(chooseText==-1){alert('投注号码加载错误,请重试！');return false;}
						if(number<=0){alert('选号注数计算失败,请重试！');return false;}
						/**************************************************************
						*开始加入数据信息
						***************************************************************/
						var disText = chooseText;
						try{
							if(disText.indexOf("-1")!=-1){disText=disText.replace(/\-1/g,"");}
							if (disText.indexOf(",") != -1 && disText.indexOf("|") != -1) {
							   disText=disText.replace(/\,/g,"").replace(/\|/g,",");
							}
							if(disText.indexOf("|")!=-1){disText=disText.replace(/\|/g,",");}
							if(disText.indexOf(" ")!=-1){disText=disText.replace(/\ /g,",");}
							if(disText.length>20){disText=disText.substring(0,20)+"……";}
						}catch(err){}
						/**************************************************************
						*计算当前下注金额
						***************************************************************/
						try{
							var multiple = parseFloat(1) || 1;
							var unit = parseFloat(cfg["unitamount"]) || 2;
							var amount = parseFloat(((unit * number) * multiple)).toFixed(2);
							/**************************************************************
							*计算返点与奖金信息
							***************************************************************/
							var bonus = parseFloat($("#frm-bonus").val()) || 0;
							var discount = parseFloat($("#frm-discount").val()) || 0;
							bonus = parseFloat(Operate['player']['maxbonus']) || 0;
							if(bonus<=0){alert('计算奖金信息失败,请重试！');return false;}
							/**************************************************************
							*构建选号列表内容信息
							***************************************************************/
							var strTemplate = "";
							strTemplate += "<div operate=\"items\" mode=\""+player["playermode"]+"\"";
							strTemplate += " playerid=\""+player["playerid"]+"\" number=\""+number+"\"";
							strTemplate += " amount=\""+amount+"\" discount=\""+discount+"\"";
							strTemplate += " bonus=\""+bonus+"\" code=\""+chooseText+"\"";
							strTemplate += ">";
							strTemplate += "<div operate=\"text\">";
							strTemplate += ""+player["playername"]+"";
							strTemplate += "<span>[共"+number+"注]</span>";
							strTemplate += "</div>";
							strTemplate += "<div style=\"color:#999\" operate=\"text\">" + disText +"</div>";
							strTemplate += "<a operate=\"delete\">删除</a>";
							strTemplate += "</div>";
							/**********************************************************************
							*将构建的内容重新定义变量
							***********************************************************************/
							var frmTemplate = $(strTemplate)[0];
							/**********************************************************************
							*删除当前建造的号码信息
							***********************************************************************/
							try{
								if(frmTemplate!=undefined && frmTemplate!=null)
								{
									$(frmTemplate).find("a[operate=\"delete\"]").click(function(){
										WindowsConfirm('你确定要移除当前选中的号码?',function(){
											try{$(frmTemplate).remove();}catch(err){}
											try{CalculationChoose();}catch(err){}									
											/***************************************************
											*重新统计追号信息
											****************************************************/
											try{
												if(document.getElementById('frmAfterTabs')!=undefined 
												&& document.getElementById('frmAfterTabs')!=null)
												{
													try{afterCalculation();	}catch(err){}
												}
											}catch(err){}
										});													  
									});
								}
							}catch(err){}
							/**********************************************************************
							*将构建内容添加到选号当中
							***********************************************************************/
							try{$("#frmChooseContianer").append(frmTemplate);}catch(err){}
							/**********************************************************************
							*统计选号个数信息
							***********************************************************************/
							try{CalculationChoose();}catch(err){}
							/**********************************************************************
							*计算追号信息
							***********************************************************************/
							try{
								if(document.getElementById('frmAfterTabs')!=undefined 
								&& document.getElementById('frmAfterTabs')!=null)
								{
									try{afterCalculation();	}catch(err){}
								}
							}catch(err){}
							/**********************************************************************
							*删除选择的号码信息
							***********************************************************************/
							try{EmptyChooseCode();}catch(err){}
						}catch(err){}
					});
				};
			}catch(err){
				alert('玩法信息加载错误,请重试！');
				return false;
			};			
		};
		/**********************************************************************
		*获取期号数据信息
		***********************************************************************/
		var GetExpect=function()
		{
			var SendOptions = {};
			SendOptions['url']="lottery.aspx?isasyn=1&action=expect&lotteryId="+cfg['lotteryid']+"";
			SendOptions['back'] = function(Json)
			{
				if(Json!=undefined && Json!=null && typeof(Json)=='object'
				&& Json['exp']!=undefined && Json['exp']!=null && Json['exp']!=""
				&& Json['interval']!=undefined && Json['interval']!=null && Json['interval']!="")
				{
					try{Operate["interval"] = parseInt(Json['interval'] ) || 0;	}catch(err){}
					try{$(Operate["frmExpectText"]).html('第'+Json['exp']+'期');}catch(err){}	
					try{Operate["expect"] = Json['exp'];}catch(err){}	
					try{Interval((parseInt(Json['interval']) || 0),function(){
						try{GetExpect();}catch(err){}													
					});}catch(err){}
				};
			};
			/**********************************************************************
			*开始请求数据信息
			***********************************************************************/
			try{
				if(GetResponse!=undefined && GetResponse!=null && typeof(GetResponse)=='function' 
				&& SendOptions!=undefined && SendOptions!=null && typeof(SendOptions)=='object'){
					try{GetResponse(SendOptions);}catch(err){}
				}
			}catch(err){}
			
		};
		/**********************************************************************
		*获取倒计时信息
		***********************************************************************/
		var Interval = function(timer,back)
		{
			var $contianer = $(Operate['frmExpectInterval']);
			if(timer>0)
			{
				var obj = setInterval(function(){
					if(timer>0)
					{
						timer=timer-1;
						var hour = parseInt(timer / 3600);
						if(hour<10){hour="0"+hour;}
						var Minute = parseInt((timer % 3600) / 60);
						if(Minute<10){Minute="0"+Minute;}
						var Second = parseInt(((timer % 3600) % 60 % 60));
						if(Second<10){Second="0"+Second;};
						var strHtml = "<font>"+hour + "</font>";
						strHtml += ":<font>" + Minute + "</font>";
						strHtml += ":<font>"+Second +"</font>";
						$($contianer).html(strHtml);
					}
					else{
						try{clearInterval(obj);}catch(err){}
						try{GetAwardNumber(Operate["expect"]);}catch(err){}
						try{$($contianer).html("<font>00</font>:<font>00</font>:<font>00</font>");}
						catch(err){}
						if(back!=undefined && typeof(back)=='function'){try{back();}catch(err){}}
						
					}
				},1000);
			}else{
				if(back!=undefined && typeof(back)=='function'){try{back();}catch(err){}}
			}	
		};
		/*******************************************************************************************
		*获取开奖号码信息
		********************************************************************************************/
		var GetAwardNumber = function(expect)
		{
			var GetContinue = function(){
				try{
					var timer = 10;
					var inter = setInterval(function(){
						if(timer>=0){timer=timer-1;}
						else {
							clearInterval(inter);
							GetAwardNumber(expect);
						}
					},1000);
				}catch(err){alert(err.message);}
			};
			try{$(Operate['frmHistoryNums']).html('<b>正</b><b>在</b><b>开</b><b>奖</b>');}
			catch(err){}
			try{
				var url="lottery.aspx?action=award&lotteryid="+options['lotteryid']+"&Expect="+expect+"";
				$.get(url,function(strRep){
					if(strRep!=undefined && strRep!=null && strRep!=""){
						try{
							if(strRep=='continue'){GetContinue();}
							else if(strRep.indexOf('error')!=-1){alert(strRep.substr(0,6));return false;}
							else{
								try{$(Operate['frmHistoryNums']).html(strRep);}catch(err){}
							}
						}catch(err){}	
					};
				});
			}catch(err){}
		};
		
		/*******************************************************************************************
		*将选择的号码购买下注
		********************************************************************************************/
		var SaveLottery = function()
		{
			/*******************************************************************************************
			*追号投注列表信息
			********************************************************************************************/
			var strXml = "";
			try{
				if(Operate['isAfterOK']!=undefined && Operate['isAfterOK']!=null 
				&& Operate['isAfterOK']==true && Operate['AfterNumber']!=undefined 
				&& Operate['AfterNumber']!=null && Operate['AfterNumber']!=0 
				&& document.getElementById('frmAfterTabs')!=undefined
				&& document.getElementById('frmAfterTabs')!=null){
					strXml = afterConfigurationRoot();	
				}
				else if(options['mdkey']!=undefined && options['mdkey']!=null && options['mdkey']!="" 
				&&Operate['expect']!=undefined && Operate['expect']!=null && Operate['expect']!="")
				{strXml = ordinaryConfigurationRoot();};
			}catch(err){}
			/****************************************************************************************
			*开始发起数据请求信息
			*****************************************************************************************/
			try{
				if(strXml!=undefined  && strXml!=null && strXml!="" && strXml.indexOf('<item')!=-1 
				&& options!=undefined && options!=null && typeof(options)=='object'
				&& options['lotteryid']!=undefined && options['lotteryid']!=null && options['lotteryid']!=""
				&& options['lotterykey']!=undefined && options['lotterykey']!=null && options['lotterykey']!="")
				{
					var cfmMessager = "您已选择<font style=\"color:#cd0000\">"+cfg['lotteryname']+"</font>";
					cfmMessager += "第<font style=\"color:#cd0000\">"+Operate['expect']+"</font>期投注";
					cfmMessager += ",共计"+Operate['OrderAmount']+"元,"
					cfmMessager += "您确定要下注吗?";
					WindowsConfirm(cfmMessager,function(){
						SubmitLottery(strXml);
					});
				}else{
					alert('构建订单过程中发生错误,请重试!');
					return false;	
				}
			}catch(err){alert('提交下注订单失败,请重试!');return false;}
		};
		/*********************************************************************************************
		*保存用户订单信息
		**********************************************************************************************/
		var SubmitLottery = function(strXml)
		{
			try{
				var SendOptions = {};
				SendOptions['url'] = 'lottery.aspx?isAsyn=1';
				SendOptions['type'] = 'post';
				SendOptions['dataType'] = 'json';
				SendOptions['data'] = {
					"action":"confirm","lotteryid":options['lotteryid'],
					"lotterykey":options['lotterykey'],"MdKey":function(){
						try{return $.md5(strXml+'|||'+options['mdkey']);}
						catch(err){return "";}	
					},
					"strXml":strXml
				};
				SendOptions['async'] = false;
				SendOptions['cache'] = false;
				SendOptions['contentType'] = "application/x-www-form-urlencoded";
				SendOptions['processData'] = true;
				SendOptions['success'] = function(json)
				{
					try{
						if(json!=undefined && json!=null && typeof(json)=='object' 
						&& json['success']!=undefined && json['success']=='true')
						{
							/*****************************************************************
							*更新我的账户余额信息
							******************************************************************/
							try{
								if(json['balance']!=undefined && json['balance']!=null 
								&& json['balance']!="" && parseFloat(json['balance'])){
									$('#frmBalance').html(json['balance']);
								}
							}catch(err){}
							/*****************************************************************
							*清空追号信息
							******************************************************************/
							try{EmptyAfterNum();}catch(err){}
							/*****************************************************************
							*清空追号信息
							******************************************************************/
							try{EmptyClearanceSelection();}catch(err){}
						}
					}catch(err){}
					/*****************************************************************
					*输出数据处理结果
					******************************************************************/
					try{SubmitHandleResponse(json);}catch(err){}
				};
				SendOptions['error'] = function(json)
				{
					try{
						if(window.messagerAlert!=undefined && window.messagerAlert!=null 
						&& typeof(window.messagerAlert)=='function'){
							window.messagerAlert({'tips':'数据请求错误,请重试'});
						}else{alert('数据请求错误,请重试!');return false;}
					}catch(err){}
				};
				/*****************************************************************
				*开始提交请求数据
				******************************************************************/
				if(SendOptions!=undefined && SendOptions!=null && typeof(SendOptions)=='object' 
				&& SendOptions['url']!=undefined && SendOptions['url']!=null && SendOptions['url']!="" 
				&& jQuery!=undefined && jQuery!=null && typeof(jQuery)=='function' 
				&& jQuery.ajax!=undefined && jQuery.ajax!=null && typeof(jQuery.ajax)=='function')
				{
					try{jQuery.ajax(SendOptions);}
					catch(err){alert(err.message);return false;}
				};
			}catch(err){alert(err.message);return false;}
		}
		/*********************************************************************************************
		*判断用户当前是否选择追号
		**********************************************************************************************/
		var isAfter = function(){
			try{
				var ThisElem = document.getElementById('frmAfterTabs');
				var strRule = "input[operate=\"checked\"]:checked";
				if(ThisElem!=undefined && ThisElem!=null 
				&& $(ThisElem).find(strRule).length!=0)
				{return true;}
				else{return false;}
			}catch(err){}
		}
		
		/*********************************************************************************************
		*统计追号模式下的数据信息
		**********************************************************************************************/
		var afterConfigurationRoot = function(){
			var strXml = "<configurationRoot>";
			try{
				var frmStop = GetWinstopText() || 0;
				/*****************************************************************
				*将订单金额重新归0
				*****************************************************************/
				try{Operate['OrderAmount'] = 0;}catch(err){}
				/*****************************************************************
				*开始构建订单内容
				*****************************************************************/
				$("#frmAfterTabs").find("input[operate=\"checked\"]:checked").each(function(k,elem){
					
					var thisTR = elem.parentNode.parentNode;
					if(thisTR!=undefined && thisTR.tagName=='TR'){
						var expect = $(elem).val();
						var thisMul = $(thisTR).find("input[operate=\"multiple\"]")[0];
						if(thisMul!=undefined && thisMul!=null && thisMul.value!=undefined && elem.value!=""){
							var Multiple = parseFloat($(thisMul).val()) || 0;
							try{
								if(Multiple<=0){Multiple=1;}
								if(Multiple>=999){Multiple=999;}
								Multiple = parseFloat(Multiple).toFixed(2);	
							}catch(err){}
							try{
								GetSelectionCode(function(options){
									try{
										/*****************************************************************
										*累加订单金额信息
										*****************************************************************/
										var OrderAmount = parseFloat(Operate['OrderAmount']) || 0;
										var thisAmount = parseFloat(options["amount"]) || 0;
										try{
											thisAmount = parseFloat((thisAmount * Multiple));
											Operate['OrderAmount'] = parseFloat(OrderAmount + thisAmount);
										}
										catch(err){}
										/*****************************************************************
										*构建Xml集合信息
										*****************************************************************/
										strXml +="<items";
										strXml +=" code=\""+options["code"]+"\"";
										strXml +=" mode=\""+options["mode"]+"\"";
										strXml +=" playerid=\""+options["playerid"]+"\"";
										strXml +=" number=\""+options["number"]+"\"";
										strXml +=" amount=\""+thisAmount.toFixed(2)+"\"";
										strXml +=" discount=\""+options["discount"]+"\"";
										strXml +=" bonus=\""+options["bonus"]+"\"";
										strXml +=" expect=\""+expect+"\"";
										strXml +=" multiple=\""+Multiple+"\"";
										strXml +=" isafter=\"1\"";
										strXml +=" stop=\""+frmStop+"\"";
										strXml +=" />";
									}catch(err){}
								});	
							}catch(err){}	
						}	
					}
					
				});	
			}catch(err){}
			strXml+='</configurationRoot>';	
			return strXml;
		};
		/*********************************************************************************************
		*获取下注备注信息
		**********************************************************************************************/
		var GetMultiple = function()
		{
			try{
				var Multiple = parseFloat($(Operate['frmMultiple']).val()) || 0;
				if(Multiple<=0){Multiple=1;}
				if(Multiple>=999){Multiple=999;}
				Multiple = parseFloat(Multiple).toFixed(2);	
				return Multiple;
			}catch(err){return 1;}
		};
		/*********************************************************************************************
		*获取中奖后停止追号信息
		**********************************************************************************************/
		var GetWinstopText = function()
		{
			var isWin = 0;
			try{
				if(Operate['frmAfterWintop']!=undefined 
				&& Operate['frmAfterWintop']!=null 
				&& Operate['frmAfterWintop'].checked){
					isWin = 1;	
				}
			}catch(err){}
			return isWin;
		}
		/*********************************************************************************************
		*统计普通模式下的投注信息
		**********************************************************************************************/
		var ordinaryConfigurationRoot = function(){
			/*********************************************************************************
			*获取下注倍数信息
			**********************************************************************************/
			var Multiple = parseFloat(GetMultiple()) || 1; 
			/*********************************************************************************
			*开始构建下注列表数据
			**********************************************************************************/
			var strXml = "<configurationRoot>";
			if(Operate['expect']!=undefined && Operate['expect']!=null 
			&& Operate['expect']!="" && Multiple!=undefined && Multiple!=null 
			&& Multiple!="" && parseFloat(Multiple) && !isNaN(Multiple))
			{
				/*****************************************************************
				*将订单金额重新归0
				*****************************************************************/
				try{Operate['OrderAmount'] = 0;}catch(err){}
				try{
					GetSelectionCode(function(options){
						try{
							/*****************************************************************
							*累加订单金额信息
							*****************************************************************/
							var OrderAmount = parseFloat(Operate['OrderAmount']);
							var thisAmount = parseFloat(options["amount"]);
							try{
								thisAmount = parseFloat(Multiple * thisAmount);
								Operate['OrderAmount'] = parseFloat(OrderAmount + thisAmount);
							}
							catch(err){}
							/*****************************************************************
							*构建网页Xml内容信息
							*****************************************************************/
							strXml +="<items";
							strXml +=" code=\""+options["code"]+"\"";
							strXml +=" mode=\""+options["mode"]+"\"";
							strXml +=" playerid=\""+options["playerid"]+"\"";
							strXml +=" number=\""+options["number"]+"\"";
							strXml +=" amount=\""+thisAmount.toFixed(2)+"\"";
							strXml +=" discount=\""+options["discount"]+"\"";
							strXml +=" bonus=\""+options["bonus"]+"\"";
							strXml +=" expect=\""+Operate['expect']+"\"";
							strXml +=" multiple=\""+Multiple+"\"";
							strXml +=" isafter=\"0\"";
							strXml +=" stop=\"0\"";
							strXml +=" />";
						}catch(err){}
					});	
				}catch(err){}	
			}
			strXml+='</configurationRoot>';	
			return strXml;
		}
		/*******************************************************************************************
		*遍历用户下注的选号信息
		********************************************************************************************/
		var GetSelectionCode = function(back)
		{
			try{
				$(Operate['frmChooseContianer']).find("div[operate=\"items\"]").each(function(){
					var options = {};
					try{
						var mode = $(this).attr("mode") || "";
						var playerid = parseInt($(this).attr("playerid")) || 0;
						var number = parseInt($(this).attr("number")) || 0;
						var amount = parseFloat($(this).attr("amount")) || 0;
						var discount = parseFloat($(this).attr("discount")) || 0;
						var bonus = parseFloat($(this).attr("bonus")) || 0;
						var Multiple = parseFloat($(this).attr("multiple")) || 1;
						var code = $(this).attr("code") || "";
						if(mode==undefined || mode==""){mode="";}
						if(playerid==undefined || isNaN(playerid)){playerid=0;}
						if(number==undefined || isNaN(number)){number=0;}
						if(amount==undefined || isNaN(amount)){amount=0;}
						if(discount==undefined || isNaN(discount)){discount=0;}
						if(bonus==undefined || isNaN(bonus)){bonus=0;}
						if(code==undefined || code==""){code="";}
					}catch(err){}
					try{
						options["mode"]=mode;
						options["playerid"]=playerid;
						options["number"]=number;
						options["amount"]=amount;
						options["discount"]=discount;
						options["bonus"]=bonus;
						options["multiple"]=Multiple;
						options["code"]=code;
					}catch(err){}
					try{
						if(back!=undefined && typeof(back)=='function' 
						 && options!=undefined && typeof(options)=='object'
						 && options["mode"]!="" && options["mode"]!=undefined
						 && options["playerid"]!=undefined && !isNaN(options["playerid"])
						 && options["number"]!=undefined && !isNaN(options["number"]))
						{
							back(options);	
						}
					}catch(err){}
				});	
			}catch(err){}
		};
		/***************************************************************************************
		*开始加载主题内容信息
		****************************************************************************************/
		try{
			if(options!=undefined && options!=null && typeof(options)=='object' 
			&& options['lotteryid']!=undefined && options['lotteryid']!=null && options['lotteryid']!=""
			&& options['lotterykey']!=undefined && options['lotterykey']!=null && options['lotterykey']!=""
			&& WinPlayer!=undefined && WinPlayer!=null && typeof(WinPlayer)=='object' 
			&& WinPlayer['items']!=undefined && WinPlayer['items']!=null 
			&& typeof(WinPlayer['items'])=='object')
			{
				try{frmRender();}catch(err){}
				try{GetExpect();}catch(err){}
				try{GetAwardNumber('previous');}catch(err){}
			}
			else{alert('获取系统配置参数信息失败,请重试!');return false;}
		}catch(err){}
	};
})(jQuery);
/***************************************************************************************
*异步请求网页参数内容信息
****************************************************************************************/
var GetResponse = function(options,back)
{
	if(options!=undefined && options!=null && typeof(options)=='object' 
	&& options['url']!=undefined && options['url']!=null && options['url']!="")
	{
		//animation('show');
		/**********************************************************************
		*申明pager集合信息
		***********************************************************************/
		var SendOptions = {};
		/**********************************************************************
		*请求的url
		***********************************************************************/
		SendOptions["url"] = options['url'];
		/**********************************************************************
		*定义请求方式
		***********************************************************************/
		SendOptions["type"] = options['type'] || "get";
		/**********************************************************************
		*定义数据请求类型
		***********************************************************************/
		SendOptions["dataType"] = options['dataType'] || "json";
		/**********************************************************************
		*定义是否同步处理信息
		***********************************************************************/
		SendOptions["async"] = true;
		/**********************************************************************
		*数据处理成功事件信息
		***********************************************************************/
		SendOptions["success"] = function(strResponse)
		{
			if(options['dateType']!=undefined && options['dateType']=='json' 
			&& strResponse!=undefined && typeof(strResponse)=='object' 
			&& strResponse['url']!=undefined && strResponse['url']!=null)
			{
				window.location = strResponse['url'];
			}
			else if(options['dateType']!=undefined && options['dateType']=='json' 
			&& strResponse!=undefined && typeof(strResponse)=='object' 
			&& options['back']!=undefined && options['back']!=null 
			&& typeof(options['back'])=='function')
			{
				try{options['back'](strResponse);}catch(err){}
			}
			else if(options['back']!=undefined && options['back']!=null 
			&& typeof(options['back'])=='function' && strResponse!=undefined && strResponse!="")
			{
				try{options['back'](strResponse);}catch(err){}	
			};
			/**********************************************************************
			*关闭动画信息
			***********************************************************************/
			//try{animation('hide');}catch(err){}
		};
		/**********************************************************************
		*定义错误返回事件信息
		***********************************************************************/
		SendOptions["error"] = function(err)
		{
			try{
				if(err!=undefined && typeof(err)=='object' 
				&& err['statusText']!=undefined && err['statusText']!='')
				{
					$('#frm-windowError').tips({
						"success":"false",
						"tips":('发生错误:'+err["statusText"])
					});
				};
			}catch(err){};
			/**********************************************************************
			*关闭动画信息
			***********************************************************************/
			//try{animation('hide');}catch(err){}
		};
		/**********************************************************************
		*开始处理数据信息
		***********************************************************************/
		if(SendOptions!=undefined && SendOptions!=null && typeof(SendOptions)=='object' 
		&& SendOptions['url']!=undefined && SendOptions['url']!=null && SendOptions['url']!="" 
		&& jQuery!=undefined && jQuery!=null && typeof(jQuery)=='function' 
		&& jQuery.ajax!=undefined && jQuery.ajax!=null && typeof(jQuery.ajax)=='function')
		{
			try{jQuery.ajax(SendOptions);}catch(err){}
		}
	};
};
/**********************************************************************
*生成数字键盘信息
***********************************************************************/
var Keyboard = function(back)
{
	/**********************************************************************
	*获取数字键盘的默认数字
	***********************************************************************/
	var strMul = 1;
	try{
		if(event!=undefined && event!=null 
		&& event.target!=undefined && event.target!=null 
		&& event.target.tagName=='INPUT')
		{
			strMul = parseFloat(event.target.value) || 1;
		}
	}catch(err){}
	/**********************************************************************
	*构建数字键盘内容信息
	***********************************************************************/
	var strTemplate = "";
	try{
		strTemplate +="<div id=\"frmKeyboardMaster\">";
		strTemplate +="<div id=\"frmKeyboardContianer\">";
		strTemplate +="<table cellpadding=\"3\" id=\"frmKeyboard\" cellspacing=\"8\" border=\"0\">";
		strTemplate +="<tr class=\"hback\">";
		strTemplate +="<td colspan=\"4\" id=\"frmKeyboardValue\" operate=\"value\">"+strMul+"</td>";
		strTemplate +="</tr>";
		strTemplate +="<tr class=\"hback\">";
		strTemplate +="<td operate=\"number\">1</td>";
		strTemplate +="<td operate=\"number\">2</td>";
		strTemplate +="<td operate=\"number\">3</td>";
		strTemplate +="<td operate=\"number\">4</td>";
		strTemplate +="</tr>";
		strTemplate +="<tr class=\"hback\">";
		strTemplate +="<td operate=\"number\">5</td>";
		strTemplate +="<td operate=\"number\">6</td>";
		strTemplate +="<td operate=\"number\">7</td>";
		strTemplate +="<td operate=\"number\">8</td>";
		strTemplate +="</tr>";
		strTemplate +="<tr class=\"hback\">";
		strTemplate +="<td operate=\"number\">9</td>";
		strTemplate +="<td operate=\"number\">0</td>";
		strTemplate +="<td operate=\"pnt\">.</td>";
		strTemplate +="<td operate=\"del\">删除</td>";
		strTemplate +="</tr>";
		strTemplate +="<tr class=\"hback\">";
		strTemplate +="<td colspan=\"4\" operate=\"ok\">确认</td>";
		strTemplate +="</tr>";
		strTemplate +="</table>";
		strTemplate +="<div id=\"frmKeyboardTips\"></div>";
		strTemplate +="</div>";
		strTemplate +="</div>";
	}catch(err){}
	var frmTemplate = $(strTemplate)[0];
	if(frmTemplate!=undefined && frmTemplate!=null){
		$(document.body).append(frmTemplate);	
	}
	/**********************************************************************
	*格式化数字信息
	***********************************************************************/
	var Format=function(number)
	{
		try{
			var strValue = $("#frmKeyboardValue").html() || 0;
			if(strValue!="" && number=='.' && strValue.indexOf(".")!=-1)
			{ErrorMessage('数据格式错误！');return false;}
			else{
				if(number=='del' && strValue!=""){
					try{strValue=strValue.substring(0,strValue.length-1);}
					catch(err){strValue="";}
				}
				else{
					if(strValue=="0" && number=="."){strValue="0.";}
					else if(strValue=="0" && number!="."){strValue=number;}
					else{strValue = strValue + "" + number;}
				}
			}
			if(strValue==''){strValue=0;}
			if(strValue<0){ErrorMessage('下注设置倍数不能少于0！');return false;}
			else if(strValue<=1 && strValue.toString().length>=5){ErrorMessage('下注倍数小数点不能超过3位数！');return false;}
			else if(strValue<=9 && strValue.toString().length>=5){ErrorMessage('下注倍数小数点不能超过3位数！');return false;}
			else if(strValue<=99 && strValue.toString().length>=6){ErrorMessage('下注倍数小数点不能超过3位数！');return false;}
			else if(strValue>=999){ErrorMessage('下注设置倍数不能超过999倍！');return false;}
			else if(strValue.toString().length>=7){ErrorMessage('下注倍数小数点不能超过3位数！');return false;}
			else{$("#frmKeyboardValue").html(strValue);}
		}catch(err){ErrorMessage(err.message);return false;}
	};
	/**********************************************************************
	*错误数据信息
	***********************************************************************/
	var ErrorMessage = function(strtips){
		try{
			$("#frmKeyboardTips").html(strtips);
			$("#frmKeyboardTips").show();	
		}catch(err){}
		try{
			var timer = 5;
			var inter = setInterval(function(){
				if(timer>=0){timer=timer-1;}
				else{clearInterval(inter);$("#frmKeyboardTips").hide();}
			},1000);
		}catch(err){}
	};
	/**********************************************************************
	*纯数字
	***********************************************************************/
	$(frmTemplate).find("td[operate=\"number\"]").click(function(){
		try{
			var strValue = parseInt(this.innerText) || 0;
			if(strValue!=undefined && strValue!=null && !isNaN(strValue)){
				Format(strValue);	
			}
		}catch(err){}
	});
	/**********************************************************************
	*小数点
	***********************************************************************/
	$(frmTemplate).find("td[operate=\"pnt\"]").click(function(){
		try{													  
			var strValue = $('#frmKeyboardValue').html();
			if(strValue!=undefined && strValue!=null 
			&& strValue!="" && strValue.indexOf('.')==-1)
			{Format(".");}
		}catch(err){}
	});
	/**********************************************************************
	*删除数字
	***********************************************************************/
	$(frmTemplate).find("td[operate=\"del\"]").click(function(){
		try{
			var strValue = $('#frmKeyboardValue').html();
			if(strValue!=undefined && strValue!=null && strValue!=""){
				try{Format('del');}catch(err){}
			}
		}catch(err){}
	});
	/**********************************************************************
	*确认保存事件信息
	***********************************************************************/
	$(frmTemplate).find("td[operate=\"ok\"]").click(function(){
		try{
			var strValue = parseFloat($('#frmKeyboardValue').html()) || 1;
			if(strValue!=undefined && strValue!=null && strValue!="" 
			&& !isNaN(strValue) && strValue>=0.001 
			&& back!=undefined && back!=null && typeof(back)=='function')
			{
				try{back(strValue);}catch(err){}
				try{CloseMaster();}catch(err){}
			}
		}catch(err){}
	});
	/**********************************************************************
	*关闭窗口信息
	***********************************************************************/
	var CloseMaster = function(){
		try{
			if(frmTemplate!=undefined && frmTemplate!=null)
			{$(frmTemplate).remove();}
			if(document.getElementById('frmKeyboardMaster'))
			{$(document.getElementById('frmKeyboardMaster')).remove();}
		}catch(err){}
	};
};