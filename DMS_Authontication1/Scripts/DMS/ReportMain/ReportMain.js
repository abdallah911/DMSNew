$(function () {
    $('#CompNo').select2();
    $('#userName').select2();
    $('#AddMedicine').select2();
    $('#CardNo').select2();

    $('#RegistrationFrom').datepicker({});
    $('#RegistrationTo').datepicker({});


    $('#CardNo').select2({
        placeholder: 'Search for a card',
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

function PrintReport() {
    var RegistrationFrom = $('#RegistrationFrom').val();
    var RegistrationTo = $('#RegistrationTo').val();    
    var CopmanyNumber = $('#CompNo').val();
    var usernam = $('#userName').val();
    var medcod = $('#AddMedicine').val();
    var crd = $('#CardNo').val();
    var typmngr = $('#TypeManager').val();
    var type = $('#Type').val();
    var RepotType = $('#RepotType').val();
    

    if (RegistrationFrom != "" && RegistrationTo != "")  {

     

            //chick if not search with card number
                if ($("#excel").is(":checked")) {
                    // do something if the excel is  checked
                    if ($("#pdf").is(":checked")) {
                        // do something if the pdf is  checked
                        // Print PDF and EXCEL
                        window.open('/ReportMain/PrintReportsMedPdf?Regfrom=' + RegistrationFrom +
                            '&&Regto=' + RegistrationTo + '&&CopmanyNumber=' + CopmanyNumber +
                            '&&usernam=' + usernam + '&&medcod=' + medcod +
                            '&&type=' + type + '&&RepotType=' + RepotType + 
                            '&&crd=' + crd + '&&typmngr=' + typmngr);

                        window.open('/ReportMain/PrintReportsMedExcel?Regfrom=' + RegistrationFrom +
                            '&&Regto=' + RegistrationTo + '&&CopmanyNumber=' + CopmanyNumber +
                            '&&usernam=' + usernam + '&&medcod=' + medcod +
                            '&&type=' + type + '&&RepotType=' + RepotType +
                            '&&crd=' + crd + '&&typmngr=' + typmngr);
                    }
                    else {
                        // Print EXCEL Only
                        window.open('/ReportMain/PrintReportsMedExcel?Regfrom=' + RegistrationFrom +
                            '&&Regto=' + RegistrationTo + '&&CopmanyNumber=' + CopmanyNumber +
                            '&&usernam=' + usernam + '&&medcod=' + medcod +
                            '&&type=' + type + '&&RepotType=' + RepotType +
                            '&&crd=' + crd + '&&typmngr=' + typmngr);
                    }

                }
                else {
                    // Print PDF as Default
                    window.open('/ReportMain/PrintReportsMedPdf?Regfrom=' + RegistrationFrom +
                        '&&Regto=' + RegistrationTo + '&&CopmanyNumber=' + CopmanyNumber +
                        '&&usernam=' + usernam + '&&medcod=' + medcod +
                        '&&type=' + type + '&&RepotType=' + RepotType +
                        '&&crd=' + crd + '&&typmngr=' + typmngr);
                }                        
        
    }
    else {
        toastr.info('select date " From " and " TO " of Registration date first');
    }


}
