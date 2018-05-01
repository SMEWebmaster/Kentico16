﻿define(['CMS/EventHub', 'jQuery'], function (EventHub, $) {
    'use strict';

    var history = [],
        $tabItems,

        getHistory = function (steps) {
            var h = history;
            if (h.length > steps) {
                return h[h.length - steps - 1];
            }

            return null;
        },
        back = function (steps) {
            var h = history;
            if (h.length > steps) {
                var p = h[h.length - steps - 1];
                redir(p.url, p.target, p.useIFrame, true);

                return p.url;
            }

            return null;
        },
        redir = function (url, target, useIFrame, noRefresh) {
            if (url != '') {
                if ((target == '_blank') || (target == '_new')) {
                    window.open(url);
                } else if (target == '_self') {
                    this.location.href = url;
                } else if (target != '') {
                    var frame;

                    if (useIFrame) {
                        frame = frames[target];
                    } else {
                        if (parent && parent.frames) {
                            frame = parent.frames[target];
                        }
                    }

                    try {
                        if (!frame || (frame.CheckChanges && !frame.CheckChanges())) {
                            return false;
                        }
                    } catch (ex) {
                        // When not a web page
                    }

                    var oldUrl = frame.location.href;

                    if (url.substr(0, 1) == '/') {
                        oldUrl = "/" + oldUrl.replace(/^(?:\/\/|[^\/]+)*\//, "");
                    }

                    if (!noRefresh || (oldUrl != url)) {
                        if (useIFrame && window.Loader) {
                            var fr = $('iframe[name="' + target + '"]');
                            if (window.Loader) {
                                window.Loader.show(fr);
                            }
                        }
                        frame.location.href = url;
                    }

                    if (typeof (frame.focus) == 'function') {
                        frame.focus();
                    }
                } else {
                    parent.location.href = url;
                }

                history.push({
                    url: url,
                    target: target,
                    useIFrame: useIFrame
                });

                return true;
            }

            return true;
        },
        selTab = function (i, clientId, p, text) {
            var tabItems = $('#' + clientId).find('li');
            tabItems.removeClass('active');

            var disabledGroupd = tabItems.find("a[data-toggle='collapse-disabled']");
            disabledGroupd.attr('data-toggle', 'collapse');

            var name = p + 'TabItem_' + i;

            var tab = $('#' + name);
            if (tab != null) {
                tab.addClass('active');

                var group = tab.parents('li').find("a[data-toggle='collapse']");
                group.attr('data-toggle', 'collapse-disabled');
            }

            var tabTitle = $('#' + name + ' > a > .tab-title');
            if (tabTitle != null) {
                tabTitle.text(text);
            }
        },
        tabs = function (initScript) {
            this.selTab = selTab;
            this.redir = redir;
            this.back = back;
            this.getHistory = getHistory;

            window.Tabs = this;

            if (initScript != null) {
                new Function(initScript)();
            }

            $tabItems = $('.nav-tabs-container-horizontal li');

            EventHub.subscribe("GlobalClick", function () {
                $tabItems.removeClass('open');
            });
        };

    return tabs;
});