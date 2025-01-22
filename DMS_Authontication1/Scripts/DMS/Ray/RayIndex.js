
$(function () {
    var id;
    $('#ddlUsers').select2();
    $('#From').datepicker({ maxDate: 0 });
    $('#To').datepicker({ maxDate: 0 });
    $('#AdminFrom').datepicker({ maxDate: 0 });
    $('#AdminTo').datepicker({ maxDate: 0 });
    $('#Print').click(function () {
        var From = $('#From').val();
        var To = $('#To').val();
        if (From != "" && To != "") {
            window.open('/Rays/PrintClams?from=' + From + '&&to=' + To + '&&Branch=' + $('#ddlUsers').val() + '&&CompHoder=' + $('#comphoder').val());
        } else {
            toastr.info('select date first')
        }

    });

    IndexDatatable();
    $('#adminSearch').click(function () {
        var result = ValidSearchRequest();
        var edit = $('#edit').val();
        var deleterosita = $('#delete').val();
        var view = $('#view').val();
        var fullcontroll = $('#fullcontroll').val();
        if (result.valid) {
            $('#Index').dataTable().fnDestroy();
            $("#Index").DataTable({
                "sAjaxSource": '/Rays/PreseptionList?Provider=' + $("#Provider").val() + '&&Company=' + $("#Company").val() +
                    '&&ApprovalNo=' + $("#ApprovalNo").val() + '&&Branch=' + $("#Branch").val() + '&&CardId=' + $("#CardId").val() +
                    '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val() + '&&CompHoder=' + $('#comphoder').val(),
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
                                return '<a class="btn btn-warning" href="/Rays/Edit/' + data + '">Edit</a>';
                            }
                            else
                                return '<input  data-id=' + data + '  hidden  /> ';
                        }
                    },
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            if (view == "True" || fullcontroll == "True") {
                                return '<a class="btn btn-info" href="/Rays/Details/' + data + '">Details</a>';
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
                    url: '/Rays/PreseptionAdminCount?Provider=' + $("#Provider").val() + '&&Company=' + $("#Company").val() + '&&ApprovalNo=' +
                        $("#ApprovalNo").val() + '&&Branch=' + $("#Branch").val() + '&&CardId=' + $("#CardId").val() + '&&From=' + $("#AdminFrom").val() +
                        '&&To=' + $("#AdminTo").val() + '&&CompHoder=' + $('#comphoder').val(),
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
    $('#RaysAdminSearch').click(function () {
        var result = ValidRaysSearchRequest();
        if (result.valid) {

            $('#Index').dataTable().fnDestroy();
            $("#Index").DataTable({
                "sAjaxSource": '/Rays/PreseptionList?Provider=&&Company=&&ApprovalNo=' + $("#RaysApprovalNo").val() + '&&Branch=' + $('#ddlUsers').val() +
                    '&&CardId=' + $("#RaysCardId").val() + '&&From=' + $("#From").val() + '&&To=' + $("#To").val() + '&&CompHoder=' + $('#comphoder').val(),
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
                            return '<a class="btn btn-warning" disabled href="/Rays/Edit/' + data + '">Edit</a>';
                        }
                    },
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            return '<a class="btn btn-info"  href="/Rays/Details/' + data + '">Details</a>';
                        }
                    }
                    ,
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            return '<button type="button" disabled class="btn btn-danger" data-id=' + data + ' onclick="Delete(this);"> Delete</button>';

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

    });

});


function Delete(button) {

    var id = $(button).data('id');

    bootbox.confirm("Are you sure to delete This Prescription Form ?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/Rays/RayDelete',
                data: { id: id },
                success: function (returndata) {
                    if (returndata.ok) {
                        toastr.success("Deleted")
                        location.reload();
                    }
                }
            });
        }
    });
}
function Print(button) {
    var id = $(button).data('id');
    window.open('/Rays/ControlPenelReport?id=' + id);
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
}
function ValidRaysSearchRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    if ($("#From").val() == "") { toastr.error("Please,select From Date."); return { valid: false }; }
    if ($("#To").val() == "") { toastr.error("Please,select To date."); return { valid: false }; }
    return $.validity.end();
}
function ClearRaysSearchData() {
    $("#From").val("");
    $("#To").val("");
    $("#RaysApprovalNo").val("");
    $("#RaysCardId").val("");
    IndexDatatable();
}
function IndexDatatable() {
    $('#Index').dataTable().fnDestroy();
    $("#Index").DataTable({
        "sAjaxSource": '/Rays/PreseptionList',
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
                    return '<a class="btn btn-warning" href="/Rays/Edit/' + data + '">Edit</a>';
                }
            },
            {
                "data": "Id",
                "mRender": function (data) {
                    return '<a class="btn btn-info" href="/Rays/Details/' + data + '">Details</a>';
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