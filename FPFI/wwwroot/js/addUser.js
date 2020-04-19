$(document).ready(function () {
    $(".selectpicker").selectpicker();
});
$("#Password").keydown(function () {
    var strength = {
        0: "Bad",
        1: "Weak",
        2: "Good",
        3: "Strong",
        4: "Very Strong"
    }
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
function claims() {
    $("span#UserClaimsVal").html("");
    if ($("input[type=checkbox]:checked").length === 0) {
        //Then display the message
        $("span#UserClaimsVal").html("Please select at least one User Claim");
        return true;
    }
    return false;
}
function roles() {
    $("span#ApplicationRolesVal").html("");
    if ($(".bootstrap-select").find("li.selected").length == 0) {
        $("span#ApplicationRolesVal").html("Please select a Role");
        return true;
    }
    return false;
}
$(function () {
    $("input[type=checkbox]").change(function () {
        claims();
    });
    $(".bootstrap-select").change(function () {
        roles();
    });
    //Change the button control here
    $("form#modal-form").submit(function (e) {
        //check if checked count is equal to zero
        var stop = false;
        stop = claims();
        if (!stop) {
            stop = roles();
        } else { roles(); }
        if (stop) {
            e.preventDefault();
        }
    });
});