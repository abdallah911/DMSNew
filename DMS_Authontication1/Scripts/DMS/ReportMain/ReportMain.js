$(function () {
    $('#CopmanyNumberTo').select2();
    $('#CopmanyNumberFrom').select2();

    $('#RegistrationFrom').datepicker({});
    $('#RegistrationTo').datepicker({});

    $('#UserName').select2({
        placeholder: 'Search for a user',
        //minimumInputLength: 5,
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
                var obj = {};
                data.push(obj)
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

    $('#MedCode').select2({
        placeholder: 'Search for a med',
        //minimumInputLength: 5,
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
                var obj = {};
                data.push(obj)
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
});

function ClearAll() {

    StopTime();

    $("#txtSearch").val('');
    $("#txtPhone").empty();
    $("#txtCardRelation").empty();
    
    $("#provider_Type").empty();
    $("#Country_select").empty();
    $("#Region_select").empty();    

    $('#CardInformationDetails').hide();
    //$("#ClaimsDataDetails").dataTable().fnDestroy();
    //$("#IndemnityDetailsData").dataTable().fnDestroy();
    //$("#MonthlyDetails").dataTable().fnDestroy();
    //$("#MonthlyDataDetails").dataTable().fnDestroy();
    //$("#LiveDetails").dataTable().fnDestroy();
    //$("#ProviderList").dataTable().fnDestroy();
    //$("#ApprovalsDetails").dataTable().fnDestroy();

    $('#hours').html('');
    $('#minutes').html('');
    $('#seconds').html('');

    $('#CardIdM').val('');
    $('#ClaimNo').val('');
    $('#CardID').val('');
    $('#EmployeeName').val('');
    $('#BirthDate').val('');
    $('#Age').val('');
    $('#StartDate').val('');

    $('#EndDate').val('');
    $('#MaxAmount').val('');
    $('#ClassName').val('');
    $('#HospitalDegree').val('');
    $('#MedicalNetwork').val('');

    $('#ExceptionPayment').val('');
    $('#ExceptionOver').val('');
    $('#NationalId').val('');
    $('#Mobile1').val('');
    $('#Mobile2').val('');

    $('#CardColor').val('');
    $('#OldCard').val('');

    $('#MedicationClaims').val('');
    $('#MedicationClaimsUnderReview').val('');
    $('#OtherConsumption').val('');
    $('#AllConsumption').val('');
    $('#Remaining').val('');
    $('#Percent').val('');
    $('#ApprovalConsumption').val('');

    $("#CountApprovals").val('');
    $("#TotalAmountApprovals").val('');

    $("#CountClaims").val('');
    $("#TotalGrossClaims").val('');
    $("#TotalNetClaims").val('');
}

