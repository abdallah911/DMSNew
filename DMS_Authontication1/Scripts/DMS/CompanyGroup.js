$(function () {
    $('#txtSearchCompany').click(function () {
        $.ajax({
            url: '/CompanyGroup/CompanyGroupList',
            dataType: 'Json',
            success: function (r) {

                $('#CompaniesModal').modal();
                var setData = $("#Companies Tbody");
                setData.empty();

                for (var i = 0; i < r.length; i++) {

                    var data = "<tr >" +
                        "<td >" + "<button type='button' id='plus' value='Search' class='claims-search-btn ms-2 rounded-3; fs-5' style='background-color: #717382' onclick='Select(this);'><i class='fas fa-plus'></i></button>" + "</td>" +
                        //"<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='Select(this);'></Button>" + "</td>" +
                        "<td>" + r[i].C_COMP_ID + "</td>" +
                        "<td>" + r[i].C_ANAME + "</td>" +
                        "</tr>"
                    setData.append(data);

                }
                $('#Companies').DataTable();
                $('#CompaniesModal').modal('show');
            },
            error: function () {
                alert("Error Retrieve");
            }

        });
    });
    $('#Add').click(function () {
        var maxCode = 0;
        $('#Sites tbody tr').each(function () {
            var row = $(this);
            if (parseInt(row.find("TD").eq(0).html()) > maxCode) {
                maxCode = parseInt(row.find("TD").eq(0).html());
            }
        });
        var tBody = $("#Sites > TBODY")[0];
        var row = tBody.insertRow(-1);
        var cell = $(row.insertCell(-1));
        cell.html(maxCode + 1);
        cell = $(row.insertCell(-1));
        cell.html($('.modal-body #name').val());
        // remove button
        cell = $(row.insertCell(-1));
        var btnRemove = $("<a  />");
        btnRemove.attr("type", "button");
        btnRemove.addClass("btn btn-danger glyphicon glyphicon-trash");
        btnRemove.attr("onclick", "Delete(this);");
        btnRemove.text("Remove");
        cell.append(btnRemove);
        $('#exampleModal').modal("hide");
        $('.modal-body #name').val(" ");
    });
    $('#submit').click(function () {
        var Sites = new Array();
        var Num = 1;
        $("#Sites TBODY TR").each(function () {
            var row = $(this);
            var Site = {};

            Site.S_NO = Num;
            Site.C_COMP_ID = CompId;
            Site.S_ID = row.find("TD").eq(0).html();
            Site.S_NAME = row.find("TD").eq(1).html();
            Sites.push(Site);
            Num += 1;
        });
        $.ajax({
            type: 'POST',
            url: '/CompanyGroup/PostCompSites',
            dataType: 'Json',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(Sites),
            success: function (r) {
                alert("Saved");
            },
            error: function (err) {
                bootbox.alert("Error Medicien");
            }
        });
    });

})
function Delete(button) {
    //Determine the reference of the Row using the Button.
    var row = $(button).closest("TR");
    var name = $("TD", row).eq(1).html();
    bootbox.confirm("Do you want to delete: " + name, function (result) {
        if (result) {
            //Delete the Table row using it's Index.
            //---------------------------
            var row = $(button).closest("TR");
            var table = $("#Sites")[0];
            table.deleteRow(row[0].rowIndex);

        }
    });

}