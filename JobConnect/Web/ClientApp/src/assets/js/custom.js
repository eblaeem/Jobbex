(function ($) {
  'use strict';
  function sticky_header() {
    if (jQuery('.sticky-header').length) {
      var sticky = new Waypoint.Sticky({
        element: jQuery('.sticky-header')
      });
    }
  }

  function sticky_sidebar() {
    $('.rightSidebar')
      .theiaStickySidebar({
        additionalMarginTop: 100
      });
  }

  function scroll_top() {
    jQuery("button.scroltop").on('click', function () {
      jQuery("html, body").animate({
        scrollTop: 0
      }, 1000);
      return false;
    });

    jQuery(window).on("scroll", function () {
      var scroll = jQuery(window).scrollTop();
      if (scroll > 900) {
        jQuery("button.scroltop").fadeIn(1000);
      } else {
        jQuery("button.scroltop").fadeOut(1000);
      }
    });
  }

  function mobile_nav() {
    jQuery(".sub-menu").parent('li').addClass('has-child');
    jQuery("<div class='fa fa-angle-left submenu-toogle'></div>").insertAfter(".has-child > a");

    jQuery('.has-child a+.submenu-toogle').on('click', function (ev) {

      jQuery(this).parent().siblings(".has-child ").children(".sub-menu").slideUp(500, function () {
        jQuery(this).parent().removeClass('nav-active');
      });

      jQuery(this).next(jQuery('.sub-menu')).slideToggle(500, function () {
        jQuery(this).parent().toggleClass('nav-active');
      });

      ev.stopPropagation();
    });
  }

  function mobile_side_drawer() {
    jQuery('#mobile-side-drawer').on('click', function () {
      jQuery('.mobile-sider-drawer-menu').toggleClass('active');
    });
  }

  function counter_section() {
    jQuery('.counter').counterUp({
      delay: 10,
      time: 3000
    });
  }
  function page_loader() {
    $('.loading-area').fadeOut(1000);
  }

  function color_fill_header() {
    var scroll = $(window).scrollTop();
    if (scroll >= 100) {
      $(".main-bar").addClass("color-fill");
    } else {
      (scroll = 100); $(".main-bar").removeClass("color-fill");
    }
  }

  jQuery(document).ready(function () {
      sticky_header(),
      sticky_sidebar(),
      scroll_top(),

      mobile_nav(),
      mobile_side_drawer()
  });

  jQuery(document).on('click', '.is-active', function () {
    jQuery('.mobile-sider-drawer-menu').toggleClass('active');
  });

  jQuery(window).on('load', function () {
      page_loader();
  });

  jQuery(window).on('scroll', function () {
    // > Window on scroll header color fill 
    color_fill_header();

  });

})(window.jQuery);
