declare var $;
import commonService from './common-service.js';
import fetchService from './fetch-service.js';
import messageService from './message-service.js';

export class Login {
    private clientGuid = '';
    constructor() {
        $(document).on("click", '#submit', target => this.submit());
        $(document).on("click", '#captcha', target => this.getCaptcha());
        this.clientGuid = commonService.generateGuid();
        this.getCaptcha();

        $("#login-form").keydown((e) => {
            const unicode = e.keyCode ? e.keyCode : e.charCode;
            if (unicode == 13) {
                this.submit();
                e.preventDefault();
                return true;
            }
            return true;
        });
    }

    async submit() {
        const data = commonService.getFormData('login-form');
        data.clientGuid = this.clientGuid;
        fetchService.post('/user/login', data).then((response: any) => {
            if (!response.isValid) {
                this.getCaptcha();
                return messageService.error(response.message)
            }
            return window.location.href = '/';
        });
    };
    getCaptcha() {
        fetchService.post('/user/captcha', { clientGuid: this.clientGuid }).then(response => {
            $('#Captcha').val('');
            $('#captcha-image').attr('src', 'data:image/png;base64,' + response.message.split('?')[0]);
        });
    }
}
new Login()
