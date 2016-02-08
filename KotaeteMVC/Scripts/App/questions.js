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

function AnsweredSuccess(userName, questionId) {
    KotaeteAlerts.GetAlertMessageFor("answerSuccess", function (message) {
        KotaeteAlerts.AddAlertSuccess(message, false);
    }, userName);
    var currentReplies = parseInt($("#reply-stats a").html());
    if (currentReplies !== NaN)
    {
        currentReplies += 1;
        $("#reply-stats a").fadeOut('slow', function() { $(this).html(currentReplies).fadeIn() });
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
                $(this).html(response).fadeIn();
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

$(function () {
    $("#ask-question-textarea, .ask-question-box, .answer-textarea").keydown(function (e) {
        if (e.ctrlKey && e.keyCode == 13) {
            $(this.form).submit();
            return false;
        }
    });
});

function OnCommentSuccess(data, location, commentLocation) {
    var newComment = $(data);
    $(location).append(newComment);
    newComment.hide().fadeIn('slow');
    $(commentLocation + " textarea").val('');
}