System.register(["./fetch-service.js", "./common-service.js", "./message-service.js"], function (exports_1, context_1) {
    "use strict";
    var fetch_service_js_1, common_service_js_1, message_service_js_1, ModalService;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (fetch_service_js_1_1) {
                fetch_service_js_1 = fetch_service_js_1_1;
            },
            function (common_service_js_1_1) {
                common_service_js_1 = common_service_js_1_1;
            },
            function (message_service_js_1_1) {
                message_service_js_1 = message_service_js_1_1;
            }
        ],
        execute: function () {
            ModalService = (function () {
                function ModalService() {
                }
                ModalService.prototype.show = function (url, jsName, data, shownCallBack) {
                    var modalId = common_service_js_1.default.genrated(6);
                    if (!data) {
                        data = {};
                    }
                    data.modalId = modalId;
                    fetch_service_js_1.default.get(url, data).then(function (response) {
                        if (response == null) {
                            return message_service_js_1.default.error('ردیفی برای نمایش وجود ندارد');
                        }
                        $("body").append(response);
                        $("#".concat(modalId)).modal('show');
                        $("#".concat(modalId)).on('shown.bs.modal', function () {
                            if (jsName) {
                                System.import(jsName).then(function (result) {
                                    var moduleName = common_service_js_1.default.toPascalCase(jsName);
                                    new result[moduleName]("#".concat(modalId));
                                });
                            }
                            if (shownCallBack) {
                                shownCallBack(response);
                            }
                        });
                        $("#".concat(modalId)).on('hide.bs.modal', function () {
                            $("#".concat(modalId)).remove();
                        });
                    });
                };
                ;
                ModalService.prototype.hide = function (event) {
                    var target = $(event.target);
                    $(target).closest('.modal').modal('hide');
                };
                return ModalService;
            }());
            exports_1("default", new ModalService());
        }
    };
});
//# sourceMappingURL=modal-service.js.map