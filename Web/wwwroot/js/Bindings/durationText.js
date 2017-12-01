(function () {
    function millisecondsToText(ms) {
        if (!ms) {
            return "0 min";
        }
        var result = "", mins, h, s;
        if (!ms) {
            return result;
        }

        mins = Math.floor(ms / 60000);

        if (mins > 60) {
            // Over an hour, include hours and minutes
            h = Math.floor(mins / 60);
            result += h + " h ";
            mins = mins - h * 60;
            
            result += mins > 0 ? mins + " min" : "";
            
        } else {
            // Less than hour, include minutes and seconds.
            // To seconds.
            s = ms / 1000;
            result = Math.floor(s / 60) + " min " + Math.floor(s % 60) + " s";
        }

        return result;
    }    

    ko.bindingHandlers.durationText = {
        init: function (element, valueAccessor) {
            var interceptor = ko.computed(function() {
                    return millisecondsToText(ko.utils.unwrapObservable(valueAccessor()));
                });

            ko.applyBindingsToNode(element, { text: interceptor });
        }
    };
    
    ko.virtualElements.allowedBindings.durationText = true;
})();