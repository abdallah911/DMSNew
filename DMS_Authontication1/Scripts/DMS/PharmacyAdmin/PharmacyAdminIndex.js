var url = window.location.href;
var id = url.substring(url.lastIndexOf('/') + 1);
$('#id').val(id);

$(function () {
    $("#ddlUsers").select2();
    $("#ddlSpeciality").change(function () {
        $("#wait").css("display", "block");
        $.ajax({
            type: 'POST',
            url: '/Pharmacy/getDiag/',
            dataType: 'json',
            data: { id: $("#ddlSpeciality").val() },
            success: function (r) {
                $('#ddlDiagnoises').empty();
                var result = [];
                for (var i = 0; i < r.length; i++) {
                    var current = {};
                    current.id = r[i].Code;
                    current.text = r[i].Name;
                    result.push(current);
                }
                $('#ddlDiagnoises').select2({
                    data: result
                })
                $("#wait").css("display", "none");

            },
            error: function (ex) {
                bootbox.alert('Failed to retrieve Diagnoses , please check your internet connection');
                $("#wait").css("display", "none");

            }

        });
    });
    $("#ddlSpeciality").map(function () {
        $.ajax({
            type: 'POST',
            url: '/Pharmacy/getDiag/',
            dataType: 'json',
            data: { id: $("#ddlSpeciality").val() },

            success: function (r) {
                $('#ddlDiagnoises').empty();
                var result = [];
                for (var i = 0; i < r.length; i++) {
                    var current = {};
                    current.id = r[i].Code;
                    current.text = r[i].Name;
                    result.push(current);
                }
                $('#ddlDiagnoises').select2({
                    data: result
                })

                $("#wait").css("display", "none");

            },
            error: function (ex) {
                bootbox.alert('Failed to retrieve Diagnoses, please check your internet connection');
            }

        });
    });
    $("#ddlSpeciality").select2();

    $("#ddlType").select2();
    //Mediciences
    $('#submit').click(function () {
        debugger;
        var SelectedDiagnosisList = $('#ddlDiagnoises').select2('data');
        var DiagnosisList = [];
        for (var i = 0; i < SelectedDiagnosisList.length; i++) {
            //var current = {};
            //current.DIAG_ANAME = SelectedDiagnosisList[i].text;
            DiagnosisList.push(SelectedDiagnosisList[i].text);
        }
        /* data: JSON.stringify(DiagnosisList),*/
        $.ajax({
            type: 'POST',
            url: '/Pharmacy/SaveDiagnoisesAdmin/',
            dataType: 'Json',
            data: {
                DiagnosisList: DiagnosisList,
                Speciality: $('#ddlSpeciality option:selected').text(),
                Roshitaid: $('#id').val(),
            },
            success: function (r) {
                bootbox.alert("Data Saved Susseccfuly ");
            },
            error: function (err) {
                bootbox.alert("Error saving roshita,please check your internet connection");
                $("#submit").attr("disabled", false);
            }
        });
    });

})
