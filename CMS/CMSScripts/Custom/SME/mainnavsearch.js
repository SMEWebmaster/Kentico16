$(document).ready(function () {
    bindDoubleTap();

    $(window).on('resize orientationchange', function () {
        bindDoubleTap();
    });


    $.ajax({
        headers: {
            'Authorization': 'Basic YXBwLnNlY3Rpb25zY2hhcHRlcnM6NU1UalowNllweW16bGRIOXV2NXpzNXlsak9ZNUJLT1F1dVZyOWFWVDVHekZWdVZHZlg='
        },
        method: 'GET',
        dataType: 'json',
        url: 'https://sme.shooju.com/api/1/series?query=productclasscodestring=(SECTION,CHAPTER)&per_page=0&facets=svc_obj.state,svc_obj.countrycode&max_facet_values=100'

    }).done(function (resp) {
        var countries = resp.facets['svc_obj.countrycode'].terms;
        var states = resp.facets['svc_obj.state'].terms;

        countries.sort(function (a, b) {
            var country1 = a.term;
            var country2 = b.term;

            if (country1 < country2)
                return -1;
            else if (country1 > country2)
                return 1;
            else
                return 0;
        });

        var countrySelect = $('select.country-select');
        $.each(countries, function () {
            countrySelect.append($('<option />').val(this.term).text(this.term));
        });

        states.sort(function (a, b) {
            var state1 = a.term;
            var state2 = b.term;

            if (state1 < state2)
                return -1;
            else if (state1 > state2)
                return 1;
            else
                return 0;
        });

        var stateSelect = $('select.state-select');
        $.each(states, function () {
            stateSelect.append($('<option />').val(this.term).text(this.term));
        });

    });
});

function bindDoubleTap() {
    if (window.matchMedia("(max-width: 849px)").matches) {
        $('.header-navMain-nav > ul > li:has(div)').off();
    } else {
        $('.header-navMain-nav > ul > li:has(div)').doubleTapToGo();        
    }
}

/*
    By Osvaldas Valutis, www.osvaldas.info
    Available for use under the MIT License
*/
;
(function ($, window, document, undefined) {
    $.fn.doubleTapToGo = function (params) {
        if (!('ontouchstart' in window) &&
            !navigator.msMaxTouchPoints &&
            !navigator.userAgent.toLowerCase().match(/windows phone os 7/i)) return false;

        this.each(function () {
            var curItem = false;

            $(this).on('click', function (e) {
                var item = $(this);
                if (item[0] != curItem[0]) {
                    e.preventDefault();
                    curItem = item;
                }
            });

            $(document).on('click touchstart MSPointerDown', function (e) {
                var resetItem = true,
                    parents = $(e.target).parents();

                for (var i = 0; i < parents.length; i++)
                    if (parents[i] == curItem[0])
                        resetItem = false;

                if (resetItem)
                    curItem = false;
            });
        });
        return this;
    };
})(jQuery, window, document);
$(document).ready(function () {
    // Show on click
    $('.searchDrop span').on('click', function (e) {
        if ($(this).closest('.searchDrop').hasClass('active')) {
            $(this).closest('.searchDrop').removeClass('active');
            $(this).closest('.searchDrop').find('*').removeClass('active');
        } else {
            $(this).closest('.searchDrop').addClass('active');
        }
        $('.searchDrop').not($(this).closest('.searchDrop')).removeClass('active');
        $('.header-main-toggles--nav').removeClass('active').find('*').removeClass('active');
        $('html').removeClass('menu-open');
    });

    $('.searchDrop').mouseout(function () {
        var $this = $(this);
        setTimeout(function () {
            if (!($this.is(":hover"))) {
                $this.removeClass('active');
                $this.find('input').blur();
            }
        }, 250);
    });

    $(document).on('click touchend', function (e) {
        var $target = $(e.target);
        if (($target.parents('.searchDrop').length <= 0)) {
            $('.searchDrop').removeClass('active');
            $('.searchDrop').find('input').blur();
        }
    });

    $('#search-value').keyup(function (e) {
        if (e.keyCode == 13) {
            $('#search-button').click();
        }
    });

    $('#search-button').on('click touchend', function (e) {
        var searchVal = $('#search-value').val();
        if (searchVal.length) {
            document.location.href = '/search?mssearch=' + encodeURI(searchVal);
        }
    });
});
$(document).ready(function () {
    $('.header-main-toggles--nav span').click(function () {
        if ($('html').hasClass('menu-open')) {
            $('.header-main-toggles--nav').removeClass('active');
            $('.mobileDrawer').children('*').removeClass('active');
            $('html').removeClass('menu-open');
        }
        else {
            $('.header-main-toggles--nav').addClass('active');
            $('html').addClass('menu-open');
        }
        menuHeight();
    });
    $(document).on('click touchstart', '.mobileDrawer > ul > li > a', function (e) {
        e.stopPropagation();
    });
    $(document).on('click touchend', '.mobileDrawer > ul > li.mega', function (e) {
        $(this).closest('li.mega').addClass('active');
        $(this).closest('ul').addClass('active');
        $(this).siblings('li').removeClass('active');
        e.stopPropagation();
    });
    $(document).on('click touchend', '.mobileDrawer > ul > li > div', function (e) {
        $(this).parents('li').removeClass('active');
        $(this).parents('ul').removeClass('active');
        e.stopPropagation();
    });
    $(document).on('click touchend', '.mobileDrawer > ul > li > div ul', function (e) {
        e.stopPropagation();
    });
});

$(window).on('resize orientationchange', function () {
    menuHeight();
    if (window.matchMedia("(max-width: 849px)").matches) {
        $('.header-main-toggles--nav').removeClass('active');
        $('.mobileDrawer').children().removeClass('active');
        $('html').removeClass('menu-open');
    }
});

function menuHeight() {
    if (window.matchMedia("(max-width: 849px)").matches) {
        var windowHeight = $(window).outerHeight();
        var headerHeight = $('.header').outerHeight();
        var menuHeight = windowHeight - headerHeight;

        $('.mobileDrawer').css('max-height', menuHeight);
        $('.mobileDrawer > ul > li > div').css('max-height', menuHeight);
    }
    else {
        $('.mobileDrawer').css('max-height', '');
        $('.mobileDrawer > ul > li > div').css('max-height', '');
    }
}
$(document).ready(function () {
    $('.modalSearch-trigger, .modalSearch-trigger a').on('click touchend', function (e) {
        e.preventDefault();

        $('.modalSearch').addClass('active');
        $('html').addClass('modal-open');

        $('.header-main-toggles--nav').removeClass('active');

        setTimeout(function () {
            $('.mobileDrawer').removeClass('active');
            $('.mobileDrawer ul').removeClass('active');
            $('.mobileDrawer li').removeClass('active');
            $('html').removeClass('menu-open');
        }, 200);
    });

    $('.modalSearch-close').on('click touchend', function () {
        $('.modalSearch').removeClass('active');
        $('html').removeClass('modal-open');
    });

    $('.modalSearch .design-icon-search').on('click touchend', function (e) {
        e.preventDefault();
        // Build Search query
        var resultsUrl = $('.modalSearch .view-all').attr('href');

        var queryString = '';

        var dataType = $('.modalSearch-searchGroup--1 input[type="radio"]:checked').data('value');
        if (dataType.length) {
            if (dataType != 'both') {
                queryString += "type=" + dataType;
            }
        }


        var zip = $('.modalSearch-searchGroup--2 input.zipcode').val();
        if (zip.length) {
            if (queryString.length)
                queryString += "&";

            queryString += "zip=" + zip;
        }

        var range = $('.modalSearch-searchGroup--2 select.range-select option:selected').val();
        if (range.length) {
            if (queryString.length)
                queryString += "&";

            queryString += "range=" + range;
        }

        var state = $('.modalSearch-searchGroup--2 select.state-select option:selected').val();
        if (state.length) {
            if (queryString.length)
                queryString += "&";

            queryString += "state=" + state;
        }


        var country = $('.modalSearch-searchGroup--2 select.country-select option:selected').val();
        if (state.length || zip.length) {
            if (queryString.length)
                queryString += "&";

            queryString += "country=USA";
        }
        else if (country.length) {
            if (queryString.length)
                queryString += "&";
            
            queryString += "country=" + country;
        }

        document.location.href = resultsUrl + '?' + queryString;
    });

    $('.tabs > a').on('click touchend', function () {
        var selectedTab = $('.tab-content-wrapper .' + $(this).data('tab'));
        if (!$(selectedTab).hasClass('opened')) {
            $('.tab-content-wrapper .tab-content').removeClass('opened');
            $(selectedTab).addClass('opened');
        }
    });
});
$(document).ready(function () {
    $('.header-navMain-nav li.search > div').retach({
        destination: '.header-main-toggles--search',
        mediaQuery: 849
    });

    if (!$('.mobileDrawer').has('ul').length) {
        $('.mobileDrawer').append('<ul></ul>');
    }

    $('.header-main-nav > ul > li').not(':has(> div)').retach({
        destination: '.mobileDrawer > ul',
        mediaQuery: 849,
    });

    $('.header-main-nav > ul > li').has('> div').retach({
        destination: '.mobileDrawer > ul',
        mediaQuery: 849,
        movedClass: 'mega'
    });

    $('.header-util-nav > ul > li').retach({
        destination: '.mobileDrawer > ul',
        mediaQuery: 849,
    });

    $('.header-navMain-nav > ul > li:not(.search)').retach({
        destination: '.mobileDrawer > ul',
        mediaQuery: 849,
        movedClass: 'mega'
    });
});



(function ($) {
    $.fn.retach = function (opts) {
        var defaults = {
            destination: 'body',
            mediaQuery: 1023,
            movedClass: 'is-moved',
            prependAppend: 'append'
        };
        var options = $.extend({}, defaults, opts);

        var $items = this;
        var $destination = $(options.destination);
        var mediaQuery = options.mediaQuery;
        var movedClass = options.movedClass;
        var $prependAppend = options.prependAppend;

        var placeholderID = Math.floor((Math.random() * 10000) + 1) + Math.floor((Math.random() * 10000) + 1);
        var $placeholder = $('<i class="placeholder" data-placeholderID="' + placeholderID + '" />');

        function moveItems() {
            if ($('i[data-placeholderID="' + placeholderID + '"]').length <= 0) {
                $items.first().before($placeholder);
            }
            if (window.matchMedia("(max-width: " + mediaQuery + "px)").matches) {
                if ($prependAppend == 'append') {
                    $destination.append($items);
                } else {
                    $destination.prepend($items);
                }

                $items.addClass(movedClass);
            } else {
                $placeholder.after($items);
                $items.removeClass(movedClass);
            }
        }

        moveItems();
        $(window).resize(function () {
            moveItems();
        });
        return $items;
    };
}(jQuery));