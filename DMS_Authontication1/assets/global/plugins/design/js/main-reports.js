$(document).ready(function () {
  $(document).ready(function () {
      $("#").select2()(["multiple: true"]);
  });
    $(document).on('select2:open', () => {
        document.querySelector('.select2-search__field').focus();
    });

    
    //$('#CardFromApproval').select2({       
    //    placeholder: 'Search for a card',
    //    minimumInputLength: 5,
    //    ajax: {
    //        url: '/EmployeeRequest/GetActiveEmployess/',
    //        delay: 250,
    //        dataType: 'json',
    //        data: function (params) {
    //            var query = {
    //                search: params.term,
    //                page: params.page || 1
    //            }
    //            return query;
    //        },
    //        processResults: function (data, params) {
    //            params.page = params.page || 1;
    //            var obj = {};
    //            data.push(obj)
    //            return {
    //                results: data,//.results,
    //                pagination: {
    //                    more: (params.page * 10) < data.count_filtered
    //                }
    //            };
    //        }
    //        // Additional AJAX parameters go here; see the end of this chapter for the full code of this example

    //    }
    //});


    //$('#CompFromApproval1').select2();


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

  //$(".left-icon").click(function () {
  //  $(".page-icon").children("div").removeClass("active");
  //  $(this).addClass("active");
  //  $(".sec-1").show();
  //  $(".sec-2-parent").hide();
  //  $(".sec-1").css("width", "100%");
  //});
  //$(".middle-icon").click(function () {
  //  $(".page-icon").children("div").removeClass("active");
  //  $(this).addClass("active");
  //  $(".sec-2-parent").show();
  //  $(".sec-1").show();

  //  if ($(window).width() >= 992) {
  //    $(".sec-2-parent").css("width", "41.667%");
  //    $(".sec-1").css("width", "58.333%");
  //  }
  //  if ($(window).width() < 992) {
  //    $(".sec-2-parent").css("width", "100%");
  //    $(".sec-2").removeClass("hide-pseudo");
  //    $(".sec-2").css("padding", " 60px 90px");
  //    $(".sec-1").css("width", "100%");
  //  }
  //});
  //$(".right-icon").click(function () {
  //  $(".page-icon").children("div").removeClass("active");
  //  $(this).addClass("active");
  //  $(".sec-2-parent").show();
  //  $(".sec-1").css("display", "none");
  //  $(".sec-2-parent").css("width", "100%");

  //  if ($(window).width() < 992) {
  //    $(".sec-2").addClass("hide-pseudo");
  //    $(".sec-2").css("padding", "20px 0 0");
  //    $(".sec-2").addClass("padding-20");
  //  }
  //});
});
//var isScrolledIntoView = function (elem) {
//  var $elem = $(elem);
//  var $window = $(window);

//  var docViewTop = $window.scrollTop();
//  var docViewBottom = docViewTop + $window.height();

//  var elemTop = $elem.offset().top;
//  var elemBottom = elemTop + $elem.height();

//  return elemBottom <= docViewBottom && elemTop >= docViewTop;
//};

$(window).on("scroll", function () {
  $(".sideBar").toggle(!isScrolledIntoView(".footer"));
});
