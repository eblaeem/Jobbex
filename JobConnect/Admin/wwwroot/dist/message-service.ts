declare var $, Notyf;
class MessageService {
    constructor() {

    }
    success(message) {
        if (!message) {
            message = 'عملیات با موفقیت انجام گردید';
        }
        new Notyf().success(message)
    };
    error(message) {
        if (!message) {
            message = 'خطایی رخ داده است';
        }
        let result = [];
        try {
            const parse = JSON.parse(message);
            parse.forEach(element => {
                result.push(element.Label);
            });
        } catch (e) {
            if (Array.isArray(message)) {
                result = message;
            }
            else {
                try {
                    const toJosn = message.toJSON();
                    if (toJosn.status == 403) {
                        message = 'به درخواست مورد نظر دسترسی ندارید';
                    }
                } catch (e) {

                }
                result.push(message);
            }
        }
        if (result) {
            const notyf = new Notyf();
            for (let i = 0; i < result.length; i++) {
                let title = result[i];
                if (title as object) {
                    if (result[i].Label) {
                        title = result[i].Label;
                    }
                }
                notyf.error(
                    {
                        message: title,
                        duration: 9000,
                        icon: false,
                        position: {
                            x: 'left',
                            y: 'bottom',
                        }
                    });
            }
        }
    }
}
export default new MessageService();