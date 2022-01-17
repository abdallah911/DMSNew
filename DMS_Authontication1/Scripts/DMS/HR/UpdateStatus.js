var url = window.location.href;
//var id = url.substring(url.lastIndexOf('/') + 1);
//$('#id').val(id);

$(function () {
    //Mediciences
    $('#submit').click(function () {
        debugger;
        $.ajax({
            type: 'POST',
            url: '/IndemnitiesAdmin/UpdateStatus/',
            dataType: 'Json',
            data: {
                statustext: $('#Status').val(),
                id: $('#id').val(),
            },
            success: function (r) {
                bootbox.alert(r);
                //bootbox.alert("Data Saved Susseccfuly ");
            },
            error: function (err) {
                bootbox.alert("Error saving response,please check your internet connection");
                $("#submit").attr("disabled", false);
            }
        });
    });

})
