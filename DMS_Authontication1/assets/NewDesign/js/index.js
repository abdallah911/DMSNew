// ======Change background link==========
$(".nav-item").on("click", function () {
  $(".nav-item").css("background-color", "");
  $(this).css("background-color", "rgba(255, 255, 255, 0.705)");
});

// ======Change backgroung nav==========

let scrollservice = $("#service").offset().top;
$(window).on("scroll", function () {
  let wScroll = $(window).scrollTop();
  if (wScroll > scrollservice - 230) {
    $(".service-header").addClass("headermove");
    $('#btnUp').show(100)

    $(".service-header").css("transform", "translateY(0%)");
    $(".navbar").css("background-color", "rgb(62, 115, 151)");
    $(".navbar").css("height", "55px");
    $(".card").css("transform", "translateX(0%)");

    $(".card").addClass("cardmove");
  } else {
    $(".navbar").css("background-color", "");
    $(".service-header").removeClass("headermove");
    $(".service-header").css("transform", "translateY(-170%)");
    $(".navbar").css("height", "");
    $('#btnUp').hide(100)
    $(".card").css("transform", "translateX(-380%)");

    $(".card").removeClass("cardmove");
  }
});

$("#login").on("click", function () {
  location.href = "login2.html";
});

// =============owl carousel==========

$('.owl-carousel').owlCarousel({
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
})


// =========location==============
// let locationScroll =  $('#location').offset().top;
// $(window).on('scroll',function(){
//   let wScroll = $(window).scrollTop();
// if(wScroll > locationScroll - 250){
//   $(".send-message").addClass("contactMove");
//   $(".send-message").css("transform", "translateY(0%)");

// }
// else{
//   $(".send-message").removeClass("contactMove");
//   $(".send-message").css("transform", "translateY(100%)");
// }
// })


$('#btnUp').on('click', function () {
  $('html,body').animate({ scrollTop: '0px' }, 1000)
})


