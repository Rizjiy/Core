<%@ Page Language="C#" %>
<%@ Import Namespace="Core.Web.Core" %>
<% AspxUtils.ExportToHtml(); %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" ng-app="DemoModule">
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>CoreWeb Demo</title>

<% AspxUtils.GetHashedLink("Content.Demo/styles/demo.all.min.css", "    <link href='/{0}' rel='stylesheet' />"); %> <%--Нельзя ставить после ngDialog.css - сыезжает диалоговое окно!--%>
<% AspxUtils.BundleFiles("Content/styles/core.all.min.css", "    <link href='/{0}' rel='stylesheet' />", !AspxUtils.IsDebugConfiguration); %>
<% AspxUtils.BundleFiles("Content/js/vender.all.min.js", "    <script src='/{0}'></script>", !AspxUtils.IsDebugConfiguration); %>
<% AspxUtils.BundleFiles("Content/js/core.all.min.js", "    <script src='/{0}'></script>", !AspxUtils.IsDebugConfiguration); %>
<% AspxUtils.BundleFiles("Content.Demo/modules/demo.all.min.js", "    <script src='/{0}'></script>", !AspxUtils.IsDebugConfiguration); %>

    <script>
        core.rootUrl = "";
    </script>

</head>
<body ng-controller="IndexController">
    <div ui-view></div>
</body>
</html>
