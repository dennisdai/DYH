$(function(){
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
            }else{
            	$(this).children("i.icon-spread").removeClass("icon-spread");
            }
        }
    });

	
});