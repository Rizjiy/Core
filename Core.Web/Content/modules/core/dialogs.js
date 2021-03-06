﻿
angular.module("dialogs", ["ngDialog"])
    .config(['ngDialogProvider', function (ngDialogProvider) {
        ngDialogProvider.setDefaults({
            closeByNavigation: true
        });
    }])
	.factory("$dialogs", ["ngDialog", function (ngDialog) {
	    return {

	        //#region custom

	        custom: function () {
	            /// <summary>
	            /// Предоставляет сервис ngDialog
	            /// </summary>
	            /// <returns type=""> ngDialog </returns>

	            return ngDialog;
	        },

	        //#endregion

	        //#region error

	        error: function (cfg) {
	            /// <summary>
	            /// Стандартный диалог для отображения ошибок
	            /// </summary>
	            /// <param name="cfg.header" type="string"> Заголовок диалога </param>
	            /// <param name="cfg.msg" type="string"> Тело диалога </param>
	            /// <param name="cfg.ok" type="string"> Текст кнопки </param>
	            /// <returns type="promise"></returns>

	            return ngDialog.openConfirm({
	                template: core.rootUrl + "/Content/views/core/dialogs/error.html",
	                className: "modal-dialog",
	                showClose: false,
	                closeByDocument: (!!cfg && angular.isDefined(cfg.closeByDocument) ? cfg.closeByDocument : false),
	                controller: ["$scope", "$sce",
                        function ($scope, $sce) {
                            $scope.header = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.header) ? angular.copy(cfg.header) : "Ошибка");
                            $scope.msg = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.msg) ? cfg.msg : "An unknown error has occurred.");
                            $scope.ok = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.ok) ? angular.copy(cfg.ok) : "Ok");
                        }]
	            });
	        },

	        //#endregion 

	        //#region notify

	        notify: function (cfg) {
	            /// <summary>
	            /// Стандартный диалог для отображения уведомлений
	            /// </summary>
	            /// <param name="cfg.header" type="string"> Заголовок диалога </param>
	            /// <param name="cfg.msg" type="string"> Тело диалога </param>
	            /// <param name="cfg.ok" type="string"> Текст кнопки </param>
	            /// <returns type="promise"></returns>

	        	return ngDialog.openConfirm({
	        	    template: core.rootUrl + "/Content/views/core/dialogs/notify.html",
	                className: "modal-dialog",
	                showClose: false,
	                closeByDocument: (!!cfg && angular.isDefined(cfg.closeByDocument) ? cfg.closeByDocument : true),
	                controller: ["$scope", "$sce",
                        function ($scope, $sce) {
                            $scope.header = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.header) ? angular.copy(cfg.header) : "Уведомление");
                            $scope.msg = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.msg) ? cfg.msg : "Unknown application notification.");
                            $scope.ok = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.ok) ? angular.copy(cfg.ok) : "Ok");
                        }]
	            });
	        },

	        wait: function (cfg) {
	        	/// <summary>
	        	/// Стандартный диалог для отображения ожидания
	        	/// </summary>
	        	/// <param name="cfg.msg" type="string"> Тело диалога </param>
	        	/// <returns type="promise"></returns>

	        	return ngDialog.open({
	        	    template: core.rootUrl + "/Content/views/core/dialogs/wait.html",
	        		className: "modal-dialog",
	        		showClose: false,
	        		closeByEscape: false,
	        		closeByDocument: (!!cfg && angular.isDefined(cfg.closeByDocument) ? cfg.closeByDocument : false),
	        		controller: ["$scope", "$sce",
                        function ($scope, $sce) {
                        	$scope.msg = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.msg) ? cfg.msg : "Unknown application notification.");
                        	$scope.close = function () { $modalInstance.close(); $scope.$destroy(); };                        	
                        }]
	        	});
	        },

	        //#endregion 

	        //#region confirm

	        confirm: function (cfg) {
	            /// <summary>
	            /// Стандартный диалог для подтверждений
	            /// </summary>
	            /// <param name="cfg.header" type="string"> Заголовок диалога </param>
	            /// <param name="cfg.msg" type="string"> Тело диалога </param>
	            /// <param name="cfg.yes" type="string"> Текст кнопки "Потверждаю" </param>
	            /// <param name="cfg.no" type="string"> Текст кнопки "Отклоняю" </param>
	            /// <param name="cfg.cancel" type="string"> Текст кнопки "Отмена", если не указан - кнопка не отображается </param>
	            /// <returns type="promise"></returns>
	            return ngDialog.openConfirm({
	                template: core.rootUrl + "/Content/views/core/dialogs/confirm.html",
	                className: "modal-dialog",
	                showClose: false,
	                closeByDocument: (!!cfg && angular.isDefined(cfg.closeByDocument) ? cfg.closeByDocument : true),
	                controller: ["$scope", "$sce",
                        function ($scope, $sce) {
                            $scope.header = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.header) ? angular.copy(cfg.header) : "Вопрос");
                            $scope.msg = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.msg) ? cfg.msg : "Unknown application notification.");
                            $scope.yes = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.yes) ? angular.copy(cfg.yes) : "Да");
                            $scope.no = $sce.trustAsHtml(!!cfg && angular.isDefined(cfg.no) ? angular.copy(cfg.no) : "Нет");
                            $scope.canCancel = !!cfg && angular.isDefined(cfg.cancel);
                            $scope.cancel = $sce.trustAsHtml($scope.canCancel ? angular.copy(cfg.cancel) : "Отмена");
                        }]
	            });
	        }

	    };
	}]); 
