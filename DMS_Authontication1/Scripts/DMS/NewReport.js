$(function () {

    $('#TypeReport').val();
    $('#CopmanyNameReportsFrom').select2();
    //$('#CopmanyNameReportsTo').select2();
    $('#RegistrationFrom').datepicker({});
    $('#RegistrationTo').datepicker({});
    $('#ActivationFrom').datepicker({});
    $('#ActivationTo').datepicker({});

    //$('#CompFromApproval1').select2();
    //$('#CompToApproval1').select2();

    //$('#CompFromApproval1').select2();
    //$('#CompToApproval1').select2();

});

function PrintReportOperation() {
    var CopmanyNameReportsFrom = $('#CopmanyNameReportsFrom').val();
    var CopmanyNameReportsTo = $('#CopmanyNameReportsTo').val();
    var RegistrationFrom = $('#RegistrationFrom').val();
    var RegistrationTo = $('#RegistrationTo').val();
    var ActivationFrom = $('#ActivationFrom').val();
    var ActivationTo = $('#ActivationTo').val();
    var TypeReport = $('#TypeReport').val();

    if (CopmanyNameReportsFrom != "" && CopmanyNameReportsTo != "" && TypeReport != "") {
        //chick if not search with card number
        if ($("#excel").is(":checked")) {      
                // Print EXCEL Only
            window.open('/NewReports/PrintReportsPdf?RegistrationFrom=' + RegistrationFrom +
                    '&&RegistrationTo=' + RegistrationTo + '&&ActivationFrom=' + ActivationFrom + '&&ActivationTo=' + ActivationTo +
                    '&&CopmanyNameReportsFrom=' + CopmanyNameReportsFrom + '&&CopmanyNameReportsTo=' + CopmanyNameReportsTo +
                    '&&TypeReport=' + TypeReport);
            }        
        else {
            // Print PDF as Default
            window.open('/NewReports/PrintReportsExcel?RegistrationFrom=' + RegistrationFrom +
                '&&RegistrationTo=' + RegistrationTo + '&&ActivationFrom=' + ActivationFrom + '&&ActivationTo=' + ActivationTo +
                '&&CopmanyNameReportsFrom=' + CopmanyNameReportsFrom + '&&CopmanyNameReportsTo=' + CopmanyNameReportsTo +
                '&&TypeReport=' + TypeReport);
        }
    }
    else {
        toastr.info('select " From " and " TO " of Company first and Type Report');
    }


}

function PrintReportTransaction() {
    debugger;
    var RegistrationFrom = $('#RegistrationFrom').val();
    var RegistrationTo = $('#RegistrationTo').val();
    var CopmanyNameReportsFrom = $('#CopmanyNameReportsFrom').val();
    var CopmanyNameReportsTo = $('#CopmanyNameReportsTo').val();
    var TypeReport = $('#TypeReport').val();
    var PrintAS = 2;

    if (CopmanyNameReportsFrom != "" && CopmanyNameReportsTo != "" && TypeReport != "") {
        //chick if not search with card number
        if ($("#excel").is(":checked"))
            // do something if the excel is  checked           
            PrintAS = 1;
        else
            PrintAS = 2;
        
        window.open('/NewReports/PrintTransactionReports?RegistrationFrom=' + RegistrationFrom +
            '&&RegistrationTo=' + RegistrationTo + '&&CopmanyNameReportsFrom=' + CopmanyNameReportsFrom + '&&CopmanyNameReportsTo=' + CopmanyNameReportsTo +
            '&&TypeReport=' + TypeReport + '&&PrintAS=' + PrintAS);
        }  
    else {
        toastr.info('select " From " and " TO " of Company first and Type Report');
    }


}


function PrintReportConsumApproval() {
    debugger;
    var CopmanyFrom = $('#CompFromApproval1').val();
    var CopmanyTo = $('#CompToApproval1').val();
    //var DateFrom = $('#RegistrationFrom').val();
    //var DateTo = $('#RegistrationTo').val();

    var DateFrom ="";
    var DateTo = "";

    var CardFrom = $('#CardFromApproval').val();
    var CardTo = $('#CardToApproval').val();
    var TypeReport = $('#TypeRepotApproval1').val();
    var ProvNo = $('#ProviderApproval').val();
    var ClassApproval = $('#ClassApproval').val();
    //var smal = $('#LessThanApproval').val();
    //var larg = $('#LargeThanApproval').val();
    var smal = "";
    var larg = "";
    var PrintAS = 2;


    if (TypeReport != "") {
        //chick if not search with card number
        if ($("#excelApproval1").is(":checked")) 
            // do something if the excel is  checked           
            PrintAS = 1;
        else
            PrintAS = 2;

            window.open('/NewReports/PrintReportsExcelApprovalConsm?DateFrom=' + DateFrom +
                '&&DateTo=' + DateTo + '&&CardFrom=' + CardFrom + '&&CardTo=' + CardTo +
                '&&CopmanyFrom=' + CopmanyFrom + '&&CopmanyTo=' + CopmanyTo +
                '&&TypeReport=' + TypeReport + '&&ProvNo=' + ProvNo + '&&ClassApproval=' + ClassApproval +
                '&&smal=' + smal + '&&larg=' + larg + '&&PrintAS=' + PrintAS);
            
        //else
        
        //    // Print PDF as Default
        //    window.open('/NewReports/PrintReportsExcelApprovalConsm?DateFrom=' + DateFrom +
        //        '&&DateTo=' + DateTo + '&&CardFrom=' + CardFrom + '&&CardTo=' + CardTo +
        //        '&&CopmanyFrom=' + CopmanyFrom + '&&CopmanyTo=' + CopmanyTo +
        //        '&&TypeReport=' + TypeReport + '&&ProvNo=' + ProvNo + '&&ClassApproval=' + ClassApproval +
        //        '&&smal=' + smal + + '&&PrintAS=' + 2);
        
    }
    else {
        toastr.info('Select Type Report Please');
    }


}

function PrintSummaryApproval() {
    debugger;
    var CopmanyFrom = $('#CompFromApproval2').val();
    var CopmanyTo = $('#CompToApproval2').val();
    var DateFrom = $('#RepordFromDateApproval').val();
    var DateTo = $('#RepordToDateApproval').val();
    var TypeReport = $('#TypeRepotApproval2').val();
    var val1 = $('#ValueApprovalFrom').val();
    var val2 = $('#ValueApprovalTo').val();
    
    var PrintAS = 2;


    if (TypeReport != "") {
        if ($("#excelApproval2").is(":checked"))
            PrintAS = 1;
        else
            PrintAS = 2;

        window.open('/NewReports/PrintSummaryApproval?DateFrom=' + DateFrom +
            '&&DateTo=' + DateTo + '&&CopmanyFrom=' + CopmanyFrom + '&&CopmanyTo=' + CopmanyTo +
            '&&TypeReport=' + TypeReport + '&&val1=' + val1 + '&&val2=' + val2 + '&&PrintAS=' + PrintAS);
    }
    else {
        toastr.info('Select Type Report Please');
    }


}

function PrintDetailsApproval() {
    debugger;
    var CopmanyFrom = $('#CompFromApproval2').val();
    var CopmanyTo = $('#CompToApproval2').val();
    var DateFrom = $('#RepordFromDateApproval').val();
    var DateTo = $('#RepordToDateApproval').val();
    var TypeReport = $('#TypeRepotApproval2').val();
    var val1 = $('#ValueApprovalFrom').val();
    var val2 = $('#ValueApprovalTo').val();

    var PrintAS = 2;


    if (TypeReport != "") {
        if ($("#excelApproval2").is(":checked"))
            PrintAS = 1;
        else
            PrintAS = 2;

        window.open('/NewReports/PrintDetailsApproval?DateFrom=' + DateFrom +
            '&&DateTo=' + DateTo + '&&CopmanyFrom=' + CopmanyFrom + '&&CopmanyTo=' + CopmanyTo +
            '&&TypeReport=' + TypeReport + '&&val1=' + val1 + '&&val2=' + val2 + '&&PrintAS=' + PrintAS);
    }
    else {
        toastr.info('Select Type Report Please');
    }


}


function ConsumptionPrint() {
    debugger;
    var RegistrationFrom = $('#RegistrationFromConsum').val();
    var RegistrationTo = $('#RegistrationToConsum').val();
    var ServiceFrom = $('#ServiceFrom').val();
    var ServiceTo = $('#ServiceTo').val();
    var CompanyFrom = $('#CopmanyNumberFrom').val();
    var CompanyTo = $('#CopmanyNumberTo').val();
    var CardFrom = $('#CardIdFrom').val();
    var CardTo = $('#CardIdTo').val();
    var ProviderName1 = $('#ProviderNameFrom').val();
    var ProviderName2 = $('#ProviderNameTo').val();
    var ClassConsum = $('#ClassCodeConsum').val();
    var RepotType = $('#RepotTypeConsum').val();
    var PrintAS = 2;

    if (RepotType != "") {
        if ($("#excelConsum").is(":checked"))
            PrintAS = 1;
        else
            PrintAS = 2;

        if ((RegistrationFrom != "" && RegistrationTo != "") || (ServiceFrom != "" && ServiceTo != "")) {
            window.open('/NewReports/PrintConsumptionReports?Regfrom=' + RegistrationFrom +
                '&&Regto=' + RegistrationTo + '&&Serfrom=' + ServiceFrom + '&&Serto=' + ServiceTo +
                '&&CompanyFrom=' + CompanyFrom + '&&CompanyTo=' + CompanyTo + '&&ProviderName1=' + ProviderName1 +
                '&&ProviderName2=' + ProviderName2 + '&&ClassConsum=' + ClassConsum + '&&CardFrom=' + CardFrom +
                '&&CardTo=' + CardTo + '&&RepotType=' + RepotType + '&&PrintAS=' + PrintAS);
        }
        else {
            toastr.info('select date " From " and " TO " of Registration or date of Service  first');
        }
    }
    else {
        toastr.info('select Type Report To print first');
    }

}
function ContractPrint() {
    debugger;

    var RepotType = $('#TypeRepotContract').val();
    var PrintAS = 2;

    if (RepotType != "") {
        if ($("#excelConsum").is(":checked"))
            PrintAS = 1;
        else
            PrintAS = 2;
              
        window.open('/NewReports/PrintContractReports?RepotType=' + RepotType + '&&PrintAS=' + PrintAS);
        
    }
    else {
        toastr.info('select Type Report To print first');
    }

}
function PrintingPrint() {
    debugger;

    var RegistrationFrom = $('#DateFrom').val();
    var RegistrationTo = $('#DateTo').val();
    var CompanyFrom = $('#CompFromPrint').val();
    var CompanyTo = $('#CompToPrint').val();
    var RepotType = $('#TypeRepotPrint').val();
    var PrintAS = 2;

    if (RepotType != "") {
        if ($("#excelPrint").is(":checked"))
            PrintAS = 1;
        else
            PrintAS = 2;

        window.open('/NewReports/PrintPrintingReports?datfrom=' + RegistrationFrom +
            '&&datto=' + RegistrationTo + '&&CompanyFrom=' + CompanyFrom + '&&CompanyTo=' + CompanyTo +
            '&&RepotType=' + RepotType + '&&PrintAS=' + PrintAS);

    }
    else {
        toastr.info('select Type Report To print first');
    }

}
function CustomerPrint() {
    debugger;

    var RegistrationFrom = $('#DateFromCustomer').val();
    var RegistrationTo = $('#DateToCustomer').val();
    
    var RepotType = $('#TypeRepotCustomer').val();
    var PrintAS = 2;

    if (RepotType != "") {
        if ($("#excelCustomer").is(":checked"))
            PrintAS = 1;
        else
            PrintAS = 2;

        window.open('/NewReports/PrintCustomerReports?datfrom=' + RegistrationFrom +
            '&&datto=' + RegistrationTo + '&&RepotType=' + RepotType + '&&PrintAS=' + PrintAS);
    }
    else {
        toastr.info('select Type Report To print first');
    }
}
function ConsumptionNew() {

    $('#RegistrationFromConsum').val('');
    $('#RegistrationToConsum').val('');
    $('#ServiceFrom').val('');
    $('#ServiceTo').val('');
    $('#CopmanyNumberFrom').val('');
    $('#CopmanyNumberTo').val('');
    $('#CardIdFrom').val('');
    $('#CardIdTo').val('');
    $('#ProviderNameFrom').val('');
    $('#ProviderNameTo').val('');
    $('#ClassCodeConsum').val('');
    $('#RepotTypeConsum').val('');
}
function NewApproval1() {

    $('#CompFromApproval1').val('');
    $('#CompToApproval1').val('');
    $('#CardFromApproval').val('');
    $('#CardToApproval').val('');
    $('#TypeRepotApproval1').val('');
    $('#ProviderApproval').val('');
    $('#ClassApproval').val('');
}
function NewApproval2() {

    $('#CompFromApproval2').val('');
    $('#CompToApproval2').val('');
    $('#RepordFromDateApproval').val('');
    $('#RepordToDateApproval').val('');
    $('#TypeRepotApproval2').val('');
    $('#ValueApprovalFrom').val('');
    $('#ValueApprovalTo').val('');
}
function NewPrint() {
    
    $('#DateFrom').val('');
    $('#DateTo').val('');
    $('#CompFromPrint').val('');
    $('#CompToPrint').val('');
    $('#TypeRepotPrint').val('');
}
function NewPrint() {

    $('#DateFromCustomer').val('');
    $('#DateToCustomer').val('');
    
    $('#TypeRepotCustomer').val('');
}