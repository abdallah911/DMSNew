function ShowDetailsCard(crd) {
    if (crd != '') {
        clearAll();
        getData(crd);
        $('#ShowDetailsModal').modal();
    }
    else
        bootbox.alert("Please Insert Card Id First");
}


function getData(crd) {
    $("#loading").show();
    $.ajax({

        type: "POST",
        dataType: "json",
        url: '/CustomerService/getData',
        data: { CardId: crd },

        success: function (cardDetails) {
            
            if (cardDetails.flg == "0") {

                $('#EmployeeName').val(cardDetails.dtDetails[0].EmployeeName);
                $('#BirthDate2').val(cardDetails.dtDetails[0].BirthDate);
                $('#Age2').val(cardDetails.dtDetails[0].Age);
                $('#Gender2').val(cardDetails.dtDetails[0].Gender);
                $('#SpecificDate2').val(cardDetails.dtDetails[0].SpecificDate);
                $('#StartDate2').val(cardDetails.dtDetails[0].StartDate);
                $('#EndDate').val(cardDetails.dtDetails[0].EndDate);
                $('#MaxAmount').val(cardDetails.dtDetails[0].MaxAmount);
                $('#ClassName').val(cardDetails.dtDetails[0].ClassName);               
                $('#OldCard2').val(cardDetails.dtDetails[0].OldCard);

                getConsumption(crd);
            }
        }

    });
    
}


function getConsumption(crd) {
    $("#loading").show();
    $.ajax({
        type: "POST",
        dataType: "json",
        url: '/CustomerService/getConsumption',
        data: { CardId: crd, dat1: $('#StartDate2').val(), dat2: $('#EndDate').val(), oldcardd: $('#OldCard2').val(), maxamt: $('#MaxAmount').val() },
        success: function (cardDetails) {           
            if (cardDetails.length > 0) {

                $('#MedicationClaims').val(cardDetails[0].MedicationClaims);
                $('#MedicationClaimsUnderReview').val(cardDetails[0].MedicationClaimsUnderReview);
                $('#MedicationConsumption').val(cardDetails[0].MedicationConsumption);
                $('#OtherConsumption').val(cardDetails[0].OtherConsumption);
                $('#AllConsumption').val(cardDetails[0].AllConsumption);
                $('#Remaining').val(cardDetails[0].Remaining);
                $('#Percent').val(cardDetails[0].Percent);
            }
        },
        complete: function () {
            $("#loading").hide();
        }
    });
}

function clearAll() {
    $('#EmployeeName').val('');
    $('#BirthDate2').val('');
    $('#Age2').val('');
    $('#Gender2').val('');
    $('#SpecificDate2').val('');
    $('#StartDate2').val('');
    $('#EndDate').val('');
    $('#MaxAmount').val('');
    $('#ClassName').val('');
    $('#OldCard2').val('');
    $('#MedicationClaims').val('');
    $('#MedicationClaimsUnderReview').val('');
    $('#MedicationConsumption').val('');
    $('#OtherConsumption').val('');
    $('#AllConsumption').val('');
    $('#Remaining').val('');
    $('#Percent').val('');
}
