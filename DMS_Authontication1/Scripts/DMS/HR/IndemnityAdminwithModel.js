var ServicesArray = new Array();
var CardServicesArray = new Array();
var selectedCard = "";
var count = 0;
var count2 = 0;
$(function () {
    if ($("#userRole").val() == "1") {
        var cardsearch = $("#RelatedCardId").val();
        $.ajax({
            url: '/IndemnitiesAdmin/GetRelatedCards/',
            data: { CardId: cardsearch },
            dataType: 'Json',
            success: function (r) {
                $('#CardId').append('<option value="0">Select Card Id </option>');
                if (r.Success == "Yes" && r.subCards.length > 0) {
                    for (var i = 0; i < r.subCards.length; i++) {
                        $('#CardId').append('<option value="' + r.subCards[i].CardIDValue + '">' + r.subCards[i].CardIdString + '</option>');
                    }
                    $('#CardId').select2();
                }
                else {
                    $('#CardId').append('<option value="' + cardsearch + '">' + cardsearch + '</option>');
                    $('#CardId').select2();
                }
            },
            error: function () {
                toastr.error("Error get related cards for this card .");
            }
        });
    }
    else {
        $("#CardId").select2();
        $('#RelatedCardId').select2({
            placeholder: 'Search for a cards',
            minimumInputLength: 5,
            ajax: {
                url: '/EmployeeRequest/GetActiveEmployess/',
                delay: 250,
                dataType: 'json',
                data: function (params) {
                    var query = {
                        search: params.term,
                        page: params.page || 1
                    }
                    return query;
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;

                    return {
                        results: data,//.results,
                        pagination: {
                            more: (params.page * 10) < data.count_filtered
                        }
                    };
                }
                // Additional AJAX parameters go here; see the end of this chapter for the full code of this example

            }
        });

        $('#RelatedCardId').on('select2:selecting', function (event) {
            if ($("#Type").val() == 1) {
                $("#CardId").val('').trigger('change');
                var cardsearch = event.params.args.data.id.trim();
                $.ajax({
                    url: '/IndemnitiesAdmin/GetRelatedCards/',
                    data: { CardId: cardsearch },
                    dataType: 'Json',
                    success: function (r) {
                        $('#CardId').append('<option value="0">Select Card Id </option>');
                        if (r.Success == "Yes" && r.subCards.length > 0) {
                            for (var i = 0; i < r.subCards.length; i++) {
                                $('#CardId').append('<option value="' + r.subCards[i].CardIDValue + '">' + r.subCards[i].CardIdString + '</option>');
                            }
                            $('#CardId').select2();
                        }
                        else {
                            $('#CardId').append('<option value="' + cardsearch + '">' + cardsearch + '</option>');
                            $('#CardId').select2();
                        }
                    },
                    error: function () {
                        toastr.error("Error get related cards for this card .");
                    }
                });
            }
            else {
                $('#CardId').val('');
                $('#CardId').select2({
                    placeholder: 'Search for a cards',
                    minimumInputLength: 5,
                    ajax: {
                        url: '/EmployeeRequest/GetActiveEmployess/',
                        delay: 250,
                        dataType: 'json',
                        data: function (params) {
                            var query = {
                                search: params.term,
                                page: params.page || 1
                            }
                            return query;
                        },
                        processResults: function (data, params) {
                            params.page = params.page || 1;

                            return {
                                results: data,//.results,
                                pagination: {
                                    more: (params.page * 10) < data.count_filtered
                                }
                            };
                        }
                        // Additional AJAX parameters go here; see the end of this chapter for the full code of this example

                    }
                });
            }
        });
    }
    $('#ServiceDateTest').datepicker({
        changeYear: true,
        yearRange: "-100:+0", // last hundred years
        maxDate: 0,
    });

    $("#Type").change(function () {
        $("#RelatedCardId").val('').trigger('change');
        $("#CardId").val('').trigger('change');
        $("#RelatedCardId").select2("val", "");
        $("#CardId").empty();
        ClearSaveIndemnty();
        if ($(this).val() == "1") {//Indvidual

            $('#CardId').select2();
            $("#divCompany").hide();
            $("#divIndividual").show();
        }
        else {

            $("#divCompany").show();
            $("#divIndividual").hide();
            $('#CardId').val('');
            $('#CardId').select2({
                placeholder: 'Search for a cards',
                minimumInputLength: 5,
                ajax: {
                    url: '/EmployeeRequest/GetActiveEmployess/',
                    delay: 250,
                    dataType: 'json',
                    data: function (params) {
                        var query = {
                            search: params.term,
                            page: params.page || 1
                        }
                        return query;
                    },
                    processResults: function (data, params) {
                        params.page = params.page || 1;

                        return {
                            results: data,//.results,
                            pagination: {
                                more: (params.page * 10) < data.count_filtered
                            }
                        };
                    }
                    // Additional AJAX parameters go here; see the end of this chapter for the full code of this example

                }
            });
        }

    });

    $('#CardId').on('select2:selecting', function (event) {

        var result = ValidAddCardServices(event.params.args.data.id);
        if (result.valid) {
            var CardService = {};

            CardService.CardId = event.params.args.data.id.trim();
            CardService.IndemnityServices = new Array();
            CardService.AttachPDF = "";
            CardServicesArray.push(CardService);

            var tBody = $("#CardServices > TBODY")[0];
            var row = tBody.insertRow(-1);

            var cell = $(row.insertCell(-1));
            cell.html(CardService.CardId);


            cell = $(row.insertCell(-1));
            cell.html('<input class="form-control text-box single-line .NationalIdPDF" id="NationalIdPDF" multiple="True" name="IndemnityCardsImages[' + count + '].NationalIdPDF" required="True" type="File" value="">');

            cell = $(row.insertCell(-1));
            cell.html('<input type="button" id="addCardServices" value="Add services" onClick="selectedcard(this);" class="btn btn-success" style="text-transform: none">');
            cell = $(row.insertCell(-1));
            cell.html('<input type="button" id="RemoveCardServices" value="Remove" onClick="deletecard(this);" class="btn btn-danger" style="text-transform: none">');

            cell = $(row.insertCell(-1));
            cell.html('<input class="form-control text-box single-line .PhotoCardId" id="PhotoCardId" name="IndemnityCardsImages[' + count + '].PhotoCardId"  type = "text" value = "' + CardService.CardId + '"  > ');
            cell.attr("hidden", true);

            ClearAddCardServices();
            count++;
        }
    });
    $('#addServices').click(function () {
        //debugger;
        var result = ValidAddServices();
        if (result.valid) {

            $.ajax({
                url: '/IndemnitiesAdmin/ChickServiceDate/',
                data: { id: selectedCard, ServType: $("#Services").val(), servdate: $("#ServiceDateTest").val() },
                dataType: 'Json',
                success: function (r) {
                    if (r.Success == "Yes") {

                        var Service = {};
                        //Service.CardId = selectedCard;
                        Service.Specialist = $("#Specialist").val();
                        Service.ServiceId = $("#Services").val();
                        Service.ServiceName = $("#Services :selected").text();
                        Service.ServiceDate = $("#ServiceDateTest").val();
                        Service.Value = $("#ValueTest").val();
                        //CardService.IndemnityServices.push(Service);
                        for (var i in CardServicesArray) {
                            if (CardServicesArray[i].CardId == selectedCard) {
                                CardServicesArray[i].IndemnityServices.push(Service);
                                break; //Stop this loop, we found it!
                            }
                        }


                        var tBody = $("#tblServices > TBODY")[0];
                        var row = tBody.insertRow(-1);
                        var cell = $(row.insertCell(-1));
                        cell.html(selectedCard);
                        //cell.html(selectedCard);

                        cell = $(row.insertCell(-1));
                        cell.html($("#Specialist :selected").text());
                        //cell.html($("#Specialist :selected").text());

                        cell = $(row.insertCell(-1));
                        cell.html($("#Services :selected").text());
                        //cell.html($("#Services :selected").text());

                        cell = $(row.insertCell(-1));
                        cell.html($("#ValueTest").val());
                        //cell.html(Service.Value);

                        cell = $(row.insertCell(-1));
                        cell.html($("#ServiceDateTest").val());
                        //cell.html(Service.ServiceDate);

                        cell = $(row.insertCell(-1));
                        cell.html('<input class="form-control text-box single-line .AttachPDF" id="AttachPDF" multiple="True" name="ServiceTest[' + count2 + '].AttachPDF" required="True" type="File" value="">');


                        cell = $(row.insertCell(-1));
                        cell.html('<input type="button" id="RemoveServices" value="Remove" onClick="deleteservice(this);" class="btn btn-danger" style="text-transform: none">');


                        cell = $(row.insertCell(-1));
                        cell.html('<input class="form-control text-box single-line .SpecialistId" id="SpecialistId" name="ServiceTest[' + count2 + '].SpecialistId"  type = "text" value = "' + $("#Specialist").val() + '"  > ');
                        cell.attr("hidden", true);

                        cell = $(row.insertCell(-1));
                        cell.html('<input class="form-control text-box single-line .ServiceId" id="ServiceId" name="ServiceTest[' + count2 + '].ServiceId"  type = "text" value = "' + $("#Services").val() + '"  > ');
                        cell.attr("hidden", true);


                        cell = $(row.insertCell(-1));
                        cell.html('<input class="form-control text-box single-line .ServiceCardId" id="serCards" name="ServiceTest[' + count2 + '].ServiceCardId"  type = "text" value = "' + selectedCard + '"  > ');
                        cell.attr("hidden", true);


                        cell = $(row.insertCell(-1));
                        cell.html('<input class="form-control text-box single-line .Value" id="Value" name="ServiceTest[' + count2 + '].Value"  type = "text" value = "' + $("#ValueTest").val() + '"  > ');
                        cell.attr("hidden", true);


                        cell = $(row.insertCell(-1));
                        cell.html('<input class="form-control text-box single-line .ServiceDate" id="ServiceDate" name="ServiceTest[' + count2 + '].ServiceDate"  type = "text" value = "' + $("#ServiceDateTest").val() + '"  > ');
                        cell.attr("hidden", true);


                        ClearAddServices();
                        count2++;
                        selectedCard = "";

                    }
                    else
                        toastr.error(r.Message);
                },
                error: function () {
                    toastr.error("Error at service date.");
                }
            });
        }
        else {

        }
    });

    $('#btn_create_new').click(function () {
        //debugger;
        $(this).attr('disabled', true);
        var Servicestot = $('#SpecialistId');
        var emailaddress = $('#Email').val();
        var isValid = true;
        var ServicesCount = Servicestot.length;
        if (ServicesCount <= 0) {
            isValid = false;
        }
        if (emailaddress == "") {
            toastr.error("Enter valid email");
            isValid = false;
        }
        if (!validateEmail(emailaddress)) {
            toastr.error("Enter valid email");
            isValid = false;
        }
        //if ($("#BankName").val() == "") {
        //    toastr.error("Enter Bank Name");
        //    isValid = false;
        //}

        //if ($("#BankAccount").val() == "") {
        //    toastr.error("Enter Bank Account");
        //    isValid = false;
        //}
        if ($("#Phone").val() == "" && $("#Phone").val().length != 11) {
            toastr.error("Please insert Vaild Phone number");
            isValid = false;
        }
        var result = ValidSaveIndemnty();
        if (!result.valid) {
            isValid = false;
        }
        if (isValid) {
            $('.createInde-form').submit();

        }

        else {
            toastr.error("Enter Valid Data ");
            $(this).attr('disabled', false);
            return false;
        }
        return false;
    });

    $("#CompanyNumber").change(function () {// class levels
        $("#CARD_ID").val(null).trigger('change');
        var compNu = $("#CompanyNumber").val();
        ClearSaveIndemnty();
        $("#CompanyName").val(compNu);
        $("#UserCompId").val(compNu);
    });

    $("#CompanyNumber").map(function () {// class levels
        if ($("#userRole").val() != "1") {
            var compNu = $("#CompanyNumber").val();
            $("#CompanyName").val(compNu);
            $("#UserCompId").val(compNu);
        }
    });

});
function ValidSaveIndemnty() {
    debugger;
    var cardnationalids = new Array();
    var Servicesattached = new Array();
    $("#CardServices TBODY TR").each(function () {

        var row = $(this);
        nationalid = $("TD", row).find("#NationalIdPDF").val();
        cardnationalids.push(nationalid);
    });

    $("#tblServices TBODY TR").each(function () {

        var row = $(this);
        servattach = $("TD", row).find("#AttachPDF").val();
        Servicesattached.push(servattach);
    });


    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    $("#Type").require();//dll
    
    if ($("#Type").val() == "1") {//==1
        $("#RelatedCardId").require();
        
    }
    
    //$("#BankName").require();
    //$("#BankAccount").require();
    $("#Phone").require();

    for (var i in cardnationalids) {
        if (cardnationalids[i] == "") {
            toastr.error("please add National Id  File ");
            return { valid: false };
            break;
        }
    }
    for (var i in Servicesattached) {
        if (Servicesattached[i] == "") {
            toastr.error("please add service File ");
            return { valid: false };
            break;
        }
    }
    
    if (CardServicesArray.length == 0) { toastr.error("Please,Insert cards."); return { valid: false }; }

    var splitCard = $("#CardId").val().split("-");
    if (splitCard.length != 4 || splitCard[0] != $('#UserCompId').val()) {
        toastr.error("Inviled card.");
        return { valid: false };
    }
    for (var i in CardServicesArray) {
        if (CardServicesArray[i].IndemnityServices.length == 0) {
            toastr.error("please add service to this card " + CardServicesArray[i].CardId);
            return { valid: false };
            break;
        }
    }
    return $.validity.end();
};
function ClearSaveIndemnty() {
   
    $("#RelatedCardId").val("").change();
    $("#EmployeeName").val("");
    $("#NationalID").val("");
    //$("#BankName").val("");
    //$("#BankBranch").val("");
    //$("#BankAccount").val("");
    $("#ServiceDateTest").val("");
    $("#ValueTest").val("");
    $("#Services").val("");
    $("#Specialist").val("");
    $("#Email").val("");

    $("#CardServices >tbody").empty();
    $("#CardId").val("").change();//serviced card
    CardServicesArray = [];
    selectedCard = "";

    $("#tblServices >tbody").empty();
    ServicesArray = [];
    //$("#AttachPDF ").val("");
};


function ResetSaveIndemnty() {
    window.location.href = "/IndemnitiesAdmin/CreateAdmin";

};

function ValidAddCardServices(CardId) {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    //$("#CardId").require();//dll
    var splitCard = CardId.split("-");

    // chick card id is correct or not
    if (splitCard.length != 4 || splitCard[3] == "" || splitCard[0] != $('#UserCompId').val()) {
        toastr.error("Inviled card.");
        return { valid: false };
    }
    //if ($("#AttachPDF").val() == "") { toastr.error("Please,select AttachPDF."); return { valid: false }; }
    for (var i in CardServicesArray) {
        if (CardServicesArray[i].CardId == CardId.trim()) {
            toastr.error("Add before.");
            return { valid: false };
            break;
        }
    }

    return $.validity.end();
};
function ClearAddCardServices() {
    //$("#CardId").val("").change();
    //  $("#AttachPDF").val("");
};

function ValidAddServices() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    $("#Specialist").require();
    $("#Services").require();
    if (selectedCard == "") { toastr.error("Please select card "); return { valid: false }; }
    if ($("#ServiceDateTest").val() == "") { toastr.error("Please insert service date."); return { valid: false }; }
    $("#ValueTest").require();
    if ($("#ValueTest").val() < 1) { toastr.error("Invaild value."); return { valid: false }; }
    for (var i in CardServicesArray) {
        for (var x in CardServicesArray[i].IndemnityServices) {
            console.log(CardServicesArray[i].IndemnityServices[x]);
            if (CardServicesArray[i].CardId == selectedCard && CardServicesArray[i].IndemnityServices[x].ServiceId == $("#Services").val()
                && CardServicesArray[i].IndemnityServices[x].Specialist == $("#Specialist").val()
                && CardServicesArray[i].IndemnityServices[x].ServiceDate == $("#ServiceDateTest").val()
                && CardServicesArray[i].IndemnityServices[x].Value == $("#ValueTest").val()) {
                toastr.error("Add before.");
                return { valid: false };
                break;
            }
        }
    }
    return $.validity.end();
};
function ClearAddServices() {
    $("#ServiceDateTest").val("");
    $("#ValueTest").val("");
    $("#Services").val("");
    $("#Specialist").val("");
    //$("#Services").val('').trigger("change");
    //$("#ServiceDate").val("");
    //$("#Value").val("");
};
function selectedcard(button) {
    var row = $(button).closest("TR");
    selectedCard = $("TD", row).eq(0).html();
    toastr.info("you selected card " + selectedCard);
};
function deletecard(button) {
    debugger;
    var row = $(button).closest("TR");
    for (var i in CardServicesArray) {
        if (CardServicesArray[i].CardId == row.find("TD").eq(0).html() && CardServicesArray[i].IndemnityServices.length == 0) {
            CardServicesArray.splice(i, 1)[0]
            var table = $("#CardServices")[0];
            table.deleteRow(row[0].rowIndex);
            break;
        } else {
            toastr.warning("Please delete related cards");
            break;
        }
    }
};

function deleteservice(button) {
    var row = $(button).closest("TR");
    var table = $("#tblServices")[0];
    var card = row.find("TD").eq(0).html();
    var serv = row.find("TD").eq(2).html();
    for (var i in CardServicesArray) {
        for (var x in CardServicesArray[i].IndemnityServices) {
            //console.log(CardServicesArray[i].CardId == card && CardServicesArray[i].IndemnityServices[x].ServiceName == serv);

            if (CardServicesArray[i].CardId == card && CardServicesArray[i].IndemnityServices[x].ServiceName == serv) {

                delete (CardServicesArray[i].IndemnityServices[x]);               //CardServicesArray[i].IndemnityServices[x].splice($.inArray(CardServicesArray[i].IndemnityServices[x].ServiceId, CardServicesArray[i].IndemnityServices[x]),1);
                //CardServicesArray[i].IndemnityServices[x].splice($.inArray(CardServicesArray[i].IndemnityServices[x].Specialist, CardServicesArray[i].IndemnityServices[x]),1);
                //console.log(CardServicesArray[i].IndemnityServices[x]);
                break;
            }
        }
    }
    table.deleteRow(row[0].rowIndex);
};

function getCurrentDate() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();
    var hh = today.getHours();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    today = mm + dd + yyyy + hh;
    return today;
};

function validateEmail(email) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test(email);
}