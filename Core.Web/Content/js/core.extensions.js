kendo.culture("ru");

var core = {
    rootUrl: "",
    keyField: "id",
    uploadUrl: "api/Core/FileStorage/PostUpload", //"api/core/storage/uploadData"
    downloadUrl: "api/Core/FileStorage/GetDownload", //"api/core/storage/downloadData"
    cache: {},
    scrollSpeed: 150,
    ie: (function () {
        var v = 3,
            div = document.createElement('div'),
            all = div.getElementsByTagName('i');
        while (
            div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->',
            all[0]
        );

        return v > 4 ? v : null;
    }())
};

// Тотальная поддержка cross-domain и CORS
// При включении запрещает синхронный AJAX
//$.ajaxSetup({
//    crossDomain: true,
//    xhrFields: {
//        withCredentials: false
//    }
//});

//$(function () {
//    //включение бутстраповских tooltip + popover
//    $("body").on("mouseover", function () {
//        $("[data-toggle='tooltip']").tooltip();
//        $('[data-toggle="popover"]').popover();
//    });
//    //невозможность выбора дней при перелистовании календаря kendo 
//    //TODO: надо переделать в будущем, так как при клике на перелистывание календаря если не увести мышку, то событие не сработает
//    $("body").on("mouseover", function () {
//        $(".disabledDay").parent().removeClass("k-link");
//        $(".disabledDay").parent().removeAttr("href");
//    });
//});

core.uuid = function () {
    /// <summary>
    /// Уникальный идентификатор в формате UUID http://www.ietf.org/rfc/rfc4122.txt
    /// </summary>
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
};

core.clone = function (entity) {
    /// <summary>
    /// Клон объекта
    /// </summary>
    /// <param name="entity"> Объект данных </param>
    /// <returns type=""> Клон </returns>
    if (!entity)
        return null;
    return JSON.parse(JSON.stringify(entity));
};

core.parseURL = function (url) {
    /// <summary>
    /// Надежный способ разбора URL-а средствами браузера
    /// </summary>
    /// <param name="url"> Строка, содержащая URL </param>
    /// <returns type=""> Комплексный объект URI </returns>
    var a = document.createElement('a');
    a.href = url;
    return {
        source: url,
        protocol: a.protocol.replace(':', ''),
        host: a.host,
        hostname: a.hostname,
        port: a.port,
        query: a.search,
        params: (function () {
            var ret = {},
                seg = a.search.replace(/^\?/, '').split('&'),
                len = seg.length, i = 0, s;
            for (; i < len; i++) {
                if (!seg[i]) { continue; }
                s = seg[i].split('=');
                ret[s[0]] = s[1];
            }
            return ret;
        })(),
        file: (a.pathname.match(/\/?([^\/?#]+)$/i) || [, ''])[1],
        hash: a.hash.replace('#', ''),
        path: a.pathname.replace(/^([^\/])/, '/$1'),
        relative: (a.href.match(/tps?:\/\/[^\/]+(.+)/) || [, ''])[1],
        segments: a.pathname.replace(/^\//, '').split('/')
    };
}

core.checkArrays = function (arrA, arrB) {
    /// <summary>
    /// Сравниваю массивы по приципу:
    /// var a = [1, 2, 3, 4, 5];
    /// var b = [5, 4, 3, 2, 1];
    /// var c = [1, 2, 3, 4];
    /// var d = [1, 2, 3, 4, 6];
    /// var e = ["1", "2", "3", "4", "5"];
    /// core.checkArrays(a, b) === true
    /// core.checkArrays(a, c) === false
    /// core.checkArrays(a, d) === false
    /// core.checkArrays(a, e) === true
    /// </summary>
    /// <param name="arrA"> Первый массив </param>
    /// <param name="arrB"> Второй массив </param>
    /// <returns type="Boolean"> Равны? </returns>

    // Сравниваю ссылки
    if (!arrA || !arrB)
        return false;

    // Сравниваю длину и, собственно, наличие длины :)
    try {
        if (arrA.length !== arrB.length)
            return false;
    } catch (e) {
        return false;
    }

    // Сортирую
    var cA = arrA.slice().sort();
    var cB = arrB.slice().sort();

    // Сравниваю до первого несовпадения
    for (var i = 0; i < cA.length; i++)
        if (cA[i] != cB[i]) return false;

    return true;
}

// Добавление записи в kendo-DataSource
core.dsCreate = function (url, options) {
    $.ajax({
        type: "POST",
        url: url,
        cache: false,
        contentType: "application/json",
        data: JSON.stringify(options.data),
        success: function (result) {
            // HttpResponseMessage -> OData
            var data = {
                d: {
                    results: [result],
                    __count: 1
                }
            };
            options.success(data);
        },
        error: function (result) {
            options.error(result);
        }
    });
};

// Модификация записи в kendo-DataSource
core.dsUpdate = function (url, options) {
    $.ajax({
        type: "POST",
        url: url,
        cache: false,
        contentType: "application/json",
        data: JSON.stringify(options.data),
        success: function (result) {
            options.success(result);
        },
        error: function (result) {
            options.error(result);
        }
    });
};

// Удаление записи в kendo-DataSource
core.dsDelete = function (url, options) {
    $.ajax({
        type: "POST",
        url: url,
        cache: false,
        contentType: "application/json",
        data: JSON.stringify({ id: options.data.id }),
        success: function (result) {
            options.success(result);
        },
        error: function (result) {
            options.error(result);
        }
    });
};

// Загрузка контента с указанного url-а в существующий div, биндинг, отображение в дилоговом окне
core.dialog = function (url, divContainer, scope, http, templateCache, compile, shown, hidden) {
    http.get(url, { cache: templateCache }).success(function (response) {

        // Превращаю, полученный из url-а, шаблон в набор элементов
        var contents = $("<div/>").html(response).contents();

        // Передаю набор элементов в контейнер
        divContainer.html(contents);

        // Нахожу корневой div - это будущий диалог
        var divWindow = divContainer.children().first();

        // Привязываю диалогу текущий скоп
        compile(divWindow)(scope);

        // Привязываю обработчики
        divWindow.on("shown.bs.modal", shown);
        divWindow.on("hidden.bs.modal", hidden);

        // Показываю диалог
        divWindow.modal("show");
    });

};

core.validator = function (scope) {
    /// <summary>
    /// Валидатор
    /// </summary>
    /// <param name="scope"> Скоп ангулара </param>
    /// <returns> Экземпляр валидатора </returns>
    var object = { count: 0, rules: {}, errors: {} };
    object.register = function (path, prediacate, message) {
        // Добавляю валидатор
        var i = this.count++;
        this.rules[i] = {
            path: path,
            prediacate: function (v) {
                // В случае ошибки в предикате: правило невалидно, соответственно учитывать его - нельзя.
                try {
                    return prediacate(v);
                } catch (e) {
                }
                return true;
            },
            message: message
        };
    };


    object.unregister = function (path) {
        for (var i = 0; i < this.count; i++) {
            var rule = this.rules[i];
            if (rule && rule.path == path) {
                delete this.rules[i];
                break;
            }
        }
    };

    object.notEmpty = function (path, message) {
        this.register(path, function (value) {
            return !!value;
        }, message);
    };

    object.regExp = function (path, regExp, message) {
        this.register(path, function (value) {
            return regExp.test(value);
        }, message);
    };

    object.email = function (path, message) {
        this.regExp(path, /^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/, message);
    };

    object.maxLength = function (path, maxLength, message) {
        this.register(path, function (value) {
            return !value || value.length <= maxLength;
        }, message);
    }

    object.getAllErrors = function (separator) {
        // Разделитель
        if (!separator)
            separator = "\r\n\r\n";
        // Собираю результат
        var result = "";
        for (var k in this.errors) {
            if (this.errors.hasOwnProperty(k)) {
                var msg = this.errors[k];
                if (msg) {
                    result += msg + separator;
                }
            }
        }
        if (result) // Убираю последний разделитель
            result = result.substr(0, result.length - separator.length);
        // Готово
        return result;
    };
    object.isInvalid = function (path) {
        // Перебираю зарегистрированные правила
        for (var i = 0; i < this.count; i++) {
            var rule = this.rules[i];
            if (rule && rule.path == path) {
                var rule = this.rules[i];
                // Получаю значение
                var value = scope.$eval(path);
                // Исполняю правило
                if (!rule.prediacate(value)) { // Не валидно
                    // Определяюсь с сообщением
                    var msg = rule.message;
                    if (msg.hasOwnProperty("prototype")) // Функция
                        msg = msg(path, value);
                    // Может получился массив?
                    if (Object.prototype.toString.call(msg) === '[object Array]')
                        msg = msg[0]; // Экранное сообщение должно быть первым
                    // Заполняю список ошибок
                    this.errors[path] = msg;
                    // Cообщаю ошибку
                    return msg;
                } else { // Валидно
                    // Вычищаю из списка ошибок 
                    this.errors[path] = undefined;
                    // Иду дальше
                }
            }
        }
        // Перебрал все правила - нет ошибок
        return null;
    };
    object.validate = function (msgIndex) { // Формирую информацию об ошибках
        // Очищаю информацию об ошибках
        this.errors = {};
        // Перебираю зарегистрированные правила
        for (var i = 0; i < this.count; i++) {
            // Получаю правило
            var rule = this.rules[i];
            if (rule == undefined)
                continue;
            // Путь
            var path = rule.path;
            // Проверяю, что по данному пути ещё нет ошибок
            if (!this.errors[path]) {
                // Получаю значение
                var value = scope.$eval(path);
                // Исполняю правило
                if (!rule.prediacate(value)) { // Не валидно
                    // Определяюсь с сообщением
                    var msg = rule.message;
                    if (msg.hasOwnProperty("prototype")) // Функция
                        msg = msg(path, value);
                    // Может получился массив?
                    if (Object.prototype.toString.call(msg) === '[object Array]')
                        msg = msg[msgIndex ? msgIndex : 0]; // Если не передан индекс - показываю экранное сообщение
                    // Заполняю список ошибок
                    this.errors[path] = msg;
                } else { // Валидно
                    // Вычищаю из списка ошибок 
                    this.errors[path] = undefined;
                    // Иду дальше
                }
            }
        }
    };
    object.reset = function () {
        // Очищаю информацию об ошибках
        this.errors = {};
    };
    return object;
};

// Смена значения path-параметров без перезагрузки страницы 
core.quietUpdatePath = function (scope, location, route, values) {

    // Оригинальный path + '/'
    var path = route.current.originalPath + "/";

    // Заполняю переданными параметрами
    for (var key in values)
        if (values.hasOwnProperty(key)) {
            path = path.replace(new RegExp("/:" + key + "/", 'g'), "/" + values[key] + "/");
        }

    // Заполненный path - '/'
    path = path.slice(0, -1);

    // Может всё так и было?
    if (location.path() == path)
        return; // Все совпадает, нечего делать 

    // Текущий маршрут
    var lastRoute = route.current;

    // Обработчик переходов маршрутов
    var handler = function (event) {
        //#region Реализация обработчика
        if (route.current.$$route.templateUrl === lastRoute.$$route.templateUrl) { // Тот-же маршрут? 

            // Он самый - ничего не гружу, оставляю как было
            route.current = lastRoute;

            // Восстанавливаю состояние обработчиков
            var array = event.currentScope.$$listeners.$locationChangeSuccess;
            var i = array.indexOf(handler);
            if (i >= 0)
                array.splice(i, 1);
        }
        ;
        //#endregion
    };

    // Добавляю отработчик маршрутов
    scope.$on('$locationChangeSuccess', handler);

    // Осуществляю смену маршрута
    location.path(path);
};

// Смена значения search без перезагрузки страницы 
core.quietUpdateSearch = function (scope, location, route, pName, pValue) {
    // Сериализую значение без null-ов
    var newPValue = JSON.stringify(pValue, function (key, value) {
        return !value ? undefined : value;
    });

    // Если значение отсутствует или отличается
    if (location.$$search[pName] != newPValue) {

        // Текущий маршрут
        var lastRoute = route.current;

        // Обработчик переходов маршрутов
        var handler = function (event) {
            //#region Реализация обработчика
            if (route.current.$$route.templateUrl === lastRoute.$$route.templateUrl) { // Тот-же маршрут? 

                // Он самый - ничего не гружу, оставляю как было
                route.current = lastRoute;

                // Восстанавливаю состояние обработчиков
                var array = event.currentScope.$$listeners.$locationChangeSuccess;
                var i = array.indexOf(handler);
                if (i >= 0)
                    array.splice(i, 1);
            }
            ;
            //#endregion
        };

        // Добавляю отработчик маршрутов
        scope.$on('$locationChangeSuccess', handler);

        // Осуществляю смену фильтрав
        scope.$apply(function () {
            if (newPValue == "{}")
                location.search(pName, null);
            else
                location.search(pName, newPValue);
        });
    }
};

core.dotToСomma = function (t, e) {
    /// <summary>
    /// Замена "." на "," в input-котролах на событии onkeypress. Не допускается множетсвенный ввод ",".
    /// Пример: <input onkeypress="core.dotToСomma(this, event)"/>
    /// </summary>
    /// <param name="t"> input </param>
    /// <param name="e"> event </param>
    /// <returns type=""> true/false </returns>

    // 46 это код "."-ки
    if (e.keyCode == 46 || e.charCode == 46) {
        // Допускается только одна ","-я
        if (!!~$(t).val().indexOf(","))
            return false;

        // IE
        if (document.selection) {
            // Определяю выделенный диапазон
            var range = document.selection.createRange();

            // Заменяю на ","-ю
            range.text = ",";

            // Chrome + FF
        } else if (t.selectionStart || t.selectionStart == 0) {

            // Определяю начало и конец выделения
            var start = t.selectionStart;
            var end = t.selectionEnd;

            // Заменяю выделленное на ","-ю
            $(t).val($(t).val().substring(0, start) + "," + $(t).val().substring(end, $(t).val().length));

            // Возвращаю курсор в исходную позицию
            t.selectionStart = start + 1;
            t.selectionEnd = start + 1;

        } else {
            // Нет выделенного диапазона, просто добавялю ","-ю
            $(t).val($(t).val() + ",");
        }
        return false;
    }
    return true;
};

core.digitsOnly = function (t, e) {
    /// <summary>
    /// Позволяет вводить только цифры в input-котролах на событии onkeydown.
    /// Пример: <input onkeydown="return core.digitsOnly(this, event)"/>
    /// </summary>
    /// <param name="t"> input </param>
    /// <param name="e"> event </param>
    /// <returns type=""> true/false </returns>

    // Получаю код
    var charCode = e.charCode ? e.charCode : e.keyCode;
    // Проверяю разрешенный диапазон: от 48 до 57
    return 48 <= charCode && charCode <= 57;
};

core.kendoDateOnly = function (t, e, f) {
    /// <summary>
    /// Позволяет вводить только символы приводящие к образования корректной даты в input-котролах на событии onkeypress.
    /// Пример: <input onkeypress="return core.kendoDateOnly(this, event, 'dd.MM.yyyy')" />
    /// </summary>
    /// <param name="t"> input </param>
    /// <param name="e"> event </param>
    /// <param name="f"> Маска (по умочанию - короткая дата в формате kendo)</param>
    /// <returns type=""> true/false </returns>

    // Цикл - корректор високосных лет
    var yearInc = 0;
    while (yearInc < 3) {
        // Типовая дата
        var emptyDate = new Date(2000 + yearInc, 0, 1);

        // Строчное представление типовой даты, отформатированное kendo
        var emptyDateStr = kendo.toString(emptyDate, f || "d");

        // Ожидаемое значение контрола
        var value = t.value.substr(0, t.selectionStart)
            + ((e.char == undefined) ? e.key : e.char)
            + t.value.substr(t.selectionEnd);

        // Базовая проверка
        if (value.length > emptyDateStr.length)
            return false;

        // Заменяю посимвольно значение в dateStr из value
        var dateStr = value + emptyDateStr.substr(value.length);

        // Пытаюсь распарсить дату
        var date = kendo.parseDate(dateStr, f || "d");
        if (!date) {
            if (dateStr.length == emptyDateStr.length) { // Если похоже на правильную дату
                // Пробую подвинуть год на 2 и пройти ещё раз ибо один из годов xxx0 или xxx2, по любому - високосный
                yearInc += 2;
                continue;
            } else
                return false;
        }
        return true;
    }
    return false;
};

core.char2char = function (t, e, charIn, charOut) {
    /// <summary>
    /// Замена символа charIn на символ charOut в input-котролах на событии onkeypress.
    /// Пример: <input onkeypress="return core.char2char(this, event, ',', '.')" />
    /// </summary>
    /// <param name="t"> input </param>
    /// <param name="e"> event </param>
    /// <param name="charIn"></param>
    /// <param name="charOut"></param>
    /// <returns type=""> true/false </returns>

    // Код charIn
    var charInCode = charIn.charCodeAt(0);
    if (e.keyCode == charInCode || e.charCode == charInCode) {

        // IE
        if (document.selection) {
            // Определяю выделенный диапазон
            var range = document.selection.createRange();

            // Заменяю на charOut
            range.text = charOut;

            // Chrome + FF
        } else if (t.selectionStart || t.selectionStart == 0) {

            // Определяю начало и конец выделения
            var start = t.selectionStart;
            var end = t.selectionEnd;

            // Заменяю выделленное на charOut
            $(t).val($(t).val().substring(0, start) + charOut + $(t).val().substring(end, $(t).val().length));

            // Возвращаю курсор в исходную позицию
            t.selectionStart = start + 1;
            t.selectionEnd = start + 1;

        } else {
            // Нет выделенного диапазона, просто добавялю charOut
            $(t).val($(t).val() + charOut);
        }
        return false;
    }
    return true;
};

//obsolete Заменяю мнимый-уникальный идентификатор на реально-уникальный и получаю текущий скоп
core.dialogScopeHandling = function (uuid) {
    var id = core.uuid();
    $("#" + uuid).attr("id", id);
    var scopeDiv = $("#" + id)[0];
    var scope = angular.element(scopeDiv).scope();
    scope.dialogUuid = id;
    return scope;
};

core.intializeApplicationBase = function (that, $rootScope, $window, $http, $state, $urlRouter, $q) {
    /// <summary>
    /// Базовая инициализация сервиса application,
    /// пример вызова:
    /// MyCustomModule.service("application", function ($rootScope, $window, $http, $state, $urlRouter) {
    ///     core.intializeApplicationBase(this, $rootScope, $window, $http, $state, $urlRouter);
    /// });
    /// </summary>
    /// <param name="that"> Ссылка на this сервиса application </param>
    /// <param name="$rootScope"> Сервис $rootScope - аргумент application </param>
    /// <param name="$window"> Сервис $window - аргумент application </param>
    /// <param name="$http"> Сервис $http - аргумент application </param>
    /// <param name="$state"> Сервис $state - аргумент application </param>
    /// <param name="$urlRouter"> Сервис $urlRouter - аргумент application </param>

    // Делаю ссылку на kendo доступной всем
    $rootScope.kendo = kendo;

    // Функция получения названия текущего статуса в любом скопе
    $rootScope.getState = function () {
        return $state.current.name;
    };

    // Функция получения строки с параметрами текущего статуса в любом скопе
    $rootScope.getStateParams = function () {
        var paramStr = $.param($state.params);
        return paramStr ? "?" + paramStr : "";
    };

    // Взаимодействие со стандартным entity-сервисом
    that.getEntity = function (url, entityId, success, error, init) {

        // Обслуживание методов Get и Ctreate
        var onGetOrCreate = function (entity) {

            // Добавляю в сущность метод $reload
            entity.$reload = reload;

            // Вызов общей, инициализирующей фукции, заданной при объявлении сущности
            if (init)
                init(entity);

            // Вызов частной фукции, обслуживающей успешную загрузку
            if (success)
                success(entity);
        }

        // Обслуживание метода Save
        var onSaveResponse = function (data) {

            var entity = data.resource;

            //обновляем идентификатор сущности (так как при создании сущности и первом сохранении он изменится)
            entityId = entity.id;

            // Добавляю в сущность метод $reload
            entity.$reload = reload;

            // Вызов общей, инициализирующей фукции, заданной при объявлении сущности
            if (init)
                init(entity);
        }

        // Функция перезагрузки
        var reload = function () {

            // Реристрирую сервис и методы сущности aka Rich Object
            var entityResource = createResource();

            // Получаю или создаю ?
            if (entityId) {
                // Получаю сущность
                return entityResource.get({ id: entityId }, onGetOrCreate, error);
            } else {
                // Создаю сущность
                return entityResource.create(null, onGetOrCreate, error);
            }
        };

        // Загрузка с инициацией внешних callback-ов
        return reload();
    };

    //#region Контроль перехода состояний

    //функция которая будет вызываться при смене состояния
    //для работы в конкретном контроллере надо написать след.код:
    //application.onStateChangeStart(function (doChangeState) {
    //      //свой код, который что-либо проверяет  
    //      doChangeState(); <-- необходимо вызвать эту функцию для осуществления перехода
    //});
    that.onStateChangeStart = function (userDelegate) {
        $rootScope.onStateChangeStart_UserDelegate = userDelegate;
    };

    //флаг для блокировки лишнего срабатывания $stateChangeStart
    $rootScope.onStateChangeStart_WorkingFlag = false;
    //системная функция, которая непосредственно меняет состояние, при этом очищая ресурсы
    $rootScope.onStateChangeStart_doChangeState = function () {
        try {
            $rootScope.onStateChangeStart_WorkingFlag = true;
            $state.go($rootScope.onStateChangeStart_toStateName, $rootScope.onStateChangeStart_toParams);
        } finally {
            $rootScope.onStateChangeStart_UserDelegate = null;
            $rootScope.onStateChangeStart_WorkingFlag = false;
        }
    };

    //подписка на событие смены состояния
    $rootScope.$on("$stateChangeStart", function (evt, toState, toParams) {
        //проверяем параметры + также ничего не делаем если установлен флаг
        if ($state == null || $state.current == null || $rootScope.onStateChangeStart_WorkingFlag)
            return;

        //если в конкретном контроллере установлен метод, который надо вызывать при смене состояния
        if ($rootScope.onStateChangeStart_UserDelegate) {
            evt.preventDefault(); //предотвращаем смену состояния

            //запоминаем параметры, куда хотелось перейти
            $rootScope.onStateChangeStart_toStateName = toState.name;
            $rootScope.onStateChangeStart_toParams = toParams;

            //вызываем пользовательскую реализацию
            $rootScope.onStateChangeStart_UserDelegate($rootScope.onStateChangeStart_doChangeState);
        }
    });
    //#endregion

    core.dsRead = function (url, options, filter) {
        var deferred = $q.defer();

        options.data.filterDto = filter;

        $http({
            url: url,
            method: "Post",
            data: options.data
        }).then(
            function success (result) {
                // подготавливаю данные, если надо
                if (options.prepareData)
                    options.prepareData(result.Items);
                // PageResult<> -> OData
                var dataResult = result.data;
                var data = {
                    d: {
                        results: dataResult.Data ? dataResult.Data : [],
                        __count: !!options.noCount ? (dataResult.Data ? dataResult.Data.length : 0) : dataResult.Total
                    }
                };

                // Сообщаю об успехе
                options.success(data);
                deferred.resolve(result);
            },
            function error (result) {
                // Сообщаю о неудаче
                options.error(result);
                deferred.reject(result);
            }
        );

        return deferred.promise;
    };

};

core.wordByNumber = function (number, wordArray) {
    /// <summary>
    /// Функция выбора слова по предоставляемому числу,
    /// пример массива: ['голос', 'голоса', 'голосов']
    /// результат для числа 123: 'голоса'
    /// </summary>
    /// <param name="number"> Предоставляемое число </param>
    /// <param name="wordArray"> Массив из 3-х слов </param>
    /// <returns type=""> </returns>

    number %= 100;
    if (number > 19) {
        number %= 10;
    }

    switch (number) {
        case 1:
            return wordArray[0];

        case 2:
        case 3:
        case 4:
            return wordArray[1];

        default:
            return wordArray[2];
    }
};

core.escapeText = function (value) {
    /// <summary>
    /// Функция преобразует обычный текст в текст, пригодный для вставки в HTML.
    /// Все служебные HTML-символы преобразуются в безопасные для HTML коды,
    /// пробелы и табуляция заменяются &nbsp;, перенос строк делается при помощи br/
    /// </summary>
    /// <param name="value"> входной текст </param>
    /// <returns type=""> </returns>

    if (value) {
        var escape = document.createElement('textarea');
        escape.innerHTML = value;
        value = escape.innerHTML;
        // Разбиение строки на массив сторк, обработка каждой строки и склейка строк при помощи <br/>
        // К одиночным строкам <br/> не добавляется
        value = value.split(/\r\n|\r|\n/g).map(
            function (str) {
                // Замена начальных и конечных табуляций на &nbsp;. Если менять во всей строке, то 'ломаются' диалоги Kendo (пропадает перенос строк)
                str = str.replace(/^\t+|\t+$/g, '&nbsp;&nbsp;&nbsp;');
                // Замена начальных и конечных пробелов на &nbsp;. Если менять во всей строке, то 'ломаются' диалоги Kendo (пропадает перенос строк)
                str = str.replace(/^\s+|\s+$/g, '&nbsp;');
                return str;
            }).join("<br/>");
    }
    return value;
}

core.violationToView = function (violations) {
    var result = "";
    for (var violation in violations) {
        result += "<div>" + core.escapeText(violations[violation].message) + "</div>";
    }
    return result;
};

core.updateResultFromCache = function (response, options) {
    if (!(response && response.response && response.response.d && response.response.d.results && options))
        return;
    // список ДТО полученный с сервера
    var results = response.response.d.results;
    for (var optionIndex = 0; optionIndex < options.length; optionIndex++) {
        // получение настроек кэширования
        // cache - имя кэша включая регион
        // key - имя колонки в результате полученном с сервера, в которой храниться ид
        // value - имя колонки в кэше, значение из которой будет помещенно результат полученный с сервера
        var option = options[optionIndex];
        // получение списка ДТО из кэша
        var cache = eval("core.cache." + option.cache);
        // кэш не найден
        if (!cache)
            continue;
        for (var resultIndex = 0; resultIndex < results.length; resultIndex++) {

            var result = results[resultIndex];
            // получение ид
            var resultKey = result[option.key];
            // ид не установлен окончание обработки записи
            if (!resultKey)
                continue;
            // поиск записи в кэше по ид
            for (var cacheIndex = 0; cacheIndex < cache.length; cacheIndex++) {
                // ид записи кэша равен ид в колонке резултата
                if (cache[cacheIndex]["id"] == resultKey) {
                    // замена данных в результате
                    result[option.key] = cache[cacheIndex][option.value];
                    break;
                }
            }
        }
    }

}

core.scrollTo = function (elementId, cfg) {
    /// <summary>
    /// Плавная прокрутка страницы до элемента с указанным id
    /// оригинал: http://www.itnewb.com/tutorial/Creating-the-Smooth-Scroll-Effect-with-JavaScript
    /// </summary>
    /// <param name="elementId"> id элемента разметки </param>
    /// <param name="cfg"> конфигурация </param>

    //#region Инициализация конфигурации

    cfg = cfg || {};

    // Дистанция, на которой осуществляется прыжок вместо прокрутки
    cfg.jumpDistance = cfg.jumpDistance || 100;

    // Имя функции - глобального хендлера
    cfg.globalHandlerName = cfg.globalHandlerName || "(function(){})";

    // Максимальная скорость
    cfg.maxSpeed = cfg.maxSpeed || 20;

    //#endregion

    var startY = currentYPosition();
    var stopY = elmYPosition(elementId);
    var distance = stopY > startY ? stopY - startY : startY - stopY;
    if (distance < cfg.jumpDistance) {
        $(window).scrollTop(stopY);
        eval(cfg.globalHandlerName + "();");
        return;
    }
    var speed = Math.round(distance / 100);
    if (speed >= cfg.maxSpeed) speed = cfg.maxSpeed;
    var step = Math.round(distance / core.scrollSpeed);
    var leapY = stopY > startY ? startY + step : startY - step;
    var timer = 0;
    var i;
    if (stopY > startY) {
        for (i = startY; i < stopY; i += step) {
            setTimeout("$(window).scrollTop(" + leapY + "); " + cfg.globalHandlerName + "();", timer * speed);
            leapY += step; if (leapY > stopY) leapY = stopY; timer++;
        } return;
    }
    for (i = startY; i > stopY; i -= step) {
        setTimeout("$(window).scrollTop(" + leapY + "); " + cfg.globalHandlerName + "();", timer * speed);
        leapY -= step; if (leapY < stopY) leapY = stopY; timer++;
    }

    function currentYPosition() {
        // Firefox, Chrome, Opera, Safari
        if (self.pageYOffset) return self.pageYOffset;
        // Internet Explorer 6 - standards mode
        if (document.documentElement && document.documentElement.scrollTop)
            return document.documentElement.scrollTop;
        // Internet Explorer 6, 7 and 8
        if (document.body.scrollTop) return document.body.scrollTop;
        return 0;
    }

    function elmYPosition(eId) {
        var elm = document.getElementById(eId);
        var y = elm.offsetTop;
        var node = elm;
        while (node.offsetParent && node.offsetParent != document.body) {
            node = node.offsetParent;
            y += node.offsetTop;
        } return y;
    }

};

core.errorMessage = function (error, dialogs) {
    /// <summary>
    /// Выводит сообщение об ошибке клиенту
    /// </summary>
    /// <param name="error" type="type"></param>
    var message = "";
    if (error.status == 409 && error.data && error.data.Violations) {
        var violations = error.data.Violations;

        for (var i = 0; i < violations.length; i++)
            message += "<p>" + violations[i].Message + "</p>";
    }
    else if (!!error.data.Message)
        message += "<p>" + error.data.Message + "</p>";
    else
        message += "<p> Произошла необработанная ошибка. Обратитесь к разработчику </p>";

    dialogs.error({ msg: message });
        
}

//#region Polyfills

$.postJSON = function (url, data, success, error) {
    /// <summary>
    /// Отсутствующй в jQuery аналог getJSON для метода POST
    /// </summary>
    return $.ajax({
        type: "POST",
        url: url,
        cache: false,
        contentType: "application/json",
        data: JSON.stringify(data),
        success: success,
        error: error
    });
};

if (!Array.isArray) {
    Array.isArray = function (arg) {
        /// <summary>
        /// Determine whether the argument is an array.
        /// </summary>
        /// <param name="arg">Object to test whether or not it is an array.</param>
        /// <returns type="boolean" />
        return Object.prototype.toString.call(arg) === '[object Array]';
    };
}

//#endregion
//#region Date extensions

Date.prototype.addDays = function (days) {
    this.setDate(this.getDate() + days);
    return this;
};

Date.prototype.toExport = function () {
    /// <summary>
    /// //Форматирует дату для передачи на сервер. Использовать если на сервер не передается таймзона
    /// </summary>
    /// <returns type="string"></returns>
    var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
    return (new Date(this - tzoffset)).toISOString().slice(0, -1);
}

//#endregion