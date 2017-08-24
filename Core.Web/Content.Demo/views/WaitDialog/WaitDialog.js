DemoModule.controller("WaitDialog", ["$scope", "$http", "$state", "DemoModule", "$dialogs",
	function ($scope, $http, $state, WaitDialogService, $dialogs) {
		$scope.showDialog = function () {
			$scope.waitDialog = $dialogs.wait({
				className: "modal-dialog",
				//closeByDocument: false, //клик на зону вне диалога НЕ закроет диалог. по умолчанию false
				msg: "Пожалуйста, ждите"
			});
			setTimeout($scope.waitDialog.close(), 5000);
		}
	}]);