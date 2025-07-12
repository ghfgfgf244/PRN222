
$("#ImageFile").on("change", function (evt) {
    const file = this.files[0];
    if (file) {
        let preview = $("#preview");
        preview.attr("src", URL.createObjectURL(file)); // Hiển thị ảnh xem trước

        // Disable nút Lưu khi bắt đầu upload
        $("#btnSave").prop("disabled", true);

        // Upload ảnh ngay khi chọn
        let formData = new FormData();
        formData.append("Upload", file);

        $.ajax({
            url: "?handler=UploadAvatar", // Razor Pages handler
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.success) {
                    $("#AvatarPath").val(data.filePath); // Lưu đường dẫn vào input hidden
                    $("#btnSave").prop("disabled", false);
                } else {
                    alert("Upload thất bại: " + data.error);
                    $("#btnSave").prop("disabled", false);
                }
            },
            error: function () {
                alert("Lỗi khi upload ảnh!");
                $("#btnSave").prop("disabled", false);
            }
        });
    }
});