function OnAskingSuccess(alertKey) {
    $("#question-modal").modal('toggle');
    KotaeteAlerts.GetAlertMessageFor(alertKey, function (message) {
        KotaeteAlerts.AddAlertSuccess(message, false);
    });
}

function ShowAskFollowersModal(data) {
    $(data.responseText).appendTo("body").on('hidden.bs.modal', function() {
     $(this).remove()
    }).modal('show');
}