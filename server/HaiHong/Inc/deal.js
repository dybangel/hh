var doSubmit = function(frmAction)
{
	try{
		if(frmAction!=undefined && frmAction!=null && frmAction!=""){
			$("#frmAction").val(frmAction);
			var strAction = document.querySelector("#frmAction").value || "";
			if(strAction!=undefined && strAction!=null && strAction!="" && strAction!="none" 
			&& document.querySelector('#formContianer'))
			{
				ajaxSubmit(document.querySelector('#formContianer'));
			}else{
				try{
					if(window.messagerAlert!=undefined && window.messagerAlert !=null 
					&& typeof(window.messagerAlert)=='function')
					{window.messagerAlert({"type":"alert","tips":"非法应用处理,请重试"});}
					else{lert('非法应用处理,请重试！');return false;}
				}catch(err){}	
			}
		}
		else
		{
			try{
				if(window.messagerAlert!=undefined && window.messagerAlert !=null 
				&& typeof(window.messagerAlert)=='function')
				{window.messagerAlert({"type":"alert","tips":"非法应用处理,请重试"});}
				else{lert('非法应用处理,请重试！');return false;}
			}catch(err){}
		}
	}catch(err){}
};
/***************************************************************************
*操作处理应用
****************************************************************************/
$(function(){
	$("#frmUnpaid").click(function(){
		doSubmit("Unpaid");
	});	 
	$("#frmReceiva").click(function(){
		doSubmit("Receiva");
	});	
	$("#frmPayment").click(function(){
		var fileValue = document.querySelector('#frmFile').value;
		if(fileValue!=undefined && fileValue!=null 
		&& fileValue!=""){
			doSubmit("Payment");	
		}else{
			try{
				if(window.messagerAlert!=undefined && window.messagerAlert !=null 
				&& typeof(window.messagerAlert)=='function')
				{window.messagerAlert({"type":"alert","tips":"请上传支付截图"});}
				else{lert('请上传支付截图！');return false;}
			}catch(err){}	
		}
		
	});
	$("#frmCollection").click(function(){
		doSubmit("UnCollection");
	});
});