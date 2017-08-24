DemoModule.controller("TableTemplateGridController", ["$scope", "$http", "$state", "$dialogs", "TableTemplateGridService",
function ($scope, $http, $state, $dialogs, tableTemplateGridService) {
    //tableTemplateGridService.ShowErrorDialog = false;

    $scope.dsReadCount = 0;

    //#region ==============  Элемент управления для отображения Главного Грида
    $scope.source = new kendo.data.DataSource({
        //sort: [{ field: "FirmName", dir: "asc" }, { field: "ClientId", dir: "desc" }],
        pageSize: 25,
        serverSorting: true,
        serverFiltering: true,
        //serverPaging: true,
        type: "odata",

        transport: {
            read: function (options) {
                var filter = {};
                tableTemplateGridService.GetList(options, filter)
                    .then(
                    function (response) {
                        $scope.dsReadCount += 1;
                    },
                    null
                );
            }
        },
        requestEnd: function (e) {
        }
    });

    $scope.mainGridOptions = {
        dataSource: $scope.source,
        // pageable: true,
        sortable: true,
        filterable: true,
        resizable: true,
        rowTemplate: $("#row-template").html(),
    };
    //#endregion =================================

    $scope.openEditDialog = function (dataItem) {

        tableTemplateGridService.GetById(dataItem).then(
            function (response) {
                var dlg = $dialogs.custom().open({

                    template: "views/TableTemplateGrid/EditDialog.html",
                    className: "modal-dialog",
                    controller: "TableTemplateGridEditDialogController",
                    showClose: false,
                    resolve: {
                        dep: function depFactory() {
                            return {
                                curItem: response.data,
                                refresh: function () {
                                    $scope.source.read();
                                }
                            };
                        }
                    }
                });
            })
    };

    //#region Обработка ошибок

    $scope.getException = function () {

        var dto = { Id: "123", DateFrom: new Date(), Rate: 0.01 };

        tableTemplateGridService.GetException(dto);
    }

    $scope.getValidationException = function () {

        tableTemplateGridService.GetValidationException();
    }

    //#endregion

}]);