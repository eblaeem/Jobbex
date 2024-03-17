System.register([], function (exports_1, context_1) {
    "use strict";
    var MessageService;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            MessageService = (function () {
                function MessageService() {
                }
                MessageService.prototype.success = function (message) {
                    if (!message) {
                        message = 'عملیات با موفقیت انجام گردید';
                    }
                    new Notyf().success(message);
                };
                ;
                MessageService.prototype.error = function (message) {
                    if (!message) {
                        message = 'خطایی رخ داده است';
                    }
                    var result = [];
                    try {
                        var parse = JSON.parse(message);
                        parse.forEach(function (element) {
                            result.push(element.Label);
                        });
                    }
                    catch (e) {
                        if (Array.isArray(message)) {
                            result = message;
                        }
                        else {
                            try {
                                var toJosn = message.toJSON();
                                if (toJosn.status == 403) {
                                    message = 'به درخواست مورد نظر دسترسی ندارید';
                                }
                            }
                            catch (e) {
                            }
                            result.push(message);
                        }
                    }
                    if (result) {
                        var notyf = new Notyf();
                        for (var i = 0; i < result.length; i++) {
                            var title = result[i];
                            if (title) {
                                if (result[i].Label) {
                                    title = result[i].Label;
                                }
                            }
                            notyf.error({
                                message: title,
                                duration: 9000,
                                icon: false,
                                position: {
                                    x: 'left',
                                    y: 'bottom',
                                }
                            });
                        }
                    }
                };
                return MessageService;
            }());
            exports_1("default", new MessageService());
        }
    };
});
//# sourceMappingURL=message-service.js.map