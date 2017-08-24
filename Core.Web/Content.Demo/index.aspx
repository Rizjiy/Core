<%@ Page Language="C#" %>
<%@ Import Namespace="Core.Web.Core" %>
<% AspxUtils.ExportToHtml(); %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" ng-app="DemoModule">
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>WebDO</title>

<% AspxUtils.BundleFiles("Content/styles/core.all.min.css", "    <link href='/{0}' rel='stylesheet' />", !AspxUtils.IsDebugConfiguration); %>
<% AspxUtils.BundleFiles("Content/js/core.all.min.js", "    <script src='/{0}'></script>", !AspxUtils.IsDebugConfiguration); %>
<% AspxUtils.GetHashedLink("Content/js/core.extensions.js", "    <script src='/{0}'></script>"); %>
    <script src="modules/DemoModule.js"></script>

    <script src="services/Services.js"></script>
    <script src="services/WaitDialogService.js"></script>

    <script src="views/Dialogs/DialogList.js"></script>  
    <script src="views/TableTemplateGrid/TableTemplateGrid.js"></script>
    <script src="views/TableTemplateGrid/EditDialog.js"></script>
	<script src="views/WaitDialog/WaitDialog.js"></script>

    <script>
        core.rootUrl = "/Content/";
    </script>

</head>
<body ng-controller="IndexController">
    <div ui-view></div>
</body>
</html>
