System.register([], function (exports_1, context_1) {
    "use strict";
    var EditorService;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            EditorService = (function () {
                function EditorService() {
                }
                EditorService.prototype.set = function (elementId, height) {
                    var options = {
                        tabsize: 2,
                        height: height != null ? height : 150,
                        lang: 'fa-IR',
                        dialogsInBody: true,
                        toolbar: [
                            ['style', ['style']],
                            ['font', ['bold', 'italic', 'underline', 'superscript', 'subscript', 'strikethrough', 'clear']],
                            ['fontsize', ['fontsize']],
                            ['fontname', ['fontname']],
                            ['color', ['color']],
                            ['para', ['ul', 'ol', 'paragraph']],
                            ['table', ['table']],
                            ['insert', ['link', 'picture']],
                            ['view', ['codeview']],
                        ]
                    };
                    $(elementId).summernote(options);
                };
                EditorService.prototype.setData = function (elementId, data) {
                    $(elementId).summernote('code', data);
                };
                EditorService.prototype.getData = function (elementId) {
                    return $(elementId).summernote('code');
                };
                return EditorService;
            }());
            exports_1("default", new EditorService());
        }
    };
});
//# sourceMappingURL=editor-service.js.map