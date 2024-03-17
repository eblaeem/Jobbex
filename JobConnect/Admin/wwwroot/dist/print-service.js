System.register(["./common-service.js"], function (exports_1, context_1) {
    "use strict";
    var common_service_js_1, PrintService;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (common_service_js_1_1) {
                common_service_js_1 = common_service_js_1_1;
            }
        ],
        execute: function () {
            PrintService = (function () {
                function PrintService() {
                }
                PrintService.prototype.print = function (fileData, fileType, onPrintDialogClose) {
                    if (onPrintDialogClose === void 0) { onPrintDialogClose = null; }
                    if (fileType && fileType.startsWith('image/')) {
                        common_service_js_1.default.download(fileData, 'download.png');
                    }
                    else {
                        printJS({
                            printable: fileData,
                            type: 'pdf',
                            base64: true,
                            onPrintDialogClose: function () {
                                if (onPrintDialogClose) {
                                    onPrintDialogClose();
                                }
                            },
                            onPdfOpen: function () {
                            }
                        });
                    }
                };
                return PrintService;
            }());
            exports_1("default", new PrintService());
        }
    };
});
//# sourceMappingURL=print-service.js.map