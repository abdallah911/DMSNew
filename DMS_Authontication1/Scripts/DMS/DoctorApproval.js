var input = document.getElementById("txtSearchCard");
var CardId;
var dat1;
var Genderr;
var Age;
var dosse;
var duration;
var totalduration;
var AnuualLimit;
var firstDate;
//var NationalId;
var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
const secondDate = new Date();
var diffDays;
input.addEventListener("keyup", function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        $('#Search').click();
    }
});

$(function () {
    $('#Search').click(function () {
        $("#wait").css("display", "block");
        $.ajax({
            url: '/Pharmacy/AddCard/',
            data: { id: $('#txtSearchCard').val() },
            dataType: 'Json',
            success: function (r) {
                $("#wait").css("display", "none");

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
                    $('#Pending').attr('disabled', false);
                    CardId = $('#txtSearchCard').val();
                    var ArName = r[0].EMP_ENAME;

                    firstDate = new Date(parseFloat(r[0].INS_END_DATE.replace(/(^.*\()|([+-].*$)/g, '')));
                    diffDays = Math.round(Math.abs((firstDate - secondDate) / oneDay));
                    var EndDate = dat1;
                    //var today = new Date();
                    //var dd = today.getDate();
                    //var mm = today.getMonth(); //January is 0!
                    //var yyyy = today.getFullYear();
                    //var CurrentDate = new Date(yyyy, mm, dd);
                    //if (EndDate != "") {
                    //    newDate = EndDate.split('/').reverse().join('.');
                    //} else {
                    //    newDate = "";
                    //}

                    //var date = new Date(newDate);
                    // today = mm + '/' + dd + '/' + yyyy;

                    //if (date > CurrentDate || newDate == "null") {
                    $('#txtSearchCard').val(CardId);
                    $('#compEmp_EMP_ANAME').val(ArName);
                    $('#compEmp_INS_END_DATE').val(EndDate);

                    $('#CardsModal').modal('hide');
                    //var url = "/Doctor/Doctor/" + CardId;
                    //$('#Doctor').attr("disabled", false).attr("href", url);
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: '/Pharmacy/GetCompName',
                        data: { id: CardId },

                        success: function (returndata) {
                            if (returndata.ok) {

                                $("#contractComp_C_ANAME").val(returndata.data.C_ANAME);

                            }
                            else {
                                window.alert(' No Company Data ');

                            }
                        }
                    });
                    //GetLimit
                    //$.ajax({
                    //    type: "POST",
                    //    dataType: "json",
                    //    url: '/Pharmacy/GetLimit',
                    //    data: { id: CardId },
                    //    success: function (returndata) {
                    //        if (returndata.ok) {
                    //            $("#insurance_LIVEL").val(returndata.limit.INSURANCE_DAY);
                    //        }
                    //        else {
                    //            bootbox.alert('No Limit Amount ');
                    //        }
                    //    }
                    //});
                    //CellingAmount
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: '/Pharmacy/CellingAmount',
                        data: {
                            id: CardId,
                            ServiceCode: '11601'
                        },
                        success: function (r) {
                            if (r.Validation == false) {
                                toastr.info(r.Message);
                                ClearCardData();
                            } else {
                                $('#ddEmp_CEILING_PERT').val(r.CeilingPert);
                                AnuualLimit = r.Limit;
                                if (r.LimitDailyPreceptionCount && r.CoInsurancelimit.INSURANCE_DAY >= 0) {
                                    $("#insurance_LIVEL").val(r.CoInsurancelimit.INSURANCE_DAY);
                                } else {
                                    alert(" لقد تم استهلاك العدد المحدد للروشتات وسوف تكون خارج التغطه ");
                                    $("#insurance_LIVEL").val("0.001");

                                    $('#ddEmp_CEILING_PERT').val("0");
                                }
                            }
                        },
                        error: function (err) {
                            alert("Company Annual Amount");
                            location.reload();
                        }
                    });

                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: '/DoctorApprovals/History',
                        data: { id: CardId },
                        success: function (r) {
                            $('#From').attr("disabled", false);
                            $('#To').attr("disabled", false);
                            $('#History').dataTable().fnDestroy();
                            var setData = $("#History Tbody");
                            setData.empty();
                            var dat;
                            for (var i = 0; i < r.length; i++) {
                                if (r[i].CreatedDate != null) {
                                    var MyDate_String_Value = r[i].CreatedDate;
                                    var value = new Date
                                        (
                                            parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                        );
                                    dat = (value.getMonth() + 1) + "/" + value.getDate() + "/" + value.getFullYear();

                                }
                                else {
                                    dat = null;

                                }
                                if (i == 0) {
                                    $('#From').val(dat);
                                }
                                if (i == r.length - 1) {
                                    $('#To').val(dat);
                                }
                                var data = "<tr >" +
                                    //"<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectHistory(this);'></Button>" + "</td>" +
                                    "<td>" + r[i].MedicienCode + "</td>" +
                                    "<td>" + r[i].MedicienName + "</td>" +
                                    "<td>" + r[i].DOSAGE_FORM + "</td>" +
                                    "<td>" + r[i].Dose + "</td>" +
                                    "<td>" + r[i].Duration + "</td>" +
                                    "<td>" + r[i].TotalDuration + "</td>" +
                                    "<td>" + r[i].TotalUnits + "</td>" +
                                    "<td>" + r[i].Amount + "</td>" +
                                    "<td>" + r[i].M_TYPE + "</td>" +
                                    "<td>" + dat + "</td>" +
                                    "<td>" + r[i].CreatedBy + "</td>" +
                                    "</tr>"
                                setData.append(data);

                            }
                            $('#History').DataTable();
                        },
                        error: function (ex) {
                            alert("Error History");

                        }
                    });
                    //}
                    //else {
                    //    bootbox.dialog({
                    //        title: 'Alert!',
                    //        message: ' Expired Card',// "Roshita ID : " + r,
                    //        buttons: {
                    //            Ok: {
                    //                label: "Ok",
                    //                className: 'btn-info',
                    //                callback: function () {
                    //                    location.reload();
                    //                }
                    //            }
                    //        }
                    //    });
                    //    //bootbox.alert("Expired Card");




                    //}
                } else if (r.length == 0) {
                    alert(' Invalid Card Number ');
                    $('#CardsModal').modal('hide');
                }

                else {

                    $('#Cards').DataTable();
                    $('#CardsModal').modal();
                }
            },
            error: function () {
                alert("Too Many Data To Retrieve");
            }

        });
    });
    $('#From').datepicker({
        onSelect: function (dateStr) {
            datestr = dateStr.toString();
            var To = $('#To').val();
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/DoctorApprovals/FromHistory',
                data: {
                    id: CardId,
                    From: dateStr,
                    To: To
                },
                success: function (r) {

                    var setData = $("#History Tbody");
                    setData.empty();
                    var dat;
                    for (var i = 0; i < r.length; i++) {
                        if (r[i].CreatedDate != null) {
                            var MyDate_String_Value = r[i].CreatedDate;
                            var value = new Date
                                (
                                    parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                );
                            dat = (value.getMonth() + 1) + "/" + value.getDate() + "/" + value.getFullYear();

                        }
                        else {
                            dat = null;

                        }
                        if (i == 0) {
                            $('#From').val(dat);
                        }
                        if (i == r.length - 1) {
                            $('#To').val(dat);
                        }
                        var data = "<tr >" +
                            //"<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectHistory(this);'></Button>" + "</td>" +
                            "<td>" + r[i].MedicienCode + "</td>" +
                            "<td>" + r[i].MedicienName + "</td>" +
                            "<td>" + r[i].DOSAGE_FORM + "</td>" +
                            "<td>" + r[i].Dose + "</td>" +
                            "<td>" + r[i].Duration + "</td>" +
                            "<td>" + r[i].TotalDuration + "</td>" +
                            "<td>" + r[i].TotalUnits + "</td>" +
                            "<td>" + r[i].Amount + "</td>" +
                            "<td>" + r[i].M_TYPE + "</td>" +
                            "<td>" + dat + "</td>" +
                            "<td>" + r[i].CreatedBy + "</td>" +
                            "</tr>";
                        setData.append(data);
                    }
                    $('#History').DataTable();
                },
                error: function (ex) {
                    alert("Error History");

                }
            });
        }
    });
    $('#To').datepicker({
        onSelect: function (dateStr) {
            datestr = dateStr.toString();
            var Fm = $('#From').val();
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/DoctorApprovals/FromHistory',
                data: {
                    id: CardId,
                    From: Fm,
                    To: dateStr
                },
                success: function (r) {
                    var setData = $("#History Tbody");
                    setData.empty();
                    var dat;
                    for (var i = 0; i < r.length; i++) {
                        if (r[i].CreatedDate != null) {
                            var MyDate_String_Value = r[i].CreatedDate;
                            var value = new Date
                                (
                                    parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                );
                            dat = (value.getMonth() + 1) + "/" + value.getDate() + "/" + value.getFullYear();

                        }
                        else {
                            dat = null;

                        }
                        if (i == 0) {
                            $('#From').val(dat);
                        }
                        if (i == r.length - 1) {
                            $('#To').val(dat);
                        }
                        var data = "<tr >" +
                            //"<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectHistory(this);'></Button>" + "</td>" +
                            "<td>" + r[i].MedicienCode + "</td>" +
                            "<td>" + r[i].MedicienName + "</td>" +
                            "<td>" + r[i].DOSAGE_FORM + "</td>" +
                            "<td>" + r[i].Dose + "</td>" +
                            "<td>" + r[i].Duration + "</td>" +
                            "<td>" + r[i].TotalDuration + "</td>" +
                            "<td>" + r[i].TotalUnits + "</td>" +
                            "<td>" + r[i].Amount + "</td>" +
                            "<td>" + r[i].M_TYPE + "</td>" +
                            "<td>" + dat + "</td>" +
                            "<td>" + r[i].CreatedBy + "</td>" +
                            "</tr>";
                        setData.append(data);
                    }
                    $('#History').DataTable();
                },
                error: function (ex) {
                    alert("Error History");

                }
            });
        }
    });
    $('#PharmacyTxt').click(function () {

        $.ajax({
            url: '/DoctorApprovals/Pharmacies',
            dataType: 'Json',
            success: function (r) {
                $('#BranchTxt').val(' ');
                $('#Pharmacies').dataTable().fnDestroy();
                $('#PharmaciesModal').modal();
                var setData = $("#Pharmacies Tbody");
                setData.empty();

                for (var i = 0; i < r.length; i++) {

                    var data = "<tr >" +
                        "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='Select1(this);'></Button>" + "</td>" +
                        "<td>" + r[i].PR_CODE + "</td>" +
                        "<td>" + r[i].PR_ANAME + "</td>" +
                        "</tr>";
                    setData.append(data);

                }
                $('#Pharmacies').DataTable();

            },
            error: function () {
                alert("Error Retrieve");
            }

        });
    });
    $('#BranchTxt').click(function () {
        if ($('#PharmacyTxt').val() != '') {
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/DoctorApprovals/Branches',
                data: { id: $('#PharmacyTxt').val() },
                success: function (r) {
                    $('#Branches').dataTable().fnDestroy();
                    $('#BranchesModal').modal();
                    var setData = $("#Branches Tbody");
                    setData.empty();

                    for (var i = 0; i < r.length; i++) {

                        var data = "<tr >" +
                            "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='Select2(this);'></Button>" + "</td>" +
                            "<td>" + r[i].PR_CODE + "</td>" +
                            "<td>" + r[i].PR_ANAME + "</td>" +
                            "<td>" + r[i].ADDRESS1 + "</td>" +
                            "</tr>";
                        setData.append(data);

                    }
                    $('#Branches').DataTable();


                },
                error: function (r) {
                    window.alert(' No Company Branches');
                }

            });
        }
        else {
            bootbox.alert("Please Insert Pharmacy First");
        }
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
    var V = 0;

    //Mediciences
    var MedicineArray = [];
    $("#AddMedicine").select2({
        placeholder: "Select a medicine",
        ajax: {
            url: '/Pharmacy/GetList',
            dataType: 'json',
            data: function (params) {
                var query = {
                    sEcho: params.page || 1,
                    //iColumns=10,
                    //iDisplayLength=10,
                    sSearch: params.term,

                }

                // Query parameters will be ?search=[term]&page=[page]
                return query;
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                var result = [];
                for (var i = 0; i < data.aaData.length; i++) {
                    var current = {};
                    current.id = data.aaData[i].M_CODE;
                    current.text = data.aaData[i].TRADE_NAME;
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
    $('#AddMedicine').on('select2:selecting', function (event) {
        if ($('#txtSearchCard').val() != '') {
            if ($('#PrescriptionDate').val() != '') {
                if ($('#PhoneNumber').val() != '') {
                    if ($('#ddlDiagnoises').val().length != 0) {
                        //setTimeout(function () {}, 3000);
                        SelectMedicien(event);

                    }
                    else {
                        toastr.info("Please insert Diagnoise Date");
                        event.preventDefault();
                    }
                } else {
                    toastr.info("Please insert Phone number");
                    event.preventDefault();
                }
            } else {
                toastr.info("Please insert Prescription Date");
                event.preventDefault();
            }
        }
        else {
            toastr.info("Please Insert Card Number");
            event.preventDefault();
            // $('#AddMedicine').val(null).trigger("change");
        }
    });
    $('#AddMedicine').on("select2:unselecting", function (event) {
        $('#Pharmacy tbody tr').each(function () {
            var row = $(this);
            if (parseInt(row.find("TD").eq(0).html()) == parseInt(event.params.args.data.id)) {
                Remove(row, event);
            }
        });

    });


    $('#submit').click(function () {
        var Id;
        var Mediciens = new Array();
        $("#Pharmacy TBODY TR").each(function () {
            var row = $(this);
            var Medicien = {};
            Medicien.RoshitaID = Id;
            Medicien.MedicienCode = row.find("TD").eq(0).html();
            Medicien.MedicienName = row.find("TD").eq(1).html();
            Medicien.Dose = $("TD", row).find(".Dose").val();
            if (Medicien.Dose == undefined) {
                Medicien.Dose = $("TD", row).eq(7).html();
            }
            Medicien.Duration = $("TD", row).find(".Duration").val();
            Medicien.TotalDuration = $("TD", row).find(".TotalDuration").val();
            Medicien.TotalUnits = $("TD", row).find(".TotalUnits").val();
            Medicien.Amount = $("TD", row).find(".Amount").val();
            Medicien.PaymentGroup = row.find("TD").eq(12).html();
            Mediciens.push(Medicien);
        });
        if ($('#txtSearchCard').val() != "") {
            changeTotalUnits();
            changeTotalDuration();
            changeTable();
            //var TotalDuration = $("TD", row).find(".TotalDuration").val();
            //if (TotalDuration != 0) { }
            //else {
            //    bootbox.alert("Invalid Total Duration");
            //}
            if (Mediciens.length != 0) {
                $("#submit").attr("disabled", "disabled");
                var SelectedDiagnosisList = $('#ddlDiagnoises').select2('data');
                var diagnose = [];
                for (var i = 0; i < SelectedDiagnosisList.length; i++) {
                    var current = {};
                    current.DIAG_CODE = SelectedDiagnosisList[i].id;
                    current.DIAG_ANAME = SelectedDiagnosisList[i].text;
                    diagnose.push(current);
                }
                var SavePrescription = {
                    //hasApproval: $('#HasApproval').prop("checked") ? true : false,
                    CardId: $('#txtSearchCard').val(),
                    RoshetaType: "11601",
                    CompanyPercent: $('#ddEmp_CEILING_PERT').val(),
                    Limit: $('#insurance_LIVEL').val(),
                    Speciality: $('#ddlSpeciality option:selected').text(),
                    Diagnose1: $('#ddlDiag1').val(),
                    Diagnose2: $('#PharmacyTxt').val(),//Pharmacy
                    TotalValue: $('#txtTotalInvoice').val(),
                    PersonPayment: $('#txtTotalCopayment').val(),
                    CompanyPayment: $('#txtValueCredit').val(),
                    OverInsurance: $('#txtOverInsurance').val(),
                    Cash: $('#txtValueCash').val(),
                    PhoneNumber: $('#BranchTxt').val(),// Branch
                    //ClaimNumber: $('#PharmacyTxt').val(),//Pharmacy
                    // createdby: $('#ddlUsers').val() == undefined ? null : $('#ddlUsers :selected').val(),
                    roshitaDetail: Mediciens,
                    diagnose: diagnose
                };
                $.ajax({
                    type: 'POST',
                    url: '/DoctorApprovals/SavePrescription/',
                    dataType: 'Json',
                    //contentType: "application/json; charset=utf-8",
                    //data: JSON.stringify(SavePrescriptipn),
                    data: SavePrescription,
                    success: function (OracleId) {
                        bootbox.dialog({
                            closeButton: false,
                            title: 'Added Sucessfully',
                            message: "Approval Number : " + OracleId,
                            buttons: {
                                //Print: {
                                //    label: "Print",
                                //    className: 'btn-info',
                                //    callback: function () {
                                //        window.location.reload();
                                //        window.open('/DoctorApprovals/RoshitaReport?id=' + OracleId);
                                //        $("#submit").attr("disabled", false);

                                //    }
                                //},
                                New: {
                                    label: "New",
                                    className: 'btn-info',
                                    callback: function () {
                                        window.location.reload();
                                        $("#submit").attr("disabled", false);

                                    }
                                }
                            }
                        });
                    },
                    error: function (err) {
                        bootbox.alert("Error saving roshita,please check your internet connection");
                        $("#submit").attr("disabled", false);
                    }
                });
                //$.ajax({
                //    type: 'POST',
                //    url: '/DoctorApprovals/SAVE/',
                //    dataType: 'Json',
                //    data: {
                //        CardId: $('#txtSearchCard').val(),
                //        RoshetaType: "11601",
                //        CompanyPercent: $('#ddEmp_CEILING_PERT').val(),
                //        Limit: $('#insurance_LIVEL').val(),
                //        Speciality: $('#ddlSpeciality option:selected').text(),
                //        Diagnose1: $('#ddlDiag1').val(),

                //        //calculation
                //        TotalValue: $('#txtTotalInvoice').val(),
                //        PersonPayment: $('#txtTotalCopayment').val(),
                //        CompanyPayment: $('#txtValueCredit').val(),
                //        OverInsurance: $('#txtOverInsurance').val(),
                //        Cash: $('#txtValueCash').val()

                //    },
                //    success: function (Oracle_Id) {
                //        //Date.prototype.yyyymmdd = function () {
                //        //    var mm = this.getMonth() + 1; // getMonth() is zero-based
                //        //    var dd = this.getDate();
                //        //    return [(dd > 9 ? '' : '0') + dd,
                //        //    (mm > 9 ? '' : '0') + mm,
                //        //    this.getFullYear()
                //        //    ].join('');
                //        //};
                //        //var date = new Date();
                //        //d = date.yyyymmdd();
                //        //r = r + d;
                //        //approval = r;
                //        //bootbox.alert("Roshita ID : " + r);
                //        bootbox.dialog({
                //            closeButton: false,
                //            title: 'Added Sucessfully',
                //            message: "Roshita ID : " + Oracle_Id,
                //            buttons: {
                //                Print: {
                //                    label: "Print",
                //                    className: 'btn-info',
                //                    callback: function () {
                //                        window.location.reload();
                //                        window.open('/DoctorApprovals/RoshitaReport?id=' + Oracle_Id);
                //                        $("#submit").attr("disabled", false);

                //                    }
                //                },
                //                New: {
                //                    label: "New",
                //                    className: 'btn-info',
                //                    callback: function () {
                //                        window.location.reload();
                //                        $("#submit").attr("disabled", false);

                //                    }
                //                }
                //            }


                //        });
                //    },
                //    error: function (err) {
                //        bootbox.alert("Error saving roshita,please check your internet connection");
                //        $("#submit").attr("disabled", false);

                //    }
                //}).done(function () {

                //    $.ajax({
                //        type: 'POST',
                //        url: '/DoctorApprovals/SaveMediciens/',
                //        dataType: 'Json',
                //        contentType: "application/json; charset=utf-8",
                //        data: JSON.stringify(Mediciens),
                //        success: function (r) {

                //        },
                //        error: function (err) {
                //            bootbox.alert("Error Medicien");
                //        }
                //    }).done(function () {

                //        if ($('#Pharmacy').val() != null) {
                //            $.ajax({
                //                type: 'POST',
                //                url: '/DoctorApprovals/SavePharmacyApproval/',
                //                dataType: 'Json',
                //                data: {
                //                    Branch: $('#BranchTxt').val(),
                //                    Pharmacy: $('#PharmacyTxt').val(),
                //                },
                //                success: function (r) {

                //                },
                //                error: function (err) {
                //                    bootbox.alert("Error Pharmacy");
                //                }
                //            });
                //        }

                //        var SelectedDiagnosisList = $('#ddlDiagnoises').select2('data');
                //        var DiagnosisList = [];
                //        for (var i = 0; i < SelectedDiagnosisList.length; i++) {
                //            var current = {};
                //            current.DIAG_CODE = SelectedDiagnosisList[i].id;
                //            current.DIAG_ANAME = SelectedDiagnosisList[i].text;
                //            DiagnosisList.push(current);
                //        }
                //        //Diagnoises
                //        $.ajax({
                //            type: 'POST',
                //            url: '/DoctorApprovals/SaveDiagnoises/',
                //            dataType: 'Json',
                //            contentType: "application/json; charset=utf-8",
                //            data: JSON.stringify(DiagnosisList),
                //            success: function (r) {
                //            },
                //            error: function (err) {
                //                bootbox.alert("Error Diagnoises");
                //            }

                //        });
                //    });
                //});
            }

            else { bootbox.alert("Please Insert medicines"); }

        }
        else {
            bootbox.alert("Please Insert Card ID");
        }
    });

});
function SelectMedicien(event) {
    var Code = event.params.args.data.id
    $("#wait").css("display", "block");
    var MedicienCode;
    var MedicienName;
    var DosageForm;
    var PackPrice;
    var PackSize;
    var UnitNumber;
    var UnitPrice;
    var Group;
    var IsCover;
    var GenderValidation = false;
    var AgeValiation = false;
    var edit = 0;
    $.ajax({
        type: 'POST',
        url: '/Pharmacy/GetMedicineByCode/',
        dataType: 'json',
        data: { code: Code },
        success: function (r) {
            MedicienCode = r.M_CODE;
            MedicienName = r.TRADE_NAME;
            DosageForm = r.DOSAGE_FORM;
            PackPrice = r.PACK_PRICE;
            PackSize = r.PACK_SIZE;
            UnitNumber = r.UNIT_NO;
            UnitPrice = r.UNIT_PRICE;
            Group = r.Group_Type;
            IsCover = r.IsCovered.toString();
            var medicineGroups = new Array();
            var medicineGroup = {};
            medicineGroup.TRADE_NAME = MedicienCode; //current medicine code
            medicineGroups.push(medicineGroup);
            $('#Pharmacy tbody tr').each(function () {
                var row = $(this);
                var medicineGroup = {};
                medicineGroup.M_CODE = row.find("TD").eq(0).html();
                medicineGroups.push(medicineGroup);
                if (parseInt(row.find("TD").eq(0).html()) == parseInt(MedicienCode)) {
                    edit = 1;
                    event.preventDefault();
                    toastr.error('Added before');
                    $("#wait").css("display", "none");

                }
            });
            var samegroup = false;
            var Duration = false;
            //
            if (edit == 0) {
                if (IsCover == "true") {
                    //Gender Check
                    $.ajax({
                        dataType: "json",
                        url: '/Pharmacy/GenderValidation',
                        data: {
                            CardId: $('#txtSearchCard').val(),
                            MedicineCode: MedicienCode,
                            Id: Genderr
                        },
                        success: function (r) {
                            if (r == "True") {
                                GenderValidation = true;
                            }
                            else {
                                RemoveSelection(Code);
                                toastr.error("You can't dispense this medicine , Gender Validation");
                                $("#wait").css("display", "none");

                            }
                        },
                        error: function (r) { }
                    }).done(function () {
                        //Age Check
                        $.ajax({
                            dataType: "json",
                            url: '/Pharmacy/AgeValidation',
                            data: {
                                CardId: $('#txtSearchCard').val(),
                                MedicineCode: MedicienCode,
                                Id: Age
                            },
                            success: function (r) {
                                if (r == "True") {
                                    AgeValiation = true;
                                }
                                else {
                                    if (GenderValidation = true) {
                                        RemoveSelection(Code);
                                        toastr.error("You can't dispense this medicine , Age Validation");
                                        $("#wait").css("display", "none");
                                    }
                                }
                            },
                            error: function (r) { }
                        }).done(function () {
                            //medicine group
                            $.ajax({
                                type: 'POST',
                                url: '/Pharmacy/MedicinesGroupValiadtion/',
                                dataType: 'Json',
                                contentType: "application/json; charset=utf-8",
                                data: JSON.stringify(medicineGroups),
                                success: function (r) {
                                    if (r == true) {
                                        if (AgeValiation = true) {
                                            RemoveSelection(Code);
                                            toastr.error("You can't dispense this medicine , Medicines Group Valiadtion");//same Group
                                            $("#wait").css("display", "none");
                                        }
                                        samegroup = true;
                                    }
                                    else {
                                        samegroup = false;
                                    }
                                },
                                error: function () {
                                    samegroup = false;
                                }
                            }).done(function () {
                                $.ajax({
                                    url: '/Pharmacy/MedicinesDurationValiadtion/',
                                    dataType: 'Json',
                                    contentType: "application/json; charset=utf-8",
                                    data: {
                                        CardId: $('#txtSearchCard').val(),
                                        MedicineCode: MedicienCode
                                    },
                                    success: function (r) {
                                        if (r == true) {
                                            if (samegroup = false) {
                                                RemoveSelection(Code);
                                                toastr.error("You can't dispense this medicine , Medicines Duration Valiadtion");//'Duration Validation'
                                                $("#wait").css("display", "none");
                                                Duration = true;
                                            }

                                        }
                                        else {
                                            Duration = false;
                                        }
                                    },
                                    error: function () {
                                        Duration = false;
                                    }
                                }).done(function () {
                                    if (GenderValidation == true && AgeValiation == true && IsCover == "true" && samegroup == false && Duration == false) {
                                        //check Daily
                                        $.ajax({
                                            dataType: "json",
                                            url: '/Pharmacy/CheckDaily',
                                            data: {
                                                id: $('#txtSearchCard').val(),
                                                code: MedicienCode
                                            },
                                            success: function (r) {
                                                if (r.check == 0) {
                                                    $("#wait").css("display", "none");
                                                    //append row
                                                    if (Group == "NO") {
                                                        var dialog = bootbox.dialog({
                                                            title: 'This Medicien is Not Covered!',
                                                            message: "<p>Pay method?</p>",
                                                            buttons: {
                                                                Cash: {
                                                                    label: "Cash",
                                                                    className: 'btn-info',
                                                                    callback: function () {
                                                                        Group = "Cash";
                                                                        AppendRow();
                                                                    }
                                                                },
                                                                Approval: {
                                                                    label: "Approved",
                                                                    className: 'btn-info',
                                                                    callback: function () {
                                                                        Group = "Approval";
                                                                        AppendRow();
                                                                    }
                                                                },
                                                                Tele: {
                                                                    label: "Pending",
                                                                    className: 'btn-info',
                                                                    callback: function () {
                                                                        Group = "Pending";
                                                                        AppendRow();
                                                                    }
                                                                }
                                                            }
                                                        });
                                                    }
                                                    else {
                                                        AppendRow();
                                                    }
                                                }
                                                else {
                                                    RemoveSelection(Code);
                                                    //bootbox.alert("This Medicine had been exchanged Today ");
                                                    bootbox.alert(r.messa);
                                                    //bootbox.alert("هذا الدواء تم التعامل معه من قبل , برجاء مراجعه الادويه المزمنه للمريض,او الاتصال بالاداره الطبيه");
                                                    $("#wait").css("display", "none");
                                                }
                                            },
                                            error: function (r) {
                                                bootbox.alert("Ajax exchanged Today  Error");
                                                $("#wait").css("display", "none");
                                            }

                                        });
                                    }
                                });
                            });
                        })
                    })
                }
                else {
                    RemoveSelection(Code);
                    toastr.error("Medicine isn't covered");
                }
            }

            $("#wait").css("display", "none");

        },
        error: function (ex) {
            bootbox.alert('Failed to retrieve Medicine Data.');
        }

    });
    function AppendRow() {
        var tBody = $("#Pharmacy > TBODY")[0];
        var row = tBody.insertRow(-1);
        var cell = $(row.insertCell(-1));
        cell.html(MedicienCode);
        cell = $(row.insertCell(-1));
        cell.html(MedicienName);
        cell = $(row.insertCell(-1));
        cell.html(DosageForm);
        cell = $(row.insertCell(-1));
        cell.html(PackSize);
        cell = $(row.insertCell(-1));
        var PackagePrice = $("<input  />");
        PackagePrice.attr("type", "text");
        PackagePrice.addClass("form-control");
        PackagePrice.addClass("PackagePrice");
        PackagePrice.attr("onkeyup", "changeTotalUnits(this);");
        PackagePrice.val(PackPrice);
        cell.append(PackagePrice);
        cell = $(row.insertCell(-1));
        cell.html(UnitNumber);
        cell = $(row.insertCell(-1));
        var unitprice = $("<input  />");
        unitprice.attr("type", "text");
        unitprice.attr('readonly', 'readonly');
        unitprice.addClass("form-control");
        unitprice.addClass('UnitPrice');
        cell.append(unitprice);
        cell = $(row.insertCell(-1));
        var Dose = $("<input  />");
        Dose.attr("type", "text");
        Dose.addClass("form-control");
        Dose.addClass("Dose");
        Dose.attr("onkeyup", "changeTable(this);");
        if (DosageForm == "GEL" || DosageForm == "CREAM" || DosageForm == "SUPP" || DosageForm == "SPRAY" || DosageForm == "DROPS" /*|| DosageForm == "SACHET"*/) {
            cell.append(1);
        } else {
            cell.append(Dose);
        }
        cell = $(row.insertCell(-1));
        var Duration = $("<input  />");
        Duration.attr("type", "text");
        Duration.addClass("form-control");
        Duration.addClass("Duration");
        //Duration.attr("onkeyup", "changeTable(this);");
        Duration.attr("onkeyup", "changeTotalDuration(this);changeTable(this);");

        cell.append(Duration);
        cell = $(row.insertCell(-1));
        var TotalDuration = $("<input  />");
        TotalDuration.attr("type", "text");
        TotalDuration.addClass("form-control");
        TotalDuration.addClass("TotalDuration");
        TotalDuration.attr("onfocusout", "changeTotalDuration(this);");
        cell.append(TotalDuration);
        cell = $(row.insertCell(-1));
        if (DosageForm == "ELIXIR" || DosageForm == "SYRUP" || DosageForm == "SUSPENTION" || DosageForm == "EMULSION" || DosageForm == "SOUTION") {
            var TotalUnits = $("<input  />");
            TotalUnits.attr("type", "text");
            TotalUnits.addClass('TotalUnits');
            TotalUnits.addClass("form-control");
            TotalUnits.attr("onkeyup", "changeTotalUnits(this);");
            cell.append(TotalUnits);
        }
        else {
            var TotalUnits = $("<input  />");
            TotalUnits.attr("type", "text");
            TotalUnits.attr('readonly', 'readonly');
            TotalUnits.addClass('TotalUnits');
            TotalUnits.addClass("form-control");
            cell.append(TotalUnits);
        }
        cell = $(row.insertCell(-1));
        var AppendAmount = $("<input  />");
        AppendAmount.attr("type", "text");
        AppendAmount.attr('readonly', 'readonly');
        AppendAmount.addClass("form-control");
        AppendAmount.addClass('Amount');
        cell.append(AppendAmount);
        cell = $(row.insertCell(-1));
        cell.html(Group);
        toastr.success('Added successfully ');
        $("#wait").css("display", "none");
    }
}
function changeTable(button) {
    //Dose and Duration
    var row = $(button).closest("TR");
    var PackSize = $("TD", row).eq(3).html();
    var UnitNumber = $("TD", row).eq(5).html();
    var Dose = $("TD", row).find(".Dose").val();

    if (isNaN(Dose) || Dose.trim() == "") {
        Dose = $("TD", row).eq(7).html();
        if (isNaN(Dose) || Dose.trim() == "") {
            Dose = 1;
        }
    }
    var Duration = $("TD", row).find(".Duration").val();
    if (isNaN(Duration) || Duration.trim() == "" || Duration == "0") {
        Duration = 1;
        $("TD", row).find(".Duration").val(Duration);
    }
    Dose = parseFloat(Dose);
    if (Dose == 0) {
        Dose = 1
        $("TD", row).find(".Dose").val(Dose);
    }
    Duration = parseFloat(Duration);
    PackSize = parseFloat(PackSize);
    UnitNumber = parseFloat(UnitNumber);

    var TotUnits = (Dose * Duration) / (PackSize / UnitNumber);
    TotUnits = Math.ceil(TotUnits);
    $("TD", row).find(".TotalUnits").val(TotUnits);
    var UnitPrice = (parseFloat($("TD", row).find(".PackagePrice").val() / parseFloat(UnitNumber))).toFixed(2);
    $("TD", row).find(".UnitPrice").val(UnitPrice);
    var Amount = (parseFloat(UnitPrice) * parseFloat($("TD", row).find(".TotalUnits").val())).toFixed(2);
    $("TD", row).find(".Amount").val(Amount);
    var TotalDuration = $("TD", row).find(".TotalDuration").val();
    if ((parseFloat(Duration)) > (parseFloat(TotalDuration))) {
        //  bootbox.alert("Total duration Must be more than Durartion ");
        $("TD", row).find(".Duration").val(TotalDuration);
        return false;
    }
    ((parseFloat(Dose)) >= 1 && (parseFloat(Dose)) <= 6) ? $("TD", row).find(".Dose").val((parseFloat(Dose))) : $("TD", row).find(".Dose").val(1);
    ((parseFloat(Duration)) < 1) ? $("TD", row).find(".Duration").val(1) : $("TD", row).find(".Duration").val((parseFloat(Duration)));
    //if ((parseFloat(Dose)) > 6) {
    //    bootbox.alert("Dose Must be less than or equal 6 ");
    //    $("TD", row).find(".Dose").val(1);
    //    return false;

    //}
    var dos = $("TD", row).find(".Dosage").val();
    var dosage = $("TD", row).eq(2).html();
    TotUnits <= 0 ? changeTable(button) : null;
    Calculation();
}
function changeTotalDuration(button) {

    var MinDay = ($('#ddlType').val() == "11601") ? 5 : 1;
    var MaxDay = ($('#ddlType').val() == "11601") ? 14 : 28;
    var row = $(button).closest("TR");
    var TotalDuration = parseFloat($("TD", row).find(".TotalDuration").val());
    var Duration = parseFloat($("TD", row).find(".Duration").val());
    Duration = isNaN(Duration) ? 1 : (MaxDay > Duration) ? Duration : MaxDay;
    TotalDuration = isNaN(TotalDuration) || TotalDuration < MinDay || TotalDuration < Duration ?
        ((Duration < MinDay) ? MinDay : Duration)
        : ((TotalDuration > MaxDay) ? MaxDay : TotalDuration);
    $("TD", row).find(".Duration").val(Duration);
    $("TD", row).find(".TotalDuration").val(TotalDuration);

    if ((parseFloat(diffDays)) < (parseFloat(Duration))) {
        //bootbox.alert("Duration Must be less than Contract days ");
        bootbox.alert("Duration Must be less than Contract days ");
        $("TD", row).find(".Duration").val(diffDays);
    }


    Calculation();
}
function changeTotalUnits(button) {

    var row = $(button).closest("TR");
    var UnitNumber = $("TD", row).eq(5).html();
    if (parseFloat($("TD", row).find(".TotalUnits").val()) > 6) {
        $("TD", row).find(".TotalUnits").val(6);
    } else if (parseFloat($("TD", row).find(".TotalUnits").val()) < 1) {
        $("TD", row).find(".TotalUnits").val(1);
    }
    if (parseFloat($("TD", row).find(".PackagePrice").val()) < 1) {
        $("TD", row).find(".PackagePrice").val(1);
    }
    //Amount value
    var UnitPrice = (parseFloat($("TD", row).find(".PackagePrice").val() / parseFloat(UnitNumber))).toFixed(2);

    $("TD", row).find(".UnitPrice").val(UnitPrice);
    var Amount = (parseFloat(UnitPrice) * parseFloat($("TD", row).find(".TotalUnits").val())).toFixed(2);
    $("TD", row).find(".Amount").val(Amount);

    Calculation();

}

function Remove(button, event) {
    var row = $(button).closest("TR");
    var name = $("TD", row).eq(0).html();
    bootbox.confirm("Do you want to delete: " + name, function (result) {
        if (result) {
            var row = $(button).closest("TR");
            var table = $("#Pharmacy")[0];
            table.deleteRow(row[0].rowIndex);
            $('#AddDiagnoise').attr('disabled', false);
            $('#RemoveDiagnoise').attr('disabled', false);
            Calculation();

        } else {
            var values = $('#AddMedicine').val();
            values.push(name);
            $('#AddMedicine').val(values).change();

        }
    });

}
function Calculation() {
    if ($("#Pharmacy >tbody TR").length != 0) {
        var sum = 0;
        var sumCash = 0;
        $('#Pharmacy TBODY TR').each(function () {
            var row = $(this);
            if (row.find("TD").eq(12).html() != "Pending") {
                sum += parseFloat(("TD", row).find(".Amount").val());
                if (row.find("TD").eq(12).html() == "Cash")
                    sumCash += parseFloat(("TD", row).find(".Amount").val());
            }
        });
        $("#txtTotalInvoice").val(sum.toFixed(2));
        $('#txtCash').val(sumCash.toFixed(2));
        $('#txtOverInsurance').val("0");
        debugger;
        var Limit = parseFloat($("#insurance_LIVEL").val());
        var co = $("#ddEmp_CEILING_PERT").val();
        var person = parseFloat(100 - co);//percentage
        //total-cash
        var total = parseFloat($('#txtTotalInvoice').val()) - sumCash;
        var cash = parseFloat($('#txtCash').val());
        var ValueCredit = 0;
        if (Limit > AnuualLimit || Limit == 0) {
            Limit = AnuualLimit;
        }
        if (Limit != 0) {
            ValueCredit = (total * (co / 100)).toFixed(2);
            if ((Limit * (co / 100)) <= (ValueCredit)) {//over insurance
                $('#txtTotalCopayment').val((Limit * (person / 100)).toFixed(2));
                Limit = (Limit * (co / 100)).toFixed(2);
                $('#txtValueCredit').val(Limit);
                $('#txtOverInsurance').val((total - Limit - parseFloat($('#txtTotalCopayment').val())).toFixed(2));
            }
            else if ((Limit * (co / 100)) > (ValueCredit)) {//no over insurance
                $('#txtValueCredit').val((total * (co / 100)).toFixed(2));
                $('#txtTotalCopayment').val((total * (person / 100)).toFixed(2));
            }
        }
        else {
            $('#txtValueCredit').val(((total) * (co / 100)).toFixed(2));
            $('#txtTotalCopayment').val(((total) * (person / 100)).toFixed(2));
        }
        $('#txtValueCash').val((sumCash + parseFloat($('#txtTotalCopayment').val()) + parseFloat($('#txtOverInsurance').val())).toFixed(2));



    }
    else {
        $('#txtTotalInvoice').val('');
        $('#txtTotalCopayment').val('');
        $('#txtValueCredit').val('');
        $('#txtOverInsurance').val('');
        $('#txtCash').val('');
        $('#txtValueCash').val('');
    }
}
function RemoveSelection(Code) {
    var values = $('#AddMedicine').val();
    if (values) {
        var i = values.indexOf(Code);
        if (i >= 0) {
            values.splice(i, 1);
            $('#AddMedicine').val(values).change();
        }
    }
}

function ClearCardData() {
    $('#txtSearchCard').val('');
    $('#PrescriptionDate').val('');
    $('#ClaimNumber').val('');
    $('#PhoneNumber').val('');
    $('#contractComp_C_ANAME').val('');
    $('#compEmp_EMP_ANAME').val('');
    $('#compEmp_INS_END_DATE').val('');
    $('#insurance_LIVEL').val('');
    $('#ddEmp_CEILING_PERT').val('');
    $('#From').val('');
    $('#To').val('');
    $("#History >tbody").empty();

}
function ClearMedicineData() {
    $("#Pharmacy >tbody").empty();
    $("#AddMedicine").empty();
    $("#ddlDiagnoises").val(null).change();

    $('#txtTotalInvoice').val('');
    $('#txtTotalCopayment').val('');
    $('#txtValueCredit').val('');
    $('#txtOverInsurance').val('');
    $('#txtCash').val('');
    $('#txtValueCash').val('');
}

