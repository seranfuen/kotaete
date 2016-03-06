$(function () {
    $(".comment-button").click(function (event) {
        event.preventDefault();
        $(this).parent().parent().siblings(".comment-list").toggleClass("hidden");
    });
});