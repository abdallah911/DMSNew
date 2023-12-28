var input = document.getElementById("txtSearchCard");
input.addEventListener("keyup", function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        $('#Search').click();
    }
});
var CompanyPayment;

var AnuualLimit;
var NationalId;
var companid;
var haveClaim = 0;

//
$(function () {
    $("#PrescriptionDate").datepicker({
        maxDate: '0',
        minDate: '-13D',
        dateFormat: 'dd-mm-yy',
    });

    $("#Help").click(function () {
        introJs().start();
    });
    $('#Search').click(function () {
        if ($('#txtSearchCard').val() != "") {
            companid = $('#txtSearchCard').val().split('-')[0];
            if (companid == "8887700") {
                $('#phone').hide();
            }
            else {
                $('#phone').show();
            }
            $("#wait").css("display", "block");
            $.ajax({
                url: '/Pharmacy/AddCard',
                data: { id: $('#txtSearchCard').val() },
                dataType: 'Json',
                success: function (r) {
                    $('#txtSearchCard').attr('disabled', true);
                    var setData = $("#Cards Tbody");
                    setData.empty();
                    for (var i = 0; i < r.length; i++) {
                        if (r[i].INS_START_DATE != null) {
                            var MyDate_String_Value = r[i].INS_START_DATE;
                            var value = new Date
                                (
                                    parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                );
                            var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                        } else {
                            dat = null;
                        }
                        if (r[i].INS_END_DATE != null) {
                            //end date
                            var MyDate_String_Value1 = r[i].INS_END_DATE;
                            var value1 = new Date
                                (
                                    parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                );
                            var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                        }
                        else {
                            dat1 = null;
                        }
                        var data = "<tr >" +
                            "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='Select(this);'></Button>" + "</td>" +
                            "<td>" + r[i].CARD_ID + "</td>" +
                            "<td>" + r[i].EMP_ANAME + "</td>" +
                            "<td>" + r[i].EMP_ENAME + "</td>" +
                            "<td>" + dat + "</td>" +
                            "<td>" + dat1 + "</td>" +
                            "</tr>"
                        var array_name = [];
                        array_name.push(data)
                        setData.append(array_name);

                    }

                    if (r.length == 1) {
                        $('#txtSearchCard').attr('disabled', true);
                        $('#Pending').attr('disabled', false);
                        CardId = $('#txtSearchCard').val();
                        var ArName;
                        for (var i = 0; i < r.length; i++) {
                            ArName = r[i].EMP_ANAME;
                        }
                        var EndDate = dat1;
                        var today = new Date();
                        var dd = today.getDate();
                        var mm = today.getMonth() + 1; //January is 0!
                        var yyyy = today.getFullYear();
                        var CurrentDate = new Date(yyyy, mm, dd);
                        if (EndDate != null) {
                            newDate = EndDate.split('/').reverse().join('.');
                        } else {
                            newDate = "";
                        }
                        var date = new Date(newDate);
                        var companyId = [];
                        companyId = CardId.split('-', 1);
                        CompId = companyId[0];
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: '/Pharmacy/CkeckCompanyClosedorOpen',
                            data: { id: CompId, CardId: CardId },
                            success: function (returndata) {
                                if (returndata.ok) {
                                    $("#wait").css("display", "none");
                                    $('#txtSearchCard').val(CardId);
                                    $('#CardsModal').modal('hide');

                                    $.ajax({
                                        type: "POST",
                                        dataType: "json",
                                        url: '/Pharmacy/GetCompActivation',
                                        data: { id: CompId, CardId: CardId },
                                        success: function (returndata) {
                                            if (returndata.ok) {
                                                if (returndata.data == "Yes") {
                                                    $("#wait").css("display", "none");
                                                    $('#txtSearchCard').val(CardId);
                                                    $('#compEmp_EMP_ANAME').val(ArName);
                                                    $('#compEmp_INS_END_DATE').val(EndDate);
                                                    //AddNationalId();
                                                    $('#CardsModal').modal('hide');
                                                    $.ajax({
                                                        type: "POST",
                                                        dataType: "json",
                                                        url: '/Labs/CellingAmount',
                                                        data: {
                                                            id: CardId,
                                                            ServiceCode: '11201'
                                                        },
                                                        success: function (r) {
                                                            if (r.Validation == false) {
                                                                alert(r.Message);
                                                                window.location.reload();
                                                            } else {
                                                                $('#ddEmp_CEILING_PERT').val(r.CeilingPert);
                                                                AnuualLimit = r.Limit;
                                                                $('#IsFamily').val(r.IsFamily);
                                                                $('#IsPool').val(r.IsPool);
                                                                $("#Co_insurance_INSURANCE_DAY_LAB").val(r.CoInsurancelimit.INSURANCE_DAY_LAB);
                                                                Calculation();
                                                                //Get ClaimNumber

                                                                $.ajax({
                                                                    type: "POST",
                                                                    dataType: "json",
                                                                    url: '/Labs/HaveClaim',
                                                                    data: { id: CardId },
                                                                    success: function (Code) {
                                                                        if (Code == "0") {
                                                                            haveClaim = 1;
                                                                            alert("هذا العميل لديه موافقة يرجي ادخال رقم الموافقة داخل " + "Calim Number");
                                                                        }
                                                                    }
                                                                });
                                                            }
                                                        },
                                                        error: function (err) {
                                                            alert("Company Annual Amount");
                                                            location.reload();
                                                        }
                                                    });
                                                }
                                                else {
                                                    $("#wait").css("display", "none");
                                                    bootbox.dialog({
                                                        title: 'Alert!',
                                                        message: returndata.message,
                                                        buttons: {
                                                            Ok: {
                                                                label: "Ok",
                                                                className: 'btn-info',
                                                                callback: function () {
                                                                    ClearCardData();
                                                                }
                                                            }
                                                        }
                                                    });
                                                }
                                            }
                                            else {
                                                if (returndata.data == "Hold") {
                                                    $("#wait").css("display", "none");
                                                    bootbox.dialog({
                                                        title: 'Alert!',
                                                        message: returndata.message,
                                                        buttons: {
                                                            Ok: {
                                                                label: "Ok",
                                                                className: 'btn-info',
                                                                callback: function () {
                                                                    ClearCardData();
                                                                }
                                                            }
                                                        }
                                                    });
                                                }
                                                else if (returndata.data == "Expire") {
                                                    $("#wait").css("display", "none");
                                                    bootbox.dialog({
                                                        title: 'Alert!',
                                                        message: returndata.message,
                                                        buttons: {
                                                            Ok: {
                                                                label: "Ok",
                                                                className: 'btn-info',
                                                                callback: function () {
                                                                    ClearCardData();
                                                                }
                                                            }
                                                        }
                                                    });
                                                }
                                                else
                                                    bootbox.alert('failed  company activation , please check your internet connection ');
                                            }
                                        }
                                    });
                                }
                                else {
                                    $("#wait").css("display", "none");
                                    $('#txtSearchCard').val(CardId);
                                    $('#CardsModal').modal('hide');
                                    bootbox.alert('برجاء الرجوع الى ادارة الشركة التابعة لسيادتكم ..');
                                }
                            }
                        });

                    }

                    else if (r.length == 0) {
                        alert(' Invalid Card Number ');
                        $('#CardsModal').modal('hide');
                        $("#wait").css("display", "none");

                    }

                    else {
                        $('#Cards').DataTable();
                        $('#CardsModal').modal();
                        $("#wait").css("display", "none");

                    }
                },
                error: function () {
                    bootbox.alert("Error Data  Retrieve");
                    $("#wait").css("display", "none");
                }

            });
        }
        else {
            bootbox.alert("Please Insert Card Id");
        }
    });

    $('#Pending').click(function () {

        location.replace("/Rays/Pending2?id=" + CardId);
    });

    $("#PrescriptionDate").datepicker({
        maxDate: '0',
        minDate: '-6D',
        dateFormat: 'dd-mm-yy',
    });
    $("#ddlSpeciality").change(function () {
        $("#wait").css("display", "block");
        $.ajax({
            type: 'POST',
            url: '/Pharmacy/getDiag/',
            dataType: 'json',
            data: { id: $("#ddlSpeciality").val() },
            success: function (r) {
                $('#ddlDiagnoises').empty();
                var result = [];
                for (var i = 0; i < r.length; i++) {
                    var current = {};
                    current.id = r[i].Code;
                    current.text = r[i].Name;
                    result.push(current);
                }
                $('#ddlDiagnoises').select2({
                    data: result
                })
                $("#wait").css("display", "none");

            },
            error: function (ex) {
                bootbox.alert('Failed to retrieve Diagnoses.');
                $("#wait").css("display", "none");

            }

        });
    });
    $("#ddlSpeciality").map(function () {
        $.ajax({
            type: 'POST',
            url: '/Pharmacy/getDiag/',
            dataType: 'json',
            data: { id: $("#ddlSpeciality").val() },

            success: function (r) {
                $('#ddlDiagnoises').empty();
                var result = [];
                for (var i = 0; i < r.length; i++) {
                    var current = {};
                    current.id = r[i].Code;
                    current.text = r[i].Name;
                    result.push(current);
                }
                $('#ddlDiagnoises').select2({
                    data: result
                })

                $("#wait").css("display", "none");

            },
            error: function (ex) {
                bootbox.alert('Failed to retrieve Diagnoses.');
            }

        });
    });
    $("#ddlSpeciality").select2();


    // var RayArray = [];
    $("#AddRay").select2({
        placeholder: "Select a Ray",
        //ariaselected:false,
        ajax: {
            url: 'GetList',
            dataType: 'json',
            data: function (params) {
                var query = {
                    sEcho: params.page || 1,
                    sSearch: params.term,
                }
                return query;
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                var result = [];
                for (var i = 0; i < data.aaData.length; i++) {
                    var current = {};
                    current.id = data.aaData[i].SERV_CODE;
                    // current.aria-selected=false;
                    current.text = data.aaData[i].SERV_ANAME;
                    result.push(current);
                }
                return {
                    results: result,
                    pagination: {
                        more: (params.page * 10) < data.count_filtered
                    }
                };
            }
        }
    });
    $('#AddRay').on('select2:selecting', function (event) {
        if ($('#txtSearchCard').val() != '') {
            if (companid == "8887700") {
                if ($('#ddlDiagnoises').val().length != 0) {
                    if (haveClaim == 1) {
                        if ($('#ClaimNumber').val() != "") {
                            var ze = document.getElementById('ClaimNumber').value;
                            $.ajax({
                                url: '/Labs/GetCardCode',
                                data: { id: $('#txtSearchCard').val(), calimNumber: ze },
                                dataType: 'Json',
                                success: function (Code) {
                                    if (Code == "0") {
                                        SelectRayCompany(event);
                                    }
                                    else {
                                        bootbox.alert("غير مسموح اجراء أشعة لهذا الكارت من خلالكم");
                                        event.preventDefault();
                                    }
                                },
                                error: function () {
                                    bootbox.alert("Too many data  Retrieve more specific characters solve the problem and check your internet connection");
                                    $("#wait").css("display", "none");
                                }
                            });
                        }
                        else {
                            toastr.info("Please enter Claim Number");
                            event.preventDefault();
                        }
                    }
                    else {
                        SelectRayCompanyPending(event);
                    }
                }
                else {
                    toastr.info("Please insert Diagnoise Date");
                    event.preventDefault();
                }
            }
            else {
                if ($('#PhoneNumber').val() != '' && $("#PhoneNumber").val().length == 11) {
                    if ($('#ddlDiagnoises').val().length != 0) {
                        SelectRay(event);
                    }
                    else {
                        toastr.info("Please insert Diagnoise Date");
                        event.preventDefault();
                    }
                }
                else {
                    toastr.info("Please insert Phone number");
                    event.preventDefault();
                }
            }

        }

        else {
            toastr.info("Please Insert Card Number");
            event.preventDefault();
        }
    });
    $('#AddRay').on("select2:unselecting", function (event) {
        $('#Ray tbody tr').each(function () {
            var row = $(this);
            if (parseInt(row.find("TD").eq(0).html()) == parseInt(event.params.args.data.id)) {
                Remove(row, event);
            }
        });

    });

    function phonenumber(inputtxt) {
        var phoneno = /01\d{9}$/;
        if (inputtxt.match(phoneno)) {
            return true;
        }
        else {
            return false;
        }

    }
    function number(v) {
        if (isNaN(v)) {
            return false;
        } else {
            return true;
        }
    }

    var approval = " ";
    $('#submit').click(function () {
        var Id; var Mediciens = new Array();
        $("#Ray TBODY TR").each(function () {
            var row = $(this);
            var Medicien = {};

            Medicien.RoshitaID = Id;
            Medicien.MedicienCode = row.find("TD").eq(0).html();
            Medicien.MedicienName = row.find("TD").eq(1).html();
            Medicien.Amount = parseFloat(("TD", row).find(".Amount").val());
            Medicien.PaymentGroup = row.find("TD").eq(3).html();
            Mediciens.push(Medicien);
        });
        if ($('#txtSearchCard').val() != "") {
            if (companid == "8887700") {
                $('#PhoneNumber').val("01000000001");
            }
            if ($('#PhoneNumber').val() != "" && $("#PhoneNumber").val().length == 11) {
                if ($('#PrescriptionDate').val() != '') {
                    var TotalDuration = $("TD", row).find(".TotalDuration").val();
                    var row = $(this); if (row.val != "") {
                        if (TotalDuration != 0) {
                            if (Mediciens.length != 0) {
                                $("#submit").attr("disabled", "disabled");

                                $.ajax({
                                    type: 'POST',
                                    url: '/Rays/SAVE/',
                                    dataType: 'Json',

                                    data: {
                                        CardId: $('#txtSearchCard').val(),
                                        CompanyPercent: $('#ddEmp_CEILING_PERT').val(),
                                        Limit: $('#Co_insurance_INSURANCE_DAY_LAB').val(),
                                        Speciality: $('#ddlSpeciality option:selected').text(),
                                        Diagnose1: $('#Diagnoise').val(),
                                        //calculation
                                        TotalValue: $('#txtTotalInvoice').val(),
                                        PersonPayment: $('#txtTotalCopayment').val(),
                                        CompanyPayment: $('#txtValueCredit').val(),
                                        OverInsurance: $('#txtOverInsurance').val(),
                                        Cash: $('#txtValueCash').val(),
                                        PhoneNumber: $('#PhoneNumber').val(),
                                        ClaimNumber: $('#ClaimNumber').val(),
                                        Diagnose1: $('#Comments').val(),
                                        Diagnose2: NationalId,
                                        createdby: $('#ddlUsers').val() == undefined ? null : $('#ddlUsers :selected').val(),
                                        IsFamily: $('#IsFamily').val() == '' ? null : $('#IsFamily').val(),
                                        IsPool: $('#IsPool').val() == '' ? null : $('#IsPool').val(),


                                    },
                                    success: function (Oracle_Id) {
                                        //var ReportId = r;

                                        //Date.prototype.yyyymmdd = function () {
                                        //    var mm = this.getMonth() + 1; // getMonth() is zero-based
                                        //    var dd = this.getDate();
                                        //    return [(dd > 9 ? '' : '0') + dd,
                                        //    (mm > 9 ? '' : '0') + mm,
                                        //    this.getFullYear()
                                        //    ].join('');
                                        //};
                                        //var date = new Date();
                                        //d = date.yyyymmdd()
                                        //r = d + r;
                                        //approval = r;
                                        //bootbox.alert("Roshita ID : " + r);
                                        bootbox.dialog({
                                            closeButton: false,
                                            title: 'Added Sucessfully',
                                            message: "Roshita ID : " + Oracle_Id,
                                            buttons: {
                                                Print: {
                                                    Rayel: "Print",
                                                    className: 'btn-info',
                                                    callback: function () {
                                                        window.location.reload();
                                                        window.open('/Rays/ControlPenelReport?id=' + Oracle_Id);
                                                    }
                                                },
                                                New: {
                                                    Rayel: "New",
                                                    className: 'btn-info',
                                                    callback: function () {
                                                        window.location.reload();
                                                    }
                                                }

                                            }
                                        });
                                    },
                                    error: function (err) {
                                        bootbox.alert("Error Roshita");
                                        $("#submit").attr("disabled", false);
                                    }
                                }).done(function () {

                                    $.ajax({
                                        type: 'POST',
                                        url: '/Rays/SaveMediciens/',
                                        dataType: 'Json',
                                        contentType: "application/json; charset=utf-8",
                                        data: JSON.stringify(Mediciens),
                                        success: function (r) {
                                            $("#submit").attr("disabled", false);
                                        },
                                        error: function (err) {
                                            $("#submit").attr("disabled", false);
                                            bootbox.alert("Error Medicien");
                                        }
                                    });
                                    var SelectedDiagnosisList = $('#ddlDiagnoises').select2('data');
                                    var DiagnosisList = [];
                                    for (var i = 0; i < SelectedDiagnosisList.length; i++) {
                                        var current = {};
                                        current.DIAG_CODE = SelectedDiagnosisList[i].id;
                                        current.DIAG_ANAME = SelectedDiagnosisList[i].text;
                                        DiagnosisList.push(current);
                                    }
                                    //Diagnoises
                                    $.ajax({
                                        type: 'POST',
                                        url: '/Rays/SaveDiagnoises/',
                                        dataType: 'Json',
                                        contentType: "application/json; charset=utf-8",
                                        data: JSON.stringify(DiagnosisList),
                                        success: function (r) {
                                        },
                                        error: function (err) {
                                            bootbox.alert("Error Diagnoises,please check your connection");
                                            $("#submit").attr("disabled", false);
                                        }

                                    });

                                });
                            } else {
                                bootbox.alert("Please Insert Rays");
                            }
                        }

                        else {
                            bootbox.alert("Invalid Total Duration");
                        }
                    }
                }
                else {
                    toastr.info("Please insert Prescription Date");
                    event.preventDefault();
                }
            } else {
                bootbox.alert("PleaseInsert phoneNumber ");
            }
        }
        else
            bootbox.alert("Please Insert Card ID");
    });
});

function DatePickerModel(flag) {
    if (flag == 0) {
        $("#PrescriptionDate").datepicker({
            maxDate: '0',
            minDate: '-13D',
            dateFormat: 'dd-mm-yy',
        });
    }
    else {
        $("#PrescriptionDate").datepicker({
            dateFormat: 'dd-mm-yy'
        });
    }
}

function SelectRayCompanyPending(event) {
    var Code = event.params.args.data.id
    $("#wait").css("display", "block");
    var Name;
    var Amount;
    var Group;

    //var edit = 0;
    $.ajax({
        type: 'Get',
        url: 'GetRayByCode/',
        dataType: 'json',
        data: { code: Code },
        success: function (r) {
            RayCode = r.SERV_CODE;
            Name = r.SERV_ANAME;
            Group = r.GRUOP_TYPE;
            Amount = r.SERV_AMOUNT;
            $("#wait").css("display", "none");

        },
        error: function (ex) {
            bootbox.alert('Failed to retrieve Ray Data.');
        }

    }).done(function () {
        Group = "Pending";
        AppendRow();
        Calculation();

    });

    function AppendRow() {
        var tBody = $("#Ray > TBODY")[0];
        var row = tBody.insertRow(-1);
        var cell = $(row.insertCell(-1));
        cell.html(Code);
        cell = $(row.insertCell(-1));
        cell.html(Name);
        cell = $(row.insertCell(-1));
        //cell.html(Amount);
        var AmountText = $("<input  />");
        AmountText.attr("type", "number");
        AmountText.attr("min", "1");
        AmountText.addClass("form-control");
        AmountText.addClass("Amount");
        AmountText.attr("onkeyup", "Calculation();");
        AmountText.val(Amount);
        cell.append(AmountText);
        cell = $(row.insertCell(-1));
        cell.html(Group);
        toastr.success('Added successfully ');
        $("#wait").css("display", "none");
    }
}
function SelectRayCompany(event) {
    var Code = event.params.args.data.id
    $("#wait").css("display", "block");
    var Name;
    var Amount;
    var Group;

    //var edit = 0;
    $.ajax({
        type: 'Get',
        url: 'GetRayByCode/',
        dataType: 'json',
        data: { code: Code },
        success: function (r) {
            RayCode = r.SERV_CODE;
            Name = r.SERV_ANAME;
            Group = r.GRUOP_TYPE;
            Amount = r.SERV_AMOUNT;
            $("#wait").css("display", "none");

        },
        error: function (ex) {
            bootbox.alert('Failed to retrieve Ray Data.');
        }

    }).done(function () {
        Group = "Accepted";
        AppendRow();
        Calculation();

    });

    function AppendRow() {
        var tBody = $("#Ray > TBODY")[0];
        var row = tBody.insertRow(-1);
        var cell = $(row.insertCell(-1));
        cell.html(Code);
        cell = $(row.insertCell(-1));
        cell.html(Name);
        cell = $(row.insertCell(-1));
        //cell.html(Amount);
        var AmountText = $("<input  />");
        AmountText.attr("type", "number");
        AmountText.attr("min", "1");
        AmountText.addClass("form-control");
        AmountText.addClass("Amount");
        AmountText.attr("onkeyup", "Calculation();");
        AmountText.val(Amount);
        cell.append(AmountText);
        cell = $(row.insertCell(-1));
        cell.html(Group);
        toastr.success('Added successfully ');
        $("#wait").css("display", "none");
    }
}
function SelectRay(event) {
    var Code = event.params.args.data.id
    $("#wait").css("display", "block");
    var Name;
    var Amount;
    var Group;

    //var edit = 0;
    $.ajax({
        type: 'Get',
        url: 'GetRayByCode/',
        dataType: 'json',
        data: { code: Code },
        success: function (r) {
            RayCode = r.SERV_CODE;
            Name = r.SERV_ANAME;
            Group = r.GRUOP_TYPE;
            Amount = r.SERV_AMOUNT;
            $("#wait").css("display", "none");

        },
        error: function (ex) {
            bootbox.alert('Failed to retrieve Ray Data.');
        }

    }).done(function () {
        //check Daily
        $.ajax({
            dataType: "json",
            url: '/Rays/CheckDaily',
            data: {
                id: $('#txtSearchCard').val(),
                code: Code
            }
            ,
            error: function (r) {
                bootbox.alert("This Ray had been exchanged Today ");
            },
            success: function (r) {
                if (r != 1) {

                    // Not exchanged today
                    $('#Ray TBODY TR').each(function () {

                        var row = $(this);
                        var table = $("#Ray")[0];
                        if (parseInt(row.find("TD").eq(0).html()) == parseInt(Code)) {
                            table.deleteRow(row[0].rowIndex);
                        }
                    });

                    if (Group == "NO") {
                        $.ajax({
                            dataType: "json",
                            url: '/Pharmacy/CheckVip',
                            data: {
                                id: $('#txtSearchCard').val()
                            },
                            success: function (r) {
                                if (r.IsVip == 1) {
                                    Group = "Accepted";
                                    AppendRow();
                                    Calculation();
                                }
                                else {
                                    var dialog = bootbox.dialog({
                                        title: 'This Rays is Not Covered!',
                                        message: "<p>Pay method?</p>",
                                        onEscape: function () {
                                            RemoveSelection(Code);
                                        },
                                        buttons: {
                                            Cash: {
                                                Rayel: "Cash",
                                                className: 'btn-info',
                                                callback: function () {
                                                    Group = "Cash";
                                                    AppendRow();
                                                    Calculation();
                                                }
                                            },
                                            Approval: {
                                                Rayel: "Approval",
                                                className: 'btn-info',
                                                callback: function () {
                                                    Group = "Approval";
                                                    AppendRow();
                                                    Calculation();
                                                }
                                            },
                                            Pending: {
                                                Rayel: "Pending",
                                                className: 'btn-info',
                                                callback: function () {
                                                    Group = "Pending";
                                                    AppendRow();
                                                    Calculation();
                                                }
                                            }
                                        }
                                    });
                                }
                            }
                        });
                    }
                    else {
                        AppendRow();
                        Calculation();
                    }

                    $('#RaysModal').modal('hide');

                } else {
                    bootbox.alert("This Ray had been exchanged Today ");

                }
            }
        });



    });

    function AppendRow() {
        var tBody = $("#Ray > TBODY")[0];
        var row = tBody.insertRow(-1);
        var cell = $(row.insertCell(-1));
        cell.html(Code);
        cell = $(row.insertCell(-1));
        cell.html(Name);
        cell = $(row.insertCell(-1));
        //cell.html(Amount);
        var AmountText = $("<input  />");
        AmountText.attr("type", "number");
        AmountText.attr("min", "1");
        AmountText.addClass("form-control");
        AmountText.addClass("Amount");
        AmountText.attr("onkeyup", "Calculation();");
        AmountText.val(Amount);
        cell.append(AmountText);
        cell = $(row.insertCell(-1));
        cell.html(Group);
        toastr.success('Added successfully ');
        $("#wait").css("display", "none");
    }
}

function Remove(button, event) {
    var row = $(button).closest("TR");
    var name = $("TD", row).eq(0).html();
    bootbox.confirm("Do you want to delete: " + name, function (result) {
        if (result) {
            var row = $(button).closest("TR");
            var table = $("#Ray")[0];
            table.deleteRow(row[0].rowIndex);
            $('#AddDiagnoise').attr('disabled', false);
            $('#RemoveDiagnoise').attr('disabled', false);
            Calculation();

        } else {
            var values = $('#AddRay').val();
            values.push(name);
            $('#AddRay').val(values).change();

        }
    });

}
function RemoveSelection(Code) {
    var values = $('#AddRay').val();
    if (values) {
        var i = values.indexOf(Code);
        if (i >= 0) {
            values.splice(i, 1);
            $('#AddRay').val(values).change();
        }
    }
}
function Calculation() {
    if ($("#Ray >tbody TR").length != 0) {
        var sum = 0;
        var sumCash = 0;
        $('#Ray TBODY TR').each(function () {
            var row = $(this);
            if (parseFloat(("TD", row).find(".Amount").val()) < 1) {
                ("TD", row).find(".Amount").val(1);
            }
            if (row.find("TD").eq(3).html() != "Pending") {
                sum += parseFloat(("TD", row).find(".Amount").val());
                if (row.find("TD").eq(3).html() == "Cash")
                    sumCash += parseFloat(("TD", row).find(".Amount").val());
            }
        });
        $("#txtTotalInvoice").val(sum.toFixed(2));
        $('#txtCash').val(sumCash.toFixed(2));
        $('#txtOverInsurance').val("0.00");
        var CompanyPayment = $("#ddEmp_CEILING_PERT").val();
        var person = parseInt(100 - CompanyPayment);//percentage
        //total-cash
        var total = parseFloat($('#txtTotalInvoice').val()) - parseFloat(sumCash);
        var cash = parseInt($('#txtCash').val());
        var ValueCredit = 0;
        var limit_Daily = parseFloat($("#Co_insurance_INSURANCE_DAY_LAB").val());

        if (limit_Daily > AnuualLimit || limit_Daily == 0) {
            limit_Daily = AnuualLimit;
        }
        if (limit_Daily != 0 && co != 0) {
            ValueCredit = (total * (CompanyPayment / 100)).toFixed(2);
            if ((limit_Daily * (CompanyPayment / 100)) <= (ValueCredit)) {
                $('#txtTotalCopayment').val((limit_Daily * (person / 100)).toFixed(2));
                limit_Daily = (limit_Daily * (CompanyPayment / 100)).toFixed(2);
                $('#txtValueCredit').val(parseFloat(limit_Daily).toFixed(2));
                $('#txtOverInsurance').val((parseFloat(total) - parseFloat(limit_Daily) - parseFloat($('#txtTotalCopayment').val())).toFixed(2));
            }
            else if ((limit_Daily * (CompanyPayment / 100)) > (ValueCredit)) {
                $('#txtValueCredit').val((total * (CompanyPayment / 100)).toFixed(2));
                $('#txtTotalCopayment').val((total * (person / 100)).toFixed(2));
            }
        }
        else {
            $('#txtValueCredit').val(((total) * (CompanyPayment / 100)).toFixed(2));
            $('#txtTotalCopayment').val(((total) * (person / 100)).toFixed(2));
        }
        $('#txtValueCash').val((sumCash + parseFloat($('#txtTotalCopayment').val()) + parseFloat($('#txtOverInsurance').val())).toFixed(2));
    } else {
        $('#txtTotalInvoice').val('');
        $('#txtTotalCopayment').val('');
        $('#txtValueCredit').val('');
        $('#txtOverInsurance').val('');
        $('#txtCash').val('');
        $('#txtValueCash').val('');
    }


}

function ClearCardData() {
    $('#txtSearchCard').val('');
    $('#PhoneNumber').val('');
    //$('#contractComp_C_ANAME').val('');
    $('#compEmp_EMP_ANAME').val('');
    $('#compEmp_INS_END_DATE').val('');
    $('#Co_insurance_INSURANCE_DAY_Ray').val('');
    //$('#Co_insurance_INSURANCE_MONTH_Ray').val('');
    $('#ddEmp_CEILING_PERT').val('');
    $('#Comments').val('');
    $("#submit").attr("disabled", false);
}
function ClearRayData() {
    $("#Ray >tbody").empty();
    $("#AddRay").empty();
    $("#ddlDiagnoises").val(null).change();

    $('#txtTotalInvoice').val('');
    $('#txtTotalCopayment').val('');
    $('#txtValueCredit').val('');
    $('#txtOverInsurance').val('');
    $('#txtCash').val('');
    $('#txtValueCash').val('');
    $("#submit").attr("disabled", false);
}
function AddNationalId() {
    //add National Id
    bootbox.prompt({
        title: "Please,Enter Patient National ID  : ",
        centerVertical: true,
        closeButton: false,
        //required: true,
        //cancel: "Reset",
        callback: function (result) {
            if (result === null) {
                window.location = '/Rays/Ray';
                return true;
            }
            if (result === "" || result.length != 14 || isNaN(result)) {
                toastr.error("Invalid Value");
                return false;
            } else {
                if (CompId.startsWith("10") || CompId.startsWith("70"))

                    toastr.info("جميع خدمات كرونا غير مغطاة و يجب محاسبة المريض نقدا");
                //$("#wait").css("display", "block");
                NationalId = result;
                return true;

            }
            return false;

        }
    });
}

//$(function () {
//    $("#btnClaim").click(function () {
//        // if (CardId != undefined) { }
//        window.location.replace('/Rays/index?id=' + CardId);
//    });
//    $('#Search').click(function () {
//        $.ajax({
//            url: '/Pharmacy/AddCard',
//            data: { id: $('#txtSearchCard').val() },
//            dataType: 'Json',
//            success: function (r) {

//                var setData = $("#Cards Tbody");
//                setData.empty();

//                for (var i = 0; i < r.length; i++) {
//                    if (r[i].INS_START_DATE != null) {
//                        var MyDate_String_Value = r[i].INS_START_DATE;
//                        var value = new Date
//                            (
//                            parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
//                            );
//                        var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
//                    } else {
//                        dat = null;
//                    }
//                    if (r[i].INS_END_DATE != null) {
//                        //end date
//                        var MyDate_String_Value1 = r[i].INS_END_DATE;
//                        var value1 = new Date
//                            (
//                            parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
//                            );
//                        var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
//                    }
//                    else {
//                        dat1 = null;
//                    }
//                    var data = "<tr >" +
//                        "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='Select(this);'></Button>" + "</td>" +
//                        "<td>" + r[i].CARD_ID + "</td>" +
//                        "<td>" + r[i].EMP_ANAME + "</td>" +
//                        "<td>" + r[i].EMP_ENAME + "</td>" +
//                        "<td>" + dat + "</td>" +
//                        "<td>" + dat1 + "</td>" +
//                        "</tr>"
//                    var array_name = [];
//                    array_name.push(data)
//                    setData.append(array_name);

//                }
//                if (r.length == 1) {
//                    $('#Pending').attr('disabled', false);

//                    CardId = $('#txtSearchCard').val();
//                    debugger;
//                    var ArName;
//                    for (var i = 0; i < r.length; i++) {
//                        ArName = r[i].EMP_ANAME;
//                    }
//                    var EndDate = dat1;
//                    var today = new Date();
//                    var dd = today.getDate();
//                    var mm = today.getMonth() + 1; //January is 0!
//                    var yyyy = today.getFullYear();
//                    //  var CurrentDate = new Date(yyyy, mm, dd);
//                    if (EndDate != null) {
//                        newDate = EndDate.split('/').reverse().join('.');
//                    } else {
//                        newDate = "";
//                    }
//                    var date = new Date(newDate);
//                    today = mm + '/' + dd + '/' + yyyy;
//                    var CurrentDate = new Date(Date.parse(today));
//                    if (date > CurrentDate) {
//                        $('#txtSearchCard').val(CardId);
//                        $('#compEmp_EMP_ANAME').val(ArName);
//                        $('#compEmp_INS_END_DATE').val(EndDate);

//                        //  $('#CardsModal').modal('hide');

//                        $.ajax({
//                            type: "POST",
//                            dataType: "json",
//                            url: '/Pharmacy/GetCompName',
//                            data: { id: CardId },

//                            success: function (returndata) {
//                                if (returndata.ok) {

//                                    $("#contractComp_C_ANAME").val(returndata.data.C_ANAME);

//                                }
//                                else {
//                                    bootbox.alert(' Error Company Name ');
//                                }
//                            }
//                        });

//                        //ddlLimit
//                        $.ajax({
//                            type: "POST",
//                            dataType: "json",
//                            url: '/Rays/GetLimit',
//                            data: { id: CardId },
//                            success: function (returndata) {
//                                if (returndata.ok) {
//                                    limit_Daily = returndata.limit.INSURANCE_DAY_RAY;
//                                    $("#Co_insurance_INSURANCE_DAY_Ray").val(limit_Daily);
//                                    Limit_Monthly = returndata.limit.INSURANCE_MONTH_RAY;
//                                    $("#Co_insurance_INSURANCE_MONTH_Ray").val(Limit_Monthly);
//                                }

//                                else {
//                                    bootbox.alert(' Error Patient Limit ');
//                                }
//                            }
//                        });


//                        $.ajax({
//                            type: "POST",
//                            dataType: "json",
//                            url: '/Pharmacy/CellingAmount',
//                            data: {
//                                id: CardId,
//                                ServiceCode: '11204'
//                            },
//                            success: function (r) {
//                                if (r.Validation == false) {
//                                    toastr.info(r.Message);
//                                    ClearCardData();
//                                } else {
//                                    $('#ddEmp_CEILING_PERT').val(r.CeilingPert);
//                                    AnuualLimit = r.Limit;
//                                    //   $("#no_data_yet").val(100 - $("#ddEmp_CEILING_PERT").val());

//                                }
//                            },
//                            error: function (err) {
//                                alert("Company Annual Amount");
//                                location.reload();
//                            }
//                        });
//                        Calculation();
//                    }
//                    else {
//                        bootbox.dialog({
//                            title: 'Alert!',
//                            message: ' Expired Card',// "Roshita ID : " + r,
//                            buttons: {
//                                Ok: {
//                                    Rayel: "Ok",
//                                    className: 'btn-info',
//                                    callback: function () {
//                                        location.reload();
//                                    }
//                                }
//                            }
//                        });
//                    }

//                    $('#txtTotalInvoice').val(' ');
//                    $('#txtTotalCopayment').val(' ');
//                    $('#txtValueCredit').val(' ');
//                    $('#txtOverInsurance').val(' ');
//                    $('#txtValueCash').val(' ');
//                    $('#txtCash').val(' ');
//                }
//                else if (r.length == 0) {
//                    alert(' Invalid Card Number ');
//                    $('#CardsModal').modal('hide');
//                }

//                else {
//                    $('#Cards').DataTable();
//                    $('#CardsModal').modal();
//                }
//            },
//            error: function () {
//                alert("Too Many Data To Retrieve");
//            }

//        });

//    });
//    $('#Pending').click(function () {
//        location.replace("/Pharmacy/Pending?id=" + CardId);
//    });
//    $("#PrescriptionDate").datepicker({
//        maxDate: '0',
//        minDate: '-6D',
//        dateFormat: 'dd-mm-yy',
//    });
//    $("#ddlSpeciality").change(function () {
//        $('#Diagnoise').val("");
//        $.ajax({
//            type: 'POST',
//            url: '/Pharmacy/getDiag/',
//            dataType: 'json',
//            data: { id: $("#ddlSpeciality").val() },

//            success: function (r) {
//                $('#Diagnoises').dataTable().fnDestroy();
//                var setData = $("#Diagnoises Tbody");
//                setData.empty();

//                for (var i = 0; i < r.length; i++) {
//                    var data = "<tr >" +
//                        "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectDiagnoise(this);'></Button>" + "</td>" +
//                        "<td>" + r[i].Code + "</td>" +
//                        "<td>" + r[i].Name + "</td>" +
//                        "</tr>"
//                    setData.append(data);
//                }
//                $('#Diagnoises').DataTable();
//            },
//            error: function (ex) {
//                alert('Failed to retrieve Diagnoses.');
//            }

//        });
//    })
//    // map //
//    $("#ddlSpeciality").map(function () {

//        //$("#ddlDiag1").empty();
//        $.ajax({
//            type: 'POST',
//            url: '/Pharmacy/getDiag/',
//            dataType: 'json',
//            data: { id: $("#ddlSpeciality").val() },

//            success: function (r) {
//                $('#Diagnoises').dataTable().fnDestroy();
//                var setData = $("#Diagnoises Tbody");
//                setData.empty();

//                for (var i = 0; i < r.length; i++) {
//                    var data = "<tr >" +
//                        "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectDiagnoise(this);'></Button>" + "</td>" +
//                        "<td>" + r[i].Code + "</td>" +
//                        "<td>" + r[i].Name + "</td>" +
//                        "</tr>"
//                    setData.append(data);
//                }
//                $('#Diagnoises').DataTable();
//            },
//            error: function (ex) {
//                alert('Failed to retrieve Diagnoses.');
//            }

//        });
//    });


//    $("#AddRay").click(function () {

//        if ($('#txtSearchCard').val() != '') {
//            if ($('#Diagnoise').val() != '') {

//                $.ajax({
//                    type: "POST",
//                    dataType: "json",
//                    url: '/Rays/GetRays',
//                    data: {},
//                    success: function (r) {
//                        $('#RaysModal').modal();
//                        var setData = $("#Rays Tbody");
//                        //setData.empty();
//                        for (var i = 0; i < r.length; i++) {
//                            var data = "<tr >" +
//                                "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectRay(this);'></Button>" + "</td>" +
//                                "<td>" + r[i].SERV_CODE + "</td>" +
//                                "<td>" + r[i].SERV_ANAME + "</td>" +
//                                "<td>" + r[i].SERV_AMOUNT + "</td>" +
//                                "<td>" + r[i].GRUOP_TYPE + "</td>"
//                            "</tr>"
//                            setData.append(data);

//                        }
//                        $('#Rays').DataTable();

//                    },
//                    error: function (ex) {
//                        alert('No Rays Data');
//                    }
//                });
//            }
//            else {
//                bootbox.alert("Please Insert Diagnoise");

//            }
//        }
//        else {
//            bootbox.alert("Please Insert Card Number");
//        }
//    });
//    $('#AddDiagnoise').click(function () {
//        $('#DiagnoiseModal').modal();
//    });

//    $('#RemoveDiagnoise').click(function () {
//        var splited = $('#Diagnoise').val().split("\n");
//        DiagnosisList.splice(-1, 1);
//        if (splited[splited.length - 1] == "") {

//            splited.splice(-1, 1);
//            splited.splice(-1, 1);
//            $('#Diagnoise').val(splited.join("\n"));
//        }
//        else {

//            splited.splice(-1, 1);
//            $('#Diagnoise').val(splited.join("\n"));
//        }
//    });


//    $('#submit').click(function () {
//        var Id;
//        if ($('#txtSearchCard').val() != "") {
//            if ($('#txtSearchCard').val() != "") {
//                $.ajax({
//                    type: 'POST',
//                    url: '/Rays/SAVE/',
//                    dataType: 'Json',
//                    data: {
//                        CardId: $('#txtSearchCard').val(),
//                        CompanyPercent: $('#ddEmp_CEILING_PERT').val(),
//                        Limit: $('#insurance_LIVEL').val(),
//                        Speciality: $('#ddlSpeciality option:selected').text(),
//                        Diagnose1: $('#Diagnoise').val(),
//                        //calculation
//                        TotalValue: $('#txtTotalInvoice').val(),
//                        PersonPayment: $('#txtTotalCopayment').val(),
//                        CompanyPayment: $('#txtValueCredit').val(),
//                        OverInsurance: $('#txtOverInsurance').val(),
//                        Cash: $('#txtValueCash').val(),
//                        PhoneNumber: $('#PhoneNumber').val(),
//                        Comments: $('#Comments').val()

//                    },
//                    success: function (r) {
//                        Date.prototype.yyyymmdd = function () {
//                            var ReportId = r;

//                            var mm = this.getMonth() + 1; // getMonth() is zero-based
//                            var dd = this.getDate();
//                            return [(dd > 9 ? '' : '0') + dd,
//                            (mm > 9 ? '' : '0') + mm,
//                            this.getFullYear()
//                            ].join('');
//                        };
//                        var date = new Date();
//                        d = date.yyyymmdd()
//                        r = d + r;
//                        approval = r;
//                        //bootbox.alert("Roshita ID : " + r);
//                        bootbox.dialog({
//                            closeButton: false,
//                            title: 'Added Sucessfully',
//                            message: "Roshita ID : " + r,
//                            buttons: {
//                                Print: {
//                                    Rayel: "Print",
//                                    className: 'btn-info',
//                                    callback: function () {
//                                        window.location.reload();
//                                        window.open('/Rays/ControlPenelReport?id=' + ReportId);
//                                    }
//                                },
//                                New: {
//                                    Rayel: "New",
//                                    className: 'btn-info',
//                                    callback: function () {
//                                        window.location.reload();
//                                    }
//                                }

//                            }
//                        });
//                    },
//                    error: function (err) {
//                        bootbox.alert("Error Roshita");
//                    }
//                }).done(function () {
//                    var Mediciens = new Array();
//                    $("#Ray TBODY TR").each(function () {
//                        var row = $(this);
//                        var Medicien = {};

//                        Medicien.RoshitaID = Id;
//                        Medicien.MedicienCode = row.find("TD").eq(0).html();
//                        Medicien.MedicienName = row.find("TD").eq(1).html();
//                        Medicien.Amount = row.find("TD").eq(2).html();
//                        Medicien.PaymentGroup = row.find("TD").eq(3).html();
//                        Mediciens.push(Medicien);
//                    });
//                    $.ajax({
//                        type: 'POST',
//                        url: '/Rays/SaveMediciens/',
//                        dataType: 'Json',
//                        contentType: "application/json; charset=utf-8",
//                        data: JSON.stringify(Mediciens),
//                        success: function (r) {

//                        },
//                        error: function (err) {
//                            bootbox.alert("Error Medicien");
//                        }
//                    });
//                });
//            } else {
//                bootbox.alert("Please Insert PhoneNumber");
//            }
//        }
//        else
//            bootbox.alert("Please Insert Card ID");
//    });
//});
//function Remove(button) {
//    //Determine the reference of the Row using the Button.
//    var row = $(button).closest("TR");
//    var name = $("TD", row).eq(1).html();
//    bootbox.confirm("Do you want to delete: " + name, function (result) {
//        if (result) {
//            //Delete the Table row using it's Index.
//            //---------------------------
//            var row = $(button).closest("TR");
//            var table = $("#Ray")[0];
//            table.deleteRow(row[0].rowIndex);
//            Calculation();

//        }
//    });

//}
//function Calculation() {

//    var sum = 0;
//    var sumCash = 0;
//    $('#Ray TBODY TR').each(function () {
//        var row = $(this);
//        if (row.find("TD").eq(3).html() != "Pending") {
//            sum += parseInt(row.find("TD").eq(2).html());
//            if (row.find("TD").eq(3).html() == "Cash")
//                sumCash += parseInt(row.find("TD").eq(2).html());
//        }
//    });
//    $("#txtTotalInvoice").val(sum);
//    $('#txtCash').val(sumCash);
//    $('#txtOverInsurance').val("0");

//    var person = parseInt(100 - CompanyPayment);//percentage
//    //total-cash
//    var total = parseInt($('#txtTotalInvoice').val()) - sumCash;
//    var cash = parseInt($('#txtCash').val());
//    var ValueCredit = 0;
//    if (limit_Daily > AnuualLimit || limit_Daily == 0) {
//        limit_Daily = AnuualLimit;
//    }

//    if (limit_Daily != 0) {
//        ValueCredit = (total * (CompanyPayment / 100)).toFixed(2);
//        if (limit_Daily < (ValueCredit)) {
//            $('#txtValueCredit').val(Limit_Daily);
//            $('#txtTotalCopayment').val((Limit_Daily * (person / 100)).toFixed(2));
//            $('#txtOverInsurance').val(total - Limit_Daily - parseFloat($('#txtTotalCopayment').val()));
//        }
//        else if (limit_Daily > (ValueCredit)) {
//            $('#txtValueCredit').val((total * (CompanyPayment / 100)).toFixed(2));
//            $('#txtTotalCopayment').val((total * (person / 100)).toFixed(2));
//        }
//    }
//    else {
//        $('#txtValueCredit').val(((total) * (CompanyPayment / 100)).toFixed(2));
//        $('#txtTotalCopayment').val(((total) * (person / 100)).toFixed(2));
//    }
//    $('#txtValueCash').val((sumCash + parseFloat($('#txtTotalCopayment').val()) + parseFloat($('#txtOverInsurance').val())).toFixed(2));
//}