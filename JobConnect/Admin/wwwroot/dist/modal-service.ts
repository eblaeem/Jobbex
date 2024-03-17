declare var System, $;
import fetchService from "./fetch-service.js";
import commonService from "./common-service.js";
import messageService from "./message-service.js";

class ModalService {
    constructor() {

    }
    show(url, jsName?, data?, shownCallBack?) {
        const modalId = commonService.genrated(6);
        if (!data) {
            data = {};
        }
        data.modalId = modalId;
        fetchService.get(url, data).then(response => {
            
            if (response == null) {
                return messageService.error('ردیفی برای نمایش وجود ندارد');
            }
            $("body").append(response);
            ($(`#${modalId}`) as any).modal('show');

            $(`#${modalId}`).on('shown.bs.modal', function () {
                if (jsName) {
                    System.import(jsName).then(result => {
                        let moduleName = commonService.toPascalCase(jsName);
                        new result[moduleName](`#${modalId}`);
                    });
                }
                if (shownCallBack) {
                    shownCallBack(response);
                }
            });
            $(`#${modalId}`).on('hide.bs.modal', function () {
                $(`#${modalId}`).remove();
            });
        });
    };
    hide(event) {
        const target = $(event.target);
        ($(target).closest('.modal') as any).modal('hide');
    }
}
export default new ModalService();