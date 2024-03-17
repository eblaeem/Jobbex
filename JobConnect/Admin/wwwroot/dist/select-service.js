System.register([], function (exports_1, context_1) {
    "use strict";
    var SelectService;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            SelectService = (function () {
                function SelectService() {
                }
                SelectService.prototype.build = function (id, placeholder, ajaxUrl, multiple) {
                    if (!placeholder) {
                        var a = id.replace('#', '');
                        placeholder = $("label[for=\"".concat(a, "\"]")).html();
                        if (!placeholder) {
                            placeholder = $(id).attr('placeholder');
                        }
                    }
                    var options = {
                        dir: 'rtl',
                        width: '100%',
                        placeholder: placeholder,
                        language: {
                            errorLoading: function () {
                                return 'امکان بارگذاری نتایج وجود ندارد.';
                            },
                            inputTooLong: function () {
                                return 'عبارت مورد نظر شما خیلی طولانی میباشد';
                            },
                            inputTooShort: function (data) {
                                var counter = data.minimum;
                                counter = counter - data.input.length;
                                return "\u06A9\u0627\u0631\u0627\u06A9\u062A\u0631\u0647\u0627\u06CC \u0628\u06CC\u0634\u062A\u0631\u06CC \u0631\u0627 \u0648\u0627\u0631\u062F \u0646\u0645\u0627\u06CC\u06CC\u062F (".concat(counter, ")");
                            },
                            loadingMore: function () {
                                return 'در حال بارگذاری نتایج بیشتر...';
                            },
                            maximumSelected: function () {
                                return '';
                            },
                            noResults: function () {
                                return 'هیچ نتیجه ای یافت نشد';
                            },
                            searching: function () {
                                return 'در حال جستجو';
                            }
                        },
                        closeOnSelect: true,
                        allowClear: true,
                        ajax: {
                            url: '',
                            processResults: {}
                        },
                        minimumInputLength: 0,
                        id: id
                    };
                    if (multiple) {
                        var json = {
                            tags: true,
                            tokenSeparators: [',', ' '],
                            allowClear: false
                        };
                        options = $.extend({}, options, json);
                        $(id).attr('multiple', 'multiple');
                    }
                    if (ajaxUrl) {
                        options.ajax.url = ajaxUrl;
                        options.ajax.processResults = function (data) {
                            var results = [];
                            var mainData = data;
                            mainData.forEach(function (item) {
                                var newItem = {
                                    text: item.text,
                                    id: item.value
                                };
                                newItem = $.extend({}, item, newItem);
                                results.push(newItem);
                            });
                            return {
                                results: results
                            };
                        };
                        options.minimumInputLength = 2;
                    }
                    else {
                        options.ajax = null;
                        options.minimumInputLength = null;
                        $(id).select2(options)
                            .on('select2:selecting', function (event) {
                        });
                    }
                    if (multiple) {
                        if (ajaxUrl) {
                            var selectOptions = $(id).attr('selected-items');
                            var selectedValues = selectOptions.split(",");
                            if (selectedValues) {
                                selectedValues.forEach(function (item) {
                                    $(id).append("<option value='".concat(item, "' selected='selected'>").concat(item, "</option>"));
                                });
                                $(id).trigger('change');
                            }
                        }
                        else {
                            var selectOptions = $(id).find("option");
                            var selectedValues_1 = [];
                            if (selectOptions.length > 0) {
                                $.each(selectOptions, function (index, item) {
                                    var val = $(item).val();
                                    if (val) {
                                        selectedValues_1.push(val);
                                    }
                                });
                            }
                            $(id).val(selectedValues_1).trigger('change');
                        }
                    }
                    else {
                        var key = $(id).attr('selected-items');
                        var value = $(id).attr('selected-value');
                        if (ajaxUrl && key) {
                            $(id).append("<option value='".concat(key, "' selected='selected'>").concat(value, "</option>"));
                            $(id).trigger('change');
                        }
                    }
                };
                ;
                return SelectService;
            }());
            exports_1("default", new SelectService());
        }
    };
});
//# sourceMappingURL=select-service.js.map