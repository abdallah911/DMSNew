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
    $('#TERMINATE_DATE').datepicker({
        changeYear: true
    });
    $('#DELIVER_CARD_DATE').datepicker({
        changeYear: true
    });
    $("#DELIVER_CARD_FLAG").change(function () {

        if ($(this).val() == "Yes") {//Indvidual
            $("#divDELIVER_CARD_DATE").show();
        }
        else {
            $("#divDELIVER_CARD_DATE").hide();
        }

    });
    $('#submit').click(function () {
        var result = ValidTerminationRequest();
        if (result.valid) {
            var Request = {
                CARD_ID: $("#CARD_ID").val(),
                TERMINATE_DATE: $("#TERMINATE_DATE").val(),
                DELIVER_CARD_FLAG: $("#DELIVER_CARD_FLAG").val(),
                DELIVER_CARD_DATE: $("#DELIVER_CARD_DATE").val()
            };

            $.ajax({
                type: 'POST',
                url: '/EmployeeRequest/SaveTerminationRequest/',
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
                                    ClearTerminationRequest();
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
function ValidTerminationRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    $("#CARD_ID").require();
    $("#DELIVER_CARD_FLAG").require();
    if ($("#TERMINATE_DATE").val() == "") { toastr.error("Please,select Termination date."); return { valid: false }; }
    if ($("#DELIVER_CARD_FLAG").val() == "Yes" && $("#DELIVER_CARD_DATE").val() == "") {
        toastr.error("Please,select Deliver card date."); return { valid: false };
    }
    return $.validity.end();
}
function ClearTerminationRequest() {
    $('#CARD_ID').val("").change();
    $("#TERMINATE_DATE").val("");
    $("#DELIVER_CARD_DATE").val("");

    $("#DELIVER_CARD_FLAG").val('').trigger("change");

} 