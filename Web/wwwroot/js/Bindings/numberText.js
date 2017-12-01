(function () {
    function format(n, decimals) {
        if (!n) {
            return "-";
        }

        // Do brute force format conversion here, use separate binding handler or 
        // extender if this same logic is needed in more places
        // http://knockoutjs.com/documentation/custom-bindings.html
        // http://stackoverflow.com/questions/13683086/knockout-js-and-comma-as-decimal-separator
        return (Math.round(100 * n) / 100).toFixed(decimals).replace(".", ",");
    }

    ko.bindingHandlers.numberText = {
        init: function (element, valueAccessor) {
            var interceptor = ko.computed(function() {
                    return format(ko.utils.unwrapObservable(valueAccessor()), 1);
                });

            ko.applyBindingsToNode(element, { text: interceptor });
        }
    };
    
    ko.virtualElements.allowedBindings.numberText = true;
})();