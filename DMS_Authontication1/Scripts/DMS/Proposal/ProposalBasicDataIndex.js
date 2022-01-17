
$(function () {
    var id;
    $('#ddlUsers').select2();
    $('#From').datepicker({
        maxDate: 0

    });
    $('#To').datepicker({ maxDate: 0 });
    $('#AdminFrom').datepicker({ maxDate: 0 });
    $('#AdminTo').datepicker({ maxDate: 0 });
    //status = l.status,
    //    Id = l.Id,

    //    TypeProposal = l.TypeProposal,
    //    CompName = l.CompName,
    //    CompId = l.CompId,
    //    ContractNo = l.ContractNo,
    //    NumProposal = l.NumProposal,
    //    CreatedDate = l.CreatedDate,
    //    CreatedBy = l.CreatedBy

    IndexDatatable();
    // $('#adminSearch').click(adminSearchdd());
   
});
//ب////////////////////////////////////////////////////////////////
 function adminSearch() {
        debugger;
       // var result = ValidSearchRequest();
     // if (result.valid) {
     if (true) {
            $('#Index').dataTable().fnDestroy();
            $("#Index").DataTable({
                //public JsonResult PreseptionAdminCount( string CompanyName = "", string CopmanyId = "", string From = "", string To = "", string ddlType = "", string ProposalId = "")

                "sAjaxSource": '/ProposalBasicDatas/PreseptionList?CompanyName=' + $("#CompanyName").val() + '&&CopmanyId=' + $("#CopmanyId").val() + '&&ddlType=' + $("#ddlType").val() + '&&ProposalId=' + $("#ProposalId").val() + '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val(),
                "bServerSide": true,
                "processing": true,
                "bFilter": true,
                "bSort": false,
                "pageLength": 10,
                "bInfo": true,
                "columns": [
                    { "data": "Id", "name": "Id" },
                    { "data": "status", "name": "status" },
                    { "data": "TypeProposal", "name": "Type Proposal" },
                    { "data": "CompName", "name": "Company Name" },
                    { "data": "CompId", "name": "Company Id" },
                    { "data": "ContractNo", "name": "Contract No" },
                    { "data": "NumProposal", "name": "Proposal No" },
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
                            //<a class="btn btn-warning" href="/ProposalBasicDatas/Edit/' + data + '">Edit</a>
                            return '';
                        }
                    },
                    {
                        "data": "Id",
                        "mRender": function (data) {
                            return '<a class="btn btn-info" href="/ProposalBasicDatas/Details/' + data + '">Details</a>';
                        }
                    }



                ]
            });
            $.ajax({
                type: "POST",
                dataType: "json",
                url: '/ProposalBasicDatas/PreseptionAdminCount?CompanyName=' + $("#CompanyName").val() + '&&CopmanyId=' + $("#CopmanyId").val() + '&&ddlType=' + $("#ddlType").val() + '&&ProposalId=' + $("#ProposalId").val() + '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val(),
                success: function (returndata) {
                    if (returndata.Count != 0) {
                        var setData = $("#Counts Tbody");
                        setData.empty();
                        var data = "<tr >" +
                            "<td>" + returndata.Count + "</td>" +
                            "<td>" + returndata.TotalPremium.toFixed(2) + "</td>" +
                            "<td>" + returndata.TotalChronic.toFixed(2) + "</td>" +
                            "<td>" + returndata.NumChronic + "</td>" +

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

                            "</tr>"
                        setData.append(data);
                    }

                }
            });


        }


    }

function ValidSearchRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();

    if ($("#AdminFrom").val() == "") { toastr.error("Please,select From Date."); return { valid: false }; }
    if ($("#AdminTo").val() == "") { toastr.error("Please,select To date."); return { valid: false }; }
    return $.validity.end();
}
//////////////////////////////////////////
function ClearSearchData() {
    // "sAjaxSource": '/ProposalBasicDatas/PreseptionList?CompanyName=' + $("#CompanyName").val() 
    //+ '&&CopmanyId=' + $("#CopmanyId").val() + '&&ddlType=' + $("#ddlType").val() +
    //'&&ProposalId=' + $("#ProposalId").val() + '&&From=' + $("#AdminFrom").val() + '&&To=' + $("#AdminTo").val(),

    $("#AdminFrom").val("");
    $("#AdminTo").val("");
    $("#CompanyName").val("");
    $("#CopmanyId").val("");
    $("#ProposalId").val("");
    $("#ddlType").val('').trigger("change");
}
//////////////////////////////////////////
function ValidPharmacySearchRequest() {
    $.validity.setup({ outputMode: 'label' });
    $.validity.start();
    if ($("#From").val() == "") { toastr.error("Please,select From Date."); return { valid: false }; }
    if ($("#To").val() == "") { toastr.error("Please,select To date."); return { valid: false }; }
    return $.validity.end();
}

//////////////////////////////////////////
function IndexDatatable() {
    $('#Index').dataTable().fnDestroy();
    $("#Index").DataTable({
        "sAjaxSource": '/ProposalBasicDatas/PreseptionList',
        "bServerSide": true,
        "processing": true,
        "bFilter": true,
        "bSort": false,
        "pageLength": 10,
        "bInfo": true,
        "columns": [
            { "data": "Id", "name": "Id" },
            { "data": "status", "name": "status" },
            { "data": "TypeProposal", "name": "Type Proposal" },
            { "data": "CompName", "name": "Company Name" },
            { "data": "CompId", "name": "Company Id" },
            { "data": "ContractNo", "name": "Contract No" },
            { "data": "NumProposal", "name": "Proposal No" },
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
                    //<a class="btn btn-warning" href="/ProposalBasicDatas/Edit/' + data + '">Edit</a>
                    return '';
                }
            },
            {
                "data": "Id",
                "mRender": function (data) {
                    return '<a class="btn btn-info" href="/ProposalBasicDatas/Details/' + data + '">Details</a>';
                }
            }



        ]
    });
}
