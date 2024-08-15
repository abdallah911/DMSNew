
$(document).ready(function () {
  $(".slider").slick({
    // Slick settings go here
    infinite: true,
    slidesToShow: 1,
    slidesToScroll: 1,
    // Add more settings as needed
  });
});

// ====================cs-Cates=======================================================

$(document).ready(function () {
  $(".cs-btn").click(function () {
    $(".cs-btn").removeClass("active");
    $(this).addClass("active");
    localStorage.setItem("activeLink", $(this).attr("id"));
  });

  var activeLink = localStorage.getItem("activeLink");
  if (activeLink) {
    $("#" + activeLink).addClass("active");
  }
});
//--------------------Modal Script-------------------------------------------------------
$(document).ready(function () {
  // Function to load the cs form
  function loadCsMedicalNetForm() {
    $.ajax({
      url: "./CsMedicalNetwork.html",
      type: "GET",
      success: function (result) {
        $("#csFormContainer").html(result);
        $("#csModal").modal("show"); // Show the modal after loading the content
        $("#exampleModalLabel").html("شاشة الشبكة الطبية");
      },
      error: function (xhr, status, error) {
        // Handle validation errors
        if (xhr.status === 400) {
          $("#csFormContainer").html(xhr.responseText);
          $("#csModal").modal("show");
          $("#exampleModalLabel").html("شاشة الشبكة الطبية");
        } else {
          console.error("Error:", error);
        }
      },
    });
  }
  // Bind the function to the button click event
  $("#medicalNetworkFormBtn").on("click", function () {
    loadCsMedicalNetForm();
  });

  function loadCsPlaintsForm() {
    $.ajax({
      url: "./CsPlaints.html",
      type: "GET",
      success: function (result) {
        $("#csFormContainer").html(result);
        $("#csModal").modal("show"); // Show the modal after loading the content
        $("#exampleModalLabel").html("الشكاوى");
      },
      error: function (xhr, status, error) {
        // Handle validation errors
        if (xhr.status === 400) {
          $("#csFormContainer").html(xhr.responseText);
          $("#csModal").modal("show");
          $("#exampleModalLabel").html("الشكاوى");
        } else {
          console.error("Error:", error);
        }
      },
    });
  }

  // Bind the function to the button click event
  $("#CsPlaintsFormBtn").on("click", function () {
    loadCsPlaintsForm();
  });

  function loadCsCompanyContractForm() {
    $.ajax({
      url: "./CsCompanyContract.html",
      type: "GET",
      success: function (result) {
        $("#csFormContainer").html(result);
        $("#csModal").modal("show"); // Show the modal after loading the content
        $("#exampleModalLabel").html("عقد الشركة");
      },
      error: function (xhr, status, error) {
        // Handle validation errors
        if (xhr.status === 400) {
          $("#csFormContainer").html(xhr.responseText);
          $("#csModal").modal("show");
          $("#exampleModalLabel").html("عقد الشركة");
        } else {
          console.error("Error:", error);
        }
      },
    });
  } // Bind the function to the button click event
  $("#CsCompanyContractFormBtn").on("click", function () {
    loadCsCompanyContractForm();
  });

  function loadCsExtendForm() {
    $.ajax({
      url: "./CsExtend.html",
      type: "GET",
      success: function (result) {
        $("#csFormContainer").html(result);
        $("#csModal").modal("show"); // Show the modal after loading the content
        $("#exampleModalLabel").html("مد");
      },
      error: function (xhr, status, error) {
        // Handle validation errors
        if (xhr.status === 400) {
          $("#csFormContainer").html(xhr.responseText);
          $("#csModal").modal("show");
          $("#exampleModalLabel").html("مد");
        } else {
          console.error("Error:", error);
        }
      },
    });
  } // Bind the function to the button click event
  $("#CsExtendFormBtn").on("click", function () {
    loadCsExtendForm();
  });

});

// ====================Index Scripts=======================================================
var offersDiv = document.querySelector(".offers-div");
var contractsDiv = document.querySelector(".contracts-div");
var operationsDiv = document.querySelector(".operations-div");
var policyDiv = document.querySelector(".policy-div");
var csDiv = document.querySelector(".cs-div");
function offersShow() {
  if (offersDiv.style.display === "block") {
    offersDiv.style.display = "none";
  } else {
    offersDiv.style.display = "block";
    policyDiv.style.display = "none";
    contractsDiv.style.display = "none";
    operationsDiv.style.display = "none";
  }
}
function policyShow() {
  if (policyDiv.style.display === "block") {
    policyDiv.style.display = "none";
  } else {
    policyDiv.style.display = "block";
    offersDiv.style.display = "none";
    contractsDiv.style.display = "none";
    operationsDiv.style.display = "none";
  }
}
function operationsShow() {
  if (operationsDiv.style.display === "block") {
    operationsDiv.style.display = "none";
  } else {
    operationsDiv.style.display = "block";
    offersDiv.style.display = "none";
    contractsDiv.style.display = "none";
    policyDiv.style.display = "none";
  }
}
function contractsShow() {
  if (contractsDiv.style.display === "block") {
    contractsDiv.style.display = "none";
  } else {
    contractsDiv.style.display = "block";
    offersDiv.style.display = "none";
    operationsDiv.style.display = "none";
    policyDiv.style.display = "none";
  }
}
function csShow() {
  if (csDiv.style.display === "block") {
    csDiv.style.display = "none";
  } else {
    csDiv.style.display = "block";
  }
}
$(function () {
  $(".date-input").datepicker();
});
 
  $(document).ready(function () {
   
      $("#example").DataTable();
      $("#index").DataTable();
      
  });
$(document).ready(function () {
          $(".select2").select2();
      });
  
 

// ====================HR Scripts=======================================================
// ====================Reports Scripts=======================================================

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
// ====================medical network=======================================================

$('input[type="radio"]').change(function () {
  var inputValue = $(this).attr("value");
  var targetPop = $("." + inputValue + "-pop-up");
  $(".pop-up").not(targetPop).addClass("d-none");
  $(".pop-up").not(targetPop).removeClass("d-flex");
  $(targetPop).addClass("d-flex");
  $(targetPop).removeClass("d-none");
}); 

// ====================doctor  Scripts=======================================================
$(document).ready(function () {
  $("#simple-tab-1").removeClass("active");

  $("#simple-tab-1").click(function () {
    $("#simple-tab-0").removeClass("chicked");
  });
});

//==========================doctor daily===================================

$(document).ready(function () { 
    $('#expandVertical').on('click', function () {
        $(this).addClass('d-none');
        $('#expandHorisontal').removeClass('d-none');
        $('.ddlDiagnoises ul#select2-ddlDiagnoises-container')
            .css({
                'overflow-y': 'scroll',
                'overflow-x': 'hidden',
                'flex-wrap': 'wrap',
                ' height': '45px !important',
            }); 
    });
    $('#expandHorisontal').on('click', function () {
        $(this).addClass('d-none');
        $('#expandVertical').removeClass('d-none');
        $('.ddlDiagnoises ul#select2-ddlDiagnoises-container')
            .css({
                'overflow-y': 'hidden',
                'overflow-x': 'scroll',
                'flex-wrap': 'nowrap',
                ' height': 'unset',
            }); 
    }); 
});


//$(document).ready(function () {
//    let styleApplied = false;
//$('#diagnoisesExpand').on('click', function () {
//    if (!styleApplied) {
//        $('.ddlDiagnoises ul#select2-ddlDiagnoises-container')
//            .css({
//                'overflow-y': 'scroll',
//                'overflow': 'hidden',
//                'flex-wrap': 'wrap !important',
//                ' height': '40px'
//            });
//        styleApplied = true;
//    } else {
//        $('.ddlDiagnoises ul#select2-ddlDiagnoises-container')
//            .css({
//                'overflow-y': 'hidden',
//                'overflow': 'scroll',
//                'flex-wrap': 'no-wrap !important',
//                ' height': 'unset'
//            });
//        styleApplied = false;
//    }
//    });
//});

// ====================Sidebar Script=======================================================

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
});

//====================DatePicker Script=======================================================
$(document).ready(function () {
  $(".date-input").datepicker({
    dateFormat: "yy-mm-dd", // Set your desired date format
    changeMonth: true,
    changeYear: true,
  });
});
//====================DatePicker validation=======================================================

$(document).ready(function () {
  $("#startDate").datepicker({
    dateFormat: "yy-mm-dd",
    changeMonth: true,
    changeYear: true,
    onSelect: function (selectedDate) {
      $("#endDate").datepicker("option", "minDate", selectedDate);
    },
  });

  $("#endDate").datepicker({
    dateFormat: "yy-mm-dd",
    changeMonth: true,
    changeYear: true,
    onSelect: function (selectedDate) {
      $("#startDate").datepicker("option", "maxDate", selectedDate);
    },
  });
});
//====================select2 Script=======================================================
$(document).ready(function () {
  $(".span.select2.select2-container.select2-container--default").focus(
    function () {
      // Change the style of the sibling element
      $(".span-label").removeClass("select2-label");
      $(this).siblings(".span-label").addClass("select2-label");
    }
  );

  // If you also want to reset the style when the input loses focus
  $(".sibling-input").blur(function () {
    // Reset the style of the sibling element
    $(this).siblings(".element-to-style").css({
      // Reset the styles to their default values
      backgroundColor: "", // or whatever the default background color is
      // Add more styles as needed
    });
  });
});

//====================slider Script=======================================================
//====================Contracts Script=======================================================
document.addEventListener("DOMContentLoaded", function () {
  // Get the input element
  var imageInput = document.getElementById("imageInput");

  // Listen for the change event on the input file
  imageInput.addEventListener("change", function (event) {
    // Get the selected file
    var selectedFile = event.target.files[0];

    // Check if a file is selected
    if (selectedFile) {
      // Create a new FileReader
      var reader = new FileReader();

      // Set up the FileReader to load the image
      reader.onload = function (e) {
        // Create a new image element
        var imageElement = document.createElement("img");
        imageElement.classList.add(
          "w-100",
          "h-100",
          "rounded",
          "shadow-sm",
          "border-img"
        );
        // Set the source of the image to the loaded data URL
        imageElement.src = e.target.result;

        // Create a new container div
        var imageContainer = document.getElementById("imageContainer");
        var newDiv = document.createElement("div");
        newDiv.classList.add("box-img", "position-relative");

        // Append the image to the new div
        newDiv.appendChild(imageElement);

        // Create a delete button
        var deleteButton = document.createElement("button");
        deleteButton.classList.add("delete-button");
        deleteButton.innerHTML = `<i class="fa-solid fa-trash"></i>`;
        deleteButton.addEventListener("click", function () {
          // Remove the parent div when the delete button is clicked
          imageContainer.removeChild(newDiv);
        });

        // Append the delete button to the new div
        newDiv.appendChild(deleteButton);

        // Append the new div to the image container
        imageContainer.appendChild(newDiv);
      };

      // Read the selected file as a data URL
      reader.readAsDataURL(selectedFile);
    }
  });
});

//====================Management Script=======================================================

$(document).ready(function () {
  // Function to load the create form
  function loadCreateForm() {
    $.ajax({
      url: "./Management_Medicines-Create.html",
      type: "GET",
      success: function (result) {
        $("#createFormContainer").html(result);
        $("#createModal").modal("show"); // Show the modal after loading the content
      },
      error: function (xhr, status, error) {
        // Handle validation errors
        if (xhr.status === 400) {
          $("#createFormContainer").html(xhr.responseText);
          $("#createModal").modal("show");
        } else {
          console.error("Error:", error);
        }
      },
    });
  }

  // Bind the function to the button click event
  $("#openCreateFormBtn").on("click", function () {
    loadCreateForm();
  });
});
