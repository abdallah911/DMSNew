$(document).ready(function () {
  $(".reports-select").select2()(["multiple: true"]);
  $("#reports").select2()(["multiple: true"]);
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