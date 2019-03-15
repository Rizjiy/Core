DemoModule.controller("ServiceGenTestController", ["$scope", "TestServiceGenService",
    function ($scope, service) {

        $scope.contentInfo = undefined;

        $scope.excelExport = function () {
            service.ExcelExport(new Date, new Date().addDays(5))
                .then(function () {
                    $scope.contentInfo = "ExcelExport Успех";
                });
        };

        //страница редактирования
        $scope.item = {};
        $scope.item.Timeout = 1;
        $scope.save = function (e) {
            //отменяем submit
            e.preventDefault();

            if (!$scope.validator.validate())
                return;

            $scope.progressMassage = "отправка начата";
            service.Save($scope.item, $scope)
                .then(function () {
                    $scope.progressMassage = "отправка завершена";
                });

        };
    }
]);