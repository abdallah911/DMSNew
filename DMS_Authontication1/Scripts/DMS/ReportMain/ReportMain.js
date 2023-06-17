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



    $('#CopmanyNumber2').select2();
    $('#ddlGroups').select2({
        placeholder: "Select Group"
    });
    $('#Date').datepicker({
        minDate: 0,
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'MM-yy',
        onClose: function (dateText, inst) {
            $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1))
        }
    });
    $("#CopmanyNumber2").change(function () {

        $("#ddlGroups").empty();
        if ($("#CopmanyNumber2").val() != "") {
            //provider
            $.ajax({
                type: 'POST',
                url: '/Pharmacy/CompanyGroupsList/',
                dataType: 'json',
                data: { CompId: $(this).val() },
                success: function (Groups) {
                    $("#ddlGroups").append('<option value="0">All</option>');
                    $.each(Groups, function (i, ddlGroups) {
                        $("#ddlGroups").append('<option value="' + ddlGroups.Value + '">' + ddlGroups.Text + '</option>');

                    });
                    $('#ddlGroups  option:eq(0)').attr('selected', 'selected');
                },
                error: function (ex) {
                    alert('Failed to retrieve Groups.');
                }

            });
        }



    });

    $('#CardNo2').select2({
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

function ClearAll() {

    $('#RegistrationFrom').val('');
    $('#RegistrationTo').val('');
    $('#CompNo').val('').trigger("change");
    $('#userName').val('').trigger("change");
    $('#AddMedicine').val('').trigger("change");
    $('#CardNo').val('').trigger("change");
    $('#TypeManager').val('All');
    $('#Type').val('All');
    $('#RepotType').val(1);

    $("#pdf").prop("checked", false);
    $("#excel").prop("checked", false);

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


function ClearAll2() {


    $('#RegistrationFrom').val('');
    $('#RegistrationTo').val('');
    $('#CompNo').val('').trigger("change");
    $('#CardNo').val('').trigger("change");
    $('#RepotType').val(1);
        
    $("#pdf").prop("checked", false);
    $("#excel").prop("checked", false);

}

function PrintReport2() {
    var RegistrationFrom = $('#RegistrationFrom').val();
    var RegistrationTo = $('#RegistrationTo').val();
    var CopmanyNumber = $('#CompNo').val();
    var crd = $('#CardNo').val();
    var RepotType = $('#RepotType').val();

        //chick if not search with card number
        if ($("#excel").is(":checked")) {
            // do something if the excel is  checked
            if ($("#pdf").is(":checked")) {
                // do something if the pdf is  checked
                // Print PDF and EXCEL
                window.open('/ReportMain/PrintReports2MedPdf?Regfrom=' + RegistrationFrom +
                    '&&Regto=' + RegistrationTo + '&&CopmanyNumber=' + CopmanyNumber +
                    '&&crd=' + crd + '&&RepotType=' + RepotType);

                window.open('/ReportMain/PrintReports2MedExcel?Regfrom=' + RegistrationFrom +
                    '&&Regto=' + RegistrationTo + '&&CopmanyNumber=' + CopmanyNumber +
                    '&&crd=' + crd + '&&RepotType=' + RepotType);
            }
            else {
                // Print EXCEL Only
                window.open('/ReportMain/PrintReports2MedExcel?Regfrom=' + RegistrationFrom +
                    '&&Regto=' + RegistrationTo + '&&CopmanyNumber=' + CopmanyNumber +
                    '&&crd=' + crd + '&&RepotType=' + RepotType);
            }

        }
        else {
            // Print PDF as Default
            window.open('/ReportMain/PrintReports2MedPdf?Regfrom=' + RegistrationFrom +
                '&&Regto=' + RegistrationTo + '&&CopmanyNumber=' + CopmanyNumber +
                '&&crd=' + crd + '&&RepotType=' + RepotType);
        }
}

function ClearAll3() {
    $('#Date').val('');    
    $('#CopmanyNumber2').val('').trigger("change");
    $('#CardNo2').val('').trigger("change");
    $('#ddlGroups').val('').trigger("change");
}

function PrintReport3() {
    var dat = $('#Date').val();
    var CopmanyNumber = $('#CopmanyNumber2').val();
    var crd = $('#CardNo2').val();
    var grup = $('#ddlGroups').val();
    if (CopmanyNumber != "") {
        window.open('/ReportMain/CompanyChronicReport?Date=' + $('#Date').val() +
            '&&CompId=' + CopmanyNumber + '&&GroupId=' + grup + '&&CardId=' + crd);
    }
    else {
        toastr.info('Please Select Company First');
    }
}