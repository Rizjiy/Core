DemoModule.service("TableTemplateGridService", ["$rootScope", "$window", "$http",
    function ($rootScope, $window, $http) {

        return {
            GetList: function (options, filter) {
                return core.dsRead("/api/TableTemplateGrid/GetList", options, filter)
            },
            GetById: function (dto) {
                return $http({
                    url: "/api/TableTemplateGrid/GetById",
                    method: "Post",
                    data: dto
                })
            },
            Save: function (dto) {
                return $http({
                    url: "/api/TableTemplateGrid/Save",
                    method: "POST",
                    data: dto
                })
            },

            GetValidationException: function () {
                return $http({
                    url: "/api/TableTemplateGrid/GetValidationException",
                    method: "POST"
                })
            },
            GetException: function (dto) {
                return $http({
                    url: "/api/TableTemplateGrid/GetException",
                    method: "POST",
                    data: dto
                })
            },

        };

    }]);