
$(function () {
    // bootbox.alert("نحيط علم سيادتكم بان سيكون هناك بعد التحديثات ع الصفحه من الساعه الثالثه مساءا الى الساعه الرابعه مساءا ");
    var id;
    $('#ddlUsers').select2();

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
    //IndexDatatable();
    $('#adminSearch').click(function () {
        var result = ValidSearchRequest();
        var edit = $('#edit').val();
        var deleterosita = $('#delete').val();
        var view = $('#view').val();
        var fullcontroll = $('#fullcontroll').val();
        if (result.valid) {
            $('#Index').dataTable().fnDestroy();
            $("#Index").DataTable({
                "sAjaxSource": '/Pharmacy/StopEditPreseptionList?Provider=' + $("#Provider").val() + '&&Company=' + $("#Company").val() + '&&ApprovalNo=' + $("#ApprovalNo").val() + '&&Branch=' + $("#Branch").val() + '&&CardId=' + $("#CardId").val() + '&&Type=' + $("#ddlType").val() + '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val(),
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
                    { "data": "UpdatedBy", "name": "UpdatedBy" },
                    {
                        "data": "UpdatedDate",
                        "name": "Updated Date",
                        "mRender": function (data) {
                            var value = new Date(parseFloat(data.replace(/(^.*\()|([+-].*$)/g, '')));
                            var date = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                            return date;
                        }
                    },
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            if (view == "True" || fullcontroll == "True") {
                                return '<a class="text-light btn" style="background-color: #717382" href="/Pharmacy/Details/' + data + '">Details</a>';
                            }
                            else
                                return '<input  data-id=' + data + '  hidden  /> ';
                        }
                    }
                    ,
                    {
                        "data": "Oracle_Id",
                        "mRender": function (data) {
                            return '<button type="button" class="text-light btn" style="background-color: #717382" data-id=' + data + ' onclick="Print(this);"> Print</button>';
                        }
                    }


                ]
            });
        }

    });

});


function Print(button) {
    var id = $(button).data('id');
    window.open('/Pharmacy/PrintDeleteEditRoshita?id=' + id);
}

function ValidSearchRequest() {
    //$.validity.setup({ outputMode: 'label' });
    //$.validity.start();

    if ($("#AdminFrom").val() == "") { toastr.error("Please,select From Date."); return { valid: false }; }
    if ($("#AdminTo").val() == "") { toastr.error("Please,select To date."); return { valid: false }; }
    return  { valid: true };
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
function IndexDatatable() {
    $('#Index').dataTable().fnDestroy();
    $("#Index").DataTable({
        "sAjaxSource": '/Pharmacy/StopEditPreseptionList',
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
            { "data": "UpdatedBy", "name": "UpdatedBy" },
            {
                "data": "UpdatedDate",
                "name": "Updated Date",
                "mRender": function (data) {
                    var value = new Date(parseFloat(data.replace(/(^.*\()|([+-].*$)/g, '')));
                    var date = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                    return date;
                }
            },
            {
                "data": "Id",
                "mRender": function (data) {
                    return '<a class="text-light btn" style="background-color: #717382" href="/Pharmacy/Details/' + data + '">Details</a>';
                }
            },
            {
                "data": "Oracle_Id",
                "mRender": function (data) {
                    return '<button type="button" class="text-light btn" style="background-color: #717382" data-id=' + data + ' onclick="Print(this);"> Print</button>';
                }
            }


        ]
    });
}