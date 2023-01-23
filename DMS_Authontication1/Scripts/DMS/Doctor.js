//var url = window.location.href;

//var id = url.substring(url.lastIndexOf('/') + 1);
var url = new URL(window.location.href);
var id = url.searchParams.get("id");
var NationalId = url.searchParams.get("NationalId");
$('#id').val(id);
var Manager;
var sum = 0;
var Limit;
var co;
var person;
var BasicAmount;
var BasicDose;
var BasicDuration;
var BasicTotalUnits;
var AnuualLimit;

$(function () {
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
    //GetLimit();
    var TxtSearch = parseInt($('#TxtSearch').val());
    $('#TxtSearch').click(function () {
        $("#wait").css("display", "block");

        $.ajax({
            url: '/Doctor/approvals/',
            data: { id: id },
            dataType: 'json',
            success: function (r) {
                $("#wait").css("display", "none");
                var setData = $("#Approvals Tbody");
                setData.empty();
                for (var i = 0; i < r.length; i++) {
                    if (r[i].CreatedDate != null) {
                        var MyDate_String_Value = r[i].CreatedDate;
                        var value = new Date
                            (
                                parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                            );
                        var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();

                    }
                    else {
                        dat = null;
                    }
                    var data = "<tr >" +
                        "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='Select(this);'></Button>" + "</td>" +
                        "<td>" + r[i].Id + "</td>" +
                        "<td>" + dat + "</td>"

                    "</tr>"
                    setData.append(data);

                }
                $('#Approvals').DataTable();
                $('#ApprovalsModal').modal();
            },
            error: function (r) {
                alert('Rrror retrive companies');
                $("#wait").css("display", "none");

            }

        });

    });
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
                                data: { CardId: id, Type: 3},
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
                                            Limit=0;
                                            //SecandCalculation();
                                        }
                                        if (Copayment == true) {
                                            co=100;
                                            //SecandCalculation();
                                        }
                                        if (DisregardCeiling == true) {
                                            AnuualLimit = 30000;
                                            
                                        }
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
                        } else {
                            bootbox.dialog({
                                title: 'Alert!',
                                message: ' you are Vip',
                                buttons: {
                                    Ok: {
                                        label: "Ok",
                                        className: 'btn-info',
                                        callback: function () {
                                            Limit=0;
                                            co=100;
                                            AnuualLimit = 30000;
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
                                    }
                                }
                            }
                        });

                    }

                });
            } else {
                GetLimit();
                //$("#Approval").removeClass('active');
                //$('#HasApproval').attr("checked", false);

            }
        } else {
            bootbox.alert("Please Insert Card Number");
            $("#Approval").removeClass('active');

        }
    });
    $('#RemoveAll').click(function () {
        $('#Secand TBODY TR').each(function () {
            var row = $(this);
            Remove(row.find("TD").eq(6));
        });
    });
    $('#Save').click(function () {
        var Mediciens = new Array();
        $("#Secand TBODY TR").each(function () {
            var row = $(this);
            var Medicien = {};
            Medicien.Id = row.find("TD").eq(0).html();
            Medicien.MedicienCode = row.find("TD").eq(1).html();
            Medicien.MedicienName = row.find("TD").eq(2).html();
            Medicien.Dose = row.find("TD").eq(8).html();
            Medicien.Duration = row.find("TD").eq(9).html();
            Medicien.TotalDuration = row.find("TD").eq(10).html();
            Medicien.TotalUnits = row.find("TD").eq(11).html();
            Medicien.Amount = row.find("TD").eq(12).html();
            //Medicien.PaymentGroup=
            Mediciens.push(Medicien);
        });
        var TotalValue = $('#Total2').val();
        var OverInsurance = $('#OverInsurance2').val();
        var Cash = $('#Cash2').val();
        var PersonPayment = $('#CoPayment2').val();
        var CompanyPayment = $('#Credit2').val();
        if (Mediciens.length != 0) {
            var SavePrescription = {
                hasApproval: $('#HasApproval').prop("checked") ? true : false,
                Id: ApprovalId,
                CardId: id,
                // RoshetaType: $('#ddlType').val(),
                //CompanyPercent: CompanyPercent,
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
                //diagnose: diagnose
            };
            $.ajax({
                type: 'POST',
                url: '/Doctor/SavePrescription/',
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
                    $("#submit").attr("disabled", false);
                }
            });
            //$.ajax({
            //    url: '/Doctor/Save/',
            //    dataType: 'Json',
            //    type: 'POST',
            //    data: {
            //        id: id,
            //        txt: ApprovalId,
            //        TotalValue: TotalValue,
            //        OverInsurance: OverInsurance,
            //        Cash: Cash,
            //        PersonPayment: PersonPayment,
            //        CompanyPayment: CompanyPayment,
            //        NationalId: NationalId

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
            //        //r = d + r;
            //        //approval = r;

            //        bootbox.dialog({
            //            closeButton: false,
            //            title: 'Added Sucessfully',
            //            message: "Roshita ID : " + Oracle_Id,
            //            buttons: {
            //                Print: {
            //                    label: "Print",
            //                    className: 'btn-info',
            //                    callback: function () {
            //                        window.open('/Pharmacy/ControlPenelReport?id=' + Oracle_Id);
            //                        window.location.reload();

            //                    }
            //                },
            //                New: {
            //                    label: "New",
            //                    className: 'btn-info',
            //                    callback: function () {
            //                        window.location.reload();
            //                    }
            //                }

            //            }
            //        });
            //        //Approval
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
            //        bootbox.alert("Failed to save Precription");
            //    }

            //}).done(function () {
            //    $.ajax({
            //        type: 'POST',
            //        url: '/Doctor/SaveMediciens/',
            //        dataType: 'Json',
            //        contentType: "application/json; charset=utf-8",
            //        data: JSON.stringify(Mediciens),
            //        success: function (r) {

            //        },
            //        error: function (err) {
            //            bootbox.alert("Error Medicien");
            //        }
            //    });
            //});
        } else { bootbox.alert("there is no medicines"); }
    });
});


//functions
function GetLimit() {

    //Co-Payment
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Pharmacy/CellingAmount',
        data: {
            id: id,
            ServiceCode: '11601'
        },
        success: function (r) {
            if (r.Validation == false) {
                // toastr.info(r.Message);
                //ClearCardData();
                alert(r.Message);
                window.location.reload();

            } else {
                co = r.CeilingPert;
                AnuualLimit = r.Limit;
                if (r.LimitDailyPreceptionCount && r.CoInsurancelimit.INSURANCE_DAY >= 0) {
                    Limit = r.CoInsurancelimit.INSURANCE_DAY;
                } else {
                    alert(" لقد تم استهلاك العدد المحدد للروشتات وسوف تكون خارج التغطه ");
                    Limit = 0.001;
                    co = 0;
                }


            }
            SecandCalculation();
        },
        error: function (err) {
            alert("Failed to retrieve Company Annual Limit. please check your internet connection");
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
    var TotalDuration = $("TD", row).eq(11).html();
    var TotalUnits = $("TD", row).eq(12).html();
    var Amount = $("TD", row).eq(13).html();
    //-----------------
    var tBody = $("#Secand > TBODY")[0];
    //Add Row.
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
    cell.html(TotalDuration);
    cell = $(row.insertCell(-1));
    cell.html(TotalUnits);
    cell = $(row.insertCell(-1));
    cell.html(Amount);

    //button
    cell = $(row.insertCell(-1));
    var btnRemove = $("<a  />");
    btnRemove.attr("type", "button");
    btnRemove.addClass("btn btn-danger");
    btnRemove.attr("onclick", "Remove(this);");
    btnRemove.text("Remove");
    cell.append(btnRemove);
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
    // $(this).parent().attr('disabled',true);

    SecandCalculation();

}

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
            var TotalDuration = $("TD", row).eq(10).html();
            var TotalUnits = $("TD", row).eq(11).html();;
            var Amount = $("TD", row).eq(12).html();
            //-----------------
            var tBody = $("#First > TBODY")[0];
            //Add Row.
            var row = tBody.insertRow(-1);
            var cell = $(row.insertCell(-1));
            //button

            var btnAdd = $("<a  />");
            btnAdd.attr("type", "button");
            btnAdd.addClass("btn btn-primary glyphicon glyphicon-plus");
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
            cell.html(TotalDuration);
            cell = $(row.insertCell(-1));
            cell.html(TotalUnits);
            cell = $(row.insertCell(-1));
            cell.html(Amount);

            //Delete the Table row using it's Index.
            //---------------------------
            var row = $(button).closest("TR");
            var table = $("#Secand")[0];
            table.deleteRow(row[0].rowIndex);
            //calculation
            SecandCalculation();

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
    var TotalDuration = $("TD", row).eq(10).html();
    var TotalUnits = $("TD", row).eq(11).html();;
    var Amount = $("TD", row).eq(12).html();
    //alert(parseInt(TotalUnits));
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
    $('.modal-body #TotalDuration').val(parseInt(TotalDuration));
    $('.modal-body #TotalUnits').val(parseInt(TotalUnits));
    $('.modal-body #Amount').val(parseFloat(Amount));
    //$('.modal-body #      ').val();

    BasicAmount = Amount;
    BasicDose = Dose;
    BasicDuration = Duration;
    BasicTotalUnits = TotalUnits;
    //  $("ModalTitle").html("Edit ");
    $('#exampleModal').modal();


}
function SecandCalculation() {
    //Secend calculation
    debugger;
    var sum = 0;
    $('#Secand TBODY TR').each(function () {
        var row = $(this);
        sum += parseFloat(row.find("TD").eq(12).html());
    });
    $('#Total2').val(sum.toFixed(2));
    $('#OverInsurance2').val("0");
    //var Limit = parseFloat(r.Limit);
    var person = parseFloat(100 - co);
    //total-cash
    var total = sum;
    var CurrentLimit = Limit;
    if (CurrentLimit > AnuualLimit || CurrentLimit == 0) {
        CurrentLimit = AnuualLimit;
    }
    if (CurrentLimit != 0) {
        ValueCredit = (total * (co / 100)).toFixed(2);
        if ((CurrentLimit * (co / 100)) <= (ValueCredit)) {
            $('#CoPayment2').val((CurrentLimit * (person / 100)).toFixed(2));
            CurrentLimit = (CurrentLimit * (co / 100)).toFixed(2);
            $('#Credit2').val(CurrentLimit);
            $('#OverInsurance2').val((total - CurrentLimit - parseFloat($('#CoPayment2').val())).toFixed(2));
        }
        else if ((CurrentLimit * (co / 100)) > (ValueCredit)) {
            $('#Credit2').val((total * (co / 100)).toFixed(2));
            $('#CoPayment2').val((total * (person / 100)).toFixed(2));
        }
    }
    else {
        $('#Credit2').val(((total) * (co / 100)).toFixed(2));
        $('#CoPayment2').val(((total) * (person / 100)).toFixed(2));
    }
    $('#Cash2').val((parseFloat($('#CoPayment2').val()) + parseFloat($('#OverInsurance2').val())).toFixed(2));

    //sum = 0;
    //$('#Secand TBODY TR').each(function () {
    //    var row = $(this);
    //    sum += parseFloat(row.find("TD").eq(11).html());
    //});
    //$('#Total2').val(sum.toFixed(2));
    //$('#OverInsurance2').val("0");
    //var CurrentLimit = Limit;

    //person = parseFloat(100 - co);
    ////total-cash
    //var total = sum;
    //var ValueCredit = 0;
    //if (CurrentLimit > AnuualLimit || CurrentLimit == 0) {
    //    CurrentLimit = AnuualLimit;
    //}
    //if (CurrentLimit != 0) {
    //    ValueCredit = (total * (co / 100)).toFixed(2);
    //    if ((CurrentLimit * (co / 100)) <= (ValueCredit)) {
    //        $('#CoPayment2').val((CurrentLimit * (person / 100)).toFixed(2));
    //        CurrentLimit = (CurrentLimit * (co / 100)).toFixed(2);
    //        $('#Credit2').val(CurrentLimit);
    //        $('#OverInsurance2').val((total - CurrentLimit - parseFloat($('#CoPayment2').val())).toFixed(2));
    //    }
    //    else if ((CurrentLimit * (co / 100)) > (ValueCredit)) {
    //        $('#Credit2').val((total * (co / 100)).toFixed(2));
    //        $('#CoPayment2').val((total * (person / 100)).toFixed(2));
    //    }
    //}
    //else {
    //    $('#Credit2').val(((total) * (co / 100)).toFixed(2));
    //    $('#CoPayment2').val(((total) * (person / 100)).toFixed(2));
    //}
    //$('#Cash2').val((parseFloat($('#CoPayment2').val()) + parseFloat($('#OverInsurance2').val())).toFixed(2));




}
