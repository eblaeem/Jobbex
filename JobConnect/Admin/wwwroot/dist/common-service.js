System.register([], function (exports_1, context_1) {
    "use strict";
    var CommonService;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            CommonService = (function () {
                function CommonService() {
                }
                CommonService.prototype.getFormData = function (formId) {
                    var _this = this;
                    if (formId === void 0) { formId = '#form'; }
                    var array = {};
                    var serializeArray = $('#' + formId).serializeArray();
                    $.map(serializeArray, function (element) {
                        var tempName = element.name.replace(/\./g, '_');
                        var elementName = "#".concat(tempName);
                        var elementValue = element.value;
                        var booleanList = ['false', 'true', 'on', 'off'];
                        if (booleanList.find(function (c) { return c == elementValue; })) {
                            elementValue = _this.parseBoolean(elementValue);
                        }
                        else if (elementValue && $(elementName).is('input') && $(elementName).attr('type') != 'hidden') {
                            elementValue = elementValue.replace(new RegExp(',', 'g'), '');
                            elementValue = elementValue.replace(new RegExp('٬', 'g'), '');
                        }
                        var checkBox = $(elementName).is(':checkbox');
                        if (checkBox) {
                            elementValue = $(elementName).prop('checked');
                        }
                        if (array[element['name']] && !checkBox) {
                            var items = array[element['name']].toString().split(',');
                            var result_1 = elementValue;
                            $.map(items, function (n) {
                                result_1 = result_1 + ',' + n;
                            });
                            array[element['name']] = result_1;
                        }
                        else {
                            array[element['name']] = elementValue;
                        }
                    });
                    return array;
                };
                ;
                CommonService.prototype.parseBoolean = function (string) {
                    if (string) {
                        switch (string.toString().toLowerCase().trim()) {
                            case 'true':
                            case 'True':
                            case 'on':
                            case 'yes':
                            case '1':
                                return true;
                            case 'false':
                            case 'False':
                            case 'off':
                            case 'no':
                            case '0':
                                return false;
                            default: return null;
                        }
                    }
                    return null;
                };
                CommonService.prototype.generateGuid = function () {
                    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                        var random = Math.random() * 16 | 0;
                        var result = c === 'x' ? random : (random & 0x3 | 0x8);
                        return result.toString(16);
                    });
                };
                ;
                CommonService.prototype.genrated = function (length) {
                    var result = '';
                    var characters = 'abcdefghijklmnopqrstuvwxyz';
                    var charactersLength = characters.length;
                    for (var i = 0; i < length; i++) {
                        result += characters.charAt(Math.floor(Math.random() *
                            charactersLength));
                    }
                    return result;
                };
                ;
                CommonService.prototype.download = function (file, fileName) {
                    if (fileName === void 0) { fileName = ''; }
                    var anchor = document.createElement('a');
                    anchor.href = this.getBlobUrl(file);
                    anchor.download = fileName;
                    anchor.style.display = 'none';
                    document.body.appendChild(anchor);
                    anchor.click();
                };
                CommonService.prototype.getBlobUrl = function (response, contentType) {
                    if (contentType === void 0) { contentType = ''; }
                    var base64ToBlob = function (base64Data, contentType, sliceSize) {
                        if (contentType === void 0) { contentType = ''; }
                        if (sliceSize === void 0) { sliceSize = 512; }
                        var byteCharacters = atob(base64Data);
                        var byteArrays = [];
                        for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
                            var slice = byteCharacters.slice(offset, offset + sliceSize);
                            var byteNumbers = new Array(slice.length);
                            for (var i = 0; i < slice.length; i++) {
                                byteNumbers[i] = slice.charCodeAt(i);
                            }
                            var byteArray = new Uint8Array(byteNumbers);
                            byteArrays.push(byteArray);
                        }
                        return new Blob(byteArrays, { type: contentType });
                    };
                    var blob = base64ToBlob(response, contentType);
                    var blobUrl = URL.createObjectURL(blob);
                    return blobUrl;
                };
                ;
                CommonService.prototype.isString = function (x) {
                    return Object.prototype.toString.call(x) === "[object String]";
                };
                CommonService.prototype.toPascalCase = function (str) {
                    if (/^[a-z\d]+$/i.test(str)) {
                        return str.charAt(0).toUpperCase() + str.slice(1);
                    }
                    return str.replace(/([a-z\d])([a-z\d]*)/gi, function (g0, g1, g2) { return g1.toUpperCase() + g2.toLowerCase(); }).replace(/[^a-z\d]/gi, '');
                };
                return CommonService;
            }());
            exports_1("default", new CommonService());
        }
    };
});
//# sourceMappingURL=common-service.js.map