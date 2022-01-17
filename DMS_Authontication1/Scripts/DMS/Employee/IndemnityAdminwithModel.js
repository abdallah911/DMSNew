var ServicesArray = new Array();
var CardServicesArray = new Array();
var selectedCard = "";
var count = 0;
var count2 = 0;
$(function () {
    $('#CardId').select2();

    $('#ServiceDateTest').datepicker({
        changeYear: true,
        yearRange: "-100:+0", // last hundred years
        maxDate: 0,
    });


    $('#CardId').on('select2:selecting', function (event) {

        var result = ValidAddCardServices(event.params.args.data.id);
        if (result.valid) {
            var CardService = {};

            CardService.CardId = event.params.args.data.id.trim();
            CardService.IndemnityServices = [];
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
        var result = ValidAddServices();
        if (result.valid) {

            $.ajax({
                url: '/Employee/ChickServiceDate/',
                data: { id: selectedCard, servdate: $("#ServiceDateTest").val() },
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

                    }
                    else {
                        toastr.error("Expire service date.");
                    }
                },
                error: function () {
                    toastr.error("Error at service date.");
                }
            });
        }
    });


    $('#btn_create_new').click(function () {
        debugger;
        $(this).attr('disabled', true);
        var Servicestot = $('#SpecialistId');
        var isValid = true;
        var ServicesCount = Servicestot.length;
        var emailaddress = $('#Email').val();
        if (ServicesCount <= 0) {
            isValid = false;
        }
        if ($("#Name").val() == "" || $("#NationalId").val() == "") {
            toastr.error("Enter Employee Name and National Id");
            isValid = false;
        }
        if (emailaddress=="") {
            toastr.error("Enter valid email");
            isValid = false;
        }
        if ($("#NationalId").val().length != 14) {
            toastr.error("Enter valid National Id");
            isValid = false;
        }
        if (!validateEmail(emailaddress)) {
            toastr.error("Enter valid email");
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


});
function ValidSaveIndemnty() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    $("#Type").require();//dll
    if ($("#Type").val() == "2") {
        $("#CompanyName").require();

    }
    else if ($("#Type").val() == "1") {//==1
        $("#RelatedCardId").require();
        $("#EmployeeName").require();
        $("#NationalID").require();
    }
    $("#BankName").require();
    $("#BankBranch").require();
    $("#BankAccount").require();
    //$("#ServiceCardID").require();
    if ($("#AttachPDF").val() == "") { toastr.error("Please,select AttachPDF."); return { valid: false }; }
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
    $("#Type").val('').trigger("change");

    $("#CompanyName").val("");
    $("#RelatedCardId").val("").change();
    $("#EmployeeName").val("");
    $("#NationalID").val("");
    $("#BankName").val("");
    $("#BankBranch").val("");
    $("#BankAccount").val("");

    $("#CardServices >tbody").empty();
    $("#CardId").val("").change();//serviced card
    CardServicesArray = [];
    selectedCard = "";

    $("#tblServices >tbody").empty();
    ServicesArray = [];
    //$("#AttachPDF ").val("");
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
            if (CardServicesArray[i].CardId == selectedCard && CardServicesArray[i].IndemnityServices[x].ServiceId == $("#Services").val()
                && CardServicesArray[i].IndemnityServices[x].Specialist == $("#Specialist").val()) {
                toastr.error("Add before.");
                return { valid: false };
                break;
            }
        }
    }
    return $.validity.end();
};
function ClearAddServices() {
    //$("#Services").val('').trigger("change");
    //$("#ServiceDate").val("");
    //$("#Value").val("");
};
function selectedcard(button) {
    var row = $(button).closest("TR");
    selectedCard = $("TD", row).eq(0).html();
    toastr.info("you have selected" + selectedCard);
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
    table.deleteRow(row[0].rowIndex);

    for (var i in CardServicesArray) {
        for (var x in CardServicesArray[i].IndemnityServices) {
            if (CardServicesArray[i].CardId == row.find("TD").eq(0).html() && CardServicesArray[i].IndemnityServices[x].ServiceName == row.find("TD").eq(1).html()) {
                CardServicesArray[i].IndemnityServices.splice(x, 1)[0];
                break;
            }
        }
    }
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

