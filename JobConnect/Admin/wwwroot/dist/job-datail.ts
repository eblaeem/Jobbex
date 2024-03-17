declare var $;
import commonService from './common-service.js';
import fetchService from './fetch-service.js';
import messageService from './message-service.js';
import modalService from './modal-service.js';
import printService from './print-service.js';
import selectService from './select-service.js';

export class CompanyJob {
    constructor() {
        const list = ['WorkExperienceYearId', 'CityId', 'OrganizationSizeId', 'JobGroupId', 'PageSize'];
        list.forEach(item => {
            selectService.build(`#${item}`);
        });

        $('#search-form').click(target => this.search());
        $(document).on("click", ".pagination a[data-ajax=true]", e => {
            e.stopImmediatePropagation();
            this.search(e.target.getAttribute("data-page"));
        });

        $(document).on("click", '.download-attachment-resumeh', target => this.downloadResumeht(target));
        $(document).on("click", '.download-site-resumeh', target => this.downloadSiteResumeht(target));
        $(document).on("click", '.change-status', target => this.changeStatusModal(target));
    }
    search(pageNumber = 1) {
        const data = commonService.getFormData('job-detail-form');
        data.pageNumber = pageNumber;
        data.pageSize = $('#PageSize').val();
        fetchService.get('/jobRequest/get', data).then((response: any) => {
            $('#response').html(response);
        });
    };

    downloadResumeht(event) {
        const item = {
            id: $(event.target).data('id')
        };
        fetchService.post('/JobRequest/downloadResumeh', item).then(response => {
            if (!response.isValid) {
                return messageService.error(response);
            }
            printService.print(response.fileData, response.fileType);
        });
    }
    downloadSiteResumeht(event) {
        const item = {
            id: $(event.target).data('user-id')
        };
        fetchService.post('/JobRequest/downloadSiteResumeh', item).then(response => {
            if (!response.isValid) {
                return messageService.error(response);
            }
            printService.print(response.fileData);
        });
    }
    changeStatusModal(event) {
        const id = $(event.target).data('id');
        modalService.show('/jobRequest/changeStatus', null, { id: $(event.target).data('id') });
    }
}
new CompanyJob()
