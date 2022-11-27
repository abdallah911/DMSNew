var interval;
var isStopTimeClick = 0;
var dateOfStartMeeting = $("#StartMeeting").val();
var dateOfEndMeeting = $("#EndMeeting").val();
var duration = $("#MeetingTime").val();
var visitedate = $("#vdate").val();
if (duration != "") {
    isStopTimeClick = 3;
}
$(document).ready(function () {
    $("#Help").click(function () {
        introJs().start();
    });

});
$(function () {
    debugger;
    $('.CreatDate').datetimepicker({
        minDate: new Date()
    });
    $('#FeedBackText').select2();
    if (/*"@Html.Raw(Model.HasFeedBack)"*/ $("#HasFedback").val() == "True") {
        var feedList = /*"@Html.Raw(Model.FeedBackText)"*/$("#fedback").val().split("-");
        $("#FeedBackText").val(feedList).change();
    }
    //$('#Department').select2();
    //$('#CompanyNameList').select2();
    $('#VisitReasonList').select2();

    $('#VisitDate').val(visitedate);

    $('#feedBackDiv').hide();
    //$('#CompanyTypeNew').hide();

    var compselect = $("#CompanyName").val(); /*"@Html.Raw(Model.CompanyName)";*/
    $("#CompanyNameList").val(compselect).select2();
    $("#CompanyName").val(compselect);

    $("#CompanyNameList").change(function () {
        var comp = $("#CompanyNameList").val();
        var compId = comp.split("||");
        $.ajax({
            type: "POST",
            url: '/AfterSales/GetBranshs',
            data: { id: compId[1] },
            success: function (dat) {
                $("#BranchName").empty();
                if (dat.providerslist.length > 0) {
                    for (var i = 0; i < dat.providerslist.length; i++) {
                        $("#BranchName").append("<option value='" + dat.providerslist[i].COST_CODE + "'>" + dat.providerslist[i].COST_CODE + " || " + dat.providerslist[i].A_NAME + "</option>");
                    }
                }
                else {
                    $("#BranchName").append("<option value='" + $("#CompanyNameList option:selected").text() + "'>" + $("#CompanyNameList option:selected").text() + "</option>");
                }

            },
            error: function (err) {
                bootbox.alert("Error Data !");
            }
        });
    });

    $("#CompanyNameList").map(function () {
        var comp = $("#CompanyNameList").val();
        var compId = comp.split("||");
        $.ajax({
            type: "POST",
            url: '/AfterSales/GetBranshs',
            data: { id: compId[1] },
            success: function (dat) {
                $("#BranchName").empty();
                if (dat.providerslist.length > 0) {
                    for (var i = 0; i < dat.providerslist.length; i++) {
                        $("#BranchName").append("<option value='" + dat.providerslist[i].COST_CODE + "'>" + dat.providerslist[i].COST_CODE + " || " + dat.providerslist[i].A_NAME + "</option>");
                    }
                }
                else {
                    $("#BranchName").append("<option value='" + $("#CompanyNameList option:selected").text() + "'>" + $("#CompanyNameList option:selected").text() + "</option>");
                }

            },
            error: function (err) {
                bootbox.alert("Error Data !");
            }
        });
    });

    var visitReason = $("#VisitReason").val() /*"@Model.VisitReason"*/;
    $('#VisitReasonList').val(visitReason).select2();

    if ($("#HasFeedBack").is(":checked")) {
        $('#feedBackDiv').show();
    }
    else {
        $('#feedBackDiv').hide();
    }
});
function ShowFeedBackDiv() {
    if ($("#HasFeedBack").is(":checked")) {
        $('#feedBackDiv').show();
    }
    else {
        $('#feedBackDiv').hide();
    }
}


// Edit Request
function SubmitUpdate() {
    if ($('#CompanyNameList').val() == "" && $('#CompanyName').val() == "") {
        bootbox.alert("You shoud enter valid company name ..");
    }
    else if ($("#BranchName").val() == "" && $("#New").is(":checked")) {
        bootbox.alert("You shoud select Branch Name ..");
    }
    else if ($('#VisitReasonList').val() == "") {
        bootbox.alert("You shoud select Visit Reason ...");
    }
    else if ($("#PersonName").val() == "") {
        bootbox.alert("You shoud enter Person Name ...");
    }
    else if ($("#VisitDate").val() == "") {
        bootbox.alert("You shoud enter  Visit Date ...");
    }
    else if ($("#Region").val() == "") {
        bootbox.alert("You shoud enter Region ...");
    }
    else if ($("#Phone").val() == "" || $("#Phone").val().length != 11) {
        bootbox.alert("You shoud enter valid phone number");
    }
    else if ($("#HasFeedBack").is(":checked") && ($("#FeedBackText").val() == "" || isStopTimeClick == 0)) {
        bootbox.alert("You select  valid Feed Back Text and Meeting Time");
    }
    else if (isStopTimeClick == -2) {
        bootbox.alert("You shoud click ' End Meeting ' before save meeting data ");
    }
    else {
        var isNew = false;
        $("#submit").attr("disabled", "disabled");
        var compname = "";
        if ($('#isnew').val()/*"@Html.Raw(Model.IsNew)"*/ == "True") {
            compname = $('#CompanyName').val();
            isNew = true;
        }
        else {
            compname = $('#CompanyNameList').val();
        }
        //var oArea = document.getElementById('FeedBackText');
        var oArea = $('#FeedBackText').val();
        //var aNewlines = oArea.value.split("\n");
        var AllFeedBack = "";
        for (i = 0; i < oArea.length; i++) {
            if (i < oArea.length - 1) {
                if (oArea[i] != "")
                    AllFeedBack += oArea[i] + '-';
            }
            else {
                AllFeedBack += oArea[i];
            }
        }

        if (isStopTimeClick == 1) {
            dateOfStartMeeting = dateOfStartMeeting.getFullYear() + "-" + (dateOfStartMeeting.getMonth() + 1) + "-" + dateOfStartMeeting.getDate() +
                " " + dateOfStartMeeting.getHours() + ":" + dateOfStartMeeting.getMinutes() + ":" + dateOfStartMeeting.getSeconds();
            dateOfEndMeeting = dateOfEndMeeting.getFullYear() + "-" + (dateOfEndMeeting.getMonth() + 1) + "-" + dateOfEndMeeting.getDate() +
                " " + dateOfEndMeeting.getHours() + ":" + dateOfEndMeeting.getMinutes() + ":" + dateOfEndMeeting.getSeconds();
            duration = $("#hours").html() + ":" + $("#minutes").html() + ":" + $("#seconds").html();

        }

        var data = {
            Id: $('#Id').val()/* "@Html.Raw(Model.Id)"*/,
            CompName: compname,
            BranchName: $('#BranchName').val(),
            PersonName: $('#PersonName').val(),
            VisitReasonList: $('#VisitReasonList').val(),
            VisitDate: $('#VisitDate').val(),
            Region: $('#Region').val(),
            PhoneNumber: $("#Phone").val(),
            FeedBackText: AllFeedBack,
            Note: $('#Note').val(),
            IsNew: isNew,
            DateOfStartMeeting: dateOfStartMeeting,
            DateOfEndtMeeting: dateOfEndMeeting,
            Duration: duration,

        };
        $.ajax({
            type: "POST",
            url: '/AfterSales/EditVisit',
            data: (data),
            success: function (ret) {
                $("#submit").attr("disabled", false);
                if (ret.msg == "ok") {
                    $("#save").attr("disabled", "disabled");
                    alert(ret.result);
                    window.location.href = '/AfterSales/Index';

                }
                else {
                    alert(ret.result);
                    window.location.href = '/AfterSales/Index';
                }

            },
            error: function (err) {

                alert("Error DATA !");
                $("#submit").attr("disabled", false);
            }
        });
    }

}

function StartTime() {
    dateOfStartMeeting = new Date($.now());
    var sec = 0;
    function pad(val) { return val > 9 ? val : "0" + val; }
    interval = setInterval(function () {
        $("#seconds").html(pad(++sec % 60));
        $("#minutes").html(pad(parseInt(sec / 60, 10)));
        $("#hours").html(pad(parseInt((sec / 60) / 60, 10)));
    }, 1000);
    isStopTimeClick = -2;
}

function StopTime() {
    if (isStopTimeClick != -2) {
        bootbox.alert("You shoud click ' Start Meeting ' first ");
    }
    else {
        dateOfEndMeeting = new Date($.now());
        clearInterval(interval);
        isStopTimeClick = 1;
        bootbox.alert("You'r Location will be Shared with you'r Company Manager ");
    }
}