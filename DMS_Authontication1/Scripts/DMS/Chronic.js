//var url = window.location.href;
var url = new URL(window.location.href);

var id = url.searchParams.get("id");
//var id =url.substring(url.lastIndexOf('/') + 1);
//$('#id').val(id);
var sum = 0;
var Limit = 0;
var NationalId = url.searchParams.get("NationalId");
var co;
var person;
var BasicAmount;
var BasicDose;
var BasicDuration;
var BasicTotalUnits;
var BasicPackagePrice;
var AnuualLimit;
var sumNoPay = 0;
$(function () {
    ////bootbox.alert("السادة العملاء جاري تحديث بيانات العلاج الشهرى برجاء إعادة المحاولة بعد الساعة ١٢ و شكرا");
    ////add National Id
    //if (NationalId == "undefined" || NationalId == null) {
    //    bootbox.prompt({
    //        title: "Please,Enter Patient National ID  : ",
    //        centerVertical: true,
    //        closeButton: false,
    //        //required: true,
    //        //cancel: "Reset",
    //        callback: function (result) {
    //            if (result === null) {
    //                window.location = '/Pharmacy/Pharmacy';
    //                return true;
    //            }
    //            if (result === "" || result.length != 14 || isNaN(result)) {
    //                toastr.error("Invalid Value");
    //                return false;
    //            } else {
    //                //$("#wait").css("display", "block");
    //                NationalId = result;
    //                return true;

    //            }
    //            return false;

    //        }
    //    });
    //}
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Shared/VerificationCardForCode',
        data: {
            CardId: id,
        },
        success: function (r) {
            if (r.Validation == true) {
                //Verfication code
                bootbox.prompt({
                    title: "Please Enter your verification code :",
                    centerVertical: true,
                    closeButton: false,
                    inputType: 'password',
                    callback: function (result) {
                        if (result === null) {
                            window.location = '/Pharmacy/Pharmacy';
                            return true;
                        }
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: '/Shared/VerificationCode',
                            data: {
                                CardId: id,
                                VerificationCode: result
                            },
                            success: function (r) {
                                if (r.Validation == false) {
                                    alert("برجاء دخول الكود المرسل لسيادتكم علي رقم الهاتف المسجل لدي الشركة وللحصول علي الكود برجاء الاتصال علي الرقم التالي (26390990) الرقم الداخلي 110");
                                    location.reload();
                                } else {
                                    toastr.success(r.Message);
                                    return true
                                }
                            },
                            error: function (err) {
                                alert("VerificationCode,please check your internet connection1");
                                location.reload();
                            }
                        });

                    }
                });
            }
        },
        error: function (err) {
            alert("VerificationCode,please check your internet connection2");
            location.reload();
        }
    });
    //if (id.split('-')[0] == "10000") {
    //    //Verfication code
    //    bootbox.prompt({
    //        title: "Please Enter your verification code :",
    //        centerVertical: true,
    //        closeButton: false,
    //        inputType: 'password',
    //        callback: function (result) {
    //            if (result === null) {
    //                window.location = '/Pharmacy/Pharmacy';
    //                return true;
    //            }
    //            $.ajax({
    //                type: "POST",
    //                dataType: "json",
    //                url: '/Shared/VerificationCode',
    //                data: {
    //                    CardId: id,
    //                    VerificationCode: result
    //                },
    //                success: function (r) {
    //                    if (r.Validation == false) {
    //                        alert(r.Message + " ,you can call technical support at 01099887396 | 01021975433 | 01021974375");
    //                        location.reload();
    //                    } else {
    //                        toastr.success(r.Message);
    //                        return true
    //                    }
    //                },
    //                error: function (err) {
    //                    alert("VerificationCode,please check your internet connection");
    //                    location.reload();
    //                }
    //            });

    //        }
    //    });
    //}
    //////Get Ceiling and Limit



    //Get Ceiling and Limit
    GetLimit();
    $('#HasApproval').change(function () {
        if (id != '') {
            if ($(this).prop("checked")) {
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: "/Pharmacy/CheckType",
                    data: { CardId: id },
                    success: function (returndata) {
                        if (returndata == false) {
                            $.ajax({
                                type: "POST",
                                dataType: "json",
                                url: '/Pharmacy/GetLastApproval',
                                data: { CardId: id, Type: 3 },
                                success: function (returndata) {
                                    if (returndata == false) {
                                        bootbox.dialog({
                                            title: 'Reasons',
                                            message: "No Approval",
                                            buttons: {
                                                Ok: {
                                                    label: "Ok",
                                                    className: 'btn-info',
                                                    callback: function () {
                                                        $("#Approval").removeClass('active');
                                                        $(this).prop("checked", false);
                                                        $('#HasApproval').prop("checked", false)

                                                    }
                                                }
                                            }
                                        });
                                    }
                                    else {
                                        var Copayment = false;
                                        var LimitBool = false;
                                        var DisregardCeiling = false;
                                        for (var i = 0; i < returndata.length; i++) {
                                            Copayment = Copayment == true ? true : returndata[i].includes("Cancel Co-Payment");
                                            LimitBool = LimitBool == true ? true : returndata[i].includes('Disregard OverInsurance');
                                            DisregardCeiling = DisregardCeiling == true ? true : returndata[i].includes("Disregard Ceiling");

                                        }

                                        if (LimitBool == true) {
                                            Limit = 0;
                                        }
                                        if (Copayment == true) {
                                            co = 100;
                                        }
                                        if (DisregardCeiling == true) {
                                            AnuualLimit = 30000;

                                        }
                                        FirstCalculation();
                                        SecandCalculation();
                                        bootbox.dialog({
                                            title: 'Reasons',
                                            message: returndata + " ",
                                            buttons: {
                                                Ok: {
                                                    label: "Ok",
                                                    className: 'btn-info',
                                                    callback: function () {
                                                    }
                                                }
                                            }
                                        });
                                    }
                                },
                                error: function () {
                                    bootbox.dialog({
                                        title: 'Contact Us!',
                                        message: ' Call Us if you need Approval Number ' +
                                            "01271703178--01227905551  ",
                                        buttons: {
                                            Ok: {
                                                label: "Ok",
                                                className: 'btn-info',
                                                callback: function () {
                                                    location.reload();
                                                }
                                            }
                                        }
                                    });

                                }
                            });
                        }
                        else {
                            bootbox.dialog({
                                title: 'Alert!',
                                message: ' you are Vip',
                                buttons: {
                                    Ok: {
                                        label: "Ok",
                                        className: 'btn-info',
                                        callback: function () {
                                            Limit = 0;
                                            co = 100;
                                            AnuualLimit = 30000;
                                            FirstCalculation();
                                            SecandCalculation();

                                        }
                                    }
                                }
                            });
                        }
                    },
                    error: function (err) {
                        bootbox.dialog({
                            title: 'Alert!',
                            message: ' you have no Approval',
                            buttons: {
                                Ok: {
                                    label: "Ok",
                                    className: 'btn-info',
                                    callback: function () {
                                        $("#Approval").removeClass('active');
                                        $(this).prop("checked", false);
                                        $('#HasApproval').prop("checked", false)
                                    }
                                }
                            }
                        });

                    }

                });
            }
            else {

                GetLimit();

            }
        } else {
            bootbox.alert("Please Insert Card Number");
            $("#Approval").removeClass('active');

        }
    });
    $('#AddAll').click(function () {

        $('#First TBODY TR').each(function () {
            var row = $(this);
            Add(row.find("TD").eq(0));
        });
    });

    $('#RemoveAll').click(function () {
        $('#Secand TBODY TR').each(function () {
            var row = $(this);
            Remove(row.find("TD").eq(11));
        });
    });

    $('#Save').click(function () {
        //save mediciens
        var Mediciens = new Array();
        $("#Secand TBODY TR").each(function () {
            var row = $(this);
            var Medicien = {};

            Medicien.Id = row.find("TD").eq(0).html();
            // Medicien.INV_ID = INV_ID;
            Medicien.MedicienCode = row.find("TD").eq(1).html().trim();
            Medicien.MedicienName = row.find("TD").eq(2).html().trim();
            Medicien.Dose = row.find("TD").eq(8).html();
            Medicien.Duration = row.find("TD").eq(9).html();
            Medicien.TotalUnits = row.find("TD").eq(10).html();
            Medicien.TotalDuration = 28;
            Medicien.Amount = row.find("TD").eq(11).html();
            Medicien.MedicineNoPay = row.find("TD").eq(12).html().trim();
            Mediciens.push(Medicien);
        });
        var TotalValue = $('#Total2').val();
        var OverInsurance = $('#OverInsurance2').val();
        var Cash = $('#Cash2').val();
        var PersonPayment = $('#CoPayment2').val();
        var CompanyPayment = $('#Credit2').val();
        var CompanyPercent = co;
        if (Mediciens.length != 0) {
            $("#Save").attr("disabled", "disabled");
            Limit = Math.round(Limit);
            var SavePrescription = {
                hasApproval: $('#HasApproval').prop("checked") ? true : false,
                CardId: id,
                // RoshetaType: $('#ddlType').val(),
                CompanyPercent: CompanyPercent,
                Limit: Limit,
                //Speciality: $('#ddlSpeciality option:selected').text(),
                //Diagnose1: $('#Comments').val(),
                Diagnose2: NationalId,
                TotalValue: TotalValue,
                PersonPayment: PersonPayment,
                CompanyPayment: CompanyPayment,
                OverInsurance: OverInsurance,
                Cash: Cash,
                //PhoneNumber: $('#PhoneNumber').val(),
                //ClaimNumber: $('#ClaimNumber').val(),
                //createdby: $('#ddlUsers').val() == undefined ? null : $('#ddlUsers :selected').val(),
                roshitaDetail: Mediciens,
                IsFamily: $('#IsFamily').val() == '' ? null : $('#IsFamily').val(),
                IsPool: $('#IsPool').val() == '' ? null : $('#IsPool').val(),
                //diagnose: diagnose
            };
            $.ajax({
                type: 'POST',
                url: '/Chronic/SavePrescription/',
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
                            Print: {
                                label: "Print",
                                className: 'btn-info',
                                callback: function () {
                                    window.open('/Pharmacy/ControlPenelReport?id=' + OracleId);
                                    window.location = '/Pharmacy/Pharmacy';
                                    $("#submit").attr("disabled", false);
                                }
                            },
                            New: {
                                label: "New",
                                className: 'btn-info',
                                callback: function () {
                                    window.location = '/Pharmacy/Pharmacy';
                                    $("#submit").attr("disabled", false);

                                }
                            }

                        }
                    });
                },
                error: function (err) {
                    bootbox.alert("Error saving roshita,please check your internet connection");
                    $("#Save").attr("disabled", false);
                }
            });
            //$.ajax({
            //    url: '/Chronic/Save/',
            //    dataType: 'Json',
            //    type: 'POST',
            //    data: {
            //        id: id,
            //        TotalValue: TotalValue,
            //        OverInsurance: OverInsurance,
            //        Cash: Cash,
            //        PersonPayment: PersonPayment,
            //        CompanyPayment: CompanyPayment,
            //        CompanyPercent: CompanyPercent,
            //        Limit: Limit,
            //        NationalId: NationalId

            //    },
            //    success: function (Oracle_Id) {
            //        //var ReportId = r;
            //        //Date.prototype.yyyymmdd = function () {
            //        //    var mm = this.getMonth() + 1; // getMonth() is zero-based
            //        //    var dd = this.getDate();
            //        //    return [(dd > 9 ? '' : '0') + dd,
            //        //    (mm > 9 ? '' : '0') + mm,
            //        //    this.getFullYear()
            //        //    ].join('');
            //        //};
            //        //var date = new Date();
            //        //d = date.yyyymmdd()
            //        //r = d + r;
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
            //                       // window.location.reload();// = '/Pharmacy/Pharmacy';
            //                        window.open('/Pharmacy/ControlPenelReport?id=' + Oracle_Id);
            //                        window.location = '/Pharmacy/Pharmacy';

            //                        //window.location.assign("");

            //                    }
            //                },
            //                New: {
            //                    label: "Back",
            //                    className: 'btn-info',
            //                    callback: function () {
            //                        // window.history.go(-1);
            //                        window.location = '/Pharmacy/Pharmacy';
            //                    }
            //                }

            //            }
            //        });
            //        if ($('#HasApproval').prop("checked")) {
            //            $.ajax({
            //                type: 'POST',
            //                url: '/Pharmacy/SaveDealApproval/',
            //                dataType: 'Json',
            //                data: { id: id },
            //                success: function (r) {

            //                },
            //                error: function (error) {
            //                    alert(error);
            //                    //$("#save").attr("disabled", false);
            //                }
            //            });
            //        }
            //    },
            //    error: function () {
            //        bootbox.alert("Error saving roshita,please check your internet connection");
            //        //bootbox.alert("Not saved");
            //        $("#submit").attr("disabled", false);
            //    }

            //}).done(function () {
            //    $.ajax({
            //        type: 'POST',
            //        url: '/Chronic/SaveMediciens/',
            //        dataType: 'Json',
            //        contentType: "application/json; charset=utf-8",
            //        data: JSON.stringify(Mediciens),
            //        success: function (r) {
            //            $("#submit").attr("disabled", false);

            //        },
            //        error: function (err) {
            //            bootbox.alert("Error save Medicines,please check your connection");
            //            //bootbox.alert("Not saved Medicien");
            //            $("#submit").attr("disabled", false);
            //        }
            //    })

            //});
        } else { bootbox.alert("Please Insert medicines"); }
    });
});


//functions
function GetLimit() {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Pharmacy/CellingAmount',
        data: {
            id: id,
            ServiceCode: '11602'
        },
        success: function (r) {
            if (r.Validation == false) {
                //toastr.info(r.Message);
                alert(r.Message);
                window.location = '/Pharmacy/Pharmacy';
            }
            else {
                debugger;
                co = r.CeilingPert;
                AnuualLimit = r.Limit;
                //Limit = Math.round(r.CoInsurancelimit.INSURANCE_MONTH);
                //Limit = r.CoInsurancelimit.INSURANCE_MONTH;
                Limit = r.CoInsurancelimit == undefined ? AnuualLimit : r.CoInsurancelimit.INSURANCE_MONTH
                $('#IsFamily').val(r.IsFamily);
                $('#IsPool').val(r.IsPool);
                GetChronicMedData();

            }
        },
        error: function (err) {
            alert("Failed to retrieve Company Annual Limit. please check your internet connection");
            //alert("Company Annual Amount");
            location.reload();
        }
    })
}
function Add(button) {

    //Determine the reference of the Row using the Button.
    var row = $(button).closest("TR");
    var Id = $("TD", row).eq(1).html();
    var MedicienCode = $("TD", row).eq(2).html();
    var MedicienName = $("TD", row).eq(3).html();
    var DosageForm = $("TD", row).eq(4).html();
    var PackPrice = $("TD", row).eq(5).html();
    var PackSize = $("TD", row).eq(6).html();
    var UnitNo = $("TD", row).eq(7).html();
    var UnitPrice = $("TD", row).eq(8).html();
    var Dose = $("TD", row).eq(9).html();
    var Duration = $("TD", row).eq(10).html();
    var TotalUnits = $("TD", row).eq(11).html();
    var Amount = $("TD", row).eq(12).html();
    var CompanyPay = $("TD", row).eq(13).html();

    //-----------------
    //add Row.
    var tBody = $("#Secand > TBODY")[0];
    var row = tBody.insertRow(-1);
    var cell = $(row.insertCell(-1));
    cell.html(Id);
    cell = $(row.insertCell(-1));
    cell.html(MedicienCode);
    cell = $(row.insertCell(-1));
    cell.html(MedicienName);
    cell = $(row.insertCell(-1));
    cell.html(DosageForm);
    cell = $(row.insertCell(-1));
    cell.html(PackPrice);
    cell = $(row.insertCell(-1));
    cell.html(PackSize);
    cell = $(row.insertCell(-1));
    cell.html(UnitNo);
    cell = $(row.insertCell(-1));
    cell.html(UnitPrice);
    cell = $(row.insertCell(-1));
    cell.html(Dose);
    cell = $(row.insertCell(-1));
    cell.html(Duration);
    cell = $(row.insertCell(-1));
    cell.html(TotalUnits);
    cell = $(row.insertCell(-1));
    cell.html(Amount);

    cell = $(row.insertCell(-1));
    cell.html(CompanyPay);
    cell.attr("hidden", true);
    // remove button
    cell = $(row.insertCell(-1));
    var btnRemove = $("<a  />");
    btnRemove.attr("type", "button");
    btnRemove.addClass("btn btn-danger");
    btnRemove.attr("onclick", "Remove(this);");
    btnRemove.text("Remove");
    cell.append(btnRemove);
    //Edit button
    cell = $(row.insertCell(-1));
    var btnEdit = $("<a  />");
    btnEdit.attr("type", "button");
    btnEdit.addClass("btn btn-warning  glyphicon glyphicon-pencil");
    btnEdit.attr("onclick", "Edit(this);");
    btnEdit.text("Edit");
    cell.append(btnEdit);

    //remove click
    var row = $(button).closest("TR");
    var table = $("#First")[0];
    table.deleteRow(row[0].rowIndex);

    //Secand calculation
    SecandCalculation();
    //first calculation
    FirstCalculation();

};

function Remove(button) {
    //Determine the reference of the Row using the Button.
    var row = $(button).closest("TR");
    var name = $("TD", row).eq(0).html();
    bootbox.confirm("Do you want to delete: " + name, function (result) {
        if (result) {
            //Add To First Row
            var row = $(button).closest("TR");
            var Id = $("TD", row).eq(0).html();
            var MedicienCode = $("TD", row).eq(1).html();
            var MedicienName = $("TD", row).eq(2).html();
            var DosageForm = $("TD", row).eq(3).html();
            var PackPrice = $("TD", row).eq(4).html();
            var PackSize = $("TD", row).eq(5).html();
            var UnitNo = $("TD", row).eq(6).html();
            var UnitPrice = $("TD", row).eq(7).html();
            var Dose = $("TD", row).eq(8).html();
            var Duration = $("TD", row).eq(9).html();
            var TotalUnits = $("TD", row).eq(10).html();;
            var Amount = $("TD", row).eq(11).html();
            var CompanyPay = $("TD", row).eq(12).html();
            //-----------------
            var tBody = $("#First > TBODY")[0];
            //Add Row.
            var row = tBody.insertRow(-1);
            var cell = $(row.insertCell(-1));
            //Add Button
            var btnAdd = $("<a  />");
            btnAdd.attr("type", "button");
            btnAdd.addClass("btn btn-primary");
            btnAdd.attr("onclick", "Add(this);");
            btnAdd.text("Dispense");
            cell.append(btnAdd);

            cell = $(row.insertCell(-1));
            cell.html(Id);
            cell = $(row.insertCell(-1));
            cell.html(MedicienCode);
            cell = $(row.insertCell(-1));
            cell.html(MedicienName);
            cell = $(row.insertCell(-1));
            cell.html(DosageForm);
            cell = $(row.insertCell(-1));
            cell.html(PackPrice);
            cell = $(row.insertCell(-1));
            cell.html(PackSize);
            cell = $(row.insertCell(-1));
            cell.html(UnitNo);
            cell = $(row.insertCell(-1));
            cell.html(UnitPrice);
            cell = $(row.insertCell(-1));
            cell.html(Dose);
            cell = $(row.insertCell(-1));
            cell.html(Duration);
            cell = $(row.insertCell(-1));
            cell.html(TotalUnits);
            cell = $(row.insertCell(-1));
            cell.html(Amount);
            cell = $(row.insertCell(-1));
            cell.html(CompanyPay);
            cell.attr("hidden", true);

            //Delete the Table row using it's Index.
            //---------------------------
            var row = $(button).closest("TR");
            var table = $("#Secand")[0];
            table.deleteRow(row[0].rowIndex);

            //Secand calculation
            SecandCalculation();
            //first calculation
            FirstCalculation();
        }
    });

}

function Edit(button) {

    //Add To First Row
    var row = $(button).closest("TR");
    var Id = $("TD", row).eq(0).html();
    var MedicienCode = $("TD", row).eq(1).html();
    var MedicienName = $("TD", row).eq(2).html();
    var DosageForm = $("TD", row).eq(3).html();
    var PackPrice = $("TD", row).eq(4).html();
    var PackSize = $("TD", row).eq(5).html();
    var UnitNo = $("TD", row).eq(6).html();
    var UnitPrice = $("TD", row).eq(7).html();
    var Dose = $("TD", row).eq(8).html();
    var Duration = $("TD", row).eq(9).html();
    var TotalUnits = $("TD", row).eq(10).html();;
    var Amount = $("TD", row).eq(11).html();

    $('.modal-body #Id').val(parseInt(Id));
    $('.modal-body #MedicienCode').val(MedicienCode);
    $('.modal-body #MedicienName').val(MedicienName);
    $('.modal-body #Dosage').val(DosageForm);
    $('.modal-body #PackagePrice').val(parseFloat(PackPrice));
    $('.modal-body #PackageSize').val(parseInt(PackSize));
    $('.modal-body #UnitNumber').val(parseInt(UnitNo));
    $('.modal-body #UnitPrice').val(parseFloat(UnitPrice));
    $('.modal-body #Dose').val(parseInt(Dose));
    $('.modal-body #Duration').val(parseInt(Duration));
    $('.modal-body #TotalUnits').val(parseInt(TotalUnits));
    $('.modal-body #Amount').val(parseFloat(Amount));

    BasicAmount = Amount;
    BasicPackagePrice = PackPrice;
    BasicDose = Dose;
    BasicDuration = Duration;
    BasicTotalUnits = TotalUnits;
    $('#exampleModal').modal();


}
function FirstCalculation() {
    sumNoPay = 0;
    sum = 0;
    $('#First TBODY TR').each(function () {
        var row = $(this);
        sum += parseFloat(row.find("TD").eq(12).html());
        //var x = row.find("TD").eq(13).html().trim();
        if (row.find("TD").eq(13).html().trim() == "yes" || row.find("TD").eq(13).html().trim() == "Yes") {
            sumNoPay += parseFloat(row.find("TD").eq(12).html());
        }
    });
    $('#Total').val(sum.toFixed(2));
    $('#OverInsurance').val("0");
    var CurrentLimit = Limit;

    person = parseFloat(100 - co);
    //total-cash
    var total = sum - sumNoPay;
    var ValueCredit = 0;
    if ((CurrentLimit > AnuualLimit && CurrentLimit != .001) || CurrentLimit == 0) {
        CurrentLimit = AnuualLimit;
    }
    if (CurrentLimit != 0) {
        ValueCredit = (total * (co / 100)).toFixed(2);
        if ((CurrentLimit * (co / 100)) <= (ValueCredit) && co != 0) {
            $('#CoPayment').val((CurrentLimit * (person / 100)).toFixed(2));
            CurrentLimit = (CurrentLimit * (co / 100)).toFixed(2);
            var credit = (parseFloat(CurrentLimit) + parseFloat(sumNoPay)).toFixed(2);
            $('#Credit').val(credit);
            $('#OverInsurance').val((total - CurrentLimit - parseFloat($('#CoPayment').val())).toFixed(2));
        }
        else if ((CurrentLimit * (co / 100)) > (ValueCredit) || co == 0) {
            $('#Credit').val(((total * (co / 100)) + sumNoPay).toFixed(2));
            $('#CoPayment').val((total * (person / 100)).toFixed(2));
        }
    }
    else {
        $('#Credit').val((((total) * (co / 100)) + sumNoPay).toFixed(2));
        $('#CoPayment').val(((total) * (person / 100)).toFixed(2));
    }
    $('#Cash').val((parseFloat($('#CoPayment').val()) + parseFloat($('#OverInsurance').val())).toFixed(2));

}
function SecandCalculation() {
    debugger;
    sumNoPay = 0;
    sum = 0;

    $('#Secand TBODY TR').each(function () {
        var row = $(this);
        sum += parseFloat(row.find("TD").eq(11).html());
        if (row.find("TD").eq(12).html().trim() == "Yes" || row.find("TD").eq(12).html().trim() == "yes") {
            sumNoPay += parseFloat(row.find("TD").eq(11).html());
        }
    });
    $('#Total2').val(sum.toFixed(2));
    $('#OverInsurance2').val("0");
    var CurrentLimit = Limit;

    person = parseFloat(100 - co);
    //total-cash
    var total = sum - sumNoPay;
    var ValueCredit = 0;
    if ((CurrentLimit > AnuualLimit && CurrentLimit != .001) || CurrentLimit == 0) {
        CurrentLimit = AnuualLimit;
    }
    if (CurrentLimit != 0) {
        ValueCredit = (total * (co / 100)).toFixed(2);
        if ((CurrentLimit * (co / 100)) <= (ValueCredit) && co != 0) {
            $('#CoPayment2').val((CurrentLimit * (person / 100)).toFixed(2));
            CurrentLimit = (CurrentLimit * (co / 100)).toFixed(2);
            var credit2 = (parseFloat(CurrentLimit) + parseFloat(sumNoPay)).toFixed(2);
            $('#Credit2').val(credit2);
            $('#OverInsurance2').val((total - CurrentLimit - parseFloat($('#CoPayment2').val())).toFixed(2));
        }
        else if ((CurrentLimit * (co / 100)) > (ValueCredit) || co == 0) {
            $('#Credit2').val(((total * (co / 100)) + sumNoPay).toFixed(2));
            $('#CoPayment2').val((total * (person / 100)).toFixed(2));
        }
    }
    else {
        $('#Credit2').val((((total) * (co / 100)) + sumNoPay).toFixed(2));
        $('#CoPayment2').val(((total) * (person / 100)).toFixed(2));
    }
    $('#Cash2').val((parseFloat($('#CoPayment2').val()) + parseFloat($('#OverInsurance2').val())).toFixed(2));




}
function GetChronicMedData() {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Chronic/GetChronicMedData',
        data: { id: id },
        success: function (returndata) {
            if (returndata.ok && returndata.medCard != null) {
                if (returndata.medCard.NO_PAY == 1) {
                    co = 100;
                }
                if (returndata.medCard.NO_OVER == 1) {
                    Limit = 0;
                }
                FirstCalculation();
                SecandCalculation();
            }
            else {
                bootbox.alert('No Limit Amount ,please check your internet connection  ');
            }
        }
    });
}