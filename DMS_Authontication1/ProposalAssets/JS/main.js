
//$(function () {
//    $(".date-input").datepicker({
//        dateFormat: "dd-mm-yy",
//    });
//});

$(document).ready(function () {


    $(function () {
        $(".date-input").datepicker({
            dateFormat: "dd/mm/yy",
        });
    });



    const toggleEnable = document.getElementById("toggleEnable");
    const disabled = document.getElementById("disabled");
    const brokeDisabled = document.querySelector(".brokeDisabled");
    const brokeEnabled = document.querySelectorAll(".brokeEnabled");
        //toggleEnable.addEventListener("change", (event) => {
        //    if (event.target.checked) {
        //        toggleEnable.style.marginTop = "10px";
        //        disabled.disabled = false;
        //        brokeDisabled.style.display = "none";
        //        brokeEnabled.forEach((el) => (el.style.display = "block"));
        //    } else {
        //        toggleEnable.style.marginTop = "3px";
        //        disabled.disabled = true;
        //        brokeDisabled.style.display = "block";
        //        brokeEnabled.forEach((el) => (el.style.display = "none"));
        //    }
        //});
    
            //var currentUrl = window.location.pathname.toLowerCase();
            //$('.nav-link').each(function () {
            //    var navUrl = $(this).find('a').attr('href').toLowerCase();
            //    if (currentUrl == navUrl) {
            //        $(this).addClass('active pharmacy-active');
            //    }
            //});


});


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