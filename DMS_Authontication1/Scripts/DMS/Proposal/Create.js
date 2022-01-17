
    //var CompId = document.getElementById("CompId")
    //var ContractNo = document.getElementById("ContractNo")

    //CompId.onchange(changeCompId());



    //function changeCompId() {
        //    debugger
        //    $.get("/ProposalBasicDatas/GetStateList",
        //        { id: CompId.val() }, function (data) {
        //            debugger
        //            ContractNo.empty();
        //            $.each(data, function (index, row) {
        //                ContractNo.append("<option value='" + row.Value + "'>" + row.Value + "</option>")
        //            });
        //        });
        //}
        hideItems();
    showPreviousClient();
    function showNewProposal() {
        $("#NewProposal").show();
        $("#divCompName").show();


        if ($('#InsuredBeforeF').is(':checked')) {
        hidedivInsuredBefore();

        }
        else {
        showPreviousClient();

        }

    };
    function showPreviousClient() {
        $("#PreviousClient").show();
    };






    function showdivInsuredOtherComp() {
        $("#divOtherCompName").show();



    };



    function hidedivInsuredOtherComp() {
        $("#divOtherCompName").hide();
        $("#OtherCompName").val("");


    };



    function showdivInsuredBefore() {
        $("#divInsuredBefore").show();
        cleare();



    }; function hidedivInsuredBefore() {
        $("#divInsuredBefore").hide();
        $("#InsuredOtherCompF").prop("checked", true);
        $("#ExClientF").prop("checked", true);
        cleare();


    };






    function showdivExClient() {
        showPreviousClient();
        cleare();



    }; function hidedivExClient() {
        $("#PreviousClient").hide();
        debugger;

        cleare();



    };


    function cleare() {


        $("#CompId").val("");
        $("#ContractNo").val("");
    }







    function hideItems() {
        $("#divCompName").hide();
        $("#NewProposal").hide();
        $("#PreviousClient").hide();
        $("#divInsuredBefore").hide();
        cleare();
        $("#CompName").val("");
        $("#InsuredBeforeF").prop("checked", true);
        hidedivInsuredBefore();


    }


    $("#CompId").change(function () {//address
        $.get("/ProposalBasicDatas/GetStateList",
            { id: $("#CompId").val() }, function (data) {

                debugger;
                if ($('#TypeProposalR').is(':checked')) {
                    $("#CompName").val(data.Name)

                }

                $("#ContractNo").empty();
                $.each(data.StateListlist, function (index, row) {
                    $("#ContractNo").append("<option value='" + row.Value + "'>" + row.Value + "</option>")
                });
            });
    });

    $('#TypeProposalR').change(function () {

        hideItems();
        showPreviousClient();

    });
    $('#TypeProposalN').change(function () {

        hideItems();
        showNewProposal();

    });

    $('#InsuredOtherCompY').change(function () {


        showdivInsuredOtherComp();

    });
    $('#InsuredOtherCompF').change(function () {


        hidedivInsuredOtherComp();

    });

    function hideinsir() {
        $("#InsuredOtherCompF").prop("checked", true);
        $("#ExClientF").prop("checked", true);
        hidedivInsuredOtherComp();
        hidedivExClient();
    }

    $('#InsuredBeforeY').change(function () {
        hideinsir()
        showdivInsuredBefore();

    });
    $('#InsuredBeforeF').change(function () {
        hideinsir()

        hidedivInsuredBefore();

    });





    $('#InsuredOtherCompY').change(function () {


        showdivInsuredOtherComp();

    });
    $('#InsuredOtherCompF').change(function () {


        hidedivInsuredOtherComp();

    });


    $('#ExClientY').change(function () {


        showdivExClient();

    });
    $('#ExClientF').change(function () {


        hidedivExClient();

    });



