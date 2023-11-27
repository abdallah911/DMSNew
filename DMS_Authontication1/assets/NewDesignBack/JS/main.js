$(document).ready(function () {
  $("#searchAction").hide();
  $("#searchDataCount").hide();
  $("#searchData").hide();
});

$("#chronicSearchBtn").click(function () {
  $("#searchAction").show();
  $("#searchDataCount").show();
  $("#searchData").show();
  $("#searchAction").css("display", "flex");
  $("#searchDataCount").css("display", "flex");
});
$(function () {
  $(".date-input").datepicker();
});
