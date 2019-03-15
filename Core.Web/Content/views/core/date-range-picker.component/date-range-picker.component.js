angular.module("coreModule").component('dateRangePicker', {

    // объявление атрибутов биндинга, через которые компонент взаимодействует с внешми миром.
    bindings: {
        // даты периода
        startDate: '=',
        endDate: '=',

        // признак валидности. 
        isValid: '=?', // '=?' необязательный параметр для двунаправленного байндинга

        //передаем в компонент функцию , используется в k-ng-change
        onChange: '<'
    },

    controllerAs: 'vm',

    templateUrl: core.rootUrl + '/Content/views/core/date-range-picker.component/date-range-picker.component.html',
    controller: function() {

        var vm = this;

        // Настройка датапикера для выбора начального значения периода.
        vm.startPeriodOptions = {
            format: "dd MMMM yyyy",
            parseformats: ["yyyy.MM.dd"]
        };

        // Настройка датапикера для выбора конечного значения периода.
        vm.endPeriodOptions = {
            format: "dd MMMM yyyy",
            parseformats: ["yyyy.MM.dd"]
        };

        vm.periodIsIncorrect = function() {
            var hasErrors = vm.startDate && vm.endDate && vm.startDate > vm.endDate;
            // заносим валидность в атрибут, доступный для биндинга из любой разметки, где используется компонент
            vm.isValid = !hasErrors;
            return hasErrors;
        };
    }
});
