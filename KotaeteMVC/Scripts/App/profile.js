$(function () {
    Profile.SelectedFile = false;
    $("#avatar-input").change(function () {
        Profile.SelectedFile = true;
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#avatar-cropper').html('<img id="avatar" />');
                $('#avatar').cropper("replace", e.target.result);
                $('#avatar').cropper('destroy').cropper({ viewMode: 3, aspectRatio : 1 });
                $('#avatar').cropper('setDragMode', 'move');
            }
            reader.readAsDataURL(this.files[0]);
        }
    });
});

$(function () {
    $("#edit-profile-button").click(function (event) {
        event.preventDefault();
        if (Profile.SelectedFile == true) {
            $("#avatar-image").val($("#avatar").cropper('getCroppedCanvas').toDataURL());
        }
        $(this).closest("form").submit();
    });
})

var Profile = Profile ||
{
    SelectedFile : false  
};