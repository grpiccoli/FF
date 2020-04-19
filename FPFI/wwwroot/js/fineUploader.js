var token = $('input:hidden[name="__RequestVerificationToken"]').val();
var manualUploader = new qq.FineUploader({
    element: document.getElementById('fine-uploader-manual-trigger'),
    form: {
        element: "upload"
    },
    template: 'qq-template-manual-trigger',
    request: {
        endpoint: '@Url.Action("CreateFito","Entries")',
        //customHeaders: {
        //    'X-CSRF-TOKEN': token,
        //    'X-CSRFToken': token
        //},  // for token validation
        params: {
            pid: 1,
            aid: 1
            //'csrfmiddlewaretoken': token
        }  // any custom params about file
    },
    thumbnails: {
        placeholders: {
            waitingPath: '/lib/fine-uploader/placeholders/waiting-generic.png',
            notAvailablePath: '/images/excel.png'
        }
    },
    validation: {
        allowedExtensions: ['xls','xlsx'],
        itemLimit: 1,
        sizeLimit: 20971520
    },
    autoUpload: false,
    debug: true,
    failedUploadTextDisplay: {
        mode: 'custom'
    },
    showMessage: function (message) {
        // for show error in some div instead of modal message
        $('#MessageUploaderError').html(message);
    },
    callbacks: {
        onSubmit: function (id, fileName) {
            // Extend the default parameters for all files
            // with the parameters for _this_ file.
            // qq.extend is part of a myriad of Fine Uploader
            // utility functions and cross-browser shims
            // found in client/js/util.js in the source.
            var newParams = {
                entry: 0
            },
                finalParams = defaultParams;

            qq.extend(finalParams, newParams);
            this.setParams(finalParams);
        }
    }
});

qq(document.getElementById("trigger-upload")).attach("click", function () {
    manualUploader.uploadStoredFiles();
});