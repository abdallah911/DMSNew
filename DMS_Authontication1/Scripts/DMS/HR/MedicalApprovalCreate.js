
var sel;
var SelectedProviderEname = "";
$(document).ready(function () {
    var type = $("#TYPE").val();
    if (type != '') {
        $("#TYPE").val(type).change();
    }

    $("#CARD_ID").select2();
    $("#TYP_ANAME").select2();
    $("#PR_ENAME").select2();
    $("#ProviderType").select2();
    $("#TYP_ANAME").change(function () {//address
        $.get("/MedicalApproval/GetAproveder",
            { id: $("#TYP_ANAME").val() }, function (data) {

                $("#PR_ENAME").empty();
                $("#PR_ENAME").append("<option value='0'>اختار مقدم الخدمة</option>");

                for (var i = 0; i < data.providerslist.length; i++) {
                    $("#PR_ENAME").append("<option value='" + data.providerslist[i].Value + "'>" + data.providerslist[i].Text + "</option>");
                };
                //  $('#Region').val(sel).change();
                if (SelectedProviderEname != "") {
                    $("#PR_ENAME").val(SelectedProviderEname).change();

                    SelectedProviderEname = "";
                }
            });

    });


});

function DownloadImage() {
    window.open('/MedicalApproval/Download/' + $('#RequestCode').html());
}
/// Save Request
function SaveRequest() {
    if ($('#CompName').val() == "") {
        alert("You shoud enter valid company name ..");
    }
    else if ($("#IMAGE").val() == "") {
        alert("You shoud enter IMAGE ..");
    }
    else if ($("#CARD_ID").val() == "") {
        alert("You shoud enter CARD_ID ..");
    }
    else if ($('#TYPE').val() == "") {
        alert("You shoud enter TYPE  ...");
    }

    else {

        var imagname = "";
        if ($('#IMAGE').val() != "") {

            var datag = new FormData();//()
            var files = $("#IMAGE").get(0).files;
            imagname = files[0].name;
            datag.append("File", files[0]);

            //Saving Image
            $.ajax({
                url: '/MedicalApproval/SaveImage/',
                type: "POST",
                processData: false,
                contentType: false,
                data: datag,
                success: function (response) {

                    //  $('#imName').val(response);
                },
                error: function (er) {
                    alert("Upload Image");
                }

            });
        }
        data = {
            CompName: $('#CompName').val(),
            CARD_ID: $('#CARD_ID').val(),
            TYPE: $('#TYPE').val(),
            NOTES: $('#NOTES').val(),
            TYP_ANAME: $("#TYP_ANAME").val(),
            PR_ENAME: $('#PR_ENAME').val(),
            APPROVAL_IMAGE: imagname,

        };
        $.ajax({
            type: "POST",
            url: '/MedicalApproval/SaveRequest',
            //processData: false,
            //contentType: false,
            data: data,
            success: function (ret) {
                if (ret.msg == "ok") {
                    $("#save").attr("disabled", "disabled");
                    bootbox.alert("Request code : " + ret.respcode);
                    $("#RequestCode").html(ret.respcode);
                }
                else {
                    alert(ret.msg);
                }

            },
            error: function (err) {
                bootbox.alert("Error DATA !");
            }
        });
    }

}

// New Request
function NewRequest() {
    window.location = '/MedicalApproval/CreateRequest';
    //$('#CompName').val();
    //$("#save").attr("disabled", false);
    //$('#Country').val("");
    //$('#Region').empty();
    //$('#ProviderType').val("");
    //$("#NumberOfPeople").val("");
    //$('#ServName').val("");
    //$("#ServAddress").val("");
    //$("#ResponsableFor").val("");
    //$("#PhoneNumber").val("");

    //  $("#save").attr("disabled", false);
    //  $("#edit").attr("disabled", "disabled");

    //  $("#RequestCode").html(("********"));
    //  //  $('#CompName').val(dat.modelreturn.CompName);
    //  $('#TYPE').val("Medications outpatient").change();
    ////  $('#CARD_ID').val(0).change();
    ////  $('#TYP_ANAME').val("0").change();
    //  $('#PR_ENAME').val("0").change();
    //  $("#NOTES").val("");
    //  $("#responsestatus").val("");
    //  debugger;
    //  //  $("#IMAGE").val(dat.modelreturn.IMAGE);
    //  var x = document.getElementById("divimage");

    //  x.style.display = "none";




}

// Search Request
function SearchRequest() {
    if ($('#txtSearchRequest').val() == "") {
        alert("Please insert valid Request Code ...")

    }
    else {
        var data = {
            searchId: $('#txtSearchRequest').val(),
        };
        $.ajax({
            type: "POST",
            url: '/MedicalApproval/SearchRequest',
            data: (data),
            success: function (dat) {
                if (dat.msg == "ok") {


                    $("#save").attr("disabled", "disabled");
                    $("#edit").attr("disabled", false);
                    sel = dat.modelreturn.Region;
                    $("#RequestCode").html(dat.modelreturn.ID);
                    $('#CompName').val(dat.modelreturn.CompName);
                    $('#TYPE').val(dat.modelreturn.TYPE);
                    debugger;
                    SelectedProviderEname = dat.modelreturn.PR_ENAME;
                    $('#TYP_ANAME').val(dat.modelreturn.TYP_ANAME).change();
                    $('#PR_ENAME').val(dat.modelreturn.PR_ENAME).change();
                    $("#CARD_ID").val(dat.modelreturn.CARD_ID).change();
                    $("#NOTES").val(dat.modelreturn.NOTES);
                    $("#responsestatus").val(dat.modelreturn.REPLAYED_BY);

                    //  $("#IMAGE").val(dat.modelreturn.IMAGE);
                    var x = document.getElementById("divimage");
                    if (dat.modelreturn.IMAGE != "") {
                        x.style.display = "block";
                    }
                    else {
                        x.style.display = "none";
                    }
                    debugger;


                }
                else {
                    alert(dat.msg);
                }

            },
            error: function (err) {
                bootbox.alert("Error Data !");
            }
        });
    }
}

// Edit Request
function EditRequest() {
    var x = document.getElementById("divimage");
    debugger;
    if ($('#CompName').val() == "") {
        alert("You shoud enter valid company name ..");
    }
    else if ($("#CARD_ID").val() == "") {
        alert("You shoud enter CARD_ID ..");
    }
    else if ($("#IMAGE").val() == "" && x.style.display == "none") {
        alert("You shoud enter IMAGE ..");
    }
    else if ($('#TYPE').val() == "") {
        alert("You shoud enter TYPE  ...");
    }

    else {

        //var data = {
        //    IMAGE: $('#IMAGE').val()

        //};
        debugger;
        var imagname = "";
        if ($('#IMAGE').val() != "") {

            var datag = new FormData();//()
            var files = $("#IMAGE").get(0).files;
            imagname = files[0].name;
            datag.append("File", files[0]);

            //for (var i = 0; i < files.length; i++) {
            //    data.append("Files", files[i]);
            //    indemnty.AttachPDF += files[i].name ;
            //    if (i < files.length-1) {
            //        indemnty.AttachPDF += ",";
            //    }
            //}

            //Saving Image
            $.ajax({
                url: '/MedicalApproval/SaveImage/',
                type: "POST",
                processData: false,
                contentType: false,
                data: datag,
                success: function (response) {

                    //  $('#imName').val(response);
                },
                error: function (er) {
                    alert("Upload Image");
                }

            });
        }

        //var fileName;
        //$('input[type="file"]').change(function (e) {
        //     fileName = e.target.files[0].name;
        //    //alert('The file "' + fileName + '" has been selected.');
        //});

        //if (data.IMAGE != String.empty)
        //{
        //    var files = $("#IMAGE").get(0).files;
        //    imagname = files[0].name;
        //}
        debugger;
        data = {

            ID: $('#RequestCode').html(),
            CompName: $('#CompName').val(),
            CARD_ID: $('#CARD_ID').val(),
            TYPE: $('#TYPE').val(),
            NOTES: $('#NOTES').val(),
            TYP_ANAME: $("#TYP_ANAME").val(),
            PR_ENAME: $('#PR_ENAME').val(),
            IMAGE: imagname,

        };
        $.ajax({
            type: "POST",
            url: '/MedicalApproval/EditRequest',
            //processData: false,
            //contentType: false,
            data: data,
            success: function (ret) {
                if (ret.msg == "ok") {
                    $("#save").attr("disabled", "disabled");
                    bootbox.alert("Saved Datat Success ...");
                    $("#RequestCode").html(ret.respcode);
                }
                else {
                    alert(ret.msg);
                }

            },
            error: function (err) {
                bootbox.alert("Error DATA !");
            }
        });
    }
}
