DemoModule.controller("TableTemplateGridEditDialogController", ["$scope", "$http", "$state", "TableTemplateGridService", "dep", "$dialogs",
function ($scope, $http, $state, tableTemplateGridService, dep, $dialogs) {

        $scope.curItem = dep.curItem;

        //Сохраним ставку
        $scope.save = function () {

            tableTemplateGridService.Save($scope.curItem).then(
                function (response) {
                    dep.refresh();
                    $scope.closeThisDialog();

                })
                .catch(function (error) {
                    core.errorMessage(error, $dialogs);
                });
        }

    }]);
