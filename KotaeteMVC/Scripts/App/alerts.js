var KotaeteAlerts = KotaeteAlerts ||
{
    alert_div_id: "#alert-container",
    info: "alert-info",
    success: "alert-success",
    warning: "alert-warning",
    error: "alert-danger",
    AddAlert: function (text, type, dismissable)
    {
        var alertContainer = $("<div></div>").addClass("user-alert alert alert-hidden").addClass(type);
        if (dismissable === true)
        {
            alertContainer.addClass("alert-dismissable");
            $('<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>').appendTo(alertContainer);
        }
        $(alertContainer).append(text);
        $(this.alert_div_id).append(alertContainer);
        alertContainer.slideDown();
    },
    AddAlertSuccess: function(text, dismissable)
    {
        dismissable = dismissable || true;
        this.AddAlert(text, this.success, dismissable);
    },
    AddAlertInfo: function(text, dismissable)
    {
        dismissable = dismissable || true;
        this.AddAlert(text, this.info, dismissable);
    },
    AddAlertWarning: function(text, dismissable) {
        dismissable = dismissable || true;
        this.AddAlert(text, this.warning, dismissable);
    },
    AddAlertError: function(text, dismissable) {
        dismissable = dismissable || true;
        this.AddAlert(text, this.error, dismissable);
    }
};
