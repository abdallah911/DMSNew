////===============navebar========================
//$(document).ready(function () {
//    $(window).scroll(function () {
//        if ($(window).scrollTop() >= $(window).height()) {
//            $('.navbar').css('background', 'white'); // Change to your desired color
//        } else {
//            $('.navbar').css('background-color', 'transparent'); // Revert to original color
//        }
//    });
//});

// ===========Loading Screen==================
$(document).ready(function(){
  $('#loading').fadeOut(500)
 

})
$(document).ready(function () {
    $(".read-more-btn").click(function () {
        // Change the style of the sibling element 
        $(this).siblings(".read-more-text").removeClass("d-none");
        $(this).siblings(".read-less-btn").removeClass("d-none");
        $(this).addClass("d-none");
        console.log("it works")
    });
    $(".read-less-btn").click(function () {
        // Change the style of the sibling element 
        $(this).siblings(".read-more-text").addClass("d-none");
        $(this).siblings(".read-more-btn").removeClass("d-none");
        $(this).addClass("d-none");
        console.log("it works")
    });
});



// ======Change background link==========
$(".nav-item").on("click", function () {
  $(".nav-item").css("background-color", "");
  $(this).css("background-color", "rgba(255, 255, 255, 0.705)");
});

// ======Change backgroung nav==========

let scrollservice = $("#service").offset().top;
$(window).on("scroll", function () {
  let wScroll = $(window).scrollTop();
  if (wScroll > scrollservice ) {
    // $(".service-header").addClass("headermove");
    $("#btnUp").show(100);

    // $(".service-header").css("transform", "translateY(0%)");
    $(".navbar").css("background-color", "white");
      $(".navbar").css("height", "70px");
      $("a.nav-link").css("color", "#4e2e69");
    // $(".card").css("transform", "translateX(0%)");

    // $(".card").addClass("cardmove");
  } else {
    $(".navbar").css("background-color", "transparent");
    // $(".service-header").removeClass("headermove");
    // $(".service-header").css("transform", "translateY(-170%)");
    $(".navbar").css("height", "");
      $("#btnUp").hide(100);
      $("a.nav-link").css("color", "#fff");

    // $(".card").css("transform", "translateX(-380%)");

    // $(".card").removeClass("cardmove");
  }
});

$("#login").on("click", function () {
  location.href = "login2.html";
});

// =============owl carousel==========

$(".owl-carousel").owlCarousel({
  loop: true,
  margin: 10,
  nav: false,
  dots: false,
  autoplay: true,
  autoplayTimeout: 2000,
  navSpeed: 700,

  autoplayHoverPause: true,

  responsive: {
    0: {
      items: 1,
    },
    600: {
      items: 3,
    },
    1000: {
      items: 5,
    },
  },
});

$("#btnUp").on("click", function () {
  $("html,body").animate({ scrollTop: "0px" }, 1000);
});

document.addEventListener("DOMContentLoaded", function () {
  new WOW().init();
});

$("#hr").on("click", function () {
    location.href = "/Home/HR";

});
$("#commerce").on("click", function () {
    location.href = "/Home/Commerce";

});
$("#enterprise").on("click", function () {
    location.href = "/Home/EnterPrise";

});
$("#clinic").on("click", function () {
    location.href = "/Home/Clinic";

});
$("#healthCare").on("click", function () {
    location.href = "/Home/Health";

});
$("#self-admin").on("click", function () {
    location.href = "/Home/Admin";

});
$("#app").on("click", function () {
    location.href = "/Home/Application";

});
$(document).ready(function () {
$("#read-more-btn").on("click", function (event) {
    event.stopPropagation();
    alert("Button inside #app clicked!");
});
});

