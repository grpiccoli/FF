$(document).ready(function () {
    $(".selectpicker").selectpicker();
});
$(function () {
    //Change the button control here
    $("form#modal-form").submit(function (e) {
        //check if checked count is equal to zero
        var stop = false;
        if ($("input[type=checkbox]:checked").length === 0) {
            //Then display the message
            $("span#UserClaimsVal").html("Please select at least one User Claim");
            stop = true;
        }
        if ($(".bootstrap-select").find("li.selected").length == 0) {
            $("span#ApplicationRolesVal").html("Please select a Role");
            stop = true;
        }
        if (stop) {
            e.preventDefault();
        }
    });
});