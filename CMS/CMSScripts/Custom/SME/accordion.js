$(function() {
  $( document ).ready(function() {
    var accordion = $('.awards-content h1 ');
    $('.awards-content h1:first ').addClass('active').parent('li').css("border","1px solid #e1e1e1");
    $('.awards-content div:first ').css("display","block");
    
    $('.awards-content .acc_btn ').click( function() {
      var checkElement = $('.awards-content .active').next();
      if((checkElement.is('div')) && (checkElement.is(':visible'))) {
        checkElement.slideUp('normal');
        $('.awards-content .active').removeClass('active').parent('li').css("border","none");
      }
    });
    
    
    
    $(accordion).click( function() {
      $(accordion).removeClass('active');
      $(this).addClass('active').parent('li').css("border","1px solid #e1e1e1");
      
      var checkElement = $('.awards-content .active').next();
      
      if((checkElement.is('div')) && (checkElement.is(':visible'))) {
        checkElement.slideUp();
        $(this).removeClass('active').parent('li').css("border","none");
      }
      
      if((checkElement.is('div')) && (!checkElement.is(':visible'))) {
        $('.awards-content div:visible').slideUp('normal');
        $('.awards-content .clear').css("display","block");
        checkElement.slideDown();
      }
      
      if((!checkElement.is(':visible'))) {
        $('.awards-content div:visible').slideUp('normal');
        checkElement.slideDown();
      }            
    });
  });
  });


  $(function() {
    $( document ).ready(function() {
      var accordion = $('.awards-content h3 ');
      $('.awards-content h3:first ').addClass('active').parent('li').css("border","1px solid #e1e1e1");
      $('.awards-content div:first ').css("display","block");
      
      $('.awards-content .acc_btn ').click( function() {
        var checkElement = $('.awards-content .active').next();
        if((checkElement.is('div')) && (checkElement.is(':visible'))) {
          checkElement.slideUp('normal');
          $('.awards-content .active').removeClass('active').parent('li').css("border","none");
        }
      });
      
      
    
    $(accordion).click( function() {
      $(accordion).removeClass('active');
      $(this).addClass('active').parent('li').css("border","1px solid #e1e1e1");
      
      var checkElement = $('.awards-content .active').next();
      
      if((checkElement.is('div')) && (checkElement.is(':visible'))) {
        checkElement.slideUp();
        $(this).removeClass('active').parent('li').css("border","none");
      }
      
      if((checkElement.is('div')) && (!checkElement.is(':visible'))) {
        $('.awards-content div:visible').slideUp('normal');
        $('.awards-content .clear').css("display","block");
        checkElement.slideDown();
      }
      
      if((!checkElement.is(':visible'))) {
        $('.awards-content div:visible').slideUp('normal');
        checkElement.slideDown();
      }            
    });
  });
  });
  
  