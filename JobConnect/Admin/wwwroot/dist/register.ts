declare var $;
import commonService from './common-service.js';
import fetchService from './fetch-service.js';
import messageService from './message-service.js';

export class Register {
    constructor() {
        $(document).on("click", '#submit',target => this.submit(target));
    }

    async submit(target) {
        const data = commonService.getFormData('register-form');
        fetchService.post('/user/register', data).then((response: any) => {
            if (!response.isValid) {
                return messageService.error(response.message)
            }
            return window.location.href = '/user/login';
        });
    };
}
new Register()
