﻿var drop = $("input");
drop.on('dragenter', function (e) {
    $(".drop").css({
        "border": "4px dashed #09f",
        "background": "rgba(0, 153, 255, .05)"
    });
    $(".cont").css({
        "color": "#09f"
    });
}).on('dragleave dragend mouseout drop', function (e) {
    $(".drop").css({
        "border": "3px dashed #DADFE3",
        "background": "transparent"
    });
    $(".cont").css({
        "color": "#8E99A5"
    });
});
function handleFileSelect(evt) {
    var files = evt.target.files;
    for (var i = 0; i < files.length; i++) {
        var f = files[i];
        var reader = new FileReader();
        reader.onload = (function (theFile) {
            return function (e) {
                $(".tit").html('<span class="icon-excel"><span class="path1"></span><span class="path2"></span></span><br/>' + decodeURI(escape(theFile.name)));
            };
        })(f);
        reader.readAsDataURL(f);
    }
}
$('#files').change(handleFileSelect);