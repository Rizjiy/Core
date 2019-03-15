var DemoModule = angular.module("DemoModule", ["coreModule"]);

DemoModule.config(["$stateProvider", "$urlRouterProvider", "$locationProvider",
    function ($stateProvider, $urlRouterProvider, $locationProvider) {
        $locationProvider.hashPrefix('');
    	$urlRouterProvider.otherwise("/table-template-grid");
        $stateProvider
            .state("Dialogs",
			{
			    url: "/dialog-list",
                templateUrl: "views/Dialogs/DialogList.html",
                controller: "DialogListController"
			})
            .state("TableTemplateGrid",
			{
			    url: "/table-template-grid",
			    templateUrl: "views/TableTemplateGrid/TableTemplateGrid.html",
			    controller: "TableTemplateGridController"
			})
			.state("WaitDialog",
			{
			    url: "/wait-dialog",
			    templateUrl: "views/WaitDialog/WaitDialog.html",
			    controller: "WaitDialogController"
            })
			.state("ServiceGenTest",
            {
                url: "/service-gen-test",
                templateUrl: "views/ServiceGenTest/ServiceGenTest.html",
                controller: "ServiceGenTestController"
            });
    }
]);

DemoModule.service("application", ["$rootScope", "$window", "$http", "$state", "$urlRouter", "$q",
    function ($rootScope, $window, $http, $state, $urlRouter, $q) {
    /// <summary>
    /// Базовая инициализация сервиса application,
    /// </summary>
    /// <param name="$rootScope" type="type"></param>
    /// <param name="$window" type="type"></param>
    /// <param name="$http" type="type"></param>
    /// <param name="$state" type="type"></param>
    /// <param name="$urlRouter" type="type"></param>
    /// <param name="$resource) {" type="type"></param>
         core.intializeApplicationBase(this, $rootScope, $window, $http, $state, $urlRouter, $q);
}]);

DemoModule.controller("IndexController", ["application",
    function (application) {

}]);