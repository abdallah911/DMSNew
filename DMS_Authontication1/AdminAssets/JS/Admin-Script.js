
$(document).ready(function () {
  $(document).ready(function () {
      $("#example").DataTable();
      $(".select2").select2()(["multiple: true"]);
  });
  $(".sidebar-item").click(function () {
    $(".sidebar-item").removeClass("active");
    $(this).addClass("active");
    localStorage.setItem("activeLink", $(this).attr("id"));
  });
 // ====================Reports Scripts=======================================================

    $(document).ready(function () {
      
        $(".sidebar-item").click(function () {
            $(".sidebar-item").removeClass("active");
            $(this).addClass("active");
            localStorage.setItem("activeLink", $(this).attr("id"));
        });

        var activeLink = localStorage.getItem("activeLink");
        if (activeLink) {
            $("#" + activeLink).addClass("active");
        }
        $(function () {
            $(".date-input").datepicker();
        });

        $(".left-icon").click(function () {
            $(".page-icon").children("div").removeClass("active");
            $(this).addClass("active");
            $(".sec-1").show();
            $(".sec-2-parent").hide();
            $(".sec-1").css("width", "100%");
        });
        $(".middle-icon").click(function () {
            $(".page-icon").children("div").removeClass("active");
            $(this).addClass("active");
            $(".sec-2-parent").show();
            $(".sec-1").show();

            if ($(window).width() >= 992) {
                $(".sec-2-parent").css("width", "41.667%");
                $(".sec-1").css("width", "58.333%");
            }
            if ($(window).width() < 992) {
                $(".sec-2-parent").css("width", "100%");
                $(".sec-2").removeClass("hide-pseudo");
                $(".sec-2").css("padding", " 60px 90px");
                $(".sec-1").css("width", "100%");
            }
        });
        $(".right-icon").click(function () {
            $(".page-icon").children("div").removeClass("active");
            $(this).addClass("active");
            $(".sec-2-parent").show();
            $(".sec-1").css("display", "none");
            $(".sec-2-parent").css("width", "100%");

            if ($(window).width() < 992) {
                $(".sec-2").addClass("hide-pseudo");
                $(".sec-2").css("padding", "20px 0 0");
                $(".sec-2").addClass("padding-20");
            }
        });
    });
    var isScrolledIntoView = function (elem) {
        var $elem = $(elem);
        var $window = $(window);

        var docViewTop = $window.scrollTop();
        var docViewBottom = docViewTop + $window.height();

        var elemTop = $elem.offset().top;
        var elemBottom = elemTop + $elem.height();

        return elemBottom <= docViewBottom && elemTop >= docViewTop;
    };

    $(window).on("scroll", function () {
        $(".sideBar").toggle(!isScrolledIntoView(".footer"));
    });

    var offersDiv = document.querySelector(".offers-div");
    var operationsDiv = document.querySelector(".operations-div");
    var policyDiv = document.querySelector(".policy-div");
    function offersShow() {
        if (offersDiv.style.display === "block") {
            offersDiv.style.display = "none";
        } else {
            offersDiv.style.display = "block";
            policyDiv.style.display = "none";
            operationsDiv.style.display = "none";
        }
    }
    function policyShow() {
        if (policyDiv.style.display === "block") {
            policyDiv.style.display = "none";
        } else {
            policyDiv.style.display = "block";
            offersDiv.style.display = "none";
            operationsDiv.style.display = "none";
        }
    }
    function operationsShow() {
        if (operationsDiv.style.display === "block") {
            operationsDiv.style.display = "none";
        } else {
            operationsDiv.style.display = "block";
            offersDiv.style.display = "none";
            policyDiv.style.display = "none";
        }
    }
// ====================HR Scripts=======================================================
    $(document).ready(function () {
        $(function () {
            $(".date-input").datepicker();
        });
// ====================medical network=======================================================

        $('input[type="radio"]').change(function () {
            var inputValue = $(this).attr("value");
            var targetPop = $("." + inputValue + "-pop-up");
            $(".pop-up").not(targetPop).addClass("d-none");
            $(".pop-up").not(targetPop).removeClass("d-flex");
            $(targetPop).addClass("d-flex");
            $(targetPop).removeClass("d-none");
        });
//  ======================Profile================================================================
        $(".profile-btn-tog").click(function () {
            $(".btn-tog-parent").children(".profile-btn-tog").removeClass("collapsed");
            $(this).addClass("collapsed");
            $(".btn-tog-parent").children(".profile-collapse").removeClass("show");
            $(this).siblings(".profile-collapse").addClass("show");
        });

    });

// ====================Ahmed Scripts=======================================================


var offersDiv = document.querySelector(".offers-div");
var operationsDiv = document.querySelector(".operations-div");
var policyDiv = document.querySelector(".policy-div");
function offersShow() {
    if (offersDiv.style.display === "block") {
        offersDiv.style.display = "none";
    } else {
        offersDiv.style.display = "block";
        policyDiv.style.display = "none";
        operationsDiv.style.display = "none";
    }
}
function policyShow() {
    if (policyDiv.style.display === "block") {
        policyDiv.style.display = "none";
    } else {
        policyDiv.style.display = "block";
        offersDiv.style.display = "none";
        operationsDiv.style.display = "none";
    }
}
function operationsShow() {
    if (operationsDiv.style.display === "block") {
        operationsDiv.style.display = "none";
    } else {
        operationsDiv.style.display = "block";
        offersDiv.style.display = "none";
        policyDiv.style.display = "none";
    }
}


// ====================Sidebar Script=======================================================

var activeLink = localStorage.getItem("activeLink");
if (activeLink) {
    $("#" + activeLink).addClass("active");
}



    var isScrolledIntoView = function (elem) {
        var $elem = $(elem);
        var $window = $(window);

        var docViewTop = $window.scrollTop();
        var docViewBottom = docViewTop + $window.height();

        var elemTop = $elem.offset().top;
        var elemBottom = elemTop + $elem.height();




        return elemBottom <= docViewBottom && elemTop >= docViewTop;
    };

    $(window).on("scroll", function () {
        $(".sidebar-parent").toggle(!isScrolledIntoView(".footer"));
    });

// ====================Table Script=======================================================


        $(document).ready(function () {
            $("#example").DataTable();
        });


//====================DatePicker Script=======================================================
$(function () {
    $(".date-input").datepicker();


});