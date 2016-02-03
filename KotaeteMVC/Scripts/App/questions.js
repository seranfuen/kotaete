function OnAskingSuccess(alertKey) {
    $("#question-modal").modal('toggle');
    KotaeteAlerts.GetAlertMessageFor(alertKey, function (message) {
        KotaeteAlerts.AddAlertSuccess(message, false);
    });
}