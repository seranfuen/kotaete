$(function () {
    BindCropperReloading("#avatar-input", "#avatar-cropper", "#avatar", 1);
    BindCropperReloading("#header-input", "#header-cropper", "#header", 3);
});

function BindCropperReloading(inputSelector, imageCropperSelector, imageSelector, cropperAspectRatio) {
    Profile.SelectedFiles.header = false;
    Profile.SelectedFiles.avatar = false;
    $(inputSelector).change(function () {
        if (inputSelector == "#avatar-input") {
            Profile.SelectedFiles.avatar = true;
        } else if (inputSelector == "#header-input") {
            Profile.SelectedFiles.header = true;
        }
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $(imageCropperSelector).html('<img id="' + imageSelector.substr(1) + '" />');
                $(imageSelector).cropper("replace", e.target.result);
                $(imageSelector).cropper('destroy').cropper({ viewMode: 3, aspectRatio: cropperAspectRatio });
                $(imageSelector).cropper('setDragMode', 'move');
            }
            reader.readAsDataURL(this.files[0]);
        }
    });
}

$(function () {
    $("#edit-profile-button").click(function (event) {
        event.preventDefault();
        if (Profile.SelectedFiles.avatar) {
            $("#avatar-image").val($("#avatar").cropper('getCroppedCanvas').toDataURL());
        }
        if (Profile.SelectedFiles.header) {
            $("#header-image").val($("#header").cropper('getCroppedCanvas').toDataURL());
        }
        $(this).closest("form").submit();
    });
})

var Profile = Profile ||
{
    SelectedFiles : { header : false, avatar : false }
};