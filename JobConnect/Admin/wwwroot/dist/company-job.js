System.register(["./common-service.js", "./fetch-service.js", "./select-service.js"], function (exports_1, context_1) {
    "use strict";
    var common_service_js_1, fetch_service_js_1, select_service_js_1, CompanyJob;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [
            function (common_service_js_1_1) {
                common_service_js_1 = common_service_js_1_1;
            },
            function (fetch_service_js_1_1) {
                fetch_service_js_1 = fetch_service_js_1_1;
            },
            function (select_service_js_1_1) {
                select_service_js_1 = select_service_js_1_1;
            }
        ],
        execute: function () {
            CompanyJob = (function () {
                function CompanyJob() {
                    var _this = this;
                    var list = ['WorkExperienceYearId', 'CityId', 'OrganizationSizeId', 'JobGroupId', 'PageSize',
                        'ContractTypeId', 'SalaryRequestedId'];
                    list.forEach(function (item) {
                        select_service_js_1.default.build("#".concat(item));
                    });
                    $('#search-form').click(function (target) { return _this.search(); });
                    $(document).on("click", ".pagination a[data-ajax=true]", function (e) {
                        e.stopImmediatePropagation();
                        _this.search(e.target.getAttribute("data-page"));
                    });
                }
                CompanyJob.prototype.search = function (pageNumber) {
                    if (pageNumber === void 0) { pageNumber = 1; }
                    var data = common_service_js_1.default.getFormData('job-form');
                    data.pageNumber = pageNumber;
                    data.pageSize = $('#PageSize').val();
                    fetch_service_js_1.default.get('/job/get', data).then(function (response) {
                        $('#response').html(response);
                    });
                };
                ;
                CompanyJob.prototype.scrollbarRemove = function () {
                    $('#response').mCustomScrollbar("destroy");
                };
                CompanyJob.prototype.scrollbar = function () {
                    $('.table-response').mCustomScrollbar({
                        theme: "dark"
                    });
                };
                return CompanyJob;
            }());
            exports_1("CompanyJob", CompanyJob);
            new CompanyJob();
        }
    };
});
//# sourceMappingURL=company-job.js.map