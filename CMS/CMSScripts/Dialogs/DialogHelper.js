﻿/*
* Initialize global variables
*/
$cmsj(function () {
    InitResizers();
});

function InitGlobalVariables() {
    window.mainBlockElem = $cmsj('.DialogMainBlock');
    window.leftBlockElem = $cmsj('.DialogLeftBlock');
    window.separatorElem = $cmsj('.DialogTreeAreaSeparator');
    window.rightBlockElem = $cmsj('.DialogRightBlock');
    window.resizerElemV = $cmsj('.DialogResizerVLine');
    window.resizerElemH = $cmsj('.DialogResizerH');
    window.resizerArrowV = $cmsj('.DialogResizerArrowV');
    window.propElem = $cmsj('.DialogProperties');
    window.previewObj = $cmsj('.DialogPropertiesPreview');
    window.propertiesFullObj = $cmsj('.DialogPropertiesFullSize');
    window.webContentObj = $cmsj('.DialogWebContent');
    window.webPropertiesObj = $cmsj('.DialogWebProperties');
    window.menuElem = $cmsj('.cms-edit-menu');
    window.viewElem = $cmsj('.DialogViewContent');
    if ($cmsj('.DialogHeader').length > 0) {
        window.titleElem = $cmsj('.DialogHeader');
    }
    else {
        window.titleElem = $cmsj('.PageTitleHeader');
    }
    window.bodyObj = $cmsj('body');
    if ($cmsj('.DialogMainBlock').length > 0) {
        window.containerElem = $cmsj('.DialogMainBlock').offsetParent();
    }
    else {
        window.containerElem = window.bodyObj;
    }
    window.isSafari = bodyObj.hasClass('Safari');
    window.isGecko = bodyObj.hasClass('Gecko');
    window.isOpera = bodyObj.hasClass('Opera');
    window.isRTL = bodyObj.hasClass('RTL');
    window.resizerLineHeight = 8;
}

function InitResizers() {
    $cmsj('.DialogResizerArrowH').unbind('click');
    $cmsj('.DialogResizerArrowH').click(function() {
        InitGlobalVariables();
        ResetListItemWidth();
        var thisElem = $cmsj(this);
        var imgUrl = thisElem.css('background-image');
        imgUrl = imgUrl.replace('RTL/Design/', 'Design/');
        imgUrl = imgUrl.replace(/\/minimize\./i, '/##status##.');
        imgUrl = imgUrl.replace(/\/maximize\./i, '/##status##.');
        var minimizeLTR = imgUrl.replace('/##status##.', '/minimize.');
        var maximizeLTR = imgUrl.replace('/##status##.', '/maximize.');
        var minimizeRTL = minimizeLTR.replace('Design/', 'RTL/Design/');
        var maximizeRTL = maximizeLTR.replace('Design/', 'RTL/Design/');
        var parentWidth = '304px';
        var thisSideUp = '304px';
        var thisSideDown = '0px';
        if (isRTL) {
            $cmsj('.JqueryUITabs').css('position', 'static');
            if (thisElem.hasClass('ResizerDown')) {
                var width = mainBlockElem.innerWidth() - separatorElem.outerWidth() - leftBlockElem.outerWidth();
                rightBlockElem.width(width);
                resizerElemH.css('margin-right', '0');
                leftBlockElem.css('margin-right', '0');
                thisElem.css({
                    'right': thisSideUp,
                    'background-image': minimizeRTL
                });
                if ((!isSafari) && (!isOpera)) {
                    rightBlockElem.css('margin-right', '304px');
                }
                thisElem.removeClass('ResizerDown');
            }
            else {
                var width = mainBlockElem.innerWidth() - 10;
                rightBlockElem.width(width);
                leftBlockElem.css('margin-right', '-' + parentWidth);
                resizerElemH.css('margin-right', '-' + parentWidth);
                thisElem.css({
                    'right': thisSideDown,
                    'background-image': maximizeRTL
                });
                if ((!isSafari) && (!isOpera)) {
                    rightBlockElem.css('margin-right', '10px');
                }
                thisElem.addClass('ResizerDown');
            }
            setTimeout("$cmsj('.JqueryUITabs').css('position', 'relative');", 100);
        }
        else {
            if (thisElem.hasClass('ResizerDown')) {
                if ((!isSafari) && (!isGecko) && (!isOpera)) {
                    rightBlockElem.css('margin-left', parentWidth);
                }
                resizerElemH.css('margin-left', '0');
                leftBlockElem.css('margin-left', '0');
                thisElem.css({
                    'left': thisSideUp,
                    'background-image': minimizeLTR
                });
                thisElem.removeClass('ResizerDown');
            }
            else {
                if ((!isSafari) && (!isGecko) && (!isOpera)) {
                    rightBlockElem.css('margin-left', '0');
                }
                resizerElemH.css('margin-left', '-' + parentWidth);
                leftBlockElem.css('margin-left', '-' + parentWidth);
                thisElem.css({
                    'left': thisSideDown,
                    'background-image': maximizeLTR
                });
                thisElem.addClass('ResizerDown');
            }
        }
        SetListItemWidth();
        SetPreviewBox();
    });

    $cmsj('.DialogResizerArrowV').unbind('click');
    $cmsj('.DialogResizerArrowV').click(function() {
        $cmsj('.DialogResizerV').css('position', 'static');
        InitGlobalVariables();
        var thisElem = $cmsj(this);
        var imgUrl = thisElem.css('background-image');
        imgUrl = imgUrl.replace(/\/minimize\./i, '/##status##.');
        imgUrl = imgUrl.replace(/\/maximize\./i, '/##status##.');
        var maximize = imgUrl.replace('/##status##.', '/minimize.');
        var minimize = imgUrl.replace('/##status##.', '/maximize.');

        var padds = parseInt(viewElem.css('padding-top'), 10) + parseInt(viewElem.css('padding-bottom'), 10);
        if ((resizerArrowV.length > 0) && resizerArrowV.hasClass('ResizerDown')) {
            viewElem.css('height', mainBlockElem.innerHeight() - propElem.outerHeight() - menuElem.outerHeight() - resizerLineHeight - padds);
            propElem.css('display', 'block');
            thisElem.css('background-image', minimize);
            thisElem.removeClass('ResizerDown');
        }
        else {
            viewElem.css('height', mainBlockElem.innerHeight() - menuElem.outerHeight() - resizerLineHeight - padds);
            propElem.css('display', 'none');
            thisElem.css('background-image', maximize);
            thisElem.addClass('ResizerDown');
        }
        setTimeout("$cmsj('.DialogResizerV').css('position', 'relative');", 100);        
    });
}

function GetSelected(hdnId, hdnAnchors, hdnIds, editorId) {
    var selElem = GetSelectedItem(editorId);
    var selected = '';
    if (selElem) {
        for (var i in selElem) {
            selected += i + '|' + selElem[i] + '|';
        }
    }
    if (selected.length > 0) {
        selected = selected.substring(0, selected.length - 1);
        var hdnElement = document.getElementById(hdnId);
        if (hdnElement) {
            hdnElement.value = selected;
        }
    }
    if (window.GetAnchorNames) {
        var aAnchors = window.GetAnchorNames();
        if ((aAnchors != null) && (aAnchors.length > 0)) {
            var sAnchors = '';
            for (i = 0; i < aAnchors.length; i++) {
                sAnchors += encodeURIComponent(aAnchors[i]) + '|';
            }
            if (sAnchors.length > 0) {
                sAnchors = sAnchors.substring(0, sAnchors.length - 1);
                var eAnchors = document.getElementById(hdnAnchors);
                if (eAnchors) {
                    eAnchors.value = sAnchors;
                }
            }
        }
    }
    if (window.GetIds) {
        var aIds = window.GetIds();
        if ((aIds != null) && (aIds.length > 0)) {
            var sIds = '';
            for (i = 0; i < aIds.length; i++) {
                sIds += encodeURIComponent(aIds[i]) + '|';
            }
            if (sIds.length > 0) {
                sIds = sIds.substring(0, sIds.length - 1);
                var eIds = document.getElementById(hdnIds);
                if (eIds) {
                    eIds.value = sIds;
                }
            }
        }
    }
    DoHiddenPostback();
}

// Design methods
function SetListItemWidth() {
    // Dialog list name row IE6 
    var listItemsObj = $cmsj('.DialogListItem');
    if (listItemsObj.length > 0) {
        var listItemCell = listItemsObj.parent();
        listItemsObj.width(listItemCell.width());
    }
}

function ResetListItemWidth() {
    var listItemsObj = $cmsj('.DialogListItem');
    if (listItemsObj.length > 0) {
        listItemsObj.width(100);
    }
}

function SetPreviewBox() {
    if (previewObj.length > 0) {
        var previewTd = previewObj.parents('div, td');
        if (previewTd.is(':visible')) {
            var previewWidth = 0;
            if (previewTd.length > 0) {
                previewWidth = previewTd.width() - 20;
            }
            if (previewWidth > 80) {
                if (webContentObj.length > 0) {
                    previewObj.width(previewWidth);
                    previewObj.height(webPropertiesObj.height() - 70);
                }
                else {
                    // Width
                    previewObj.width(previewWidth);
                    // Height
                    if (propertiesFullObj.length > 0) {
                        previewObj.height(mainBlockElem.height() - 160);
                    }
                    else {
                        previewObj.height(380);
                    }
                }
                previewObj.css({ 'visibility': 'visible', 'display': 'block' });
            }
        }
        else {
            // Delayed display if jTabs not yet loaded
            setTimeout(SetPreviewBox, 100);
        }
    }
}

function InitializeDesign() {

    InitGlobalVariables();
    ResetListItemWidth();

    var menuHeight = menuElem.outerHeight();
    // Dialog view content height
    if (viewElem.length > 0) {
        var mainBlockHeight = containerElem.height();
        if (titleElem.parents(".UIHeader").length == 0) {
            // Decrease the content height only when not used in the Portal engine UI
            mainBlockHeight -= titleElem.outerHeight();
        }
        mainBlockElem.height(mainBlockHeight);

        var mainHeight = mainBlockHeight - menuHeight;
        var propHeight = propElem.outerHeight();
        var padds = parseInt(viewElem.css('padding-top'), 10) + parseInt(viewElem.css('padding-bottom'), 10);
        if (resizerArrowV.hasClass('ResizerDown')) {
            viewElem.height(mainHeight - resizerLineHeight - padds);
        }
        else
            if (viewElem.height() != (mainHeight - propHeight)) {
                var viewHeight = mainHeight - propHeight - resizerLineHeight - padds;
                // Ensure minimal height for small dialog
                if (viewHeight < 16) {
                    viewHeight = 16;
                }
                viewElem.height(viewHeight);
        }
    }
    // Dialog tree height
    var treeAreaObj = $cmsj('.DialogTreeArea');
    var siteBlockObj = $cmsj('.DialogSiteBlock');
    var mediaLibraryBlockObj = $cmsj('.DialogMediaLibraryBlock');
    if (mediaLibraryBlockObj.length == 0) {
        mediaLibraryBlockObj = $cmsj('.tree-actions-panel');
    }
    var mediaLibraryTreeBlockObj = $cmsj('.DialogMediaLibraryTreeArea');

    if (mainBlockElem.length > 0) {
        var treeHeight = 0;
        if (siteBlockObj.length > 0) {
            // Content tree
            treeHeight = mainBlockElem.innerHeight() - siteBlockObj.outerHeight();
        }
        else
            if (mediaLibraryBlockObj.length > 0) {
            // Media library tree
            treeHeight = mainBlockElem.innerHeight() - mediaLibraryBlockObj.outerHeight();
        }
        else {
            // Copy/Move tree
            treeHeight = mainBlockElem.innerHeight();
        }
        if (rightBlockElem.length > 0) {
            var rightWidth = 0;
            if ($cmsj('.DialogResizerArrowH').hasClass('ResizerDown')) {
                rightWidth = mainBlockElem.innerWidth() - 10;
            }
            else {
                rightWidth = mainBlockElem.innerWidth() - treeAreaObj.outerWidth() - separatorElem.outerWidth();
                // Fix IE6 double margin bug
                if ($cmsj.browser.msie && $cmsj.browser.version == '6.0') {
                    rightWidth = rightWidth - 5;
                }
            }
            if (rightBlockElem.width() != rightWidth) {
                rightBlockElem.width(rightWidth);
            }
        }
        if ((treeAreaObj.length > 0) && (treeAreaObj.height() != treeHeight)) {
            treeAreaObj.height(treeHeight);
        }
        if (mediaLibraryTreeBlockObj.length > 0) {
            var diferent = mediaLibraryTreeBlockObj.outerHeight() - mediaLibraryTreeBlockObj.height();
            if (mediaLibraryTreeBlockObj.height() != (treeHeight - diferent)) {
                mediaLibraryTreeBlockObj.height(treeHeight - diferent);
            }
        }
        else {
            var contentTreeObj = $cmsj('.ContentTree');
            var diferent = contentTreeObj.outerHeight() - contentTreeObj.height();
            treeHeight = treeHeight - diferent;
            if (contentTreeObj.height() != treeHeight) {
                contentTreeObj.height(treeHeight);
            }
        }
    }

    // Dialog preview box
    SetPreviewBox();

    // Dialog list name row IE6
    SetListItemWidth();

    // Ensure preview box size
    $cmsj('a[href$=tabImageGeneral]').click(SetPreviewBox);
    $cmsj('a[href$=tabFlashGeneral]').click(SetPreviewBox);
    $cmsj('a[href$=tabVideoGeneral]').click(SetPreviewBox);
}

// YouTube properties handling
function Loading(preview) {
    $cmsj('.youtube-loader').remove();
    var loader;
    if ($cmsj('.js-youtube-url').val() == '') {
        loader = $cmsj('<div class="youtube-loader"><span>' + preview + '</span></div>');
        $cmsj('div.js-youtube-preview-box').append(loader);
    }
    else {
        $cmsj('div.youtube-loader').hide();
    }
    window.youTubeLoaded = false;
}

function onYouTubePlayerReady(playerId) {
    if (!window.youTubeLoaded) {
        $cmsj('div.youtube-loader').hide();
        window.youTubeLoaded = true;
    }
}