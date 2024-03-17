declare var $;
class EditorService {
    set(elementId, height?) {
        const options = {
            tabsize: 2,
            height: height != null ? height : 150,
            lang: 'fa-IR',
            dialogsInBody: true,
            toolbar: [
                ['style', ['style']],
                ['font', ['bold', 'italic', 'underline', 'superscript', 'subscript', 'strikethrough', 'clear']],
                ['fontsize', ['fontsize']],
                ['fontname', ['fontname']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link', 'picture']],
                ['view', ['codeview']],
            ]
        } as any;

        $(elementId).summernote(options);
    }

    setData(elementId, data) {
        $(elementId).summernote('code', data);
    }

    getData(elementId) {
        return $(elementId).summernote('code');
    }

}
export default new EditorService();