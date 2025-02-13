var input1 = document.getElementById("AddCardTxt");
input1.addEventListener("keyup", function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        $('#AddCard').click();
    }
});
$(function () {
    $('#AddCard').click(function () {
        var AddCardTxt = $('#AddCardTxt').val();
        if (AddCardTxt != "") {
            $.ajax({
                dataType: "json",
                url: '/Pharmacy/AddCard',
                data: { id: AddCardTxt },
                success: function (r) {
                    $('#ApprovalSearchCards').dataTable().fnDestroy();
                    if (r != null) {
                        var setData = $("#ApprovalSearchCards Tbody");
                        setData.empty();

                        for (var i = 0; i < r.length; i++) {
                            if (r[i].INS_START_DATE != null && r[i].INS_END_DATE != null) {
                                var MyDate_String_Value = r[i].INS_START_DATE;
                                var value = new Date
                                    (
                                        parseInt(MyDate_String_Value.replace(/(^.*\()|([+-].*$)/g, ''))
                                    );
                                var dat = value.getDate() + "/" + (value.getMonth() + 1) + "/" + value.getFullYear();
                                //end date
                                var MyDate_String_Value1 = r[i].INS_END_DATE;
                                var value1 = new Date
                                    (
                                        parseInt(MyDate_String_Value1.replace(/(^.*\()|([+-].*$)/g, ''))
                                    );
                                var dat1 = value1.getDate() + "/" + (value1.getMonth() + 1) + "/" + value1.getFullYear();
                            }
                            else {
                                dat = null;
                                dat1 = null;
                            }
                            var data = "<tr >" +
                                "<td >" + "<Button  class='btn btn-Primary glyphicon glyphicon-ok' onclick='SelectApprovalSearchCards(this);'></Button>" + "</td>" +
                                "<td>" + r[i].CARD_ID + "</td>" +
                                "<td>" + r[i].EMP_ANAME_ST + " " + r[i].EMP_ANAME_SC + " " + r[i].EMP_ANAME_TH + "</td>" +
                                "<td>" + r[i].EMP_ENAME_ST + " " + r[i].EMP_ENAME_SC + " " + r[i].EMP_ENAME_TH + "</td>" +
                                    "<td>" + dat + "</td>" +
                                    "<td>" + dat1 + "</td>" +
                                    "</tr>"
                            setData.append(data);

                        }
                        $('#ApprovalSearchCards').DataTable();
                        $('#ApprovalSearchCardsModal').modal();
                    }
                    else {
                        bootbox.alert("Inviled Card");
                    }
                },
                error: function (r) {
                    window.alert(' Too Many Data ');
                }

            });
        }
        else {
            bootbox.alert("Please insert Card ID")
        }

    });

});
function Select(button) {


    var row = $(button).closest("TR");
    var ApprovalId = $("TD", row).eq(1).html();
    ApprovalId = parseInt(ApprovalId);
    $.ajax({
        type: 'POST',
        url: '/DoctorMedicinesLabsRaysApproval/ChangeStatus',
        dataType: 'Json',
        data: {
            ApprovalId: ApprovalId,
            status: $(button).val()
        },
        dataType: 'Json',
        success: function (r) {

            if ($(button).val() != "N") {
                $("TD", row).eq(6).html($(button).val());
                toastr.success($(button).val());

            }

        },
        error: function () {
            alert("Error Retrieve");
        }
    });


}