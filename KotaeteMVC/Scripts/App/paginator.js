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