var ServicesArray = new Array();
var CardServicesArray = new Array();
var selectedCard = "";

$(function () {
    $('#CardID').select2({
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
    $('#ServiceDate').datepicker({ changeYear: true });
    $("#Type").change(function () {

        if ($(this).val() == "1") {//Indvidual
            $("#divCompany").hide();
            $("#divIndividual").show();
        }
        else {
            $("#divCompany").show();
            $("#divIndividual").hide();
        }

    });
    $('#CardId').on('select2:selecting', function (event) {

        var result = ValidAddCardServices(event.params.args.data.id);
        if (result.valid) {
            var CardService = {};

            CardService.CardId = event.params.args.data.id.trim();
            CardService.IndemnityServices = [];
            //CardService.AttachPDF = new FormData();
            //CardService.NationalIdPDF = new FormData();
            CardServicesArray.push(CardService);

            var tBody = $("#CardServices > TBODY")[0];
            var row = tBody.insertRow(-1);
            var cell = $(row.insertCell(-1));

            cell.html(CardService.CardId);

            cell = $(row.insertCell(-1));
            cell.html('<input class="form-control text-box single-line" id="AttachPDF" multiple="True" name="AttachPDF" required="True" type="File" value="">');

            cell = $(row.insertCell(-1));
            cell.html('<input class="form-control text-box single-line" id="NationalIdPhoto" multiple="True" name="NationalIdPhoto" required="True" type="File" value="">');

            cell = $(row.insertCell(-1));
            cell.html('<input type="button" id="addCardServices" value="Add services" onClick="selectedcard(this);" class="btn btn-success" style="text-transform: none">');

            cell = $(row.insertCell(-1));
            cell.html('<input type="button" id="RemoveCardServices" value="Remove" onClick="deletecard(this);" class="btn btn-danger" style="text-transform: none">');

            ClearAddCardServices();
        }
    })
    $('#addServices').click(function () {
        var result = ValidAddServices();
        if (result.valid) {
            var Service = {};
            //Service.CardId = selectedCard;
            Service.ServiceId = $("#Services").val();
            Service.ServiceName = $("#Services :selected").text();
            Service.SpecialistId = $("#Specialist").val();
            Service.SpecialistName = $("#Specialist :selected").text();
            Service.ServiceDate = $("#ServiceDate").val();
            Service.Value = $("#Value").val();
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

            cell = $(row.insertCell(-1));
            cell.html($("#Specialist :selected").text());

            cell = $(row.insertCell(-1));
            cell.html($("#Services :selected").text());

            cell = $(row.insertCell(-1));
            cell.html(Service.Value);

            cell = $(row.insertCell(-1));
            cell.html(Service.ServiceDate);

            cell = $(row.insertCell(-1));
            cell.html('<input type="button" id="RemoveServices" value="Remove" onClick="deleteservice(this);" class="btn btn-danger" style="text-transform: none">');

            ClearAddServices();
        }
    })
    $('#submit').click(function () {
        debugger;
        var result = ValidSaveIndemnty();
        if (result.valid) {

            //CardServicesArray.IndemnityServices = ServicesArray;

            var indemnityVMs = {
                Type: $('#Type').val(),
                BankName: $('#BankName').val(),
                BankBranch: $('#BankBranch').val(),
                BankAccount: $('#BankAccount').val(),
                // ServiceCardID: $('#ServiceCardID').val(),
                IndemnityCardsServices: CardServicesArray,
                //AttachPDF: "",
                CompanyName: "",
                CardID: "",
                EmployeeName: "",
                NationalID: ""
            };
            if ($("#Type").val() == "2") {

                indemnityVMs.CompanyName = $('#CompanyName').val();
            } else {//==1
                indemnityVMs.CardID = $('#CardID').val();
                indemnityVMs.EmployeeName = $('#EmployeeName').val();
                indemnityVMs.NationalID = $('#NationalID').val();
            }
            //var files2 = [];
            //var files = [];
            //var data = new FormData();//()
            //var data2 = new FormData();//()
            //$('#CardServices tbody tr').each(function () {
            //    var row = $(this);
            //    var Cardfiles = row.find(":file").get(0).files;
            //    var Nsationfiles = row.find(":file").get(1).files;
            //    //for (var i = 0; i < Cardfiles.length; i++) {
            //    // Cardfiles[i].name = row.find("TD").eq(0).html() + "_" + getCurrentDate() + "_" + Cardfiles[i].name;
            //    for (var x in CardServicesArray) {
            //        if (CardServicesArray[x].CardId == row.find("TD").eq(0).html()) {

                       
            //            for (var i = 0; i < Cardfiles.length; i++) {
            //                CardServicesArray[x].AttachPDF.append("AttachPDF", Cardfiles[i]);
            //            }
            //            for (var i = 0; i < files.Nsationfiles; i++) {
            //                CardServicesArray[x].NationalIdPDF.append("NationalIdPDF", Nsationfiles[i]);
            //            }
            //            //CardServicesArray[x].AttachPDF.append("Files", Cardfiles[i]); 
            //            //CardServicesArray[x].NationalIdPDF.append("Files", Cardfiles[i]);
            //            //CardServicesArray[x].AttachPDF=data;
            //            //CardServicesArray[x].NationalIdPDF=data2;
            //            break;
            //        }
            //    }
            //    //files.push(Cardfiles[i]);
            //    //}
            //});
            //var files = $("#AttachPDF").get(0).files;
            // data.append("Files", files[0]);
            //for (var i = 0; i < files.length; i++) {
            //    data.append("Files", files[i]);
            //    // indemnty.AttachPDF += files[i].name ;
            //    //if (i < files.length-1) {
            //    //    indemnty.AttachPDF += ",";
            //    //}
            //}

            //Saving Image
            //$.ajax({
            //    url: '/Indemnities/SaveAttaches/',
            //    type: "POST",
            //    processData: false,
            //    contentType: false,
            //    data: data,
            //    success: function (response) {

            //        //response image url instead of session
            //    },
            //    error: function (er) {
            //        alert("Error Upload Image");
            //    }

            //})
            console.log(indemnityVMs);
            $.ajax({
                type: 'POST',
                url: '/IndemnitiesAdmin/SaveIndemnity/',
                dataType: 'Json',
                //cache: false,
                //contentType: false,
                
                data: indemnityVMs,
                //processData: false,
                success: function (result) {

                    bootbox.dialog({
                        closeButton: false,
                        title: 'Added Sucessfully',
                        message: "Batch Number : " + result,
                        buttons: {
                            New: {
                                label: "New",
                                className: 'btn-info',
                                callback: function () {
                                    ClearSaveIndemnty();
                                }
                            }

                        }
                    });
                },
                error: function (err) {
                    bootbox.alert("Error saving Indemnty");
                }
            })
        }

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
        $("#CardId").require();
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
}
function ClearSaveIndemnty() {
    $("#Type").val('').trigger("change");

    $("#CompanyName").val("");
    $("#CardId").val("").change();
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
}
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
}
function ClearAddCardServices() {
    $("#CardId").val("").change();
    //  $("#AttachPDF").val("");
}

function ValidAddServices() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    $("#Services").require();//dll
    if (selectedCard == "") { toastr.error("Please select card "); return { valid: false }; }
    $("#Specialist").require();//dll
    if (selectedCard == "") { toastr.error("Please select card "); return { valid: false }; }
    if ($("#ServiceDate").val() == "") { toastr.error("Please insert service date."); return { valid: false }; }
    $("#Value").require();
    if ($("#Value").val() < 1) { toastr.error("Invaild value."); return { valid: false }; }
    for (var i in CardServicesArray) {
        for (var x in CardServicesArray[i].IndemnityServices) {
            if (CardServicesArray[i].CardId == selectedCard && CardServicesArray[i].IndemnityServices[x].ServiceId == $("#Services").val()) {
                toastr.error("Add before.");
                return { valid: false };
                break;
            }
        }
    }
    return $.validity.end();
}
function ClearAddServices() {
    $("#Services").val('').trigger("change");
    $("#ServiceDate").val("");
    $("#Value").val("");
}
function selectedcard(button) {
    var row = $(button).closest("TR");
    selectedCard = $("TD", row).eq(0).html();
    toastr.info("you have selected" + selectedCard);
}
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
}
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
}
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
}


