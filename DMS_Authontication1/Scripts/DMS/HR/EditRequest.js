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

        }
    });
    $('#BIRTHDATE').datepicker({
        changeYear: true,
        yearRange: "-100:+0", // last hundred years
        maxDate: 0,
    });
    $("#EMP_RELATION").change(function () {

        if ($(this).val() != "1" && $(this).val() != "") {
            $("#divCardId").show();
        }
        else {
            $("#divCardId").hide();
        }

    });
    $('#START_DATE').datepicker({
        changeYear: true,
        minDate: 0,
    });
    $("#EditLevel").find('#START_DATE2').datepicker({
        changeYear: true,
        minDate: 0,
    });

    $('#TERMINATE_DATE').datepicker({
        changeYear: true,
        minDate: 0,
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

    $("#EditLevel").find('#CARD_ID').select2({
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
    $("#ChangeEmployeeNumber").find('#CARD_ID').select2({
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

    $("#ChangeEmployeeNumber").find('#CARD_ID').on('select2:selecting', function (event) {
        var splitCard = event.params.args.data.id.split("-");
        $("#lbSplitCard1").text(splitCard[0] + "-" + splitCard[1] + "-");
        $("#lbSplitCard2").text("-" + splitCard[3]);
    });
    $("#Reprint").find('#CARD_ID').select2({
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
    $("#ChangeName").find('#CARD_ID').select2({
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
    $("#Termination").find('#CARD_ID').select2({
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
    $("#Reactive").find('#CARD_ID').select2({
        placeholder: 'Search for a cards',
        minimumInputLength: 5,
        ajax: {
            url: '/EmployeeRequest/GetInActiveEmployess/',
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


    $('#REOPEN_DATE').datepicker({
        minDate: 0
    });

    $("#PRINT_REASON").change(function () {

        if ($(this).val() == "تغير صوره") {
            $("#divPRINT_IMG").show();
        }
        else {
            $("#divPRINT_IMG").hide();
        }

    });
    $('#submit').click(function () {
        var result = ValidSaveEditRequest();
        if (result.valid) {

            if ($("div.tab-content div.active")[0].id != "ChangeEmployeeNumber" &&
                $("div.tab-content div.active")[0].id != "CreateEmployee" &&
                $("div.tab-content div.active")[0].id != "Termination") {//save at validation function#exeption because of ajax call
                $("#submit").attr("disabled", "disabled");
                var Imagedata = new FormData();//pdf

                var Request = {
                    CARD_ID: "",
                    TYPE: "",
                    EMP_ENAME_ST: "",
                    EMP_ENAME_SC: "",
                    EMP_ENAME_FR: "",
                    EMP_ANAME_ST: "",
                    EMP_ANAME_SC: "",
                    EMP_ANAME_TH: "",
                    EMP_ENAME_TH: "",
                    EMP_ANAME_FR: "",
                    NEW_CARD_ID: "",
                    PRINT_REASON: "",
                    PRINT_IMG: "",
                    REOPEN_DATE: "",
                    EMP_CLASS: "",
                    RESON: "",
                    TYP_EMP_UPDATE: ""

                };

                if ($("div.tab-content div.active")[0].id == "ChangeName") {

                    Request.CARD_ID = $("#ChangeName ").find('#CARD_ID').val();
                    Request.EMP_ENAME_ST = $("#ChangeName").find("#EMP_ENAME_ST").val();
                    Request.EMP_ENAME_SC = $("#ChangeName").find("#EMP_ENAME_SC").val();
                    Request.EMP_ENAME_FR = $("#ChangeName").find("#EMP_ENAME_FR").val();
                    Request.EMP_ANAME_ST = $("#ChangeName").find("#EMP_ANAME_ST").val();
                    Request.EMP_ANAME_SC = $("#ChangeName").find("#EMP_ANAME_SC").val();
                    Request.EMP_ANAME_TH = $("#ChangeName").find("#EMP_ANAME_TH").val();
                    Request.EMP_ENAME_TH = $("#ChangeName").find("#EMP_ENAME_TH").val();
                    Request.EMP_ANAME_FR = $("#ChangeName").find("#EMP_ANAME_FR").val();
                    Request.TYPE = "7";
                }
                else if ($("div.tab-content div.active")[0].id == "ChangeEmployeeNumber") {
                    Request.CARD_ID = $("#ChangeEmployeeNumber ").find('#CARD_ID').val();
                    var NewCardId = $("#lbSplitCard1").text() + $("#NEW_CARD_ID").val() + $("#lbSplitCard2").text();
                    Request.NEW_CARD_ID = NewCardId;
                    Request.TYPE = "6";
                }
                else if ($("div.tab-content div.active")[0].id == "Reprint") {
                    Request.CARD_ID = $("#Reprint ").find('#CARD_ID').val();
                    Request.PRINT_REASON = $("#PRINT_REASON").val();
                    Request.TYPE = "4";
                    if ($("#PRINT_REASON").val() == "تغير صوره") {
                        var files = $("#PRINT_IMG").get(0).files;//pdf
                        Request.PRINT_IMG = files[0].name;
                        Imagedata.append("File", files[0]);


                    }
                }
                else if ($("div.tab-content div.active")[0].id == "Reactive") {
                    Request.CARD_ID = $("#Reactive ").find('#CARD_ID').val();
                    Request.TYPE = "5";
                    Request.REOPEN_DATE = $("#REOPEN_DATE").val();
                }
                else if ($("div.tab-content div.active")[0].id == "EditLevel") {
                    Request.CARD_ID = $("#EditLevel ").find('#CARD_ID').val();
                    Request.TYPE = "2";
                    Request.EMP_CLASS = $("#EMP_CLASS").val();
                    Request.RESON = $("#RESON").val();
                    Request.START_DATE = $("#START_DATE2").val(),
                        Request.TYP_EMP_UPDATE = $("#TYP_EMP_UPDATE").val();//pdf
                    var files = $("#TYP_EMP_UPDATE").get(0).files;
                    Request.TYP_EMP_UPDATE = files[0].name;
                    Imagedata.append("File", files[0]);

                }

                //Saving Image
                if (Request.TYP_EMP_UPDATE != "" || Request.PRINT_IMG != "") {
                    $.ajax({
                        url: '/EmployeeRequest/SaveImage/',
                        type: "POST",
                        processData: false,
                        contentType: false,
                        data: Imagedata,
                        success: function (response) {
                            if (response != "No") {
                                if (Request.TYPE == "2") {
                                    Request.TYP_EMP_UPDATE = response;
                                }
                                if (Request.TYPE == "4") {
                                    Request.PRINT_IMG = response;
                                }
                                $.ajax({
                                    type: 'POST',
                                    url: '/EmployeeRequest/SaveEditRequest/',
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
                                                        ClearSaveEditRequest();
                                                        $("#submit").attr("disabled", false);
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
                            else {
                                alert("Error Upload Image");
                            }
                        },
                        error: function (er) {
                            alert("Error Upload Image");
                        }

                    })
                }
                else {
                    $.ajax({
                        type: 'POST',
                        url: '/EmployeeRequest/SaveEditRequest/',
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
                                            ClearSaveEditRequest();
                                            $("#submit").attr("disabled", false);
                                            //window.location.href = "/EmployeeRequest/index";
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
            }
        }
        if ($("div.tab-content div.active")[0].id == "CreateEmployee") {

            var files = $("#UploadExcel").get(0).files;//excel
            if ($("#UploadExcel").get(0).files.length != 0) {
                var exceldata = new FormData();
                exceldata.append("File", files[0]);

                $.ajax({
                    url: '/EmployeeRequest/SendExcelFile/',
                    type: "POST",
                    processData: false,
                    contentType: false,
                    data: exceldata,
                    success: function (response) {
                        if (response == 1)
                            toastr.success("Data Send Susseccfuly ");
                        //response image url instead of session
                    },
                    error: function (er) {
                        //alert("Error Upload Image");
                    }

                })
            }

            else {
                var result = ValidSaveRequest();
                if (result.valid) {

                    $("#submit").attr("disabled", "disabled");

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
                        },
                        error: function (er) {
                            //alert("Error Upload Image");
                        }

                    })

                }
            }
        }

        else if ($("div.tab-content div.active")[0].id == "Termination") {
            var result = ValidTerminationRequest();
            if (result.valid) {
                $("#submit").attr("disabled", "disabled");
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
                                        $("#submit").attr("disabled", false);
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
        }
    });


    $("#EditLevel").find("#CompanyNumber").change(function () {// class levels
        $("#CARD_ID").val(null).trigger('change');
        var compNu = $("#CompanyNumber").val();
        $("#compname").val(compNu);
        $("#txtSearchCard").val(null).trigger('change');
        $.get("/EmployeeRequest/GetClassList",
            { compId: compNu }, function (data) {
                $("#EditLevel").find("#EMP_CLASS").empty();
                $("#EditLevel").find("#EMP_CLASS").append("<option value=''> Select Class</option>")
                $.each(data, function (index, row) {
                    $("#EditLevel").find("#EMP_CLASS").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
                });
            });
    });

    $("#EditLevel").find("#CompanyNumber").map(function () {// class levels
        var compNu = $("#CompanyNumber").val();
        $("#compname").val(compNu);
        $.get("/EmployeeRequest/GetClassList",
            { compId: compNu }, function (data) {
                $("#EditLevel").find("#EMP_CLASS").empty();
                $("#EditLevel").find("#EMP_CLASS").append("<option value=''> Select Class</option>")
                $.each(data, function (index, row) {
                    $("#EditLevel").find("#EMP_CLASS").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
                });
            });
    });


    $("#CompanyNumber").change(function () {// class levels
        $("#CARD_ID").val(null).trigger('change');
        var compNu = $("#CompanyNumber").val();
        $("#compname").val(compNu);
        $("#txtSearchCard").val(null).trigger('change');
        $.get("/EmployeeRequest/GetClassList",
            { compId: compNu }, function (data) {
                $("#EMP_CLASS").empty();
                $("#EMP_CLASS").append("<option value=''> Select Class</option>")
                $.each(data, function (index, row) {
                    $("#EMP_CLASS").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
                });
            });
        $.get("/EmployeeRequest/GetBranchList",
            { compId: compNu }, function (data) {
                $("#BRANCH").empty();
                $("#BRANCH").append("<option value=''>Select Branch</option>")
                $.each(data, function (index, row) {
                    $("#BRANCH").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
                });
            });
    });

    $("#CompanyNumber").map(function () {// class levels
        var compNu = $("#CompanyNumber").val();
        $("#compname").val(compNu);
        $.get("/EmployeeRequest/GetClassList",
            { compId: compNu }, function (data) {
                $("#EMP_CLASS").empty();
                $("#EMP_CLASS").append("<option value=''> Select Class</option>")
                $.each(data, function (index, row) {
                    $("#EMP_CLASS").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
                });
            });
        $.get("/EmployeeRequest/GetBranchList",
            { compId: compNu }, function (data) {
                $("#BRANCH").empty();
                $("#BRANCH").append("<option value=''>Select Branch</option>")
                $.each(data, function (index, row) {
                    $("#BRANCH").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
                });
            });
    });
});
function ValidSaveEditRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    if ($("div.tab-content div.active")[0].id == "ChangeName") {
        if ($("#ChangeName ").find('#CARD_ID').val() == null) { toastr.error("Please,select Card Id."); return { valid: false }; }
        if ($("#ChangeName ").find("#EMP_ENAME_ST").val() == "" || $("#ChangeName ").find("#EMP_ENAME_SC").val() == "" ||
            $("#ChangeName ").find("#EMP_ENAME_TH").val() == "" || $("#ChangeName ").find("#EMP_ANAME_ST").val() == "" ||
            $("#ChangeName ").find("#EMP_ANAME_SC").val() == "" || $("#ChangeName ").find("#EMP_ANAME_TH").val() == "") {
            toastr.error("Please,enter name arabic and english.");
            return { valid: false };
        }
        //$("#ChangeName ").find('#CARD_ID').require();
        //$("#EMP_ENAME_ST").require();
        //$("#EMP_ENAME_SC").require();
        //$("#EMP_ENAME_TH").require();
        //$("#EMP_ENAME_FR").require();
        //$("#EMP_ANAME_ST").require();
        //$("#EMP_ANAME_SC").require();
        //$("#EMP_ANAME_TH").require();
        //$("#EMP_ANAME_FR").require();
    }
    else if ($("div.tab-content div.active")[0].id == "ChangeEmployeeNumber") {
        $("#ChangeEmployeeNumber").find('#CARD_ID').require();
        $("#NEW_CARD_ID").require();

        if ($("#ChangeEmployeeNumber").find('#CARD_ID').val() != null && $('#NEW_CARD_ID').val() != "") {
            var NewCardId = $("#lbSplitCard1").text() + $("#NEW_CARD_ID").val() + $("#lbSplitCard2").text();
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/EmployeeRequest/CardValidation/',
                data: {
                    id: NewCardId
                },
                success: function (r) {
                    if (r.Validation == false) {
                        toastr.info("Card Exsited,Try anther number ");
                        $("#NEW_CARD_ID").val("");
                        return { valid: false };
                    }
                    else {
                        SaveCardChangeName();
                    }
                },
                error: function (err) {
                    alert(err);
                    //location.reload();
                }
            });
        }
    }
    else if ($("div.tab-content div.active")[0].id == "Reprint") {
        $("#Reprint").find('#CARD_ID').require();
        $("#PRINT_REASON").require();
        if ($("#PRINT_REASON").val() == "تغير صوره") {
            if ($("#PRINT_IMG").val() == "") { toastr.error("Please,select PRINT_IMG."); return { valid: false }; }
        }
    }
    else if ($("div.tab-content div.active")[0].id == "Reactive") {
        $("#Reactive").find('#CARD_ID').require();
        if ($("#REOPEN_DATE").val() == "") { toastr.error("Please,select REOPEN_DATE."); return { valid: false }; }

    }
    else if ($("div.tab-content div.active")[0].id == "EditLevel") {
        //$("#EditLevel").find('#CARD_ID').require();
        //$("#EMP_CLASS").require();
        if ($("#EditLevel").find("#RESON").val() == "") { toastr.error("Please,insert reason ."); return { valid: false }; }
        if ($("#EditLevel").find("#EMP_CLASS").val() == "") { toastr.error("Please,select Employee Class ."); return { valid: false }; }

        //$("#EditLevel").find("#RESON").require();
        var splitCard = $("#EditLevel").find('#CARD_ID').val().split("-");
        if (splitCard[1] == $("#EditLevel").find('#EMP_CLASS :selected').text()) {
            toastr.error("Inviled Class.");
            return { valid: false };
        }
        if ($("#EditLevel").find("#TYP_EMP_UPDATE").val() == "") {
            toastr.error("Please,select TYP_EMP_UPDATE.");
            return { valid: false };
        }

    }
    return $.validity.end();
}
function ClearSaveEditRequest() {
    // Termination
    $("#Termination").find('#CARD_ID').val("").change();
    $("#Termination").find("#TERMINATE_DATE").val("");
    $("#Termination").find("#DELIVER_CARD_DATE").val("");

    $("#Termination").find("#DELIVER_CARD_FLAG").val('').trigger("change");

    //Change Name
    $("#ChangeName ").find('#CARD_ID').val("").change();
    $("#EMP_ENAME_ST").val("");
    $("#EMP_ENAME_SC").val("");
    $("#EMP_ENAME_TH").val("");
    $("#EMP_ENAME_FR").val("");
    $("#EMP_ANAME_ST").val("");
    $("#EMP_ANAME_SC").val("");
    $("#EMP_ANAME_TH").val("");
    $("#EMP_ANAME_FR").val("");
    //Change number
    $("#ChangeEmployeeNumber").find('#CARD_ID').val("").change();
    $("#NEW_CARD_ID").val("");
    $("#lbSplitCard1").text("0000-0-");
    $("#lbSplitCard2").text("-0")
    //reprint
    $("#Reprint").find('#CARD_ID').val("").change();
    $("#PRINT_REASON").val('').trigger("change");
    $("#PRINT_IMG").val("");
    //reactive
    $("#Reactive").find('#CARD_ID').val("").change();
    $("#REOPEN_DATE").val("");
    //edit level
    $("#EditLevel").find('#CARD_ID').val("").change();
    $("#EMP_CLASS").val('').trigger("change");
    $("#RESON").val("");
    $("#TYP_EMP_UPDATE").val("");

    $("#EMP_ENAME_ST").val("");
    $("#EMP_ENAME_SC").val("");
    $("#EMP_ENAME_TH").val("");
    $("#EMP_ENAME_FR").val("");
    $("#EMP_ANAME_ST").val("");
    $("#EMP_ANAME_SC").val("");
    $("#EMP_ANAME_TH").val("");
    $("#EMP_ANAME_FR").val("");

    $("#ChangeName").find("#EMP_ENAME_ST").val("");
    $("#ChangeName").find("#EMP_ENAME_SC").val("");
    $("#ChangeName").find("#EMP_ENAME_TH").val("");
    $("#ChangeName").find("#EMP_ENAME_FR").val("");
    $("#ChangeName").find("#EMP_ANAME_ST").val("");
    $("#ChangeName").find("#EMP_ANAME_SC").val("");
    $("#ChangeName").find("#EMP_ANAME_TH").val("");
    $("#ChangeName").find("#EMP_ANAME_FR").val("");

    $("#NATIONAL_ID").val("");
    $('#CARD_ID').val("").change();
    $("#BIRTHDATE").val("");
    $("#GENDER").val('').trigger("change");
    $("#MOBILE").val("");
    $("#EMAIL").val("");
    $("#START_DATE").val("");
    $("#START_DATE2").val("");
    $("#ADDRESS").val("");
    $("#BRANCH").val('').trigger("change");
    $("#GLASSES").val('').trigger("change");
    $("#DISEASE").val('').trigger("change");
    $("#EMP_RELATION").val('').trigger("change");
    $("#EMP_IMG").val("");
    $("#EditLevel").find("#EMP_CLASS").val('').trigger("change");
    $("#EMP_CLASS").val('').trigger("change");

    $('#CARD_ID').val("").change();
    $("#TERMINATE_DATE").val("");
    $("#DELIVER_CARD_DATE").val("");

    $("#DELIVER_CARD_FLAG").val('').trigger("change");
}
function SaveCardChangeName() {
    var Request = {
        CARD_ID: "",
        TYPE: "",
        EMP_ENAME_ST: "",
        EMP_ENAME_SC: "",
        EMP_ENAME_FR: "",
        EMP_ANAME_ST: "",
        EMP_ANAME_SC: "",
        EMP_ANAME_TH: "",
        EMP_ENAME_TH: "",
        EMP_ANAME_FR: "",
        NEW_CARD_ID: "",
        PRINT_REASON: "",
        PRINT_IMG: "",
        REOPEN_DATE: "",
        EMP_CLASS: "",
        RESON: "",
        TYP_EMP_UPDATE: ""

    };
    if ($("div.tab-content div.active")[0].id == "ChangeEmployeeNumber") {
        Request.CARD_ID = $("#ChangeEmployeeNumber ").find('#CARD_ID').val();
        var NewCardId = $("#lbSplitCard1").text() + $("#NEW_CARD_ID").val() + $("#lbSplitCard2").text();
        Request.NEW_CARD_ID = NewCardId;
        Request.TYPE = "6";
    }
    $.ajax({
        type: 'POST',
        url: '/EmployeeRequest/SaveEditRequest/',
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
                            ClearSaveEditRequest();
                            //window.location.href = "/EmployeeRequest/index";
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

function ValidTerminationRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    //$("#CARD_ID").require();
    //$("#DELIVER_CARD_FLAG").require();
    if ($("#TERMINATE_DATE").val() == "") { toastr.error("Please,select Termination date."); return { valid: false }; }
    if ($("#DELIVER_CARD_FLAG").val() == "Yes" && $("#DELIVER_CARD_DATE").val() == "") {
        toastr.error("Please,select Deliver card date."); return { valid: false };
    }
    return $.validity.end();
}

function ValidSaveRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    //$("#EMP_ENAME_ST").require();
    //$("#EMP_ENAME_SC").require();
    //$("#EMP_ENAME_TH").require();
    ////$("#EMP_ENAME_FR").require();
    //$("#EMP_ANAME_ST").require();
    //$("#EMP_ANAME_SC").require();//dll
    //$("#EMP_ANAME_TH").require();
    ////$("#EMP_ANAME_FR").require();
    //$("#NATIONAL_ID").require();
    //if ($("#EMP_RELATION").val() != "1" && $("#EMP_RELATION").val() != "") {
    //    $('#CARD_ID').require();
    //}

    ////$("#BIRTHDATE").require();
    //$("#GENDER").require();
    //$("#MOBILE").require();
    ////$("#EMAIL").require();
    ////$("#START_DATE").require();
    ////$("#ADDRESS").require();
    ////$("#BRANCH").require();
    //$("#EMP_CLASS").require();
    ////$("#GLASSES").require();
    ////$("#DISEASE").require();
    //$("#EMP_RELATION").require();
    if ($("#EMP_RELATION").val() != "1" && $("#EMP_RELATION").val() != "") {
        if ($('#CARD_ID').val() == "") { toastr.error("Please,select Class."); return { valid: false }; }
        /* $('#CARD_ID').require();*/
    }
    if ($("#EMP_ENAME_ST").val() == "" || $("#EMP_ENAME_SC").val() == "" || $("#EMP_ENAME_TH").val() == "" ||
        $("#EMP_ANAME_ST").val() == "" || $("#EMP_ANAME_SC").val() == "" || $("#EMP_ANAME_TH").val() == "") {
        toastr.error("Please,enter name arabic and english.");
        return { valid: false };
    }
    if ($("#GENDER").val() == "") { toastr.error("Please,select Gender."); return { valid: false }; }
    if ($("#EMP_CLASS").val() == "") { toastr.error("Please,select Class."); return { valid: false }; }
    if ($("#EMP_RELATION").val() == "") { toastr.error("Please,select Relation."); return { valid: false }; }
    if ($("#EMP_IMG").val() == "") { toastr.error("Please,select EMP_IMG."); return { valid: false }; }
    if ($("#MOBILE").val().length != 11) { toastr.error("Please,Enter correct Mobile ."); return { valid: false }; }
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
function DownLoadEXCLFunc() {
    window.open('/EmployeeRequest/PrintXLC');
}
function ClearTerminationRequest() {
    $("#Termination").find('#CARD_ID').val("").change();
    $("#Termination").find("#TERMINATE_DATE").val("");
    $("#Termination").find("#DELIVER_CARD_DATE").val("");

    $("#Termination").find("#DELIVER_CARD_FLAG").val('').trigger("change");

}