function OnAskingSuccess(alertKey) {
    $("#question-modal").modal('hide');
    KotaeteAlerts.GetAlertMessageFor(alertKey, function (message) {
        KotaeteAlerts.AddAlertSuccess(message, true);
    });
}

function OnAskingFailure(alertKey) {
    $("#question-modal").modal('hide');
    KotaeteAlerts.GetAlertMessageFor(alertKey, function (message) {
        KotaeteAlerts.AddAlertError(message, false);
    });
}

function ShowAskFollowersModal(data) {
    $(data.responseText).appendTo("body").on('hidden.bs.modal', function () {
        $(this).remove()
    }).modal('show');
    $.validator.unobtrusive.parse($("#question-modal"));
}

function AskedSuccess(userName) {
    KotaeteAlerts.GetAlertMessageFor("askSuccess", function (message) {
        KotaeteAlerts.AddAlertSuccess(message, false);
    }, userName);
    $(".ask-question-box").val('');
}

function AnsweredSuccess(data, userName, questionId) {
    KotaeteAlerts.GetAlertMessageFor("answerSuccess", function (message) {
        KotaeteAlerts.AddAlertSuccess(message, false);
    }, userName);
    var currentReplies = parseInt($("#reply-stats a").html());
    if (currentReplies !== NaN) {
        currentReplies += 1;
        $("#reply-stats a").fadeOut('slow', function () { $(this).html(currentReplies).fadeIn() });
    }
    UpdateInboxCount();
    $("#" + questionId).slideUp('slow', function () {
        $(this).remove();
        if ($(".question-detail-panel").length == 0) {
            $.get("NoAnswers", "", function (response) {
                $("#inbox-questions").html(response);
            });
        }
    });
}

function UpdateInboxCount() {
    $.post('InboxCount', "", function (response) {
        $("#inbox-count").fadeOut('slow', function () {
            if (response > 0) {
                $(this).hide().html(response).fadeIn();
            }
        });
    }, 'json');
}

$(function () {
    if ($("#question-modal").length && $("#question-modal").hasClass("modal") == false) {
        $("#question-modal").remove();
        KotaeteAlerts.GetAlertMessageFor("modalJS", function (message) {
            KotaeteAlerts.AddAlertWarning(message, false);
        })
    };
});

$(function () {
    window.setInterval(UpdateInboxCount, 2 * 60 * 1000);
});

$(SetEnterKeyFormBindings);

function SetEnterKeyFormBindings() {
    $("#ask-question-textarea, .ask-question-box, .answer-textarea").keydown(OnFormKeyDown);
}

function OnFormKeyDown(e) {
    if (e.ctrlKey && e.keyCode == 13) {
        $(this.form).submit();
        $(this).blur();
        return false;
    }
}

function OnCommentSuccess(data, location, commentLocation) {
    var newComment = $(data);
    $(location).append(newComment);
    newComment.hide().fadeIn('slow');
    $(commentLocation + " textarea").val('');
}

$(function () {
    $(".delete-button").click(OnDeleteQuestion);
})

function OnDeleteQuestion(event) {
    event.preventDefault();
    var form = $(event.target).closest('form');
    $("#confirm-delete-modal").modal({ backdrop: 'static', keyboard: false }).one('click', "#delete", function (event) {
        DeleteQuestionAjax(form)
    }).one('click', "#cancel", function (event) {
        $("#confirm-delete-modal").off('click');
    });
}

function DeleteQuestionAjax(form) {
    var model = {};
    model['questionDetailId'] = $(form).find("input[name='QuestionDetailId']").val();
    model['__RequestVerificationToken'] = $(form).find("input[name='__RequestVerificationToken']").val();
    $.ajax({
        type: "POST",
        url: $(form).find("input[name='AjaxDeleteUrl']").val(),
        dataType : "json",
        data: model,
        error: function () {
            KotaeteAlerts.AddAlertError($(form).find("input[name='DeleteQuestionError']").val(), false);
        },
        success: function () {
            KotaeteAlerts.GetAlertMessageFor("deleteSuccess", function (message) {
                KotaeteAlerts.AddAlertSuccess(message, true);
            });
            $(form).closest(".question-detail-panel").fadeOut('slow', function () {
                $(this).remove();
            });
            UpdateInboxCount();
        }
    });
}