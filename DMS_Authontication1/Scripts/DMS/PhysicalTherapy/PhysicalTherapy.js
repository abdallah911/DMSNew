var input = document.getElementById("txtSearchCard");
input.addEventListener("keyup", function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        $('#Search').click();
    }
});
var v = 0;
var x = 0;
var CardId;
var Genderr;
var Age;
var dat1;
var ses_num = 0;
$(function () {
    $('#Search').map(function () {

        $('#compEmp_EMP_ANAME').val("");
        $('#compEmp_INS_END_DATE').val("");
        $("#contractComp_C_ANAME").val("");
        $("#Services").empty();
        $("#txtSearchCard").val("");
        $(".celling-pert").val("");
        $("#Physical").html("");
        $("#pan").hide();
        $("#SaveAll").html('<input id="submit" type="button"  onclick="Submit_Save()" value="Save" class="btn btn-success" style="padding:10px 20px" />');

    });

    $('#Search').click(function () {
        var V = 0;
        $('#compEmp_EMP_ANAME').val("");
        $('#compEmp_INS_END_DATE').val("");
        $("#contractComp_C_ANAME").val("");
        $("#Services").empty();
        $("#Diagnoise").val("");
        $("#txtTotal").val("");
        $(".celling-pert").val("");
        $("#txtTotalCopayment").val("");
        $('#AddDiagnoise').attr('disabled', true);
        $('#RemoveDiagnoise').attr('disabled', true);
        $("#Physical").html("");
        $("#pan").hide();
        $("#SaveAll").html('<input id="submit" type="button"  onclick="Submit_Save()" value="Save" class="btn btn-success" style="padding:10px 20px" />');

        if ($("#txtSearchCard").val() != "") {
            $.ajax({
                url: '/Hospital/AddCard/',
                data: { id: $('#txtSearchCard').val(), provider: $("#providerHospialCode").val() },
                dataType: 'Json',
                success: function (r) {


                    if (r.Success == "True") {
                        if (r.Data.length == 1) {
                            if (r.Data[0].HOSPITAL_DEGREE <= r.Data[0].Provider_Level) {
                                if (r.Data[0].TERMINATE_FLAG == 'N' || r.Data[0].TERMINATE_FLAG == null) {

                                    var com_Name = r.Data[0].C_ENAME;
                                    if (r.Data[0].INS_START_DATE != null) {
                                        var MyDate_String_Value = r.Data[0].INS_START_DATE;
                                        var value = new Date
                                            (
                                                parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                                    } else {
                                        dat = null;
                                    }
                                    if (r.Data[0].INS_END_DATE != null) {
                                        //end date
                                        var MyDate_String_Value1 = r.Data[0].INS_END_DATE;
                                        var value1 = new Date
                                            (
                                                parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                    }
                                    else {
                                        dat1 = null;
                                    }
                                    CardId = r.Data[0].CARD_ID;
                                    var ArName = r.Data[0].EMP_ANAME;
                                    ///r.Data[0].EMP_ENAME ;
                                    ///dat ;
                                    var EndDate = dat1;
                                    $('#txtSearchCard').val(CardId);
                                    $('#compEmp_EMP_ANAME').val(ArName);
                                    $('#compEmp_INS_END_DATE').val(EndDate);
                                    $("#contractComp_C_ANAME").val(com_Name);


                                    $('#Con_Num').val(r.Data[0].CONTRACT_NO);
                                    $('#Class_Code').val(r.Data[0].CLASS_CODE);
                                    $('#Company_ID').val(r.Data[0].COMP_ID);

                                    var serid = 11204;
                                    $.ajax({
                                        type: 'POST',
                                        url: '/Hospital/GetSer_Services/',
                                        dataType: 'json',
                                        data: {
                                            SubServiceCode: serid, Contract_Number: $('#Con_Num').val()
                                            , Class_Code: $('#Class_Code').val()
                                            , C_Comp_ID: $('#Company_ID').val()
                                            , CardId: $('#txtSearchCard').val()
                                        },

                                        success: function (r) {
                                            $("#insurance_LIVEL").val(r.celing);
                                            $("#max_amount0").val(r.max_am);


                                            $.ajax({
                                                url: '/PhysicalTherapy/Physical_Therapy_View',
                                                type: 'GET',
                                                success: function (data) {
                                                    $("#Physical").html(data);
                                                    $("#SaveAll").html("");
                                                    $("#Patient_Celling").val(100 - r.celing + "%");
                                                    $("#Comp_Celling").val((r.celing) + "%");

                                                    //  Fill list of Specialist
                                                    $.ajax({
                                                        type: 'POST',
                                                        url: '/Hospital/Get_Specialist/',
                                                        dataType: 'json',
                                                        data: {
                                                            Services_id: 11204, cardID: $('#txtSearchCard').val(),
                                                            providerID: $("#providerHospialCode").val(),
                                                        },

                                                        success: function (r) {
                                                            var myType = r.msg;
                                                            if (myType === "ok") {
                                                                $("#pan").show();
                                                                $("#Specialist").empty();
                                                                $("#Specialist").append('<option value="0"> اختر التخصص</option>');
                                                                ////////////
                                                                for (var i = 0; i < r.Specialists.length; i++) {
                                                                    $("#Specialist").append('<option value="'
                                                                        + r.Specialists[i].PR_CODE + '">' + r.Specialists[i].PR_ANAME + r.Specialists[i].PR_CODE + ' </option>');
                                                                }
                                                            }
                                                            else {
                                                                alert(r.msg);
                                                                $("#insurance_LIVEL").val(0);
                                                                $("#max_amount0").val(0);
                                                                $("#Specialist").empty();
                                                                $("#Specialist").append('<option value="0"> اختر التخصص</option>');

                                                                for (var i = 0; i < r.Specialists.length; i++) {
                                                                    $("#Specialist").append('<option value="'
                                                                        + r.Specialists[i].PR_CODE + '">' + r.Specialists[i].PR_ANAME + r.Specialists[i].PR_CODE + ' </option>');
                                                                }
                                                                //$("#pan").hide();
                                                                //alert("لا يمكن تقديم هذة الخدمة حيث انها غير مغطاه.");
                                                            }
                                                        },
                                                        error:
                                                            function () {
                                                                alert("error list");
                                                            }
                                                    });
                                                },
                                                error: function () {
                                                    alert("error");
                                                }
                                            });

                                        },
                                        error: function () {

                                            bootbox.alert('لا يمكن تقديم الخدمة لهذا الموظف ');
                                        }
                                    });


                                }
                                ////
                                else if (r.Data[0].TERMINATE_FLAG == 'Y') {
                                    var EndDateIns = r.Data[0].INS_END_DATE;
                                    var newDateIns = EndDateIns.split('/').reverse().join('.');
                                    var dateIns = new Date(newDateIns);

                                    var EndDateCard = r.Data[0].TERMINATE_DATE;
                                    var newDateCard = EndDateCard.split('/').reverse().join('.');
                                    var dateCard = new Date(newDateCard);

                                    var today = new Date();
                                    var dd = today.getDate();
                                    var mm = today.getMonth() + 1; //January is 0!
                                    var yyyy = today.getFullYear();
                                    var CurrentDate = new Date(yyyy, mm, dd);


                                    if (r.Data[0].TERMINATE_DATE >= r.Data[0].Now) {
                                        var com_Name = r.Data[0].C_ENAME;
                                        if (r.Data[0].INS_START_DATE != null) {
                                            var MyDate_String_Value = r.Data[0].INS_START_DATE;
                                            var value = new Date
                                                (
                                                    parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                                );
                                            var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                                        } else {
                                            dat = null;
                                        }
                                        if (r.Data[0].INS_END_DATE != null) {
                                            //end date
                                            var MyDate_String_Value1 = r.Data[0].INS_END_DATE;
                                            var value1 = new Date
                                                (
                                                    parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                                );
                                            var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                        }
                                        else {
                                            dat1 = null;
                                        }
                                        CardId = r.Data[0].CARD_ID;
                                        var ArName = r.Data[0].EMP_ANAME;
                                        ///r.Data[0].EMP_ENAME ;
                                        ///dat ;
                                        var EndDate = dat1;

                                        $('#txtSearchCard').val(CardId);
                                        $('#compEmp_EMP_ANAME').val(ArName);
                                        $('#compEmp_INS_END_DATE').val(EndDate);
                                        $("#contractComp_C_ANAME").val(com_Name);

                                        $('#Con_Num').val(r.Data[0].CONTRACT_NO);
                                        $('#Class_Code').val(r.Data[0].CLASS_CODE);
                                        $('#Company_ID').val(r.Data[0].COMP_ID);


                                        var serid = 11204;
                                        $.ajax({
                                            type: 'POST',
                                            url: '/Hospital/GetSer_Services/',
                                            dataType: 'json',
                                            data: {
                                                SubServiceCode: serid, Contract_Number: $('#Con_Num').val()
                                                , Class_Code: $('#Class_Code').val()
                                                , C_Comp_ID: $('#Company_ID').val()
                                                , CardId: $('#txtSearchCard').val()
                                            },

                                            success: function (r) {
                                                $("#insurance_LIVEL").val(r.celing);
                                                $("#max_amount0").val(r.max_am);
                                                $("#pan").show();

                                                $.ajax({
                                                    url: '/PhysicalTherapy/PhysicalTherapyView',
                                                    type: 'GET',
                                                    success: function (data) {
                                                        $("#Physical").html(data);
                                                        $("#SaveAll").html("");
                                                        $("#Patient_Celling").val(100 - r.celing + "%");
                                                        $("#Comp_Celling").val((r.celing) + "%");

                                                        //  Fill list of Specialist
                                                        $.ajax({
                                                            type: 'POST',
                                                            url: '/Hospital/Get_Specialist/',
                                                            dataType: 'json',
                                                            data: {
                                                                Services_id: 11204, cardID: $('#txtSearchCard').val(),
                                                                providerID: $("#providerHospialCode").val(),
                                                            },

                                                            success: function (r) {
                                                                var myType = r.msg;
                                                                if (myType === "ok") {
                                                                    $("#pan").show();
                                                                    $("#Specialist").empty();
                                                                    $("#Specialist").append('<option value="0"> اختر التخصص</option>');
                                                                    ////////////
                                                                    for (var i = 0; i < r.Specialists.length; i++) {
                                                                        $("#Specialist").append('<option value="'
                                                                            + r.Specialists[i].PR_CODE + '">' + r.Specialists[i].PR_ANAME + r.Specialists[i].PR_CODE + ' </option>');
                                                                    }
                                                                }
                                                                else {
                                                                    alert(r.msg);
                                                                    $("#insurance_LIVEL").val(0);
                                                                    $("#max_amount0").val(0);
                                                                    for (var i = 0; i < r.Specialists.length; i++) {
                                                                        $("#Specialist").append('<option value="'
                                                                            + r.Specialists[i].PR_CODE + '">' + r.Specialists[i].PR_ANAME + r.Specialists[i].PR_CODE + ' </option>');
                                                                    }
                                                                    $("#Specialist").empty();
                                                                    $("#Specialist").append('<option value="0"> اختر التخصص</option>');

                                                                    //$("#pan").hide();
                                                                    //alert("لا يمكن تقديم هذة الخدمة حيث انها غير مغطاه.");
                                                                }
                                                            },
                                                            error:
                                                                function () {
                                                                    alert("error list");
                                                                }
                                                        });
                                                    },
                                                    error: function () {
                                                        alert("error");
                                                    }
                                                });

                                            },
                                            error: function () {

                                                bootbox.alert('لا يمكن تقديم الخدمة لهذا الموظف ');
                                            }
                                        });

                                    }
                                    else {
                                        alert(' لا يمكن تقديم الخدمه لهذا الموظف لانتهاء الكارت');
                                    }
                                }
                                else {
                                    alert(' Invalid Card Number Termintate Flag2 ');
                                }
                            }
                            else {
                                alert(" لا يمكن تقديم الخدمة للموظف الا فى حالات الطوارئ");
                            }

                        }
                        else {

                            alert('please e Check Card Validation ');
                        }
                    }

                    else if (r.Success == "Error") {
                        alert("Invalid Card ID ");
                    }

                    else {
                        alert(" لا يمكن تقديم الخدمة للموظف الا فى حالات الطوارئ");
                        $(".celling-pert").val("100");
                        if (r.Data[0].TERMINATE_FLAG == 'N' || r.Data[0].TERMINATE_FLAG == null) {
                            if (r.Data[0].INS_END_DATE != null) {
                                //end date
                                var MyDate_String_Value1 = r.Data[0].INS_END_DATE;
                                var myTerminateDate_String_Value = r.Data[0].TERMINATE_DATE;
                                var value1 = new Date
                                    (
                                        parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                    );
                                var TerminateDatevalue = new Date
                                    (
                                        parseInt(myTerminateDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                    );
                                var dateTerminate = TerminateDatevalue.getDate() + "/" + (TerminateDatevalue.getMonth() + 1) + "/" + TerminateDatevalue.getFullYear();
                                var datTer = dateTerminate.split("/");
                                var DTer = new Date(datTer[2], parseInt(datTer[1]) - 1, datTer[0]);


                                var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                var d2 = dat1.split("/");
                                var d = new Date();
                                var Mysysdate = d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear();
                                var d1 = Mysysdate.split("/");
                                var from = new Date(d1[2], parseInt(d1[1]) - 1, d1[0]);  // -1 because months are from 0 to 11
                                var to = new Date(d2[2], parseInt(d2[1]) - 1, d2[0]);

                                if (from < to) {

                                    var com_Name = r.Data[0].C_ENAME;
                                    if (r.Data[0].INS_START_DATE != null) {
                                        var MyDate_String_Value = r.Data[0].INS_START_DATE;
                                        var value = new Date
                                            (
                                                parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                                    } else {
                                        dat = null;
                                    }
                                    if (r.Data[0].INS_END_DATE != null) {
                                        //end date
                                        var MyDate_String_Value1 = r.Data[0].INS_END_DATE;
                                        var value1 = new Date
                                            (
                                                parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                    }
                                    else {
                                        dat1 = null;
                                    }
                                    CardId = r.Data[0].CARD_ID;
                                    var ArName = r.Data[0].EMP_ANAME;
                                    ///r[0].EMP_ENAME ;
                                    ///dat ;
                                    var EndDate = dat1;
                                    $('#txtSearchCard').val(CardId);
                                    $('#compEmp_EMP_ANAME').val(ArName);
                                    $('#compEmp_INS_END_DATE').val(EndDate);
                                    $("#contractComp_C_ANAME").val(com_Name);


                                    $('#Con_Num').val(r.Data[0].CONTRACT_NO);
                                    $('#Class_Code').val(r.Data[0].CLASS_CODE);
                                    $('#Company_ID').val(r.Data[0].COMP_ID);
                                    if (!$('#main_services').val()) {
                                        // do something
                                        $("#main_services").append(
                                            '<option value="11104"> Emergancy __ 11104 </option>');
                                        $("#main_services").prop("disabled", true);
                                        $("#emergancyTxt").show();
                                    }



                                }
                                else {
                                    alert(' لا يمكن تقديم الخدمه لهذا الموظف لانتهاء تعاقد الشركة');
                                }
                            }

                        }
                        else if (r.Data[0].TERMINATE_FLAG == 'Y') {
                            var EndDateIns = r.Data[0].INS_END_DATE;
                            var newDateIns = EndDateIns.split('/').reverse().join('.');
                            var dateIns = new Date(newDateIns);

                            var EndDateCard = r.Data[0].TERMINATE_DATE;
                            var newDateCard = EndDateCard.split('/').reverse().join('.');
                            var dateCard = new Date(newDateCard);

                            var today = new Date();
                            var dd = today.getDate();
                            var mm = today.getMonth() + 1; //January is 0!
                            var yyyy = today.getFullYear();
                            var CurrentDate = new Date(yyyy, mm, dd);
                            var myTerminateDate_String_Value = r.Data[0].TERMINATE_DATE;

                            if (r.Data[0].TERMINATE_DATE >= r.Data[0].Now) {
                                var MyDate_String_Value1 = r.Data[0].INS_END_DATE;
                                var value1 = new Date
                                    (
                                        parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                    );
                                var TerminateDatevalue = new Date
                                    (
                                        parseInt(myTerminateDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                    );
                                var dateTerminate = TerminateDatevalue.getDate() + "/" + (TerminateDatevalue.getMonth() + 1) + "/" + TerminateDatevalue.getFullYear();
                                var datTer = dateTerminate.split("/");
                                var DTer = new Date(datTer[2], parseInt(datTer[1]) - 1, datTer[0]);
                                var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                var d2 = dat1.split("/");
                                var d = new Date();
                                var Mysysdate = d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear();
                                var d1 = Mysysdate.split("/");
                                var from = new Date(d1[2], parseInt(d1[1]) - 1, d1[0]);  // -1 because months are from 0 to 11
                                var to = new Date(d2[2], parseInt(d2[1]) - 1, d2[0]);
                                if (from < to && from < DTer) {

                                    var com_Name = r.Data[0].C_ENAME;
                                    if (r.Data[0].INS_START_DATE != null) {
                                        var MyDate_String_Value = r.Data[0].INS_START_DATE;
                                        var value = new Date
                                            (
                                                parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                                    } else {
                                        dat = null;
                                    }
                                    if (r.Data[0].INS_END_DATE != null) {
                                        //end date
                                        var MyDate_String_Value1 = r.Data[0].INS_END_DATE;
                                        var value1 = new Date
                                            (
                                                parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                    }
                                    else {
                                        dat1 = null;
                                    }
                                    CardId = r.Data[0].CARD_ID;
                                    var ArName = r.Data[0].EMP_ANAME;
                                    ///r[0].EMP_ENAME ;
                                    ///dat ;
                                    var EndDate = dat1;

                                    $('#txtSearchCard').val(CardId);
                                    $('#compEmp_EMP_ANAME').val(ArName);
                                    $('#compEmp_INS_END_DATE').val(EndDate);
                                    $("#contractComp_C_ANAME").val(com_Name);

                                    $('#Con_Num').val(r.Data[0].CONTRACT_NO);
                                    $('#Class_Code').val(r.Data[0].CLASS_CODE);
                                    $('#Company_ID').val(r.Data[0].COMP_ID);

                                    if (!$('#main_services').val()) {
                                        // do something
                                        $("#main_services").append(
                                            '<option value="11104"> Emergancy __ 11104 </option>');
                                        $("#main_services").prop("disabled", true);
                                        $("#emergancyTxt").show();
                                    }
                                }
                                else {
                                    alert('لا يمكن تقديم الخدمه لهذا الموظف لانتهاء تعاقد الشركة');
                                }


                            }
                            else {
                                alert(' لا يمكن تقديم الخدمه لهذا الموظف لانتهاء الكارت');
                            }
                        }
                        else {
                            alert(' Invalid Card Number Termintate Flag2 ');
                        }
                    }



                },
                error: function () {
                    alert("Invalid Card ID ");
                }
            });
        }

        else {
            bootbox.alert("please e Insert Card ID");
        }

    });
});

function MyFunc_Extend() {
    if ($("#Seession_Num").val() != 0 && $("#Members_Num").val() != 0 && $("#Seession_Price").val() != 0) {

        if (ses_num < $("#Seession_Num").val()) {
            var price = $("#Seession_Price").val() * $("#Seession_Num").val();
            var Comp_Payment = ($("#Seession_Price").val() * $("#Seession_Num").val() * $("#insurance_LIVEL").val()) / 100.00;
            var insurance_LIVEL = $("#insurance_LIVEL").val();
            var Max_Amount = 0;
            var Max_Amount2 = $("#max_amount1").val();
            var Max_Amount3 = $("#max_amount0").val();
            if (Max_Amount3 == -2) {

                var data = {
                    Doctor_Name: $("#Doct_Name").val(),
                    Doctor_Special: $("#Specialist").val(),
                    Seession_Num: $("#Seession_Num").val(),
                    Member_Num: $("#Members_Num").val(),
                    Amount: $("#Seession_Price").val()

                };

                $.ajax({
                    type: "POST",
                    url: "/User/Update",
                    data: (data),
                    success: function (rx) {
                        bootbox.alert(rx);
                    },
                    error: function (err) {
                        bootbox.alert("Error DATA !");
                    }
                });

            }
            else {
                if (Max_Amount2 != -1 && Max_Amount2 != 0 && Max_Amount2 < Max_Amount3) {
                    Max_Amount = Max_Amount2;
                }
                else if (Max_Amount2 == -1) {
                    Max_Amount = 0;
                }
                else
                    Max_Amount = $("#max_amount0").val();

                if (Comp_Payment > Max_Amount) {
                    alert("لا يمكن تقديم الخدمة طبقا لاسعار التعاقد وسوف يتم دفع خدمات الموظف نقدا ");
                    var remain = Comp_Payment - Max_Amount;
                    alert(" سيتم دفع الفرق نقدا بقيمه  " + remain);
                    $("#max_amount1").val(-1);
                    $("#Patient_Pay").val(remain);
                }

                else {
                    Max_Amount = Max_Amount - parseInt(Comp_Payment);
                    $("#max_amount1").val(Max_Amount);
                }

                var data = {
                    Doctor_Name: $("#Doct_Name").val(),
                    Doctor_Special: $("#Specialist").val(),
                    Seession_Num: $("#Seession_Num").val(),
                    Member_Num: $("#Members_Num").val(),
                    Amount: $("#Seession_Price").val()

                };

                $.ajax({
                    type: "POST",
                    url: "/User/Update",
                    data: (data),
                    success: function (rx) {
                        bootbox.alert(rx);
                    },
                    error: function (err) {
                        bootbox.alert("Error DATA !");
                    }
                });
            }
        }
        else {
            alert("يجب ادخال عدد جلسات اكبر من " + ses_num)
        }


    }

    else
        bootbox.alert("please e Insert All Records Correctly");
}

function MyFunc_Extend2() {

    if ($("#Seession_Num").val() != 0 && $("#Members_Num").val() != 0 && $("#Seession_Price").val() != 0) {


        var data = {
            Session_Code: $("#Seession_Code").val(),
            Num_Seessions: $("#Num_Seessions").val(),
            Seessions: $("#Seession_Num").val()
        };

        $.ajax({
            type: "POST",
            url: "/PhysicalTherapy/AddSeession",
            data: (data),
            success: function (rx) {

                $("#Details").html("");
                $.ajax({
                    url: '/PhysicalTherapy/LoadDate',
                    data: {
                        seession_code: $("#Seession_Code").val()
                    },
                    dataType: 'Json',

                    success: function (date1) {

                        for (var i = 0; i < date1.length; i++) {
                            $("#Details").append("<tr>"
                                + "<td>"
                                + date1[i]
                                + "</td></tr>");
                        }
                        $("#Num_Seessions").val(date1.length);
                    },
                    error: function () {
                        alert("error Date");
                    }
                });
                if (rx == " لا يمكن اضافة جلسه اخري لقد تعدي الحد الاقصي للجلسات" || rx == "Data Not Saved")
                    bootbox.alert(rx);
            },
            error: function (err) {
                bootbox.alert("Error DATA !");
            }
        });

    }

    else
        bootbox.alert("please e Insert All Records Correctly");
}

function MyFunc() {

    if ($("#Seession_Num").val() != 0 && $("#Members_Num").val() != 0 && $("#Seession_Price").val() != 0 && $("#Specialist").val() != 0) {

        var price = $("#Seession_Price").val() * $("#Seession_Num").val();
        var Comp_Payment = ($("#Seession_Price").val() * $("#Seession_Num").val() * $("#insurance_LIVEL").val()) / 100.00;
        var insurance_LIVEL = $("#insurance_LIVEL").val();
        var Max_Amount = 0;
        var Max_Amount2 = $("#max_amount1").val();
        var Max_Amount3 = $("#max_amount0").val();
        if (Max_Amount3 == -2) {
            var data = {
                Provider_Code: $("#providerHospialCode").val(),
                Card_ID: $("#txtSearchCard").val(),
                Doctor_Name: $("#Doct_Name").val(),
                Doctor_Special: $("#Specialist").val(),
                Seession_Num: $("#Seession_Num").val(),
                Member_Num: $("#Members_Num").val(),
                Contract_Num: $("#Con_Num").val(),
                Class_Code: $("#Class_Code").val(),
                Amount: $("#Seession_Price").val(),
                Services_ID: 11204,
                Total_Cash: $("#Total_Amount").val(),
                Person_Payment: $("#Patient_Pay").val(),
                Comp_Payment: $("#Com_Cach").val()

            };
            $.ajax({
                type: "POST",
                url: "/PhysicalTherapy/Add",
                data: (data),
                success: function (rx) {
                    bootbox.alert(rx);
                    ValidatePetSelection();
                    $("#Total_Amount").val();
                    $("#Patient_Pay").val();
                },
                error: function (err) {
                    bootbox.alert("Error DATA !");
                }
            });


        }
        else {

            if (Max_Amount2 != -1 && Max_Amount2 != 0 && Max_Amount2 < Max_Amount3) {
                Max_Amount = Max_Amount2;
            }
            else if (Max_Amount2 == -1) {
                Max_Amount = 0;
            }
            else
                Max_Amount = $("#max_amount0").val();

            if (Comp_Payment > Max_Amount) {
                alert("لا يمكن تقديم الخدمة طبقا لاسعار التعاقد وسوف يتم دفع خدمات الموظف نقدا ");
                var remain = Comp_Payment - Max_Amount;
                alert(" سيتم دفع الفرق نقدا بقيمه  " + remain);
                $("#max_amount1").val(-1);
                $("#Patient_Pay").val(remain);
            }

            else {
                Max_Amount = Max_Amount - parseInt(Comp_Payment);
                $("#max_amount1").val(Max_Amount);
            }
            data = {
                Provider_Code: $("#providerHospialCode").val(),
                Card_ID: $("#txtSearchCard").val(),
                Doctor_Name: $("#Doct_Name").val(),
                Doctor_Special: $("#Specialist").val(),
                Seession_Num: $("#Seession_Num").val(),
                Member_Num: $("#Members_Num").val(),
                Contract_Num: $("#Con_Num").val(),
                Class_Code: $("#Class_Code").val(),
                Amount: $("#Seession_Price").val(),
                Services_ID: 11204,
                Total_Cash: $("#Total_Amount").val(),
                Person_Payment: $("#Patient_Pay").val(),
                Comp_Payment: $("#Com_Cach").val()

            };

            $.ajax({
                type: "POST",
                url: "/PhysicalTherapy/Add",
                data: (data),
                success: function (rx) {
                    bootbox.alert(rx);
                },
                error: function (err) {
                    bootbox.alert("Error DATA !");
                }
            });

        }





    }

    else
        bootbox.alert("please e Insert All Records Correctly");
}
function ValidatePetSelection() {
    $("#OLD").prop('checked', false);
    $("#EXTEND").prop('checked', false);
    $("#Total_Amount").val();
    $("#Patient_Pay").val();
    $.ajax({
        url: '/PhysicalTherapy/New',
        type: 'GET',
        success: function (data1) {
            $("#Session_Di").html(data1);


            $.ajax({
                type: 'POST',
                url: '/Hospital/Get_Specialist/',
                dataType: 'json',
                data: {
                    Services_id: 11204, cardID: $('#txtSearchCard').val(),
                    providerID: $("#providerHospialCode").val(),
                },

                success: function (list) {

                    if (list.msg == "ok") {
                        $("#pan").show();
                        $("#Specialist").empty();
                        $("#Specialist").append('<option value="0"> اختر التخصص</option>');
                        ////////////
                        for (var i = 0; i < list.Specialists.length; i++) {
                            $("#Specialist").append('<option value="'
                                + list.Specialists[i].PR_CODE + '">' + list.Specialists[i].PR_ANAME + list.Specialists[i].PR_CODE + ' </option>');
                        }
                    }
                    else {
                        $("#pan").hide();
                        alert("لا يمكن تقديم هذة الخدمة حيث انها غير مغطاه.");
                    }
                },
                error:
                    function () {
                        alert("error list");
                    }
            });

        },
        error: function () {
            alert("error");
        }
    });
}

function ValidatePetSelection2() {
    $("#NEW").prop('checked', false);
    $("#EXTEND").prop('checked', false);
    $("#Total_Amount").val("");
    $("#Patient_Pay").val("");

    $.ajax({
        url: '/PhysicalTherapy/OLD',
        type: 'GET',
        success: function (data2) {
            $("#Session_Di").html(data2);
            $("#Details").html("");

            // to fill doctors specialist
            $.ajax({
                type: 'POST',
                url: '/Hospital/Get_Specialist/',
                dataType: 'json',
                data: {
                    Services_id: 11204, cardID: $('#txtSearchCard').val(),
                    providerID: $("#providerHospialCode").val(),
                },

                success: function (list) {
                    if (list.msg == "ok") {
                        $("#pan").show();
                        $("#Specialist").empty();
                        $("#Specialist").append('<option value="0"> اختر التخصص</option>');
                        ////////////
                        for (var i = 0; i < list.Specialists.length; i++) {
                            $("#Specialist").append('<option value="'
                                + list.Specialists[i].PR_CODE + '">' + list.Specialists[i].PR_ANAME + list.Specialists[i].PR_CODE + ' </option>');
                        }
                    }
                    else {
                        $('#Specialist').prop('disabled', 'disabled');
                        $("#pan").hide();
                        alert("لا يمكن تقديم هذة الخدمة حيث انها غير مغطاه.");
                    }
                },
                error:
                    function () {
                        alert("error list");
                    }
            });

            // get seessions data
            $.ajax({
                url: '/PhysicalTherapy/Load',
                data: {
                    Card_ID: $('#txtSearchCard').val(),
                    Provider_Code: $("#providerHospialCode").val(),
                    Contract_Num: $("#Con_Num").val()
                },
                dataType: 'Json',

                success: function (data3) {
                    $("#table1").html("");
                    for (var i = 0; i < data3.length; i++) {
                        $("#table1").append("<tr>"
                            + "<td>"
                            + data3[i].Code
                            + "</td>"
                            + "<td>"
                            + data3[i].Doctor_Name
                            + "</td>"
                            + "<td>"
                            + data3[i].Doctor_Specialist
                            + "</td>"
                            + "<td>"
                            + "<input id='sub_Extend' type='button' value='Select' onclick='MySelect2(" + data3[i].Code + ")' class='btn btn-primary' /> "
                            + "</td>"
                            + "</tr>");
                    }
                },
                error: function () {
                    alert("error");
                }
            });


        },
        error: function () {
            alert("error");
        }
    });


}

function ValidatePetSelection3() {

    $("#NEW").prop('checked', false);
    $("#OLD").prop('checked', false);
    $("#Total_Amount").val("");
    $("#Patient_Pay").val("");
    $.ajax({
        url: '/PhysicalTherapy/Extend',
        type: 'GET',
        success: function (data4) {
            $("#Session_Di").html(data4);
            $.ajax({
                type: 'POST',
                url: '/Hospital/Get_Specialist/',
                dataType: 'json',
                data: {
                    Services_id: 11204, cardID: $('#txtSearchCard').val(),
                    providerID: $("#providerHospialCode").val(),
                },

                success: function (list) {

                    if (list.msg == "ok") {
                        $("#pan").show();
                        $("#Specialist").empty();
                        $("#Specialist").append('<option value="0"> اختر التخصص</option>');
                        ////////////
                        for (var i = 0; i < list.Specialists.length; i++) {
                            $("#Specialist").append('<option value="'
                                + list.Specialists[i].PR_CODE + '">' + list.Specialists[i].PR_ANAME + list.Specialists[i].PR_CODE + ' </option>');
                        }
                    }
                    else {
                        $("#pan").hide();
                        $('#Specialist').prop('disabled', 'disabled');
                        alert("لا يمكن تقديم هذة الخدمة حيث انها غير مغطاه.");
                    }
                },
                error:
                    function () {
                        alert("error list");
                    }
            });

            // get seessions data

            $.ajax({
                url: '/PhysicalTherapy/Load',
                data: {
                    Card_ID: $('#txtSearchCard').val(),
                    Provider_Code: $("#providerHospialCode").val(),
                    Contract_Num: $("#Con_Num").val()
                },
                dataType: 'Json',

                success: function (data3) {
                    $("#table1").html("");
                    for (var i = 0; i < data3.length; i++) {
                        $("#table1").append("<tr>"
                            + "<td>"
                            + data3[i].Code
                            + "</td>"
                            + "<td>"
                            + data3[i].Doctor_Name
                            + "</td>"
                            + "<td>"
                            + data3[i].Doctor_Specialist
                            + "</td>"
                            + "<td>"
                            + "<input id='sub_Extend' type='button' value='Edit' onclick='MySelect(" + data3[i].Code + ")' class='btn btn-primary' /> "
                            + "</td>"
                            + "</tr>");
                    }
                },
                error: function () {
                    alert("error");
                }
            });
        },
        error: function () {
            alert("error");
        }
    });
}

//For Fill Data For Edit

function MySelect(selected) {
    $("#Seession_Num").val("");
    $("#Specialist").val("");
    $("#Doct_Name").val("");
    $("#Members_Num").val("");
    $("#Seession_Price").val("");
    $.ajax({
        url: '/PhysicalTherapy/Get_Seession',
        data: {
            Code: selected
        },
        dataType: 'Json',

        success: function (seession) {
            $("#Seession_Num").val(seession.Seession_Num);
            ses_num = seession.Seession_Num;
            $("#Specialist").val(seession.Doctor_Specialist);
            $('#Specialist').prop('disabled', 'disabled');
            $('#sub_Extend').prop('disabled', false);

            $("#Doct_Name").val(seession.Doctor_Name);
            $("#Members_Num").val(seession.Member_Num);
            $("#Seession_Price").val(seession.Amount);
            $('#Doct_Name').prop('disabled', 'disabled');
            $('#Members_Num').prop('disabled', 'disabled');
            $('#Seession_Price').prop('disabled', 'disabled');
            var Person_Payments = ($("#Seession_Price").val() * $("#Seession_Num").val() * $("#insurance_LIVEL").val()) / 100.00;
            $("#Patient_Pay").val(Person_Payments);
            var tot = $("#Seession_Price").val() * $("#Seession_Num").val();
            $("#Total_Amount").val(tot);
        },
        error: function () {
            alert("لا يمكن مد هذه الجلسه لانهاء المريض جميع الجلسات المسجله مسبقا يرجي تسجيل جلسه جديده");
        }
    });


}

//For Fill Data For Add Seession
function MySelect2(selected) {
    $("#Seession_Num").val("");
    $("#Specialist").val("");
    $("#Doct_Name").val("");
    $("#Members_Num").val("");
    $("#Seession_Price").val("");
    $("#Seession_Code").val(selected);
    $.ajax({
        url: '/PhysicalTherapy/Get_Seession',
        data: {
            Code: selected
        },
        dataType: 'Json',

        success: function (seession) {
            $("#Seession_Num").val(seession.Seession_Num);
            $("#Specialist").val(seession.Doctor_Specialist);
            $('#Specialist').prop('disabled', 'disabled');
            $('#sub_OLD').prop('disabled', false);
            $("#Doct_Name").val(seession.Doctor_Name);
            $("#Members_Num").val(seession.Member_Num);
            $("#Seession_Price").val(seession.Amount);
            var Person_Payments = ($("#Seession_Price").val() * $("#Seession_Num").val() * $("#insurance_LIVEL").val()) / 100.00;
            $("#Patient_Pay").val(Person_Payments);
            var tot = $("#Seession_Price").val() * $("#Seession_Num").val();
            $("#Total_Amount").val(tot);

        },
        error: function () {
            alert(" لا يمكن اضافة جلسه اخري لقد تعدي الحد الاقصي للجلسات");
        }
    });

    //Get Seession Data Like Dates
    $("#Details").html("");

    $.ajax({
        url: '/PhysicalTherapy/LoadDate',
        data: {
            seession_code: selected
        },
        dataType: 'Json',

        success: function (date1) {

            for (var i = 0; i < date1.length; i++) {
                $("#Details").append("<tr>"
                    + "<td>"
                    + date1[i]
                    + "</td></tr>");
            }
            $("#Num_Seessions").val(date1.length);
        },
        error: function () {
            alert("error Date");
        }
    });


}

function Submit_Save() {
    debugger;
    if ($('#txtSearchCard').val() != "") {

        var data = {
            C_Com_ID: $('#Company_ID').val(),
            Provider_Code: $("#providerHospialCode").val(),
            Card_ID: $('#txtSearchCard').val(),
            Services_ID: $("#Services").val(),
            Total_Cash: $("#txtTotal").val(),
            Person_Payment: $("#txtTotalCopayment").val(),
            Services: $("#Diagnoise").val()

        };

        $.ajax({
            type: "POST",
            url: '/PhysicalTherapy/Save',
            data: (data),
            success: function (rwt) {
                bootbox.alert("Saved Datat Success ...");
            },
            error: function (err) {
                bootbox.alert("Error DATA !");
            }
        });

    }

    else
        bootbox.alert("please e Insert Card ID");
}

function Accounting_Info() {

    var Person_Payments = ($("#Seession_Price").val() * $("#Seession_Num").val() * ((100 - $("#insurance_LIVEL").val()) / 100.00));
    $("#Patient_Pay").val(Person_Payments);
    var tot = $("#Seession_Price").val() * $("#Seession_Num").val();
    $("#Total_Amount").val(tot);
    $("#Com_Cach").val(tot - Person_Payments);
}

function asd() {
    var price = $("#Seession_Price").val() * $("#Seession_Num").val();
    var Comp_Payment = ($("#Seession_Price").val() * $("#Seession_Num").val() * $("#insurance_LIVEL").val()) / 100.00;
    var insurance_LIVEL = $("#insurance_LIVEL").val();
    var tot = (price - (price * (insurance_LIVEL / 100)));
    var Max_Amount = 0;
    var Max_Amount2 = $("#max_amount2").val();
    var Max_Amount3 = $("#max_amount").val();
    if (Max_Amount2 != -1 && Max_Amount2 != 0 && Max_Amount2 < Max_Amount3) {
        Max_Amount = Max_Amount2;
    }
    else if (Max_Amount2 == -1) {
        Max_Amount = 0;
    }
    else
        Max_Amount = $("#max_amount").val();

    if (Operations_Price > Max_Amount) {
        alert("لا يمكن تقديم الخدمة طبقا لاسعار التعاقد وسوف يتم دفع خدمات الموظف نقدا ");
        var remain = price - Max_Amount;
        alert(" سيتم دفع الفرق نقدا بقيمه  " + remain);
        $("#max_amount2").val(-1);
    }
    else {
        $("#Price").val(price);
        $("#Patient_Pay").val(tot);
        Max_Amount = Max_Amount - parseInt(price);
        $("#max_amount2").val(Max_Amount);
        $("#details").show();
    }
}