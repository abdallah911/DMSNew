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
});

function ClearAll() {

    
    $('#RegistrationFrom').val('');
    $('#RegistrationTo').val('');
    $('#CompNo').val(0);
    $('#userName').val(0);
    $('#AddMedicine').val(0);
    $('#CardNo').val(0);
    $('#TypeManager').val('All');
    $('#Type').val('All');
    $('#RepotType').val(1);    
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
