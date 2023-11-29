
$(function () {
    // bootbox.alert("نحيط علم سيادتكم بان سيكون هناك بعد التحديثات ع الصفحه من الساعه الثالثه مساءا الى الساعه الرابعه مساءا ");
    var id;
    var role = $("#role").val();
    $('#ddlUsers').select2();
    //$('#From').datepicker({
    //    maxDate: 0
    //});
    //$('#To').datepicker({ maxDate: 0 });
    $('#AdminFrom').datepicker({ maxDate: 0 });
    $('#AdminTo').datepicker({ maxDate: 0 });
    $('#Print').click(function () {
        var From = $('#From').val();
        var To = $('#To').val();
        if (From != "" && To != "") {
            window.open('/Pharmacy/PrintClams?from=' + From + '&&to=' + To + '&&Branch=' + $('#ddlUsers').val());
        } else {
            toastr.info('select date first')
        }

    });
    $('#PrintExcel').click(function () {
        debugger;
        var From = $('#AdminFrom').val();
        var To = $('#AdminTo').val();
        if (From != "" && To != "") {
            window.open('/Pharmacy/PrintNewXlxClams?from=' + From + '&&to=' + To + '&&Branch=' + $('#Branch').val()
                + '&&Provider=' + $('#Provider').val() + '&&Company=' + $('#Company').val() + '&&ApprovalNo=' + $('#ApprovalNo').val()
                + '&&CardId=' + $('#CardId').val() + '&&ddlType=' + $('#ddlType').val());
            //window.open('/Pharmacy/PrintXlxClams?from=' + From + '&&to=' + To);
        } else {
            toastr.info('select date first')
        }

    });
    if (role == "1") {
        IndexDatatable();
    }
    else {
        IndexDatatableAdmin();
    }
    $('#adminSearch').click(function () {
        var result = ValidSearchRequest();
        var edit = $('#edit').val();
        var deleterosita = $('#delete').val();
        var view = $('#view').val();
        var fullcontroll = $('#fullcontroll').val();
        if (result.valid) {
            $('#Index').dataTable().fnDestroy();
            $("#Index").DataTable({
                "sAjaxSource": '/Pharmacy/PreseptionList?Provider=' + $("#Provider").val() + '&&Company=' + $("#Company").val() + '&&ApprovalNo=' + $("#ApprovalNo").val() + '&&Branch=' + $("#Branch").val() + '&&CardId=' + $("#CardId").val() + '&&Type=' + $("#ddlType").val() + '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val(),
                "bServerSide": true,
                "processing": true,
                "bFilter": true,
                "bSort": false,
                "pageLength": 10,
                "bInfo": true,
                "columns": [
                    { "data": "Oracle_Id", "name": "Oracle_Id" },
                    { "data": "CardId", "name": "Card Id" },
                    { "data": "CompanyPercent", "name": "Company Percent" },
                    { "data": "TotalValue", "name": "Total Value" },
                    { "data": "Manager", "name": "Manager" },
                    { "data": "CreatedBy", "name": "CreatedBy" },
                    {
                        "data": "CreatedDate",
                        "name": "Created Date",
                        "mRender": function (data) {
                            var value = new Date(parseFloat(data.replace(/(^.*\()|([+-].*$)/g, '')));
                            var date = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                            return date;
                        }
                    },
                    {

                        "data": "Id",
                        "mRender": function (data) {
                            if (edit == "True" || fullcontroll == "True") {
                                return '<a class="btn btn-warning" href="/Pharmacy/Edit/' + data + '">Edit</a>';
                            }
                            else
                                return '<input  data-id=' + data + '  hidden  /> ';
                        }
                    },
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            if (view == "True" || fullcontroll == "True") {
                                return '<a class="btn btn-info" href="/Pharmacy/Details/' + data + '">Details</a>';
                            }
                            else
                                return '<input  data-id=' + data + '  hidden  /> ';
                        }
                    }
                    ,
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            if (deleterosita == "True" || fullcontroll == "True") {
                                return '<button type="button" class="btn btn-danger" data-id=' + data + ' onclick="Delete(this);"> Delete</button>';

                            }
                            else
                                return '<input  data-id=' + data + '  hidden  /> ';
                        }
                    },
                    {
                        "data": "Oracle_Id",
                        "mRender": function (data) {
                            return '<button type="button" class="btn btn-default" data-id=' + data + ' onclick="Print(this);"> Print</button>';
                        }
                    }


                ]
            });
            if (fullcontroll == "True") {
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: '/Pharmacy/PreseptionAdminCount?Provider=' + $("#Provider").val() + '&&Company=' + $("#Company").val() + '&&ApprovalNo=' + $("#ApprovalNo").val() + '&&Branch=' + $("#Branch").val() + '&&CardId=' + $("#CardId").val() + '&&Type=' + $("#ddlType").val() + '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val(),
                    success: function (returndata) {
                        if (returndata.Count != 0) {
                            var setData = $("#Counts Tbody");
                            setData.empty();
                            var data = "<tr >" +
                                "<td>" + returndata.Count + "</td>" +
                                "<td>" + returndata.TotalValue.toFixed(2) + "</td>" +
                                "<td>" + returndata.CompanyPayment.toFixed(2) + "</td>" +
                                "<td>" + returndata.personpayment.toFixed(2) + "</td>" +
                                "<td>" + returndata.cash.toFixed(2) + "</td>" +
                                "<td>" + returndata.overinsurance.toFixed(2) + "</td>" +
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
        }

    });
    $('#PharmacyAdminSearch').click(function () {
        var result = ValidPharmacySearchRequest();
        if (result.valid) {
            $('#Index').dataTable().fnDestroy();
            $("#Index").DataTable({
                "sAjaxSource": '/Pharmacy/PreseptionList?Provider=&&Company=&&ApprovalNo=' + $("#PharmacyApprovalNo").val() + '&&Branch=' + $('#ddlUsers').val() + '&&CardId=' + $("#PharmacyCardId").val() + '&&Type=' + $("#PharmacyddlType").val() + '&&From=' + $("#From").val() + '&&To=' + $("#To").val(),
                "bServerSide": true,
                "processing": true,
                "bFilter": true,
                "bSort": false,
                "pageLength": 10,
                "bInfo": true,
                "columns": [
                    { "data": "Oracle_Id", "name": "Oracle_Id" },
                    { "data": "CardId", "name": "Card Id" },
                    { "data": "CompanyPercent", "name": "Company Percent" },
                    { "data": "TotalValue", "name": "Total Value" },
                    { "data": "Manager", "name": "Manager" },
                    { "data": "CreatedBy", "name": "CreatedBy" },
                    {
                        "data": "CreatedDate",
                        "name": "Created Date",
                        "mRender": function (data) {
                            var value = new Date(parseFloat(data.replace(/(^.*\()|([+-].*$)/g, '')));
                            var date = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                            return date;
                        }
                    },
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            return '<a class="edit-btn btn" disabled href="/Pharmacy/Edit/' + data + '">Edit</a>';
                        }
                    },
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            return '<a class="details-btn btn"  href="/Pharmacy/Details/' + data + '">Details</a>';
                        }
                    }
                    ,
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            return '<button type="button" disabled class="delete-btn btn" data-id=' + data + ' onclick="Delete(this);"> Delete</button>';

                        }
                    },
                    {
                        "data": "Oracle_Id",
                        "mRender": function (data) {
                            return '<button type="button" class="print-claims-btn btn" data-id=' + data + ' onclick="Print(this);"> Print</button>';
                        }
                    }


                ]
            });
        }

    });

});


function Delete(button) {

    var id = $(button).data('id');

    bootbox.confirm("Are you sure to delete This Prescription Form ?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/Pharmacy/PharmacyDelete',
                data: { id: id },
                success: function (returndata) {
                    if (returndata.ok) {
                        toastr.success("Deleted");
                        if (role == "1") {
                            location.reload();
                        }
                        else {
                            IndexDatatableAdmin();
                        }
                        //location.reload();
                    }
                }
            });
        }
    });
}
function Print(button) {
    var id = $(button).data('id');
    window.open('/Pharmacy/ControlPenelReport?id=' + id);
}

function ValidSearchRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();

    if ($("#AdminFrom").val() == "") { toastr.error("Please,select From Date."); return { valid: false }; }
    if ($("#AdminTo").val() == "") { toastr.error("Please,select To date."); return { valid: false }; }
    return $.validity.end();
}
function ClearSearchData() {
    $("#AdminFrom").val("");
    $("#AdminTo").val("");
    $("#Provider").val("");
    $("#Company").val("");
    $("#ApprovalNo").val("");
    $("#Branch").val("");
    $("#CardId").val("");
    $("#ddlType").val('').trigger("change");
}
function ValidPharmacySearchRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    if ($("#From").val() == "") { toastr.error("Please,select From Date."); return { valid: false }; }
    if ($("#To").val() == "") { toastr.error("Please,select To date."); return { valid: false }; }
    return $.validity.end();
}
function ClearPharmacySearchData() {
    $("#From").val("");
    $("#To").val("");
    $("#PharmacyApprovalNo").val("");
    $("#PharmacyCardId").val("");
    $("#PharmacyddlType").val('').trigger("change");
    IndexDatatable();
}
function IndexDatatable() {
    $('#Index').dataTable().fnDestroy();
    $("#Index").DataTable({
        "sAjaxSource": '/Pharmacy/PreseptionList',
        "bServerSide": true,
        "processing": true,
        "bFilter": true,
        "bSort": false,
        "pageLength": 10,
        "bInfo": true,
        "columns": [
            { "data": "Oracle_Id", "name": "Oracle_Id" },
            { "data": "CardId", "name": "Card Id" },
            { "data": "CompanyPercent", "name": "Company Percent" },
            { "data": "TotalValue", "name": "Total Value" },
            { "data": "Manager", "name": "Manager" },
            { "data": "CreatedBy", "name": "CreatedBy" },
            {
                "data": "CreatedDate",
                "name": "Created Date",
                "mRender": function (data) {
                    var value = new Date(parseFloat(data.replace(/(^.*\()|([+-].*$)/g, '')));
                    var date = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                    return date;
                }
            },
            {
                "data": "Id",
                "mRender": function (data) {
                    return '<a class="btn btn-warning" href="/Pharmacy/Edit/' + data + '">Edit</a>';
                }
            },
            {
                "data": "Id",
                "mRender": function (data) {
                    return '<a class="btn btn-info" href="/Pharmacy/Details/' + data + '">Details</a>';
                }
            }
            ,
            {
                "data": "Id",
                "mRender": function (data) {
                    return '<button type="button" class="btn btn-danger" data-id=' + data + ' onclick="Delete(this);"> Delete</button>';

                }
            },
            {
                "data": "Oracle_Id",
                "mRender": function (data) {
                    return '<button type="button" class="btn btn-default" data-id=' + data + ' onclick="Print(this);"> Print</button>';
                }
            }


        ]
    });
}
function IndexDatatableAdmin() {
    var result = ValidSearchRequest();
    var edit = $('#edit').val();
    var deleterosita = $('#delete').val();
    var view = $('#view').val();
    var fullcontroll = $('#fullcontroll').val();
    if (result.valid) {
        $('#Index').dataTable().fnDestroy();
        $("#Index").DataTable({
            "sAjaxSource": '/Pharmacy/PreseptionList?Provider=' + $("#Provider").val() + '&&Company=' + $("#Company").val() + '&&ApprovalNo=' + $("#ApprovalNo").val() + '&&Branch=' + $("#Branch").val() + '&&CardId=' + $("#CardId").val() + '&&Type=' + $("#ddlType").val() + '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val(),
            "bServerSide": true,
            "processing": true,
            "bFilter": true,
            "bSort": false,
            "pageLength": 10,
            "bInfo": true,
            "columns": [
                { "data": "Oracle_Id", "name": "Oracle_Id" },
                { "data": "CardId", "name": "Card Id" },
                { "data": "CompanyPercent", "name": "Company Percent" },
                { "data": "TotalValue", "name": "Total Value" },
                { "data": "Manager", "name": "Manager" },
                { "data": "CreatedBy", "name": "CreatedBy" },
                {
                    "data": "CreatedDate",
                    "name": "Created Date",
                    "mRender": function (data) {
                        var value = new Date(parseFloat(data.replace(/(^.*\()|([+-].*$)/g, '')));
                        var date = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                        return date;
                    }
                },
                {

                    "data": "Id",
                    "mRender": function (data) {
                        if (edit == "True" || fullcontroll == "True") {
                            return '<a class="btn btn-warning" href="/Pharmacy/Edit/' + data + '">Edit</a>';
                        }
                        else
                            return '<input  data-id=' + data + '  hidden  /> ';
                    }
                },
                {
                    "data": "Id",
                    "mRender": function (data) {
                        if (view == "True" || fullcontroll == "True") {
                            return '<a class="btn btn-info" href="/Pharmacy/Details/' + data + '">Details</a>';
                        }
                        else
                            return '<input  data-id=' + data + '  hidden  /> ';
                    }
                }
                ,
                {
                    "data": "Id",
                    "mRender": function (data) {
                        if (deleterosita == "True" || fullcontroll == "True") {
                            return '<button type="button" class="btn btn-danger" data-id=' + data + ' onclick="Delete(this);"> Delete</button>';

                        }
                        else
                            return '<input  data-id=' + data + '  hidden  /> ';
                    }
                },
                {
                    "data": "Oracle_Id",
                    "mRender": function (data) {
                        return '<button type="button" class="btn btn-default" data-id=' + data + ' onclick="Print(this);"> Print</button>';
                    }
                }


            ]
        });
        if (fullcontroll == "True") {
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/Pharmacy/PreseptionAdminCount?Provider=' + $("#Provider").val() + '&&Company=' + $("#Company").val() + '&&ApprovalNo=' + $("#ApprovalNo").val() + '&&Branch=' + $("#Branch").val() + '&&CardId=' + $("#CardId").val() + '&&Type=' + $("#ddlType").val() + '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val(),
                success: function (returndata) {
                    if (returndata.Count != 0) {
                        var setData = $("#Counts Tbody");
                        setData.empty();
                        var data = "<tr >" +
                            "<td>" + returndata.Count + "</td>" +
                            "<td>" + returndata.TotalValue.toFixed(2) + "</td>" +
                            "<td>" + returndata.CompanyPayment.toFixed(2) + "</td>" +
                            "<td>" + returndata.personpayment.toFixed(2) + "</td>" +
                            "<td>" + returndata.cash.toFixed(2) + "</td>" +
                            "<td>" + returndata.overinsurance.toFixed(2) + "</td>" +
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
    }
    else {
        $('#Index').dataTable().fnDestroy();
        $("#Index").DataTable({
            "sAjaxSource": '/Pharmacy/PreseptionList',
            "bServerSide": true,
            "processing": true,
            "bFilter": true,
            "bSort": false,
            "pageLength": 10,
            "bInfo": true,
            "columns": [
                { "data": "Oracle_Id", "name": "Oracle_Id" },
                { "data": "CardId", "name": "Card Id" },
                { "data": "CompanyPercent", "name": "Company Percent" },
                { "data": "TotalValue", "name": "Total Value" },
                { "data": "Manager", "name": "Manager" },
                { "data": "CreatedBy", "name": "CreatedBy" },
                {
                    "data": "CreatedDate",
                    "name": "Created Date",
                    "mRender": function (data) {
                        var value = new Date(parseFloat(data.replace(/(^.*\()|([+-].*$)/g, '')));
                        var date = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                        return date;
                    }
                },
                {
                    "data": "Id",
                    "mRender": function (data) {
                        return '<a class="btn btn-warning" href="/Pharmacy/Edit/' + data + '">Edit</a>';
                    }
                },
                {
                    "data": "Id",
                    "mRender": function (data) {
                        return '<a class="btn btn-info" href="/Pharmacy/Details/' + data + '">Details</a>';
                    }
                }
                ,
                {
                    "data": "Id",
                    "mRender": function (data) {
                        return '<button type="button" class="btn btn-danger" data-id=' + data + ' onclick="Delete(this);"> Delete</button>';

                    }
                },
                {
                    "data": "Oracle_Id",
                    "mRender": function (data) {
                        return '<button type="button" class="btn btn-default" data-id=' + data + ' onclick="Print(this);"> Print</button>';
                    }
                }


            ]
        });
    }
}