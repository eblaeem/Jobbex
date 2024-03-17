declare var $;
import commonService from './common-service.js';
import fetchService from './fetch-service.js';
import messageService from './message-service.js';
import selectService from './select-service.js';
import editorService from './editor-service.js';

export class JobSave {
    constructor() {
        const list = ["JobGroupId", "CityId", "WorkExperienceYearId", "ContractTypeId",
            "SalaryTypeId", "GenderRequired", "EducationLevelRequired", "MilitaryStateRequired"];
        list.forEach(item => {
            selectService.build(`#${item}`);
        });
        editorService.set('#Description', 110);

        $(document).on("click", '#submit', target => this.submit(target));
        $(document).on("change", '.RequiredLoginToSite', target => this.requiredLoginToSite(target))
    }

    async submit(target) {
        let data = commonService.getFormData('job-save-form');
        if ($('#CityId').select2('data')[0]) {
            data.stateId = $($('#CityId').select2('data')[0].element).data('state-id');
        }
        fetchService.post('/job/save', data).then((response: any) => {
            if (!response.isValid) {
                return messageService.error(response.message)
            }
            return location.href = '/job';
        });
    };
    requiredLoginToSite(target) {
        $('#RequiredLoginToSite').prop('checked', 'checked');
    }
}
new JobSave()
