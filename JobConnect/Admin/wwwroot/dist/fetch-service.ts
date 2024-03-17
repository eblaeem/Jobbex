declare var $;
import commonService from "./common-service.js";
import messageService from "./message-service.js";
class FetchService {
    post(url, data) {
        this.blockUi();
        let headers = {
            "Content-Type": "application/json",
        };

        let fomrmData = data;
        if (data instanceof FormData) {
            fomrmData = data;
            delete headers["Content-Type"];
        }
        else {
            let parametrs = {};
            if (data) {
                const keys = Object.keys(data);
                if (keys) {
                    keys.forEach(element => {
                        let value = data[element];
                        if (value != null && value != 'null' && (value != '' || value === false)) {
                            if (commonService.isString(value)) {
                                value = value.trim();
                            }
                            parametrs[element] = value;
                        }
                    });
                }
            }
            fomrmData = JSON.stringify(parametrs);
        }

        const response = fetch(url, {
            method: "POST",
            headers: headers,
            body: fomrmData
        });
        return response.then(res => {
            return res.json()
        }).then(response => {
            this.unBlockUi();
            return response
        }).catch(error => {
            this.unBlockUi();

            const message = error?.message;
            messageService.error(message);

            return error;
        });
    }
    async get(url, data?, blockUi = true) {
        if (blockUi) {
            this.blockUi();
        }
        if (data) {
            const keys = Object.keys(data);
            if (keys) {
                url += '?';
                keys.forEach(element => {
                    const value = data[element];
                    if (value != null && value != '') {
                        url += `${element}=${value}&`;
                    }
                });
            }
        }
        const response = fetch(url, {
            method: "GET",
            headers: {
                "X-Requested-With": "XMLHttpRequest"
            }
        });
        return response.then(response => {
            this.unBlockUi();
            return response.text();
        }).catch(error => {
            this.unBlockUi();

            const message = error?.message;
            messageService.error(message);

            return error;
        });
    }
    blockUi(message = '') {
        if (!message) {
            message = 'لطفا منتظر بمانید ...';
        }
        $.blockUI({
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .5,
                color: '#fff'
            },
            message: message
        });
    }
    unBlockUi() {
        $.unblockUI();
    }
}
export default new FetchService();