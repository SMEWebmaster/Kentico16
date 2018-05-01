$(document).ready(function() {   
  setNavigation();     
  function setNavigation() {
    var path = window.location.pathname;
    path = path.replace(/\/$/, "");
    path = decodeURIComponent(path);      
    jQuery("#mobile-nav li a").each(function () {
      var href = jQuery(this).attr('href');
      if (path.substring(0, href.length) === href) {
        jQuery(this).parent('li').first().addClass('active');  
        // $(event.currentTarget).find("a.active1").removeClass("active1"); 
        jQuery(".col_1 ul li").removeClass("active");
      }    
    });  
  }  
});