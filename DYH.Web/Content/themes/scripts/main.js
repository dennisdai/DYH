$(function () {
    $('.carte').slimScroll({
        height: '100%'
    });

    $(".carte li>a").on("click", function () {
        var value = $(this).attr("href");
        if (value === "#") {
            var parent = $(this).parent();

            parent.siblings("li").children(".carte-children").slideUp(300, function () {
                $(this).parent("li").removeClass("active");
            });
            parent.siblings("li").find("a>i").removeClass("icon-spread");

            parent.siblings("li").find("li").children(".carte-children").slideUp(300, function () {
                $(this).parent("li").removeClass("active");
            });
            parent.siblings("li").find("li>a>i").removeClass("icon-spread");

            parent.children(".carte-children").slideToggle(300, function () {
                $(this).parent("li").addClass("active");
            });

            var is_menu_children = parent.parent().is(".carte-children");
            var has_menu_children = $(this).next().is(".carte-children");
            var size = $(this).children("i.icon-spread").size();
            if (has_menu_children && is_menu_children && size == 0) {
                $(this).children("i").addClass("icon-spread");
            } else {
                $(this).children("i.icon-spread").removeClass("icon-spread");
            }
        }
    });

    $(".powertip").powerTip({
        placement: 's',
        smartPlacement: true
    });

    $(".table th input:checkbox").on("click", function () {
        var that = this;
        $(this).closest('table').find('tr > td:first-child input:checkbox').each(function () {
            this.checked = that.checked;
            if (that.checked) {
                $(this).closest('tr').addClass('active');
            } else {
                $(this).closest('tr').removeClass('active');
            }
        });
    });

    //µã»÷ÐÐ
    $(".table td").not(":first-child").not(":not(.last):last-child").on("click", function () {
        $(this).closest('tr').toggleClass('active');
        var checkbox = $(this).closest('tr').find("input:checkbox").get(0);
        if (checkbox.checked) {
            checkbox.checked = false;
        } else {
            checkbox.checked = true;
        }
    });


    $(".table input:checkbox").on("change", function () {
        if (this.checked) {
            $(this).closest('tr').addClass('active');
        } else {
            $(this).closest('tr').removeClass('active');
        }
    });

    $(".table tbody tr").on({
        mouseenter: function () {
            $(this).addClass("hover");
        }, mouseleave: function () {
            $(this).removeClass("hover");
        }
    });
});