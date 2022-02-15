var input1 = document.getElementById("AddCardTxt");
input1.addEventListener("keyup", function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        $('#AddCard').click();
    }
});
$(function () {
    $('#txtSearchCompany').click(function () {
        $("#wait").css("display", "block");
        $.ajax({
            url: '/CompanyGroup/CompanyGroupList',
            dataType: 'Json',
            success: function (r) {
                $('#CompaniesModal').modal();
                var setData = $("#Companies Tbody");
                setData.empty();
                for (var i = 0; i < r.length; i++) {
                    var data = "<tr >" +
                        "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='Select(this);'></Button>" + "</td>" +
                        "<td>" + r[i].C_COMP_ID + "</td>" +
                        "<td>" + r[i].C_ANAME + "</td>" +
                        "</tr>"
                    setData.append(data);

                }
                $('#Companies').dataTable().fnDestroy();
                $('#Companies').DataTable();
                $("#wait").css("display", "none");

            },
            error: function () {
                $("#wait").css("display", "none");
                toastr.error("Error Retrieve CompanyGroup");
            }

        });
    });
    $('#AddCard').click(function () {
        //var IsExist = 0;
        $("#wait").css("display", "block");
        var AddCardTxt = $('#AddCardTxt').val();
        if (AddCardTxt != "") {
            //Clear Data
            $(".modal-body #Provider").val('');
            $('.modal-body #StartDate').val('');
            $('.modal-body #ExchangeDay').val('1');
            $(".modal-body #Group option:eq(0)").attr('selected', 'selected');
            $(".modal-body #Diagnoise option:eq(0)").attr('selected', 'selected');
            $('.modal-body #CompanyPaymentLimit').val('1');
            $('.modal-body #OverInsuranceLimit').val('1');
            $('.modal-body #LockStatus').val('Open');

            $(".modal-body #Provider1").val('');
            $('.modal-body #StartDate1').val('');
            $('.modal-body #ExchangeDay1').val('1');
            $(".modal-body #Group1 option:eq(0)").attr('selected', 'selected');
            $(".modal-body #Diagnoise1 option:eq(0)").attr('selected', 'selected');
            $('.modal-body #CompanyPaymentLimit1').val('1');
            $('.modal-body #OverInsuranceLimit1').val('1');
            $('.modal-body #LockStatus1').val('Open');

            //-------------------
            $.ajax({
                dataType: "json",
                url: '/Pharmacy/AddCard',
                data: { id: AddCardTxt },
                success: function (r) {
                    if (r != null) {
                        $('#SearchCards').dataTable().fnDestroy();
                        var setData = $("#SearchCards Tbody");
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
                            if (r[i].TERMINATE_DATE != null) {
                                //end date
                                var MyDate_String_Value1 = r[i].TERMINATE_DATE;
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
                                "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectSearchCards(this);'></Button>" + "</td>" +
                                "<td>" + r[i].CARD_ID + "</td>" +
                                "<td>" + r[i].EMP_ANAME + "</td>" +
                                "<td>" + r[i].EMP_ENAME + "</td>" +
                                "<td>" + dat + "</td>" +
                                "<td>" + dat1 + "</td>" +
                                "</tr>"
                            setData.append(data);

                        }
                        $('#SearchCards').DataTable();
                        $('#SearchCardsModal').modal();
                        $("#wait").css("display", "none");
                    }
                    else {
                        $("#wait").css("display", "none");
                        bootbox.alert("Inviled Card");
                    }
                },
                error: function (r) {
                    $("#wait").css("display", "none");
                    window.alert('Error retrive  Data ');
                }

            });
        }
        else {
            bootbox.alert("Please insert Card ID")
        }
    });
    $('#Cards').on('search.dt', function () {
        var setData = $("#CardMedicines tbody");
        setData.empty();
    });

    $('#ChangeProvider').click(function () {
        $("#wait").css("display", "block");

        $.ajax({
            url: '/DoctorApprovals/getProviders/',
            dataType: 'Json',
            success: function (r) {
                $('#Providers').dataTable().fnDestroy();
                prov = "Change";
                $('#ProvidersModal').modal();
                var setData = $("#Providers Tbody");
                setData.empty();

                for (var i = 0; i < r.length; i++) {

                    var data = "<tr >" +
                        "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectProvider(this);'></Button>" + "</td>" +
                        "<td>" + r[i].PR_CODE + "</td>" +
                        "<td>" + r[i].PR_ANAME + "</td>" +
                        "<td>" + r[i].PR_ENAME + "</td>" +
                        "</tr>"
                    setData.append(data);

                }
                $('#Providers').DataTable();
                $("#wait").css("display", "none");

            },
            error: function () {
                $("#wait").css("display", "none");
                alert("Error Providers");
            }

        });

    });
    $('#ChangeGroup').change(function () {
        $("#wait").css("display", "block");

        var CardIds = new Array;
        $('#Cards tbody tr').each(function () {
            var row = $(this);
            if ($("TD", row).eq(1).html() != undefined) {
                var CurrentCardId = {};
                CurrentCardId.CARD_NO = $("TD", row).eq(1).html();
                CurrentCardId.MED_NAME = $('#ChangeGroup :selected').text();
                CardIds.push(CurrentCardId);
            }
        });
        if (CardIds.length != 0) {
            $.ajax({
                type: 'POST',
                url: '/DoctorApprovals/ChangeGroup',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(CardIds),
                dataType: 'Json',
                success: function (r) {
                    $('#Cards tbody tr').each(function () {
                        var row = $(this);
                        $("TD", row).eq(6).html($('#ChangeGroup :selected').text());

                    });
                    $("#wait").css("display", "none");

                },
                error: function () {
                    $("#wait").css("display", "none");
                    alert("Error Retrieve groups");
                }
            });
        } else {
            toastr.warning("Nothing to change");

        }
    });

    $('#CoPayStatus').change(function () {
        $("#wait").css("display", "block");
        if ($('#CoPayStatus').val() != "N") {

            $.ajax({
                type: 'POST',
                url: '/DoctorApprovals/PaymentStatus',
                dataType: 'Json',
                data: {
                    CompId: CompId,
                    state: $('#CoPayStatus').val()
                },
                dataType: 'Json',
                success: function (r) {
                    $('#Cards_filter input[type="search"]').val('').keyup();
                    if (r > 0) {
                        $('#Cards tbody tr').each(function () {
                            var row = $(this);
                            if ($('#CoPayStatus').val() != "N") {
                                $("TD", row).eq(8).html($('#CoPayStatus').val());
                            }
                        });
                        toastr.success("Updated")
                    } else {
                        toastr.warning("Nothing to change");
                    }
                    $("#wait").css("display", "none");

                },
                error: function () {
                    $("#wait").css("display", "none");

                    alert("Error Retrieve staus");
                }
            });
        }
    });
    $('#OverStatus').change(function () {
        $("#wait").css("display", "block");
        if ($('#OverStatus').val() != "N") {
            $.ajax({
                type: 'POST',
                url: '/DoctorApprovals/OverStatus',
                dataType: 'Json',
                data: {
                    CompId: CompId,
                    state: $('#OverStatus').val()
                },
                dataType: 'Json',
                success: function (r) {

                    $('#Cards_filter input[type="search"]').val('').keyup();
                    if (r > 0) {
                        $('#Cards tbody tr').each(function () {
                            var row = $(this);
                            if ($('#OverStatus').val() != "N") {
                                $("TD", row).eq(9).html($('#OverStatus').val());
                            }
                        });
                        toastr.success("Updated");
                    } else {
                        toastr.warning("Nothing to change");
                    }
                    $("#wait").css("display", "none");

                },
                error: function () {
                    alert("Error Retrieve staus");
                    $("#wait").css("display", "none");

                }
            });
        }


    });
    $('#LockStatus2').change(function () {
        $("#wait").css("display", "block");
        if ($('#LockStatus2').val() != "N") {
            $.ajax({
                type: 'POST',
                url: '/DoctorApprovals/LockStatus',
                dataType: 'Json',
                data: {
                    CompId: CompId,
                    state: $('#LockStatus2').val()
                },
                dataType: 'Json',
                success: function (r) {

                    $('#Cards_filter input[type="search"]').val('').keyup();
                    if (r > 0) {
                        $('#Cards tbody tr').each(function () {
                            var row = $(this);
                            if ($('#LockStatus2').val() != "N") {
                                $("TD", row).eq(10).html($('#LockStatus2').val());
                            }
                        });
                        toastr.success("Updated");
                    } else {
                        toastr.warning("Nothing to change");
                    }
                    $("#wait").css("display", "none");

                },
                error: function () {
                    alert("Error Retrieve");
                    $("#wait").css("display", "none");

                }
            });
        }


    });
    //Dignosies
    $.ajax({
        type: 'POST',
        url: '/DoctorApprovals/getDiag/',
        dataType: 'json',
        data: {},
        success: function (diag) {
            $.each(diag, function (i, ddlDiag1) {
                $("#Diagnoise").append('<option value="' + ddlDiag1.Code + '">' +
                    ddlDiag1.Name + '</option>');
                $("#Diagnoise1").append('<option value="' + ddlDiag1.Code + '">' +
                    ddlDiag1.Name + '</option>');
            });
            $('#Diagnoise  option:eq(0)').attr('selected', 'selected');
        },
        error: function (ex) {
            alert('Failed to retrieve Diagnoses.');
        }

    });

    //Mediciences
    var ajaxcounter = 0;
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Pharmacy/Medicines',
        data: {},
        success: function (r) {
            ajaxcounter += 1;
            var setData = $("#Medicines Tbody");
            //setData.empty();
            for (var i = 0; i < r.length; i++) {
                var data = "<tr >" +
                    "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectMedicien(this);'></Button>" + "</td>" +
                    "<td>" + r[i].M_CODE + "</td>" +
                    "<td>" + r[i].TRADE_NAME + "</td>" +
                    "<td>" + r[i].DOSAGE_FORM + "</td>" +
                    "<td>" + r[i].PACK_PRICE + "</td>" +
                    "<td>" + r[i].PACK_SIZE + "</td>" +
                    "<td>" + r[i].UNIT_NO + "</td>" +
                    "<td>" + r[i].UNIT_PRICE + "</td>" +
                    "<td>" + r[i].Group_Type + "</td>"+
                    "<td>" + r[i].MED_GROUP + "</td>"+
                "</tr>"
                setData.append(data);
            }
        },
        error: function (ex) {
            // alert('No Medicen data ');
        }
    });
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Pharmacy/Medicines2',
        data: {},
        success: function (r) {
            ajaxcounter += 1;
            var setData = $("#Medicines Tbody");
            //setData.empty();
            for (var i = 0; i < r.length; i++) {
                var data = "<tr >" +
                    "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectMedicien(this);'></Button>" + "</td>" +
                    "<td>" + r[i].M_CODE + "</td>" +
                    "<td>" + r[i].TRADE_NAME + "</td>" +
                    "<td>" + r[i].DOSAGE_FORM + "</td>" +
                    "<td>" + r[i].PACK_PRICE + "</td>" +
                    "<td>" + r[i].PACK_SIZE + "</td>" +
                    "<td>" + r[i].UNIT_NO + "</td>" +
                    "<td>" + r[i].UNIT_PRICE + "</td>" +
                    "<td>" + r[i].Group_Type + "</td>"+
                    "<td>" + r[i].MED_GROUP + "</td>"+
                "</tr>"
                setData.append(data);

            }
        },
        error: function (ex) {
            //alert('No Medicen');
        }
    });
    var counter = 0;
    $("#AddMedicien").click(function () {
        if (CardId != "") {
            $("#wait").css("display", "block");
            $(document).ajaxComplete(function (event, xhr, settings) {
                if (settings.url === "/Pharmacy/Medicines") {
                    counter += 1;
                }
                if (settings.url === "/Pharmacy/Medicines2") {
                    counter += 1;
                }
                if (counter == 2) {
                    $("#wait").css("display", "none");
                    $('#ApprovalMedicinesModal').modal();
                    $('#Medicines').DataTable();
                }
            });
            if (counter == 2 || ajaxcounter == 2) {
                $("#wait").css("display", "none");
                $('#Medicines').DataTable();
                $('#ApprovalMedicinesModal').modal();
            }
            $('#Medicines_filter input[type="search"]').val('').keyup();
        }
        else {
            bootbox.alert("Please  Select Card ");
        }
    });
    $('#LastApproval').click(function () {

        if (CardId != "") {
            $("#wait").css("display", "block");

            $.ajax({
                dataType: "json",
                url: '/DoctorApprovals/LastApproval',
                data: { id: CardId },
                success: function (r) {
                    if (r.length != 0) {
                        if (r[0].INS_END_DATE != null) {
                            var MyDate_String_Value = r[0].INS_END_DATE;
                            var value = new Date(parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, '')));
                            var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                        }
                        else {
                            dat = null;
                        }
                        if (r[0].CreatedDate != null) {
                            var MyDate_String_Value1 = r[0].CreatedDate;
                            var value1 = new Date(parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, '')));
                            var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                        }
                        else {
                            dat = null;
                        }
                        $('#ApprvalId').val(r[0].Id);
                        $('#LastProvider_Provider').val(r[0].CreatedBy);
                        $('#PrescriptionDate').val(dat1);
                        $('#LastProvider_CARD_ID').val(r[0].CardId);
                        $('#LastProvider_EMP_ANAME').val(r[0].EMP_ANAME);
                        $('#LastProvider_COMP_ID').val(r[0].C_COMP_ID);
                        $('#LastProvider_INS_END_DATE').val(dat);
                        $('#LastProvider_INSURANCE_DAY').val(r[0].INSURANCE_DAY);
                        $('#LastProvider_INSURANCE_MONTH').val(r[0].INSURANCE_MONTH);
                        $('#LastProvider_CEILING_PERT').val(r[0].CompanyPercent);
                        var setData = $("#LastApprovals Tbody");
                        setData.empty();
                        for (var i = 0; i < r.length; i++) {
                            var data = "<tr >" +
                                "<td>" + r[i].MedicienCode + "</td>" +
                                "<td>" + r[i].MedicienName + "</td>" +
                                "<td>" + r[i].DOSAGE_FORM + "</td>" +
                                "<td>" + r[i].PACK_SIZE + "</td>" +
                                "<td>" + r[i].PACK_PRICE + "</td>" +
                                "<td>" + r[i].UNIT_NO + "</td>" +
                                "<td>" + r[i].UNIT_PRICE + "</td>" +
                                "<td>" + r[i].Dose + "</td>" +
                                "<td>" + r[i].Duration + "</td>" +
                                "<td>" + r[i].TotalUnits + "</td>" +
                                "<td>" + r[i].Amount + "</td>" +
                                "<td>" + r[i].RoshetaType + "</td>" +
                                "</tr>"
                            setData.append(data);

                        }
                        $('#LastApprovalModal').modal();
                        $("#wait").css("display", "none");

                    }
                    else {
                        bootbox.alert("No previous data");
                        $("#wait").css("display", "none");

                    }
                },
                error: function (r) {
                    window.alert('No Data to retrive ');
                    $("#wait").css("display", "none");

                }

            });
        }
        else {
            bootbox.alert("Please  Select Card ");
        }
    });
    $("#SubmetChronic").click(function () {

        if (CardId != "") {
            $("#wait").css("display", "block");
            var Mediciens = new Array();
            $("#CardMedicines TBODY TR").each(function () {
                var row = $(this);
                var Medicien = {};
                Medicien.CARD_NO = CardId;
                Medicien.MED_CODE = row.find("TD").eq(0).html();
                Medicien.MED_NAME = row.find("TD").eq(1).html();
                Medicien.DOSAGE_FORM = row.find("TD").eq(2).html();
                Medicien.PACK_SIZE = row.find("TD").eq(3).html();
                Medicien.PACK_PRICE = row.find("TD").eq(4).html();
                Medicien.UNIT_NO = row.find("TD").eq(5).html();
                Medicien.UNIT_PRICE = row.find("TD").eq(6).html();
                Medicien.DOSE = row.find("TD").eq(7).html();
                Medicien.MED_DURATION = row.find("TD").eq(8).html();
                Medicien.DOS_DUR = row.find("TD").eq(9).html();
                Medicien.NO_OF_UINT = row.find("TD").eq(10).html();
                Medicien.EXCESS = row.find("TD").eq(11).html();
                Medicien.TOTAL_AMT = row.find("TD").eq(12).html();
                if (row.find("TD").eq(13).html() == "Chronic") {
                    Medicien.MED_TYP = 5;
                }
                else {
                    Medicien.MED_TYP = 9;
                }
                Medicien.LFT_MONTH = row.find("TD").eq(14).html();
                Medicien.MONTH_DATE_STOP = row.find("TD").eq(15).html();
                Medicien.ACTIVE = row.find("TD").eq(16).html();

                Mediciens.push(Medicien);
            });

            if (Mediciens.length > null) {
                $("#SubmetChronic").attr("disabled", "disabled");
                $.ajax({
                    type: 'POST',
                    url: '/DoctorApprovals/SubmetChronic/',
                    dataType: 'Json',
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(Mediciens),
                    success: function (r) {
                        if (r == "False") {
                            toastr.error("غير مسموح  بالتعديل او الاضافة  علي هذا الكارت يرجي الرجوع الي ادارة المراجعه  ");
                            $("#SubmetChronic").attr("disabled", false);
                        }
                        else {
                            bootbox.alert("Saved Successfully");
                            $("#SubmetChronic").attr("disabled", false);
                        }
                    },
                    error: function (err) {
                        bootbox.alert("Error Medicien");
                        $("#SubmetChronic").attr("disabled", false);

                    }

                });
                $("#wait").css("display", "none");
            }
            else {
                bootbox.alert("Please Add medicine to this card Card ");
                $("#wait").css("display", "none");

            }
        } else {
            bootbox.alert("Please Select Card ");
        }
    });
});
var CardId = "";
function ActiveOrNotActiveMedicine() {
    if (CardId != "") {
        debugger;
        var buttontype = $("#ActiveMedicine").html();
        var Mediciens = new Array();
        $("#CardMedicines TBODY TR").each(function () {
            var row = $(this);
            var Medicien = {};
            Medicien.MED_CODE = row.find("TD").eq(0).html();
            Medicien.MED_NAME = row.find("TD").eq(1).html();
            Medicien.DOSAGE_FORM = row.find("TD").eq(2).html();
            Medicien.PACK_SIZE = row.find("TD").eq(3).html();
            Medicien.PACK_PRICE = row.find("TD").eq(4).html();
            Medicien.UNIT_NO = row.find("TD").eq(5).html();
            Medicien.UNIT_PRICE = row.find("TD").eq(6).html();
            Medicien.DOSE = row.find("TD").eq(7).html();
            Medicien.MED_DURATION = row.find("TD").eq(8).html();
            Medicien.DOS_DUR = row.find("TD").eq(9).html();
            Medicien.NO_OF_UINT = row.find("TD").eq(10).html();
            Medicien.EXCESS = row.find("TD").eq(11).html();
            Medicien.TOTAL_AMT = row.find("TD").eq(12).html();
            Medicien.MED_TYP = row.find("TD").eq(13).html();
            Medicien.LFT_MONTH = row.find("TD").eq(14).html();
            Medicien.MONTH_DATE_STOP = row.find("TD").eq(15).html();
            Medicien.ACTIVE = row.find("TD").eq(16).html();

            Mediciens.push(Medicien);
        });

        if (buttontype == "All") {
            var setData = $("#CardMedicines tbody");
            setData.empty();
            for (var i = 0; i < Mediciens.length; i++) {
                var data = "<tr >" +
                    "<td>" + Mediciens[i].MED_CODE + "</td>" +
                    "<td>" + Mediciens[i].MED_NAME + "</td>" +
                    "<td>" + Mediciens[i].DOSAGE_FORM + "</td>" +
                    "<td>" + Mediciens[i].PACK_SIZE + "</td>" +
                    "<td>" + Mediciens[i].PACK_PRICE + "</td>" +
                    "<td>" + Mediciens[i].UNIT_NO + "</td>" +
                    "<td>" + Mediciens[i].UNIT_PRICE + "</td>" +
                    "<td>" + Mediciens[i].DOSE + "</td>" +
                    "<td>" + Mediciens[i].MED_DURATION + "</td>" +
                    "<td>" + Mediciens[i].DOS_DUR + "</td>" +
                    "<td>" + Mediciens[i].NO_OF_UINT + "</td>" +
                    "<td>" + Mediciens[i].EXCESS + "</td>" +
                    "<td>" + Mediciens[i].TOTAL_AMT + "</td>" +
                    "<td>" + Mediciens[i].MED_TYP + "</td>" +
                    "<td>" + Mediciens[i].LFT_MONTH + "</td>" +
                    "<td>" + Mediciens[i].MONTH_DATE_STOP + "</td>" +
                    "<td>" + Mediciens[i].ACTIVE + "</td>" +
                    " <td>" + "<Button class='btn btn-warning glyphicon glyphicon-pencil' onclick='EditMedicine(this);'>Edit</Button>" + "</td>" +
                    "</tr>"
                setData.append(data);
            }

            $("#ActiveMedicine").html("Active");
        }
        else if (buttontype == "Active") {

            var setData = $("#CardMedicines tbody");
            setData.empty();
            for (var i = 0; i < Mediciens.length; i++) {
                if (Mediciens[i].ACTIVE == "N") {
                    var data = "<tr hidden>" +
                        "<td>" + Mediciens[i].MED_CODE + "</td>" +
                        "<td>" + Mediciens[i].MED_NAME + "</td>" +
                        "<td>" + Mediciens[i].DOSAGE_FORM + "</td>" +
                        "<td>" + Mediciens[i].PACK_SIZE + "</td>" +
                        "<td>" + Mediciens[i].PACK_PRICE + "</td>" +
                        "<td>" + Mediciens[i].UNIT_NO + "</td>" +
                        "<td>" + Mediciens[i].UNIT_PRICE + "</td>" +
                        "<td>" + Mediciens[i].DOSE + "</td>" +
                        "<td>" + Mediciens[i].MED_DURATION + "</td>" +
                        "<td>" + Mediciens[i].DOS_DUR + "</td>" +
                        "<td>" + Mediciens[i].NO_OF_UINT + "</td>" +
                        "<td>" + Mediciens[i].EXCESS + "</td>" +
                        "<td>" + Mediciens[i].TOTAL_AMT + "</td>" +
                        "<td>" + Mediciens[i].MED_TYP + "</td>" +
                        "<td>" + Mediciens[i].LFT_MONTH + "</td>" +
                        "<td>" + Mediciens[i].MONTH_DATE_STOP + "</td>" +
                        "<td>" + Mediciens[i].ACTIVE + "</td>" +
                        " <td>" + "<Button class='btn btn-warning glyphicon glyphicon-pencil' onclick='EditMedicine(this);'>Edit</Button>" + "</td>" +
                        "</tr>"
                    setData.append(data);
                }
                else {
                    var data = "<tr >" +
                        "<td>" + Mediciens[i].MED_CODE + "</td>" +
                        "<td>" + Mediciens[i].MED_NAME + "</td>" +
                        "<td>" + Mediciens[i].DOSAGE_FORM + "</td>" +
                        "<td>" + Mediciens[i].PACK_SIZE + "</td>" +
                        "<td>" + Mediciens[i].PACK_PRICE + "</td>" +
                        "<td>" + Mediciens[i].UNIT_NO + "</td>" +
                        "<td>" + Mediciens[i].UNIT_PRICE + "</td>" +
                        "<td>" + Mediciens[i].DOSE + "</td>" +
                        "<td>" + Mediciens[i].MED_DURATION + "</td>" +
                        "<td>" + Mediciens[i].DOS_DUR + "</td>" +
                        "<td>" + Mediciens[i].NO_OF_UINT + "</td>" +
                        "<td>" + Mediciens[i].EXCESS + "</td>" +
                        "<td>" + Mediciens[i].TOTAL_AMT + "</td>" +
                        "<td>" + Mediciens[i].MED_TYP + "</td>" +
                        "<td>" + Mediciens[i].LFT_MONTH + "</td>" +
                        "<td>" + Mediciens[i].MONTH_DATE_STOP + "</td>" +
                        "<td>" + Mediciens[i].ACTIVE + "</td>" +
                        " <td>" + "<Button class='btn btn-warning glyphicon glyphicon-pencil' onclick='EditMedicine(this);'>Edit</Button>" + "</td>" +
                        "</tr>"
                    setData.append(data);
                }

            }
            $("#ActiveMedicine").html("All");

        }
    }
    else {
        bootbox.alert("Please  Select Card ");
    }
}
function SelectCard(button) {
    $("#wait").css("display", "block");

    //Determine the reference of the Row using the Button.
    var row = $(button).closest("TR");
    CardId = $("TD", row).eq(1).html();
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/DoctorApprovals/MonthlyMedicines',
        data: { id: CardId },
        success: function (r) {

            var setData = $("#CardMedicines tbody");
            setData.empty();
            var Type;
            for (var i = 0; i < r.length; i++) {
                if (r[i].MONTH_DATE_STOP != null) {
                    var MyDate_String_Value = r[i].MONTH_DATE_STOP;
                    var value = new Date
                        (
                            parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                        );
                    var dat =  (value.getMonth() + 1) + "/" + value.getFullYear();
                }
                else {
                    dat = "";
                }
                if (r[i].MED_TYP == 5)
                    Type = "Chronic"
                else
                    Type = "Monthly"
                if (r[i].LFT_MONTH == null) {
                    r[i].LFT_MONTH = " ";
                }
                var data = "<tr >" +
                    "<td>" + r[i].MED_CODE + "</td>" +
                    "<td>" + r[i].MED_NAME + "</td>" +
                    "<td>" + r[i].DOSAGE_FORM + "</td>" +
                    "<td>" + r[i].PACK_SIZE + "</td>" +
                    "<td>" + r[i].PACK_PRICE + "</td>" +
                    "<td>" + r[i].UNIT_NO + "</td>" +
                    "<td>" + r[i].UNIT_PRICE + "</td>" +
                    "<td>" + r[i].DOSE + "</td>" +
                    "<td>" + r[i].MED_DURATION + "</td>" +
                    "<td>" + r[i].DOS_DUR + "</td>" +
                    "<td>" + r[i].NO_OF_UINT + "</td>" +
                    "<td>" + r[i].EXCESS + "</td>" +
                    "<td>" + r[i].TOTAL_AMT + "</td>" +
                    "<td>" + Type + "</td>" +
                    "<td>" + r[i].LFT_MONTH + "</td>" +
                    "<td>" + dat + "</td>" +
                    "<td>" + r[i].ACTIVE + "</td>" +
                    " <td >" + "<Button class='btn btn-warning glyphicon glyphicon-pencil' onclick='EditMedicine(this);'>Edit</Button>" + "</td>" +
                    "</tr>"
                setData.append(data);
            }
            //Highlight
            $("#Cards tbody tr").css("background", "white");
            row.css("background", "gray");
            $("#wait").css("display", "none");

        },
        error: function (r) {
            window.alert(' No medicine data ');
            $("#wait").css("display", "none");

        }

    });

}
function EditCard(button) {
    AddOrEdit = 0;
    var row = $(button).closest("TR");
    $('.modal-body #CardId1').val($("TD", row).eq(1).html());
    $('.modal-body #CardName1').val($("TD", row).eq(2).html());
    $(".modal-body #Provider1").val($("TD", row).eq(3).html());
    $('.modal-body #StartDate1').val($("TD", row).eq(4).html());
    $('.modal-body #ExchangeDay1').val($("TD", row).eq(5).html());
    $(".modal-body #Group1 option").filter(function () {
        return $(this).text() == $("TD", row).eq(6).html();
    }).prop('selected', true);
    var DiagnosisList = $("TD", row).eq(7).html().split('_');
    $('#Diagnoise1').select2({
        dropdownParent: $('#EditCardModal .modal-content')
    });
    let $element = $('#Diagnoise1');
    var stVal = new Array();
    for (var i = 0; i < DiagnosisList.length; i++) {
        stVal[i] = $element.find("option:contains('" + DiagnosisList[i] + "')").val();
    }
    $element.val(stVal).trigger('change.select2');
    $('.modal-body #CompanyPaymentLimit1').val($("TD", row).eq(8).html());
    $('.modal-body #OverInsuranceLimit1').val($("TD", row).eq(9).html());
    $('.modal-body #LockStatus1').val($("TD", row).eq(10).html());
    $('.modal-body #NOTES1').val($("TD", row).eq(11).html());
    $('.modal-body #PhoneNumber1').val($("TD", row).eq(12).html());
    $('.modal-body #NationalId1').val($("TD", row).eq(13).html());
    $('#EditCardModal').modal();

}
function EditMedicine(button) {
    var row = $(button).closest("TR");
    $('.modal-body #MedicienCode').val($("TD", row).eq(0).html());
    $('.modal-body #MedicienName').val($("TD", row).eq(1).html());
    $('.modal-body #Dosage').val($("TD", row).eq(2).html());
    $('.modal-body #PackageSize').val($("TD", row).eq(3).html());
    $('.modal-body #PackagePrice').val($("TD", row).eq(4).html());
    $('.modal-body #UnitNumber').val($("TD", row).eq(5).html());
    $('.modal-body #UnitPrice').val($("TD", row).eq(6).html());
    $('.modal-body #Dose').val($("TD", row).eq(7).html());
    $('.modal-body #Duration').val($("TD", row).eq(8).html());
    $('.modal-body #TotalDuration').val($("TD", row).eq(9).html());
    $('.modal-body #TotalUnits').val($("TD", row).eq(10).html());
    $('.modal-body #Excess').val($("TD", row).eq(11).html());
    $('.modal-body #Amount').val($("TD", row).eq(12).html());
    $('.modal-body #MedicineType').val($("TD", row).eq(13).html());
    if ($("TD", row).eq(13).html() == "Monthly") {
        $('.modal-body #NOMD').attr("hidden", false);
    }
    $('.modal-body #NOM').val($("TD", row).eq(14).html());
    $('.modal-body #Lock').val($("TD", row).eq(16).html()).prop('selected', true);
    $('.modal-footer #AddApproval').val("Edit");
    $('.modal-title').html("Edit");
    $('#ApprovalEditModal').modal();
}
function RemoveMedicine(button) {
    //Determine the reference of the Row using the Button.
    var row = $(button).closest("TR");
    var name = $("TD", row).eq(1).html();
    bootbox.confirm("Do you want to delete: " + name, function (result) {
        if (result) {
            //Delete the Table row using it's Index.
            //---------------------------
            var row = $(button).closest("TR");
            var table = $("#CardMedicines")[0];
            table.deleteRow(row[0].rowIndex);

        }
    });

}