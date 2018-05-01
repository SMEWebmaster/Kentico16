  $(function() {
    var pull 		= $('.grid_8 .menu a');
    menu 		= $('.menu ul');
    menuHeight	= menu.height();
    
    $(pull).on('click', function(e) {
      e.preventDefault();
      menu.slideToggle();
    });
    
    $(window).resize(function(){
      var w = $(window).width();
      if(w>320 && menu.is(':hidden')) {
        menu.removeAttr('style');
      }
    });
  });
