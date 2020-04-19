const entry = new signalR.HubConnectionBuilder()
    .withUrl("/entryHub")
    .build();

entry.on("Update", function (func, e) {
    console.log(func, e);
    if (func === "log") {
        const p = document.createElement("p");
        p.textContent = e;
        $("#log").html(p);
        $("#log").scrollTop = $("#log").scrollHeight;
    } else if (func === "agregada") {
        $(".agregadas").html(e);
    } else if (func === "actualizada") {
        $(".actualizadas").html(e);
    } else if (func === "progress") {
        var pgr = e.toFixed(2);
        $("#progressHub").css('width', pgr + "%").attr('aria-valuenow', pgr).html(pgr + '%');
    } else if (func === "status") {
        var status = "bg-" + e;
        $("#progressHub").removeClass('bg-danger bg-warning bg-success bg-info');
        $("#progressHub").addClass(status);
    } else if (func === "stage") {
        $("#stage").html(e);
    } else if (func === "complete") {
        $("#running").removeClass("danger").addClass("success");
        $("#log").addClass("hidden");
    }
});

entry.start().catch(function (err) {
    return console.error(err.toString());
}).then(function () {
    entry.invoke('getConnectionId')
        .then(function (connectionId) {
            console.log("connectionID: " + connectionId);
            $("input[name=userId]").val(connectionId.toString());
        });
});