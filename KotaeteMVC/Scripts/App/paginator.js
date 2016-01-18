function PaginationOnBegin(entityName) {
    toReplaceId = "#" + entityName;
    $(toReplaceId).addClass('fadeout');
    $("#paginator-loading").clone().appendTo(toReplaceId).show();
}

function PaginationOnSuccess(entityName) {
    $(toReplaceId).removeClass('fadeout');
}

function PaginationOnFailure(entityName) {
    $("#" + entityName).html("Error loading page");
}

function RemoveHref() {
    $("#pagination-list li a").attr('href', '#');
}

//$(RemoveHref);