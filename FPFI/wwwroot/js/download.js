$('.add').on('click', function () {
        var id = $(this).attr('id');
    $('input#' + id).prop('checked', !$(this).hasClass('added'));
    console.log($('input#' + id));
    $(this).toggleClass('added just-added');
        $(this).mouseleave(function () {
        $(this).removeClass("just-added");
    });
    checkbox();
});
    $("#Password").keydown(function () {
        var strength = {
            0: "Bad",
            1: "Weak",
            2: "Good",
            3: "Strong",
            4: "Very Strong"
        };
var val = $("#Password").val();
var result = zxcvbn(val);
$("#password-strength-meter").val(result.score + 1);
console.log(result);
        if (val !== "") {
        $("#result").html("Strength: " + strength[result.score]);
    } else {
        $("#result").html("");
    }
});
    function checkbox() {
        $("span#FilesVal").html("");
    if ($(".added").length === 0) {
        //Then display the message
        $("span#FilesVal").html("Please select at least one File Type");
    return true;
}
return false;
}
    function password() {
        $("#PasswordVal").html("");
    if (!$("#Password").val().match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/)) {
        //Then display the message
        $("#PasswordVal").html("Password must at least have one upper-case and lower-case letter and one number");
    return true;
}
return false;
}
    $(function () {
        $("#Password").change(function () {
            password();
        });
    //Change the button control here
    $("form#modal-form").submit(function (e) {
            //check if checked count is equal to zero
            //Then display the message
            var stop = false;
    stop = checkbox();
            if (!stop) {
        stop = password();
    } else {password(); }
            if (stop) {
        e.preventDefault();
    }
});
});