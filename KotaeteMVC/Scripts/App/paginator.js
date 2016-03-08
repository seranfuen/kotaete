var Paginator = {
    CallbackOnSuccess: [],
    CallbackOnBegin: [],
    CallbackOnFailure: [],

    AddSuccessCallback: function (callback) {
        this.CallbackOnSuccess.push(callback);
    },

    AddBeginCallback: function (callback) {
        this.CallbackOnBegin.push(callback);
    },

    AddFailureCallback: function (callback) {
        this.CallbackOnFailure.push(callback);
    },

    PaginationOnSuccess: function (entityName) {
        $(toReplaceId).removeClass('fadeout').unwrap();
        $('html, body').animate({
            scrollTop: $("#page-body").offset().top - 30
        }, 500);
        for (callback of this.CallbackOnSuccess) {
            callback();
        }
    },

    PaginationOnBegin: function (entityName) {
        toReplaceId = "#" + entityName;
        $("#paginator-loading").clone().appendTo(toReplaceId).show();
        $(toReplaceId).wrap('<div id="ajax-overlay" class="fadeout noselect"></div>');
        for (callback of this.CallbackOnBegin) {
            callback();
        }
    },

    PaginationOnFailure: function (entityName) {
        $("#" + entityName).unwrap().html("Error loading page");
        for (callback of this.CallbackOnFailure) {
            callback();
        }
    },

    RemoveHref: function () {
        $("#pagination-list li a").attr('href', '#');
    },

    AttachMoreButtonHandlers : function () {
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
                else {
                    button.attr("data-url", data.url);
                }
            });
        });
    }
}

var Paginator = Paginator || {};

// Init more button
$(function () {
    Paginator.AttachMoreButtonHandlers();
    Paginator.AddSuccessCallback(Paginator.AttachMoreButtonHandlers);
});

