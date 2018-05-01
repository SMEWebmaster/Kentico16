$(function() {
  $( document ).ready(function() {
    var pstval="";
    var leftmenu = $('.article-page-content .grid_2 .menu ul li ');
    var innermenu = $('.article-page-content .grid_2 .menu ul li ul ');
    
    $(window).load(function(){
      var check=$.cookie('t');
      if($.cookie('t')) {
        $(leftmenu).eq( $.cookie('t') ).addClass( 'active');         
        var menuactive = $('.article-page-content .grid_2 .menu .leftnav .active ul');
        var checkElement = $('.article-page-content .grid_2 .menu .leftnav .active a ').next();
        
        if((checkElement.is('ul')) && (!checkElement.is(':visible'))) {
          $('.article-page-content .grid_2 .menu ul li ul:visible').slideUp('normal');
          checkElement.slideDown();
        }
        if((!checkElement.is(':visible'))) {
          $('.article-page-content .grid_2 .menu ul li ul:visible').slideUp('normal');
          checkElement.slideDown();
        } 
      }
    });
    
    $('#mobile-nav li a').click( function() {
      $.removeCookie("t");
    });
    
    $(leftmenu).click( function() {
      var index = $( leftmenu ).index( this );
      $.cookie('t',index);
      $(leftmenu).removeClass('active');
      $(this).addClass('active');        
      var menuactive = $('.article-page-content .grid_2 .menu .leftnav .active ul');
      var checkElement = $('.article-page-content .grid_2 .menu .leftnav .active a ').next();
      if((checkElement.is('ul')) && (checkElement.is(':visible'))) {
        menuactive.slideUp();  
        
      }
      if((checkElement.is('ul')) && (!checkElement.is(':visible'))) {
        $('.article-page-content .grid_2 .menu ul li ul:visible').slideUp('normal');
        checkElement.slideDown();
      }
      if((!checkElement.is(':visible'))) {
        $('.article-page-content .grid_2 .menu ul li ul:visible').slideUp('normal');
        checkElement.slideDown();
      }
    });
  });
});
