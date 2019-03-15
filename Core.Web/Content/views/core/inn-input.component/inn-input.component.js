angular.module("coreModule").component('innInput', {

    // объявление атрибутов биндинга, через которые компонент взаимодействует с внешми миром.
    bindings: {
        inn: '=',
        api: '='
      
    },

    controllerAs: 'vm',

    templateUrl: core.rootUrl + '/Content/views/core/inn-input.component/inn-input.component.html',

    controller: function () {

        let vm = this;

        this.$onInit = function () {
            this.api = {};
            this.api.isValid = isValid;
        };

        function isValid() {

            let result = /^(\d{10}|\d{12})$/.test(vm.inn);

            return result;
        };

      
        
    }
});