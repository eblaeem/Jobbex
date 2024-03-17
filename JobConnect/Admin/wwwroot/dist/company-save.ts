declare var $;
import commonService from './common-service.js';
import editorService from './editor-service.js';
import fetchService from './fetch-service.js';
import messageService from './message-service.js';
import selectService from './select-service.js';

export class CompanySave {
    constructor() {
        const list = ["JobGroupId", "CityId", "OrganizationSizeId"];
        list.forEach(item => {
            selectService.build(`#${item}`);
        });

        editorService.set('#Description');

        $('#attachment-logo-upload').change(target => this.uploadFile(target, 3));
        $('#attachment-background-upload').change(target => this.uploadFile(target, 4));
        $('#attachment-others').change(target => this.uploadFile(target, 5));

        $('#submit').click(target => this.submit(target));
    }

    async submit(target) {
        const data = commonService.getFormData('company-save-form');
        if ($('#CityId').select2('data')[0]) {
            data.stateId = $($('#CityId').select2('data')[0].element).data('state-id');
        }
        fetchService.post('/company/save', data).then((response: any) => {
            if (!response.isValid) {
                return messageService.error(response.message)
            }
        });
    };
    uploadFile(event, type) {
        const file = event.target.files[0];
        const formData = new FormData();
        formData.append("file", file);
        formData.append("type", type);

        fetchService.post('/company/upload', formData).then(response => {
            if (!response.isValid) {
                return messageService.error(response.message);
            }
            return location.reload();
        });
    }
}
new CompanySave()
