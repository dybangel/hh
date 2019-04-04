var PowerList = 
{
	"list":[
		{"name":"内容管理",options:[{"char":"系统参数配置"},{"char":"模型管理"},{"char":"内容管理"},{"char":"标签模版"},{"char":"广告系统"},{"char":"友情连接"},{"char":"投票系统"},{"char":"在线表单"},{"char":"发布管理"}]},
		{"name":"用户系统",options:[{"char":"用户参数配置"},{"char":"用户管理"},{"char":"余额明细"},{"char":"余额充值"},{"char":"积分明细"},{"char":"积分充值"},{"char":"数据报表"},{"char":"登陆历史"},{"char":"用户提现"},{"char":"用户签到"},{"char":"留言反馈"}]},
		{"name":"任务中心",options:[{"char":"渠道管理"},{"char":"统计报表"},{"char":"任务记录"}]},
		{"name":"自有渠道",options:[{"char":"应用分类"},{"char":"应用管理"},{"char":"计划任务"},{"char":"下载记录"},{"char":"截图审核"}]}
	]
}
var loadPower=function()
{
	var strTxt = "<table cellpadding=\"3\" width=\"100%\" cellspacing=\"1\" class=\"table1\" border=\"0\">";
	for(var k in PowerList.list){
		var thisJSON = PowerList.list[k];
		strTxt += "<tr class=\"hback\">";
		strTxt += "<td class=\"tips\">"+thisJSON.name+"</td>";
		strTxt += "<td class=\"check_box\">";
		var j = 0;
		for(var n in thisJSON.options){
			j = j+1;
			var JSON = thisJSON.options[n];
			strTxt += "<label";
			if(PowerXML.indexOf(JSON.char)!=-1){
				strTxt+=" class=\"current\"";	
			}
			strTxt +=">";
			strTxt += "<input type=\"checkbox\" name=\"PowerXML\" value=\""+JSON.char+"\" ";
			if(PowerXML.indexOf(JSON.char)!=-1){strTxt+=" checked";}
			strTxt +="/>"+JSON.char+"";
			strTxt += "</label>";
			if(j>8){strTxt+="<br/>";j=0}
		}
		strTxt +="</td>";
		strTxt += "</tr>";
	}
	strTxt += "</table>";
	$("#power-frm-td-box").html(strTxt);
}

loadPower();