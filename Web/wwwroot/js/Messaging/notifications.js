var Ksx = Ksx || {};
Ksx.notify = Ksx.notify || {};

(function(context) {
    "use strict";

    // All methods accept three arguments: text, title and options override. 
    // e.g. Ksx.notify.error(text, [title], [optionsOverride])
    ["success", "info", "error", "warning"].forEach(function(f) {
        context[f] = toastr[f];

        // Subscribe to pub/sub queue for this notification type
        Messaging.Queues[f].subscribe(context[f]);
    });

    // Using same options for all notifications. 
    toastr.options = {
        "debug": false,
        "positionClass": "toast-bottom-full-width",
        "onclick": null,
        "showDuration": 300,
        "hideDuration": 1000,
        "timeOut": 8000, // Use 0 to get sticky
        "extendedTimeOut": 1000
    };
}(Ksx.notify));