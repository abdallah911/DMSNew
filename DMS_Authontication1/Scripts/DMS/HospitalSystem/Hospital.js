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
var exceptionHospital = null;
var exceptionLabRay = null;
$(document).ready(function () {
    $("#Help").click(function () {
        introJs().start();
    });
    $("#emergancyTxt").hide();
    setInputFilter(document.getElementById("txtTotal"), function (value) {
        return /^\d*$/.test(value); // Allow digits and '.' only, using a RegExp
    });
    $("#ddlDiagnoises").select2();
    $("#SpecialitySelect").select2();
});
function setInputFilter(textbox, inputFilter) {
    ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach(function (event) {
        textbox.addEventListener(event, function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    });
}
var exceptionHospital = null;
var exceptionLabRay = null;
$(function () {

    $('#Search').map(function () {
        exceptionHospital = null;
        exceptionLabRay = null;
        $('#compEmp_EMP_ANAME').val("");
        $('#compEmp_INS_END_DATE').val("");
        $("#contractComp_C_ANAME").val("");
        $("#Services").empty();
        $("#txtSearchCard").val("");
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
        $("#Notetext").val("");

        $("#degreeSelect").val("0");
        $("#doctorNameSelect").val("0");
        $("#DoctortDegree").hide();
        $("#DoctortName").hide();

        //$('#AddDiagnoise').attr('disabled', true);
        //$('#RemoveDiagnoise').attr('disabled', true);
        $("#Physical").html("");
        $("#div_of_serv").hide();
        $("#divNotes").hide();

        $("#other-data").show();
        $("#total_id").show();
        $("#SpecialityDiv").hide();
        $("#serviceselect").hide();

        $("#main_services").html("");
    });

    $('#Search').click(function () {
        var V = 0;
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
        $("#Notetext").val("");

        $("#degreeSelect").val("0");
        $("#doctorNameSelect").val("0");
        $("#DoctortDegree").hide();
        $("#DoctortName").hide();

        $("#other-data").show();
        $("#total_id").show();
        $("#DoctortName").hide();

        $("#Physical").html("");
        $("#main_services").html("");
        $("#div_of_serv").hide();
        $("#divNotes").hide();
        $("#serviceselect").hide();
        $("#SpecialityDiv").hide();
        if ($("#txtSearchCard").val() != "") {
            $.ajax({
                url: '/Hospital/AddCard/',
                data: { id: $('#txtSearchCard').val(), provider: $("#providerHospialCode").val() },
                dataType: 'Json',
                success: function (r) {

                    if (r.Success == "True") {
                        if (r.Data.length == 1) {
                            exceptionHospital = null;
                            exceptionLabRay = null;
                            exceptionHospital = r.Exceptions;
                            $("#main_services").prop("disabled", false);

                            if (r.Data[0].HOSPITAL_DEGREE <= r.Data[0].Provider_Level || r.messages == "ok") {
                                if (r.Data[0].TERMINATE_FLAG == 'N' || r.Data[0].TERMINATE_FLAG == null) {
                                    if (r.Data[0].INS_END_DATE != null) {
                                        //end date
                                        var MyDate_String_Value1 = r.Data[0].INS_END_DATE;
                                        var MyDate_String_Value2 = r.Data[0].INS_START_DATE;
                                        var myTerminateDate_String_Value = r.Data[0].TERMINATE_DATE;
                                        var value1 = new Date
                                            (
                                                parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var value2 = new Date
                                            (
                                                parseInt(MyDate_String_Value2.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var TerminateDatevalue = new Date
                                            (
                                                parseInt(myTerminateDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dateTerminate = TerminateDatevalue.getDate() + "/" + (TerminateDatevalue.getMonth() + 1) + "/" + TerminateDatevalue.getFullYear();
                                        var datTer = dateTerminate.split("/");
                                        var DTer = new Date(datTer[2], parseInt(datTer[1]) - 1, datTer[0]);
                                        var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                        var dat2 = value2.getDate() + "/" + (value2.getMonth() + 1) + "/" + value2.getFullYear();
                                        var d2 = dat1.split("/");
                                        var d = new Date();
                                        var Mysysdate = d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear();
                                        var d1 = Mysysdate.split("/");
                                        var from = new Date(d1[2], parseInt(d1[1]) - 1, d1[0]);  // -1 because months are from 0 to 11
                                        var to = new Date(d2[2], parseInt(d2[1]) - 1, d2[0]);

                                        //if (from < to) {

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
                                        var StartDate = dat2;
                                        $('#txtSearchCard').val(CardId);
                                        $('#compEmp_EMP_ANAME').val(ArName);
                                        $('#compEmp_INS_END_DATE').val(EndDate);
                                        $('#compEmp_INS_START_DATE').val(StartDate);
                                        $("#contractComp_C_ANAME").val(com_Name);
                                        $('#Con_Num').val(r.Data[0].CONTRACT_NO);
                                        $('#Class_Code').val(r.Data[0].CLASS_CODE);
                                        $('#Company_ID').val(r.Data[0].COMP_ID);

                                        if (!$('#main_services').val()) {
                                            // do something
                                            $("#main_services").append('<option value="0"> اختر نوع الخدمه المطلوبه </option>' +
                                                '<option value="111">  عمليات  </option>' +
                                                '<option value="112"> عيادات خارجية  </option>' +
                                                '<option value="11104"> طوارئ  </option>');
                                        }


                                        //}
                                        //else {
                                        //    alert(' لا يمكن تقديم الخدمه لهذا الموظف لانتهاء تعاقد الشركة');
                                        //}
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
                                        var MyDate_String_Value2 = r.Data[0].INS_START_DATE;
                                        var value1 = new Date
                                            (
                                                parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var value2 = new Date
                                            (
                                                parseInt(MyDate_String_Value2.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var TerminateDatevalue = new Date
                                            (
                                                parseInt(myTerminateDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dateTerminate = TerminateDatevalue.getDate() + "/" + (TerminateDatevalue.getMonth() + 1) + "/" + TerminateDatevalue.getFullYear();
                                        var datTer = dateTerminate.split("/");
                                        var DTer = new Date(datTer[2], parseInt(datTer[1]) - 1, datTer[0]);
                                        var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                        var dat2 = value2.getDate() + "/" + (value2.getMonth() + 1) + "/" + value2.getFullYear();
                                        var d2 = dat1.split("/");
                                        var d = new Date();
                                        var Mysysdate = d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear();
                                        var d1 = Mysysdate.split("/");
                                        var from = new Date(d1[2], parseInt(d1[1]) - 1, d1[0]);  // -1 because months are from 0 to 11
                                        var to = new Date(d2[2], parseInt(d2[1]) - 1, d2[0]);
                                        //if (from < to && from < DTer) {

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
                                        var StartDate = dat2;

                                        $('#txtSearchCard').val(CardId);
                                        $('#compEmp_EMP_ANAME').val(ArName);
                                        $('#compEmp_INS_END_DATE').val(EndDate);
                                        $('#compEmp_INS_START_DATE').val(StartDate);

                                        $("#contractComp_C_ANAME").val(com_Name);

                                        $('#Con_Num').val(r.Data[0].CONTRACT_NO);
                                        $('#Class_Code').val(r.Data[0].CLASS_CODE);
                                        $('#Company_ID').val(r.Data[0].COMP_ID);

                                        if (!$('#main_services').val()) {
                                            $("#main_services").append('<option value="0"> اختر نوع الخدمه المطلوبه </option>' +
                                                '<option value="111">  عمليات  </option>' +
                                                '<option value="112"> عيادات خارجية  </option>' +
                                                '<option value="11104"> طوارئ  </option>');
                                        }
                                        //}
                                        //else {
                                        //    alert('لا يمكن تقديم الخدمه لهذا الموظف لانتهاء تعاقد الشركة');
                                        //}


                                    }
                                    else {
                                        alert(' لا يمكن تقديم الخدمه لهذا الموظف لانتهاء الكارت');
                                    }
                                }
                                else {
                                    alert(' Invalid Card Number Termintate Flag ');
                                }
                            }
                            else {
                                alert(" لا يمكن تقديم الخدمة للموظف الا فى حالات الطوارئ");

                                $("#other-data").hide();
                                $("#divNotes").hide();
                                $("#total_id").hide();
                                $("#DoctortName").show();
                                $("#SpecialityDiv").show();
                                $("#serviceselect").hide();
                                $("#doctorNameSelect").select2({
                                    placeholder: "Select a Doctor",
                                    ajax: {
                                        url: '/Hospital/GetDoctors/',
                                        dataType: 'json',
                                        data: function (params) {
                                            var query = {
                                                sEcho: params.page || 1,
                                                //iColumns=10,
                                                //iDisplayLength=10,
                                                sSearch: params.term,
                                                Provider: $("#providerHospialCode").val(),

                                            }

                                            // Query parameters will be ?search=[term]&page=[page]
                                            return query;
                                        },
                                        processResults: function (data, params) {
                                            params.page = params.page || 1;
                                            var result = [];
                                            for (var i = 0; i < data.aaData.length; i++) {
                                                var current = {};
                                                current.id = data.aaData[i].Doctorid;
                                                current.text = data.aaData[i].doctorName;
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

                                $(".celling-pert").val("100");
                                $("#Limit").val(0);
                                if (r.Data[0].TERMINATE_FLAG == 'N' || r.Data[0].TERMINATE_FLAG == null) {
                                    if (r.Data[0].INS_END_DATE != null) {
                                        //end date
                                        var MyDate_String_Value1 = r.Data[0].INS_END_DATE;
                                        var MyDate_String_Value2 = r.Data[0].INS_START_DATE;
                                        var myTerminateDate_String_Value = r.Data[0].TERMINATE_DATE;
                                        var value1 = new Date
                                            (
                                                parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var value2 = new Date
                                            (
                                                parseInt(MyDate_String_Value2.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var TerminateDatevalue = new Date
                                            (
                                                parseInt(myTerminateDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dateTerminate = TerminateDatevalue.getDate() + "/" + (TerminateDatevalue.getMonth() + 1) + "/" + TerminateDatevalue.getFullYear();
                                        var datTer = dateTerminate.split("/");
                                        var DTer = new Date(datTer[2], parseInt(datTer[1]) - 1, datTer[0]);


                                        var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                        var dat2 = value2.getDate() + "/" + (value2.getMonth() + 1) + "/" + value2.getFullYear();
                                        var d2 = dat1.split("/");
                                        var d = new Date();
                                        var Mysysdate = d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear();
                                        var d1 = Mysysdate.split("/");
                                        var from = new Date(d1[2], parseInt(d1[1]) - 1, d1[0]);  // -1 because months are from 0 to 11
                                        var to = new Date(d2[2], parseInt(d2[1]) - 1, d2[0]);

                                        //if (from < to) {

                                            var com_Name = r.Data[0].C_ENAME;
                                            if (r.Data[0].INS_START_DATE != null) {
                                                var MyDate_String_Value = r.Data[0].INS_START_DATE;
                                                var value = new Date
                                                    (
                                                        parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                                    );
                                                var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                                            }
                                            else {
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
                                            var StartDate = dat2;
                                            $('#txtSearchCard').val(CardId);
                                            $('#compEmp_EMP_ANAME').val(ArName);
                                            $('#compEmp_INS_END_DATE').val(EndDate);
                                            $("#contractComp_C_ANAME").val(com_Name);
                                            $('#compEmp_INS_START_DATE').val(StartDate);


                                            $('#Con_Num').val(r.Data[0].CONTRACT_NO);
                                            $('#Class_Code').val(r.Data[0].CLASS_CODE);
                                            $('#Company_ID').val(r.Data[0].COMP_ID);
                                            if (!$('#main_services').val()) {
                                                // do something
                                                $("#main_services").append(
                                                    '<option value="11104"> طوارئ  </option>');
                                                $("#main_services").prop("disabled", true);
                                                $("#emergancyTxt").show();
                                            }



                                        //}
                                        //else {
                                        //    alert(' لا يمكن تقديم الخدمه لهذا الموظف لانتهاء تعاقد الشركة');
                                        //}
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
                                        var MyDate_String_Value2 = r.Data[0].INS_START_DATE;
                                        var value1 = new Date
                                            (
                                                parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var value2 = new Date
                                            (
                                                parseInt(MyDate_String_Value2.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var TerminateDatevalue = new Date
                                            (
                                                parseInt(myTerminateDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                            );
                                        var dateTerminate = TerminateDatevalue.getDate() + "/" + (TerminateDatevalue.getMonth() + 1) + "/" + TerminateDatevalue.getFullYear();
                                        var datTer = dateTerminate.split("/");
                                        var DTer = new Date(datTer[2], parseInt(datTer[1]) - 1, datTer[0]);
                                        var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                                        var dat2 = value2.getDate() + "/" + (value2.getMonth() + 1) + "/" + value2.getFullYear();
                                        var d2 = dat1.split("/");
                                        var d = new Date();
                                        var Mysysdate = d.getDate() + "/" + (d.getMonth() + 1) + "/" + d.getFullYear();
                                        var d1 = Mysysdate.split("/");
                                        var from = new Date(d1[2], parseInt(d1[1]) - 1, d1[0]);  // -1 because months are from 0 to 11
                                        var to = new Date(d2[2], parseInt(d2[1]) - 1, d2[0]);
                                        //if (from < to && from < DTer) {

                                            var com_Name = r.Data[0].C_ENAME;
                                            if (r.Data[0].INS_START_DATE != null) {
                                                var MyDate_String_Value = r.Data[0].INS_START_DATE;
                                                var value = new Date
                                                    (
                                                        parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                                    );
                                                var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                                            }
                                            else {
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
                                            var StartDate = dat2;

                                            $('#txtSearchCard').val(CardId);
                                            $('#compEmp_EMP_ANAME').val(ArName);
                                            $('#compEmp_INS_END_DATE').val(EndDate);
                                            $("#contractComp_C_ANAME").val(com_Name);
                                            $('#compEmp_INS_START_DATE').val(StartDate);
                                            $('#Con_Num').val(r.Data[0].CONTRACT_NO);
                                            $('#Class_Code').val(r.Data[0].CLASS_CODE);
                                            $('#Company_ID').val(r.Data[0].COMP_ID);

                                            if (!$('#main_services').val()) {
                                                // do something
                                                $("#main_services").append(
                                                    '<option value="11104"> طوارئ  </option>');
                                                $("#main_services").prop("disabled", true);
                                                $("#emergancyTxt").show();
                                            }
                                        //}
                                        //else {
                                        //    alert('لا يمكن تقديم الخدمه لهذا الموظف لانتهاء تعاقد الشركة');
                                        //}


                                    }
                                    else {
                                        alert(' لا يمكن تقديم الخدمه لهذا الموظف لانتهاء الكارت');
                                    }
                                }
                                else {
                                    bootbox.alert(' Invalid Card Number Termintate Flag ');
                                }
                            }

                        }
                        else {

                            bootbox.alert('Please Check Card Validation ');
                        }
                    }
                    else {
                        bootbox.alert("Card Not Found Enter Correct Card ID ");
                    }

                },
                error: function () {
                    bootbox.alert("Invalid Card ID ");
                }
            });
        }

        else {
            bootbox.alert("Please Insert Card Id");
        }


    });

    $("#main_services").change(function () {
        exceptionHospital = null;
        exceptionLabRay = null;
        $("#ddlDiagnoises").empty();
        $("#txtTotal").val("");
        $(".celling-pert").val("");
        $("#txtTotalCopayment").val("");
        $("#Limit").val("");

        $("#ClaimNumber").val("");
        $("#CompanyPayment").val("");
        $("#PatientCoPayment").val("");
        $("#OverInsurance").val("");
        $("#Cash").val("");
        $("#IsCash").val(0);
        $("#Notetext").val("");

        $("#degreeSelect").val("0");
        $("#doctorNameSelect").val("0");
        $("#DoctortDegree").hide();
        $("#DoctortName").hide();

        //$('#AddDiagnoise').attr('disabled', true);
        //$('#RemoveDiagnoise').attr('disabled', true);
        $("#Physical").html("");
        $("#div_of_serv").hide();
        $("#divNotes").hide();
        $("#serviceselect").hide();
        $("#Services").empty();

        $("#other-data").show();
        $("#total_id").show();
        $("#SpecialityDiv").hide();

        $("#SaveAll").html('<input id="submit" type="button"  onclick="Submit_Save()" value="Save" class="btn btn-success" style="padding:10px 20px" />');

        if ($("#main_services").val() != 0) {

            if ($("#main_services").val() == 111) {
                $('#txtTotal').attr('disabled', 'disabled');
                $('#submit').attr('disabled', true);
                var data = {
                    CardId: $('#txtSearchCard').val(),
                    ContractNum: $('#Con_Num').val(),
                    ClassCode: $('#Class_Code').val(),
                    ProvideCode: $("#providerHospialCode").val()
                };
                //$.ajax({
                //    type: 'POST',
                //    url: '/User/GetSer_Services/',
                //    dataType: 'json',
                //    data: {
                //        Services_id: $("#main_services").val(), Contract_Number: $('#Con_Num').val()
                //        , Class_Code: $('#Class_Code').val()
                //        , C_Comp_ID: $('#Company_ID').val()
                //        , CardId: $('#txtSearchCard').val()
                //    },
                //    success: function (r) {
                //        debugger;
                //        $("#insurance_LIVEL").val(100 - r.celing);
                //        $("#max_amount0").val(r.max_am);
                //    },
                //    error: function () {

                //        bootbox.alert('لا يمكن تقديم الخدمة لهذا الموظف ');
                //    }

                //});
                $.ajax({
                    type: "POST",
                    url: '/Hospital/Get_Approvels',
                    data: (data),
                    success: function (apprs) {
                        if (apprs.length > 0) {
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
                                    "<td>" + apprs[i].CreatedBy + "</td>" +
                                    "<td>" + apprs[i].End_Date + "</td>" +
                                    "<td>" + apprs[i].Expaire_Date + "</td>" +
                                    "<td >" + "<Button  class='btn btn-Primary ' onclick='Print(\"" + apprs[i].Code + "\");'>Print</Button>" + "</td>" +
                                    "</tr>";
                                setData2.append(data);
                            }


                            //$('#insurance_LIVEL').val("");
                            //  $('#Diagnoise').val("");
                            //  $("#Diagnoise").empty();
                            //  $("#txtTotal").val("");
                            //  $(".celling-pert").val("");
                            //  $("#txtTotalCopayment").val("");
                            //  $('#AddDiagnoise').attr('disabled', true);
                            //  $('#RemoveDiagnoise').attr('disabled', true);
                            //  $("#Physical").html("");
                            //  $("#SaveAll").html('<input id="submit" type="button"  onclick="Submit_Save()" value="Save" class="btn btn-success" style="padding:10px 20px" />');


                            $("#Approvals").DataTable();
                            $("#ApprovalModal").modal();
                            $("#main_services").val(0);
                            $("#insurance_LIVEL").val("");
                            //var xs = $("#main_services").val();
                            //var sas = $('#Con_Num').val();
                            //var xdfss = $("#Class_Code").val();
                            //var sgfdgdas = $('#Company_ID').val();
                            //var xewws = $("#txtSearchCard").val();


                        }

                        else {
                            $("#main_services").val(0);
                            $("#insurance_LIVEL").val("");
                            bootbox.alert("لا يوجد موافقة موجهة لهذا الكارت للاستفسار او الاستعلام برجاء الاتصال على رقم 01227905551");
                            $('#submit').attr('disabled', false);
                        }


                    },
                    error: function (err) {
                        $("#main_services").val(0);
                        $("#insurance_LIVEL").val("");
                        bootbox.alert("You'r session has been expired please  log in again and try it ...!");
                    }
                });
                $("#emergancyTxt").hide();
            }

            else if ($("#main_services").val() == 112) {
                $('#txtTotal').attr('disabled', false);
                $("#div_of_serv").show();
                //$("#serviceselect").show();
                $("#divNotes").hide();

                $("#other-data").show();
                $("#total_id").show();
                $("#DoctortName").hide();
                $("#SpecialityDiv").hide();
                $("#serviceselect").hide();

                $('#Services').append('<option value="0"> اختر نوع الخدمه المطلوبه </option>'
                    + '<option value="11201"> أشعة </option>'
                    + '<option value="11206">  تحاليل </option>'
                    + '<option value="11203"> خدمة العيادة الخارجية </option>'
                    + '<option value="11205"> كشف </option>'
                    + '<option value="11414"> كشف أسنان </option>'
                    + '<option value="11301"> رمد </option>');
                $("#emergancyTxt").hide();
            }

            else if ($("#main_services").val() == 11104) {
                //Co-Payment
                $("#divNotes").show();

                $("#other-data").hide();
                $("#divNotes").hide();
                $("#total_id").hide();
                $("#DoctortName").show();
                $("#SpecialityDiv").show();
                $("#serviceselect").hide();
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: '/Hospital/CellingAmount',
                    data: {
                        //Services_id: 111,
                        ServiceCode: 11104,
                        id: $('#txtSearchCard').val()
                    },
                    success: function (r) {
                        //if (r.Validation == false) {
                        //    toastr.info(r.Message);
                        //    $(".celling-pert").val(100);
                        //    $("#Limit").val(0);
                        //    $("#max_amount0").val(0);
                        //    $("#IsCash").val(1);
                        //}
                        //else {

                        var excepitonResult = null;
                        exceptionHospital = null;
                        exceptionLabRay = null;
                        $.ajax({
                            type: 'POST',
                            url: '/Hospital/GetExceptions/',
                            dataType: 'json',
                            data: {
                                cardID: $('#txtSearchCard').val(),
                                exceptionReasonId: 7,
                            },

                            success: function (r2) {
                                excepitonResult = r2.msg;
                                exceptionHospital = r2.ExceptionResult;
                                if (excepitonResult == "ok") {
                                    $(".celling-pert").val(0);
                                    $("#Limit").val(r.Limit);
                                    $("#max_amount0").val(r.Limit);

                                }
                                else {
                                    if (r.Validation != false) {
                                        $(".celling-pert").val(100 - r.CeilingPert);
                                        $("#Limit").val(r.Limit);
                                        $("#max_amount0").val(r.Limit);
                                    }
                                    else {
                                        alert(r.Message);
                                        $(".celling-pert").val(100 - r.CeilingPert);
                                        $("#Limit").val(r.Limit);
                                        $("#max_amount0").val(r.Limit);
                                    }
                                }
                                $.ajax({
                                    type: "POST",
                                    dataType: "json",
                                    url: '/Hospital/ChickCrona',
                                    data: {
                                        CompNumber: $('#Company_ID').val(),
                                        ContractNumber: $('#Con_Num').val(),
                                        ClassCode: $('#Class_Code').val()
                                    },
                                    success: function (r) {
                                        if (r.Success == "Yes") {
                                            alert("Emergancy ' COVID_ 19 ' Coverd For This Card ");
                                        }
                                        else {
                                            alert("Emergancy ' COVID_ 19 ' Not Coverd For This Card  And Will Pay Any Thing Related with ' COVID_ 19 '");

                                        }
                                    },
                                    error: function (err) {
                                        alert("Company Annual Amount");
                                        location.reload();
                                    }
                                });

                            },
                            error: function (Data) {

                                alert("Card Not Found");
                            }

                        });
                        //    }
                    },
                    error: function (err) {
                        alert("Company Annual Amount");
                        location.reload();
                    }
                });


            }
        }
        else
            alert("Please Select Service !");

    });


    $("#Services").change(function () {
        exceptionHospital = null;
        exceptionLabRay = null;
        $("#Diagnoise").val("");
        $("#txtTotal").val("");
        $(".celling-pert").val("");
        $("#txtTotalCopayment").val("");
        $("#Limit").val("");

        $("#ClaimNumber").val("");
        $("#CompanyPayment").val("");
        $("#PatientCoPayment").val("");
        $("#OverInsurance").val("");
        $("#Cash").val("");
        $("#IsCash").val(0);

        $("#degreeSelect").val("0");
        $("#doctorNameSelect").val("0");
        $("#DoctortDegree").hide();
        $("#DoctortName").hide();

        $("#divNotes").hide();
        $("#SpecialityDiv").hide();
        $("#serviceselect").hide();
        $("#ddlDiagnoises").empty();

        //$('#AddDiagnoise').attr('disabled', true);
        //$('#RemoveDiagnoise').attr('disabled', true);
        $("#Physical").html("");
        $("#SaveAll").html('<input id="submit" type="button"  onclick="Submit_Save()" value="Save" class="btn btn-success" style="padding:10px 20px" />');

        if ($("#Services").val() != 0) {

            //Co-Payment
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/Hospital/CellingAmount',
                data: {
                    //Services_id: $("#main_services").val(),
                    ServiceCode: $("#Services").val(),
                    id: $('#txtSearchCard').val()
                },
                success: function (r) {

                    var excepitonResult = null;
                    exceptionHospital = null;
                    exceptionLabRay = null;
                    $.ajax({
                        type: 'POST',
                        url: '/Hospital/GetExceptions/',
                        dataType: 'json',
                        data: {
                            cardID: $('#txtSearchCard').val(),
                            exceptionReasonId: 7,
                        },

                        success: function (r2) {
                            excepitonResult = r2.msg;
                            if (excepitonResult == "ok") {
                                exceptionHospital = r2.ExceptionResult
                                $(".celling-pert").val(0);
                                $("#Limit").val(r.Limit);
                                $("#max_amount0").val(r.Limit);

                            }
                            else {
                                if (r.Validation != false) {
                                    $(".celling-pert").val(100 - r.CeilingPert);
                                    $("#Limit").val(r.Limit);
                                    $("#max_amount0").val(r.Limit);
                                }
                                else {
                                    alert(r.Message);
                                    $(".celling-pert").val(100 - r.CeilingPert);
                                    $("#Limit").val(r.Limit);
                                    $("#max_amount0").val(r.Limit);
                                    $("#insurance_LIVEL").val(100);
                                    $("#IsCash").val(1);
                                }
                            }



                        },
                        error: function (Data) {

                            //alert("Card Not Found");
                        }

                    });
                    ////////

                    $.ajax({
                        type: 'POST',
                        url: '/Hospital/Get_Specialist/',
                        dataType: 'json',
                        data: {
                            Services_id: $("#Services").val(), cardID: $('#txtSearchCard').val(),
                            ContractNum: $("#Con_Num").val(), ClassCode: $("#Class_Code").val(),
                        },

                        success: function (r) {
                            var myType = r.msg;
                            var ExceptionId = null;
                            if (myType !== "ok") {
                                if ($("#Services").val() == "11201" || $("#Services").val() == "11206") {
                                    ExceptionId = 9;
                                }
                                else if ($("#Services").val() == "11205") {
                                    ExceptionId = 10;
                                }
                                var excepitonResult = null;
                                exceptionHospital = null;
                                exceptionLabRay = null;
                                if (ExceptionId != null) {
                                    $.ajax({
                                        type: 'POST',
                                        url: '/Hospital/GetExceptions/',
                                        dataType: 'json',
                                        data: {
                                            cardID: $('#txtSearchCard').val(),
                                            exceptionReasonId: ExceptionId,
                                        },

                                        success: function (r2) {
                                            excepitonResult = r2.msg;
                                            if (excepitonResult != "ok") {
                                                exceptionLabRay = r2.ExceptionResult
                                                alert(r.msg);
                                                $("#insurance_LIVEL").val(100);
                                                $("#Limit").val(0);
                                                $("#IsCash").val(1);
                                                $("#max_amount0").val(0);
                                                $(".celling-pert").val(100);

                                            }

                                        },
                                        error: function (Data) {

                                            //alert("Card Not Found");
                                        }

                                    });
                                }
                                else {

                                }
                            }
                        },
                        error: function (Data) {

                        }

                    });

                    ///////
                    // }

                    if ($("#Services").val() == 11205 /*|| $("#Services").val() == 11414*/ || $("#Services").val() == 11203) {

                        $("#other-data").hide();
                        $("#divNotes").hide();
                        $("#total_id").hide();
                        $("#DoctortName").show();
                        $("#SpecialityDiv").show();
                        $("#serviceselect").hide();
                        $("#doctorNameSelect").select2({
                            placeholder: "Select a Doctor",
                            ajax: {
                                url: '/Hospital/GetDoctors/',
                                dataType: 'json',
                                data: function (params) {
                                    var query = {
                                        sEcho: params.page || 1,
                                        //iColumns=10,
                                        //iDisplayLength=10,
                                        sSearch: params.term,
                                        Provider: $("#providerHospialCode").val(),

                                    }

                                    // Query parameters will be ?search=[term]&page=[page]
                                    return query;
                                },
                                processResults: function (data, params) {
                                    params.page = params.page || 1;
                                    var result = [];
                                    for (var i = 0; i < data.aaData.length; i++) {
                                        var current = {};
                                        current.id = data.aaData[i].Doctorid;
                                        current.text = data.aaData[i].doctorName;
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



                        //$.ajax({
                        //    type: "GET",
                        //    dataType: "json",
                        //    url: '/Hospital/GetDoctors',
                        //    //data: {
                        //    //    id: $('#txtSearchCard').val(),
                        //    //    ServiceCode: $("#Services").val()
                        //    //},
                        //    success: function (result) {

                        //    },
                        //    error: function (err) {
                        //        bootbox.alert('لا يوجد دكاترة مسجلين لهذة المستشفي  ');
                        //    }
                        //});
                    }
                    if ($("#Services").val() == 11414 || $("#Services").val() == 11301) {
                        $("#other-data").hide();
                        $("#divNotes").hide();
                        $("#total_id").hide();
                        $("#serviceselect").hide();
                        $("#SpecialityDiv").hide();
                    }

                    if ($("#Services").val() == 11201 || $("#Services").val() == 11206) {
                        $("#other-data").show();
                        $("#divNotes").hide();
                        $("#total_id").show();
                        $("#serviceselect").show();
                        $("#SpecialityDiv").hide();
                    }

                },
                error: function (err) {
                    bootbox.alert('لا يمكن تقديم الخدمة لهذا الموظف ');
                }
            });
        }

        else
            alert("Please Select Service !");

    });




    $("#ddlDiagnoises").select2({
        placeholder: "Select a Service",
        //overflow: scroll,
        ajax: {
            url: '/Hospital/Get_Specialist2/',
            dataType: 'json',
            data: function (params) {
                var query = {
                    sEcho: params.page || 1,
                    //iColumns=10,
                    //iDisplayLength=10,
                    sSearch: params.term,
                    Provider: $("#providerHospialCode").val(),
                    Services_id: $("#Services").val(), cardID: $('#txtSearchCard').val(),
                    ContractNum: $("#Con_Num").val(), ClassCode: $("#Class_Code").val(),

                }

                // Query parameters will be ?search=[term]&page=[page]
                return query;
            },
            processResults: function (data, params) {
                //params.scroll = true;
                params.page = params.page || 1;
                var result = [];
                for (var i = 0; i < data.aaData.length; i++) {
                    var current = {};
                    current.id = data.aaData[i].Price;
                    current.text = data.aaData[i].ServiceName;
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
    $('#ddlDiagnoises').on('select2:selecting', function (event) {
        var price = parseFloat((event.params.args.data.id).split("|")[0]);
        var total = $("#txtTotal").val() == "" ? 0 : parseFloat($("#txtTotal").val());
        var result = total + price;
        $("#txtTotal").val(result);
        PersonPayments();
    });

    $('#ddlDiagnoises').on("select2:unselecting", function (event) {
        var price = parseFloat((event.params.args.data.id).split("|")[0]);
        var total = $("#txtTotal").val() == "" ? 0 : parseFloat($("#txtTotal").val());
        var result = total - price;
        if (result < 0) {
            $("#txtTotal").val(0);
        }
        else {
            $("#txtTotal").val(result);
        }
        PersonPayments();
    });

});
//Save Data
function Submit_Save() {
    if ($('#txtSearchCard').val() != "") {

        ChickSaveData();
        //$.ajax({
        //    type: "POST",
        //    dataType: "json",
        //    url: '/Hospital/ChickNationalId',
        //    data: {
        //        NationalId: $('#NationalId').val(),
        //        cardId: $('#txtSearchCard').val()
        //    },
        //    success: function (r) {
        //        debugger;
        //        if (r.Success != "Yes") {

        //            bootbox.confirm({
        //                title: "حالة الطلب ",
        //                message: " برجاء التاكد من صحة ادخال الرقم القومي وادخال الرقم القومي من الدرجه الاولي لاقل من 16 سنه تجنبا لخصم ال  Claim  بالكامل  ",
        //                buttons: {
        //                    cancel: {
        //                        label: '<i class="fa fa-reply"></i> الغاء '
        //                    },
        //                    confirm: {
        //                        label: '<i class="fa fa-check"></i> حفظ '
        //                    }
        //                },
        //                callback: function (result) {
        //                    if (result == false) {

        //                    }
        //                    else {
        //                        ChickSaveData();

        //                    }
        //                }
        //            });
        //        }
        //        else {


        //            ChickSaveData();


        //        }
        //    },
        //    error: function (err) {
        //        alert("Company Annual Amount");
        //        location.reload();
        //    }
        //});

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
    else if (isCash == 1) {
        $("#CompanyPayment").val(0);
        $("#PatientCoPayment").val(Total);
        $("#OverInsurance").val(0);
        //$("#OverInsurance").val(Total);
        $("#Cash").val(Total);
        $("#txtTotalCopayment").val(Total);
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
        //$("#AddDiagnoise").attr("disabled", true);
        //$("#RemoveDiagnoise").attr("disabled", true);

    }
}


function ClearHospitalData() {
    $("#txtSearchCard").val("");
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

    $("#degreeSelect").val("0");
    $("#doctorNameSelect").val("0");
    $("#DoctortDegree").hide();
    $("#DoctortName").hide();
    $("#Notetext").val("");
    $("#divNotes").hide();

    //$('#AddDiagnoise').attr('disabled', true);
    //$('#RemoveDiagnoise').attr('disabled', true);
    $("#Physical").html("");
    $("#main_services").html("");
    $("#div_of_serv").hide();

    $("#other-data").show();
    $("#total_id").show();
    $("#SpecialityDiv").hide();
    $("#serviceselect").hide();
}

function ChickSaveData() {

    var mainSer = $("#main_services option:selected").index();
    var mainSerEmer = $("#main_services option:selected").val();
    var subser = $("#Services option:selected").index();
    var serviceID = 0;
    var compPercent = parseFloat(100 - $(".celling-pert").val());
    var nId = $("#NationalId").val();
    //emergency only 
    if (mainSerEmer == "11104") {
        serviceID = $("#main_services").val();
        /*if ($('#emergancyTxt').val() != "") {*/
        var data = {
            C_Com_ID: $('#Company_ID').val(),
            Provider_Code: $("#providerHospialCode").val(),
            Card_ID: $('#txtSearchCard').val(),
            Services_ID: serviceID,
            ServType: mainSerEmer,
            Services: AllDiagnos,
            Contract_Number: $('#Con_Num').val(),
            Class_Code: $('#Class_Code').val(),
            Person_Payment: 0/* $("#PatientCoPayment").val()*/,
            Comp_Payment: 0 /*$("#CompanyPayment").val()*/,
            Phone: "" /*$("#PhoneNumber").val()*/,
            NATIONAL_ID: "" /*$("#NationalId").val()*/,
            CLAIM_NO: "" /*$("#ClaimNumber").val()*/,
            COMP_PERC: 0/*compPercent*/,
            OverInsurance: 0 /*$("#OverInsurance").val()*/,
            TotalValue: 0/*$("#txtTotal").val()*/,
            Total_Cash: 0/*$("#txtTotalCopayment").val()*/,
            Cash: 0 /*$("#Cash").val()*/,
            Notes: ""/* $("#Notetext").val()*/,
            HospitalException: exceptionHospital,
            ExceptionLabRayDoctor: exceptionLabRay,
            SpecalistID: $("#SpecialitySelect").val(),
            DoctorName: $("#doctorNameSelect").val()
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
                            ClearHospitalData();
                            exceptionHospital = null;
                            exceptionLabRay = null;
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
        //}
        //else {
        //    alert("Please Enter Emergance Data ...!");
        //    $("#emergancyTxt").focus();

        //}
    }

    // out patient 
    else if (mainSerEmer == "112") {
        // for labs and rays 
        if ($("#Services").val() == "11201" || $("#Services").val() == "11206") {

            if ($("#txtTotal").val() != "") {
                var data = {
                    C_Com_ID: $('#Company_ID').val(),
                    Provider_Code: $("#providerHospialCode").val(),
                    Card_ID: $('#txtSearchCard').val(),
                    Services_ID: $("#Services").val(),
                    ServType: mainSerEmer,
                    Services: AllDiagnos,
                    Contract_Number: $('#Con_Num').val(),
                    Class_Code: $('#Class_Code').val(),
                    Person_Payment: $("#PatientCoPayment").val(),
                    Comp_Payment: $("#CompanyPayment").val(),
                    Phone: $("#PhoneNumber").val(),
                    NATIONAL_ID: $("#NationalId").val(),
                    CLAIM_NO: $("#ClaimNumber").val(),
                    COMP_PERC: compPercent,
                    OverInsurance: $("#OverInsurance").val(),
                    TotalValue: $("#txtTotal").val(),
                    Total_Cash: $("#txtTotalCopayment").val(),
                    Cash: $("#Cash").val(),
                    Notes: ""/* $("#Notetext").val()*/,
                    HospitalException: exceptionHospital,
                    ExceptionLabRayDoctor: exceptionLabRay,
                    SpecalistID: null,
                    DoctorName: null
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
                                    ClearHospitalData();
                                    exceptionHospital = null;
                                    exceptionLabRay = null;
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
                alert("Please Enter Total Invoice ...!");
                $("#txtTotal").focus();
            }
        }
        // for doctor check 
        else {
            var data = {
                C_Com_ID: $('#Company_ID').val(),
                Provider_Code: $("#providerHospialCode").val(),
                Card_ID: $('#txtSearchCard').val(),
                Services_ID: serviceID,
                ServType: mainSerEmer,
                Services: AllDiagnos,
                Contract_Number: $('#Con_Num').val(),
                Class_Code: $('#Class_Code').val(),
                Person_Payment: 0/* $("#PatientCoPayment").val()*/,
                Comp_Payment: 0 /*$("#CompanyPayment").val()*/,
                Phone: "" /*$("#PhoneNumber").val()*/,
                NATIONAL_ID: "" /*$("#NationalId").val()*/,
                CLAIM_NO: "" /*$("#ClaimNumber").val()*/,
                COMP_PERC: 0/*compPercent*/,
                OverInsurance: 0 /*$("#OverInsurance").val()*/,
                TotalValue: 0/*$("#txtTotal").val()*/,
                Total_Cash: 0/*$("#txtTotalCopayment").val()*/,
                Cash: 0 /*$("#Cash").val()*/,
                Notes: ""/* $("#Notetext").val()*/,
                HospitalException: exceptionHospital,
                ExceptionLabRayDoctor: exceptionLabRay,
                SpecalistID: $("#SpecialitySelect").val(),
                DoctorName: $("#doctorNameSelect").val()
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
                                ClearHospitalData();
                                exceptionHospital = null;
                                exceptionLabRay = null;
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
    }
    else if ($("#txtTotal").val() != "") {
        if ($("#PhoneNumber").val() != "") {
            if (true/*$("#NationalId").val() != "" && nId.length==14*/) {
                if ($("#ClaimNumber").val() != "") {
                    if (mainSerEmer != "11104") {
                        if ($("#main_services option:selected").index() > 0) {
                            if (mainSer == 2) {
                                if (subser > 0) {
                                    //if (subser != 4) {
                                    if ($("#ddlDiagnoises").val() != "") {
                                        debugger;
                                        var oArea = document.getElementById('ddlDiagnoises');
                                        var aNewlines = oArea.innerText.split("\n");
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
                                            Notes: $("#Notetext").val(),
                                            HospitalException: exceptionHospital,
                                            ExceptionLabRayDoctor: exceptionLabRay,
                                            SpecalistID: 0
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
                                                            $("#Notetext").val("");

                                                            $("#degreeSelect").val("0");
                                                            $("#doctorNameSelect").val("0");
                                                            $("#DoctortDegree").hide();
                                                            $("#DoctortName").hide();

                                                            $("#Physical").html("");
                                                            $("#main_services").html("");
                                                            $("#div_of_serv").hide();
                                                            $("#divNotes").hide();
                                                            $("#serviceselect").hide();
                                                            exceptionHospital = null;
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
                                                        $("#Notetext").val("");

                                                        $("#Physical").html("");
                                                        $("#main_services").html("");
                                                        $("#div_of_serv").hide();
                                                        $("#divNotes").hide();
                                                        $("#serviceselect").hide();
                                                        exceptionHospital = null;
                                                        exceptionLabRay = null;
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

                    // Emergancy had been here
                }
                else {
                    alert("Please Enter Claim Number ...!");
                    $("#txtTotal").focus();
                }
            }
            else {
                alert("Please Enter National Id Correct ...!");
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