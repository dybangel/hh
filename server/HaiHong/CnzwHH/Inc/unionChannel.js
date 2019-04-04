var changeConfig=function(params)
{
	try{$("input[name=\"OpenName\"]").val(params.user);	}catch(e){}
	try{$("input[name=\"NumberName\"]").val(params.point);}catch(err){}
	try{$("input[name=\"ApplicationID\"]").val(params.appid);}catch(err){}
	try{$("input[name=\"ApplicationName\"]").val(params.appname);}catch(err){}
};

var DoUnionModel = function(obj)
{
	try{$("input[name=\"strUnion\"]").val(obj.value);}
	catch(err){}
	try{
		switch(obj.value){
			case "有米":changeConfig({user:'user',point:"points",appid:"adid",appname:"ad"});break;
			case "点入":changeConfig({user:'userid',point:"point",appid:"adid",appname:"adname"});break;
			case "赢告":changeConfig({user:'key',point:"pay",appid:"rid",appname:"appname"});break;
			case "爱普动力":changeConfig({user:'identifier',point:"point",appid:"campaign_id ",appname:"appname"});break;
			case "万普":changeConfig({user:'key',point:"points",appid:"app_Id",appname:"ad_name"});break;
			case "贝多":changeConfig({user:'userid',point:"currency",appid:"app_id",appname:"ad_name"});break;
			case "椰果":changeConfig({user:'userid',point:"point",appid:"appid",appname:"adname"});break;
			case "点乐":changeConfig({user:'snuid',point:"currency",appid:"order_id",appname:"ad_name"});break;
			case "果盟":changeConfig({user:'other',point:"points",appid:"adsid",appname:"ad"});break;
			case "米迪":changeConfig({user:'param0',point:"cash",appid:"bundleId",appname:"appName"});break;
			case "易积分":changeConfig({user:'userid',point:"score",appid:"adId",appname:"adName"});break;
			case "指盟":changeConfig({user:'UID',point:"POINTS",appid:"JOBID",appname:"APPNAME"});break;
			case "多盟":changeConfig({user:'user',point:"point",appid:"adid",appname:"ad"});break;
			case "有盟":changeConfig({user:'other',point:"add_points",appid:"advert_id",appname:"adhibition_name"});break;
			case "第七传媒":changeConfig({user:'adWid',point:"adPoint",appid:"app_id",appname:"adName"});break;
			case "力美":changeConfig({user:'aid',point:"point",appid:"aduid",appname:"title"});break;
			case "点财":changeConfig({user:'userId',point:"score",appid:"appId",appname:"adName"});break;
			case "磨盘":changeConfig({user:'param0',point:"cash",appid:"call_adid",appname:"appName"});break;
			case "交点":changeConfig({user:'uid',point:"score",appid:"orderId",appname:"appName"});break;
			case "中亿":changeConfig({user:'other',point:"integral",appid:"adid",appname:"ad"});break;
			case "炅友":changeConfig({user:'user',point:"points",appid:"appid",appname:"adname"});break;
			case "麒点":changeConfig({user:'other',point:"point",appid:"adsid",appname:"adtitle"});break;
			case "安沃":changeConfig({user:'keyword',point:"point",appid:"adid",appname:"adname"});break;
			case "行云":changeConfig({user:'userid',point:"score",appid:"appid",appname:"appName"});break;
			case "趣米":changeConfig({user:'user',point:"points",appid:"app",appname:"ad"});break;
			case "酷果":changeConfig({user:'userid',point:"points",appid:"advertid",appname:"advertName"});break;
			case "触控":changeConfig({user:'token',point:"coins",appid:"adid",appname:"adtitle"});break;
			case "巨朋":changeConfig({user:'userdef',point:"score",appid:"adid",appname:"adShowName"});break;
			case "巨掌":changeConfig({user:'appuserid',point:"point",appid:"adid",appname:"adname"});break;
			case "艾德":changeConfig({user:'userid',point:"points",appid:"adid",appname:"adname"});break;
			case "大头鸟":changeConfig({user:'userid',point:"currency",appid:"app_id",appname:"adname"});break;
		}
	}catch(err){}
};