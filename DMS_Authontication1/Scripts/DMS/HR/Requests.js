$(function () {
    $('#CARD_ID').select2({
        placeholder: 'Search for a cards',
        minimumInputLength: 5,
        ajax: {
            url: '/EmployeeRequest/GetActiveEmployess/',
            delay: 250,
            dataType: 'json',
            data: function (params) {
                var query = {
                    search: params.term,
                    page: params.page || 1
                }
                return query;
            },
            processResults: function (data, params) {
                params.page = params.page || 1;

                return {
                    results: data,//.results,
                    pagination: {
                        more: (params.page * 10) < data.count_filtered
                    }
                };
            }
            // Additional AJAX parameters go here; see the end of this chapter for the full code of this example

        }
    });
    $('#BIRTHDATE').datepicker({
        changeYear: true,
        yearRange: "-100:+0", // last hundred years
        maxDate: 0,
    });
    $('#START_DATE').datepicker({
        changeYear: true,
        minDate: 0,
    });
    $("#EMP_RELATION").change(function () {

        if ($(this).val() != "1" && $(this).val() != "") {
            $("#divCardId").show();
        }
        else {
            $("#divCardId").hide();
        }

    });
    $('#submit').click(function () {
        var result = ValidSaveRequest();
        if (result.valid) {
            var Request = {
                EMP_ENAME_ST: $("#EMP_ENAME_ST").val(),
                EMP_ENAME_SC: $("#EMP_ENAME_SC").val(),
                EMP_ENAME_FR: $("#EMP_ENAME_FR").val(),
                EMP_ANAME_ST: $("#EMP_ANAME_ST").val(),
                EMP_ANAME_SC: $("#EMP_ANAME_SC").val(),
                EMP_ANAME_TH: $("#EMP_ANAME_TH").val(),
                EMP_ENAME_TH: $("#EMP_ENAME_TH").val(),
                EMP_ANAME_FR: $("#EMP_ANAME_FR").val(),
                CARD_ID: $('#CARD_ID').val(),
                NATIONAL_ID: $("#NATIONAL_ID").val(),
                BIRTHDATE: $("#BIRTHDATE").val(),
                GENDER: $("#GENDER").val(),
                MOBILE: $("#MOBILE").val(),
                EMAIL: $("#EMAIL").val(),
                START_DATE: $("#START_DATE").val(),
                ADDRESS: $("#ADDRESS").val(),
                BRANCH: $("#BRANCH").val(),
                GLASSES: $("#GLASSES").val(),
                DISEASE: $("#DISEASE").val(),
                EMP_RELATION: $("#EMP_RELATION").val(),
                EMP_IMG: $("#EMP_IMG").val(),
                EMP_CLASS: $("#EMP_CLASS").val(),
                REQUEST_CODE: $("#REQUEST_CODE").val()

            };

            var data = new FormData();//()
            var files = $("#EMP_IMG").get(0).files;
            Request.EMP_IMG = files[0].name;
            data.append("File", files[0]);

            //Saving Image
            $.ajax({
                url: '/EmployeeRequest/SaveImage/',
                type: "POST",
                processData: false,
                contentType: false,
                data: data,
                success: function (response) {

                    //response image url instead of session
                },
                error: function (er) {
                    alert("Error Upload Image");
                }

            })
            $.ajax({
                type: 'POST',
                url: '/EmployeeRequest/SaveRequest/',
                dataType: 'Json',
                data: Request,
                success: function (result) {

                    bootbox.dialog({
                        closeButton: false,
                        title: 'Added Sucessfully',
                        message: "Request Number : " + result,
                        buttons: {
                            New: {
                                label: "New",
                                className: 'btn-info',
                                callback: function () {
                                    ClearSaveRequest();
                                    window.location.href = "/EmployeeRequest/index";
                                }
                            }

                        }
                    });
                },
                error: function (err) {
                    bootbox.alert("Error saving Request");
                }
            })
        }

    });

});
function ValidSaveRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    $("#EMP_ENAME_ST").require();
    $("#EMP_ENAME_SC").require();
    $("#EMP_ENAME_TH").require();
    //$("#EMP_ENAME_FR").require();
    $("#EMP_ANAME_ST").require();
    $("#EMP_ANAME_SC").require();//dll
    $("#EMP_ANAME_TH").require();
    //$("#EMP_ANAME_FR").require();
    $("#NATIONAL_ID").require();
    if ($("#EMP_RELATION").val() != "1" && $("#EMP_RELATION").val() != "") {
        $('#CARD_ID').require();
    }
     
    //$("#BIRTHDATE").require();
    $("#GENDER").require();
    $("#MOBILE").require();
    //$("#EMAIL").require();
    //$("#START_DATE").require();
    //$("#ADDRESS").require();
    //$("#BRANCH").require();
    $("#EMP_CLASS").require();
    //$("#GLASSES").require();
    //$("#DISEASE").require();
    $("#EMP_RELATION").require();
    if ($("#EMP_IMG").val() == "") { toastr.error("Please,select EMP_IMG."); return { valid: false }; }
    if ($("#NATIONAL_ID").val().length != 14) { toastr.error("Please,Enter correct National Id."); return { valid: false }; }

    if ($("#BIRTHDATE").val() == "") { toastr.error("Please,select Birthdate."); return { valid: false }; }
    if ($("#START_DATE").val() == "") { toastr.error("Please,select start date."); return { valid: false }; }
    return $.validity.end();
}
function ClearSaveRequest() {
    $("#EMP_ENAME_ST").val("");
    $("#EMP_ENAME_SC").val("");
    $("#EMP_ENAME_TH").val("");
    $("#EMP_ENAME_FR").val("");
    $("#EMP_ANAME_ST").val("");
    $("#EMP_ANAME_SC").val("");
    $("#EMP_ANAME_TH").val("");
    $("#EMP_ANAME_FR").val("");
    $("#NATIONAL_ID").val("");
    $('#CARD_ID').val("").change();
    $("#BIRTHDATE").val("");
    $("#GENDER").val('').trigger("change");
    $("#MOBILE").val("");
    $("#EMAIL").val("");
    $("#START_DATE").val("");
    $("#ADDRESS").val("");
    $("#BRANCH").val('').trigger("change");
    $("#GLASSES").val('').trigger("change");
    $("#DISEASE").val('').trigger("change");
    $("#EMP_RELATION").val('').trigger("change");
    $("#EMP_IMG").val("");
    $("#EMP_CLASS").val('').trigger("change");
} 