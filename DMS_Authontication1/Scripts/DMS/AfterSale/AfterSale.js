
$(document).ready(function () {
    $("#Help").click(function () {
        introJs().start();
    });
   
});
$(function () {
    $('.CreatDate').datetimepicker();
    $('#Department').select2();
    $('#CompanyNameList').select2();
    $('#VisitReasonList').select2();
    $('#FeedBackText').select2();
    $('#feedBackDiv').hide();
    $('#CompanyTypeNew').hide();
    if ($("#HasFeedBack").is(":checked")) {
        $('#feedBackDiv').show();
    }
    else {
        $('#feedBackDiv').hide();
    }
    if ($("#New").is(":checked")) {
        $('#CompanyTypeNew').show();
        $('#CompanyTypeOld').hide();
    }
    else {
        $('#CompanyTypeNew').hide();
        $('#CompanyTypeOld').show();
    }
});
function ShowFeedBackDiv() {
    if ($("#HasFeedBack").is(":checked")) {
        $('#feedBackDiv').show();
    }
    else {
        $('#feedBackDiv').hide();
    }
}

function ShowCompanyDiv() {
    if ($("#New").is(":checked")) {
        $('#CompanyTypeNew').show();
        $('#CompanyTypeOld').hide();
    }
    else {
        $('#CompanyTypeNew').hide();
        $('#CompanyTypeOld').show();
    }
}





//Save Data
function Submit_Save() {
    debugger;
    if ($('#txtSearchCard').val() != "") {
        var mainSer = $("#main_services option:selected").index();
        var mainSerEmer = $("#main_services option:selected").val();
        var subser = $("#Services option:selected").index();
        var serviceID = 0;
        var compPercent = parseFloat(100 - $(".celling-pert").val());
        if ($("#txtTotal").val() != "") {
            if ($("#PhoneNumber").val() != "") {
                if ($("#NationalId").val() != "") {
                    if ($("#ClaimNumber").val() != "") {
                        if (mainSerEmer != "11104") {
                            if ($("#main_services option:selected").index() > 0) {
                                if (mainSer == 2) {
                                    if (subser > 0) {
                                        //if (subser != 4) {
                                        if ($("#ddlDiagnoises").val() != "") {
                                            var oArea = document.getElementById('ddlDiagnoises');
                                            var aNewlines = oArea.value.split("\n");
                                            var AllDiagnos = "";
                                            for (i = 0; i < aNewlines.length; i++) {
                                                if (aNewlines[i] != "")
                                                    AllDiagnos += aNewlines[i] + '-';
                                            }

                                            //  var iNewlineCount = aNewlines.length();
                                            serviceID = $("#Services").val();
                                            var data = {
                                                C_Com_ID: $('#Company_ID').val(),
                                                Provider_Code: $("#providerHospialCode").val(),
                                                Card_ID: $('#txtSearchCard').val(),
                                                Services_ID: serviceID,
                                                ServType: mainSerEmer,
                                                Person_Payment: $("#PatientCoPayment").val(),
                                                Services: AllDiagnos,
                                                Contract_Number: $('#Con_Num').val(),
                                                Class_Code: $('#Class_Code').val(),
                                                Comp_Payment: $("#CompanyPayment").val(),
                                                Phone: $("#PhoneNumber").val(),
                                                NATIONAL_ID: $("#NationalId").val(),
                                                CLAIM_NO: $("#ClaimNumber").val(),
                                                COMP_PERC: compPercent,
                                                OverInsurance: $("#OverInsurance").val(),
                                                TotalValue: $("#txtTotal").val(),
                                                Total_Cash: $("#txtTotalCopayment").val(),
                                                Cash: $("#Cash").val(),
                                            };
                                            $.ajax({
                                                type: "POST",
                                                url: '/Hospital/Save',
                                                data: (data),
                                                success: function (rwt) {

                                                    if (rwt.msg != "OK") {
                                                        bootbox.alert(rwt.result.toString());
                                                    }
                                                    else {

                                                        bootbox.confirm({
                                                            title: "حالة الطلب ",
                                                            message: rwt.result.toString(),
                                                            buttons: {
                                                                cancel: {
                                                                    label: '<i class="fa fa-reply"></i> جديد'
                                                                },
                                                                confirm: {
                                                                    label: '<i class="fa fa-check"></i> طباعه'
                                                                }
                                                            },
                                                            callback: function (result) {
                                                                $('#compEmp_EMP_ANAME').val("");
                                                                $('#compEmp_INS_END_DATE').val("");
                                                                $("#contractComp_C_ANAME").val("");
                                                                $("#Services").empty();
                                                                $("#ddlDiagnoises").empty();
                                                                $("#txtTotal").val("");
                                                                $(".celling-pert").val("");
                                                                $("#txtTotalCopayment").val("");
                                                                $("#Limit").val("");
                                                                $('#compEmp_INS_START_DATE').val("");

                                                                $("#PhoneNumber").val("");
                                                                $("#ClaimNumber").val("");
                                                                $("#NationalId").val("");
                                                                $("#CompanyPayment").val("");
                                                                $("#PatientCoPayment").val("");
                                                                $("#OverInsurance").val("");
                                                                $("#Cash").val("");

                                                                //$('#AddDiagnoise').attr('disabled', true);
                                                                //$('#RemoveDiagnoise').attr('disabled', true);
                                                                $("#Physical").html("");
                                                                $("#main_services").html("");
                                                                $("#div_of_serv").hide();
                                                                if (result == false) {

                                                                }
                                                                else {
                                                                    window.open('/Hospital/PrintRequest?ID=' + rwt.ID);

                                                                }
                                                            }
                                                        });



                                                    }                                                //bootbox.alert({
                                                    //    message: rwt.result.toString(),
                                                    //    callback: function () {
                                                    //        document.location.reload();
                                                    //    }
                                                    //});

                                                },
                                                error: function (err) {
                                                    bootbox.alert("You'r session has been expired please  log in again and try it ...!");
                                                    //bootbox.alert("Error DATA 24 !");
                                                }
                                            });
                                        }
                                        else {
                                            alert("قم بتحديد الخدمة...!");

                                        }
                                        //}
                                        //else if (subser == 4) {
                                        //    if ($("#Diagnoise").val() != "") {
                                        //        oArea = document.getElementById('Diagnoise');
                                        //        aNewlines = oArea.value.split("\n");
                                        //        AllDiagnos = "";
                                        //        for (i = 0; i < aNewlines.length; i++) {
                                        //            if (aNewlines[i] != "")
                                        //                AllDiagnos += aNewlines[i] + '-';
                                        //        }
                                        //        serviceID = $("#Services").val();
                                        //        data = {
                                        //            C_Com_ID: $('#Company_ID').val(),
                                        //            Provider_Code: $("#providerHospialCode").val(),
                                        //            Card_ID: $('#txtSearchCard').val(),
                                        //            Services_ID: serviceID,
                                        //            Total_Cash: $("#txtTotal").val(),
                                        //            Person_Payment: $("#txtTotalCopayment").val(),
                                        //            Services: AllDiagnos,
                                        //            EmergancyTxt: $("#emergancyTxt").val(),
                                        //            Contract_Number: $('#Con_Num').val(),
                                        //            Class_Code: $('#Class_Code').val(),
                                        //            Comp_Payment: $("#Com_Cach").val()
                                        //        };
                                        //        $.ajax({
                                        //            type: "POST",
                                        //            url: '/User/Save',
                                        //            data: (data),
                                        //            success: function (rwt) {

                                        //                if (rwt.result.toString() == "لا يمكن صرف لتعدي الحد الاقصي") {
                                        //                    alert(rwt.result.toString());
                                        //                }
                                        //                else {
                                        //                    bootbox.alert({
                                        //                        message: rwt.result.toString(),
                                        //                        callback: function () {
                                        //                            document.location.reload();
                                        //                        }
                                        //                    });
                                        //                }
                                        //            },
                                        //            error: function (err) {
                                        //                bootbox.alert("Error DATA !");
                                        //            }
                                        //        });

                                        //    }
                                        //    else {
                                        //        alert("قم بتحديد الخدمة ...!");

                                        //    }
                                        //}
                                        //else {
                                        //    alert("قم بتحديد الخدمة ...!");
                                        //    $("#AddDiagnoise").focus();
                                        //}
                                    }
                                    else {
                                        alert("Please Choose Services ...!");
                                        $("#Services").focus();
                                    }
                                }
                                else if (mainSer == 1) {
                                    serviceID = $("#main_services").val();
                                    data = {
                                        C_Com_ID: $('#Company_ID').val(),
                                        Provider_Code: $("#providerHospialCode").val(),
                                        Card_ID: $('#txtSearchCard').val(),
                                        Services_ID: serviceID,
                                        Total_Cash: $("#txtTotal").val(),
                                        Person_Payment: $("#txtTotalCopayment").val(),
                                        Services: $("#Diagnoise").val(),
                                        EmergancyTxt: $("#emergancyTxt").val(),
                                        Contract_Number: $('#Con_Num').val(),
                                        Class_Code: $('#Class_Code').val(),
                                        Comp_Payment: $("#Com_Cach").val()
                                    };
                                    $.ajax({
                                        type: "POST",
                                        url: '/User/Save',
                                        data: (data),
                                        success: function (rwt) {
                                            if (rwt.msg != "OK") {
                                                alert(rwt.result.toString());
                                            }
                                            else {
                                                bootbox.alert({
                                                    message: rwt.result.toString(),
                                                    callback: function () {
                                                        document.location.reload();
                                                    }
                                                });
                                            }
                                        },
                                        error: function (err) {
                                            bootbox.alert("Error DATA !");
                                        }
                                    });
                                }
                                else if (mainSer == 3) {
                                    serviceID = $("#main_services").val();
                                    if ($('#emergancyTxt').val() != "") {
                                        var data = {
                                            C_Com_ID: $('#Company_ID').val(),
                                            Provider_Code: $("#providerHospialCode").val(),
                                            Card_ID: $('#txtSearchCard').val(),
                                            Services_ID: serviceID,
                                            ServType: mainSerEmer,
                                            Person_Payment: $("#PatientCoPayment").val(),
                                            Services: AllDiagnos,
                                            Contract_Number: $('#Con_Num').val(),
                                            Class_Code: $('#Class_Code').val(),
                                            Comp_Payment: $("#CompanyPayment").val(),
                                            Phone: $("#PhoneNumber").val(),
                                            NATIONAL_ID: $("#NationalId").val(),
                                            CLAIM_NO: $("#ClaimNumber").val(),
                                            COMP_PERC: compPercent,
                                            OverInsurance: $("#OverInsurance").val(),
                                            TotalValue: $("#txtTotal").val(),
                                            Total_Cash: $("#txtTotalCopayment").val(),
                                            Cash: $("#Cash").val(),
                                        };
                                        $.ajax({
                                            type: "POST",
                                            url: '/Hospital/Save',
                                            data: (data),
                                            success: function (rwt) {
                                                if (rwt.msg != "OK") {
                                                    alert(rwt.result.toString());
                                                }
                                                else {

                                                    bootbox.confirm({
                                                        title: "حالة الطلب ",
                                                        message: rwt.result.toString(),
                                                        buttons: {
                                                            cancel: {
                                                                label: '<i class="fa fa-reply"></i> جديد'
                                                            },
                                                            confirm: {
                                                                label: '<i class="fa fa-check"></i> طباعه'
                                                            }
                                                        },
                                                        callback: function (result) {
                                                            $('#compEmp_EMP_ANAME').val("");
                                                            $('#compEmp_INS_END_DATE').val("");
                                                            $("#contractComp_C_ANAME").val("");
                                                            $("#Services").empty();
                                                            $("#ddlDiagnoises").empty();
                                                            $("#txtTotal").val("");
                                                            $(".celling-pert").val("");
                                                            $("#txtTotalCopayment").val("");
                                                            $("#Limit").val("");
                                                            $('#compEmp_INS_START_DATE').val("");

                                                            $("#PhoneNumber").val("");
                                                            $("#ClaimNumber").val("");
                                                            $("#NationalId").val("");
                                                            $("#CompanyPayment").val("");
                                                            $("#PatientCoPayment").val("");
                                                            $("#OverInsurance").val("");
                                                            $("#Cash").val("");

                                                            //$('#AddDiagnoise').attr('disabled', true);
                                                            //$('#RemoveDiagnoise').attr('disabled', true);
                                                            $("#Physical").html("");
                                                            $("#main_services").html("");
                                                            $("#div_of_serv").hide();
                                                            if (result == false) {

                                                            }
                                                            else {
                                                                window.open('/Hospital/PrintRequest?ID=' + rwt.ID);

                                                            }
                                                        }
                                                    });



                                                }
                                            },
                                            error: function (err) {
                                                bootbox.alert("Error DATA !");
                                            }
                                        });
                                    }
                                    else {
                                        alert("Please Enter Emergance Data ...!");
                                        $("#emergancyTxt").focus();

                                    }

                                }
                                else {
                                    alert("Please Complete Data ...!");
                                }
                            }

                            else {
                                alert("Please Choose Main Services.....!");
                                $("#main_services").focus();
                            }
                        }
                        else if (mainSerEmer == "11104") {
                            serviceID = $("#main_services").val();
                            if ($('#emergancyTxt').val() != "") {
                                var data = {
                                    C_Com_ID: $('#Company_ID').val(),
                                    Provider_Code: $("#providerHospialCode").val(),
                                    Card_ID: $('#txtSearchCard').val(),
                                    Services_ID: serviceID,
                                    ServType: mainSerEmer,
                                    Person_Payment: $("#PatientCoPayment").val(),
                                    Services: AllDiagnos,
                                    Contract_Number: $('#Con_Num').val(),
                                    Class_Code: $('#Class_Code').val(),
                                    Comp_Payment: $("#CompanyPayment").val(),
                                    Phone: $("#PhoneNumber").val(),
                                    NATIONAL_ID: $("#NationalId").val(),
                                    CLAIM_NO: $("#ClaimNumber").val(),
                                    COMP_PERC: compPercent,
                                    OverInsurance: $("#OverInsurance").val(),
                                    TotalValue: $("#txtTotal").val(),
                                    Total_Cash: $("#txtTotalCopayment").val(),
                                    Cash: $("#Cash").val(),
                                };
                                $.ajax({
                                    type: "POST",
                                    url: '/Hospital/Save',
                                    data: (data),
                                    success: function (rwt) {
                                        if (rwt.msg.toString() != "OK") {
                                            alert(rwt.result.toString());
                                        }
                                        else {

                                            bootbox.confirm({
                                                title: "حالة الطلب ",
                                                message: rwt.result.toString(),
                                                buttons: {
                                                    cancel: {
                                                        label: '<i class="fa fa-reply"></i> جديد'
                                                    },
                                                    confirm: {
                                                        label: '<i class="fa fa-check"></i> طباعه'
                                                    }
                                                },
                                                callback: function (result) {
                                                    $('#compEmp_EMP_ANAME').val("");
                                                    $('#compEmp_INS_END_DATE').val("");
                                                    $("#contractComp_C_ANAME").val("");
                                                    $("#Services").empty();
                                                    $("#ddlDiagnoises").empty();
                                                    $("#txtTotal").val("");
                                                    $(".celling-pert").val("");
                                                    $("#txtTotalCopayment").val("");
                                                    $("#Limit").val("");
                                                    $('#compEmp_INS_START_DATE').val("");

                                                    $("#PhoneNumber").val("");
                                                    $("#ClaimNumber").val("");
                                                    $("#NationalId").val("");
                                                    $("#CompanyPayment").val("");
                                                    $("#PatientCoPayment").val("");
                                                    $("#OverInsurance").val("");
                                                    $("#Cash").val("");

                                                    //$('#AddDiagnoise').attr('disabled', true);
                                                    //$('#RemoveDiagnoise').attr('disabled', true);
                                                    $("#Physical").html("");
                                                    $("#main_services").html("");
                                                    $("#div_of_serv").hide();
                                                    if (result == false) {

                                                    }
                                                    else {
                                                        window.open('/Hospital/PrintRequest?ID=' + rwt.ID);

                                                    }
                                                }
                                            });



                                        }
                                    },
                                    error: function (err) {
                                        bootbox.alert("Error DATA !");
                                    }
                                });
                            }
                            else {
                                alert("Please Enter Emergance Data ...!");
                                $("#emergancyTxt").focus();

                            }
                        }
                    }
                    else {
                        alert("Please Enter Claim Number ...!");
                        $("#txtTotal").focus();
                    }
                }
                else {
                    alert("Please Enter National Id ...!");
                    $("#txtTotal").focus();
                }
            }
            else {
                alert("Please Enter Phone Number ...!");
                $("#txtTotal").focus();
            }
        }
        else {
            alert("Please Enter Total Invoice ...!");
            $("#txtTotal").focus();
        }
    }

    else {
        bootbox.alert("Please Insert Card ID");
        $("#txtSearchCard").focus();
    }
}

function Accounting_Info() {

    var Person_Payments = ($("#Seession_Price").val() * $("#Seession_Num").val() * $("#insurance_LIVEL").val()) / 100.00;
    $("#Patient_Pay").val(Person_Payments);
    var tot = $("#Seession_Price").val() * $("#Seession_Num").val();
    $("#Total_Amount").val(tot);
}

function PersonPayments() {
    debugger;
    var Limit = parseFloat($("#max_amount0").val());
    var Total = parseFloat($("#txtTotal").val());
    var PatientPercent = $(".celling-pert").val();
    var isCash = parseFloat($("#IsCash").val());
    if (Limit > Total || Limit == Total) {
        var PersonPayment = parseFloat(PatientPercent / 100) * Total;
        var CompanyPayment = Total - PersonPayment;
        $("#CompanyPayment").val(CompanyPayment);
        $("#PatientCoPayment").val(PersonPayment);
        $("#OverInsurance").val(0);
        $("#Cash").val(0);
        $("#txtTotalCopayment").val(PersonPayment);
    }
    else if (Limit < Total && isCash == 0) {
        var overInsurace = Total - Limit;
        var PersonPayment = parseFloat(PatientPercent / 100) * Limit;
        var CompanyPayment = Limit - PersonPayment;
        $("#CompanyPayment").val(CompanyPayment);
        $("#PatientCoPayment").val(PersonPayment);
        $("#OverInsurance").val(overInsurace);
        $("#Cash").val(0);
        $("#txtTotalCopayment").val(PersonPayment + overInsurace);
    }
    else {
        $("#CompanyPayment").val(0);
        $("#PatientCoPayment").val(0);
        $("#OverInsurance").val(0);
        $("#Cash").val(Total);
        $("#txtTotalCopayment").val(0);
    }

}

function PersonPayments1() {
    var mainSerEmer = $("#main_services option:selected").val();
    //if (mainSerEmer != "11104") {
    debugger;
    var Person_Payments = (parseFloat($("#txtTotal").val()) * parseFloat($(".celling-pert").val() / 100.00));
    if (isNaN(Person_Payments)) {
        Person_Payments = 0;
    }

    var price = $("#txtTotal").val();
    //if ($("#main_services option:selected").index() == 3) {
    //    // $(".celling-pert").val() = 100;
    //    // $(".celling-pert").text("100");
    //    Person_Payments = 0;
    //}

    var Comp_Payment = (parseFloat($("#txtTotal").val()) * parseFloat((100 - $(".celling-pert").val()) / 100.00));
    if (isNaN(Comp_Payment)) {
        Comp_Payment = 0;
    }
    var insurance_LIVEL = $("#insurance_LIVEL").val();
    var Max_Amount3 = $("#max_amount0").val();
    //alert(Max_Amount3);
    if (Max_Amount3 == -2) {
        $("#txtTotalCopayment").val(Person_Payments);
    }
    else {
        if (Comp_Payment > Max_Amount3) {
            alert("لا يمكن تقديم الخدمة طبقا لاسعار التعاقد وسوف يتم دفع خدمات الموظف نقدا ");
            var x = parseFloat(Max_Amount3 * 100 / (100 - $(".celling-pert").val()));

            var remain = parseFloat(price - x);
            alert(" سيتم دفع الفرق نقدا بقيمه  " + remain);
            $("#max_amount1").val(-1);
            Comp_Payment = Max_Amount3;
            //alert(Max_Amount3);
            Person_Payments = parseFloat((x * $(".celling-pert").val() / 100.00) + remain);

            //parseFloat(Max_Amount3) * parseFloat((100 - $(".celling-pert").val()) / 100.00);
            $("#txtTotalCopayment").val(Person_Payments);
        }
        else
            $("#txtTotalCopayment").val(Person_Payments);
    }
    //}

    //else {
    //    Person_Payments = (parseFloat($("#txtTotal").val()) * parseFloat((100 - 100) / 100.00));
    //    if (isNaN(Person_Payments)) {
    //        Person_Payments = 0;
    //    }


    //    //var price = $("#txtTotal").val();
    //    if ($("#main_services option:selected").index() == 3) {
    //        // $(".celling-pert").val() = 100;
    //        // $(".celling-pert").text("100");
    //        Person_Payments = 0;
    //    }

    //    Comp_Payment = (parseFloat($("#txtTotal").val()) * parseFloat(100 / 100.00));
    //    if (isNaN(Comp_Payment)) {
    //        Comp_Payment = 0;
    //    }

    //    $("#txtTotalCopayment").val(Math.round(Person_Payments));

    //}

    $("#Com_Cach").val(Math.round(Comp_Payment));
    ////var ComCach = $("#txtTotal").val() - Person_Payments;
    //alert(Comp_Payment); Com_Cach
}
function Print(AppCode) {
    var x = String(AppCode);
    //  window.location.reload();
    window.open('/Hospital/Print?ApprovalCode=' + AppCode);
}

function selectionChanged() {
    var txt;
    var r = confirm("هل تريد طباعة موافقات مرة اخري ؟");
    if (r == true) {
        var data = {
            CardId: $('#txtSearchCard').val(),
            ContractNum: $('#Con_Num').val(),
            ClassCode: $('#Class_Code').val(),
            ProvideCode: '0'
        };
        $.ajax({
            type: "POST",
            url: '/User/Get_Approvels',
            data: (data),
            success: function (apprs) {

                var setData2 = $("#Approvals Tbody");
                setData2.empty();
                for (var i = 0; i < apprs.length; i++) {
                    var data = "<tr >" +

                        "<td>" + apprs[i].Code + "</td>" +
                        "<td>" + apprs[i].Approval_Type + "</td>" +
                        "<td>" + apprs[i].Medical_Replay + "</td>" +
                        "<td>" + apprs[i].Value_After + "</td>" +
                        "<td>" + apprs[i].Recieve_Date + "</td>" +
                        "<td>" + apprs[i].Created_Date + "</td>" +
                        "<td>" + apprs[i].End_Date + "</td>" +
                        "<td>" + apprs[i].Expaire_Date + "</td>" +
                        "<td >" + "<Button  class='btn btn-Primary ' onclick='Print(\"" + apprs[i].Code + "\");'>Print</Button>" + "</td>" +
                        "</tr>"
                    setData2.append(data);
                }
                $("#Approvals").DataTable();
                $("#ApprovalModal").modal();
                var xs = $("#main_services").val();
                var sas = $('#Con_Num').val();
                var xdfss = $("#Class_Code").val();
                var sgfdgdas = $('#Company_ID').val();
                var xewws = $("#txtSearchCard").val();
            },
            error: function (err) {
                bootbox.alert("No Approvals Retriev ... !");
            }
        });
        $("#emergancyTxt").hide();
    } else {
        // txt = "You pressed Cancel!";
        // $('#Approvals').dataTable().fnClearTable();
        //$('#ApprovalModal').remove();
        $('#ApprovalModal').modal('hide');

        // var table = $('#ApprovalModal').DataTable();
        // table.destroy();
        //  $('#Approvals').dataTable().fnDestroy();
    }
    if ($("#Diagnoise").val() == "") {
        //  $("#main_services").val(0);
    }
}
function selectionChang() {
    if ($("#Diagnoise").val() == "") {
        $("#Services").val(0);
        $("#AddDiagnoise").attr("disabled", true);
        $("#RemoveDiagnoise").attr("disabled", true);

    }
}