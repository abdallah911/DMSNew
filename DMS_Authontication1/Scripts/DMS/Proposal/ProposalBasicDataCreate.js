
$(function () {
   
});
//ب////////////////////////////////////////////////////////////////



}
//////////////////////////////////////////
function ClearSearchData() {
  

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
                    return '<a class="btn btn-warning" href="/ProposalBasicDatas/Edit/' + data + '">Edit</a>';
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
