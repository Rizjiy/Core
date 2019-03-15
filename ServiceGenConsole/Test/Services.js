

function AddShowErrorDialogCatch(promise, showErrorDialog, dialogs) {
    if (showErrorDialog)
        promise.catch(
            function (error) {
                core.errorMessage(error, dialogs);
            }
        );
};


  DemoModule.service("TableTemplateGridService", ["$http", "$dialogs",
      function($http, $dialogs) {
          return {
              ShowErrorDialog: true,
      GetById: function (dto) {
          var promise = $http({
              url: "/api/TableTemplateGrid/GetById",
              method: "POST",
              data: dto,
              });
          AddShowErrorDialogCatch(promise, this.ShowErrorDialog, $dialogs);
          return promise;
        },
      GetException: function (dto) {
          var promise = $http({
              url: "/api/TableTemplateGrid/GetException",
              method: "POST",
              data: dto,
              });
          AddShowErrorDialogCatch(promise, this.ShowErrorDialog, $dialogs);
          return promise;
        },
      GetList: function (options, filter) {
          var promise = core.dsRead("/api/TableTemplateGrid/GetList", options, filter);
          AddShowErrorDialogCatch(promise, this.ShowErrorDialog, $dialogs);
          return promise;
        },
      GetValidationException: function () {
          var promise = $http({
              url: "/api/TableTemplateGrid/GetValidationException",
              method: "POST",
              });
          AddShowErrorDialogCatch(promise, this.ShowErrorDialog, $dialogs);
          return promise;
        },
      Save: function (dto) {
          var promise = $http({
              url: "/api/TableTemplateGrid/Save",
              method: "POST",
              data: dto,
              });
          AddShowErrorDialogCatch(promise, this.ShowErrorDialog, $dialogs);
          return promise;
        },
  };
  }]);
  DemoModule.service("TestServiceGenService", ["$http", "$dialogs",
      function($http, $dialogs) {
          return {
              ShowErrorDialog: true,
      ExcelExport: function (startDatePeriod, endDatePeriod) {
          var promise = $http({
              url: "/api/TestServiceGen/ExcelExport?startDatePeriod=&amp;endDatePeriod=",
              method: "GET",
              params: {startDatePeriod: startDatePeriod,endDatePeriod: endDatePeriod},
              });
          AddShowErrorDialogCatch(promise, this.ShowErrorDialog, $dialogs);
          return promise;
        },
  };
  }]);

