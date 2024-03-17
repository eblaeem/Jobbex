System.register(["./common-service.js", "./fetch-service.js", "./message-service.js", "./modal-service.js", "./print-service.js", "./select-service.js"], function (exports_1, context_1) {
    "use strict";
    var common_service_js_1, fetch_service_js_1, message_service_js_1, modal_service_js_1, print_service_js_1, select_service_js_1, CompanyJob;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (common_service_js_1_1) {
                common_service_js_1 = common_service_js_1_1;
            },
            function (fetch_service_js_1_1) {
                fetch_service_js_1 = fetch_service_js_1_1;
            },
            function (message_service_js_1_1) {
                message_service_js_1 = message_service_js_1_1;
            },
            function (modal_service_js_1_1) {
                modal_service_js_1 = modal_service_js_1_1;
            },
            function (print_service_js_1_1) {
                print_service_js_1 = print_service_js_1_1;
            },
            function (select_service_js_1_1) {
                select_service_js_1 = select_service_js_1_1;
            }
        ],
        execute: function () {
            CompanyJob = (function () {
                function CompanyJob() {
                    var _this = this;
                    var list = ['WorkExperienceYearId', 'CityId', 'OrganizationSizeId', 'JobGroupId', 'PageSize'];
                    list.forEach(function (item) {
                        select_service_js_1.default.build("#".concat(item));
                    });
                    $('#search-form').click(function (target) { return _this.search(); });
                    $(document).on("click", ".pagination a[data-ajax=true]", function (e) {
                        e.stopImmediatePropagation();
                        _this.search(e.target.getAttribute("data-page"));
                    });
                    $(document).on("click", '.download-attachment-resumeh', function (target) { return _this.downloadResumeht(target); });
                    $(document).on("click", '.download-site-resumeh', function (target) { return _this.downloadSiteResumeht(target); });
                    $(document).on("click", '.change-status', function (target) { return _this.changeStatusModal(target); });
                }
                CompanyJob.prototype.search = function (pageNumber) {
                    if (pageNumber === void 0) { pageNumber = 1; }
                    var data = common_service_js_1.default.getFormData('job-detail-form');
                    data.pageNumber = pageNumber;
                    data.pageSize = $('#PageSize').val();
                    fetch_service_js_1.default.get('/jobRequest/get', data).then(function (response) {
                        $('#response').html(response);
                    });
                };
                ;
                CompanyJob.prototype.downloadResumeht = function (event) {
                    var item = {
                        id: $(event.target).data('id')
                    };
                    fetch_service_js_1.default.post('/JobRequest/downloadResumeh', item).then(function (response) {
                        if (!response.isValid) {
                            return message_service_js_1.default.error(response);
                        }
                        print_service_js_1.default.print(response.fileData, response.fileType);
                    });
                };
                CompanyJob.prototype.downloadSiteResumeht = function (event) {
                    var item = {
                        id: $(event.target).data('user-id')
                    };
                    fetch_service_js_1.default.post('/JobRequest/downloadSiteResumeh', item).then(function (response) {
                        if (!response.isValid) {
                            return message_service_js_1.default.error(response);
                        }
                        print_service_js_1.default.print(response.fileData);
                    });
                };
                CompanyJob.prototype.changeStatusModal = function (event) {
                    var id = $(event.target).data('id');
                    modal_service_js_1.default.show('/jobRequest/changeStatus', null, { id: $(event.target).data('id') });
                };
                return CompanyJob;
            }());
            exports_1("CompanyJob", CompanyJob);
            new CompanyJob();
        }
    };
});
//# sourceMappingURL=job-datail.js.map