$(function () {
    $('#CompNo').select2();


    $('#userName').select2();
    $('#AddMedicine').select2();
    $('#CardNo').select2();

    $('#RegistrationFrom').datepicker({});
    $('#RegistrationTo').datepicker({});

    $('#RegistrationFromReportProviderCheck').datepicker({});
    $('#RegistrationToReportProviderCheck').datepicker({});

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

    $('#CardNoMed').select2({
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


    $('#CompNoPrint').select2();

/*    $('#ContractNo').select2();*/

    $("#CompNoPrint").change(function () {//address
        if ($("#CompNoPrint").val() != "" && $("#CompNoPrint").val() != "0") {
            $.get("/ReportMain/GetContractNo",
                { CompId: $("#CompNoPrint").val() }, function (data) {
                    $("#ContractNo").empty();
                    $('#ContractNo').append('<option value="">Select Contract</option>');
                    $.each(data, function (index, row) {
                        $("#ContractNo").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
                    });
                });
        }
        else {
            $("#ContractNo").empty();
            $('#ContractNo').append('<option value="">Select Region</option>');
        }
    });

    $('#CopmanyNumberReports').select2();
    $('#RegistrationFromReports').datepicker({});
    $('#RegistrationToReports').datepicker({});

    
    $("#RepotTypeReports").change(function () {//address
        var div = document.getElementById("RecordDateScreen");
        if ($("#RepotTypeReports").val() > 4) {
            div.style.display = "block";
        }
        else {
            div.style.display = "none";
        }
    });



  
    $('#CompNoMed').select2();
  

    $('#RegistrationReviewFrom').datepicker({});
    $('#RegistrationReviewTo').datepicker({});
    $('#ProviderNo').select2({
        placeholder: 'Search for a Provider ',
        minimumInputLength: 2,
        ajax: {
            url: '/ReportMain/GetProvider/',
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

function SearchRenewal() {
    debugger;
    if ($('#CompNo').val() != "") {
        $("#wait").css("display", "block");
        $.ajax({
            type: "POST",
            dataType: "json",
            url: '/ProposalRenewals/getInformation',
            data: { CompId: $('#CompNo').val() },
            success: function (data) {
                $("#wait").css("display", "none");

                //debugger;
                $('#ContractNo').val(data.maxContract);
                $('#ClassCount').val(data.countClass);
                $('#EmployeeCount').val(data.countEmp);

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
