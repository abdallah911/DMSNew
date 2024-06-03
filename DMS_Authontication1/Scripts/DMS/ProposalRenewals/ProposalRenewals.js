
$(document).ready(function () {
     
    $("select2").select2();
});
 
$(function () {
    $('#CompId').select2();

});
function ClearAll() {

    $('#CompId').val('');
    $('#ContractNo').val('');

    $('#ClassCount').val('');
    $('#EmpCount').val('');

}

function ShowBroker() {
    if ($("#brokerCheck").is(":checked")) {
        $('#brockDisable').show();
    }
    else {
        $('#brockDisable').hide();
    }
}
function SearchRenewal() {
    if ($('#CompId').val() != "" && !isNaN($('#CompId').val())) {
        $("#wait").css("display", "block");
        $.ajax({
            type: "POST",
            dataType: "json",
            url: '/ProposalRenewals/getInformation',
            data: { CompId: $('#CompId').val() },
            success: function (data) {
                $("#wait").css("display", "none");

                //debugger;
                $('#ContractNo').val(data.maxContract);
                $('#ClassCount').val(data.countClass);
                $('#EmpCount').val(data.countEmp);


                //$('#ContractNoDisply').val(data.maxContract);
                //$('#EmployeeCount').val(data.countClass);
                //$('#CountClass').val(data.countEmp);

            }
        });
    }
    else
        toastr.error("من فضلك إختر رقم الشركة أولا");  
}

function NextScreen() {
    debugger;
    //// Replace 'yourFormId' with the actual ID of your form
    //var form = document.getElementById('RenewalMainForm');
    //sessionStorage.setItem('formStepOneSubmitted', 'true');
    //// Submit the form
    //form.submit();

    window.location.href = '/ProposalRenewals/ProposalStepTwoCreate?countCat=' + $('#ClassCount').val();
}
function PrintRenewal(id, countClas) {
    debugger;
    window.open('/ProposalRenewals/PrintRenewalReports?id=' + id +
    '&&countClass=' + countClas);
}
  

