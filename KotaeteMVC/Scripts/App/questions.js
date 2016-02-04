function OnAskingSuccess(alertKey) {
    $("#question-modal").modal('hide');
    KotaeteAlerts.GetAlertMessageFor(alertKey, function (message) {
        KotaeteAlerts.AddAlertSuccess(message, true);
    });
}

function OnAskingFailure(alertKey) {
    $("#question-modal").modal('hide');
    KotaeteAlerts.GetAlertMessageFor(alertKey, function (message) {
        KotaeteAlerts.AddAlertError(message, false);
    });
}

function ShowAskFollowersModal(data) {
    $(data.responseText).appendTo("body").on('hidden.bs.modal', function() {
     $(this).remove()
    }).modal('show');
    $.validator.unobtrusive.parse($("#question-modal"));
}

$(function () {
    if ($("#question-modal").length && $("#question-modal").hasClass("modal") == false) {
        $("#question-modal").remove();
        KotaeteAlerts.GetAlertMessageFor("modalJS", function (message) {
            KotaeteAlerts.AddAlertWarning(message, false);
        })
    };
});