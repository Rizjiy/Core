DemoModule.controller("TableTemplateGridController", ["$scope", "$http", "$state", "$dialogs", "TableTemplateGridService",
function ($scope, $http, $state, $dialogs, tableTemplateGridService) {
    //tableTemplateGridService.ShowErrorDialog = false;

    $scope.startDT = new Date(2018, 3, 1);
    $scope.endDT = new Date(2018, 3, 30);

    $scope.dsReadCount = 0;
    $scope.options = undefined;

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
                $scope.options = options;
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
        },
        aggregate: [{ field: "Rate", aggregate: "sum" }]
    });

    $scope.mainGridOptions = {
        dataSource: $scope.source,
        pageable: true,
        sortable: true,
        filterable: true,
        resizable: true,
        rowTemplate: $("#row-template").html(),
        columns: [{ field: "DateFrom", title: "Дата с" },
            { field: "Rate", title: "toLocaleString('ru-RU')" },
            { field: "Rate", title: "без форматирования", aggregates: ["sum"], footerTemplate: "Sum: #=sum#" },
            { field: "Rate", title: "number"},
            { field: "Rate", title: "numberRu:4" },
            { field: "Rate", title: "numberRu:2" },
            { field: "Rate", title: "numberRu:0" },
            { field: "Rate", title: "numberRu" },
            { field: "Rate", title: "Комментарий"},
            { title: "Действие"}
        ]
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

    $scope.downloadExcel = function ()
    {
        var filter = {};
        var options = $scope.options;

        options.data.filterDto = filter;

        tableTemplateGridService.DownloadExcel(options, filter);
     
    }

}]);