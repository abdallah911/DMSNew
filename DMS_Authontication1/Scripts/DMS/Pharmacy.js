var input = document.getElementById("txtSearchCard");
var CardId;
var dat1;
var StartDate;
var birthdate;
var Genderr;
var Age;
var AnuualLimit;
var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
var firstDate;
var NationalId;
const secondDate = new Date();
var diffDays;

input.addEventListener("keyup", function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        $('#Search').click();
    }
});
$('body').on('keyup', '.Dose, .Duration, .TotalDuration', function (e) {
    var self = $(this), form = self.parents('div:eq(0)'), focusable, next;
    if (e.keyCode == 13) {
        focusable = form.find('.Dose, .Duration, .TotalDuration').filter(':visible');
        next = focusable.eq(focusable.index(this) + 1);
        if (next.length) {
            next.focus();
        } else {
            form.submit();
        }
        return false;
    }
});

$(function () {
    $("#ddlUsers").select2();
    $("#PrescriptionDate").datepicker({
        maxDate: '0',
        minDate: '-6D',
        dateFormat: 'dd-mm-yy',
    });
    $("#Help").click(function () {
        introJs().start();
    });
    var V = 0;
    function getDignoseNum(variable) {
        if (variable > 0) {
            return V = 1;
        }
        return V = 0;
    }
    $('#Search').click(function () {
        $("#Approval").removeClass('active');
        $('#Pending').attr('disabled', true);
        $("#ddlDiagnoises").val(null).change();

        $('#PrescriptionDate').val('');
        $('#ClaimNumber').val('');
        $('#PhoneNumber').val('');
        $('#contractComp_C_ANAME').val('');
        $('#compEmp_EMP_ANAME').val('');
        $('#compEmp_INS_START_DATE').val('');
        $('#compEmp_INS_END_DATE').val('');
        $('#compEmp_BIRTH_DATE').val('');
        $('#insurance_LIVEL').val('');
        $('#ddEmp_CEILING_PERT').val('');
        var companid = $('#txtSearchCard').val().split('-')[0];
        if (companid == "500142" || companid == "500103" || companid == "500125" || companid == "10560") {
            alert("هذا العميل لايحتاج موافقة علي الروشتات الخارجية علي ان يتم ادخال كافة البيانات والادوية علي السيستم");

        }
        if ($('#txtSearchCard').val() != "") {
            $('#txtSearchCard').val($('#txtSearchCard').val().trim())
            $("#wait").css("display", "block");

            $.ajax({
                url: '/Pharmacy/AddCardPharmacy',
                data: { id: $('#txtSearchCard').val() },
                dataType: 'Json',
                success: function (r) {
                    $('#txtSearchCard').attr('disabled', true);
                    $('#Cards').dataTable().fnDestroy();
                    var setData = $("#Cards Tbody");
                    setData.empty();
                    for (var i = 0; i < r.length; i++) {
                        if (r[i].BIRTH_DATE != null) {
                            var MyDate_String_Value = r[i].BIRTH_DATE;
                            var value = new Date
                                (
                                    parseFloat(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                );
                            birthdate = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                        } else {
                            birthdate = null;
                        }
                        if (r[i].SPECIFIC_DATE != null) {
                            var MyDate_String_Value = r[i].SPECIFIC_DATE;
                            var value = new Date
                                (
                                    parseFloat(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                );
                            StartDate = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                        } else {
                            StartDate = null;
                        }
                        if (r[i].TERMINATE_DATE >= r[i].NoW) {
                            dat1 = null
                        }
                        if (r[i].INS_END_DATE != null && StartDate != null) {
                            //end date
                            var MyDate_String_Value1 = r[i].INS_END_DATE;
                            var value1 = new Date(parseFloat(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, '')));
                            dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                        }
                        else {
                            dat1 = null;
                        }
                        var data = "<tr >" +
                            "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-plus' onclick='Select(this);'></Button>" + "</td>" +
                            "<td>" + r[i].CARD_ID + "</td>" +
                            "<td>" + r[i].EMP_ANAME + "</td>" +
                            "<td>" + r[i].EMP_ENAME + "</td>" +
                            "<td>" + StartDate + "</td>" +
                            "<td>" + dat1 + "</td>" +
                            "<td>" + birthdate + "</td>" +
                            "</tr>"
                        var array_name = [];
                        array_name.push(data)
                        setData.append(array_name);

                    }

                    if (r.length == 1) {
                        $('#txtSearchCard').attr('disabled', true);
                        $('#Pending').attr('disabled', false);
                        $('#Approval').attr('disabled', false);
                        CardId = r[0].CARD_ID;
                        $('#txtSearchCard').val(CardId);
                        var ArName = r[0].EMP_ENAME;
                        firstDate = new Date(parseFloat(r[0].INS_END_DATE.replace(/(^.*\()|([+-].*$)/g, '')));
                        diffDays = Math.round(Math.abs((firstDate - secondDate) / oneDay));
                        var EndDate = dat1;
                        //var today = new Date();
                        //var dd = today.getDate();
                        //var mm = today.getMonth(); //January is 0!
                        //var yyyy = today.getFullYear();
                        //var CurrentDate = new Date(yyyy, mm, dd);
                        //if (EndDate != null) {
                        //    newDate = EndDate.split('/').reverse().join('.');
                        //} else {
                        //    newDate = "";
                        //}
                        //var date = new Date(newDate);
                        var companyId = [];
                        companyId = CardId.split('-', 1);
                        CompId = companyId[0];
                        // Chick company is closed or oopen

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


                                    //

                                    $.ajax({
                                        type: "POST",
                                        dataType: "json",
                                        url: '/Pharmacy/GetCompActivation',
                                        data: { id: CompId, CardId: CardId },
                                        success: function (returndata) {
                                            if (returndata.ok) {
                                                if (returndata.data == "Yes") {
                                                    debugger;
                                                    $("#wait").css("display", "none");
                                                    $('#txtSearchCard').val(CardId);
                                                    $('#compEmp_EMP_ANAME').val(ArName);
                                                    $('#compEmp_INS_START_DATE').val(StartDate);
                                                    $('#compEmp_INS_END_DATE').val(EndDate);
                                                    $('#compEmp_BIRTH_DATE').val(birthdate);
                                                    //AddNationalId();
                                                    $('#CardsModal').modal('hide');
                                                    //Get ceiling and Limit
                                                    GetLimit();

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
                                    bootbox.alert('برجاء الرجوع الى ادارة الشركة التابعة لسيادتكم ..');
                                    $("#wait").css("display", "none");
                                    $('#txtSearchCard').val(CardId);
                                    $('#CardsModal').modal('hide');
                                }
                            }
                        });



                    }
                    else if (r.length == 0) {
                        alert(' برجاء التأكد من الرقم الطبي وفي حاله استمرار المشكله ارسال صوره البطاقه علي رقم01205566050 ');
                        $('#CardsModal').modal('hide');
                        $("#wait").css("display", "none");
                    }
                    else {
                        $('#Cards').DataTable();
                        $('#CardsModal').modal({ backdrop: 'static', keyboard: false });
                        $("#wait").css("display", "none");
                    }
                },
                error: function () {
                    bootbox.alert("Too many data  Retrieve more specific characters solve the problem and check your internet connection");
                    $("#wait").css("display", "none");
                }
            });
        }
        else {
            bootbox.alert("Please Insert Card Id");
        }
    });
    $('#HasApproval').change(function () {
        if ($('#txtSearchCard').val() != '') {
            if ($(this).prop("checked")) {
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: "/Pharmacy/CheckType",
                    data: { CardId: $('#txtSearchCard').val() },
                    success: function (returndata) {
                        if (returndata == false) {
                            $.ajax({
                                type: "POST",
                                dataType: "json",
                                url: '/Pharmacy/GetLastApproval',
                                data: { CardId: $('#txtSearchCard').val(), Type: 3 },
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
                                                        //$("#Approval").removeClass('active');
                                                        //$('#Approval').attr('disabled', false);
                                                        //$('#HasApproval').prop("checked", false);

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
                                        var Limit = false;
                                        var Adult = false;
                                        var Date = false;
                                        var Diagnose = false;
                                        var Gender = false;
                                        var DisregardCeiling = false;
                                        var ExternalPrescription = false;
                                        var PrescriptionPerDay = false;
                                        for (var i = 0; i < returndata.length; i++) {
                                            Copayment = Copayment == true ? true : returndata[i].includes("Cancel Co-Payment");
                                            Limit = Limit == true ? true : returndata[i].includes('Disregard OverInsurance');
                                            Adult = Adult == true ? true : returndata[i].includes("Ignore Age");
                                            Date = Date == true ? true : returndata[i].includes("Expired Date");
                                            Diagnose = Diagnose == true ? true : returndata[i].includes("Diagnose");
                                            Gender = Gender == true ? true : returndata[i].includes("Ignore Gender");
                                            DisregardCeiling = DisregardCeiling == true ? true : returndata[i].includes("Disregard Ceiling");
                                            ExternalPrescription = ExternalPrescription == true ? true : returndata[i].includes("External Prescription");
                                            PrescriptionPerDay = PrescriptionPerDay == true ? true : returndata[i].includes("Unlimited Examination per day");

                                        }
                                        if (Adult == true) {
                                            // console.log("Age is checked");
                                            GetAge(1);
                                        }
                                        if (PrescriptionPerDay == true) {
                                            //Co-Payment
                                            $.ajax({
                                                type: "POST",
                                                dataType: "json",
                                                url: '/Pharmacy/CellingAmount',
                                                data: {
                                                    id: CardId,
                                                    ServiceCode: $('#ddlType').val()
                                                },
                                                success: function (r) {
                                                    if (r.Validation == false) {
                                                        // toastr.info(r.Message);
                                                        //ClearCardData();
                                                        alert(r.Message);
                                                        //history.go(0);
                                                        window.location.replace("/Pharmacy/Pharmacy");
                                                        //window.location.reload();

                                                    } else {
                                                        $('#ddEmp_CEILING_PERT').val(r.CeilingPert);
                                                        AnuualLimit = r.Limit;
                                                        $('#IsFamily').val(r.IsFamily);
                                                        $('#IsPool').val(r.IsPool);
                                                        if (r.CoInsurancelimit.INSURANCE_DAY >= 0) {
                                                            $("#insurance_LIVEL").val(r.CoInsurancelimit.INSURANCE_DAY);
                                                        } else {
                                                            alert(" تم استهلاك العدد المحدد للروشتات في الشهر وسوف يتحمل المريض المبلغ بالكامل نقدا");
                                                            $("#insurance_LIVEL").val("0.001");

                                                            $('#ddEmp_CEILING_PERT').val("0");
                                                        }
                                                    }
                                                },
                                                error: function (err) {
                                                    alert("Failed to retrieve Company Annual Limit. please check your internet connection");
                                                    location.reload();
                                                }
                                            });
                                            Calculation();
                                        }
                                        if (Limit == true) {
                                            $("#insurance_LIVEL").val(0);
                                            AnuualLimit = $('#AllLimit').val();
                                            Calculation();
                                        }
                                        if (Copayment == true) {
                                            $("#ddEmp_CEILING_PERT").val(100);
                                            Calculation();
                                        }
                                        if (Date == true) {
                                            $("#PrescriptionDate").datepicker("option", {
                                                minDate: null,
                                                maxDate: null
                                            });
                                        }
                                        if (Diagnose == true) {
                                            getDignoseNum(1);
                                        }
                                        if (Gender == true) {
                                            GetGender(1);
                                            $.ajax({
                                                type: "POST",
                                                dataType: "json",
                                                url: '/Pharmacy/AgeAndGender',
                                                data: { id: Genderr },
                                                success: function (returndata) { }
                                            });
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
                                    bootbox.dialog({
                                        title: 'Contact Us!',
                                        message: ' Call Us if you need Approval Number ' +
                                            "01271703178--01227905551",
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
                                            GetAge(1);
                                            $("#insurance_LIVEL").val(0);
                                            $("#ddEmp_CEILING_PERT").val(100);
                                            $('#ClaimNumber').val(' ');
                                            AnuualLimit = 30000;
                                            Calculation();
                                            $("#PrescriptionDate").datepicker("option", {
                                                minDate: null,
                                                maxDate: null
                                            });
                                            getDignoseNum(1);
                                            GetGender(1);

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
                ClearCardData();
                ClearMedicineData();
                //$("#Approval").removeClass('active');
                //$('#HasApproval').attr("checked", false);

            }
        } else {
            bootbox.alert("Please Insert Card Number");
            ClearCardData();
            ClearMedicineData();
        }
    });
    function GetGender(variable) {
        if (variable > 0) {
            return Genderr = 1;
        }
        return Genderr = 0;
    }
    function GetAge(variable) {
        if (variable > 0) {
            return Age = 1;
        }
        return Age = 0;
    }
    $("#ddlType").change(function () {
        if ($('#txtSearchCard').val() != '') {
            $("#Pharmacy >tbody").empty();
            $("#AddMedicine").empty();

            $("#Approval").removeClass('active');
            $('#Approval').attr('disabled', false);
            $('#HasApproval').prop("checked", false);
            //$("#Approval").removeClass('active');
            //$("#Approval").prop("checked", false);
            //$('#HasApproval').prop("checked", false);

            //if ($("#ddlType").val() == "11602") {
            //    window.location.replace("/Chronic/Chronic/" + CardId);
            //}
            //if ($("#ddlType").val() == "11604") {
            //    window.location.replace("/Doctor/Doctor/" + CardId);
            //}

            //Get CeilingPert and Limit
            GetLimit();

        }
        else {
            bootbox.alert("Please Insert Card Number");
        }
    });
    $('#Pending').click(function () {
        location.replace("/Pharmacy/Pending2?id=" + CardId);
    });
    function DatePickerModel(flag) {
        if (flag == 0) {
            $("#PrescriptionDate").datepicker({
                maxDate: '0',
                minDate: '-6D',
                dateFormat: 'dd-mm-yy',
            });
        }
        else {
            $("#PrescriptionDate").datepicker({
                dateFormat: 'dd-mm-yy'
            });
        }
    }
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
                bootbox.alert('Failed to retrieve Diagnoses , please check your internet connection');
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
                bootbox.alert('Failed to retrieve Diagnoses, please check your internet connection');
            }

        });
    });
    $("#ddlSpeciality").select2();

    $("#ddlType").select2();
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
                if ($('#PhoneNumber').val() != '' && $("#PhoneNumber").val().length == 11) {
                    if ($('#ddlDiagnoises').val().length != 0) {
                        //setTimeout(function () {}, 3000);
                        SelectMedicien(event);

                    }
                    else {
                        toastr.info("برجاء ادخال التشخيص الصحيح الموجود بالنموذج وفي حاله عدم مطابقه التشخيص الموجود بالنموذج للادويه او عدم وجود تشخيص يتم الرجوع الي الاداره الطبيه");
                        event.preventDefault();
                    }
                } else {
                    toastr.info("برجاء ادخال رقم الموبايل صحيح المتكون من 11 رقم");
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

    $('#submit').click(function () {
        var Id;
        var Mediciens = new Array();
        $("#Pharmacy TBODY TR").each(function () {

            var row = $(this);
            var Dose = $("TD", row).find(".Dose").val();
            var Duration = $("TD", row).find(".Duration").val();
            var Medicien = {};
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
            if ($('#PhoneNumber').val() != "") {
                var te = document.getElementById('PhoneNumber').value;
                phonenumber(te)
                if (phonenumber(te) == true && number(te) == true && $("#PhoneNumber").val().length == 11) {
                    // if (Dosse >= 1 && Durationn >= 1) { } else { bootbox.alert("Please Insert Valid Dose or Duration Numbers"); }
                    //if (parseFloat(totalduration) >= parseFloat(Durationn) && parseFloat(totalduration) > 1) { } else { bootbox.alert("Total Duration must be greater than duration"); }
                    changeTotalUnits();
                    changeTotalDuration();
                    changeTable();
                    if ($('#ClaimNumber').val() != "") {
                        var ze = document.getElementById('ClaimNumber').value;
                        var TotalDuration = $("TD", row).find(".TotalDuration").val();
                        var row = $(this);
                        if (number(ze) == true) {
                            if (row.val != "") {
                                if (TotalDuration != 0) {
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
                                            hasApproval: $('#HasApproval').prop("checked") ? true : false,
                                            CardId: $('#txtSearchCard').val(),
                                            RoshetaType: $('#ddlType').val(),
                                            CompanyPercent: $('#ddEmp_CEILING_PERT').val(),
                                            Limit: $('#insurance_LIVEL').val(),
                                            Speciality: $('#ddlSpeciality option:selected').text(),
                                            Diagnose1: $('#Comments').val(),
                                            Diagnose2: NationalId,
                                            TotalValue: $('#txtTotalInvoice').val(),
                                            PersonPayment: $('#txtTotalCopayment').val(),
                                            CompanyPayment: $('#txtValueCredit').val(),
                                            OverInsurance: $('#txtOverInsurance').val(),
                                            Cash: $('#txtValueCash').val(),
                                            PhoneNumber: $('#PhoneNumber').val(),
                                            ClaimNumber: $('#ClaimNumber').val(),
                                            createdby: $('#ddlUsers').val() == undefined ? null : $('#ddlUsers :selected').val(),
                                            roshitaDetail: Mediciens,
                                            diagnose: diagnose,
                                            IsFamily: $('#IsFamily').val() == '' ? null : $('#IsFamily').val(),
                                            IsPool: $('#IsPool').val() == '' ? null : $('#IsPool').val(),
                                        };
                                        $.ajax({
                                            type: 'POST',
                                            url: '/Pharmacy/SavePrescription/',
                                            dataType: 'Json',
                                            //contentType: "application/json; charset=utf-8",
                                            //data: JSON.stringify(SavePrescriptipn),
                                            data: SavePrescription,
                                            success: function (OracleId) {
                                                if (OracleId == "Failed") {
                                                    window.location.replace("/Pharmacy/Pharmacy");
                                                }
                                                else {
                                                    bootbox.dialog({
                                                        closeButton: false,
                                                        title: 'Added Sucessfully',
                                                        message: "Approval Number : " + OracleId,
                                                        buttons: {
                                                            Print: {
                                                                label: "Print",
                                                                className: 'btn-info',
                                                                callback: function () {
                                                                    // window.location.reload();
                                                                    ClearCardData();
                                                                    ClearMedicineData();
                                                                    $("#submit").attr("disabled", false);
                                                                    window.open('/Pharmacy/ControlPenelReport?id=' + OracleId);
                                                                }
                                                            },
                                                            New: {
                                                                label: "New",
                                                                className: 'btn-info',
                                                                callback: function () {
                                                                    ClearCardData();
                                                                    ClearMedicineData();
                                                                    $("#submit").attr("disabled", false);
                                                                    //window.location.reload();
                                                                }
                                                            }

                                                        }
                                                    });
                                                }
                                            },
                                            error: function (err) {
                                                bootbox.alert("Error saving roshita,please check your internet connection");
                                                $("#submit").attr("disabled", false);
                                            }
                                        });
                                        //save version 2 
                                        //$.ajax({
                                        //    type: 'POST',
                                        //    url: '/Pharmacy/SAVE/',
                                        //    dataType: 'Json',
                                        //    data: {
                                        //        CardId: $('#txtSearchCard').val(),
                                        //        RoshetaType: $('#ddlType').val(),
                                        //        CompanyPercent: $('#ddEmp_CEILING_PERT').val(),
                                        //        Limit: $('#insurance_LIVEL').val(),
                                        //        Speciality: $('#ddlSpeciality option:selected').text(),
                                        //        Diagnose1: $('#Comments').val(),
                                        //        Diagnose2: NationalId,
                                        //        //calculation
                                        //        TotalValue: $('#txtTotalInvoice').val(),
                                        //        PersonPayment: $('#txtTotalCopayment').val(),
                                        //        CompanyPayment: $('#txtValueCredit').val(),
                                        //        OverInsurance: $('#txtOverInsurance').val(),
                                        //        Cash: $('#txtValueCash').val(),
                                        //        PhoneNumber: $('#PhoneNumber').val(),
                                        //        ClaimNumber: $('#ClaimNumber').val(),
                                        //        createdby: $('#ddlUsers').val() == undefined ? null : $('#ddlUsers :selected').val(),
                                        //    },
                                        //    success: function (OracleId) {
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
                                        //        bootbox.dialog({
                                        //            closeButton: false,
                                        //            title: 'Added Sucessfully',
                                        //            message: "Approval Number : " + OracleId,
                                        //            buttons: {
                                        //                Print: {
                                        //                    label: "Print",
                                        //                    className: 'btn-info',
                                        //                    callback: function () {
                                        //                        // window.location.reload();
                                        //                        ClearCardData();
                                        //                        ClearMedicineData();
                                        //                        $("#submit").attr("disabled", false);
                                        //                        window.open('/Pharmacy/ControlPenelReport?id=' + OracleId);
                                        //                    }
                                        //                },
                                        //                New: {
                                        //                    label: "New",
                                        //                    className: 'btn-info',
                                        //                    callback: function () {
                                        //                        ClearCardData();
                                        //                        ClearMedicineData();
                                        //                        $("#submit").attr("disabled", false);
                                        //                        //window.location.reload();
                                        //                    }
                                        //                }

                                        //            }
                                        //        });
                                        //    },
                                        //    error: function (err) {
                                        //        bootbox.alert("Error saving roshita,please check your internet connection");
                                        //        $("#submit").attr("disabled", false);
                                        //    }
                                        //}).done(function () {// Last ajax
                                        //    $.ajax({
                                        //        type: 'POST',
                                        //        url: '/Pharmacy/SaveMediciens/',
                                        //        dataType: 'Json',
                                        //        contentType: "application/json; charset=utf-8",
                                        //        data: JSON.stringify(Mediciens),
                                        //        success: function (r) {
                                        //        },
                                        //        error: function (err) {
                                        //            bootbox.alert("Error save Medicines,please check your connection");
                                        //            $("#submit").attr("disabled", false);
                                        //        }
                                        //    });
                                        //    //Diagnoises
                                        //    $.ajax({
                                        //        type: 'POST',
                                        //        url: '/Pharmacy/SaveDiagnoises/',
                                        //        dataType: 'Json',
                                        //        contentType: "application/json; charset=utf-8",
                                        //        data: JSON.stringify(DiagnosisList),
                                        //        success: function (r) {
                                        //        },
                                        //        error: function (err) {
                                        //            bootbox.alert("Error Diagnoises,please check your connection");
                                        //            $("#submit").attr("disabled", false);
                                        //        }

                                        //    });
                                        //    //Approval
                                        //    if ($('#HasApproval').prop("checked")) {
                                        //        $.ajax({
                                        //            type: 'POST',
                                        //            url: '/Pharmacy/SaveDealApproval/',
                                        //            dataType: 'Json',
                                        //            data: { id: $('#txtSearchCard').val() },
                                        //            success: function (r) {

                                        //            },
                                        //            error: function (error) {
                                        //                alert(error);
                                        //                $("#submit").attr("disabled", false);
                                        //            }
                                        //        });
                                        //    }
                                        //});

                                        //$("#submit").attr("disabled", false);
                                    }

                                    else { bootbox.alert("Please Insert medicines"); }
                                }

                                else {
                                    bootbox.alert("Invalid Total Duration");
                                }


                            }
                        } else { bootbox.alert("Please Insert Valid ClaimNumber"); }

                    }
                    else {
                        //bootbox.confirm("يرجي التحقق من وجود ختم الطبيب المعالج وفي  حاله عدم وجود ختم مقدم الخدمه لايتم صرف الروشته والا سيتم خصمها بالكامل علي سيادتكم"
                        //شاملا الحرف الموجود مع الرقم 
                        var compid = $('#txtSearchCard').val().split('-')[0];
                        if ((compid == "500142" || compid == "500103" || compid == "500125" || compid == "10560") && ($('#ddlType').val() == "11603" || $('#ddlType').val() == "11601")) {
                            alert("هذا العميل لايحتاج موافقة علي الروشتات الخارجية علي ان يتم ادخال كافة البيانات والادوية علي السيستم");
                            $('#ClaimNumber').val(' ');
                            $('#submit').click();
                        }
                        else {
                            bootbox.confirm("رجاء كتابه رقم النموذج وذلك بشكل صحيح وف حالة الصرف من نموذج خارجى يلزم الحصول على رقم موافقة من الشركة وذلك بالاتصال على الارقام التالية:01099887396 | 01021975433 | 01021974375 هام جدا وذلك لعدم الخصم"
                                , function (result) {
                                    if (result) {
                                        $('#ClaimNumber').val(' ');
                                        $('#submit').click();
                                    } else toastr.error('Please Insert Valid ClaimNumber');
                                });
                        }
                    }


                }
                else { bootbox.alert("Please Insert Valid PhoneNumber"); }

            } else {
                bootbox.alert("Please Insert Valid PhoneNumber");

            }
        }
        else
            bootbox.alert("Please Insert Card ID");
    });

})
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
    var isChronic = 0;
    $.ajax({
        type: 'POST',
        url: '/Pharmacy/GetMedicineByCode/',
        dataType: 'json',
        data: { code: Code },
        success: function (r) {
            if (r.M_TYPE == "CHRONIC") {
                isChronic = 1;
                //edit = 1;
                //toastr.error('لا يمكن صرف هذا الدواء ضمن الادويه اليوميه للاستفسار برجاء الاتصال علي رقم الادارة الطبيه');
                //$("#AddMedicine option[value='" + Code + "']").remove();
                //$("#wait").css("display", "none");

            }
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
                                //toastr.error("You can't dispense this medicine");
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
                                        //toastr.error("You can't dispense this medicine");
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
                                        if (AgeValiation = true) {
                                            RemoveSelection(Code);
                                            //toastr.error("You can't dispense this medicine");//same Group
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
                                                // toastr.error("You can't dispense this medicine");//'Duration Validation'
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
                                    if (GenderValidation == true && AgeValiation == true && IsCover == "true" && samegroup == false && Duration == false) {
                                        //check Daily
                                        $("#wait").css("display", "block");
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
                                                    // chick if chronic or not 
                                                    if (isChronic == 0) {
                                                        //append row
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
                                                                                        Group = "Pending";
                                                                                        toastr.info('برجاءالتواصل مع الاداره الطبيه');
                                                                                        AppendRow();
                                                                                    }
                                                                                }
                                                                            }
                                                                        });
                                                                    }
                                                                }
                                                            });
                                                            ////if ($('#txtSearchCard').val().split('-')[0].includes("500")) {
                                                            ////    Group = "Accepted";
                                                            ////    AppendRow();
                                                            ////}
                                                            ////else {
                                                            //var dialog = bootbox.dialog({
                                                            //    title: 'This Medicien is Not Covered!',
                                                            //    message: "<p>Pay method?</p>",
                                                            //    //title: '!هذا الدواء غير مغطى',
                                                            //    //message: "<p float='right'>طريقه الدفع؟</p>",
                                                            //    onEscape: function () {
                                                            //        RemoveSelection(MedicienCode);
                                                            //    },
                                                            //    //backdrop: true,
                                                            //    buttons: {
                                                            //        Cash: {
                                                            //            label: "Cash",
                                                            //            className: 'btn-info',
                                                            //            callback: function () {
                                                            //                Group = "Cash";
                                                            //                AppendRow();
                                                            //            }
                                                            //        }
                                                            //        //, Approval: {
                                                            //        //    label: "Approved",
                                                            //        //    className: 'btn-info',
                                                            //        //    callback: function () {
                                                            //        //        Group = "Approval";
                                                            //        //        AppendRow();
                                                            //        //    }
                                                            //        //}
                                                            //        , Tele: {
                                                            //            label: "Pending",
                                                            //            className: 'btn-info',
                                                            //            callback: function () {
                                                            //                Group = "Pending";
                                                            //                toastr.info('برجاءالتواصل مع الاداره الطبيه');
                                                            //                AppendRow();
                                                            //            }
                                                            //        }
                                                            //    }
                                                            //});
                                                            ////}
                                                            // end if for group no
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
                                                                        toastr.info('برجاءالتواصل مع الاداره الطبيه');
                                                                        AppendRow();
                                                                    }
                                                                }
                                                            }
                                                        });
                                                    }
                                                }
                                                else {
                                                    RemoveSelection(Code);
                                                    //bootbox.alert("This Medicine had been exchanged ");
                                                    bootbox.alert(r.messa);
                                                    //bootbox.alert("هذا الدواء تم التعامل معه من قبل , برجاء مراجعه الادويه المزمنه للمريض,او الاتصال بالاداره الطبيه");
                                                    $("#wait").css("display", "none");
                                                }
                                            },
                                            error: function (r) {
                                                bootbox.alert("failed Exchanged validation ,chacke your internet connection  ");
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
    TotUnits = Math.ceil(TotUnits)
    //TotUnits = Math.round(TotUnits) == 0 ? 1 : Math.round(TotUnits);
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
//function changeTotalDuration(button) {

//    var row = $(button).closest("TR");
//    var TotalDuration = $("TD", row).find(".TotalDuration").val();
//    var Duration = $("TD", row).find(".Duration").val();
//    var Dose = $("TD", row).find(".Dose").val();
//    if ((parseFloat(diffDays)) < (parseFloat(Duration))) {
//        bootbox.alert("Duration Must be less than Contract days ");
//        $("TD", row).find(".Duration").val(diffDays);
//        return false;
//    }

//    if ($('#ddlType').val() == "11601" && (parseFloat(Duration)) > 21) {
//        bootbox.alert(" duration Must be more than less than 21 ");
//        $("TD", row).find(".Duration").val(5);
//        return false;
//    }
//    if ($('#ddlType').val() == "11603" && (parseFloat(Duration)) >= 28) {
//        bootbox.alert(" Duration Must be less than  or equal 28 ");
//        $("TD", row).find(".Duration").val(28);
//        return false;
//    }
//    if ((parseFloat(Duration)) > (parseFloat(TotalDuration))) {
//        bootbox.alert("Total duration Must be greater than Durartion ");
//        $("TD", row).find(".TotalDuration").val(Duration);
//        return false;
//    }
//    if ($('#ddlType').val() == "11601" && (parseFloat(TotalDuration)) < 5) {
//        bootbox.alert("Total duration Must be more than 5 and less than 21 ");
//        $("TD", row).find(".TotalDuration").val(5);
//        return false;
//    }
//    if ($('#ddlType').val() == "11601" && (parseFloat(TotalDuration)) > 21) {
//        bootbox.alert("Total duration Must be more than 5 and less than 21 ");
//        $("TD", row).find(".TotalDuration").val(5);
//        return false;
//    }
//    if ($('#ddlType').val() == "11603" && TotalDuration < 5) {
//        bootbox.alert("Total duration Must be more than 5 and less than or equal 28 ");
//        $("TD", row).find(".TotalDuration").val(28);
//        return false;
//    }
//    if ($('#ddlType').val() == "11603" && TotalDuration >= 28) {
//        bootbox.alert("Total duration Must be less than  or equal 28 ");
//        $("TD", row).find(".TotalDuration").val(28);
//        return false;
//    }

//    Calculation();
//}

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

    if (!($('#txtSearchCard').val().split('-')[0].includes("500"))) {
        if ((parseFloat(diffDays)) < (parseFloat(Duration))) {
            bootbox.alert("Duration Must be less than Contract days ");
            $("TD", row).find(".Duration").val(diffDays);
            //bootbox.alert("Duration Must be less than Contract days ");

            //if ($('#txtSearchCard').val().split('-')[0].includes("500")) {
            //    $.ajax({
            //        dataType: "json",
            //        url: '/Pharmacy/GetSecondContract',
            //        data: {
            //            id: $('#txtSearchCard').val()
            //        },
            //        success: function (r) {
            //            if (r.check == 1) {
            //                firstDate = new Date(parseFloat(r.Employee.INS_END_DATE.replace(/(^.*\()|([+-].*$)/g, '')));
            //                diffDays = Math.round(Math.abs((firstDate - secondDate) / oneDay));
            //                if ((parseFloat(diffDays)) < (parseFloat(Duration))) {
            //                    bootbox.alert("Duration Must be less than Contract days ");
            //                    $("TD", row).find(".Duration").val(diffDays);
            //                }
            //            }
            //            else {
            //                bootbox.alert("Duration Must be less than Contract days ");
            //                $("TD", row).find(".Duration").val(diffDays);
            //            }
            //        },
            //        error: function (r) {
            //            bootbox.alert("Duration Must be less than Contract days ");
            //            $("TD", row).find(".Duration").val(diffDays);
            //        }

            //    });
            //}

            //else {
            //    bootbox.alert("Duration Must be less than Contract days ");
            //    $("TD", row).find(".Duration").val(diffDays);
            //}

        }
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
            if (row.find("TD").eq(12).html() != "Pending" && row.find("TD").eq(12).html() != "PendingChronic") {
                sum += parseFloat(("TD", row).find(".Amount").val());
                if (row.find("TD").eq(12).html() == "Cash")
                    sumCash += parseFloat(("TD", row).find(".Amount").val());
            }
        });
        debugger;
        $("#txtTotalInvoice").val(sum.toFixed(2));
        $('#txtCash').val(sumCash.toFixed(2));
        $('#txtOverInsurance').val("0");
        if ($('#ddlType').val() == "11601") {
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
                if ((Limit * (co / 100)) <= (ValueCredit) && co != 0) {//over insurance
                    $('#txtTotalCopayment').val((Limit * (person / 100)).toFixed(2));
                    Limit = (Limit * (co / 100)).toFixed(2);
                    $('#txtValueCredit').val(Limit);
                    $('#txtOverInsurance').val((total - Limit - parseFloat($('#txtTotalCopayment').val())).toFixed(2));
                }
                else if ((Limit * (co / 100)) > (ValueCredit) /*|| co == 0*/) {//no over insurance
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
        else if ($('#ddlType').val() == "11603") {
            //monthly
            var Limit = parseFloat($("#insurance_LIVEL").val());
            var co = $("#ddEmp_CEILING_PERT").val();
            var person = parseFloat(100 - co);
            //total-cash
            var total = parseFloat($('#txtTotalInvoice').val()) - sumCash;
            var cash = parseFloat($('#txtCash').val());
            var ValueCredit = 0;
            if (Limit > AnuualLimit || Limit == 0) {
                Limit = AnuualLimit;
            }

            if (Limit != 0) {
                ValueCredit = (total * (co / 100)).toFixed(2);
                if ((Limit * (co / 100)) <= (ValueCredit) && co != 0) {
                    $('#txtTotalCopayment').val((Limit * (person / 100)).toFixed(2));
                    Limit = (Limit * (co / 100)).toFixed(2);
                    $('#txtValueCredit').val(Limit);//(Limit * (co / 100)).toFixed(2)
                    $('#txtOverInsurance').val((total - Limit - parseFloat($('#txtTotalCopayment').val())).toFixed(2));
                }
                else if ((Limit * (co / 100)) > (ValueCredit) /*|| co == 0*/) {
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
    else {
        $('#txtTotalInvoice').val('');
        $('#txtTotalCopayment').val('');
        $('#txtValueCredit').val('');
        $('#txtOverInsurance').val('');
        $('#txtCash').val('');
        $('#txtValueCash').val('');
    }


}
function GetLimit() {

    //Co-Payment
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/Pharmacy/CellingAmount',
        data: {
            id: CardId,
            ServiceCode: $('#ddlType').val()
        },
        success: function (r) {
            if (r.Validation == false) {
                // toastr.info(r.Message);
                //ClearCardData();
                alert(r.Message);
                //history.go(0);
                window.location.replace("/Pharmacy/Pharmacy");
                //window.location.reload();

            } else {
                $('#ddEmp_CEILING_PERT').val(r.CeilingPert);
                AnuualLimit = r.Limit;
                $('#IsFamily').val(r.IsFamily);
                $('#IsPool').val(r.IsPool);
                $('#AllLimit').val(r.AnnualLimit);
                if ($("#ddlType").val() == "11601") {
                    if (r.LimitDailyPreceptionCount && r.CoInsurancelimit.INSURANCE_DAY >= 0) {
                        $("#insurance_LIVEL").val(r.CoInsurancelimit.INSURANCE_DAY);
                    } else {
                        alert(" تم استهلاك العدد المحدد للروشتات في الشهر وسوف يتحمل المريض المبلغ بالكامل نقدا");
                        $("#insurance_LIVEL").val("0.001");

                        $('#ddEmp_CEILING_PERT').val("0");
                    }
                }
                else if ($("#ddlType").val() == "11603") {

                    //if (CompId.startsWith("10") || CompId.startsWith("70") || CompId == "500135" || CompId == "500136" || CompId == "500137" || CompId == "500138" || CompId == "500139" || CompId == "500140" || CompId == "500145") {
                    //    alert("برجاء الرجوع للإداره الطبيه");
                    //    window.location.reload();
                    //} else {}
                    if (r.LimitMonthlyPreceptionCount && r.CoInsurancelimit.INSURANCE_MONTH >= 0) {
                        $("#insurance_LIVEL").val(r.CoInsurancelimit.INSURANCE_MONTH);

                    } else {
                        alert(" تم استهلاك العدد المحدد للروشتات في الشهر وسوف يتحمل المريض المبلغ بالكامل نقدا");
                        $("#insurance_LIVEL").val("0.001");
                        $('#ddEmp_CEILING_PERT').val("0");
                    }

                }
                else if ($("#ddlType").val() == "11602") {
                    window.location.replace("/Chronic/Chronic?id=" + CardId + "&&NationalId=" + NationalId);
                }
                else if ($("#ddlType").val() == "11604") {
                    window.location.replace("/Doctor/Doctor?id=" + CardId + "&&NationalId=" + NationalId);
                    //window.location.replace("/Doctor/Doctor/" + CardId);
                }
            }
        },
        error: function (err) {
            alert("Failed to retrieve Company Annual Limit. please check your internet connection");
            location.reload();
        }
    })
    Calculation();

}

function GetCompName() {
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
                bootbox.alert(' No Company Name ');
            }
        }
    });
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
    $('#Comments').val('');
    $("#Approval").removeClass('active');
    $('#HasApproval').attr("checked", false)
    $('#Approval').attr('disabled', true);
    $('#Pending').attr('disabled', true);
    $("#PrescriptionDate").datepicker('destroy').datepicker({
        maxDate: '0',
        minDate: '-6D',
        dateFormat: 'dd-mm-yy',
    });
    $('#PrescriptionDate').val('');
    $('#ClaimNumber').val('');
    $('#PhoneNumber').val('');
    $('#contractComp_C_ANAME').val('');
    $('#compEmp_EMP_ANAME').val('');
    $('#compEmp_INS_START_DATE').val('');
    $('#compEmp_INS_END_DATE').val('');
    $('#compEmp_BIRTH_DATE').val('');
    $('#insurance_LIVEL').val('');
    $('#ddEmp_CEILING_PERT').val('');
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
                window.location = '/Pharmacy/Pharmacy';
                return true;
            }
            if (result === "" || result.length != 14 || isNaN(result)) {
                toastr.error("برجاء ادخال الرقم القومي الصحيح المتكون من 14 رقم");
                return false;
            } else {
                //$("#wait").css("display", "block");
                NationalId = result;
                return true;

            }
            return false;

        }
    });
}

