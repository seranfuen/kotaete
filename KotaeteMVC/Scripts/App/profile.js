$(function () {
    $('#avatar').cropper({
        aspectRatio: 1,
    });
})

$(function () {
    $("#avatar-input").change(function () {
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#avatar').cropper("replace", e.target.result)
            }
            reader.readAsDataURL(this.files[0]);
        }
    });
})