declare var $;
import commonService from './common-service.js';
import fetchService from './fetch-service.js';
import selectService from './select-service.js';

export class CompanyJob {
    constructor() {
        const list = ['WorkExperienceYearId', 'CityId', 'OrganizationSizeId', 'JobGroupId', 'PageSize',
            'ContractTypeId','SalaryRequestedId'];
        list.forEach(item => {
            selectService.build(`#${item}`);
        });
        //this.scrollbar();
        $('#search-form').click(target => this.search());

        $(document).on("click", ".pagination a[data-ajax=true]", e => {
            e.stopImmediatePropagation();
            this.search(e.target.getAttribute("data-page"));
        });
    }

    search(pageNumber = 1) {
        const data = commonService.getFormData('job-form');
        data.pageNumber = pageNumber;
        data.pageSize = $('#PageSize').val();
        fetchService.get('/job/get', data).then((response: any) => {
            //this.scrollbarRemove();

            $('#response').html(response);

            //this.scrollbar();
        });
    };

    scrollbarRemove() {
        $('#response').mCustomScrollbar("destroy")
    }
    scrollbar() {
        $('.table-response').mCustomScrollbar({
            theme: "dark"
        });
    }
}
new CompanyJob()
