import commonService from "./common-service.js";

declare var printJS;

class PrintService {
    print(fileData, fileType?, onPrintDialogClose = null) {
        if (fileType && fileType.startsWith('image/')) {
            commonService.download(fileData, 'download.png');
        }
        else {
            printJS({
                printable: fileData,
                type: 'pdf',
                base64: true,
                onPrintDialogClose: () => {
                    if (onPrintDialogClose) {
                        onPrintDialogClose();
                    }
                },
                onPdfOpen: () => {
                }
            });
        }

    }
}
export default new PrintService();
