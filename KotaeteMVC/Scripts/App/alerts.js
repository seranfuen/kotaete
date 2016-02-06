var KotaeteAlerts = KotaeteAlerts ||
{
    alert_div_id: "#alert-container",
    info: "alert-info",
    success: "alert-success",
    warning: "alert-warning",
    error: "alert-danger",
    RemovePreviousAlerts: function () {
        $('.alert-success, .alert-warning, .alert-info').hide();
    },
    AddAlert: function (text, type, dismissable) {
        this.RemovePreviousAlerts();
        var alertContainer = $("<div></div>").addClass("user-alert alert alert-hidden").addClass(type);
        if (dismissable === true) {
            alertContainer.addClass("alert-dismissable");
            $('<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>').appendTo(alertContainer);
        }
        $(alertContainer).append(text);
        $(this.alert_div_id).append(alertContainer);
        alertContainer.slideDown();
    },
    AddAlertSuccess: function (text, dismissable) {
        dismissable = dismissable !== false;
        this.AddAlert(text, this.success, dismissable);
    },
    AddAlertInfo: function (text, dismissable) {
        dismissable = dismissable !== false;
        this.AddAlert(text, this.info, dismissable);
    },
    AddAlertWarning: function (text, dismissable) {
        dismissable = dismissable !== false;
        this.AddAlert(text, this.warning, dismissable);
    },
    AddAlertError: function (text, dismissable) {
        dismissable = dismissable !== false;
        this.AddAlert(text, this.error, dismissable);
    },
    GetAlertMessageFor: function (key, func, param1, param2, param3) {
        var url = $("#AlertCtrlPath").val();
        $.ajax({
            url: this.BuildUrl(url + "/" + key, param1, param2, param3)
        }).done(function (data) {
            func(data)
        });
    },
    BuildUrl: function (url, param1, param2, param3) {
        url = this.AddDefinedParamToUrl(url, param1);
        url = this.AddDefinedParamToUrl(url, param2);
        return this.AddDefinedParamToUrl(url, param3);
    },
    AddDefinedParamToUrl: function (url, param) {
        if (param === undefined) {
            return url;
        } else {
            url += "/" + param;
            return url;
        }
    }
};