var KotaeteAlerts = KotaeteAlerts ||
{
    secondsToShow: 10,
    alert_div_id: "#alert-container",
    info: "alert-info",
    success: "alert-success",
    warning: "alert-warning",
    error: "alert-danger",
    RemovePreviousAlerts: function () {
        $('.alert-success, .alert-danger, .alert-warning, .alert-info').hide('fast', function () {
            $(this).remove();
        });
    },
    AddAlert: function (text, type) {
        this.RemovePreviousAlerts();
        var alertContainer = $("<div></div>").addClass("user-alert alert alert-hidden").addClass(type);
        alertContainer.addClass("alert-dismissable");
        $('<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>').appendTo(alertContainer);
        $(alertContainer).append(text);
        $(this.alert_div_id).append(alertContainer);
        alertContainer.fadeIn();
        window.setTimeout(function () {
            $(alertContainer).fadeOut('slow', function () { $(this).remove(); });
        }, this.secondsToShow * 1000);
    },
    AddAlertSuccess: function (text) {
        this.AddAlert(text, this.success);
    },
    AddAlertInfo: function (text) {
        this.AddAlert(text, this.info);
    },
    AddAlertWarning: function (text) {
        this.AddAlert(text, this.warning);
    },
    AddAlertError: function (text) {
        this.AddAlert(text, this.error);
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

$(function () {
    window.setTimeout(function () {
        $(".user-alert").fadeOut('slow', function () {
            $(this).remove();
        })
    }, KotaeteAlerts.secondsToShow * 1000)
})