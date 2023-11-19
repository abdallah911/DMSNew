$(function () {
    $('#TypeReport').val();
    $('#CopmanyNameReportsFrom').select2();
    $('#CopmanyNameReportsTo').select2();
    $('#RegistrationFrom').datepicker({});
    $('#RegistrationTo').datepicker({});
    $('#ActivationFrom').datepicker({});
    $('#ActivationTo').datepicker({});

});

function PrintReport() {
    var CopmanyNameReportsFrom = $('#CopmanyNameReportsFrom').val();
    var CopmanyNameReportsTo = $('#CopmanyNameReportsTo').val();
    var RegistrationFrom = $('#RegistrationFrom').val();
    var RegistrationTo = $('#RegistrationTo').val();
    var ActivationFrom = $('#ActivationFrom').val();
    var ActivationTo = $('#ActivationTo').val();
    var TypeReport = $('#TypeReport').val();

    if (RegistrationFrom != "" && RegistrationTo != "" ) {
        //chick if not search with card number
        if ($("#excel").is(":checked")) {
            // do something if the excel is  checked
            if ($("#pdf").is(":checked")) {
                // do something if the pdf is  checked
                // Print PDF and EXCEL
                window.open('/NewReports/PrintReportsPdf?RegistrationFrom=' + RegistrationFrom +
                    '&&RegistrationTo=' + RegistrationTo + '&&ActivationFrom=' + ActivationFrom + '&&ActivationTo=' + ActivationTo +
                    '&&CopmanyNameReportsFrom=' + CopmanyNameReportsFrom + '&&CopmanyNameReportsTo=' + CopmanyNameReportsTo +
                    '&&TypeReport=' + TypeReport);

                window.open('/NewReports/PrintReportsExcel?RegistrationFrom=' + RegistrationFrom +
                    '&&RegistrationTo=' + RegistrationTo + '&&ActivationFrom=' + ActivationFrom + '&&ActivationTo=' + ActivationTo +
                    '&&CopmanyNameReportsFrom=' + CopmanyNameReportsFrom +'&&CopmanyNameReportsTo=' + CopmanyNameReportsTo + 
                    '&&TypeReport=' + TypeReport);
            }
            else {
                // Print EXCEL Only
                window.open('/NewReports/PrintReportsExcel?RegistrationFrom=' + RegistrationFrom +
                    '&&RegistrationTo=' + RegistrationTo + '&&ActivationFrom=' + ActivationFrom + '&&ActivationTo=' + ActivationTo +
                    '&&CopmanyNameReportsFrom=' + CopmanyNameReportsFrom + '&&CopmanyNameReportsTo=' + CopmanyNameReportsTo +
                    '&&TypeReport=' + TypeReport);
            }

        }
        else {
            // Print PDF as Default
            window.open('/NewReports/PrintReportsPdf?RegistrationFrom=' + RegistrationFrom +
                '&&RegistrationTo=' + RegistrationTo + '&&ActivationFrom=' + ActivationFrom + '&&ActivationTo=' + ActivationTo +
                '&&CopmanyNameReportsFrom=' + CopmanyNameReportsFrom + '&&CopmanyNameReportsTo=' + CopmanyNameReportsTo +
                '&&TypeReport=' + TypeReport);
        }

    }
    else {
        toastr.info('select date " From " and " TO " of Registration and Activation date first');
    }


}
