$(document).ready(function () {
  $(window).resize(function () {
    if ($(window).width() >= 992) {
      $(".btns-box").insertAfter(".date-form");
    } else if ($(window).width() < 992 && $(window).width() > 576) {
      $(".btns-box").insertAfter(".main-form");
    } else if ($(window).width() < 576) {
      $(".btns-box").insertAfter(".sec-3");
    }
  });
  $(function () {
    $(".date-input").datepicker();
  });
});
console.log("908787");




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




function toggleDiv(divId) {
  var div = document.querySelector('.' + divId);
  if (div.style.display === 'none') {
    div.style.display = 'block';
  } else {
    div.style.display = 'none';
  }
}


var inputs = document.querySelectorAll(".chronic-form-control");
var spans = document.querySelectorAll(".if-has-value");

// Convert NodeList to an array for iteration
inputs = Array.from(inputs);
spans = Array.from(spans);

inputs.forEach((input) => {
  // Check if the input is disabled
  if (input.disabled) {
    spans.forEach((span) => {
      // Add the "has-value" class to the span
      span.classList.add("has-value");
    });
  }
});

const gotChecked = () => {
  const checked = document.querySelectorAll(".got-checked");
  const checkbtn = document.querySelectorAll(".approval-btn");

  checkbtn.forEach((btn) => {
    btn.addEventListener("click", () => {
      checked.forEach((check) => {
        check.innerHTML = "<i class='fas fa-check me-2'></i>";
      });
    });
  })
}



// POP UP !

// When the user clicks on <div>, open the popup
function openForm() {
  document.getElementById("pop-up-behind").style.display = "block";
  document.getElementById("myForm").style.display = "block";
}

function closeForm() {
  document.getElementById("pop-up-behind").style.display = "none";
  document.getElementById("myForm").style.display = "none";
}

function closePopDiv() {
  document.getElementById("pop-up-behind").style.display = "none";
  document.getElementById("myForm").style.display = "none";
}


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
