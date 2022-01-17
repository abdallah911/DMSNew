
$(function () {
    // bootbox.alert("نحيط علم سيادتكم بان سيكون هناك بعد التحديثات ع الصفحه من الساعه الثالثه مساءا الى الساعه الرابعه مساءا ");
    //var id;
    $('#ddlCompanies').select2();
    $('#ddlGroups').select2({
        placeholder:"Select Group"
    });
    $('#Date').datepicker({
        minDate: 0,
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'MM-yy',
        onClose: function (dateText, inst) {
            debugger;
            $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth,1))
        }
    });
    $("#ddlCompanies").change(function () {
          
            $("#ddlGroups").empty();
            if ($("#ddlCompanies").val() != "") {
                //provider
                $.ajax({
                    type: 'POST',
                    url: '/Pharmacy/CompanyGroupsList/',
                    dataType: 'json',
                    data: { CompId: $(this).val() },
                    success: function (Groups) {
                        $("#ddlGroups").append('<option value="0">All</option>');
                        $.each(Groups, function (i, ddlGroups) {
                            $("#ddlGroups").append('<option value="' + ddlGroups.Value + '">' +ddlGroups.Text + '</option>');

                        });
                        $('#ddlGroups  option:eq(0)').attr('selected', 'selected');
                    },
                    error: function (ex) {
                        alert('Failed to retrieve Groups.');
                    }

                });
            }
           
            

    });
    $('#Print').click(function () {
        var result = ValidSearchRequest();
        if (result.valid) {
            window.open('/Pharmacy/CompaniesChronicDeliveryReport?Date=' + $('#Date').val() + '&&CompId=' + $("#ddlCompanies").val() + '&&GroupId=' + $('#ddlGroups').val() + '&&CardId=' + $('#CardId').val());
        }

    });

    
    $('#Search').click(function () {
        var result = ValidSearchRequest();
        if (result.valid) {
            IndexDatatable();
        }

    });

});


function ValidSearchRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    //$("#ddlCompanies").require();
    if ($("#ddlCompanies").val() == "") { toastr.error("Please,select Company."); return { valid: false }; }
    if ($("#Date").val() == "") { toastr.error("Please,select From Date."); return { valid: false }; }
    return $.validity.end();
}
function ClearSearchData() {
    $("#Date").val("");
 
    $("#CardId").val("");
    //$("#ddlDiagnoises").val(null).change();
    $("#ddlCompanies").val('').trigger("change");
    $("#ddlGroups").val('').trigger("change");
}
function IndexDatatable() {
    $('#Index').dataTable().fnDestroy();
    $("#Index").DataTable({
        "sAjaxSource": '/Pharmacy/CompaniesChronicMedicinesList?Date=' + $('#Date').val() + '&&CompId=' + $("#ddlCompanies").val() + '&&GroupId=' + $('#ddlGroups').val() + '&&CardId=' + $('#CardId').val(),
        //"sAjaxSource": '/Pharmacy/CompaniesChronicMedicinesList',
        "bServerSide": true,
        "processing": true,
        "bFilter": true,
        "bSort": false,
        "pageLength": 10,
        "bInfo": true,
        "columns": [
            { "data": "CardId" },
            { "data": "CompanyGroup" },
            { "data": "MedicienCode" },
            { "data": "Name" },
            { "data": "Dosage" },
            { "data": "PackageSize" },
            { "data": "PackagePrice" },
            { "data": "UnitNumber" },
            { "data": "UnitPrice" },
            { "data": "Dose" },
            { "data": "DoseDuration" },
            { "data": "MedicineDuration" },
            { "data": "TotalUnits" },
            { "data": "Amount" },
            //{ "data": "MedicineType" },
            {       
                "data": "Month",
                "mRender": function (data) {
                    if (data == null) {
                        return ""
                    }
                    var value = new Date(parseFloat(data.replace(/(^.*\()|([+-].*$)/g, '')));
                    var date =  (value.getMonth() + 1) + "/" + value.getFullYear();
                    return date;
                }
            },
            { "data": "Act" }
        ]
    });

    $.ajax({
        type: "POST",
        dataType: "json",
        url:'/Pharmacy/CompaniesChronicMedicinesListCount?Date=' + $('#Date').val() + '&&CompId=' + $("#ddlCompanies").val() + '&&GroupId=' + $('#ddlGroups').val() + '&&CardId=' + $('#CardId').val(),
        success: function (returndata) {
            if (returndata.Count != 0) {
                var setData = $("#Counts Tbody");
                setData.empty();
                debugger;
                var data = "<tr >" +
                    "<td>" + returndata.TotalCounts	+ "</td>" +
                    "<td>" + returndata.TotalCards + "</td>" +
                    "<td>" + returndata.TotalValue.toFixed(2) + "</td>" 
                   
                    "</tr>"
                setData.append(data);

            } else {
                var setData = $("#Counts Tbody");
                setData.empty();
                var data = "<tr >" +
                    "<td>0</td>" +
                    "<td>0</td>" +
                    "<td>0</td>" +
                    "<td>0</td>" +
                    "<td>0</td>" +
                    "<td>0</td>" +
                    "</tr>"
                setData.append(data);
            }

        }
    });
}