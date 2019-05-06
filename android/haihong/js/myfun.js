//读取配置文件
	if(typeof(Gconfigjson)!="undefined"){
	configjson=Gconfigjson;
	}else{
	configjson="config/config.json";	
	}
	$.ajax({
        url : configjson,
        type : 'get',
        dataType : 'json',
		async:false,
		success : function(res){
    	//alert(res.server);  
        Gserver=res.server;
        Gimgserver=res.imgserver;
        Gmainviewid=res.mainviewid;
       // alert("gserver="+Gserver);
        }
	});

//计算data数量的函数
function JSONLength(obj) {
			var size = 0, key;
			for (key in obj) {
			if (obj.hasOwnProperty(key)) size++;
			}
			return size;
};	

function closeme(){
 	var ws=plus.webview.currentWebview();
 	plus.webview.close(ws);
 }

function getsendsms(action,par){
	 var Gurl=Gimgserver+"/api/user.aspx?action=SendMobile"+par;
    //console.log("url is ++++++++++++++:"+Gurl);
	var actions=action;
	var action=actions.split("|")[0];
	var action_2=actions.split("|")[1];
    var action_3=actions.split("|")[2];
   
    //alert('action ready');
	 $.ajax({
      		type:"get",
      		url:Gurl,
			async:true,
      		//data:{"action":action},
      		dataType:"json", // 因为PHP返回数据是JSON格式，所以这里类似要用JSON
      		success:function(data){

      			if(data!="null"){	
      				try{		

      					
      						if (typeof(action_2)=="undefined") {
      						//alert('test='+data);
      					//	eval(action+'(data)');
							//						alert('不用模板机制');
							}else{
								var tpid=$('#'+action+'_template')[0];
								if(typeof(tpid)=="undefined"){
									alert("模板："+action+"_template 不存在，请设置");
									return;
								}
								var conid=$('#'+action+'_container')[0];
								if(typeof(conid)=="undefined"){
									alert("容器："+action+"_container 不存在，请设置");
									return;
								}
								
								var template = $('#'+action+'_template')[0];
								var  dot = doT.template(template.innerHTML);
								if(action_3=="append"){
									$('#'+action+'_container')[0].innerHTML+=dot(data);
								}else{
									$('#'+action+'_container')[0].innerHTML=dot(data);
								}
								//alert('需要找模板绑定');
							}
						//再执行同action私有实现函数以满足一些特殊要求
						//alert("eval run.."+action);
      					eval(action+'(data)');  		
      					}catch(e){
      					//TODO handle the exception
      					//alert(e);
      					}								
      							}else{
      								console.log(action+'返回空值，增加一个procedure返回值');
      								eval(action+'(data)');  
      								//$('.mui-spinner').removeClass('mui-spinner');
									//$('.mui-pull-caption-refresh')[0].innerHTML='没有更多的数据了';
      							}
								 },
			error: function(xhr, type, errorThrown) {
					console.log(errorThrown);
							 						}
			})
}

function getjson(action,par){
	var actions=action;
	var action=actions.split("|")[0];
	var action_2=actions.split("|")[1];
    var action_3=actions.split("|")[2];
    console.log("appid is:"+plus.push.getClientInfo().appid);
    //pPyZWvH3Fa6PXba10aJ009 hbulider
    //alert('action ready');
	 $.ajax({
      		type:"get",
      		url:Gserver+"/api.aspx?"+par,//+"&appid="+plus.push.getClientInfo().appid 
			async:true, 
      		data:{"appid":plus.push.getClientInfo().appid,"action":action}, 
      		dataType:"json", // 因为PHP返回数据是JSON格式，所以这里类似要用JSON
      		success:function(data){ 

      			if(data!="null"){	
      				try{		

      					
      						if (typeof(action_2)=="undefined") {
      						//alert('test='+data);
      					//	eval(action+'(data)');
							//						alert('不用模板机制');
							}else{
								var tpid=$('#'+action+'_template')[0];
								if(typeof(tpid)=="undefined"){
									alert("模板："+action+"_template 不存在，请设置");
									return;
								}
								var conid=$('#'+action+'_container')[0];
								if(typeof(conid)=="undefined"){
									alert("容器："+action+"_container 不存在，请设置");
									return;
								}
								
								var template = $('#'+action+'_template')[0];
								var  dot = doT.template(template.innerHTML);
								if(action_3=="append"){
									$('#'+action+'_container')[0].innerHTML+=dot(data);
								}else{
									$('#'+action+'_container')[0].innerHTML=dot(data);
								}
								//alert('需要找模板绑定');
							}
						//再执行同action私有实现函数以满足一些特殊要求
						//alert("eval run.."+action);
      					eval(action+'(data)');  		
      					}catch(e){
      					//TODO handle the exception
      					//alert(e);
      					}								
      							}else{
      								console.log(action+'返回空值，增加一个procedure返回值');
      								eval(action+'(data)');  
      								//$('.mui-spinner').removeClass('mui-spinner');
									//$('.mui-pull-caption-refresh')[0].innerHTML='没有更多的数据了';
      							}
								 },
			error: function(xhr, type, errorThrown) {
					console.log(errorThrown);
							 						}
			})
}

//打开新窗体优化
function openwin_plus(winpath,par){
	mui.openWindow({
    url:winpath,
    id:winpath,
    styles:{
      top:'0px',//新页面顶部位置
      bottom:'0px',//新页面底部位置
      //width:newpage-width,//新页面宽度，默认为100%
      //height:newpage-height,//新页面高度，默认为100%
     // ......
    },
    extras:{
    	pars:par
     // .....//自定义扩展参数，可以用来处理页面间传值
    },
    createNew:false,//是否重复创建同样id的webview，默认为false:不重复创建，直接显示
    show:{
      autoShow:true,//页面loaded事件发生后自动显示，默认为true
      aniShow:"slide-in-right",//页面显示动画，默认为”slide-in-right“；
      duration:50 //页面动画持续时间，Android平台默认100毫秒，iOS平台默认200毫秒；
    },
    waiting:{
      autoShow:true,//自动显示等待框，默认为true
      title:'正在拼命加载...',//等待对话框上显示的提示内容
      options:{
        width:'120px',//等待框背景区域宽度，默认根据内容自动计算合适宽度
        height:'100px',//等待框背景区域高度，默认根据内容自动计算合适高度
      //  ......
      }
    }
})
	
}

//接受数值 可打印调试Gparent_value
function getvalue_plus(){
	var pars=plus.webview.currentWebview().pars;
	var cmds='';
	for(var key in pars){
   		var cmd=''+key+'="'+pars[key]+'";'
   		cmds +=cmd;
		eval(cmd);
		}
	Gvalue_plus=cmds;
	//alert("This is fun.js info:"+Gvalue_plus);
}
 
//全文替换
function replaceall(str,src,dst){
	var reg1=new RegExp(src,"g"); //创建正则RegExp对象   
    str=str.replace(reg1,dst);
    return str;
}
//根据屏幕修改html front-size
//	(function (doc, win) {
//		try{
//			if(Gnoresize==true){
//			return;
//		}
//		}catch(e){
//			//TODO handle the exception
//		}
//      var docEl = doc.documentElement,
//          resizeEvt = "orientationchange" in window ? "orientationchange" : "resize",
//          recalc = function () {
//              var clientWidth = docEl.clientWidth;
//              if (!clientWidth) return;
//             //   alert(clientWidth);
//              //iphone5
//              if(320<=clientWidth && clientWidth<375){
//                  docEl.style.fontSize = "16px";
//              }else if(375<=clientWidth && clientWidth<414){
//              //	alert("i6");
//                  docEl.style.fontSize = "21px";
//              }else if(415<=clientWidth && clientWidth<768){
//                  docEl.style.fontSize = "28px";
//              }
//              
//          };
//      if (!doc.addEventListener) return;
//      win.addEventListener(resizeEvt, recalc, false);
//      doc.addEventListener("DOMContentLoaded", recalc, false);
//  })(document, window);
    //日期格式化函数
	Date.prototype.Format = function(fmt)   
							{ //author: meizz   
							  var o = {   
							    "M+" : this.getMonth()+1,                 //月份   
							    "d+" : this.getDate(),                    //日   
							    "h+" : this.getHours(),                   //小时   
							    "m+" : this.getMinutes(),                 //分   
							    "s+" : this.getSeconds(),                 //秒   
							    "q+" : Math.floor((this.getMonth()+3)/3), //季度   
							    "S"  : this.getMilliseconds()             //毫秒   
							  };   
							  if(/(y+)/.test(fmt))   
							    fmt=fmt.replace(RegExp.$1, (this.getFullYear()+"").substr(4 - RegExp.$1.length));   
							  for(var k in o)   
							    if(new RegExp("("+ k +")").test(fmt))   
							  fmt = fmt.replace(RegExp.$1, (RegExp.$1.length==1) ? (o[k]) : (("00"+ o[k]).substr((""+ o[k]).length)));   
							  return fmt;   
							}
							
	var wgtWaiting = null;  

function downloadWgt(strInstall,functions) {  
		//console.log("myfun callback is:"+callback);
	var action=functions;
	//eval(action+'()');
    
    wgtWaiting = plus.nativeUI.showWaiting("开始下载");
    var wgtUrl = strInstall; 
    var wgtOption = { filename: "_doc/update/", retry: 1 };  
    var task = plus.downloader.createDownload(wgtUrl, wgtOption, function (download, status) {  
        if (status == 200) {  
            wgtWaiting.setTitle("开始安装");  
            if("update"==action){
    		updateWgt(download.filename);  
    		}else{
    		installApp(download.filename,action);  	
    		}
            
        } else {  
            mui.alert("安装包下载失败！");  
            wgtWaiting.close();  
        }  
    });  
    task.addEventListener("statechanged", function (download, status) {  
        switch (download.state) {  
            case 2:  
                wgtWaiting.setTitle("已连接到服务器");  
                break;  
            case 3:  
                var percent = download.downloadedSize / download.totalSize * 100;  
                wgtWaiting.setTitle("已下载 " + parseInt(percent) + "%");  
                break;  
            case 4:  
                wgtWaiting.setTitle("下载完成");  
                break;  
        }  
    });  
    task.start();  
};  

function installApp(wgtpath,functions) {  
// var action=functions;	
    plus.runtime.install(wgtpath, {}, function (wgtinfo) {     
        //eval(action+'()');
        wgtWaiting.close();        
    }, function (error) {  
        wgtWaiting.close();  
      //  mui.alert("应用更新失败！\n" + error.message);  
    });  
}; 

function updateWgt(wgtpath) {  
// var action=functions;	
    plus.runtime.install(wgtpath, {}, function (wgtinfo) {     
         mui.alert("更新完成，须重启应用！", function () {  
          plus.runtime.restart();  
      });  
        wgtWaiting.close();        
    }, function (error) {  
        wgtWaiting.close();  
        mui.alert("应用更新失败！\n" + error.message);  
    });  
}; 
function checkapp(packname){
//var packageName = 'com.tencent.mm';
//var main = plus.android.runtimeMainActivity();  
//var packageManager = main.getPackageManager();  
//var PackageManager = plus.android.importClass(packageManager)  
//var packageInfo = packageManager.getPackageInfo(packageName,PackageManager.GET_ACTIVITIES);  
//if(packageInfo ){  
//  alert('已安装' + packageName + '')  
//}  
if(plus.runtime.isApplicationExist({pname:packname})){
		return 1;
	}else{
		return 0;
	}

}

function openapp(packname,function1,function2){
	//console.log("this is openapp diaoyong callback");
	//eval(function1+'()');
	plus.runtime.launchApplication(		
		{pname:packname}, 
		function ( e ) {
				alert( "failed: " + e.message );
				//eval(function2+'()');
})
	
}
//获取cpu信息
function getCpuInfo() {
 var cpuInfo = '/proc/cpuinfo';
 var temp = '',
  cpuHardware;
 var fileReader = plus.android.importClass("java.io.FileReader");
 var bufferedReader = plus.android.importClass("java.io.BufferedReader");
 var FileReader = new fileReader(cpuInfo);
 var BufferedReader = new bufferedReader(FileReader, 8192);
 while ((temp = BufferedReader.readLine()) != null) {
  if (-1 != temp.indexOf('Hardware')) {
   cpuHardware = temp.substr(parseInt(temp.indexOf(":")) + 1);
  }
 }
 return cpuHardware;
}

//本地数据存储
function setItemFun(key,value) {
		plus.storage.setItem( key, value );		
		//mui.toast( "数据存储成功，存储了"+key+" value为；"+value );
}
//循环打印obj
function printobj(obj){
for(var key in obj){
  console.log('key='+key+'  value='+obj[key]);
}
	
}
//function copytoclip(mystr){
//	var tempstr=mystr;
//  var Context = plus.android.importClass("android.content.Context");
//  var main = plus.android.runtimeMainActivity();
//  var clip = main.getSystemService(Context.CLIPBOARD_SERVICE);
//  plus.android.invoke(clip,"setText",tempstr);
//}
//复制内容到剪切板
function copy_fun(copy){//参数copy是要复制的文本内容
	mui.plusReady(function(){
		//判断是安卓还是ios
		if(mui.os.ios){
			//ios
			var UIPasteboard = plus.ios.importClass("UIPasteboard");
		    var generalPasteboard = UIPasteboard.generalPasteboard();
		    //设置/获取文本内容:
		    generalPasteboard.plusCallMethod({
		        setValue:copy,
		        forPasteboardType: "public.utf8-plain-text"
		    });
		    generalPasteboard.plusCallMethod({
		        valueForPasteboardType: "public.utf8-plain-text"
		    });
		    mui.toast("已成功复制到剪贴板");
		}else{
			//安卓
			var context = plus.android.importClass("android.content.Context");
			var main = plus.android.runtimeMainActivity();
			var clip = main.getSystemService(context.CLIPBOARD_SERVICE);
			plus.android.invoke(clip,"setText",copy);
			mui.toast("已成功复制到剪贴板");
		}
	});
}

//function showmessage(title,content,nofun,yesfun){
//		var btnArray = ['否', '是'];
//  var message = "<h4>"+content+"</h4>";
// 	//console.log("yesfun  is:"+yesfun);
// 	Gyesfun=yesfun;
// 	Gnofun=nofun;
//  //$('#alert-bg').css('display','block');
//  mui.confirm(message, title, btnArray, function(e,yesfun,nofun) {
//  	console.log("dddyesfun  is:"+Gyesfun);
//  	//printobj(e);
//      if (e.index == 1) {
//      	eval(Gyesfun+'();');
//        //  alert("shi"); 
// 
//      } else {
//      	eval(Gnofun+'();');
//      	//alert("0");
//
//      }
//  },'div');
//}



