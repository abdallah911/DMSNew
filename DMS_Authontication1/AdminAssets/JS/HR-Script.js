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
