(function ($) {
    "use strict";

    $.subscribe("navigation/downloaddata", function (event, data) {
        // Use random url ending to circumvent browser caching
        var dlframe = document.createElement('iframe');
        
        dlframe.style.display = "none";
        dlframe.src = data.url + "?" + Math.random();
        
        document.body.appendChild(dlframe);
    });
})(jQuery);