DemoModule.controller("DialogListController", ["$scope", "$dialogs",
    function ($scope, $dialogs) { 

    //#region Диалоги

    $scope.dialog1 = function () {
        $dialogs.custom().openConfirm({
            template: "views/Dialogs/Dialog1.html",
            className: "modal-dialog",
            showClose: false
        }).then(function (arg) {
            alert("Confirm: " + arg);
        }, function (arg) {
            alert("Cancel: " + arg);
        });
    };

    $scope.dialog2 = function () {
        $dialogs.error({ msg: "Ошибка в программе!" })
            .then(function () {
                alert('"error" ready');
            });
    };

    $scope.dialog3 = function () {
        $dialogs.notify({ msg: "Получилось!" })
            .then(function () {
                alert('"notify" ready');
            });
    };

    $scope.dialog4 = function () {
        $dialogs.confirm({ msg: "Согласен ?", cancel: "Отмена" })
            .then(function (result) {
                alert('"confirm" result = ' + result);
            });
    };

    $scope.dialog5 = function () {
        $dialogs.confirm({ msg: "Уверен ?" })
            .then(function (result) {
                alert('"confirm" result = ' + result);
            });
    };

    //#endregion

}]);