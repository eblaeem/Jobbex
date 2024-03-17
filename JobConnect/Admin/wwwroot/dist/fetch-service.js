System.register(["./common-service.js", "./message-service.js"], function (exports_1, context_1) {
    "use strict";
    var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
        function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
        return new (P || (P = Promise))(function (resolve, reject) {
            function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
            function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
            function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
            step((generator = generator.apply(thisArg, _arguments || [])).next());
        });
    };
    var __generator = (this && this.__generator) || function (thisArg, body) {
        var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
        return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
        function verb(n) { return function (v) { return step([n, v]); }; }
        function step(op) {
            if (f) throw new TypeError("Generator is already executing.");
            while (g && (g = 0, op[0] && (_ = 0)), _) try {
                if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
                if (y = 0, t) op = [op[0] & 2, t.value];
                switch (op[0]) {
                    case 0: case 1: t = op; break;
                    case 4: _.label++; return { value: op[1], done: false };
                    case 5: _.label++; y = op[1]; op = [0]; continue;
                    case 7: op = _.ops.pop(); _.trys.pop(); continue;
                    default:
                        if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                        if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                        if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                        if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                        if (t[2]) _.ops.pop();
                        _.trys.pop(); continue;
                }
                op = body.call(thisArg, _);
            } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
            if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
        }
    };
    var common_service_js_1, message_service_js_1, FetchService;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (common_service_js_1_1) {
                common_service_js_1 = common_service_js_1_1;
            },
            function (message_service_js_1_1) {
                message_service_js_1 = message_service_js_1_1;
            }
        ],
        execute: function () {
            FetchService = (function () {
                function FetchService() {
                }
                FetchService.prototype.post = function (url, data) {
                    var _this = this;
                    this.blockUi();
                    var headers = {
                        "Content-Type": "application/json",
                    };
                    var fomrmData = data;
                    if (data instanceof FormData) {
                        fomrmData = data;
                        delete headers["Content-Type"];
                    }
                    else {
                        var parametrs_1 = {};
                        if (data) {
                            var keys = Object.keys(data);
                            if (keys) {
                                keys.forEach(function (element) {
                                    var value = data[element];
                                    if (value != null && value != 'null' && (value != '' || value === false)) {
                                        if (common_service_js_1.default.isString(value)) {
                                            value = value.trim();
                                        }
                                        parametrs_1[element] = value;
                                    }
                                });
                            }
                        }
                        fomrmData = JSON.stringify(parametrs_1);
                    }
                    var response = fetch(url, {
                        method: "POST",
                        headers: headers,
                        body: fomrmData
                    });
                    return response.then(function (res) {
                        return res.json();
                    }).then(function (response) {
                        _this.unBlockUi();
                        return response;
                    }).catch(function (error) {
                        _this.unBlockUi();
                        var message = error === null || error === void 0 ? void 0 : error.message;
                        message_service_js_1.default.error(message);
                        return error;
                    });
                };
                FetchService.prototype.get = function (url, data, blockUi) {
                    if (blockUi === void 0) { blockUi = true; }
                    return __awaiter(this, void 0, void 0, function () {
                        var keys, response;
                        var _this = this;
                        return __generator(this, function (_a) {
                            if (blockUi) {
                                this.blockUi();
                            }
                            if (data) {
                                keys = Object.keys(data);
                                if (keys) {
                                    url += '?';
                                    keys.forEach(function (element) {
                                        var value = data[element];
                                        if (value != null && value != '') {
                                            url += "".concat(element, "=").concat(value, "&");
                                        }
                                    });
                                }
                            }
                            response = fetch(url, {
                                method: "GET",
                                headers: {
                                    "X-Requested-With": "XMLHttpRequest"
                                }
                            });
                            return [2, response.then(function (response) {
                                    _this.unBlockUi();
                                    return response.text();
                                }).catch(function (error) {
                                    _this.unBlockUi();
                                    var message = error === null || error === void 0 ? void 0 : error.message;
                                    message_service_js_1.default.error(message);
                                    return error;
                                })];
                        });
                    });
                };
                FetchService.prototype.blockUi = function (message) {
                    if (message === void 0) { message = ''; }
                    if (!message) {
                        message = 'لطفا منتظر بمانید ...';
                    }
                    $.blockUI({
                        css: {
                            border: 'none',
                            padding: '15px',
                            backgroundColor: '#000',
                            '-webkit-border-radius': '10px',
                            '-moz-border-radius': '10px',
                            opacity: .5,
                            color: '#fff'
                        },
                        message: message
                    });
                };
                FetchService.prototype.unBlockUi = function () {
                    $.unblockUI();
                };
                return FetchService;
            }());
            exports_1("default", new FetchService());
        }
    };
});
//# sourceMappingURL=fetch-service.js.map