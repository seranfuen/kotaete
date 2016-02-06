function OnFollowError(alertKey) {
    KotaeteAlerts.GetAlertMessageFor(alertKey, function (message) {
        KotaeteAlerts.AddAlertError(message, false);
    });
}

function OnFollowSuccess(alertKey, userName) {
    KotaeteAlerts.GetAlertMessageFor(alertKey, function (message) {
        KotaeteAlerts.AddAlertSuccess(message, false);
    }, userName);
}