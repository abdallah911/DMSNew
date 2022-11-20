
var input = document.getElementById("txtSearchCard");
input.addEventListener("keyup", function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {

        $('#Search').click();
    }
});
var searchtype = "unselect";
$(document).ready(function () {

    $('#RegistrationFrom').datepicker({});
    $('#RegistrationTo').datepicker({});

    $('#ServiceFrom').datepicker({});
    $('#ServiceTo').datepicker({});

    // $('#AproveList').DataTable();
    $('#list_div').hide();
    $("#Country_select").select2();
    $("#Region_select").select2();
    $("#Aprovel_Type").select2();

    $("#Country_select").change(function () {//address
        $.get("/Account/GetStateList",
            { id: $("#Country_select").val() }, function (data) {
                $("#Region_select").empty();
                $.each(data, function (index, row) {
                    $("#Region_select").append("<option value='" + row.Text + "'>" + row.Value + "</option>")
                });
            });
    });

    $('input:radio[name="Search_Type"]').change(
        function () {
            if ($(this).is(':checked')) {
                // append goes here
                if ($(this).val() == 'Class') {
                    $("#class_levels").show();
                    $("#card_div").hide();
                    searchtype = "Class";
                }
                else if ($(this).val() == 'Card') {
                    $("#card_div").show();
                    $("#class_levels").hide();
                    searchtype = "Card";

                }
            }
        });


});

//New Request
function NewRequest() {
    //  window.location = '/MedicalApproval/Search';

    $("#txtSearchCard").val("");
    $('#RegistrationFrom').val("");
    $('#RegistrationTo').val("");
    $('#txtSearchCode').val("");
    $('#list_div').hide();
    $('#AproveList').dataTable().fnDestroy();
    var setData = $("#AproveList Tbody");
    setData.empty();

    //$('#list_div').dataTable().fnDestroy();


}


// Search Aprovel
function SearchProvider() {
    $('#list_div').hide();
    //$("#AproveListBody").html("");
    var compname = $("#compname").val();
    var cardNum = $("#txtSearchCard").val();
    var RegistrationFrom = $('#RegistrationFrom').val();
    var RegistrationTo = $('#RegistrationTo').val();
    var code = $('#txtSearchCode').val();






    // chick card id  is empty
    if (cardNum == "") {
        SearchAprovel();
    }
    else {

        var arra = cardNum.split("-");

        // chick card id is correct or not
        if (arra.length == 4 && arra[3] != "") {

            // final chick if card id start with company id or not
            if (arra[0] == compname) {

                ////////
                SearchAprovel();

                ////////

            }
            else {
                toastr.info("Please enter correct card that belongs to your company only start with : " + compname);
            }

        }
        else {
            toastr.info("Please enter correct card ... ");
        }
    }

}

function SearchAprovel() {

    var compname = $("#compname").val();
    var cardNum = $("#txtSearchCard").val();
    var RegistrationFrom = $('#RegistrationFrom').val();
    var RegistrationTo = $('#RegistrationTo').val();
    var code = $('#txtSearchCode').val();
    $.ajax({
        url: '/MedicalApproval/GetAprovel/',

        data: {
            CardID: cardNum, compId: compname, code: code, datefrom: RegistrationFrom, dateto: RegistrationTo
        },
        dataType: 'Json',
        success: function (r) {
            $('#AproveList').dataTable().fnDestroy();
            if (r.msg == "ok") {
                var setData = $("#AproveList Tbody");
                setData.empty();
                for (var i = 0; i < r.AproveList.length; i++) {
                    var data = "<tr >" +
                        "<td>" + r.AproveList[i].ID + "</td>";
                    if (r.AproveList[i].TYPE == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].TYPE + "</td>";
                    if (r.AproveList[i].CARD_ID == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].CARD_ID + "</td>";
                    if (r.AproveList[i].TYP_ANAME == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].TYP_ANAME + "</td>";
                    if (r.AproveList[i].PR_ENAME == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].PR_ENAME + "</td>";
                    if (r.AproveList[i].REQ_DATE == null)
                        data += "<td>--</td>";
                    else {
                        var MyDate_String_Value = r.AproveList[i].REQ_DATE;
                        var value = new Date
                            (
                            parseFloat(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                            );

                        data += "<td>" + value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear() + "</td>";
                    }
                    if (r.AproveList[i].EMP_ENAME == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].EMP_ENAME + "</td>";
                    if (r.AproveList[i].NOTES == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].NOTES + "</td>";

                    if (r.AproveList[i].REPLAY == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].REPLAY + "</td>";
                    if (r.AproveList[i].STATE == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].STATE + "</td>";
                    if (r.AproveList[i].REQUEST_TYP == null)
                        data += "<td>--</td>";
                    else
                        data += "<td>" + r.AproveList[i].REQUEST_TYP + "</td>";

                    data += "<td><a class='btn btn-info' href= '/MedicalApproval/CreateRequest/"+ r.AproveList[i].ID + "'>Edit</a></td>";
                    "</tr>";
                    setData.append(data);
                }
                $('#AproveList').DataTable();
                $('#list_div').show();
            }
            else {
                alert(r.msg);
            }

        },
        error: function () {
            alert("Invalid Request ");
        }
    });


}

