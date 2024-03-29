System.register(["./common-service.js", "./fetch-service.js", "./message-service.js", "./select-service.js", "./editor-service.js"], function (exports_1, context_1) {
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
    var common_service_js_1, fetch_service_js_1, message_service_js_1, select_service_js_1, editor_service_js_1, JobSave;
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
            function (select_service_js_1_1) {
                select_service_js_1 = select_service_js_1_1;
            },
            function (editor_service_js_1_1) {
                editor_service_js_1 = editor_service_js_1_1;
            }
        ],
        execute: function () {
            JobSave = (function () {
                function JobSave() {
                    var _this = this;
                    var list = ["JobGroupId", "CityId", "WorkExperienceYearId", "ContractTypeId",
                        "SalaryTypeId", "GenderRequired", "EducationLevelRequired", "MilitaryStateRequired"];
                    list.forEach(function (item) {
                        select_service_js_1.default.build("#".concat(item));
                    });
                    editor_service_js_1.default.set('#Description', 110);
                    $(document).on("click", '#submit', function (target) { return _this.submit(target); });
                    $(document).on("change", '.RequiredLoginToSite', function (target) { return _this.requiredLoginToSite(target); });
                }
                JobSave.prototype.submit = function (target) {
                    return __awaiter(this, void 0, void 0, function () {
                        var data;
                        return __generator(this, function (_a) {
                            data = common_service_js_1.default.getFormData('job-save-form');
                            if ($('#CityId').select2('data')[0]) {
                                data.stateId = $($('#CityId').select2('data')[0].element).data('state-id');
                            }
                            fetch_service_js_1.default.post('/job/save', data).then(function (response) {
                                if (!response.isValid) {
                                    return message_service_js_1.default.error(response.message);
                                }
                                return location.href = '/job';
                            });
                            return [2];
                        });
                    });
                };
                ;
                JobSave.prototype.requiredLoginToSite = function (target) {
                    $('#RequiredLoginToSite').prop('checked', 'checked');
                };
                return JobSave;
            }());
            exports_1("JobSave", JobSave);
            new JobSave();
        }
    };
});
//# sourceMappingURL=job-save.js.map