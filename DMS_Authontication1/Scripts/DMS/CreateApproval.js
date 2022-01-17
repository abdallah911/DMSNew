var input1 = document.getElementById("AddCardTxt");
input1.addEventListener("keyup", function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        $('#AddCard').click();
    }
});
var IsEdit = false;
var ApprovalSavedCode = "";
$(function () {
   // $('#TeethImageModal').modal();
    //const Page = "Monthly";

    //$('#txtSearchCompany').click(function () {
    //    //var table = $("#CardMedicines")[0];
    //    //table.deleteRow();
    //    $.ajax({
    //        url: '/CompanyGroup/CompanyGroupList',
    //        dataType: 'Json',
    //        success: function (r) {

    //            $('#CompaniesModal').modal();
    //            var setData = $("#Companies Tbody");
    //            setData.empty();

    //            for (var i = 0; i < r.length; i++) {

    //                var data = "<tr >" +
    //                    "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='Select(this);'></Button>" + "</td>" +
    //                    "<td>" + r[i].C_COMP_ID + "</td>" +
    //                    "<td>" + r[i].C_ANAME + "</td>" +
    //                    "</tr>"
    //                setData.append(data);

    //            }
    //            $('#Companies').dataTable().fnDestroy();
    //            $('#Companies').DataTable();

    //        },
    //        error: function () {
    //            alert("Error Retrieve CompanyGroup");
    //        }

    //    });
    //});
    $('#AddCard').click(function () {
        //var IsExist = 0;
        var AddCardTxt = $('#AddCardTxt').val();
        if (AddCardTxt != "") {
            $('#AddCardTxt').css("border-color", "#ccc");
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
                    }
                    else {
                        bootbox.alert("Inviled Card");
                    }
                },
                error: function (r) {
                    window.alert(' failed to retrive card list ');
                }

            });
        }
        else {
            bootbox.alert("Please insert Card ID")
        }
        //} else {
        //    bootbox.alert("Invalid Card")
        //}
    });
    $('#Cards').on('search.dt', function () {
        var setData = $("#CardMedicines tbody");
        setData.empty();
    });

    $('#ServiceProvider').click(function () {
        if ($('#AddCardTxt').val() != ""){
        $.ajax({
            dataType: "json",
            url: '/DoctorMedicinesLabsRaysApproval/ServiceProviderType',
            data: {},
            success: function (r) {

                if (r != null) {
                   
                    var setData = $("#ServiceProviders Tbody");
                    setData.empty();

                    for (var i = 0; i < r.length; i++) {
                        var data = "<tr >" +
                            "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectProviders(this);'></Button>" + "</td>" +
                            "<td>" + r[i].ProviderType + "</td>" +
                            "<td>" + r[i].ProviderName + "</td>" +
                            "</tr>"
                        setData.append(data);

                    }
                    // $('#SearchCards').DataTable();
                    $('#ServiceProvidersModal').modal();
                }
                else {
                    bootbox.alert("Inviled Card");
                }
            },
            error: function (r) {
                window.alert(' Service Provider Ajax ');
            }

        });
        } else {
            toastr.error("Please Insert Card Id");

            //$('#AddCardTxt').css("border-color","red")
        }
    });
    $("#ServiceType").change(function () {
        $.ajax({
            dataType: "json",
            url: '/DoctorMedicinesLabsRaysApproval/SubService',
            data: { ServiceType: $("#ServiceType").val() },
            success: function (r) {

                if (r != null) {
                    $('#Subservice').val("");
                    var setData = $("#SubServices Tbody");
                    setData.empty();

                    for (var i = 0; i < r.length; i++) {
                        var data = "<tr >" +
                            "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectSubService(this);'></Button>" + "</td>" +
                            "<td>" + r[i].Code + "</td>" +
                            "<td>" + r[i].Name + "</td>" +
                            "</tr>"
                        setData.append(data);

                    }
                    //$('#SubServices').DataTable();
                    $('#SubServiceModal').modal();
                }
                else {
                    bootbox.alert("Inviled Card");
                }
            },
            error: function (r) {
                window.alert(' Sub Service  Ajax Loading ');
            }

        });
        $.ajax({
            type: 'POST',
            url: '/DoctorMedicinesLabsRaysApproval/MedicalReplay/',
            dataType: 'json',
            data: {
                ServiceTypeName: $('#ServiceType option:selected').text(),
                //SubServiceCode: SubServiceCode,
                //ServiceTypeCode: $('#ServiceType option:selected').val()
            },
            success: function (r) {

                if (r != null) {

                    var setData = $("#MedicalResponses Tbody");
                    setData.empty();

                    for (var i = 0; i < r.length; i++) {
                        var data = "<tr >" +
                            "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectMedicalResponse(this);'></Button>" + "</td>" +
                            "<td>" + r[i].reply + "</td>" +
                            //"<td>" + r[i].ProviderName + "</td>" +
                            "</tr>"
                        setData.append(data);

                    }
                    // $('#SearchCards').DataTable();
                    // $('#MedicalResponseModal').modal();
                }
                else {
                    bootbox.alert("Inviled Card");
                }
            },
            error: function (ex) {
                alert('Failed to retrieve service type.');
            }
        });
    })
    // map //
    $("#ServiceType").map(function () {


    });
    $("#AgreementType").change(function () {
   
        if ($('#AgreementType option:selected').text() == "Exception") {
            $('#ExceptionResponsibilityModal').modal();
        }

    })
    // map //
    $("#AgreementType").map(function () {


    });
    $('#ApprovalValue').keyup(function () {
        if ($('#ApprovalValue').val()!="") {
            $('#ApprovalValue').css("border-color", "#ccc");
        }
        else {
            $('#ApprovalValue').css("border-color", "orange");
        }
        if ($('#Subservice').val() != "") {
            var CeilingPertPerecentage = $('#Ceilingpercentage').val();
            var ApprovalValue = $('#ApprovalValue').val();

            $('#PaidValue').val((CeilingPertPerecentage * (ApprovalValue / 100)).toFixed(2));
        if (MaxValueService < parseInt($('#ApprovalValue').val())) {
               
            bootbox.alert("The Maximum Value for this service can't be more than " + MaxValueService);
            $('#PaidValue').val((CeilingPertPerecentage * (MaxValueService / 100)).toFixed(2));
        }
        if (parseInt($('#Residual').val()) < parseInt($('#ApprovalValue').val()))
        {
        bootbox.dialog({
            closeButton: false,
            title: 'Maximum Value',
            message: "This Patient upon to maximum value,so it will be delgated to company resposibles as recollection ,or approval to maximum value only . ",
            buttons: {
                MaximumValue: {
                    label: "Approval to Max Value",
                    className: 'btn-info',
                    callback: function () {
                        $('#PaidValue').val($('#Residual').val());
                    }
                },
                Agree: {
                    label: "Aggree",
                    className: 'btn-info',
                    callback: function () {
                        Recollection = "RECOLMONAY";
                    }
                },
                Disagree: {
                    label: "Disggree",
                    className: 'btn-info',
                    callback: function () {
                        $('#ApprovalValue').val('');
                    }
                },
                Refuse: {
                    label: "Refuse Approval",
                    className: 'btn-info',
                    callback: function () {
                        $('#MedicalResponse').val('تم رفض هذه الموافقه لان قيمه الخدمه اكبر من قيمه لمتبقى');
                        $('#Response option[value=Rejected]').prop('selected', 'selected').change();
                    }
                }

            }
            });
        }
      
        }
        else {
            toastr.error('Please Insert Sub Service');
        }
    });
    $('#Ceilingpercentage').keyup(function () {
        var CeilingPertPerecentage = $('#Ceilingpercentage').val();
        var ApprovalValue = $('#ApprovalValue').val();
        if (CeilingPertPerecentage <= 100) {
            $('#PaidValue').val((CeilingPertPerecentage * (ApprovalValue / 100)).toFixed(2));
        }
        else {
            $('#Ceilingpercentage').val('0');
            $('#PaidValue').val('0');
        }

    });
 
    //Dignosies
    $.ajax({
        type: 'POST',
        url: '/DoctorMedicinesLabsRaysApproval/getDiag/',
        dataType: 'json',
        data: {},
        success: function (r) {
            $('#Diagnoises').dataTable().fnDestroy();
            var setData = $("#Diagnoises Tbody");
            setData.empty();

            for (var i = 0; i < r.length; i++) {
                var data = "<tr >" +
                    "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectDiagnoise(this);'></Button>" + "</td>" +
                    "<td>" + r[i].Code + "</td>" +
                    "<td>" + r[i].Name + "</td>" +
                    "</tr>"
                setData.append(data);
            }
            $('#Diagnoises').DataTable();
            
        },
        error: function (ex) {
            alert('Failed to retrieve Diagnoses.');
        }

    });
    
    $("#Email").change(function () {

        if ($('#Email').val() != "") {
            $('#Email').css("border-color", "#ccc");
        }
        else {
            $('#Email').css("border-color", "Orange");
        }

    });
    $("#Mobile").change(function () {

        if ($('#Mobile').val() != "") {
            $('#Mobile').css("border-color", "#ccc");
        }
        else {
            $('#Mobile').css("border-color", "Orange");
        }

    });
    $("#Upload").change(function () {
        if ($("#Upload").get(0).files.length <= 0) {
            $('#Upload').css("border", "1px solid red");
        }
        else {
            $('#Upload').css("border", "");
        }
    });
    var IsChangeCeilingpercentage = "No"
    $('#ChangeCeilingpercentage').click(function () {
        //if ($('#Ceilingpercentage').attr("disabled", true)){
         IsChangeCeilingpercentage = "yes";
        bootbox.alert("Change the ceilling value under your responsibility ");
        $('#Ceilingpercentage').attr("disabled", false);
        //}
        //else {
        //    IsChangeCeilingpercentage = "No";
           
        //    $('#Ceilingpercentage').attr("disabled", true);
        //}
    });
    var CurrentDate = new Date();
    //$('#ReceiveDate, #SentDate').val(CurrentDate.getDate().toString() + '-' + (CurrentDate.getMonth() + 1).toString() + '-' + CurrentDate.getFullYear().toString());
    $('#ReceiveDate, #SentDate').datepicker(
        {
            changeYear: true,
            dateFormat: 'dd-mm-yy',
            minDate: '-100 y',
            yearRange: "-100:+0",
            //maxDate:''

        });
    $('#AddMedicalResponse').click(function () {

        if ($('#ServiceType option:selected').text() != "--Select Service type--") {
            $('#MedicalResponseModal').modal();
        }
        else {
            toastr.error("Please Enter Service provider");
        }
    });
    $('#PersonalData').hover(function () {
        $('#PersonalData').addClass("glyphicon glyphicon-eye-close");
    });
    $('#PersonalData').mouseout(function () {
        $('#PersonalData').removeClass(" glyphicon-eye-close");
    });
    $('#History').hover(function () {
        $('#ShowHistory').addClass("glyphicon glyphicon-eye-close");
    });
    $('#ShowHistory').mouseout(function () {
        $('#ShowHistory').removeClass(" glyphicon-eye-close");
    });
    $('#PersonalData').click(function () {

        if (CardId != "") {
            $('#personalDataModal').modal();
        }
        else {
            bootbox.alert("Please  Select Card ");
        }
    });
    $('#ShowPools').click(function () {

        if (CardId != "") {
            $('#PoolsModal').modal();
        }
        else {
            bootbox.alert("Please  Select Card ");
        }
    });
    $('#ShowCardDesign').click(function () {

        if (CardId != "") {
            if (SrcIsExist == 1) {
                $('#CardDesignModal').modal();

            }
            else {
                toastr.error('No Card Design')
            }
        }
        else {
            bootbox.alert("Please  Select Card ");
        }
    });
    $('#ShowHistory').click(function () {

        if (CardId != "") {
            $('#HistoryModal').modal();

        }
        else {
            bootbox.alert("Please  Select Card ");
        }
    });
    //$('#SaveCounter').on('change', function (e) {
    //    if ($('#SaveCounter').val()==""){
    //        $('#Save').attr("disabled", false);
    //    } else {
    //        $('#Save').attr("disabled", true);
    //    }
    //});

    $('#ShowImg').click(function () {
        
        var preview = $('#ApprovalImg');// document.querySelector('img'); //selects the query named img
        var file =document.querySelector('input[type=file]').files[0]; //sames as here
        var reader = new FileReader();

        reader.onloadend = function () {
            preview.src = reader.result;
            $('#ApprovalImg').attr('src', reader.result);

        }

        if (file) {
            reader.readAsDataURL(file); //reads the data as a URL
        } else {
            preview.src = "";
        }
        $('#ApprovalImageModal').modal();
    });
    $('#AddDiagnoise').click(function () {
        $('#DiagnoiseModal').modal();
    });
    $('#AddSubService').click(function () {
        if ($('#ServiceType option:selected').text() != "--Select Service type--") {
            $('#SubServiceModal').modal();
        } else {
            toastr.error("Please Enter Service provider");
        }
    });
    $('#RemoveDiagnoise').click(function () {
        var splited = $('#Diagnoise').val().split("\n");
        if (splited[splited.length - 1] == "") {
            splited.splice(-1, 1);
            splited.splice(-1, 1);
            $('#Diagnoise').val(splited.join("\n"));
        }
        else {
            splited.splice(-1, 1);
            $('#Diagnoise').val(splited.join("\n"));
        }
    });

    $("#Save").click(function () {

        if ($('#AddCardTxt').val() != "" && $('#ServiceProvider').val() != "" && $('#Subservice').val() != "" && $('#ApprovalValue').val() != "" && $('#Diagnoise').val() != "" && $('#Email').val() != "" && $('#Mobile').val() != "" && $('#MedicalResponse').val() != "" && $("#Upload").get(0).files.length > 0)
        {
            var AgreementType;
            if ($('#AgreementType').val() == 'Exception') {
                AgreementType = ExceptionResponseName;
            }
            else {
                AgreementType = $('#AgreementType').val();
            }
            var response;
            if ($('#Response option:selected').val() == "Accepted") {
                response = "Y";
            }
            else {
                response = "N";
            }
           
            var Approval = {
                cod: ApprovalCode,
                comp: CompId,
                crd: $('.modal #CardId').val(),
                fx: $('#Fax').val(),
                emil: $('#Email').val(),
                mob: $('#Mobile').val(),
                rcdat: $('#ReceiveDate').val(),
                sedat: $('#SentDate').val(),
                apptyp: AgreementType,
                pvdnum: $('#ProviderPhone').val(),
                rply: response,
                medrply: $('#MedicalResponse').val(),
                nts: $('#Notes').val(),
                appval: $('#ApprovalValue').val(),
                valaft: $('#PaidValue').val(),
                srvtyp: $('#ServiceType option:selected').val(),
                pvd: ProviderTypeName,
                rato: $('#Ceilingpercentage').val(),
                //mxamun: $('.modal #MaxAmount').val(),#
               // appimg: data, //server
                ImageFile: data,
                //contr: $('#').val(),server
                mxamutcontr: $('.modal #MaxAmount').val(),
                //clss: $('#').val(),server
                //cretby: $('#').val(),server
                //cretdat: $('#').val(),server
                //anam: $('#').val(),#
                enam: $('.modal #EmpName').val(),
                birth: $('.modal #BirthDate').val(),
                strtdat: $('.modal #StartDate').val(),
                enddat: $('.modal #EndDate').val(),
                totcon: $('.modal #TotalConsumptiont').val(),
                //sbcod: $('#').val(),#
                pvdnam: ProviderId,
                cmpnam: CompName,
                digcod:DiagnoiseCode,
                dignam: DiagnoiseName,
                //flg: $('#').val(),#
                flg2: Recollection,
                chpcent: IsChangeCeilingpercentage,
                rsnrecol: RecolectionReasonText,
                vist: "N",//#
                EditReason:""

            };
            var data = new FormData();//()
            var files = $("#Upload").get(0).files;
            if (files.length > 0) {
                data.append("ImageFile", files[0]);

            }
            if (IsEdit == true) {
                
                Approval.EditReason = reasonValue;
                $.ajax({
                    //save Data After Saving Image
                    type: 'POST',
                    url: '/DoctorMedicinesLabsRaysApproval/SaveEdit/',
                    dataType: 'Json',
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(Approval),
                    success: function (r) {
                        bootbox.alert("Approval Code : " + r)
                    },
                    error: function (err) {
                        bootbox.alert("Error Approval");
                    }

                });
            }else{
            //Saving Image
                $.ajax({
                    url: '/DoctorMedicinesLabsRaysApproval/SaveImage/',
                    type: "POST",
                    processData: false,
                    contentType: false,
                    data: data,
                    success: function (response) {

                        //response image url instead of session
                    },
                    error: function (er) {
                        alert("Error Upload Image");
                    }

                }).done(function () {
                    $.ajax({
                        //save Data After Saving Image
                        type: 'POST',
                        url: '/DoctorMedicinesLabsRaysApproval/Save/',
                        dataType: 'Json',
                        contentType: "application/json; charset=utf-8",
                        data: JSON.stringify(Approval),
                        success: function (r) {
                            
                            ApprovalSavedCode = r;
                            bootbox.alert("Approval Code : " + r)
                        },
                        error: function (err) {
                            bootbox.alert("Error Approval");
                        }

                    }).done(function () {
                        //save diagnoise

                        var arrayOfLines = $('#Diagnoise').val().split('\n');
                        var Diagnoises = new Array();
                        $.each(arrayOfLines, function (index, item) {
                            var Diagnoise = {};
                            Diagnoise.DIAG_CODE = ApprovalSavedCode;
                            Diagnoise.DIAG_ANAME = item;
                            Diagnoises.push(Diagnoise);

                        });
                        $.ajax({
                            type: 'POST',
                            url: '/DoctorMedicinesLabsRaysApproval/SaveDiagnoises/',
                            dataType: 'Json',
                            contentType: "application/json; charset=utf-8",
                            data: JSON.stringify(Diagnoises),
                            success: function (r) {
                                //    bootbox.alert("Diagnoise saved ")
                            },
                            error: function (err) {
                                bootbox.alert("Error Approval");
                            }

                        });

                      //  var ServicesLines = $('#Subservice').val().split('\n');
                        var Services = new Array();

                        $.each(serviceList, function (index, item) {
                            var Service = {};
                            Service.ApprovalCode = ApprovalSavedCode;
                            Service.ServiceName = item.ServiceName;
                            Service.ServiceCode = item.ServiceCode;
                            Service.Details = item.Details;
                            Service.Description = item.Description;
                            Services.push(Service);
                        });
                        $.ajax({
                            type: 'POST',
                            url: '/DoctorMedicinesLabsRaysApproval/SaveServices/',
                            dataType: 'Json',
                            contentType: "application/json; charset=utf-8",
                            data: JSON.stringify(Services),
                            success: function (r) {
                                //bootbox.alert("Service saved ")
                            },
                            error: function (err) {
                                bootbox.alert("Error Service");
                            }

                        });
                    });
                });
            }
        }
        else {
            toastr.error("Please Insert Required data");
            if ($('#AddCardTxt').val() == "" ){
            $('#AddCardTxt').css("border-color","red");
            }
            if ($('#ServiceProvider').val() == ""){
                $('#ServiceProvider').css("border-color","red");
            }
            if ($('#Subservice').val() == ""){
                $('#Subservice').css("border-color", "red");
            }
            if ($('#ApprovalValue').val() == ""){
                $('#ApprovalValue').css("border-color", "red");
            }
            if ($('#Diagnoise').val() == ""){
                $('#Diagnoise').css("border-color", "red");
            }
            if ($('#Email').val() == "" ){
                $('#Email').css("border-color", "red");
            }
            if ($('#Mobile').val() == ""){
                $('#Mobile').css("border-color", "red");
            }
            if ($('#MedicalResponse').val() == ""){
                $('#MedicalResponse').css("border-color", "red");
            }
            if ($("#Upload").get(0).files.length <= 0){
                $('#Upload').css("border", "1px solid red");
            }
            
        }
  
    });
    $('#Recollection').change(function () {
        if ($('#Recollection').is(':checked')) {
            //reasons Ajax
            $('#RecolectionReasonModal').modal();
            Recollection = "RECALL";
        }
        else {
            Recollection = "yes";
            RecolectionReasonText = "";
        }
    });
    
    $('#Manager').change(function () {
       
        if ($('#Manager').val() == 'Edit') {
            //edit reason ajax
            $('#EditApprovalReasonsModal').modal();
            
            
        }
        else if ($('#Manager').val() == 'Delete') {
            //delete reason ajax
            $('#DeleteApprovalReasonsModal').modal();
            
        }
        else {
            ApprovalCode = "";
            IsEdit = false;
        }
    })
});