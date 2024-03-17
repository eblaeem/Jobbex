(function ($) {

    'use strict';


    function sticky_header() {
        if (jQuery('.sticky-header').length) {
            var sticky = new Waypoint.Sticky({
                element: jQuery('.sticky-header')
            });
        }
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
            scroll_top(),

            mobile_nav(),
            mobile_side_drawer()

    });


    jQuery(window).on('load', function () {

        page_loader();
    });



    jQuery(window).on('scroll', function () {
        // > Window on scroll header color fill 
        color_fill_header();

    });











    var TxtType = function (el, toRotate, period) {
        this.toRotate = toRotate;
        this.el = el;
        this.loopNum = 0;
        this.period = parseInt(period, 10) || 2000;
        this.txt = '';
        this.tick();
        this.isDeleting = false;
    };

    TxtType.prototype.tick = function () {
        var i = this.loopNum % this.toRotate.length;
        var fullTxt = this.toRotate[i];

        if (this.isDeleting) {
            this.txt = fullTxt.substring(0, this.txt.length - 1);
        } else {
            this.txt = fullTxt.substring(0, this.txt.length + 1);
        }

        this.el.innerHTML = '<span class="wrap">' + this.txt + '</span>';

        var that = this;
        var delta = 200 - Math.random() * 100;

        if (this.isDeleting) { delta /= 2; }

        if (!this.isDeleting && this.txt === fullTxt) {
            delta = this.period;
            this.isDeleting = true;
        } else if (this.isDeleting && this.txt === '') {
            this.isDeleting = false;
            this.loopNum++;
            delta = 500;
        }

        setTimeout(function () {
            that.tick();
        }, delta);
    };

    window.onload = function () {
        var elements = document.getElementsByClassName('typewrite');
        for (var i = 0; i < elements.length; i++) {
            var toRotate = elements[i].getAttribute('data-type');
            var period = elements[i].getAttribute('data-period');
            if (toRotate) {
                new TxtType(elements[i], JSON.parse(toRotate), period);
            }
        }
        // INJECT CSS
        var css = document.createElement("style");
        css.type = "text/css";
        //css.innerHTML = ".typewrite > .wrap { border-right: 0.08em solid #fff}";
        document.body.appendChild(css);
    };

})(window.jQuery);
