
var CardId;
var CompId;
var Manager;
var ServiceCode;
var url = window.location.href;
var id = url.substring(url.lastIndexOf('/') + 1);
var fixedLimit = 0;
var Limit = 0;
var AnuualLimit = 0;
var CeilingPert = 0;
var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
var firstDate;
var NationalId;
const secondDate = new Date();
var diffDays = 0;
var sumNoPay = 0;
var FixedTotalUnits = [];
$(function () {
    $("#wait").css("display", "block");


    $('#Pharmacy tbody tr').each(function () {
        var row = $(this);
        var currentMedicine = parseInt(row.find("TD").eq(0).html());
        var newOption = new Option(row.find("TD").eq(1).html(), currentMedicine, true, true);
        $('#AddMedicine').append(newOption).trigger('change');
    });
    $("#Backbutton").click(function () {
        window.location.replace('/Pharmacy/index?id=undefined');
    });
    $.get('/Pharmacy/Manger/', { id: id }, function (data) {
        Manager = data.Manager;
        ServiceCode = data.RoshetaType;
        CardId = data.CardId;
        CompId = CardId.split('-')[0];
        if (ServiceCode == "11602") {//Chronic
            $('.PackagePrice').attr('disabled', true);
            $('.Dose').attr('disabled', true);
            $('.Duration').attr('disabled', true);
            $('.TotalDuration').attr('disabled', true);
            $('.TotalUnits').attr('readonly', false);
            $('#Pharmacy tbody tr').each(function () {
                var row = $(this);
                FixedTotalUnits.push(parseInt($("TD", row).find(".TotalUnits").val()));
            });
        }
    }).done(function () {
        //GetActivation
        $.ajax({
            type: "POST",
            dataType: "json",
            url: '/Pharmacy/GetCompActivation',
            data: { id: CompId, CardId: CardId },
            success: function (returndata) {
                if (returndata.ok) {
                    if (returndata.data == "Yes") {

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
                                        window.location.replace('/Pharmacy/index?id=undefined');
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
                                        window.location.replace('/Pharmacy/index?id=undefined');
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
                                        window.location.replace('/Pharmacy/index?id=undefined');
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

        //Get Ceiling and Limit
        getlimit();

    }).done(function () {
        $.ajax({
            dataType: "json",
            url: '/Pharmacy/GetSecondContract',
            data: {
                id: CardId
            },
            success: function (r) {
                if (r.check == 1) {
                    firstDate = new Date(parseFloat(r.Employee.INS_END_DATE.replace(/(^.*\()|([+-].*$)/g, '')));
                    diffDays = Math.round(Math.abs((firstDate - secondDate) / oneDay));
                }
                else {
                    diffDays = 0;
                }
            },
            error: function (r) {
                diffDays = 0;
            }

        });
        $("#wait").css("display", "none");
    })

    //Mediciences
    $("#AddMedicine").select2({
        placeholder: "Select a medicine",
        ajax: {
            url: '/Pharmacy/GetList',
            dataType: 'json',
            data: function (params) {
                var query = {
                    sEcho: params.page || 1,
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
        if (ServiceCode != "11602") {
            if (CompId == "8887700") {
                SelectMedicienCompany(event);
            }
            else {
                SelectMedicien(event);
            }
        }
        else {
            toastr.warning('Can not add chronic medicine');
            event.preventDefault();

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

    $('#Update').click(function () {
        var dose = $('#Dosee').val();
        var duration = $('#Durationn').val();
        var totalduration = $('#TotalDuration').val();
        var Mediciens = new Array();
        $("#Pharmacy TBODY TR").each(function () {

            var row = $(this);
            var Dose = $("TD", row).find(".Dose").val();
            var Duration = $("TD", row).find(".Duration").val();
            var Medicien = {};
            Medicien.RoshitaID = id;
            Medicien.MedicienCode = row.find("TD").eq(0).html().trim();
            Medicien.MedicienName = row.find("TD").eq(1).html().trim();
            Medicien.Dose = $("TD", row).find(".Dose").val();
            if (Medicien.Dose == undefined) {
                Medicien.Dose = $("TD", row).eq(7).html();
            }
            Medicien.Duration = $("TD", row).find(".Duration").val();
            Medicien.TotalDuration = $("TD", row).find(".TotalDuration").val();
            Medicien.TotalUnits = $("TD", row).find(".TotalUnits").val();
            Medicien.Amount = $("TD", row).find(".Amount").val();
            Medicien.PaymentGroup = row.find("TD").eq(12).html().trim();
            if (ServiceCode == "11602") {
                if (row.find("TD").eq(13).html().trim() == "Yes") {
                    Medicien.MedicineNoPay = row.find("TD").eq(13).html().trim();
                }
            }
            Mediciens.push(Medicien);
        });
        if (Mediciens.length != 0) {
            $("#Update").attr("disabled", "disabled");
            if ($('#txtTotalInvoice').val() != 0 && $('#txtTotalInvoice').val() != undefined && $('#txtTotalInvoice').val() != "") {
                var UpdatePrescription = {
                    Id: id,
                    // hasApproval: $('#HasApproval').prop("checked") ? true : false,
                    // CardId: $('#txtSearchCard').val(),
                    // RoshetaType: $('#ddlType').val(),
                    // CompanyPercent: $('#ddEmp_CEILING_PERT').val(),
                    // Limit: $('#insurance_LIVEL').val(),
                    // Speciality: $('#ddlSpeciality option:selected').text(),
                    // Diagnose1: $('#Comments').val(),
                    // Diagnose2: NationalId,
                    TotalValue: $('#txtTotalInvoice').val(),
                    PersonPayment: $('#txtTotalCopayment').val(),
                    CompanyPayment: $('#txtValueCredit').val(),
                    OverInsurance: $('#txtOverInsurance').val(),
                    Cash: $('#txtValueCash').val(),
                    // PhoneNumber: $('#PhoneNumber').val(),
                    // ClaimNumber: $('#ClaimNumber').val(),
                    // createdby: $('#ddlUsers').val() == undefined ? null : $('#ddlUsers :selected').val(),
                    roshitaDetail: Mediciens,
                    IsFamily: $('#IsFamily').val() == '' ? null : $('#IsFamily').val(),
                    IsPool: $('#IsPool').val() == '' ? null : $('#IsPool').val(),
                    // diagnose: diagnose
                };
                $.ajax({
                    type: 'POST',
                    url: '/Pharmacy/UpdatePrescription/',
                    dataType: 'Json',
                    //contentType: "application/json; charset=utf-8",
                    //data: JSON.stringify(SavePrescriptipn),
                    data: UpdatePrescription,
                    success: function (OracleId) {
                        bootbox.dialog({
                            closeButton: false,
                            title: 'Added Sucessfully',
                            message: " يرجي اعادة الطباعه حيث ان الكليم القديم يعتبر لاغي وعدم الطباعه سيؤدي الي خصم الكيم بالكامل علي مقدم الخدمه " + "Approval Number : " + OracleId,
                            buttons: {
                                Print: {
                                    label: "Print",
                                    className: 'btn-info',
                                    callback: function () {
                                        //window.location.reload();
                                        $("#Update").attr("disabled", "disabled");
                                        ClearCardData();
                                        ClearMedicineData();
                                        window.open('/Pharmacy/ControlPenelReport?id=' + OracleId);
                                        //window.location = "/Pharmacy/index";
                                    }
                                },
                                New: {
                                    label: "New",
                                    className: 'btn-info',
                                    callback: function () {
                                        window.location = "/Pharmacy/index";
                                        $("#Update").attr("disabled", "disabled");
                                        //window.location.reload();
                                    }
                                }

                            }
                        });

                    },
                    error: function (err) {
                        bootbox.alert("Error saving roshita,please check your internet connection");
                        $("#Update").attr("disabled", false);
                    }
                });

            }
            else {
                bootbox.alert("Please wait untaill data load correctly");
                $("#Update").attr("disabled", false);
            }
        }
        else {
            bootbox.alert("Please Insert medicines");
            $("#Update").attr("disabled", false);
        }

    })
});
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
    var sum = 0;
    var sumCash = 0;
    sumNoPay = 0;
    $('#Pharmacy TBODY TR').each(function () {
        var row = $(this);
        if (row.find("TD").eq(12).html().trim() != "Pending") {
            sum += parseFloat(("TD", row).find(".Amount").val());
            if (row.find("TD").eq(12).html().trim() == "Cash" || row.find("TD").eq(12).html().trim() == "Rejected" || row.find("TD").eq(12).html().trim() == "Pending")
                sumCash += parseFloat(("TD", row).find(".Amount").val());
        }
        if (ServiceCode == "11602") {
            if (row.find("TD").eq(13).html().trim() == "Yes") {
                sumNoPay += parseFloat(("TD", row).find(".Amount").val());
            }
        }
    });
    $("#txtTotalInvoice").val(sum.toFixed(2));
    $('#txtCash').val(sumCash);
    $('#txtOverInsurance').val("0");
    if (ServiceCode == "11601") {
        var Limit = fixedLimit;
        var co = CeilingPert;//precentage
        var person = parseFloat(100 - co);//percentage
        //total-cash
        var total = parseFloat($('#txtTotalInvoice').val()) - sumCash;
        var cash = parseFloat($('#txtCash').val());
        var ValueCredit = 0;

        //if (Limit != 0) {
        //    Limit += total * (co / 100);
        //}
        //AnuualLimit += total * (co / 100);
        if (Limit > AnuualLimit || Limit == 0) {
            Limit = AnuualLimit;
        }
        if (Limit != 0) {
            ValueCredit = (total * (co / 100)).toFixed(2);
            if ((Limit * (co / 100)) <= (ValueCredit)) {
                $('#txtTotalCopayment').val((Limit * (person / 100)).toFixed(2));
                Limit = (Limit * (co / 100)).toFixed(2);
                $('#txtValueCredit').val(Limit);
                $('#txtOverInsurance').val((total - Limit - parseFloat($('#txtTotalCopayment').val())).toFixed(2));
            }
            else if ((Limit * (co / 100)) > (ValueCredit)) {
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
    else if (ServiceCode == "11603") {
        //monthly
        var Limit = fixedLimit;
        var co = CeilingPert;
        var person = parseFloat(100 - co);
        //total-cash
        var total = parseFloat($('#txtTotalInvoice').val()) - sumCash;
        var cash = parseFloat($('#txtCash').val());
        var ValueCredit = 0;
        //if (Limit != 0) {
        //    Limit += total * (co / 100);
        //}
        // AnuualLimit += total * (co / 100);
        if (Limit > AnuualLimit || Limit == 0) {
            Limit = AnuualLimit;
        }
        if (Limit != 0) {
            ValueCredit = (total * (co / 100)).toFixed(2);
            if ((Limit * (co / 100)) <= (ValueCredit)) {
                $('#txtTotalCopayment').val((Limit * (person / 100)).toFixed(2));
                Limit = (Limit * (co / 100)).toFixed(2);
                $('#txtValueCredit').val(Limit);//(Limit * (co / 100)).toFixed(2)
                $('#txtOverInsurance').val((total - Limit - parseFloat($('#txtTotalCopayment').val())).toFixed(2));
            }
            else if ((Limit * (co / 100)) > (ValueCredit)) {
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
    else if (ServiceCode == "11602") {
        var Limit = fixedLimit;

        co = CeilingPert;
        person = parseFloat(100 - co);
        //total-cash
        var total = sum - sumNoPay;
        //if (Limit != 0) {
        //    Limit += total * (co / 100);
        //}
        //AnuualLimit += total * (co / 100);
        if (Limit > AnuualLimit || Limit == 0) {
            Limit = AnuualLimit;
        }
        if (Limit < (total) && Limit != 0) {
            $('#txtValueCredit').val((Limit * (co / 100) + parseFloat(sumNoPay)).toFixed(2));
            $('#txtTotalCopayment').val((Limit * (person / 100)).toFixed(2));
            $('#txtOverInsurance').val((total - Limit).toFixed(2));
        }
        else if (Limit > (total) && Limit != 0) {
            $('#txtValueCredit').val(((total * (co / 100)) + parseFloat(sumNoPay)).toFixed(2));
            $('#txtTotalCopayment').val((total * (person / 100)).toFixed(2));

        }
        else {
            $('#txtValueCredit').val(((total * (co / 100)) + parseFloat(sumNoPay)).toFixed(2));
            $('#txtTotalCopayment').val((total * (person / 100)).toFixed(2));

        }
        //var totalcash = (sumCash + parseInt($('#txtTotalCopayment').val())).toFixed(2);
        // $('#txtCash').val((parseFloat($('#txtTotalCopayment').val()) + parseFloat($('#txtOverInsurance').val())).toFixed(2));
        $('#txtValueCash').val((sumCash + parseFloat($('#txtTotalCopayment').val()) + parseFloat($('#txtOverInsurance').val())).toFixed(2));

    }
    else {

        var co = CeilingPert;
        var person = parseFloat(100 - co);
        //total-cash
        var total = parseFloat($('#txtTotalInvoice').val()) - sumCash;
        var cash = parseFloat($('#txtCash').val());
        var ValueCredit = 0;
        if (Limit != 0) {
            ValueCredit = (total * (co / 100)).toFixed(2);
            if (Limit < (ValueCredit)) {
                $('#txtTotalCopayment').val((Limit * (person / 100)).toFixed(2));
                Limit = (Limit * (co / 100)).toFixed(2);
                $('#txtValueCredit').val(Limit);//(Limit * (co / 100)).toFixed(2)
                $('#txtOverInsurance').val(total - Limit - parseFloat($('#txtTotalCopayment').val()).toFixed(2));
            }
            else if (Limit > (ValueCredit)) {
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


}
function SelectMedicienCompany(event) {
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
    var edit = 0;
    var isChronic = 0;
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
                medicineGroup.M_CODE = parseInt(row.find("TD").eq(0).html());
                medicineGroups.push(medicineGroup);
                if (parseInt(row.find("TD").eq(0).html()) == parseInt(MedicienCode)) {
                    edit = 1;
                    event.preventDefault();
                    toastr.error('Added before');
                    $("#wait").css("display", "none");

                }
            });

            if (edit == 0) {
                Group = "Pending";
                AppendRow();
            }

            $("#wait").css("display", "none");

        },
        error: function (ex) {
            bootbox.alert('Failed to retrieve Medicine Data,please chack your internet connection');
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
        var PackagePrice = $("<input />");
        PackagePrice.attr("type", "text");
        PackagePrice.addClass("form-control");
        PackagePrice.addClass("PackagePrice");
        PackagePrice.attr("onkeyup", "changeTotalUnits(this);");
        PackagePrice.val(PackPrice);
        cell.append(PackagePrice);
        cell = $(row.insertCell(-1));
        cell.html(UnitNumber);
        cell = $(row.insertCell(-1));
        var unitprice = $("<input />");
        unitprice.attr("type", "text");
        unitprice.attr('readonly', 'readonly');
        unitprice.addClass("form-control");
        unitprice.addClass('UnitPrice');
        cell.append(unitprice);
        cell = $(row.insertCell(-1));
        var Dose = $("<input />");
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
        var Duration = $("<input />");
        Duration.attr("type", "text");
        Duration.addClass("form-control");
        Duration.addClass("Duration");
        Duration.attr("onkeyup", "changeTotalDuration(this);changeTable(this);");
        cell.append(Duration);
        cell = $(row.insertCell(-1));
        var TotalDuration = $("<input />");
        TotalDuration.attr("type", "text");
        TotalDuration.addClass("form-control");
        TotalDuration.addClass("TotalDuration");
        TotalDuration.attr("onfocusout", "changeTotalDuration(this);");
        cell.append(TotalDuration);
        cell = $(row.insertCell(-1));
        if (DosageForm == "ELIXIR" || DosageForm == "SYRUP" || DosageForm == "SUSPENTION" || DosageForm == "EMULSION" || DosageForm == "SOUTION") {
            var TotalUnits = $("<input />");
            TotalUnits.attr("type", "text");
            TotalUnits.addClass('TotalUnits');
            TotalUnits.addClass("form-control");
            TotalUnits.attr("onkeyup", "changeTotalUnits(this);");
            cell.append(TotalUnits);
        }
        else {
            var TotalUnits = $("<input />");
            TotalUnits.attr("type", "text");
            TotalUnits.attr('readonly', 'readonly');
            TotalUnits.addClass('TotalUnits');
            TotalUnits.addClass("form-control");
            cell.append(TotalUnits);
        }
        cell = $(row.insertCell(-1));
        var AppendAmount = $("<input />");
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
function SelectMedicien(event) {
    var Code = event.params.args.data.id
    var done = 0;
    $('#PharmacyPending tbody tr').each(function () {
        var row = $(this);
        if (parseInt(row.find("TD").eq(0).html()) == parseInt(Code)) {
            done = 1;

            //$("#AddMedicine option[value='" + id + "']").prop("selected", false);
            //$("#AddMedicine option[value='" + Code + "']").prop("selected", false);
            //$("#AddMedicine").(Code);
            toastr.error('تم ارسال هذا الدواء من قبل للموافقة و جارى الرد من الادارة الطبية');
            var currentMedicine = parseInt(row.find("TD").eq(0).html());
            var newOption = new Option(row.find("TD").eq(1).html(), currentMedicine, false, false);
            $('#AddMedicine').Remove(newOption).trigger('change');
        }
    });
    if (done == 0) {
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
        var isChronic = 0;
        $.ajax({
            type: 'POST',
            url: '/Pharmacy/GetMedicineByCode/',
            dataType: 'json',
            data: { code: Code },
            success: function (r) {
                if (ServiceCode != "11602" && r.M_TYPE == "CHRONIC") {
                    isChronic = 1;
                    //edit = 1;
                    //toastr.error('Can Not Add This Medicine Chronic');
                    //$("#AddMedicine option[value='" + Code + "']").remove();
                    //$("#wait").css("display", "none");
                }
                //else {
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
                    medicineGroup.M_CODE = parseInt(row.find("TD").eq(0).html());
                    medicineGroups.push(medicineGroup);
                    if (parseInt(row.find("TD").eq(0).html()) == parseInt(MedicienCode)) {
                        edit = 1;
                        event.preventDefault();
                        toastr.error('Added before');
                        $("#wait").css("display", "none");

                    }
                });

                var samegroup = false;
                var samegroupAll = false;
                var Duration = false;
                //
                if (edit == 0) {
                    if (IsCover == "true") {
                        //Gender Check
                        $.ajax({
                            dataType: "json",
                            url: '/Pharmacy/GenderValidation',
                            data: {
                                CardId: CardId,
                                MedicineCode: MedicienCode,
                                Id: 1
                            },
                            success: function (r) {
                                if (r == "True") {
                                    GenderValidation = true;
                                }
                                else {
                                    RemoveSelection(Code);
                                    toastr.error("this medicine dosn't match patient's gender");
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
                                    CardId: CardId,
                                    MedicineCode: MedicienCode,
                                    Id: 1
                                },
                                success: function (r) {
                                    if (r == "True") {
                                        AgeValiation = true;
                                    }
                                    else {
                                        if (GenderValidation == true) {
                                            RemoveSelection(Code);
                                            toastr.error("this medicine dosn't match patient's Age");
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
                                            if (AgeValiation == true) {
                                                RemoveSelection(Code);
                                                toastr.error("medicines has the same medicine group");
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
                                    //medicine group All
                                    $.ajax({
                                        url: '/Pharmacy/MedicinesGroupValiadtionAll/',
                                        dataType: 'Json',
                                        contentType: "application/json; charset=utf-8",
                                        data: {
                                            CardId: $('#txtSearchCard').val(),
                                            MedicineCode: MedicienCode
                                        },
                                        success: function (r) {

                                            if (r == true) {
                                                if (samegroup == false) {
                                                    RemoveSelection(Code);
                                                    //toastr.error("You can't dispense this medicine");//same Group
                                                    toastr.error("medicines has the same medicine group");
                                                    $("#wait").css("display", "none");
                                                }
                                                samegroupAll = true;
                                            }
                                            else {
                                                samegroupAll = false;
                                            }
                                        },
                                        error: function () {
                                            samegroupAll = false;
                                        }
                                    }).done(function () {
                                        $.ajax({
                                            url: '/Pharmacy/MedicinesDurationValiadtion/',
                                            dataType: 'Json',
                                            contentType: "application/json; charset=utf-8",
                                            data: {
                                                CardId: CardId,
                                                MedicineCode: MedicienCode
                                            },
                                            success: function (r) {
                                                if (r == true) {
                                                    if (samegroupAll == false) {
                                                        RemoveSelection(Code);
                                                        toastr.error("this medicine still in your previous duration ");//'Duration Validation'
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
                                            if (GenderValidation == true && AgeValiation == true && IsCover == "true" && samegroup == false && samegroupAll == false && Duration == false) {
                                                //check Daily
                                                $.ajax({
                                                    dataType: "json",
                                                    url: '/Pharmacy/CheckDaily',
                                                    data: {
                                                        id: CardId,
                                                        code: MedicienCode
                                                    },
                                                    success: function (r) {
                                                        if (r.check == 0) {
                                                            $("#wait").css("display", "none");
                                                            // chick if chronic or no
                                                            if (isChronic == 0) {
                                                                //append row
                                                                if (Group == "NO") {
                                                                    $.ajax({
                                                                        dataType: "json",
                                                                        url: '/Pharmacy/CheckVip',
                                                                        data: {
                                                                            id: CardId
                                                                        },
                                                                        success: function (r) {
                                                                            //if (CompId.includes("500")) {
                                                                            //    Group = "Accepted";
                                                                            //    AppendRow();
                                                                            //}
                                                                            //else {
                                                                            if (r.IsVip == 1) {
                                                                                Group = "Accepted";
                                                                                AppendRow();
                                                                            }
                                                                            else {
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
                                                                        }
                                                                    });

                                                                }
                                                                else {
                                                                    AppendRow();
                                                                }
                                                            }
                                                            else {
                                                                var dialog = bootbox.dialog({
                                                                    //title: 'This Medicien is Not Covered!',
                                                                    title: 'This medicine requires approval',
                                                                    message: "<p>Pay method?</p>",
                                                                    onEscape: function () {
                                                                        RemoveSelection(MedicienCode);
                                                                    },
                                                                    //backdrop: true,
                                                                    buttons: {
                                                                        Cash: {
                                                                            label: "Cash",
                                                                            className: 'btn-info',
                                                                            callback: function () {
                                                                                Group = "Cash";
                                                                                AppendRow();
                                                                            }
                                                                        }
                                                                        , Tele: {
                                                                            label: "Pending",
                                                                            className: 'btn-info',
                                                                            callback: function () {
                                                                                Group = "PendingChronic";
                                                                                //toastr.info('برجاءالتواصل مع الاداره الطبيه');
                                                                                AppendRow();
                                                                            }
                                                                        }
                                                                    }
                                                                });
                                                            }
                                                        }
                                                        else {
                                                            RemoveSelection(Code);
                                                            bootbox.alert(r.messa);
                                                            $("#wait").css("display", "none");
                                                        }
                                                    },
                                                    error: function (r) {
                                                        bootbox.alert("failed Exchanged validation ,chacke your internet connection ");
                                                        $("#wait").css("display", "none");
                                                    }

                                                });
                                            }
                                        });
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

                //}
            },
            error: function (ex) {
                bootbox.alert('Failed to retrieve Medicine Data,please chack your internet connection');
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
            var PackagePrice = $("<input />");
            PackagePrice.attr("type", "text");
            PackagePrice.addClass("form-control");
            PackagePrice.addClass("PackagePrice");
            PackagePrice.attr("onkeyup", "changeTotalUnits(this);");
            PackagePrice.val(PackPrice);
            cell.append(PackagePrice);
            cell = $(row.insertCell(-1));
            cell.html(UnitNumber);
            cell = $(row.insertCell(-1));
            var unitprice = $("<input />");
            unitprice.attr("type", "text");
            unitprice.attr('readonly', 'readonly');
            unitprice.addClass("form-control");
            unitprice.addClass('UnitPrice');
            cell.append(unitprice);
            cell = $(row.insertCell(-1));
            var Dose = $("<input />");
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
            var Duration = $("<input />");
            Duration.attr("type", "text");
            Duration.addClass("form-control");
            Duration.addClass("Duration");
            Duration.attr("onkeyup", "changeTotalDuration(this);changeTable(this);");
            cell.append(Duration);
            cell = $(row.insertCell(-1));
            var TotalDuration = $("<input />");
            TotalDuration.attr("type", "text");
            TotalDuration.addClass("form-control");
            TotalDuration.addClass("TotalDuration");
            TotalDuration.attr("onfocusout", "changeTotalDuration(this);");
            cell.append(TotalDuration);
            cell = $(row.insertCell(-1));
            if (DosageForm == "ELIXIR" || DosageForm == "SYRUP" || DosageForm == "SUSPENTION" || DosageForm == "EMULSION" || DosageForm == "SOUTION") {
                var TotalUnits = $("<input />");
                TotalUnits.attr("type", "text");
                TotalUnits.addClass('TotalUnits');
                TotalUnits.addClass("form-control");
                TotalUnits.attr("onkeyup", "changeTotalUnits(this);");
                cell.append(TotalUnits);
            }
            else {
                var TotalUnits = $("<input />");
                TotalUnits.attr("type", "text");
                TotalUnits.attr('readonly', 'readonly');
                TotalUnits.addClass('TotalUnits');
                TotalUnits.addClass("form-control");
                cell.append(TotalUnits);
            }
            cell = $(row.insertCell(-1));
            var AppendAmount = $("<input />");
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
}
function getlimit() {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Pharmacy/CellingAmountEditPage',
        data: {
            id: CardId,
            ServiceCode: ServiceCode,
            RoshitaId: id
        },
        success: function (r) {
            if (r.Validation == false) {
                //toastr.info(r.Message);
                alert(r.Message);
                window.location = '/Pharmacy/index';
            } else {
                CeilingPert = r.CeilingPert;
                AnuualLimit = r.Limit;
                $('#IsFamily').val(r.IsFamily);
                $('#IsPool').val(r.IsPool);
                $('#AllLimit').val(r.AnnualLimit);
                if (ServiceCode == "11601") {
                    if (r.LimitDailyPreceptionCount && r.CoInsurancelimit.INSURANCE_DAY >= 0) {
                        Limit = r.CoInsurancelimit.INSURANCE_DAY;
                        fixedLimit = Limit;
                    } else {
                        alert(" لقد تم استهلاك العدد المحدد للروشتات وسوف تكون خارج التغطه ");
                        Limit = 0.001;
                        fixedLimit = Limit;
                        CeilingPert = 0;
                    }
                }
                else if (ServiceCode == "11603" || ServiceCode == "11602") {
                    //if (ServiceCode == "11603" && (CompId.startsWith("10") || CompId.startsWith("70") || CompId == "500135" || CompId == "500136" || CompId == "500137" || CompId == "500138" || CompId == "500139" || CompId == "500140" || CompId == "500145")) {
                    //    alert("برجاء الرجوع للإداره الطبيه");
                    //    window.location = "/Pharmacy/index";
                    //}

                    if (r.LimitMonthlyPreceptionCount && r.CoInsurancelimit.INSURANCE_MONTH >= 0) {
                        Limit = r.CoInsurancelimit.INSURANCE_MONTH;
                        fixedLimit = Limit;

                    } else {
                        alert(" لقد تم استهلاك العدد المحدد للروشتات وسوف تكون خارج التغطه ");
                        Limit = 0.001;
                        fixedLimit = Limit;
                        CeilingPert = 0;
                    }

                    GetChronicMedData()
                }
                Calculation();
            }
        },
        error: function (err) {
            alert("Failed to retrieve Company Annual Limit. please check your internet connection");
            location.reload();
        }
    }).done(function () {
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "/Pharmacy/CheckType",
            data: { CardId: CardId },
            success: function (returndata) {
                if (returndata == false) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: '/Pharmacy/GetRoshitaApproval',
                        data: { RoshitaId: id, Type: 3 },
                        success: function (returndata) {
                            if (returndata == false) {

                            }
                            else {
                                var Copayment = false;
                                var Limit = false;
                                var DisregardCeiling = false;
                                var ExternalPrescription = false;
                                var PrescriptionPerDay = false;
                                for (var i = 0; i < returndata.length; i++) {
                                    Copayment = Copayment == true ? true : returndata[i].includes("Cancel Co-Payment");
                                    Limit = Limit == true ? true : returndata[i].includes('Disregard OverInsurance');
                                    DisregardCeiling = DisregardCeiling == true ? true : returndata[i].includes("Disregard Ceiling");
                                    ExternalPrescription = ExternalPrescription == true ? true : returndata[i].includes("External Prescription");
                                    PrescriptionPerDay = PrescriptionPerDay == true ? true : returndata[i].includes("Unlimited Examination per day");

                                }

                                if (Limit == true) {
                                    fixedLimit = 0;
                                    //$("#insurance_LIVEL").val(0);
                                    AnuualLimit = $('#AllLimit').val();
                                    Calculation();
                                }
                                if (Copayment == true) {
                                    CeilingPert = 100;
                                    //$("#ddEmp_CEILING_PERT").val(100);
                                    Calculation();
                                }
                                if (DisregardCeiling == true) {
                                    AnuualLimit = 30000;
                                    Calculation();
                                }
                                if (ExternalPrescription == true) {
                                    $('#ClaimNumber').val(' ');
                                }
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
                                    fixedLimit = 0;
                                    //$("#insurance_LIVEL").val(0);
                                    CeilingPert = 100;
                                    $("#ddEmp_CEILING_PERT").val(100);
                                    $('#ClaimNumber').val(' ');
                                    AnuualLimit = 30000;
                                    Calculation();

                                }
                            }
                        }
                    });
                }
            },
            error: function (err) {
            }

        });
    });
}
function GetChronicMedData() {
    if (ServiceCode == "11602") {

        $.ajax({
            type: "POST",
            dataType: "json",
            url: '/Chronic/GetChronicMedData',
            data: { id: CardId },
            success: function (returndata) {
                if (returndata.ok && returndata.medCard != null) {
                    if (returndata.medCard.NO_PAY == 1) {
                        CeilingPert = 100;
                    }
                    if (returndata.medCard.NO_OVER == 1) {
                        Limit = 0;
                    }
                    Calculation();
                }
                else {
                    bootbox.alert('No Limit Amount ,please check your internet connection ');
                }
            }
        });
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
    TotUnits = Math.ceil(TotUnits)
    //TotUnits = Math.round(TotUnits) == 0 ? 1 : Math.round(TotUnits);
    $("TD", row).find(".TotalUnits").val(TotUnits);
    var UnitPrice = (parseFloat($("TD", row).find(".PackagePrice").val() / parseFloat(UnitNumber))).toFixed(2);
    $("TD", row).find(".UnitPrice").val(UnitPrice);
    var Amount = (parseFloat(UnitPrice) * parseFloat($("TD", row).find(".TotalUnits").val())).toFixed(2);
    $("TD", row).find(".Amount").val(Amount);
    var TotalDuration = $("TD", row).find(".TotalDuration").val();
    if ((parseFloat(Duration)) > (parseFloat(TotalDuration))) {
        bootbox.alert("Total duration Must be more than Durartion ");
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
    var MinDay = (ServiceCode == "11601") ? 5 : 1;
    var MaxDay = (ServiceCode == "11601") ? 14 : 28;
    if (CompId == "8887700") {
        MinDay = 1;
        MaxDay = 28;
    }
    var row = $(button).closest("TR");
    var TotalDuration = parseFloat($("TD", row).find(".TotalDuration").val());
    var Duration = parseFloat($("TD", row).find(".Duration").val());
    Duration = isNaN(Duration) ? 1 : (MaxDay > Duration) ? Duration : MaxDay;
    TotalDuration = isNaN(TotalDuration) || TotalDuration < MinDay || TotalDuration < Duration ?
        ((Duration < MinDay) ? MinDay : Duration)
        : ((TotalDuration > MaxDay) ? MaxDay : TotalDuration);
    $("TD", row).find(".Duration").val(Duration);
    $("TD", row).find(".TotalDuration").val(TotalDuration);
    if (!(CardId.split('-')[0].includes("500"))) {
        if ((parseFloat(diffDays)) < (parseFloat(Duration))) {
            bootbox.alert("Duration Must be less than Contract days ");
            $("TD", row).find(".Duration").val(diffDays);
        }
    }



    Calculation();
}
function changeTotalUnits(button) {
    var row = $(button).closest("TR");
    if (ServiceCode == "11602") {
        var minimum = FixedTotalUnits[row[0].sectionRowIndex];
        if (parseInt(minimum) < parseInt($("TD", row).find(".TotalUnits").val())) {
            $("TD", row).find(".TotalUnits").val(parseInt(minimum))
        }

        if (parseInt($("TD", row).find(".TotalUnits").val()) < 1) {
            $("TD", row).find(".TotalUnits").val(1);
        }
    }
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
    $('#txtTotalInvoice').val('');
    $('#txtTotalCopayment').val('');
    $('#txtValueCredit').val('');
    $('#txtOverInsurance').val('');
    $('#txtCash').val('');
    $('#txtValueCash').val('');
}
function ClearMedicineData() {
    $("#Pharmacy >tbody").empty();
    $("#AddMedicine").empty();
    $("#AddMedicine").attr("disabled", "disabled");
}
