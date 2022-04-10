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
var diffDays = 265;
var FixedTotalUnits = [];
$(function () {
    $("#wait").css("display", "block");
    $('#Rays tbody tr').each(function () {
        var row = $(this);
        var currentMedicine = parseInt(row.find("TD").eq(0).html());
        var newOption = new Option(row.find("TD").eq(1).html(), currentMedicine, true, true);
        $('#AddRays').append(newOption).trigger('change');
    });
    $("#Backbutton").click(function () {
        window.location.replace('/Rays/index');
    });
    $.get('/Rays/Manger/', { id: id }, function (data) {
        Manager = data.Manager;
        ServiceCode = data.RoshetaType;
        CardId = data.CardId;
        CompId = CardId.split('-')[0];

    }).done(function () {
        //GetActivation
        $.ajax({
            type: "POST",
            dataType: "json",
            url: '/Pharmacy/GetCompActivation',
            data: { id: CompId, CardId: CardId },
            success: function (returndata) {
                if (returndata.ok) {
                    if (returndata.data == "Y") {
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
                                        window.location.replace('/Rays/index');
                                    }
                                }
                            }
                        });
                    }
                }
                else {
                    bootbox.alert('Error activation ');
                    window.location.replace('/Rays/index');

                }
            }
        });
        getlimit()

    }).done(function () {
        $("#wait").css("display", "none");
    })


    $("#AddRays").select2({
        placeholder: "Select a medicine",
        ajax: {
            url: '/Rays/GetList',
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
    $('#AddRays').on('select2:selecting', function (event) {
        SelectMedicien(event);
    });
    $('#AddRays').on("select2:unselecting", function (event) {
        $('#Rays tbody tr').each(function () {
            var row = $(this);
            if (parseInt(row.find("TD").eq(0).html()) == parseInt(event.params.args.data.id)) {
                Remove(row, event);
            }
        });

    });

    $('#Update').click(function () {

        $.ajax({
            type: 'POST',
            url: '/Rays/update/',
            dataType: 'Json',
            data: {
                Id: id,
                //calculation
                TotalValue: $('#txtTotalInvoice').val(),
                PersonPayment: $('#txtTotalCopayment').val(),
                CompanyPayment: $('#txtValueCredit').val(),
                OverInsurance: $('#txtOverInsurance').val(),
                Cash: $('#txtValueCash').val()
            },
            success: function (Oracle_Id) {
                bootbox.dialog({
                    closeButton: false,
                    title: 'Updated Sucessfully',
                    message: "Roshita ID : " + Oracle_Id,
                    buttons: {
                        Print: {
                            label: "Print",
                            className: 'btn-info',
                            callback: function () {
                                window.location.reload();
                                window.open('/Rays/ControlPenelReport?id=' + Oracle_Id);
                            }
                        },
                        New: {
                            label: "Back",
                            className: 'btn-info',
                            callback: function () {
                                window.location = "/Rays/index";
                            }
                        }

                    }
                });
            },
            error: function (err) {
                bootbox.alert("Error prescription");
            }
        }).done(function () {

            var Mediciens = new Array();
            $("#Rays TBODY TR").each(function () {

                var row = $(this);
                var Medicien = {};
                Medicien.MedicienCode = row.find("TD").eq(0).html().trim();
                Medicien.MedicienName = row.find("TD").eq(1).html().trim();
                Medicien.Amount = parseFloat(("TD", row).find(".Amount").val());
                Medicien.PaymentGroup = row.find("TD").eq(3).html().trim();
                Mediciens.push(Medicien);
            });

            $.ajax({
                type: 'POST',
                url: '/Rays/UpdateMediciens/',
                dataType: 'Json',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(Mediciens),
                success: function (r) {

                },
                error: function (err) {
                    bootbox.alert("Error Deatils");
                }
            });

        });

    })
});
function getlimit() {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Labs/CellingAmountEditPage',
        data: {
            id: CardId,
            ServiceCode: '11201',
            RoshitaId: id
        },
        success: function (r) {
            if (r.Validation == false) {
                //toastr.info(r.Message);
                alert(r.Message);
                window.location = '/Labs/index';
            } else {
                CeilingPert = r.CeilingPert;
                AnuualLimit = r.Limit;
                Limit = r.CoInsurancelimit.INSURANCE_DAY_LAB;
                fixedLimit = Limit;
                Calculation();
            }
        },
        error: function (err) {
            alert("Failed to retrieve Company Annual Limit. please check your internet connection");
            location.reload();
        }
    });
}
function Remove(button, event) {
    var row = $(button).closest("TR");
    var name = $("TD", row).eq(0).html();
    bootbox.confirm("Do you want to delete: " + name, function (result) {
        if (result) {
            var row = $(button).closest("TR");
            var table = $("#Rays")[0];
            table.deleteRow(row[0].rowIndex);
            $('#AddDiagnoise').attr('disabled', false);
            $('#RemoveDiagnoise').attr('disabled', false);
            Calculation();

        } else {
            var values = $('#AddRays').val();
            values.push(name);
            $('#AddRays').val(values).change();

        }
    });

}
function Calculation() {
    var sum = 0;
    var sumCash = 0;

    $('#Rays TBODY TR').each(function () {
        var row = $(this);
        if (parseFloat(("TD", row).find(".Amount").val()) < 1) {
            toastr.warning('invalid Amount ');
            ("TD", row).find(".Amount").val(1);
            //return false
        }
        if (row.find("TD").eq(3).html().trim() != "Pending") {
            sum += parseFloat(("TD", row).find(".Amount").val());
            if (row.find("TD").eq(3).html().trim() == "Cash" || row.find("TD").eq(3).html().trim() == "Rejected" || row.find("TD").eq(3).html().trim() == "Pending")
                sumCash += parseFloat(("TD", row).find(".Amount").val());
        }
    });

    $("#txtTotalInvoice").val(sum.toFixed(2));
    $('#txtCash').val(sumCash);
    $('#txtOverInsurance').val("0");
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
function SelectMedicien(event) {
    var Code = event.params.args.data.id
    $("#wait").css("display", "block");
    var MedicienCode;
    var MedicienName;
    var Group;
    // var IsCover;
    $.ajax({
        type: 'POST',
        url: '/Rays/GetRayByCode/',
        dataType: 'json',
        data: { code: Code },
        success: function (r) {
            MedicienCode = r.SERV_CODE;
            MedicienName = r.SERV_ANAME;
            Group = r.GRUOP_TYPE;
            Amount = r.SERV_AMOUNT;
            //IsCover = r.IsCovered.toString();
            var medicineGroups = new Array();
            var medicineGroup = {};
            medicineGroup.TRADE_NAME = MedicienCode; //current medicine code
            medicineGroups.push(medicineGroup);
            $('#Rays tbody tr').each(function () {
                var row = $(this);
                var medicineGroup = {};
                medicineGroup.M_CODE = parseInt(row.find("TD").eq(0).html());
                medicineGroups.push(medicineGroup);
                if (parseInt(row.find("TD").eq(0).html()) == parseInt(MedicienCode)) {
                    event.preventDefault();
                    toastr.error('Added before');
                    $("#wait").css("display", "none");

                }
            });

            var samegroup = false;
            var Duration = false;
            //check Daily
            $.ajax({
                dataType: "json",
                url: '/Rays/CheckDaily',
                data: {
                    id: CardId,
                    code: MedicienCode
                },
                success: function (r) {
                    if (r == 0) {
                        $("#wait").css("display", "none");
                        //append row
                        if (Group == "NO") {
                            $.ajax({
                                dataType: "json",
                                url: '/Pharmacy/CheckVip',
                                data: {
                                    id: CardId
                                },
                                success: function (r) {
                                    if (r.IsVip == 1) {
                                        Group = "Accepted";
                                        AppendRow();
                                        Calculation();
                                    }
                                    else {
                                        var dialog = bootbox.dialog({
                                            title: 'This Ray is Not Covered!',
                                            message: "<p>Pay method?</p>",
                                            onEscape: function () {
                                                RemoveSelection(MedicienCode);
                                            },
                                            buttons: {
                                                Cash: {
                                                    label: "Cash",
                                                    className: 'btn-info',
                                                    callback: function () {
                                                        Group = "Cash";
                                                        AppendRow();
                                                        Calculation();
                                                    }
                                                },
                                                Approval: {
                                                    label: "Approved",
                                                    className: 'btn-info',
                                                    callback: function () {
                                                        Group = "Approval";
                                                        AppendRow();
                                                        Calculation();
                                                    }
                                                },
                                                Tele: {
                                                    label: "Pending",
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
                    }
                    else {
                        RemoveSelection(Code);
                        bootbox.alert("This Ray had been exchanged Today ");
                        $("#wait").css("display", "none");
                    }
                },
                error: function (r) {
                    bootbox.alert("Ajax exchanged Today  Error");
                    $("#wait").css("display", "none");
                }

            });
            $("#wait").css("display", "none");

        },
        error: function (ex) {
            bootbox.alert('Failed to retrieve Ray Data.');
        }

    });
    function AppendRow() {
        var tBody = $("#Rays > TBODY")[0];
        var row = tBody.insertRow(-1);
        var cell = $(row.insertCell(-1));
        cell.html(MedicienCode);
        cell = $(row.insertCell(-1));
        cell.html(MedicienName);

        cell = $(row.insertCell(-1));
        var AppendAmount = $("<input  />");
        AppendAmount.attr("type", "text");
        //AppendAmount.attr('readonly', 'readonly');
        AppendAmount.addClass("form-control");
        AppendAmount.attr("onkeyup", "Calculation();");
        AppendAmount.addClass('Amount');
        AppendAmount.val(Amount);

        cell.append(AppendAmount);
        cell = $(row.insertCell(-1));
        cell.html(Group);
        toastr.success('Added successfully ');
        $("#wait").css("display", "none");
    }
}


function RemoveSelection(Code) {
    var values = $('#AddRays').val();
    if (values) {
        var i = values.indexOf(Code.toString());
        if (i >= 0) {
            values.splice(i, 1);
            $('#AddRays').val(values).change();
        }
    }
}
