
declare var $;
class SelectService {
    build(id, placeholder?, ajaxUrl?, multiple?) {
        if (!placeholder) {
            const a = id.replace('#', '');
            placeholder = $(`label[for="${a}"]`).html();
            if (!placeholder) {
                placeholder = $(id).attr('placeholder');
            }
        }

        let options = {
            dir: 'rtl',
            width: '100%',
            placeholder: placeholder,
            language: {
                errorLoading: () => {
                    return 'امکان بارگذاری نتایج وجود ندارد.';
                },
                inputTooLong: () => {
                    return 'عبارت مورد نظر شما خیلی طولانی میباشد';
                },
                inputTooShort: (data) => {
                    let counter = data.minimum;
                    counter = counter - data.input.length;
                    return `کاراکترهای بیشتری را وارد نمایید (${counter})`;
                },
                loadingMore: () => {
                    return 'در حال بارگذاری نتایج بیشتر...';
                },
                maximumSelected: () => {
                    return '';
                },
                noResults: () => {
                    return 'هیچ نتیجه ای یافت نشد';
                },
                searching: () => {
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
            const json = {
                tags: true,
                tokenSeparators: [',', ' '],
                allowClear: false
            }
            options = $.extend({}, options, json);
            $(id).attr('multiple', 'multiple');
        }

        if (ajaxUrl) {
            options.ajax.url = ajaxUrl;
            options.ajax.processResults = (data) => {
                const results = [];
                const mainData = data;
                mainData.forEach((item) => {
                    let newItem = {
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


            ($(id) as any).select2(options)
                .on('select2:selecting', (event) => {
                    
                });
        }

        if (multiple) {
            if (ajaxUrl) {
                const selectOptions = $(id).attr('selected-items');
                const selectedValues = selectOptions.split(",");
                if (selectedValues) {
                    selectedValues.forEach(item => {
                        $(id).append(`<option value='${item}' selected='selected'>${item}</option>`);
                    });
                    $(id).trigger('change');
                }
            }
            else {
                const selectOptions = $(id).find("option");
                const selectedValues = [];
                if (selectOptions.length > 0) {
                    $.each(selectOptions, (index, item) => {
                        const val = $(item).val();
                        if (val) {
                            selectedValues.push(val)
                        }
                    });
                }
                ($(id) as any).val(selectedValues).trigger('change');
            }
        }
        else {
            const key = $(id).attr('selected-items');
            const value = $(id).attr('selected-value');
            if (ajaxUrl && key) {
                $(id).append(`<option value='${key}' selected='selected'>${value}</option>`);
                $(id).trigger('change');
            }
        }
    };
}
export default new SelectService();