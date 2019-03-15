angular.module("coreModule", ["ui.router", "kendo.directives", "dialogs", "ngSanitize"])

//(#11593359) кастномный фильтр, который реплейсит запятые пробелами. Понадобился для разделения тыcяч пробелами.
//Т.к.ануляровский встроенный фильтр number не позволяет задать свой разделитель.
.filter('numberRu', function ($filter) {
    return function (value, fractionSize) {
        // применяем number - стандартный англурявоский фильтр форматирования чисел
        var numberValue = $filter('number')(value, fractionSize);

        //angular - locale_ru - ru.js запятые меняем на точки
        return numberValue ? numberValue.replace(new RegExp(',', 'g'), '.') : null;
    };
});