<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Single.aspx.cs" Inherits="Fooke.Web.Admin.Single" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>单页列表函数</title>
<link href="../template/images/style.css" rel="stylesheet" type="text/css" />
<link href="../template/images/jquery-ui.css" rel="stylesheet" type="text/css" />
<link href="../inc/jquery-easyui/themes/default/easyui.css" rel="stylesheet" type="text/css" />
<link href="../inc/jquery-easyui/themes/icon.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" language="javascript" src="../inc/jquery.js"></script>
<script type="text/javascript" language="javascript" src="../inc/jquery-ui.js"></script>
<script type="text/javascript" language="javascript" src="../inc/jquery.form.js"></script>
<script type="text/javascript" language="javascript" src="../inc/forwork.js"></script>
<script type="text/javascript" language="javascript" src="../inc/jquery-easyui/jquery.easyui.min.js"></script>
<script type="text/javascript" language="javascript" src="../inc/WdatePicker.js"></script>
<script type="text/javascript" language="javascript" src="../inc/label.js"></script>
</head>
<body>
<div class="tips_menu_bar">
<%Fooke.Code.LabelHelper.Menus("单页列表"); %>
</div>
<div class="clear" id="FRMSpacing"></div>
<%
    switch (this.strRequest)
    {
        case "add": this.Add();break;
        case "edit": this.Update(); break;
    }    
%>
</body>
</html>


