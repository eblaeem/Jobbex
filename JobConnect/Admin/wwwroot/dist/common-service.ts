declare var $;
class CommonService {
    getFormData(formId = '#form'): any {
        const array = {};
        let serializeArray = $('#' + formId).serializeArray()

        $.map(serializeArray, (element) => {
            const tempName = element.name.replace(/\./g, '_');
            const elementName = `#${tempName}`;
            let elementValue = element.value;

            const booleanList = ['false', 'true', 'on', 'off']
            if (booleanList.find(c => c == elementValue)) {
                elementValue = this.parseBoolean(elementValue) as any;
            }
            else if (elementValue && $(elementName).is('input') && $(elementName).attr('type') != 'hidden') {
                elementValue = elementValue.replace(new RegExp(',', 'g'), '');
                elementValue = elementValue.replace(new RegExp('٬', 'g'), '');
            }
            const checkBox = $(elementName).is(':checkbox');
            if (checkBox) {
                elementValue = $(elementName).prop('checked');
            }
            if (array[element['name']] && !checkBox) {
                // send array names
                const items = array[element['name']].toString().split(',');
                let result = elementValue;
                $.map(items, (n) => {
                    result = result + ',' + n;
                });
                array[element['name']] = result;
            }
            else {
                array[element['name']] = elementValue;
            }
        });
        return array;
    };
    parseBoolean(string) {
        if (string) {
            switch (string.toString().toLowerCase().trim()) {
                case 'true': case 'True': case 'on': case 'yes': case '1':
                    return true;
                case 'false': case 'False': case 'off': case 'no': case '0':
                    return false;
                default: return null;
            }
        }
        return null;
    }
    generateGuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
            const random = Math.random() * 16 | 0;
            const result = c === 'x' ? random : (random & 0x3 | 0x8);
            return result.toString(16);
        });
    };
    genrated(length) {
        var result = '';
        var characters = 'abcdefghijklmnopqrstuvwxyz';
        var charactersLength = characters.length;
        for (var i = 0; i < length; i++) {
            result += characters.charAt(Math.floor(Math.random() *
                charactersLength));
        }
        return result;
    };
    download(file, fileName = '') {
        const anchor = document.createElement('a');
        anchor.href = this.getBlobUrl(file);
        anchor.download = fileName;
        anchor.style.display = 'none';
        document.body.appendChild(anchor);
        anchor.click();
    }
    getBlobUrl(response, contentType = '') {
        const base64ToBlob = (base64Data, contentType = '', sliceSize = 512) => {
            const byteCharacters = atob(base64Data);
            const byteArrays = [];
            for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
                const slice = byteCharacters.slice(offset, offset + sliceSize);

                const byteNumbers = new Array(slice.length);
                for (let i = 0; i < slice.length; i++) {
                    byteNumbers[i] = slice.charCodeAt(i);
                }

                const byteArray = new Uint8Array(byteNumbers);
                byteArrays.push(byteArray);
            }
            return new Blob(byteArrays, { type: contentType });
        };
        const blob = base64ToBlob(response, contentType);
        const blobUrl = URL.createObjectURL(blob);
        return blobUrl;
    };
    isString(x) {
        return Object.prototype.toString.call(x) === "[object String]"
    }
    toPascalCase(str) {
        if (/^[a-z\d]+$/i.test(str)) {
            return str.charAt(0).toUpperCase() + str.slice(1);
        }
        return str.replace(
            /([a-z\d])([a-z\d]*)/gi,
            (g0, g1, g2) => g1.toUpperCase() + g2.toLowerCase()
        ).replace(/[^a-z\d]/gi, '');
    }
}
export default new CommonService();