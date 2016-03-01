function PaginationOnBegin(entityName) {
    toReplaceId = "#" + entityName;
    $("#paginator-loading").clone().appendTo(toReplaceId).show();
    $(toReplaceId).wrap('<div id="ajax-overlay" class="fadeout noselect"></div>');
}

function PaginationOnSuccess(entityName) {
    $(toReplaceId).removeClass('fadeout').unwrap();
    $('html, body').animate({
        scrollTop: $("#page-body").offset().top - 30
    }, 500);
}

function PaginationOnFailure(entityName) {
    $("#" + entityName).unwrap().html("Error loading page");
}

function RemoveHref() {
    $("#pagination-list li a").attr('href', '#');
}

// Init more button
$(function () {
    $(".more-button input").click(function () {
        var callUrl = $(this).attr("data-url");
        var elementToUpdate = $(this).attr("data-target");
        var button = $(this);
        $.ajax({
            url: callUrl,
            method: "POST"
        }).done(function (data, status) {
            $(data.html).hide().appendTo("#" + elementToUpdate).slideDown('slow');
            if (data.hasMore === false) {
                $(button).fadeOut('fast', function () { $(button).remove(); });
            }
            else
            {
                button.attr("data-url", data.url);
            }
        });
    });
});