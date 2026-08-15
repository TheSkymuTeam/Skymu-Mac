/*
 * Copyright (c) 2009 Skype Technologies S.A. All rights reserved.
 */

YAHOO.util.Event.onDOMReady(function() {
    closeMICWindow = function() {
        var D = YAHOO.util.Dom;
        var E = YAHOO.util.Event;
        var closeLoadingWindow = D.get("closeLoadingWindow");
        if (closeLoadingWindow) {
            E.addListener(closeLoadingWindow, "click", function(ev) {
                E.preventDefault(ev);
                window.close();
            });
        }

    }();
});
