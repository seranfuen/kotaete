var KotaeteNotifications = KotaeteNotifications ||
{
    AddNotificationClass: function () {
        $(".notification").addClass("notification-link");
    },

    AddLinks: function () {
        $(".notification").click(function () {
            window.location.href = $(this).attr('data-notification-link');
        });
        $(".notification").hover(function () {
            window.status = $(this).attr('data-notification-link');
        }, function () {
            window.status = "";
        });;
    }
}

$(function () {
    KotaeteNotifications.AddNotificationClass();
    KotaeteNotifications.AddLinks();
})